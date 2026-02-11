# GameObject

GameObject 是 UTMX 引擎内部包装的 JavaScript 对象。每个 GameObject 都绑定了一个 .Net 对象。

UTMX 中的一切 **场景实例** 都继承于它。

!> 实例化一个 `GameObject` 通常没有任何意义，因为我们找不到任何与之绑定的 .Net 实例。UTMX 通过为每个对象的 JavaScript Wrapper 类实现自己独立的 `new` 方法，从而自定义初始化逻辑。
**因此，UTMX 没有将这个类型暴露给 JavaScript**

---


## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| __instance | .Net Clr Object | null     | 与 GameObject 绑定的 Clr 对象引用 |


---

## 方法（Methods）

### new

```javascript
new() -> null
```

静态方法，实例化该 GameObject。因为没有任何与之绑定的.Net实例，所以这个函数总是返回 `null`。

**Returns** `null`

---

### destroy

```javascript
destroy() -> void
```

销毁该 GameObject。

**Returns** `void`