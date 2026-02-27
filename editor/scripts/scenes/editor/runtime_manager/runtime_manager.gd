extends Node

@onready var editor: Panel = $".."
@onready var play_project_button: Button = %PlayProjectButton
@onready var stop_project_button: Button = %StopProjectButton
@export var console: EditorConsole

var is_running: bool = false:
	set(value):
		is_running = value
		if value:
			play_project_button.disabled = true
			stop_project_button.disabled = false
		else:
			play_project_button.disabled = false
			stop_project_button.disabled = true


func _ready() -> void:
	is_running = false
	play_project_button.pressed.connect(_on_play_project_button_pressed)
	stop_project_button.pressed.connect(GlobalEditorRunnerManager.kill_runner)
	GlobalEditorRunnerManager.output_content.connect(_on_runner_output)
	GlobalEditorRunnerManager.stderr_content.connect(_on_runner_stderr)
	GlobalEditorRunnerManager.program_endded.connect(_on_runner_ended)


func _exit_tree() -> void:
	if GlobalEditorRunnerManager.output_content.is_connected(_on_runner_output):
		GlobalEditorRunnerManager.output_content.disconnect(_on_runner_output)
	if GlobalEditorRunnerManager.stderr_content.is_connected(_on_runner_stderr):
		GlobalEditorRunnerManager.stderr_content.disconnect(_on_runner_stderr)
	if GlobalEditorRunnerManager.program_endded.is_connected(_on_runner_ended):
		GlobalEditorRunnerManager.program_endded.disconnect(_on_runner_ended)
	is_running = false


func _on_play_project_button_pressed() -> void:
	if !is_running:
		is_running = true
		_save_open_scripts_before_run()
		if console != null:
			console.clear()
		var pck_name: String = EditorProjectManager.opened_project.project_name + ".pck"
		var output_dir = EditorConfigureManager.get_data_path().path_join(".build_cache")
		var output: String = output_dir.path_join(pck_name)
		if !DirAccess.dir_exists_absolute(output_dir):
			var make_output_dir_err: int = DirAccess.make_dir_recursive_absolute(output_dir)
			if make_output_dir_err != OK:
				is_running = false
				_report_runtime_error(
					"Runtime build cache directory create failed: %s (%d)"
					% [output_dir, make_output_dir_err]
				)
				return
		UtmxPackPicker.pick_pack(editor.root_path, output)
		var build_record: Dictionary = UtmxPackPicker.get_last_build_record()
		var build_result_code: int = int(build_record.get("result_code", FAILED))
		if build_record.is_empty() or build_result_code != OK or !FileAccess.file_exists(output):
			is_running = false
			_report_runtime_error(
				"Runtime pack build failed (code=%d): %s" % [build_result_code, output]
			)
			return

		var execute_result: Dictionary = GlobalEditorRunnerManager.execute_runner(
			["--pack=" + output, "--debug-collisions=true"]
		)
		var pid: int = int(execute_result.get("pid", -1))
		if pid < 0:
			is_running = false
			var runner_error: String = String(execute_result.get("error", "")).strip_edges()
			var runner_status: String = String(execute_result.get("status", "")).strip_edges()
			var extra: String = ""
			if !runner_error.is_empty():
				extra = runner_error
			elif !runner_status.is_empty():
				extra = runner_status
			if extra.is_empty():
				_report_runtime_error("Runner failed to start.")
			else:
				_report_runtime_error("Runner failed to start: %s" % extra)
			return
		_push_output_message("Runner started: pid=%d" % pid, "info")


func _save_open_scripts_before_run() -> void:
	if editor == null:
		return
	var script_editor_node: Node = editor.get_node_or_null(
		"VBoxContainer/MarginContainer/MainDockable/Script"
	)
	if script_editor_node == null:
		return
	if script_editor_node.has_method("save_all_open_scripts"):
		# skip_deleted = true: don't restore files already deleted on disk.
		script_editor_node.call("save_all_open_scripts", true)


func _on_runner_output(msg: String):
	_push_output_message(msg, "info")


func _on_runner_stderr(msg: String):
	_push_output_message(msg, "error")


func _on_runner_ended():
	is_running = false


func _push_output_message(message: String, level: String = "info") -> void:
	var output_manager: Node = get_node_or_null("/root/EditorOutputManager")
	if output_manager == null || !output_manager.has_method("push"):
		return
	output_manager.call("push", String(message), String(level))


func _report_runtime_error(message: String) -> void:
	var text: String = String(message)
	push_error(text)
	_push_output_message(text, "error")
