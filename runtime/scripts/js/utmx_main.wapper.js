import {UtmxDebugger} from "res://scripts/js/wappers/utmx_debugger.wapper.js";
import {UtmxAudioPlayer} from "res://scripts/js/wappers/utmx_audio_player.wapper.js";
import {UtmxInput} from "res://scripts/js/wappers/utmx_input.wapper.js";
import {UtmxBaseItem} from "res://scripts/js/wappers/utmx_item.wapper.js";
import {UtmxBaseEnemy} from "res://scripts/js/wappers/utmx_enemy.wapper.js";
import {UtmxBaseEncounter} from "res://scripts/js/wappers/utmx_encounter.wapper.js";

class UTMX {
	static debug = UtmxDebugger;
	static audio = UtmxAudioPlayer;
	static input = UtmxInput;

	static Item = UtmxBaseItem;
	static Enemy = UtmxBaseEnemy;
	static Encounter = UtmxBaseEncounter;
}

// 第二步：导出已定义的UTMX类 + Vector2（规范ES6命名导出）
export { UTMX };