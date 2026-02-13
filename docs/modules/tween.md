# Tween 模块

Tween 是 **UTMX 框架的补间动画模块**，用于创建需要将一个数值属性插值到一系列值的补间动画。

Tween 是对 Godot 常规补间动画的封装与拓展，向 JavaScript 提供接口，并允许对 JavaScript 对象属性进行补间，支持 `number` / `Vector2` / `Vector3` / `Vector4` / `Color` 等类型补间。

?> Tween 为全局模块，可在游戏任意生命周期阶段调用。

---

## 常量（Constants）

通过补间的插值方式和缓动方式，你可以组合出多种补间动画效果，具体可查看在线网站 [缓动函数速查表](https://easings.net/zh-cn)。

### TransitionType

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| Linear | number | 0       | 动画使用线性插值 |
| Sine | number | 1       | 动画使用正弦函数插值 |
| Quint | number | 2       | 动画使用五次函数（5次方）插值 |
| Quart | number | 3       | 动画使用四次函数（4次方）插值 |
| Quad | number | 4       | 动画使用二次函数（2次方）插值 |
| Expo | number | 5       | 动画使用指数函数插值 |
| Elastic | number | 6       | 动画使用弹性插值，在边缘处摆动 |
| Cubic | number | 7       | 动画使用三次函数（3次方）插值 |
| Circ | number | 8       | 动画使用平方根函数插值 |
| Bounce | number | 9       | 动画在结束时以弹跳方式插值 |
| Back | number | 10      | 动画在两端处回退插值 |
| Spring | number | 11      | 动画在结束时以弹簧方式插值 |

### EaseType

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| In | number | 0       | 插值开始缓慢，向结束方向加速 |
| Out | number | 1       | 插值开始快速，向结束方向减速 |
| InOut | number | 2       | 结合In和Out，插值在两端最慢 |
| OutIn | number | 3       | 结合In和Out，插值在两端最快 |

---

## 方法（Methods）

### createTween

```javascript
createTween() -> TweenInstance
```

创建一个补间动画。返回 `TweenInstance`。

**Returns** `TweenInstance`

---

### getTweenList

```javascript
getTweenList() -> TweenInstance[]
```

返回所有存在的 `TweenInstance` 对象，包括已暂停的动画。

**Returns** `TweenInstance[]`

---