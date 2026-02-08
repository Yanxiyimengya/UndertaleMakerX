# GameRegisterDB 模块

GameRegisterDB 是 **UTMX 框架的核心注册中心模块**，用于在 JavaScript 层统一管理游戏实体注册流程。

支持实体类型：

* Item（物品）
* Enemy（敌人）
* Encounter（战斗遭遇）

注册完成后，对应实体才可被游戏系统识别并调用。

通过全局对象 `UTMX.registerDB` 访问。

?> GameRegisterDB 为全局模块，可在游戏任意生命周期阶段调用。

---

## Item 管理

用于管理游戏内道具与装备的注册与查询。

### 查询方法

---

#### isItemRegistered

```javascript
isItemRegistered(itemId: string) -> boolean
```

检查指定物品是否已完成注册。

**Returns** `boolean` — 是否已注册

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| itemId    | string | 物品唯一标识 ID   |

---

### 注册与注销

---

#### registerItem

```javascript
registerItem(itemId: string, filePath: string) -> void
```

注册物品并关联其脚本文件路径。

!> 路径必须是可访问的资源绝对路径，不能使用相对路径，如 : `./cmy-custom-item.js` ， `../my-custom-encounter.js`

**Returns** `void`

| Parameter | Type   | Description                      |
| --------- | ------ | -------------------------------- |
| itemId    | string | 物品唯一标识 ID                        |
| filePath  | string | 物品脚本资源路径（例如 `"item/bandage.js"`） |

---

#### unregisterItem

```javascript
unregisterItem(itemId: string) -> void
```

注销已注册的物品。

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| itemId    | string | 物品唯一标识 ID   |

---

## Enemy 管理

用于登记敌人实体定义并关联脚本路径。

### 查询方法

---

#### isEnemyRegistered

```javascript
isEnemyRegistered(enemyId: string) -> boolean
```

检查指定敌人是否已完成注册。

**Returns** `boolean` — 是否已注册

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| enemyId   | string | 敌人唯一标识 ID   |

---

### 注册与注销

---

#### registerEnemy

```javascript
registerEnemy(enemyId: string, filePath: string) -> void
```

注册敌人并关联其脚本文件路径。

!> 路径必须是可访问的资源绝对路径，不能使用相对路径，如 : `./sans.js` ， `../papyrus.js`

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| enemyId   | string | 敌人唯一标识 ID   |
| filePath  | string | 敌人脚本资源路径    |

---

#### unregisterEnemy

```javascript
unregisterEnemy(enemyId: string) -> void
```

注销已注册的敌人。

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| enemyId   | string | 敌人唯一标识 ID   |

---

## Encounter 管理

用于注册战斗遭遇或场景事件逻辑。

### 查询方法

---

#### isEncounterRegistered

```javascript
isEncounterRegistered(encounterId: string) -> boolean
```

检查指定遭遇是否已完成注册。

**Returns** `boolean` — 是否已注册

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| encounterId | string | 遭遇唯一标识 ID   |

---

### 注册与注销

---

#### registerEncounter

```javascript
registerEncounter(encounterId: string, filePath: string) -> void
```

注册战斗遭遇并关联其脚本文件路径。

!> 路径必须是可访问的资源绝对路径，不能使用相对路径，如 : `./sans-encounter.js` ， `../papyrus-encounter.js`

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| encounterId | string | 遭遇唯一标识 ID   |
| filePath    | string | 遭遇脚本资源路径    |

---

#### unregisterEncounter

```javascript
unregisterEncounter(encounterId: string) -> void
```

注销已注册的遭遇。

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| encounterId | string | 遭遇唯一标识 ID   |

---

#### 使用示例

```javascript
import { UTMX } from "UTMX";

// 注册物品
UTMX.registerDB.registerItem("Bandage", "item/bandage.js");

// 检查物品是否注册
if (UTMX.registerDB.isItemRegistered("Bandage")) {
    UTMX.player.inventory.addItem("Bandage");
}

// 注册敌人
UTMX.registerDB.registerEnemy("Slime", "enemy/slime.js");

// 注册遭遇
UTMX.registerDB.registerEncounter("ForestBattle", "encounter/forest_battle.js");
```