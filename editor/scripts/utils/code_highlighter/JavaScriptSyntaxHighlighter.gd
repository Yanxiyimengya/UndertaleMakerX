@tool
class_name JavaScriptSyntaxHighlighter
extends CodeHighlighter


class JavaScriptSyntaxDB:
	const KEYWORDS: PackedStringArray = [
		"break",
		"case",
		"catch",
		"class",
		"const",
		"continue",
		"debugger",
		"default",
		"delete",
		"do",
		"else",
		"export",
		"extends",
		"finally",
		"for",
		"function",
		"if",
		"import",
		"in",
		"instanceof",
		"let",
		"new",
		"return",
		"super",
		"switch",
		"this",
		"throw",
		"try",
		"typeof",
		"var",
		"void",
		"while",
		"with",
		"yield",
		"async",
		"await",
		"enum",
		"implements",
		"interface",
		"package",
		"private",
		"protected",
		"public",
		"static"
	]

	const CONTROL_FLOW_KEYWORDS: PackedStringArray = [
		"if",
		"else",
		"for",
		"while",
		"do",
		"switch",
		"case",
		"default",
		"break",
		"continue",
		"return",
		"await",
		"export",
		"import",
	]

	const NORMAL_KEYWORDS: PackedStringArray = [
		"var",
		"let",
		"const",
		"class",
		"extends",
		"super",
		"try",
		"catch",
		"finally",
		"throw",
		"debugger",
		"delete",
		"in",
		"instanceof",
		"new",
		"this",
		"typeof",
		"void",
		"with",
		"yield",
		"async",
		"enum",
		"constructor",
		"function"
	]


@export var string_color: Color = Color8(206, 145, 120)
@export var comment_color: Color = Color8(106, 153, 85)
@export var control_flow_keyword_color: Color = Color8(206, 146, 164)
@export var keyword_color: Color = Color8(86, 156, 214)


func _init() -> void:
	_rebuild_highlight_rules()


func _rebuild_highlight_rules() -> void:
	clear_color_regions()
	clear_keyword_colors()

	add_color_region("'", "'", string_color)
	add_color_region('"', '"', string_color)
	add_color_region("//", "", comment_color, true)
	add_color_region("/*", "*/", comment_color)

	number_color = Color8(181, 206, 168)
	symbol_color = Color8(203, 203, 204)
	function_color = Color8(220, 220, 170)
	member_variable_color = Color8(155, 215, 255)

	for word in JavaScriptSyntaxDB.CONTROL_FLOW_KEYWORDS:
		add_keyword_color(word, control_flow_keyword_color)
	for word in JavaScriptSyntaxDB.NORMAL_KEYWORDS:
		add_keyword_color(word, keyword_color)
