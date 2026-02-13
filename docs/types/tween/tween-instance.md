# TweenInstance

TweenInstance 是由 `UTMX.Tween` 模块创建的补间动画对象实例，持有并维护多个 `TweenerInstance`。

当全部补间动画播放完成后，会调用自身的 [kill](#kill) 完成销毁。

!> 只有通过 UTMX.tween 模块创建的 TweenInstance 才有效，手动创建的 TweenInstance 无法正常运行。

---

## 方法（Methods）

### addTweenProperty

```javascript
addTweenProperty(target: Object, property: string, finalValue: any, duration: number) -> PropertyTweenerInstance
```

创建并追加一个 `PropertyTweenerInstance` 实例。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| target    | Object           | 目标对象     |
| property  | string           | 对象的属性名称     |
| finalValue | any（只能为number/Vector2/Vector3/Vector4/Color）| 补间的目标值     |
| duration  | number           | 补间持续的时长     |

---

## kill

```javascript
kill() -> void
```

中止所有补间动画，并使该 TweenInstance 无效。

**Returns** `void`

---

## pause

```javascript
pause() -> void
```

暂停所有补间动画，等待调用 `play` 恢复。

**Returns** `void`

---

## play

```javascript
play() -> void
```

恢复所有已被暂停的补间动画。

**Returns** `void`

---

### setParallel

```javascript
setParallel(parallel = true)-> TweenInstance
```

如果 parallel 为 true，则后续追加的 `Tweener` 默认就是同时运行的，否则默认依次运行。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| parallel  | boolean          | 是否启用并行 |

**Returns** `TweenInstance`

---

### chain

```javascript
chain()-> TweenInstance
```

如果动画已经启用并行，调用这个方法可以将前面两个已添加的 `Tweener` 串联，使其依次执行。

| Parameter | Type             | Description |
| --------- | ---------------- | ----------- |
| parallel  | boolean          | 是否启用并行 |

**Returns** `TweenInstance`

---

### isRunning

```javascript
isRunning()-> boolean
```

若该动画正在执行，返回 `true`，否则返回 `false`。

**Returns** `boolean`

---

### callback

```javascript
callback(callback: function)-> TweenInstance
```

设置当前动画持有的补间动画实例全部完成所有补间时执行的回调函数，该回调函数不接收任何参数和返回值。

**Returns** `TweenInstance`

---

