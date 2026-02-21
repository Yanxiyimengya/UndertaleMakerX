import { __Vector2 , __DrawableObject } from "__UTMX";
import { __Color } from "res://scripts/js/wrappers/types/utmx-color.wrapper.js";
import { UtmxTransformableObject } from "./utmx-transformable-object.wrapper.js";

export class UtmxDrawableObject extends UtmxTransformableObject {
	static new()
	{
		let ins = new this();
		ins.__instance = __DrawableObject.New(ins);
		return ins;
	}

	get color() {
		return this.__instance.Modulate;
	}
	set color(value) {
		this.__instance.Modulate = value;
	}
	get shader() { return this.__instance.ShaderInstance; }
	set shader(value) { this.__instance.ShaderInstance = value; }

	drawCircle(pos, radius, color = new __Color(1,1,1))
	{
		this.__instance.DrawCircle(pos, radius, color);
	}
	drawRect(pos, size, color = new __Color(1,1,1))
	{
		this.__instance.DrawRect(pos, size, color);
	}
	drawLine(from, to , color = new __Color(1,1,1), width = -1)
	{
		this.__instance.DrawLine(from, to, color, width);
	}
	drawTextureRect(path, pos, size, color = new __Color(1,1,1))
	{
		this.__instance.DrawTextureRect(pos, size, path, color);
	}
	drawTexturePos(path, tl, tr, br, bl, colors = [])
	{
		this.__instance.DrawTexturePos(path, tl, tr, br, bl, colors);
	}
	drawPolygon(vertices, colors = [], uvs = [], path = "")
	{
		this.__instance.DrawPolygon(vertices, colors, uvs, path);
	}
	drawText(pos, text, color = Color.White, size = 16, font = "")
	{
		this.__instance.DrawText(pos, text, color, size, font);
	}
	redraw() {
		this.__instance.Redraw();
	}
}
