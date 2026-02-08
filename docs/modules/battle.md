# Battle 模块

Battle 是 **UTMX 框架的战斗管理模块**，用于统一管理 `Encounter` 战斗。

访问或调用 Battle 模块的大部分属性/方法之前，请务必确保已经开始战斗，调用[isInBattle](#isInBattle)方法可以查询是否在正在战斗中。

!> 虽然 Battle 为全局模块，但其中的特定方法依赖战斗场景元素，请确保是否已经开始战斗。

## 战斗管理

### isInBattle

```javascript
isInBattle() -> boolean
```

查询战斗是否开始，当玩家处于战斗中时，该方法返回 true。

### startEncounter

```javascript
startEncounter(encounterId: string) -> void
```

开启一场指定的遭遇战斗，如果战斗已经开始，则会立刻结束先前的战斗。

#### 使用示例

```javascript
import { UTMX } from "UTMX";

if (!UTMX.registerDB.isEncounterRegistered("MyCustomEncounter"))
    UTMX.registerDB.registerEncounter("MyCustomEncounter", "encounters/my-custom-encounter.js");

UTMX.battle.startEncounter("MyCustomEncounter");
```

---

### endEncounter

```javascript
endEncounter() -> void
```
结束当前的遭遇战斗。

---

# Arena 子模块

# Camera 子模块

# Player 子模块
