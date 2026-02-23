extends PanelContainer

const TREE_COLUMN := 0

signal script_saved(path: String)

@onready var java_script_code_edit: CodeEdit = %JavaScriptCodeEdit
@onready var file_list_tree: Tree = %FileListTree

var _file_cache: Dictionary = {}
var _open_order: Array[String] = []
var _current_path := ""
var _tree_root: TreeItem = null
var _is_switching_document := false
var _is_restoring_document_state := false
var _document_restore_version := 0


func _ready() -> void:
	_setup_file_tree()
	if not java_script_code_edit.text_changed.is_connected(_on_code_edit_text_changed):
		java_script_code_edit.text_changed.connect(_on_code_edit_text_changed)
	if not java_script_code_edit.gui_input.is_connected(_on_code_edit_gui_input):
		java_script_code_edit.gui_input.connect(_on_code_edit_gui_input)
	if not file_list_tree.item_activated.is_connected(_on_file_list_item_activated):
		file_list_tree.item_activated.connect(_on_file_list_item_activated)
	if not file_list_tree.item_mouse_selected.is_connected(_on_file_list_item_mouse_selected):
		file_list_tree.item_mouse_selected.connect(_on_file_list_item_mouse_selected)
	if not file_list_tree.gui_input.is_connected(_on_file_list_tree_gui_input):
		file_list_tree.gui_input.connect(_on_file_list_tree_gui_input)
	if not GlobalEditorFileSystem.entry_removed.is_connected(_on_filesystem_entry_removed):
		GlobalEditorFileSystem.entry_removed.connect(_on_filesystem_entry_removed)
	if not GlobalEditorFileSystem.filesystem_changed.is_connected(_on_filesystem_changed):
		GlobalEditorFileSystem.filesystem_changed.connect(_on_filesystem_changed)


func _exit_tree() -> void:
	if GlobalEditorFileSystem.entry_removed.is_connected(_on_filesystem_entry_removed):
		GlobalEditorFileSystem.entry_removed.disconnect(_on_filesystem_entry_removed)
	if GlobalEditorFileSystem.filesystem_changed.is_connected(_on_filesystem_changed):
		GlobalEditorFileSystem.filesystem_changed.disconnect(_on_filesystem_changed)


func open_script(path: String) -> void:
	var target_path := _normalize_path(path.strip_edges())
	if target_path.is_empty():
		return

	if not _file_cache.has(target_path):
		var loaded_text := _read_script_file(target_path)
		var item := _create_file_tree_item(target_path)
		_file_cache[target_path] = {
			"path": target_path,
			"text": loaded_text,
			"saved_text": loaded_text,
			"dirty": false,
			"missing_on_disk": false,
			"cursor_line": 0,
			"cursor_column": 0,
			"item": item,
		}
		_open_order.append(target_path)
	else:
		var cached: Dictionary = _file_cache[target_path]
		if not cached.has("cursor_line"):
			cached["cursor_line"] = 0
		if not cached.has("cursor_column"):
			cached["cursor_column"] = 0
		if not cached.has("item") or not is_instance_valid(cached["item"]):
			cached["item"] = _create_file_tree_item(target_path)
		_file_cache[target_path] = cached

	_switch_to_script(target_path)


func sync_file_from_disk(path: String, force: bool = true) -> bool:
	var target_path: String = _resolve_open_path_key(path)
	if target_path.is_empty():
		return false
	if not _file_cache.has(target_path):
		return false

	var cached: Dictionary = _file_cache[target_path]
	var is_dirty: bool = bool(cached.get("dirty", false))
	if is_dirty and not force:
		return false

	var latest_text: String = _read_script_file(target_path)
	cached["text"] = latest_text
	cached["saved_text"] = latest_text
	cached["missing_on_disk"] = false
	cached["dirty"] = false
	_file_cache[target_path] = cached

	if _current_path == target_path:
		_is_switching_document = true
		java_script_code_edit.text = latest_text
		_is_switching_document = false
		_reset_code_edit_undo_history()
		_schedule_document_state_restore(target_path)
	_update_file_tree_item(target_path)
	return true


func save_all_open_scripts(skip_deleted: bool = false) -> void:
	_flush_current_document_state()
	var refresh_targets: Dictionary = {}
	var saved_paths: Array[String] = []
	for path in _open_order:
		if not _file_cache.has(path):
			continue
		var cached: Dictionary = _file_cache[path]
		if bool(cached.get("dirty", false)):
			if skip_deleted and bool(cached.get("missing_on_disk", false)):
				_update_file_tree_item(path)
				continue
			var target_text := str(cached.get("text", ""))
			if _write_script_file(path, target_text):
				cached["saved_text"] = target_text
				cached["missing_on_disk"] = false
				cached["dirty"] = false
				_file_cache[path] = cached
				refresh_targets[_get_refresh_target(path)] = true
				saved_paths.append(path)
		_update_file_tree_item(path)
	for target in refresh_targets.keys():
		_refresh_filesystem_incremental(str(target))
	for saved_path in saved_paths:
		script_saved.emit(saved_path)


func close_script(path: String) -> void:
	_request_close_script(path)


func _unhandled_key_input(event: InputEvent) -> void:
	if not visible:
		return
	if event is InputEventKey and event.is_pressed() and not event.is_echo():
		if event.is_command_or_control_pressed() and event.keycode == KEY_S:
			save_all_open_scripts()
			get_viewport().set_input_as_handled()


func _on_code_edit_gui_input(event: InputEvent) -> void:
	if event is InputEventKey and event.is_pressed() and not event.is_echo():
		if event.is_command_or_control_pressed() and event.keycode == KEY_S:
			save_all_open_scripts()
			java_script_code_edit.accept_event()


func _on_code_edit_text_changed() -> void:
	if _is_switching_document or _is_restoring_document_state:
		return
	if _current_path.is_empty() or not _file_cache.has(_current_path):
		return

	var cached: Dictionary = _file_cache[_current_path]
	var current_text := java_script_code_edit.text
	cached["text"] = current_text
	cached["dirty"] = _compute_dirty_state(cached, current_text)
	_file_cache[_current_path] = cached
	_update_file_tree_item(_current_path)


func _on_file_list_item_activated() -> void:
	var selected := file_list_tree.get_selected()
	if selected == null:
		return
	var path := str(selected.get_metadata(TREE_COLUMN))
	if path.is_empty():
		return
	_switch_to_script(path)


func _on_file_list_item_mouse_selected(pos: Vector2, mouse_button_index: int) -> void:
	if mouse_button_index != MOUSE_BUTTON_MIDDLE:
		return
	var item := file_list_tree.get_item_at_position(pos)
	if item == null:
		return
	var path := str(item.get_metadata(TREE_COLUMN))
	if path.is_empty():
		return
	_request_close_script(path)


func _on_file_list_tree_gui_input(event: InputEvent) -> void:
	if not (event is InputEventMouseButton):
		return
	var mouse_event := event as InputEventMouseButton
	if not mouse_event.pressed or mouse_event.button_index != MOUSE_BUTTON_MIDDLE:
		return
	var item := file_list_tree.get_item_at_position(mouse_event.position)
	if item == null:
		return
	var path := str(item.get_metadata(TREE_COLUMN))
	if path.is_empty():
		return
	_request_close_script(path)
	file_list_tree.accept_event()


func _switch_to_script(path: String) -> void:
	var target_path := _normalize_path(path)
	if not _file_cache.has(target_path):
		return
	if target_path == _current_path:
		_select_file_tree_item(target_path)
		_update_file_tree_item(target_path)
		java_script_code_edit.grab_focus()
		return
	_flush_current_document_state()
	_current_path = target_path

	var cached: Dictionary = _file_cache[target_path]
	_is_switching_document = true
	java_script_code_edit.text = str(cached.get("text", ""))
	_is_switching_document = false
	_reset_code_edit_undo_history()

	_select_file_tree_item(target_path)
	_update_file_tree_item(target_path)
	_schedule_document_state_restore(target_path)


func _request_close_script(path: String) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	if bool(cached.get("dirty", false)):
		var file_name := path.get_file()
		var close_message := tr("Script %s has unsaved changes. Save before closing?") % file_name
		if bool(cached.get("missing_on_disk", false)):
			close_message = (
				tr("Script %s was deleted from disk. Save before closing to restore it?")
				% file_name
			)
		WindowManager.open_question_window(
			tr("Close Script"),
			close_message,
			func(result: int) -> void:
				match result:
					1:
						if _save_script(path):
							_close_script_internal(path)
					0:
						_close_script_internal(path)
					2:
						pass
		)
	else:
		_close_script_internal(path)


func _close_script_internal(path: String) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	var tree_item: TreeItem = cached.get("item", null)
	if tree_item and is_instance_valid(tree_item):
		tree_item.free()

	_file_cache.erase(path)
	_open_order.erase(path)

	if _current_path == path:
		_current_path = ""
		if _open_order.is_empty():
			_is_switching_document = true
			java_script_code_edit.text = ""
			_is_switching_document = false
			_reset_code_edit_undo_history()
		else:
			_switch_to_script(_open_order.back())


func _save_script(path: String) -> bool:
	if not _file_cache.has(path):
		return false
	if path == _current_path:
		_flush_current_document_state()

	var cached: Dictionary = _file_cache[path]
	var target_text := str(cached.get("text", ""))
	if not _write_script_file(path, target_text):
		return false
	cached["saved_text"] = target_text
	cached["missing_on_disk"] = false
	cached["dirty"] = false
	_file_cache[path] = cached
	_refresh_filesystem_incremental(_get_refresh_target(path))
	_update_file_tree_item(path)
	script_saved.emit(path)
	return true


func _flush_current_document_state() -> void:
	if _current_path.is_empty() or not _file_cache.has(_current_path):
		return
	var cached: Dictionary = _file_cache[_current_path]
	var current_text := java_script_code_edit.text
	cached["text"] = current_text
	cached["dirty"] = _compute_dirty_state(cached, current_text)
	cached["cursor_line"] = java_script_code_edit.get_caret_line()
	cached["cursor_column"] = java_script_code_edit.get_caret_column()
	_file_cache[_current_path] = cached
	_update_file_tree_item(_current_path)


func _setup_file_tree() -> void:
	file_list_tree.columns = 1
	file_list_tree.hide_root = true
	file_list_tree.select_mode = Tree.SELECT_SINGLE
	file_list_tree.allow_rmb_select = true
	_ensure_file_tree_root()


func _ensure_file_tree_root() -> void:
	_tree_root = file_list_tree.get_root()
	if _tree_root == null:
		_tree_root = file_list_tree.create_item()
		_tree_root.set_text(TREE_COLUMN, "__open_scripts_root__")


func _create_file_tree_item(path: String) -> TreeItem:
	_ensure_file_tree_root()
	var item := file_list_tree.create_item(_tree_root)
	item.set_metadata(TREE_COLUMN, path)
	item.set_tooltip_text(TREE_COLUMN, path)
	item.set_selectable(TREE_COLUMN, true)
	var extension := path.get_extension().to_lower()
	if extension == "gd":
		item.set_icon(TREE_COLUMN, file_list_tree.get_theme_icon("GDScript", "EditorIcons"))
	elif extension == "cs":
		item.set_icon(TREE_COLUMN, file_list_tree.get_theme_icon("CSharpScript", "EditorIcons"))
	elif extension == "js":
		item.set_icon(TREE_COLUMN, file_list_tree.get_theme_icon("Script", "EditorIcons"))
	_update_file_tree_item(path, item)
	return item


func _update_file_tree_item(path: String, explicit_item: TreeItem = null) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	var item: TreeItem = explicit_item if explicit_item != null else cached.get("item", null)
	if item == null or not is_instance_valid(item):
		return
	var file_name := path.get_file()
	var text := file_name
	if bool(cached.get("missing_on_disk", false)):
		text += " [deleted]"
	if bool(cached.get("dirty", false)):
		text += "(*)"
	item.set_text(TREE_COLUMN, text)
	item.set_tooltip_text(TREE_COLUMN, path)
	item.set_metadata(TREE_COLUMN, path)


func _on_filesystem_entry_removed(removed_path: String) -> void:
	var target := removed_path.strip_edges()
	if target.is_empty():
		return
	for open_path in _open_order:
		if _is_path_affected_by_removed_entry(open_path, target):
			if _path_exists(open_path):
				_clear_missing_state(open_path)
				continue
			_mark_script_missing(open_path)


func _on_filesystem_changed(changed_path: String) -> void:
	var normalized_changed := _normalize_path(changed_path.strip_edges())
	var open_paths: Array[String] = _open_order.duplicate()
	for open_path in open_paths:
		if not _file_cache.has(open_path):
			continue

		if not _is_path_in_change_scope(open_path, normalized_changed):
			continue

		if _path_exists(open_path):
			_clear_missing_state(open_path)
			continue

		var cached: Dictionary = _file_cache[open_path]
		if not bool(cached.get("missing_on_disk", false)):
			continue

		var remapped_path := _try_resolve_renamed_path(open_path, normalized_changed)
		if remapped_path.is_empty():
			continue
		_remap_open_script_path(open_path, remapped_path)


func _mark_script_missing(path: String) -> void:
	if not _file_cache.has(path):
		return
	if _path_exists(path):
		_clear_missing_state(path)
		return
	var cached: Dictionary = _file_cache[path]
	if bool(cached.get("missing_on_disk", false)):
		return
	cached["missing_on_disk"] = true
	cached["dirty"] = true
	_file_cache[path] = cached
	_update_file_tree_item(path)


func _clear_missing_state(path: String) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	if not bool(cached.get("missing_on_disk", false)):
		return
	cached["missing_on_disk"] = false
	var current_text := str(cached.get("text", ""))
	cached["dirty"] = _compute_dirty_state(cached, current_text)
	_file_cache[path] = cached
	_update_file_tree_item(path)


func _is_path_affected_by_removed_entry(open_path: String, removed_path: String) -> bool:
	var normalized_open := _normalize_path(open_path)
	var normalized_removed := _normalize_path(removed_path)
	if normalized_open == normalized_removed:
		return true
	return normalized_open.begins_with(normalized_removed + "/")


func _is_path_in_change_scope(path: String, changed_root: String) -> bool:
	if changed_root.is_empty():
		return true
	var normalized_path := _normalize_path(path)
	if normalized_path == changed_root:
		return true
	if normalized_path.begins_with(changed_root + "/"):
		return true
	var normalized_parent := _normalize_path(path.get_base_dir())
	return normalized_parent == changed_root


func _try_resolve_renamed_path(old_path: String, changed_root: String) -> String:
	if not _file_cache.has(old_path):
		return ""
	var normalized_old_path := _normalize_path(old_path)
	var parent_dir := _normalize_path(old_path.get_base_dir())
	if parent_dir.is_empty() or not DirAccess.dir_exists_absolute(parent_dir):
		return ""
	if not changed_root.is_empty() and not _is_path_in_change_scope(old_path, changed_root):
		return ""

	var cached: Dictionary = _file_cache[old_path]
	var expected_saved_text := str(cached.get("saved_text", ""))
	var old_ext := normalized_old_path.get_extension().to_lower()

	var candidates: Array[String] = []
	var dir := DirAccess.open(parent_dir)
	if dir == null:
		return ""

	dir.list_dir_begin()
	var entry_name := dir.get_next()
	while entry_name != "":
		if entry_name != "." and entry_name != ".." and not dir.current_is_dir():
			var candidate_path := _normalize_path(parent_dir.path_join(entry_name))
			if candidate_path != normalized_old_path and not _file_cache.has(candidate_path):
				if old_ext.is_empty() or candidate_path.get_extension().to_lower() == old_ext:
					var candidate_text := _read_script_file(candidate_path)
					if candidate_text == expected_saved_text:
						candidates.append(candidate_path)
		entry_name = dir.get_next()
	dir.list_dir_end()

	if candidates.size() == 1:
		return candidates[0]
	return ""


func _remap_open_script_path(old_path: String, new_path: String) -> void:
	var normalized_old := _normalize_path(old_path)
	var normalized_new := _normalize_path(new_path)
	if normalized_old.is_empty() or normalized_new.is_empty():
		return
	if normalized_old == normalized_new:
		_clear_missing_state(normalized_old)
		return
	if not _file_cache.has(normalized_old):
		return
	if _file_cache.has(normalized_new):
		return

	var cached: Dictionary = _file_cache[normalized_old]
	_file_cache.erase(normalized_old)

	cached["path"] = normalized_new
	cached["missing_on_disk"] = false
	cached["dirty"] = _compute_dirty_state(cached, str(cached.get("text", "")))
	_file_cache[normalized_new] = cached

	var open_idx := _open_order.find(normalized_old)
	if open_idx != -1:
		_open_order[open_idx] = normalized_new

	if _current_path == normalized_old:
		_current_path = normalized_new

	_update_file_tree_item(normalized_new)


func _normalize_path(path: String) -> String:
	var normalized := String(path).replace("\\", "/").simplify_path()
	if normalized.ends_with("/"):
		normalized = normalized.trim_suffix("/")
	return normalized


func _path_exists(path: String) -> bool:
	var normalized_path := _normalize_path(path)
	return FileAccess.file_exists(normalized_path) or DirAccess.dir_exists_absolute(normalized_path)


func _restore_caret_for_path(cached: Dictionary) -> void:
	var line_count: int = maxi(java_script_code_edit.get_line_count(), 1)
	var line: int = clampi(int(cached.get("cursor_line", 0)), 0, line_count - 1)
	java_script_code_edit.set_caret_line(line)
	var line_text: String = java_script_code_edit.get_line(line)
	var column: int = clampi(int(cached.get("cursor_column", 0)), 0, line_text.length())
	java_script_code_edit.set_caret_column(column)


func _schedule_document_state_restore(path: String) -> void:
	var target_path := _normalize_path(path)
	if target_path.is_empty():
		return
	_document_restore_version += 1
	var restore_version: int = _document_restore_version
	call_deferred("_restore_document_state_for_path", target_path, restore_version)


func _restore_document_state_for_path(path: String, restore_version: int) -> void:
	var target_path := _normalize_path(path)
	if not is_inside_tree():
		return
	await get_tree().process_frame
	if restore_version != _document_restore_version:
		return
	if target_path != _current_path:
		return
	if not _file_cache.has(target_path):
		return

	_is_restoring_document_state = true
	_apply_cached_cursor_and_scroll(target_path)

	await get_tree().process_frame
	if restore_version == _document_restore_version:
		if target_path == _current_path and _file_cache.has(target_path):
			_apply_cached_cursor_and_scroll(target_path)
			java_script_code_edit.grab_focus()
	_is_restoring_document_state = false


func _apply_cached_cursor_and_scroll(path: String) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	_restore_caret_for_path(cached)
	var caret_line: int = java_script_code_edit.get_caret_line()
	_scroll_code_edit_to_caret(caret_line)


func _scroll_code_edit_to_caret(target_line: int) -> void:
	if java_script_code_edit == null:
		return
	if java_script_code_edit.has_method("center_viewport_to_caret"):
		java_script_code_edit.call("center_viewport_to_caret")
		return
	if java_script_code_edit.has_method("adjust_viewport_to_caret"):
		java_script_code_edit.call("adjust_viewport_to_caret")
		return
	if java_script_code_edit.has_method("set_line_as_first_visible"):
		var first_visible_line: int = maxi(target_line - 3, 0)
		java_script_code_edit.call("set_line_as_first_visible", first_visible_line)


func _compute_dirty_state(cached: Dictionary, current_text: String) -> bool:
	return (
		bool(cached.get("missing_on_disk", false))
		or current_text != str(cached.get("saved_text", ""))
	)


func _get_refresh_target(path: String) -> String:
	var target := path.get_base_dir()
	if target.is_empty():
		return path
	return target


func _refresh_filesystem_incremental(target_path: String) -> void:
	if target_path.is_empty():
		return
	GlobalEditorFileSystem.scan_project_incremental(target_path)


func _read_script_file(path: String) -> String:
	if not FileAccess.file_exists(path):
		push_warning("Script file not found: %s" % path)
		return ""
	var file := FileAccess.open(path, FileAccess.READ)
	if file == null:
		push_warning("Failed to open script for read: %s" % path)
		return ""
	return file.get_as_text()


func _write_script_file(path: String, content: String) -> bool:
	var base_dir := path.get_base_dir()
	if not base_dir.is_empty() and not DirAccess.dir_exists_absolute(base_dir):
		var make_dir_err := DirAccess.make_dir_recursive_absolute(base_dir)
		if make_dir_err != OK and not DirAccess.dir_exists_absolute(base_dir):
			push_warning("Failed to create script directory: %s" % base_dir)
			return false
	var file := FileAccess.open(path, FileAccess.WRITE)
	if file == null:
		push_warning("Failed to open script for write: %s" % path)
		return false
	file.store_string(content)
	file.close()
	return true


func _reset_code_edit_undo_history() -> void:
	if java_script_code_edit == null:
		return
	if java_script_code_edit.has_method("clear_undo_history"):
		java_script_code_edit.call("clear_undo_history")


func _select_file_tree_item(path: String) -> void:
	if not _file_cache.has(path):
		return
	var cached: Dictionary = _file_cache[path]
	var item: TreeItem = cached.get("item", null)
	if item == null or not is_instance_valid(item):
		return
	file_list_tree.deselect_all()
	item.select(TREE_COLUMN)
	file_list_tree.scroll_to_item(item)


func _resolve_open_path_key(path: String) -> String:
	if path.is_empty():
		return ""
	if _file_cache.has(path):
		return path

	var normalized_target: String = _normalize_path(path)
	for open_path in _open_order:
		if _normalize_path(open_path) == normalized_target:
			return open_path
	return ""
