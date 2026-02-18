extends Tree


func _can_drop_data(pos: Vector2, data: Variant) -> bool:
	var handler := _find_drop_handler()
	if handler == null:
		return false
	return bool(handler.call("can_drop_property_data", pos, data))


func _drop_data(pos: Vector2, data: Variant) -> void:
	var handler := _find_drop_handler()
	if handler == null:
		return
	handler.call("drop_property_data", pos, data)


func _find_drop_handler() -> Node:
	var node: Node = self
	while node != null:
		if node.has_method("can_drop_property_data") and node.has_method("drop_property_data"):
			return node
		node = node.get_parent()
	return null
