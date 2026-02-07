import { UTMX } from "UTMX";


export default class MyProjectile extends UTMX.BattleProjectile
{
	active()
	{
		this.timer = 0.0;
		this.damage = 1;
	}

	disabled()
	{
	}

	update(delta)
	{
		this.rotation = Math.sin(this.timer * 3.0) * 45;
		this.timer += delta;
	}
}
