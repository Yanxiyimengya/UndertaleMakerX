extends Tree

@export_dir var root_path: String = EditorProjectManager.get_default_project_path()

@onready var context_menu: PopupMenu = $PopupFileSelectMenu as PopupMenu
@onready var popup_multi_select_menu: PopupMenu = $PopupMultiSelectMenu as PopupMenu

var _selected_item: TreeItem = null
var _expanded_paths: Array[String] = []
var _all_items_cache: Dictionary = {}
var title_visible: bool = false

var _clipboard_paths: Array[String] = []
var _is_cut_mode: bool = false

enum MenuID {
	CREATE_FOLDER = 0,
	DELETE = 1,
	RENAME = 2,
	OPEN_IN_EXPLORER = 3,
	OPEN_EXTERNAL = 4,
	COPY = 5,
	CUT = 6,
	PASTE = 7
}

## 初始化组件与信号连接
func _ready() -> void:
	if (title_visible): 
		column_titles_visible = true
		set_column_title(0, "项目文件结构")
	
	allow_rmb_select = true
	select_mode = SELECT_MULTI
	
	_setup_menu(context_menu, false)
	_setup_menu(popup_multi_select_menu, true)
	
	context_menu.id_pressed.connect(_on_menu_id_pressed)
	popup_multi_select_menu.id_pressed.connect(_on_menu_id_pressed)
	
	item_activated.connect(_on_item_activated)
	item_mouse_selected.connect(_on_item_mouse_selected)
	item_edited.connect(_on_item_edited)
	
	refresh_tree()

## 配置右键菜单项
func _setup_menu(menu: PopupMenu, is_multi: bool) -> void:
	menu.clear()
	if (!is_multi):
		menu.add_item("创建文件夹", MenuID.CREATE_FOLDER)
		menu.add_item("重命名 (F2)", MenuID.RENAME)
		menu.add_separator()
	
	menu.add_item("复制 (Ctrl+C)", MenuID.COPY)
	menu.add_item("剪切 (Ctrl+X)", MenuID.CUT)
	menu.add_item("粘贴 (Ctrl+V)", MenuID.PASTE)
	menu.add_separator()
	menu.add_item("删除 (Delete)", MenuID.DELETE)
	
	if (!is_multi):
		menu.add_separator()
		menu.add_item("在资源管理器中显示", MenuID.OPEN_IN_EXPLORER)
		menu.add_item("用外部程序打开", MenuID.OPEN_EXTERNAL)

## 全量刷新树结构并恢复展开/选中状态
func refresh_tree() -> void:
	_expanded_paths.clear()
	if (get_root()): _save_expanded_state(get_root())
	
	var saved_selected_paths: Array[String] = []
	var it = get_next_selected(null)
	while it:
		saved_selected_paths.append(it.get_metadata(0).path)
		it = get_next_selected(it)
	
	_all_items_cache.clear()
	clear()
	
	if (!DirAccess.dir_exists_absolute(root_path)): 
		DirAccess.make_dir_recursive_absolute(root_path)
		
	var root_item: TreeItem = create_item()
	root_item.set_text(0, root_path.get_file() if (root_path.get_file() != "") else root_path)
	root_item.set_metadata(0, {"path": root_path, "is_dir": true})
	_all_items_cache[root_path] = root_item
	
	_scan_folder(root_path, root_item)
	
	for path in saved_selected_paths:
		if (_all_items_cache.has(path)):
			_all_items_cache[path].select(0)
			scroll_to_item(_all_items_cache[path])

## 记录当前所有展开节点的路径
func _save_expanded_state(item: TreeItem) -> void:
	if (!item): return
	var data: Dictionary = item.get_metadata(0)
	if (data.is_dir && !item.collapsed): _expanded_paths.append(data.path)
	var child: TreeItem = item.get_first_child()
	while (child):
		_save_expanded_state(child)
		child = child.get_next()

## 递归扫描目录并构建子节点
func _scan_folder(path: String, parent_item: TreeItem) -> void:
	var dir = DirAccess.open(path)
	if (dir):
		var dirs: Array[String] = []
		var files: Array[String] = []
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while (file_name != ""):
			if (file_name != "." && file_name != ".."):
				if (dir.current_is_dir()): dirs.append(file_name)
				else: files.append(file_name)
			file_name = dir.get_next()
		
		dirs.sort_custom(func(a, b): return a.naturalnocasecmp_to(b) < 0)
		files.sort_custom(func(a, b): return a.naturalnocasecmp_to(b) < 0)
		
		for d in dirs:
			var full_path = path.path_join(d)
			var item = create_item(parent_item)
			item.set_text(0, d)
			item.set_metadata(0, {"path": full_path, "is_dir": true})
			item.collapsed = !(_expanded_paths.has(full_path))
			_all_items_cache[full_path] = item
			_decorate_item(item)
			_scan_folder(full_path, item)
			
		for f in files:
			var full_path = path.path_join(f)
			var item = create_item(parent_item)
			item.set_text(0, f)
			item.set_metadata(0, {"path": full_path, "is_dir": false})
			_all_items_cache[full_path] = item
			_decorate_item(item)

## 菜单信号处理入口
func _on_menu_id_pressed(id: int) -> void:
	_execute_command(id)

## 指令执行核心逻辑，包含无限递归安全检查
func _execute_command(id: int) -> void:
	var selected_items: Array[TreeItem] = []
	var it = get_next_selected(null)
	while it:
		selected_items.append(it)
		it = get_next_selected(it)
	
	if (selected_items.is_empty() && _selected_item): selected_items.append(_selected_item)
	if (selected_items.is_empty()): return

	match (id):
		MenuID.COPY, MenuID.CUT:
			_clipboard_paths.clear()
			for item in selected_items:
				_clipboard_paths.append(item.get_metadata(0).path)
			_is_cut_mode = (id == MenuID.CUT)
			
		MenuID.PASTE:
			if (_clipboard_paths.is_empty()): return
			var target_item = selected_items[0]
			var target_data = target_item.get_metadata(0)
			var dest_dir = target_data.path if target_data.is_dir else target_data.path.get_base_dir()
			
			for src_path in _clipboard_paths:
				# 安全检查：禁止将父目录粘贴到自身或其子目录（防止无限递归闪退）
				if (dest_dir == src_path or dest_dir.begins_with(src_path + "/")):
					push_warning("无法将文件夹粘贴到自身或其子文件夹内: %s" % src_path)
					continue
				
				var dest_path = _get_safe_move_path(dest_dir, src_path.get_file())
				if (_is_cut_mode): _execute_move_action(src_path, dest_path)
				else: _copy_recursive(src_path, dest_path)
			
			if (target_data.is_dir):
				target_item.collapsed = false
				if (!_expanded_paths.has(target_data.path)): _expanded_paths.append(target_data.path)
			
			if (_is_cut_mode): _clipboard_paths.clear()
			refresh_tree()
		
		MenuID.DELETE:
			if (selected_items.is_empty()): return
			var perform_delete = func(items: Array[TreeItem]):
				for item in items:
					var p: String = item.get_metadata(0).path
					if (p == root_path): continue
					var err: Error = _delete_dir_recursive(p) if item.get_metadata(0).is_dir else DirAccess.remove_absolute(p)
					if (err == OK or !DirAccess.dir_exists_absolute(p)):
						_all_items_cache.erase(p)
						item.free()
			
			if (selected_items.size() > 1):
				WindowManager.open_confirmation_window("确定删除 %d 项？" % selected_items.size(), func(confirm):
					if (confirm): perform_delete.call(selected_items)
				)
			else: perform_delete.call(selected_items)

		MenuID.CREATE_FOLDER:
			var data = selected_items[0].get_metadata(0)
			selected_items[0].collapsed = false
			var base_path = data.path if data.is_dir else data.path.get_base_dir()
			var full_new_path = _get_safe_move_path(base_path, "新建文件夹")
			GlobalEditorUndoRedoManager.create_action("创建文件夹")
			GlobalEditorUndoRedoManager.history.add_do_method(_action_make_dir.bind(full_new_path))
			GlobalEditorUndoRedoManager.history.add_undo_method(_action_remove_dir.bind(full_new_path))
			GlobalEditorUndoRedoManager.commit()

		MenuID.RENAME:
			selected_items[0].set_editable(0, true)
			edit_selected()

		MenuID.OPEN_IN_EXPLORER, MenuID.OPEN_EXTERNAL:
			var path = ProjectSettings.globalize_path(selected_items[0].get_metadata(0).path)
			if (id == MenuID.OPEN_IN_EXPLORER):
				OS.shell_show_in_file_manager(path if selected_items[0].get_metadata(0).is_dir else path.get_base_dir())
			else: OS.shell_open(path)

## 递归删除目录及其内容
func _delete_dir_recursive(path: String) -> Error:
	var dir = DirAccess.open(path)
	if (!dir): return OK
	dir.list_dir_begin()
	var file_name = dir.get_next()
	while (file_name != ""):
		if (file_name != "." and file_name != ".."):
			var full_path = path.path_join(file_name)
			if (dir.current_is_dir()): _delete_dir_recursive(full_path)
			else: DirAccess.remove_absolute(full_path)
		file_name = dir.get_next()
	return DirAccess.remove_absolute(path)

## 设置节点图标与缩略图
func _decorate_item(item: TreeItem) -> void:
	var data = item.get_metadata(0)
	if (data.is_dir):
		item.set_icon(0, get_theme_icon("folder", "FileDialog"))
		return
	var ext = data.path.get_extension().to_lower()
	if (ext in ["png", "jpg", "jpeg", "svg", "webp"]):
		var img = Image.load_from_file(data.path)
		if (img):
			img.resize(16, 16, Image.INTERPOLATE_LANCZOS)
			item.set_icon(0, ImageTexture.create_from_image(img))
	elif (ext == "gd"): item.set_icon(0, get_theme_icon("GDScript", "EditorIcons"))
	elif (ext == "tscn"): item.set_icon(0, get_theme_icon("PackedScene", "EditorIcons"))
	else: item.set_icon(0, get_theme_icon("file", "FileDialog"))

## 快捷键监听
func _gui_input(event: InputEvent) -> void:
	if (event is InputEventKey and event.is_pressed()):
		if (event.is_command_or_control_pressed()):
			match event.keycode:
				KEY_C: _execute_command(MenuID.COPY)
				KEY_X: _execute_command(MenuID.CUT)
				KEY_V: _execute_command(MenuID.PASTE)
		elif (event.keycode == KEY_F2): _execute_command(MenuID.RENAME)
		elif (event.keycode == KEY_DELETE): _execute_command(MenuID.DELETE)

## 鼠标右键选中逻辑
func _on_item_mouse_selected(pos: Vector2, mouse_button_index: int) -> void:
	if (mouse_button_index == MOUSE_BUTTON_RIGHT):
		var hovered = get_item_at_position(pos)
		if (!hovered): return
		var it = get_next_selected(null)
		var count = 0
		var hovered_selected = false
		while it:
			count += 1
			if (it == hovered): hovered_selected = true
			it = get_next_selected(it)
		if (!hovered_selected):
			deselect_all()
			hovered.select(0)
			count = 1
		_selected_item = hovered
		var menu = popup_multi_select_menu if count > 1 else context_menu
		var p_idx = menu.get_item_index(MenuID.PASTE)
		if (p_idx != -1): menu.set_item_disabled(p_idx, _clipboard_paths.is_empty())
		menu.position = Vector2i(get_viewport().get_mouse_position()) + get_window().position
		menu.popup()

## 稳健的递归复制逻辑
func _copy_recursive(from: String, to: String) -> void:
	if (DirAccess.dir_exists_absolute(from)):
		if (!DirAccess.dir_exists_absolute(to)): DirAccess.make_dir_recursive_absolute(to)
		var dir = DirAccess.open(from)
		if (dir):
			dir.list_dir_begin()
			var fn = dir.get_next()
			while (fn != ""):
				if (fn != "." and fn != ".."): _copy_recursive(from.path_join(fn), to.path_join(fn))
				fn = dir.get_next()
	else: DirAccess.copy_absolute(from, to)

## 自动递增文件名避免冲突
func _get_safe_move_path(dest: String, n: String) -> String:
	var base = n.get_basename()
	var ext = n.get_extension()
	var d = "." if ext != "" else ""
	var f = dest.path_join(n)
	var c = 1
	while (FileAccess.file_exists(f) or DirAccess.dir_exists_absolute(f)):
		f = dest.path_join(base + " (" + str(c) + ")" + d + ext)
		c += 1
	return f

## 磁盘操作辅助
func _action_rename(f: String, t: String) -> void:
	DirAccess.rename_absolute(f, t)
	refresh_tree()

func _action_make_dir(p: String) -> void:
	DirAccess.make_dir_absolute(p)
	refresh_tree()

func _action_remove_dir(p: String) -> void:
	DirAccess.remove_absolute(p)
	refresh_tree()

## 节点编辑完成处理
func _on_item_edited() -> void:
	var item = get_edited()
	var old_p = item.get_metadata(0).path
	var new_p = old_p.get_base_dir().path_join(item.get_text(0))
	item.set_editable(0, false)
	if (old_p == new_p or item.get_text(0).is_empty()): return
	GlobalEditorUndoRedoManager.create_action("重命名")
	GlobalEditorUndoRedoManager.history.add_do_method(_action_rename.bind(old_p, new_p))
	GlobalEditorUndoRedoManager.history.add_undo_method(_action_rename.bind(new_p, old_p))
	GlobalEditorUndoRedoManager.commit()

## 剪切/移动逻辑处理
func _execute_move_action(f: String, t: String) -> void:
	GlobalEditorUndoRedoManager.create_action("移动")
	GlobalEditorUndoRedoManager.history.add_do_method(_action_rename.bind(f, t))
	GlobalEditorUndoRedoManager.history.add_undo_method(_action_rename.bind(t, f))
	GlobalEditorUndoRedoManager.commit()

## 激活节点处理
func _on_item_activated() -> void:
	var item = get_selected()
	if (item and item.get_metadata(0).is_dir): item.collapsed = !item.collapsed
