# Sprite

Sprite 允许渲染一张静态精灵，或一组动态序列帧。

通过 `UTMX.Sprite` 访问。

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| texture | string / string[] | ""     | 该 `Sprite` 使用的纹理，如果传入了字符串数组，则会显示序列帧 |
| offset | Vector2 | (0, 0) | 该 `Sprite` 渲染时基于原点的偏移值 |
| frame | number | 0 | 当前帧 |
| frameSpeed | number | 1 | 序列帧的播放速度 |
| loop | boolean | true | 是否循环播放序列帧动画 |
| color | Color | Color.White     | 该 `Sprite` 的颜色 |
| shader | UTMX.Shader | null     | 该 `Sprite` 使用的着色器 |

---

## 方法（Methods）

### new

```javascript
new() -> Sprite | null
```

静态方法，实例化该 Sprite。

**Returns** `Sprite`

---

### play

```javascript
play() -> void
```

播放该精灵的序列帧动画。

**Returns** `void`

---

### stop

```javascript
stop() -> void
```

停止该精灵的序列帧动画。这会使该精灵的 `frame` 重置为 `0`。

**Returns** `void`

---

### pause

```javascript
pause() -> void
```

暂停当前序列帧动画，冻结 `frame`。

**Returns** `void`

---

### resume

```javascript
resume() -> void
```

恢复播放当前序列帧动画。

**Returns** `void`

---

### isPlaying

```javascript
isPlaying() -> boolean
```

返回序列帧动画是否正在播放。

**Returns** `boolean`