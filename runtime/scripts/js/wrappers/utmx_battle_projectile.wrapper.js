
import { __logger } from "__UTMX";
import { UtmxGameSprite } from "./utmx_game_sprite.wrapper";

export class UtmxBattleProjectile extends UtmxGameSprite { 

    constructor()
    {
        super();
    }
    
    get destroyOnTurnEnd() {
        return this.__instance.DestroyOnTurnEnd;
    }
    set destroyOnTurnEnd(value) {
        this.__instance.DestroyOnTurnEnd = value;
    }
    get collisionMode() {
        return this.__instance.CollisionMode;
    }
    set collisionMode(value) {
        this.__instance.CollisionMode = value;
    }

    get damage() {
        return this.__instance.Damage;
    }
    set damage(value) {
        this.__instance.Damage = value;
    }

    get preciseEpsilon() {
        return this.__instance.PreciseEpsilon;
    }
    set preciseEpsilon(value) {
        this.__instance.PreciseEpsilon = value;
    }

    get enabled() {
        return this.__instance.Enabled;
    }
    set enabled(value) {
        this.__instance.Enabled = value;
    }
}
