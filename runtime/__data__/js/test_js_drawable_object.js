import { UTMX, Color, Vector2 } from "UTMX";

export default class MyDO extends UTMX.DrawableObject
{
	constructor() {
		super();
	}

	active()
	{
	}
	disabled()
	{
	}
    update(delta)
    {
		if (UTMX.input.isActionPressed("ui_accept"))
			this.redraw();
		if (UTMX.input.isActionPressed("up"))
		{
			this.drawTexturePos(
				"s.png",
				new Vector2(0,0),
				new Vector2(100,0),
				UTMX.input.getMousePosition(),
				new Vector2(0,100),
				[new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1), new Color(1, 1, 0)]
			);
		}
		UTMX.debug.print(UTMX.input.getMousePosition().length());
    }
}
