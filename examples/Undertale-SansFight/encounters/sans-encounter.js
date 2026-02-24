import { UTMX } from "UTMX";
import EnemySans from "enemies/sans.js";

export default class SansEncounter extends UTMX.Encounter 
{
    constructor() 
    {
        super();
        this.enemies = ["SANS"];
        this.encounterText = "[size=24]* 你感觉自己接下来得\n  吃苦头了。";
        this.encounterBattleFirstState = UTMX.battle.BattleStatus.ENEMY_DIALOGUE;
        UTMX.player.invincibleTime = 0.01;
        UTMX.player.lv = 19;
        UTMX.player.hp = 92;
        UTMX.player.maxHp = 92;
        this.canFree = false;
    }
    
    onGameover()
    {
        if (EnemySans.instance.canSpare)
        {
            this.deathText = `[voice="audios/voices/snd_txtsans.wav"][speed=0.02]花花花花花花花[speed=0.1]式\n吊打！！！[waitfor=confirm][clear]如果我们还是朋友...[waitfor=confirm][clear]你就不要再回来了。`;
            UTMX.audio.playBgm("_GAME_OVER", "audios/bgm/mus_dogsong.ogg", true);
            UTMX.audio.setBgmPitch("_GAME_OVER", 1.33);
            UTMX.audio.setBgmPaused("_GAME_OVER", true);
        }
        else 
        {
            this.deathText = `现在还不能放弃[waitfor=confirm]\n${UTMX.player.name} [wait=0.2]保持你的决心。`;
        }
    }

    onPlayerTurn() {
        if (EnemySans.instance.canSpare)
        {
            this.encounterText = "[size=24]* Sans正在饶恕你。";
        }
        else 
        {
            if (EnemySans.instance.stage == 2)
            {
                if (EnemySans.instance.attackCount > 8)
                {
                    this.encounterText = "[size=24]* Sans 正准备用它的特殊攻击。";
                }
                else if (EnemySans.instance.attackCount > 8)
                {
                    this.encounterText = "[size=24]* Sans 正在准备着什么。";
                }
                else if (EnemySans.instance.attackCount > 7)
                {
                    this.encounterText = "[size=24]* Sans 现在看上去真的\n  很疲惫了。";
                }
                else 
                {
                    this.encounterText = "[size=24]* 好像逆转的时刻到来了。";
                }
            }
            else 
            {
                this.encounterText = "[size=24]* 你感觉自己接下来得\n  吃苦头了。";
            }
        }
    }
}