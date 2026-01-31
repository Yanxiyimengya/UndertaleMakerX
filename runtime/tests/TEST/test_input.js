import {UTMX} from "UTMX";

export default class TestInput
{
	constructor()
	{
		UTMX.debug.print("intro");
		UTMX.input.addAction("my_custom_action");
		UTMX.input.actionAddKeyButton("my_custom_action", "A".charCodeAt(0));

		UTMX.input.actionAddMouseButton("my_custom_action", 
			UTMX.input.MouseButton.LEFT
		);
	}

	update(delta)
	{
		if (UTMX.input.isActionDown("my_custom_action"))
		{
			UTMX.debug.print("Custom action just pressed!");
		}
	}
}
