# 项目结构

本页主要介绍 UndertaleMakerX 默认的项目文件结构。

```md
项目根目录 
├── utmx.cfg                # 项目元信息（名称、图标、引擎版本、最后打开时间） 
├── icon.svg                # 项目图标（创建时自动生成） 
├── project_config.json     # 运行时配置
├── main.js                 # 主脚本
└── ...
```

## project_config.json

`project_config.json` 指定运行时的配置，会在打包后由运行时读取

```javascript
{
	"application": {
		"author": "",                           // 作者名称
		"main_scene": "",                       // 主场景（留空即可）
		"main_script": "main.js",               // 主脚本路径
		"max_fps": 60.0,                        // 游戏的FPS
		"name": "UNDERTALE MAKER X",            // 游戏名称，这会影响窗口名称
		"vsync": false                          // 垂直同步模式
	},
	"boot_splash": {
		"background_color": "#101020",        // 启动页面背景颜色
		"display_text": "UNDERTALE MAKER\nX",   // 启动页面显示的文本
		"enabled": false,                       // 是否启用启动页面
		"speed_scale": 1.0                      // 启动页面动画速度
	},
	"virtual_input": {
		"dead_zone": 0.1,                       // 虚拟摇杆死区
		"enabled": false                        // 是否启用虚拟摇杆
	},
	"window": {
		"boderless": false,                     // 无边框窗口
		"clear_color": "#000000",             // 背景颜色
		"fullscreen": false,                    // 全屏模式
		"width": 640.0                          // 窗口宽度
		"height": 480.0,                        // 窗口高度
		"resizable": false,                     // 是否允许调整大小
	}
}
```