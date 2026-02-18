@tool
extends UtmxEditorWindow;

signal choiced(confirm : bool);

@onready var message_label: Label = %MessageLabel;
@onready var confirmation_window_content: PanelContainer = $ConfirmationWindowContent;

@onready var confirm_button: Button = %ConfirmButton;
@onready var cancel_button: Button = %CancelButton;

@export_multiline()
var message : String = "" : 
	set(value) : 
		message = value;
		if (!is_node_ready()) : await ready;
		await get_tree().process_frame;
		message_label.text = message;
		size = Vector2.ZERO;
		min_size = confirmation_window_content.get_minimum_size();

func _open() -> void:
	min_size = Vector2.ZERO;
	confirm_button.grab_focus();

func _on_confirm_button_pressed() -> void:
	choiced.emit(true);
	self.close();

func _on_cancel_button_pressed() -> void:
	close_requested.emit();
	choiced.emit(false);
	self.close();
