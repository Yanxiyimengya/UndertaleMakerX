extends Node
# 单例名称: GlobalEditorFileSystem

## 信号：文件系统变更通知 (path 为变动的根目录)
signal filesystem_changed(path: String);
signal entry_removed(path: String);

## 缓存结构: { "path": { "is_dir": bool, "verified": bool } }
var _resource_cache: Dictionary = {};

var root_path: String;
var _last_disk_fingerprint: int = 0;
var trash_path: String = "";

# 状态锁
var _is_logic_processing: bool = false;

func _init() -> void:
	_refresh_trash_path();

func _enter_tree() -> void:
	_refresh_trash_path();

func _refresh_trash_path() -> void:
	var data_path: String = EditorConfigureManager.get_data_path();
	if (data_path.is_empty()):
		data_path = OS.get_executable_path().get_base_dir().path_join("editor_data");
	if (!DirAccess.dir_exists_absolute(data_path)):
		DirAccess.make_dir_recursive_absolute(data_path);
	trash_path = data_path.path_join(".undo_trash");
	if (!DirAccess.dir_exists_absolute(trash_path)):
		DirAccess.make_dir_recursive_absolute(trash_path);

#region 外部变更监控

func _notification(what: int) -> void:
	if (what == NOTIFICATION_APPLICATION_FOCUS_IN):
		_check_external_changes_manual();

func _check_external_changes_manual() -> void:
	if (root_path.is_empty() || _is_logic_processing): return;
	
	var current_fp: int = _generate_fingerprint(root_path);
	if (_last_disk_fingerprint != 0 && current_fp != _last_disk_fingerprint):
		scan_project_incremental(); # 确保此处调用的名称正确
	_last_disk_fingerprint = current_fp;

## 快速指纹生成
func _generate_fingerprint(path: String) -> int:
	var dir = DirAccess.open(path);
	if (!dir): return 0;
	return path.hash() + FileAccess.get_modified_time(path);

# --- 增量扫描核心逻辑 ---

## 扫描项目：如果提供 target_path，则仅更新该目录及其子项
func scan_project_incremental(target_path: String = "") -> void:
	if (root_path.is_empty()): return;
	var scan_root = target_path if !target_path.is_empty() else root_path;
	
	_is_logic_processing = true;
	for path in _resource_cache:
		if (path.begins_with(scan_root)):
			_resource_cache[path]["verified"] = false;
	
	_scan_recursive_internal(scan_root);
	
	var dead_paths: Array = [];
	for path in _resource_cache:
		if (path.begins_with(scan_root) && !_resource_cache[path]["verified"]):
			dead_paths.append(path);
	
	for path in dead_paths:
		_resource_cache.erase(path);
		entry_removed.emit(path);
		
	_last_disk_fingerprint = _generate_fingerprint(root_path);
	filesystem_changed.emit(target_path); 
	_is_logic_processing = false;

func _scan_recursive_internal(path: String) -> void:
	var dir = DirAccess.open(path)
	if dir == null: return
	
	dir.list_dir_begin()
	var file_name = dir.get_next()
	var sub_dirs: Array[String] = []
	
	while file_name != "":
		if file_name != "." and file_name != "..":
			var full_path = path.path_join(file_name)
			var is_dir = dir.current_is_dir()
			
			if _resource_cache.has(full_path):
				_resource_cache[full_path]["verified"] = true
				_resource_cache[full_path]["is_dir"] = is_dir
			else:
				_resource_cache[full_path] = {
					"is_dir": is_dir,
					"verified": true
				}
			if is_dir:
				sub_dirs.append(full_path)
		file_name = dir.get_next()
	dir.list_dir_end()
	
	for sub_path in sub_dirs:
		_scan_recursive_internal(sub_path)

# --- 命令执行 (集成 UndoRedo) ---

func execute_move_batch_undoable(paths: Array[String], dest_dir: String, action_name: String = "移动项目") -> void:
	if (paths.is_empty()): return;
	
	GlobalEditorUndoRedoManager.create_action(action_name);
	var ur = GlobalEditorUndoRedoManager.history;
	var affected_parents: Dictionary = { dest_dir: true };
	
	for src in paths:
		if (!entry_exists(src)): continue;
		affected_parents[src.get_base_dir()] = true;
		
		var dst = dest_dir.path_join(src.get_file());
		if (entry_exists(dst) && src != dst):
			dst = get_safe_move_path(dest_dir, src.get_file());
		
		if (src == dst): continue;
		
		ur.add_do_method(_safe_rename_physical.bind(src, dst));
		ur.add_undo_method(_safe_rename_physical.bind(dst, src));
	
	for p in affected_parents.keys():
		ur.add_do_method(scan_project_incremental.bind(p));
		ur.add_undo_method(scan_project_incremental.bind(p));
	
	GlobalEditorUndoRedoManager.commit();

func execute_rename_undoable(old_path: String, new_path: String) -> void:
	if (old_path == new_path): return;
	if (entry_exists(new_path)):
		execute_merge_move_undoable(old_path, new_path);
		return;

	GlobalEditorUndoRedoManager.create_action("重命名项目");
	var ur = GlobalEditorUndoRedoManager.history;
	var parent_dir = old_path.get_base_dir();
	
	ur.add_do_method(_safe_rename_physical.bind(old_path, new_path));
	ur.add_undo_method(_safe_rename_physical.bind(new_path, old_path));
	ur.add_do_method(scan_project_incremental.bind(parent_dir));
	ur.add_undo_method(scan_project_incremental.bind(parent_dir));
	GlobalEditorUndoRedoManager.commit();

func execute_merge_move_undoable(src: String, dst: String) -> void:
	GlobalEditorUndoRedoManager.create_action("合并移动项目");
	var ur = GlobalEditorUndoRedoManager.history;
	_build_undoable_merge_recursive(src, dst, ur);
	
	var s_parent = src.get_base_dir();
	var d_parent = dst.get_base_dir();
	ur.add_do_method(scan_project_incremental.bind(s_parent));
	ur.add_do_method(scan_project_incremental.bind(d_parent));
	ur.add_undo_method(scan_project_incremental.bind(s_parent));
	ur.add_undo_method(scan_project_incremental.bind(d_parent));
	GlobalEditorUndoRedoManager.commit();

# --- 内部原子方法 ---

func _safe_rename_physical(f: String, t: String) -> void:
	if (f != t && entry_exists(f)):
		if (entry_exists(t)): _safe_remove_absolute(t);
		DirAccess.rename_absolute(f, t);

func _safe_remove_absolute(p: String) -> void:
	if (DirAccess.dir_exists_absolute(p)): _delete_dir_recursive(p);
	elif (FileAccess.file_exists(p)): DirAccess.remove_absolute(p);
	entry_removed.emit(p);

func _delete_dir_recursive(path: String) -> void:
	var dir = DirAccess.open(path);
	if (!dir): return;
	dir.list_dir_begin();
	var fn = dir.get_next();
	while (fn != ""):
		if (fn != "." && fn != ".."):
			var fp = path.path_join(fn);
			if (dir.current_is_dir()): _delete_dir_recursive(fp);
			else: DirAccess.remove_absolute(fp);
		fn = dir.get_next();
	DirAccess.remove_absolute(path);

func _build_undoable_merge_recursive(src: String, dst: String, ur: UndoRedo) -> void:
	if (!DirAccess.dir_exists_absolute(src)):
		if (FileAccess.file_exists(dst)):
			var backup = trash_path.path_join(str(Time.get_ticks_usec()) + "_" + dst.get_file());
			ur.add_do_method(_safe_rename_physical.bind(dst, backup));
			ur.add_undo_method(_safe_rename_physical.bind(backup, dst));
		ur.add_do_method(_safe_rename_physical.bind(src, dst));
		ur.add_undo_method(_safe_rename_physical.bind(dst, src));
	else:
		if (!DirAccess.dir_exists_absolute(dst)):
			ur.add_do_method(DirAccess.make_dir_recursive_absolute.bind(dst));
			ur.add_undo_method(_safe_remove_absolute.bind(dst));
		
		var dir = DirAccess.open(src);
		if (dir):
			dir.list_dir_begin();
			var fn = dir.get_next();
			while (fn != ""):
				if (fn != "." && fn != ".."):
					_build_undoable_merge_recursive(src.path_join(fn), dst.path_join(fn), ur);
				fn = dir.get_next();
			ur.add_do_method(_safe_remove_absolute.bind(src));
			ur.add_undo_method(DirAccess.make_dir_recursive_absolute.bind(src));

# --- 公共业务方法 ---

func execute_create_action(is_dir: bool, final_path: String, template_text: String = "") -> void:
	if (final_path.is_empty()):
		push_warning("execute_create_action: final_path is empty.");
		return;

	if (is_dir):
		var make_dir_err: int = DirAccess.make_dir_recursive_absolute(final_path);
		if (make_dir_err != OK && make_dir_err != ERR_ALREADY_EXISTS):
			push_warning("Failed to create directory: %s (error=%d)" % [final_path, make_dir_err]);
			return;
	else:
		var file: FileAccess = FileAccess.open(final_path, FileAccess.WRITE);
		if (file == null):
			var open_err: int = FileAccess.get_open_error();
			push_warning("Failed to create file: %s (error=%d)" % [final_path, open_err]);
			return;

		var content: String = template_text;
		if (content.is_empty() && final_path.get_extension().to_lower() == "tscn"):
			content = "[gd_scene format=3]\n\n[node name=\"Node\" type=\"Node\"]\n";
		if (!content.is_empty()):
			file.store_string(content);
			var write_err: int = file.get_error();
			if (write_err != OK):
				push_warning("Failed to write file: %s (error=%d)" % [final_path, write_err]);
				return;

	scan_project_incremental(final_path.get_base_dir());

func execute_delete_batch(paths: Array[String]) -> void:
	for p in paths: _safe_remove_absolute(p);
	scan_project_incremental();

func execute_paste_batch_copy(clipboard_paths: Array[String], dest_dir: String) -> void:
	for src in clipboard_paths:
		if (dest_dir == src || dest_dir.begins_with(src + "/")): continue;
		var dst = get_safe_move_path(dest_dir, src.get_file());
		_copy_recursive(src, dst);
	scan_project_incremental(dest_dir);

func _copy_recursive(from: String, to: String) -> void:
	if (DirAccess.dir_exists_absolute(from)):
		DirAccess.make_dir_recursive_absolute(to);
		var dir = DirAccess.open(from);
		if (dir):
			dir.list_dir_begin();
			var fn = dir.get_next();
			while (fn != ""):
				if (fn != "." && fn != ".."): _copy_recursive(from.path_join(fn), to.path_join(fn));
				fn = dir.get_next();
	else: DirAccess.copy_absolute(from, to);

func get_safe_move_path(dest: String, n: String) -> String:
	var base = n.get_basename();
	var ext = n.get_extension();
	var dot = "." if ext != "" else "";
	var path = dest.path_join(n);
	var count = 1;
	while (entry_exists(path)):
		path = dest.path_join(base + " (" + str(count) + ")" + dot + ext);
		count += 1;
	return path;

func entry_exists(path: String) -> bool:
	return _resource_cache.has(path)

func set_root_path(p: String) -> void:
	root_path = p
	GlobalEditorFileSystem.scan_project_incremental()

func clear_project_state() -> void:
	_resource_cache.clear()
	root_path = ""
	_last_disk_fingerprint = 0
	_is_logic_processing = false
