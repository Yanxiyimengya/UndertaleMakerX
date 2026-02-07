import { __Color } from "__UTMX";
import { __Vector2, __Vector3, __Vector4 } from "res://scripts/js/wrappers/utmx_vector.weapper.js";

import { UtmxDebugger } from "res://scripts/js/wrappers/utmx_debugger.wrapper.js";
import { UtmxAudioPlayer } from "res://scripts/js/wrappers/utmx_audio_player.wrapper.js";
import { UtmxInput } from "res://scripts/js/wrappers/utmx_input.wrapper.js";
import { UtmxBaseItem } from "res://scripts/js/wrappers/utmx_item.wrapper.js";
import { UtmxBaseEnemy } from "res://scripts/js/wrappers/utmx_enemy.wrapper.js";
import { UtmxBaseEncounter } from "res://scripts/js/wrappers/utmx_encounter.wrapper.js";
import { UtmxBattleTurn } from "res://scripts/js/wrappers/utmx_battle_turn.wrapper.js";
import { UtmxPlayerDataManager } from "res://scripts/js/wrappers/utmx_player.wrapper.js";
import { UtmxBattleManager } from "res://scripts/js/wrappers/utmx_battle_manager.wrapper.js";
import { UtmxGameRegisterDB } from "res://scripts/js/wrappers/utmx_game_register_db.wrapper.js";
import { UtmxGameManager } from "res://scripts/js/wrappers/utmx_game_manager.wrapper.js";
import { UtmxSceneManager } from "res://scripts/js/wrappers/utmx_scene_manager.weapper.js";
import { UtmxBattleProjectile } from "res://scripts/js/wrappers/utmx_battle_projectile.wrapper.js";
import { UtmxGameSprite } from "res://scripts/js/wrappers/utmx_game_sprite.wrapper.js";
import { UtmxDrawableObject } from "res://scripts/js/wrappers/utmx_drawable_object.wrapper.js";

class UTMX {
	static debug = UtmxDebugger;
	static audio = UtmxAudioPlayer;
	static input = UtmxInput;
	static player = UtmxPlayerDataManager;
	static battle = UtmxBattleManager;
	static registerDB = UtmxGameRegisterDB;
	static game = UtmxGameManager;
	static scene = UtmxSceneManager;

	static Item = UtmxBaseItem;
	static Enemy = UtmxBaseEnemy;
	static Encounter = UtmxBaseEncounter;
	static BattleTurn = UtmxBattleTurn;
	static BattleProjectile = UtmxBattleProjectile;
	static Sprite = UtmxGameSprite;
	static DrawableObject = UtmxDrawableObject;
}

export {
	UTMX,
	__Vector2 as Vector2,
	__Vector3 as Vector3,
	__Vector4 as Vector4,
	__Color as Color,
};
