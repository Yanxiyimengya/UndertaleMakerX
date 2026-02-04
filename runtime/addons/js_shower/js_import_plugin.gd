@tool
extends EditorImportPlugin

# 导入器唯一标识符
func _get_importer_name():
	return "utmx.js_placeholder"

# 在编辑器界面显示的名称
func _get_visible_name():
	return "JS Placeholder"

# 处理的后缀名
func _get_recognized_extensions():
	return ["js"]

# 导入后的资源保存后缀名
func _get_save_extension():
	return "res"

# 导入后的资源类型
func _get_resource_type():
	return "Resource"

# 导入选项（这里留空，因为我们不需要配置）
func _get_import_options(path, preset_index):
	return []

func _get_preset_count():
	return 0

# 核心导入逻辑
func _import(source_file, save_path, options, platform_variants, gen_files):
	var resource = Resource.new()
	resource.set_meta("original_path", source_file)
	return ResourceSaver.save(resource, "%s.%s" % [save_path, _get_save_extension()])
