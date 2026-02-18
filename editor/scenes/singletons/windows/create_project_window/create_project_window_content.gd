extends UtmxEditorWindow

signal create_project_requset(project_name: String, project_dir: String, template: UtmxProjectTemplate);
@export var template_item_scene: PackedScene;

@onready var create_project_window_content: PanelContainer = %CreateProjectWindowContent;
@onready var template_grid_container: GridContainer = %TemplateGridContainer;
@onready var create_button: Button = %CreateButton;
@onready var project_dir_edit: LineEdit = %ProjectDirEdit;
@onready var project_name_edit: LineEdit = %ProjectNameEdit;
@onready var dir_texture_button: TextureButton = %DirTextureButton;
@onready var cancel_button: Button = %CancelButton;
@onready var tab_container: TabContainer = %TabContainer;

@onready var project_info_label: RichTextLabel = %ProjectInfoLabel;

var project_path: String = EditorProjectManager.get_default_project_path();
var used_tamplate: UtmxProjectTemplate = null;
var default_tamplate_item: TemplateListItem = null;

var can_create: bool = true:
	set(value):
		can_create = value;
		if (! is_node_ready()): await ready;
		create_button.disabled = !value;

func _ready() -> void:
	_apply_translations();
	project_dir_edit.text_changed.connect(_on_inputs_changed);
	project_name_edit.text_changed.connect(_on_inputs_changed);
	create_button.pressed.connect(_on_create_button_pressed);
	cancel_button.pressed.connect(_on_cancel_button_pressed);
	dir_texture_button.pressed.connect(_on_dir_select_pressed);
	add_template_item(UtmxProjectTemplate.new(), true);

func _notification(what: int) -> void:
	if (what == NOTIFICATION_TRANSLATION_CHANGED):
		_apply_translations();

func _open() -> void:
	used_tamplate = default_tamplate_item.target_template;
	default_tamplate_item.pressed_button(true);
	project_name_edit.text = tr("New Project");
	project_dir_edit.text = project_path;
	self.min_size = create_project_window_content.get_combined_minimum_size();
	check_is_can_create();

# 添加模板项目逻辑
func add_template_item(template: UtmxProjectTemplate, default : bool = false) -> void:
	var item: TemplateListItem = template_item_scene.instantiate();
	template_grid_container.add_child(item);
	item.set_target_template(template);
	item.selected.connect(func():
		used_tamplate = item.target_template;
		check_is_can_create();
	);
	if (default): 
		default_tamplate_item = item;

# 文件夹选择对话框
func _on_dir_select_pressed() -> void:
	DisplayServer.file_dialog_show(
		tr("Select Folder"), 
		project_dir_edit.text, 
		"", 
		false, 
		DisplayServer.FILE_DIALOG_MODE_OPEN_DIR, 
		[],
		func(status: bool, selected_paths: PackedStringArray, _idx: int):
			if (!status): return;
			project_dir_edit.text = selected_paths[0];
			check_is_can_create();
	)

func _on_inputs_changed(_text: String) -> void:
	check_is_can_create();

func _on_create_button_pressed() -> void:
	if (can_create):
		create_project_requset.emit(project_name_edit.text, project_dir_edit.text, used_tamplate);

func _on_cancel_button_pressed() -> void:
	close();

func check_is_can_create() -> void:
	can_create = false;
	var project_name: String = project_name_edit.text;
	var project_dir: String = project_dir_edit.text;
	project_info_label.text = "";
	if (project_name.is_empty()):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Project name cannot be empty")];
		return;
	if (project_name.contains("/") or project_name.contains("\\") or project_name.contains(":")):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Folder name contains invalid characters")];
		return;
	if (project_dir.is_empty()):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Project path cannot be empty")];
		return;
	if (!DirAccess.dir_exists_absolute(project_dir)):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Project path does not exist or is inaccessible")];
		return;
	var target_path = project_dir.path_join(project_name);
	if (DirAccess.dir_exists_absolute(target_path)):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Folder already exists")];
		return;
	if (used_tamplate == null):
		project_info_label.text = "[color=red]%s[/color]" % [tr("Please select a valid project template")];
		return;
	can_create = true;

func _apply_translations() -> void:
	if (tab_container != null && tab_container.get_tab_count() > 0):
		tab_container.set_tab_title(0, tr("Template"));
