import { UTMX , Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import EnemySans from "enemies/sans.js";
import BlackScreen from "singalton/black-screen.js";
import GasterBlaster from "libraries/gaster-blaster/gaster-blaster.js";
import WarningBox from "libraries/warning-box/warning-box.js";
import Bone from "libraries/bone/bone.js";

export default class FirstTurn extends UTMX.BattleTurn
{
    blueSoulController = new BlueSoulController();
    timer = -1;
    bottomBoneWall = null;
    
    onTurnInit()
    {
        UTMX.battle.arena.getMainArena().size = new Vector2(150, 150);
        this.arenaInitSize = new Vector2(150, 150);
        this.turnTime = 999999.0;
        UTMX.audio.playBgm("BGM", "audios/bgm/mus_birdnoise.ogg", true);
        EnemySans.instance.anim = 0;
    }
    
    onTurnStart()
    {
        BlackScreen.instance.setVisible(true);
        UTMX.audio.stopBgm("BGM");
    }
    
    onTurnEnd()
    {
        UTMX.audio.playBgm("BGM", "audios/bgm/mus_zz_megalovania.ogg", true);
        EnemySans.instance.anim = 1;
    }
    
    onTurnUpdate(delta)
    {
        this.blueSoulController.update(delta);
        this.timer += 1;
        
        if (this.timer == 5) {
            EnemySans.instance.setSlam("down", 999);
            EnemySans.instance.setBlueEyes(true);
            BlackScreen.instance.setVisible(false);
        }
        if (this.timer == 10) {
            this.blueSoulController.enabled = true;
            this.blueSoulController.slam();
            let snd = UTMX.audio.playSound("audios/sfx/mus_sfx_segapower.wav");
            UTMX.audio.setSoundPitch(snd, 1.2);
        }
        if (this.timer == 30) {
            EnemySans.instance.setBlueEyes(false);
            EnemySans.instance.setFace(5);
            EnemySans.instance.setSlam("right", 999);
            
            let mainArena = UTMX.battle.arena.getMainArena();
            WarningBox.create(
                new Vector2(mainArena.position.x - 75, mainArena.position.y - 50),
                new Vector2(mainArena.position.x + 75, mainArena.position.y),
                10);
        }
        if (this.timer == 40)
        {   
            this.bottomBoneWall = UTMX.BattleProjectile.new();
            this.bottomBoneWall.useMask = true;
            this.bottomBoneWall.textures = "textures/sans/attack/spr_s_bonestab_v_wide_0.png";
            this.bottomBoneWall.position = new Vector2(320, 445);
            var tween = UTMX.tween.createTween();
            tween.addTweenProperty(this.bottomBoneWall, "position", new Vector2(320, 380), 0.1);
            UTMX.audio.playSound("audios/sfx/snd_spearrise.wav");
        }
        if (this.timer == 55) {
            this.blueSoulController.setEnabled(false);
        }
        if (this.timer == 70) {
            EnemySans.instance.setSlam(3, 200);
            let snd = UTMX.audio.playSound("audios/sfx/mus_sfx_segapower.wav");
            UTMX.audio.setSoundPitch(snd, 1.2);
        }
        
        if (this.timer > 70 && this.timer < 300)
        {
            if (this.timer == 130) {
                var tween = UTMX.tween.createTween();
                tween.addTweenProperty(this.bottomBoneWall, "position", new Vector2(320, 445), 0.1);
            }
            if (this.timer == 200) {
                this.bottomBoneWall.destroy();
            }
            if (this.timer < 180 && this.timer % 5 == 0)
            {
                this.spawnWaveBones();
            }
        }
        
        if (this.timer == 180)
        {
            GasterBlaster.create(Vector2.Zero, new Vector2(190, 255), 0, -90);
            GasterBlaster.create(Vector2.Zero, new Vector2(265, 180), 0,  0);
            
            GasterBlaster.create(new Vector2(640, 480), new Vector2(380, 440), 0, 180);
            GasterBlaster.create(new Vector2(640, 480), new Vector2(450, 370), 0,  90);
        }
        
        if (this.timer == 180+50)
        {
            GasterBlaster.create(Vector2.Zero, new Vector2(190, 180), 0, -45);
            GasterBlaster.create(new Vector2(640, 0), new Vector2(450, 180), 0, 45);
            
            GasterBlaster.create(new Vector2(0, 480), new Vector2(190, 440), 0, -135);
            GasterBlaster.create(new Vector2(640, 480), new Vector2(450, 440), 0, 135); 
        }
        
        if (this.timer == 180+50*2)
        {
            GasterBlaster.create(Vector2.Zero, new Vector2(190, 255), 0, -90);
            GasterBlaster.create(Vector2.Zero, new Vector2(265, 180), 0,  0);
            
            GasterBlaster.create(new Vector2(640, 480), new Vector2(380, 440), 0, 180);
            GasterBlaster.create(new Vector2(640, 480), new Vector2(450, 370), 0,  90);
        }
        if (this.timer == 180+50*3)
        {
            let gbLeft = GasterBlaster.create(new Vector2(-160, 320), new Vector2(130, 310), 0, -90);
            gbLeft.scale = new Vector2(4, 4);
            gbLeft.shootDelay = 70;
            
            let gbRight = GasterBlaster.create(new Vector2(700, 320), new Vector2(510, 310), 0,  90);
            gbRight.scale = new Vector2(4, 4);
            gbRight.shootDelay = 70;
        }
        if (this.timer == 500)
        {
            EnemySans.instance.setFace(0);
            let bubble = UTMX.SpeechBubble.new("[speed=0.05][voice='audios/voices/snd_txtsans.wav'][font='fonts/sans.ttf'][size=16]我们开始吧。[waitfor=confirm][start][end]");
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(200, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmdName, args) => {
                const cmd = String(cmdName).trim().toLowerCase();
                if (cmd === "start") {
                    this.end();
                    return true;
                }
                return false;
            }
        }
    }

    spawnWaveBones()
    {
        const mainArena = UTMX.battle.arena.getMainArena();
        const arenaWidth = mainArena.size.x;
        const arenaHeight = mainArena.size.y;
        const arenaLeft = mainArena.position.x - arenaWidth * 0.5;
        const arenaRight = mainArena.position.x + arenaWidth * 0.5;
        const arenaTop = mainArena.position.y - arenaHeight + 2;
        const arenaBottom = mainArena.position.y - 3;

        const waveSpawnX = arenaLeft - 15;
        const waveSpeedX = 4;
        const waveGapSize = 40;
        const waveCenterBaseOffset = arenaHeight * (35 / 150);
        const waveAmplitude = arenaHeight * (20 / 150);
        const waveGapCenterY =
            (mainArena.position.y - arenaHeight * 0.5) +
            (Math.sin(this.timer * 0.08) * waveAmplitude - waveCenterBaseOffset);

        const gapHalf = waveGapSize * 0.5;
        const minGapCenterY = arenaTop + gapHalf;
        const maxGapCenterY = arenaBottom - gapHalf;
        const clampedGapCenterY = Math.max(minGapCenterY, Math.min(maxGapCenterY, waveGapCenterY));
        const gapTopY = clampedGapCenterY - gapHalf;
        const gapBottomY = clampedGapCenterY + gapHalf;

        const travelDistance = (arenaRight + 25) - waveSpawnX;
        const lifeTime = Math.ceil(travelDistance / waveSpeedX) + 2;

        const topBone = Bone.create(1, lifeTime);
        topBone.useMask = true;
        topBone.speed = new Vector2(waveSpeedX, 0);
        topBone.setVertexMode(
            new Vector2(waveSpawnX, arenaTop),
            new Vector2(waveSpawnX, gapTopY)
        );

        const bottomBone = Bone.create(1, lifeTime);
        bottomBone.useMask = true;
        bottomBone.speed = new Vector2(waveSpeedX, 0);
        bottomBone.setVertexMode(
            new Vector2(waveSpawnX, gapBottomY),
            new Vector2(waveSpawnX, arenaBottom)
        );
    }
}
