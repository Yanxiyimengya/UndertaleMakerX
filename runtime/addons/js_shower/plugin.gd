@tool  
extends EditorPlugin  

var already_has_setting : bool = false;
#var import_plugin : EditorImportPlugin = preload("res://addons/js_shower/js_import_plugin.gd").new();

func _enter_tree():  
	#add_import_plugin(import_plugin);
	var settings : EditorSettings = EditorInterface.get_editor_settings();
	var textfile_extensions : String = settings.get("docks/filesystem/textfile_extensions");
	var textfiles : PackedStringArray = textfile_extensions.split(",");
	if (textfiles.has("js")) : 
		already_has_setting = true;
		return;
	textfiles.append("js")
	settings.set("docks/filesystem/textfile_extensions", ",".join(textfiles));

func _exit_tree():  
	#remove_import_plugin(import_plugin)
	if (!already_has_setting) :
		var settings : EditorSettings = EditorInterface.get_editor_settings();
		var textfile_extensions : String = settings.get("docks/filesystem/textfile_extensions");
		var textfiles : PackedStringArray = textfile_extensions.split(",");
		if (textfiles.has("js")) : 
			textfiles.erase("js");
		settings.set("docks/filesystem/textfile_extensions", ",".join(textfiles));
