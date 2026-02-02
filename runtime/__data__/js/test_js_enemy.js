import { UTMX , Vector2} from "UTMX";
import MyBattleTurn from "./test_js_turn.js";

export default class PapyrusEnemy extends UTMX.Enemy {
    constructor() {
        super();
        this.displayName = "Papyrus";
        this.hp = 10;
        this.maxHp = 10;
        this.attack = 20;
        this.defense = 5;

        this.centerPosition = new Vector2(0.0, -150.0);
    }

    onGetNextTurn()
    {
        UTMX.debug.print("PapyrusEnemy onGetNextTurn");
        return new MyBattleTurn();
    }
}