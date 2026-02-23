extends PanelContainer

signal preview_file_requested(path: String)
signal preview_files_requested(paths: Array[String])

const MODULE_IMAGE := "image"
const MODULE_AUDIO := "audio"
const MODULE_FONT := "font"
const MODULE_UNSUPPORTED := "unsupported"

const IMAGE_EXTENSIONS := {
	"png": true,
	"jpg": true,
	"jpeg": true,
	"webp": true,
	"svg": true,
	"bmp": true,
	"tga": true,
}

const AUDIO_EXTENSIONS := {
	"wav": true,
	"ogg": true,
	"mp3": true,
	"flac": true,
}

const FONT_EXTENSIONS := {
	"ttf": true,
	"otf": true,
	"fnt": true,
	"woff": true,
	"woff2": true,
}

const MODULE_SCENES := {
	MODULE_IMAGE: preload("res://scenes/editor/file_browser/modules/image_browser_module.tscn"),
	MODULE_AUDIO: preload("res://scenes/editor/file_browser/modules/audio_browser_module.tscn"),
	MODULE_FONT: preload("res://scenes/editor/file_browser/modules/font_browser_module.tscn"),
	MODULE_UNSUPPORTED:
	preload("res://scenes/editor/file_browser/modules/unsupported_browser_module.tscn"),
}

@onready var file_name_label: Label = %FileNameLabel
@onready var module_container: Control = %ModuleContainer

var _module_instances: Dictionary = {}
var _current_module_key: String = ""
var _current_path: String = ""


func _ready() -> void:
	_update_file_name_label()


func _notification(what: int) -> void:
	if what == NOTIFICATION_TRANSLATION_CHANGED:
		if not is_node_ready():
			return
		_update_file_name_label()


func _can_drop_data(_at_position: Vector2, data: Variant) -> bool:
	return not _extract_droppable_file_paths(data).is_empty()


func _drop_data(_at_position: Vector2, data: Variant) -> void:
	var file_paths: Array[String] = _extract_droppable_file_paths(data)
	if file_paths.is_empty():
		return
	if file_paths.size() == 1:
		preview_file_requested.emit(file_paths[0])
		return
	preview_files_requested.emit(file_paths)


func can_open_file(path: String) -> bool:
	var normalized_path: String = _normalize_path(path)
	if normalized_path.is_empty():
		return false
	if DirAccess.dir_exists_absolute(normalized_path):
		return false
	if not FileAccess.file_exists(normalized_path):
		return false
	var extension: String = normalized_path.get_extension().to_lower()
	return _resolve_module_key(extension) != MODULE_UNSUPPORTED


func preview_file(path: String) -> bool:
	return open_file(path)


func preview_files(paths: Array[String]) -> bool:
	var normalized_paths: Array[String] = _normalize_existing_files(paths)
	if normalized_paths.is_empty():
		return false
	if normalized_paths.size() == 1:
		return preview_file(normalized_paths[0])

	var first_extension: String = normalized_paths[0].get_extension().to_lower()
	var module_key: String = _resolve_module_key(first_extension)
	if module_key == MODULE_UNSUPPORTED:
		return false

	for file_path: String in normalized_paths:
		var ext: String = file_path.get_extension().to_lower()
		if _resolve_module_key(ext) != module_key:
			return preview_file(normalized_paths[0])

	var module_instance: Control = _get_or_create_module(module_key)
	if module_instance == null:
		return false
	_show_module(module_key)

	var opened: bool = false
	if module_instance.has_method("open_files"):
		var open_result: Variant = module_instance.call("open_files", normalized_paths)
		opened = bool(open_result)
	elif module_instance.has_method("open_file"):
		var single_result: Variant = module_instance.call("open_file", normalized_paths[0])
		opened = bool(single_result)

	if not opened:
		return false

	_current_path = normalized_paths[0]
	_update_file_name_label()
	return true


func open_file(path: String) -> bool:
	var normalized_path: String = _normalize_path(path)
	if normalized_path.is_empty():
		return false
	if DirAccess.dir_exists_absolute(normalized_path):
		return false
	if not FileAccess.file_exists(normalized_path):
		return false

	var extension: String = normalized_path.get_extension().to_lower()
	var module_key: String = _resolve_module_key(extension)
	if module_key == MODULE_UNSUPPORTED:
		return false

	var opened: bool = _open_with_module(module_key, normalized_path)
	if not opened:
		_open_with_module(MODULE_UNSUPPORTED, normalized_path)

	_current_path = normalized_path
	_update_file_name_label()
	return true


func _open_with_module(module_key: String, path: String) -> bool:
	var module_instance: Control = _get_or_create_module(module_key)
	if module_instance == null:
		return false
	_show_module(module_key)
	if not module_instance.has_method("open_file"):
		return false
	var open_result: Variant = module_instance.call("open_file", path)
	return bool(open_result)


func _get_or_create_module(module_key: String) -> Control:
	if _module_instances.has(module_key):
		var cached: Variant = _module_instances[module_key]
		if cached is Control and is_instance_valid(cached):
			return cached as Control

	if not MODULE_SCENES.has(module_key):
		return null
	var packed_scene: PackedScene = MODULE_SCENES[module_key]
	if packed_scene == null:
		return null

	var instance: Control = packed_scene.instantiate() as Control
	if instance == null:
		return null
	instance.visible = false
	instance.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	instance.size_flags_vertical = Control.SIZE_EXPAND_FILL
	module_container.add_child(instance)
	_install_drop_forwarding(instance)
	_module_instances[module_key] = instance
	return instance


func _show_module(module_key: String) -> void:
	for key in _module_instances.keys():
		var module_instance: Variant = _module_instances[key]
		if module_instance is Control and is_instance_valid(module_instance):
			(module_instance as Control).visible = str(key) == module_key
	_current_module_key = module_key


func _resolve_module_key(extension: String) -> String:
	if IMAGE_EXTENSIONS.has(extension):
		return MODULE_IMAGE
	if AUDIO_EXTENSIONS.has(extension):
		return MODULE_AUDIO
	if FONT_EXTENSIONS.has(extension):
		return MODULE_FONT
	return MODULE_UNSUPPORTED


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()


func _update_file_name_label() -> void:
	if file_name_label == null or not is_instance_valid(file_name_label):
		return
	if _current_path.is_empty():
		file_name_label.text = tr("No file selected")
		return
	file_name_label.text = _current_path.get_file()


func _extract_droppable_file_paths(data: Variant) -> Array[String]:
	var raw_paths: Array[String] = []
	if data is Dictionary:
		if data.has("paths"):
			var dict_paths: Variant = data.get("paths")
			if dict_paths is PackedStringArray:
				for p in dict_paths:
					raw_paths.append(String(p))
			elif dict_paths is Array:
				for p in dict_paths:
					if p is String:
						raw_paths.append(String(p))
		elif data.has("files"):
			var files: Variant = data.get("files")
			if files is PackedStringArray:
				for p in files:
					raw_paths.append(String(p))
			elif files is Array:
				for p in files:
					if p is String:
						raw_paths.append(String(p))
	elif data is PackedStringArray:
		for p in data:
			raw_paths.append(String(p))
	elif data is Array:
		for p in data:
			if p is String:
				raw_paths.append(String(p))

	var file_paths: Array[String] = []
	var unique_paths: Dictionary = {}
	for raw_path in raw_paths:
		var normalized_path: String = _normalize_path(raw_path)
		if normalized_path.is_empty():
			continue
		if DirAccess.dir_exists_absolute(normalized_path):
			continue
		if not FileAccess.file_exists(normalized_path):
			continue
		if unique_paths.has(normalized_path):
			continue
		unique_paths[normalized_path] = true
		file_paths.append(normalized_path)
	return file_paths


func _normalize_existing_files(paths: Array[String]) -> Array[String]:
	var normalized_files: Array[String] = []
	var unique_path_map: Dictionary = {}
	for raw_path: String in paths:
		var normalized_path: String = _normalize_path(raw_path)
		if normalized_path.is_empty():
			continue
		if DirAccess.dir_exists_absolute(normalized_path):
			continue
		if not FileAccess.file_exists(normalized_path):
			continue
		if unique_path_map.has(normalized_path):
			continue
		unique_path_map[normalized_path] = true
		normalized_files.append(normalized_path)
	return normalized_files


func _forward_get_drag_data(_at_position: Vector2) -> Variant:
	return null


func _forward_can_drop_data(at_position: Vector2, data: Variant) -> bool:
	return _can_drop_data(at_position, data)


func _forward_drop_data(at_position: Vector2, data: Variant) -> void:
	_drop_data(at_position, data)


func _install_drop_forwarding(root: Node) -> void:
	if root is Control:
		var root_control: Control = root as Control
		_try_set_drop_forwarding(root_control)
	for child in root.get_children():
		_install_drop_forwarding(child)


func _try_set_drop_forwarding(control: Control) -> void:
	if _should_skip_drop_forwarding(control):
		return
	if not control.has_method("set_drag_forwarding"):
		return
	control.set_drag_forwarding(
		Callable(self, "_forward_get_drag_data"),
		Callable(self, "_forward_can_drop_data"),
		Callable(self, "_forward_drop_data")
	)


func _should_skip_drop_forwarding(control: Control) -> bool:
	var script_resource: Script = control.get_script() as Script
	if script_resource == null:
		return false
	var script_path: String = String(script_resource.resource_path)
	return script_path.ends_with("shader_path_line_edit.gd")
