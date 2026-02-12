# TextTyper

继承 [TransformableObject](types/game-object/transformable-object.md)

TextTyper 实现了一个逐字符打印文本的处理系统，且支持 [文本命令](#文本命令)。

通过 `UTMX.TextTyper` 访问。

---

### BBCode

BBCode（Bulletin Board Code）是专门用于在线论坛、公告板设计的轻量级标记语言，其语法通过方括号包围标签实现文本格式化。

UTMX 使用 BBCode 实现 TextTyper 的富文本功能。

BBCode 有两种格式，一种为 `[<name>=<value>]` 的形式，另一种为多参数形式 `[<name> <key>=<value> <key>=<value> ...]`

### 文本命令

UTMX 拓展了 BBCode，它们被称为 **文本命令**，当打字机文本打印到指定文本命令时，TextTyper 就会执行它。

可以通过 [processCmd](#processCmd) 实现自定义的文本命令处理回调函数

<br>

<details>
<summary><b>文本命令列表</b></summary>

| 命令名 | 语法格式 | 功能说明 |
|--------|----------|----------|
| waitfor | `[waitfor=<按键/动作名>]` | 暂停文本打印，等待指定按键或输入动作被按下后恢复 |
| wait | `[wait=<时间值>]` | 暂停文本打印指定时长（单位：秒），时长结束后恢复打印 |
| blend | `[blend=<颜色值>]` | 设置文本的整体混合颜色（Modulate），颜色值支持字符串格式（如 "#FFFFFF"） |
| speed | `[speed=<数值>]` | 修改文本打印速度（单位：秒/字符），数值越小打印越快 |
| size | `[size=<数值>]` | 设置后续文本的字体大小（单位：像素） |
| font | `[font=<字体路径>]` | 加载并应用指定路径的字体到后续文本 |
| instant | `[instant=<布尔值>]` / `[instant]` | 设置是否立即打印全部文本：传入布尔值则按值设置，无参数则切换当前状态 |
| noskip | `[noskip=<布尔值>]` / `[noskip]` | 设置是否禁止跳过打印：传入布尔值则按值设置，无参数则切换当前状态 |
| clear | `[clear]` | 清空当前已打印的文本，并重置字体、字号、颜色等样式为默认值 |
| end | `[end]` | 销毁当前 TextTyper 实例 |
| img | `[img path=<图片路径> width=?<宽度> height=?<高度> color=?<颜色值>]` | 在文本中插入图片：path 为图片资源路径，width/height 为显示尺寸，color 为图片混合颜色 |
| shader | `[shader=<着色器路径>` | 设置文本使用的着色器，应用的着色器效果是全局的 |
| voice | `[voice=<音频路径>]` / `[voice=null]` | 设置打印字符时播放的语音音频：传入 null 则清空语音 |
| play_sound | `[play_sound=<音频路径>]` | 播放指定路径的音效音频（一次性播放） |
| play_bgm | `[play_bgm path=<音频路径> id=<标识> loop=?<布尔值> pitch=?<音调> volume=?<音量> position=?<起始位置>]` | 播放指定路径的背景音乐：<br>- id：BGM 标识（默认 "_TYPER_BGM"）<br>- loop：是否循环播放<br>- pitch：音调（默认不变）<br>- volume：音量（默认不变）<br>- position：播放位置（默认：0） |
| stop_bgm | `[stop_bgm=<标识>]` / `[stop_bgm id=<标识>]` | 停止指定标识的背景音乐：无参数时停止默认标识 "_TYPER_BGM" 的 BGM |
</details>

关于其他 BBCode，请前往 [RichTextLabel 中的 BBCode](https://docs.godotengine.org/zh-cn/4.x/tutorials/ui/bbcode_in_richtextlabel.html) 查看

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| text     | string | ""      | 该 `TextTyper` 的文本 |
| instant  | boolean | false  | 若为 `true` 则使打字机一瞬间显示所有文本，直到文本末尾 |
| noskip   | boolean | false  | 若为 `true` 则禁止当 `cancel` 触发后跳过文本 |
| processCmd  | function | undefined  | 用于处理文本命令的回调函数 |
| shader | UTMX.Shader | null     | 该 `DrawableObject` 使用的着色器 |

### processCmd

这是 TextTyper 中的一个特殊内置属性，你可以设置一个匿名函数用于抓取特定的文本命令，并执行特定处理。

这个函数的声明格式如下：

```javascript
(cmdName, args) -> boolean
```

| Parameter | Type    | Description |
| --------- | ------- | ----------- |
| cmdName   | string | 文本命令的名称 |
| args      | object | 文本命令参数  |

如果成功抓取，这个函数应该返回 `true`，这会覆盖 TextTyper 内置的处理文本命令的逻辑，如果没有成功抓取，这个函数应该返回 `false`。

---

#### 使用示例

该示例展示了如何通过 [processCmd](#processCmd)，实现自定义文本命令。

```javascript
import { UTMX } from "UTMX";

let typer = UTMX.TextTyper.new("[setFace=1][print message1='Hello, ' message2='UndertaleMakerX!']Test!");
let faceIndex = 0;
typer.processCmd = (cmdName, args) => {
    switch(cmdName)
    {
        case "setFace" : {
            faceIndex = Number(args.value);
            UTMX.debug.log(faceIndex);
            return true;
        }
        case "print" :  {
                UTMX.debug.log(args.message1, args.message2);
            return true;
        }
        default : {
            return false;
        }
    }
}
```

**控制台输出:**

```
> 1
> Hello, UndertaleMakerX!
```

## 方法（Methods）

### new

```javascript
new(text: string) -> TextTyper
```

静态方法，实例化该 TextTyper，并且以 `text` 开始打印文本。

**Returns** `TextTyper`

---

### start

```javascript
start(text: string) -> void
```

清空先前的文本，以 `text` 开始重新打印文本。

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| text | string | 打印的目标文本   |

---

### getProgress

```javascript
getProgress() -> number
```

返回当前 TextTyper 的打印进度。

**Returns** `number`

---

### isFinished

```javascript
isFinished() -> boolean
```

检查当前 TextTyper 的打印进度是否已经在文本末尾。

**Returns** `boolean`
