class_name EditorResourceLoader extends Node

## 存储后缀名与加载逻辑的映射。例如 {"gd": load_gd_script, "png": load_texture}
var _importers : Dictionary = {}
## 资源路径到 Resource 实例的强引用缓存
var _resource_cache : Dictionary = {}

func _ready() -> void:
	GlobalEditorFileSystem.entry_removed.connect(unload_resource);
	# 注册图像加载器，支持多种常见格式
	register_importer("png", _load_image_as_texture)
	register_importer("jpg", _load_image_as_texture)
	register_importer("jpeg", _load_image_as_texture)
	register_importer("webp", _load_image_as_texture)
	register_importer("svg", _load_image_as_texture)

## 注册特定后缀的加载方法
func register_importer(extension: String, loader_callable: Callable) -> void:
	_importers[extension.to_lower()] = loader_callable

## 核心加载方法
func load_resource(path: String) -> Resource:
	if (path.is_empty()):
		return null
	
	if (_resource_cache.has(path)):
		var cached_res : Resource = _resource_cache[path]
		if (is_instance_valid(cached_res)):
			return cached_res
		else:
			_resource_cache.erase(path)
	
	# 检查物理文件是否存在
	if (!FileAccess.file_exists(path)):
		push_error("ResourceLoader: 文件不存在 -> " + path)
		return null

	var ext = path.get_extension().to_lower()
	var res: Resource = null
	
	if _importers.has(ext): res = _importers[ext].call(path)
	else: res = ResourceLoader.load(path)
		
	if res != null:
		_resource_cache[path] = res
	else:
		push_error("ResourceLoader: 资源加载失败 -> " + path)
		
	return res

# --- 图像加载逻辑 ---

## 将磁盘上的图像文件转换为 ImageTexture
func _load_image_as_texture(path: String) -> ImageTexture:
	var img := Image.load_from_file(path)
	if img == null or img.is_empty():
		push_error("无法加载图像数据: " + path)
		return null
	var tex := ImageTexture.create_from_image(img)
	if tex == null:
		push_error("无法创建纹理: " + path)
		return null
		
	return tex

# --- 其他管理方法 ---

## 检查资源是否已加载或可加载
func exists(path: String) -> bool:
	if _resource_cache.has(path):
		return is_instance_valid(_resource_cache[path])
	return FileAccess.file_exists(path)

## 强制卸载资源以节省内存
func unload_resource(path: String) -> void:
	if _resource_cache.has(path):
		_resource_cache.erase(path)

## 清空所有缓存
func clear_cache() -> void:
	_resource_cache.clear()
