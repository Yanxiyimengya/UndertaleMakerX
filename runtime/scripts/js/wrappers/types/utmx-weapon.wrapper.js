import { UtmxBaseItem } from "./utmx-item.wrapper";

export class UtmxBaseWeapon extends UtmxBaseItem {
    set attack(value) {
        this.Attack = value;
    }   
    get attack() {
        return this.Attack;
    }
    
    set attackAnimation(value) {
        this.AttackAnimation = value;
    }   
    get attackAnimation() {
        return this.AttackAnimation;
    }
    
    set attackAnimationSpeed(value) {
        this.AttackAnimationSpeed = value;
    }   
    get attackAnimationSpeed() {
        return this.AttackAnimationSpeed;
    }
    
    set attackSound(value) {
        this.AttackSound = value;
    }   
    get attackSound() {
        return this.AttackSound;
    }

    //onAttack(value, targetEnemy) {}
}