# 创建回合

[回合](/types/turn.md) 通常指战斗中怪物使用的回合，具有 **专属的生命周期**，通过创建一个继承 `UTMX.Turn` 的导出类，你可以定义一个回合。


```javascript
// my_custom_turn.js
 import { UTMX , Vector2 } from "UTMX";

export default class MyCustomTurn extends UTMX.BattleTurn
{
    timer = 0; // 你可以编写一个 timer 变量，表示计时器
    bullet = null;

    onTurnInit()
    {
        this.arenaInitSize = new Vector2(155, 130);     // 战斗框初始大小
        this.soulInitPosition = new Vector2(0, 0);      // 玩家灵魂初始位置
        this.turnTime = 9999.0;
        // 回合时间，可以将其设置为一个非常大的数字，自定义管理如何结束回合
    }

    onTurnStart()
    {
        const proj = UTMX.BattleProjectile.new();
        proj.textures = "textures/bullets/bullet.png";
        proj.position = new Vector2(320, 320 - UTMX.battle.arena.getMainArena().size.y);
        this.bullet = proj;
        // 回合开始，创建射弹
    }

    onTurnUpdate(delta)
    {
        this.timer++;
        
        if (this.bullet != null)
        {
            const lerp = (a, b, t) => a + (b - a) * t;
            let bulletPosition = this.bullet.globalPosition;
            let soulPosition = UTMX.battle.soul.position;
            const nextPosition = new Vector2(
                lerp(bulletPosition.x, soulPosition.x, 0.025),
                lerp(bulletPosition.y, soulPosition.y, 0.025)
                );
            this.bullet.globalPosition = nextPosition;
            // 持续移动子弹
        }

        if (this.timer == 300)
        {
            this.end(); // 结束当前回合
        }
    }
}
```

若希望使用这个回合，你需要在 Enemy 的 onGetNextTurn 回调方法中，正确返回回合对象脚本的路径。

```javascript


export default class MyCustomEnemy extends UTMX.Enemy
{
    onGetNextTurn()
    {
        return "js/tuens/my_custom_turn.js";
    }
}

```