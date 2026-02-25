# Player 模块

Player 是 **UTMX 框架的核心玩家管理模块**，负责维护玩家基础属性（等级、生命值、经验值、金币等）以及战斗与库存相关逻辑。

通过全局对象 `UTMX.player` 访问。

?> Player 为全局模块，可在游戏任意生命周期阶段调用。

## 属性（Properties）

以下属性均为 **可读写属性**：

| Property       | Type   | Default   | Description        |
| -------------- | ------ | --------- | ------------------ |
| name           | string | `"FRISK"` | 玩家名称               |
| lv             | number | `1`       | 玩家等级（LV）           |
| hp             | number | `20`      | 当前生命值（不超过 `maxHp`） |
| maxHp          | number | `20`      | 最大生命值              |
| exp            | number | `0`       | 当前经验值              |
| gold           | number | `0`       | 当前金币数量             |
| attack         | number | `0`       | 基础攻击力（不含装备加成）      |
| defence        | number | `0`       | 基础防御力（不含装备加成）      |
| invincibleTime | number | `0.75`    | 受击后的默认无敌时间（秒）      |

---

## 方法（Methods）

### hurt

```javascript
hurt(damage: number, invtime?: number) -> void
```

使玩家受到指定伤害。

* 自动播放受击音效
* 若处于战斗场景，将触发无敌保护时间
* 无敌时间优先级：`invtime > invincibleTime`

**Returns** `void`

| Parameter | Type   | Default | Description         |
| --------- | ------ | ------- | ------------------- |
| damage    | number | —       | 伤害值（必须为非负数）         |
| invtime   | number | `-1`    | 自定义无敌时间；小于 0 时使用默认值 |

---

### heal

```javascript
heal(amount: number) -> void
```

为玩家恢复生命值。

* 自动播放治疗音效
* 恢复后的生命值不会超过 `maxHp`

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| amount    | number | 恢复生命值（非负数）  |

---

# Inventory 子模块

Inventory 为 Player 的内置子模块，用于管理玩家物品库存。
通过 `UTMX.player.inventory` 访问。

---

## 属性（Properties）

以下属性均为 **可读写属性**：

| Property            | Type   | Default   | Description             |
| ------------------- | ------ | --------- | ----------------------- |
| maxInventoryCount   | number | 8         | 玩家持有的物品库存最大数量 |

---

### addItem

```javascript
addItem(id: string) -> void
```

向库存添加指定物品。

> 物品必须先通过 `UTMX.registerDb` 注册，否则添加无效。

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| id        | string | 物品唯一 ID     |


#### 使用示例

```javascript
import { UTMX } from "UTMX";

UTMX.registerDb.registerItem("Monster Candy", "item/monster-candy.js");

UTMX.player.inventory.addItem("Monster Candy"); // 为玩家添加 ID为"Monster Candy"的物品
```

---

### setItem

```javascript
setItem(slot: number, id: string) -> void
```

替换指定槽位的物品。

* 槽位必须有效 (大于 `0` 且小于 `maxInventoryCount`)
* 新物品必须已注册

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| slot      | number | 槽位编号（从 0 开始） |
| id        | string | 物品唯一 ID      |

---

### removeItem

```javascript
removeItem(slot: number) -> void
```

移除指定槽位的物品。

* 若槽位为空或无效，则无效果

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| slot      | number | 槽位编号（从 0 开始） |

---

### getItem

```javascript
getItem(slot: number) -> object | null
```

获取指定槽位的物品实例。

* 若槽位为空或无效，返回 `null`

**Returns** `object | null`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| slot      | number | 槽位编号（从 0 开始） |

---

### setWeapon

```javascript
setWeapon(weaponId: string) -> void
```

设置玩家使用的武器，使用注册id。

* 武器ID必须已注册到物品数据库

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| weaponId  | string | 武器物品唯一 ID   |

---

### getWeapon

```javascript
getWeapon() -> UTMX.Weapon
```

获取玩家持有的武器对象。

**Returns** `UTMX.Weapon`

---

### setArmor

```javascript
setArmor(id: string) -> void
```

设置玩家使用的装备，使用注册id。

* 装备ID必须已注册到物品数据库

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| id        | string | 装备物品唯一 ID   |

---

### getArmor

```javascript
getArmor() -> UTMX.Armor
```

获取玩家持有的装备对象。

**Returns** `UTMX.Armor`

---

### getItemCount

```javascript
getItemCount() -> number
```

返回当前库存中的物品总数量。

**Returns** `number`
