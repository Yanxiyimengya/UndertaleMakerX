import { UTMX } from "UTMX";


export default class PapyrusEnemy extends UTMX.Enemy {
    constructor() {
        super();
        this.displayName = "Papyrus";
        this.hp = 10;
        this.maxHp = 10;
        this.attack = 20;
        this.defense = 5;
    }

    onGetNextTurn()
    {
        return "js/test_js_turn.js"
    }
}