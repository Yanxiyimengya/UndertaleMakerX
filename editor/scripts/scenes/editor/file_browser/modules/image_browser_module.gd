extends PanelContainer

const FILTER_MODE_NEAREST: int = 0
const FILTER_MODE_LINEAR: int = 1
const PLAY_ICON: Texture2D = preload("res://assets/icons/play/play.svg")
const PAUSE_ICON: Texture2D = preload("res://assets/icons/paused/paused.svg")
const DEFAULT_SEQUENCE_FPS: float = 12.0
const SHADER_EXTENSIONS: Dictionary = {
	"gdshader": true,
	"gdshade": true,
	"shader": true,
}
const IMAGE_EXTENSIONS: Dictionary = {
	"png": true,
	"jpg": true,
	"jpeg": true,
	"webp": true,
	"svg": true,
	"bmp": true,
	"tga": true,
}

@onready var preview_texture_rect: TextureRect = %PreviewTextureRect
@onready var filter_mode_option_button: OptionButton = %FilterModeOptionButton
@onready var shader_path_line_edit: LineEdit = %ShaderPathLineEdit
@onready var info_label: Label = %InfoLabel
@onready var clear_shader_button: Button = %ClearShaderButton
@onready var sequence_controls_container: HBoxContainer = %SequenceControlsContainer
@onready var sequence_play_pause_button: TextureButton = %SequencePlayPauseButton
@onready var sequence_progress_slider: HSlider = %SequenceProgressSlider
@onready var sequence_speed_spin_box: SpinBox = %SequenceSpeedSpinBox
@onready var sequence_frame_label: Label = %SequenceFrameLabel
@onready var sequence_playback_timer: Timer = %SequencePlaybackTimer

var _current_filter_mode: int = FILTER_MODE_NEAREST
var _shader_source_path: String = ""
var _shader_material: ShaderMaterial = null
var _is_syncing_shader_line_edit_text: bool = false
var _image_info_text: String = ""
var _shader_status_text: String = ""

var _sequence_paths: Array[String] = []
var _sequence_textures: Array[Texture2D] = []
var _sequence_index: int = 0
var _is_sequence_slider_syncing: bool = false
var _is_sequence_playing: bool = false
var _is_sequence_speed_syncing: bool = false
var _sequence_speed: float = 1.0


func _ready() -> void:
	_image_info_text = tr("Select an image file")
	_setup_filter_options()
	_setup_shader_path_line_edit()
	_setup_sequence_controls()
	if not filter_mode_option_button.item_selected.is_connected(_on_filter_mode_selected):
		filter_mode_option_button.item_selected.connect(_on_filter_mode_selected)
	if not clear_shader_button.pressed.is_connected(_on_clear_button_pressed):
		clear_shader_button.pressed.connect(_on_clear_button_pressed)
	_apply_filter_mode()
	_apply_shader_material()
	_refresh_info_label()


func _notification(what: int) -> void:
	if what == NOTIFICATION_TRANSLATION_CHANGED:
		if not is_node_ready():
			return
		_setup_filter_options()
		_refresh_info_label()
	elif what == NOTIFICATION_VISIBILITY_CHANGED:
		if not is_node_ready():
			return
		if not is_visible_in_tree():
			_stop_sequence_playback()
			_clear_shader_preview()


func open_file(path: String) -> bool:
	_clear_sequence_preview()
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


func open_files(paths: Array[String]) -> bool:
	var normalized_paths: Array[String] = _normalize_image_paths(paths)
	if normalized_paths.is_empty():
		return false
	if normalized_paths.size() == 1:
		return open_file(normalized_paths[0])

	_clear_sequence_preview()
	_clear_shader_preview()

	var loaded_paths: Array[String] = []
	var loaded_textures: Array[Texture2D] = []
	for file_path: String in normalized_paths:
		var image: Image = Image.load_from_file(file_path)
		if image == null or image.is_empty():
			continue
		var texture: ImageTexture = ImageTexture.create_from_image(image)
		loaded_paths.append(file_path)
		loaded_textures.append(texture)

	if loaded_textures.size() <= 1:
		if loaded_paths.is_empty():
			preview_texture_rect.texture = null
			_image_info_text = tr("Failed to load image")
			_shader_status_text = ""
			_refresh_info_label()
			return false
		return open_file(loaded_paths[0])

	_sequence_paths = loaded_paths
	_sequence_textures = loaded_textures
	_sequence_index = 0
	_show_sequence_frame(0)
	_apply_filter_mode()
	_set_sequence_controls_visible(true)
	_sync_sequence_slider_bounds()
	_set_sequence_slider_value(0)
	_update_sequence_frame_label()
	_sync_sequence_play_pause_button()

	_image_info_text = tr("%s (%d frames)") % [loaded_paths[0].get_file(), loaded_textures.size()]
	_refresh_info_label()
	return true


func _setup_filter_options() -> void:
	if filter_mode_option_button == null or not is_instance_valid(filter_mode_option_button):
		return
	var selected_index: int = _current_filter_mode
	filter_mode_option_button.clear()
	filter_mode_option_button.add_item(tr("Nearest"))
	filter_mode_option_button.add_item(tr("Linear"))
	filter_mode_option_button.select(
		clampi(selected_index, 0, filter_mode_option_button.item_count - 1)
	)


func _setup_shader_path_line_edit() -> void:
	if shader_path_line_edit == null or not is_instance_valid(shader_path_line_edit):
		return
	if not shader_path_line_edit.text_changed.is_connected(_on_shader_path_line_edit_text_changed):
		shader_path_line_edit.text_changed.connect(_on_shader_path_line_edit_text_changed)
	if shader_path_line_edit.has_signal("shader_file_dropped"):
		var dropped_callable: Callable = Callable(self, "_on_shader_file_dropped")
		if not shader_path_line_edit.is_connected("shader_file_dropped", dropped_callable):
			shader_path_line_edit.connect("shader_file_dropped", dropped_callable)


func _setup_sequence_controls() -> void:
	if sequence_controls_container == null or not is_instance_valid(sequence_controls_container):
		return
	_set_sequence_controls_visible(false)

	if sequence_playback_timer != null and is_instance_valid(sequence_playback_timer):
		sequence_playback_timer.wait_time = 1.0 / DEFAULT_SEQUENCE_FPS
		sequence_playback_timer.one_shot = false
		if not sequence_playback_timer.timeout.is_connected(_on_sequence_playback_timer_timeout):
			sequence_playback_timer.timeout.connect(_on_sequence_playback_timer_timeout)

	if sequence_play_pause_button != null and is_instance_valid(sequence_play_pause_button):
		if not sequence_play_pause_button.pressed.is_connected(
			_on_sequence_play_pause_button_pressed
		):
			sequence_play_pause_button.pressed.connect(_on_sequence_play_pause_button_pressed)

	if sequence_progress_slider != null and is_instance_valid(sequence_progress_slider):
		sequence_progress_slider.step = 1.0
		if not sequence_progress_slider.value_changed.is_connected(
			_on_sequence_progress_slider_value_changed
		):
			sequence_progress_slider.value_changed.connect(
				_on_sequence_progress_slider_value_changed
			)
	if sequence_speed_spin_box != null and is_instance_valid(sequence_speed_spin_box):
		sequence_speed_spin_box.step = 0.01
		sequence_speed_spin_box.rounded = false
		if not sequence_speed_spin_box.value_changed.is_connected(
			_on_sequence_speed_spin_box_value_changed
		):
			sequence_speed_spin_box.value_changed.connect(_on_sequence_speed_spin_box_value_changed)
		_apply_sequence_speed(sequence_speed_spin_box.value)

	_sync_sequence_slider_bounds()
	_update_sequence_frame_label()
	_sync_sequence_play_pause_button()


func _on_filter_mode_selected(index: int) -> void:
	_current_filter_mode = index
	_apply_filter_mode()


func _on_clear_button_pressed() -> void:
	if shader_path_line_edit == null or not is_instance_valid(shader_path_line_edit):
		return
	shader_path_line_edit.clear()


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
	if shader_path_line_edit != null and is_instance_valid(shader_path_line_edit):
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


func _set_sequence_controls_visible(visible: bool) -> void:
	if sequence_controls_container == null or not is_instance_valid(sequence_controls_container):
		return
	sequence_controls_container.visible = visible


func _sync_sequence_slider_bounds() -> void:
	if sequence_progress_slider == null or not is_instance_valid(sequence_progress_slider):
		return
	sequence_progress_slider.min_value = 0.0
	sequence_progress_slider.max_value = float(maxi(_sequence_textures.size() - 1, 0))


func _set_sequence_slider_value(index: int) -> void:
	if sequence_progress_slider == null or not is_instance_valid(sequence_progress_slider):
		return
	_is_sequence_slider_syncing = true
	sequence_progress_slider.value = float(index)
	_is_sequence_slider_syncing = false


func _update_sequence_frame_label() -> void:
	if sequence_frame_label == null or not is_instance_valid(sequence_frame_label):
		return
	var total_count: int = _sequence_textures.size()
	if total_count <= 0:
		sequence_frame_label.text = "0 / 0"
		return
	sequence_frame_label.text = "%d / %d" % [_sequence_index + 1, total_count]


func _show_sequence_frame(index: int) -> void:
	if _sequence_textures.is_empty():
		preview_texture_rect.texture = null
		return
	_sequence_index = clampi(index, 0, _sequence_textures.size() - 1)
	preview_texture_rect.texture = _sequence_textures[_sequence_index]
	_set_sequence_slider_value(_sequence_index)
	_update_sequence_frame_label()


func _advance_sequence_frame() -> void:
	if _sequence_textures.is_empty():
		return
	var total_count: int = _sequence_textures.size()
	var next_index: int = (_sequence_index + 1) % total_count
	_show_sequence_frame(next_index)


func _on_sequence_play_pause_button_pressed() -> void:
	if _sequence_textures.size() <= 1:
		return
	if _is_sequence_playing:
		_stop_sequence_playback()
	else:
		_start_sequence_playback()


func _on_sequence_progress_slider_value_changed(value: float) -> void:
	if _is_sequence_slider_syncing:
		return
	if _sequence_textures.is_empty():
		return
	_stop_sequence_playback()
	_show_sequence_frame(int(round(value)))


func _on_sequence_speed_spin_box_value_changed(value: float) -> void:
	if _is_sequence_speed_syncing:
		return
	_apply_sequence_speed(value)


func _on_sequence_playback_timer_timeout() -> void:
	_advance_sequence_frame()


func _start_sequence_playback() -> void:
	if _sequence_textures.size() <= 1:
		return
	_is_sequence_playing = true
	if sequence_playback_timer != null and is_instance_valid(sequence_playback_timer):
		sequence_playback_timer.start(_get_sequence_wait_time())
	_sync_sequence_play_pause_button()


func _stop_sequence_playback() -> void:
	_is_sequence_playing = false
	if sequence_playback_timer != null and is_instance_valid(sequence_playback_timer):
		sequence_playback_timer.stop()
	_sync_sequence_play_pause_button()


func _sync_sequence_play_pause_button() -> void:
	if sequence_play_pause_button == null or not is_instance_valid(sequence_play_pause_button):
		return
	var has_sequence: bool = _sequence_textures.size() > 1
	sequence_play_pause_button.disabled = not has_sequence
	var icon: Texture2D = PAUSE_ICON if (_is_sequence_playing and has_sequence) else PLAY_ICON
	sequence_play_pause_button.texture_normal = icon
	sequence_play_pause_button.texture_pressed = icon
	sequence_play_pause_button.texture_hover = icon


func _clear_sequence_preview() -> void:
	_stop_sequence_playback()
	_sequence_paths.clear()
	_sequence_textures.clear()
	_sequence_index = 0
	_set_sequence_controls_visible(false)
	_sync_sequence_slider_bounds()
	_set_sequence_slider_value(0)
	_update_sequence_frame_label()


func _normalize_image_paths(paths: Array[String]) -> Array[String]:
	var normalized_paths: Array[String] = []
	var unique_path_map: Dictionary = {}
	for raw_path: String in paths:
		var normalized_path: String = _normalize_path(raw_path)
		if normalized_path.is_empty():
			continue
		if DirAccess.dir_exists_absolute(normalized_path):
			continue
		if not FileAccess.file_exists(normalized_path):
			continue
		var extension: String = normalized_path.get_extension().to_lower()
		if not IMAGE_EXTENSIONS.has(extension):
			continue
		if unique_path_map.has(normalized_path):
			continue
		unique_path_map[normalized_path] = true
		normalized_paths.append(normalized_path)

	normalized_paths.sort_custom(
		func(a: String, b: String) -> bool:
			return a.get_file().naturalnocasecmp_to(b.get_file()) < 0
	)
	return normalized_paths


func _apply_sequence_speed(value: float) -> void:
	var normalized_speed: float = snappedf(clampf(value, 0.1, 8.0), 0.01)
	_sequence_speed = normalized_speed
	if sequence_speed_spin_box != null and is_instance_valid(sequence_speed_spin_box):
		_is_sequence_speed_syncing = true
		sequence_speed_spin_box.value = normalized_speed
		_is_sequence_speed_syncing = false
	if sequence_playback_timer != null and is_instance_valid(sequence_playback_timer):
		sequence_playback_timer.wait_time = _get_sequence_wait_time()
		if _is_sequence_playing:
			sequence_playback_timer.start(_get_sequence_wait_time())


func _get_sequence_wait_time() -> float:
	var effective_speed: float = maxf(_sequence_speed, 0.1)
	var effective_fps: float = DEFAULT_SEQUENCE_FPS * effective_speed
	if effective_fps <= 0.0:
		effective_fps = DEFAULT_SEQUENCE_FPS
	return 1.0 / effective_fps


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
