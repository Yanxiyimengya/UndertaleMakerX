class_name WinbdowsManager extends CanvasLayer

@onready var create_project_window: Window = %CreateProjectWindow
@onready var confirmation_window: Window = %ConfirmationWindow
@onready var question_window: Window = %QuestionWindow
@onready var export_window: Window = %ExportWindow

var gradient_color_rect: ColorRect
var tween: Tween
var prev_window: Window = null


func _enter_tree() -> void:
	gradient_color_rect = ColorRect.new()
	gradient_color_rect.color = Color.BLACK
	gradient_color_rect.visible = false
	add_child(gradient_color_rect, false, Node.INTERNAL_MODE_FRONT)
	gradient_color_rect.set_anchors_preset(Control.PRESET_FULL_RECT)
	gradient_color_rect.set_offsets_preset(Control.PRESET_FULL_RECT)


func open_confirmation_window(title: String, message: String, callback: Callable) -> void:
	open_window(confirmation_window)
	confirmation_window.message = message
	confirmation_window.title = title
	for dict: Dictionary in confirmation_window.choiced.get_connections():
		confirmation_window.choiced.disconnect(dict["callable"])
	confirmation_window.choiced.connect(callback, Object.CONNECT_ONE_SHOT)


func open_question_window(title: String, message: String, callback: Callable) -> void:
	open_window(question_window)
	question_window.message = message
	question_window.title = title
	for dict: Dictionary in question_window.choiced.get_connections():
		question_window.choiced.disconnect(dict["callable"])
	question_window.choiced.connect(callback, Object.CONNECT_ONE_SHOT)


func open_create_project_window(callback: Callable) -> void:
	open_window(create_project_window)
	for dict: Dictionary in create_project_window.create_project_requset.get_connections():
		create_project_window.create_project_requset.disconnect(dict["callable"])
	create_project_window.create_project_requset.connect(callback, Object.CONNECT_ONE_SHOT)


func open_export_window(callback: Callable = Callable()) -> void:
	open_window(export_window)
	for dict: Dictionary in export_window.export_project_requset.get_connections():
		export_window.export_project_requset.disconnect(dict["callable"])
	var target_callback: Callable = callback
	if !target_callback.is_valid():
		target_callback = Callable(self, "_on_export_project_requset")
	export_window.export_project_requset.connect(target_callback)


func _on_export_project_requset(export_path: String) -> void:
	if export_window != null && export_window.has_method("set_export_state"):
		export_window.call("set_export_state", false)
	var selected_platform: String = "windows"
	if export_window != null && export_window.has_method("get_selected_platform_id"):
		selected_platform = String(export_window.call("get_selected_platform_id")).strip_edges()
	_push_export_message("Export started (%s): %s" % [selected_platform, export_path], "info")
	var result: Dictionary = _export_project_by_platform(selected_platform, export_path)
	if bool(result.get("success", false)):
		var output_executable: String = String(result.get("output_executable", export_path))
		print("Export succeeded: %s" % output_executable)
		_push_export_message("Export succeeded: %s" % output_executable, "info")
		if export_window != null && export_window.has_method("set_status"):
			export_window.call("set_status", tr("Export succeeded"), false)
		if export_window != null && export_window.has_method("close"):
			export_window.call("close")
	else:
		var error_message: String = String(result.get("error", tr("Export failed")))
		push_error("Export failed: %s" % error_message)
		_push_export_message("Export failed: %s" % error_message, "error")
		if export_window != null && export_window.has_method("set_status"):
			export_window.call("set_status", error_message, true)
	if export_window != null && export_window.has_method("set_export_state"):
		export_window.call("set_export_state", true)


func _export_project_by_platform(platform_id: String, export_path: String) -> Dictionary:
	match platform_id.to_lower():
		"windows":
			return ProgramExporter.export_windows_embedded(
				EditorProjectManager.get_opened_project_path(),
				GlobalEditorRunnerManager.get_runner_executable_path_for_platform("windows"),
				export_path
			)
		"linux":
			return ProgramExporter.export_linux_embedded(
				EditorProjectManager.get_opened_project_path(),
				GlobalEditorRunnerManager.get_runner_executable_path_for_platform("linux"),
				export_path
			)
		"android":
			var android_signing_options: Dictionary = {}
			if export_window != null && export_window.has_method("get_android_signing_options"):
				var options_variant: Variant = export_window.call("get_android_signing_options")
				if options_variant is Dictionary:
					android_signing_options = options_variant
			return ProgramExporter.export_android_apk(
				EditorProjectManager.get_opened_project_path(),
				GlobalEditorRunnerManager.get_runner_executable_path_for_platform("android"),
				export_path,
				android_signing_options
			)
		_:
			return {
				"success": false,
				"error": tr("Unsupported export platform: %s") % platform_id,
			}


func _push_export_message(message: String, level: String = "info") -> void:
	var output_manager: Node = get_node_or_null("/root/EditorOutputManager")
	if output_manager == null || !output_manager.has_method("push"):
		return
	output_manager.call("push", message, level)


func open_window(window: Window):
	if prev_window != null:
		prev_window.hide()
	window.show()
	window._open()
	prev_window = window
	gradient_color_rect.show()

	if tween != null && tween.is_running():
		tween.kill()
	tween = create_tween()
	tween.tween_property(gradient_color_rect, "color:a", 0.5, 0.2).from(0.0)

	window.close_requested.connect(
		func():
			window.hide()
			if tween != null && tween.is_running():
				tween.kill()
			tween = create_tween()
			tween.tween_property(gradient_color_rect, "color:a", 0.0, 0.2)
			await tween.finished
			gradient_color_rect.hide()
			if prev_window == window:
				prev_window = null,
		Object.ConnectFlags.CONNECT_ONE_SHOT
	)
