import {__logger} from "__UTMX";

export class UtmxDebugger 
{
	static print(...message)
	{
		return __logger.Log(message);
	}
}
