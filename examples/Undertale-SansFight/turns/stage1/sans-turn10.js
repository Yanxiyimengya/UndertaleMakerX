import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import Bone from "libraries/bone/bone.js";

export default class SansTurn10 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;

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

        if (this.timer > 30 && this.timer < 450) {
            if (this.timer % 55 == 1) {
                this.spawnBottomShortBone();

                this.spawnTopLongBone();
            }
        }

        if (this.timer == 620) {
            this.end();
        }
    }

    spawnBottomShortBone() {
        let bone = Bone.create(11, 320);
        bone.useMask = true;
        bone.position = new Vector2(100, 366+8);
        bone.speed = new Vector2(1.5, 0);
    }

    spawnTopLongBone() {
        let bone = Bone.create(55, 320);
        bone.useMask = true;
        bone.position = new Vector2(540, 310);
        bone.speed = new Vector2(-1.5, 0);
    }

    onTurnEnd() {
    }
}
