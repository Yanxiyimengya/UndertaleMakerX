import { UtmxDrawableObject } from "./utmx_drawable_object.wrapper";
import { __Color , __GameSprite } from "__UTMX";

export class UtmxGameSprite extends UtmxDrawableObject {
    static new()
    {
        let ins = new this();
        ins.__instance = __GameSprite.New(ins);
        return ins;
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
    get speed() {
        return this.__instance.SpeedScale;
    }
    set speed(value) {
        this.__instance.SpeedScale = value;
    }
    
    play()
    {
        this.__instance.Play();
    }

    setLoop(loop)
    {
        this.__instance.SetLoop(loop);
    }
}
