# Vector2
包含两个 ${$.var.typeReferenceNumber} 元素的，可用于代表 2D 坐标或任何数值的二元组。

这个类实际上只是 Jint 引擎对 Godot Vector2 类的包装，因此你可以通过 Godot API 直接访问其内部的任何属性/方法。

关于与 Godot API 的差异部分，本文档会列出。

## 常量
- Vector2.Zero = `Vector2(0, 0)`

    零向量，所有分量都设置为 `0` 的向量。<br><br>

- Vector2.One = `Vector2(1, 1)`

    一向量，所有分量都设置为 `1` 的向量。<br><br>

- Vector2.Inf = `Vector2(Infinity, Infinity)`

    无穷大向量，所有分量都设置为 `Infinity` 的向量。<br><br>

- Vector2.Left = `Vector2(-1, 0)`

    左单位向量。代表左的方向。<br><br>

-  Vector2.Right = `Vector2(1, 0)`

    右单位向量。代表右的方向。<br><br>

- Vector2.Up = `Vector2(0, -1)`

    上单位向量。在 2D 中 Y 是向下的，所以这个向量指向 -Y。<br><br>

- Vector2.Down = `Vector2(0, 1)`

    下单位向量。在 2D 中 Y 是向下的，所以这个向量指向 +Y。<br><br>

## 方法列表

### add
用于替代 Godot 加法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector2 的每个分量加上该数值。  
若传入的参数是 ${$.var.typeReferenceVector2}，则将该 Vector2 的每个分量加上给定 Vector2 的对应分量。

**返回值** : ${$.var.typeReferenceVector2} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector2} | ❌ | - | 加数 |

---

### subtract
用于替代 Godot 减法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector2 的每个分量减去该数值。  
若传入的参数是 ${$.var.typeReferenceVector2}，则将该 Vector2 的每个分量减去给定 Vector2 的对应分量。

**返回值** : ${$.var.typeReferenceVector2} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector2} | ❌ | - | 减数 |

---

### multiply
用于替代 Godot 乘法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector2 的每个分量乘以该数值（缩放）。  
若传入的参数是 ${$.var.typeReferenceVector2}，则将该 Vector2 的每个分量乘以给定 Vector2 的对应分量。

**返回值** : ${$.var.typeReferenceVector2} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector2} | ❌ | - | 乘数 |

---

### divide
用于替代 Godot 除法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector2 的每个分量除以该数值。  
若传入的参数是 ${$.var.typeReferenceVector2}，则将该 Vector2 的每个分量除以给定 Vector2 的对应分量。

**返回值** : ${$.var.typeReferenceVector2} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector2} | ❌ | - | 除数 |

### 其他API

详细请阅读 [Godot 中的 Vector2](https://docs.godotengine.org/zh-cn/4.x/classes/class_vector2.html)

## 核心属性

| 属性 | 类型 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- |
| x | ${$.var.typeReferenceNumber} | `0` | 向量的 X 分量 |
| y | ${$.var.typeReferenceNumber} | `0` | 向量的 Y 分量）|

## 使用示例
```javascript
import { UTMX, Vector2 } from "UTMX"; // 从 UTMX 包中导入 Vector2 类型
let pos = Vector2.Zero; // 创建一个零向量，你也可以使用 new Vector2(0, 0) 来创建同样内容
pos.add(2);
UTMX.debug.log(pos); // 打印 (2, 2)
```