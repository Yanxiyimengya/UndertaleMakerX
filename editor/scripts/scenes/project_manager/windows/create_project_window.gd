extends PanelContainer

signal create_project_requset(
	project_name: String, project_dir: String, template: UtmxProjectTemplate
)

@export var template_item_scene: PackedScene
@export var parent_window: Window

@onready var template_grid_container: GridContainer = %TemplateGridContainer
@onready var create_button: Button = %CreateButton
@onready var project_dir_edit: LineEdit = %ProjectDirEdit
@onready var project_name_info_label: RichTextLabel = %ProjectNameInfoLabel
@onready var project_name_edit: LineEdit = %ProjectNameEdit

const TEMPLATE_INFO_PANEL: PackedScene = preload(
	"res://scenes/project_manager/template_info_panel/template_info_panel.tscn"
)

var project_path: String = EditorProjectManager.get_default_project_path()
var used_tamplate: UtmxProjectTemplate = null

var can_create: bool = true:
	set(value):
		can_create = value
		if !is_node_ready():
			await ready
		create_button.disabled = !value


func _ready() -> void:
	add_template_item(UtmxProjectTemplate.new())


func open() -> void:
	used_tamplate = null
	parent_window.min_size = self.get_minimum_size()
	project_dir_edit.text = project_path
	check_is_can_create()


func add_template_item(template: UtmxProjectTemplate) -> void:
	var item: TemplateListItem = template_item_scene.instantiate()
	template_grid_container.add_child(item)
	item.set_target_template(template)
	item.selected.connect(
		func():
			used_tamplate = item.target_template
			check_is_can_create()
	)


func _on_dir_texture_button_pressed() -> void:
	DisplayServer.file_dialog_show(
		tr("Select Folder"),
		"",
		"",
		false,
		DisplayServer.FILE_DIALOG_MODE_OPEN_DIR,
		[],
		func(status: bool, selected_paths: PackedStringArray, _selected_filter_index: int):
			if !status:
				return
			project_dir_edit.text = selected_paths[0]
			check_is_can_create()
	)


func _on_project_dir_edit_text_changed(_new_text: String) -> void:
	check_is_can_create()


func _on_project_name_edit_text_changed(_new_text: String) -> void:
	check_is_can_create()


func _on_create_button_pressed() -> void:
	if can_create:
		create_project_requset.emit(project_name_edit.text, project_dir_edit.text, used_tamplate)


func _on_cancel_button_pressed() -> void:
	if parent_window != null:
		parent_window.close_requested.emit()
	else:
		var parent: Node = get_parent()
		if parent is Window && parent != get_tree().root:
			parent.close_requested.emit()


func check_is_can_create() -> void:
	can_create = false
	var project_name: String = project_name_edit.text
	var project_dir: String = project_dir_edit.text
	project_name_info_label.text = ""
	if project_name.is_empty():
		project_name_info_label.text = (
			"[color=red]%s[/color]" % [tr("Project name cannot be empty")]
		)
		return
	if project_name.contains("/") || project_name.contains("\\") || project_name.contains(":"):
		project_name_info_label.text = (
			"[color=red]%s[/color]" % [tr("Folder name contains invalid characters")]
		)
		return
	var target_path = project_dir.path_join(project_name)
	if DirAccess.dir_exists_absolute(target_path):
		project_name_info_label.text = "[color=red]%s[/color]" % [tr("Folder already exists")]
		return
	if used_tamplate == null:
		return
	can_create = true
