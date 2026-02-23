class_name UtmxProject extends Resource

## 偏好
var favorite: bool = false

## 项目名称
var project_name: String = ""

## 项目绝对路径
var project_path: String = ""

## 项目icon的相对路径
var icon: String = ""

## 项目icon的纹理对象
var icon_texture: Texture2D = null

## 项目最后一次打开的时间（使用UTC时间）
var last_open_time: int = 0

## 项目最后一次打开使用的引擎版本
var engine_version: String = ""

## 文件树展开目录（存储为相对项目根目录路径）
var file_tree_expanded_dirs: Array[String] = []

## 编辑器主布局状态（可序列化到utmx.cfg）
var editor_layout_state: Dictionary = {}


func set_file_tree_expanded_dirs_from_absolute(paths: Array[String]) -> void:
	var root_path: String = _normalize_path(project_path)
	var root_with_slash: String = root_path
	if !root_with_slash.ends_with("/"):
		root_with_slash += "/"

	var result: Array[String] = []
	for raw_path in paths:
		var abs_path: String = _normalize_path(String(raw_path))
		if abs_path.is_empty():
			continue
		if abs_path == root_path:
			if !result.has("."):
				result.append(".")
			continue
		if !abs_path.begins_with(root_with_slash):
			continue
		var relative_path: String = abs_path.trim_prefix(root_with_slash)
		if relative_path.is_empty():
			relative_path = "."
		if !result.has(relative_path):
			result.append(relative_path)

	file_tree_expanded_dirs = result


func get_file_tree_expanded_dirs_as_absolute() -> Array[String]:
	var root_path: String = _normalize_path(project_path)
	var root_with_slash: String = root_path
	if !root_with_slash.ends_with("/"):
		root_with_slash += "/"

	var result: Array[String] = []
	for raw_path in file_tree_expanded_dirs:
		var relative_path: String = _normalize_path(String(raw_path))
		if relative_path.is_empty() || relative_path == ".":
			if !result.has(root_path):
				result.append(root_path)
			continue

		var absolute_path: String = ""
		if relative_path == root_path || relative_path.begins_with(root_with_slash):
			absolute_path = relative_path
		else:
			absolute_path = _normalize_path(root_path.path_join(relative_path))
		if !result.has(absolute_path):
			result.append(absolute_path)

	return result


func set_editor_layout_state(state: Dictionary) -> void:
	editor_layout_state = state.duplicate(true)


func get_editor_layout_state() -> Dictionary:
	return editor_layout_state.duplicate(true)


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()
