import { UTMX, Vector2 } from "UTMX";
import MyProjectile from "./test_js_projectile";
import MySpr from "./test_js_sprite";


export default class MyBattleTurn extends UTMX.BattleTurn {
	
	static a = false;
	
	constructor() {
		super();
		this.turnTime = 3.0; 
		this.time = 0;
	}

    
    onTurnStart()
	{
		this.typing_chicken = UTMX.scene.createTextTyper(
			"[instant=false][color=red][font='built-in-resources/fonts/Text.ttf']Hello, [hello sb=10]World[play_sound=SeaTea.wav][end]");
		this.typing_chicken.position = new Vector2(320, 100);
		this.typing_chicken.z = 2000;
		
		UTMX.debug.print(MyBattleTurn.a);
		if (!MyBattleTurn.a) {
			UTMX.debug.print(123123123123);
			this.typing_chicken.processCmd = (cmd, args) => {
				if (cmd == "hello")
				{
					if (args.sb == 10)
					{
						UTMX.audio.playSound("built-in-resources/sounds/sfx/escaped.wav");
					}
					else
					{
						
						UTMX.audio.playSound("built-in-resources/sounds/sfx/heal.wav");
					}
					return true;
				}
				return false;
			};
			MyBattleTurn.a = true;
		}

		this.spr = UTMX.scene.createSprite(MySpr, "a.png");
        UTMX.debug.print(this.spr);
		this.spr.z = 1000;
		this.spr.position = new Vector2(320, 180);
		this.spr.color = "00ff00aa";

		this.proj = UTMX.battle.createProjectile(MyProjectile, "a.png", 10, true);
		this.proj.position = new Vector2(320, 320);
		this.proj.collisionMode = UTMX.battle.ProjectileCollisionMode.PRECISE;
	}

    onTurnUpdate(delta) {
		if (UTMX.input.isActionDown("ui_accept"))
		{
			//UTMX.audio.playSound("built-in-resources/sounds/sfx/snd_break1.wav");
			this.proj.x += 10;
			this.proj.rotation += 50;
			this.spr.xscale += 0.1;
			this.spr.destroy();
		}
		if (UTMX.input.isActionDown("menu"))
		{
			this.proj.enabled = !this.proj.enabled;
		}
		this.time += 1;
		UTMX.battle.player.sprite.rotation += 1;
		//this.typing_chicken.position = UTMX.input.getMousePosition();

	}
}
