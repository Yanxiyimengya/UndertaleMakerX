import {Core} from 'UTMX';

export default class MyClass {
	invoke_count = 0;
	
	constructor()
	{
		Core.Print("JS构造函数被触发");
	}
	
	_enter_tree()
	{
		Core.Print("JS _EnterTree函数被触发");
	}
}
