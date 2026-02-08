# Vector2

Vector2 是包含两个 `number` 分量的二维向量类型，可用于表示 2D 坐标或任意二元数值组合。

该类型为 Jint 引擎对 Godot `Vector2` 的包装对象，因此可以直接访问 Godot API 中的大部分属性与方法。
本文档仅列出 UTMX 层扩展或与 Godot 存在差异的部分。

---

## 常量（Constants）

?> 可以使用常量快速初始化特定值的 Vector2

<br>

| Name          | Type    | Description                         |
| ------------- | ------- | ----------------------------------- |
| Vector2.Zero  | Vector2 | `Vector2(0, 0)`，零向量                 |
| Vector2.One   | Vector2 | `Vector2(1, 1)`，一向量                 |
| Vector2.Inf   | Vector2 | `Vector2(Infinity, Infinity)`，无穷大向量 |
| Vector2.Left  | Vector2 | `Vector2(-1, 0)`，左单位向量              |
| Vector2.Right | Vector2 | `Vector2(1, 0)`，右单位向量               |
| Vector2.Up    | Vector2 | `Vector2(0, -1)`，上单位向量（2D 中 Y 轴向下）  |
| Vector2.Down  | Vector2 | `Vector2(0, 1)`，下单位向量（2D 中 Y 轴向下）   |

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| x        | number | `0`     | 向量的 X 分量    |
| y        | number | `0`     | 向量的 Y 分量    |

---

## 方法（Methods）

### add

```javascript
add(value: number | Vector2) -> Vector2
```

替代 Godot 加法运算符重载。

* 传入 `number` 时：对每个分量执行数值相加
* 传入 `Vector2` 时：对每个分量执行对应相加

**Returns**
`Vector2` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector2 | 加数          |

---

### subtract

```javascript
subtract(value: number | Vector2) -> Vector2
```

替代 Godot 减法运算符重载。

* 传入 `number` 时：对每个分量执行数值相减
* 传入 `Vector2` 时：对每个分量执行对应相减

**Returns**
`Vector2` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector2 | 减数          |

---

### multiply

```javascript
multiply(value: number | Vector2) -> Vector2
```

替代 Godot 乘法运算符重载。

* 传入 `number` 时：对每个分量执行数值缩放
* 传入 `Vector2` 时：对每个分量执行对应相乘

**Returns**
`Vector2` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector2 | 乘数          |

---

### divide

```javascript
divide(value: number | Vector2) -> Vector2
```

替代 Godot 除法运算符重载。

* 传入 `number` 时：对每个分量执行数值相除
* 传入 `Vector2` 时：对每个分量执行对应相除

**Returns**
`Vector2` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number | Vector2 | 除数          |

---

### copy

```javascript
copy(value: Vector2) -> Vector2
```

将指定 Vector2 类型中的每个分量拷贝到自身

**Returns**
`Vector2` — 返回当前实例

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | Vector2          | 目标 Vector2 |
---

## 其他 API

完整功能请参考 Godot 官方文档中的 `Vector2` 类说明。

[Godot 中的 Vector2](https://docs.godotengine.org/zh-cn/stable/classes/class_vector2.html)

---

#### 使用示例

```javascript
import { UTMX, Vector2 } from "UTMX";

let pos = Vector2.Zero; 
pos.add(2);

UTMX.debug.log(pos); // (2, 2)
```
