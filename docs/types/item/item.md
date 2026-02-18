# Item

Item 表示一个能够被玩家使用的物品对象。通过 [GameRegisterDB](modules/game-register-db.md) 将 Item 的脚本路径注册到数据库，然后通过 `UTMX.player.inventory.addItem` 方法为玩家添加指定注册Id的物品。

通过 `UTMX.Item` 访问

> 若 Item 需要被引擎加载，必须继承 `UTMX.Item`，且必须以 `default` 关键字默认导出。

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| displayName | string | "" | 物品显示的名称  |
| usedText | string | "" | 使用物品后显示的名称  |
| droppedText | string | "" | 丢弃物品后显示的名称  |
| infoText | string | "" | 物品的信息  |

## 方法（Methods）

### removeSelf

```javascript
removeSelf() -> void
```

将自己从玩家的库存列表移除，这相当于调用 `UTMX.player.inventory.removeItem(this.getSlot())`

**Returns** `void`

---

### getSlot

```javascript
getSlot() -> void
```

获取物品位于玩家库存列表的槽位索引。

**Returns** `number`

---

### onBattleStart

```javascript
onUse() -> void
```

使用物品时调用的回调函数。

---

### onDrop

```javascript
onDrop() -> void
```

丢弃物品时调用的回调函数。

---

### onInfo

```javascript
onInfo() -> void
```

查看物品信息时调用的回调函数。

---