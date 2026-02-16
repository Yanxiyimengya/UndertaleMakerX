extends Node;

@onready var play_project_button: Button = %PlayProjectButton;
@onready var stop_project_button: Button = %StopProjectButton;

var is_running : bool = false : 
	set(value) :
		is_running = value;
		if (value) : 
			stop_project_button.disabled = false;
		else : 
			stop_project_button.disabled = true;
			

func _ready() -> void:
	is_running = false;
