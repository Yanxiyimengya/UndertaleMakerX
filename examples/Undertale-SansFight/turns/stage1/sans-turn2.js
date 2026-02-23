import { UTMX , Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn2 extends UTMX.BattleTurn
{
    blueSoulController = new BlueSoulController();
    timer  = 0;
    
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(360, 130);
        this.soulInitPosition = new Vector2(0, 57);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = true;
    }
    
    onTurnStart()
    {
        this.blueSoulController.enabled = true;
    }
    
    onTurnUpdate(delta)
    {
        this.blueSoulController.update(delta);
        this.timer += 1;
        
        if (this.timer < 125)
        {
            if (this.timer % 50 == 1)
            {
                this.createWall(0);
            }
        }
        else if (this.timer < 320)
        {
            if (this.timer % 50 == 20)
            {
                this.createWall(1);
            }
        }
        
        if (this.timer == 370)
        {
            this.end();
        }
        
    }
    
    createWall(dir = 0)
    {
        if (dir == 0)
        {
            let bottomRBone = Bone.create(12, 130);
            bottomRBone.useMask = true;
            bottomRBone.position = new Vector2(320+260, 373);
            bottomRBone.speed = new Vector2(-4, 0);
            
            let bottomRBBone = Bone.create(53, 130);
            bottomRBBone.useMask = true;
            bottomRBBone.setStatus(1);
            bottomRBBone.position = new Vector2(320+200, 332);
            bottomRBBone.speed = new Vector2(-4, 0);
        }
        else
        {
            let bottomRBone = Bone.create(13, 130);
            bottomRBone.useMask = true;
            bottomRBone.position = new Vector2(320-200, 373);
            bottomRBone.speed = new Vector2(4, 0);
            
            let bottomRBBone = Bone.create(53, 130);
            bottomRBBone.useMask = true;
            bottomRBBone.setStatus(1);
            bottomRBBone.position = new Vector2(320-300, 332);
            bottomRBBone.speed = new Vector2(4, 0);
        }
    }
}