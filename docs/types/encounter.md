# Encounter

Encounter 表示一场遭遇战斗，你可以编写一个脚本，通过继承这个类型从而定义一场遭遇战斗，然后通过 [GameRegisterDB](modules/game-register-db.md) 将 Encounter 的脚本路径注册到数据库，最后调用 `UTMX.battle.startEncounter` 开始战斗。

通过 `UTMX.Encounter` 访问。

> 若 Encounter 需要被引擎加载，必须继承 `UTMX.Encounter`，且必须以 `default` 关键字默认导出。

<br>

## 战斗流程

调用 `UTMX.battle.startEncounter` 后，UTMX 首先尝试从 GameRegisterDB 中读取指定的 Encounter 脚本实例，随后跳转到战斗场景。

战斗场景的初始化流程大概如下：

- 预初始化阶段，战斗场景会读取当前战斗 Encounter 实例中的 `enemies` 列表，根据列表元素的中有效的 **怪物ID** 初始化怪物实例节点，并放置到场景中。

- 战斗场景向UTMX内部的全局战斗管理器注册自身，我们就可以通过 `UTMX.battle` 访问战斗模块的全部有效方法。后续调用，`UTMX.battle.isInBattle()` 将返回 `true`，战斗正式开始。

- 随后，战斗场景会读取当前战斗 Encounter 实例中的 `encounterBattleFirstState`，并跳转到此状态。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| encounterText | string | "" | 战斗显示的文本 |
| freeText | string | "" | 逃跑成功后显示的文本 |
| deathText | string | "" | 游戏失败后显示的文本 |
| endText | string | "" | 游戏获胜后显示的文本 |
| encounterBattleFirstState | UTMX.battle.BattleStatus | PLAYER | 战斗开始时最初始的状态 |
| enemies | string[] | [] | 战斗中有效的怪物 ID 列表 |
| canFree | boolean | true | 是否启用逃跑 |

## 方法（Methods）

### onBattleStart

```javascript
onBattleStart() -> void
```

战斗正式开始前触发的回调函数。

---

### onGameover

```javascript
onGameover() -> void
```

玩家被击败时触发。

---

### onBattleEnd

```javascript
onBattleEnd() -> void
```

战斗正式结束前触发的回调函数。

---

### onPlayerTurn

```javascript
onPlayerTurn() -> void
```

战斗进入玩家回合前触发的回调函数。

---

### onPlayerDialogue

```javascript
onPlayerDialogue() -> void
```

战斗进入玩家对话状态前触发的回调函数。

---

### onEnemyDialogue

```javascript
onEnemyDialogue() -> void
```

战斗进入怪物对话状态前触发的回调函数。

---

### onEnemyTurn

```javascript
onEnemyTurn() -> void
```

战斗进入怪物回合前触发的回调函数。

---
