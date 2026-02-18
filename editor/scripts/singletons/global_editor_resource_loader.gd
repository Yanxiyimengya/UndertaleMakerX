extends Node;

static var _importers : Dictionary = {};
static var _resource_cache : Dictionary = {};

func _ready() -> void:
	GlobalEditorFileSystem.entry_removed.connect(unload_resource);
	_setup_importers();

func _setup_importers() -> void:
	var img_loader : Callable = _load_image_as_texture;
	var ogg_loader : Callable = _load_ogg_as_audio_stream;
	var font_loader : Callable = _load_font_file;
	register_importer("png", img_loader);
	register_importer("jpg", img_loader);
	register_importer("jpeg", img_loader);
	register_importer("webp", img_loader);
	register_importer("svg", img_loader);
	register_importer("ogg", ogg_loader);
	register_importer("ttf", font_loader);
	register_importer("otf", font_loader);
	register_importer("woff", font_loader);
	register_importer("woff2", font_loader);
	register_importer("fnt", font_loader);

func register_importer(ext: String, callable: Callable) -> void:
	_importers[ext.to_lower()] = callable;

func load_resource(path: String) -> Resource:
	if (path.is_empty()): return null;
	if (_importers.is_empty()):
		_setup_importers();

	var normalized_path : String = _normalize_path(path);
	if (_resource_cache.has(normalized_path)):
		var wref : WeakRef = _resource_cache[normalized_path];
		var cached_res : Resource = wref.get_ref();
		if (cached_res != null): return cached_res;
		_resource_cache.erase(normalized_path);

	var absolute_path : String = _to_project_absolute_path(normalized_path);
	var res_path : String = _to_res_path(absolute_path);
	var path_for_check : String = absolute_path if !absolute_path.is_empty() else normalized_path;

	if (!GlobalEditorFileSystem.entry_exists(path_for_check) && !FileAccess.file_exists(path_for_check)):
		push_warning("Loader: missing resource " + path_for_check);
		return null;

	var ext : String = path_for_check.get_extension().to_lower();
	var res : Resource = null;

	if (_importers.has(ext)):
		res = _importers[ext].call(path_for_check);
	else:
		if (!res_path.is_empty()):
			res = ResourceLoader.load(res_path);
		if (res == null && _can_load_directly(path_for_check)):
			res = ResourceLoader.load(path_for_check);

	if (res != null):
		var wref : WeakRef = weakref(res);
		_resource_cache[normalized_path] = wref;
		_resource_cache[path_for_check] = wref;
		if (!res_path.is_empty()):
			_resource_cache[res_path] = wref;
		return res;
	return null;

func _load_image_as_texture(path: String) -> ImageTexture:
	var img : Image = Image.load_from_file(path);
	if (img == null || img.is_empty()): return null;
	return ImageTexture.create_from_image(img);

func _load_ogg_as_audio_stream(path: String) -> AudioStreamOggVorbis:
	return AudioStreamOggVorbis.load_from_file(path);

func _load_font_file(path: String) -> FontFile:
	var font : FontFile = FontFile.new();
	var ext : String = path.get_extension().to_lower();
	var err : int = OK;

	if (ext == "fnt"):
		err = font.load_bitmap_font(path);
	else:
		err = font.load_dynamic_font(path);

	if (err != OK):
		push_warning("Loader: failed to load font file " + path + " (err=" + str(err) + ")");
		return null;
	return font;

func unload_resource(path: String) -> void:
	var normalized_path : String = _normalize_path(path);
	if (_resource_cache.has(normalized_path)):
		_resource_cache.erase(normalized_path);

	var absolute_path : String = _to_project_absolute_path(normalized_path);
	if (_resource_cache.has(absolute_path)):
		_resource_cache.erase(absolute_path);

	var res_path : String = _to_res_path(absolute_path);
	if (!res_path.is_empty() && _resource_cache.has(res_path)):
		_resource_cache.erase(res_path);

func clear_cache() -> void:
	_resource_cache.clear();

func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path();

func _to_project_absolute_path(path: String) -> String:
	var normalized_path : String = _normalize_path(path);
	if (normalized_path.begins_with("res://")):
		return _normalize_path(ProjectSettings.globalize_path(normalized_path));
	return normalized_path;

func _to_res_path(path: String) -> String:
	var normalized_path : String = _normalize_path(path);
	if (normalized_path.begins_with("res://")):
		return normalized_path;
	var project_root : String = _normalize_path(ProjectSettings.globalize_path("res://"));
	if (!project_root.ends_with("/")):
		project_root += "/";
	if (normalized_path.begins_with(project_root)):
		return "res://" + normalized_path.trim_prefix(project_root);
	return "";

func _can_load_directly(path: String) -> bool:
	return path.begins_with("res://") || path.begins_with("user://");
