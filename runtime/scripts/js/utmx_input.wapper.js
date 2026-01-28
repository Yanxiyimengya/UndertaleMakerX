import {__input} from "__UTMX";

export class UtmxInput 
{
	static getAction(message)
	{
		return __input.IsActionJustPressed(message);
	}
}
