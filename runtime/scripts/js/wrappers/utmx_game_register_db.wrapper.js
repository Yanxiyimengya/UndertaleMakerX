import { __game_register_db } from "__UTMX";

export class UtmxGameRegisterDB {

    TryGetItem = null;
    TryGetEnemy = null;
    TryGetEncounter = null;
    // 不公开的函数引用

    static registerItem(item, path) {
        __game_register_db.RegisterItem(item, path);
    }
    static unregisterItem(item) {
        __game_register_db.UnregisterItem(item);
    }

    static registerEnemy(enemy, path) {
        __game_register_db.RegisterEnemy(enemy, path);
    }
    static unregisterEnemy(enemy) {
        __game_register_db.UnregisterEnemy(enemy);
    }

    static registerEncounter(encounter, path) {
        __game_register_db.RegisterEncounter(encounter, path);
    }
    static unregisterEncounter(encounter) {
        __game_register_db.UnregisterEncounter(encounter);
    }
}