extends UtmxEditorWindow

signal export_project_requset(export_dir: String)

const TREE_COLUMN := 0
const PLATFORM_WINDOWS := "windows"
const PLATFORM_LINUX := "linux"
const PLATFORM_ANDROID := "android"
const EXPORT_PLATFORMS := [
	{
		"id": PLATFORM_WINDOWS,
		"display_name": "Windows",
		"tab_panel": "WindowsExportPanel",
		"runner_platform": "windows",
		"default_extension": "exe",
		"dialog_filters": ["*.exe ; Windows Executable"],
	},
	{
		"id": PLATFORM_LINUX,
		"display_name": "Linux",
		"tab_panel": "WindowsExportPanel",
		"runner_platform": "linux",
		"default_extension": "",
		"dialog_filters": ["* ; Linux Executable"],
	},
	{
		"id": PLATFORM_ANDROID,
		"display_name": "Android",
		"tab_panel": "WindowsExportPanel",
		"runner_platform": "android",
		"default_extension": "apk",
		"dialog_filters": ["*.apk ; Android Package"],
	}
]
const WINDOWS_INVALID_FILE_CHARS := ["<", ">", ":", "\"", "/", "\\", "|", "?", "*"]
const LINUX_INVALID_FILE_CHARS := ["/"]
const WINDOWS_RESERVED_FILE_NAMES := [
	"con",
	"prn",
	"aux",
	"nul",
	"com1",
	"com2",
	"com3",
	"com4",
	"com5",
	"com6",
	"com7",
	"com8",
	"com9",
	"lpt1",
	"lpt2",
	"lpt3",
	"lpt4",
	"lpt5",
	"lpt6",
	"lpt7",
	"lpt8",
	"lpt9",
]

@onready var app_name_edit: LineEdit = %AppNameEdit
@onready var app_name_label: Label = %AppNameLabel
@onready var platform_title_label: Label = %WindowsTitleLabel
@onready var platforms_title_label: Label = %PlatformsTitleLabel
@onready var project_dir_edit: LineEdit = %ProjectDirEdit
@onready var project_dir_label: Label = %ProjectDirLabel
@onready var info_label: RichTextLabel = %InfoLabel
@onready var android_signing_group: VBoxContainer = %AndroidSigningGroup
@onready var android_signing_title_label: Label = %AndroidSigningTitleLabel
@onready var android_jdk_path_label: Label = %AndroidJdkPathLabel
@onready var android_jdk_path_edit: LineEdit = %AndroidJdkPathEdit
@onready var android_keystore_path_label: Label = %AndroidKeystorePathLabel
@onready var android_keystore_path_edit: LineEdit = %AndroidKeystorePathEdit
@onready var android_keystore_alias_label: Label = %AndroidKeystoreAliasLabel
@onready var android_keystore_alias_edit: LineEdit = %AndroidKeystoreAliasEdit
@onready var android_keystore_password_label: Label = %AndroidKeystorePasswordLabel
@onready var android_keystore_password_edit: LineEdit = %AndroidKeystorePasswordEdit
@onready var android_key_password_label: Label = %AndroidKeyPasswordLabel
@onready var android_key_password_edit: LineEdit = %AndroidKeyPasswordEdit
@onready var cancel_button: Button = %CancelButton
@onready var export_button: Button = %ExportButton
@onready var status_label: Label = %StatusLabel
@onready var export_platforms_tree: Tree = %ExportPlatformsTree
@onready var export_platform_tabs: TabContainer = %ExportPlatformTabs

var _platform_tab_index_by_id: Dictionary = {}
var _platform_tree_item_by_id: Dictionary = {}
var _selected_platform_id: String = PLATFORM_WINDOWS
var _is_syncing_output_path: bool = false
var _is_output_path_customized: bool = false
var _is_loading_android_signing_options: bool = false
var _android_signing_loaded_project_path: String = ""

var can_export: bool = true:
	set(value):
		can_export = value
		if !is_node_ready():
			await ready
		export_button.disabled = !value


func _ready() -> void:
	_setup_export_platforms()
	if !project_dir_edit.text_changed.is_connected(_on_project_dir_edit_text_changed):
		project_dir_edit.text_changed.connect(_on_project_dir_edit_text_changed)
	if !app_name_edit.text_changed.is_connected(_on_app_name_edit_text_changed):
		app_name_edit.text_changed.connect(_on_app_name_edit_text_changed)
	if !android_jdk_path_edit.text_changed.is_connected(_on_android_signing_field_changed):
		android_jdk_path_edit.text_changed.connect(_on_android_signing_field_changed)
	if !android_keystore_path_edit.text_changed.is_connected(_on_android_signing_field_changed):
		android_keystore_path_edit.text_changed.connect(_on_android_signing_field_changed)
	if !android_keystore_alias_edit.text_changed.is_connected(_on_android_signing_field_changed):
		android_keystore_alias_edit.text_changed.connect(_on_android_signing_field_changed)
	if !android_keystore_password_edit.text_changed.is_connected(_on_android_signing_field_changed):
		android_keystore_password_edit.text_changed.connect(_on_android_signing_field_changed)
	if !export_platforms_tree.item_selected.is_connected(_on_export_platform_tree_item_selected):
		export_platforms_tree.item_selected.connect(_on_export_platform_tree_item_selected)
	if !export_platform_tabs.tab_changed.is_connected(_on_export_platform_tabs_tab_changed):
		export_platform_tabs.tab_changed.connect(_on_export_platform_tabs_tab_changed)
	if !visibility_changed.is_connected(_on_visibility_changed):
		visibility_changed.connect(_on_visibility_changed)
	_configure_android_password_fields()
	_ensure_android_signing_options_loaded()
	_apply_translations()
	check_is_can_create()


func _notification(what: int) -> void:
	if what == NOTIFICATION_TRANSLATION_CHANGED && is_node_ready():
		_apply_translations()


func _open() -> void:
	_is_output_path_customized = false
	app_name_edit.text = _build_default_export_app_name()
	_ensure_android_signing_options_loaded()
	_sync_android_key_password_with_keystore()
	_set_project_dir_text(_build_default_export_path())
	_apply_platform_selection(_selected_platform_id, true)
	check_is_can_create()
	call_deferred("check_is_can_create")
	app_name_edit.grab_focus()
	app_name_edit.caret_column = app_name_edit.text.length()


func _on_dir_texture_button_pressed() -> void:
	var current_output_path: String = _resolve_export_output_path(
		project_dir_edit.text, _selected_platform_id
	)
	var current_dir: String = current_output_path.get_base_dir()
	var current_file: String = current_output_path.get_file()
	if current_dir.is_empty():
		current_dir = EditorProjectManager.get_opened_project_path()
	if current_file.is_empty():
		current_file = _get_default_export_file_name(_selected_platform_id)
	var filters: PackedStringArray = _get_platform_dialog_filters(_get_selected_platform_config())
	if filters.is_empty():
		filters = PackedStringArray(["*.* ; All Files"])
	DisplayServer.file_dialog_show(
		tr("Select Export File"),
		current_dir,
		current_file,
		false,
		DisplayServer.FILE_DIALOG_MODE_SAVE_FILE,
		filters,
		func(status: bool, selected_paths: PackedStringArray, _idx: int):
			if !status || selected_paths.is_empty():
				return
			_is_output_path_customized = true
			_set_project_dir_text(selected_paths[0])
			check_is_can_create()
	)


func _on_cancel_button_pressed() -> void:
	self.close()


func _on_export_button_pressed() -> void:
	check_is_can_create()
	if !can_export:
		var status_text: String = _get_status_plain_text()
		if !status_text.is_empty():
			var output_manager: Node = get_node_or_null("/root/EditorOutputManager")
			if output_manager != null && output_manager.has_method("push"):
				output_manager.call("push", status_text, "error")
		return
	export_project_requset.emit(_resolve_export_output_path(project_dir_edit.text, _selected_platform_id))


func _on_project_dir_edit_text_changed(_new_text: String) -> void:
	if _is_syncing_output_path:
		return
	_is_output_path_customized = true
	check_is_can_create()


func _on_app_name_edit_text_changed(_new_text: String) -> void:
	if !_is_output_path_customized:
		_set_project_dir_text(_build_default_export_path())
	check_is_can_create()


func _on_export_platform_tree_item_selected() -> void:
	var selected_item: TreeItem = export_platforms_tree.get_selected()
	if selected_item == null:
		return
	var selected_platform_id: String = String(selected_item.get_metadata(TREE_COLUMN)).strip_edges()
	if selected_platform_id.is_empty():
		return
	_apply_platform_selection(selected_platform_id)


func _on_export_platform_tabs_tab_changed(_tab: int) -> void:
	check_is_can_create()


func _on_visibility_changed() -> void:
	if visible:
		check_is_can_create()


func _on_android_signing_field_changed(_new_text: String) -> void:
	if _is_loading_android_signing_options:
		return
	_sync_android_key_password_with_keystore()
	_save_android_signing_options_to_project()
	check_is_can_create()


func _on_android_jdk_path_button_pressed() -> void:
	var current_dir: String = _normalize_path(android_jdk_path_edit.text.strip_edges())
	if current_dir.is_empty():
		current_dir = EditorProjectManager.get_opened_project_path()
	DisplayServer.file_dialog_show(
		tr("Select JDK Bin Directory"),
		current_dir,
		"",
		false,
		DisplayServer.FILE_DIALOG_MODE_OPEN_DIR,
		PackedStringArray(),
		func(status: bool, selected_paths: PackedStringArray, _idx: int):
			if !status || selected_paths.is_empty():
				return
			android_jdk_path_edit.text = selected_paths[0]
			check_is_can_create()
	)


func _on_android_keystore_path_button_pressed() -> void:
	var current_path: String = _normalize_path(android_keystore_path_edit.text.strip_edges())
	var current_dir: String = current_path.get_base_dir()
	var current_file: String = current_path.get_file()
	if current_dir.is_empty():
		current_dir = EditorProjectManager.get_opened_project_path()
	DisplayServer.file_dialog_show(
		tr("Select Keystore"),
		current_dir,
		current_file,
		false,
		DisplayServer.FILE_DIALOG_MODE_OPEN_FILE,
		PackedStringArray(
			[
				"*.jks,*.keystore,*.p12",
				"*.* ; All Files",
			]
		),
		func(status: bool, selected_paths: PackedStringArray, _idx: int):
			if !status || selected_paths.is_empty():
				return
			android_keystore_path_edit.text = selected_paths[0]
			check_is_can_create()
	)


func check_is_can_create() -> void:
	can_export = false
	set_status("", false)

	var resolved_platform_id: String = _resolve_platform_id(_selected_platform_id)
	if resolved_platform_id.is_empty():
		set_status(tr("No export platform is available"), true)
		return

	var app_name: String = _get_export_app_name()
	if app_name.is_empty():
		set_status(tr("Application name cannot be empty"), true)
		return
	var app_name_error: String = _validate_app_name_for_platform(app_name, resolved_platform_id)
	if !app_name_error.is_empty():
		set_status(app_name_error, true)
		return
	if resolved_platform_id == PLATFORM_ANDROID:
		var android_error: String = _validate_android_signing_inputs()
		if !android_error.is_empty():
			set_status(android_error, true)
			return

	var output_path: String = _resolve_export_output_path(project_dir_edit.text, resolved_platform_id)
	if output_path.is_empty():
		set_status(tr("Export path cannot be empty"), true)
		return
	if !output_path.is_absolute_path():
		set_status(tr("Export path must be an absolute path"), true)
		return
	var output_dir: String = output_path.get_base_dir()
	if output_dir.is_empty():
		set_status(tr("Export directory is invalid"), true)
		return

	var opened_project_path: String = EditorProjectManager.get_opened_project_path()
	if opened_project_path.is_empty():
		set_status(tr("No project is currently open"), true)
		return

	var runner_platform: String = _get_runner_platform_name(resolved_platform_id)
	var runner_executable: String = (
		GlobalEditorRunnerManager.get_runner_executable_path_for_platform(runner_platform)
	)
	if runner_executable.is_empty() or !FileAccess.file_exists(runner_executable):
		set_status(
			tr("Runner executable not found for platform: %s") % runner_platform,
			true
		)
		return
	if _normalize_path(output_path) == _normalize_path(runner_executable):
		set_status(tr("Export path cannot be runner executable"), true)
		return

	if _normalize_path(project_dir_edit.text) != _normalize_path(output_path):
		_set_project_dir_text(output_path)
	set_status(tr("Ready to export"), false)
	can_export = true


func get_selected_platform_id() -> String:
	return _selected_platform_id


func set_export_state(enabled: bool) -> void:
	can_export = enabled


func set_status(message: String, is_error: bool = false) -> void:
	if !is_node_ready():
		await ready
	var plain_message: String = String(message).strip_edges()
	if message.is_empty():
		info_label.text = ""
		if status_label != null:
			status_label.text = ""
		return
	var color: String = "red" if is_error else "#97f28f"
	info_label.text = "[color=%s]%s[/color]" % [color, message]
	if status_label != null:
		status_label.text = plain_message
		status_label.modulate = Color(1.0, 0.38, 0.38, 1.0) if is_error else Color(0.59, 0.95, 0.56, 1.0)


func _get_status_plain_text() -> String:
	if status_label != null:
		return String(status_label.text).strip_edges()
	return info_label.get_parsed_text().strip_edges()


func _apply_translations() -> void:
	title = tr("Export")
	platforms_title_label.text = tr("Platforms")
	app_name_label.text = tr("Application Name")
	project_dir_label.text = tr("Location")
	android_signing_title_label.text = tr("Android Signing")
	android_jdk_path_label.text = tr("JDK Bin Directory")
	android_keystore_path_label.text = tr("Keystore Path")
	android_keystore_alias_label.text = tr("Keystore Alias")
	android_keystore_password_label.text = tr("Keystore Password")
	android_key_password_label.text = tr("Key Password")
	cancel_button.text = tr("Cancel")
	export_button.text = tr("Export")
	_refresh_platform_title()
	_refresh_platform_tree_labels()


func _build_default_export_path() -> String:
	var opened_project_path: String = EditorProjectManager.get_opened_project_path()
	var default_file_name: String = _get_default_export_file_name(_selected_platform_id)
	if opened_project_path.is_empty():
		return default_file_name
	return _normalize_path(opened_project_path.path_join(default_file_name))


func _build_default_export_app_name() -> String:
	var project_name: String = "game"
	if is_instance_valid(EditorProjectManager.opened_project):
		project_name = String(EditorProjectManager.opened_project.project_name).strip_edges()
	if project_name.is_empty():
		project_name = "game"
	return project_name


func _get_export_app_name() -> String:
	var app_name: String = String(app_name_edit.text).strip_edges()
	if app_name.is_empty():
		app_name = _build_default_export_app_name()
	return app_name


func _get_default_export_file_name(platform_id: String) -> String:
	var app_name: String = _get_export_app_name()
	var extension: String = _get_platform_default_extension(platform_id)
	if extension.is_empty():
		return app_name
	if app_name.get_extension().to_lower() == extension:
		return app_name
	return "%s.%s" % [app_name, extension]


func _resolve_export_output_path(raw_path: String, platform_id: String = _selected_platform_id) -> String:
	var output_path: String = _normalize_path(raw_path.strip_edges())
	if output_path.is_empty():
		return ""
	var extension: String = _get_platform_default_extension(platform_id)
	if output_path.ends_with("/") || DirAccess.dir_exists_absolute(output_path):
		return _normalize_path(output_path.path_join(_get_default_export_file_name(platform_id)))
	if !extension.is_empty() && output_path.get_extension().to_lower() != extension:
		return output_path + "." + extension
	return output_path


func _setup_export_platforms() -> void:
	export_platforms_tree.columns = 1
	export_platforms_tree.hide_root = true
	export_platforms_tree.select_mode = Tree.SELECT_SINGLE
	export_platforms_tree.allow_rmb_select = false
	export_platforms_tree.clear()
	_platform_tab_index_by_id.clear()
	_platform_tree_item_by_id.clear()

	var root: TreeItem = export_platforms_tree.create_item()
	var first_platform_id: String = ""
	for platform_variant in EXPORT_PLATFORMS:
		if !(platform_variant is Dictionary):
			continue
		var platform: Dictionary = platform_variant
		var platform_id: String = String(platform.get("id", "")).strip_edges().to_lower()
		if platform_id.is_empty():
			continue
		var tab_index: int = _find_platform_tab_index(platform)
		if tab_index < 0:
			continue
		_platform_tab_index_by_id[platform_id] = tab_index
		var item: TreeItem = export_platforms_tree.create_item(root)
		item.set_metadata(TREE_COLUMN, platform_id)
		item.set_text(TREE_COLUMN, _get_platform_display_name(platform))
		_platform_tree_item_by_id[platform_id] = item
		if first_platform_id.is_empty():
			first_platform_id = platform_id

	if first_platform_id.is_empty():
		_selected_platform_id = ""
		return
	if !_platform_tab_index_by_id.has(_selected_platform_id):
		_selected_platform_id = first_platform_id
	_apply_platform_selection(_selected_platform_id, true)


func _refresh_platform_tree_labels() -> void:
	for platform_variant in EXPORT_PLATFORMS:
		if !(platform_variant is Dictionary):
			continue
		var platform: Dictionary = platform_variant
		var platform_id: String = String(platform.get("id", "")).strip_edges().to_lower()
		var item_variant: Variant = _platform_tree_item_by_id.get(platform_id, null)
		if item_variant is TreeItem:
			var item: TreeItem = item_variant
			item.set_text(TREE_COLUMN, _get_platform_display_name(platform))


func _apply_platform_selection(platform_id: String, force_tree_selection: bool = false) -> void:
	var resolved_platform_id: String = _resolve_platform_id(platform_id)
	if resolved_platform_id.is_empty():
		return
	if resolved_platform_id == PLATFORM_ANDROID:
		_ensure_android_signing_options_loaded()
	_selected_platform_id = resolved_platform_id
	_refresh_platform_title()
	_refresh_platform_specific_sections()

	var tab_index: int = int(_platform_tab_index_by_id.get(_selected_platform_id, -1))
	if tab_index >= 0 && tab_index < export_platform_tabs.get_tab_count():
		export_platform_tabs.current_tab = tab_index

	if force_tree_selection:
		var item_variant: Variant = _platform_tree_item_by_id.get(_selected_platform_id, null)
		if item_variant is TreeItem:
			var item: TreeItem = item_variant
			export_platforms_tree.deselect_all()
			item.select(TREE_COLUMN)
			export_platforms_tree.scroll_to_item(item)

	if !_is_output_path_customized:
		_set_project_dir_text(_build_default_export_path())
	check_is_can_create()


func _refresh_platform_title() -> void:
	var platform: Dictionary = _get_platform_config(_selected_platform_id)
	if platform.is_empty():
		platform_title_label.text = ""
		return
	platform_title_label.text = _get_platform_display_name(platform)


func _refresh_platform_specific_sections() -> void:
	android_signing_group.visible = (_selected_platform_id == PLATFORM_ANDROID)


func _resolve_platform_id(platform_id: String) -> String:
	var normalized_id: String = String(platform_id).strip_edges().to_lower()
	if _platform_tab_index_by_id.has(normalized_id):
		return normalized_id
	for platform_key in _platform_tab_index_by_id.keys():
		return String(platform_key)
	return ""


func _find_platform_tab_index(platform: Dictionary) -> int:
	var tab_panel_name: String = String(platform.get("tab_panel", "")).strip_edges()
	if tab_panel_name.is_empty():
		return -1
	for i in range(export_platform_tabs.get_child_count()):
		var child: Node = export_platform_tabs.get_child(i)
		if child != null && child.name == tab_panel_name:
			return i
	return -1


func _get_selected_platform_config() -> Dictionary:
	return _get_platform_config(_selected_platform_id)


func _get_platform_config(platform_id: String) -> Dictionary:
	var normalized_id: String = String(platform_id).strip_edges().to_lower()
	for platform_variant in EXPORT_PLATFORMS:
		if !(platform_variant is Dictionary):
			continue
		var platform: Dictionary = platform_variant
		var current_id: String = String(platform.get("id", "")).strip_edges().to_lower()
		if current_id == normalized_id:
			return platform
	return {}


func _get_platform_display_name(platform: Dictionary) -> String:
	var display_name: String = String(platform.get("display_name", "")).strip_edges()
	if display_name.is_empty():
		display_name = String(platform.get("id", "")).strip_edges().capitalize()
	return tr(display_name)


func _get_platform_default_extension(platform_id: String) -> String:
	var platform: Dictionary = _get_platform_config(platform_id)
	return String(platform.get("default_extension", "")).strip_edges().trim_prefix(".").to_lower()


func _get_platform_dialog_filters(platform: Dictionary) -> PackedStringArray:
	var filters_variant: Variant = platform.get("dialog_filters", PackedStringArray())
	if filters_variant is PackedStringArray:
		return filters_variant
	if filters_variant is Array:
		var filters := PackedStringArray()
		for filter_variant in filters_variant:
			filters.append(String(filter_variant))
		return filters
	return PackedStringArray()


func _get_runner_platform_name(platform_id: String) -> String:
	var platform: Dictionary = _get_platform_config(platform_id)
	var runner_platform: String = String(platform.get("runner_platform", platform_id))
	runner_platform = runner_platform.strip_edges().to_lower()
	if runner_platform.is_empty():
		return platform_id
	return runner_platform


func _validate_app_name_for_platform(app_name: String, platform_id: String) -> String:
	var normalized_name: String = app_name.strip_edges()
	if normalized_name.is_empty():
		return tr("Application name cannot be empty")
	match platform_id:
		PLATFORM_WINDOWS:
			for invalid_char: String in WINDOWS_INVALID_FILE_CHARS:
				if normalized_name.contains(invalid_char):
					return tr("Application name contains invalid Windows filename characters")
			if normalized_name.ends_with(".") || normalized_name.ends_with(" "):
				return tr("Application name cannot end with a dot or space")
			var base_name: String = normalized_name.get_basename().to_lower()
			if WINDOWS_RESERVED_FILE_NAMES.has(base_name):
				return tr("Application name is a reserved Windows filename")
		PLATFORM_LINUX:
			for invalid_char: String in LINUX_INVALID_FILE_CHARS:
				if normalized_name.contains(invalid_char):
					return tr("Application name contains invalid Linux filename characters")
			if normalized_name == "." || normalized_name == "..":
				return tr("Application name cannot be . or ..")
	return ""


func _validate_android_signing_inputs() -> String:
	var jdk_path: String = _normalize_path(android_jdk_path_edit.text.strip_edges())
	if jdk_path.is_empty():
		return tr("JDK path cannot be empty")
	if !jdk_path.is_absolute_path():
		return tr("JDK path must be an absolute path")
	if !DirAccess.dir_exists_absolute(jdk_path):
		return tr("JDK path does not exist")

	var keystore_path: String = _normalize_path(android_keystore_path_edit.text.strip_edges())
	if keystore_path.is_empty():
		return tr("Keystore path cannot be empty")
	if !keystore_path.is_absolute_path():
		return tr("Keystore path must be an absolute path")
	if !FileAccess.file_exists(keystore_path):
		return tr("Keystore file does not exist")

	var alias: String = android_keystore_alias_edit.text.strip_edges()
	if alias.is_empty():
		return tr("Keystore alias cannot be empty")

	if android_keystore_password_edit.text.is_empty():
		return tr("Keystore password cannot be empty")

	var jarsigner_path: String = _resolve_jarsigner_in_jdk_bin(jdk_path)
	if jarsigner_path.is_empty():
		return tr("Cannot find jarsigner in selected JDK directory")
	_sync_android_key_password_with_keystore()

	return ""


func get_android_signing_options() -> Dictionary:
	_sync_android_key_password_with_keystore()
	return {
		"jdk_bin_dir": _normalize_path(android_jdk_path_edit.text.strip_edges()),
		"keystore_path": _normalize_path(android_keystore_path_edit.text.strip_edges()),
		"keystore_alias": android_keystore_alias_edit.text.strip_edges(),
		"keystore_password": android_keystore_password_edit.text,
		"key_password": android_key_password_edit.text,
	}


func _load_android_signing_options_from_project() -> void:
	var project_path: String = _normalize_path(EditorProjectManager.get_opened_project_path())
	if project_path.is_empty():
		return
	var options: Dictionary = EditorProjectManager.get_project_android_export_options(project_path)
	_is_loading_android_signing_options = true
	android_jdk_path_edit.text = String(options.get("jdk_bin_dir", ""))
	android_keystore_path_edit.text = String(options.get("keystore_path", ""))
	android_keystore_alias_edit.text = String(options.get("keystore_alias", ""))
	android_keystore_password_edit.text = String(options.get("keystore_password", ""))
	android_key_password_edit.text = String(options.get("key_password", ""))
	_is_loading_android_signing_options = false
	_android_signing_loaded_project_path = project_path


func _ensure_android_signing_options_loaded(force: bool = false) -> void:
	var project_path: String = _normalize_path(EditorProjectManager.get_opened_project_path())
	if project_path.is_empty():
		return
	if !force and _android_signing_loaded_project_path == project_path:
		return
	_load_android_signing_options_from_project()


func _save_android_signing_options_to_project() -> void:
	var opened_project_path: String = EditorProjectManager.get_opened_project_path()
	if opened_project_path.is_empty():
		return
	EditorProjectManager.set_project_android_export_options(
		get_android_signing_options(), opened_project_path
	)


func _configure_android_password_fields() -> void:
	if android_key_password_edit != null:
		var key_password_group: Control = android_key_password_edit.get_parent() as Control
		if key_password_group != null:
			key_password_group.visible = false
	_sync_android_key_password_with_keystore()


func _sync_android_key_password_with_keystore() -> void:
	if android_key_password_edit == null or android_keystore_password_edit == null:
		return
	var keystore_password: String = android_keystore_password_edit.text
	if android_key_password_edit.text == keystore_password:
		return
	android_key_password_edit.text = keystore_password


func _resolve_jarsigner_in_jdk_bin(jdk_bin_dir: String) -> String:
	var normalized_dir: String = _normalize_path(jdk_bin_dir.strip_edges())
	if normalized_dir.is_empty():
		return ""

	var candidates: PackedStringArray = PackedStringArray()
	if OS.get_name().to_lower() == "windows":
		candidates.append(_normalize_path(normalized_dir.path_join("jarsigner.exe")))
		candidates.append(_normalize_path(normalized_dir.path_join("bin/jarsigner.exe")))
	else:
		candidates.append(_normalize_path(normalized_dir.path_join("jarsigner")))
		candidates.append(_normalize_path(normalized_dir.path_join("bin/jarsigner")))

	for candidate_path: String in candidates:
		if FileAccess.file_exists(candidate_path):
			return candidate_path
	return ""


func _set_project_dir_text(path: String) -> void:
	var normalized_target: String = _normalize_path(path)
	if _normalize_path(project_dir_edit.text) == normalized_target:
		return
	_is_syncing_output_path = true
	project_dir_edit.text = path
	_is_syncing_output_path = false


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path()
