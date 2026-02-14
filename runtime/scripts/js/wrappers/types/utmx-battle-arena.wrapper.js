import { UtmxTransformableObject } from "./utmx-transformable-object.wrapper";

class BattleArena extends UtmxTransformableObject
{
    get enabled() {
        return this.__instance.Enabled;
    }
    set enabled(value) {
        this.__instance.Enabled = value;
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
    
    static resize(value)
    {
        this.__instance.Resize(value);
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