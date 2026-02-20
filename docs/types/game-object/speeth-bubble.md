# SpeechBubble

继承 [TextTyper](types/game-object/text-typer.md)

SpeechBubble 表示一个带尖角的对话气泡对象，内部使用 TextTyper 逐字打印文本。

通过 `UTMX.SpeechBubble` 访问。

---

## 常量（Constants）

### Direction

用于设置 `dir` 属性的方向枚举。

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| Top   | number | 0       | 尖角朝上 |
| Bottom   | number | 1       | 尖角朝下 |
| Left   | number | 2       | 尖角朝左 |
| Right   | number | 3       | 尖角朝右 |

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| text     | string | ""      | 气泡中的文本，修改后会重新开始逐字打印 |
| size     | Vector2 | (180, 90)      | 气泡主体尺寸，最小值会被限制为 `(45, 45)` |
| dir   | UTMX.SpeechBubble.Direction | Left     | 气泡尖角方向 |
| Dir   | UTMX.SpeechBubble.Direction | Left     | `dir` 的别名，行为与 `dir` 完全一致 |
| spikeOffset   | number | 0       | 气泡尖角沿边缘的偏移量，超出范围会被自动裁剪 |
| hideSpike   | boolean | false       | 若为 `true` 则隐藏尖角 |
| inSpike   | boolean | true       | 若为 `true` 则尖角布局在气泡内侧，否则布局在外侧 |

---

## 方法（Methods）

### new

```javascript
new(text: string) -> SpeechBubble | null
```

静态方法，实例化该 SpeechBubble，并以 `text` 作为初始文本。

**Returns** `SpeechBubble | null`

| Parameter | Type   | Description |
| --------- | ------ | ----------- |
| text   | string | 初始文本 |

> SpeechBubble 继承自 TextTyper，因此可继续使用 `start`、`isFinished`、`getProgress` 等 TextTyper 方法。

---

#### 使用示例

```javascript
import { UTMX, Vector2 } from "UTMX";

let bubble = UTMX.SpeechBubble.new("你好，我是[color=yellow]小花[/color]。[waitfor=confirm][end]");
bubble.position = new Vector2(300, 180);
bubble.size = new Vector2(220, 96);
bubble.dir = UTMX.SpeechBubble.Direction.Left;
bubble.spikeOffset = 24;
bubble.inSpike = false;
```
