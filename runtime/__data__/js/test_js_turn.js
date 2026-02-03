import { UTMX, Vector2 } from "UTMX";



export default class MyBattleTurn extends UTMX.BattleTurn {
	
	constructor() {
		super();
		this.turnTime = 3.0;
		this.time = 0;
	}

    
    onTurnInitialize()
	{
		this.proj = UTMX.BattleProjectile.new("a.png", 10, true);
		this.proj.position = new Vector2(320, 320);
		this.proj.collisionMode = UTMX.BattleProjectile.CollisionMode.PRECISE;
	}

    onTurnUpdate(delta) {
		
		if (UTMX.input.isActionDown("ui_accept"))
		{
			UTMX.audio.playSound("built-in-resources/sounds/sfx/snd_break1.wav");
			this.proj.x += 10;
			this.proj.rotation += 50;
		}
		if (UTMX.input.isActionDown("menu"))
		{
			this.proj.destroy();
		}
		this.time += 1;


	}
}
