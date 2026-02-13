# Game 模块

Game 是 **UTMX 引擎进程核心模块**，用于管理游戏进程整体。

通过 `UTMX.game` 访问

## FPS 管理

### getFpsReal

```javascript
getFpsReal() -> number
```

获取游戏进程的 FPS（每秒帧数）。

**Returns** `number`

---

### setMaxFps

```javascript
setMaxFps(fps: number) -> void
```

获取游戏进程的 FPS（每秒帧数）。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| fps       | number | 游戏进程的最大帧数 |

## 游戏生命周期

### quitGame

```javascript
quitGame() -> void
```

直接退出当前游戏进程。

**Returns** `void`
