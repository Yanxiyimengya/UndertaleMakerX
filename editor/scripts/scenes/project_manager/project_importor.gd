extends Node

@onready var project_manager: PanelContainer = $"..";

func _on_import_button_pressed() -> void:
	DisplayServer.file_dialog_show("选择文件夹",
		EditorProjectManager.get_default_project_path(),
		"",
		false,
		DisplayServer.FILE_DIALOG_MODE_OPEN_ANY, 
		[EditorProjectManager.PROJECT_CONFIG_FILE_NAME + ",*.zip"],
		func(status: bool, selected_paths: PackedStringArray, _selected_filter_index: int) : 
			if (!status) : return;
			import(selected_paths[0]);
	);

func import(path : String) -> UtmxProject : 
	var extension : String = path.get_extension();
	var project : UtmxProject;
	if (extension == "cfg") : 
		project = EditorProjectManager.load_project(path);
	elif (extension == "zip") : 
		pass;
	if (project != null) :
		project_manager.add_project_list_item(project);
		project_manager.sort_project_item_list();
		#EditorProjectManager.open_project(project);
	return project;
