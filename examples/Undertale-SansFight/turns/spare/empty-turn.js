import { UTMX , Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import EnemySans from "enemies/sans.js";
import BlackScreen from "singalton/black-screen.js";

export default class EmptyTurn extends UTMX.BattleTurn
{
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(155, 130);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 0.1;
    }
}