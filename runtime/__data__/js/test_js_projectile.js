import { UTMX } from "UTMX";

export default class BlueProjectile extends UTMX.BattleProjectile
{
    start()
    {
        this.damage = 10;
    }

    onHit()
    {
        if (UTMX.battle.soul.isMoving())
        {
            UTMX.player.hurt(this.damage);
        }
    }
}