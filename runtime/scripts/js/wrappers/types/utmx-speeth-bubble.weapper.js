import { __SpeechBubble, __scene_manager } from "__UTMX";
import { UtmxTextTyper } from "./utmx-text-typer.wrapper.js";

export class UtmxSpeechBubble extends UtmxTextTyper
{
    static Direction = Object.freeze({
        Top: 0,
        Bottom: 1,
        Left: 2,
        Right: 3,
    });

    __bubble = null;
    static new(text = "")
    {
        let typerWrapper = new this();
        if (typerWrapper == null) return null;

        let bubble = __SpeechBubble.New(typerWrapper);
        if (bubble == null) return null;
        if (bubble.SpeechBubbleTextTyper == null) {
            __scene_manager.DeleteSpeechBubble(bubble);
            return null;
        }

        typerWrapper.__bubble = bubble;
        typerWrapper.__instance = bubble.SpeechBubbleTextTyper;
        typerWrapper.text = text;
        return typerWrapper;
    }

    get text() {
        return this.__bubble.Text;
    }
    set text(value) {
        this.__bubble.Text = value;
    }

    get size() {
        return this.__bubble.Size;
    }
    set size(value) {
        this.__bubble.Size = value;
    }

    get dir() {
        return this.__bubble.Dir;
    }
    set dir(value) {
        this.__bubble.Dir = value;
    }

    get spikeOffset() {
        return this.__bubble.SpikeOffset;
    }
    set spikeOffset(value) {
        this.__bubble.SpikeOffset = value;
    }

    get hideSpike() {
        return this.__bubble.HideSpike;
    }
    set hideSpike(value) {
        this.__bubble.HideSpike = value;
    }

    get inSpike() {
        return this.__bubble.InSpike;
    }
    set inSpike(value) {
        this.__bubble.InSpike = value;
    }

    _getTransformTarget() {
        return this.__bubble ?? this.__instance;
    }

    get visible() {
        return this._getTransformTarget().GetVisible();
    }
    set visible(value) {
        this._getTransformTarget().SetVisible(value);
    }

    get position() {
        return this._getTransformTarget().GetPosition();
    }
    set position(value) {
        this._getTransformTarget().SetPosition(value);
    }

    get globalPosition() {
        return this._getTransformTarget().GetGlobalPosition();
    }
    set globalPosition(value) {
        this._getTransformTarget().SetGlobalPosition(value);
    }

    get z() {
        return this._getTransformTarget().GetZIndex();
    }
    set z(value) {
        this._getTransformTarget().SetZIndex(value);
    }

    get rotation() {
        return this._getTransformTarget().GetRotationDegrees();
    }
    set rotation(value) {
        this._getTransformTarget().SetRotationDegrees(value);
    }

    get globalRotation() {
        return this._getTransformTarget().GetGlobalRotationDegrees();
    }
    set globalRotation(value) {
        this._getTransformTarget().SetGlobalRotationDegrees(value);
    }

    get scale() {
        return this._getTransformTarget().GetScale();
    }
    set scale(value) {
        this._getTransformTarget().SetScale(value);
    }

    get globalScale() {
        return this._getTransformTarget().GetGlobalScale();
    }
    set globalScale(value) {
        this._getTransformTarget().SetGlobalScale(value);
    }

    get skew() {
        return this._getTransformTarget().GetSkew();
    }
    set skew(value) {
        this._getTransformTarget().SetSkew(value);
    }

    get globalSkew() {
        return this._getTransformTarget().GetGlobalSkew();
    }
    set globalSkew(value) {
        this._getTransformTarget().SetGlobalSkew(value);
    }

    destroy() {
        if (this.__instance != null) {
            this.__instance.Destroy();
        } else if (this.__bubble != null) {
            __scene_manager.DeleteSpeechBubble(this.__bubble);
        }
        this.__bubble = null;
        this.__instance = null;
    }

}
