extends Control

@export var project_item_packed_scene : PackedScene = null
@onready var project_item_list: VBoxContainer = %ProjectItemList
@onready var project_name_line_edit: LineEdit = %ProjectNameLineEdit

enum SortType {
	NAME,
	OPEN_TIME
}
enum SortOrder {
	ASCENDING,
	DESCENDING
}

@export var sort_order : SortOrder = SortOrder.DESCENDING
@export var sort_type : SortType = SortType.OPEN_TIME

func _enter_tree() -> void:
	EditorProjectManager.load_editor_all_projects()

func _exit_tree() -> void:
	EditorProjectManager.save_editor_all_projects()

func _ready() -> void:
	for project: UtmxProject in EditorProjectManager.projects.values(): 
		add_project_list_item(project)
	sort_project_item_list()

func add_project_list_item(project : UtmxProject) -> void: 
	var item : ProjectListItem = project_item_packed_scene.instantiate()
	item.set_target_project(project)
	project_item_list.add_child(item)
	item.favorite_button_pressed.connect(sort_project_item_list)
	item.deleted.connect(func(): delete_project_item(item))

func sort_project_item_list() -> void: 
	var items : Array[ProjectListItem] = []
	for child in project_item_list.get_children():
		if child is ProjectListItem:
			items.append(child)
	
	items.sort_custom(func(a: ProjectListItem, b: ProjectListItem):
		if (a.favorite != b.favorite):
			return a.favorite
		
		var result : bool = false
		match (sort_type):
			SortType.NAME:
				result = a.project_name.naturalnocasecmp_to(b.project_name) < 0
			SortType.OPEN_TIME:
				result = a.last_open_time < b.last_open_time
		
		if (sort_order == SortOrder.DESCENDING):
			return !result
		return result
	)
	
	for i in range(items.size()):
		project_item_list.move_child(items[i], i)

func delete_project_item(item : ProjectListItem) -> void: 
	WindowManager.open_confirmation_window("移除项目",
		"确定移除该项目吗？\n该操作不会删除任何物理文件。", func(confirm : bool): 
		if (confirm): 
			EditorProjectManager.remove_project(item.target_project.project_path)
			item.queue_free()
			EditorProjectManager.save_editor_all_project()
	)

func _on_recent_button_pressed() -> void:
	sort_order = SortOrder.ASCENDING if (sort_order == SortOrder.DESCENDING) else SortOrder.DESCENDING
	sort_project_item_list()

func _on_project_name_line_edit_text_changed(new_text: String) -> void:
	var filter_text : String = new_text.strip_edges().to_lower()
	for child in project_item_list.get_children():
		if (child is ProjectListItem):
			if (filter_text.is_empty()):
				child.visible = true
			else:
				var item_name : String = child.project_name.to_lower()
				child.visible = item_name.contains(filter_text)
	sort_project_item_list()

func _on_create_button_pressed() -> void:
	WindowManager.open_create_project_window(_on_create_project_window_content_create_project_requset)

func _on_create_project_window_content_create_project_requset( \
		project_name: String, project_dir: String, template: UtmxProjectTemplate) -> void:
	var proj : UtmxProject = null
	var proj_path : String = project_dir.path_join(project_name)
	
	if (template != null and FileAccess.file_exists(template.zip_path)): 
		proj = EditorProjectManager.create_project_from_zip(project_name, template.zip_path, proj_path)
	else: 
		proj = EditorProjectManager.create_default_project(project_name, proj_path)
	
	if (proj == null): return
	add_project_list_item(proj)
	EditorProjectManager.save_editor_all_projects()
	EditorProjectManager.open_project(proj)
