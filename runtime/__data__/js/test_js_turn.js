import { UTMX,Vector2, Color } from "UTMX";
import MyProjectile from "./test_js_projectile";
import MySpr from "./test_js_sprite";


export default class MyBattleTurn extends UTMX.BattleTurn {
	
	static a = false;

	onTurnInitialize()
	{
		this.turnTime = 20.0; 
		this.time = 0;
		let t = Color.Red
		t.r = 0.5;
		UTMX.debug.log("1:", Color.Blue);
		UTMX.debug.log("2:", t.add(Color.Blue));
	}
	
	onTurnStart()
	{
		this.typing_chicken = UTMX.scene.createTextTyper(
			"[color=red][font='built-in-resources/fonts/Text.ttf']Hello, [hello sb=10]World[play_sound=SeaTea.wav][end]");
		this.typing_chicken.position = new Vector2(320, 100);
		this.typing_chicken.z = 2000;

		this.proj = MyProjectile.new();
		this.proj.textures = "a.png";
		this.proj.damage = 2;
		this.proj.position = new Vector2(320, 320);
		this.proj.collisionMode = UTMX.battle.ProjectileCollisionMode.PRECISE;
		this.proj.useMask = true;

		UTMX.battle.arena.resize(new Vector2(600, 600), 0.8);
		this.circleArena = UTMX.battle.arena.createRectangleCulling(new Vector2(320, 100), new Vector2(100, 100));
	}

	onTurnEnd()
	{
		this.circleArena.destroy();
	}

	onTurnUpdate(delta) {

		if (UTMX.input.isActionDown("ui_accept"))
		{
			//UTMX.audio.playSound("built-in-resources/sounds/sfx/snd_break1.wav");
			this.proj.x += 10;
			this.proj.rotation += 50;
			this.proj.useMask = !this.proj.useMask;
		}
		if (UTMX.input.isActionDown("menu"))
		{
			this.proj.enabled = !this.proj.enabled;
		}
		this.time += 1;
		UTMX.battle.player.sprite.rotation += 1;

		this.circleArena.position = UTMX.input.getMousePosition();
		this.circleArena.rotation += delta * 45;
	}
}
