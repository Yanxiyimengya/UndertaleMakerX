class_name ProjectFileTemplates extends Object

const JS_SCRIPT_TEMPLATE : String = \
"""import { UTMX } from "UTMX";
"""


const MAIN_JS_SCRIPT_TEMPLATE : String = \
"""import { UTMX } from "UTMX";
export default class Main
{
	constructor() {
	}
	
	onGameStart()
	{
	}
	
	onGameEnd()
	{
	}
}"""

const GDSHADER_TEMPLATE : String = """shader_type canvas_item;

void vertex() {
}

void fragment() {
}
"""
