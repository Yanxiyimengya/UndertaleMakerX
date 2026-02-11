#  UndertaleMakerX 模块参考
UndertaleMakerX 将所有引擎功能全部集成到 `UTMX` 包中的 `UTMX` 模块导入类内。

我们可以通过以下方式引入 UTMX 主包的模块导入类。

```javascript
import { UTMX } from "UTMX"; // 引入 UTMX 包
```

模块导入类通过静态变量作为类成员属性，导出内部对应功能的模块类。

在某些模块内部还会包含一些 **子模块** ，这些子模块也是以静态访问的，例如我们可以通过 `UTMX.player.inventory` 访问 `Player` 模块的 `Inventroy` 子模块。

### 一些限制

!> 从模块中直接返回的类型大多数不是 UTMX 封装的类型，而是与之对应的 `Godot .Net Clr` 类型，

因此，如果需要在执行 UTMX 拓展属性/方法，请在引擎返回的对应类型中使用对应类型的 `copy` 方法，例如：

```javascript
import { UTMX } from "UTMX";

let mousePosition = new Vector2().copy(UTMX.input.getMousePosition());

UTMX.debug.log(mousePosition.add(Vector2.One)); // 这样就可以调用 UTMX 为 Vector2 包装的方法了。
```

已知的类型有:
- [Vector2](/types/vector2)
- [Vector3](/types/vector3)
- [Vector4](/types/vector4)
- [Color](/types/color)

## 模块列表

在下方列表中，你可以预览 UTMX 提供的所有模块内容：

- [Audio 模块](modules/audio.md)
- [Battle 模块](modules/battle.md)
- [Debugger 模块](modules/debugger.md)
- [GameRegisterDB 模块](modules/game-register-db.md)
- [Input 模块](modules/input.md)
- [Player 模块](modules/player.md)
- [Scene 模块](modules/scene.md)