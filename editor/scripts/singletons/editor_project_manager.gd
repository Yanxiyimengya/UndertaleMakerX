extends Node;
const PROJECT_CONFIG_FILE_NAME : String = "utmx.cfg";
const PROJECTS_LIST_FILE_NAME : String = "projects.cfg";
const ENGINE_VERSION : String = "1.0.0-alpha";

var projects : Dictionary[String, UtmxProject] = {};

func load_editor_all_project() -> void: 
	projects.clear();
	var path : String = EditorConfigureManager.get_data_path().path_join(PROJECTS_LIST_FILE_NAME);
	var config_file : ConfigFile = ConfigFile.new();
	config_file.load(path);
	var sections : PackedStringArray = config_file.get_sections();
	for project_path : String in sections : 
		var proj : UtmxProject = load_project(project_path);
		if (proj == null) : 
			config_file.erase_section(project_path);
			continue;
		proj.favorite = config_file.get_value(project_path, "favorite", false);

func save_editor_project_config() -> void:
	var path : String = EditorConfigureManager.get_data_path().path_join(PROJECTS_LIST_FILE_NAME);
	var config_file : ConfigFile = ConfigFile.new();
	for proj_path : String in projects : 
		var proj : UtmxProject = projects[proj_path];
		config_file.set_value(proj_path, "favorite", proj.favorite);
		save_propject_config(proj_path, proj);
	config_file.save(path);

func save_propject_config(path : String, project : UtmxProject) -> void : 
	## config
	var config_file : ConfigFile = ConfigFile.new();
	var cfg_path : String = path.path_join(PROJECT_CONFIG_FILE_NAME);
	config_file.set_value("application", "name", project.project_name);
	config_file.set_value("application", "icon", project.icon);
	config_file.set_value("application", "last_open_time", project.last_open_time);
	config_file.set_value("application", "engine_version", project.engine_version);
	config_file.save(cfg_path);

func create_default_propject(project_name : String, path : String) -> bool : 
	if (projects.has(path)) : 
		return false;
	
	if (!DirAccess.dir_exists_absolute(path)) : 
		DirAccess.make_dir_recursive_absolute(path);
	var default_project : UtmxProject = UtmxProject.new();
	default_project.project_name = project_name;
	default_project.project_path = path;
	default_project.icon = "/icon.svg";
	default_project.favorite = false;
	default_project.engine_version = ENGINE_VERSION;
	default_project.last_open_time = int(Time.get_unix_time_from_system());
	save_propject_config(path, default_project);
	projects[path] = default_project;
	
	## 创建默认图标
	var icon_file : FileAccess = \
			FileAccess.open("res://assets/icons/utmx-icon-256.svg", FileAccess.READ);
	var file_buffer : PackedByteArray = icon_file.get_buffer(icon_file.get_length());
	icon_file.close();
	var file : FileAccess = \
			FileAccess.open(path.path_join(default_project.icon),FileAccess.WRITE);
	file.store_buffer(file_buffer);
	file.close();
	return true;


func load_project(path : String) -> UtmxProject : 
	var cfg_path : String;
	if (!path.ends_with(PROJECT_CONFIG_FILE_NAME)) :
		cfg_path = path.path_join(PROJECT_CONFIG_FILE_NAME);
	else : 
		cfg_path = path;
		path = path.get_base_dir();
	if (!FileAccess.file_exists(cfg_path) || projects.has(cfg_path)) : return null;
	
	var config_file : ConfigFile = ConfigFile.new();
	config_file.load(cfg_path);
	var result : UtmxProject = UtmxProject.new();
	
	## name
	result.project_name = config_file.get_value("application", "name", "");
	
	## icon
	result.icon = config_file.get_value("application", "icon", "");
	var icon_path = path.path_join(result.icon);
	if (!FileAccess.file_exists(icon_path)) : 
		var icon_img : Image = Image.load_from_file(icon_path);
		result.icon_texture = ImageTexture.create_from_image(icon_img);;
	
	##time
	result.last_open_time = config_file.get_value("application", "last_open_time", 0);
	
	#engine_version
	result.engine_version = config_file.get_value("application", "engine_version", "");
	
	projects[cfg_path] = result;
	result.project_path = cfg_path;
	return result;

func save_propject(path : String, project : UtmxProject) -> void : 
	save_propject_config(path, project);

func remove_project(key : String) : 
	projects.erase(key);

## 从 ZIP 加载项目
func load_project_from_zip(zip_path : String, target_path : String) -> UtmxProject:
	if (!FileAccess.file_exists(zip_path)): return null;
	
	var reader : ZIPReader = ZIPReader.new()
	if (reader.open(zip_path) != OK): return null;
	
	var files : PackedStringArray = reader.get_files();
	var has_config : bool = false
	for file_path : String in files:
		if (file_path.get_file() == PROJECT_CONFIG_FILE_NAME):
			has_config = true;
			break;
	if (!has_config):
		reader.close();
		return null;
	
	if (!DirAccess.dir_exists_absolute(target_path)):
		DirAccess.make_dir_recursive_absolute(target_path);
	
	for file_path : String in files:
		var full_path : String = target_path.path_join(file_path);
		if (file_path.ends_with("/")):
			DirAccess.make_dir_recursive_absolute(full_path);
			continue;
		var parent_dir : String = full_path.get_base_dir();
		if (!DirAccess.dir_exists_absolute(parent_dir)):
			DirAccess.make_dir_recursive_absolute(parent_dir);
		var buffer : PackedByteArray = reader.read_file(file_path);
		var file : FileAccess = FileAccess.open(full_path, FileAccess.WRITE);
		if (file):
			file.store_buffer(buffer);
			file.close();
	reader.close();
	var project : UtmxProject = load_project(target_path);
	if (project != null):
		save_editor_project_config();
		return project;
	return null;

	

func open_project(project: UtmxProject) : 
	project;
