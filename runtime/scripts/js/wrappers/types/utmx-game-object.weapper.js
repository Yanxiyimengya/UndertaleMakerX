export class UtmxGameObject {
    constructor() {}

    set instance(value) {
        this.__instance = value;
    }
    get instance() {
        return this.__instance;
    }

    destroy()
    {
        this.__instance.Destroy();
    }
}