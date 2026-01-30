import { UTMX } from "UTMX";

export default class MyEnemy extends UTMX.Encounter {
    
    constructor() {
        super();
        this.freeText = "这是一个测试遭遇战！我跑了！";
        this.encounterText = "你遇到了一个测试敌人！";
        this.deathText = "我被打败了！";
        this.endText = "遭遇战结束！";
        this.enemysList = ["MyEnemy"];
        //this.canFree = false;
    }

    onBattleStart() {
    }

    onBattleEnd() {
    }

    onPlayerTurn() {
    }

    onPlayerDialogue() {
    }

    onEnemyDialogue() {
    }

    onEnemyTurn() {
    }
}
