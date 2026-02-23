# Audio 模块

Audio 是 **UTMX 框架的音频管理核心模块**，用于统一管理：

* 音效（Sound）
* 背景音乐（BGM）

提供播放、停止、音量控制、音调调节及渐变控制能力。
通过全局对象 `UTMX.audio` 访问。

?> Audio 为全局模块，可在游戏任意生命周期阶段调用。

---

## Sound

用于播放短时音效（点击、受击、UI 操作等）。

---

### playSound

```javascript
playSound(soundFilePath: string) -> number
```

播放指定路径的音效文件。

返回音效实例唯一 ID，用于后续停止播放。

**Returns** `number`

| Parameter     | Type   | Description |
| ------------- | ------ | ----------- |
| soundFilePath | string | 音效资源路径      |

---

### stopSound

```javascript
stopSound(soundId: number) -> void
```

停止指定音效实例。

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| soundId   | number | 音效实例 ID     |

---

### setSoundVolume

```javascript
setSoundVolume(soundId: number, volume: number) -> void
```

设置指定 id 音效的音量

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| soundId   | number | 音效实例 ID     |
| volume   | number | 音效音量     |

---

### setSoundPitch

```javascript
setSoundPitch(soundId: number, pitch: number) -> void
```

设置指定 id 音效的音量

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| soundId   | number | 音效实例 ID     |
| pitch   | number | 音效音调     |

## BGM

用于管理可持续播放的背景音乐。

---

### playBgm

```javascript
playBgm(bgmId: string, bgmFilePath: string, loop?: boolean) -> void
```

播放背景音乐并绑定自定义 ID 进行管理。

**Returns** `void`

| Parameter   | Type    | Default | Description |
| ----------- | ------- | ------- | ----------- |
| bgmId       | string  | —       | 背景音乐唯一 ID   |
| bgmFilePath | string  | —       | 背景音乐资源路径    |
| loop        | boolean | `false` | 是否循环播放      |

---

### stopBgm

```javascript
stopBgm(bgmId: string) -> void
```

停止指定 ID 的背景音乐。

**Returns** `void`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| bgmId     | string | 背景音乐 ID     |

---

### isBgmValid

```javascript
isBgmValid(bgmId: string) -> boolean
```

检查指定 Bgm 是否有效。

**Returns** `boolean`

| Parameter     | Type   | Description |
| ------------- | ------ | ----------- |
| bgmId       | string  | —       | 背景音乐唯一 ID   |

---

### getBgmVolume

```javascript
getBgmVolume(bgmId: string) -> number
```

获取当前背景音乐音量。

**Returns** `number` — 当前音量（建议范围 `0.0 ~ 1.0`）

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| bgmId     | string | 背景音乐 ID     |

---

### setBgmVolume

```javascript
setBgmVolume(bgmId: string, volume: number, duration?: number) -> void
```

设置背景音乐音量，支持渐变控制。

**Returns** `void`

| Parameter | Type   | Default | Description            |
| --------- | ------ | ------- | ---------------------- |
| bgmId     | string | —       | 背景音乐 ID                |
| volume    | number | —       | 目标音量（建议范围 `0.0 ~ 1.0`） |
| duration  | number | `0`     | 渐变时长（秒），`0` 表示立即生效     |

---

#### getBgmPitch

```javascript
getBgmPitch(bgmId: string) -> number
```

获取背景音乐当前音调。
`1.0` 为原速，大于 `1.0` 加速，小于 `1.0` 减速。

**Returns** `number`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| bgmId     | string | 背景音乐 ID     |

---

#### setBgmPitch

```javascript
setBgmPitch(bgmId: string, pitch: number, duration?: number) -> void
```

设置背景音乐音调，支持渐变控制。

**Returns** `void`

| Parameter | Type   | Default | Description            |
| --------- | ------ | ------- | ---------------------- |
| bgmId     | string | —       | 背景音乐 ID                |
| pitch     | number | —       | 目标音调（推荐范围 `0.5 ~ 2.0`） |
| duration  | number | `0`     | 渐变时长（秒），`0` 表示立即生效     |

---


#### getBgmPosition

```javascript
getBgmPosition(bgmId: string) -> number
```

获取背景音乐当前播放的位置，以秒为单位。

**Returns** `number`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| bgmId     | string | 背景音乐 ID     |

---

#### setBgmPosition

```javascript
setBgmPosition(bgmId: string, position: number) -> void
```

设置背景音乐当前播放的位置。

**Returns** `void`

| Parameter | Type   | Default | Description            |
| --------- | ------ | ------- | ---------------------- |
| bgmId     | string | —       | 背景音乐 ID             |
| position  | number | `0`     | 播放位置（秒）           |

---

#### getBgmPaused

```javascript
getBgmPaused(bgmId: string) -> boolean
```

获取指定 BGM 是否为已暂停状态。

**Returns** `boolean`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| bgmId     | string | 背景音乐 ID     |

---

#### setBgmPaused

```javascript
setBgmPaused(bgmId: string, paused: boolean) -> void
```

设置指定 BGM 是否暂停播放，`true` 为暂停，`false` 为继续播放。

**Returns** `void`

| Parameter | Type    | Description |
| --------- | ------- | ----------- |
| bgmId     | string  | 背景音乐 ID     |
| paused    | boolean | 是否暂停播放      |

---

## Control

---

### stopAll

```javascript
stopAll() -> void
```

立即停止所有正在播放的音效与背景音乐。

**Returns** `void`

---

#### 使用示例

```javascript
import { UTMX } from "UTMX";

// 播放音效
const clickId = UTMX.audio.playSound("audio/sound/click.wav");

// 停止音效
UTMX.audio.stopSound(clickId);

// 播放循环背景音乐
UTMX.audio.playBgm("bgm_main", "audio/bgm/main_theme.mp3", true);

// 5 秒渐变降低音量
UTMX.audio.setBgmVolume("bgm_main", 0.5, 5);

// 加速播放
UTMX.audio.setBgmPitch("bgm_main", 1.2);

// 停止指定 BGM
UTMX.audio.stopBgm("bgm_main");

// 停止所有音频
UTMX.audio.stopAll();
```
