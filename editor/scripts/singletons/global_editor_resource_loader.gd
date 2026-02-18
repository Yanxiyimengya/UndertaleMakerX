extends Node;

static var _importers : Dictionary = {};
static var _resource_cache : Dictionary = {};

func _ready() -> void:
	GlobalEditorFileSystem.entry_removed.connect(unload_resource);
	_setup_importers();

func _setup_importers() -> void:
	var img_loader = _load_image_as_texture;
	register_importer("png", img_loader);
	register_importer("jpg", img_loader);
	register_importer("jpeg", img_loader);
	register_importer("webp", img_loader);
	register_importer("svg", img_loader);

func register_importer(ext: String, callable: Callable) -> void:
	_importers[ext.to_lower()] = callable

## 外部统一调用的加载接口
func load_resource(path: String) -> Resource:
	if (path.is_empty()): return null;
	if (_resource_cache.has(path)):
		var wref : WeakRef = _resource_cache[path];
		var _res = wref.get_ref();
		if (_res): return _res;
		else: _resource_cache.erase(path);
	if (!GlobalEditorFileSystem.entry_exists(path)):
		push_warning("Loader: 尝试加载文件系统中不存在的项目: " + path);
		return null;
	var ext = path.get_extension().to_lower();
	var res : Resource = null;
	
	if (_importers.has(ext)):
		res = _importers[ext].call(path);
	else:
		res = ResourceLoader.load(path);
	if (res != null):
		_resource_cache[path] = weakref(res);
		return res;
	return null;

func _load_image_as_texture(path: String) -> ImageTexture:
	var img = Image.load_from_file(path);
	if (img == null || img.is_empty()): return null;
	return ImageTexture.create_from_image(img);

func unload_resource(path: String) -> void:
	if (_resource_cache.has(path)):
		_resource_cache.erase(path);

func clear_cache() -> void : 
	_resource_cache.clear();
