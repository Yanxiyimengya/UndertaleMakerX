import { __player_data_manager } from "__UTMX";
import { UtmxBaseItem } from "./types/utmx-item.wrapper.js";

class PlayerInventory
{
    static get maxInventoryCount() { return __player_data_manager.MaxInventoryCount; }
    static set maxInventoryCount(value) { __player_data_manager.MaxInventoryCount = value; }

    static addItem(itemId) {
        __player_data_manager.AddItem(itemId);
    }
    static setItem(itemId, slot) {
        __player_data_manager.SetItem(itemId, slot);
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
    
    static setWeapon(weaponId) {
        __player_data_manager.SetWeapon(weaponId);
    }
    static getWeapon() {
        return __player_data_manager.Weapon;
    }

    static setArmor(armorId) {
        __player_data_manager.SetArmor(armorId);
    }
    static getArmor() {
        return __player_data_manager.Armor;
    }

    static getItemCount() {
        return __player_data_manager.GetItemCount();
    }
}

export class UtmxPlayerDataManager {
    static get name() { return __player_data_manager.PlayerName; }
    static set name(value) { __player_data_manager.PlayerName = value; }

    static get lv() { return __player_data_manager.PlayerLv; }
    static set lv(value) { __player_data_manager.PlayerLv = value; }

    static get hp() { return __player_data_manager.PlayerHp; }
    static set hp(value) { __player_data_manager.PlayerHp = value; }

    static get maxHp() { return __player_data_manager.PlayerMaxHp; }
    static set maxHp(value) { __player_data_manager.PlayerMaxHp = value; }

    static get exp() { return __player_data_manager.PlayerExp; }
    static set exp(value) { __player_data_manager.PlayerExp = value; }

    static get gold() { return __player_data_manager.PlayerGold; }
    static set gold(value) { __player_data_manager.PlayerGold = value; }

    static get attack() { return __player_data_manager.PlayerAttack; }
    static set attack(value) { __player_data_manager.PlayerAttack = value; }

    static get defence() { return __player_data_manager.PlayerDefence; }
    static set defence(value) { __player_data_manager.PlayerDefence = value; }

    static get invincibleTime() { return __player_data_manager.PlayerInvincibleTime; }
    static set invincibleTime(value) { __player_data_manager.PlayerInvincibleTime = value; }

    static hurt(damage, invtime = -1) {
        __player_data_manager.Hurt(damage, invtime);
    }
    static heal(heal) {
        __player_data_manager.Heal(heal);
    }

    static inventory = PlayerInventory;
}