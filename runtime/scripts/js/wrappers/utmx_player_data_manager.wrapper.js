import { __player_data_manager } from "__UTMX";
import { UtmxBaseItem } from "./utmx_item.wrapper";

class PlayerInventory
{
    static addItem(itemId) {
        __player_data_manager.AddItem(itemId);
    }
    static removeItem(slot) {
        __player_data_manager.RemoveItem(slot);
    }
    static getItem(slot) {
        let item = __player_data_manager.GetItemAt(slot);
        if (item.JsInstance != null && item.JsInstance instanceof UtmxBaseItem.constructor)
        {
            return item.JsInstance;
        }
        return item;
    }

    static get itemCount() {
        return __player_data_manager.GetItemCount();
    }
    static set itemCount(value) {};
}

export class UtmxPlayerDataManager {
    get name() { return __player_data_manager.PlayerName; }
    set name(value) { __player_data_manager.PlayerName = value; }

    get lv() { return __player_data_manager.PlayerLv; }
    set lv(value) { __player_data_manager.PlayerLv = value; }

    get hp() { return __player_data_manager.PlayerHp; }
    set hp(value) { __player_data_manager.PlayerHp = value; }

    get maxHp() { return __player_data_manager.PlayerMaxHp; }
    set maxHp(value) { __player_data_manager.PlayerMaxHp = value; }

    get exp() { return __player_data_manager.PlayerExp; }
    set exp(value) { __player_data_manager.PlayerExp = value; }

    get gold() { return __player_data_manager.PlayerGold; }
    set gold(value) { __player_data_manager.PlayerGold = value; }

    get attack() { return __player_data_manager.PlayerAttack; }
    set attack(value) { __player_data_manager.PlayerAttack = value; }

    get defence() { return __player_data_manager.PlayerDefence; }
    set defence(value) { __player_data_manager.PlayerDefence = value; }

    get invincibleTime() { return __player_data_manager.PlayerInvincibleTime; }
    set invincibleTime(value) { __player_data_manager.PlayerInvincibleTime = value; }

    static hurt(value, invtime = -1) {
        __player_data_manager.Hurt(value, invtime);
    }
    static heal(value) {
        __player_data_manager.Heal(value);
    }

    static inventory = PlayerInventory;
}