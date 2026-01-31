import {UtmxDebugger} from "res://scripts/js/wappers/utmx_debugger.wapper.js";
import {UtmxAudioPlayer} from "res://scripts/js/wappers/utmx_audio_player.wapper.js";
import {UtmxInput} from "res://scripts/js/wappers/utmx_input.wapper.js";
import {UtmxBaseItem} from "res://scripts/js/wappers/utmx_item.wapper.js";
import {UtmxBaseEnemy} from "res://scripts/js/wappers/utmx_enemy.wapper.js";
import {UtmxBaseEncounter} from "res://scripts/js/wappers/utmx_encounter.wapper.js";
import {UtmxBattleTurn} from "res://scripts/js/wappers/utmx_battle_turn.wapper.js";
import {__Vector2} from "__UTMX";

class UTMX {
	static debug = UtmxDebugger;
	static audio = UtmxAudioPlayer;
	static input = UtmxInput;

	static Item = UtmxBaseItem;
	static Enemy = UtmxBaseEnemy;
	static Encounter = UtmxBaseEncounter;
	static BattleTurn = UtmxBattleTurn;
}

export { UTMX, __Vector2 as Vector2};