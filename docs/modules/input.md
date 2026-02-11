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

| ConstantName | Value | ConstantName | Value | ConstantName | Value | ConstantName | Value |
| :----- | :---- | :----- | :---- | :----- | :---- | :----- | :---- |
| NONE | 0 | F31 | 4194362 | F24 | 4194355 | KEY_3 | 51 |
| SPACE | 32 | F32 | 4194363 | F25 | 4194356 | KEY_4 | 52 |
| EXCLAM | 33 | F33 | 4194364 | F26 | 4194357 | KEY_5 | 53 |
| QUOTEDBL | 34 | F34 | 4194365 | F27 | 4194358 | KEY_6 | 54 |
| NUMBERSIGN | 35 | F35 | 4194366 | F28 | 4194359 | KEY_7 | 55 |
| DOLLAR | 36 | HELP | 4194373 | F29 | 4194360 | KEY_8 | 56 |
| PERCENT | 37 | BACK | 4194376 | F30 | 4194361 | KEY_9 | 57 |
| AMPERSAND | 38 | FORWARD | 4194377 | F31 | 4194362 | COLON | 58 |
| APOSTROPHE | 39 | STOP | 4194378 | F32 | 4194363 | SEMICOLON | 59 |
| PARENLEFT | 40 | REFRESH | 4194379 | F33 | 4194364 | LESS | 60 |
| PARENRIGHT | 41 | VOLUMEDOWN | 4194380 | F34 | 4194365 | EQUAL | 61 |
| ASTERISK | 42 | VOLUMEMUTE | 4194381 | F35 | 4194366 | GREATER | 62 |
| PLUS | 43 | VOLUMEUP | 4194382 | MENU | 4194370 | QUESTION | 63 |
| COMMA | 44 | MEDIAPLAY | 4194388 | HYPER | 4194371 | AT | 64 |
| MINUS | 45 | MEDIASTOP | 4194389 | HELP | 4194373 | A | 65 |
| PERIOD | 46 | MEDIAPREVIOUS | 4194390 | BACK | 4194376 | B | 66 |
| SLASH | 47 | MEDIANEXT | 4194391 | FORWARD | 4194377 | C | 67 |
| KEY_0 | 48 | MEDIARECORD | 4194392 | STOP | 4194378 | D | 68 |
| KEY_1 | 49 | HOMEPAGE | 4194393 | REFRESH | 4194379 | E | 69 |
| KEY_2 | 50 | FAVORITES | 4194394 | VOLUMEDOWN | 4194380 | F | 70 |
| SEARCH | 4194395 | HYPER | 4194371 | VOLUMEMUTE | 4194381 | G | 71 |
| STANDBY | 4194396 | GLOBE | 4194416 | VOLUMEUP | 4194382 | H | 72 |
| OPENURL | 4194397 | KEYBOARD | 4194417 | MEDIAPLAY | 4194388 | I | 73 |
| LAUNCHMAIL | 4194398 | JIS_EISU | 4194418 | MEDIASTOP | 4194389 | J | 74 |
| LAUNCHMEDIA | 4194399 | JIS_KANA | 4194419 | MEDIAPREVIOUS | 4194390 | K | 75 |
| LAUNCH0 | 4194400 | UP | 4194320 | MEDIANEXT | 4194391 | L | 76 |
| LAUNCH1 | 4194401 | RIGHT | 4194321 | MEDIARECORD | 4194392 | M | 77 |
| LAUNCH2 | 4194402 | DOWN | 4194322 | HOMEPAGE | 4194393 | N | 78 |
| LAUNCH3 | 4194403 | PAGEUP | 4194323 | FAVORITES | 4194394 | O | 79 |
| LAUNCH4 | 4194404 | PAGEDOWN | 4194324 | SEARCH | 4194395 | P | 80 |
| LAUNCH5 | 4194405 | SHIFT | 4194325 | STANDBY | 4194396 | Q | 81 |
| LAUNCH6 | 4194406 | CTRL | 4194326 | OPENURL | 4194397 | R | 82 |
| LAUNCH7 | 4194407 | META | 4194327 | LAUNCHMAIL | 4194398 | S | 83 |
| LAUNCH8 | 4194408 | ALT | 4194328 | LAUNCHMEDIA | 4194399 | T | 84 |
| LAUNCH9 | 4194409 | CAPSLOCK | 4194329 | LAUNCH0 | 4194400 | U | 85 |
| LAUNCHA | 4194410 | NUMLOCK | 4194330 | LAUNCH1 | 4194401 | V | 86 |
| LAUNCHB | 4194411 | SCROLLLOCK | 4194331 | LAUNCH2 | 4194402 | W | 87 |
| LAUNCHC | 4194412 | F1 | 4194332 | LAUNCH3 | 4194403 | X | 88 |
| LAUNCHD | 4194413 | F2 | 4194333 | LAUNCH4 | 4194404 | Y | 89 |
| LAUNCHE | 4194414 | F3 | 4194334 | LAUNCH5 | 4194405 | Z | 90 |
| LAUNCHF | 4194415 | F4 | 4194335 | LAUNCH6 | 4194406 | BRACKETLEFT | 91 |
| GLOBE | 4194416 | F5 | 4194336 | LAUNCH7 | 4194407 | BACKSLASH | 92 |
| KEYBOARD | 4194417 | F6 | 4194337 | LAUNCH8 | 4194408 | BRACKETRIGHT | 93 |
| JIS_EISU | 4194418 | F7 | 4194338 | LAUNCH9 | 4194409 | ASCIICIRCUM | 94 |
| JIS_KANA | 4194419 | F8 | 4194339 | LAUNCHA | 4194410 | UNDERSCORE | 95 |
| SPECIAL | 4194304 | F9 | 4194340 | LAUNCHB | 4194411 | QUOTELEFT | 96 |
| ESCAPE | 4194305 | F10 | 4194341 | LAUNCHC | 4194412 | BRACELEFT | 123 |
| TAB | 4194306 | F11 | 4194342 | LAUNCHD | 4194413 | BAR | 124 |
| BACKTAB | 4194307 | F12 | 4194343 | LAUNCHE | 4194414 | BRACERIGHT | 125 |
| BACKSPACE | 4194308 | F13 | 4194344 | LAUNCHF | 4194415 | ASCIITILDE | 126 |
| ENTER | 4194309 | F14 | 4194345 | KP_ENTER | 4194310 | YEN | 165 |
| KP_ENTER | 4194310 | F15 | 4194346 | INSERT | 4194311 | SECTION | 167 |
| INSERT | 4194311 | F16 | 4194347 | DELETE | 4194312 | UNKNOWN | 8388607 |
| DELETE | 4194312 | F17 | 4194348 | PAUSE | 4194313 | - | - |
| PAUSE | 4194313 | F18 | 4194349 | PRINT | 4194314 | - | - |
| PRINT | 4194314 | F19 | 4194350 | SYSREQ | 4194315 | - | - |
| SYSREQ | 4194315 | F20 | 4194351 | CLEAR | 4194316 | - | - |
| CLEAR | 4194316 | F21 | 4194352 | HOME | 4194317 | - | - |
| HOME | 4194317 | F22 | 4194353 | END | 4194318 | - | - |
| END | 4194318 | F23 | 4194354 | LEFT | 4194319 | - | - |
| LEFT | 4194319 | F24 | 4194355 | KP_MULTIPLY | 4194433 | - | - |
| KP_DIVIDE | 4194434 | F25 | 4194356 | KP_SUBTRACT | 4194435 | - | - |
| KP_SUBTRACT | 4194435 | F26 | 4194357 | KP_PERIOD | 4194436 | - | - |
| KP_PERIOD | 4194436 | F27 | 4194358 | KP_ADD | 4194437 | - | - |
| KP_ADD | 4194437 | F28 | 4194359 | KP_0 | 4194438 | - | - |
| KP_0 | 4194438 | F29 | 4194360 | KP_1 | 4194439 | - | - |
| KP_1 | 4194439 | F30 | 4194361 | KP_2 | 4194440 | - | - |
| KP_2 | 4194440 | F31 | 4194362 | KP_3 | 4194441 | - | - |
| KP_3 | 4194441 | F32 | 4194363 | KP_4 | 4194442 | - | - |
| KP_4 | 4194442 | F33 | 4194364 | KP_5 | 4194443 | - | - |
| KP_5 | 4194443 | F34 | 4194365 | KP_6 | 4194444 | - | - |
| KP_6 | 4194444 | F35 | 4194366 | KP_7 | 4194445 | - | - |
| KP_7 | 4194445 | - | - | KP_8 | 4194446 | - | - |
| KP_8 | 4194446 | - | - | KP_9 | 4194447 | - | - |
| KP_9 | 4194447 | - | - | UNKNOWN | 8388607 | - | - |

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