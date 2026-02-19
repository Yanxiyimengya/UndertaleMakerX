extends MenuBar

@onready var project_popup_menu: PopupMenu = $ProjectMenu;
@onready var editor_popup_menu: PopupMenu = $EditorMenu;
@onready var tool_popup_menu: PopupMenu = $ToolsMenu;

enum ProjectMenuId {
	PACK = 0,
	EXIT_TO_PROJECT_LIST = 1,
}

func _ready() -> void:
	var menu : PopupMenu = _get_project_popup_menu();
	if (menu != null && !menu.id_pressed.is_connected(_on_project_menu_id_pressed)):
		menu.id_pressed.connect(_on_project_menu_id_pressed);
	_setup_project_menu();
	_apply_translations();

func _notification(what: int) -> void:
	if (what == NOTIFICATION_TRANSLATION_CHANGED && is_node_ready()):
		_apply_translations();

func _apply_translations() -> void:
	var menu : PopupMenu = _get_project_popup_menu();
	if (menu == null): return;
	if (get_menu_count() > 0): set_menu_title(0, tr("Project"));
	if (get_menu_count() > 1): set_menu_title(1, tr("Editor"));
	if (get_menu_count() > 2): set_menu_title(2, tr("Tools"));
	var pack_idx : int = menu.get_item_index(ProjectMenuId.PACK);
	if (pack_idx != -1): menu.set_item_text(pack_idx, tr("打包"));
	var exit_idx : int = menu.get_item_index(ProjectMenuId.EXIT_TO_PROJECT_LIST);
	if (exit_idx != -1): menu.set_item_text(exit_idx, tr("退出到项目列表"));

func _setup_project_menu() -> void:
	var menu : PopupMenu = _get_project_popup_menu();
	if (menu == null): return;
	menu.clear();
	menu.add_item(tr("打包"), ProjectMenuId.PACK);
	menu.add_separator("");
	menu.add_item(tr("退出到项目列表"), ProjectMenuId.EXIT_TO_PROJECT_LIST);

func _get_project_popup_menu() -> PopupMenu:
	if (project_popup_menu == null):
		project_popup_menu = get_node_or_null("ProjectMenu") as PopupMenu;
	return project_popup_menu;

func _on_project_menu_id_pressed(id : int) -> void:
	match (id):
		ProjectMenuId.PACK:
			push_warning("Project menu pack action is not implemented yet.");
		ProjectMenuId.EXIT_TO_PROJECT_LIST:
			EditorProjectManager.back_to_project_list();

func _exit_tree() -> void:
	pass;
