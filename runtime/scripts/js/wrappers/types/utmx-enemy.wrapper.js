import { __UtmxEnemy } from "__UTMX";
import { UtmxGameSprite } from "./utmx-game-sprite.wrapper";

export class UtmxBaseEnemy extends __UtmxEnemy  
{
	constructor()
	{
		super();
		this.__sprite = new UtmxGameSprite();
		this.__sprite.__instance = this;
	}
    get sprite() { return this.__sprite; }
    set sprite(value) { }

	set displayName(value) {
		this.DisplayName = value;
	}
	get displayName() {
		return this.DisplayName;
	}
	get attack() {
		return this.Attack;
	}
	set attack(value) {	
		this.Attack = value;
	}
	get defence() {
		return this.Defence;
	}
	set defence(value) {	
		this.Defence = value;
	}
	get hp() {
		return this.Hp;
	}
	set hp(value) {	
		this.Hp = value;
	}
	get maxHp() {
		return this.MaxHp;
	}
	set maxHp(value) {	
		this.MaxHp = value;
	}
	get allowSpare() {
		return this.AllowSpare;
	}
	set allowSpare(value) {	
		this.AllowSpare = value;
	}
    get canSpare() {
        return this.CanSpare;
    }
    set canSpare(value) {
        this.CanSpare = value;
    }
    get missText() {
        return this.MissText;
    }
    set missText(value) {
        this.MissText = value;
    }
    get actions() {
        return this.Actions;
    }
    set actions(value) {
        this.Actions = value;
    }
    get position() {
        return this.Position;
    }
    set position(value) {
        this.Position = value;
    }
    get centerPosition() {
        return this.CenterPosition;
    }
    set centerPosition(value) {
        this.CenterPosition = value;
    }

    onHandleAction(action) {}
	onHandleAttack(status) {}
	onGetNextTurn() {}
    onDialogueStarting() {}
    onTurnStarting() {}
    onSpare() {}
	onDead() {}

	hurt(damage) {
		this.Hurt(damage);
	}
	kill() {
		this.Kill(damage);
	}
	getSlot() {
		return this.EnemySlot;
	}
    appendDialogue(...dialogues) {
        this.AppendEnemyDialogue(...dialogues);
    }
}
