import { __scene_manager } from "__UTMX";
import { UtmxGameObject } from "./types/utmx-game-object.weapper.js"

export class UtmxSceneManager
{
    static changeScene(path)
    {
        __scene_manager.Instance.ChangeSceneToFile(path);
    }
    
    static addSingleton(name, object)
    {
        if (object instanceof UtmxGameObject)
            __scene_manager.Instance.AddSingleton(name, object.__instance);
    }
    static getSingleton(name)
    {
        return __scene_manager.Instance.GetSingleton(name);
    }
}