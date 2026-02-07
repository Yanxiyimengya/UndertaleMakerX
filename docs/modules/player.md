# Player 模块

Player 模块作为 UTMX 框架的核心玩家管理模块，负责维护玩家的基础属性、生命值、等级等核心数据，并提供角色受击、回血、库存管理等核心功能。
通过 `UTMX.player` 全局对象访问。

<br>

?> Player 模块是全局模块，在游戏的任意生命周期阶段都可直接调用。

---

## 核心属性

Player 模块内置以下可读写的核心属性，用于存储玩家的基础状态信息：

| 属性 | 类型 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- |
| name | ${$.var.typeReferenceString} | `"FRISK"` | 玩家的自定义名称 |
| lv | ${$.var.typeReferenceNumber} | `1` | 玩家的等级（LV） |
| hp | ${$.var.typeReferenceNumber} | `20` | 玩家当前的生命值（不会超过 maxHp） |
| maxHp | ${$.var.typeReferenceNumber} | `20` | 玩家的最大生命值上限 |
| exp | ${$.var.typeReferenceNumber} | `0` | 玩家当前的经验值（Exp） |
| gold | ${$.var.typeReferenceNumber} | `0` | 玩家持有的游戏货币（金币） |
| attack | ${$.var.typeReferenceNumber} | `0` | 玩家基础攻击力（不含武器/装备加成） |
| defence | ${$.var.typeReferenceNumber} | `0` | 玩家基础防御力（不含防具/装备加成） |
| invincibleTime | ${$.var.typeReferenceNumber} | `0.75` | 战斗中受击后获得的无敌时长（单位：秒） |

---

## 核心方法

### hurt
使玩家受到指定数值的伤害，自动播放受击音效；若在战斗场景中，会为玩家触发无敌保护时间（优先级：传入的 invtime > 模块默认的 invincibleTime）。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| damage | ${$.var.typeReferenceNumber} | ❌ | - | 要扣除的生命值（伤害值，非负数值） |
| invtime | ${$.var.typeReferenceNumber} | ✅ | `-1` | 自定义无敌时长，若小于 0 则会使用玩家默认的无敌时间 |

---

### heal
为玩家恢复指定数值的生命值，自动播放治疗音效，恢复后的生命值不会超过 `maxHp` 上限（若治疗值超出剩余生命值，仅恢复至 maxHp）。

**返回值** : `void`  

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| heal | ${$.var.typeReferenceNumber} | ❌ | - | 要恢复的生命值（治疗值，非负数值） |

---

## 库存 (Inventory) 子模块

Inventory 是 Player 模块的内置子模块，专门用于管理玩家的物品库存，包括物品的添加、替换、移除、查询等操作。
通过 `UTMX.player.inventory` 访问。

### addItem
向玩家的库存中添加指定 ID 的物品，**注意**：物品必须先通过 [GameRegisterDB](api/game-register-db) 完成注册，否则添加无效。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| id | ${$.var.typeReferenceString} | ❌ | - | 要添加的物品唯一 ID |

#### 使用示例
```javascript
import { UTMX } from "UTMX";

// 推荐：先检查物品是否已注册，避免添加无效物品
if (UTMX.gameDB.isItemRegistered("Bandage")) {
    // 向玩家库存添加绷带物品
    UTMX.player.inventory.addItem("Bandage");
    UTMX.debug.log("绷带已添加至玩家库存");
} else {
    UTMX.debug.error("物品未注册：Bandage，请先通过 registerDB 注册");
}
```

---

### setItem
将指定槽位的库存物品替换为目标物品，**注意**：① 目标槽位必须已存在（需在合法的槽位范围内）；② 新物品需提前通过 [UTMX.gameDB](./gameDB) 注册。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| slot | ${$.var.typeReferenceNumber} | ❌ | - | 库存槽位编号（从 0 开始计数，如 0 代表第一个槽位） |
| id | ${$.var.typeReferenceString} | ❌ | - | 要设置的物品唯一 ID |

#### 使用示例
```javascript
import { UTMX } from "UTMX";

// 将第 3 个槽位（slot=2）的物品替换为药水
if (UTMX.gameDB.isItemRegistered("Potion")) {
    UTMX.player.inventory.setItem(2, "Potion");
}
```

---

### removeItem
移除玩家库存中指定槽位的物品，移除后该槽位变为空状态（若槽位无物品，该方法无任何效果）。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| slot | ${$.var.typeReferenceNumber} | ❌ | - | 要移除物品的库存槽位编号（从 0 开始计数） |

---

### getItem
获取玩家库存中指定槽位的物品信息，返回该物品的封装对象（需在 JavaScript 端创建）；若槽位为空/不存在，返回 `null`。

**返回值** : 物品包装类对象 / `null`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| slot | ${$.var.typeReferenceNumber} | ❌ | - | 要查询的库存槽位编号（从 0 开始计数） |

#### 使用示例
```javascript
import { UTMX } from "UTMX";

// 获取第 1 个槽位的物品
const firstItem = UTMX.player.inventory.getItem(0);
if (firstItem) {
    UTMX.debug.log("第一个槽位物品：", firstItem.displayName); // 输出物品的显示名称
} else {
    UTMX.debug.log("第一个槽位无物品");
}
```

---

### getItemCount
统计并返回玩家当前持有的物品的总数量。

**返回值** : ${$.var.typeReferenceNumber}
