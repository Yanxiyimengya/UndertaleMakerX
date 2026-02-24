import { UTMX , Vector2 , Color } from "UTMX";
import EnemySans from "enemies/sans.js";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import WarningBox from "libraries/warning-box/warning-box.js";
import Bone from "libraries/bone/bone.js";
import BlackScreen from "singalton/black-screen.js";
import GasterBlaster from "libraries/gaster-blaster/gaster-blaster.js";

const SANS_DIALOG = "[speed=0.05][voice='audios/voices/snd_txtsans.wav'][font='fonts/sans.ttf'][size=16]";

var blueSoulController = new BlueSoulController();

let SLAM_INTERVAL = 40;
let WARNING_FRAMES = 7;
let POST_SLAM_TO_WALL_FRAMES = 20;
let BONE_WALL_HOLD_FRAMES = 10;
let BONE_WALL_RETRACT_FRAMES = 20;
let BONE_WALL_OFFSET = 65;
let WALL_DEPTH = 40;
let BONE_WALL_HALF_THICKNESS = 50;
const WINDMILL_TARGET_LEAD_DEG = 10;
const WINDMILL_SPAWN_RADIUS = 360;
const WINDMILL_TARGET_RADIUS = 180;
const WINDMILL_GB_SCALE_X = 0.5;
const WINDMILL_GB_SCALE_Y = 1;
const WINDMILL_GB_SHOOT_DELAY = 70;
const WINDMILL_GB_HOLD_FIRE = 2;
const WINDMILL_GB_INTRO_SPEED = 0.15;

export default class FinalTurn extends UTMX.BattleTurn
{
    timer = 0;
    timerDelta = 1;
    side = 0;
    slamTimer = 0;
    pendingSlamDir = null;
    pendingSlamTimer = 0;
    pendingWallDir = null;
    pendingWallTimer = 0;
    boneWalls = [];
    windmillPhaseDeg = 30;
    windmillStarted = false;
    windmillGbs = [];
    specialAttack = false;
    
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(150, 150);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 9999999;
        blueSoulController.enabled = false;
    }
    
    onTurnStart()
    {
        blueSoulController.enabled = true;
        this.slamTimer = 0;
        this.pendingSlamDir = null;
        this.pendingSlamTimer = 0;
        this.pendingWallDir = null;
        this.pendingWallTimer = 0;
        this.windmillPhaseDeg = 30;
        this.windmillStarted = false;
        this.windmillGbs = [];
        this.clearBoneWall();
    }
    
    onTurnEnd()
    {
        this.clearBoneWall();
        this.clearWindmillGbs();
    }
    
    onTurnUpdate(delta)
    {
        this.timer += this.timerDelta;
        blueSoulController.update(delta);
        if (this.timer > 0) this.updateBoneWall();
        if (this.timer > 0 && this.timer <= 160)
        {
            if (this.slamTimer > 0) {
                this.slamTimer -= 1;
            } else {
                const dir = ["down", "right", "up", "left"][Math.floor(Math.random() * 4)];
                this.slamTimer = SLAM_INTERVAL;
                this.createWarning(dir);
                this.slam(dir);
                this.pendingWallDir = dir;
                this.pendingWallTimer = POST_SLAM_TO_WALL_FRAMES;
            }
            if (this.pendingWallDir != null)
            {
                this.pendingWallTimer -= 1;
                if (this.pendingWallTimer <= 0)
                {
                    const dir = this.pendingWallDir;
                    this.pendingWallDir = null;
                    this.createBoneWall(dir);
                }
            }
        }
        if (this.timer == 160)
        {
            EnemySans.instance.setSlam("up", 99999);
            UTMX.battle.soul.rotation = 0;
            blueSoulController.enabled = false;
        }
        if (this.timer > 160 && this.timer < 280)
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
        if (this.timer == 310)
        {
            blueSoulController.enabled = true;
            this.slam("left", 100);
        }
        if (this.timer == 340)
        {
            blueSoulController.enabled = false;
            EnemySans.instance.setSlam("right", 9999);
            UTMX.battle.soul.rotation = 0;
            UTMX.battle.soul.sprite.rotation = -90;
            UTMX.battle.soul.sprite.color = Color.Blue;
            UTMX.battle.soul.movable = false;
            
            let mainArena = UTMX.battle.arena.getMainArena();
            this.arenaTween = UTMX.tween.createTween();
            this.arenaTween.setParallel(true);
            this.arenaTween.addTweenProperty(mainArena, "position", new Vector2(320+80, 385), 0.25);
            this.arenaTween.addTweenProperty(mainArena, "size", new Vector2(mainArena.size.x + 160, 150), 0.25);
        }
        
        if (this.timer >= 360 && this.timer <= 1130)
        {
            if (this.timer == 360)
            {
                if (this.arenaTween != null && this.arenaTween.isRunning())
                {
                    this.arenaTween.kill();
                }
                const rendomAnim = () =>
                    {
                        const faces = [0,1,3,4,5];
                        EnemySans.instance.setFace(faces[Math.floor(Math.random() * faces.length)] );
                        EnemySans.instance.setBody(Math.floor(Math.random() * 2));
                    }
                let mainArena = UTMX.battle.arena.getMainArena();
                this.arenaTween = UTMX.tween.createTween();
                this.arenaTween.setParallel(true);
                this.arenaTween.addTweenProperty(mainArena, "size", new Vector2(650, 120), 0.5);
                this.arenaTween.addTweenProperty(mainArena, "position", new Vector2(320, 385-5), 0.5);
                this.arenaTween.addTweenProperty(UTMX.battle.soul, "position", new Vector2(25, 320), 0.75).delay(0.3);
                this.arenaTween.addTweenProperty(EnemySans.instance, "position", new Vector2(-420, 
                    EnemySans.instance.position.y), 0.6).callback(rendomAnim);
                this.arenaTween.setParallel(false);
                for (let i = 0; i < 15; i ++)
                {
                    const fromPos = new Vector2(420, EnemySans.instance.position.y);
                    const targetPos = new Vector2(-420, EnemySans.instance.position.y);
                    this.arenaTween.addTweenProperty(EnemySans.instance, "position", 
                        targetPos, 0.7).from(fromPos).delay(0.2).callback(rendomAnim);
                }
            }
            if (this.timer > 450 && this.timer < 680)
            {
                if (this.timer % 3 == 0)
                {
                    const sinValue = 0.5 + Math.sin(this.timer * 0.1) * 0.3;
                    this.spawnSideWall(40, sinValue);
                }
            }
            if (this.timer == 680)
            {
                this.side = 0;
                this.gapSize = 90;
            }
            if (this.timer > 700 && this.timer < 950)
            {
                if (this.timer % 25 == 0)
                {
                    this.createPair(this.side);
                    this.side = this.side == 0 ? 1 : 0;
                }
            }
            if (this.timer > 950 && this.timer < 1020)
            {
                if (this.timer % 3 == 0)
                {
                    this.spawnSideWall(this.gapSize, 0.5);
                    this.gapSize -= 2.5;
                }
            }
            if (this.timer == 1050)
            {
                let mainArena = UTMX.battle.arena.getMainArena();
                this.arenaTween2 = UTMX.tween.createTween();
                this.arenaTween2.setParallel(true);
                this.arenaTween2.addTweenProperty(mainArena, "position", new Vector2(70, 385-5), 0.5);
                this.arenaTween2.addTweenProperty(UTMX.battle.soul, "position",
                    new Vector2(380, 320), 0.65).callback(()=>{
                        blueSoulController.enabled = true;
                        UTMX.battle.soul.rotation = -90;
                        UTMX.battle.soul.sprite.rotation = 0;
                        this.slam("right", 100);
                        if (this.arenaTween != null && this.arenaTween.isRunning())
                        {
                            this.arenaTween.kill();
                        }
                    });
            }
            if (this.timer == 1100)
            {
                WARNING_FRAMES = 10;
                BONE_WALL_HOLD_FRAMES = 20;
                BONE_WALL_RETRACT_FRAMES = 20;
                this.createWarning("right");
            }
            if (this.timer == 1120)
            {
                this.createBoneWall("right");
            }
            if (UTMX.input.isActionHeld("up"))
            {
                UTMX.battle.soul.tryMoveTo(
                    new Vector2(UTMX.battle.soul.position.x, UTMX.battle.soul.position.y - 2.5));
            }
            else if (UTMX.input.isActionHeld("down"))
            {
                UTMX.battle.soul.tryMoveTo(
                    new Vector2(UTMX.battle.soul.position.x, UTMX.battle.soul.position.y + 2.5));
            }
        }

        if (this.timer == 1150)
        {
            BlackScreen.instance.setVisible(true);
            WARNING_FRAMES = 15;
            BONE_WALL_RETRACT_FRAMES = 15;
            BONE_WALL_HOLD_FRAMES = 25;
        }
        if (this.timer == 1150+30)
        {
            let mainArena = UTMX.battle.arena.getMainArena();
            mainArena.size = new Vector2(150, 150);
            mainArena.position = new Vector2(320, 380);
            BlackScreen.instance.setVisible(false);
            this.createWarning("up", 30);
            this.createWarning("down", 30);
            UTMX.battle.soul.rotation = 0;
            UTMX.battle.soul.position = new Vector2(320, 380);
        }
        if (this.timer == 1150+60)
        {
            this.createBoneWall("up");
            this.createBoneWall("down");
        }
        
        // ===---

        if (this.timer == 1240)
        {
            BlackScreen.instance.setVisible(true);
        }
        if (this.timer == 1240+10)
        {
            UTMX.battle.soul.position = new Vector2(320-70, 320-70);
            UTMX.battle.soul.rotation = 180;
            BlackScreen.instance.setVisible(false);
            this.createWarning("up", 30);
            this.createWarning("left", 30);
        }
        if (this.timer == 1240+40)
        {
            this.createBoneWall("up");
            this.createBoneWall("left");
        }

        // ===---
        
        if (this.timer == 1310)
        {
            BlackScreen.instance.setVisible(true);
        }
        if (this.timer == 1310+10)
        {
            UTMX.battle.soul.position = new Vector2(320+70, 320+70);
            UTMX.battle.soul.rotation = -90;
            BlackScreen.instance.setVisible(false);
            this.createWarning("down", 30);
            this.createWarning("right", 30);
        }
        if (this.timer == 1310+40)
        {
            this.createBoneWall("down");
            this.createBoneWall("right");
        }

        // ===---
        
        if (this.timer == 1380)
        {
            BlackScreen.instance.setVisible(true);
        }
        if (this.timer == 1380+10)
        {
            EnemySans.instance.position = new Vector2(0, -10);
            UTMX.battle.soul.position = new Vector2(320-70, 320-70);
            UTMX.battle.soul.rotation = 90;
            BlackScreen.instance.setVisible(false);
            this.createWarning("left", 30);
        }
        if (this.timer == 1380+40)
        {
            this.createBoneWall("left");
        }

        if (this.timer == 1430)
        {
            if (this.windmillStarted) return;
            this.windmillStarted = true;
            blueSoulController.enabled = false;
            UTMX.battle.soul.rotation = 0;
            UTMX.battle.soul.sprite.rotation = 0;
        }
        if (this.timer >= 1430 && this.timer <= 1860)
        {
            if ((this.timer - 1450) % 4 == 0)
            {
                this.spawnWindmillGbSingle();
                this.windmillPhaseDeg -= 12;
            }
        }

        if (this.timer == 1930)
        {
            EnemySans.instance.anim = 2;
            this.slamSpeed = 10;
            EnemySans.instance.setBlueEyes(true);
            blueSoulController.slamGravityScale = 4;
            blueSoulController.enabled = true;
        }
        if (this.timer > 1930 && this.timer < 2600)
        {
            if (this.timer % this.slamSpeed == 0)
            {
                const dir = ["down", "right", "up", "left"][Math.floor(Math.random() * 4)];
                this.slam(dir, 999999);
            }
            if (this.timer == 2200) {
                this.slamSpeed = 15;
            }
            if (this.timer == 2300) {
                this.slamSpeed = 30;
                EnemySans.instance.setBlueEyes(false);
                EnemySans.instance.setFace(0);
                EnemySans.instance.setSweat(true);
                blueSoulController.slamGravityScale = 3.0;
            }
            if (this.timer == 2400)
            {
                this.slamSpeed = 70;
                blueSoulController.slamGravityScale = 2.0;
            }
            if (this.timer == 2450)
            {
                EnemySans.instance.setFace(2);
                this.slamSpeed = 100;
                blueSoulController.slamGravityScale = 1;
            }
        }
        if (this.timer == 2700)
        {
            if (UTMX.audio.isBgmValid("BGM")) {
                UTMX.audio.setBgmPaused("BGM", true);
            }
            this.slam("down", 999999);
            blueSoulController.slamGravityScale = 0.5;
        }
        if (this.timer == 2800)
        {
            blueSoulController.enabled = false;
            this.timerDelta = 0;
            this.timer += 1;
            let bubble = UTMX.SpeechBubble.new(
                SANS_DIALOG + "[setFace=9][setBody=0]呼...哈...[waitfor=confirm][clear]" +
                SANS_DIALOG + "好了，\n[wait=0.2]我已经受够了。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setBody=1]是时候使出我的\n[color=red]特殊攻击[/color]了。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]准备好了没。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]没什么可怕的。[waitfor=confirm][setBody=0][setFace=2][end]"
            );
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(200, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmd, args) => {
                if (cmd == "setFace") { EnemySans.instance.setFace(args.value); return true; }
                if (cmd == "setBody") { EnemySans.instance.setBody(args.value); return true; }
                if (cmd == "end")
                {
                    this.timerDelta = 1;
                }
                return false;
            };
        }
        if (this.timer == 3300)
        {
            this.timerDelta = 0;
            this.timer += 1;
            let bubble = UTMX.SpeechBubble.new(
                SANS_DIALOG + "[setFace=1][setBody=0]对。[waitfor=confirm][clear]" +
                SANS_DIALOG + "没错。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]字面上的没什么。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]而且，[wait=0.2]接下来\n也什么都不会有。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]呵呵呵...\n[wait=0.2]懂了不？[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]我知道\n我打不赢你。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]说不准在你的\n哪个回合。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=9]你就能把我杀了。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]所以，[wait=0.2]嗯。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]我决定了...[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]你的回合永远\n都不会到来了...[wait=0.5]\n永远...[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=5]只要你不放弃，[wait=0.2]\n我的回合永远\n都不会结束。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]懂了吗？[waitfor=confirm][setBody=0][setFace=0][end]"
            );
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(200, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmd, args) => {
                if (cmd == "setFace") { EnemySans.instance.setFace(args.value); return true; }
                if (cmd == "setBody") { EnemySans.instance.setBody(args.value); return true; }
                if (cmd == "end")
                {
                    this.timerDelta = 1;
                }
                return false;
            };
        }
        if (this.timer == 3600)
        {
            this.timerDelta = 0;
            this.timer += 1;
            let bubble = UTMX.SpeechBubble.new(
                SANS_DIALOG + "[setFace=2][setBody=0]这样下去\n你肯定会厌倦的。[waitfor=confirm][clear]" +
                SANS_DIALOG + "[setFace=1]我是说，[wait=0.2]\n前提是你现在\n还没厌倦。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=5]到时候[wait=0.2]\n你肯定会退出的。[waitfor=confirm][setBody=0][setFace=0][end]"
            );
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(200, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmd, args) => {
                if (cmd == "setFace") { EnemySans.instance.setFace(args.value); return true; }
                if (cmd == "setBody") { EnemySans.instance.setBody(args.value); return true; }
                if (cmd == "end")
                {
                    this.timerDelta = 1;
                }
                return false;
            };
        }
        if (this.timer == 3800)
        {
            this.timerDelta = 0;
            this.timer += 1;
            let bubble = UTMX.SpeechBubble.new(
                SANS_DIALOG + "[setFace=5][setBody=0]我知道你\n是哪种人。[waitfor=confirm][clear]" +
                SANS_DIALOG + "[setFace=1]你是，[wait=0.2]呃，[wait=0.2]\n非常有决心的人。[wait=0.2]\n不是吗？[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]你不会放弃，\n[wait=0.2]尽管这，[wait=0.2]\n呃...[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]确实没有\n任何益处能让人\n去寻求。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]也许我能\n说得更明白。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=4]不论如何，\n[wait=0.2]你会继续前进。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=9]不是为了寻求\n什么好的坏的。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]只是因为\n你觉得自己\n做得到。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]也是因为你\"可以\"，\n[wait=0.2]所以你\"必须去做\"。[setBody=0][setFace=0][waitfor=confirm][end]"
            );
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(210, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmd, args) => {
                if (cmd == "setFace") { EnemySans.instance.setFace(args.value); return true; }
                if (cmd == "setBody") { EnemySans.instance.setBody(args.value); return true; }
                if (cmd == "end")
                {
                    this.timerDelta = 1;
                }
                return false;
            };
        }
        if (this.timer == 4000)
        {
            this.timerDelta = 0;
            this.timer += 1;
            let bubble = UTMX.SpeechBubble.new(
                SANS_DIALOG + "[setFace=9][setBody=0]不过现在\n你已经走到头了。[waitfor=confirm][clear]" +
                SANS_DIALOG + "[setFace=4]对你来说[wait=0.2]\n现在这里\n什么也不剩了。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]所以...[wait=0.2]呃。[wait=0.2]\n我的个人意见是...[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=3]你现在能做的\n最有\"决心\"的事。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]就是...[wait=0.2]呃。\n彻底放弃。[waitfor=confirm][clear]" + 
                SANS_DIALOG + "[setFace=1]并且...[wait=0.2](打哈欠)[wait=0.2]\n如字面上说的\n去做其他事。[waitfor=confirm][setBody=0][setFace=2][end]"
            );
            bubble.position = new Vector2(380, 120);
            bubble.size = new Vector2(200, 96);
            bubble.dir = UTMX.SpeechBubble.Direction.Left;
            bubble.spikeOffset = -24;
            bubble.inSpike = true;
            bubble.processCmd = (cmd, args) => {
                if (cmd == "setFace") { EnemySans.instance.setFace(args.value); return true; }
                if (cmd == "setBody") { EnemySans.instance.setBody(args.value); return true; }
                if (cmd == "end")
                {
                    this.timerDelta = 1;
                }
                return false;
            };
        }

        if (this.timer == 4300)
        {
            EnemySans.instance.setFace(9)
        }
        if (this.timer == 4600)
        {
            EnemySans.instance.setFace(12)
        }
        if (this.timer == 4900)
        {
            EnemySans.instance.setFace(13)
        }
        if (this.timer == 5200)
        {
            EnemySans.instance.setFace(14)
        }
        if (this.timer == 5500)
        {
            EnemySans.instance.setFace(4)
        }
    }



    createPair(side = 0)
    {
        const arena = UTMX.battle.arena.getMainArena();
        const arenaHeight = arena.size.y;
        const arenaRight = arena.position.x + arena.size.x * 0.5;
        const arenaTop = arena.position.y - arenaHeight + 2;
        const arenaBottom = arena.position.y - 3;
        const arenaCenterY = arena.position.y - arenaHeight * 0.5;

        const groupCount = 3;
        const spacing = 20;
        const spawnPadding = 22;
        const bottomOffsetX = 100;
        const speedX = -20;
        const lifeTime = 120;
        const isTopGroup = (side == 0);

        const startX = arenaRight + spawnPadding + (isTopGroup ? 0 : bottomOffsetX);

        for (let i = 0; i < groupCount; i++)
        {
            const x = startX + i * spacing;
            let bone = Bone.create(1, lifeTime);
            bone.useMask = true;
            bone.speed = new Vector2(speedX, 0);
            bone.setVertexMode(
                new Vector2(x, isTopGroup ? arenaTop : arenaCenterY),
                new Vector2(x, isTopGroup ? arenaCenterY : arenaBottom)
            );
        }
    }

    spawnSideWall(gapSize, gapPercent)
    {
        const edgePadding = 18;
        const speed = 20;

        const arena = UTMX.battle.arena.getMainArena();
        const arenaWidth = arena.size.x;
        const arenaHeight = arena.size.y;
        const arenaLeft = arena.position.x - arenaWidth * 0.5;
        const arenaRight = arena.position.x + arenaWidth * 0.5;
        const arenaTop = arena.position.y - arenaHeight + 2;
        const arenaBottom = arena.position.y - 3;

        const clampedPercent = Math.max(0, Math.min(1, gapPercent));
        const clampedGapSize = Math.max(2, Math.min(arenaHeight - 4, gapSize));
        const halfGap = clampedGapSize * 0.5;
        const gapCenterY = arenaTop + arenaHeight * clampedPercent;
        const gapTopY = gapCenterY - halfGap;
        const gapBottomY = gapCenterY + halfGap;

        const spawnX = arenaRight + edgePadding;
        const speedX = -speed;
        const lifeDistance = spawnX - (arenaLeft - edgePadding);
        const lifeTime = Math.ceil(lifeDistance / speed) + 3;

        let topBone = Bone.create(1, lifeTime);
        topBone.useMask = true;
        topBone.speed = new Vector2(speedX, 0);
        topBone.setVertexMode(
            new Vector2(spawnX, arenaTop),
            new Vector2(spawnX, gapTopY)
        );

        let bottomBone = Bone.create(1, lifeTime);
        bottomBone.useMask = true;
        bottomBone.speed = new Vector2(speedX, 0);
        bottomBone.setVertexMode(
            new Vector2(spawnX, gapBottomY),
            new Vector2(spawnX, arenaBottom)
        );
    }
    spawnWindmillGbSingle()
    {
        const center = new Vector2(320, 320);
        const spawnAngleDeg = this.windmillPhaseDeg;
        const targetAngleDeg = spawnAngleDeg + WINDMILL_TARGET_LEAD_DEG;
        const spawnPos = this.polarToWorld(center, WINDMILL_SPAWN_RADIUS, spawnAngleDeg);
        const targetPos = this.polarToWorld(center, WINDMILL_TARGET_RADIUS, targetAngleDeg);
        const targetRot = this.getLookRotation(targetPos, center);

        let gb = GasterBlaster.create(
            spawnPos,
            targetPos,
            targetRot-180,
            targetRot,
            WINDMILL_GB_SCALE_X,
            WINDMILL_GB_SCALE_Y,
            WINDMILL_GB_HOLD_FIRE
        );
        gb.shootDelay = WINDMILL_GB_SHOOT_DELAY;
        gb.introSpeed = WINDMILL_GB_INTRO_SPEED;
        gb.holdFire = WINDMILL_GB_HOLD_FIRE;
        this.windmillGbs.push(gb);
    }

    clearWindmillGbs()
    {
        for (let i = 0; i < this.windmillGbs.length; i++)
        {
            const gb = this.windmillGbs[i];
            if (gb != null) gb.destroy();
        }
        this.windmillGbs = [];
    }

    polarToWorld(center, radius, angleDeg)
    {
        const rad = angleDeg * Math.PI / 180;
        return new Vector2(
            center.x + Math.cos(rad) * radius,
            center.y + Math.sin(rad) * radius
        );
    }

    getLookRotation(fromPos, toPos)
    {
        const dx = toPos.x - fromPos.x;
        const dy = toPos.y - fromPos.y;
        return Math.atan2(dy, dx) * 180 / Math.PI - 90;
    }

    createWarning(dir, frames = WARNING_FRAMES)
    {
        const arena = UTMX.battle.arena.getMainArena();
        const left = arena.position.x - arena.size.x * 0.5;
        const right = arena.position.x + arena.size.x * 0.5;
        const top = arena.position.y - arena.size.y;
        const bottom = arena.position.y;
        const centerY = (top + bottom) * 0.5;
        const warnDepthY = Math.max(0, Math.min(WALL_DEPTH, arena.size.y));
        const warnDepthX = Math.max(0, Math.min(WALL_DEPTH, arena.size.x));

        let from;
        let to;
        switch (dir)
        {
            case "down":
                from = new Vector2(left, bottom - warnDepthY);
                to = new Vector2(right, bottom);
                break;
            case "up":
                from = new Vector2(left, top);
                to = new Vector2(right, top + warnDepthY);
                break;
            case "right":
                from = new Vector2(right - warnDepthX, top);
                to = new Vector2(right, bottom);
                break;
            case "left":
                from = new Vector2(left, top);
                to = new Vector2(left + warnDepthX, bottom);
                break;
            default:
                from = new Vector2(left, centerY - warnDepthY * 0.5);
                to = new Vector2(right, centerY + warnDepthY * 0.5);
                break;
        }
        return WarningBox.create(from, to, frames);
    }

    createBoneWall(dir)
    {
        const arena = UTMX.battle.arena.getMainArena();
        const left = arena.position.x - arena.size.x * 0.5;
        const right = arena.position.x + arena.size.x * 0.5;
        const top = arena.position.y - arena.size.y;
        const bottom = arena.position.y;
        const centerX = (left + right) * 0.5;
        const centerY = (top + bottom) * 0.5;
        const wallDepthY = Math.max(0, Math.min(WALL_DEPTH, arena.size.y));
        const wallDepthX = Math.max(0, Math.min(WALL_DEPTH, arena.size.x));
        const wallStartOffset = BONE_WALL_OFFSET;

        let startPos;
        let targetPos;
        let exitPos;
        let rotation = 0;

        switch (dir)
        {
            case "down":
                startPos = new Vector2(centerX, bottom + wallStartOffset);
                targetPos = new Vector2(centerX, bottom - wallDepthY + BONE_WALL_HALF_THICKNESS);
                exitPos = new Vector2(centerX, bottom + wallStartOffset);
                break;
            case "up":
                startPos = new Vector2(centerX, top - wallStartOffset);
                targetPos = new Vector2(centerX, top + wallDepthY - BONE_WALL_HALF_THICKNESS);
                exitPos = new Vector2(centerX, top - wallStartOffset);
                break;
            case "right":
                startPos = new Vector2(right + wallStartOffset, centerY);
                targetPos = new Vector2(right - wallDepthX + BONE_WALL_HALF_THICKNESS, centerY);
                exitPos = new Vector2(right + wallStartOffset, centerY);
                rotation = 90;
                break;
            case "left":
                startPos = new Vector2(left - wallStartOffset, centerY);
                targetPos = new Vector2(left + wallDepthX - BONE_WALL_HALF_THICKNESS, centerY);
                exitPos = new Vector2(left - wallStartOffset, centerY);
                rotation = 90;
                break;
            default:
                return;
        }

        let wall = UTMX.BattleProjectile.new();
        wall.useMask = true;
        wall.textures = "textures/sans/attack/spr_s_bonestab_v_wide_0.png";
        wall.rotation = rotation;
        wall.position = startPos;

        const wallState = {
            projectile: wall,
            exitPos: exitPos,
            timer: 0,
            exitStarted: false,
            tween: null
        };

        wallState.tween = UTMX.tween.createTween();
        wallState.tween.addTweenProperty(wallState.projectile, "position", targetPos, 0.08);
        this.boneWalls.push(wallState);
        UTMX.audio.playSound("audios/sfx/snd_spearrise.wav");
    }

    updateBoneWall()
    {
        if (this.boneWalls.length == 0) return;

        for (let i = this.boneWalls.length - 1; i >= 0; i--)
        {
            const wall = this.boneWalls[i];
            wall.timer += 1;

            if (!wall.exitStarted && wall.timer >= BONE_WALL_HOLD_FRAMES)
            {
                wall.exitStarted = true;
                this.killBoneWallTween(wall);
                wall.tween = UTMX.tween.createTween();
                wall.tween.addTweenProperty(wall.projectile, "position", wall.exitPos, 0.08);
            }

            if (wall.exitStarted && wall.timer >= BONE_WALL_HOLD_FRAMES + BONE_WALL_RETRACT_FRAMES)
            {
                this.killBoneWallTween(wall);
                if (wall.projectile != null)
                {
                    wall.projectile.destroy();
                    wall.projectile = null;
                }
                this.boneWalls.splice(i, 1);
            }
        }
    }

    clearBoneWall()
    {
        for (let i = 0; i < this.boneWalls.length; i++)
        {
            const wall = this.boneWalls[i];
            this.killBoneWallTween(wall);
            if (wall.projectile != null)
            {
                wall.projectile.destroy();
                wall.projectile = null;
            }
        }
        this.boneWalls = [];
    }

    killBoneWallTween(wall)
    {
        if (wall == null) return;
        if (wall.tween != null)
        {
            wall.tween.kill();
            wall.tween = null;
        }
    }

    slam(dir, keep=100)
    {
        switch (dir)
        {
            case "down":
                UTMX.battle.soul.rotation = 0;
                break;
            case "right":
                UTMX.battle.soul.rotation = -90;
                break;
            case "up":
                UTMX.battle.soul.rotation = 180;
                break;
            case "left":
                UTMX.battle.soul.rotation = 90;
                break;
        }
        blueSoulController.slam();
        EnemySans.instance.setSlam(dir, keep);
    }
}

