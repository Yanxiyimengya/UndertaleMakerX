import { __battle_manager } from "__UTMX";

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