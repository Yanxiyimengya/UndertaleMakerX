import { UTMX } from "UTMX";
export default class Main
{
	constructor()
    {
        UTMX.registerDb.registerEnemy("DUMMY", "js/enemies/dummy.js");
        UTMX.registerDb.registerEncounter("DUMMY_ENCOUNTER", "js/encounters/dummy_encounter.js");
	}
	
	onGameStart()
	{
        UTMX.battle.startEncounter("DUMMY_ENCOUNTER");
        UTMX.player.hp = 20;
        UTMX.player.maxHp = 20;
        UTMX.player.lv = 1;
	}
	
	onGameEnd()
	{
	}
}
