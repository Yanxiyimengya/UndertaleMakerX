export class UtmxGameObject {
    __instance = null;

    constructor() {
    }

    static new()
    {
        return null;
    }

    destroy()
    {
        this.__instance.Destroy();
    }
}