extends Node

const PROJECT_CONFIG_FILE_NAME: String = "utmx.cfg"
const TEMPLATE_METADATA_FILE_NAME: String = "template.json"
const DEFAULT_TEMPLATE_DIRECTORY: String = "res://assets/templates/projects"
const DEFAULT_EMPTY_TEMPLATE_SOURCE_DIRECTORY: String = "res://assets/templates/projects/empty_template"
const DEFAULT_EMPTY_TEMPLATE_ZIP_NAME: String = "empty_template.zip"
const TEMPLATE_ICON_CACHE_DIRECTORY: String = ".cache/template_icons"

var templates: Array[UtmxProjectTemplate] = []
var _loaded_template_directory: String = DEFAULT_TEMPLATE_DIRECTORY
var _template_icon_cache: Dictionary = {}


func get_default_template_directory() -> String:
	return DEFAULT_TEMPLATE_DIRECTORY


func load_templates_from_directory(template_directory: String = DEFAULT_TEMPLATE_DIRECTORY) -> Array[UtmxProjectTemplate]:
	templates.clear()
	_template_icon_cache.clear()
	_loaded_template_directory = template_directory
	var resolved_directory: String = _resolve_directory_path(template_directory)
	if resolved_directory.is_empty() || !_directory_exists(resolved_directory):
		return templates

	_ensure_default_empty_template_zip(resolved_directory)

	var zip_paths: Array[String] = _collect_zip_files_recursive(resolved_directory)
	zip_paths.sort()
	for zip_path in zip_paths:
		var template: UtmxProjectTemplate = _build_template_from_zip(zip_path)
		if template != null:
			templates.append(template)
	return templates


func get_default_empty_template_zip_path(template_directory: String = "") -> String:
	var target_directory: String = template_directory
	if target_directory.is_empty():
		target_directory = _loaded_template_directory
	var resolved_directory: String = _resolve_directory_path(target_directory)
	if resolved_directory.is_empty():
		return ""
	var zip_path: String = _normalize_path(
		resolved_directory.path_join(DEFAULT_EMPTY_TEMPLATE_ZIP_NAME)
	)
	if !FileAccess.file_exists(zip_path):
		return ""
	return zip_path


func get_default_template() -> UtmxProjectTemplate:
	if templates.is_empty():
		return null
	var default_empty_template_zip: String = _normalize_path(get_default_empty_template_zip_path())
	if !default_empty_template_zip.is_empty():
		for template: UtmxProjectTemplate in templates:
			if _normalize_path(template.zip_path) == default_empty_template_zip:
				return template
	return templates[0]


func _build_template_from_zip(zip_path: String) -> UtmxProjectTemplate:
	var reader := ZIPReader.new()
	if reader.open(zip_path) != OK:
		return null

	var files: PackedStringArray = reader.get_files()
	var project_config_entry: String = _find_zip_entry_by_file_name(files, PROJECT_CONFIG_FILE_NAME)
	if project_config_entry.is_empty():
		reader.close()
		return null

	var template: UtmxProjectTemplate = UtmxProjectTemplate.new()
	template.zip_path = _normalize_path(zip_path)
	template.template_path = template.zip_path.get_base_dir()
	template.template_name = _build_template_name(zip_path.get_file())
	template.template_description = "Project template loaded from zip"
	if zip_path.get_file().to_lower() == DEFAULT_EMPTY_TEMPLATE_ZIP_NAME:
		template.template_name = "Empty Template"
		template.template_description = "Project template without sample code"

	var metadata: Dictionary = _read_template_metadata(reader, files, project_config_entry)
	if !metadata.is_empty():
		var metadata_name: String = String(metadata.get("name", "")).strip_edges()
		if !metadata_name.is_empty():
			template.template_name = metadata_name
		var metadata_description: String = String(metadata.get("description", "")).strip_edges()
		if !metadata_description.is_empty():
			template.template_description = metadata_description
		var metadata_image_path: String = _extract_template_image_path(metadata)
		if !metadata_image_path.is_empty():
			template.template_image_path = metadata_image_path

	var image_entry: String = _resolve_template_image_entry(
		files, project_config_entry, template.template_image_path
	)
	if !image_entry.is_empty():
		template.template_image_path = image_entry
		template.template_image = _load_template_texture_from_zip_entry(
			reader, template.zip_path, image_entry
		)

	reader.close()
	return template


func _read_template_metadata(
	reader: ZIPReader, files: PackedStringArray, project_config_entry: String
) -> Dictionary:
	var metadata_entry: String = _find_template_metadata_entry(files, project_config_entry)
	if metadata_entry.is_empty():
		return {}

	var metadata_buffer: PackedByteArray = reader.read_file(metadata_entry)
	if metadata_buffer.is_empty():
		return {}
	var metadata_text: String = metadata_buffer.get_string_from_utf8()
	if metadata_text.is_empty():
		return {}
	var metadata_variant: Variant = JSON.parse_string(metadata_text)
	if metadata_variant is Dictionary:
		return metadata_variant
	return {}


func _find_template_metadata_entry(
	files: PackedStringArray, project_config_entry: String
) -> String:
	var project_root: String = _normalize_path(project_config_entry.get_base_dir())
	if project_root == ".":
		project_root = ""
	var direct_candidate: String = TEMPLATE_METADATA_FILE_NAME
	if !project_root.is_empty():
		direct_candidate = project_root.path_join(TEMPLATE_METADATA_FILE_NAME)
	if files.has(direct_candidate):
		return direct_candidate
	return _find_zip_entry_by_file_name(files, TEMPLATE_METADATA_FILE_NAME)


func _find_zip_entry_by_file_name(files: PackedStringArray, file_name: String) -> String:
	for entry: String in files:
		if entry.get_file() == file_name:
			return entry
	return ""


func _extract_template_image_path(metadata: Dictionary) -> String:
	var image_keys: Array[String] = [
		"image_path",
		"icon",
		"icon_path",
		"image",
		"template_image",
	]
	for key: String in image_keys:
		var value: String = String(metadata.get(key, "")).strip_edges()
		if !value.is_empty():
			return value
	return ""


func _resolve_template_image_entry(
	files: PackedStringArray, project_config_entry: String, raw_image_path: String
) -> String:
	var project_root: String = _normalize_zip_entry_path(project_config_entry.get_base_dir())
	if project_root == ".":
		project_root = ""

	var normalized_raw_path: String = _normalize_zip_entry_path(raw_image_path)
	if !normalized_raw_path.is_empty():
		if files.has(normalized_raw_path):
			return normalized_raw_path
		if !project_root.is_empty():
			var project_relative_entry: String = _normalize_zip_entry_path(
				project_root.path_join(normalized_raw_path)
			)
			if files.has(project_relative_entry):
				return project_relative_entry
		var target_file_name: String = normalized_raw_path.get_file().to_lower()
		for entry in files:
			if entry.get_file().to_lower() == target_file_name:
				return _normalize_zip_entry_path(entry)

	return _find_fallback_template_image_entry(files, project_root)


func _find_fallback_template_image_entry(files: PackedStringArray, project_root: String) -> String:
	var root_level_images: Array[String] = []
	var all_images: Array[String] = []
	for entry in files:
		var normalized_entry: String = _normalize_zip_entry_path(entry)
		if normalized_entry.is_empty() || normalized_entry.ends_with("/"):
			continue
		if !_is_supported_template_image_entry(normalized_entry):
			continue
		all_images.append(normalized_entry)

		var parent_path: String = _normalize_zip_entry_path(normalized_entry.get_base_dir())
		if parent_path == ".":
			parent_path = ""
		if parent_path == project_root:
			root_level_images.append(normalized_entry)

	var candidates: Array[String] = root_level_images if !root_level_images.is_empty() else all_images
	if candidates.is_empty():
		return ""

	for candidate in candidates:
		if _is_preferred_template_image_name(candidate.get_file().to_lower()):
			return candidate
	return candidates[0]


func _is_supported_template_image_entry(entry_path: String) -> bool:
	var ext: String = entry_path.get_extension().to_lower()
	return ext in ["png", "jpg", "jpeg", "webp", "svg", "bmp", "tga"]


func _is_preferred_template_image_name(file_name: String) -> bool:
	return (
		file_name.begins_with("icon.")
		or file_name.contains("icon")
		or file_name.contains("thumb")
		or file_name.contains("cover")
		or file_name.contains("preview")
		or file_name.contains("template")
	)


func _load_template_texture_from_zip_entry(
	reader: ZIPReader, zip_path: String, entry_path: String
) -> Texture2D:
	var normalized_entry: String = _normalize_zip_entry_path(entry_path)
	if normalized_entry.is_empty():
		return null

	var cache_key: String = "%s::%s" % [_normalize_path(zip_path), normalized_entry]
	var cached_texture_variant: Variant = _template_icon_cache.get(cache_key, null)
	if cached_texture_variant is Texture2D:
		return cached_texture_variant

	var image_buffer: PackedByteArray = reader.read_file(normalized_entry)
	if image_buffer.is_empty():
		return null

	var cached_image_file: String = _write_template_image_cache_file(cache_key, normalized_entry, image_buffer)
	if cached_image_file.is_empty():
		return null

	var image: Image = Image.load_from_file(cached_image_file)
	if image == null or image.is_empty():
		return null

	var texture: ImageTexture = ImageTexture.create_from_image(image)
	if texture == null:
		return null

	_template_icon_cache[cache_key] = texture
	return texture


func _write_template_image_cache_file(
	cache_key: String, source_entry_path: String, image_buffer: PackedByteArray
) -> String:
	var cache_dir: String = _get_template_icon_cache_directory()
	if cache_dir.is_empty():
		return ""
	if !DirAccess.dir_exists_absolute(cache_dir):
		var mkdir_err: int = DirAccess.make_dir_recursive_absolute(cache_dir)
		if mkdir_err != OK and !DirAccess.dir_exists_absolute(cache_dir):
			return ""

	var file_extension: String = source_entry_path.get_extension().to_lower()
	if file_extension.is_empty():
		file_extension = "img"
	var cache_file_name: String = "%s.%s" % [str(cache_key.hash()), file_extension]
	var cache_file_path: String = _normalize_path(cache_dir.path_join(cache_file_name))
	var file: FileAccess = FileAccess.open(cache_file_path, FileAccess.WRITE)
	if file == null:
		return ""
	file.store_buffer(image_buffer)
	file.close()
	return cache_file_path


func _get_template_icon_cache_directory() -> String:
	var base_dir: String = ""
	if Engine.get_main_loop() != null && !EditorConfigureManager.get_data_path().is_empty():
		base_dir = _normalize_path(EditorConfigureManager.get_data_path())
	if base_dir.is_empty():
		base_dir = _normalize_path(ProjectSettings.globalize_path("user://"))
	return _normalize_path(base_dir.path_join(TEMPLATE_ICON_CACHE_DIRECTORY))


func _normalize_zip_entry_path(path: String) -> String:
	var normalized: String = String(path).strip_edges().replace("\\", "/")
	normalized = normalized.trim_prefix("./").trim_prefix("/")
	while normalized.contains("//"):
		normalized = normalized.replace("//", "/")
	return normalized


func _collect_zip_files_recursive(directory_path: String) -> Array[String]:
	var result: Array[String] = []
	_collect_zip_files_recursive_impl(directory_path, result)
	return result


func _collect_zip_files_recursive_impl(directory_path: String, result: Array[String]) -> void:
	var dir: DirAccess = DirAccess.open(directory_path)
	if dir == null:
		return
	dir.list_dir_begin()
	var entry_name: String = dir.get_next()
	while !entry_name.is_empty():
		if entry_name != "." && entry_name != "..":
			var full_path: String = _normalize_path(directory_path.path_join(entry_name))
			if dir.current_is_dir():
				_collect_zip_files_recursive_impl(full_path, result)
			elif entry_name.get_extension().to_lower() == "zip":
				result.append(full_path)
		entry_name = dir.get_next()
	dir.list_dir_end()


func _ensure_default_empty_template_zip(target_directory: String) -> void:
	if _is_resource_path(target_directory):
		return

	var zip_path: String = _normalize_path(target_directory.path_join(DEFAULT_EMPTY_TEMPLATE_ZIP_NAME))
	if FileAccess.file_exists(zip_path):
		return

	var source_directory: String = _resolve_directory_path(DEFAULT_EMPTY_TEMPLATE_SOURCE_DIRECTORY)
	if source_directory.is_empty() || !_directory_exists(source_directory):
		return

	var source_files: Array[String] = []
	_collect_source_files_recursive(source_directory, source_files)
	if source_files.is_empty():
		return

	var packer: ZIPPacker = ZIPPacker.new()
	if packer.open(zip_path) != OK:
		return

	for source_file_path: String in source_files:
		var relative_path: String = source_file_path.trim_prefix(source_directory).trim_prefix("/")
		if relative_path.is_empty():
			continue
		var source_file: FileAccess = FileAccess.open(source_file_path, FileAccess.READ)
		if source_file == null:
			continue
		var buffer: PackedByteArray = source_file.get_buffer(source_file.get_length())
		source_file.close()
		if packer.start_file(relative_path) != OK:
			continue
		packer.write_file(buffer)
		packer.close_file()

	packer.close()


func _collect_source_files_recursive(directory_path: String, result: Array[String]) -> void:
	var dir: DirAccess = DirAccess.open(directory_path)
	if dir == null:
		return
	dir.list_dir_begin()
	var entry_name: String = dir.get_next()
	while !entry_name.is_empty():
		if entry_name != "." && entry_name != "..":
			var full_path: String = _normalize_path(directory_path.path_join(entry_name))
			if dir.current_is_dir():
				_collect_source_files_recursive(full_path, result)
			else:
				if entry_name.get_extension().to_lower() == "import":
					entry_name = dir.get_next()
					continue
				result.append(full_path)
		entry_name = dir.get_next()
	dir.list_dir_end()


func _build_template_name(zip_file_name: String) -> String:
	var base_name: String = zip_file_name.get_basename().replace("_", " ").replace("-", " ")
	base_name = base_name.strip_edges()
	if base_name.is_empty():
		return "Template"
	return base_name.capitalize()


func _resolve_directory_path(path: String) -> String:
	var raw_path: String = String(path).strip_edges()
	if raw_path.is_empty():
		return ""
	if _is_virtual_path(raw_path):
		return _normalize_path(raw_path)
	if raw_path.is_absolute_path():
		return _normalize_path(raw_path)
	return _normalize_path(ProjectSettings.globalize_path(raw_path))


func _directory_exists(path: String) -> bool:
	if path.is_empty():
		return false
	if _is_virtual_path(path):
		return DirAccess.open(path) != null
	return DirAccess.dir_exists_absolute(path)


func _is_virtual_path(path: String) -> bool:
	var value: String = String(path)
	return value.begins_with("res://") || value.begins_with("user://")


func _is_resource_path(path: String) -> bool:
	return String(path).begins_with("res://")


func _normalize_virtual_path_body(path_body: String) -> String:
	var normalized_body: String = String(path_body).strip_edges()
	normalized_body = normalized_body.replace("\\", "/")
	while normalized_body.begins_with("/"):
		normalized_body = normalized_body.trim_prefix("/")
	while normalized_body.contains("//"):
		normalized_body = normalized_body.replace("//", "/")

	var segments: Array[String] = []
	for raw_segment: String in normalized_body.split("/", false):
		var segment: String = raw_segment.strip_edges()
		if segment.is_empty() || segment == ".":
			continue
		if segment == "..":
			if !segments.is_empty():
				segments.remove_at(segments.size() - 1)
			continue
		segments.append(segment)
	return "/".join(segments)


func _normalize_path(path: String) -> String:
	var raw_path: String = String(path).strip_edges().replace("\\", "/")
	if raw_path.is_empty():
		return ""
	if raw_path.begins_with("res://"):
		return "res://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("res://"))]
	if raw_path.begins_with("user://"):
		return "user://%s" % [_normalize_virtual_path_body(raw_path.trim_prefix("user://"))]
	return raw_path.simplify_path()
