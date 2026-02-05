import { UtmxObject } from "./utmx_node_object.weapper.js";
import { __Color } from "__UTMX";

export class UtmxGameSprite extends UtmxObject {

    constructor()
    {
        super();
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
    get position() {
        return this.__instance.Position;
    }
    set position(value) {
        this.__instance.Position = value;
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

    get textures() {
        return this.__instance.TexturesPath;
    }
    set textures(value) {
        if (typeof value === "string")
        {
            this.__instance.TexturesPath = [value];
        }
        else
        {
            this.__instance.TexturesPath = value;
        }
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
    get offset() {
        return this.__instance.Offset;
    }
    set offset(value) {
        this.__instance.Offset = value;
    }
    
    play()
    {
        this.__instance.Play();
    }

    setLoop(loop)
    {
        this.__instance.SetLoop(loop);
    }

    destroy()
    {
        this.__instance.Destroy();
    }
}
