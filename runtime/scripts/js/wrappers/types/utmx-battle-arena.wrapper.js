import { UtmxGameObject } from "./utmx-game-object.wrapper.js";

class BattleArena extends UtmxGameObject
{
    get position() {
        return this.__instance.Position;
    }
    set position(value) {
        this.__instance.Position = value;
    }
    get x() {
        return this.__instance.X;
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
    get rotation() {
        if (this.__instance != null) {
            return this.__instance.RotationDegrees;
        }
    }
    set rotation(value) {
        this.__instance.RotationDegrees = value;
    }

    get borderWidth() {
        return this.__instance.BorderWidth;
    }
    set borderWidth(value) {
        this.__instance.BorderWidth = value;
    }

    destroy()
    {
        this.__instance.QueueFree();
    }
}

class BattleArenaRectangle extends BattleArena
{
    get size() {
        return this.__instance.Size;
    }
    set size(value) {
        this.__instance.Size = value;
    }
    
    static resize(value)
    {
        this.__instance.Resize(value);
    }
}

class BattleArenaCircle extends BattleArena
{
    get radius() {
        return this.__instance.Radius;
    }
    set radius(value) {
        this.__instance.Radius = value;
    }
}

class BattleArenaPolygon extends BattleArena
{
    get vertices() {
        return this.__instance.Vertices;
    }
    set vertices(value) {
        this.__instance.Vertices = value;
    }
}


export {
    BattleArenaRectangle,
    BattleArenaCircle,
    BattleArenaPolygon
}