import { UtmxBaseItem } from "./utmx-item.wrapper";

export class UtmxBaseArmor extends UtmxBaseItem {
    set defense(value) {
        this.Defense = value;
    }   
    get defense() {
        return this.Defense;
    }
    
    //onDefend(value) {}
}