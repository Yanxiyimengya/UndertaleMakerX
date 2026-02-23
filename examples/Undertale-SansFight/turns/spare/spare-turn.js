import { UTMX , Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import EnemySans from "enemies/sans.js";
import BlackScreen from "singalton/black-screen.js";

export default class FirstTurn extends UTMX.BattleTurn
{
    timer = -1;
    blueSoulController = new BlueSoulController();
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(100, 100);
        this.soulInitPosition = new Vector2(0, 15);
        this.turnTime = 999999.0;
        this.blueSoulController.setEnabled(false);
    }
    
    onTurnStart()
    {
    }
    
    onTurnEnd()
    {
    }
    
    onTurnUpdate(delta)
    {
        this.timer += 1;
        if (this.timer == 10)
        {   
            EnemySans.instance.setFace(3);
            EnemySans.instance.setBody(1);
            this.bottomBoneWall = UTMX.BattleProjectile.new();
            this.bottomBoneWall.useMask = true;
            this.bottomBoneWall.textures = "textures/sans/attack/spr_s_bonestab_v_wide_0.png";
            this.bottomBoneWall.position = new Vector2(320, 445);
            var tween = UTMX.tween.createTween();
            tween.addTweenProperty(this.bottomBoneWall, "position", new Vector2(320, 335), 0.1);
            UTMX.audio.playSound("audios/sfx/snd_spearrise.wav");
        }
    }
}