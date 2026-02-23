extends Node
@onready var project_importor: Node = %ProjectImportor


func _ready() -> void:
	get_window().files_dropped.connect(_on_files_dropped)


func _on_files_dropped(files: PackedStringArray) -> void:
	for file_path: String in files:
		_process_external_file(file_path)


func _process_external_file(path: String) -> void:
	project_importor.import(path)
