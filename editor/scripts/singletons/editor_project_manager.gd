extends Node

const PROJECT_CONFIG_FILE_NAME: String = "utmx.cfg"
const PROJECTS_LIST_FILE_NAME: String = "projects.cfg"
const ENGINE_VERSION: String = "1.0.0-beta"
const DEFAULT_PROJECT_DIR_NAME: String = "UndertaleMakerProject"
const EXPORT_ANDROID_CONFIG_SECTION: String = "export_android"
const EXPORT_ANDROID_KEY_JDK_BIN_DIR: String = "jdk_bin_dir"
const EXPORT_ANDROID_KEY_KEYSTORE_PATH: String = "keystore_path"
const EXPORT_ANDROID_KEY_KEYSTORE_ALIAS: String = "keystore_alias"
const EXPORT_ANDROID_KEY_KEYSTORE_PASSWORD: String = "keystore_password"
const EXPORT_ANDROID_KEY_KEY_PASSWORD: String = "key_password"

var projects: Dictionary[String, UtmxProject] = {}
var opened_project: UtmxProject = null


func _ready() -> void:
	var base_path: String = get_default_project_path()
	if !DirAccess.dir_exists_absolute(base_path):
		DirAccess.make_dir_recursive_absolute(base_path)


func get_default_project_path() -> String:
	return OS.get_system_dir(OS.SystemDir.SYSTEM_DIR_DOCUMENTS).path_join(DEFAULT_PROJECT_DIR_NAME)


func get_opened_project_path() -> String:
	if is_instance_valid(opened_project):
		return opened_project.project_path
	return ""


func load_project(path: String) -> UtmxProject:
	var normalized_path: String = _normalize_path(path.strip_edges())
	if normalized_path.is_empty():
		return null

	if DirAccess.dir_exists_absolute(normalized_path):
		return load_project_config(normalized_path)

	if FileAccess.file_exists(normalized_path):
		if normalized_path.get_file() == PROJECT_CONFIG_FILE_NAME:
			return load_project_config(normalized_path.get_base_dir())

	return null


func load_editor_all_projects() -> void:
	projects.clear()
	var list_path: String = EditorConfigureManager.get_data_path().path_join(PROJECTS_LIST_FILE_NAME)
	if !FileAccess.file_exists(list_path):
		return

	var config_file: ConfigFile = ConfigFile.new()
	if config_file.load(list_path) != OK:
		return

	for project_dir in config_file.get_sections():
		var proj: UtmxProject = load_project_config(project_dir)
		if proj == null:
			continue
		proj.favorite = config_file.get_value(project_dir, "favorite", false)


func save_editor_all_projects() -> void:
	var list_path: String = EditorConfigureManager.get_data_path().path_join(PROJECTS_LIST_FILE_NAME)
	var config_file: ConfigFile = ConfigFile.new()

	for project_dir in projects:
		var proj: UtmxProject = projects[project_dir]
		config_file.set_value(project_dir, "favorite", proj.favorite)
		save_project_config(proj)

	config_file.save(list_path)


func save_project_config(project: UtmxProject) -> void:
	var config_file: ConfigFile = ConfigFile.new()
	var cfg_full_path: String = _normalize_path(
		project.project_path.path_join(PROJECT_CONFIG_FILE_NAME)
	)
	if FileAccess.file_exists(cfg_full_path):
		config_file.load(cfg_full_path)

	config_file.set_value("application", "name", project.project_name)
	config_file.set_value("application", "icon", project.icon)
	config_file.set_value("application", "last_open_time", project.last_open_time)
	config_file.set_value("application", "engine_version", project.engine_version)
	config_file.set_value("editor", "file_tree_expanded_dirs", project.file_tree_expanded_dirs)
	config_file.set_value("editor", "layout_state", project.get_editor_layout_state())
	config_file.save(cfg_full_path)


func get_project_android_export_options(project_dir_path: String = "") -> Dictionary:
	var defaults: Dictionary = _build_default_android_export_options()
	var cfg_full_path: String = _resolve_project_config_file_path(project_dir_path)
	if cfg_full_path.is_empty() or !FileAccess.file_exists(cfg_full_path):
		return defaults

	var config_file: ConfigFile = ConfigFile.new()
	if config_file.load(cfg_full_path) != OK:
		return defaults
	return _read_android_export_options(config_file)


func set_project_android_export_options(
	options: Dictionary, project_dir_path: String = ""
) -> bool:
	var cfg_full_path: String = _resolve_project_config_file_path(project_dir_path)
	if cfg_full_path.is_empty():
		return false

	var cfg_dir: String = cfg_full_path.get_base_dir()
	if !cfg_dir.is_empty() and !DirAccess.dir_exists_absolute(cfg_dir):
		var mkdir_err: int = DirAccess.make_dir_recursive_absolute(cfg_dir)
		if mkdir_err != OK:
			return false

	var config_file: ConfigFile = ConfigFile.new()
	if FileAccess.file_exists(cfg_full_path):
		config_file.load(cfg_full_path)

	var normalized_options: Dictionary = _build_default_android_export_options()
	for key_variant in normalized_options.keys():
		var key: String = String(key_variant)
		normalized_options[key] = String(options.get(key, normalized_options[key]))

	if String(normalized_options.get(EXPORT_ANDROID_KEY_KEY_PASSWORD, "")).is_empty():
		normalized_options[EXPORT_ANDROID_KEY_KEY_PASSWORD] = String(
			normalized_options.get(EXPORT_ANDROID_KEY_KEYSTORE_PASSWORD, "")
		)

	for key_variant in normalized_options.keys():
		var key: String = String(key_variant)
		config_file.set_value(EXPORT_ANDROID_CONFIG_SECTION, key, normalized_options[key])

	return config_file.save(cfg_full_path) == OK


func load_project_config(dir_path: String) -> UtmxProject:
	var normalized_dir_path: String = _normalize_path(dir_path)
	var cfg_full_path: String = normalized_dir_path.path_join(PROJECT_CONFIG_FILE_NAME)
	if !FileAccess.file_exists(cfg_full_path):
		return null

	var config_file: ConfigFile = ConfigFile.new()
	if config_file.load(cfg_full_path) != OK:
		return null

	var result: UtmxProject = UtmxProject.new()
	result.project_path = normalized_dir_path
	result.project_name = config_file.get_value("application", "name", "Unnamed Project")
	result.icon = config_file.get_value("application", "icon", "icon.svg")
	result.last_open_time = config_file.get_value("application", "last_open_time", 0)
	result.engine_version = config_file.get_value("application", "engine_version", "")

	var expanded_dirs_var: Variant = config_file.get_value("editor", "file_tree_expanded_dirs", [])
	if expanded_dirs_var is Array:
		for value in expanded_dirs_var:
			result.file_tree_expanded_dirs.append(String(value))

	var layout_state_var: Variant = config_file.get_value("editor", "layout_state", {})
	if layout_state_var is Dictionary:
		result.set_editor_layout_state(layout_state_var)

	var icon_full_path: String = normalized_dir_path.path_join(result.icon.trim_prefix("/"))
	if FileAccess.file_exists(icon_full_path):
		var icon_img: Image = Image.load_from_file(icon_full_path)
		if icon_img:
			result.icon_texture = ImageTexture.create_from_image(icon_img)

	projects[normalized_dir_path] = result
	return result


func _resolve_project_config_file_path(project_dir_path: String = "") -> String:
	var normalized: String = _normalize_path(String(project_dir_path).strip_edges())
	if normalized.is_empty():
		normalized = _normalize_path(get_opened_project_path())
	if normalized.is_empty():
		return ""

	if (
		FileAccess.file_exists(normalized)
		and normalized.get_file().to_lower() == PROJECT_CONFIG_FILE_NAME
	):
		return normalized

	return _normalize_path(normalized.path_join(PROJECT_CONFIG_FILE_NAME))


func _build_default_android_export_options() -> Dictionary:
	return {
		EXPORT_ANDROID_KEY_JDK_BIN_DIR: "",
		EXPORT_ANDROID_KEY_KEYSTORE_PATH: "",
		EXPORT_ANDROID_KEY_KEYSTORE_ALIAS: "",
		EXPORT_ANDROID_KEY_KEYSTORE_PASSWORD: "",
		EXPORT_ANDROID_KEY_KEY_PASSWORD: "",
	}


func _read_android_export_options(config_file: ConfigFile) -> Dictionary:
	var result: Dictionary = _build_default_android_export_options()
	for key_variant in result.keys():
		var key: String = String(key_variant)
		result[key] = String(config_file.get_value(EXPORT_ANDROID_CONFIG_SECTION, key, result[key]))

	if String(result.get(EXPORT_ANDROID_KEY_KEY_PASSWORD, "")).is_empty():
		result[EXPORT_ANDROID_KEY_KEY_PASSWORD] = String(
			result.get(EXPORT_ANDROID_KEY_KEYSTORE_PASSWORD, "")
		)

	return result


func remove_project(dir_path: String) -> void:
	projects.erase(_normalize_path(dir_path))


func create_default_project(project_name: String, dir_path: String) -> UtmxProject:
	return create_project_from_template(project_name, dir_path)


func create_project_from_template(
	project_name: String, target_dir: String, template: UtmxProjectTemplate = null
) -> UtmxProject:
	var normalized_target_dir: String = _normalize_path(target_dir)
	if projects.has(normalized_target_dir):
		return projects[normalized_target_dir]

	var template_zip_path: String = _resolve_template_zip_path(template)
	if template_zip_path.is_empty():
		push_warning("Failed to create project: no valid template zip is available.")
		return null

	return create_project_from_zip(project_name, template_zip_path, normalized_target_dir)


func _resolve_template_zip_path(template: UtmxProjectTemplate) -> String:
	if template != null:
		var template_zip_path: String = _normalize_path(String(template.zip_path).strip_edges())
		if !template_zip_path.is_empty() and FileAccess.file_exists(template_zip_path):
			return template_zip_path

	if EditorTemplateManager.templates.is_empty():
		EditorTemplateManager.load_templates_from_directory(
			EditorTemplateManager.get_default_template_directory()
		)

	var default_template: UtmxProjectTemplate = EditorTemplateManager.get_default_template()
	if default_template != null:
		var default_template_zip: String = _normalize_path(default_template.zip_path)
		if !default_template_zip.is_empty() and FileAccess.file_exists(default_template_zip):
			return default_template_zip

	var default_empty_template_zip: String = _normalize_path(
		EditorTemplateManager.get_default_empty_template_zip_path()
	)
	if !default_empty_template_zip.is_empty() and FileAccess.file_exists(default_empty_template_zip):
		return default_empty_template_zip

	return ""


func create_project_from_zip(
	project_name: String, zip_path: String, target_dir: String
) -> UtmxProject:
	var normalized_zip_path: String = _normalize_path(zip_path)
	var normalized_target_dir: String = _normalize_path(target_dir)
	if !FileAccess.file_exists(normalized_zip_path):
		return null

	var reader: ZIPReader = ZIPReader.new()
	if reader.open(normalized_zip_path) != OK:
		return null

	var files: PackedStringArray = reader.get_files()
	var project_root_prefix: String = _resolve_zip_project_root_prefix(files)
	var has_project_config: bool = false
	for entry in files:
		if entry.get_file() == PROJECT_CONFIG_FILE_NAME:
			has_project_config = true
			break
	if !has_project_config:
		reader.close()
		return null

	if !DirAccess.dir_exists_absolute(normalized_target_dir):
		DirAccess.make_dir_recursive_absolute(normalized_target_dir)

	for zip_entry: String in files:
		var relative_entry: String = _build_relative_zip_entry_path(zip_entry, project_root_prefix)
		if relative_entry.is_empty():
			continue
		if relative_entry.get_extension().to_lower() == "import":
			continue
		var destination_path: String = _normalize_path(normalized_target_dir.path_join(relative_entry))
		if zip_entry.ends_with("/"):
			DirAccess.make_dir_recursive_absolute(destination_path)
			continue

		DirAccess.make_dir_recursive_absolute(destination_path.get_base_dir())
		var buffer: PackedByteArray = reader.read_file(zip_entry)
		var file: FileAccess = FileAccess.open(destination_path, FileAccess.WRITE)
		if file == null:
			reader.close()
			return null
		file.store_buffer(buffer)
		file.close()

	reader.close()

	var project: UtmxProject = load_project_config(normalized_target_dir)
	if project == null:
		return null

	project.project_name = project_name
	project.favorite = false
	project.last_open_time = int(Time.get_unix_time_from_system())
	if project.engine_version.is_empty():
		project.engine_version = ENGINE_VERSION
	save_project_config(project)
	return project


func _resolve_zip_project_root_prefix(files: PackedStringArray) -> String:
	var found_project_config: bool = false
	var shortest_prefix: String = ""
	for entry in files:
		if entry.ends_with("/"):
			continue
		if entry.get_file() != PROJECT_CONFIG_FILE_NAME:
			continue

		var candidate_prefix: String = _normalize_path(entry.get_base_dir())
		if candidate_prefix == ".":
			candidate_prefix = ""
		if !found_project_config or candidate_prefix.length() < shortest_prefix.length():
			shortest_prefix = candidate_prefix
			found_project_config = true

	if !found_project_config:
		return ""
	if shortest_prefix.is_empty():
		return ""
	if !shortest_prefix.ends_with("/"):
		shortest_prefix += "/"
	return shortest_prefix


func _build_relative_zip_entry_path(zip_entry: String, root_prefix: String) -> String:
	var normalized_entry: String = _normalize_path(zip_entry)
	if normalized_entry.is_empty() or normalized_entry == ".":
		return ""

	var relative_entry: String = normalized_entry
	if !root_prefix.is_empty():
		if !normalized_entry.begins_with(root_prefix):
			return ""
		relative_entry = normalized_entry.trim_prefix(root_prefix)

	relative_entry = relative_entry.trim_prefix("/")
	return relative_entry


const EDITOR_SCENE: PackedScene = preload("uid://cp81di5w374d2")
const PROJECT_MANAGER_SCENE: PackedScene = preload("uid://djn5y1cfoknas")


func open_project(project: UtmxProject) -> void:
	if project == null:
		return
	project.last_open_time = int(Time.get_unix_time_from_system())
	save_project_config(project)
	opened_project = project
	get_tree().change_scene_to_packed(EDITOR_SCENE)


func back_to_project_list() -> void:
	get_tree().change_scene_to_packed(PROJECT_MANAGER_SCENE)
	call_deferred("_clear_opened_project_reference")


func _clear_opened_project_reference() -> void:
	opened_project = null


func _normalize_path(path: String) -> String:
	var raw_path: String = String(path).strip_edges().replace("\\", "/")
	if raw_path.is_empty():
		return ""
	if raw_path.begins_with("res://"):
		return "res://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("res://"))]
	if raw_path.begins_with("user://"):
		return "user://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("user://"))]
	return raw_path.simplify_path()


func _normalize_virtual_path_body(path_body: String) -> String:
	var normalized_body: String = String(path_body).strip_edges().replace("\\", "/")
	while normalized_body.begins_with("/"):
		normalized_body = normalized_body.trim_prefix("/")
	while normalized_body.contains("//"):
		normalized_body = normalized_body.replace("//", "/")

	var segments: Array[String] = []
	for raw_segment: String in normalized_body.split("/", false):
		var segment: String = raw_segment.strip_edges()
		if segment.is_empty() || segment == ".":
			continue
		if segment == "..":
			if !segments.is_empty():
				segments.remove_at(segments.size() - 1)
			continue
		segments.append(segment)
	return "/".join(segments)
