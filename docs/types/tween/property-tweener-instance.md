# PropertyTweenerInstance

PropertyTweenerInstance 由 [TweenInstance](/types/tween/tween-instance.md) 创建，会随着时间对指定的 JavaScript 对象的属性进行插值。

!> 只有通过 `TweenInstance` 创建的 PropertyTweenerInstance 才有效，手动创建的 PropertyTweenerInstance 无法正常运行。

---

## 方法（Methods）

### from

```javascript
from(value: any) -> PropertyTweenerInstance
```

设置补间动画的起始值，若未设置则使用属性当前值作为起始值。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| value     | number/Vector2/Vector3/Vector4/Color| 补间的起始值     |

**Returns** `PropertyTweenerInstance`

---

### trans

```javascript
trans(trans: number) -> PropertyTweenerInstance
```

设置补间动画的过渡类型，对应 `TransitionType` 枚举值。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| trans     | UTMX.tween.TransitionType           | 过渡类型枚举值|

**Returns** `PropertyTweenerInstance`

---

### ease

```javascript
ease(ease: number) -> PropertyTweenerInstance
```

设置补间动画的缓动类型，对应 `EaseType` 枚举值。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| ease      | UTMX.tween.EaseType           | 缓动类型枚举值|

**Returns** `PropertyTweenerInstance`

---

### callback

```javascript
callback(callback: function)-> PropertyTweenerInstance
```

设置当前补间完成所有补间时执行的回调函数，该回调函数不接收任何参数和返回值。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| callback  | function         | 补间完成后执行的回调函数 |

**Returns** `PropertyTweenerInstance`

---

### asRelative

```javascript
asRelative() -> PropertyTweenerInstance
```

将补间的目标值设置为相对值（基于起始值的偏移量），而非绝对值。

**Returns** `PropertyTweenerInstance`

---