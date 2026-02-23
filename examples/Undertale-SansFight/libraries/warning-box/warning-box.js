import { UTMX , Color , Vector2 } from "UTMX";


export default class WarningBox extends UTMX.DrawableObject {
    frame = 0;
    lifeTime = 3;
    from = Vector2.Zero;
    to = Vector2.Zero;
    
    static create(from, to, lifeTime = 3) {
        let wb = WarningBox.new();
        wb.from = from;
        wb.to = to;
        wb.lifeTime = lifeTime;
        return wb;
    }
    
    start()
    {
        this.color = Color.Red;
        this.drawLine(new Vector2(this.from.x, this.from.y), new Vector2(this.to.x, this.from.y));
        this.drawLine(new Vector2(this.to.x, this.from.y), new Vector2(this.to.x, this.to.y));
        this.drawLine(new Vector2(this.to.x, this.to.y), new Vector2(this.from.x, this.to.y));
        this.drawLine(new Vector2(this.from.x, this.from.y), new Vector2(this.from.x, this.to.y));
        UTMX.audio.playSound("libraries/warning-box/snd_b.wav");
    }
    
    update(delta)
    {
        this.frame += 1;
        if (this.frame % 5 == 0)
        {
            this.color = (this.color==Color.Red)?Color.Yellow:Color.Red;
        }
        if (this.frame >= this.lifeTime) {
            this.destroy();
        }
    }
}