import { __BattleTurn } from "__UTMX";

export class UtmxBattleTurn extends __BattleTurn {
    get arenaInitializeSize() {
        return this.ArenaInitializeSize;
    }
    set arenaInitializeSize(value) {
        this.ArenaInitializeSize = value;
    }
    get soulInitializePosition() {
        return this.SoulInitializePosition;
    }
    set soulInitializePosition(value) {
        this.SoulInitializePosition = value;
    }
    get turnTime() {
        return this.TurnTime;
    }
    set turnTime(value) {
        this.TurnTime = value;
    }

    onTurnInitialize() { }
    onTurnStart() { }
    onTurnEnd() { }
    onTurnUpdate(delta) { }
}
