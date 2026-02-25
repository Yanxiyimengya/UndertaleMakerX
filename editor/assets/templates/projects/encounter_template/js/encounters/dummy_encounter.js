import { UTMX , Vector2 } from "UTMX";

const EXP = 2;
const GOLD = 2;

export default class DummyEncounter extends UTMX.Encounter
{
    constructor()
    {
        super();
        this.enemies = ["DUMMY"];
        this.encounterText = "* You encountered the Dummy.";
        this.freeText = "* Escaped...";
        this.endText = `* YOU WON!\n[wait=0.2]* You earned ${EXP} EXP and ${GOLD} gold.`;
    }
    
    onBattleStart()
    { 
        UTMX.audio.playBgm("DUMMY", "audios/bgm/mus_prebattle1.ogg", true);
        
        this.backgroundSprite = UTMX.Sprite.new();
        this.backgroundSprite.textures = "textures/background/spr_battlebg_0.png";
        this.backgroundSprite.globalPosition = new Vector2(320, 130);
    }
    
    onBattleEnd()
    {
        if (UTMX.audio.isBgmValid("DUMMY"))
        {
            UTMX.audio.stopBgm("DUMMY");
        }
    }
}