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
        
        if (this.timer > 60 && this.timer < 460) {
            if (this.timer % 7 == 1)
            {
                let b = UTMX.BattleProjectile.new();
                b.useMask = true;
                b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
                b.position = new Vector2(510, 380);
                this.boneList.push(b);
            }
        }
        if (this.timer == 1)
        {
            let bone, platform;
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740, 348);
            platform.speed = new Vector2(-2.4, 0);
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740+120, 348-40);
            platform.speed = new Vector2(-2.4, 0);
            bone = Bone.create(31);
            bone.useMask = true;
            bone.position = new Vector2(740+120-20, 355);
            bone.speed = new Vector2(-2.4, 0);
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740+190, 348);
            platform.speed = new Vector2(-2.4, 0);
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740+400, 348-25);
            platform.speed = new Vector2(-2.4, 0);
            bone = Bone.create(21);
            bone.useMask = true;
            bone.position = new Vector2(740+400, 275);
            bone.speed = new Vector2(-2.4, 0);
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740+520, 348-15);
            platform.speed = new Vector2(-2.4, 0);
            bone = Bone.create(26);
            bone.useMask = true;
            bone.position = new Vector2(740+520, 280);
            bone.speed = new Vector2(-2.4, 0);
            
            platform = this.blueSoulController.createPlatform(25);
            platform.position = new Vector2(740+640, 348-35);
            platform.speed = new Vector2(-2.4, 0);
            bone = Bone.create(16);
            bone.useMask = true;
            bone.position = new Vector2(740+640, 270);
            bone.speed = new Vector2(-2.4, 0);
        }
        if (this.timer == 230)
        {
            let platform = this.blueSoulController.createPlatform(14);
            platform.position = new Vector2(740, 348-50);
            platform.speed = new Vector2(-1.5, 0);
        }
        
        
        if (this.timer ==420)
        {
            let bone = Bone.create(46);
            bone.useMask = true;
            bone.position = new Vector2(120, 300);
            bone.speed = new Vector2(1.5, 0);
        }
        else if (this.timer == 450)
        {
            let bone = Bone.create(51);
            bone.useMask = true;
            bone.position = new Vector2(555, 335);
            bone.speed = new Vector2(-3.5, 0);
        }
        
        for (let i = 0; i < this.boneList.length; i ++)
        {
            let bone = this.boneList[i];
            if (bone.position.x < 120) {
                bone.destroy();
                this.boneList.splice(i, 1);
                i--;
                continue;
            }
            bone.position = new Vector2(bone.position.x - 2, bone.position.y);
        }
        
        
        if (this.timer == 620)
        {
            this.end();
        }
    }
    
    onTurnEnd()
    {
    }
}