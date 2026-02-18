extends MenuBar
@onready var file_popup_menu: PopupMenu = $文件;
@onready var project_popup_menu: PopupMenu = $项目;
@onready var editor_popup_menu: PopupMenu = $编辑器;
@onready var tool_popup_menu: PopupMenu = $工具;

func _enter_tree() -> void:
	pass;
	
func _exit_tree() -> void:
	pass;
