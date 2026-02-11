import { __TextTyper } from "__UTMX";
import { UtmxTransformableObject } from "./utmx-transformable-object.weapper.js";

export class UtmxTextTyper extends UtmxTransformableObject {
    
    static new(text)
    {
        let typerWrapper = new UtmxTextTyper();
        if (typerWrapper != null) 
        {
            let typer = __TextTyper.New(typerWrapper);
            typerWrapper.__instance = typer;
            typerWrapper.start(text);
            return typerWrapper;
        }
        return null;
    }
    
    get text() {
        return this.__instance.TyperText;
    }
    set text(value) {
        this.__instance.TyperText = value;
    }
    get instant() {
        return this.__instance.Instant;
    }
    set instant(value) {
        this.__instance.Instant = value;
    }
    get noskip() {
        return this.__instance.NoSkip;
    }
    set noskip(value) {
        this.__instance.NoSkip = value;
    }
	get shader() { return this.__instance.ShaderInstance; }
	set shader(value) { this.__instance.ShaderInstance = value; }
    //processCmd = null;

    start(text)
    {
        this.__instance.Start(text);
    }
    
    isFinished()
    {
        return this.__instance.IsFinished();
    }

    getProgress()
    {
        return this.__instance.GetProgress();
    }
}