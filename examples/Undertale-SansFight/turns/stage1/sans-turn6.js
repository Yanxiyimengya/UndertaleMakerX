import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn6 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;

    onTurnInit() {
        this.arenaInitSize = new Vector2(360, 130);
        this.soulInitPosition = new Vector2(0, 57);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = true;
    }

    onTurnStart() {
        this.blueSoulController.enabled = true;;
    }

    onTurnUpdate(delta) {
        this.blueSoulController.update(delta);
        this.timer += 1;
        
        if (this.timer % 100 == 1)
        {
            let platform;
            
            platform = this.blueSoulController.createPlatform(70);
            platform.position = new Vector2(-120, 300);
            platform.speed = new Vector2(2.5, 0);
            
            platform = this.blueSoulController.createPlatform(70);
            platform.position = new Vector2(640+120,345);
            platform.speed = new Vector2(-2.5, 0);
        }
        if (this.timer % 90 == 50) {
            let bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(560, 368);
            bone.speed = new Vector2(-2.5, 0);
            bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(560+50, 368);
            bone.speed = new Vector2(-2.5, 0);
        }
        if (this.timer % 160 == 70) {
            let bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(80, 324);
            bone.speed = new Vector2(2.5, 0);
            bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(80-100, 324);
            bone.speed = new Vector2(2.5, 0);
        }
        if (this.timer % 190 == 100)
        {
            let bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(560+70, 280);
            bone.speed = new Vector2(-2.5, 0);
            bone = Bone.create(18);
            bone.useMask = true;
            bone.position = new Vector2(560+50+70, 280);
            bone.speed = new Vector2(-2.5, 0);
        }
        
        if (this.timer == 680)
        {
            this.end();
        }
    }
    
    onTurnEnd()
    {
    }
}