import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn3 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;
    gapPos = 0.5;

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

        if (this.timer < 440) {
            if (this.timer % 90 == 1) {
                this.gapPos = 0.45 + Math.random() * 0.4; 
                this.createWall(this.gapPos);
            }
        }

        if (this.timer == 500) {
            this.end();
        }
    }

    createWall(posPercent) {
        const arenaHeight = 130;
        const arenaCenterY = 320;
        const gapSize = 13;
        
        const arenaTop = arenaCenterY - (arenaHeight / 2);
        const arenaBottom = arenaCenterY + (arenaHeight / 2);

        const margin = 0;
        const minGapY = arenaTop + (gapSize / 2) + margin;
        const maxGapY = arenaBottom - (gapSize / 2) - margin;
        
        const sharedGapY = minGapY + (maxGapY - minGapY) * posPercent;

        const rightSpawnX = 320 + 250;
        const leftSpawnX = 320 - 250;

        const spawnBonesAt = (x, speedX) => {
            const visualTopHeight = (sharedGapY - gapSize / 2) - arenaTop;
            const visualBottomHeight = arenaBottom - (sharedGapY + gapSize / 2);

            let tLen = (visualTopHeight / 2);
            if (tLen < 0) tLen = 0;
            let topBone = Bone.create(tLen, -1);
            topBone.useMask = true;
            topBone.position = new Vector2(x, arenaTop + (visualTopHeight / 2));
            topBone.speed = new Vector2(speedX, 0);

            let bLen = (visualBottomHeight / 2);
            if (bLen < 0) bLen = 0;
            let bottomBone = Bone.create(bLen, -1);
            bottomBone.useMask = true;
            bottomBone.position = new Vector2(x, arenaBottom - (visualBottomHeight / 2));
            bottomBone.speed = new Vector2(speedX, 0);
        };

        spawnBonesAt(rightSpawnX, -3);
        spawnBonesAt(leftSpawnX, 3);
    }
}