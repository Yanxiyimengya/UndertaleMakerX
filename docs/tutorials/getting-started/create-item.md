# 创建物品

[物品](/types/item.md) 通常指玩家库存持有的物品，可以由玩家使用，通过创建一个继承 `UTMX.Item` 的导出类，你可以定义一个物品。


```javascript
// pie.js
import { UTMX } from "UTMX";

export default class ItemPie extends UTMX.Item
{
    constructor()
    {
        super();
        this.displayName = "派";
        this.usedText = ["* 你吃掉了奶油糖果派\n* 你的HP满了。"];
    }
    
    onUse()
    {
        UTMX.player.heal(UTMX.player.maxHp); // 回复玩家全部生命值
        this.removeSelf(); // 使用物品后，从物品栏删除自身
    }
}
```

随后，你需要在合适的位置（通常是主脚本）注册物品，就像这样：

```javascript
import { UTMX } from "UTMX";
export default class Main
{
	constructor()
    {
        UTMX.registerDb.registerItem("PIE", "js/items/pie.js");
	}

	onGameStart()
	{
        UTMX.player.inventory.addItem("PIE"); // 为玩家库存添加一个物品
	}
}
```