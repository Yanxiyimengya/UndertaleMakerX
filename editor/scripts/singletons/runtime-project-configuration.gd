## 运行时的项目设置信息
extends Node;

func _init() -> void:
	var arguments : Dictionary = {};
	for argument in OS.get_cmdline_args():
		if argument.contains("="):
			var key_value = argument.split("=")
			arguments[key_value[0].trim_prefix("--")] = key_value[1]
		else:
			# 没有参数的选项将出现在字典中，
			# 其值被设置为空字符串。
			arguments[argument.trim_prefix("--")] = ""
