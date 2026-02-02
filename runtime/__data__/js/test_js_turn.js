import { UTMX } from "UTMX";


export default class MyBattleTurn extends UTMX.BattleTurn {
	
	constructor() {
		super();
		this.turnTime = 30.0;
		this.time = 0;
	}

    
    onTurnInitialize() {
	}

    onTurnUpdate(delta) {
		
		if (UTMX.input.isActionDown("ui_accept"))
		{
			UTMX.audio.playSound("built-in-resources/sounds/sfx/snd_break1.wav");
		}
		this.time += 1;
	}
}
