import { UtmxGameObject } from "./utmx_game_object.weapper.js";
import { __Vector2 , __Color , __DrawableObject } from "__UTMX";

export class UtmxDrawableObject extends UtmxGameObject {
    static new()
    {
        let ins = new this();
        ins.__instance = __DrawableObject.New(ins);
        return ins;
    }

    get position() {
        return this.__instance.Position;
    }
    set position(value) {
        this.__instance.Position = value;
    }
    get x() {
        return this.__instance.Position.X;
    }
    set x(value) {
        let newPosition = this.__instance.Position;
        newPosition.X = value;
        this.__instance.Position = newPosition;
    }
    get y() {
        return this.__instance.Position.Y;
    }
    set y(value) {
        let newPosition = this.__instance.Position;
        newPosition.Y = value;
        this.__instance.Position = newPosition;
    }
    get z() {
        return this.__instance.ZIndex;
    }
    set z(value) {
        this.__instance.ZIndex = value;
    }
    get rotation() {
        return this.__instance.RotationDegrees;
    }
    set rotation(value) {
        this.__instance.RotationDegrees = value;
    }
    get xscale() {
        return this.__instance.Scale.X;
    }
    set xscale(value) {
        let newScale = this.__instance.Scale;
        newScale.X = value;
        this.__instance.Scale = newScale;
    }
    get yscale() {
        return this.__instance.Scale.Y;
    }
    set yscale(value) {
        let newScale = this.__instance.Scale;
        newScale.Y = value;
        this.__instance.Scale = newScale;
    }
    get scale() {
        return this.__instance.Scale;
    }
    set scale(value) {
        this.__instance.Scale = value;
    }
    get skew() {
        return this.__instance.Skew;
    }
    set skew(value) {
        this.__instance.Skew = value;
    }
    
    get color() {
        return this.__instance.Modulate;
    }
    set color(value) {
        if (value instanceof __Color || typeof value == 'object')
        {
            this.__instance.Modulate = value;
        }
        else if (value instanceof Array)
        {
            if (value.length == 3)
            {
                this.__instance.Modulate = __Color.Color8(value[0], value[1], value[2], 255);
            }
            else if (value.length == 4)
            {
                this.__instance.Modulate = __Color.Color8(value[0], value[1], value[2], value[3]);
            }
        }
        else if (value instanceof String || typeof value == 'string')
        {
            this.__instance.Modulate = __Color.FromString(value, this.__instance.Modulate);
        }
    }

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
    drawTextureRect(path, x, y, width, height, color = new __Color(1,1,1))
    {
        this.__instance.DrawTextureRect(new __Vector2(x, y), new __Vector2(width, height), path, color);
    }
    drawTexturePos(path, tl, tr, br, bl, colors = [])
    {
        this.__instance.DrawTexturePos(path, tl, tr, br, bl, colors);
    }

    redraw()
    {
        this.__instance.Redraw();
    }
}
