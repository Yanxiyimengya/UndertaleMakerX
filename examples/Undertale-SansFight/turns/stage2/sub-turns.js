import { UTMX , Vector2 } from "UTMX";
import Bone from "libraries/bone/bone.js";
import EnemySans from "enemies/sans.js";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import GasterBlaster from "libraries/gaster-blaster/gaster-blaster.js";

var blueSoulController = new BlueSoulController();

var autoDestroyList = [];
function createWall(posPercent) {
    const arena = UTMX.battle.arena.getMainArena();
    const arenaWidth = arena.size.x;
    const arenaHeight = arena.size.y;
    const arenaCenterX = arena.position.x;
    const arenaCenterY = arena.position.y - arenaHeight * 0.5;
    const gapSize = 13;
    
    const arenaTop = arenaCenterY - (arenaHeight / 2);
    const arenaBottom = arenaCenterY + (arenaHeight / 2);

    const margin = 0;
    const minGapY = arenaTop + (gapSize / 2) + margin;
    const maxGapY = arenaBottom - (gapSize / 2) - margin;
    
    const sharedGapY = minGapY + (maxGapY - minGapY) * posPercent;

    const rightSpawnX = arenaCenterX + (arenaWidth * 0.5) + 15;
    const leftSpawnX = arenaCenterX - (arenaWidth * 0.5) - 15;

    const spawnBonesAt = (x, speedX) => {
        const visualTopHeight = (sharedGapY - gapSize / 2) - arenaTop;
        const visualBottomHeight = arenaBottom - (sharedGapY + gapSize / 2);

        let tLen = (visualTopHeight / 2);
        if (tLen < 0) tLen = 0;
        let topBone = Bone.create(tLen, -1);
        topBone.useMask = true;
        topBone.position = new Vector2(x, arenaTop + (visualTopHeight / 2));
        topBone.speed = new Vector2(speedX, 0);
        autoDestroyList.push(topBone);

        let bLen = (visualBottomHeight / 2);
        if (bLen < 0) bLen = 0;
        let bottomBone = Bone.create(bLen, -1);
        bottomBone.useMask = true;
        bottomBone.position = new Vector2(x, arenaBottom - (visualBottomHeight / 2));
        bottomBone.speed = new Vector2(speedX, 0);
        autoDestroyList.push(bottomBone);
    };
    spawnBonesAt(rightSpawnX, -3);
    spawnBonesAt(leftSpawnX, 3);
}
function createBone(len, pos, speed)
{
    let bone = Bone.create(len);
    bone.useMask = true;
    bone.position = pos;
    bone.speed = new Vector2(speed, 0);
    autoDestroyList.push(bone);
    return bone;
}
function clearProjectiles()
{
    for (let i = 0; i < autoDestroyList.length; i ++) 
        if (autoDestroyList[i] != null) autoDestroyList[i].destroy();
    autoDestroyList = [];
}


class SubTurn
{
    endded = false;
    timer = 0;
    start() {
        EnemySans.instance.position = new Vector2( (Math.random()-0.5)*300 , EnemySans.instance.position.y);
    };
    update(delta)
    {
        this.timer += 1;
        blueSoulController.update(delta);
    }
    end() {
        clearProjectiles();
        this.endded = true;
    };
}

// 固定跳跃骨头 1L 4S
class JumpTurn01 extends SubTurn 
{
    spd = 4.5;
    start()
    {
        super.start();
        blueSoulController.enabled = true;
        UTMX.battle.soul.position = new Vector2(320, 375);
        UTMX.battle.arena.getMainArena().size = new Vector2(400, 120);
    }
    update(delta)
    {
        super.update(delta);
        if (this.timer == 1)
        {
            createBone(55, new Vector2(600, 331), -this.spd);
            for (let i = 1; i < 5; i ++) createBone(23, new Vector2(600-18*i, 363), -this.spd);
            createBone(55, new Vector2(40, 331), +this.spd);
            for (let i = 1; i < 5; i ++) createBone(23, new Vector2(40+18*i, 363), +this.spd);
        }
        if (this.timer == 55)
        {
            this.end();
        }
    }
}


// 随机的跳跃骨头
class JumpTurn02 extends SubTurn 
{
    start()
    {
        super.start();
        blueSoulController.enabled = true;
        UTMX.battle.soul.position = new Vector2(320, 375);
        UTMX.battle.arena.getMainArena().size = new Vector2(360, 120);
    }
    update(delta)
    {
        super.update(delta);
        if (this.timer % 50 == 1)
        {
            createWall( 0.6 + Math.random() * 0.2 );
        }
        if (this.timer == 130)
        {
            this.end();
        }
    }
}

// 蓝骨+ 1L 1S 跳跃骨头
class JumpTurn03 extends SubTurn 
{
    start()
    {
        super.start();
        blueSoulController.enabled = true;
        UTMX.battle.soul.position = new Vector2(320, 375);
        UTMX.battle.arena.getMainArena().size = new Vector2(360, 120);
    }
    update(delta)
    {
        super.update(delta);
        if (this.timer == 1)
        {
            let b = createBone(55, new Vector2(490, 331), -4.5);
            b.setStatus(1);
            b = createBone(10, new Vector2(490+80, 331+45), -4.5);
            b = createBone(55, new Vector2(490+110, 331), -4.5);
            
            b = createBone(55, new Vector2(150, 331), 4.5);
            b.setStatus(1);
            b = createBone(10, new Vector2(150-80, 331+45), 4.5);
            b = createBone(55, new Vector2(150-110, 331), 4.5);
        }
        if (this.timer == 57)
        {
            this.end();
        }
    }
}

// 随机左右跳跃骨头 11-10
class JumpTurn04 extends SubTurn 
{
    dir = 0;
    spd = 7;
    
    start()
    {
        super.start();
        blueSoulController.enabled = true;
        UTMX.battle.arena.getMainArena().size = new Vector2(400, 120);
        
        this.dir = (Math.random() > 0.5) ? 1 : -1;
        UTMX.battle.soul.position = new Vector2(320-(190*this.dir), 375);
        
        let len = 190 * this.dir;
        for (let i = 0; i < 11; i ++)
        {
            let b = createBone(30, new Vector2(320+len, 331+25), -this.spd*this.dir);
            len += 25 * this.dir;
        }
        for (let i = 0; i < 10; i ++)
        {
            let b = createBone(10, new Vector2(320+len, 331+45), -this.spd*this.dir);
            len += 25 * this.dir;
        }
    }
    update(delta)
    {
        super.update(delta);
        if (this.timer == 103)
        {
            this.end();
        }
    }
}

// sin 骨头
class WaveTurn extends SubTurn 
{
    dir = 1;
    spawnInterval = 4;
    spawnEndFrame = 70;
    endFrame = 103;
    gapSize = 20;
    speed = 4.5;
    waveSpeed = 0.10;
    waveRange = 0.16;
    edgePadding = 18;
    seed = 0;

    start()
    {
        super.start();
        blueSoulController.enabled = false;
        this.dir = Math.random() > 0.5 ? 1 : -1;
        UTMX.battle.soul.position = new Vector2(320-(70*this.dir), 320);
        UTMX.battle.arena.getMainArena().size = new Vector2(180, 130);
        this.seed = Math.random() * Math.PI * 2;
    }

    update(delta)
    {
        super.update(delta);

        if (this.timer <= this.spawnEndFrame && this.timer % this.spawnInterval == 1)
        {
            const sinValue = Math.sin(this.seed + this.timer * this.waveSpeed) * this.waveRange;
            const gapPercent = this.dir > 0 ? (0.5 + sinValue) : (0.5 - sinValue);
            this.spawnSideWall(this.dir, gapPercent);
        }

        if (this.timer == this.endFrame)
        {
            this.end();
        }
    }

    spawnSideWall(dir, gapPercent)
    {
        const arena = UTMX.battle.arena.getMainArena();
        const arenaWidth = arena.size.x;
        const arenaHeight = arena.size.y;
        const arenaLeft = arena.position.x - arenaWidth * 0.5;
        const arenaRight = arena.position.x + arenaWidth * 0.5;
        const arenaTop = arena.position.y - arenaHeight + 2;
        const arenaBottom = arena.position.y - 3;

        const clampedPercent = Math.max(0, Math.min(1, gapPercent));
        const gapCenterY = arenaTop + arenaHeight * clampedPercent;
        const gapTopY = gapCenterY - this.gapSize;
        const gapBottomY = gapCenterY + this.gapSize;

        const spawnX = dir > 0 ? (arenaRight + this.edgePadding) : (arenaLeft - this.edgePadding);
        const speedX = dir > 0 ? -this.speed : this.speed;
        const lifeDistance = dir > 0
            ? (spawnX - (arenaLeft - this.edgePadding))
            : ((arenaRight + this.edgePadding) - spawnX);
        const lifeTime = Math.ceil(lifeDistance / this.speed) + 3;

        let topBone = Bone.create(1, lifeTime);
        topBone.useMask = true;
        topBone.speed = new Vector2(speedX, 0);
        topBone.setVertexMode(
            new Vector2(spawnX, arenaTop),
            new Vector2(spawnX, gapTopY)
        );
        autoDestroyList.push(topBone);

        let bottomBone = Bone.create(1, lifeTime);
        bottomBone.useMask = true;
        bottomBone.speed = new Vector2(speedX, 0);
        bottomBone.setVertexMode(
            new Vector2(spawnX, gapBottomY),
            new Vector2(spawnX, arenaBottom)
        );
        autoDestroyList.push(bottomBone);
    }
}


class JumpTurn05 extends SubTurn 
{
    dir = 0; // 1=灵魂初始在左, -1=灵魂初始在右
    pairCount = 10; // 直接 for 循环生成 8 对
    pairSpacing = 75; // 相邻骨头对在出生侧的横向间隔
    edgePadding = -220; // 出生点相对 arena 边缘的外扩距离
    shortSpeed = 2.2; // 下方短骨速度
    longSpeed = 2.2; // 上方长骨速度
    shortHeight = 20; // 下方短骨高度
    laneGap = 3; // 上长骨与下短骨之间的间隙
    endFrame = 110; // 子回合结束帧

    start()
    {
        super.start();
        this.dir = Math.random() > 0.5 ? 1 : -1;
        blueSoulController.enabled = true;
        UTMX.battle.soul.position = new Vector2(320-(170*this.dir), 375);
        UTMX.battle.arena.getMainArena().size = new Vector2(400, 120);
        this.spawnAllPairs();
    }
    update(delta)
    {
        super.update(delta);

        if (this.timer == this.endFrame)
        {
            this.end();
        }
    }

    spawnAllPairs()
    {
        const arena = UTMX.battle.arena.getMainArena();
        const arenaWidth = arena.size.x;
        const arenaHeight = arena.size.y;
        const arenaLeft = arena.position.x - arenaWidth * 0.5;
        const arenaRight = arena.position.x + arenaWidth * 0.5;
        const arenaTop = arena.position.y - arenaHeight;
        const arenaBottom = arena.position.y;

        const shortTopY = arenaBottom - this.shortHeight;
        const longBottomY = Math.max(arenaTop + 8, shortTopY - this.laneGap);

        // 下短骨朝灵魂初始方向，上长骨顺玩家方向
        const shortMoveDir = -this.dir;
        const longMoveDir = this.dir;

        for (let i = 0; i < this.pairCount; i++)
        {
            this.spawnStreamBone(
                shortMoveDir,
                this.shortSpeed,
                shortTopY,
                arenaBottom,
                i,
                arenaLeft,
                arenaRight
            );
            this.spawnStreamBone(
                longMoveDir,
                this.longSpeed,
                arenaTop,
                longBottomY,
                i,
                arenaLeft,
                arenaRight
            );
        }
    }

    spawnStreamBone(moveDir, speed, yTop, yBottom, order, arenaLeft, arenaRight)
    {
        const speedX = moveDir * speed;
        const baseX = moveDir > 0 ? (arenaLeft - this.edgePadding) : (arenaRight + this.edgePadding);
        const spawnX = baseX + (-moveDir) * this.pairSpacing * order;

        let bone = Bone.create(1, 200);
        bone.useMask = true;
        bone.speed = new Vector2(speedX, 0);
        bone.setVertexMode(
            new Vector2(spawnX, yTop),
            new Vector2(spawnX, yBottom)
        );
        autoDestroyList.push(bone);
    }
}

class JumpTurn06 extends SubTurn 
{
    start()
    {
        super.start();
        this.dir = Math.random() > 0.5 ? 1 : -1;
        blueSoulController.enabled = true;
        UTMX.battle.arena.getMainArena().size = new Vector2(400, 120);
        
        let p1 = blueSoulController.createPlatform(22);
        p1.position = new Vector2(320, 330-25);
        autoDestroyList.push(p1);
        let p2 = blueSoulController.createPlatform(22);
        p2.position = new Vector2(320, 330+20);
        autoDestroyList.push(p2);

        UTMX.battle.soul.position = new Vector2(320, 297);
        
        let posX = -193;
        for (let i = 0; i < 27; i ++)
        {
            createBone(20, new Vector2(320+posX, 375), 0);
            posX += 15;
        }
        
        const dis = 320*this.dir;
        createBone(20, new Vector2(320-dis+this.dir*100, 285), 4.3*this.dir);
        createBone(40, new Vector2(320+dis+this.dir*100, 345), -4.3*this.dir);
    }
    
    update(delta)
    {
        super.update(delta);
        if(this.timer == 1)
        {
            UTMX.battle.soul.position = new Vector2(320, 295);
        }
        if(this.timer == 110)
        {
            this.end();
        }
    }
}

class GbTurn extends SubTurn 
{
    start()
    {
        UTMX.battle.arena.getMainArena().size = new Vector2(150, 150);
        UTMX.battle.soul.position = new Vector2(320, 320);
        blueSoulController.enabled = false;
        
        const distance = 130;
        let transform = Math.random() > 0.5 ? [
            [new Vector2(320, 320-distance), 0],
            [new Vector2(320+distance, 320), 90],
            [new Vector2(320, 320+distance), -180],
            [new Vector2(320-distance, 320), -90],
        ] : [
            [new Vector2(320-distance, 320-distance), -45],
            [new Vector2(320+distance, 320-distance), 45],
            [new Vector2(320+distance, 320+distance), 135],
            [new Vector2(320-distance, 320+distance), -135],
        ];
        for (let i = 0; i < transform.length; i ++) 
        {
            let gb =GasterBlaster.create(transform[i][0], transform[i][0], 
                transform[i][1], transform[i][1]);
            gb.shootDelay = 50;
            autoDestroyList.push(gb);
        }
    }
    
    update(delta)
    {
        super.update(delta);
        if (this.timer == 80)
        {
            this.end();
        }
    }
}

export {
    JumpTurn01,
    JumpTurn02,
    JumpTurn03,
    JumpTurn04,

    // 3

    WaveTurn,
    JumpTurn05,
    JumpTurn06,
    GbTurn
};
