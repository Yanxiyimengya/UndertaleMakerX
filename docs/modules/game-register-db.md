# GameRegisterDB 模块

GameRegisterDB 模块作为 UTMX 框架的核心注册中心，负责在 JavaScript 环境中统一管理游戏内物品、敌人、战斗遭遇的定义与注册流程。
通过 `UTMX.registerDB` 全局对象访问。

<br>

?> GameRegisterDB 模块是全局模块，在游戏的任意生命周期阶段都可直接调用。

---

## 物品 (Item) 注册

用于管理游戏内道具、装备的注册与注销，确保物品可被其他模块（如 Player 库存）正常调用。

### registerItem
注册物品并关联其对应的脚本文件路径，完成注册后物品才可被游戏系统识别。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| item | ${$.var.typeReferenceString} | ❌ | - | 物品唯一标识 ID |
| path | ${$.var.typeReferenceString} | ❌ | - | 物品脚本文件的资源路径（如："item/bandage.js"） |

---

### unregisterItem
根据唯一 ID 注销已注册的物品，注销后该物品无法被游戏系统调用。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| item | ${$.var.typeReferenceString} | ❌ | - | 物品唯一标识 ID |

---

### isItemRegistered
检查指定 ID 的物品是否已完成注册，常用于添加物品前的有效性校验。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| item | ${$.var.typeReferenceString} | ❌ | - | 物品唯一标识 ID |

---

## 使用示例

以下展示物品注册与状态检查的基础用法：

```javascript
import { UTMX } from "UTMX";

// 注册新物品（绷带）
UTMX.registerDB.registerItem("Bandage", "item/bandage.js");

// 检查物品是否注册，确认后添加至玩家库存
if (UTMX.registerDB.isItemRegistered("Bandage"))
{
    UTMX.player.inventory.addItem("Bandage");
}
```

---

## 敌人 (Enemy) 注册

用于登记敌人实体信息并关联脚本路径，注册后敌人可在战斗场景中动态生成。

### registerEnemy
注册敌人并关联其对应的脚本文件路径，完成注册后敌人可被战斗系统调用。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| enemy | ${$.var.typeReferenceString} | ❌ | - | 敌人唯一标识 ID |
| path | ${$.var.typeReferenceString} | ❌ | - | 敌人脚本文件的资源路径 |

---

### unregisterEnemy
根据唯一 ID 注销已注册的敌人，注销后该敌人无法在战斗中生成。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| enemy | ${$.var.typeReferenceString} | ❌ | - | 敌人唯一标识 ID |

---

### isEnemyRegistered
检查指定 ID 的敌人是否已完成注册，常用于战斗生成前的有效性校验。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| enemy | ${$.var.typeReferenceString} | ❌ | - | 敌人唯一标识 ID |

---

## 遭遇 (Encounter) 注册

用于注册战斗遭遇或场景事件逻辑，注册后可在指定场景触发对应的战斗/事件。

### registerEncounter
注册战斗遭遇并关联其脚本文件路径，完成注册后可触发对应遭遇逻辑。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| encounter | ${$.var.typeReferenceString} | ❌ | - | 遭遇唯一标识 ID |
| path | ${$.var.typeReferenceString} | ❌ | - | 遭遇脚本文件的资源路径 |

---

### unregisterEncounter
根据唯一 ID 注销已注册的遭遇，注销后该遭遇无法被场景触发。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| encounter | ${$.var.typeReferenceString} | ❌ | - | 遭遇唯一标识 ID |

---

### isEncounterRegistered
检查指定 ID 的遭遇是否已完成注册，常用于场景触发前的有效性校验。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| encounter | ${$.var.typeReferenceString} | ❌ | - | 遭遇唯一标识 ID |