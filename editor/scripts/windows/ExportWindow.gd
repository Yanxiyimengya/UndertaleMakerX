extends UtmxEditorWindow;

signal export_project_requset(export_dir: String);

@onready var project_dir_edit: LineEdit = %ProjectDirEdit;
@onready var info_label: RichTextLabel = %InfoLabel;
@onready var export_button: Button = %ExportButton;

var can_export: bool = true:
	set(value):
		can_export = value;
		if (! is_node_ready()): await ready;
		export_button.disabled = !value;

func _ready() -> void:
	_apply_translations();
	project_dir_edit.text_changed.connect(_on_project_dir_edit_text_changed);
	check_is_can_create();

func _notification(what: int) -> void:
	if (what == NOTIFICATION_TRANSLATION_CHANGED && is_node_ready()):
		_apply_translations();

func _open() -> void : 
	project_dir_edit.text = _build_default_export_path();
	check_is_can_create();
	project_dir_edit.grab_focus();
	project_dir_edit.caret_column = project_dir_edit.text.length();

func _on_dir_texture_button_pressed() -> void:
	var current_output_path : String = _resolve_export_output_path(project_dir_edit.text);
	var current_dir : String = current_output_path.get_base_dir();
	var current_file : String = current_output_path.get_file();
	if (current_dir.is_empty()):
		current_dir = EditorProjectManager.get_opened_project_path();
	if (current_file.is_empty()):
		current_file = _get_default_export_file_name();
	DisplayServer.file_dialog_show(
		tr("Select Export File"), 
		current_dir, 
		current_file, 
		false, 
		DisplayServer.FILE_DIALOG_MODE_SAVE_FILE, 
		PackedStringArray(["*.exe ; Windows Executable"]),
		func(status: bool, selected_paths: PackedStringArray, _idx: int):
			if (!status || selected_paths.is_empty()): return;
			project_dir_edit.text = selected_paths[0];
			check_is_can_create();
	)

func _on_cancel_button_pressed() -> void:
	self.close();

func _on_export_button_pressed() -> void:
	check_is_can_create();
	if (!can_export): return;
	export_project_requset.emit(_resolve_export_output_path(project_dir_edit.text));

func _on_project_dir_edit_text_changed(_new_text: String) -> void:
	check_is_can_create();

func check_is_can_create() -> void:
	can_export = false;
	var output_path : String = _resolve_export_output_path(project_dir_edit.text);
	set_status("", false);
	if (output_path.is_empty()):
		set_status(tr("Export path cannot be empty"), true);
		return;
	var output_dir : String = output_path.get_base_dir();
	if (output_dir.is_empty()):
		set_status(tr("Export directory is invalid"), true);
		return;
	var opened_project_path : String = EditorProjectManager.get_opened_project_path();
	if (opened_project_path.is_empty()):
		set_status(tr("No project is currently open"), true);
		return;
	var runner_executable : String = GlobalEditorRunnerManager.get_runner_executable_path_for_platform("windows");
	if (_normalize_path(output_path) == _normalize_path(runner_executable)):
		set_status(tr("Export path cannot be runner executable"), true);
		return;
	project_dir_edit.text = output_path;
	set_status(tr("Ready to export"), false);
	can_export = true;

func set_export_state(enabled: bool) -> void:
	can_export = enabled;

func set_status(message: String, is_error: bool = false) -> void:
	if (!is_node_ready()): await ready;
	if (message.is_empty()):
		info_label.text = "";
		return;
	var color : String = "red" if is_error else "#97f28f";
	info_label.text = "[color=%s]%s[/color]" % [color, message];

func _apply_translations() -> void:
	title = tr("Export");

func _build_default_export_path() -> String:
	var opened_project_path : String = EditorProjectManager.get_opened_project_path();
	if (opened_project_path.is_empty()):
		return _get_default_export_file_name();
	return _normalize_path(opened_project_path.path_join(_get_default_export_file_name()));

func _get_default_export_file_name() -> String:
	var project_name : String = "game";
	if (is_instance_valid(EditorProjectManager.opened_project)):
		project_name = String(EditorProjectManager.opened_project.project_name).strip_edges();
	if (project_name.is_empty()):
		project_name = "game";
	return project_name + ".exe";

func _resolve_export_output_path(raw_path: String) -> String:
	var output_path : String = _normalize_path(raw_path.strip_edges());
	if (output_path.is_empty()):
		return "";
	if (output_path.ends_with("/") || DirAccess.dir_exists_absolute(output_path)):
		return _normalize_path(output_path.path_join(_get_default_export_file_name()));
	if (output_path.get_extension().to_lower() != "exe"):
		return output_path + ".exe";
	return output_path;

func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path();
