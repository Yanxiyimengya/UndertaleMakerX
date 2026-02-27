class_name ProgramExporter
extends RefCounted

const GODOT_PCK_MAGIC: int = 0x43504447
const UTMX_PCK_MAGIC: int = 0x4B505455
const GODOT_TAIL_SIZE: int = 12
const STREAM_CHUNK_SIZE: int = 1024 * 1024
const WINDOWS_EXECUTABLE_SUFFIX: String = ".exe"
const APK_FILE_SUFFIX: String = ".apk"
const PLATFORM_WINDOWS: String = "windows"
const PLATFORM_LINUX: String = "linux"
const PLATFORM_ANDROID: String = "android"
const ANDROID_PACK_ENTRY_PATH: String = "assets/utmx.pck"


## Build an executable with embedded UTMX patch pack.
## Final layout:
## [raw_exe + original_pck_data][utmx_patch_pck][utmx_patch_size(8)][utmx_magic(4)]
## [original_pck_size + utmx_patch_size + 4 (8, little endian)][godot_magic(4)]
static func export_windows_embedded(
	project_root_path: String, source_executable_path: String, output_executable_path: String
) -> Dictionary:
	return export_embedded(
		project_root_path,
		source_executable_path,
		output_executable_path,
		PLATFORM_WINDOWS
	)


static func export_linux_embedded(
	project_root_path: String, source_executable_path: String, output_executable_path: String
) -> Dictionary:
	return export_embedded(
		project_root_path,
		source_executable_path,
		output_executable_path,
		PLATFORM_LINUX
	)


static func export_android_apk(
	project_root_path: String,
	source_apk_path: String,
	output_apk_path: String,
	signing_options: Dictionary
) -> Dictionary:
	var project_root := _trim_trailing_slash(_normalize_path(project_root_path))
	var source_apk := _resolve_source_executable_path(source_apk_path, PLATFORM_ANDROID)
	var output_apk := _resolve_output_executable_path(output_apk_path, project_root, PLATFORM_ANDROID)
	var requested_output_apk: String = output_apk
	output_apk = _resolve_android_output_apk_path(project_root, output_apk)
	if project_root.is_empty() or !DirAccess.dir_exists_absolute(project_root):
		return _error_result("Project root is invalid: %s" % project_root)
	if source_apk.is_empty() or !FileAccess.file_exists(source_apk):
		return _error_result("Android runner APK is missing: %s" % source_apk)
	if output_apk.is_empty():
		return _error_result("Output APK path is empty.")
	if _normalize_path(requested_output_apk) != _normalize_path(output_apk):
		_emit_output("Android APK output renamed to project name: %s" % output_apk, "info")
	if _normalize_path(source_apk) == _normalize_path(output_apk):
		return _error_result("Output APK must be different from source APK.")

	var output_dir: String = output_apk.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_err: int = DirAccess.make_dir_recursive_absolute(output_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create output directory: %s (err=%d)" % [output_dir, mkdir_err]
			)

	var signing_result: Dictionary = _validate_android_signing_options(signing_options)
	if !bool(signing_result.get("success", false)):
		return signing_result
	var apksigner_path: String = String(signing_result.get("apksigner_path", ""))
	var zipalign_path: String = String(signing_result.get("zipalign_path", ""))
	var jarsigner_path: String = String(signing_result.get("jarsigner_path", ""))
	var jar_tool_path: String = String(signing_result.get("jar_tool_path", ""))
	var keystore_path: String = String(signing_result.get("keystore_path", ""))
	var keystore_alias: String = String(signing_result.get("keystore_alias", ""))
	var keystore_password: String = String(signing_result.get("keystore_password", ""))
	var key_password: String = String(signing_result.get("key_password", ""))
	var project_icon_path: String = ""
	var project_app_name: String = ""
	var icon_path: String = ""
	if is_instance_valid(EditorProjectManager.opened_project):
		project_app_name = String(EditorProjectManager.opened_project.project_name).strip_edges()
	if is_instance_valid(EditorProjectManager.opened_project):
		icon_path = String(EditorProjectManager.opened_project.icon).strip_edges()
	if project_app_name.is_empty():
		project_app_name = _trim_trailing_slash(_normalize_path(project_root)).get_file()
	if project_app_name.is_empty():
		project_app_name = "game"
	if icon_path.is_empty():
		icon_path = "icon.svg"
	var normalized_icon_path: String = _normalize_path(icon_path)
	if normalized_icon_path.is_absolute_path() and FileAccess.file_exists(normalized_icon_path):
		project_icon_path = normalized_icon_path
	else:
		var relative_icon_path: String = normalized_icon_path.trim_prefix("/")
		var project_icon_candidate: String = _normalize_path(project_root.path_join(relative_icon_path))
		if FileAccess.file_exists(project_icon_candidate):
			project_icon_path = project_icon_candidate

	_emit_output("Preparing Android patch pack...", "info")
	var temp_patch_pck_path: String = _build_temp_patch_path()
	var patch_result: Dictionary = _build_patch_pck(
		project_root, temp_patch_pck_path, [output_apk]
	)
	if !bool(patch_result.get("success", false)):
		_try_remove_file(temp_patch_pck_path)
		return patch_result

	var temp_unsigned_apk_path: String = _build_temp_android_apk_path("unsigned")
	var temp_aligned_apk_path: String = _build_temp_android_apk_path("aligned")
	var temp_signed_apk_path: String = _build_temp_android_apk_path("signed")
	var inject_result: Dictionary = _inject_patch_into_android_apk(
		source_apk,
		temp_patch_pck_path,
		temp_unsigned_apk_path,
		jar_tool_path,
		project_icon_path,
		project_app_name
	)
	if !bool(inject_result.get("success", false)):
		_try_remove_file(temp_patch_pck_path)
		_try_remove_file(temp_unsigned_apk_path)
		_try_remove_file(temp_aligned_apk_path)
		_try_remove_file(temp_signed_apk_path)
		return inject_result

	var sign_result: Dictionary = {}
	if !apksigner_path.is_empty():
		var signing_input_apk: String = temp_unsigned_apk_path
		if !zipalign_path.is_empty():
			_emit_output("Aligning Android APK...", "info")
			var align_result: Dictionary = _zipalign_android_apk(
				temp_unsigned_apk_path, temp_aligned_apk_path, zipalign_path
			)
			if !bool(align_result.get("success", false)):
				_try_remove_file(temp_patch_pck_path)
				_try_remove_file(temp_unsigned_apk_path)
				_try_remove_file(temp_aligned_apk_path)
				_try_remove_file(temp_signed_apk_path)
				return align_result
			signing_input_apk = temp_aligned_apk_path

		_emit_output("Signing Android APK with apksigner...", "info")
		sign_result = _sign_android_apk_with_apksigner(
			signing_input_apk,
			output_apk,
			apksigner_path,
			keystore_path,
			keystore_alias,
			keystore_password,
			key_password
		)
	else:
		_emit_output("Signing Android APK with jarsigner...", "info")
		var jarsigner_output_apk: String = output_apk
		if !zipalign_path.is_empty():
			jarsigner_output_apk = temp_signed_apk_path
		sign_result = _sign_android_apk_with_jarsigner(
			temp_unsigned_apk_path,
			jarsigner_output_apk,
			jarsigner_path,
			keystore_path,
			keystore_alias,
			keystore_password,
			key_password
		)
	if !bool(sign_result.get("success", false)):
		_try_remove_file(temp_patch_pck_path)
		_try_remove_file(temp_unsigned_apk_path)
		_try_remove_file(temp_aligned_apk_path)
		_try_remove_file(temp_signed_apk_path)
		return sign_result

	if apksigner_path.is_empty() and !zipalign_path.is_empty():
		_emit_output("Aligning Android APK...", "info")
		var jarsigner_align_result: Dictionary = _zipalign_android_apk(
			temp_signed_apk_path, output_apk, zipalign_path
		)
		if !bool(jarsigner_align_result.get("success", false)):
			_try_remove_file(temp_patch_pck_path)
			_try_remove_file(temp_unsigned_apk_path)
			_try_remove_file(temp_aligned_apk_path)
			_try_remove_file(temp_signed_apk_path)
			return jarsigner_align_result

	_try_remove_file(temp_patch_pck_path)
	_try_remove_file(temp_unsigned_apk_path)
	_try_remove_file(temp_aligned_apk_path)
	_try_remove_file(temp_signed_apk_path)

	return {
		"success": true,
		"project_root": project_root,
		"source_executable": source_apk,
		"output_executable": output_apk,
		"platform": PLATFORM_ANDROID,
		"utmx_patch_pck_size": int(patch_result.get("patch_pck_size", 0)),
	}


static func export_embedded(
	project_root_path: String,
	source_executable_path: String,
	output_executable_path: String,
	platform_id: String
) -> Dictionary:
	var project_root := _trim_trailing_slash(_normalize_path(project_root_path))
	var normalized_platform := _normalize_platform(platform_id)
	var source_exe := _resolve_source_executable_path(source_executable_path, normalized_platform)
	var output_exe := _resolve_output_executable_path(
		output_executable_path, project_root, normalized_platform
	)

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
	var patch_result := _build_patch_pck(project_root, temp_patch_pck_path, [output_exe])
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
		"platform": normalized_platform,
		"original_pck_size": original_pck_size,
		"utmx_patch_pck_size": patch_size,
		"relocated_pck_size": relocated_pck_size,
	}


static func _resolve_source_executable_path(raw_source_path: String, platform_id: String) -> String:
	var source_path: String = _normalize_path(raw_source_path.strip_edges())
	if source_path.is_empty():
		return ""
	var normalized_platform := _normalize_platform(platform_id)

	var candidates: PackedStringArray = []
	match normalized_platform:
		PLATFORM_WINDOWS:
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
		PLATFORM_LINUX:
			candidates.append(source_path)
			if DirAccess.dir_exists_absolute(source_path):
				candidates.append(_normalize_path(source_path.path_join("Linux")))
				candidates.append(_normalize_path(source_path.path_join("linux")))
		PLATFORM_ANDROID:
			if source_path.get_extension().to_lower() == "apk":
				candidates.append(source_path)
			else:
				candidates.append(source_path + APK_FILE_SUFFIX)
				candidates.append(source_path)
			var source_dir: String = _normalize_path(source_path.get_base_dir())
			if !source_dir.is_empty() and source_dir != ".":
				candidates.append(_normalize_path(source_dir.path_join("Android.apk")))
				candidates.append(_normalize_path(source_dir.path_join("android.apk")))
			if DirAccess.dir_exists_absolute(source_path):
				candidates.append(_normalize_path(source_path.path_join("Android.apk")))
				candidates.append(_normalize_path(source_path.path_join("android.apk")))
				candidates.append(_normalize_path(source_path.path_join("Android")))
				candidates.append(_normalize_path(source_path.path_join("android")))
		_:
			candidates.append(source_path)
			if DirAccess.dir_exists_absolute(source_path):
				candidates.append(_normalize_path(source_path.path_join(normalized_platform)))

	for candidate: String in candidates:
		if FileAccess.file_exists(candidate):
			return candidate

	return candidates[0] if (!candidates.is_empty()) else source_path


static func _resolve_output_executable_path(
	raw_output_path: String, project_root: String, platform_id: String
) -> String:
	var output_path := _normalize_path(raw_output_path.strip_edges())
	if output_path.is_empty():
		return ""
	if output_path.ends_with("/") || DirAccess.dir_exists_absolute(output_path):
		return _normalize_path(
			output_path.path_join(_build_default_output_name(project_root, platform_id))
		)
	var extension := _get_platform_default_extension(platform_id)
	if !extension.is_empty() and output_path.get_extension().to_lower() != extension:
		output_path += "." + extension
	return _normalize_path(output_path)


static func _resolve_android_output_apk_path(project_root: String, output_apk_path: String) -> String:
	var normalized_output_path: String = _normalize_path(output_apk_path)
	var output_dir: String = normalized_output_path.get_base_dir()
	if output_dir.is_empty():
		output_dir = _trim_trailing_slash(_normalize_path(project_root))
	var file_name: String = _build_default_output_name(project_root, PLATFORM_ANDROID)
	if file_name.is_empty():
		file_name = "game.apk"
	return _normalize_path(output_dir.path_join(file_name))


static func _build_default_output_name(project_root: String, platform_id: String) -> String:
	var project_name: String = ""
	if is_instance_valid(EditorProjectManager.opened_project):
		project_name = String(EditorProjectManager.opened_project.project_name).strip_edges()
	if project_name.is_empty():
		project_name = _trim_trailing_slash(_normalize_path(project_root)).get_file()
	if project_name.is_empty():
		project_name = "game"
	var extension := _get_platform_default_extension(platform_id)
	if extension.is_empty():
		return project_name
	return project_name + "." + extension


static func _get_platform_default_extension(platform_id: String) -> String:
	match _normalize_platform(platform_id):
		PLATFORM_WINDOWS:
			return "exe"
		PLATFORM_ANDROID:
			return "apk"
		_:
			return ""


static func _normalize_platform(platform_id: String) -> String:
	var normalized: String = String(platform_id).strip_edges().to_lower()
	if normalized.is_empty():
		return PLATFORM_WINDOWS
	return normalized


static func _build_patch_pck(
	project_root: String, output_pck_path: String, ignored_source_paths: Array = []
) -> Dictionary:
	var output_dir := output_pck_path.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_err := DirAccess.make_dir_recursive_absolute(output_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create patch directory: %s (err=%d)" % [output_dir, mkdir_err]
			)
	UtmxPackPicker.pick_pack(project_root, output_pck_path, ignored_source_paths)
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


static func _validate_android_signing_options(signing_options: Dictionary) -> Dictionary:
	var jdk_bin_dir: String = _normalize_path(
		String(signing_options.get("jdk_bin_dir", "")).strip_edges()
	)
	var keystore_path: String = _normalize_path(
		String(signing_options.get("keystore_path", "")).strip_edges()
	)
	var keystore_alias: String = String(signing_options.get("keystore_alias", "")).strip_edges()
	var keystore_password: String = String(signing_options.get("keystore_password", ""))
	var key_password: String = String(signing_options.get("key_password", ""))
	if key_password.is_empty():
		key_password = keystore_password

	if jdk_bin_dir.is_empty():
		return _error_result("Android signing JDK bin directory cannot be empty.")
	if !jdk_bin_dir.is_absolute_path():
		return _error_result("Android signing JDK path must be absolute.")
	if !DirAccess.dir_exists_absolute(jdk_bin_dir):
		return _error_result("Android signing JDK directory does not exist: %s" % jdk_bin_dir)

	var jarsigner_path: String = _resolve_jarsigner_path(jdk_bin_dir)
	var jar_tool_path: String = _resolve_jar_tool_path(jdk_bin_dir)
	var android_sdk_root: String = _resolve_android_sdk_root(signing_options)
	var apksigner_path: String = _resolve_build_tools_tool_path(android_sdk_root, "apksigner")
	var zipalign_path: String = _resolve_build_tools_tool_path(android_sdk_root, "zipalign")
	if jarsigner_path.is_empty() and apksigner_path.is_empty():
		return _error_result(
			(
				"Cannot find apksigner (Android SDK Build-Tools) or jarsigner (JDK).\n"
				+ "JDK path: %s\nAndroid SDK root: %s"
			)
			% [jdk_bin_dir, android_sdk_root]
		)
	if jar_tool_path.is_empty():
		return _error_result("Cannot find jar tool in JDK path: %s" % jdk_bin_dir)
	if apksigner_path.is_empty():
		_emit_output(
			(
				"apksigner not found under Android SDK (%s), "
				+ "fallback to jarsigner (v1 signature only)."
			)
			% android_sdk_root,
			"warning"
		)
	elif zipalign_path.is_empty():
		_emit_output(
			"zipalign not found under Android SDK (%s), continue without alignment."
			% android_sdk_root,
			"warning"
		)

	if keystore_path.is_empty():
		return _error_result("Android keystore path cannot be empty.")
	if !keystore_path.is_absolute_path():
		return _error_result("Android keystore path must be absolute.")
	if !FileAccess.file_exists(keystore_path):
		return _error_result("Android keystore file does not exist: %s" % keystore_path)
	if keystore_alias.is_empty():
		return _error_result("Android keystore alias cannot be empty.")
	if keystore_password.is_empty():
		return _error_result("Android keystore password cannot be empty.")
	if key_password.is_empty():
		return _error_result("Android key password cannot be empty.")

	return {
		"success": true,
		"android_sdk_root": android_sdk_root,
		"apksigner_path": apksigner_path,
		"zipalign_path": zipalign_path,
		"jarsigner_path": jarsigner_path,
		"jar_tool_path": jar_tool_path,
		"keystore_path": keystore_path,
		"keystore_alias": keystore_alias,
		"keystore_password": keystore_password,
		"key_password": key_password,
	}


static func _resolve_jarsigner_path(jdk_bin_dir: String) -> String:
	var normalized_jdk_dir: String = _normalize_path(jdk_bin_dir)
	var candidates: PackedStringArray = PackedStringArray()
	if OS.get_name().to_lower() == "windows":
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("jarsigner.exe")))
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("bin/jarsigner.exe")))
	else:
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("jarsigner")))
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("bin/jarsigner")))

	for candidate_path: String in candidates:
		if FileAccess.file_exists(candidate_path):
			return candidate_path
	return ""


static func _resolve_jar_tool_path(jdk_bin_dir: String) -> String:
	var normalized_jdk_dir: String = _normalize_path(jdk_bin_dir)
	var candidates: PackedStringArray = PackedStringArray()
	if OS.get_name().to_lower() == "windows":
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("jar.exe")))
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("bin/jar.exe")))
	else:
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("jar")))
		candidates.append(_normalize_path(normalized_jdk_dir.path_join("bin/jar")))

	for candidate_path: String in candidates:
		if FileAccess.file_exists(candidate_path):
			return candidate_path
	return ""


static func _resolve_android_sdk_root(signing_options: Dictionary) -> String:
	var seen: Dictionary = {}
	var candidates: Array[String] = []

	var option_roots: PackedStringArray = PackedStringArray(
		[
			String(signing_options.get("android_sdk_root", "")).strip_edges(),
			String(signing_options.get("android_sdk_path", "")).strip_edges(),
		]
	)
	for option_root: String in option_roots:
		var normalized_option_root: String = _normalize_path(option_root)
		if normalized_option_root.is_empty():
			continue
		if seen.has(normalized_option_root):
			continue
		seen[normalized_option_root] = true
		candidates.append(normalized_option_root)

	var env_candidates: PackedStringArray = PackedStringArray(
		[
			String(OS.get_environment("ANDROID_SDK_ROOT")).strip_edges(),
			String(OS.get_environment("ANDROID_HOME")).strip_edges(),
		]
	)
	for env_candidate: String in env_candidates:
		var normalized_env_candidate: String = _normalize_path(env_candidate)
		if normalized_env_candidate.is_empty():
			continue
		if seen.has(normalized_env_candidate):
			continue
		seen[normalized_env_candidate] = true
		candidates.append(normalized_env_candidate)

	var os_name: String = OS.get_name().to_lower()
	if os_name == "windows":
		var local_app_data: String = _normalize_path(
			String(OS.get_environment("LOCALAPPDATA")).strip_edges()
		)
		if !local_app_data.is_empty():
			var windows_default_sdk: String = _normalize_path(local_app_data.path_join("Android/Sdk"))
			if !seen.has(windows_default_sdk):
				seen[windows_default_sdk] = true
				candidates.append(windows_default_sdk)
	else:
		var home_dir: String = _normalize_path(String(OS.get_environment("HOME")).strip_edges())
		if !home_dir.is_empty():
			var linux_default_sdk: String = _normalize_path(home_dir.path_join("Android/Sdk"))
			if !seen.has(linux_default_sdk):
				seen[linux_default_sdk] = true
				candidates.append(linux_default_sdk)
			var macos_default_sdk: String = _normalize_path(home_dir.path_join("Library/Android/sdk"))
			if !seen.has(macos_default_sdk):
				seen[macos_default_sdk] = true
				candidates.append(macos_default_sdk)

	for candidate: String in candidates:
		if DirAccess.dir_exists_absolute(candidate):
			return candidate
	return ""


static func _resolve_build_tools_tool_path(android_sdk_root: String, tool_name: String) -> String:
	var sdk_root: String = _normalize_path(android_sdk_root)
	if sdk_root.is_empty():
		return ""
	if !DirAccess.dir_exists_absolute(sdk_root):
		return ""

	var build_tools_root: String = _normalize_path(sdk_root.path_join("build-tools"))
	if !DirAccess.dir_exists_absolute(build_tools_root):
		return ""

	var build_tool_dirs: Array[String] = []
	var dir: DirAccess = DirAccess.open(build_tools_root)
	if dir == null:
		return ""

	dir.list_dir_begin()
	var entry_name: String = dir.get_next()
	while !entry_name.is_empty():
		if entry_name != "." and entry_name != ".." and dir.current_is_dir():
			build_tool_dirs.append(_normalize_path(build_tools_root.path_join(entry_name)))
		entry_name = dir.get_next()
	dir.list_dir_end()

	if build_tool_dirs.is_empty():
		return ""
	build_tool_dirs.sort()
	build_tool_dirs.reverse()

	for build_tool_dir in build_tool_dirs:
		var candidate_paths: PackedStringArray = PackedStringArray()
		if OS.get_name().to_lower() == "windows":
			candidate_paths.append(_normalize_path(build_tool_dir.path_join(tool_name + ".bat")))
			candidate_paths.append(_normalize_path(build_tool_dir.path_join(tool_name + ".cmd")))
			candidate_paths.append(_normalize_path(build_tool_dir.path_join(tool_name + ".exe")))
		else:
			candidate_paths.append(_normalize_path(build_tool_dir.path_join(tool_name)))
		candidate_paths.append(_normalize_path(build_tool_dir.path_join("bin/" + tool_name)))
		candidate_paths.append(_normalize_path(build_tool_dir.path_join("bin/" + tool_name + ".exe")))

		for candidate_path: String in candidate_paths:
			if FileAccess.file_exists(candidate_path):
				return candidate_path

	return ""


static func _inject_patch_into_android_apk(
	source_apk_path: String,
	patch_pck_path: String,
	output_apk_path: String,
	jar_tool_path: String,
	project_icon_path: String,
	project_app_name: String
) -> Dictionary:
	if !FileAccess.file_exists(source_apk_path):
		return _error_result("Android runner APK is missing: %s" % source_apk_path)
	if !FileAccess.file_exists(patch_pck_path):
		return _error_result("Patch PCK is missing for Android export: %s" % patch_pck_path)
	if !FileAccess.file_exists(jar_tool_path):
		return _error_result("JDK jar tool is missing: %s" % jar_tool_path)

	var output_dir: String = output_apk_path.get_base_dir()
	if !output_dir.is_empty() and !DirAccess.dir_exists_absolute(output_dir):
		var mkdir_err: int = DirAccess.make_dir_recursive_absolute(output_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create Android temp output directory: %s (err=%d)"
				% [output_dir, mkdir_err]
			)

	_try_remove_file(output_apk_path)
	var copy_err: int = DirAccess.copy_absolute(source_apk_path, output_apk_path)
	if copy_err != OK:
		return _error_result(
			"Failed to copy source APK before patching: %s -> %s (err=%d)"
			% [source_apk_path, output_apk_path, copy_err]
		)

	var stage_root: String = _build_temp_android_stage_path()
	var stage_assets_dir: String = _normalize_path(stage_root.path_join("assets"))
	var stage_patch_path: String = _normalize_path(stage_assets_dir.path_join("utmx.pck"))
	var stage_mkdir_err: int = DirAccess.make_dir_recursive_absolute(stage_assets_dir)
	if stage_mkdir_err != OK:
		_try_remove_file(output_apk_path)
		return _error_result(
			"Failed to create Android patch stage directory: %s (err=%d)"
			% [stage_assets_dir, stage_mkdir_err]
		)
	var stage_copy_err: int = DirAccess.copy_absolute(patch_pck_path, stage_patch_path)
	if stage_copy_err != OK:
		_try_remove_dir_recursive(stage_root)
		_try_remove_file(output_apk_path)
		return _error_result(
			"Failed to prepare Android patch payload: %s -> %s (err=%d)"
			% [patch_pck_path, stage_patch_path, stage_copy_err]
		)
	var update_entries: PackedStringArray = PackedStringArray([ANDROID_PACK_ENTRY_PATH])
	var icon_stage_result: Dictionary = _prepare_android_icon_stage_files(
		source_apk_path, stage_root, project_icon_path
	)
	if !bool(icon_stage_result.get("success", false)):
		_try_remove_dir_recursive(stage_root)
		_try_remove_file(output_apk_path)
		return icon_stage_result
	var icon_entries_variant: Variant = icon_stage_result.get("entries", PackedStringArray())
	if icon_entries_variant is PackedStringArray:
		for icon_entry: String in icon_entries_variant:
			update_entries.append(icon_entry)
	var app_name_stage_result: Dictionary = _prepare_android_app_name_stage_file(
		source_apk_path, stage_root, project_app_name
	)
	if !bool(app_name_stage_result.get("success", false)):
		_try_remove_dir_recursive(stage_root)
		_try_remove_file(output_apk_path)
		return app_name_stage_result
	var stored_entries: PackedStringArray = PackedStringArray()
	var app_name_entries_variant: Variant = app_name_stage_result.get("entries", PackedStringArray())
	if app_name_entries_variant is PackedStringArray:
		for app_name_entry: String in app_name_entries_variant:
			if stored_entries.find(app_name_entry) < 0:
				stored_entries.append(app_name_entry)

	var update_result: Dictionary = _run_jar_update(
		jar_tool_path, output_apk_path, stage_root, update_entries, false
	)
	if !bool(update_result.get("success", false)):
		_try_remove_dir_recursive(stage_root)
		_try_remove_file(output_apk_path)
		return update_result
	if !stored_entries.is_empty():
		var stored_update_result: Dictionary = _run_jar_update(
			jar_tool_path, output_apk_path, stage_root, stored_entries, true
		)
		if !bool(stored_update_result.get("success", false)):
			_try_remove_dir_recursive(stage_root)
			_try_remove_file(output_apk_path)
			return stored_update_result

	_try_remove_dir_recursive(stage_root)
	if !FileAccess.file_exists(output_apk_path):
		return _error_result("jar update did not produce output APK.")
	return {"success": true}


static func _run_jar_update(
	jar_tool_path: String,
	output_apk_path: String,
	stage_root: String,
	entries: PackedStringArray,
	store_only: bool
) -> Dictionary:
	if entries.is_empty():
		return {"success": true}

	var mode: String = "uf0" if store_only else "uf"
	var command_output: Array = []
	var args: PackedStringArray = PackedStringArray([mode, output_apk_path])
	for update_entry: String in entries:
		args.append("-C")
		args.append(stage_root)
		args.append(update_entry)
	var exit_code: int = OS.execute(jar_tool_path, args, command_output, true)

	var output_lines: PackedStringArray = PackedStringArray()
	for line in command_output:
		output_lines.append(String(line))
	var output_text: String = "\n".join(output_lines)
	if !output_text.is_empty():
		_emit_output(output_text, "info")
	if exit_code != OK:
		var details: String = output_text.strip_edges()
		if details.is_empty():
			return _error_result("jar %s update failed with exit code %d." % [mode, exit_code])
		return _error_result(
			"jar %s update failed with exit code %d.\n%s" % [mode, exit_code, details]
		)
	return {"success": true}


static func _prepare_android_app_name_stage_file(
	source_apk_path: String, stage_root: String, project_app_name: String
) -> Dictionary:
	var normalized_app_name: String = String(project_app_name).strip_edges()
	if normalized_app_name.is_empty():
		return {"success": true, "entries": PackedStringArray()}

	var reader: ZIPReader = ZIPReader.new()
	if reader.open(source_apk_path) != OK:
		return _error_result("Failed to open source APK for app name patching: %s" % source_apk_path)
	var resources_path: String = "resources.arsc"
	var files: PackedStringArray = reader.get_files()
	if files.find(resources_path) < 0:
		reader.close()
		_emit_output("resources.arsc not found, skip Android app name replacement.", "warning")
		return {"success": true, "entries": PackedStringArray()}

	var resources_buffer: PackedByteArray = reader.read_file(resources_path)
	reader.close()
	if resources_buffer.is_empty():
		_emit_output("resources.arsc is empty, skip Android app name replacement.", "warning")
		return {"success": true, "entries": PackedStringArray()}

	var patch_result: Dictionary = _patch_android_resources_app_name(resources_buffer, normalized_app_name)
	if !bool(patch_result.get("success", false)):
		return patch_result
	var replaced_count: int = int(patch_result.get("replaced_count", 0))
	if replaced_count <= 0:
		_emit_output("No app-name string matched in resources.arsc, keep default app name.", "warning")
		return {"success": true, "entries": PackedStringArray()}

	var staged_resources_path: String = _normalize_path(stage_root.path_join(resources_path))
	var staged_dir: String = staged_resources_path.get_base_dir()
	if !DirAccess.dir_exists_absolute(staged_dir):
		var mkdir_err: int = DirAccess.make_dir_recursive_absolute(staged_dir)
		if mkdir_err != OK:
			return _error_result(
				"Failed to create app-name stage directory: %s (err=%d)" % [staged_dir, mkdir_err]
			)

	var staged_file: FileAccess = FileAccess.open(staged_resources_path, FileAccess.WRITE)
	if staged_file == null:
		return _error_result(
			"Failed to write staged resources.arsc: %s (err=%d)"
			% [staged_resources_path, FileAccess.get_open_error()]
		)
	staged_file.store_buffer(resources_buffer)
	staged_file.flush()
	staged_file.close()
	if !FileAccess.file_exists(staged_resources_path):
		return _error_result("Failed to create staged resources.arsc: %s" % staged_resources_path)

	var actual_name: String = String(patch_result.get("app_name", normalized_app_name))
	var truncated: bool = bool(patch_result.get("truncated", false))
	if truncated:
		_emit_output(
			"Android app name was truncated to fit template limit: %s" % actual_name,
			"warning"
		)
	_emit_output(
		"Android app name replaced in resources.arsc: %s (%d entries)"
		% [actual_name, replaced_count],
		"info"
	)
	return {
		"success": true,
		"entries": PackedStringArray([resources_path]),
	}


static func _patch_android_resources_app_name(
	resources_buffer: PackedByteArray, project_app_name: String
) -> Dictionary:
	var template_app_name: String = "UTMX-Runtime"
	var normalized_name: String = String(project_app_name).strip_edges()
	if normalized_name.is_empty():
		return {"success": true, "replaced_count": 0, "app_name": ""}

	var template_len: int = template_app_name.length()
	var truncated: bool = false
	if normalized_name.length() > template_len:
		normalized_name = normalized_name.substr(0, template_len)
		truncated = true

	var template_bytes: PackedByteArray = _string_to_utf16le_bytes(template_app_name)
	var replacement_bytes: PackedByteArray = _string_to_utf16le_bytes(normalized_name)
	var replaced_count: int = 0
	var old_len_le0: int = template_len & 0xFF
	var old_len_le1: int = (template_len >> 8) & 0xFF
	var replacement_len: int = normalized_name.length()
	var replacement_len_le0: int = replacement_len & 0xFF
	var replacement_len_le1: int = (replacement_len >> 8) & 0xFF

	var scan_index: int = 0
	var min_entry_size: int = 2 + template_bytes.size() + 2
	while scan_index + min_entry_size <= resources_buffer.size():
		if (
			resources_buffer[scan_index] != old_len_le0
			or resources_buffer[scan_index + 1] != old_len_le1
		):
			scan_index += 1
			continue

		var is_match: bool = true
		for i in range(template_bytes.size()):
			if resources_buffer[scan_index + 2 + i] != template_bytes[i]:
				is_match = false
				break
		if !is_match:
			scan_index += 1
			continue
		if (
			resources_buffer[scan_index + 2 + template_bytes.size()] != 0
			or resources_buffer[scan_index + 2 + template_bytes.size() + 1] != 0
		):
			scan_index += 1
			continue

		replaced_count += 1
		resources_buffer[scan_index] = replacement_len_le0
		resources_buffer[scan_index + 1] = replacement_len_le1
		for i in range(template_bytes.size() + 2):
			resources_buffer[scan_index + 2 + i] = 0
		for i in range(replacement_bytes.size()):
			resources_buffer[scan_index + 2 + i] = replacement_bytes[i]
		var terminator_index: int = scan_index + 2 + replacement_bytes.size()
		resources_buffer[terminator_index] = 0
		resources_buffer[terminator_index + 1] = 0
		scan_index += min_entry_size

	return {
		"success": true,
		"replaced_count": replaced_count,
		"truncated": truncated,
		"app_name": normalized_name,
	}


static func _prepare_android_icon_stage_files(
	source_apk_path: String, stage_root: String, project_icon_path: String
) -> Dictionary:
	var normalized_icon_path: String = _normalize_path(project_icon_path.strip_edges())
	if normalized_icon_path.is_empty():
		return {"success": true, "entries": PackedStringArray()}
	if !FileAccess.file_exists(normalized_icon_path):
		_emit_output(
			"Project icon file not found, skip Android icon replacement: %s" % normalized_icon_path,
			"warning"
		)
		return {"success": true, "entries": PackedStringArray()}

	var source_icon: Image = Image.load_from_file(normalized_icon_path)
	if source_icon == null || source_icon.is_empty():
		_emit_output(
			"Failed to load project icon image, skip Android icon replacement: %s"
			% normalized_icon_path,
			"warning"
		)
		return {"success": true, "entries": PackedStringArray()}

	var icon_entries: PackedStringArray = _collect_android_icon_entry_paths(source_apk_path)
	if icon_entries.is_empty():
		_emit_output("No icon entries found in runner APK, skip Android icon replacement.", "warning")
		return {"success": true, "entries": PackedStringArray()}

	var written_entries: PackedStringArray = PackedStringArray()
	for icon_entry: String in icon_entries:
		var staged_icon_path: String = _normalize_path(stage_root.path_join(icon_entry))
		var staged_icon_dir: String = staged_icon_path.get_base_dir()
		if !DirAccess.dir_exists_absolute(staged_icon_dir):
			var mkdir_err: int = DirAccess.make_dir_recursive_absolute(staged_icon_dir)
			if mkdir_err != OK:
				return _error_result(
					"Failed to create icon stage directory: %s (err=%d)"
					% [staged_icon_dir, mkdir_err]
				)

		var target_icon_size: int = _get_android_icon_target_size(icon_entry)
		var staged_image_variant: Variant = source_icon.duplicate()
		var staged_image: Image = null
		if staged_image_variant is Image:
			staged_image = staged_image_variant
		if staged_image == null:
			staged_image = source_icon
		if target_icon_size > 0 and (
			staged_image.get_width() != target_icon_size
			or staged_image.get_height() != target_icon_size
		):
			staged_image.resize(target_icon_size, target_icon_size, Image.INTERPOLATE_LANCZOS)
		if staged_image.get_format() != Image.FORMAT_RGBA8:
			staged_image.convert(Image.FORMAT_RGBA8)
		var save_err: int = _save_android_icon_image(staged_image, staged_icon_path, icon_entry)
		if save_err != OK:
			return _error_result(
				"Failed to write staged Android icon: %s (err=%d)" % [icon_entry, save_err]
			)
		written_entries.append(icon_entry)

	_emit_output(
		"Android icon replaced from project icon: %s (%d entries)"
		% [normalized_icon_path, written_entries.size()],
		"info"
	)
	return {"success": true, "entries": written_entries}


static func _collect_android_icon_entry_paths(apk_path: String) -> PackedStringArray:
	var entries: PackedStringArray = PackedStringArray()
	var reader: ZIPReader = ZIPReader.new()
	if reader.open(apk_path) != OK:
		return entries

	var files: PackedStringArray = reader.get_files()
	for file_path: String in files:
		var normalized: String = _normalize_zip_entry_path(file_path).to_lower()
		if !(normalized.begins_with("res/mipmap") or normalized.begins_with("res/drawable")):
			continue
		if normalized.ends_with("/"):
			continue
		if !normalized.contains("/icon"):
			continue
		if !(normalized.ends_with(".png") or normalized.ends_with(".webp")):
			continue
		entries.append(_normalize_zip_entry_path(file_path))

	reader.close()
	entries.sort()
	return entries


static func _get_android_icon_target_size(entry_path: String) -> int:
	var lower_entry_path: String = _normalize_zip_entry_path(entry_path).to_lower()
	var density: String = ""
	var density_order: PackedStringArray = PackedStringArray(
		["xxxhdpi", "xxhdpi", "xhdpi", "hdpi", "mdpi"]
	)
	for density_name: String in density_order:
		if lower_entry_path.contains("mipmap-%s" % density_name):
			density = density_name
			break

	var is_adaptive: bool = (
		lower_entry_path.contains("_foreground")
		or lower_entry_path.contains("_background")
		or lower_entry_path.contains("_monochrome")
	)
	var icon_size_table: Dictionary = {
		"mdpi": 48,
		"hdpi": 72,
		"xhdpi": 96,
		"xxhdpi": 144,
		"xxxhdpi": 192,
	}
	var adaptive_icon_size_table: Dictionary = {
		"mdpi": 108,
		"hdpi": 162,
		"xhdpi": 216,
		"xxhdpi": 324,
		"xxxhdpi": 432,
	}
	if density.is_empty():
		return 192 if !is_adaptive else 432
	if is_adaptive:
		return int(adaptive_icon_size_table.get(density, 432))
	return int(icon_size_table.get(density, 192))


static func _save_android_icon_image(image: Image, output_path: String, entry_path: String) -> int:
	var ext: String = _normalize_zip_entry_path(entry_path).get_extension().to_lower()
	if ext == "webp" and image.has_method("save_webp"):
		var save_webp_result: Variant = image.call("save_webp", output_path)
		if save_webp_result is int and int(save_webp_result) == OK:
			return OK
	return image.save_png(output_path)


static func _zipalign_android_apk(
	input_apk_path: String, output_apk_path: String, zipalign_path: String
) -> Dictionary:
	if !FileAccess.file_exists(input_apk_path):
		return _error_result("zipalign input APK is missing: %s" % input_apk_path)
	if !FileAccess.file_exists(zipalign_path):
		return _error_result("zipalign executable is missing: %s" % zipalign_path)

	_try_remove_file(output_apk_path)
	var command_output: Array = []
	var args: PackedStringArray = PackedStringArray(
		[
			"-f",
			"-p",
			"4",
			input_apk_path,
			output_apk_path,
		]
	)
	var exit_code: int = OS.execute(zipalign_path, args, command_output, true)
	var output_lines: PackedStringArray = PackedStringArray()
	for line in command_output:
		output_lines.append(String(line))
	var output_text: String = "\n".join(output_lines)
	if !output_text.is_empty():
		_emit_output(output_text, "info")
	if exit_code != OK:
		var details: String = output_text.strip_edges()
		if details.is_empty():
			return _error_result("zipalign failed with exit code %d." % exit_code)
		return _error_result("zipalign failed with exit code %d.\n%s" % [exit_code, details])
	if !FileAccess.file_exists(output_apk_path):
		return _error_result("zipalign did not produce output APK.")
	return {"success": true}


static func _sign_android_apk_with_apksigner(
	unsigned_apk_path: String,
	signed_apk_path: String,
	apksigner_path: String,
	keystore_path: String,
	keystore_alias: String,
	keystore_password: String,
	key_password: String
) -> Dictionary:
	if !FileAccess.file_exists(unsigned_apk_path):
		return _error_result("Unsigned APK is missing before apksigner: %s" % unsigned_apk_path)
	if !FileAccess.file_exists(apksigner_path):
		return _error_result("apksigner executable is missing: %s" % apksigner_path)

	_try_remove_file(signed_apk_path)
	var command_output: Array = []
	var args: PackedStringArray = PackedStringArray(
		[
			"sign",
			"--ks",
			keystore_path,
			"--ks-key-alias",
			keystore_alias,
			"--ks-pass",
			"pass:" + keystore_password,
			"--key-pass",
			"pass:" + key_password,
			"--v1-signing-enabled",
			"true",
			"--v2-signing-enabled",
			"true",
			"--v3-signing-enabled",
			"true",
			"--out",
			signed_apk_path,
			unsigned_apk_path,
		]
	)
	var exit_code: int = OS.execute(apksigner_path, args, command_output, true)
	var output_lines: PackedStringArray = PackedStringArray()
	for line in command_output:
		output_lines.append(String(line))
	var output_text: String = "\n".join(output_lines)
	if !output_text.is_empty():
		_emit_output(output_text, "info")
	if exit_code != OK:
		var details: String = output_text.strip_edges()
		if details.is_empty():
			return _error_result("apksigner failed with exit code %d." % exit_code)
		return _error_result("apksigner failed with exit code %d.\n%s" % [exit_code, details])
	if !FileAccess.file_exists(signed_apk_path):
		return _error_result("apksigner did not produce a signed APK.")
	return {"success": true}


static func _sign_android_apk_with_jarsigner(
	unsigned_apk_path: String,
	signed_apk_path: String,
	jarsigner_path: String,
	keystore_path: String,
	keystore_alias: String,
	keystore_password: String,
	key_password: String
) -> Dictionary:
	if !FileAccess.file_exists(unsigned_apk_path):
		return _error_result("Unsigned APK is missing before signing: %s" % unsigned_apk_path)
	if !FileAccess.file_exists(jarsigner_path):
		return _error_result("jarsigner executable is missing: %s" % jarsigner_path)

	_try_remove_file(signed_apk_path)
	var command_output: Array = []
	var args: PackedStringArray = PackedStringArray(
		[
			"-verbose",
			"-sigalg",
			"SHA256withRSA",
			"-digestalg",
			"SHA-256",
			"-keystore",
			keystore_path,
			"-storepass",
			keystore_password,
			"-keypass",
			key_password,
			"-signedjar",
			signed_apk_path,
			unsigned_apk_path,
			keystore_alias,
		]
	)
	var exit_code: int = OS.execute(jarsigner_path, args, command_output, true)
	var output_lines: PackedStringArray = PackedStringArray()
	for line in command_output:
		output_lines.append(String(line))
	var output_text: String = "\n".join(output_lines)
	if !output_text.is_empty():
		_emit_output(output_text, "info")
	if exit_code != OK:
		var details: String = output_text.strip_edges()
		if details.is_empty():
			return _error_result("jarsigner failed with exit code %d." % exit_code)
		return _error_result(
			"jarsigner failed with exit code %d.\n%s" % [exit_code, details]
		)
	if !FileAccess.file_exists(signed_apk_path):
		return _error_result("jarsigner did not produce a signed APK.")
	return {"success": true}


static func _build_temp_android_apk_path(name: String) -> String:
	var base_dir: String = _get_editor_data_path().path_join(".build_cache").path_join("export_patch")
	if !DirAccess.dir_exists_absolute(base_dir):
		DirAccess.make_dir_recursive_absolute(base_dir)
	var file_name: String = "utmx_android_%s_%d.apk" % [name, Time.get_ticks_usec()]
	return _normalize_path(base_dir.path_join(file_name))


static func _build_temp_android_stage_path() -> String:
	var base_dir: String = _get_editor_data_path().path_join(".build_cache").path_join("export_patch")
	if !DirAccess.dir_exists_absolute(base_dir):
		DirAccess.make_dir_recursive_absolute(base_dir)
	var dir_name: String = "utmx_android_stage_%d" % Time.get_ticks_usec()
	return _normalize_path(base_dir.path_join(dir_name))


static func _string_to_utf16le_bytes(text: String) -> PackedByteArray:
	var out: PackedByteArray = PackedByteArray()
	for i in range(text.length()):
		var code: int = text.unicode_at(i)
		out.append(code & 0xFF)
		out.append((code >> 8) & 0xFF)
	return out


static func _normalize_zip_entry_path(path: String) -> String:
	var normalized: String = String(path).strip_edges().replace("\\", "/")
	normalized = normalized.trim_prefix("./").trim_prefix("/")
	while normalized.contains("//"):
		normalized = normalized.replace("//", "/")
	return normalized


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


static func _try_remove_dir_recursive(path: String) -> void:
	if path.is_empty() or !DirAccess.dir_exists_absolute(path):
		return
	var dir: DirAccess = DirAccess.open(path)
	if dir == null:
		return

	dir.list_dir_begin()
	var entry_name: String = dir.get_next()
	while !entry_name.is_empty():
		if entry_name != "." and entry_name != "..":
			var child_path: String = _normalize_path(path.path_join(entry_name))
			if dir.current_is_dir():
				_try_remove_dir_recursive(child_path)
			else:
				DirAccess.remove_absolute(child_path)
		entry_name = dir.get_next()
	dir.list_dir_end()
	DirAccess.remove_absolute(path)


static func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()


static func _trim_trailing_slash(path: String) -> String:
	var out := _normalize_path(path)
	while out.ends_with("/") and out.length() > 1:
		out = out.substr(0, out.length() - 1)
	return out


static func _emit_output(message: String, level: String = "info") -> void:
	var main_loop := Engine.get_main_loop()
	if main_loop is SceneTree:
		var tree := main_loop as SceneTree
		if tree.root != null:
			var output_manager: Node = tree.root.get_node_or_null("EditorOutputManager")
			if output_manager != null and output_manager.has_method("push"):
				output_manager.call("push", message, level)


static func _error_result(message: String) -> Dictionary:
	var formatted_message: String = "ProgramExporter: " + message
	push_error(formatted_message)
	_emit_output(formatted_message, "error")
	return {
		"success": false,
		"error": message,
	}
