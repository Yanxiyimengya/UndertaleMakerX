import { UTMX } from "UTMX";

export default class MyItem extends UTMX.Item {
	
	constructor() {
		super();
		this.displayName = "我的物品";
	}
	
	onUsed() {
		UTMX.player.heal(30);
		UTMX.audio.playSound("built-in-resources/sounds/sfx/heal.wav");
		return "你使用了我的物品！";
	}
}
