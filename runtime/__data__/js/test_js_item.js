import { UTMX } from "UTMX";

export default class MyItem extends UTMX.BaseItem {
	
	constructor() {
		super();
		this.name = "我的物品";
	}
	
	onUsed() {
		UTMX.debug.print("test" + this.slot);
		UTMX.audio.playBgm("test" + this.slot, "see.mp3");
		this.removeSelf();
	}
}
