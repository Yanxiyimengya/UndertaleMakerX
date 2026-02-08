import { UTMX, Vector2 } from "UTMX";

export default class Main
{
	onGameStart()
	{
		UTMX.debug.print(new Vector2().copy(UTMX.input.getViewportMousePosition()));
		UTMX.registerDB.registerItem("Test Item from JS", "js/test_js_item");

		UTMX.player.inventory.addItem("Test Item from JS");
		UTMX.player.inventory.addItem("Test Item from JS");
		UTMX.player.inventory.addItem("Test Item from JS");
		UTMX.player.inventory.addItem("Test Item from JS");

		UTMX.registerDB.registerEnemy("MyEnemy", "js/test_js_enemy");
		UTMX.registerDB.registerEncounter("Test Encounter from JS", "js/test_js_encounter");
		
		UTMX.battle.startEncounter("Test Encounter from JS");

		UTMX.debug.print("Game Started - from JS Main");
	}
	
	onGameEnd()
	{
		UTMX.debug.print("Game Ended - from JS Main");
	}
}
