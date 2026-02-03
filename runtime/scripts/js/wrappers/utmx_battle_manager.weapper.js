import { __battle_manager } from "__UTMX";

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

    static player = BattlePlayer;

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