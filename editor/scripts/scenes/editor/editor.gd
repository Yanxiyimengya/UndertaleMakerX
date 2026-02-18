extends Panel

const TEXT_FILE_EXTENSIONS := {
	"txt": true,
	"md": true,
	"rst": true,
	"json": true,
	"json5": true,
	"ini": true,
	"cfg": true,
	"conf": true,
	"toml": true,
	"yaml": true,
	"yml": true,
	"csv": true,
	"tsv": true,
	"xml": true,
	"html": true,
	"htm": true,
	"css": true,
	"js": true,
	"mjs": true,
	"cjs": true,
	"ts": true,
	"jsx": true,
	"tsx": true,
	"gd": true,
	"gdshader": true,
	"shader": true,
	"tres": true,
	"tscn": true,
	"cs": true,
	"java": true,
	"kt": true,
	"kts": true,
	"py": true,
	"rb": true,
	"php": true,
	"go": true,
	"rs": true,
	"c": true,
	"h": true,
	"cpp": true,
	"hpp": true,
	"cc": true,
	"hh": true,
	"swift": true,
	"sql": true,
	"sh": true,
	"zsh": true,
	"fish": true,
	"bat": true,
	"cmd": true,
	"ps1": true,
	"log": true,
	"env": true,
	"properties": true,
	"editorconfig": true,
	"gitattributes": true,
	"gitignore": true,
}

const INSPECTOR_FILE_EXTENSIONS := {
	"ini": true,
	"cfg": true,
	"conf": true,
	"properties": true,
	"json": true,
}

const SCRIPT_AND_INSPECTOR_SYNC_EXTENSIONS := {
	"ini": true,
	"cfg": true,
	"conf": true,
	"properties": true,
	"json": true,
}

@export_dir var root_path: String = EditorProjectManager.get_opened_project_path()
@onready var main_dockable: DockableContainer = %MainDockable
@onready var file_system_panel = $VBoxContainer/MarginContainer/MainDockable/FileSystem
@onready var file_browser_panel = $VBoxContainer/MarginContainer/MainDockable/FileBrowser
@onready var script_editor_panel = $VBoxContainer/MarginContainer/MainDockable/Script
@onready var inspector_panel = $VBoxContainer/MarginContainer/MainDockable/Inspector
@export var script_panel: Control


func _ready() -> void:
	if file_system_panel and file_system_panel.has_signal("selected_file"):
		if not file_system_panel.selected_file.is_connected(_on_file_system_selected_file):
			file_system_panel.selected_file.connect(_on_file_system_selected_file)
	if file_browser_panel and file_browser_panel.has_signal("preview_file_requested"):
		if not file_browser_panel.preview_file_requested.is_connected(_on_file_browser_preview_file_requested):
			file_browser_panel.preview_file_requested.connect(_on_file_browser_preview_file_requested)
	if file_browser_panel and file_browser_panel.has_signal("preview_files_requested"):
		if not file_browser_panel.preview_files_requested.is_connected(_on_file_browser_preview_files_requested):
			file_browser_panel.preview_files_requested.connect(_on_file_browser_preview_files_requested)
	if script_editor_panel and script_editor_panel.has_signal("script_saved"):
		var script_saved_callable: Callable = Callable(self, "_on_script_editor_script_saved")
		if not script_editor_panel.is_connected("script_saved", script_saved_callable):
			script_editor_panel.connect("script_saved", script_saved_callable)
	if inspector_panel and inspector_panel.has_signal("resource_saved"):
		var resource_saved_callable: Callable = Callable(self, "_on_inspector_resource_saved")
		if not inspector_panel.is_connected("resource_saved", resource_saved_callable):
			inspector_panel.connect("resource_saved", resource_saved_callable)


func _enter_tree() -> void:
	GlobalEditorFileSystem.set_root_path(root_path)


func _exit_tree() -> void:
	GlobalEditorResourceLoader.clear_cache()


func _on_file_system_selected_file(path: String) -> void:
	preview_file(path)


func _on_file_browser_preview_file_requested(path: String) -> void:
	preview_file(path)


func _on_file_browser_preview_files_requested(paths: Array[String]) -> void:
	preview_files(paths)


func preview_file(path: String) -> bool:
	var extension := path.get_extension().to_lower()
	if SCRIPT_AND_INSPECTOR_SYNC_EXTENSIONS.has(extension):
		var opened_in_inspector := false
		if inspector_panel and inspector_panel.has_method("open_resource"):
			opened_in_inspector = bool(inspector_panel.call("open_resource", path))
		if script_editor_panel and script_editor_panel.has_method("open_script"):
			script_editor_panel.call("open_script", path)
			main_dockable.set_control_as_current_tab(script_editor_panel)
			return true
		if opened_in_inspector:
			main_dockable.set_control_as_current_tab(inspector_panel)
			return true

	if INSPECTOR_FILE_EXTENSIONS.has(extension):
		if inspector_panel and inspector_panel.has_method("open_resource"):
			var opened: bool = inspector_panel.call("open_resource", path)
			if opened:
				main_dockable.set_control_as_current_tab(inspector_panel)
				return true
	if file_browser_panel and file_browser_panel.has_method("can_open_file"):
		var can_open_in_browser: bool = file_browser_panel.call("can_open_file", path)
		if can_open_in_browser and file_browser_panel.has_method("preview_file"):
			var opened_in_browser: bool = file_browser_panel.call("preview_file", path)
			if opened_in_browser:
				main_dockable.set_control_as_current_tab(file_browser_panel)
				return true

	if not _is_text_file(path):
		return false
	if script_editor_panel and script_editor_panel.has_method("open_script"):
		script_editor_panel.call("open_script", path)
		main_dockable.set_control_as_current_tab(script_editor_panel)
		return true
	return false


func preview_files(paths: Array[String]) -> bool:
	if paths.is_empty():
		return false
	if paths.size() == 1:
		return preview_file(paths[0])
	if file_browser_panel and file_browser_panel.has_method("preview_files"):
		var opened_in_browser: bool = bool(file_browser_panel.call("preview_files", paths))
		if opened_in_browser:
			main_dockable.set_control_as_current_tab(file_browser_panel)
			return true
	return preview_file(paths[0])


func _on_script_editor_script_saved(path: String) -> void:
	var normalized_saved_path: String = _normalize_path(path)
	var extension: String = normalized_saved_path.get_extension().to_lower()
	if not SCRIPT_AND_INSPECTOR_SYNC_EXTENSIONS.has(extension):
		return
	if inspector_panel == null or not inspector_panel.has_method("get_opened_resource_path"):
		return
	if not inspector_panel.has_method("load_from_disk"):
		return

	var opened_path: String = _normalize_path(str(inspector_panel.call("get_opened_resource_path")))
	if opened_path != normalized_saved_path:
		return

	# load_from_disk() returns false on parse errors (e.g. invalid JSON),
	# and in that case we keep the existing inspector data unchanged.
	inspector_panel.call("load_from_disk", normalized_saved_path)


func _on_inspector_resource_saved(path: String) -> void:
	var normalized_saved_path: String = _normalize_path(path)
	var extension: String = normalized_saved_path.get_extension().to_lower()
	if not SCRIPT_AND_INSPECTOR_SYNC_EXTENSIONS.has(extension):
		return
	if script_editor_panel == null or not script_editor_panel.has_method("sync_file_from_disk"):
		return
	script_editor_panel.call("sync_file_from_disk", normalized_saved_path, true)


func _is_text_file(path: String) -> bool:
	if path.is_empty():
		return false
	if DirAccess.dir_exists_absolute(path):
		return false
	if not FileAccess.file_exists(path):
		return false

	var ext := path.get_extension().to_lower()
	if TEXT_FILE_EXTENSIONS.has(ext):
		return true

	return _is_probably_text_content(path)


func _is_probably_text_content(path: String) -> bool:
	var file := FileAccess.open(path, FileAccess.READ)
	if file == null:
		return false

	var sample_size := mini(4096, int(file.get_length()))
	if sample_size <= 0:
		return true
	var sample := file.get_buffer(sample_size)
	if sample.is_empty():
		return true

	var non_text_bytes := 0
	for byte_value in sample:
		var b: int = byte_value
		if b == 0:
			return false
		if b < 32 and b != 9 and b != 10 and b != 13:
			non_text_bytes += 1

	return float(non_text_bytes) / float(sample.size()) < 0.05


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()
