# UndoManager.gd (全局单例)
extends Node;

var history: UndoRedo = UndoRedo.new();

## 处理快捷键监听
func _unhandled_input(event: InputEvent) -> void:
	if (event is InputEventKey and event.is_pressed()):
		if (event.is_action_pressed("ui_undo") or (event.ctrl_pressed and event.keycode == KEY_Z)):
			undo_action()
		elif (event.is_action_pressed("ui_redo") or (event.ctrl_pressed and event.keycode == KEY_Y)):
			redo_action()

func undo_action() -> void:
	if (history.has_undo()):
		history.undo()
		print("撤销: ", history.get_current_action_name())

func redo_action() -> void:
	if (history.has_redo()):
		history.redo()
		print("重放: ", history.get_current_action_name())

## 便捷封装：创建一个新动作
func create_action(action_name: String) -> void:
	history.create_action(action_name)

## 提交动作
func commit() -> void:
	history.commit_action()
