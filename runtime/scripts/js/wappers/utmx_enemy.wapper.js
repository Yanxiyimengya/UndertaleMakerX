import { __UtmxEnemy,__logger } from "__UTMX";

export class UtmxBaseEnemy extends __UtmxEnemy  
{
	set name(value) {
		__logger.Log(value)
		this.DisplayName = value;
	}
	get name() {
		return this.DisplayName;
	}
	get slot() {
		return this.EnemySlot;
	}
	set slot(value) {	
		this.EnemySlot = value;
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
    get centerPosition() {
        return this.CenterPosition;
    }
    set centerPosition(value) {
        this.CenterPosition = value;
    }

    onSpare()
    {
    }

    handleAction(action)
    {
    }

    handleAttack(status)
    {
    }
}
