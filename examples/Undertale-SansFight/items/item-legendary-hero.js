import { UTMX } from "UTMX";

export default class ItemLegendaryHero extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "传说英雄";
        this.usedText = ["* 你吃掉了传说英雄。\n* 攻击力提升了 4 点。\n* 你回复了 44 HP。"];
    }
    
    onUse()
    {
        UTMX.player.heal(44);
        UTMX.player.attack += 4;
        this.removeSelf();
    }
}