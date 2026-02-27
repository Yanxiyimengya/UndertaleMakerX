class_name EditorConsole extends PanelContainer

@onready var rich_text_label: RichTextLabel = %RichTextLabel
@onready var clear_console_button: Button = %ClearConsoleButton
@onready var copy_console_button: Button = %CopyConsoleButton

var _output_manager: Node = null


func _ready() -> void:
	clear_console_button.pressed.connect(clear)
	copy_console_button.pressed.connect(copy_conent)
	_connect_output_manager()


func _exit_tree() -> void:
	_disconnect_output_manager()


func push_message(...msgs: Array[Variant]):
	for msg: Variant in msgs:
		rich_text_label.append_text(str(msg))


func clear():
	rich_text_label.clear()


func copy_conent():
	DisplayServer.clipboard_set(rich_text_label.get_parsed_text())
	pass


func _connect_output_manager() -> void:
	_disconnect_output_manager()
	_output_manager = get_node_or_null("/root/EditorOutputManager")
	if _output_manager == null or !_output_manager.has_signal("message_emitted"):
		return
	var callback: Callable = Callable(self, "_on_output_message_emitted")
	if !_output_manager.is_connected("message_emitted", callback):
		_output_manager.connect("message_emitted", callback)


func _disconnect_output_manager() -> void:
	if _output_manager == null:
		return
	var callback: Callable = Callable(self, "_on_output_message_emitted")
	if _output_manager.is_connected("message_emitted", callback):
		_output_manager.disconnect("message_emitted", callback)
	_output_manager = null


func _on_output_message_emitted(level: String, message: String) -> void:
	var plain_message: String = String(message)
	if plain_message.is_empty():
		return

	var normalized_level: String = String(level).strip_edges().to_lower()
	var bbcode_message: String = plain_message
	match normalized_level:
		"error":
			bbcode_message = "[color=red]%s[/color]" % plain_message
		"warning":
			bbcode_message = "[color=yellow]%s[/color]" % plain_message
	push_message(bbcode_message, "\n")
