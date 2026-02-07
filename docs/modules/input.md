# Input 模块
Input 是 UTMX 框架的输入管理核心类，负责管理游戏内鼠标、键盘的输入检测及自定义输入动作的注册与监听。
通过 `UTMX.input` 全局对象访问。

<br>

?> Input 类为全局模块，在游戏的任意生命周期阶段都可直接调用。

---

## 枚举常量
### MouseButton
鼠标按键枚举，用于标识各类鼠标操作按键。

| 常量名 | 值 | 简介 |
| :----- | :---- | :---- |
| NONE | 0 | 无按键 |
| LEFT | 1 | 鼠标左键 |
| RIGHT | 2 | 鼠标右键 |
| MIDDLE | 3 | 鼠标中键 |
| WHEEL_UP | 4 | 鼠标滚轮向上 |
| WHEEL_DOWN | 5 | 鼠标滚轮向下 |
| WHEEL_LEFT | 6 | 鼠标滚轮向左 |
| WHEEL_RIGHT | 7 | 鼠标滚轮向右 |
| XBUTTON1 | 8 | 鼠标侧键1 |
| XBUTTON2 | 9 | 鼠标侧键2 |

---

### KeyboardButton
键盘按键枚举，包含全量标准键盘按键的标识值，覆盖功能键、方向键、字母键、数字键等所有类型。

| 常量名 | 值 | 常量名 | 值 | 常量名 | 值 | 常量名 | 值 |
| :----- | :---- | :----- | :---- | :----- | :---- | :----- | :---- |
| NONE | 0 | UP | 4194320 | F1 | 4194332 | KP_0 | 4194438 |
| SPECIAL | 4194304 | RIGHT | 4194321 | F2 | 4194333 | KP_1 | 4194439 |
| UNKNOWN | 8388607 | DOWN | 4194322 | F3 | 4194334 | KP_2 | 4194440 |
| ESCAPE | 4194305 | PAGEUP | 4194323 | F4 | 4194335 | KP_3 | 4194441 |
| TAB | 4194306 | PAGEDOWN | 4194324 | F5 | 4194336 | KP_4 | 4194442 |
| BACKTAB | 4194307 | SHIFT | 4194325 | F6 | 4194337 | KP_5 | 4194443 |
| BACKSPACE | 4194308 | CTRL | 4194326 | F7 | 4194338 | KP_6 | 4194444 |
| ENTER | 4194309 | META | 4194327 | F8 | 4194339 | KP_7 | 4194445 |
| INSERT | 4194311 | ALT | 4194328 | F9 | 4194340 | KP_8 | 4194446 |
| DELETE | 4194312 | CAPSLOCK | 4194329 | F10 | 4194341 | KP_9 | 4194447 |
| PAUSE | 4194313 | NUMLOCK | 4194330 | F11 | 4194342 | VOLUMEDOWN | 4194380 |
| PRINT | 4194314 | SCROLLLOCK | 4194331 | F12 | 4194343 | VOLUMEMUTE | 4194381 |
| SYSREQ | 4194315 | KP_ENTER | 4194310 | F13 | 4194344 | VOLUMEUP | 4194382 |
| CLEAR | 4194316 | KP_MULTIPLY | 4194433 | F14 | 4194345 | MEDIAPLAY | 4194388 |
| HOME | 4194317 | KP_DIVIDE | 4194434 | F15 | 4194346 | MEDIASTOP | 4194389 |
| END | 4194318 | KP_SUBTRACT | 4194435 | F16 | 4194347 | MEDIAPREVIOUS | 4194390 |
| LEFT | 4194319 | KP_PERIOD | 4194436 | F17 | 4194348 | MEDIANEXT | 4194391 |
| KP_ADD | 4194437 | F18 | 4194349 | MEDIARECORD | 4194392 | STOP | 4194378 |
| MENU | 4194370 | F19 | 4194350 | HELP | 4194373 | BACK | 4194376 |
| FORWARD | 4194377 | F20 | 4194351 | REFRESH | 4194379 | HOMEPAGE | 4194393 |
| FAVORITES | 4194394 | F21 | 4194352 | SEARCH | 4194395 | STANDBY | 4194396 |
| OPENURL | 4194397 | F22 | 4194353 | LAUNCHMAIL | 4194398 | LAUNCHMEDIA | 4194399 |
| LAUNCH0 | 4194400 | F23 | 4194354 | LAUNCH1 | 4194401 | LAUNCH2 | 4194402 |
| LAUNCH3 | 4194403 | F24 | 4194355 | LAUNCH4 | 4194404 | LAUNCH5 | 4194405 |
| LAUNCH6 | 4194406 | F25 | 4194356 | LAUNCH7 | 4194407 | LAUNCH8 | 4194408 |
| LAUNCH9 | 4194409 | F26 | 4194357 | LAUNCHA | 4194410 | LAUNCHB | 4194411 |
| LAUNCHC | 4194412 | F27 | 4194358 | LAUNCHD | 4194413 | LAUNCHE | 4194414 |
| LAUNCHF | 4194415 | F28 | 4194359 | HYPER | 4194371 | GLOBE | 4194416 |
| KEYBOARD | 4194417 | F29 | 4194360 | JIS_EISU | 4194418 | JIS_KANA | 4194419 |
| SPACE | 32 | F30 | 4194361 | EXCLAM | 33 | QUOTEDBL | 34 |
| NUMBERSIGN | 35 | F31 | 4194362 | DOLLAR | 36 | PERCENT | 37 |
| AMPERSAND | 38 | F32 | 4194363 | APOSTROPHE | 39 | PARENLEFT | 40 |
| PARENRIGHT | 41 | F33 | 4194364 | ASTERISK | 42 | PLUS | 43 |
| COMMA | 44 | F34 | 4194365 | MINUS | 45 | PERIOD | 46 |
| SLASH | 47 | F35 | 4194366 | COLON | 58 | SEMICOLON | 59 |
| LESS | 60 | KEY_0 | 48 | EQUAL | 61 | GREATER | 62 |
| QUESTION | 63 | KEY_1 | 49 | AT | 64 | KEY_2 | 50 |
| KEY_3 | 51 | A | 65 | KEY_4 | 52 | B | 66 |
| KEY_5 | 53 | C | 67 | KEY_6 | 54 | D | 68 |
| KEY_7 | 55 | E | 69 | KEY_8 | 56 | F | 70 |
| KEY_9 | 57 | G | 71 | H | 72 | I | 73 |
| J | 74 | K | 75 | L | 76 | M | 77 |
| N | 78 | O | 79 | P | 80 | Q | 81 |
| R | 82 | S | 83 | T | 84 | U | 85 |
| V | 86 | W | 87 | X | 88 | Y | 89 |
| Z | 90 | BRACKETLEFT | 91 | BACKSLASH | 92 | BRACKETRIGHT | 93 |
| ASCIICIRCUM | 94 | UNDERSCORE | 95 | QUOTELEFT | 96 | BRACELEFT | 123 |
| BAR | 124 | BRACERIGHT | 125 | ASCIITILDE | 126 | YEN | 165 |
| SECTION | 167 | - | - | - | - | - | - |

---

## 自定义动作管理
### addAction
注册自定义输入动作，用于绑定按键/鼠标操作。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### hasAction
检查指定的自定义输入动作是否已注册。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### eraseAction
注销已注册的自定义输入动作，注销后该动作无法被检测。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### actionAddKeyButton
为已注册的自定义动作绑定键盘按键。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |
| key | ${$.var.typeReferenceNumber} | ❌ | - | 键盘按键值（取 KeyboardButton 枚举） |

---

### actionAddMouseButton
为已注册的自定义动作绑定鼠标按键。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |
| key | ${$.var.typeReferenceNumber} | ❌ | - | 鼠标按键值（取 MouseButton 枚举） |

---

## 输入检测
### isActionHeld
检测指定的自定义输入动作是否处于持续按下状态。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### isActionDown
检测指定的自定义输入动作是否在当前帧被按下（单次触发）。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### isActionReleased
检测指定的自定义输入动作是否在当前帧被松开（单次触发）。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| action | ${$.var.typeReferenceString} | ❌ | - | 自定义动作的唯一标识 ID |

---

### isKeyPressed
检测指定的键盘按键是否处于持续按下状态。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| key | ${$.var.typeReferenceNumber} | ❌ | - | 键盘按键值（取 KeyboardButton 枚举） |

---

### isPhysicalKeyPressed
检测指定的物理键盘按键是否处于持续按下状态。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| key | ${$.var.typeReferenceNumber} | ❌ | - | 键盘按键值（取 KeyboardButton 枚举） |

---

### isMouseButtonPressed
检测指定的鼠标按键是否处于持续按下状态。

**返回值** : ${$.var.typeReferenceBoolean}

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| mouseButton | ${$.var.typeReferenceNumber} | ❌ | - | 鼠标按键值（取 MouseButton 枚举） |

---

### getMousePosition
获取鼠标的全局坐标位置。

**返回值** : ${$.var.typeReferenceVector2}

---

## 使用示例
```javascript
import { UTMX } from "UTMX";

// 注册自定义动作并绑定按键
UTMX.input.addAction("player_move_up");
UTMX.input.actionAddKeyButton("player_move_up", UTMX.input.KeyboardButton.UP);
UTMX.input.actionAddKeyButton("player_move_up", UTMX.input.KeyboardButton.W);

// 帧更新中检测输入
function _process(delta) {
    // 检测自定义动作持续按下
    if (UTMX.input.isActionHeld("player_move_up")) {
        console.log("玩家向上移动");
    }

    // 检测键盘按键单次按下
    if (UTMX.input.isKeyPressed(UTMX.input.KeyboardButton.SPACE)) {
        console.log("空格键被按下");
    }

    // 检测鼠标左键单次点击
    if (UTMX.input.isMouseButtonPressed(UTMX.input.MouseButton.LEFT)) {
        console.log("鼠标左键点击，坐标：", UTMX.input.getMousePosition());
    }
}
```