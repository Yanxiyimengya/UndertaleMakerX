import { UTMX } from "UTMX";


let niubi = 19;

export default class MyProjectile extends UTMX.BattleProjectile
{
	constructor() {
		super();
	}

	active()
	{
		this.timer = 0.0;
		this.damage = 1;
		niubi += 1;
		UTMX.debug.print(niubi);
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
