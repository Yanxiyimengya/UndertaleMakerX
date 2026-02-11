# Shader

Shader 表示一个 UTMX 的着色器资源，它使用 Godot 着色语言实现自定义着色器程序。
通过 Shader，你可以灵活实现画面的渲染视觉效果。

通过 `UTMX.Shader` 访问。

你可以在 Shader 中通过 `uniform` 导出一些参数，这样你就可以通过[setParameter](#setParameter)设置这些值，Shader会使用这些值。

关于 JavaScript 类型如何对应到 Shader 中的类型，如下表所示：

<br>

| JavaScript | Shader   |
| ---------- | -------- |
| bool       | bool     |
| number     | float    |
| Vector2    | vec2     |
| Vector3    | vec3     |
| Vector4    | vec4     |
| Color      | vec4     |
| Float32Array[4]（具有`4`个参数的Float32Array）  | mat2     |
| Float32Array[9]（具有`9`个参数的Float32Array）  | mat3     |
| Float32Array[16]（具有`16`个参数的Float32Array） | mat4     |
| Float32Array[4*n]  | mat2[n]     |
| Float32Array[9*n]  | mat3[n]     |
| Float32Array[16*n]  | mat4[n]     |
| string（图片文件路径）  | sampler2D |
| string[n]（图片文件路径）| sampler2D[n] |

<br>

?> 矩阵遵循 std140 布局。

<br>

关于 Godot Shader 语言的细节请查看 [Godot 着色器参考](https://docs.godotengine.org/zh-cn/4.x/tutorials/shaders/shader_reference/index.html)。

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| shaderCode | string | "" | 该 `Shader` 使用的着色器代码 |

---

## 方法（Methods）

### 构造函数 constructor

```javascript
new UTMX.Shader(filePath: string)
```

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| filePath  | string           | 着色器的文件路径 |

### new

```javascript
new(filePath: string) -> UTMX.Shader
```

同构造函数，尝试从指定文件路径创建着色器。

**Returns** `UTMX.Shader`

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| filePath  | string           | 着色器的文件路径 |

### loadFrom

```javascript
loadFrom(filePath: string) -> void
```

从指定的文件目录读取 Shader 代码，并更新 `shaderCode`。

**Returns** `void`

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| filePath  | string           | 着色器的文件路径 |

---

### setParameter

```javascript
setParameter(param: string, value: any) -> void
```

设置当前 Shader 的指定着色器参数值。

**Returns** `void`

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| param     | string           | 着色器参数名称|
| value     | any              | 着色器参数   |

---

### getParameter

```javascript
getParameter(param: string) -> any
```

获取当前 Shader 的着色器的指定着色器参数值。

**Returns** `any`

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| param     | string           | 着色器参数名称|

#### 使用示例

该示例展示了如何在一个 DrawableObject 中加载并应用着色器，实现屏幕着色器负片效果。

```javascript
import { UTMX , Vector2 } from "UTMX";

let shader = new UTMX.Shader();
shader.shaderCode = `
shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

void fragment() {
    vec4 col = texture(SCREEN_TEXTURE, SCREEN_UV);

    col.rgb = 1.0 - col.rgb;

    COLOR = col;
}
`;

let drawableObject = UTMX.DrawableObject.new();
drawableObject.shader = shader;
drawableObject.z = 4096;
drawableObject.drawRect(new Vector2(0, 0), new Vector2(640,  480));
```