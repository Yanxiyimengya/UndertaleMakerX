import { UTMX , Vector2 , Color } from "UTMX";

export default class BlackScreen extends UTMX.DrawableObject
{
    static instance;
    start()
    {
        BlackScreen.instance = this;
    }
    
    setVisible(visible)
    {
        const size = new Vector2(640*2, 480*2);
        this.redraw();
        if (visible) {
            this.drawRect(Vector2.Zero, size, Color.Black);
        }
        UTMX.audio.playSound("audios/sfx/snd_test.wav");
    }
}