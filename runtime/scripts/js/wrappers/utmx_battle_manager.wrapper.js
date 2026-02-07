import { __battle_manager, __BattleProjectile, __logger } from "__UTMX";
import { UtmxGameSprite } from "./utmx_game_sprite.wrapper.js";
import { UtmxGameObject } from "./utmx_game_object.weapper.js";
import {
    BattleArenaRectangle,
    BattleArenaCircle,
    BattleArenaPolygon,
} from "./utmx_battle_arena.wrapper.js";

class BattleCamera extends UtmxGameObject
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
class BattlePlayer extends UtmxGameObject
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
    
    static __sprite = new UtmxGameSprite();
    static get sprite() {
        if (! __battle_manager.IsInBattle()) return null;
        this.__sprite.instance = __battle_manager.GetBattlePlayerController().PlayerSoul.Sprite;
        return this.__sprite;
    }
    static set sprite(value) { } // 只读
}
class BattleArenaAccess
{
    static get x() {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        return mainArena.Position.X;
    }
    static set x(value) {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        let newPosition = mainArena.Position;
        newPosition.X = value;
        mainArena.Position = newPosition;
    }
    static get y() {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        return mainArena.Position.Y;
    }
    static set y(value) {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        let newPosition = mainArena.Position;
        newPosition.Y = value;
        mainArena.Position = newPosition;
    }
    static get rotation() {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        if (mainArena != null) {
            return mainArena.RotationDegrees;
        }
    }
    static set rotation(value) {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        mainArena.RotationDegrees = value;
    }
    static get size() {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        if (mainArena != null) {
            return mainArena.Size;
        }
    }
    static set size(value) {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        mainArena.Size = value;
    }

    static resize(value)
    {
        let mainArena = __battle_manager.GetBattleArenaController().MainArena;
        mainArena.Resize(value);
    }

    static createRectangleExpand(pos = new Vector2(320, 320), size = new Vector2(130, 130))
    {
        let arenaWrapper = new BattleArenaRectangle();
        arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateRectangleArenaExpand();
        return arenaWrapper;
    }
    static createRectangleCulling(pos = new Vector2(320, 320), size = new Vector2(130, 130))
    {
        let arenaWrapper = new BattleArenaRectangle();
        arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateRectangleArenaCulling();
        return arenaWrapper;
    }
    static createCircleExpand(pos = new Vector2(320, 320), radius = 120)
    {
        let arenaWrapper = new BattleArenaCircle();
        arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateCircleArenaExpand();
        arenaWrapper.position = pos;
        arenaWrapper.radius = radius;
        return arenaWrapper;
    }
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
    static arena = BattleArenaAccess;

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