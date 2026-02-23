import { UTMX } from "UTMX";

export default class ItemFaceSteak extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "牛排";
        this.usedText = ["* 你吃掉了牛排\n* 你回复了 60 HP。"];
    }
    
    onUse()
    {
        UTMX.player.heal(60);
        this.removeSelf();
    }
}