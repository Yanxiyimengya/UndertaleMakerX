import { UtmxGameSprite } from "./utmx-game-sprite.wrapper";

export class UtmxBattleButton extends UtmxGameSprite { 
    get hover() {
        return this.__instance.Hover;
    }
    set hover(value) {
        this.__instance.Hover = value;
    }

    setSoulPosition(pos)
    {
        this.__instance.SetSoulPosition(pos);
    }
}