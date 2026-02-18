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

@export_dir var root_path: String = EditorProjectManager.get_opened_project_path()
@onready var main_dockable: DockableContainer = %MainDockable
@onready var file_system_panel = $VBoxContainer/MarginContainer/MainDockable/FileSystem
@onready var scene_browser_panel = $VBoxContainer/MarginContainer/MainDockable/Scene
@onready var script_editor_panel = $VBoxContainer/MarginContainer/MainDockable/Script
@onready var inspector_panel = $VBoxContainer/MarginContainer/MainDockable/Inspector
@export var script_panel: Control


func _ready() -> void:
	if file_system_panel and file_system_panel.has_signal("selected_file"):
		if not file_system_panel.selected_file.is_connected(_on_file_system_selected_file):
			file_system_panel.selected_file.connect(_on_file_system_selected_file)


func _enter_tree() -> void:
	GlobalEditorFileSystem.set_root_path(root_path)


func _exit_tree() -> void:
	GlobalEditorResourceLoader.clear_cache()


func _on_file_system_selected_file(path: String) -> void:
	var extension := path.get_extension().to_lower()
	if extension == "tscn":
		if scene_browser_panel and scene_browser_panel.has_method("load_scene"):
			scene_browser_panel.call("load_scene", path)
			main_dockable.set_control_as_current_tab(scene_browser_panel)
		return
	if INSPECTOR_FILE_EXTENSIONS.has(extension):
		if inspector_panel and inspector_panel.has_method("open_resource"):
			var opened: bool = inspector_panel.call("open_resource", path)
			if opened:
				main_dockable.set_control_as_current_tab(inspector_panel)
				return

	if not _is_text_file(path):
		return
	if script_editor_panel and script_editor_panel.has_method("open_script"):
		script_editor_panel.call("open_script", path)
		main_dockable.set_control_as_current_tab(script_editor_panel)


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
