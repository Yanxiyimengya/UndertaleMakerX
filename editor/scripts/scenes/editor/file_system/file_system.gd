extends PanelContainer

signal selected_file(path: String)

@onready var file_system_tree: Tree = %FileSystemTree
@onready var search_line_edit: LineEdit = %SearchLineEdit

var _pre_search_expanded_paths: Array[String] = []
var _file_drop_source: Object = null


func _ready() -> void:
	search_line_edit.text_changed.connect(_on_search_text_changed)
	file_system_tree.selected_file.connect(selected_file.emit)
	_connect_file_drop_signal()


func _exit_tree() -> void:
	_disconnect_file_drop_signal()


func _notification(what: int) -> void:
	if what == NOTIFICATION_APPLICATION_FOCUS_IN:
		GlobalEditorFileSystem.scan_project_incremental()


func _on_search_text_changed(new_text: String) -> void:
	if !new_text.is_empty():
		if file_system_tree._last_search_text.is_empty():
			_pre_search_expanded_paths = file_system_tree._expanded_paths.duplicate()
	file_system_tree.search(new_text)
	if new_text.is_empty():
		file_system_tree._expanded_paths = _pre_search_expanded_paths.duplicate()
		file_system_tree.refresh_tree("")


func _on_files_dropped(files: PackedStringArray, _screen: int = 0) -> void:
	if files.is_empty():
		return
	if GlobalEditorFileSystem.root_path.is_empty():
		return

	var dest_dir: String = GlobalEditorFileSystem.root_path
	if file_system_tree != null && file_system_tree.has_method("get_import_target_dir"):
		dest_dir = file_system_tree.get_import_target_dir()
	if dest_dir.is_empty():
		dest_dir = GlobalEditorFileSystem.root_path

	var import_paths: Array[String] = []
	for p in files:
		var source_path: String = String(p).replace("\\", "/").simplify_path()
		if source_path.is_empty():
			continue
		if !DirAccess.dir_exists_absolute(source_path) && !FileAccess.file_exists(source_path):
			continue
		import_paths.append(source_path)

	if import_paths.is_empty():
		return
	if file_system_tree != null && file_system_tree.has_method("ensure_directory_expanded"):
		file_system_tree.call("ensure_directory_expanded", dest_dir)
	GlobalEditorFileSystem.execute_paste_batch_copy(import_paths, dest_dir)
	if file_system_tree != null && file_system_tree.has_method("ensure_directory_expanded"):
		file_system_tree.call("ensure_directory_expanded", dest_dir)


func _connect_file_drop_signal() -> void:
	_disconnect_file_drop_signal()
	var win := get_window()
	if win != null && win.has_signal("files_dropped"):
		if !win.is_connected("files_dropped", _on_files_dropped):
			win.connect("files_dropped", _on_files_dropped)
		_file_drop_source = win
		return
	if get_tree().has_signal("files_dropped"):
		if !get_tree().is_connected("files_dropped", _on_files_dropped):
			get_tree().connect("files_dropped", _on_files_dropped)
		_file_drop_source = get_tree()


func _disconnect_file_drop_signal() -> void:
	if _file_drop_source == null:
		return
	if _file_drop_source.has_signal("files_dropped"):
		if _file_drop_source.is_connected("files_dropped", _on_files_dropped):
			_file_drop_source.disconnect("files_dropped", _on_files_dropped)
	_file_drop_source = null
