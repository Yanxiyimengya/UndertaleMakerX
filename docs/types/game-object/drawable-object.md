# DrawableObject

DrawableObject 是一切可 2D 渲染对象的基类。当 `new` 被调用后，会自动将自身添加到当前场景中。

通过 `UTMX.DrawableObject` 访问。我们可以继承这个类型，从而实现自定义绘图。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| color | Color | Color.White     | 该 `DrawableObject` 的颜色 |
| shader | UTMX.Shader | null     | 该 `DrawableObject` 使用的着色器 |

---

## 方法（Methods）

### new

```javascript
new() -> DrawableObject
```

静态方法，实例化该 DrawableObject。

**Returns**
`DrawableObject | null`

---

### redraw

```javascript
redraw() -> void
```

重新绘制该 DrawableObject。

触发对象的重新渲染，更新视觉展示效果

**Returns** `void`

---

### drawCircle

```javascript
drawCircle(pos: Vector2, radius: number, color: Color = new Color(1,1,1)) -> void
```

绘制圆形。

* 传入 `pos` 确定圆形位置，`radius` 确定圆形大小，`color` 自定义圆形颜色

**Returns** `void`

| Parameter | Type    | Default         | Description |
| --------- | ------- | --------------- | ----------- |
| pos       | Vector2 | -               | 圆形的位置坐标 |
| radius    | number  | -               | 圆形的半径    |
| color     | Color   | new Color(1,1,1) | 圆形的颜色，默认为白色 |

---

### drawRect

```javascript
drawRect(pos: Vector2, size: Vector2, color: Color = new Color(1,1,1)) -> void
```

绘制矩形。

* 传入 `pos` 确定矩形位置，`size` 确定矩形尺寸，`color` 自定义矩形颜色

**Returns** `void`

| Parameter | Type    | Default         | Description |
| --------- | ------- | --------------- | ----------- |
| pos       | Vector2 | -               | 矩形的位置坐标 |
| size      | Vector2 | -               | 矩形的尺寸（宽和高） |
| color     | Color   | new Color(1,1,1) | 矩形的颜色，默认为白色 |

---

### drawLine

```javascript
drawLine(from: Vector2, to: Vector2, color: Color = new Color(1,1,1), width: number = -1) -> void
```

绘制线段。

* 传入 `from` 和 `to` 确定线段起止位置，`color` 自定义线段颜色，`width` 设定线段宽度

**Returns** `void`

| Parameter | Type    | Default         | Description |
| --------- | ------- | --------------- | ----------- |
| from      | Vector2 | -               | 线段的起始坐标 |
| to        | Vector2 | -               | 线段的终止坐标 |
| color     | Color   | Color(1,1,1)    | 线段的颜色 |
| width     | number  | -1              | 线段的宽度，如果传递 `-1` 则绘制宽度为 `1` 的常规线条，否则，按照一个四边形渲染带有宽度的线条 |

---

### drawTextureRect

```javascript
drawTextureRect(path: string, pos: Vector2, size: Vector2, color: Color = new Color(1,1,1)) -> void
```

绘制纹理矩形。

* 传入 `path` 指定纹理资源，`x`/`y` 确定位置，`width`/`height` 确定尺寸，`color` 自定义纹理颜色

**Returns**
`void`

| Parameter | Type    | Default         | Description |
| --------- | ------- | --------------- | ----------- |
| path      | string  | -               | 纹理资源的路径 |
| pos       | Vector2 | -               | 矩形的坐标 |
| size      | Vector2 | -               | 矩形的长宽 |
| color     | Color   | Color(1,1,1) | 纹理矩形的颜色 |

---

### drawTexturePos

```javascript
drawTexturePos(path: string, tl: Vector2, tr: Vector2, br: Vector2, bl: Vector2, colors: Color[] = []) -> void
```

根据自定义顶点位置绘制纹理。

传入 `path` 指定纹理资源，`tl`/`tr`/`br`/`bl` 分别设定纹理四个顶点位置，`colors` 为顶点自定义颜色

**Returns**
`void`

| Parameter | Type       | Default | Description |
| --------- | ---------- | ------- | ----------- |
| path      | string     | -       | 纹理资源的路径 |
| tl        | Vector2    | -       | 纹理左上角的坐标 |
| tr        | Vector2    | -       | 纹理右上角的坐标 |
| br        | Vector2    | -       | 纹理右下角的坐标 |
| bl        | Vector2    | -       | 纹理左下角的坐标 |
| colors    | Color[]    | []      | 顶点对应的颜色数组，默认为空数组 |

---

### drawPolygon

```javascript
drawPolygon(vertices: Vector2[], colors: Color[], uvs: Vector2[] = [], path: string = "") -> void
```

绘制多边形。

* 传入 `vertices` 确定多边形顶点位置，`colors` 设定顶点颜色，`uvs` 自定义纹理坐标，`path` 指定纹理资源路径
* 多边形的 `vertices` 必须大于等于 `3`，此外，`vertices`/`colors`/`uvs` 数组的参数数量必须一致。

**Returns**
`void`

| Parameter | Type       | Default | Description |
| --------- | ---------- | ------- | ----------- |
| vertices  | Vector2[]  | -       | 多边形的顶点坐标数组 |
| colors    | Color[]    | []      | 顶点对应的颜色数组 |
| uvs       | Vector2[]  | []      | 纹理 UV 坐标数组 |
| path      | string     | ""      | 纹理资源的路径 |

---

### drawText

```javascript
drawText(pos: Vector2, text: string, color: Color = Color.White, size: number = 16, font: string = "") -> void
```

在指定位置绘制字符串文本。允许指定绘制颜色、字体、字体大小。

**Returns**
`void`

| Parameter | Type       | Default | Description |
| --------- | ---------- | ------- | ----------- |
| pos       | Vector2    | -       | 绘制文本的坐标 |
| text      | string     | -       | 指定文本 |
| color     | Color      | Color.White  | 文字颜色|
| size      | number     | 16      | 文字大小 |
| font      | string     | ""      | 字体资源的路径 |