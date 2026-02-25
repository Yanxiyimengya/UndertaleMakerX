# Scene 模块

Scene 是 **UTMX 框架的场景管理器模块**，用于管理场景的跳转，或管理全局单例对象。

通过全局对象 `UTMX.scene` 访问。

全局单例不会随场景跳转而释放。

?> Scene 为全局模块，可在游戏任意生命周期阶段调用。

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

### removeSingleton

```javascript
removeSingleton(name) -> void
```

移除名为 `name` 的全局单例。

你通常不需要释放单例对象，单例对象的生命周期理应跟随整个游戏流程。

> 这个方法不会真正销毁单例对象，它只是将单例对象从场景树和单例列表中移除，因此你需要在移除之前手动销毁单例对象，避免内存泄露。

**Returns** `void`

| Parameter | Type   | Default | Description         |
| --------- | ------ | ------- | ------------------- |
| name      | string | -       | 单例名称     |

#### 使用示例

下面的示例代码尝试真正销毁一个名为 `MyCustomSingleton` 的单例对象。

```javascript
import { UTMX } from "UTMX";

let singleton = UTMX.scene.getSingleton("MyCustomSingleton");
if (singleton != null)
{
    singleton.destroy();
    removeSingleton("MyCustomSingleton");
}
```

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

### getCamera

```javascript
getCamera() -> Camera
```

获取当前场景有效的 Camera，注意，某些场景不存在 Camera，因此这个函数将返回 `null`。

**Returns** `Camera`

---

Camera 类型的属性与方法（如 `startShake`）请参考 [Camera](types/game-object/camera.md)。
