@tool
extends UtmxEditorWindow;

signal choiced(result : int);

@onready var message_label: Label = %MessageLabel;
@onready var question_window_content: PanelContainer = $QuestionWindowContent;

@onready var yes_button: Button = %YesButton;
@onready var no_button: Button = %NoButton;
@onready var cancel_button: Button = %CancelButton;

@export_multiline()
var message : String = "" : 
	set(value) : 
		message = value;
		if (!is_node_ready()) : await ready;
		await get_tree().process_frame;
		message_label.text = message;
		size = Vector2.ZERO;
		min_size = question_window_content.get_minimum_size();

var yes_text : String = "" : 
	set(value) : 
		yes_text = value;
		if (!is_node_ready()) : await ready;
		yes_button.text = message;

var no_text : String = "" : 
	set(value) : 
		no_text = value;
		if (!is_node_ready()) : await ready;
		no_button.text = message;

var cancel_text : String = "" : 
	set(value) : 
		cancel_text = value;
		if (!is_node_ready()) : await ready;
		cancel_button.text = message;

func _open() -> void:
	min_size = Vector2.ZERO;
	yes_button.grab_focus();

func _on_yes_button_pressed() -> void:
	choiced.emit(1);
	self.close();

func _on_no_button_pressed() -> void:
	choiced.emit(0);
	self.close();

func _on_cancel_button_pressed() -> void:
	close_requested.emit(2);
	self.close();
