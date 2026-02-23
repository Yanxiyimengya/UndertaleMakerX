import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import GasterBlaster from "libraries/gaster-blaster/gaster-blaster.js";

export default class SansTurn12 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    gasterBlasters = [];
    timer = 0;
    summonStartFrame = 20;
    summonEndFrame = 450;
    summonInterval = 35;

    turnEndFrame = 540;
    nearDistanceMin = 180;
    nearDistanceMax = 220;
    arenaSafeMinX = 40;
    arenaSafeMaxX = 600;
    arenaSafeMinY = 40;
    arenaSafeMaxY = 440;

    onTurnInit() {
        this.arenaInitSize = new Vector2(400, 180);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = false;
    }

    onTurnStart() {
        this.timer = 0;
        this.gasterBlasters = [];
        this.blueSoulController.enabled = false;
    }

    onTurnUpdate(delta) {
        this.blueSoulController.update(delta);
        this.timer += 1;

        const inSummonWindow =
            this.timer >= this.summonStartFrame &&
            this.timer <= this.summonEndFrame &&
            (this.timer - this.summonStartFrame) % this.summonInterval === 0;
        if (inSummonWindow) {
            this.spawnTrackingGasterBlaster();
        }

        if (this.timer >= this.turnEndFrame) {
            this.end();
        }
    }

    spawnTrackingGasterBlaster() {
        const soulPos = UTMX.battle.soul.position;
        const dir = this.pickSpawnDirection(soulPos);
        const nearDistance = this.randomRange(this.nearDistanceMin, this.nearDistanceMax);
        
        const targetPos = new Vector2(
            soulPos.x + dir.x * nearDistance,
            soulPos.y + dir.y * nearDistance
        );
        const spawnPos = new Vector2(
            targetPos.x + dir.x * (nearDistance + 80),
            targetPos.y + dir.y * (nearDistance + 80)
        );

        const targetRot = this.getLookRotation(targetPos, UTMX.battle.soul.position);
        const gb = GasterBlaster.create(
            spawnPos,
            targetPos,
            0,
            targetRot,
            0.5,
            1
        );
        gb.shootDelay = 70;
        gb.introSpeed = 0.2;
        gb.holdFire = 5;
        this.gasterBlasters.push(gb);
    }

    pickSpawnDirection(soulPos) {
        const sampleDistance = this.nearDistanceMax;
        for (let i = 0; i < 12; i++) {
            const angle = Math.random() * Math.PI * 2;
            const dir = new Vector2(Math.cos(angle), Math.sin(angle));
            const samplePos = new Vector2(
                soulPos.x + dir.x * sampleDistance,
                soulPos.y + dir.y * sampleDistance
            );
            if (this.isPositionSafe(samplePos)) {
                return dir;
            }
        }

        const centerOffset = new Vector2(320 - soulPos.x, 320 - soulPos.y);
        const length = Math.sqrt(centerOffset.x * centerOffset.x + centerOffset.y * centerOffset.y);
        if (length > 0.0001) {
            return new Vector2(centerOffset.x / length, centerOffset.y / length);
        }
        return new Vector2(1, 0);
    }

    isPositionSafe(pos) {
        return (
            pos.x >= this.arenaSafeMinX &&
            pos.x <= this.arenaSafeMaxX &&
            pos.y >= this.arenaSafeMinY &&
            pos.y <= this.arenaSafeMaxY
        );
    }

    getLookRotation(fromPos, toPos) {
        const dx = toPos.x - fromPos.x;
        const dy = toPos.y - fromPos.y;
        return Math.atan2(dy, dx) * 180 / Math.PI - 90;
    }

    randomRange(min, max) {
        return min + Math.random() * (max - min);
    }

    onTurnEnd() {
    }
}
