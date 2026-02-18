extends Node

signal output_content(content: String);
signal program_endded();

const RUNNER_PATH : String = "runner/";

var runner_path : String;
var program_id : int = -1;
var program_io : FileAccess;

# 线程相关
var read_thread : Thread;
var is_monitoring : bool = false;

func _ready() -> void:
	runner_path = EditorConfigureManager.get_data_path().path_join(RUNNER_PATH);
	if (!DirAccess.dir_exists_absolute(runner_path)):
		DirAccess.make_dir_recursive_absolute(runner_path);

func _exit_tree() -> void:
	kill_runner();

func execute_runner(_cmd : PackedStringArray) -> Dictionary:
	if (program_id != -1):
		kill_runner();
	var dict : Dictionary = \
			OS.execute_with_pipe(runner_path.path_join(OS.get_name()), _cmd);
	program_id = dict.get("pid", -1);
	program_io = dict.get("stdio");
	if (program_id != -1 && program_io != null):
		is_monitoring = true;
		read_thread = Thread.new();
		read_thread.start(_thread_read_loop);
	return dict;

# 线程主循环：完全脱离渲染帧率，实时抽干管道
func _thread_read_loop() -> void:
	while (is_monitoring):
		if (program_io != null && program_io.get_error() == OK):
			while (program_io.get_position() < program_io.get_length()):
				var available : int = program_io.get_length() - program_io.get_position();
				if (available > 0):
					var buffer : PackedByteArray = program_io.get_buffer(available);
					var text : String = buffer.get_string_from_utf8();
					if (text != ""):
						output_content.emit.call_deferred(text);
				else:
					break;
		if (!OS.is_process_running(program_id)):
			is_monitoring = false;
			break;
		#OS.delay_msec(1);
	# 线程退出后的收尾工作
	call_deferred("_on_program_exited");

func _on_program_exited():
	is_monitoring = false;
	program_id = -1;
	program_io = null;
	program_endded.emit();
	if (read_thread != null):
		if (read_thread.is_alive()):
			read_thread.wait_to_finish();
		read_thread = null;

func kill_runner():
	if (program_id < 0):
		return;
	is_monitoring = false; # 先通知线程停止
	OS.kill(program_id);
	if (read_thread != null):
		read_thread.wait_to_finish();
		read_thread = null;
	_on_program_exited();
