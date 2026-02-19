class_name WinbdowsManager extends CanvasLayer;

@onready var create_project_window: Window = %CreateProjectWindow;
@onready var confirmation_window: Window = %ConfirmationWindow;
@onready var question_window: Window = %QuestionWindow;
@onready var export_window: Window = %ExportWindow;

var gradient_color_rect : ColorRect;
var tween : Tween;
var prev_window : Window = null;

func _enter_tree() -> void:
	gradient_color_rect = ColorRect.new();
	gradient_color_rect.color = Color.BLACK;
	gradient_color_rect.visible = false;
	add_child(gradient_color_rect, false, Node.INTERNAL_MODE_FRONT);
	gradient_color_rect.set_anchors_preset(Control.PRESET_FULL_RECT);
	gradient_color_rect.set_offsets_preset(Control.PRESET_FULL_RECT);

func open_confirmation_window(title : String, message : String, \
		callback : Callable) -> void : 
	open_window(confirmation_window);
	confirmation_window.message = message;
	confirmation_window.title = title;
	for dict : Dictionary in confirmation_window.choiced.get_connections() : 
		confirmation_window.choiced.disconnect(dict["callable"]);
	confirmation_window.choiced.connect(callback, Object.CONNECT_ONE_SHOT);

func open_question_window(title : String, message : String, callback : Callable) -> void : 
	open_window(question_window);
	question_window.message = message;
	question_window.title = title;
	for dict : Dictionary in question_window.choiced.get_connections() : 
		question_window.choiced.disconnect(dict["callable"]);
	question_window.choiced.connect(callback, Object.CONNECT_ONE_SHOT);

func open_create_project_window(callback : Callable) -> void : 
	open_window(create_project_window);
	for dict : Dictionary in create_project_window.create_project_requset.get_connections() : 
		create_project_window.create_project_requset.disconnect(dict["callable"]);
	create_project_window.create_project_requset.connect(callback, Object.CONNECT_ONE_SHOT);

func open_export_window(callback : Callable = Callable()) -> void : 
	open_window(export_window);
	for dict : Dictionary in export_window.export_project_requset.get_connections() : 
		export_window.export_project_requset.disconnect(dict["callable"]);
	var target_callback : Callable = callback;
	if (!target_callback.is_valid()):
		target_callback = Callable(self, "_on_export_project_requset");
	export_window.export_project_requset.connect(target_callback);

func _on_export_project_requset(export_path : String) -> void:
	if (export_window != null && export_window.has_method("set_export_state")):
		export_window.call("set_export_state", false);
	var result : Dictionary = ProgramExporter.export_windows_embedded(
		EditorProjectManager.get_opened_project_path(),
		GlobalEditorRunnerManager.get_runner_executable_path_for_platform("windows"),
		export_path
	);
	if (bool(result.get("success", false))):
		var output_executable : String = String(result.get("output_executable", export_path));
		print("Export succeeded: %s" % output_executable);
		if (export_window != null && export_window.has_method("set_status")):
			export_window.call("set_status", tr("Export succeeded"), false);
		if (export_window != null && export_window.has_method("close")):
			export_window.call("close");
	else:
		var error_message : String = String(result.get("error", tr("Export failed")));
		push_error("Export failed: %s" % error_message);
		if (export_window != null && export_window.has_method("set_status")):
			export_window.call("set_status", error_message, true);
	if (export_window != null && export_window.has_method("set_export_state")):
		export_window.call("set_export_state", true);

func open_window(window : Window) : 
	if (prev_window != null) : 
		prev_window.hide();
	window.show();
	window._open();
	prev_window = window;
	gradient_color_rect.show();
	
	if (tween != null && tween.is_running()) : 
		tween.kill();
	tween = create_tween();
	tween.tween_property(gradient_color_rect, "color:a", 0.5, 0.2).from(0.0);
	
	window.close_requested.connect(func() : 
		window.hide();
		if (tween != null && tween.is_running()) : 
			tween.kill();
		tween = create_tween();
		tween.tween_property(gradient_color_rect, "color:a", 0.0, 0.2);
		await tween.finished;
		gradient_color_rect.hide();
		if (prev_window == window):
			prev_window = null;
	, Object.ConnectFlags.CONNECT_ONE_SHOT);
