# Input 模块

Input 是 **UTMX 框架的输入管理核心模块**，用于统一管理键盘输入、鼠标输入以及自定义输入动作（Action）。

支持：

* 按键检测
* 动作映射绑定
* 动作映射检测

通过全局对象 `UTMX.input` 访问。

?> Input 为全局模块，可在游戏任意生命周期阶段调用。

---

## 枚举常量

### MouseButton

鼠标按键枚举，用于标识各类鼠标按键或滚轮操作。

| Name        | Value | Description |
| ----------- | ----- | ----------- |
| NONE        | 0     | 无按键         |
| LEFT        | 1     | 鼠标左键        |
| RIGHT       | 2     | 鼠标右键        |
| MIDDLE      | 3     | 鼠标中键        |
| WHEEL_UP    | 4     | 鼠标滚轮向上      |
| WHEEL_DOWN  | 5     | 鼠标滚轮向下      |
| WHEEL_LEFT  | 6     | 鼠标滚轮向左      |
| WHEEL_RIGHT | 7     | 鼠标滚轮向右      |
| XBUTTON1    | 8     | 鼠标侧键 1      |
| XBUTTON2    | 9     | 鼠标侧键 2      |

---

### KeyboardButton

键盘按键枚举，包含标准键盘完整按键标识值。

覆盖：

* 功能键
* 方向键
* 字母键
* 数字键
* 多媒体扩展键

访问方式：

```javascript
UTMX.input.KeyboardButton.KEY_NAME
```

（此处保留完整枚举表，数值不变）

---

## Action 管理

自定义动作（Action）用于将多个输入设备按键统一映射为一个逻辑动作。

示例：

* `player_jump`
* `player_move_up`
* `attack`

---

### addAction

```javascript
addAction(actionId: string) -> void
```

注册新的自定义输入动作。

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### hasAction

```javascript
hasAction(actionId: string) -> boolean
```

检查指定动作是否已注册。

**Returns** `boolean`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### eraseAction

```javascript
eraseAction(actionId: string) -> void
```

注销已注册的动作。

注销后该动作将无法再被检测。

**Returns** `void`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### actionAddKeyButton

```javascript
actionAddKeyButton(actionId: string, key: number) -> void
```

为指定动作绑定键盘按键。

**Returns** `void`

| Parameter | Type   | Description                  |
| --------- | ------ | ---------------------------- |
| actionId  | string | 自定义动作唯一标识 ID                 |
| key       | number | 键盘按键值（取 `KeyboardButton` 枚举） |

---

### actionAddMouseButton

```javascript
actionAddMouseButton(actionId: string, mouseButton: number) -> void
```

为指定动作绑定鼠标按键。

**Returns** `void`

| Parameter   | Type   | Description               |
| ----------- | ------ | ------------------------- |
| actionId    | string | 自定义动作唯一标识 ID              |
| mouseButton | number | 鼠标按键值（取 `MouseButton` 枚举） |

---

## Action 查询

### isActionHeld

```javascript
isActionHeld(actionId: string) -> boolean
```

检测指定动作是否处于持续按下状态。

**Returns** `boolean`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### isActionPressed

```javascript
isActionPressed(actionId: string) -> boolean
```

检测指定动作是否在当前帧被按下（单次触发）。

**Returns** `boolean`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### isActionReleased

```javascript
isActionReleased(actionId: string) -> boolean
```

检测指定动作是否在当前帧被松开（单次触发）。

**Returns** `boolean`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| actionId  | string | 自定义动作唯一标识 ID |

---

### getActionStrength

```javascript
getActionStrength(action) -> number
```
返回一个介于 `0` 和 `1` 之间的值，表示给定动作的强度。

**Returns** `number`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| action  | string | 自定义动作唯一标识 ID |

---

### getActionAxis

```javascript
getActionAxis(negativeAction, positiveAction) -> number
```

通过指定两个动作来获取轴的输入，一个是负的，一个是正的。

**Returns** `number`

| Parameter | Type   | Description  |
| --------- | ------ | ------------ |
| negativeAction  | string | 负方向，自定义动作唯一标识 ID |
| positiveAction  | string | 正方向，自定义动作唯一标识 ID |

---

## 直接输入查询

### isKeyPressed

```javascript
isKeyPressed(key: number) -> boolean
```

检测指定键盘按键是否处于持续按下状态。

**Returns** `boolean`

| Parameter | Type   | Description                  |
| --------- | ------ | ---------------------------- |
| key       | number | 键盘按键值（取 `KeyboardButton` 枚举） |

---

### isPhysicalKeyPressed

```javascript
isPhysicalKeyPressed(key: number) -> boolean
```

检测指定物理键盘按键是否处于持续按下状态。

**Returns** `boolean`

| Parameter | Type   | Description                  |
| --------- | ------ | ---------------------------- |
| key       | number | 键盘按键值（取 `KeyboardButton` 枚举） |

---

### isMouseButtonPressed

```javascript
isMouseButtonPressed(mouseButton: number) -> boolean
```

检测指定鼠标按键是否处于持续按下状态。

**Returns** `boolean`

| Parameter   | Type   | Description               |
| ----------- | ------ | ------------------------- |
| mouseButton | number | 鼠标按键值（取 `MouseButton` 枚举） |

---

### getMousePosition

```javascript
getMousePosition() -> Vector2
```

获取鼠标相对于 **世界原点** 的坐标位置。

**Returns** `Vector2`

---

### getViewportMousePosition

```javascript
getViewportMousePosition() -> Vector2
```

获取鼠标相对于 **视口** （通常是窗口左上角）的坐标位置。

**Returns** `Vector2`

---

### getScreenMousePosition

```javascript
getScreenMousePosition() -> Vector2
```

获取鼠标相对于 **屏幕原点** 的坐标位置。

**Returns** `Vector2`

---

#### 使用示例

```javascript
import { UTMX } from "UTMX";

UTMX.input.addAction("player_move_up");
UTMX.input.actionAddKeyButton("player_move_up", UTMX.input.KeyboardButton.UP);
UTMX.input.actionAddKeyButton("player_move_up", UTMX.input.KeyboardButton.W);
// 定义自定义行为 player_move_up

function update(delta) {

    if (UTMX.input.isActionHeld("player_move_up")) {
        console.log("玩家向上移动");
    }

    if (UTMX.input.isKeyPressed(UTMX.input.KeyboardButton.SPACE)) {
        console.log("空格键被按下");
    }

    if (UTMX.input.isMouseButtonPressed(UTMX.input.MouseButton.LEFT)) {
        console.log("鼠标左键点击，坐标：", UTMX.input.getMousePosition());
    }
}
```