import { UTMX } from "UTMX";

export default class ItemSnowPiece extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "雪块";
        this.usedText = ["* 你吃掉了雪人的雪块\n* 你回复了 45 HP。"];
    }
    
    onUse()
    {
        UTMX.player.heal(45);
        this.removeSelf();
    }
}