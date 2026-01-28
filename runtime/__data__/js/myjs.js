import {UTMX} from "UTMX";


export default class MyClass
{
	constructor()
	{
		UTMX.debug.print("Hello, UndertaleMakerX");
	}

	update(delta)
	{
		if (UTMX.input.getAction("ui_accept"))
		{
			UTMX.audio.playSound("built-in-resources/sounds/sfx/snd_laz.wav");
		}
	}
}
