# BattleArena

继承 [TransformableObject](types/game-object/transformable-object.md)

BattleArena 表示战斗场景中的 **竞技场对象**，竞技场可以限制在角色灵魂的移动。

BattleArena 是 `UTMX` 战斗系统中非常强大的功能，支持创建支持 `3` 种不同形状的竞技场，每种形状的竞技场都有对应的 **ExpandArena** 与 **CullingArena** 分支。

!> 只有通过 `UTMX.battle.arena` 模块创建的 BattleArena 才有效，手动创建的 BattleArena 无法生效。

## 扩展竞技场（ExpandArena、加框）与剔除竞技场（CullingArena、减框）

### 扩展竞技场（ExpandArena）

扩展竞技场用于“可通行/可碰撞”的竞技场区域，内容会影响遮罩范围：

![ExpandArena](../../imgs/arena-expand.jpg ':size=50%')

剔除竞技场用于在扩展竞技场内，额外限制“禁止通行”的竞技场区域：

![CullingArena](../../imgs/arena-culling.jpg ':size=50%')


## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| borderWidth   | number | 5      | 竞技场边框的宽度 |
| enabled   | boolean | true      | 决定当前竞技场对象是否启用 |

---

# BattleArenaRectangle

继承 [BattleArena](#BattleArena)

形状为矩形的竞技场，中心点在矩形正中央。


## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| size     | Vector2 | -      | 矩形的大小 |

---

# BattleArenaCircle

继承 [BattleArena](#BattleArena)

形状为圆形的竞技场，中心点在圆心。

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| radius     | number | -      | 圆的半径的大小 |

---

# BattleArenaPolygon

继承 [BattleArena](#BattleArena)

形状为多边形的竞技场。

> 多边形的顶点数量**必须大于等于3**，在定义顶点时，需注意顺序及位置合理性。

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| vertices | Vector2[] | -      | 多边形的顶点数组 |