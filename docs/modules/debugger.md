# Debugger 模块

Debugger 是 **UTMX 框架的调试日志管理核心模块**，用于统一输出游戏运行时日志信息，便于开发阶段进行调试与问题排查。

支持日志类型：

* 普通日志
* 警告信息
* 错误信息

通过全局对象 `UTMX.debug` 访问。

?> Debugger 为全局模块，可在游戏任意生命周期阶段调用。

---

## 日志方法

### print

```javascript
print(...message: any[]) -> void
```

输出普通日志信息，支持传入多个参数进行拼接输出。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| message   | any[] | 要输出的日志内容（支持多个参数） |

---

### log

```javascript
log(...message: any[]) -> void
```

`print` 方法的别名，功能完全一致。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| message   | ...any | 要输出的日志内容 |

---

### warning

```javascript
warning(...message: any[]) -> void
```

输出警告级别日志信息，通常用于提示非致命性问题。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| message   | any[] | 要输出的警告内容（支持多个参数） |

---

### error

```javascript
error(...message: any[]) -> void
```

输出错误级别日志信息，用于提示严重问题或运行时异常。

**Returns** `void`

| Parameter | Type  | Description      |
| --------- | ----- | ---------------- |
| message   | any[] | 要输出的错误内容（支持多个参数） |

---

#### 使用示例

```javascript
import { UTMX } from "UTMX";

// 输出普通日志
UTMX.debug.log("玩家等级提升至：", 5);

// 输出警告日志
UTMX.debug.warning("物品 ID 未注册");

// 输出错误日志
UTMX.debug.error("战斗系统初始化失败");
```