extends PanelContainer

@onready var message_label: Label = %MessageLabel


func open_file(path: String) -> bool:
	message_label.text = "No preview is available for %s" % path.get_file()
	return true
