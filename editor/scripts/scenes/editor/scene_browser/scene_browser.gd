extends PanelContainer;
@onready var sub_viewport: SubViewport = %SubViewport;

var _current_scene_path: String = "";
var _current_scene_root: Node = null;
var _pending_scene_path: String = "";
var _is_switching_scene: bool = false;
var _is_scene_dirty: bool = false;


func open_scene(path: String) -> void:
	load_scene(path);


func load_scene(path: String) -> void:
	var target_path := _normalize_path(path);
	if (target_path.is_empty()):
		return;
	if (!FileAccess.file_exists(target_path)):
		push_warning("Scene file not found: %s" % target_path);
		return;
	if (_is_switching_scene):
		return;
	if (_has_loaded_scene() && target_path == _current_scene_path):
		return;

	if (_has_loaded_scene() && target_path != _current_scene_path && _is_scene_dirty):
		_pending_scene_path = target_path;
		WindowManager.open_question_window(
			"Save Scene",
			"Current scene has changes. Save before opening another scene?",
			func(result: int) -> void:
				match (result):
					1:
						if (save_scene()):
							_unload_scene_internal();
							_load_scene_internal(_pending_scene_path);
					0:
						_unload_scene_internal();
						_load_scene_internal(_pending_scene_path);
					2:
						pass
				_pending_scene_path = "";
		);
		return;

	_unload_scene_internal();
	_load_scene_internal(target_path);


func unload_scene() -> void:
	_unload_scene_internal();


func save_scene() -> bool:
	if (!_has_loaded_scene()):
		return false;
	var saved := _save_scene_to_path(_current_scene_path);
	if (saved):
		_is_scene_dirty = false;
	return saved;


func mark_scene_dirty() -> void:
	if (_has_loaded_scene()):
		_is_scene_dirty = true;


func _unhandled_key_input(event: InputEvent) -> void:
	if (!visible):
		return;
	if (event is InputEventKey && event.is_pressed() && !event.is_echo()):
		if (event.is_command_or_control_pressed() && event.keycode == KEY_S):
			if (save_scene()):
				get_viewport().set_input_as_handled();


func _has_loaded_scene() -> bool:
	return _current_scene_root != null && is_instance_valid(_current_scene_root) && !_current_scene_path.is_empty();


func _load_scene_internal(path: String) -> void:
	if (path.is_empty()):
		return;
	_is_switching_scene = true;

	var packed_scene := _load_packed_scene(path);
	if (packed_scene == null):
		push_warning("Failed to load scene: %s" % path);
		_is_switching_scene = false;
		return;

	var scene_instance := packed_scene.instantiate();
	if (scene_instance == null):
		push_warning("Failed to instantiate scene: %s" % path);
		_is_switching_scene = false;
		return;

	sub_viewport.add_child(scene_instance);
	_current_scene_root = scene_instance;
	_current_scene_path = path;
	_is_scene_dirty = false;
	_is_switching_scene = false;


func _unload_scene_internal() -> void:
	if (_current_scene_root != null && is_instance_valid(_current_scene_root)):
		var parent := _current_scene_root.get_parent();
		if (parent != null):
			parent.remove_child(_current_scene_root);
		_current_scene_root.free();
	_current_scene_root = null;
	_current_scene_path = "";
	_is_scene_dirty = false;


func _load_packed_scene(path: String) -> PackedScene:
	var packed_scene : PackedScene = ResourceLoader.load(path) as PackedScene;
	if (packed_scene != null):
		return packed_scene;

	var project_absolute_root := _normalize_path(ProjectSettings.globalize_path("res://"));
	if (!project_absolute_root.ends_with("/")):
		project_absolute_root += "/";
	var absolute_path := _normalize_path(path);
	if (absolute_path.begins_with(project_absolute_root)):
		var relative := absolute_path.trim_prefix(project_absolute_root);
		packed_scene = ResourceLoader.load("res://%s" % relative) as PackedScene;
		if (packed_scene != null):
			return packed_scene;

	var loaded := GlobalEditorResourceLoader.load_resource(path);
	return loaded as PackedScene;


func _save_scene_to_path(target_path: String) -> bool:
	if (_current_scene_root == null || !is_instance_valid(_current_scene_root)):
		return false;
	if (target_path.is_empty()):
		return false;

	var base_dir := target_path.get_base_dir();
	if (!base_dir.is_empty() && !DirAccess.dir_exists_absolute(base_dir)):
		var make_err := DirAccess.make_dir_recursive_absolute(base_dir);
		if (make_err != OK && !DirAccess.dir_exists_absolute(base_dir)):
			push_warning("Failed to create scene directory: %s" % base_dir);
			return false;

	_assign_owner_recursive(_current_scene_root, _current_scene_root);

	var packed_scene := PackedScene.new();
	var pack_err := packed_scene.pack(_current_scene_root);
	if (pack_err != OK):
		push_warning("Failed to pack scene tree: %s (err=%d)" % [target_path, pack_err]);
		return false;

	var save_err := ResourceSaver.save(packed_scene, target_path);
	if (save_err != OK):
		var res_path := _to_res_path(target_path);
		if (!res_path.is_empty()):
			save_err = ResourceSaver.save(packed_scene, res_path);
	if (save_err != OK):
		push_warning("Failed to save scene: %s (err=%d)" % [target_path, save_err]);
		return false;

	GlobalEditorFileSystem.scan_project_incremental(base_dir);
	return true;


func _assign_owner_recursive(node: Node, owner: Node) -> void:
	for child in node.get_children():
		var child_node := child as Node;
		if (child_node == null):
			continue;
		child_node.owner = owner;
		_assign_owner_recursive(child_node, owner);


func _to_res_path(path: String) -> String:
	var project_absolute_root := _normalize_path(ProjectSettings.globalize_path("res://"));
	var absolute_path := _normalize_path(path);
	if (!project_absolute_root.ends_with("/")):
		project_absolute_root += "/";
	if (!absolute_path.begins_with(project_absolute_root)):
		return "";
	var relative := absolute_path.trim_prefix(project_absolute_root);
	return "res://%s" % relative;


func _normalize_path(path: String) -> String:
	return String(path).replace("\\", "/").simplify_path();
