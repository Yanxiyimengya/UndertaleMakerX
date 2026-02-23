@tool
class_name TemplateListItem extends PanelContainer

signal selected

@onready var button: Button = %Button
@onready var template_image_rect: TextureRect = %TemplateImageRect
@onready var name_label: Label = %NameLabel
@onready var description_label: RichTextLabel = %DescriptionLabel

var target_template: UtmxProjectTemplate

@export var template_name: String = "":
	set(value):
		template_name = value
		if !is_node_ready():
			await ready
		name_label.text = tr(value)

@export_multiline() var template_description: String = "":
	set(value):
		template_description = value
		if !is_node_ready():
			await ready
		description_label.text = tr(value)

@export var template_image: Texture2D = null:
	set(value):
		template_image = value
		if !is_node_ready():
			await ready
		template_image_rect.texture = value


func set_target_template(template: UtmxProjectTemplate) -> void:
	if template == null:
		return
	template_name = template.template_name
	template_description = template.template_description
	if template.template_image != null:
		template_image = template.template_image
	target_template = template


func _on_button_pressed() -> void:
	selected.emit()


func _on_visibility_changed() -> void:
	if !is_visible_in_tree():
		button.button_pressed = false


func pressed_button(press: bool) -> void:
	button.button_pressed = press
