# Vector3

Vector3 是包含三个 `number` 分量的三维向量类型，可用于表示 3D 坐标或任意三元数值组合。

该类型为 Jint 引擎对 Godot `Vector3` 的包装对象，因此可以直接访问 Godot API 中的大部分属性与方法。
本文档仅列出 UTMX 层扩展或与 Godot 存在差异的部分。

---

## 常量（Constants）

?> 可以使用常量快速初始化特定值的 Vector3

<br>

| Name                | Type    | Description                                   |
| ------------------- | ------- | --------------------------------------------- |
| Vector3.Zero        | Vector3 | `Vector3(0, 0, 0)`，零向量                        |
| Vector3.One         | Vector3 | `Vector3(1, 1, 1)`，一向量                        |
| Vector3.Inf         | Vector3 | `Vector3(Infinity, Infinity, Infinity)`，无穷大向量 |
| Vector3.Left        | Vector3 | `Vector3(-1, 0, 0)`，左单位向量                     |
| Vector3.Right       | Vector3 | `Vector3(1, 0, 0)`，右单位向量                      |
| Vector3.Up          | Vector3 | `Vector3(0, 1, 0)`，上单位向量                      |
| Vector3.Down        | Vector3 | `Vector3(0, -1, 0)`，下单位向量                     |
| Vector3.Forward     | Vector3 | `Vector3(0, 0, -1)`，前单位向量                     |
| Vector3.Back        | Vector3 | `Vector3(0, 0, 1)`，后单位向量                      |
| Vector3.ModelLeft   | Vector3 | `Vector3(1, 0, 0)`，模型空间左方向                    |
| Vector3.ModelRight  | Vector3 | `Vector3(-1, 0, 0)`，模型空间右方向                   |
| Vector3.ModelTop    | Vector3 | `Vector3(0, 1, 0)`，模型空间上方向                    |
| Vector3.ModelBottom | Vector3 | `Vector3(0, -1, 0)`，模型空间下方向                   |
| Vector3.ModelFront  | Vector3 | `Vector3(0, 0, 1)`，模型空间前方向                    |
| Vector3.ModelRear   | Vector3 | `Vector3(0, 0, -1)`，模型空间后方向                   |

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| x        | number | `0`     | 向量的 X 分量    |
| y        | number | `0`     | 向量的 Y 分量    |
| z        | number | `0`     | 向量的 Z 分量    |

---

## 方法（Methods）

### add

```javascript
add(value: number | Vector3) -> Vector3
```

替代 Godot 加法运算符重载。

* 传入 `number` 时：对每个分量执行数值相加
* 传入 `Vector3` 时：对每个分量执行对应相加

**Returns**
`Vector3` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector3 | 加数          |

---

### subtract

```javascript
subtract(value: number | Vector3) -> Vector3
```

替代 Godot 减法运算符重载。

* 传入 `number` 时：对每个分量执行数值相减
* 传入 `Vector3` 时：对每个分量执行对应相减

**Returns**
`Vector3` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector3 | 减数          |

---

### multiply

```javascript
multiply(value: number | Vector3) -> Vector3
```

替代 Godot 乘法运算符重载。

* 传入 `number` 时：对每个分量执行数值缩放
* 传入 `Vector3` 时：对每个分量执行对应相乘

**Returns**
`Vector3` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector3 | 乘数          |

---

### divide

```javascript
divide(value: number | Vector3) -> Vector3
```

替代 Godot 除法运算符重载。

* 传入 `number` 时：对每个分量执行数值相除
* 传入 `Vector3` 时：对每个分量执行对应相除

**Returns**
`Vector3` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector3 | 除数          |

---

### copy

```javascript
copy(value: Vector3) -> Vector3
```

将指定 Vector3 类型中的每个分量拷贝到自身

**Returns**
`Vector3` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | Vector3          | 目标 Vector3 |

---

## 其他 API

完整功能请参考 Godot 官方文档中的 `Vector3` 类说明。

[Godot 中的 Vector3](https://docs.godotengine.org/zh-cn/stable/classes/class_vector3.html)

---

#### 使用示例

```javascript
import { UTMX, Vector3 } from "UTMX";

let pos = Vector3.One;

UTMX.debug.log(pos.length());
```
