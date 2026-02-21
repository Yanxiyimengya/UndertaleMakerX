export class UtmxGameObject {
    __instance = null;
    #destroyed = false;

    constructor() {
    }

    static new()
    {
        return null;
    }

    destroy()
    {
        if (this.#destroyed) return;
        this.__instance.Destroy();
        this.#destroyed = true;
    }
}