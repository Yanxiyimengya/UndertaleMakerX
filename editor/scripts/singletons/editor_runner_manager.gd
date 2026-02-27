extends Node

signal output_content(content: String)
signal stderr_content(content: String)
signal program_endded

const RUNNER_PATH: String = "runner/"
const WINDOWS_EXECUTABLE_SUFFIX: String = ".exe"

var runner_path: String
var program_id: int = -1
var program_io: FileAccess
var program_stderr_io: FileAccess

# 线程相关
var read_thread: Thread
var is_monitoring: bool = false


func _ready() -> void:
	runner_path = EditorConfigureManager.get_data_path().path_join(RUNNER_PATH)
	if !DirAccess.dir_exists_absolute(runner_path):
		DirAccess.make_dir_recursive_absolute(runner_path)


func _exit_tree() -> void:
	kill_runner()


func get_runner_executable_path() -> String:
	return get_runner_executable_path_for_platform(OS.get_name())


func get_runner_executable_path_for_platform(platform_name: String) -> String:
	var raw_platform: String = String(platform_name).strip_edges()
	if raw_platform.is_empty():
		raw_platform = String(OS.get_name())
	var normalized_platform: String = raw_platform.to_lower()
	var base_candidates: PackedStringArray = [
		runner_path.path_join(raw_platform),
		runner_path.path_join(normalized_platform),
	]
	match normalized_platform:
		"windows":
			for base_path: String in base_candidates:
				var windows_runner: String = base_path + WINDOWS_EXECUTABLE_SUFFIX
				if FileAccess.file_exists(windows_runner):
					return windows_runner
				if FileAccess.file_exists(base_path):
					return base_path
			return base_candidates[0] + WINDOWS_EXECUTABLE_SUFFIX
		"android":
			for base_path: String in base_candidates:
				var android_candidates: PackedStringArray = PackedStringArray(
					[
						base_path + ".apk",
						_normalize_path(base_path.get_base_dir().path_join("Android.apk")),
						_normalize_path(base_path.get_base_dir().path_join("android.apk")),
					]
				)
				for android_runner: String in android_candidates:
					if FileAccess.file_exists(android_runner):
						return android_runner
				if FileAccess.file_exists(base_path):
					return base_path
			return base_candidates[0] + ".apk"
		_:
			for base_path: String in base_candidates:
				if FileAccess.file_exists(base_path):
					return base_path
			return base_candidates[0]


func execute_runner(_cmd: PackedStringArray) -> Dictionary:
	if program_id != -1:
		kill_runner()
	#_cmd.append("--verbose");
	var dict: Dictionary = OS.execute_with_pipe(get_runner_executable_path(), _cmd)
	program_id = dict.get("pid", -1)
	program_io = dict.get("stdio")
	program_stderr_io = dict.get("stderr")
	if program_id != -1 && (program_io != null || program_stderr_io != null):
		is_monitoring = true
		read_thread = Thread.new()
		read_thread.start(_thread_read_loop)
	return dict


# 线程主循环：完全脱离渲染帧率，实时抽干管道
func _thread_read_loop() -> void:
	while is_monitoring:
		if program_io != null && program_io.get_error() == OK:
			while program_io.get_position() < program_io.get_length():
				var available: int = program_io.get_length() - program_io.get_position()
				if available > 0:
					var buffer: PackedByteArray = program_io.get_buffer(available)
					var text: String = buffer.get_string_from_utf8()
					if text != "":
						output_content.emit.call_deferred(text)
				else:
					break
		if program_stderr_io != null && program_stderr_io.get_error() == OK:
			while program_stderr_io.get_position() < program_stderr_io.get_length():
				var available_stderr: int = (
					program_stderr_io.get_length() - program_stderr_io.get_position()
				)
				if available_stderr > 0:
					var stderr_buffer: PackedByteArray = program_stderr_io.get_buffer(
						available_stderr
					)
					var stderr_text: String = stderr_buffer.get_string_from_utf8()
					if stderr_text != "":
						stderr_content.emit.call_deferred(stderr_text)
				else:
					break
		if !OS.is_process_running(program_id):
			is_monitoring = false
			break
		#OS.delay_msec(1);
	# 线程退出后的收尾工作
	call_deferred("_on_program_exited")


func _on_program_exited():
	is_monitoring = false
	program_id = -1
	program_io = null
	program_stderr_io = null
	program_endded.emit()
	if read_thread != null:
		if read_thread.is_alive():
			read_thread.wait_to_finish()
		read_thread = null


func kill_runner():
	if program_id < 0:
		return
	is_monitoring = false  # 先通知线程停止
	OS.kill(program_id)
	if read_thread != null:
		read_thread.wait_to_finish()
		read_thread = null
	_on_program_exited()


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()
