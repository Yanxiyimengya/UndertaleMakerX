import { UTMX } from "UTMX";

export default class ItemInstantNoodles extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "方便面";
        this.usedText = ["* 干吃更好\n* 你回复了 90 HP。"];
    }
    
    onUse()
    {
        UTMX.player.heal(90);
        this.removeSelf();
    }
}