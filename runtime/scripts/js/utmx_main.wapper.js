import { __Object, __Vector2, __Vector3, __Vector4 } from "__UTMX";

import { UtmxDebugger } from "res://scripts/js/wrappers/utmx_debugger.wrapper.js";
import { UtmxAudioPlayer } from "res://scripts/js/wrappers/utmx_audio_player.wrapper.js";
import { UtmxInput } from "res://scripts/js/wrappers/utmx_input.wrapper.js";
import { UtmxBaseItem } from "res://scripts/js/wrappers/utmx_item.wrapper.js";
import { UtmxBaseEnemy } from "res://scripts/js/wrappers/utmx_enemy.wrapper.js";
import { UtmxBaseEncounter } from "res://scripts/js/wrappers/utmx_encounter.wrapper.js";
import { UtmxBattleTurn } from "res://scripts/js/wrappers/utmx_battle_turn.wrapper.js";
import { UtmxPlayerDataManager } from "res://scripts/js/wrappers/utmx_player_data_manager.wrapper.js";
import { UtmxBattleManager } from "res://scripts/js/wrappers/utmx_battle_manager.weapper.js";
import { UtmxGameRegisterDB } from "res://scripts/js/wrappers/utmx_game_register_db.wrapper.js";
import { UtmxGameManager } from "res://scripts/js/wrappers/utmx_game_manager.wrapper";
import { UtmxBattleProjectile } from "res://scripts/js/wrappers/utmx_battle_projectile.wrapper.js";

class UTMX {
	static debug = UtmxDebugger;
	static audio = UtmxAudioPlayer;
	static input = UtmxInput;
	static player = UtmxPlayerDataManager;
	static battle = UtmxBattleManager;
	static gameDB = UtmxGameRegisterDB;
	static game = UtmxGameManager;

	static Item = UtmxBaseItem;
	static Enemy = UtmxBaseEnemy;
	static Encounter = UtmxBaseEncounter;
	static BattleTurn = UtmxBattleTurn;
	static BattleProjectile = UtmxBattleProjectile;
}

export {
	UTMX,
	__Vector2 as Vector2,
	__Vector3 as Vector3,
	__Vector4 as Vector4,
	__Object as Object
};