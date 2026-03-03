# BattleButton

继承 [Sprite](types/game-object/sprite.md)

BattleButton 表示战斗 UI 中的按钮对象（如 `FIGHT` / `ACT` / `ITEM` / `MERCY`）。

该类型不通过 `UTMX` 顶层导出，通常也不应该自行 `new`。  
你应当通过 `UTMX.battle.ui` 的 `getButton*` 系列方法获取它。

!> 只有通过 `UTMX.battle.ui.getButtonById(...)`、`UTMX.battle.ui.getButtonByIndex(...)` 等 `getButton` 系列方法获得的按钮对象才是有效的。手动创建的对象不会绑定战斗按钮实例。

---

## 获取方式（BattleUiAccess）

### getButtonById

```javascript
UTMX.battle.ui.getButtonById(id: string) -> BattleButton
```

根据按钮 Id 获取对应的 BattleButton 对象。

---

### getButtonByIndex

```javascript
UTMX.battle.ui.getButtonByIndex(index: number) -> BattleButton
```

根据按钮索引获取对应的 BattleButton 对象。

---

## 核心属性（Properties）

> BattleButton 继承 `Sprite` 的全部属性与方法，下面仅列出 BattleButton 自身扩展属性。

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| hover | boolean | false | 是否处于高亮/选中状态 |

---

## 方法（Methods）

### setSoulPosition

```javascript
setSoulPosition(pos: Vector2) -> void
```

设置该按钮的灵魂光标锚点位置（按钮局部坐标）。

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| pos | Vector2 | - | 灵魂光标在按钮内的目标位置 |

---

## 示例

```javascript
import { UTMX } from "UTMX";

let fightButton = UTMX.battle.ui.getButtonById("FIGHT");
fightButton.hover = true;
fightButton.setSoulPosition(new Vector2(-26, 0));

let firstButton = UTMX.battle.ui.getButtonByIndex(0);
firstButton.hover = true;
```
