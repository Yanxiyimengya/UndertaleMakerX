extends MenuBar
@onready var file_popup_menu: PopupMenu = $FileMenu;
@onready var project_popup_menu: PopupMenu = $ProjectMenu;
@onready var editor_popup_menu: PopupMenu = $EditorMenu;
@onready var tool_popup_menu: PopupMenu = $ToolsMenu;

func _ready() -> void:
	_apply_translations();

func _notification(what: int) -> void:
	if (what == NOTIFICATION_TRANSLATION_CHANGED):
		_apply_translations();

func _apply_translations() -> void:
	if (get_menu_count() > 0): set_menu_title(0, tr("File"));
	if (get_menu_count() > 1): set_menu_title(1, tr("Project"));
	if (get_menu_count() > 2): set_menu_title(2, tr("Editor"));
	if (get_menu_count() > 3): set_menu_title(3, tr("Tools"));
	
func _exit_tree() -> void:
	pass;
