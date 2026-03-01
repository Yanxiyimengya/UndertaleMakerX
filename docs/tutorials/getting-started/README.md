# 入门

这个系列讲述如何下载安装，配置使用 UndertaleMakerX

## 下载

- 在 UndertaleMakerX 的发布页中，你可以找到最新的发布版本。

- 点击下载符合当前操作系统的编辑器。

- 对应平台的编辑器通常已经附带相关的运行时可执行程序。

- 解压压缩包到一个空的目录即可运行。


运行 UndertaleMakerX ，引擎会在可执行文件同级目录创建 `editor_data` 目录，这是编辑器的数据目录。

```md
editor_data
├── .build_cache            # 项目数据包的构建缓存
├── .cache                  # 编辑器资源导入数据缓存
├── .undo_trash             # 文件操作后撤销文件暂存的目录
├── runner                  # 运行时目录
└── projects.cfg            # 编辑器打开的项目列表配置
```