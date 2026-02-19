import { UtmxTransformableObject } from "./utmx-transformable-object.wrapper.js";

export class UtmxCamera extends UtmxTransformableObject {
    static get zoom() {
        return __instance.Zoom;
    }
    static set zoom(value) {
        __instance.Zoom = value;
    }
}