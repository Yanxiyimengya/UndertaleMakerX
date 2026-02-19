import { __BattleTurn } from "__UTMX";

export class UtmxBattleTurn extends __BattleTurn {
    static new()
    {
        let ins = new this();
        return ins;
    }

    get arenaInitSize() {
        return this.arenaInitSize;
    }
    set arenaInitSize(value) {
        this.arenaInitSize = value;
    }
    get soulInitPosition() {
        return this.SoulInitializePosition;
    }
    set soulInitPosition(value) {
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

    
    end() { 
        this.Ending = true;
    }
}
