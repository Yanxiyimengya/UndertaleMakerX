extends PanelContainer

@onready var file_system_tree: Tree = %FileSystemTree
@onready var search_line_edit: LineEdit = %SearchLineEdit

var _pre_search_expanded_paths: Array[String] = [];

func _ready() -> void:
	search_line_edit.text_changed.connect(_on_search_text_changed);

func _on_search_text_changed(new_text: String) -> void:
	if (! new_text.is_empty()):
		if file_system_tree._last_search_text.is_empty():
			_pre_search_expanded_paths = file_system_tree._expanded_paths.duplicate();
	file_system_tree.search(new_text);
	if (new_text.is_empty()):
		file_system_tree._expanded_paths = _pre_search_expanded_paths.duplicate();
		file_system_tree.refresh_tree("");
