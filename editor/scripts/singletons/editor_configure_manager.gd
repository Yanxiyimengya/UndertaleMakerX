extends Node;

static var data_path : String = "";
static var data_path_override : String = "C:/Users/guosh/Desktop/UndertaleMakerX";
const EDITOR_CONFIG_FILE_NAME : String = "editor.cfg";

var configs : Dictionary[String, Variant] = {};

func _enter_tree() -> void:
	if (data_path_override.is_empty()) : 
		data_path = OS.get_executable_path().get_base_dir().path_join("editor_data");
	else : 
		data_path = data_path_override;

func get_data_path() -> String : 
	return data_path;

func load_config() -> void: 
	var path : String = get_data_path().path_join(EDITOR_CONFIG_FILE_NAME);
	if (!FileAccess.file_exists(path)) : return;
	var config_file : ConfigFile = ConfigFile.new();
	config_file.load(path);

func save_config() -> void: 
	var folder_path : String = get_data_path();
	if (DirAccess.dir_exists_absolute(folder_path)) : 
		DirAccess.make_dir_recursive_absolute(folder_path);
	var path : String = folder_path.path_join(EDITOR_CONFIG_FILE_NAME);
	var config_file : ConfigFile = ConfigFile.new();
	config_file.save(path);
