extends PanelContainer

signal resource_saved(path: String)

const COLUMN_NAME := 0
const COLUMN_VALUE := 1

const INI_FILE_EXTENSIONS := {
	"ini": true,
	"cfg": true,
	"conf": true,
	"properties": true,
}

const JSON_FILE_EXTENSIONS := {
	"json": true,
}

enum ResourceKind {
	NONE,
	INI,
	JSON,
}

@onready var file_name_label: Label = %FileNameLabel
@onready var filter_line_edit: LineEdit = %FilterLineEdit
@onready var property_tree: Tree = %PropertyTree

var _resource_kind: int = ResourceKind.NONE
var _current_path: String = ""
var _resource_data: Variant = null
var _saved_resource_data: Variant = null
var _is_dirty: bool = false
var _is_rebuilding_tree: bool = false


func _ready() -> void:
	_setup_property_tree()
	if not filter_line_edit.text_changed.is_connected(_on_filter_text_changed):
		filter_line_edit.text_changed.connect(_on_filter_text_changed)
	if not property_tree.item_edited.is_connected(_on_property_tree_item_edited):
		property_tree.item_edited.connect(_on_property_tree_item_edited)
	_update_status_ui()


func open_resource(path: String) -> bool:
	return load_from_disk(path)


func get_opened_resource_path() -> String:
	return _current_path


func load_from_disk(path: String) -> bool:
	var normalized_path := _normalize_path(path)
	if normalized_path.is_empty():
		return false
	if not FileAccess.file_exists(normalized_path):
		push_warning("Inspector file not found: %s" % normalized_path)
		return false

	var extension := normalized_path.get_extension().to_lower()
	var loaded := false
	if INI_FILE_EXTENSIONS.has(extension):
		loaded = _load_ini_file(normalized_path)
	elif JSON_FILE_EXTENSIONS.has(extension):
		loaded = _load_json_file(normalized_path)
	else:
		push_warning("Inspector unsupported file type: %s" % extension)
		return false

	if not loaded:
		return false

	_current_path = normalized_path
	_saved_resource_data = _duplicate_variant(_resource_data)
	filter_line_edit.text = ""
	_rebuild_property_tree()
	_refresh_dirty_state()
	return true


func save_resource() -> bool:
	if _current_path.is_empty() or _resource_kind == ResourceKind.NONE:
		return false

	var saved := false
	match _resource_kind:
		ResourceKind.INI:
			saved = _save_ini_file(_current_path)
		ResourceKind.JSON:
			saved = _save_json_file(_current_path)

	if not saved:
		return false

	_saved_resource_data = _duplicate_variant(_resource_data)
	_refresh_dirty_state()
	var refresh_path := _current_path.get_base_dir()
	if not refresh_path.is_empty():
		GlobalEditorFileSystem.scan_project_incremental(refresh_path)
	resource_saved.emit(_current_path)
	return true


func _unhandled_key_input(event: InputEvent) -> void:
	if not visible:
		return
	if event is InputEventKey and event.is_pressed() and not event.is_echo():
		if event.is_command_or_control_pressed() and event.keycode == KEY_S:
			if save_resource():
				get_viewport().set_input_as_handled()


func _setup_property_tree() -> void:
	property_tree.columns = 2
	property_tree.hide_root = true
	property_tree.select_mode = Tree.SELECT_SINGLE


func _load_ini_file(path: String) -> bool:
	var config := ConfigFile.new()
	var err := config.load(path)
	if err != OK:
		push_warning("Failed to parse ini file: %s (err=%d)" % [path, err])
		return false

	var root := {}
	for section in config.get_sections():
		var section_data := {}
		for key in config.get_section_keys(section):
			section_data[key] = config.get_value(section, key)
		root[section] = section_data

	_resource_data = root
	_resource_kind = ResourceKind.INI
	return true


func _load_json_file(path: String) -> bool:
	var file := FileAccess.open(path, FileAccess.READ)
	if file == null:
		push_warning("Failed to open json file: %s" % path)
		return false
	var content := file.get_as_text()

	var json := JSON.new()
	var err := json.parse(content)
	if err != OK:
		push_warning(
			(
				"Failed to parse json file: %s (line=%d, msg=%s)"
				% [path, json.get_error_line(), json.get_error_message()]
			)
		)
		return false

	_resource_data = json.data
	_resource_kind = ResourceKind.JSON
	return true


func _save_ini_file(path: String) -> bool:
	if not (_resource_data is Dictionary):
		push_warning("INI save failed because root data is not Dictionary: %s" % path)
		return false

	var config := ConfigFile.new()
	for section_key in _resource_data.keys():
		var section_name := str(section_key)
		var section_value: Variant = _resource_data[section_key]
		if section_value is Dictionary:
			for key in section_value.keys():
				config.set_value(section_name, str(key), section_value[key])
		else:
			config.set_value(section_name, "value", section_value)

	var err := config.save(path)
	if err != OK:
		push_warning("Failed to save ini file: %s (err=%d)" % [path, err])
		return false
	return true


func _save_json_file(path: String) -> bool:
	var file := FileAccess.open(path, FileAccess.WRITE)
	if file == null:
		push_warning("Failed to open json file for save: %s" % path)
		return false
	file.store_string(JSON.stringify(_resource_data, "\t"))
	file.store_string("\n")
	return true


func _rebuild_property_tree() -> void:
	_is_rebuilding_tree = true
	property_tree.clear()

	var root := property_tree.create_item()
	root.set_text(COLUMN_NAME, "__root__")

	if _resource_data is Dictionary:
		var dict_data: Dictionary = _resource_data
		var keys := dict_data.keys()
		keys.sort_custom(func(a, b): return str(a).naturalnocasecmp_to(str(b)) < 0)
		for key in keys:
			_build_property_item(root, str(key), dict_data[key], [key], 0)
	elif _resource_data is Array:
		var arr_data: Array = _resource_data
		for i in arr_data.size():
			_build_property_item(root, "[%d]" % i, arr_data[i], [i], 0)
	else:
		_build_property_item(root, "value", _resource_data, [], 0)

	_is_rebuilding_tree = false
	_apply_filter(filter_line_edit.text)


func _build_property_item(
	parent: TreeItem, display_name: String, value: Variant, path: Array, depth: int
) -> void:
	var item := property_tree.create_item(parent)
	var should_hide_json_subobject_name := (
		_resource_kind == ResourceKind.JSON and value is Dictionary and depth > 0
	)
	item.set_text(COLUMN_NAME, "" if should_hide_json_subobject_name else display_name)
	item.set_tooltip_text(COLUMN_NAME, _path_to_text(path))

	var value_type := typeof(value)
	(
		item
		. set_metadata(
			COLUMN_NAME,
			{
				"path": path,
				"value_type": value_type,
			}
		)
	)

	if value is Dictionary:
		item.set_text(COLUMN_VALUE, "")
		item.collapsed = depth > 0
		var dict_value: Dictionary = value
		var keys := dict_value.keys()
		keys.sort_custom(func(a, b): return str(a).naturalnocasecmp_to(str(b)) < 0)
		for key in keys:
			var child_path := path.duplicate()
			child_path.append(key)
			_build_property_item(item, str(key), dict_value[key], child_path, depth + 1)
		return

	if value is Array:
		item.set_text(COLUMN_VALUE, "" if _resource_kind == ResourceKind.JSON else "[...]")
		item.collapsed = depth > 0
		var array_value: Array = value
		for i in array_value.size():
			var child_path := path.duplicate()
			child_path.append(i)
			_build_property_item(item, "[%d]" % i, array_value[i], child_path, depth + 1)
		return

	_setup_leaf_item_editor(item, value, value_type)


func _setup_leaf_item_editor(item: TreeItem, value: Variant, value_type: int) -> void:
	match value_type:
		TYPE_BOOL:
			item.set_cell_mode(COLUMN_VALUE, TreeItem.CELL_MODE_CHECK)
			item.set_editable(COLUMN_VALUE, true)
			item.set_checked(COLUMN_VALUE, bool(value))
		TYPE_INT, TYPE_FLOAT, TYPE_STRING:
			item.set_editable(COLUMN_VALUE, true)
			item.set_text(COLUMN_VALUE, str(value))
		_:
			item.set_editable(COLUMN_VALUE, false)
			item.set_text(COLUMN_VALUE, str(value))


func _on_property_tree_item_edited() -> void:
	if _is_rebuilding_tree:
		return
	var item := property_tree.get_edited()
	if item == null:
		return

	var metadata: Variant = item.get_metadata(COLUMN_NAME)
	if not (metadata is Dictionary):
		return
	var info: Dictionary = metadata
	var path: Array = info.get("path", [])
	var value_type: int = int(info.get("value_type", TYPE_NIL))

	if value_type in [TYPE_DICTIONARY, TYPE_ARRAY, TYPE_NIL]:
		return

	var old_value: Variant = _get_value_at_path(path)
	var parse_result := _extract_item_value(item, value_type)
	if not bool(parse_result.get("success", false)):
		_restore_item_value(item, old_value, value_type)
		return

	var new_value: Variant = parse_result.get("value")
	if old_value == new_value:
		return

	_record_property_change(path, old_value, new_value)


func can_drop_property_data(pos: Vector2, data: Variant) -> bool:
	var item := property_tree.get_item_at_position(pos)
	if item == null:
		return false
	var info := _get_editable_string_item_info(item)
	if info.is_empty():
		return false
	return not _extract_drop_paths(data).is_empty()


func drop_property_data(pos: Vector2, data: Variant) -> void:
	var item := property_tree.get_item_at_position(pos)
	if item == null:
		return
	var info := _get_editable_string_item_info(item)
	if info.is_empty():
		return
	var drop_paths := _extract_drop_paths(data)
	if drop_paths.is_empty():
		return

	var dropped_path := _to_project_relative_path(drop_paths[0])
	var path: Array = info.get("path", [])
	var old_value: Variant = _get_value_at_path(path)
	if old_value == dropped_path:
		return

	_record_property_change(path, old_value, dropped_path)


func _extract_item_value(item: TreeItem, value_type: int) -> Dictionary:
	match value_type:
		TYPE_BOOL:
			return {"success": true, "value": item.is_checked(COLUMN_VALUE)}
		TYPE_INT:
			var int_text := item.get_text(COLUMN_VALUE).strip_edges()
			if int_text.is_valid_int():
				return {"success": true, "value": int(int_text)}
		TYPE_FLOAT:
			var float_text := item.get_text(COLUMN_VALUE).strip_edges()
			if float_text.is_valid_float():
				return {"success": true, "value": float(float_text)}
		TYPE_STRING:
			return {"success": true, "value": item.get_text(COLUMN_VALUE)}
	return {"success": false, "value": null}


func _get_editable_string_item_info(item: TreeItem) -> Dictionary:
	var metadata: Variant = item.get_metadata(COLUMN_NAME)
	if not (metadata is Dictionary):
		return {}
	var info: Dictionary = metadata
	var value_type: int = int(info.get("value_type", TYPE_NIL))
	if value_type != TYPE_STRING:
		return {}
	return info


func _extract_drop_paths(data: Variant) -> Array[String]:
	var paths: Array[String] = []
	if data is Dictionary and data.has("paths"):
		var dict_paths: Variant = data.get("paths")
		if dict_paths is PackedStringArray:
			for p in dict_paths:
				paths.append(String(p))
		elif dict_paths is Array:
			for p in dict_paths:
				if p is String:
					paths.append(String(p))
		return paths
	if data is PackedStringArray:
		for p in data:
			paths.append(String(p))
		return paths
	if data is Array:
		for p in data:
			if p is String:
				paths.append(String(p))
	return paths


func _record_property_change(path: Array, old_value: Variant, new_value: Variant) -> void:
	if old_value == new_value:
		return
	var target_path: Array = path.duplicate(true)
	GlobalEditorUndoRedoManager.create_action("Edit Property")
	var ur: UndoRedo = GlobalEditorUndoRedoManager.history
	ur.add_do_method(_apply_property_change.bind(target_path, new_value))
	ur.add_undo_method(_apply_property_change.bind(target_path, old_value))
	GlobalEditorUndoRedoManager.commit()


func _apply_property_change(path: Array, value: Variant) -> void:
	_set_value_at_path(path, value)
	var item := _find_item_by_path(path)
	if item != null:
		_apply_tree_item_value(item, value)
	_refresh_dirty_state()


func _apply_tree_item_value(item: TreeItem, value: Variant) -> void:
	var metadata: Variant = item.get_metadata(COLUMN_NAME)
	var value_type: int = TYPE_NIL
	if metadata is Dictionary:
		var info: Dictionary = metadata
		value_type = int(info.get("value_type", TYPE_NIL))

	match value_type:
		TYPE_BOOL:
			item.set_checked(COLUMN_VALUE, bool(value))
		TYPE_INT, TYPE_FLOAT, TYPE_STRING:
			item.set_text(COLUMN_VALUE, str(value))
		_:
			item.set_text(COLUMN_VALUE, str(value))


func _find_item_by_path(path: Array) -> TreeItem:
	var root := property_tree.get_root()
	if root == null:
		return null
	var child := root.get_first_child()
	while child != null:
		var found := _find_item_by_path_recursive(child, path)
		if found != null:
			return found
		child = child.get_next()
	return null


func _find_item_by_path_recursive(item: TreeItem, path: Array) -> TreeItem:
	var metadata: Variant = item.get_metadata(COLUMN_NAME)
	if metadata is Dictionary:
		var info: Dictionary = metadata
		var item_path: Array = info.get("path", [])
		if item_path == path:
			return item

	var child := item.get_first_child()
	while child != null:
		var found := _find_item_by_path_recursive(child, path)
		if found != null:
			return found
		child = child.get_next()
	return null


func _restore_item_value(item: TreeItem, value: Variant, value_type: int) -> void:
	if value_type == TYPE_BOOL:
		item.set_checked(COLUMN_VALUE, bool(value))
	else:
		item.set_text(COLUMN_VALUE, str(value))


func _get_value_at_path(path: Array) -> Variant:
	if path.is_empty():
		return _resource_data

	var cursor: Variant = _resource_data
	for segment in path:
		if cursor is Dictionary:
			var dict_cursor: Dictionary = cursor
			if not dict_cursor.has(segment):
				return null
			cursor = dict_cursor[segment]
		elif cursor is Array:
			var array_cursor: Array = cursor
			var idx := int(segment)
			if idx < 0 or idx >= array_cursor.size():
				return null
			cursor = array_cursor[idx]
		else:
			return null
	return cursor


func _set_value_at_path(path: Array, value: Variant) -> void:
	if path.is_empty():
		_resource_data = value
		return

	var cursor: Variant = _resource_data
	for i in range(path.size() - 1):
		var segment = path[i]
		if cursor is Dictionary:
			var dict_cursor: Dictionary = cursor
			cursor = dict_cursor.get(segment)
		elif cursor is Array:
			var array_cursor: Array = cursor
			var idx := int(segment)
			if idx < 0 or idx >= array_cursor.size():
				return
			cursor = array_cursor[idx]
		else:
			return

	var last_segment = path[path.size() - 1]
	if cursor is Dictionary:
		var dict_target: Dictionary = cursor
		dict_target[last_segment] = value
	elif cursor is Array:
		var array_target: Array = cursor
		var last_idx := int(last_segment)
		if last_idx >= 0 and last_idx < array_target.size():
			array_target[last_idx] = value


func _on_filter_text_changed(new_text: String) -> void:
	_apply_filter(new_text)


func _apply_filter(text: String) -> void:
	var root := property_tree.get_root()
	if root == null:
		return

	var query := text.strip_edges().to_lower()
	if query.is_empty():
		_set_item_visibility_recursive(root, true)
		return

	_filter_item_recursive(root, query)


func _set_item_visibility_recursive(item: TreeItem, visible: bool) -> void:
	item.visible = visible
	var child := item.get_first_child()
	while child != null:
		_set_item_visibility_recursive(child, visible)
		child = child.get_next()


func _filter_item_recursive(item: TreeItem, query: String) -> bool:
	var has_visible_child := false
	var child := item.get_first_child()
	while child != null:
		if _filter_item_recursive(child, query):
			has_visible_child = true
		child = child.get_next()

	var root := property_tree.get_root()
	var self_match := false
	if item != root:
		var key_text := item.get_text(COLUMN_NAME).to_lower()
		var value_text := item.get_text(COLUMN_VALUE).to_lower()
		self_match = key_text.contains(query) or value_text.contains(query)

	var _visible := item == root or self_match or has_visible_child
	item.visible = _visible
	if has_visible_child:
		item.collapsed = false
	return _visible


func _update_status_ui() -> void:
	if _current_path.is_empty():
		file_name_label.text = tr("No resource loaded")
		return

	var file_name := _current_path.get_file()
	if file_name.is_empty():
		file_name = _current_path
	if _is_dirty:
		file_name += "(*)"
	file_name_label.text = file_name


func _refresh_dirty_state() -> void:
	_is_dirty = not _variant_deep_equal(_resource_data, _saved_resource_data)
	_update_status_ui()


func _variant_deep_equal(a: Variant, b: Variant) -> bool:
	if typeof(a) != typeof(b):
		return false

	if a is Dictionary:
		var dict_a: Dictionary = a
		var dict_b: Dictionary = b
		if dict_a.size() != dict_b.size():
			return false
		for key in dict_a.keys():
			if not dict_b.has(key):
				return false
			if not _variant_deep_equal(dict_a[key], dict_b[key]):
				return false
		return true

	if a is Array:
		var arr_a: Array = a
		var arr_b: Array = b
		if arr_a.size() != arr_b.size():
			return false
		for i in arr_a.size():
			if not _variant_deep_equal(arr_a[i], arr_b[i]):
				return false
		return true

	return a == b


func _duplicate_variant(value: Variant) -> Variant:
	if value is Dictionary:
		var src_dict: Dictionary = value
		var copied_dict := {}
		for key in src_dict.keys():
			copied_dict[key] = _duplicate_variant(src_dict[key])
		return copied_dict

	if value is Array:
		var src_array: Array = value
		var copied_array: Array = []
		for elem in src_array:
			copied_array.append(_duplicate_variant(elem))
		return copied_array

	return value


func _path_to_text(path: Array) -> String:
	if path.is_empty():
		return "/"
	var parts: Array[String] = []
	for segment in path:
		if segment is int:
			parts.append("[%d]" % int(segment))
		else:
			parts.append(str(segment))
	return "/".join(PackedStringArray(parts))


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()


func _to_project_relative_path(path: String) -> String:
	var normalized_path := _normalize_path(path)
	var project_root := _normalize_path(EditorProjectManager.get_opened_project_path())
	if project_root.is_empty():
		return normalized_path
	if normalized_path == project_root:
		return "."
	if not project_root.ends_with("/"):
		project_root += "/"
	if normalized_path.begins_with(project_root):
		return normalized_path.trim_prefix(project_root)
	return normalized_path
