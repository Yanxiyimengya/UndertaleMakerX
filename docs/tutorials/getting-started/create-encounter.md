# 创建遭遇

[遭遇战](/types/encounter.md) 是战斗模块的核心，通过创建一个继承 `UTMX.Encounter` 的导出类，你可以定义一个遭遇战斗。

```javascript
// my_custom_encounter.js
import { UTMX } from "UTMX";

export default class MyCustomEncounter extends UTMX.Encounter
{
    constructor()
    {
        super();
        this.enemies = ["MY_CUSTOM_ENEMY"]; // 指定战斗中的怪物列表
        this.encounterText = "* You encountered the enemy."; // 遭遇文本
        this.freeText = "* Escaped..."; // 逃跑文本
        this.endText = `* YOU WON!\n[wait=0.2]* You earned 0 EXP and 0 gold.`; // 获胜文本
    }
}
```

随后，你需要在合适的位置（通常是主脚本）注册并触发战斗，就像这样：

```javascript
import { UTMX } from "UTMX";
export default class Main
{
	constructor()
    {
        UTMX.registerDb.registerEnemy("MY_CUSTOM_ENEMY", "js/enemies/my_custom_enemy.js");
        UTMX.registerDb.registerEncounter("MY_CUSTOM_ENCOUNTER", "js/encounters/my_custom_encounter.js");
	}

    
	onGameStart()
	{
        UTMX.battle.startEncounter("MY_CUSTOM_ENCOUNTER");
        // 游戏启动后，就会开始遭遇战斗。
	}
}
```