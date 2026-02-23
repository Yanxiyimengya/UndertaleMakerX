import { UTMX , Vector2 } from "UTMX";
import EnemySans from "enemies/sans.js";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import WarningBox from "libraries/warning-box/warning-box.js";

var blueSoulController = new BlueSoulController();

const SLAM_INTERVAL = 55;
const WARNING_FRAMES = 7;
const POST_SLAM_TO_WALL_FRAMES = 25;
const BONE_WALL_HOLD_FRAMES = 15;
const BONE_WALL_RETRACT_FRAMES = 20;
const BONE_WALL_OFFSET = 65;
const WALL_DEPTH = 25;
const BONE_WALL_HALF_THICKNESS = 50;

export default class GravityControlTurn2 extends UTMX.BattleTurn
{
    timer = -1;
    slamTimer = 0;
    pendingSlamDir = null;
    pendingSlamTimer = 0;
    pendingWallDir = null;
    pendingWallTimer = 0;
    boneWall = null;
    boneWallTween = null;
    boneWallDir = "down";
    boneWallTimer = 0;
    boneWallExitStarted = false;
    
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(150, 150);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 9999999;
    }
    
    onTurnStart()
    {
        blueSoulController.enabled = true;
        this.slamTimer = 0;
        this.pendingSlamDir = null;
        this.pendingSlamTimer = 0;
        this.pendingWallDir = null;
        this.pendingWallTimer = 0;
        this.clearBoneWall();
    }
    
    onTurnEnd()
    {
        this.clearBoneWall();
    }
    
    onTurnUpdate(delta)
    {
        this.timer += 1;
        blueSoulController.update(delta);

        if (this.slamTimer > 0) {
            this.slamTimer -= 1;
        }

        if (this.slamTimer <= 0)
        {
            const dir = this.randomSlamDir();
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

        this.updateBoneWall();

        if (this.timer >= 480)
        {
            this.end();
        }
    }

    randomSlamDir()
    {
        return ["down", "right", "up", "left"][Math.floor(Math.random() * 4)];
    }

    createWarning(dir)
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
        WarningBox.create(from, to, WARNING_FRAMES);
    }

    createBoneWall(dir)
    {
        this.clearBoneWall();

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

        this.boneWall = UTMX.BattleProjectile.new();
        this.boneWall.useMask = true;
        this.boneWall.textures = "textures/sans/attack/spr_s_bonestab_v_wide_0.png";
        this.boneWall.rotation = rotation;
        this.boneWall.position = startPos;
        this.boneWall.targetPos = targetPos;
        this.boneWall.exitPos = exitPos;
        this.boneWallDir = dir;
        this.boneWallTimer = 0;
        this.boneWallExitStarted = false;

        this.killBoneWallTween();
        this.boneWallTween = UTMX.tween.createTween();
        this.boneWallTween.addTweenProperty(this.boneWall, "position", targetPos, 0.08);
        UTMX.audio.playSound("audios/sfx/snd_spearrise.wav");
    }

    updateBoneWall()
    {
        if (this.boneWall == null) return;

        this.boneWallTimer += 1;

        if (!this.boneWallExitStarted && this.boneWallTimer >= BONE_WALL_HOLD_FRAMES)
        {
            this.boneWallExitStarted = true;
            this.killBoneWallTween();
            this.boneWallTween = UTMX.tween.createTween();
            this.boneWallTween.addTweenProperty(this.boneWall, "position", this.boneWall.exitPos, 0.08);
        }

        if (this.boneWallExitStarted && this.boneWallTimer >= BONE_WALL_HOLD_FRAMES + BONE_WALL_RETRACT_FRAMES)
        {
            this.clearBoneWall();
        }
    }

    clearBoneWall()
    {
        this.killBoneWallTween();
        if (this.boneWall != null)
        {
            this.boneWall.destroy();
            this.boneWall = null;
        }
        this.boneWallTimer = 0;
        this.boneWallExitStarted = false;
    }

    killBoneWallTween()
    {
        if (this.boneWallTween != null)
        {
            this.boneWallTween.kill();
            this.boneWallTween = null;
        }
    }

    slam(dir)
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
        EnemySans.instance.setSlam(dir, 100);
    }
}
