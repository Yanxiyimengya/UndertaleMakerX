import { __tween_manager } from "__UTMX";

export class UtmxTweenManager
{
    static TransitionType = Object.freeze({
        Linear: 0,
        Sine: 1,
        Quint: 2,
        Quart: 3,
        Quad: 4,
        Expo: 5,
        Elastic: 6,
        Cubic: 7,
        Circ: 8,
        Bounce: 9,
        Back: 10,
        Spring: 11
    });

    static EaseType = Object.freeze({
        In: 0,
        Out: 1,
        InOut: 2,
        OutIn: 3
    });

    static createTween()
    {
        return __tween_manager.CreateJavaScriptTween();
    }
}