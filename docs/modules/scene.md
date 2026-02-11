# Scene 模块

Scene 是 **UTMX 框架的场景管理器模块**，用于管理场景的跳转，或管理全局单例对象。

---

### changeScene

```javascript
changeScene(path: string) -> void
```

加载场景路径 `path`，并跳转到此场景。

**Returns** `void`

| Parameter | Type   | Default | Description         |
| --------- | ------ | ------- | ------------------- |
| path    | string | -      | 跳转目标场景路径     |

---

### addSingleton

```javascript
addSingleton(name, object) -> void
```

将 `object` 添加为名为 `name` 的全局单例，全局单例不会随着场景切换被删除。

**Returns** `void`

| Parameter | Type   | Default | Description         |
| --------- | ------ | ------- | ------------------- |
| name      | string | -       | 单例名称     |
| object    | GameObject | -       | 单例对象     |

---

### getSingleton

```javascript
getSingleton(name) -> GameObject
```

尝试获取名为 `name` 的全局单例。

**Returns** `GameObject`

| Parameter | Type   | Default | Description         |
| --------- | ------ | ------- | ------------------- |
| name      | string | -       | 单例名称     |

---