# Vector3
包含三个 ${$.var.typeReferenceNumber} 元素的，可用于代表 3D 坐标或任何数值的三元组。

这个类实际上只是 Jint 引擎对 Godot Vector3 类的包装，因此你可以通过 Godot API 直接访问其内部的任何属性/方法。

关于与 Godot API 的差异部分，本文档会列出。
## 常量
- Vector3.Zero = `Vector3(0, 0, 0)`

    零向量，所有分量都设置为 0 的向量。<br><br>

- Vector3.One = `Vector3(1, 1, 1)`

    一向量，所有分量都设置为 1 的向量。<br><br>

- Vector3.Inf = `Vector3(Infinity, Infinity, Infinity)`

    无穷大向量，所有分量都设置为 @GDScript.INF 的向量。<br><br>

- Vector3.Left = `Vector3(-1, 0, 0)`

    左单位向量。代表局部的左方向，全局的西方向。<br><br>

- Vector3.Right = `Vector3(1, 0, 0)`

    右单位向量。代表局部的右方向，全局的东方向。<br><br>

- Vector3.Up = `Vector3(0, 1, 0)`

    上单位向量。<br><br>

- Vector3.Down = `Vector3(0, -1, 0)`

    下单位向量。<br><br>

- Vector3.Forward = `Vector3(0, 0, -1)`

    向前的单位向量。代表局部的前方，全局的北方。<br><br>

- Vector3.Back = `Vector3(0, 0, 1)`

    向后的单位向量。代表局部的后方，全局的南方。<br><br>

- Vector3.ModelLeft = `Vector3(1, 0, 0)`

    指向导入后 3D 资产左侧的单位向量。<br><br>

- Vector3.ModelRight = `Vector3(-1, 0, 0)`

    指向导入后 3D 资产右侧的单位向量。<br><br>

- Vector3.ModelTop = `Vector3(0, 1, 0)`

    指向导入后 3D 资产顶部（上方）的单位向量。<br><br>

- Vector3.ModelBottom = `Vector3(0, -1, 0)`

    指向导入后 3D 资产底部（下方）的单位向量。<br><br>

- Vector3.ModelFront = `Vector3(0, 0, 1)`

    指向导入后 3D 资产正面（前方）的单位向量。<br><br>

- Vector3.ModelRear = `Vector3(0, 0, -1)`

    指向导入后 3D 资产背面（后方）的单位向量。<br><br>

## 方法列表

### add
用于替代 Godot 加法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector3 的每个分量加上该数值。  
若传入的参数是 ${$.var.typeReferenceVector3}，则将该 Vector3 的每个分量加上给定 Vector3 的对应分量。

**返回值** : ${$.var.typeReferenceVector3} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector3} | ❌ | - | 加数 |

---

### subtract
用于替代 Godot 减法运算符重载腔。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector3 的每个分量减去该数值。  
若传入的参数是 ${$.var.typeReferenceVector3}，则将该 Vector3 的每个分量减去给定 Vector3 的对应分量。

**返回值** : ${$.var.typeReferenceVector3} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector3} | ❌ | - | 减数 |

---

### multiply
用于替代 Godot 乘法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector3 的每个分量乘以该数值（缩放）。  
若传入的参数是 ${$.var.typeReferenceVector3}，则将该 Vector3 的每个分量乘以给定 Vector3 的对应分量。

**返回值** : ${$.var.typeReferenceVector3} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector3} | ❌ | - | 乘数 |

---

### divide
用于替代 Godot 除法运算符重载。

若传入的参数是 ${$.var.typeReferenceNumber}，则将该 Vector3 的每个分量除以该数值。  
若传入的参数是 ${$.var.typeReferenceVector3}，则将该 Vector3 的每个分量除以给定 Vector3 的对应分量。

**返回值** : ${$.var.typeReferenceVector3} (返回自身实例以支持链式调用)

| 参数 | 类型 | 可选 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- | :---- |
| value | ${$.var.typeReferenceNumber} / ${$.var.typeReferenceVector3} | ❌ | - | 除数 |

### 其他API

详细请阅读 [Godot 中的 Vector3](https://docs.godotengine.org/zh-cn/4.x/classes/class_vector3.html)

## 核心属性

| 属性 | 类型 | 默认值 | 简介 |
| :----- | :---- | :---- | :---- |
| x | ${$.var.typeReferenceNumber} | `0` | 向量的 X 分量 |
| y | ${$.var.typeReferenceNumber} | `0` | 向量的 Y 分量）|
| z | ${$.var.typeReferenceNumber} | `0` | 向量的 Z 分量）|

## 使用示例
```javascript
import { UTMX, Vector3 } from "UTMX"; // 从 UTMX 包中导入 Vector3 类型
let pos = Vector3.One; // 创建一个一向量，你也可以使用 new Vector3(1, 1, 1) 来创建同样内容
UTMX.debug.log(pos.length()); // 打印 pos 向量的模长
```