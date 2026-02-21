import { UtmxTransformableObject } from "./utmx-transformable-object.wrapper.js";
import { __GameSprite , __Color } from "__UTMX";

export class UtmxGameSprite extends UtmxTransformableObject {
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
    get frame() {
        return this.__instance.Frame;
    }
    set frame(value) {
        this.__instance.Frame = value;
    }
    get offset() {
        return this.__instance.Offset;
    }
    set offset(value) {
        this.__instance.Offset = value;
    }
    get frameSpeed() {
        return this.__instance.SpeedScale;
    }
    set frameSpeed(value) {
        this.__instance.SpeedScale = value;
    }
    get loop() {
        return this.__instance.Loop;
    }
    set loop(value) {
        this.__instance.Loop = value;
    }
    
	get color() {
		return this.__instance.Modulate;
	}
	set color(value) {
		this.__instance.Modulate = value;
	}
    
	get shader() { return this.__instance.ShaderInstance; }
	set shader(value) { this.__instance.ShaderInstance = value; }
    
    play()
    {
        this.__instance.PlayAnimation();
    }
    stop()
    {
        this.__instance.Stop();
    }
    pause()
    {
        this.__instance.Pause();
    }
    resume()
    {
        this.__instance.Resume();
    }
    isPlaying()
    {
        return this.__instance.IsPlaying();
    }
}
