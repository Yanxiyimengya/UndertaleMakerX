extends Control;

@export var project_item_packed_scene : PackedScene = null;

@onready var project_item_list: VBoxContainer = %ProjectItemList;
@onready var project_name_line_edit: LineEdit = %ProjectNameLineEdit;
@onready var create_project_window: Window = %CreateProjectWindow;
@onready var color_rect: ColorRect = %ColorRect;

enum SortType {
	NAME,
	OPEN_TIME
};
enum SortOrder {
	ASCENDING,  # 正序
	DESCENDING  # 倒序
};

@export var sort_order : SortOrder = SortOrder.DESCENDING;
@export var sort_type : SortType = SortType.OPEN_TIME;

func _enter_tree() -> void:
	EditorProjectManager.load_editor_all_project();

func _exit_tree() -> void:
	EditorProjectManager.save_editor_project_config();

func _ready() -> void:
	for project: UtmxProject in EditorProjectManager.projects.values() : 
		add_project_list_item(project);
	sort_project_item_list();

func add_project_list_item(project : UtmxProject) : 
	var item : ProjectListItem = project_item_packed_scene.instantiate();
	item.set_target_project(project);
	project_item_list.add_child(item);
	item.favorite_button_pressed.connect(sort_project_item_list);

func sort_project_item_list() : 
	var items : Array[ProjectListItem] = [];
	for child in project_item_list.get_children():
		if child is ProjectListItem:
			items.append(child);
	items.sort_custom(func(a: ProjectListItem, b: ProjectListItem):
		if (a.favorite != b.favorite):
			return a.favorite;
		var result : bool = false;
		match (sort_type):
			SortType.NAME:
				result = a.project_name.naturalnocasecmp_to(b.project_name) < 0;
			SortType.OPEN_TIME:
				result = a.last_open_time < b.last_open_time;
		if (sort_order == SortOrder.DESCENDING):
			return !result;
		return result;
	);
	for i in range(items.size()):
		project_item_list.move_child(items[i], i);

func _on_recent_button_pressed() -> void:
	sort_order = SortOrder.ASCENDING if (sort_order == SortOrder.DESCENDING) else SortOrder.DESCENDING;
	sort_project_item_list();

func _on_project_name_line_edit_text_changed(new_text: String) -> void:
	var filter_text : String = new_text.strip_edges().to_lower();
	for child in project_item_list.get_children():
		if (child is ProjectListItem):
			if (filter_text.is_empty()):
				child.visible = true;
			else:
				var item_name : String = child.project_name.to_lower();
				child.visible = item_name.contains(filter_text);
	sort_project_item_list();

func _on_import_button_pressed() -> void:
	DisplayServer.file_dialog_show("选择文件夹", "" , \
		"", false, DisplayServer.FILE_DIALOG_MODE_OPEN_ANY, 
		[EditorProjectManager.PROJECT_CONFIG_FILE_NAME + ",*.zip"],
		func(status: bool, selected_paths: PackedStringArray, _selected_filter_index: int) : 
			if (!status) : return;
			var path : String = selected_paths[0];
			var extension : String = path.get_extension();
			var project : UtmxProject;
			if (extension == "cfg") : 
				project = EditorProjectManager.load_project(path);
			elif (extension == "zip") : 
				pass;
			
			if (project != null) :
				add_project_list_item(project);
	);

var tween : Tween;
func _on_create_button_pressed() -> void:
	create_project_window.show();
	color_rect.show();
	if (tween != null && tween.is_running()) : 
		tween.kill();
	tween = create_tween();
	tween.tween_property(color_rect, "color:a", 0.75, 0.1).from(0.0);
	create_project_window.close_requested.connect(func() :
		if (tween != null && tween.is_running()) : 
			tween.kill();
		tween = create_tween();
		tween.tween_property(color_rect, "color:a", 0.0, 0.1);
		create_project_window.hide();
		await tween.finished
		color_rect.hide();
	, Object.ConnectFlags.CONNECT_ONE_SHOT);
