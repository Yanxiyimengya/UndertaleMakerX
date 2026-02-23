class_name ProgramExporter
extends RefCounted

const GODOT_PCK_MAGIC: int = 0x43504447
const UTMX_PCK_MAGIC: int = 0x4B505455
const GODOT_TAIL_SIZE: int = 12
const STREAM_CHUNK_SIZE: int = 1024 * 1024
const WINDOWS_EXECUTABLE_SUFFIX: String = ".exe"


## Build an executable with embedded UTMX patch pack.
## Final layout:
## [raw_exe + original_pck_data][utmx_patch_pck][utmx_patch_size(8)][utmx_magic(4)]
## [original_pck_size + utmx_patch_size + 4 (8, little endian)][godot_magic(4)]
static func export_windows_embedded(
	project_root_path: String, source_executable_path: String, output_executable_path: String
) -> Dictionary:
	var project_root := _trim_trailing_slash(_normalize_path(project_root_path))
	var source_exe := _resolve_windows_source_executable_path(source_executable_path)
	var output_exe := _resolve_output_executable_path(output_executable_path, project_root)

	if project_root.is_empty() or !DirAccess.dir_exists_absolute(project_root):
		return _error_result("Project root is invalid: %s" % project_root)
	if source_exe.is_empty() or !FileAccess.file_exists(source_exe):
		return _error_result("Source executable is missing: %s" % source_exe)
	if output_exe.is_empty():
		return _error_result("Output executable path is empty.")
	if source_exe == output_exe:
		return _error_result("Output executable must be different from source executable.")

	var output_dir := output_exe.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_err := DirAccess.make_dir_recursive_absolute(output_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create output directory: %s (err=%d)" % [output_dir, mkdir_err]
			)

	var source_meta := _read_source_tail_metadata(source_exe)
	if !bool(source_meta.get("success", false)):
		return source_meta
	var original_pck_size: int = int(source_meta.get("original_pck_size", -1))
	if original_pck_size < 0:
		return _error_result("Source executable contains invalid original PCK size.")

	var temp_patch_pck_path := _build_temp_patch_path()
	var patch_result := _build_patch_pck(project_root, temp_patch_pck_path)
	if !bool(patch_result.get("success", false)):
		_try_remove_file(temp_patch_pck_path)
		return patch_result

	var patch_size: int = int(patch_result.get("patch_pck_size", -1))
	if patch_size < 0:
		_try_remove_file(temp_patch_pck_path)
		return _error_result("Patch PCK size is invalid.")

	var relocated_pck_size: int = original_pck_size + patch_size + 4
	if relocated_pck_size < 0:
		_try_remove_file(temp_patch_pck_path)
		return _error_result("Relocated PCK size overflow.")

	var write_result := _write_embedded_output(
		source_exe, temp_patch_pck_path, output_exe, patch_size, relocated_pck_size
	)
	_try_remove_file(temp_patch_pck_path)
	if !bool(write_result.get("success", false)):
		_try_remove_file(output_exe)
		return write_result

	return {
		"success": true,
		"project_root": project_root,
		"source_executable": source_exe,
		"output_executable": output_exe,
		"original_pck_size": original_pck_size,
		"utmx_patch_pck_size": patch_size,
		"relocated_pck_size": relocated_pck_size,
	}


static func _resolve_windows_source_executable_path(raw_source_path: String) -> String:
	var source_path: String = _normalize_path(raw_source_path.strip_edges())
	if source_path.is_empty():
		return ""

	var candidates: PackedStringArray = []
	if source_path.get_extension().to_lower() == "exe":
		candidates.append(source_path)
	else:
		candidates.append(source_path + WINDOWS_EXECUTABLE_SUFFIX)
		candidates.append(source_path)

	if DirAccess.dir_exists_absolute(source_path):
		candidates.append(
			_normalize_path(source_path.path_join("Windows" + WINDOWS_EXECUTABLE_SUFFIX))
		)
		candidates.append(
			_normalize_path(source_path.path_join("windows" + WINDOWS_EXECUTABLE_SUFFIX))
		)

	for candidate: String in candidates:
		if FileAccess.file_exists(candidate):
			return candidate

	return candidates[0] if (!candidates.is_empty()) else source_path


static func _resolve_output_executable_path(
	raw_output_path: String, project_root: String
) -> String:
	var output_path := _normalize_path(raw_output_path.strip_edges())
	if output_path.is_empty():
		return ""
	if output_path.ends_with("/") || DirAccess.dir_exists_absolute(output_path):
		return _normalize_path(output_path.path_join(_build_default_output_name(project_root)))
	if output_path.get_extension().to_lower() != "exe":
		output_path += ".exe"
	return _normalize_path(output_path)


static func _build_default_output_name(project_root: String) -> String:
	var project_name: String = ""
	if is_instance_valid(EditorProjectManager.opened_project):
		project_name = String(EditorProjectManager.opened_project.project_name).strip_edges()
	if project_name.is_empty():
		project_name = _trim_trailing_slash(_normalize_path(project_root)).get_file()
	if project_name.is_empty():
		project_name = "game"
	return project_name + ".exe"


static func _build_patch_pck(project_root: String, output_pck_path: String) -> Dictionary:
	var output_dir := output_pck_path.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_err := DirAccess.make_dir_recursive_absolute(output_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create patch directory: %s (err=%d)" % [output_dir, mkdir_err]
			)
	UtmxPackPicker.pick_pack(project_root, output_pck_path)
	var build_record := _find_picker_record(output_pck_path)
	if !build_record.is_empty():
		var result_code: int = int(build_record.get("result_code", FAILED))
		if result_code != OK:
			return _error_result("Build patch pack failed with result code: %d" % result_code)
		var record_root := _trim_trailing_slash(
			_normalize_path(str(build_record.get("root_path", "")))
		)
		if (
			!record_root.is_empty()
			and record_root != _trim_trailing_slash(_normalize_path(project_root))
		):
			return _error_result("Build record root mismatch: %s" % record_root)

	if !FileAccess.file_exists(output_pck_path):
		return _error_result("Patch PCK was not created: %s" % output_pck_path)

	var patch_file := FileAccess.open(output_pck_path, FileAccess.READ)
	if patch_file == null:
		return _error_result("Failed to open patch PCK: %s" % output_pck_path)
	var patch_size: int = int(patch_file.get_length())
	patch_file.close()

	if patch_size <= 0:
		return _error_result("Patch PCK is empty: %s" % output_pck_path)

	return {
		"success": true,
		"patch_pck_path": output_pck_path,
		"patch_pck_size": patch_size,
		"build_record": build_record,
	}


static func _find_picker_record(output_pck_path: String) -> Dictionary:
	var target := _normalize_path(output_pck_path)
	var records_variant: Variant = UtmxPackPicker.get_build_records()
	var records: Array = []
	if records_variant is Array:
		records = records_variant
	for i in range(records.size() - 1, -1, -1):
		var entry: Variant = records[i]
		if !(entry is Dictionary):
			continue
		var record := entry as Dictionary
		var pck_path := _normalize_path(str(record.get("pck_path", "")))
		if pck_path == target:
			return record.duplicate(true)

	var last_record_variant: Variant = UtmxPackPicker.get_last_build_record()
	var last_record: Dictionary = {}
	if last_record_variant is Dictionary:
		last_record = last_record_variant
	var last_pck_path := _normalize_path(str(last_record.get("pck_path", "")))
	if !last_record.is_empty() and last_pck_path == target:
		return last_record.duplicate(true)
	return {}


static func _write_embedded_output(
	source_exe: String,
	patch_pck_path: String,
	output_exe: String,
	patch_size: int,
	relocated_pck_size: int
) -> Dictionary:
	var in_exe := FileAccess.open(source_exe, FileAccess.READ)
	if in_exe == null:
		return _error_result("Failed to open source executable: %s" % source_exe)

	var in_patch := FileAccess.open(patch_pck_path, FileAccess.READ)
	if in_patch == null:
		in_exe.close()
		return _error_result("Failed to open patch PCK: %s" % patch_pck_path)

	var out_file := FileAccess.open(output_exe, FileAccess.WRITE)
	if out_file == null:
		in_exe.close()
		in_patch.close()
		return _error_result("Failed to create output executable: %s" % output_exe)

	var source_size: int = int(in_exe.get_length())
	if source_size < GODOT_TAIL_SIZE:
		in_exe.close()
		in_patch.close()
		out_file.close()
		return _error_result("Source executable is too small to contain Godot trailer.")

	# Keep [raw exe + original pck] and drop original [pck_size + godot_magic] trailer.
	var source_payload_size: int = source_size - GODOT_TAIL_SIZE
	var copy_exe_err := _copy_file_stream(in_exe, out_file, source_payload_size)
	if copy_exe_err != OK:
		in_exe.close()
		in_patch.close()
		out_file.close()
		return _error_result("Failed while copying source executable (err=%d)." % copy_exe_err)

	var copy_patch_err := _copy_file_stream(in_patch, out_file)
	if copy_patch_err != OK:
		in_exe.close()
		in_patch.close()
		out_file.close()
		return _error_result("Failed while appending patch PCK (err=%d)." % copy_patch_err)

	out_file.store_buffer(_encode_u64_le(patch_size))
	out_file.store_buffer(_encode_u32_le(UTMX_PCK_MAGIC))
	# MUST be 8-byte little endian:
	# [original_pck_size + utmx_patch_size + 4]
	out_file.store_buffer(_encode_u64_le(relocated_pck_size))
	out_file.store_buffer(_encode_u32_le(GODOT_PCK_MAGIC))

	in_exe.close()
	in_patch.close()
	out_file.flush()
	out_file.close()

	return {"success": true}


static func _read_source_tail_metadata(source_exe: String) -> Dictionary:
	var file := FileAccess.open(source_exe, FileAccess.READ)
	if file == null:
		return _error_result("Failed to open source executable: %s" % source_exe)

	var file_size: int = int(file.get_length())
	if file_size < GODOT_TAIL_SIZE:
		file.close()
		return _error_result("Source executable is too small: %s" % source_exe)

	file.seek(file_size - GODOT_TAIL_SIZE)
	var tail_data := file.get_buffer(GODOT_TAIL_SIZE)
	file.close()
	if tail_data.size() != GODOT_TAIL_SIZE:
		return _error_result("Failed to read source executable trailer.")

	var original_pck_size: int = _decode_u64_le(tail_data, 0)
	var tail_magic: int = _decode_u32_le(tail_data, 8)
	if tail_magic != GODOT_PCK_MAGIC:
		return _error_result("Source executable does not end with GODOT PCK magic.")
	if original_pck_size < 0 or original_pck_size > file_size - GODOT_TAIL_SIZE:
		return _error_result("Source executable has invalid original PCK size.")

	return {
		"success": true,
		"original_pck_size": original_pck_size,
	}


static func _copy_file_stream(src: FileAccess, dst: FileAccess, max_bytes: int = -1) -> int:
	var bytes_left: int = max_bytes
	if bytes_left < 0:
		bytes_left = int(src.get_length() - src.get_position())

	while bytes_left > 0:
		var remaining: int = int(src.get_length() - src.get_position())
		if remaining <= 0:
			return FAILED
		var chunk_size: int = mini(STREAM_CHUNK_SIZE, remaining)
		chunk_size = mini(chunk_size, bytes_left)
		if chunk_size <= 0:
			return FAILED
		var buffer := src.get_buffer(chunk_size)
		if buffer.size() != chunk_size:
			return FAILED
		dst.store_buffer(buffer)
		bytes_left -= chunk_size
	return OK


static func _build_temp_patch_path() -> String:
	var base_dir := _get_editor_data_path().path_join(".build_cache").path_join("export_patch")
	if !DirAccess.dir_exists_absolute(base_dir):
		DirAccess.make_dir_recursive_absolute(base_dir)
	var file_name := "utmx_patch_%d.pck" % Time.get_ticks_usec()
	return _normalize_path(base_dir.path_join(file_name))


static func _get_editor_data_path() -> String:
	var main_loop := Engine.get_main_loop()
	if main_loop is SceneTree:
		var tree := main_loop as SceneTree
		if tree.root != null:
			var configure_manager := tree.root.get_node_or_null("EditorConfigureManager")
			if configure_manager != null and configure_manager.has_method("get_data_path"):
				var value: Variant = configure_manager.call("get_data_path")
				if value is String and !String(value).is_empty():
					return _normalize_path(String(value))
	return _normalize_path(ProjectSettings.globalize_path("user://"))


static func _encode_u64_le(value: int) -> PackedByteArray:
	var out := PackedByteArray()
	out.resize(8)
	var v: int = value
	for i in range(8):
		out[i] = v & 0xFF
		v >>= 8
	return out


static func _encode_u32_le(value: int) -> PackedByteArray:
	var out := PackedByteArray()
	out.resize(4)
	var v: int = value
	for i in range(4):
		out[i] = v & 0xFF
		v >>= 8
	return out


static func _decode_u64_le(buffer: PackedByteArray, offset: int) -> int:
	if offset < 0 or offset + 8 > buffer.size():
		return -1
	var value: int = 0
	for i in range(8):
		value |= int(buffer[offset + i]) << (i * 8)
	return value


static func _decode_u32_le(buffer: PackedByteArray, offset: int) -> int:
	if offset < 0 or offset + 4 > buffer.size():
		return -1
	var value: int = 0
	for i in range(4):
		value |= int(buffer[offset + i]) << (i * 8)
	return value


static func _try_remove_file(path: String) -> void:
	if path.is_empty():
		return
	if FileAccess.file_exists(path):
		DirAccess.remove_absolute(path)


static func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()


static func _trim_trailing_slash(path: String) -> String:
	var out := _normalize_path(path)
	while out.ends_with("/") and out.length() > 1:
		out = out.substr(0, out.length() - 1)
	return out


static func _error_result(message: String) -> Dictionary:
	push_error("ProgramExporter: " + message)
	return {
		"success": false,
		"error": message,
	}
