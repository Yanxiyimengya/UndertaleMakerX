# 创建敌人

[怪物](/types/enemy.md) 是玩家战斗的敌人，通过创建一个继承 `UTMX.Enemy` 的导出类，你可以定义一个敌人。

```javascript
// my_custom_enemy.js
import { UTMX } from "UTMX";

export default class MyCustomEnemy extends UTMX.Enemy
{
    constructor()
    {
        super();
        this.displayName = "Enemy";
        this.hp = 100;
        this.maxHp = 100;
        this.actions = ["Check"];
        // 指定怪物的基础属性
        
        this.sprite.textures = "textures/enemies/my-custom-enemy.png";
        this.position = new Vector2(0, -10);
        // 指定怪物的精灵
    }
    
    onHandleAction(action)
    {
        switch(action)
        {
            case this.actions[0]:
                return "* A custom Enemy.";
        }
        // 在这里处理 ACTION
    }
    
    onDialogueStarting()
    {
        this.appendDialogue(
            [
              "[speed=0.5][tornado radius=4.0 freq=7.0 connected=0].....[/tornado]"
              // [tornado] 表示让文字转圈移动的打字机特效
            ],
            new Vector2(30, -10),      // OFFSET
            new Vector2(90, 110),      // SIZE
        );
        // 我们可以调用 appendDialogue 方法触发对话。
    }
}
```

随后，你需要在合适的位置（通常是主脚本）注册怪物，就像这样：

```javascript
import { UTMX } from "UTMX";
export default class Main
{
	constructor()
    {
        UTMX.registerDb.registerEnemy("MY_CUSTOM_ENEMY", "js/enemies/my_custom_enemy.js");
	}
}
```

随后，你就可以在自定义的 `Encounter` 中指定这个怪物ID。