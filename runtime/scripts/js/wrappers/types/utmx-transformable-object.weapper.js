import { UtmxGameObject } from "./utmx-game-object.weapper.js";

export class UtmxTransformableObject extends UtmxGameObject
{
	get position() {
		return this.__instance.Position;
	}
	set position(value) {
		this.__instance.Position = value;
	}
	get globalPosition() {
		return this.__instance.GlobalPosition;
	}
	set globalPosition(value) {
		this.__instance.GlobalPosition = value;
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
	get globalRotation() {
		return this.__instance.GlobalRotationDegrees;
	}
	set globalRotation(value) {
		this.__instance.GlobalRotationDegrees = value;
	}
	get scale() {
		return this.__instance.Scale;
	}
	set scale(value) {
		this.__instance.Scale = value;
	}
	get globalScale() {
		return this.__instance.GlobalScale;
	}
	set globalScale(value) {
		this.__instance.GlobalScale = value;
	}
	get skew() {
		return this.__instance.Skew;
	}
	set skew(value) {
		this.__instance.Skew = value;
	}
	get globalSkew() {
		return this.__instance.globalSkew;
	}
	set globalSkew(value) {
		this.__instance.globalSkew = value;
	}

	__prevParent = null;
	getParent()
	{
		let parent = this.__instance.GetParent();
		if (parent instanceof UtmxTransformableObject)
			return parent;
		return null;
	}

	addChild(child, resetTransform = false)
	{
		if (child instanceof UtmxTransformableObject)
		{
			child.__prevParent = child.__instance.GetParent();
			if (child.__prevParent == null)
				this.__instance.AddChild(child.__instance);
			else 
				child.__instance.Reparent(this.__instance, resetTransform);
		}
	}

	removeChild(child,)
	{
		this.__instance.RemoveChild(child.__instance);
		if (child.__prevParent != null)
		{
			child.__prevParent.AddChild(child.__instance);
		}
	}
}