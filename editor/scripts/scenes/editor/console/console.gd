class_name EditorConsole extends PanelContainer;

@onready var rich_text_label: RichTextLabel = %RichTextLabel;
@onready var clear_console_button: Button = %ClearConsoleButton;
@onready var copy_console_button: Button = %CopyConsoleButton;

func _ready() -> void:
	clear_console_button.pressed.connect(clear);
	copy_console_button.pressed.connect(copy_conent)

func push_message(...msgs : Array[Variant]) : 
	for msg : Variant in msgs : 
		rich_text_label.append_text(str(msg));

func clear() : 
	rich_text_label.clear();

func copy_conent() : 
	DisplayServer.clipboard_set(rich_text_label.get_parsed_text());
	pass;
