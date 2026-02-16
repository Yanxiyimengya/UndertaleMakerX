# Enemy

Enemy 表示一个活跃在战斗场景中的怪物对象。通过 [GameRegisterDB](modules/game-register-db.md) 将 Enemy 的脚本路径注册到数据库，然后在 `Encounter` 的 `enemies` 列表中添加这个注册 ID ，当战斗开始时，此怪物就会被引擎自动创建在战斗场景中。

通过 `UTMX.Enemy` 访问。

> 若 Enemy 需要被引擎加载，必须继承 `UTMX.Enemy`，且必须以 `default` 关键字默认导出。

<br>

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| displayName | string | "" | 怪物在菜单中显示的名字 |
| attack | number | 0 | 该怪物的攻击力 |
| defence | number | 0 | 该怪物的防御力 |
| hp | number | 10 | 该怪物当前的生命值，当 hp 归零时，怪物会从战斗中移除 |
| maxHp | number | 10 | 该怪物的最大生命值 |
| allowSpare | boolean | true | 该怪物在战斗中是否允许玩家饶恕，如果战斗中有怪物允许被饶恕，在 `Mercy` 菜单中就会出现 `Spare` 选项 |
| canSpare | boolean | false | 该怪物是可以被真正饶恕，当值为 `true` 时，该怪物在战斗菜单中显示的名字会变为黄色 |
| missText | string | "MISS" | 当攻击造成 `Miss` 时，显示的文字 |
| actions | string[] | ["CHECK"] | 该怪物的行动列表 |
| position | Vector2 | (0, 0) | 该怪物的位置 |
| centerPosition | Vector2 | (0, 0) | 该怪物的中心位置，基于 Enemy.position ，这会影响攻击动画、伤害文字等显示的位置 |
| sprite | UTMX.Sprite | - | 该怪物使用的精灵图，以中下为原点，操作精灵图的属性会直接影响到 Enemy 本身（实际上，Enemy 在 UTMX 内部本身就是一个 Sprite） |

## 方法（Methods）

### onHandleAction

```javascript
onHandleAction(action: string) -> void
```

当玩家选择指定的 Action 时触发的回调函数。action 为行动选项名称

---

### onHandleAttack

```javascript
onHandleAttack(attackState: UTMX.battle.AttackStatus) -> void
```

当玩家选择 FIGHT 指定此怪物为目标时，会根据不同时机触发对应回调函数。attackState 为状态名称。详细请看 [Battle AttackStatus](modules/battle.md#AttackStatus)。

---

### onGetNextTurn

```javascript
onGetNextTurn() -> string | BattleTurn
```

当进入怪物对话阶段时，会调用此方法获取一个指定的回合。

- 若返回字符串，该字符串必须是一个文件系统中的绝对路径，且需要指向一个有效的 Turn JavaScript 脚本类。

- 若返回对象，该对象必须是一个有效的 BattleTurn 实例。

#### 使用示例

下面展示了通过两种方式指定两个不同的目标回合。

```javascript
import { UTMX } from "UTMX";
import MyCustomTurn2 from "js/turn/my-custom-turn-2.js";

export default class MyCustomEnemy : extends UTMX.Enemy
{
    turnCounter = 0; // 回合计数器

    onGetNextTurn()
    {
        this.turnCounter += 1;
        switch(this.turnCounter)
        {
            case 1: return "js/turns/my-custom-turn-1.js";
            case 2: return new MyCustomTurn2();
        }
    }
}
```

---

### onDialogueStarting

```javascript
onDialogueStarting() -> void
```

当怪物即将触发对话时触发，可以在此调用 [appendDialogue](#appendDialogue) 添加对话。

---

### onTurnStarting

```javascript
onTurnStarting() -> void
```

当怪物回合开始时触发。

---

### onSpare

```javascript
onSpare() -> void
```

当怪物被成功饶恕时触发。

---

### onDead

```javascript
onDead() -> void
```

当怪物被击败时触发。

---

### hurt

```javascript
hurt(damage: number) -> void
```

对怪物造成 `damage` 点伤害。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| damage   | number| 造成的伤害点数 |

---

### kill

```javascript
kill() -> void
```

立即杀死怪物，并从战斗中的怪物列表中移除它。

**Returns** `void`

---

### getSlot

```javascript
getSlot() -> number
```

返回该怪物被分配在在战斗中的怪物列表中的槽位。

**Returns** `number`

---

### appendDialogue

```javascript
appendDialogue(...dialogues) -> void
```

将一段文本追加到该怪物的对话列表中，当战斗进入怪物对话状态时就会读取列表中的对话显示。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| dialogues   | ...any | 要显示的对话内容 |

---