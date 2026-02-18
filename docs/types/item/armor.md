# Armor

继承 [Item](types/item/item.md)

Armor 表示一个能够被玩家装备的防具物品对象。通过 [GameRegisterDB](modules/game-register-db.md) 将 Armor 物品的脚本路径注册到数据库，然后通过 `UTMX.player.inventory.setArmor` 方法为玩家添加指定注册Id的物品作为防具。

通过 `UTMX.Armor` 访问

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| defense | number | 0 | 防具的防御力  |


### onDefend

```javascript
onDefend(damage: number) -> number
```

当玩家装备此防具时触发的回调函数，用于战斗中的射弹对玩家默认造成的伤害。

* `damage` 为玩家受到的伤害。