import { UTMX, Vector2 } from "UTMX";
import MyProjectile from "./test_js_projectile";


export default class MyBattleTurn extends UTMX.BattleTurn {
	
	constructor() {
		super();
		this.turnTime = 3.0;
		this.time = 0;
	}

    
    onTurnInitialize()
	{
		this.proj = UTMX.battle.createProjectile(MyProjectile, "a.png", 10, true);
		UTMX.debug.print(this.proj);
		this.proj.position = new Vector2(320, 320);
		this.proj.collisionMode = UTMX.battle.ProjectileCollisionMode.PRECISE;
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
			this.proj.enabled = !this.proj.enabled;
		}
		this.time += 1;

	}
}
