# UndertaleMakerX
[![License](https://img.shields.io/github/license/Yanxiyimengya/UndertaleMakerX.svg?style=flat-square)](LICENSE)
[![Godot Engine](https://img.shields.io/badge/基于-Godot_4.x-478CBF.svg?style=flat-square)](https://godotengine.org/)
[![Development Status](https://img.shields.io/badge/开发状态-开发中-FFD700.svg?style=flat-square)](https://github.com/Yanxiyimengya/UndertaleMakerX)
[![Latest Release](https://img.shields.io/github/v/release/Yanxiyimengya/UndertaleMakerX?style=flat-square&label=最新版本&color=9370DB&include_prereleases&allow_blank=true)](https://github.com/Yanxiyimengya/UndertaleMakerX/releases)
[![Bilibili](https://img.shields.io/badge/Bilibili-烟汐忆梦_YM-FF8800.svg?style=flat-square)](https://space.bilibili.com/481430814)
[![GitHub Stars](https://img.shields.io/github/stars/Yanxiyimengya/UndertaleMakerX.svg?style=flat-square&label=收藏)](https://github.com/Yanxiyimengya/UndertaleMakerX/stargazers)
[![GitHub Issues](https://img.shields.io/github/issues/Yanxiyimengya/UndertaleMakerX.svg?style=flat-square&label=问题)](https://github.com/Yanxiyimengya/UndertaleMakerX/issues)

![UndertaleMakerX图标](icons/utmx-icon-256.svg)

一款基于 **Godot Engine 4.x** 开发的、高灵活性的跨平台粉丝向 Undertale 风格游戏制作框架，旨在突破传统UT同人游戏制作工具的限制，同时支持**零代码开发**与**脚本化扩展**，规划实现桌面、移动等平台的导出能力，让不同技术水平的创作者都能轻松制作UT风格游戏。

## 🎯 核心设计目标
UndertaleMakerX 的设计初衷是降低 Undertale 风格游戏的制作门槛，同时为高阶开发者保留足够的定制扩展空间：
- **新手友好**：通过可视化配置、拖拽式资源管理实现零代码开发，无需编程基础即可上手
- **跨平台兼容**：规划支持一键导出至Windows/macOS/Linux（桌面）、Android（移动）、HTML5（网页）
- **UT原生还原**：内置的Undertale战斗系统、美术风格与音视觉资源的适配支持
- **高度可扩展**：模块化架构设计，支持自定义资源包、脚本扩展与社区内容贡献

<img width="320" height="240" alt="游戏页面" src="https://github.com/user-attachments/assets/bb3d78e0-eb04-449f-a4e2-00c4d12191ee" />

## 🛠️ 规划功能（开发中）
### 双开发模式
- **零代码可视化模式**（优先开发）
  - 游戏基础配置可视化面板（分辨率、战斗规则、角色属性等）
  - 预置UT风格资源库（经典角色、战斗UI、音效、背景等）
  - 战斗/大地图场景的拖拽式编辑功能
  - 游戏场景一键预览（无需编写任何代码）

- **脚本扩展模式**（设计中）
  - 支持JavaScript，实现自定义业务逻辑
  - 开放战斗系统核心API（攻击模式、对话系统、战斗流程等）
  - 支持自定义角色AI，甚至可实现非UT风格的创新玩法
  - 自定义脚本与第三方类库的导入/导出能力

### 跨平台支持（开发路线）
- 桌面端：Windows（EXE）、macOS（APP）、Linux（x86_64/AppImage）
- 移动端：Android（APK）

### 资源系统（规划中）
- 官方UT原生资源包（经典角色、战斗UI、音频素材等）
- 模板系统（经典战斗、大地图、解谜玩法模板）
- 社区资源提交与共享机制

## 📋 开发路线图
| 开发阶段 | 当前状态 | 核心开发重点 | 预计完成时间 |
|----------|----------|--------------|--------------|
| 第一阶段 | 开发中   | 核心框架搭建、零代码战斗系统原型 | 待定 |
| 第二阶段 | 规划中   | 资源库集成、场景预览功能实现 | 待定 |
| 第三阶段 | 规划中   | 脚本扩展API、跨平台导出基础 | 待定 |
| 第四阶段 | 规划中   | 社区功能、资源共享、完整文档 | 待定 |

## 🔧 技术栈
- **核心引擎**：Godot Engine 4.x（GDScript为主，支持C#/JavaScript扩展）
- **架构设计**：模块化设计（核心引擎 / 资源管理器 / 导出模块）
- **构建系统**：Godot Export Templates（跨平台打包）

## 🤝 贡献指南
UndertaleMakerX 是开源项目，欢迎社区所有开发者参与贡献。因项目目前处于早期开发阶段，现阶段主要接受以下形式的贡献：
- 通过 [GitHub Issues](https://github.com/Yanxiyimengya/UndertaleMakerX/issues) 提交bug反馈与功能建议
- 参与项目设计方案、技术实现的讨论
- 核心框架稳定后，接受代码提交与PR
- UT风格素材、制作模板、项目文档的创作与提交

## 📌 作者主页
- **GitHub**：[Yanxiyimengya](https://github.com/Yanxiyimengya)
- **Bilibili**：[https://space.bilibili.com/481430814](https://space.bilibili.com/481430814)

## 📄 许可证
本项目基于 MIT 许可证开源 - 详情请查看 [LICENSE](LICENSE) 文件。

## ❗ 重要声明
1. UndertaleMakerX 是**非商业用途**的粉丝向制作工具，仅用于学习与交流
2. 所有与Undertale相关的知识产权（角色、素材、玩法等）均归属于 Toby Fox
3. 本项目严格遵循Toby Fox发布的Undertale同人内容创作规范
4. 禁止使用本工具制作违反法律法规、公序良俗及原作者规范的内容
