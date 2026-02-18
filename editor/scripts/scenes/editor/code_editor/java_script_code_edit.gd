@tool
extends CodeEdit

const DECLARED_PATTERN := "(?<=[^\\p{L}\\p{N}_$]|^)(\\p{L}[\\p{L}\\p{N}_$]*)(?=\\s*\\()|(?<=[^\\p{L}\\p{N}_$]|^)(\\p{L}[\\p{L}\\p{N}_$]*)(?=[^\\p{L}\\p{N}_$]|$)(?!\\s*\\()"
const STRING_PATTERN := "\"[^\"]*\"|'[^']*'"
const BLOCK_COMMENT_PATTERN := "/\\*[\\s\\S]*?\\*/"
const LINE_COMMENT_PATTERN := "//.*$"
const IDENTIFIER_CHAR_PATTERN := "^[\\p{L}\\p{N}_$]$"
const DROP_DEDUP_WINDOW_MS: int = 40

const JS_KEYWORDS : PackedStringArray = [
	"arguments", "as", "async", "await", "break", "case", "catch", "class",
	"const", "continue", "debugger", "default", "delete", "do", "else",
	"enum", "export", "extends", "false", "finally", "for", "from",
	"function", "get", "if", "implements", "import", "in", "instanceof",
	"interface", "let", "new", "null", "of", "package", "private",
	"protected", "public", "return", "set", "static", "super", "switch",
	"this", "throw", "true", "try", "typeof", "var", "void", "while",
	"with", "yield"
];

@export var field_auto_completion_icon: Texture2D
@export var function_auto_completion_icon: Texture2D
@export var keyword_auto_completion_icon: Texture2D
@export var code_complete_timer: Timer
@export_range(8, 72, 1) var zoom_font_min_size := 10
@export_range(8, 72, 1) var zoom_font_max_size := 30
@export_range(1, 8, 1) var zoom_font_step := 1

var _last_parsed_line := -1
var _last_parsed_text := ""
var _code_complete_timer_line := -1
var _static_keywords: Dictionary = {}
#var _prev_caret_position := Vector2i(-1, -1)
var _cached_completion_options: Array[Dictionary] = []
var _cached_tokens: Dictionary = {}
var _last_drop_signature: String = ""
var _last_drop_tick_msec: int = -1

var _declared_regex := RegEx.new()
var _string_regex := RegEx.new()
var _block_comment_regex := RegEx.new()
var _line_comment_regex := RegEx.new()
var _identifier_char_regex := RegEx.new()


func _init() -> void:
	line_folding = true
	gutters_draw_fold_gutter = true
	gutters_draw_line_numbers = true
	code_completion_enabled = true
	
	indent_use_spaces = true
	indent_automatic = true

	auto_brace_completion_enabled = true
	auto_brace_completion_highlight_matching = true

	context_menu_enabled = false
	emoji_menu_enabled = false

	caret_blink = true
	caret_blink_interval = 0.45

	highlight_all_occurrences = true
	highlight_current_line = true

	_compile_regex(_declared_regex, DECLARED_PATTERN, "declared token")
	_compile_regex(_string_regex, STRING_PATTERN, "string")
	_compile_regex(_block_comment_regex, BLOCK_COMMENT_PATTERN, "block comment")
	_compile_regex(_line_comment_regex, LINE_COMMENT_PATTERN, "line comment")
	_compile_regex(_identifier_char_regex, IDENTIFIER_CHAR_PATTERN, "identifier char")

	for keyword in JS_KEYWORDS:
		_static_keywords[keyword] = true


func _enter_tree() -> void:
	if Engine.is_editor_hint():
		return
	if not text_changed.is_connected(_on_text_changed):
		text_changed.connect(_on_text_changed)
	if code_complete_timer and not code_complete_timer.timeout.is_connected(_on_code_complete_timer_timeout):
		code_complete_timer.timeout.connect(_on_code_complete_timer_timeout)


func _exit_tree() -> void:
	if text_changed.is_connected(_on_text_changed):
		text_changed.disconnect(_on_text_changed)
	if code_complete_timer and code_complete_timer.timeout.is_connected(_on_code_complete_timer_timeout):
		code_complete_timer.timeout.disconnect(_on_code_complete_timer_timeout)


func _gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		var mouse_event := event as InputEventMouseButton
		if mouse_event.pressed and mouse_event.is_command_or_control_pressed():
			var zoom_step := 0
			if mouse_event.button_index == MOUSE_BUTTON_WHEEL_UP:
				zoom_step = 1
			elif mouse_event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				zoom_step = -1

			if zoom_step != 0:
				_adjust_font_zoom(zoom_step)
				accept_event()
				return

	if event is InputEventKey and event.is_pressed():
		var key_event := event as InputEventKey
		if key_event.keycode == KEY_ENTER and key_event.shift_pressed:
			insert_text_at_caret("\n")


func _can_drop_data(_at_position: Vector2, data: Variant) -> bool:
	return not _extract_droppable_resource_paths(data).is_empty()


func _drop_data(_at_position: Vector2, data: Variant) -> void:
	var relative_paths: Array[String] = _extract_droppable_resource_paths(data)
	if relative_paths.is_empty():
		return
	var insertion_text: String = _build_resource_drop_text(relative_paths)
	if insertion_text.is_empty():
		return
	var signature_paths: String = "|".join(PackedStringArray(relative_paths))
	var drop_signature: String = "%s|%s|%d|%d" % [
		insertion_text,
		signature_paths,
		int(_at_position.x),
		int(_at_position.y)
	]
	var now_msec: int = Time.get_ticks_msec()
	if _last_drop_signature == drop_signature and _last_drop_tick_msec >= 0:
		if now_msec - _last_drop_tick_msec <= DROP_DEDUP_WINDOW_MS:
			return
	_last_drop_signature = drop_signature
	_last_drop_tick_msec = now_msec

	var drop_position: Vector2i = Vector2i(_at_position)
	var line_column: Vector2i = get_line_column_at_pos(drop_position, true, true)
	if line_column.x >= 0 and line_column.y >= 0:
		set_caret_line(line_column.y)
		set_caret_column(line_column.x)
	insert_text_at_caret(insertion_text)


func _request_code_completion(force: bool) -> void:
	var current_line := get_caret_line()
	var current_text := text
	var needs_reparse := force or current_text != _last_parsed_text or current_line > _last_parsed_line

	if needs_reparse:
		_cached_tokens = _extract_tokens(current_line)
		_last_parsed_line = current_line
		_last_parsed_text = current_text
		_cached_completion_options.clear()

	var word_before_cursor := _get_word_before_cursor()
	if word_before_cursor.strip_edges().is_empty():
		return

	if _cached_completion_options.size() > 0 and not force and word_before_cursor.length() > 0:
		_filter_and_show_completions(word_before_cursor)
	else:
		_build_and_show_completions(word_before_cursor)


func calculate_visible_column_capacity_width() -> float:
	var stylebox := get_theme_stylebox("normal")
	return (
		size.x
		- stylebox.get_margin(SIDE_LEFT)
		- stylebox.get_margin(SIDE_RIGHT)
		- get_total_gutter_width()
	)


func calculate_visible_column_capacity() -> int:
	var available_width := calculate_visible_column_capacity_width()
	var font := get_theme_font("font")
	if font == null:
		return 0
	var char_width := font.get_string_size(
		"0",
		HORIZONTAL_ALIGNMENT_LEFT,
		-1,
		get_theme_font_size("font_size")
	).x
	if is_zero_approx(char_width):
		return 0
	return int(available_width / char_width)


func _notification(what: int) -> void:
	if what == NOTIFICATION_RESIZED:
		_update_editor_visual_metrics()


func _on_text_changed() -> void:
	if code_completion_enabled and code_complete_timer:
		_code_complete_timer_line = get_caret_line()
		code_complete_timer.start()


func _on_code_complete_timer_timeout() -> void:
	if not is_inside_tree() or _code_complete_timer_line != get_caret_line():
		return
	call_deferred("request_code_completion", false)


func _extract_tokens(up_to_line: int) -> Dictionary:
	var content: Dictionary = {}
	var line_count := mini(up_to_line + 1, get_line_count())

	for i in line_count:
		var line := get_line(i)
		if line.strip_edges().is_empty():
			continue

		line = _string_regex.sub(line, "", true)
		line = _block_comment_regex.sub(line, "", true)
		line = _line_comment_regex.sub(line, "", true)

		var matches := _declared_regex.search_all(line)
		for m in matches:
			var token_name := ""
			var completion_kind := CodeEdit.KIND_VARIABLE
			var function_name := m.get_string(1)
			var member_name := m.get_string(2)

			if not function_name.is_empty():
				token_name = function_name
				completion_kind = CodeEdit.KIND_FUNCTION
			elif not member_name.is_empty():
				token_name = member_name
				completion_kind = CodeEdit.KIND_MEMBER

			if not token_name.is_empty() and not _static_keywords.has(token_name):
				if not content.has(token_name):
					content[token_name] = completion_kind

	return content


func _build_and_show_completions(prefix: String) -> void:
	_cached_completion_options.clear()

	for keyword in JS_KEYWORDS:
		if prefix.is_empty() or keyword.begins_with(prefix):
			_cached_completion_options.append({
				"kind": CodeEdit.KIND_CONSTANT,
				"display": keyword,
				"insert_text": keyword,
			})

	for token_name in _cached_tokens.keys():
		var token_name_string := str(token_name)
		if prefix.is_empty() or _starts_with_ignore_case(token_name_string, prefix):
			_cached_completion_options.append({
				"kind": int(_cached_tokens[token_name]),
				"display": token_name_string,
				"insert_text": token_name_string,
			})

	_filter_and_show_completions(prefix)


func _filter_and_show_completions(prefix: String) -> void:
	for option in _cached_completion_options:
		var display := str(option["display"])
		if _starts_with_ignore_case(display, prefix):
			var kind: int = option["kind"]
			add_code_completion_option(
				kind,
				display,
				str(option["insert_text"]),
				Color.WHITE,
				_get_icon_for_completion_kind(kind)
			)

	update_code_completion_options(false)


func _get_word_before_cursor() -> String:
	var column := get_caret_column()
	if column <= 0:
		return ""

	var line_index := get_caret_line()
	var line_text := get_line(line_index)
	column = mini(column, line_text.length())

	var start := column - 1
	while start >= 0 and _is_identifier_char(line_text.substr(start, 1)):
		start -= 1
	start += 1

	if start >= column:
		return ""
	return line_text.substr(start, column - start)


func _is_identifier_char(ch: String) -> bool:
	if ch.is_empty():
		return false
	return _identifier_char_regex.search(ch) != null


func _starts_with_ignore_case(value: String, prefix: String) -> bool:
	if prefix.is_empty():
		return true
	return value.to_lower().begins_with(prefix.to_lower())


func _get_icon_for_completion_kind(kind: int) -> Texture2D:
	match kind:
		CodeEdit.KIND_CONSTANT:
			return keyword_auto_completion_icon
		CodeEdit.KIND_FUNCTION:
			return function_auto_completion_icon
		_:
			return field_auto_completion_icon


func _compile_regex(regex: RegEx, pattern: String, name_for_log: String) -> void:
	var result := regex.compile(pattern)
	if result != OK:
		push_warning("Failed to compile %s regex: %s" % [name_for_log, pattern])


func _adjust_font_zoom(delta_steps: int) -> void:
	if delta_steps == 0:
		return
	var current_size := get_theme_font_size("font_size")
	if current_size <= 0:
		return
	var target_size := clampi(
		current_size + delta_steps * zoom_font_step,
		zoom_font_min_size,
		zoom_font_max_size
	)
	if target_size == current_size:
		return
	add_theme_font_size_override("font_size", target_size)
	_update_editor_visual_metrics()


func _update_editor_visual_metrics() -> void:
	minimap_width = mini(120, int(calculate_visible_column_capacity_width() * 0.17))
	minimap_draw = minimap_width > 30
	var v_scroll_bar := get_v_scroll_bar()
	if v_scroll_bar:
		v_scroll_bar.set_deferred("visible", not minimap_draw)

	var visible_columns := calculate_visible_column_capacity()
	var guideline := int(visible_columns * 0.8)
	if guideline > 0:
		set_line_length_guidelines(PackedInt32Array([guideline]))
	else:
		set_line_length_guidelines(PackedInt32Array())


func _extract_droppable_resource_paths(data: Variant) -> Array[String]:
	var raw_paths: Array[String] = []
	if data is Dictionary:
		if data.has("paths"):
			var dict_paths: Variant = data.get("paths")
			if dict_paths is PackedStringArray:
				for p in dict_paths:
					raw_paths.append(String(p))
			elif dict_paths is Array:
				for p in dict_paths:
					if p is String:
						raw_paths.append(String(p))
		elif data.has("files"):
			var files: Variant = data.get("files")
			if files is PackedStringArray:
				for p in files:
					raw_paths.append(String(p))
			elif files is Array:
				for p in files:
					if p is String:
						raw_paths.append(String(p))
	elif data is PackedStringArray:
		for p in data:
			raw_paths.append(String(p))
	elif data is Array:
		for p in data:
			if p is String:
				raw_paths.append(String(p))

	var relative_paths: Array[String] = []
	var unique_path_map: Dictionary = {}
	for raw_path in raw_paths:
		var normalized_path: String = String(raw_path).replace("\\", "/").simplify_path()
		if normalized_path.is_empty():
			continue
		if DirAccess.dir_exists_absolute(normalized_path):
			continue
		if not FileAccess.file_exists(normalized_path):
			continue
		var relative_path: String = _to_project_relative_path(normalized_path)
		if relative_path.is_empty():
			continue
		if unique_path_map.has(relative_path):
			continue
		unique_path_map[relative_path] = true
		relative_paths.append(relative_path)
	return relative_paths


func _to_project_relative_path(path: String) -> String:
	var normalized_path: String = String(path).replace("\\", "/").simplify_path()
	var project_root: String = String(EditorProjectManager.get_opened_project_path()).replace("\\", "/").simplify_path()
	if project_root.is_empty():
		return normalized_path
	if normalized_path == project_root:
		return "."
	if not project_root.ends_with("/"):
		project_root += "/"
	if normalized_path.begins_with(project_root):
		return normalized_path.trim_prefix(project_root)
	return normalized_path


func _build_resource_drop_text(relative_paths: Array[String]) -> String:
	if relative_paths.size() == 1:
		return _quote_as_script_string(relative_paths[0])

	var quoted_paths: Array[String] = []
	for path in relative_paths:
		quoted_paths.append(_quote_as_script_string(path))
	return "[" + ", ".join(PackedStringArray(quoted_paths)) + "]"


func _quote_as_script_string(value: String) -> String:
	var escaped: String = value.replace("\\", "\\\\").replace("\"", "\\\"")
	return "\"" + escaped + "\""
