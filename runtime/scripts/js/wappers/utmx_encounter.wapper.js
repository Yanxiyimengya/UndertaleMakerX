import { __UtmxEncounter } from "__UTMX";

export class UtmxBaseEncounter extends __UtmxEncounter
{
    get encounterText() {
        return this.EncounterText;
    }
    set encounterText(value) {
        this.EncounterText = value;
    }
    get freeText() {
        return this.FreeText;
    }
    set freeText(value) {
        this.FreeText = value;
    }

    get deathText() {
        return this.DeathText;
    }
    set deathText(value) {
        this.DeathText = value;
    }

    get endText() {
        return this.EndText;
    }
    set endText(value) {
        this.EndText = value;
    }

    get encounterBattleFirstState() {
        return this.EncounterBattleFirstState;
    }
    set encounterBattleFirstState(value) {
        this.EncounterBattleFirstState = value;
    }

    get enemysList() {
        return this.EnemysList;
    }
    set enemysList(value) {
        this.EnemysList = value;
    }

    get canFree() {
        return this.CanFree;
    }
    set canFree(value) {
        this.CanFree = value;
    }
    
    onBattleStart(){}
    onBattleEnd() {}
    onPlayerTurn() {}
    onPlayerDialogue() {}
    onEnemyDialogue() {}
    onEnemyTurn() {}
}
