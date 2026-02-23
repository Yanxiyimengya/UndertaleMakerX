# Enemy

Enemy 表示战斗中的怪物对象。通过 `UTMX.Enemy` 访问。

> 若要被引擎加载，脚本必须 `extends UTMX.Enemy` 并使用 `export default` 导出。

## 核心属性（Properties）

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| displayName | string | "" | 怪物显示名称 |
| attack | number | 0 | 攻击力 |
| defence | number | 0 | 防御力 |
| hp | number | 10 | 当前生命值 |
| maxHp | number | 10 | 最大生命值 |
| allowSpare | boolean | true | 是否允许在 Mercy 菜单出现 Spare |
| canSpare | boolean | false | 是否可被真正 Spare（战斗菜单中名称会高亮） |
| missText | string | "MISS" | Miss 时显示文本 |
| actions | string[] | ["CHECK"] | 可选 Action 列表 |
| position | Vector2 | (0, 0) | 怪物位置 |
| centerPosition | Vector2 | (0, 0) | 怪物中心点（用于伤害文本、对话气泡等定位） |
| sprite | UTMX.Sprite | - | 绑定的精灵对象 |

## 方法（Methods）

### onHandleAction

```javascript
onHandleAction(action: string) -> string
```

玩家选择 Action 时触发。

返回行动触发的对话文本。

---

### onPlayerUsedItem

```javascript
onPlayerUsedItem() -> void
```

玩家在战斗中使用 Item 时触发。

---

### onHandleAttack

```javascript
onHandleAttack(attackState: UTMX.battle.AttackStatus) -> void
```

玩家攻击该怪物时触发，`attackState` 见 `Battle AttackStatus`。

---

### onGetNextTurn

```javascript
onGetNextTurn() -> string | BattleTurn
```

返回下一回合对象：
- 返回 `string`：回合脚本路径。
- 返回 `BattleTurn`：回合实例。

---

### onDialogueStarting

```javascript
onDialogueStarting() -> void
```

进入怪物对话阶段前触发，通常在这里调用 `appendDialogue`。

---

### onTurnStarting

```javascript
onTurnStarting() -> void
```

怪物回合开始时触发。

---

### onTurnEnd

```javascript
onTurnEnd() -> void
```

怪物回合结束时触发。

---

### onSpare

```javascript
onSpare() -> void
```

怪物被 Spare 时触发。

---

### onDead

```javascript
onDead() -> void
```

怪物死亡时触发。

---

### hurt

```javascript
hurt(damage: number) -> void
```

对怪物造成伤害。

---

### kill

```javascript
kill() -> void
```

立即杀死怪物并移出战斗。

---

### getSlot

```javascript
getSlot() -> number
```

返回怪物在战斗中的槽位索引。

---

### appendDialogue

```javascript
appendDialogue(dialogueMessage, offset = null, size = null, processCmd = null) -> void
```

将文本加入该怪物的对话队列，在怪物对话阶段创建对话气泡并播放。

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| dialogueMessage | string \| string[] | - | 对话内容，支持单条或数组 |
| offset | Vector2 | (30, 0) | 相对 `centerPosition` 的偏移 |
| size | Vector2 | (180, 90) | 气泡尺寸 |
| processCmd | function \| null | null | 自定义文本命令回调，签名为 `(cmdName, args) => boolean` |

> 说明：如果要在对话里控制尖角显示/方向，可在文本中使用 `SpeechBubble` 扩展命令：`[spikeVisible]`、`[spikeVisible=true/false]`、`[dir=top|bottom|left|right|0..3]`。

#### 示例

```javascript
onDialogueStarting() {
  this.appendDialogue("[dir=top][spikeVisible=true]Hello!");

  // 直接传入箭头函数处理自定义命令
  this.appendDialogue(
    "[mycmd foo=1]Custom",
    null,
    null,
    (cmdName, args) => {
      if (cmdName === "mycmd") {
        return true;
      }
      return false;
    }
  );
}
```
