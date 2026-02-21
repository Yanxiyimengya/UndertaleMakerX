class_name UtmxPackPicker
extends Node

const PACK_ROOT_RES: String = "res://__data__"
const BUILD_CACHE_DIR_NAME: String = ".build_cache"

const IGNORE_DIRS: PackedStringArray = [
	".git",
	".godot",
	".import"
]

const ignore_files: PackedStringArray = [
	EditorProjectManager.PROJECT_CONFIG_FILE_NAME
]

const ignore_ext: PackedStringArray = [
	"import",
	"uid"
]

static var _build_records: Array = []
static var _last_build_record: Dictionary = {}


static func pick_pack(path: String, output: String) -> void:
	if path.is_empty() or output.is_empty():
		push_error("PickPack: path or output is empty.")
		return

	var root_path := _trim_trailing_slash(_normalize_path(path))
	if !DirAccess.dir_exists_absolute(root_path):
		push_error("PickPack: root path not found -> %s" % root_path)
		return

	var output_path := _normalize_path(output)
	var output_dir := output_path.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_output_err: int = int(DirAccess.make_dir_recursive_absolute(output_dir))
		if mkdir_output_err != OK:
			push_error("PickPack: failed to create output directory -> %s (%d)" % [output_dir, mkdir_output_err])
			return

	var build_cache_root := _build_cache_root()
	if !build_cache_root.is_empty() and !DirAccess.dir_exists_absolute(build_cache_root):
		DirAccess.make_dir_recursive_absolute(build_cache_root)

	var packer := PCKPacker.new()
	var start_err: int = int(packer.pck_start(output_path))
	if start_err != OK:
		push_error("PickPack: failed to start pack (%d)." % start_err)
		_record_build(root_path, output_path, [], [], start_err)
		return

	var packed_targets := {}
	_recursive_pack(root_path, root_path, packer, packed_targets)

	var flush_err: int = int(packer.flush())
	_record_build(root_path, output_path, [], packed_targets.keys(), flush_err)
	if flush_err != OK:
		push_error("PickPack: flush failed (%d)." % flush_err)
		return


static func get_last_build_record() -> Dictionary:
	return _last_build_record.duplicate(true)


static func get_build_records() -> Array:
	return _build_records.duplicate(true)

static func destroy_temporary_resources(remove_pck: bool = false, only_pck_path: String = "") -> void:
	var normalized_filter := _normalize_path(only_pck_path)
	for i in range(_build_records.size() - 1, -1, -1):
		var record: Dictionary = _build_records[i]
		var pck_path := _normalize_path(str(record.get("pck_path", "")))
		if !normalized_filter.is_empty() and pck_path != normalized_filter:
			continue
		if remove_pck and FileAccess.file_exists(pck_path):
			DirAccess.remove_absolute(pck_path)
		_build_records.remove_at(i)

	if _last_build_record.is_empty():
		return

	var last_pck_path := _normalize_path(str(_last_build_record.get("pck_path", "")))
	if normalized_filter.is_empty() or last_pck_path == normalized_filter:
		_last_build_record = {}


static func _recursive_pack(root_path: String, current_path: String, packer: PCKPacker, packed_targets: Dictionary) -> void:
	var dir := DirAccess.open(current_path)
	if dir == null:
		push_warning("PickPack: cannot open directory -> %s" % current_path)
		return

	dir.list_dir_begin()
	var f_name := dir.get_next()
	while !f_name.is_empty():
		if f_name != "." and f_name != "..":
			var full_path := _normalize_path(current_path.path_join(f_name))
			if dir.current_is_dir():
				if !(f_name in IGNORE_DIRS):
					_recursive_pack(root_path, full_path, packer, packed_targets)
			else:
				_process_file(root_path, full_path, packer, packed_targets)
		f_name = dir.get_next()
	dir.list_dir_end()


static func _process_file(root_path: String, source_path: String, packer: PCKPacker, packed_targets: Dictionary) -> void:
	var relative_path := _make_relative_path(root_path, source_path)
	if relative_path.is_empty():
		return

	var file_name := relative_path.get_file()
	var ext := relative_path.get_extension().to_lower()
	if file_name in ignore_files:
		return
	if ext in ignore_ext:
		return

	var target_res_path := _to_data_res_path(relative_path)
	_pack_file_once(target_res_path, source_path, packer, packed_targets)


static func _pack_file_once(target_res_path: String, source_abs_path: String, packer: PCKPacker, packed_targets: Dictionary) -> void:
	var normalized_target := _normalize_path(target_res_path)
	if packed_targets.has(normalized_target):
		return

	var normalized_source := _normalize_path(source_abs_path)
	if !FileAccess.file_exists(normalized_source):
		push_warning("PickPack: missing source -> %s" % normalized_source)
		return

	var err: int = int(packer.add_file(normalized_target, normalized_source))
	if err != OK:
		push_error("PickPack: add_file failed (%d): %s <- %s" % [err, normalized_target, normalized_source])
		return

	packed_targets[normalized_target] = true


static func _record_build(
	root_path: String,
	output_path: String,
	temp_resources: Array,
	packed_entries: Array,
	result_code: int
) -> void:
	var record := {
		"root_path": root_path,
		"pck_path": output_path,
		"temp_resources": temp_resources.duplicate(),
		"packed_entries": packed_entries.duplicate(),
		"result_code": result_code,
		"created_at_unix": Time.get_unix_time_from_system()
	}
	_last_build_record = record.duplicate(true)
	_build_records.append(record.duplicate(true))


static func _build_cache_root() -> String:
	var base_data_path := _normalize_path(EditorConfigureManager.get_data_path())
	if base_data_path.is_empty():
		base_data_path = _normalize_path(ProjectSettings.globalize_path("user://"))
	return _normalize_path(base_data_path.path_join(BUILD_CACHE_DIR_NAME))


static func _to_data_res_path(relative_path: String) -> String:
	var clean := _normalize_path(relative_path).trim_prefix("/")
	return _normalize_path(PACK_ROOT_RES.path_join(clean))


static func _make_relative_path(root_path: String, full_path: String) -> String:
	var root := _trim_trailing_slash(_normalize_path(root_path))
	var full := _normalize_path(full_path)
	var prefix := root + "/"
	if full.begins_with(prefix):
		return full.trim_prefix(prefix)
	if full == root:
		return ""
	return full


static func _normalize_path(path: String) -> String:
	return path.replace("\\", "/")


static func _trim_trailing_slash(path: String) -> String:
	var out := _normalize_path(path)
	while out.ends_with("/") and out.length() > 1:
		out = out.substr(0, out.length() - 1)
	return out
