@tool
class_name ProjectListItem extends PanelContainer

signal favorite_button_pressed
signal folder_button_pressed
signal more_button_pressed
signal deleted

@onready var star_button: TextureButton = %StarButton
@onready var icon_texture_rect: TextureRect = %IconTextureRect
@onready var project_name_edit: LineEdit = %ProjectNameEdit
@onready var project_path_label: Label = %ProjectPathLabel
@onready var last_open_time_label: Label = %LastOpenTimeLabel
@onready var more_button: MenuButton = %MoreButton
@onready var engine_version_label: Label = %EngineVersionLabel
@onready var button: Button = %Button

@export var renamming: bool = false:
	set(value):
		renamming = value
		if !is_node_ready():
			await ready
		if value:
			project_name_edit.editable = true
			project_name_edit.process_mode = Node.PROCESS_MODE_INHERIT
			project_name_edit.mouse_filter = Control.MOUSE_FILTER_STOP
			project_name_edit.select_all()
			project_name_edit.grab_focus()
		else:
			project_name_edit.editable = false
			project_name_edit.process_mode = Node.PROCESS_MODE_DISABLED
			project_name_edit.mouse_filter = Control.MOUSE_FILTER_IGNORE
			project_name_edit.release_focus()

@export var favorite: bool = false:
	set(value):
		if value == favorite:
			return
		favorite = value
		if !is_node_ready():
			await ready
		star_button.button_pressed = value

@export var project_name: String = "":
	set(value):
		project_name = value
		if !is_node_ready():
			await ready
		project_name_edit.text = value

@export var project_path: String = "":
	set(value):
		if !is_node_ready():
			await ready
		project_path_label.text = value
		project_path = value

@export var engine_version: String = "":
	set(value):
		engine_version = value
		if !is_node_ready():
			await ready
		engine_version_label.text = value

@export var icon_texture: Texture2D = null:
	set(value):
		if value == null:
			return
		icon_texture = value
		if !is_node_ready():
			await ready
		icon_texture_rect.texture = value

@export var last_open_time: int = 0:
	set(value):
		if value == last_open_time:
			return
		last_open_time = value
		if !is_node_ready():
			await ready
		var current_time: float = int(Time.get_unix_time_from_system())
		var time_change: float = current_time - value
		var time_str: String = ""
		const MINUTE = 60
		const HOUR = 3600
		const DAY = 86400
		const MONTH = 2592000
		const YEAR = 31536000
		if time_change < MINUTE:
			time_str = tr("Just now")
		elif time_change < HOUR:
			time_str = tr("%d minutes ago") % [time_change / MINUTE]
		elif time_change < DAY:
			time_str = tr("%d hours ago") % [time_change / HOUR]
		elif time_change < MONTH:
			time_str = tr("%d days ago") % [time_change / DAY]
		elif time_change < YEAR:
			time_str = tr("%d months ago") % [time_change / MONTH]
		else:
			var years: float = time_change / YEAR
			var remaining_seconds: float = int(time_change) % YEAR
			var months: float = remaining_seconds / MONTH
			if months > 0:
				time_str = tr("%d years %d months ago") % [years, months]
			else:
				time_str = tr("%d years ago") % [years]
		last_open_time_label.text = time_str

var target_project: UtmxProject = null


func _ready() -> void:
	var popup: PopupMenu = more_button.get_popup()
	popup.id_pressed.connect(_on_more_button_popup_menu_id_pressed)
	renamming = false
	button.gui_input.connect(_gui_input)


func _on_more_button_popup_menu_id_pressed(idx: int) -> void:
	if idx == 1:
		open_project()
	elif idx == 2:
		deleted.emit()
	elif idx == 3:
		renamming = true


func set_target_project(project: UtmxProject) -> void:
	if project == null:
		return
	project_name = project.project_name
	project_path = project.project_path
	last_open_time = project.last_open_time
	icon_texture = project.icon_texture
	engine_version = project.engine_version
	favorite = project.favorite
	target_project = project


func _on_star_button_pressed() -> void:
	favorite = !favorite
	target_project.favorite = favorite
	favorite_button_pressed.emit()


func _on_dir_button_pressed() -> void:
	folder_button_pressed.emit()
	OS.shell_open(project_path)


func _on_more_button_pressed() -> void:
	more_button_pressed.emit()


func _on_project_name_edit_focus_exited() -> void:
	target_project.project_name = project_name_edit.text
	renamming = false
	EditorProjectManager.save_project_config(target_project)


func _gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT && event.pressed:
			if event.double_click:
				open_project()
		elif event.button_index == MOUSE_BUTTON_RIGHT && event.pressed && button.button_pressed:
			more_button.show_popup()
			more_button.get_popup().position = DisplayServer.mouse_get_position()


func open_project() -> void:
	EditorProjectManager.open_project(target_project)
