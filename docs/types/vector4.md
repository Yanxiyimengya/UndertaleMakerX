# Vector4

Vector4 是包含四个 `number` 分量的四维向量类型，可用于表示 3D 扩展坐标（如齐次坐标）或任意四元数值组合。

该类型为 Jint 引擎对 Godot `Vector4` 的包装对象，因此可以直接访问 Godot API 中的大部分属性与方法。
本文档仅列出 UTMX 层扩展或与 Godot 存在差异的部分。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| x        | number | `0`     | 向量的 X 分量    |
| y        | number | `0`     | 向量的 Y 分量    |
| z        | number | `0`     | 向量的 Z 分量    |
| w        | number | `0`     | 向量的 W 分量    |

---

## 方法（Methods）

### 构造函数 constructor

```javascript
new Vector4()
```

构造一个Vector4零向量。

---

```javascript
new Vector4(x: number, y: number, z: number, w: number)
```

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| x         | number           | 向量的 X 分量 |
| y         | number           | 向量的 Y 分量 |
| z         | number           | 向量的 Z 分量 |
| w         | number           | 向量的 W 分量 |

---

```javascript
new Vector4(from: Vector4)
```

同 `copy`，构造给定向量的副本。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| from      | Vector4             | 目标向量 |

---

### add

```javascript
add(value: number | Vector4) -> Vector4
```

替代 Godot 加法运算符重载。

* 传入 `number` 时：对每个分量执行数值相加
* 传入 `Vector4` 时：对每个分量执行对应相加

**Returns**
`Vector4` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector4 | 加数          |

---

### subtract

```javascript
subtract(value: number | Vector4) -> Vector4
```

替代 Godot 减法运算符重载。

* 传入 `number` 时：对每个分量执行数值相减
* 传入 `Vector4` 时：对每个分量执行对应相减

**Returns**
`Vector4` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector4 | 减数          |

---

### multiply

```javascript
multiply(value: number | Vector4) -> Vector4
```

替代 Godot 乘法运算符重载。

* 传入 `number` 时：对每个分量执行数值缩放
* 传入 `Vector4` 时：对每个分量执行对应相乘

**Returns**
`Vector4` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector4 | 乘数          |

---

### divide

```javascript
divide(value: number | Vector4) -> Vector4
```

替代 Godot 除法运算符重载。

* 传入 `number` 时：对每个分量执行数值相除
* 传入 `Vector4` 时：对每个分量执行对应相除

**Returns**
`Vector4` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector4 | 除数          |

---

### copy

```javascript
copy(value: Vector4) -> Vector4
```

将指定 Vector4 类型中的每个分量拷贝到自身

**Returns**
`Vector4` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | Vector4          | 目标 Vector4 |

---

## 其他 API

完整功能请参考 Godot 官方文档中的 `Vector4` 类说明。

[Godot 中的 Vector4](https://docs.godotengine.org/zh-cn/stable/classes/class_vector4.html)

---

#### 使用示例

```javascript
import { UTMX, Vector4 } from "UTMX";

let pos1 = new Vector4(1, 0, 0, 0);
let pos2 = new Vector4(0, 1, 0, 0);

UTMX.debug.log(
    pos1.multiply(2).subtract(pos2)
);
```