extends PanelContainer

const DEFAULT_SAMPLE_TEXT: String = "The quick brown fox jumps over the lazy dog 0123456789"
const PREVIEW_UPDATE_DELAY_SEC: float = 0.25

enum CaseMode {
	ORIGINAL,
	UPPER,
	LOWER,
}

@onready var info_label: Label = %InfoLabel
@onready var sample_input: TextEdit = %SampleInput
@onready var case_option_button: OptionButton = %CaseOptionButton
@onready var preview_label: Label = %PreviewLabel
@onready var preview_update_timer: Timer = %PreviewUpdateTimer

var _case_mode: int = CaseMode.ORIGINAL


func _ready() -> void:
	_setup_case_option_button()
	preview_update_timer.wait_time = PREVIEW_UPDATE_DELAY_SEC
	preview_update_timer.one_shot = true
	if not preview_update_timer.timeout.is_connected(_on_preview_update_timer_timeout):
		preview_update_timer.timeout.connect(_on_preview_update_timer_timeout)
	if sample_input.text.is_empty():
		sample_input.text = DEFAULT_SAMPLE_TEXT
	if not sample_input.text_changed.is_connected(_on_sample_input_text_changed):
		sample_input.text_changed.connect(_on_sample_input_text_changed)
	if not case_option_button.item_selected.is_connected(_on_case_option_button_item_selected):
		case_option_button.item_selected.connect(_on_case_option_button_item_selected)
	_update_preview_text()


func open_file(path: String) -> bool:
	var loaded: Resource = GlobalEditorResourceLoader.load_resource(path)
	var font: Font = loaded as Font
	if font == null:
		preview_label.text = "Failed to load font"
		info_label.text = path.get_file()
		return false

	preview_label.add_theme_font_override("font", font)
	preview_label.add_theme_font_size_override("font_size", 28)
	_update_preview_text()
	info_label.text = path.get_file()
	return true


func _setup_case_option_button() -> void:
	case_option_button.clear()
	case_option_button.add_item("Original")
	case_option_button.add_item("UPPER")
	case_option_button.add_item("lower")
	case_option_button.select(_case_mode)


func _on_sample_input_text_changed() -> void:
	_restart_preview_update_timer()


func _on_case_option_button_item_selected(index: int) -> void:
	_case_mode = index
	_update_preview_text()


func _update_preview_text() -> void:
	var base_text: String = sample_input.text
	if base_text.is_empty():
		base_text = DEFAULT_SAMPLE_TEXT
	preview_label.text = _apply_case_mode(base_text)


func _apply_case_mode(text: String) -> String:
	match _case_mode:
		CaseMode.UPPER:
			return text.to_upper()
		CaseMode.LOWER:
			return text.to_lower()
		_:
			return text


func _restart_preview_update_timer() -> void:
	preview_update_timer.start(PREVIEW_UPDATE_DELAY_SEC)


func _on_preview_update_timer_timeout() -> void:
	_update_preview_text()
