import { __scene_manager } from "__UTMX";
import { UtmxGameObject } from "./types/utmx-game-object.wrapper.js"

export class UtmxSceneManager
{
    static changeScene(path)
    {
        __scene_manager.ChangeSceneToFile(path);
    }
    
    static addSingleton(name, object)
    {
        if (object instanceof UtmxGameObject)
            __scene_manager.AddSingleton(name, object.__instance);
    }
    static removeSingleton(name)
    {
        return __scene_manager.RemoveSingleton(name);
    }
    static getSingleton(name)
    {
        return __scene_manager.GetSingleton(name);
    }
}