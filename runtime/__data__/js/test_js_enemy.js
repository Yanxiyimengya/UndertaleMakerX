import { UTMX } from "UTMX";

export default class MyEnemy extends UTMX.Enemy {
	
	constructor() {
		super();
		this.name = "我的敌人";
	}
	
	handleAction(action) {
		UTMX.debug.print("active:name -> " + this.name);
		UTMX.debug.print("active:DisplayName -> " + this.DisplayName);
	}
}
