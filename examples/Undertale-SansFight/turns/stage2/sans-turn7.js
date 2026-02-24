import { UTMX , Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn extends UTMX.BattleTurn
{
    timer = -1;
    blueSoulController = new BlueSoulController();
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(150, 150);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 9999999;
        this.blueSoulController.enabled = false;
    }
    
    onTurnStart()
    {
    }
    
    onTurnEnd()
    {
        this.arenaInitSize = new Vector2(555, 130);
    }
    
    onTurnUpdate(delta)
    {
        this.timer += 1;

        if (this.timer > 0 && this.timer < 290)
        {
            if (this.timer % 40 == 1)
            {
                var lr = Bone.create(90, 100);
                lr.position = new Vector2(320 - 90, -5);
                lr.rotation = 90;
                lr.speed = new Vector2(0, 5);
                
                var lr = Bone.create(90, 100);
                lr.position = new Vector2(320 + 90, 485);
                lr.rotation = 90;
                lr.speed = new Vector2(0, -5);
            }
        }
        if (this.timer == 400)
        {
            this.end();
        }
    }
}