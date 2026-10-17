---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "README.md"
ref: HEAD
---

# Warcraft 3 地图解析器

这是一个用于解析和操作 Warcraft 3 地图文件（.w3x/.w3m）的 TypeScript 库。

## 功能特性

- 解析 Warcraft 3 地图的各种文件格式
- 读取和修改地图数据
- 支持 MPQ 存档格式
- 完整的触发器系统支持
- 浏览器和 Node.js 双端支持

## 安装

```bash
# 使用 pnpm
pnpm install

# 使用 npm
npm install

# 使用 yarn
yarn install
```

## 快速开始

### 浏览器中使用

```html
<!DOCTYPE html>
<html>
<head>
    <title>Warcraft 3 地图解析器示例</title>
</head>
<body>
    <input type="file" id="map" accept=".w3x,.w3m">
    <script type="module">
        import Map from './dist/parsers/w3x/index.js'

        document.getElementById('map').addEventListener('change', async (event) => {
            const input = event.target;

            if (input.files && input.files.length > 0) {
                const file = input.files[0];
                const arrayBuffer = await file.arrayBuffer();
                const map = new Map(new Uint8Array(arrayBuffer));
                console.log(map);
            }
        });
    </script>
</body>
</html>
```

### 基本用法

```typescript
import Map from './src/parsers/w3x/index';

// 从 Uint8Array 加载地图
const mapData = new Uint8Array(/* ... 从文件或其他来源获取的数据 ... */);
const map = new Map(mapData);

// 读取地图基本信息
const info = map.getMapInformation();
console.log('地图名称:', info.name);
console.log('地图作者:', info.author);
console.log('地图描述:', info.description);

// 读取触发器
const triggers = map.readTriggers(triggerData);
if (triggers) {
    console.log('触发器数量:', triggers.triggers.length);
}

// 读取自定义文本触发器
const customTriggers = map.readCustomTextTriggers();
if (customTriggers) {
    console.log('自定义触发器:', customTriggers);
}

// 读取字符串表
const stringTable = map.readStringTable();
if (stringTable) {
    console.log('字符串映射:', stringTable.stringMap);
}

// 读取所有修改表（单位、物品、技能等）
const modifications = map.readModifications();
console.log('单位修改:', modifications.w3u);
console.log('物品修改:', modifications.w3t);
console.log('技能修改:', modifications.w3a);

// 保存修改后的地图
const modifiedMapData = map.save();
```

## 支持的文件格式

### 地图信息
- **w3i**: 地图基本信息（名称、作者、玩家设置等）
- **w3e**: 地图环境（地形、瓦片设置等）
- **w3f**: 地图过滤器设置

### 单位和物品
- **w3u**: 单位修改
- **w3t**: 物品修改
- **w3b**: 可破坏物修改
- **unitsdoo**: 单位和物品数据

### 技能和效果
- **w3a**: 技能修改
- **w3h**: 魔法效果修改
- **w3q**: 升级修改
- **w3d**: 装饰物修改

### 触发器系统
- **wtg**: 触发器数据
- **wct**: 自定义文本触发器
- **wts**: 字符串表

### 其他
- **w3c**: 摄像机设置
- **w3r**: 地区设置
- **w3s**: 音效设置
- **w3o**: 组合修改文件
- **wpm**: 寻路图
- **doo**: 装饰物和可破坏物
- **imp**: 导入文件列表

## API 参考

### W3XParser 类

#### 构造函数
```typescript
new W3XParser(buffer: Uint8Array, readonly: boolean = true)
```

- `buffer`: 包含地图数据的 Uint8Array
- `readonly`: 是否以只读模式打开地图（默认为 true）

#### 方法

##### `save(): Uint8Array`
保存地图并返回新的 Uint8Array 数据。

##### `getFileNames(): string[]`
获取存档中所有文件的名称。

##### `getImportNames(): string[]`
获取地图中导入的文件名称列表。

##### `get(name: string): MpqFile | null`
获取存档中的特定文件。

##### `has(name: string): boolean`
检查存档中是否存在特定文件。

##### `set(name: string, buffer: ArrayBuffer | string): boolean`
设置存档中的文件（需要非只读模式）。

##### `delete(name: string): boolean`
删除存档中的文件（需要非只读模式）。

##### `rename(name: string, newName: string): boolean`
重命名存档中的文件（需要非只读模式）。

##### `import(name: string, buffer: ArrayBuffer | string): boolean`
导入文件到地图（需要非只读模式）。

##### `getMapInformation(): War3MapW3i`
获取地图的基本信息。

##### `readTriggers(triggerData: TriggerData): War3MapWtg | undefined`
读取地图触发器。

##### `readCustomTextTriggers(): War3MapWct | undefined`
读取自定义文本触发器。

##### `readStringTable(): War3MapWts | undefined`
读取字符串表。

##### `readModifications(): War3MapModifications`
读取所有修改表（单位、物品、技能等）。

## 开发

### 构建项目

```bash
# 开发模式
npm run dev

# 构建
npm run build
```

### 项目结构

- `src/common/`: 通用工具函数
- `src/mpq/`: MPQ 存档格式解析
- `src/parsers/`: 各种地图文件解析器
  - `w3x/`: 主地图解析器
  - `w3i/`: 地图信息解析器
  - `w3e/`: 环境解析器
  - 等等...

## 贡献

欢迎提交 Issue 和 Pull Request 来改进这个项目。

## 许可证

[请在此处添加许可证信息]