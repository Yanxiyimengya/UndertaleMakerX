import { UTMX } from "UTMX";

export default class ItemPie extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "派";
        this.usedText = ["* 你吃掉了奶油糖果派\n* 你的HP满了。"];
    }
    
    onUse()
    {
        UTMX.player.heal(UTMX.player.maxHp);
        this.removeSelf();
    }
}