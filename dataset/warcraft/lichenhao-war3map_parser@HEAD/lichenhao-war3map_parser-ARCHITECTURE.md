---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "ARCHITECTURE.md"
ref: HEAD
---

# Warcraft 3 地图解析器 - 项目架构文档

## 项目概述

这是一个用于解析和操作 Warcraft 3 地图文件（.w3x/.w3m）的 TypeScript 库。该项目提供了一套完整的解析器，能够读取、修改和保存 Warcraft 3 地图文件中的各种数据。

## 项目结构

```
src/
├── common/          # 通用工具函数
├── mpq/            # MPQ 文件格式解析
├── parsers/        # 各种地图文件解析器
└── main.ts         # 示例入口文件
```

## 核心组件

### 1. MPQ 模块 (src/mpq/)
Warcraft 3 地图使用 MPQ（MoPaQ）存档格式。该模块提供了完整的 MPQ 文件操作功能：

- **MPQArchive**: 主要的 MPQ 存档类，负责加载、保存和管理存档中的文件
- **MpqFile**: 表示存档中的单个文件
- **MpqHashTable/MpqBlockTable**: MPQ 内部的哈希表和块表结构
- **MpqCrypto**: 加密/解密功能
- **explode**: MPQ 压缩算法实现

### 2. 通用工具 (src/common/)
提供基础工具函数：

- **binarystream**: 二进制数据流处理
- **math**: 数学运算工具
- **utf8**: UTF-8 编码处理
- **typecast**: 类型转换工具

### 3. 解析器模块 (src/parsers/)

#### 地图核心解析器
- **w3x/map**: 主地图解析器，继承自 `W3XParser`，处理整个地图存档

#### 地图文件格式解析器
- **w3i**: 地图基本信息 (war3map.w3i) - 包含地图名称、作者、描述、玩家设置、科技/随机单位表等
- **w3e**: 地图环境 (war3map.w3e) - 包含地形、瓦片设置、角落数据等
- **w3u**: 单位修改 (war3map.w3u) - 用于单位、物品、可破坏物、魔法效果等
- **w3d**: 装饰物修改 (war3map.w3d) - 用于装饰物、技能、升级等（使用可选整数）
- **w3o**: 组合修改文件 (war3map.w3o) - 包含所有修改的组合文件
- **w3c**: 摄像机设置 (war3map.w3c) - 摄像机参数
- **w3r**: 地区设置 (war3map.w3r) - 地图区域定义
- **w3s**: 音效设置 (war3map.w3s) - 地图音效定义
- **w3f**: 地图过滤器 (war3map.w3f) - 地图加载过滤器设置

#### 单位和物品相关
- **unitsdoo**: 单位和物品数据 (war3mapUnits.doo)
- **doo**: 装饰物和可破坏物 (war3map.doo)

#### 触发器系统
- **wtg**: 触发器数据 (war3map.wtg) - 完整的触发器系统
- **wct**: 自定义文本触发器 (war3map.wct)
- **wts**: 字符串表 (war3map.wts) - 数字到字符串的映射

#### 其他
- **imp**: 导入文件列表 (war3map.imp)
- **ini**: INI 文件解析
- **mmp**: 最小地图图标
- **shd**: 阴影文件
- **wpm**: 寻路图 (war3map.wpm)

## 主要功能

### 地图加载和保存
- 从 Uint8Array 加载 Warcraft 3 地图
- 保存修改后的地图
- 支持只读模式和编辑模式

### 文件操作
- 获取、设置、删除存档中的文件
- 重命名存档中的文件
- 导入新文件到地图

### 数据解析
- 解析地图的各种数据结构
- 提供对地图信息的访问
- 支持修改地图数据

## 使用模式

### 核心流程
1. 创建 `W3XParser` 实例并加载地图数据
2. 使用相应的解析器读取特定数据
3. 修改数据
4. 保存地图

### 数据访问
- 通过 `getMapInformation()` 获取地图基本信息
- 通过 `readTriggers()` 读取触发器
- 通过 `readModifications()` 读取所有修改表
- 通过 `getScriptFile()` 获取脚本文件

## 技术特点

- **TypeScript**: 完全使用 TypeScript 编写，提供类型安全
- **浏览器兼容**: 可以在浏览器环境中运行
- **内存效率**: 高效的二进制数据处理
- **完整支持**: 支持 Warcraft 3 地图的大部分文件格式
- **MPQ 支持**: 完整的 MPQ 存档格式支持