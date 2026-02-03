import { UTMX } from "UTMX";

export default class MyProjectile extends UTMX.BattleProjectile
{
    constructor() {
        super();
    }

    active()
    {
        UTMX.debug.print(this.rotation);
    }

    update(delta)
    {
        this.rotation += delta * 3.0;
        UTMX.debug.print(this.rotation);
    }
}