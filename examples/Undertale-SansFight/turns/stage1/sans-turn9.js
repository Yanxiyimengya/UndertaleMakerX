import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn9 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;
    boneList = [];
    mainPlatform = null;
    state = 0;
    frameQuick = 170;

    onTurnInit() {
        this.arenaInitSize = new Vector2(400, 160);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = true;
    }

    onTurnStart() {
        this.blueSoulController.enabled = true;

        this.mainPlatform = this.blueSoulController.createPlatform(15);
        this.mainPlatform.position = new Vector2(320-150, 330);
        
        this.state = 0;
    }

    onTurnUpdate(delta) {
        this.blueSoulController.update(delta);
        this.timer += 1;
        if (this.timer == 1) {
            UTMX.battle.soul.position = new Vector2(320-150, 318);
            UTMX.battle.soul.rotation = 0;
        }
        if (this.timer == 460)
        {
            this.end();
        }
        
        if (this.state == 0) {
            if (this.mainPlatform.position.x > 320+180) {
                this.state = 1;
            }
            this.mainPlatform.speed = new Vector2(1.25, 0);
        }
        else if (this.state == 1) {
            this.mainPlatform.speed = new Vector2(-1.25, 0);
        }

        while (this.frameQuick > 0) {
            this.update();
            this.frameQuick -= 1;
        }
        this.update();
    }

    frame = 0;
    boneList = [];
    update()
    {
        if (this.frame % 60 == 30)
        {
            let b = UTMX.BattleProjectile.new();
            b.useMask = true;
            b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
            b.position = new Vector2(270, 430);
            b.moveSpeed = new Vector2(0, -1.4);
            this.boneList.push(b);

            b = UTMX.BattleProjectile.new();
            b.useMask = true;
            b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
            b.position = new Vector2(270+160, 430);
            b.moveSpeed = new Vector2(0, -1.4);
            this.boneList.push(b);
        }
        if (this.frame % 80 == 0)
        {
            let b = UTMX.BattleProjectile.new();
            b.useMask = true;
            b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
            b.position = new Vector2(270+80, 200);
            b.moveSpeed = new Vector2(0, 1.4);
            this.boneList.push(b);
        }
        if (this.frame % 7 == 0)
        {
            let b = UTMX.BattleProjectile.new();
            b.useMask = true;
            b.textures = "textures/sans/attack/spr_s_bonewall_0.png";
            b.position = new Vector2(120, 380);
            b.moveSpeed = new Vector2(2, 0);
            this.boneList.push(b);
        }
        for (let i = 0; i < this.boneList.length; i ++)
        {
            let bone = this.boneList[i];
            if (bone.position.x > 520 || bone.position.y < 200 || bone.position.y > 430) {
                bone.destroy();
                this.boneList.splice(i, 1);
                i--;
                continue;
            }
            bone.position = 
                new Vector2(bone.position.x + bone.moveSpeed.x,
                    bone.position.y + bone.moveSpeed.y);
        }
        this.frame++;
    }
    
    onTurnEnd()
    {
    }
}
