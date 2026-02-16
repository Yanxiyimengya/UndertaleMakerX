@abstract
class_name UtmxEditorWindow extends Window;

@abstract
func _open() -> void;

func close() -> void : 
	close_requested.emit();
	hide();
