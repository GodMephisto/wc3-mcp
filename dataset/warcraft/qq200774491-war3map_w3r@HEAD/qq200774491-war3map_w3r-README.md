---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "README.md"
ref: HEAD
---

# war3map_w3r

Warcraft III 矩形区域文件 (`war3map.w3r`) 解析工具。将二进制区域数据解析并生成带有 LuaCATS 类型注解的 Lua 数据文件。

## 编译

### 环境要求

- Windows 10+
- Visual Studio 2022（需要 C++20 支持）

### 使用 MSVC 编译

```batch
:: 打开 "x64 Native Tools Command Prompt for VS 2022"，然后执行：
cd D:\code2\war3map_w3r
cl /std:c++20 /utf-8 /EHsc /O2 /Fe:war3map_w3r.exe src\main.cpp src\binary_reader.cpp src\w3r_parser.cpp src\lua_generator.cpp /Isrc
```

### 使用 xmake 编译（如已安装）

```bash
xmake f -m release
xmake build
```

## 使用方法

```
war3map_w3r.exe <输入文件.w3r> [输出文件.lua]
```

| 参数 | 说明 | 是否必填 |
|------|------|----------|
| `<输入文件.w3r>` | war3map.w3r 文件路径 | 必填 |
| `[输出文件.lua]` | 输出 Lua 文件路径，省略则自动生成同名 `.lua` | 可选 |

### 示例

```bash
# 指定输出路径
war3map_w3r.exe war3map.w3r regions.lua

# 省略输出路径，自动生成 war3map.lua
war3map_w3r.exe war3map.w3r

# 解析其他目录的文件
war3map_w3r.exe D:\maps\mymap\war3map.w3r D:\output\regions.lua
```

### 运行输出

```
Parsing: war3map.w3r
Version: 5
Regions: 158
Output:  war3map.lua
```

## 输出格式

生成的 Lua 文件包含完整的 LuaCATS 类型注解，可被 LuaLS (Lua Language Server) 识别，提供代码补全和类型检查。

```lua
-- Generated from: war3map.w3r (version 5)

---@class W3RColor
---@field b integer 蓝色通道 (0-255)
---@field g integer 绿色通道 (0-255)
---@field r integer 红色通道 (0-255)
---@field a integer 透明度通道 (0-255)

---@class W3RRegion
---@field left number 左边界坐标
---@field bottom number 下边界坐标
---@field right number 右边界坐标
---@field top number 上边界坐标
---@field name string 区域名称
---@field creation_number integer 区域唯一标识符
---@field weather_effect_id string 天气效果 FourCC 编码
---@field ambient_sound string 环境音效引用 ID
---@field color W3RColor 编辑器显示颜色 (BGRA)

---@type W3RRegion[]
local regions = {
    {
        left = -12352.0,
        bottom = -3136.0,
        right = -12000.0,
        top = -2848.0,
        name = "T1",
        creation_number = 69,
        weather_effect_id = "",
        ambient_sound = "",
        color = { b = 255, g = 128, r = 128, a = 255 },
    },
    -- ...
}

return regions
```

## 解析的字段说明

### W3RRegion（矩形区域）

| 字段 | 类型 | 说明 |
|------|------|------|
| `left` | `number` | 左边界坐标 |
| `bottom` | `number` | 下边界坐标 |
| `right` | `number` | 右边界坐标 |
| `top` | `number` | 上边界坐标 |
| `name` | `string` | 区域名称 |
| `creation_number` | `integer` | 区域唯一标识符 |
| `weather_effect_id` | `string` | 天气效果 FourCC 编码（如 `"RLlr"` 表示下雨，空字符串表示无天气） |
| `ambient_sound` | `string` | 环境音效引用 ID（对应 war3map.w3s 中的声音定义） |
| `color` | `W3RColor` | 编辑器中显示的区域颜色 (BGRA 格式) |

### W3RColor（颜色）

| 字段 | 类型 | 说明 |
|------|------|------|
| `b` | `integer` | 蓝色通道 (0-255) |
| `g` | `integer` | 绿色通道 (0-255) |
| `r` | `integer` | 红色通道 (0-255) |
| `a` | `integer` | 透明度通道 (0-255) |

## 项目结构

```
war3map_w3r/
├── xmake.lua                # xmake 构建配置
├── src/
│   ├── main.cpp             # CLI 入口
│   ├── binary_reader.h/cpp  # 二进制数据读取器
│   ├── w3r_parser.h/cpp     # war3map.w3r 解析器
│   └── lua_generator.h/cpp  # Lua 文件生成器
└── README.md
```

## 支持的文件版本

支持 war3map.w3r version 5（当前魔兽争霸III 1.27的版本）。所有数据均为小端序存储。
