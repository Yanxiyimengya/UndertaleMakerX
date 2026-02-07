import { UtmxGameObject } from "./utmx_game_object.weapper.js";

export class UtmxTextTyper extends UtmxGameObject {
    constructor()
    {
        super();
    }

    get x() {
        return this.__instance.Position.X;
    }
    set x(value) {
        let newPosition = this.__instance.Position;
        newPosition.X = value;
        this.__instance.Position = newPosition;
    }
    get y() {
        return this.__instance.Position.Y;
    }
    set y(value) {
        let newPosition = this.__instance.Position;
        newPosition.Y = value;
        this.__instance.Position = newPosition;
    }
    get z() {
        return this.__instance.ZIndex;
    }
    set z(value) {
        this.__instance.ZIndex = value;
    }
    get rotation() {
        return this.__instance.RotationDegrees;
    }
    set rotation(value) {
        this.__instance.RotationDegrees = value;
    }
    get position() {
        return this.__instance.Position;
    }
    set position(value) {
        this.__instance.Position = value;
    }
    get xscale() {
        return this.__instance.Scale.X;
    }
    set xscale(value) {
        let newScale = this.__instance.Scale;
        newScale.X = value;
        this.__instance.Scale = newScale;
    }
    get yscale() {
        return this.__instance.Scale.Y;
    }
    set yscale(value) {
        let newScale = this.__instance.Scale;
        newScale.Y = value;
        this.__instance.Scale = newScale;
    }
    get scale() {
        return this.__instance.Scale;
    }
    set scale(value) {
        this.__instance.Scale = value;
    }
    get xsize() {
        return this.__instance.Size.X;
    }
    set xsize(value) {
        let newSize = this.__instance.Size;
        newSize.X = value;
        this.__instance.Size = newSize;
    }
    get ysize() {
        return this.__instance.Size.Y;
    }
    set ysize(value) {
        let newSize = this.__instance.Size;
        newSize.Y = value;
        this.__instance.Size = newSize;
    }
    get size() {
        return this.__instance.Size;
    }
    set size(value) {
        this.__instance.Size = value;
    }

    
    get instant() {
        return this.__instance.Instant;
    }
    set instant(value) {
        this.__instance.Instant = value;
    }
    get noskip() {
        return this.__instance.NoSkip;
    }
    set noskip(value) {
        this.__instance.NoSkip = value;
    }

    start(text)
    {
        this.__instance.Start(text);
    }
    
    isFinished(text)
    {
        this.__instance.IsFinished(text);
    }
}