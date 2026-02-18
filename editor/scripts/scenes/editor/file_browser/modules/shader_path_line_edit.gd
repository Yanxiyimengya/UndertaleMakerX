extends LineEdit

signal shader_file_dropped(path: String)

const SHADER_EXTENSIONS: Dictionary = {
	"gdshader": true,
	"gdshade": true,
	"shader": true,
}


func _ready() -> void:
	clear_button_enabled = true
	context_menu_enabled = false
	editable = false
	placeholder_text = tr("Drop shader file path here")


func _notification(what: int) -> void:
	if what == NOTIFICATION_TRANSLATION_CHANGED:
		placeholder_text = tr("Drop shader file path here")


func _can_drop_data(_at_position: Vector2, data: Variant) -> bool:
	return not _extract_shader_paths(data).is_empty()


func _drop_data(_at_position: Vector2, data: Variant) -> void:
	var shader_paths: Array[String] = _extract_shader_paths(data)
	if shader_paths.is_empty():
		return
	shader_file_dropped.emit(shader_paths[0])


func _extract_shader_paths(data: Variant) -> Array[String]:
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

	var shader_paths: Array[String] = []
	var unique_path_map: Dictionary = {}
	for raw_path in raw_paths:
		var normalized_path: String = String(raw_path).replace("\\", "/").simplify_path()
		if normalized_path.is_empty():
			continue
		if DirAccess.dir_exists_absolute(normalized_path):
			continue
		if not FileAccess.file_exists(normalized_path):
			continue
		var extension: String = normalized_path.get_extension().to_lower()
		if not SHADER_EXTENSIONS.has(extension):
			continue
		if unique_path_map.has(normalized_path):
			continue
		unique_path_map[normalized_path] = true
		shader_paths.append(normalized_path)
	return shader_paths
