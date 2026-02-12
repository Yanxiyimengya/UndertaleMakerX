# BattleTurn

BattleTurn 表示一场遭遇战斗中的怪物回合，你可以编写一个脚本，通过继承这个类型从而定义回合处理。

战斗中的每一个怪物都会在自身对话开始前尝试通过 `getNextTurn` 方法返回一个 BattleTurn 给引擎，如果一场战斗有多个怪物，并且返回了多个 BattleTurn ，战斗就会尝试同时加载这些 BattleTurn，并同时运行它们。

> 若 BattleTurn 需要被引擎加载，必须继承 `UTMX.BattleTurn`，且必须以 `default` 关键字默认导出。

## 回合流程

- 在怪物对话阶段，游戏会尝试调用 Enemy 的  `getNextTurn` 方法获取 BattleTurn。

- BattleTurn 的初始化阶段，调用 `onTurnInitialize` 完成初始化，然后引擎会读取 `arenaInitializeSize` 与 `soulInitPosition` 属性完成回合预备。如果存在多个 Enemy，则会选取第一个 Enemy 的初始化数据。

- 进入怪物回合后的第一帧，BattleTurn 开始，调用 `onTurnStart`

- 进入怪物回合后的每一帧都会调用 `onTurnUpdate`

- 当回合计时器结束，游戏会调用 `onTurnEnd`，然后结束回合。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| arenaInitSize | Vector2 | (155, 130) | 回合初始化时主 Arena 的大小  |
| soulInitPosition | Vector2 | (320, 320) | 回合初始化时玩家灵魂的全局位置 |
| turnTime | number | 0.0 | 回合时长（单位/秒） |


---

### onTurnInitialize

```javascript
onTurnInitialize() -> void
```

当怪物即将触发对话前触发，用于准备回合的初始化参数。

---

### onTurnStart

```javascript
onTurnStart() -> void
```

当怪物回合开始前触发一次。

---

### onTurnStart

```javascript
onTurnUpdate(delta: number) -> void
```

当怪物回合运行时的每一个渲染帧都会触发。


---

### onTurnEnd

```javascript
onTurnEnd() -> void
```

当怪物回合结束后触发。