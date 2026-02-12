import { UTMX } from "UTMX";

export default class MySpr extends UTMX.Sprite
{
	constructor() {
		super();
		this.timer = 0.0;
	}

	start()
	{
		UTMX.debug.print(this.start);
	}
	
	onDestroy()
	{
		UTMX.debug.print(this.onDestroy);
	}

	update(delta)
	{
		UTMX.debug.print(delta);
	}
}
