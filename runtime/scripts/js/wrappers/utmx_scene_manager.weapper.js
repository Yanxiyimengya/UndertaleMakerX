import { __scene_manager , __GameSprite , __TextTyper , __logger } from "__UTMX";
import { UtmxGameSprite } from "./utmx_game_sprite.wrapper.js";
import { UtmxTextTyper } from "./utmx_text_typer.wrapper.js";

export class UtmxSceneManager
{
    static changeScene(path)
    {
        __scene_manager.Instance.ChangeSceneToFile(path);
    }

    static createSprite(spriteConstructor = UtmxGameSprite, textures = "") {
        if (typeof spriteConstructor !== "function") return null;
        if (spriteConstructor === UtmxGameSprite || 
            UtmxGameSprite.prototype.isPrototypeOf(spriteConstructor.prototype))
        {
            try
            {
                let spriteWrapper = new spriteConstructor();
                if (spriteWrapper != null) 
                {
                    let sprite = __GameSprite.New(spriteWrapper);
                    spriteWrapper.instance = sprite;
                    spriteWrapper.textures = textures;
                    return spriteWrapper;
                }
            }
            catch (e)
            {
                let message = (e && e.message) ? e.message : JSON.stringify(e);
                __logger.Error(message);
            }
        }
        return null;
    }
    
    static createTextTyper(text) {
        try
        {
            let typerWrapper = new UtmxTextTyper();
            if (typerWrapper != null) 
            {
                let typer = __TextTyper.New(typerWrapper);
                typerWrapper.instance = typer;
                typerWrapper.start(text);
                return typerWrapper;
            }
        }
        catch (e)
        {
            let message = (e && e.message) ? e.message : JSON.stringify(e);
            __logger.Error(message);
        }
        return null;
    }
}