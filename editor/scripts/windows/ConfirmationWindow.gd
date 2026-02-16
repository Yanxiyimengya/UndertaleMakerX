@tool
extends UtmxEditorWindow;

signal choiced(confirm : bool);

@onready var message_label: Label = %MessageLabel;
@onready var confirmation_window_content: PanelContainer = $ConfirmationWindowContent;

@export_multiline()
var message : String = "" : 
	set(value) : 
		message = value;
		if (!is_node_ready()) : await ready;
		message_label.text = message;
		min_size.x = max(min_size.x, confirmation_window_content.get_minimum_size().x);
		min_size.y = max(min_size.y, confirmation_window_content.get_minimum_size().y);

func _open() -> void:
	pass;

func _on_confirm_button_pressed() -> void:
	choiced.emit(true);
	self.close();

func _on_cancel_button_pressed() -> void:
	close_requested.emit();
	choiced.emit(false);
	self.close();

func _on_close_requested() -> void:
	choiced.emit(false);
