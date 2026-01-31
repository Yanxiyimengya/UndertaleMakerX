import { UTMX } from "UTMX";

export default class MyBattleTurn extends UTMX.BattleTurn {
	
	constructor() {
		super();
	}

    
    onTurnUpdate(delta) {UTMX.debug.print("MyBattleTurn onTurnUpdate", delta); }
}
