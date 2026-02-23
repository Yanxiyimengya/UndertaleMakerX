import { UTMX, Vector2 } from "UTMX";
import BlueSoulController from "libraries/blue-soul/blue-soul.js";
import GasterBlaster from "libraries/gaster-blaster/gaster-blaster.js";

export default class SansTurn8 extends UTMX.BattleTurn {
    blueSoulController = new BlueSoulController();
    timer = 0;
    laneY = [278, 323, 366];
    lastLaneY = null;
    lastLaneX = null;

    onTurnInit() {
        this.arenaInitSize = new Vector2(360, 130);
        this.soulInitPosition = new Vector2(0, 57);
        this.turnTime = 999999.0;
        this.blueSoulController.enabled = true;
    }

    onTurnStart() {
        this.blueSoulController.enabled = true;;
    }

    onTurnUpdate(delta) {
        this.blueSoulController.update(delta);
        this.timer += 1;
        
        if (this.timer % 75 == 1)
        {
            let platform;
            
            platform = this.blueSoulController.createPlatform(30);
            platform.position = new Vector2(-120, 300);
            platform.speed = new Vector2(2.5, 0);
            
            platform = this.blueSoulController.createPlatform(30);
            platform.position = new Vector2(640+120,345);
            platform.speed = new Vector2(-2.5, 0);
        }

        if (this.timer > 30 && this.timer < 620 && this.timer % 55 == 1)
        {
            let lane, dir;
            do {
                lane = this.getRandomLaneY();
                dir = Math.random() < 0.5;
            } while (lane == this.lastLaneY && dir == this.lastLaneX);

            this.lastLaneY = lane;
            this.lastLaneX = dir;

            if (dir) {
                let gbLeft = GasterBlaster.create(
                    new Vector2(-50, 80),
                    new Vector2(75, lane),
                    0,
                    -90,
                    0.5,
                    1
                );
                gbLeft.shootDelay = 50;
                gbLeft.introSpeed = 0.2;
            } else {
                let gbRight = GasterBlaster.create(
                    new Vector2(690, 80),
                    new Vector2(570, lane),
                    0,
                    90,
                    0.5,
                    1
                );
                gbRight.shootDelay = 50;
                gbRight.introSpeed = 0.2;
            }
        }
        
        if (this.timer == 680)
        {
            this.end();
        }
    }

    getRandomLaneY() {
        return this.laneY[Math.floor(Math.random() * this.laneY.length)];
    }
    
    onTurnEnd()
    {
    }
}
