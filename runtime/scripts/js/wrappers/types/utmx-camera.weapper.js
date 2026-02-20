import { UtmxTransformableObject } from "./utmx-transformable-object.wrapper.js";

export class UtmxCamera extends UtmxTransformableObject {
    get zoom() {
        return this.__instance.Zoom;
    }
    set zoom(value) {
        this.__instance.Zoom = value;
    }

    startShake(duration = 0, shakeAmplitude = null, tempFrequency = null) {
        if (tempFrequency != null) {
            return this.__instance.StartShake(duration, shakeAmplitude, tempFrequency);
        }
        if (shakeAmplitude != null) {
            return this.__instance.StartShake(duration, shakeAmplitude);
        }
        if (arguments.length > 0) {
            return this.__instance.StartShake(duration);
        }
        return this.__instance.StartShake();
    }
}
