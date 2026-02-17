extends Tree
# EditorFileSystemTree.gd

## 双击一个文件时触发
signal selected_file(path: String);

@export_dir var root_path : String = EditorProjectManager.get_default_project_path();
@onready var context_menu : PopupMenu = %PopupFileSelectMenu as PopupMenu;
@onready var popup_multi_select_menu : PopupMenu = %PopupMultiSelectMenu as PopupMenu;

var _selected_item : TreeItem = null;
var _expanded_paths : Array[String] = []; 
var _all_items_cache : Dictionary = {};
var _clipboard_paths : Array[String] = [];
var _is_cut_mode : bool = false;
var _last_search_text : String = "";

enum MenuID {
	CREATE_FOLDER = 0,
	CREATE_SCRIPT = 101,
	CREATE_TSCN = 102,
	DELETE = 1, 
	RENAME = 2, 
	OPEN_IN_EXPLORER = 3, 
	OPEN_EXTERNAL = 4, 
	COPY = 5, 
	CUT = 6, 
	PASTE = 7,
	NEW_SUBMENU = 99
};

const FILE_TEMPLATES : Dictionary = {
	MenuID.CREATE_FOLDER: {"name": "new folder", "is_dir": true},
	MenuID.CREATE_SCRIPT: {"name": "new_script.js", "is_dir": false},
};

# --- 初始化逻辑 ---

func _ready() -> void:
	GlobalEditorFileSystem.root_path = root_path;
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

func _setup_menu(menu: PopupMenu, is_multi: bool) -> void:
	if (menu == null): return;
	menu.clear();
	
	if (!is_multi):
		var submenu_new : PopupMenu = PopupMenu.new();
		submenu_new.name = "SubmenuNew";
		submenu_new.add_item("新建文件夹", MenuID.CREATE_FOLDER);
		submenu_new.add_item("新建脚本", MenuID.CREATE_SCRIPT);
		submenu_new.id_pressed.connect(_on_menu_id_pressed);
		menu.add_child(submenu_new);
		menu.add_submenu_node_item("新建...", submenu_new, MenuID.NEW_SUBMENU);
		menu.add_item("重命名 (F2)", MenuID.RENAME);
		menu.add_separator();
	
	menu.add_item("复制 (Ctrl+C)", MenuID.COPY);
	menu.add_item("剪切 (Ctrl+X)", MenuID.CUT);
	menu.add_item("粘贴 (Ctrl+V)", MenuID.PASTE);
	menu.add_separator();
	menu.add_item("删除 (Delete)", MenuID.DELETE);
	
	if (!is_multi):
		menu.add_separator();
		menu.add_item("在资源管理器中显示", MenuID.OPEN_IN_EXPLORER);
		menu.add_item("用外部程序打开", MenuID.OPEN_EXTERNAL);

#region --- 状态维护与扫描 ---

func _on_entry_removed_remotely(path: String) -> void:
	if (_all_items_cache.has(path)):
		var item = _all_items_cache[path];
		if (is_instance_valid(item)):
			item.free();
		_all_items_cache.erase(path);

func _on_item_collapsed(item: TreeItem) -> void:
	if (!_last_search_text.is_empty()): return;
	var data : Dictionary = item.get_metadata(0);
	if (data == null || !data.is_dir): return;
	var path : String = data.path;
	if (item.collapsed):
		if (_expanded_paths.has(path)): _expanded_paths.erase(path);
	else:
		if (!_expanded_paths.has(path)): _expanded_paths.append(path);

func refresh_tree(target_path: String = "") -> void:
	if (!target_path.is_empty() && _all_items_cache.has(target_path)):
		var item : TreeItem = _all_items_cache[target_path];
		var data : Dictionary = item.get_metadata(0);
		if (data != null && data.is_dir):
			_update_local_directory(target_path, item);
			return;
	_rebuild_full_tree();

func _rebuild_full_tree() -> void:
	if (get_root() != null):
		_expanded_paths.clear();
		_save_expanded_state(get_root());
	
	var saved_selected : Array[String] = [];
	var it : TreeItem = get_next_selected(null);
	while (it != null):
		saved_selected.append(it.get_metadata(0).path);
		it = get_next_selected(it);
	
	_all_items_cache.clear();
	clear();
	
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
	var child : TreeItem = parent_item.get_first_child();
	while (child != null):
		var c_path : String = child.get_metadata(0).path;
		_all_items_cache.erase(c_path);
		child.free();
		child = parent_item.get_first_child();
	_scan_folder(path, parent_item);
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
				if (dir.current_is_dir()): dirs.append(fn);
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

func _decorate_item(item: TreeItem) -> void:
	var data : Dictionary = item.get_metadata(0);
	if (data.is_dir): 
		item.set_icon(0, get_theme_icon("folder", "FileDialog")); 
		return; 
	var ext : String = data.path.get_extension().to_lower();
	if (ext in ["png", "jpg", "jpeg", "svg", "webp"]):
		var img : Image = GlobalEditorResourceLoader.load_resource(data.path).get_image();
		if (img != null): 
			img.resize(16, 16, Image.INTERPOLATE_LANCZOS);
			item.set_icon(0, ImageTexture.create_from_image(img));
	elif (ext == "gd"): 
		item.set_icon(0, get_theme_icon("GDScript", "EditorIcons")); 
	elif (ext == "tscn"): 
		item.set_icon(0, get_theme_icon("PackedScene", "EditorIcons")); 
	else: 
		item.set_icon(0, get_theme_icon("file", "FileDialog")); 

#endregion

#region --- 命令执行 ---

func _execute_command(id: int) -> void:
	var selected_items : Array[TreeItem] = [];
	var it : TreeItem = get_next_selected(null);
	while (it != null):
		selected_items.append(it);
		it = get_next_selected(it);
	if (selected_items.is_empty() && _selected_item != null): selected_items.append(_selected_item);
	
	var target_item : TreeItem = selected_items[0] if !selected_items.is_empty() else get_root();
	if (target_item == null): return;

	match (id):
		MenuID.CREATE_FOLDER, MenuID.CREATE_SCRIPT:
			var config : Dictionary = FILE_TEMPLATES[id];
			var data : Dictionary = target_item.get_metadata(0);
			target_item.collapsed = false;
			var base_dir : String = data.path if data.is_dir else data.path.get_base_dir();
			# 修复：调用 GlobalEditorFileSystem 中正确的公共方法名
			var final_path : String = GlobalEditorFileSystem.get_safe_move_path(base_dir, config.name);
			GlobalEditorFileSystem.execute_create_action(config.is_dir, final_path);
			
			await get_tree().process_frame;
			if (_all_items_cache.has(final_path)):
				var ni : TreeItem = _all_items_cache[final_path];
				deselect_all(); ni.select(0); scroll_to_item(ni); edit_item(ni);
		
		MenuID.DELETE:
			var paths_to_delete : Array[String] = [];
			for item : TreeItem in selected_items:
				var p : String = item.get_metadata(0).path;
				if (p != root_path): paths_to_delete.append(p);
			if (paths_to_delete.is_empty()): return;
			
			WindowManager.open_confirmation_window("移除文件", "确定删除 %d 项吗？" % paths_to_delete.size(), 
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
					WindowManager.open_confirmation_window("覆盖确认", "粘贴的文件已存在，是否覆盖？", func(confirmed):
						if (confirmed):
							GlobalEditorFileSystem.execute_move_batch_undoable(actual_move_paths, dest_dir, "剪切粘贴");
							_clipboard_paths.clear();
					);
				else:
					GlobalEditorFileSystem.execute_move_batch_undoable(actual_move_paths, dest_dir, "剪切粘贴");
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
		WindowManager.open_confirmation_window("覆盖确认", "目标已存在同名项目，是否覆盖？", func(confirmed):
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
	preview.text = "移动 %d 项" % paths.size(); 
	set_drag_preview(preview);
	return {"paths": paths};

func _can_drop_data(_pos: Vector2, data: Variant) -> bool:
	if (!data is Dictionary || !data.has("paths")): return false;
	var target : TreeItem = get_item_at_position(_pos);
	if (target == null || !target.get_metadata(0).is_dir): return false;
	var tp : String = target.get_metadata(0).path;
	
	var can_move = false;
	for sp : String in data.paths:
		if (sp.get_base_dir() != tp && sp != tp && !tp.begins_with(sp + "/")):
			can_move = true;
			break;
	return can_move;

func _drop_data(_pos: Vector2, data: Variant) -> void:
	var target : TreeItem = get_item_at_position(_pos);
	var dest_dir : String = target.get_metadata(0).path;
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
		WindowManager.open_confirmation_window("覆盖确认", "目标目录存在冲突，是否覆盖？", func(confirmed):
			if (confirmed): GlobalEditorFileSystem.execute_move_batch_undoable(move_paths, dest_dir, "拖拽移动");
		);
	else:
		GlobalEditorFileSystem.execute_move_batch_undoable(move_paths, dest_dir, "拖拽移动");

#endregion

func _on_menu_id_pressed(id: int) -> void: _execute_command(id);

func _on_item_activated() -> void:
	var it : TreeItem = get_selected();
	if (it == null): return;
	var data : Dictionary = it.get_metadata(0);
	if (data.is_dir): it.collapsed = !it.collapsed;
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

func _gui_input(event: InputEvent) -> void:
	if (event is InputEventKey && event.is_pressed()):
		if (event.is_command_or_control_pressed()):
			match (event.keycode):
				KEY_C: _execute_command(MenuID.COPY); accept_event();
				KEY_X: _execute_command(MenuID.CUT); accept_event();
				KEY_V: _execute_command(MenuID.PASTE); accept_event();
		elif (event.keycode == KEY_F2): _execute_command(MenuID.RENAME); accept_event();
		elif (event.keycode == KEY_DELETE): _execute_command(MenuID.DELETE); accept_event();
