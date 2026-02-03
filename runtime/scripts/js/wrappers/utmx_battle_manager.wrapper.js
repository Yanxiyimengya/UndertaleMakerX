import { __battle_manager, __BattleProjectile, __logger } from "__UTMX";
import { UtmxBattleProjectile } from "./utmx_battle_projectile.wrapper";

class BattlePlayerSprite
{
    static get textures() {
        if (! __battle_manager.IsInBattle()) return false;
        return __battle_manager.GetBattlePlayerController().PlayerSoul.Sprite;
    }
    static set textures(value) {
        if (! __battle_manager.IsInBattle()) return;
        __battle_manager.GetBattlePlayerController().PlayerSoul.Sprite.SetTextures(value);
    }
}

class BattleCamera
{
    static get x() {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        return battleCamera.Position.X;
    }
    static set x(value) {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        let newPosition = battleCamera.Position;
        newPosition.X = value;
        battleCamera.Position = newPosition;
    }
    static get y() {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        return battleCamera.Position.Y;
    }
    static set y(value) {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        let newPosition = battleCamera.Position;
        newPosition.Y = value;
        battleCamera.Position = newPosition;
    }

    static get zoom() {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        return battleCamera.Zoom;
    }
    static set zoom(value) {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        battleCamera.Zoom = value;
    }

    static get rotation() {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        if (battleCamera != null) {
            return battleCamera.RotationDegrees;
        }
    }
    static set rotation(value) {
        let battleCamera = __battle_manager.GetBattleController().Camera;
        battleCamera.RotationDegrees = value;
    }
}

class BattlePlayer
{
    static get enabledCollision() {
        if (! __battle_manager.IsInBattle()) return false;
        return __battle_manager.GetBattlePlayerController().PlayerSoul.EnabledCollision;
    }
    static set enabledCollision(value) {
        if (! __battle_manager.IsInBattle()) return;
        __battle_manager.GetBattlePlayerController().PlayerSoul.EnabledCollision = value;
    }
    
    static get movable() {
        if (! __battle_manager.IsInBattle()) return false;
        return __battle_manager.GetBattlePlayerController().PlayerSoul.Movable;
    }
    static set movable(value) {
        if (! __battle_manager.IsInBattle()) return;
        __battle_manager.GetBattlePlayerController().PlayerSoul.Movable = value;
    }

    static get sprite() {
        if (! __battle_manager.IsInBattle()) return null;
        return BattlePlayerSprite;
    }
    static set sprite(value) { } // 只读
}

export class UtmxBattleManager {
    InitializeBattle = null;
    // 不公开的函数引用
    
    static ProjectileCollisionMode = Object.freeze({
        FULL_TEXTURE : 0,
        USED_RECT : 1,
        PRECISE : 2
    });

    static player = BattlePlayer;
    static camera = BattleCamera;

    static createProjectile(projectileConstructor = UtmxBattleProjectile, textures = "", 
        damage = 1, mask = false) {
        try
        {
            let projectileWrapper = new projectileConstructor();
            if (projectileWrapper != null) 
            {
                let projectile = __BattleProjectile.New(projectileWrapper, mask);
                projectileWrapper.instance = projectile;
                projectileWrapper.textures = textures;
                projectileWrapper.damage = damage;
                return projectileWrapper;
            }
        }
        catch (e)
        {
            let message = (e && e.message) ? e.message : JSON.stringify(e);
            __logger.Error(message);
            return null;
        }
        return null;
    }

    static startEncounter(encounterId)
    {
        __battle_manager.EncounterBattleStart(encounterId);
    }

    static endEncounter()
    {
        __battle_manager.EncounterBattleEnd();
    }

    static gameOver()
    {
        __battle_manager.GameOver();
    }

    static isInBattle() {
        return __battle_manager.IsInBattle();
    }
}