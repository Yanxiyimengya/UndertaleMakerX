extends Node

var history: UndoRedo = UndoRedo.new()

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey and event.is_pressed():
		if event.ctrl_pressed and event.keycode == KEY_Z and not event.shift_pressed:
			undo_action()
		elif event.ctrl_pressed and (event.keycode == KEY_Y or (event.shift_pressed and event.keycode == KEY_Z)):
			redo_action()

func undo_action() -> void:
	if history.has_undo():
		history.undo()

func redo_action() -> void:
	if history.has_redo():
		history.redo()

func create_action(action_name: String) -> void:
	history.create_action(action_name)

func commit() -> void:
	history.commit_action()
