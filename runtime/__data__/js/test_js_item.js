import { UTMX } from "UTMX";

export default class MyItem extends UTMX.Item {
	
	constructor() {
		super();
		this.displayName = "我的物品";
	}
	
	onUse() {
		UTMX.player.heal(30);
		this.removeSelf();
		UTMX.audio.playSound("built-in-resources/sounds/sfx/heal.wav");
		this.usedText2 = "你使用了我的物品！";
	}
}
