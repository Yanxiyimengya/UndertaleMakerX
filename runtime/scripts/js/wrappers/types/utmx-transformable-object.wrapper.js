import { UtmxGameObject } from "./utmx-game-object.wrapper.js";

export class UtmxTransformableObject extends UtmxGameObject
{
	get visible() {
		return this.__instance.GetVisible();
	}
	set visible(value) {
		this.__instance.SetVisible(value);
	}

	get position() {
		return this.__instance.GetPosition();
	}
	set position(value) {
		this.__instance.SetPosition(value);
	}
	get globalPosition() {
		return this.__instance.GetGlobalPosition();
	}
	set globalPosition(value) {
		this.__instance.SetGlobalPosition(value);
	}
	get z() {
		return this.__instance.GetZIndex();
	}
	set z(value) {
		this.__instance.SetZIndex(value);
	}
	get rotation() {
		return this.__instance.GetRotationDegrees();
	}
	set rotation(value) {
		this.__instance.SetRotationDegrees(value);
	}
	get globalRotation() {
		return this.__instance.GetGlobalRotationDegrees();
	}
	set globalRotation(value) {
		this.__instance.SetGlobalRotationDegrees(value);
	}
	get scale() {
		return this.__instance.GetScale();
	}
	set scale(value) {
		this.__instance.SetScale(value);
	}
	get globalScale() {
		return this.__instance.GetGlobalScale();
	}
	set globalScale(value) {
		this.__instance.SetGlobalScale(value);
	}
	get skew() {
		return this.__instance.GetSkew();
	}
	set skew(value) {
		this.__instance.SetSkew(value);
	}
	get globalSkew() {
		return this.__instance.GetGlobalSkew();
	}
	set globalSkew(value) {
		this.__instance.SetGlobalSkew(value);
	}

	#__prevParent = null;
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
			child.#__prevParent = child.__instance.GetParent();
			if (child.#__prevParent == null)
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