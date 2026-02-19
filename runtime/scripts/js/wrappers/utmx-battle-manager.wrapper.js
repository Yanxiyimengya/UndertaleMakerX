import { __battle_manager, __BattleProjectile, __logger } from "__UTMX";
import { UtmxGameSprite } from "./types/utmx-game-sprite.wrapper.js";
import { UtmxGameObject } from "./types/utmx-game-object.wrapper.js";
import {
	BattleArenaRectangle,
	BattleArenaCircle,
	BattleArenaPolygon,
} from "./types/utmx-battle-arena.wrapper.js";

class BattleSoul extends UtmxGameObject
{
	static get enabledCollision() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.EnabledCollision;
	}
	static set enabledCollision(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.EnabledCollision = value;
	}
	
	static get movable() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.Movable;
	}
	static set movable(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.Movable = value;
	}
	
	static get position() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.Position;
	}
	static set position(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.Position = value;
	}
	static get z() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.ZIndex;
	}
	static set z(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.ZIndex = value;
	}
	static get rotation() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.RotationDegrees;
	}
	static set rotation(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.RotationDegrees = value;
	}
	static get scale() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.Scale;
	}
	static set scale(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.Scale = value;
	}
	static get skew() {
		return __battle_manager.GetBattlePlayerController().PlayerSoul.Skew;
	}
	static set skew(value) {
		__battle_manager.GetBattlePlayerController().PlayerSoul.Skew = value;
	}
	
	static #__sprite = new UtmxGameSprite();
	static get sprite() {
		if (! __battle_manager.IsInBattle()) return null;
		this.#__sprite.__instance = __battle_manager.GetBattlePlayerController().PlayerSoul.Sprite;
		return this.#__sprite;
	}
    set sprite(value) { }

	static tryMoveTo(target)
	{
		__battle_manager.GetBattlePlayerController().PlayerSoul.TryMoveTo(target);
	}
	static isOnArenaFloor()
	{
		return __battle_manager.GetBattlePlayerController().PlayerSoul.IsOnArenaFloor();
	}
	static isOnArenaCeiling()
	{
		return __battle_manager.GetBattlePlayerController().PlayerSoul.IsOnArenaCeiling();
	}
	static isMoving()
	{
		return __battle_manager.GetBattlePlayerController().PlayerSoul.IsMoving;
	}
}
class BattleArenaAccess
{
	static #__mainArena = new BattleArenaRectangle();
	static getMainArena()
	{
		this.#__mainArena.__instance = __battle_manager.GetBattleArenaController().MainArena;
		return this.#__mainArena;
	}
	static isPointInArenas(pos)
	{
		return __battle_manager.GetBattleArenaController().ArenaGroup.IsPointInArenas(pos);
	}

	static createRectangleExpand(pos = new Vector2(320, 320), size = new Vector2(130, 130))
	{
		let arenaWrapper = new BattleArenaRectangle();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateRectangleArenaExpand();
		arenaWrapper.size = size;
		arenaWrapper.position = pos;
		return arenaWrapper;
	}
	static createRectangleCulling(pos = new Vector2(320, 320), size = new Vector2(130, 130))
	{
		let arenaWrapper = new BattleArenaRectangle();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateRectangleArenaCulling();
		arenaWrapper.size = size;
		arenaWrapper.position = pos;
		return arenaWrapper;
	}
	static createCircleExpand(pos = new Vector2(320, 320), radius = 120)
	{
		let arenaWrapper = new BattleArenaCircle();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateCircleArenaExpand();
		arenaWrapper.position = pos;
		arenaWrapper.radius = radius;
		return arenaWrapper;
	}
	static createCircleCulling(pos = new Vector2(320, 320), radius = 120)
	{
		let arenaWrapper = new BattleArenaCircle();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreateCircleArenaCulling();
		arenaWrapper.position = pos;
		arenaWrapper.radius = radius;
		return arenaWrapper;
	}
	static createPolygonArenaExpand(pos = new Vector2(320, 320), vertices = [])
	{
		if (vertices.length < 3) return;
		let arenaWrapper = new BattleArenaPolygon();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreatePolygonArenaExpand();
		arenaWrapper.position = pos;
		arenaWrapper.vertices = vertices;
		return arenaWrapper;
	}
	static createPolygonArenaCulling(pos = new Vector2(320, 320), vertices = [])
	{
		if (vertices.length < 3) return;
		let arenaWrapper = new BattleArenaPolygon();
		arenaWrapper.__instance =  __battle_manager.GetBattleArenaController().CreatePolygonArenaCulling();
		arenaWrapper.position = pos;
		arenaWrapper.vertices = vertices;
		return arenaWrapper;
	}
}
class BattleUiAccess
{
	static get visible() {
		return __battle_manager.GetBattleUiController().UiVisible
	}
	static set visible(value) {
		__battle_manager.GetBattleUiController().UiVisible = value;
	}
}

export class UtmxBattleManager {
	InitializeBattle = null;
	// 不公开的函数引用
	static BattleStatus = Object.freeze({
		PLAYER : 0,
		PLAYER_DIALOGUE : 1,
		ENEMY_DIALOGUE : 2,
		ENEMY : 3,
		END : 4,
	});

	static soul = BattleSoul;
	static arena = BattleArenaAccess;
	static ui = BattleUiAccess;

	static startEncounter(encounterId)
	{
		__battle_manager.StartEncounterBattle(encounterId);
	}

	static endEncounter()
	{
		__battle_manager.EndEncounterBattle();
	}

	static gameOver()
	{
		__battle_manager.GameOver();
	}

	static isInBattle() {
		return __battle_manager.IsInBattle();
	}

	static switchStatus(status) {
		return __battle_manager.SwitchStatus(status);
	}
}
