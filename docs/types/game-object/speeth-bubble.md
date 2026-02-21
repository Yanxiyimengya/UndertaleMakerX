# SpeechBubble

继承 [TextTyper](types/game-object/text-typer.md)

SpeechBubble 是带尖角的对话气泡对象，内部使用 TextTyper 逐字打印文本。通过 `UTMX.SpeechBubble` 访问。

## 常量（Constants）

### Direction

用于设置 `dir` 属性。

| Property | Type | Value | Description |
| --- | --- | --- | --- |
| Top | number | 0 | 尖角朝上 |
| Bottom | number | 1 | 尖角朝下 |
| Left | number | 2 | 尖角朝左 |
| Right | number | 3 | 尖角朝右 |

## 核心属性（Properties）

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| text | string | "" | 气泡文本，修改后会重新开始逐字打印 |
| size | Vector2 | (180, 90) | 气泡尺寸（最小 `(45, 45)`） |
| dir | UTMX.SpeechBubble.Direction | Left | 尖角方向 |
| spikeOffset | number | 0 | 尖角沿边缘偏移 |
| hideSpike | boolean | false | 是否隐藏尖角 |
| inSpike | boolean | true | 尖角是否在气泡内侧布局 |

## 方法（Methods）

### new

```javascript
new(text: string) -> SpeechBubble | null
```

创建气泡并以 `text` 作为初始文本。

> SpeechBubble 继承 TextTyper，可继续使用 `start`、`isFinished`、`getProgress`。

## SpeechBubble 扩展文本命令

以下命令是 **SpeechBubbleTextTyper** 增加的扩展命令（普通 TextTyper 不支持）：

| 命令 | 语法 | 说明 |
| --- | --- | --- |
| spikeVisible | `[spikeVisible=true]` / `[spikeVisible=false]` / `[spikeVisible]` | 控制尖角可见性；不传参数时切换当前状态 |
| dir | `[dir=top]` / `[dir=bottom]` / `[dir=left]` / `[dir=right]` / `[dir=0..3]` / `[dir]` | 设置尖角方向；不传参数时按 `Top -> Bottom -> Left -> Right` 循环 |

## 示例

```javascript
import { UTMX, Vector2 } from "UTMX";

let bubble = UTMX.SpeechBubble.new(
  "[spikeVisible=true][dir=left]Hello[wait=0.2][dir=top] World[end]"
);

bubble.position = new Vector2(300, 180);
bubble.size = new Vector2(220, 96);
bubble.spikeOffset = 24;
bubble.inSpike = false;
```
