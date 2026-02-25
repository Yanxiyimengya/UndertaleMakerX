import { UTMX } from "UTMX";
import BlackScreen from "singalton/black-screen.js";

export default class Main
{
	constructor() {
        UTMX.registerDb.registerItem("BUTTERSCOTCH", "items/item-pie.js");
        UTMX.registerDb.registerItem("FACE_STEAK", "items/item-face-steak.js");
        UTMX.registerDb.registerItem("LEGENDARY_HERO", "items/item-legendary-hero.js");
        UTMX.registerDb.registerItem("INSTANT_NOODLES", "items/item-instant-noodles.js");
        UTMX.registerDb.registerItem("SNOW_PIECE", "items/item-snow-piece.js");
        
        UTMX.registerDb.registerEnemy("SANS", "enemies/sans.js");
        UTMX.registerDb.registerEncounter("SANS_ENCOUNTER", "encounters/sans-encounter.js");
	} 
	
	onGameStart()
	{
        if (UTMX.scene.getSingleton("BLACK_SCREEN") == null)
        {
            let bs = BlackScreen.new();
            UTMX.scene.addSingleton("BLACK_SCREEN", bs);
        }
        
        for (let i = 0; i < 8 ; i ++)
            UTMX.player.inventory.addItem("BUTTERSCOTCH");
        
        UTMX.player.inventory.setItem("BUTTERSCOTCH", 0);
        UTMX.player.inventory.setItem("FACE_STEAK", 1);
        UTMX.player.inventory.setItem("LEGENDARY_HERO", 2);
        UTMX.player.inventory.setItem("LEGENDARY_HERO", 3);
        UTMX.player.inventory.setItem("LEGENDARY_HERO", 4);
        UTMX.player.inventory.setItem("INSTANT_NOODLES", 5);
        UTMX.player.inventory.setItem("SNOW_PIECE", 6);
        UTMX.player.inventory.setItem("SNOW_PIECE", 7);
        
        
        UTMX.battle.startEncounter("SANS_ENCOUNTER");
	}
	
	onGameEnd()
	{
	}
}