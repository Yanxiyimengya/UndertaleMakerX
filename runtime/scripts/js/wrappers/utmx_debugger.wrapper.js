import {__logger} from "__UTMX";

export class UtmxDebugger 
{
	static print(...message)
	{
		return __logger.Log(message);
	}
	
	static warning(...message)
	{
		return __logger.Warning(message);
	}
	
	static error(...message)
	{
		return __logger.Error(message);
	}
}
