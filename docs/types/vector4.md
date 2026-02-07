# Vector3
包含三个 ${$.var.typeReferenceNumber} 元素的，可用于代表 3D 坐标或任何数值的三元组。

这个类实际上只是 Jint 引擎对 Godot Vector3 类的包装，因此你可以通过 Godot API 直接访问其内部的任何属性/方法。

关于与 Godot API 的差异部分，本文档会列出。

## 方法列表

### add
用于替代 Godot 加法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector4 的每个分量加上该数值。  
若传入的参数是 ${$.var.typeReferenceVector4}，则将该 Vector4 的每个分量加上给定 Vector4 的对应分量。

**返回值** : ${$.var.typeReferenceVector4} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector4} | ❌ | - | 加数 |

---

### subtract
用于替代 Godot 减法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector4 的每个分量减去该数值。  
若传入的参数是 ${$.var.typeReferenceVector4}，则将该 Vector4 的每个分量减去给定 Vector4 的对应分量。

**返回值** : ${$.var.typeReferenceVector4} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector4} | ❌ | - | 减数 |

---

### multiply
用于替代 Godot 乘法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector4 的每个分量乘以该数值（缩放）。  
若传入的参数是 ${$.var.typeReferenceVector4}，则将该 Vector4 的每个分量乘以给定 Vector4 的对应分量。

**返回值** : ${$.var.typeReferenceVector4} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector4} | ❌ | - | 乘数 |

---

### divide
用于替代 Godot 除法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector4 的每个分量除以该数值。  
若传入的参数是 ${$.var.typeReferenceVector4}，则将该 Vector4 的每个分量除以给定 Vector4 的对应分量。

**返回值** : ${$.var.typeReferenceVector4} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector4} | ❌ | - | 除数 |

### 其他API

详细请阅读 [Godot 中的 Vector4](https://docs.godotengine.org/zh-cn/4.x/classes/class_vector4.html)

## 核心属性

| 属性 | 类型 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- |
| x | ${$.var.typeReferenceNumber} | `0` | 向量的 X 分量 |
| y | ${$.var.typeReferenceNumber} | `0` | 向量的 Y 分量）|
| z | ${$.var.typeReferenceNumber} | `0` | 向量的 Z 分量）|
| w | ${$.var.typeReferenceNumber} | `0` | 向量的 W 分量）|

## 使用示例
```javascript
import { UTMX, Vector4 } from "UTMX"; // 从 UTMX 包中导入 Vector4 类型
let pos1 = new Vector4(1, 0, 0, 0);
let pos2 = new Vector4(0, 1, 0, 0);
UTMX.debug.log(pos1.multiply(2).subtract(pos2)); // 支持链式调用，输出 (2, -1, 0, 0)

```