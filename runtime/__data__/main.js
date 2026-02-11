import { UTMX, Vector2 } from "UTMX";
import MyDO from "js/test_js_drawable_object.js"

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
		

		UTMX.debug.print("Game Started - from JS Main");

		UTMX.scene.addSingleton("DrawO", MyDO.new());
		UTMX.battle.startEncounter("Test Encounter from JS");
	}
	
	onGameEnd()
	{
		UTMX.debug.print("Game Ended - from JS Main");
	}
}
