import { __BattleProjectile, __logger } from "__UTMX";
import { UtmxGameSprite } from "./utmx-game-sprite.wrapper";

export class UtmxBattleProjectile extends UtmxGameSprite { 
	static ProjectileCollisionMode = Object.freeze({
		FULL_TEXTURE : 0,
		USED_RECT : 1,
		PRECISE : 2
	});

    static new()
    {
        let ins = new this();
        ins.__instance = __BattleProjectile.New(ins);
        return ins;
    }

    get damage() {
        return this.__instance.Damage;
    }
    set damage(value) {
        this.__instance.Damage = value;
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

    get preciseEpsilon() {
        return this.__instance.PreciseEpsilon;
    }
    set preciseEpsilon(value) {
        this.__instance.PreciseEpsilon = value;
    }
    get useMask() {
        return this.__instance.UseMask;
    }
    set useMask(value) {
        this.__instance.UseMask = value;
    }

    get enabled() {
        return this.__instance.Enabled;
    }
    set enabled(value) {
        this.__instance.Enabled = value;
    }
}
