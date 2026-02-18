extends Tree
# EditorFileSystemTree.gd

## 双击一个文件时触发
signal selected_file(path: String);

@onready var context_menu : PopupMenu = %PopupFileSelectMenu as PopupMenu;
@onready var popup_multi_select_menu : PopupMenu = %PopupMultiSelectMenu as PopupMenu;

var _selected_item : TreeItem = null;
var _expanded_paths : Array[String] = []; 
var _all_items_cache : Dictionary = {};
var _clipboard_paths : Array[String] = [];
var _is_cut_mode : bool = false;
var _last_search_text : String = "";
var _is_refreshing_tree : bool = false;
var _pending_expand_paths : Array[String] = [];
 
enum MenuID {
	CREATE_FOLDER = 0,
	CREATE_SCRIPT = 101,
	CREATE_TSCN = 102,
	CREATE_TEXT = 103,
	CREATE_SHADER = 104,
	DELETE = 1, 
	RENAME = 2, 
	OPEN_IN_EXPLORER = 3, 
	OPEN_EXTERNAL = 4, 
	COPY = 5, 
	CUT = 6, 
	PASTE = 7,
	NEW_SUBMENU = 99
};

const JS_SCRIPT_TEMPLATE : String = """import * as UTMX from "utmx";

// TODO: write your game logic with UTMX.
"""

const GDSHADER_TEMPLATE : String = """shader_type canvas_item;

void vertex() {
}

void fragment() {
}
"""

const FILE_TEMPLATES : Dictionary = {
	MenuID.CREATE_FOLDER: {"name": "new folder", "is_dir": true},
	MenuID.CREATE_SCRIPT: {"name": "new_script.js", "is_dir": false, "template": JS_SCRIPT_TEMPLATE},
	MenuID.CREATE_TSCN: {"name": "new_scene.tscn", "is_dir": false},
	MenuID.CREATE_TEXT: {"name": "new_text.txt", "is_dir": false},
	MenuID.CREATE_SHADER: {"name": "new_shader.gdshader", "is_dir": false, "template": GDSHADER_TEMPLATE},
};

const HIDDEN_ROOT_FILES : Dictionary = {
	"utmx.cfg": true,
};

# --- 初始化逻辑 ---

func _ready() -> void:
	GlobalEditorFileSystem.filesystem_changed.connect(refresh_tree);
	GlobalEditorFileSystem.entry_removed.connect(_on_entry_removed_remotely);
	
	self.allow_rmb_select = true;
	self.select_mode = SELECT_MULTI;
	
	_setup_menu(context_menu, false);
	_setup_menu(popup_multi_select_menu, true);
	
	context_menu.id_pressed.connect(_on_menu_id_pressed);
	popup_multi_select_menu.id_pressed.connect(_on_menu_id_pressed);
	
	item_activated.connect(_on_item_activated);
	item_mouse_selected.connect(_on_item_mouse_selected);
	item_edited.connect(_on_item_edited);
	item_collapsed.connect(_on_item_collapsed);
	
	refresh_tree();

func _notification(what: int) -> void:
	if (what == NOTIFICATION_TRANSLATION_CHANGED):
		_setup_menu(context_menu, false);
		_setup_menu(popup_multi_select_menu, true);

func _exit_tree() -> void:
	GlobalEditorFileSystem.root_path = "";

func _setup_menu(menu: PopupMenu, is_multi: bool) -> void:
	if (menu == null): return;
	menu.clear();
	
	if (!is_multi):
		var submenu_new : PopupMenu = PopupMenu.new();
		submenu_new.name = "SubmenuNew";
		submenu_new.add_item(tr("Create Folder"), MenuID.CREATE_FOLDER);
		submenu_new.add_separator("");
		submenu_new.add_item(tr("Create Text File"), MenuID.CREATE_TEXT);
		submenu_new.add_item(tr("Create Shader"), MenuID.CREATE_SHADER);
		submenu_new.add_item(tr("Create Script"), MenuID.CREATE_SCRIPT);
		submenu_new.add_item(tr("Create Scene"), MenuID.CREATE_TSCN);
		submenu_new.id_pressed.connect(_on_menu_id_pressed);
		menu.add_child(submenu_new);
		menu.add_submenu_node_item(tr("Create..."), submenu_new, MenuID.NEW_SUBMENU);
		menu.add_item(tr("Rename (F2)"), MenuID.RENAME);
		menu.add_separator();
	
	menu.add_item(tr("Copy (Ctrl+C)"), MenuID.COPY);
	menu.add_item(tr("Cut (Ctrl+X)"), MenuID.CUT);
	menu.add_item(tr("Paste (Ctrl+V)"), MenuID.PASTE);
	menu.add_separator();
	menu.add_item(tr("Delete (Delete)"), MenuID.DELETE);
	
	if (!is_multi):
		menu.add_separator();
		menu.add_item(tr("Show in File Manager"), MenuID.OPEN_IN_EXPLORER);
		menu.add_item(tr("Open Externally"), MenuID.OPEN_EXTERNAL);

#region --- 状态维护与扫描 ---

func _on_entry_removed_remotely(path: String) -> void:
	if (_all_items_cache.has(path)):
		var item = _all_items_cache[path];
		if (is_instance_valid(item)):
			item.free();
		_all_items_cache.erase(path);

func _on_item_collapsed(item: TreeItem) -> void:
	if (_is_refreshing_tree): return;
	if (!_last_search_text.is_empty()): return;
	_sync_expanded_state_for_item(item);

func _sync_expanded_state_for_item(item: TreeItem) -> void:
	var data : Dictionary = item.get_metadata(0);
	if (data == null || !data.is_dir): return;
	var path : String = data.path;
	if (item.collapsed):
		if (_expanded_paths.has(path)): _expanded_paths.erase(path);
	else:
		if (!_expanded_paths.has(path)): _expanded_paths.append(path);

func _snapshot_expanded_paths_from_tree() -> void:
	var root : TreeItem = get_root();
	if (root == null): return;
	_expanded_paths.clear();
	_save_expanded_state(root);

func _queue_expand_path_and_parents(path: String) -> void:
	if (path.is_empty()): return;
	var curr : String = path;
	while (!curr.is_empty()):
		if (!_expanded_paths.has(curr)): _expanded_paths.append(curr);
		if (!_pending_expand_paths.has(curr)): _pending_expand_paths.append(curr);
		if (curr == GlobalEditorFileSystem.root_path): break;
		var parent : String = curr.get_base_dir();
		if (parent == curr): break;
		curr = parent;

func ensure_directory_expanded(path: String) -> void:
	if (path.is_empty()): return;
	var normalized_path : String = String(path).replace("\\", "/").simplify_path();
	_queue_expand_path_and_parents(normalized_path);
	_apply_pending_expand_paths();

func _apply_pending_expand_paths() -> void:
	if (_pending_expand_paths.is_empty()): return;
	var remaining : Array[String] = [];
	for p : String in _pending_expand_paths:
		if (!_all_items_cache.has(p)):
			remaining.append(p);
			continue;
		var item : TreeItem = _all_items_cache[p];
		if (item == null || !is_instance_valid(item)):
			remaining.append(p);
			continue;
		item.collapsed = false;
		_sync_expanded_state_for_item(item);
	_pending_expand_paths = remaining;

func refresh_tree(target_path: String = "") -> void:
	_snapshot_expanded_paths_from_tree();
	_is_refreshing_tree = true;
	if (!target_path.is_empty() && _all_items_cache.has(target_path)):
		var item : TreeItem = _all_items_cache[target_path];
		var data : Dictionary = item.get_metadata(0);
		if (data != null && data.is_dir):
			_update_local_directory(target_path, item);
			_apply_pending_expand_paths();
			_is_refreshing_tree = false;
			return;
	_rebuild_full_tree();
	_apply_pending_expand_paths();
	_is_refreshing_tree = false;

func _rebuild_full_tree() -> void:
	_snapshot_expanded_paths_from_tree();
	
	var saved_selected : Array[String] = [];
	var it : TreeItem = get_next_selected(null);
	while (it != null):
		saved_selected.append(it.get_metadata(0).path);
		it = get_next_selected(it);
	
	_all_items_cache.clear();
	clear();
	
	var root_path : String = GlobalEditorFileSystem.root_path;
	if (!DirAccess.dir_exists_absolute(root_path)): 
		DirAccess.make_dir_recursive_absolute(root_path);
		
	var root_item : TreeItem = create_item();
	var r_name : String = root_path.get_file() if (root_path.get_file() != "") else root_path;
	root_item.set_text(0, r_name);
	root_item.set_metadata(0, {"path": root_path, "is_dir": true});
	_all_items_cache[root_path] = root_item;
	root_item.collapsed = false; 
	
	_scan_folder(root_path, root_item);
	
	for p : String in saved_selected:
		if (_all_items_cache.has(p)): _all_items_cache[p].select(0);
	
	if (!_last_search_text.is_empty()): search(_last_search_text);

func _update_local_directory(path: String, parent_item: TreeItem) -> void:
	var should_expand_parent : bool = _expanded_paths.has(path) || !parent_item.collapsed;
	var child : TreeItem = parent_item.get_first_child();
	while (child != null):
		var c_path : String = child.get_metadata(0).path;
		_all_items_cache.erase(c_path);
		child.free();
		child = parent_item.get_first_child();
	_scan_folder(path, parent_item);
	parent_item.collapsed = !should_expand_parent;
	_sync_expanded_state_for_item(parent_item);
	if (!_last_search_text.is_empty()): search(_last_search_text);

func _save_expanded_state(item: TreeItem) -> void:
	if (item == null): return;
	var data : Dictionary = item.get_metadata(0);
	if (data.is_dir && !item.collapsed): _expanded_paths.append(data.path);
	var child : TreeItem = item.get_first_child();
	while (child != null):
		_save_expanded_state(child);
		child = child.get_next();

func _scan_folder(path: String, parent_item: TreeItem) -> void:
	var dir : DirAccess = DirAccess.open(path);
	if (dir != null):
		dir.list_dir_begin();
		var dirs : Array[String] = [];
		var files : Array[String] = [];
		var fn : String = dir.get_next();
		while (fn != ""):
			if (fn != "." && fn != ".."):
				var is_current_dir : bool = dir.current_is_dir();
				if (!_is_hidden_entry(path, fn, is_current_dir)):
					if (is_current_dir): dirs.append(fn);
					else: files.append(fn);
			fn = dir.get_next();
		dirs.sort_custom(func(a, b): return a.naturalnocasecmp_to(b) < 0);
		files.sort_custom(func(a, b): return a.naturalnocasecmp_to(b) < 0);
		for d : String in dirs:
			var fp : String = path.path_join(d);
			var item : TreeItem = create_item(parent_item);
			item.set_text(0, d);
			item.set_metadata(0, {"path": fp, "is_dir": true});
			item.collapsed = !(_expanded_paths.has(fp));
			_all_items_cache[fp] = item;
			_decorate_item(item);
			_scan_folder(fp, item);
		for f : String in files:
			var fp : String = path.path_join(f);
			var item : TreeItem = create_item(parent_item);
			item.set_text(0, f);
			item.set_metadata(0, {"path": fp, "is_dir": false});
			_all_items_cache[fp] = item;
			_decorate_item(item);

func _is_hidden_entry(parent_dir: String, entry_name: String, is_dir: bool) -> bool:
	if (is_dir): return false;
	var lower_name : String = entry_name.to_lower();
	if (!HIDDEN_ROOT_FILES.has(lower_name)): return false;
	var normalized_parent : String = String(parent_dir).replace("\\", "/").simplify_path();
	var normalized_root : String = String(GlobalEditorFileSystem.root_path).replace("\\", "/").simplify_path();
	return normalized_parent == normalized_root;

func _decorate_item(item: TreeItem) -> void:
	var data : Dictionary = item.get_metadata(0);
	if (data.is_dir): 
		item.set_icon(0, get_theme_icon("folder", "FileDialog")); 
		return; 
	var ext : String = data.path.get_extension().to_lower();
	if (ext in ["png", "jpg", "jpeg", "svg", "webp"]):
		var res : Resource = GlobalEditorResourceLoader.load_resource(data.path);
		if (res != null && res is Texture): 
			var img : Image = res.get_image();
			img.resize(16, 16, Image.INTERPOLATE_LANCZOS);
			item.set_icon(0, ImageTexture.create_from_image(img));
	elif (ext == "gd"): 
		item.set_icon(0, get_theme_icon("GDScript", "EditorIcons")); 
	elif (ext == "tscn"): 
		item.set_icon(0, get_theme_icon("PackedScene", "EditorIcons")); 
	else: 
		item.set_icon(0, get_theme_icon("file", "FileDialog")); 

func _extract_external_drop_paths(data: Variant) -> Array[String]:
	var paths : Array[String] = [];
	if (data is PackedStringArray):
		for p in data: paths.append(String(p));
		return paths;
	if (data is Array):
		for p in data:
			if (p is String): paths.append(p);
		return paths;
	if (data is Dictionary && data.has("files")):
		var files = data.files;
		if (files is PackedStringArray):
			for p in files: paths.append(String(p));
		elif (files is Array):
			for p in files:
				if (p is String): paths.append(p);
	return paths;

func _resolve_drop_target_dir(pos: Vector2) -> String:
	var target : TreeItem = get_item_at_position(pos);
	if (target != null):
		var data : Dictionary = target.get_metadata(0);
		if (data != null):
			if (data.is_dir):
				return data.path;
			return String(data.path).get_base_dir();
	return get_import_target_dir();

#endregion

#region --- 命令执行 ---

func _execute_command(id: int, prefer_context_target: bool = false) -> void:
	var selected_items : Array[TreeItem] = [];
	var it : TreeItem = get_next_selected(null);
	while (it != null):
		selected_items.append(it);
		it = get_next_selected(it);
	if (selected_items.is_empty() && _selected_item != null): selected_items.append(_selected_item);
	
	var target_item : TreeItem = null;
	if (prefer_context_target && _selected_item != null && is_instance_valid(_selected_item)):
		target_item = _selected_item;
	else:
		target_item = selected_items[0] if !selected_items.is_empty() else get_root();
	if (target_item == null): return;

	match (id):
		MenuID.CREATE_FOLDER, MenuID.CREATE_SCRIPT, MenuID.CREATE_TSCN, MenuID.CREATE_TEXT, MenuID.CREATE_SHADER:
			var config : Dictionary = FILE_TEMPLATES[id];
			var data : Dictionary = target_item.get_metadata(0);
			target_item.collapsed = false;
			_sync_expanded_state_for_item(target_item);
			var base_dir : String = data.path if data.is_dir else data.path.get_base_dir();
			_queue_expand_path_and_parents(base_dir);
			var final_path : String = GlobalEditorFileSystem.get_safe_move_path(base_dir, config.name);
			var template_text : String = str(config.get("template", ""));
			GlobalEditorFileSystem.execute_create_action(config.is_dir, final_path, template_text);
			
			await get_tree().process_frame;
			_queue_expand_path_and_parents(base_dir);
			_apply_pending_expand_paths();
			if (_all_items_cache.has(final_path)):
				var ni : TreeItem = _all_items_cache[final_path];
				deselect_all(); ni.select(0); scroll_to_item(ni); edit_item(ni);
		
		MenuID.DELETE:
			var paths_to_delete : Array[String] = [];
			for item : TreeItem in selected_items:
				var p : String = item.get_metadata(0).path;
				if (p != GlobalEditorFileSystem.root_path): paths_to_delete.append(p);
			if (paths_to_delete.is_empty()): return;
			
			WindowManager.open_confirmation_window(tr("Delete Files"), tr("Delete %d selected item(s)?") % paths_to_delete.size(), 
				func(confirmed): if (confirmed): GlobalEditorFileSystem.execute_delete_batch(paths_to_delete)
			);

		MenuID.RENAME: edit_item(target_item);
		
		MenuID.COPY, MenuID.CUT:
			_clipboard_paths.clear();
			for item : TreeItem in selected_items: _clipboard_paths.append(item.get_metadata(0).path);
			_is_cut_mode = (id == MenuID.CUT);

		MenuID.PASTE:
			if (_clipboard_paths.is_empty()): return;
			var target_data : Dictionary = target_item.get_metadata(0);
			var dest_dir : String = target_data.path if target_data.is_dir else target_data.path.get_base_dir();
			
			if (_is_cut_mode):
				var actual_move_paths : Array[String] = [];
				var conflicts = [];
				for src in _clipboard_paths:
					var dst = dest_dir.path_join(src.get_file());
					if (src == dst): continue;
					if (dest_dir.begins_with(src + "/")): continue;
					actual_move_paths.append(src);
					if (GlobalEditorFileSystem.entry_exists(dst)):
						conflicts.append(src.get_file());
				
				if (actual_move_paths.is_empty()): return;
				if (conflicts.size() > 0):
					WindowManager.open_confirmation_window(tr("Overwrite Confirmation"), tr("The pasted file already exists. Overwrite?"), func(confirmed):
						if (confirmed):
							GlobalEditorFileSystem.execute_move_batch_undoable(actual_move_paths, dest_dir, tr("Cut and Paste"));
							_clipboard_paths.clear();
					);
				else:
					GlobalEditorFileSystem.execute_move_batch_undoable(actual_move_paths, dest_dir, tr("Cut and Paste"));
					_clipboard_paths.clear();
			else:
				GlobalEditorFileSystem.execute_paste_batch_copy(_clipboard_paths, dest_dir);

		MenuID.OPEN_IN_EXPLORER:
			var p : String = ProjectSettings.globalize_path(target_item.get_metadata(0).path);
			OS.shell_show_in_file_manager(p if target_item.get_metadata(0).is_dir else p.get_base_dir());
		
		MenuID.OPEN_EXTERNAL:
			OS.shell_open(ProjectSettings.globalize_path(target_item.get_metadata(0).path));

#endregion

#region --- 事件与拖拽 ---

func _on_item_mouse_selected(pos: Vector2, mouse_button_index: int) -> void:
	if (mouse_button_index == MOUSE_BUTTON_RIGHT):
		var hovered : TreeItem = get_item_at_position(pos);
		if (hovered == null): return;
		var selected_count : int = 0;
		var it : TreeItem = get_next_selected(null);
		var is_hovered_selected : bool = false;
		while (it != null):
			selected_count += 1;
			if (it == hovered): is_hovered_selected = true;
			it = get_next_selected(it);
		if (!is_hovered_selected): 
			deselect_all(); 
			hovered.select(0); 
			selected_count = 1; 
		_selected_item = hovered;
		var menu : PopupMenu = popup_multi_select_menu if (selected_count > 1) else context_menu;
		var p_idx : int = menu.get_item_index(MenuID.PASTE);
		if (p_idx != -1): menu.set_item_disabled(p_idx, _clipboard_paths.is_empty());
		menu.position = Vector2i(get_viewport().get_mouse_position()) + get_window().position;
		menu.popup();

func _on_item_edited() -> void:
	var item : TreeItem = get_edited();
	var old_p : String = item.get_metadata(0).path;
	var new_name : String = item.get_text(0).strip_edges();
	var new_p : String = old_p.get_base_dir().path_join(new_name);

	if (new_name.is_empty() || old_p == new_p):
		item.set_text(0, old_p.get_file()); 
		return; 

	if (GlobalEditorFileSystem.entry_exists(new_p)):
		WindowManager.open_confirmation_window(tr("Overwrite Confirmation"), tr("Target item already exists. Overwrite?"), func(confirmed):
			if (confirmed): GlobalEditorFileSystem.execute_rename_undoable(old_p, new_p);
			else: item.set_text(0, old_p.get_file());
		);
	else:
		GlobalEditorFileSystem.execute_rename_undoable(old_p, new_p);

func _get_drag_data(_pos: Vector2) -> Variant:
	var paths : Array[String] = [];
	var it : TreeItem = get_next_selected(null);
	while (it != null): 
		paths.append(it.get_metadata(0).path); 
		it = get_next_selected(it); 
	if (paths.is_empty()): return null;
	var preview : Label = Label.new(); 
	preview.text = tr("Move %d item(s)") % paths.size(); 
	set_drag_preview(preview);
	return {"paths": paths};

func _can_drop_data(_pos: Vector2, data: Variant) -> bool:
	var dest_dir := _resolve_drop_target_dir(_pos);
	if (dest_dir.is_empty()): return false;
	if (data is Dictionary && data.has("paths")):
		var can_move = false;
		for sp : String in data.paths:
			if (sp.get_base_dir() != dest_dir && sp != dest_dir && !dest_dir.begins_with(sp + "/")):
				can_move = true;
				break;
		return can_move;
	var external_paths := _extract_external_drop_paths(data);
	return !external_paths.is_empty();

func _drop_data(_pos: Vector2, data: Variant) -> void:
	var dest_dir := _resolve_drop_target_dir(_pos);
	if (dest_dir.is_empty()): return;
	var external_paths := _extract_external_drop_paths(data);
	if (!external_paths.is_empty()):
		var import_paths : Array[String] = [];
		for p in external_paths:
			var source_path : String = String(p).replace("\\", "/").simplify_path();
			if (source_path.is_empty()): continue;
			if (!DirAccess.dir_exists_absolute(source_path) && !FileAccess.file_exists(source_path)): continue;
			import_paths.append(source_path);
		if (import_paths.is_empty()): return;
		GlobalEditorFileSystem.execute_paste_batch_copy(import_paths, dest_dir);
		return;
	var conflicts = [];
	var move_paths : Array[String] = [];
	
	for src in data.paths:
		var dst = dest_dir.path_join(src.get_file());
		if (src == dst || dest_dir.begins_with(src + "/")): continue;
		move_paths.append(src);
		if (GlobalEditorFileSystem.entry_exists(dst)):
			conflicts.append(src.get_file());
	
	if (move_paths.is_empty()): return;
	if (conflicts.size() > 0):
		WindowManager.open_confirmation_window(tr("Overwrite Confirmation"), tr("Target directory has conflicts. Overwrite?"), func(confirmed):
			if (confirmed): GlobalEditorFileSystem.execute_move_batch_undoable(move_paths, dest_dir, tr("Drag and Drop Move"));
		);
	else:
		GlobalEditorFileSystem.execute_move_batch_undoable(move_paths, dest_dir, tr("Drag and Drop Move"));

#endregion

func _on_menu_id_pressed(id: int) -> void: _execute_command(id, true);

func _on_item_activated() -> void:
	var it : TreeItem = get_selected();
	if (it == null): return;
	var data : Dictionary = it.get_metadata(0);
	if (data.is_dir):
		it.collapsed = !it.collapsed;
		_sync_expanded_state_for_item(it);
	else:
		selected_file.emit(data.path);
		scroll_to_item(it);

func edit_item(item: TreeItem) -> void: 
	if (item == get_root()) : return;
	edit_selected(true);

func search(text: String) -> void:
	_last_search_text = text;
	if (text.is_empty()):
		for p : String in _all_items_cache:
			var item : TreeItem = _all_items_cache[p];
			if (item != null):
				item.visible = true;
				if (item.get_metadata(0).is_dir): item.collapsed = !(_expanded_paths.has(p));
		return;
	for p : String in _all_items_cache:
		var item : TreeItem = _all_items_cache[p];
		if (item != null): item.visible = false;
	var query : String = text.to_lower();
	for p : String in _all_items_cache:
		var item : TreeItem = _all_items_cache[p];
		if (item != null && query in item.get_text(0).to_lower()): _show_item_and_parents(item);

func _show_item_and_parents(item: TreeItem) -> void:
	var curr : TreeItem = item;
	while (curr != null):
		curr.visible = true;
		if (curr != item && curr.get_metadata(0).is_dir): curr.collapsed = false;
		curr = curr.get_parent();

func get_import_target_dir() -> String:
	var item : TreeItem = get_selected();
	if (item == null):
		item = _selected_item;
	if (item == null):
		return GlobalEditorFileSystem.root_path;

	var data : Dictionary = item.get_metadata(0);
	if (data == null):
		return GlobalEditorFileSystem.root_path;
	if (data.is_dir):
		return data.path;
	return String(data.path).get_base_dir();

func _gui_input(event: InputEvent) -> void:
	if (event is InputEventKey && event.is_pressed()):
		if (event.is_command_or_control_pressed()):
			match (event.keycode):
				KEY_C: _execute_command(MenuID.COPY); accept_event();
				KEY_X: _execute_command(MenuID.CUT); accept_event();
				KEY_V: _execute_command(MenuID.PASTE); accept_event();
		elif (event.keycode == KEY_F2): _execute_command(MenuID.RENAME); accept_event();
		elif (event.keycode == KEY_DELETE): _execute_command(MenuID.DELETE); accept_event();
