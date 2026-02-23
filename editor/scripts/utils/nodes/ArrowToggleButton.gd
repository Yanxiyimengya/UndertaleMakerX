class_name ArrowToggleButton extends Button

const DOWN_ARROW: Texture2D = preload("res://assets/icons/arrow/down.svg")
const TOP_ARROW: Texture2D = preload("res://assets/icons/arrow/top.svg")


func _pressed() -> void:
	icon = DOWN_ARROW if (button_pressed) else TOP_ARROW
