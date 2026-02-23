import { UTMX , Vector2 } from "UTMX";
import EnemySans from "enemies/sans.js";
import BlackScreen from "singalton/black-screen.js";
import { JumpTurn01 , JumpTurn02 , JumpTurn03 , JumpTurn04 } from "turns/stage2/sub-turns.js";

const WAT_TIME = 25;
const RANDOM_TRUNS = [
    JumpTurn01,
    JumpTurn02,
    JumpTurn03,
    JumpTurn04,
    JumpTurn04
];

export default class RandomTurn1 extends UTMX.BattleTurn
{
    timer = -1;
    subTurnList = [];
    currentSubTurn = -1;
    waittingTimer = 0;
    isStart = false;
    
    onTurnInit()
    {
        this.arenaInitSize = new Vector2(380, 130);
        this.soulInitPosition = new Vector2(0, 0);
        this.turnTime = 9999999;
        
        let lastIndex = -1; 
        for (let i = 0; i < 5; i ++)
        {
            let randomIndex;
            do {
                randomIndex = Math.floor(Math.random() * RANDOM_TRUNS.length);
            } while (i > 0 && randomIndex === lastIndex);
            this.subTurnList.push(new RANDOM_TRUNS[randomIndex]());
            lastIndex = randomIndex;
        }
    }
    
    onTurnStart()
    {
        UTMX.battle.soul.moveable = false;
        this.currentSubTurn = 0;
        this.waittingTimer = WAT_TIME;
        this.isStart = false;
        BlackScreen.instance.setVisible(true);
    }
    
    onTurnEnd()
    {
    }
    
    onTurnUpdate(delta)
    {
        if (this.waittingTimer > 0)
        {
            this.waittingTimer -= 1;
            if (this.waittingTimer === 0) 
            {
                if (this.currentSubTurn < this.subTurnList.length)
                {
                    this.subTurnList[this.currentSubTurn].start();
                    BlackScreen.instance.setVisible(false);
                    
                    if (!this.isStart)
                    {
                        this.isStart = true;
                        if (!UTMX.audio.isBgmValid("BGM")) {
                            UTMX.audio.playBgm("BGM", "audios/bgm/mus_zz_megalovania.ogg", true);
                        }
                        UTMX.audio.setBgmPaused("BGM", false);
                    }
                }
                else
                {
                    const arena = UTMX.battle.arena.getMainArena();
                    arena.size = new Vector2(575, 130);
                    this.end();
                    EnemySans.instance.position = new Vector2(0, EnemySans.instance.position.y);
                    BlackScreen.instance.setVisible(false);
                }
            }
        }
        else
        {
            if (this.currentSubTurn >= 0 && this.currentSubTurn < this.subTurnList.length)
            {
                const currentTurn = this.subTurnList[this.currentSubTurn];
                if (currentTurn.endded)
                {
                    this.currentSubTurn += 1;
                    this.waittingTimer = WAT_TIME;
                    BlackScreen.instance.setVisible(true);
                }
                else
                {
                    currentTurn.update(delta);
                }
            }
        }
    }
}