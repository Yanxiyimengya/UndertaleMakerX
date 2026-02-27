extends Node

signal message_emitted(level: String, message: String)

const LEVEL_INFO: String = "info"
const LEVEL_WARNING: String = "warning"
const LEVEL_ERROR: String = "error"


func push(message: String, level: String = LEVEL_INFO) -> void:
	var normalized_message: String = String(message)
	if normalized_message.is_empty():
		return

	var normalized_level: String = String(level).strip_edges().to_lower()
	if normalized_level.is_empty():
		normalized_level = LEVEL_INFO

	message_emitted.emit(normalized_level, normalized_message)


func push_info(message: String) -> void:
	push(message, LEVEL_INFO)


func push_warning(message: String) -> void:
	push(message, LEVEL_WARNING)


func push_error(message: String) -> void:
	push(message, LEVEL_ERROR)
