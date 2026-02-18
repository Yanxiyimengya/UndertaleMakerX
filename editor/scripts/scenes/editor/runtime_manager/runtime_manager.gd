extends Node;

@onready var editor: Panel = $"..";
@onready var play_project_button: Button = %PlayProjectButton;
@onready var stop_project_button: Button = %StopProjectButton;
@export var console : EditorConsole;

var is_running : bool = false : 
	set(value) :
		is_running = value;
		if (value) : 
			play_project_button.disabled = true;
			stop_project_button.disabled = false;
		else : 
			play_project_button.disabled = false;
			stop_project_button.disabled = true;
			

func _ready() -> void:
	is_running = false;
	play_project_button.pressed.connect(_on_play_project_button_pressed);
	stop_project_button.pressed.connect(GlobalEditorRunnerManager.kill_runner);
	GlobalEditorRunnerManager.output_content.connect(_on_runner_output);
	GlobalEditorRunnerManager.program_endded.connect(_on_runner_ended);

func _on_play_project_button_pressed() -> void : 
	if (!is_running) :
		is_running = true;
		console.clear();
		var pck_name : String = EditorProjectManager.opened_project.project_name + ".pck";
		var output_dir = EditorConfigureManager.get_data_path().path_join(".build_cache");
		var output : String = output_dir.path_join(pck_name);
		if (!DirAccess.dir_exists_absolute(output_dir)) :
			DirAccess.make_dir_recursive_absolute(output_dir);
		UtmxPackPicker.pick_pack(editor.root_path, output);
		GlobalEditorRunnerManager.execute_runner(["--pack="+output]);

func _on_runner_output(msg : String) : 
	console.push_message(msg);

func _on_runner_ended() : 
	is_running = false;
