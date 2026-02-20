# Camera

继承 [TransformableObject](types/game-object/transformable-object.md)

Camera 表示当前场景有效的摄像机对象，支持常规 2D 变换与镜头震动。

通过 `UTMX.scene.getCamera()` 获取。

> 继承自 `TransformableObject`。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| zoom | Vector2 | (1, 1) | 摄像机视角缩放 |

---

## 方法（Methods）

### startShake

```javascript
startShake(duration: number = 0, shakeAmplitude?: Vector2, tempFrequency?: Vector2) -> void
```

启动摄像机震动效果。

- 当仅传入 `duration` 时：使用当前摄像机已有的震动幅度配置。
- 当传入 `shakeAmplitude` 时：使用传入幅度进行震动。
- 当传入 `tempFrequency` 时：临时使用该频率，震动结束后恢复原频率。

**Returns** `void`

| Parameter | Type   | Default | Description |
| --------- | ------ | ------- | ----------- |
| duration | number | 0 | 震动持续时间（秒），传 `0` 时使用引擎默认时长 |
| shakeAmplitude | Vector2 | 当前配置值 | 震动幅度（X/Y） |
| tempFrequency | Vector2 | 当前配置值 | 临时震动频率（X/Y） |

---

#### 使用示例

```javascript
import { UTMX, Vector2 } from "UTMX";

let camera = UTMX.scene.getCamera();
if (camera != null)
{
    camera.startShake(0.12, new Vector2(6, 4), new Vector2(28, 24));
}
```
