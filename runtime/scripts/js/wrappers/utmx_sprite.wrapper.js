import { UtmxObject } from "./utmx_node_object.weapper.js";

export class UtmxSprite extends UtmxObject {

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
    get offset() {
        return this.__instance.Offset;
    }
    set offset(value) {
        this.__instance.Offset = value;
    }
    
    destroy()
    {
        this.__instance.Destroy();
    }
}
