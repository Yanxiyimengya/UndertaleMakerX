extends PanelContainer

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
	MODULE_UNSUPPORTED: preload("res://scenes/editor/file_browser/modules/unsupported_browser_module.tscn"),
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
