# TransformableObject

继承 [GameObject](types/game-object/README.md)

具有 2D 变换的场景实例对象基类，UTMX 中的一切 **2D 场景实例** 都继承于它。

2D 场景实例通常包括：

- [DrawableObject](/types/game-object/drawable-object.md)
- [Sprite](/types/game-object/sprite.md)
- [TextTyper](/types/game-object/text-typer.md)

!> 实例化一个 `TransformableObject` 通常没有任何意义，因为没有任何与之绑定的.Net实例。
**因此，UTMX 没有将这个类型暴露给 JavaScript 中的 UTMX包**

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| position | Vector2 | 0     | 该 `TransformableObject` 的局部坐标（相对于父节点） |
| globalPosition | Vector2 | 0     | 该 `TransformableObject` 的全局坐标 |
| z | number | 0     | 该 `TransformableObject` 的渲染排序优先级，值越大代表优先级越高，它的取值范围在 `-4096` - `+4096` 之间 |
| rotation | number | 0     | 该 `TransformableObject` 的局部旋转角度（相对于父节点） |
| globalRotation | number | 0     | 该 `TransformableObject` 的全局旋转角度 |
| scale | Vector2 | 0     | 该 `TransformableObject` 的局部缩放（相对于父节点） |
| globalScale | Vector2 | 0     | 该 `TransformableObject` 的全局缩放 |
| skew | number | 0     | 该 `TransformableObject` 的局部倾斜（相对于父节点） |
| globalSkew | number | 0     | 该 `TransformableObject` 的全局倾斜 |

## 方法（Methods）

### getParent

```javascript
getParent() -> TransformableObject
```

返回当前对象的父级，如果不存在父级，则返回 `null`。

**Returns** `TransformableObject`

---

### addChild

```javascript
addChild(child: TransformableObject) -> void
```

将一个 `TransformableObject` 对象添加到子级，子 `TransformableObject` 会继承父 `TransformableObject` 的位置、缩放、旋转等。

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| child | TransformableObject | 目标子级对象   |


---

### addChild

```javascript
addChild(child: TransformableObject, resetTransform?: boolean) -> void
```

将一个 `TransformableObject` 对象添加到子级

子 `TransformableObject` 会继承父 `TransformableObject` 的位置、缩放、旋转等。

**Returns** `void`

| Parameter   | Type   | Default | Description |
| ----------- | ------ | ------- | ----------- |
| child | TransformableObject | - | 目标子级对象   |
| resetTransform | boolean | false | 是否重置子级的变换   |

---

### removeChild

```javascript
removeChild(child: TransformableObject) -> void
```

将一个子 `TransformableObject` 对象从自身子级列表中移除，这个方法不会销毁子级对象。

若 `TransformableObject` 在 UTMX 中存在旧的内部父级节点，就会将其移回原先的父级节点中。

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| child | TransformableObject | 目标子级对象   |

---