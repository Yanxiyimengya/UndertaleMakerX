extends Node
# 单例名称: ProjectManager

const PROJECT_CONFIG_FILE_NAME: String = "utmx.cfg"
const RUNTIME_PROJECT_CONFIG_FILE_NAME: String = "project_config.json"
const PROJECTS_LIST_FILE_NAME: String = "projects.cfg"
const ENGINE_VERSION: String = "1.0.0-alpha"
const DEFAULT_PROJECT_DIR_NAME: String = "UndertaleMakerProject"

## 存储结构: { "项目目录路径": UtmxProject实例 }
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


## 兼容入口：支持传入项目目录或 utmx.cfg 文件路径
func load_project(path: String) -> UtmxProject:
	if path.is_empty():
		return null

	if DirAccess.dir_exists_absolute(path):
		return load_project_config(path)

	if FileAccess.file_exists(path):
		var file_name: String = path.get_file()
		if file_name == PROJECT_CONFIG_FILE_NAME:
			return load_project_config(path.get_base_dir())

	return null


# --- 核心：加载与保存项目清单 (编辑器全局) ---


## 从编辑器的项目清单文件加载所有已知项目
func load_editor_all_projects() -> void:
	projects.clear()
	var list_path: String = EditorConfigureManager.get_data_path().path_join(
		PROJECTS_LIST_FILE_NAME
	)
	if !FileAccess.file_exists(list_path):
		return

	var config_file: ConfigFile = ConfigFile.new()
	config_file.load(list_path)

	for project_dir in config_file.get_sections():
		# 这里 project_dir 就是项目的文件夹路径
		var proj: UtmxProject = load_project_config(project_dir)
		if proj == null:
			# 如果项目文件夹或配置文件不存在，可以考虑从清单中移除
			continue
		proj.favorite = config_file.get_value(project_dir, "favorite", false)


## 保存当前项目列表到编辑器全局清单
func save_editor_all_projects() -> void:
	var list_path: String = EditorConfigureManager.get_data_path().path_join(
		PROJECTS_LIST_FILE_NAME
	)
	var config_file: ConfigFile = ConfigFile.new()

	for project_dir in projects:
		var proj: UtmxProject = projects[project_dir]
		config_file.set_value(project_dir, "favorite", proj.favorite)
		# 同步保存该项目的具体配置
		save_project_config(proj)

	config_file.save(list_path)


# --- 核心：单个项目配置文件操作 (项目目录下) ---


## 保存项目的 utmx.cfg
func save_project_config(project: UtmxProject) -> void:
	var config_file: ConfigFile = ConfigFile.new()
	var cfg_full_path: String = project.project_path.path_join(PROJECT_CONFIG_FILE_NAME)

	config_file.set_value("application", "name", project.project_name)
	config_file.set_value("application", "icon", project.icon)
	config_file.set_value("application", "last_open_time", project.last_open_time)
	config_file.set_value("application", "engine_version", project.engine_version)
	config_file.set_value("editor", "file_tree_expanded_dirs", project.file_tree_expanded_dirs)
	config_file.set_value("editor", "layout_state", project.get_editor_layout_state())

	config_file.save(cfg_full_path)


## 加载特定目录下的项目配置
func load_project_config(dir_path: String) -> UtmxProject:
	var cfg_full_path: String = dir_path.path_join(PROJECT_CONFIG_FILE_NAME)

	if !FileAccess.file_exists(cfg_full_path):
		return null

	var config_file: ConfigFile = ConfigFile.new()
	var err = config_file.load(cfg_full_path)
	if err != OK:
		return null

	var result: UtmxProject = UtmxProject.new()
	result.project_path = dir_path  # 存储目录路径

	result.project_name = config_file.get_value("application", "name", "未命名项目")
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

	# 加载图标纹理
	var icon_full_path = dir_path.path_join(result.icon.trim_prefix("/"))
	if FileAccess.file_exists(icon_full_path):
		var icon_img: Image = Image.load_from_file(icon_full_path)
		if icon_img:
			result.icon_texture = ImageTexture.create_from_image(icon_img)

	projects[dir_path] = result
	return result


# --- 业务逻辑 ---


func remove_project(dir_path: String) -> void:
	projects.erase(dir_path)


## 创建默认的空项目
func create_default_project(project_name: String, dir_path: String) -> UtmxProject:
	if projects.has(dir_path):
		return projects[dir_path]

	if !DirAccess.dir_exists_absolute(dir_path):
		DirAccess.make_dir_recursive_absolute(dir_path)

	var proj: UtmxProject = UtmxProject.new()
	proj.project_name = project_name
	proj.project_path = dir_path
	proj.icon = "icon.svg"
	proj.favorite = false
	proj.engine_version = ENGINE_VERSION
	proj.last_open_time = int(Time.get_unix_time_from_system())

	# 保存配置
	save_project_config(proj)
	projects[dir_path] = proj

	var icon_src = "res://assets/icons/utmx-icon-256.svg"
	if FileAccess.file_exists(icon_src):
		var dir = DirAccess.open(dir_path)
		dir.copy(icon_src, dir_path.path_join(proj.icon))

	_create_default_runtime_project_config(dir_path)

	return proj


func _create_default_runtime_project_config(project_dir_path: String) -> void:
	var file_path: String = project_dir_path.path_join(RUNTIME_PROJECT_CONFIG_FILE_NAME)
	var config: Dictionary = {
		"application":
		{
			"author": "",
			"main_scene": "",
			"main_script": "main.js",
			"max_fps": 60.0,
			"name": "UNDERTALE MAKER X",
			"vsync": false
		},
		"boot_splash":
		{
			"background_color": "#101020",
			"display_text": "UNDERTALE MAKER\nX",
			"enabled": false,
			"speed_scale": 1.0
		},
		"virtual_input": {"dead_zone": 0.1, "enabled": false},
		"window":
		{
			"boderless": false,
			"clear_color": "#000000",
			"fullscreen": false,
			"height": 480.0,
			"resizable": false,
			"width": 640.0
		}
	}
	var file: FileAccess = FileAccess.open(file_path, FileAccess.WRITE)
	if file == null:
		push_warning("Failed to create default project config: %s" % file_path)
		return
	file.store_string(JSON.stringify(config, "\t"))
	file.close()
	file = null

	var script_path: String = project_dir_path.path_join(config["application"]["main_script"])
	file = FileAccess.open(script_path, FileAccess.WRITE)
	if file == null:
		return
	file.store_string(ProjectFileTemplates.MAIN_JS_SCRIPT_TEMPLATE)
	file.close()


## 从 ZIP 加载项目
func create_project_from_zip(
	project_name: String, zip_path: String, target_dir: String
) -> UtmxProject:
	if !FileAccess.file_exists(zip_path):
		return null

	var reader: ZIPReader = ZIPReader.new()
	if reader.open(zip_path) != OK:
		return null

	var files: PackedStringArray = reader.get_files()

	# 检查 ZIP 内是否存在配置文件
	var valid_zip: bool = false
	for f in files:
		if f.get_file() == PROJECT_CONFIG_FILE_NAME:
			valid_zip = true
			break

	if !valid_zip:
		reader.close()
		return null

	# 解压过程
	for file_path in files:
		var dest_path: String = target_dir.path_join(file_path)
		if file_path.ends_with("/"):
			DirAccess.make_dir_recursive_absolute(dest_path)
		else:
			DirAccess.make_dir_recursive_absolute(dest_path.get_base_dir())
			var buffer: PackedByteArray = reader.read_file(file_path)
			var f: FileAccess = FileAccess.open(dest_path, FileAccess.WRITE)
			if f:
				f.store_buffer(buffer)
				f.close()
	reader.close()

	# 加载并更新名称
	var proj: UtmxProject = load_project_config(target_dir)
	if proj:
		proj.project_name = project_name
		save_project_config(proj)
	return proj


# --- 场景切换 ---

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
