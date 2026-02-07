# Audio 模块

Audio 模块是 UTMX 框架的音频管理核心类，负责统一管理游戏内音效（Sound）和背景音乐（BGM）的播放、停止、音量/音调调节等操作，是音频相关功能的唯一入口。
通过 `UTMX.audio` 全局对象访问。

<br>

?>Audio 模块是一个全局模块，在游戏的任何生命周期内都可以直接调用。

---

## 音效 (Sound) 管理

用于播放和停止游戏内的短音效（如按钮点击、受击、拾取物品等）。

### playSound
播放指定路径的音效文件。

**返回值** : `number`（返回音效的唯一 ID，可用于后续停止该音效）

| 参数 | 类型 | 简介 |
| :----- | :---- | :---- |
| soundFilePath | ${$.var.typeReferenceString} | 音效文件的资源路径 |

---

### stopSound
停止指定 ID 的音效播放。

**返回值** : `void`

| 参数 | 类型 | 简介 |
| :----- | :---- | :---- |
| soundId | ${$.var.typeReferenceNumber} | 要停止的音效 ID |

---

## 背景音乐 (BGM) 管理

用于播放、停止、调节游戏背景音乐，支持循环播放和渐变调节。

### playBgm
播放指定路径的背景音乐，并为其分配唯一 ID 以便后续管理。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| bgmId | ${$.var.typeReferenceString} | ❌ | - | 背景音乐的自定义唯一 ID |
| bgmFilePath | ${$.var.typeReferenceString} | ❌ | - | 背景音乐文件的资源路径 |
| loop | ${$.var.typeReferenceBoolean} | ✅ | false | 是否循环播放该背景音乐 |

---

### stopBgm
停止指定 ID 的背景音乐播放。

**返回值** : `void`

| 参数 | 类型 | 简介 |
| :----- | :---- | :---- |
| bgmId | ${$.var.typeReferenceString} | 要停止的背景音乐 ID |

---

### getBgmVolume
获取指定背景音乐当前的音量大小。

**返回值** : `${$.var.typeReferenceNumber}`

| 参数 | 类型 | 简介 |
| :----- | :---- | :---- |
| bgmId | ${$.var.typeReferenceString} | 要查询的背景音乐 ID |

---

### setBgmVolume
设置指定背景音乐的音量，支持渐变调节。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| bgmId | ${$.var.typeReferenceNumber} | ❌ | - | 要调节的背景音乐 ID |
| volume | ${$.var.typeReferenceNumber} | ❌ | - | 目标音量 |
| duration | ${$.var.typeReferenceNumber} | ✅ | `0` | 音量渐变时长（秒），0 表示立即生效 |

---

### getBgmPitch
获取指定背景音乐当前的音调（播放速度）。

**返回值** : `${$.var.typeReferenceNumber}`（音调值，1.0 为原速，大于 1 加速，小于 1 减速）

| 参数 | 类型 | 简介 |
| :----- | :---- | :---- |
| bgmId | ${$.var.typeReferenceString} | 要查询的背景音乐 ID |

---

### setBgmPitch
设置指定背景音乐的音调，支持渐变调节。

**返回值** : `void`

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| bgmId | ${$.var.typeReferenceString} | ❌ | - | 要调节的背景音乐 ID |
| pitch | ${$.var.typeReferenceNumber} | ❌ | - | 目标音调（1.0 为原速，建议范围 0.5 ~ 2.0） |
| duration | ${$.var.typeReferenceNumber} | ✅ | `0` | 音调渐变时长（秒），0 表示立即生效 |

---

## 全局音频控制

### stopAll
停止当前正在播放的所有音效和背景音乐。

**返回值** : `void`

---

## 使用示例

以下展示了音频模块的常见使用场景：

```javascript
import { UTMX } from "UTMX";

// 1. 播放点击音效
const clickSoundId = UtmxAudioPlayer.playSound("audio/sound/click.wav");
// 停止点击音效
UTMX.audio.stopSound(clickSoundId);

// 2. 播放循环的主界面背景音乐
UTMX.audio.playBgm("bgm_main", "audio/bgm/main_theme.mp3", true);

// 3. 渐变调节背景音乐音量（5秒内降到0.5）
UTMX.audio.setBgmVolume("bgm_main", 0.5, 5);

// 4. 加快背景音乐播放速度（立即生效）
UTMX.audio.setBgmPitch("bgm_main", 1.2);

// 5. 停止主界面背景音乐
UTMX.audio.stopBgm("bgm_main");

// 6. 紧急停止所有音频
UTMX.audio.stopAll();
```