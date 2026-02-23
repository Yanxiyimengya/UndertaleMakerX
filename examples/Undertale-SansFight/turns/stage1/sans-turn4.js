import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn4 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;
    boneList = [];

    onTurnInit() {
        this.arenaInitSize = new Vector2(360, 130);
        this.soulInitPosition = new Vector2(0, 57);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = true;
    }

    onTurnStart() {
        this.blueSoulController.enabled = true;
    }

    onTurnUpdate(delta) {
        this.blueSoulController.update(delta);
        this.timer += 1;
        
        if (this.timer > 50 && this.timer < 380) {
            if (this.timer % 7 == 1)
            {
                let b = UTMX.BattleProjectile.new();
                b.useMask = true;
                b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
                b.position = new Vector2(120, 380);
                this.boneList.push(b);
            }
        }
        if (this.timer == 10)
        {
            let platform = this.blueSoulController.createPlatform(30);
            platform.position = new Vector2(-30, 348);
            platform.speed = new Vector2(2, 0);
        }
        if (this.timer == 150)
        {
            let platform = this.blueSoulController.createPlatform(30);
            platform.position = new Vector2(-30, 348);
            platform.speed = new Vector2(2.5, 0);
        }
        if (this.timer == 250)
        {
            let platform = this.blueSoulController.createPlatform(30);
            platform.position = new Vector2(-30, 348);
            platform.speed = new Vector2(2.7, 0);
            
            let createBone = (x) => {
                 let bone = Bone.create(26);
                bone.useMask = true;
                bone.position = new Vector2(x, 280);
                bone.speed = new Vector2(3.25, 0);
            }
            createBone(-50);
            createBone(-50-15);
            createBone(-50-30);
        }
        if (this.timer == 465)
        {
            let bone = Bone.create(46);
            bone.useMask = true;
            bone.position = new Vector2(120, 300);
            bone.speed = new Vector2(4, 0);
        }
        
        for (let i = 0; i < this.boneList.length; i ++)
        {
            let bone = this.boneList[i];
            if (bone.position.x > 510) {
                bone.destroy();
                this.boneList.splice(i, 1);
                i--;
                continue;
            }
            bone.position = new Vector2(bone.position.x + 2, bone.position.y);
        }
        
        
        if (this.timer == 560)
        {
            this.end();
        }
    }
    
    onTurnEnd()
    {
    }
}