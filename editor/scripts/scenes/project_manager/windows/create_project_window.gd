extends PanelContainer

signal create_project_requset(
	project_name: String, project_dir: String, template: UtmxProjectTemplate
)

const DEFAULT_TEMPLATE_ZIP_DIRECTORY: String = "res://assets/templates/projects"

@export var template_item_scene: PackedScene
@export var parent_window: Window
@export var template_zip_directory: String = DEFAULT_TEMPLATE_ZIP_DIRECTORY

@onready var template_grid_container: GridContainer = %TemplateGridContainer
@onready var create_button: Button = %CreateButton
@onready var project_dir_edit: LineEdit = %ProjectDirEdit
@onready var project_name_info_label: RichTextLabel = %ProjectNameInfoLabel
@onready var project_name_edit: LineEdit = %ProjectNameEdit

var project_path: String = EditorProjectManager.get_default_project_path()
var used_tamplate: UtmxProjectTemplate = null
var default_tamplate_item: TemplateListItem = null

var can_create: bool = true:
	set(value):
		can_create = value
		if !is_node_ready():
			await ready
		create_button.disabled = !value


func _ready() -> void:
	_reload_templates_from_directory(template_zip_directory)


func open() -> void:
	_reload_templates_from_directory(template_zip_directory)
	_select_default_template_item()
	if parent_window != null:
		parent_window.min_size = self.get_minimum_size()
	project_dir_edit.text = project_path
	check_is_can_create()


func add_template_item(template: UtmxProjectTemplate, default: bool = false) -> void:
	var item: TemplateListItem = template_item_scene.instantiate()
	template_grid_container.add_child(item)
	item.set_target_template(template)
	item.selected.connect(
		func():
			used_tamplate = item.target_template
			check_is_can_create()
	)
	if default:
		default_tamplate_item = item


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
	var target_path: String = project_dir.path_join(project_name)
	if DirAccess.dir_exists_absolute(target_path):
		project_name_info_label.text = "[color=red]%s[/color]" % [tr("Folder already exists")]
		return
	if used_tamplate == null:
		project_name_info_label.text = (
			"[color=red]%s[/color]" % [tr("Please select a valid project template")]
		)
		return
	can_create = true


func _reload_templates_from_directory(directory_path: String) -> void:
	for child: Node in template_grid_container.get_children():
		child.free()
	default_tamplate_item = null
	used_tamplate = null

	var target_directory: String = String(directory_path).strip_edges()
	if target_directory.is_empty():
		target_directory = EditorTemplateManager.get_default_template_directory()

	var templates: Array[UtmxProjectTemplate] = EditorTemplateManager.load_templates_from_directory(
		target_directory
	)
	if templates.is_empty():
		return

	var default_template: UtmxProjectTemplate = EditorTemplateManager.get_default_template()
	var default_zip_path: String = ""
	if default_template != null:
		default_zip_path = _normalize_path(default_template.zip_path)

	for template: UtmxProjectTemplate in templates:
		var is_default_item: bool = (
			!default_zip_path.is_empty()
			&& _normalize_path(template.zip_path) == default_zip_path
		)
		add_template_item(template, is_default_item)

	if default_tamplate_item == null && template_grid_container.get_child_count() > 0:
		var first_item: Node = template_grid_container.get_child(0)
		if first_item is TemplateListItem:
			default_tamplate_item = first_item as TemplateListItem


func _select_default_template_item() -> void:
	if default_tamplate_item == null:
		used_tamplate = null
		return
	used_tamplate = default_tamplate_item.target_template
	default_tamplate_item.pressed_button(true)


func _normalize_path(path: String) -> String:
	var raw_path: String = String(path).strip_edges().replace("\\", "/")
	if raw_path.is_empty():
		return ""
	if raw_path.begins_with("res://"):
		return "res://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("res://"))]
	if raw_path.begins_with("user://"):
		return "user://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("user://"))]
	return raw_path.simplify_path()


func _normalize_virtual_path_body(path_body: String) -> String:
	var normalized_body: String = String(path_body).strip_edges().replace("\\", "/")
	while normalized_body.begins_with("/"):
		normalized_body = normalized_body.trim_prefix("/")
	while normalized_body.contains("//"):
		normalized_body = normalized_body.replace("//", "/")

	var segments: Array[String] = []
	for raw_segment: String in normalized_body.split("/", false):
		var segment: String = raw_segment.strip_edges()
		if segment.is_empty() || segment == ".":
			continue
		if segment == "..":
			if !segments.is_empty():
				segments.remove_at(segments.size() - 1)
			continue
		segments.append(segment)
	return "/".join(segments)
