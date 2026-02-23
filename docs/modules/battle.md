# Battle 模块

Battle 是 **UTMX 竞技场架的战斗管理模块**，用于统一管理 `Encounter` 战斗。

访问或调用 Battle 模块的大部分属性/方法之前，请务必确保已经开始战斗，调用 [isInBattle](#isInBattle) 方法可以查询是否在正在战斗中。

!> 虽然 Battle 是一个全局模块，但其中的子模块依赖战斗场景元素，在使用时请务必确保是否已经开始战斗。


## 常量（Constants）

### BattleStatus

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| PLAYER   | number | 0       | 表示玩家回合的状态 |
| PLAYER_DIALOGUE   | number | 1       | 表示玩家回合显示对话的状态 |
| ENEMY_DIALOGUE   | number | 2       | 表示怪物回合显示怪物对话的状态 |
| ENEMY   | number | 3       | 表示怪物回合的状态 |
| END   | number | 4       | 表示游戏结束的状态 |

### AttackStatus

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| SELECTED   | number | 0       | 表示怪物在 `FIGHT` 菜单被选中时的状态 |
| HIT   | number | 1       | 表示怪物在菜单中被选中后确认进行攻击的状态 |
| MISSED   | number | 2       | 表示怪物在菜单中被选中，但未按下攻击键的状态。 |

## 战斗管理

### isInBattle

```javascript
isInBattle() -> boolean
```

查询战斗是否开始，当玩家处于战斗中时，该方法返回 true。

**Returns** `void`

### startEncounter

```javascript
startEncounter(encounterId: string) -> void
```

开启一场指定的遭遇战斗，如果战斗已经开始，则会立刻结束先前的战斗。

**Returns** `void`

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

立即结束当前的遭遇战斗，如果处于战斗场景，则会回退到发起战斗之前的场景，无论处于什么状态。

**Returns** `void`

---

### gameOver

```javascript
gameOver() -> void
```

结束当前的遭遇战斗，如果处于战斗场景 ，则会跳转到 `GameOver` 场景。

**Returns** `void`

---

### switchStatus

```javascript
switchStatus(status: BattleStatus) -> void
```

处于战斗中时，切换当前战斗的战斗状态。

| Property   | Type    | Default   | Description     |
| ---------- | ------- | ----      | --------------- |
| status  | BattleStatus       | - | 当前战斗的战斗状态  |

**Returns** `void`

---

# Arena 子模块

Arena 为 Battle 的内置子模块，用于创建和管理玩家在战斗中的 **竞技场**。
通过 `UTMX.battle.arena` 访问。

---

### getMainArena

```javascript
getMainArena() -> BattleArenaRectangle
```

获取战斗的主竞技场对象，类型是一个 `BattleArenaRectangle`，但区别于常规的 `BattleArenaRectangle`，主竞技场的原点在中下方。

**Returns** `BattleArenaRectangle`

---

### isPointInArenas

```javascript
isPointInArenas(pos: Vector2, ignoreCulling?: boolean = false) -> boolean
```


检查一个世界坐标 `pos` 是否处于任意竞技场的有效范围内。若 `pos` 处于竞技场内，返回 `true`。

| Property   | Type    | Default   | Description     |
| ---------- | ------- | ----      | --------------- |
| pos  | Vector2       | - | 指定世界坐标的一个点  |
| ignoreCulling  | boolean  | false | 是否忽略剔除竞技场，只检查扩展竞技场范围 |

**Returns** `BattleArenaRectangle`

---

### createRectangleExpand

```javascript
createRectangleExpand(position: Vector2, size: Vector2) -> BattleArenaRectangle
```

在 `position` 位置创建一个大小为 `size` 的矩形扩展竞技场。

矩形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaRectangle)

**Returns** `BattleArenaRectangle`

---

### createRectangleCulling

```javascript
createRectangleCulling(position: Vector2, size: Vector2) -> BattleArenaRectangle
```

在 `position` 位置创建一个大小为 `size` 的矩形剔除竞技场。

矩形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaRectangle)

**Returns** `BattleArenaRectangle`

---

### createCircleExpand

```javascript
createCircleExpand(position: Vector2, radius: float) -> BattleArenaCircle
```

在 `position` 位置创建一个半径为 `radius` 的圆形扩展竞技场。

圆形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaCircle)

**Returns** `BattleArenaCircle`

---

### createCircleCulling

```javascript
createCircleCulling(position: Vector2, radius: float) -> BattleArenaCircle
```

在 `position` 位置创建一个半径为 `radius` 的圆形剔除竞技场。

圆形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaCircle)

**Returns** `BattleArenaCircle`

---

### createPolygonArenaExpand

```javascript
createPolygonArenaExpand(position: Vector2, vertices: Array(Vector2)) -> BattleArenaPolygon
```

在 `position` 位置创建一个顶点列表为 `vertices` 的多边形扩展竞技场。

多边形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaPolygon)

**Returns** `BattleArenaPolygon`

---

### createPolygonArenaCulling

```javascript
createPolygonArenaCulling(position: Vector2, vertices: Array(Vector2)) -> BattleArenaPolygon
```

在 `position` 位置创建一个顶点列表为 `vertices` 的多边形扩展竞技场。

多边形竞技场的介绍请见：[BattleArenaRectangle](types/game-object/battle-arena.md#BattleArenaPolygon)

**Returns** `BattleArenaPolygon`

# Soul 子模块

Soul 为 Battle 的内置子模块，用于管理玩家在战斗中的 **灵魂**。
通过 `UTMX.battle.soul` 访问。

## 属性（Properties）

| Property          | Type    | Default   | Description                                      |
| ----------------- | ------- | --------- | ------------------------------------------------ |
| position  | Vector2 | (0, 0)      | 玩家灵魂的位置（相当于世界原点） |
| rotation  | number | 0      | 玩家灵魂的旋转角度（单位为角度） |
| scale  | Vector2 | (1, 1) | 玩家灵魂的缩放     |
| skew  | number | 0 | 玩家灵魂的倾斜（单位为弧度）     |
| enabledCollisionWithArena  | boolean | true      | 是否启用碰撞，这会影响与 Arena 的碰撞。 |
| enabledCollisionWithProjectile  | boolean | true      | 是否启用碰撞，这会影响与 Projectile 的碰撞。 |
| movable | boolean | true | 是否停用移动，如果停用移动，引擎就不会代理灵魂的移动，这对自定义灵魂行为很有帮助。|
| sprite | Sprite | null | （只读）玩灵魂对应的 Sprite 访问对象。 |

---

### tryMoveTo

```javascript
tryMoveTo(target: Vector2) -> void
```
 
尝试移动玩家灵魂，这会使灵魂受到 Arena 的阻碍从而阻止移动，通常我们需要这个方法安全地移动灵魂。

**Returns** `void`

---

### isOnArenaFloor

```javascript
isOnArenaFloor() -> void
```
 
判断灵魂是否处于战斗竞技场中的“地板”上。

这便于我们实现横板平台跳跃的游戏机制。

**Returns** `void`

---

### IsOnArenaCeiling

```javascript
IsOnArenaCeiling() -> void
```
 
判断灵魂是否处于战斗竞技场中的“天花板”上。

**Returns** `void`

---

### isMoving

```javascript
isMoving() -> boolean
```
 
判断灵魂是否正在移动，若玩家移动，返回 `true`，这是通过判断上一帧的位置是否与当前帧一致实现的。

**Returns** `boolean`

---

#### 使用示例

该示例展示了如何在一个自定义的类中，实现一个完整的**蓝色**灵魂控制器。

```javascript
import { UTMX, Vector2, Color } from "UTMX";

export default class BlueSoulController
{
	constructor()
    {
		this.jumping = false;
		this.moveSpeed = 130.0;
		this.gravity = 300.0;
		this.jumpSpeed = 0.0;
	}

	init()
    {
		UTMX.battle.soul.sprite.color = Color.Blue;
        UTMX.battle.soul.movable = false;
	}

	uninit()
    {
		UTMX.battle.soul.sprite.color = Color.Blue;
        UTMX.battle.soul.movable = false;
	}

	update(delta)
    {
		let moveSpeed = new Vector2(0, 0);
		
		if (UTMX.battle.soul.isOnArenaFloor())
        {
			if (UTMX.input.isActionHeld("up") && !this.jumping)
            {
				this.jumping = true;
				this.jumpSpeed = -200;
				moveSpeed.y = this.jumpSpeed;
			}
            else
            {
				this.jumping = false;
				this.jumpSpeed = 0.0;
			}
		}
		else
		{
			this.jumpSpeed += this.gravity * delta;
			moveSpeed.y = this.jumpSpeed;
		}
		
		if (this.jumping && moveSpeed.y < 0)
        {
			if (UTMX.input.isActionReleased("up") || UTMX.battle.soul.isOnArenaCeiling())
            {
				this.jumping = false;
				this.jumpSpeed = 0.0;
			}
		}
		
		moveSpeed.x = UTMX.input.getActionAxis("left", "right") * this.moveSpeed;
		let soulPosition = new Vector2().copy(UTMX.battle.soul.position);
		UTMX.battle.soul.tryMoveTo(
			soulPosition.add(moveSpeed.multiply(delta).rotated(UTMX.battle.soul.rotation * Math.PI / 180)));
	}
}
```

---

# UI 子模块

UI 为 Battle 的内置子模块，用于管理玩家在战斗中的 **UI** 等。
通过 `UTMX.battle.ui` 访问。

战斗中的玩家对话框、按钮等都属于 UI 元素。

## 属性（Properties）

| Property          | Type    | Default   | Description                                      |
| ----------------- | ------- | --------- | ------------------------------------------------ |
| visible  | boolean | true      | 控制战斗中的 UI 是否显示 |
