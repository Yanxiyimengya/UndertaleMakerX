extends PanelContainer

const FILTER_MODE_NEAREST: int = 0
const FILTER_MODE_LINEAR: int = 1
const SHADER_EXTENSIONS: Dictionary = {
	"gdshader": true,
	"gdshade": true,
	"shader": true,
}

@onready var preview_texture_rect: TextureRect = %PreviewTextureRect
@onready var filter_mode_option_button: OptionButton = %FilterModeOptionButton
@onready var shader_path_line_edit: LineEdit = %ShaderPathLineEdit
@onready var info_label: Label = %InfoLabel
@onready var clear_shader_button: Button = %ClearShaderButton;

var _current_filter_mode: int = FILTER_MODE_NEAREST
var _shader_source_path: String = ""
var _shader_material: ShaderMaterial = null
var _is_syncing_shader_line_edit_text: bool = false
var _image_info_text: String = ""
var _shader_status_text: String = ""


func _ready() -> void:
	_image_info_text = tr("Select an image file")
	_setup_filter_options()
	_setup_shader_path_line_edit()
	if not filter_mode_option_button.item_selected.is_connected(_on_filter_mode_selected):
		filter_mode_option_button.item_selected.connect(_on_filter_mode_selected)
	if not clear_shader_button.pressed.is_connected(_on_clear_button_pressed) : 
		clear_shader_button.pressed.connect(_on_clear_button_pressed);
	_apply_filter_mode()
	_apply_shader_material()
	_refresh_info_label()


func _notification(what: int) -> void:
	if what == NOTIFICATION_TRANSLATION_CHANGED:
		if not is_node_ready():
			return
		_setup_filter_options()
		_refresh_info_label()


func open_file(path: String) -> bool:
	_clear_shader_preview()
	var image: Image = Image.load_from_file(path)
	if image == null or image.is_empty():
		preview_texture_rect.texture = null
		_image_info_text = tr("Failed to load image")
		_shader_status_text = ""
		_refresh_info_label()
		return false

	var texture: ImageTexture = ImageTexture.create_from_image(image)
	preview_texture_rect.texture = texture
	_apply_filter_mode()
	_apply_shader_material()
	_image_info_text = tr("%s (%d x %d)") % [path.get_file(), image.get_width(), image.get_height()]
	_refresh_info_label()
	return true


func _setup_filter_options() -> void:
	if filter_mode_option_button == null or not is_instance_valid(filter_mode_option_button):
		return
	var selected_index: int = _current_filter_mode
	filter_mode_option_button.clear()
	filter_mode_option_button.add_item(tr("Nearest"))
	filter_mode_option_button.add_item(tr("Linear"))
	filter_mode_option_button.select(clampi(selected_index, 0, filter_mode_option_button.item_count - 1))


func _setup_shader_path_line_edit() -> void:
	if shader_path_line_edit == null or not is_instance_valid(shader_path_line_edit):
		return
	if not shader_path_line_edit.text_changed.is_connected(_on_shader_path_line_edit_text_changed):
		shader_path_line_edit.text_changed.connect(_on_shader_path_line_edit_text_changed)
	if shader_path_line_edit.has_signal("shader_file_dropped"):
		var dropped_callable: Callable = Callable(self, "_on_shader_file_dropped")
		if not shader_path_line_edit.is_connected("shader_file_dropped", dropped_callable):
			shader_path_line_edit.connect("shader_file_dropped", dropped_callable)


func _on_filter_mode_selected(index: int) -> void:
	_current_filter_mode = index
	_apply_filter_mode()

func _on_clear_button_pressed() -> void: 
	if shader_path_line_edit == null or not is_instance_valid(shader_path_line_edit):
		return
	shader_path_line_edit.clear();

func _apply_filter_mode() -> void:
	if _current_filter_mode == FILTER_MODE_LINEAR:
		preview_texture_rect.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR
	else:
		preview_texture_rect.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST


func _on_shader_file_dropped(path: String) -> void:
	_apply_shader_from_path(path)


func _on_shader_path_line_edit_text_changed(new_text: String) -> void:
	if _is_syncing_shader_line_edit_text:
		return
	if new_text.strip_edges().is_empty():
		_clear_shader_preview()
	else:
		_sync_shader_path_line_edit_text(_to_project_relative_path(_shader_source_path))


func _apply_shader_from_path(path: String) -> void:
	var normalized_path: String = _normalize_path(path)
	if normalized_path.is_empty():
		_set_shader_status_text(tr("Shader path is empty"))
		return
	if not FileAccess.file_exists(normalized_path):
		_set_shader_status_text(tr("Shader file not found: %s") % path.get_file())
		return
	if not _is_supported_shader_file(normalized_path):
		_set_shader_status_text(tr("Unsupported shader file: %s") % normalized_path.get_extension())
		return

	_shader_source_path = normalized_path
	_sync_shader_path_line_edit_text(_to_project_relative_path(normalized_path))
	_apply_shader_material()


func _clear_shader_preview() -> void:
	_shader_source_path = ""
	_shader_material = null
	preview_texture_rect.material = null
	_sync_shader_path_line_edit_text("")
	_set_shader_status_text("")


func _apply_shader_material() -> void:
	if _shader_source_path.is_empty():
		preview_texture_rect.material = null
		_set_shader_status_text("")
		return

	var shader: Shader = _load_shader_resource(_shader_source_path)
	if shader == null:
		preview_texture_rect.material = null
		_shader_material = null
		_set_shader_status_text(tr("Shader load failed: %s") % _shader_source_path.get_file())
		return

	if _shader_material == null:
		_shader_material = ShaderMaterial.new()
	_shader_material.shader = shader
	preview_texture_rect.material = _shader_material
	_set_shader_status_text(tr("Shader: %s") % _shader_source_path.get_file())


func _load_shader_resource(path: String) -> Shader:
	var shader_code: String = _read_text_file(path)
	if shader_code.is_empty():
		return null
	if shader_code.findn("shader_type") == -1:
		push_warning("Shader source missing 'shader_type': %s" % path)
		return null

	var shader: Shader = Shader.new()
	shader.code = shader_code
	return shader


func _read_text_file(path: String) -> String:
	var file: FileAccess = FileAccess.open(path, FileAccess.READ)
	if file == null:
		push_warning("Failed to open shader file for read: %s" % path)
		return ""
	return file.get_as_text()


func _is_supported_shader_file(path: String) -> bool:
	var extension: String = path.get_extension().to_lower()
	return SHADER_EXTENSIONS.has(extension)


func _sync_shader_path_line_edit_text(text: String) -> void:
	_is_syncing_shader_line_edit_text = true
	shader_path_line_edit.text = text
	_is_syncing_shader_line_edit_text = false


func _set_shader_status_text(text: String) -> void:
	_shader_status_text = text
	_refresh_info_label()


func _refresh_info_label() -> void:
	if info_label == null or not is_instance_valid(info_label):
		return
	if _shader_status_text.is_empty():
		info_label.text = _image_info_text
	else:
		info_label.text = "%s | %s" % [_image_info_text, _shader_status_text]


func _to_project_relative_path(path: String) -> String:
	var normalized_path: String = _normalize_path(path)
	var project_root: String = _normalize_path(EditorProjectManager.get_opened_project_path())
	if project_root.is_empty():
		return normalized_path
	if normalized_path == project_root:
		return "."
	if not project_root.ends_with("/"):
		project_root += "/"
	if normalized_path.begins_with(project_root):
		return normalized_path.trim_prefix(project_root)
	return normalized_path


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()
