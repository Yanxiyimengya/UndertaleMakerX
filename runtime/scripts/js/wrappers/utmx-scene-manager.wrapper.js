import { __scene_manager } from "__UTMX";
import { UtmxGameObject } from "./types/utmx-game-object.wrapper.js"
import { UtmxCamera } from "./types/utmx-camera.weapper.js"

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

    static #__camera = new UtmxCamera();
    static getCamera()
    {
        let _ins = __scene_manager.GetCamera();
        if (_ins == null) return null;
        this.#__camera.__instance = _ins;
        return this.#__camera;
    }
}