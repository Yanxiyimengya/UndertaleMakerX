import { UTMX } from "UTMX";

export default class MyItem extends UTMX.Item {
	
	constructor() {
		super();
		this.displayName = "我的物品";
	}
	
	onUsed() {
		return "你使用了我的物品！";
	}
}
