import { UTMX } from "UTMX";

export default class MySpr extends UTMX.Sprite
{
	constructor() {
		super();
		this.timer = 0.0;
	}

	active()
	{
		UTMX.debug.print(this.rotation);
	}
	disabled()
	{
		UTMX.debug.print(this.rotation + 114514);
	}

	update(delta)
	{
		this.rotation = Math.sin(this.timer * 3.0) * 45;
		this.timer += delta;
	}
}
