
import { __Node, __battle_manager } from "__UTMX";

export class UtmxBattleProjectile extends __Node {

    static CollisionMode = Object.freeze({
        FULL_TEXTURE : 0,
        USED_RECT : 1,
        PRECISE : 2
    });

    constructor(textures = "", damage = 1, mask = false)
    {
        this.instance = __battle_manager.GetBattleProjectileController().CreateProjectile(mask);
        this.instance.SetTextures(textures);
        this.instance.Damage = damage;
    }

    get destroyOnTurnEnd() {
        return this.instance.DestroyOnTurnEnd;
    }
    set destroyOnTurnEnd(value) {
        this.instance.DestroyOnTurnEnd = value;
    }
    get collisionMode() {
        return this.instance.CollisionMode;
    }
    set collisionMode(value) {
        this.instance.CollisionMode = value;
    }

    get x() {
        return this.instance.Position.X;
    }
    set x(value) {
        let newPosition = this.instance.Position;
        newPosition.X = value;
        this.instance.Position = newPosition;
    }
    get y() {
        return this.instance.Position.Y;
    }
    set y(value) {
        let newPosition = this.instance.Position;
        newPosition.Y = value;
        this.instance.Position = newPosition;
    }

    get rotation() {
        return this.instance.RotationDegress;
    }
    set rotation(value) {
        this.instance.RotationDegress = value;
    }

    get position() {
        return this.instance.Position;
    }
    set position(value) {
        this.instance.Position = value;
    }

    destroy()
    {
        this.instance.Destroy();
    }

}
