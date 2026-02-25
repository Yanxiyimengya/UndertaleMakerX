import { UTMX , Vector2 } from "UTMX";

export default class DummyEnemy extends UTMX.Enemy
{
    constructor()
    {
        super();
        this.displayName = "Dummy";
        this.defence = -10;
        this.hp = 18;
        this.maxHp = 18;
        this.actions = ["Check", "Talk"];
        
        this.sprite.textures = "textures/dummy/spr_dummybattle_0.png";
        this.position = new Vector2(0, -10);
    }
    
    onHandleAction(action)
    {
        switch(action)
        {
            case this.actions[0]:
                return "* A cotton heart and a button eye[wait=0.2]\n* You are the apple of my eye";
            
            case this.actions[1]:
                return [
                    "* You talk to the DUMMY.[wait=0.5]\n* ...",
                    "* It doesn't seem much for\n  conversation."
                ];
        }
    }
    
    onDialogueStarting()
    {
        this.appendDialogue(
            [
              "[speed=0.5][tornado radius=4.0 freq=7.0 connected=0].....[/tornado]"  
            ],
            new Vector2(30, -10),      // OFFSET
            new Vector2(90, 110),              // SIZE
        );
    }
}