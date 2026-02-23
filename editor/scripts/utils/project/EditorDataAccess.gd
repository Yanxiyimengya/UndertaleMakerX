class_name EditorDataAccess extends RefCounted

var path: String = OS.get_executable_path().get_base_dir().path_join("editor_data")


func get_data_path() -> void:
	pass
