import { UtmxDebugger } from "res://scripts/js/wrappers/utmx-debugger.wrapper.js";
import { UtmxAudioPlayer } from "res://scripts/js/wrappers/utmx-audio-player.wrapper.js";
import { UtmxInput } from "res://scripts/js/wrappers/utmx-input.wrapper.js";
import { UtmxPlayerDataManager } from "res://scripts/js/wrappers/utmx-player.wrapper.js";
import { UtmxBattleManager } from "res://scripts/js/wrappers/utmx-battle-manager.wrapper.js";
import { UtmxGameRegisterDB } from "res://scripts/js/wrappers/utmx-game-register-db.wrapper.js";
import { UtmxGameManager } from "res://scripts/js/wrappers/utmx-game-manager.wrapper.js";
import { UtmxSceneManager } from "res://scripts/js/wrappers/utmx-scene-manager.weapper.js";

import { __Color } from "res://scripts/js/wrappers/types/utmx-color.weapper.js";
import { __Vector2, __Vector3, __Vector4 } from "res://scripts/js/wrappers/types/utmx-vector.weapper.js";
import { UtmxBattleProjectile } from "res://scripts/js/wrappers/types/utmx-battle-projectile.wrapper.js";
import { UtmxGameSprite } from "res://scripts/js/wrappers/types/utmx-game-sprite.wrapper.js";
import { UtmxDrawableObject } from "res://scripts/js/wrappers/types/utmx-drawable-object.wrapper.js";
import { UtmxBaseItem } from "res://scripts/js/wrappers/types/utmx-item.wrapper.js";
import { UtmxBaseEnemy } from "res://scripts/js/wrappers/types/utmx-enemy.wrapper.js";
import { UtmxBaseEncounter } from "res://scripts/js/wrappers/types/utmx-encounter.wrapper.js";
import { UtmxBattleTurn } from "res://scripts/js/wrappers/types/utmx-battle-turn.wrapper.js";

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
