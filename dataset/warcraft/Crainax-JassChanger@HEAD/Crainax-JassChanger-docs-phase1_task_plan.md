---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase1_task_plan.md"
ref: HEAD
---

# vJass/Zinc -> JASS 编译器：阶段 1 任务计划

> 交付对象：Codex / 编码代理  
> 推荐语言：C++20  
> 阶段目标：建立可扩展的编译器骨架，并完成“预处理 + 词法 + 顶层解析 + 基础 Zinc 转 JASS + library 排序 + 简单代码生成”。  
> 非目标：阶段 1 不要求完整替代 JassHelper，也不要求编译完整 `input.j` 成可运行地图脚本。阶段 1 要能对完整 `input.j` 做稳定扫描、统计、诊断，不崩溃；对阶段 1 范围内的小型用例要能生成可用 JASS。

---

## 0. 背景与阶段 1 边界

本项目目标是实现一个高速 `vJass/Zinc -> JASS war3map.j` 编译器，用来替代旧 JassHelper。项目根目录中假定已经放入以下文件：

```text
input.j                    # 大型原始脚本，约 10 万行
output_jasshelper.j         # JassHelper 对 input.j 的编译结果，用作后续 golden reference
官方API教学1.2a版.pdf       # DZAPI / 平台 API 参考
Vjass完整版中文教程.pdf      # vJass 参考
vJass 系列教程.pdf           # vJass 参考
Vjass系列教程第一版【TigerCN】.pdf
Zinc.chm                    # Zinc 参考，如能解析则作为辅助资料
别人的项目JassForge.md      # 可参考的现代语法/架构资料
```

阶段 1 重点不是一次性吃下全部 vJass，而是先把“编译器地基”做正确：

1. 能稳定读取大文件。
2. 能处理 JassHelper 常见预处理指令。
3. 能区分 vJASS/JASS 区域与 Zinc 区域。
4. 能 tokenize。
5. 能解析基础顶层结构。
6. 能输出一个结构清楚的阶段 1 JASS。
7. 能跑测试，能输出诊断，能对 `input.j` 做扫描统计。

`input.j` 的基线规模大约如下，阶段 1 应至少能在 `--scan-only` 下处理完毕：

```text
input.j:
  lines: 约 104,237
  bytes: 约 4.36 MB
  //! zinc: 307
  //! endzinc: 307
  library/library_once: 约 340
  struct: 约 166
  method: 约 1,378
  static method: 约 1,096
  native: 约 553
  module: 约 51
  static if: 约 93
  //! textmacro: 3
  //! runtextmacro: 18
```

阶段 1 的明确边界：

| 范围 | 阶段 1 是否实现 | 说明 |
|---|---:|---|
| CLI、项目骨架、CMake | 是 | 必须完成 |
| 文件读取、编码容错、换行统一 | 是 | 默认按 UTF-8 读，无法解码时允许 byte-preserving 读入 |
| `//! import` | 是 | 支持相对路径、绝对路径、重复导入保护 |
| `//! novjass` / `//! endnovjass` | 是 | vJass 编译模式下删除块内容 |
| `debug` 行处理 | 是 | release 删除；debug 去掉 `debug` 前缀 |
| `//! textmacro` / `//! runtextmacro` | 是 | 支持参数替换 `$NAME$`；支持 optional；不支持嵌套宏 |
| `//! zinc` / `//! endzinc` | 是 | 作为语法模式切换边界 |
| JASS/vJASS lexer | 是 | 支持基础 token、注释、字符串、rawcode |
| Zinc lexer | 是 | 支持 `{}`、`;`、`->`、`+=` 等 |
| `globals` 合并 | 是 | 输出单个 globals 块 |
| `native` 收集/移动 | 是 | 收集并输出到统一位置；重复 native 去重 |
| `type` 声明收集 | 是 | 原样输出 |
| `function` 基础解析 | 是 | vJASS function 原样或规范化输出 |
| `library` / `library_once` | 是 | 解析 initializer/requires/needs/uses/optional，做拓扑排序 |
| `scope` | 是 | 解析基础结构；不参与 requires；可有 initializer |
| `public/private` 基础改名 | 是，最小实现 | 仅支持函数和 globals 名称；不处理 struct 成员 |
| Zinc 基础函数转换 | 是 | function/local/if/while/for/return/call/set/赋值 |
| library/scope initializer 注入 | 最小实现 | 生成 helper，并在发现 `main` 时插入调用 |
| `static if` | 否 | 阶段 1 只识别并报告 unsupported |
| `struct/method/thistype/allocate/create/destroy` | 否 | 阶段 2 |
| `interface/module/delegate/operator` | 否 | 后续阶段 |
| `function interface` / lambda / prototype wrapper | 否 | 后续阶段 |
| 完整语义类型检查 | 否 | 阶段 1 只做轻量符号收集和错误定位 |
| 字节级匹配 `output_jasshelper.j` | 否 | 后续阶段 |

---

## 1. 开发环境安装流程

### 1.1 Windows 推荐环境

最低建议：Windows 10/11 x64、CMake、Git、MSVC C++20 工具链。

#### 方案 A：Visual Studio Build Tools，只装命令行工具

用管理员 PowerShell 执行：

```powershell
New-Item -ItemType Directory -Force C:\tools | Out-Null
Invoke-WebRequest `
  -Uri "https://aka.ms/vs/17/release/vs_BuildTools.exe" `
  -OutFile "C:\tools\vs_BuildTools.exe"

C:\tools\vs_BuildTools.exe `
  --quiet --wait --norestart --nocache `
  --add Microsoft.VisualStudio.Workload.VCTools `
  --includeRecommended
```

如果静默安装失败，改用 GUI：运行 `vs_BuildTools.exe`，选择 **Desktop development with C++ / 使用 C++ 的桌面开发** 工作负载，然后安装。

安装完成后，从开始菜单打开：

```text
x64 Native Tools Command Prompt for VS 2022
```

验证：

```bat
cl
where cl
```

能看到 MSVC 版本信息即通过。

#### 方案 B：Visual Studio Community

安装 Visual Studio Community 2022，并选择 **Desktop development with C++ / 使用 C++ 的桌面开发** 工作负载。该方案体积更大，但 IDE 调试方便。

#### 安装 Git、CMake、Ninja

推荐用 winget：

```powershell
winget install --id Git.Git -e
winget install --id Kitware.CMake -e
winget install --id Ninja-build.Ninja -e
```

验证：

```powershell
git --version
cmake --version
ninja --version
```

Ninja 是可选项；如果没有 Ninja，也可以用 Visual Studio generator 构建。

### 1.2 Linux / WSL / Codex 环境

如果 Codex 在 Linux 容器中执行开发，先安装基础工具：

```bash
sudo apt update
sudo apt install -y build-essential cmake ninja-build git
```

验证：

```bash
g++ --version
cmake --version
ninja --version
git --version
```

Linux 构建出的不是 Windows `.exe`，但能保证核心编译器逻辑、测试和扫描通过。最终 Windows `.exe` 可在 Windows 上用 MSVC 构建。

---

## 2. 仓库结构要求

请按以下结构初始化项目：

```text
.
├─ CMakeLists.txt
├─ README.md
├─ docs/
│  ├─ phase1_task_plan.md
│  ├─ phase1_status.md
│  └─ compiler_pipeline.md
├─ samples/
│  ├─ input.j
│  ├─ output_jasshelper.j
│  └─ minimal_phase1.j
├─ src/
│  ├─ main.cpp
│  ├─ cli/
│  │  ├─ CliOptions.h
│  │  └─ CliOptions.cpp
│  ├─ core/
│  │  ├─ SourceManager.h
│  │  ├─ SourceManager.cpp
│  │  ├─ Diagnostics.h
│  │  ├─ Diagnostics.cpp
│  │  ├─ StringInterner.h
│  │  └─ StringInterner.cpp
│  ├─ preprocess/
│  │  ├─ Preprocessor.h
│  │  ├─ Preprocessor.cpp
│  │  ├─ TextMacro.h
│  │  └─ TextMacro.cpp
│  ├─ lexer/
│  │  ├─ Token.h
│  │  ├─ Lexer.h
│  │  └─ Lexer.cpp
│  ├─ parser/
│  │  ├─ Ast.h
│  │  ├─ Parser.h
│  │  └─ Parser.cpp
│  ├─ sema/
│  │  ├─ SymbolTable.h
│  │  ├─ SymbolTable.cpp
│  │  ├─ LibraryGraph.h
│  │  └─ LibraryGraph.cpp
│  ├─ codegen/
│  │  ├─ CodeWriter.h
│  │  ├─ CodeWriter.cpp
│  │  ├─ Phase1Codegen.h
│  │  └─ Phase1Codegen.cpp
│  └─ util/
│     ├─ PathUtil.h
│     ├─ PathUtil.cpp
│     ├─ JsonWriter.h
│     └─ JsonWriter.cpp
├─ tests/
│  ├─ CMakeLists.txt
│  ├─ test_main.cpp
│  ├─ fixtures/
│  │  ├─ 01_globals_native.in.j
│  │  ├─ 01_globals_native.expected.j
│  │  ├─ 02_debug_release.in.j
│  │  ├─ 02_debug_release.expected.release.j
│  │  ├─ 02_debug_release.expected.debug.j
│  │  ├─ 03_novjass.in.j
│  │  ├─ 03_novjass.expected.j
│  │  ├─ 04_import_root.in.j
│  │  ├─ imported/
│  │  │  └─ imported_a.j
│  │  ├─ 05_textmacro.in.j
│  │  ├─ 05_textmacro.expected.j
│  │  ├─ 06_library_sort.in.j
│  │  ├─ 06_library_sort.expected.j
│  │  ├─ 07_zinc_basic.in.j
│  │  ├─ 07_zinc_basic.expected.j
│  │  ├─ 08_private_public.in.j
│  │  ├─ 08_private_public.expected.j
│  │  ├─ 09_unsupported_struct.in.j
│  │  └─ 09_unsupported_struct.expected.diagnostics.txt
│  └─ golden_runner.cpp
└─ tools/
   └─ normalize_jass.py              # 可选；用于后续对比 output_jasshelper.j
```

不要在阶段 1 引入复杂第三方依赖。测试框架可以用一个轻量自写 test runner，或者仅用 CTest 执行编译器并比较文本输出。

---

## 3. CMake 与构建任务

### 3.1 根目录 `CMakeLists.txt`

要求：

1. C++ 标准为 C++20。
2. 生成可执行文件名：`vjassc`。
3. Release 默认优化。
4. 打开基本 warning。
5. 注册 CTest。

建议内容：

```cmake
cmake_minimum_required(VERSION 3.20)
project(vjassc LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

add_executable(vjassc
    src/main.cpp
    src/cli/CliOptions.cpp
    src/core/SourceManager.cpp
    src/core/Diagnostics.cpp
    src/core/StringInterner.cpp
    src/preprocess/Preprocessor.cpp
    src/preprocess/TextMacro.cpp
    src/lexer/Lexer.cpp
    src/parser/Parser.cpp
    src/sema/SymbolTable.cpp
    src/sema/LibraryGraph.cpp
    src/codegen/CodeWriter.cpp
    src/codegen/Phase1Codegen.cpp
    src/util/PathUtil.cpp
    src/util/JsonWriter.cpp
)

target_include_directories(vjassc PRIVATE src)

if (MSVC)
    target_compile_options(vjassc PRIVATE /W4 /permissive- /utf-8)
else()
    target_compile_options(vjassc PRIVATE -Wall -Wextra -Wpedantic)
endif()

enable_testing()
add_subdirectory(tests)
```

### 3.2 构建命令

Windows + Ninja：

```powershell
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

Windows + Visual Studio generator：

```powershell
cmake -S . -B build -G "Visual Studio 17 2022" -A x64
cmake --build build --config Release
ctest --test-dir build -C Release --output-on-failure
```

Linux / WSL：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

---

## 4. CLI 需求

可执行文件名：

```text
vjassc.exe    # Windows
vjassc        # Linux/WSL
```

阶段 1 必须支持：

```bash
vjassc <input.j> -o <output.j>
vjassc <input.j> -o <output.j> --debug
vjassc <input.j> -o <output.j> --release
vjassc <input.j> --scan-only
vjassc <input.j> --emit-preprocessed <path>
vjassc <input.j> --emit-tokens <path>
vjassc <input.j> --emit-ast <path>
vjassc <input.j> --emit-stats <path>
vjassc <input.j> --import-path <dir>
vjassc <input.j> --allow-unsupported
vjassc --version
vjassc --help
```

### 4.1 CLI 行为

默认模式：release。

```bash
vjassc samples/minimal_phase1.j -o build/minimal_phase1.out.j
```

等价于：

```bash
vjassc samples/minimal_phase1.j -o build/minimal_phase1.out.j --release
```

`--debug`：保留 debug 行，但去掉 `debug` 关键字。

`--release`：删除以 `debug` 开头的预处理行。

`--scan-only`：

1. 执行文件读取、import、novjass、debug、textmacro、zinc 区域切换、lexer、基础 parser。
2. 不生成最终 JASS，除非同时指定 `--emit-*`。
3. 遇到阶段 1 不支持语法时输出 diagnostics，但不崩溃。
4. 适合在 `input.j` 上使用。

`--allow-unsupported`：

1. 允许解析阶段生成 `UnsupportedDecl` 或 `UnsupportedStmt`。
2. Codegen 阶段遇到 unsupported 时输出注释占位或直接失败，由 `--scan-only` 决定。
3. 非 `--scan-only` 模式下，如果输出会不合法，必须返回非 0，不能假装成功。

推荐退出码：

```text
0  成功
1  参数错误
2  文件读取/写入错误
3  词法错误
4  语法错误
5  语义/依赖错误
6  阶段 1 不支持特性导致无法生成
```

---

## 5. 核心数据结构

### 5.1 SourceManager

目标：统一管理源文件、import 文件、行列映射。

必须提供：

```cpp
struct SourceLocation {
    uint32_t fileId = 0;
    uint32_t line = 1;
    uint32_t column = 1;
    uint32_t offset = 0;
};

struct SourceFile {
    uint32_t id;
    std::filesystem::path path;
    std::string text;
    std::vector<uint32_t> lineOffsets;
};
```

要求：

1. 读取整个文件到内存。
2. 统一换行为 `\n`。
3. 保留原始 file path。
4. 支持 `getLineText(fileId, line)`，用于诊断。
5. 支持 import 递归，并记录 include stack。

### 5.2 Diagnostics

目标：所有错误可定位。

输出格式建议：

```text
samples/input.j:123:17: error: expected 'endlibrary' before end of file
samples/input.j:125:5: note: library started here
```

类结构建议：

```cpp
enum class Severity { Note, Warning, Error };

struct Diagnostic {
    Severity severity;
    SourceLocation loc;
    std::string message;
};
```

要求：

1. 诊断累积，不要遇到第一个错误就直接退出，除非无法继续。
2. `--scan-only` 下尽量恢复解析。
3. 每个 unsupported feature 要有明确提示，例如：

```text
input.j:456:1: warning: phase1 does not lower 'struct'; counted as unsupported declaration
```

### 5.3 StringInterner

目标：减少大文件 identifier 拷贝。

阶段 1 可以简单实现：

```cpp
using SymbolId = uint32_t;
SymbolId intern(std::string_view);
std::string_view get(SymbolId) const;
```

Lexer token 的 identifier 不要反复分配字符串。

---

## 6. 预处理器任务

### 6.1 总体目标

阶段 1 预处理器负责把输入源变成“带模式信息的逻辑源”：

```cpp
enum class SyntaxMode { JassLike, Zinc };

struct LogicalLine {
    SyntaxMode mode;
    SourceLocation loc;
    std::string text;
};
```

预处理器不要做复杂语义，不要 lowering struct。它只处理文本层面的 JassHelper 指令。

### 6.2 换行、注释、字符串规则

必须做到：

1. 统一 CRLF/CR 为 LF。
2. 普通 `//` 注释在 lexer 阶段处理；但 `//!` 是预处理指令，不可当普通注释丢掉。
3. 支持 `/* ... */` 块注释；建议支持嵌套块注释。
4. 字符串内的 `//`、`/*`、`//!` 不应被识别为注释或指令。
5. rawcode 如 `'hfoo'` 应作为 token 保留。

### 6.3 `//! import`

支持：

```jass
//! import "path/to/file.j"
//! import zinc "path/to/file.zn"
```

要求：

1. 路径查找顺序：
   - 当前文件所在目录。
   - CLI 指定的 `--import-path`，可多个。
   - 项目根目录。
2. 重复导入同一 canonical path 时只导入一次。
3. import 文件中的 import 继续处理。
4. import 文件里的 textmacro、globals、library 都要进入同一处理流。
5. 找不到文件时报 error。
6. `//! import zinc` 导入内容默认按 Zinc mode 处理，除非文件内另有 `//! zinc`/`//! endzinc` 指令。

暂不支持：

```jass
//! external
//! externalblock
//! loaddata
```

这些指令阶段 1 只报 warning 或 unsupported。

### 6.4 `//! novjass`

规则：

```jass
//! novjass
    ... ignored by vJass compiler ...
//! endnovjass
```

阶段 1 编译器处于 vJass 编译模式时，直接删除块内容。要求：

1. 支持多行块。
2. 不支持嵌套 novjass；遇到嵌套给 warning。
3. 缺少 endnovjass 报 error。
4. 被删除的内容不参与 macro、lexer、parser。

### 6.5 `debug` 行

支持：

```jass
debug call BJDebugMsg("x")
debug if condition then
debug endif
```

release 模式：删除整行。  
debug 模式：去掉最前面的 `debug` 和紧随其后的一个空格，保留后面内容。

注意：

1. 只处理行首忽略缩进后的 `debug`。
2. 标识符中含有 debug 不处理。
3. 注释和字符串里的 debug 不处理。

### 6.6 textmacro

支持声明：

```jass
//! textmacro NAME
//! textmacro NAME takes A, B, C
//! textmacro_once NAME takes A, B
    ... body ...
//! endtextmacro
```

支持调用：

```jass
//! runtextmacro NAME()
//! runtextmacro NAME("a", "b")
//! runtextmacro optional NAME("a")
```

参数替换：

```jass
$A$ -> 实参文本
```

要求：

1. 宏体按文本保存，不参与预先 lexer。
2. 调用时复制宏体并替换 `$PARAM$`。
3. 支持无参宏。
4. 支持参数中包含 rawcode、字符串、逗号保护，例如：`"'hfoo'"`、`"A,B"`。
5. 支持 `optional`：宏不存在时不报错，直接删除调用行。
6. `textmacro_once`：重复声明同名宏时保留第一次，忽略后续声明并给 warning。
7. 普通 `textmacro` 重名时报 error。
8. 宏不允许嵌套声明；遇到嵌套报 error。
9. 防止递归展开：维护 expansion stack，递归时报 error。
10. 展开后的行保留调用点 SourceLocation，同时可在 diagnostics note 中标出宏定义位置。

### 6.7 Zinc 区域切换

支持：

```jass
//! zinc
    ... Zinc syntax ...
//! endzinc
```

要求：

1. `//! zinc` 后 mode = Zinc。
2. `//! endzinc` 后 mode = JassLike。
3. 嵌套 zinc 报 warning 或 error。
4. 缺少 endzinc 报 error。
5. 输出 `--emit-preprocessed` 时保留注释标记，便于调试：

```jass
// [mode: zinc begin] original line ...
```

---

## 7. Lexer 任务

### 7.1 TokenKind

至少支持：

```cpp
enum class TokenKind {
    EndOfFile,
    Identifier,
    IntegerLiteral,
    RealLiteral,
    StringLiteral,
    RawCodeLiteral,
    Keyword,
    LParen, RParen,
    LBrace, RBrace,
    LBracket, RBracket,
    Comma, Dot, Colon, Semicolon,
    Plus, Minus, Star, Slash, Percent,
    Assign, PlusAssign, MinusAssign, StarAssign, SlashAssign,
    EqualEqual, NotEqual,
    Less, LessEqual, Greater, GreaterEqual,
    Arrow,
    And, Or, Not,
    LineComment,
    Unknown
};
```

### 7.2 Keyword 集合

JASS/vJASS 基础关键字：

```text
type extends native globals endglobals function takes returns endfunction
local set call return if then elseif else endif loop endloop exitwhen
library library_once endlibrary scope endscope initializer requires needs uses optional
public private constant array debug keyword
struct endstruct method endmethod static module endmodule implement interface endinterface
```

Zinc 基础关键字：

```text
library scope struct function method static public private
if else while for return break continue loop exitwhen
integer real boolean string code handle nothing native type globals
```

阶段 1 对 `struct/method/module/interface` 只识别 token，不 lowering。

### 7.3 Lexer 输出

`--emit-tokens tokens.txt` 格式建议：

```text
file:line:col  mode=Zinc  kind=Identifier  text="library"
file:line:col  mode=Zinc  kind=Identifier  text="Demo"
file:line:col  mode=Zinc  kind=LBrace      text="{"
```

要求：

1. token 持有 `std::string_view` 或 intern id，避免大量复制。
2. 每个 token 有 SourceLocation。
3. 词法错误要收集：未闭合字符串、未闭合 rawcode、未知字符。

---

## 8. Parser / AST 任务

### 8.1 AST 顶层结构

建议：

```cpp
struct Program {
    std::vector<Decl*> decls;
    std::vector<Diagnostic> diagnostics;
};

struct Decl {
    SourceLocation loc;
    DeclKind kind;
    SyntaxMode mode;
};

struct GlobalBlockDecl : Decl {
    std::vector<GlobalVarDecl> vars;
};

struct NativeDecl : Decl {
    std::string name;
    std::string signatureText;
};

struct TypeDecl : Decl {
    std::string text;
};

struct FunctionDecl : Decl {
    std::string name;
    std::vector<Param> params;
    std::string returnType;
    std::vector<Stmt*> body;
    std::string rawBodyText;
    Visibility visibility;
};

struct LibraryDecl : Decl {
    std::string name;
    bool once;
    std::optional<std::string> initializer;
    std::vector<LibraryRequirement> requirements;
    std::vector<Decl*> body;
};

struct ScopeDecl : Decl {
    std::string name;
    std::optional<std::string> initializer;
    std::vector<Decl*> body;
};

struct UnsupportedDecl : Decl {
    std::string feature;
    std::string rawText;
};
```

### 8.2 JASS/vJASS 顶层解析

必须解析：

```jass
type name extends parent
native Name takes ... returns ...
globals ... endglobals
function Name takes ... returns ... endfunction
library Name [initializer Init] [requires|needs|uses ...] endlibrary
library_once Name ... endlibrary
scope Name [initializer Init] endscope
```

`library` 依赖语法：

```jass
library A initializer Init requires B, C
library A requires B, C initializer Init
library A needs optional B, C
library A uses B
```

要求：

1. `requires`、`needs`、`uses` 视为同义。
2. `optional` 只作用于紧随的库名；如果写法复杂，阶段 1 做合理解析并给 warning。
3. `library_once` 重名时保留第一次声明，后续忽略。
4. 普通 `library` 重名时报 error。
5. `library` 不能嵌套 `library`。
6. `scope` 可以在 `library` 内，但 `scope` 不参与依赖排序。
7. `scope` 基础 initializer 收集。

### 8.3 JASS function body

阶段 1 对 vJASS/JASS function body 可采用“轻解析 + 原样保留”策略：

1. 识别 `local`、`set`、`call`、`return`、`if/then/else/endif`、`loop/endloop/exitwhen`。
2. 不做完整表达式类型检查。
3. 输出时尽量保持原样，只做 public/private 名称替换。
4. 遇到 `.method`、`thistype`、`struct field` 等高级用法时，不在阶段 1 报错；只有在 codegen 确定无法输出合法 JASS 时才报 unsupported。

### 8.4 Zinc 顶层解析

支持：

```jass
//! zinc
library Demo requires A, B initializer Init {
    integer X = 0;

    function Add(integer a, integer b) -> integer {
        return a + b;
    }
}
//! endzinc
```

阶段 1 Zinc 支持范围：

```text
library/scope with { }
function Name(type a, type b) -> type { }
function Name() { }       # 默认 returns nothing
变量声明：integer x; integer x = 1;
赋值：x = y; x += 1; x -= 1; x *= 2; x /= 2;
函数调用：Foo();
return expr; / return;
if (...) { } else { }
while (...) { }
for (init; cond; inc) { }
```

阶段 1 不支持的 Zinc：

```text
struct/method lowering
lambda/function(...) { }
default arguments
& reference parameters
generics
enum
new/delete/let
sys::function
operator overload
```

遇到这些语法：

1. `--scan-only` 下记录 unsupported 并继续。
2. 非 scan-only 下返回错误，除非该 unsupported 位于不会输出的上下文。

---

## 9. Zinc 基础 lowering 规则

阶段 1 需要能把简单 Zinc 函数转成 JASS。

### 9.1 function signature

输入：

```jass
function Add(integer a, integer b) -> integer {
    return a + b;
}
```

输出：

```jass
function Add takes integer a, integer b returns integer
    return a + b
endfunction
```

输入：

```jass
function Hello() {
    BJDebugMsg("hello");
}
```

输出：

```jass
function Hello takes nothing returns nothing
    call BJDebugMsg("hello")
endfunction
```

### 9.2 variable declaration

Zinc：

```jass
integer i = 0;
real x;
```

JASS：

```jass
local integer i = 0
local real x
```

在函数体内的类型声明视为 local。阶段 1 不处理 Zinc 局部变量必须提前到函数开头的问题；如果变量声明出现在语句中间，为保证 JASS 合法，Codegen 应把所有 local 声明提升到函数开头，并把初始化拆成 set：

Zinc：

```jass
BJDebugMsg("x");
integer i = 1;
```

JASS：

```jass
local integer i
call BJDebugMsg("x")
set i = 1
```

### 9.3 call / assignment

Zinc：

```jass
Foo(a, b);
x = y;
x += 1;
```

JASS：

```jass
call Foo(a, b)
set x = y
set x = x + 1
```

### 9.4 if

Zinc：

```jass
if (a > b) {
    Foo();
} else {
    Bar();
}
```

JASS：

```jass
if (a > b) then
    call Foo()
else
    call Bar()
endif
```

### 9.5 while

Zinc：

```jass
while (i < 10) {
    i += 1;
}
```

JASS：

```jass
loop
    exitwhen not (i < 10)
    set i = i + 1
endloop
```

### 9.6 for

Zinc：

```jass
for (integer i = 0; i < 10; i += 1) {
    Foo(i);
}
```

JASS：

```jass
local integer i
set i = 0
loop
    exitwhen not (i < 10)
    call Foo(i)
    set i = i + 1
endloop
```

阶段 1 只支持 C 风格 for；不支持 range-based for。

---

## 10. SymbolTable 与名称改写

阶段 1 只做最低限度符号管理。

### 10.1 符号类型

```text
GlobalVar
Function
Native
Type
Library
Scope
Unsupported
```

### 10.2 public/private 规则，阶段 1 简化版

在 `library A` 内：

```jass
public function Foo takes nothing returns nothing
endfunction

private function Bar takes nothing returns nothing
endfunction
```

阶段 1 输出名称建议：

```jass
function A_Foo takes nothing returns nothing
endfunction

function A___Bar takes nothing returns nothing
endfunction
```

说明：

1. public：`ScopePath_Name`。
2. private：`ScopePath___Name`。
3. 普通函数：保持原名。
4. 对 globals 变量应用同样规则。
5. 在同一 scope/library 内，直接使用 `Foo`、`Bar` 时要改写为实际名称。
6. 在外部使用 public 成员必须是 `A_Foo`；阶段 1 不强制检查所有访问权限，但要尽量正确改名。
7. 阶段 1 不实现 JassHelper 的随机 private 前缀。使用 deterministic 名称，便于测试。
8. 阶段 2/后续再追求 JassHelper 输出风格兼容。

### 10.3 函数体名称替换

对 raw body 做 token-level 替换，而不是字符串 replace。

要求：

1. 只替换 Identifier token。
2. 不替换字符串字面量里的内容。
3. 不替换注释里的内容。
4. 不替换 rawcode。
5. 支持 `ExecuteFunc("A_Foo")` 这种字符串暂不改写；后续处理。

---

## 11. LibraryGraph 任务

### 11.1 依赖排序

输入：

```jass
library C requires A, B
endlibrary
library B requires A
endlibrary
library A
endlibrary
```

输出顺序：

```text
A, B, C
```

要求：

1. 使用拓扑排序。
2. required library 必须存在；不存在时报 error。
3. optional library 不存在时不报错，但在 stats 中记录。
4. 循环依赖时报 error，并输出循环路径。
5. 排序稳定：同层无依赖项保持源码出现顺序。
6. `library_once` 重复时忽略后续库体。

### 11.2 LIBRARY_ 常量

每个 library 输出一个常量：

```jass
constant boolean LIBRARY_A=true
```

插入到 globals 中。optional 不存在时不生成对应常量。

---

## 12. Codegen 任务

### 12.1 输出总体顺序

阶段 1 输出建议：

```jass
// Generated by vjassc phase1

globals
    // LIBRARY_ constants
    // merged global vars
endglobals

// type declarations
// native declarations

// library functions in sorted order
// scope/functions/non-library declarations

function vjassc__init_libraries takes nothing returns nothing
    // library initializer ExecuteFunc calls
    // scope initializer direct calls
endfunction

function main takes nothing returns nothing
    ... original main body with injected call ...
endfunction
```

注意：如果源文件里已有 `main`，不要生成第二个 `main`。

### 12.2 globals 合并

所有：

```jass
globals
    integer A
endglobals
```

合并成一个 `globals` 块。

输出时加入注释，方便 debug：

```jass
// globals from LibraryName:
integer A
// endglobals from LibraryName
```

### 12.3 native 去重

按 native 名称去重。第一次出现保留，后续重复给 warning。

```jass
native UnitAlive takes unit id returns boolean
```

### 12.4 initializer helper

对于 library initializer：

```jass
library A initializer InitA
endlibrary
```

生成：

```jass
function vjassc__init_libraries takes nothing returns nothing
    call ExecuteFunc("A___InitA")
endfunction
```

对于 scope initializer：

```jass
scope S initializer InitS
endscope
```

生成：

```jass
function vjassc__init_libraries takes nothing returns nothing
    call S___InitS()
endfunction
```

如果有多个 initializer，按 library 拓扑顺序 + 源码顺序输出。

### 12.5 main 注入

如果发现：

```jass
function main takes nothing returns nothing
    call InitBlizzard()
    ...
endfunction
```

插入：

```jass
    call vjassc__init_libraries()
```

建议插在 `call InitBlizzard()` 之后。如果找不到 `InitBlizzard()`，插入到 `main` 的第一条语句之前，并给 warning。

如果没有 `main`，只生成 helper，不创建 main。

---

## 13. `input.j` 扫描与统计

阶段 1 必须提供：

```bash
vjassc samples/input.j --scan-only --emit-stats build/input.stats.json
```

`input.stats.json` 至少包含：

```json
{
  "files": 1,
  "bytes": 4357817,
  "lines": 104237,
  "zincBlocks": 307,
  "libraries": 331,
  "libraryOnce": 9,
  "scopes": 0,
  "globalsBlocks": 0,
  "natives": 553,
  "functions": 4701,
  "structsUnsupported": 166,
  "methodsUnsupported": 1378,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "textmacros": 3,
  "runtextmacros": 18,
  "diagnostics": {
    "errors": 0,
    "warnings": 0,
    "unsupported": 0
  },
  "timingMs": {
    "read": 0,
    "preprocess": 0,
    "lex": 0,
    "parse": 0,
    "total": 0
  }
}
```

实际数值允许略有差异，但统计字段必须稳定存在。

阶段 1 性能目标：

```text
现代 Windows 桌面 Release 构建：input.j scan-only 目标 < 2 秒
普通 Codex/Linux 容器：input.j scan-only 目标 < 10 秒
```

不要为了阶段 1 性能牺牲可维护性；但要避免明显的 O(n²) 字符串拼接。

---

## 14. 测试任务

### 14.1 测试运行方式

```bash
ctest --test-dir build --output-on-failure
```

或直接：

```bash
build/vjassc tests/fixtures/01_globals_native.in.j -o build/01.out.j
```

测试比较时应做换行归一化。

### 14.2 必须测试用例

#### 01 globals + native

输入：

```jass
function A takes nothing returns nothing
endfunction

globals
    integer X = 1
endglobals

native UnitAlive takes unit id returns boolean

globals
    real Y = 2.0
endglobals
```

期望：一个 globals 块，native 被收集，function 保留。

#### 02 debug release/debug

输入：

```jass
function Test takes nothing returns nothing
    debug call BJDebugMsg("debug")
    call BJDebugMsg("always")
endfunction
```

release 期望：删除 debug 行。  
debug 期望：保留 `call BJDebugMsg("debug")`。

#### 03 novjass

输入：

```jass
function Verify takes nothing returns nothing
    local boolean b = true
//! novjass
    set b = false
//! endnovjass
    if b then
        call BJDebugMsg("vjass")
    endif
endfunction
```

期望：删除 novjass 块。

#### 04 import

根文件：

```jass
//! import "imported/imported_a.j"
function MainA takes nothing returns nothing
endfunction
```

导入文件：

```jass
globals
    integer ImportedValue = 1
endglobals
```

期望：导入 globals 被合并。

#### 05 textmacro

输入：

```jass
//! textmacro MakeFunc takes NAME, VALUE
function $NAME$ takes nothing returns integer
    return $VALUE$
endfunction
//! endtextmacro

//! runtextmacro MakeFunc("GetA", "1")
//! runtextmacro MakeFunc("GetB", "2")
```

期望：生成 `GetA` 和 `GetB` 两个 function。

#### 06 library sort

输入：

```jass
library B requires A
function BFunc takes nothing returns nothing
    call AFunc()
endfunction
endlibrary

library A
function AFunc takes nothing returns nothing
endfunction
endlibrary
```

期望：输出中 `AFunc` 在 `BFunc` 前。

#### 07 Zinc basic

输入：

```jass
//! zinc
library Demo {
    integer X = 0;

    function Add(integer a, integer b) -> integer {
        integer c = a + b;
        return c;
    }

    function Run() {
        integer i = 0;
        while (i < 3) {
            i += 1;
        }
        if (i == 3) {
            BJDebugMsg("ok");
        } else {
            BJDebugMsg("bad");
        }
    }
}
//! endzinc
```

期望：Zinc function 转成 JASS function，while 转 loop，调用加 call，赋值加 set。

#### 08 private/public

输入：

```jass
library A
    globals
        public integer X = 1
        private integer Y = 2
    endglobals

    public function Foo takes nothing returns nothing
        set X = X + 1
        set Y = Y + 1
    endfunction

    private function Bar takes nothing returns nothing
    endfunction
endlibrary
```

期望：

```text
A_X
A___Y
A_Foo
A___Bar
```

函数体里的 `X/Y` 也被 token-level 改写。

#### 09 unsupported struct

输入：

```jass
struct A
    integer x
endstruct
```

`--scan-only --allow-unsupported` 期望：返回 0，stats 中 `structsUnsupported=1`。  
普通 codegen 期望：返回 6，并输出 unsupported 诊断。

---

## 15. 文档交付

阶段 1 完成后，必须生成或更新：

```text
README.md
    - 项目简介
    - 环境安装
    - 构建命令
    - CLI 用法

docs/compiler_pipeline.md
    - Read -> Preprocess -> Lex -> Parse -> Symbol -> Codegen
    - 每阶段输入输出

docs/phase1_status.md
    - 已实现功能
    - 未实现功能
    - 对 input.j 的扫描统计
    - 已知问题
```

`phase1_status.md` 必须明确写出：

```text
阶段 1 不是完整 JassHelper 替代。
阶段 1 不能 lowering struct/method/function interface/lambda。
阶段 1 能对完整 input.j 扫描统计，但不能完整生成最终 war3map.j。
```

---

## 16. 给 Codex 的执行顺序

请按以下顺序实现，不要跳到 struct/method lowering。

### Task 1.1 初始化仓库与构建

交付：

```text
CMakeLists.txt
src/main.cpp
README.md 初版
```

验收：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
build/vjassc --version
build/vjassc --help
```

### Task 1.2 CLI 参数解析

交付：

```text
src/cli/CliOptions.h/cpp
```

验收：

```bash
vjassc --help
vjassc samples/input.j --scan-only --emit-stats build/stats.json
```

即使后续编译逻辑还没完成，也要能正确解析参数并给出清晰错误。

### Task 1.3 SourceManager + Diagnostics

交付：

```text
src/core/SourceManager.*
src/core/Diagnostics.*
```

验收：

1. 能读取 `samples/input.j`。
2. 能输出行列号。
3. 文件不存在时报清晰错误。

### Task 1.4 Preprocessor

交付：

```text
src/preprocess/Preprocessor.*
src/preprocess/TextMacro.*
```

实现顺序：

1. 换行统一。
2. `//! zinc` mode 切换。
3. `//! novjass` 删除。
4. `debug` release/debug 处理。
5. `//! import`。
6. textmacro/runtextmacro。

验收：通过 fixtures 02、03、04、05。

### Task 1.5 Lexer

交付：

```text
src/lexer/Token.h
src/lexer/Lexer.h/cpp
```

验收：

```bash
vjassc tests/fixtures/07_zinc_basic.in.j --scan-only --emit-tokens build/tokens.txt
```

检查 tokens 中能正确看到 mode、identifier、brace、semicolon、arrow。

### Task 1.6 Parser 基础 AST

交付：

```text
src/parser/Ast.h
src/parser/Parser.h/cpp
```

实现：

1. JASS globals/native/type/function。
2. library/library_once/scope。
3. Zinc library/function/basic statements。
4. UnsupportedDecl 恢复。

验收：fixtures 01、06、07、09 能 parse。

### Task 1.7 SymbolTable + LibraryGraph

交付：

```text
src/sema/SymbolTable.*
src/sema/LibraryGraph.*
```

实现：

1. 收集 library。
2. library_once 重复处理。
3. requires/needs/uses/optional。
4. 拓扑排序。
5. 循环依赖诊断。
6. public/private 基础符号映射。

验收：fixture 06、08。

### Task 1.8 Phase1Codegen

交付：

```text
src/codegen/CodeWriter.*
src/codegen/Phase1Codegen.*
```

实现：

1. globals 合并。
2. native/type 输出。
3. vJASS function 输出。
4. Zinc basic lowering。
5. library sorted output。
6. initializer helper。
7. main 注入。
8. unsupported codegen 拒绝。

验收：fixtures 01、06、07、08。

### Task 1.9 测试系统

交付：

```text
tests/CMakeLists.txt
tests/golden_runner.cpp
tests/fixtures/*
```

验收：

```bash
ctest --test-dir build --output-on-failure
```

所有阶段 1 fixtures 通过。

### Task 1.10 `input.j` 扫描基线

交付：

```text
docs/phase1_status.md
build/input.stats.json
```

命令：

```bash
vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.stats.json
```

验收：

1. 返回 0。
2. 不崩溃。
3. 输出 stats。
4. 输出耗时。
5. docs/phase1_status.md 记录结果。

---

## 17. 阶段 1 完成标准

阶段 1 视为完成，必须满足：

```text
[ ] Windows 或 Linux 至少一种环境可完整构建。
[ ] `vjassc --help`、`vjassc --version` 正常。
[ ] 所有 tests/fixtures 通过。
[ ] `samples/input.j --scan-only --allow-unsupported` 能完成。
[ ] `--emit-preprocessed` 可输出预处理结果。
[ ] `--emit-tokens` 可输出 token 流。
[ ] `--emit-ast` 可输出 JSON 或文本 AST。
[ ] `--emit-stats` 可输出 JSON 统计。
[ ] 阶段 1 范围内的小型 vJASS/Zinc 用例能生成 JASS。
[ ] 遇到 struct/method/lambda/function interface 等非阶段 1 特性时，有清晰 unsupported 诊断。
[ ] docs/phase1_status.md 明确列出已完成/未完成/下一阶段建议。
```

---

## 18. 阶段 1 禁止事项

为了避免 Codex 失控扩大范围，阶段 1 禁止做：

```text
1. 不要实现 struct lowering。
2. 不要实现 method 调用改写。
3. 不要实现 thistype/create/allocate/destroy。
4. 不要实现 interface/module/delegate/operator。
5. 不要实现 function interface/lambda/prototype trigger wrapper。
6. 不要追求和 output_jasshelper.j 字节级一致。
7. 不要用正则硬替换整个语言。
8. 不要引入大型第三方库。
9. 不要为了通过 input.j codegen 而输出伪成功结果。
10. 不要把 unsupported 当普通注释悄悄吞掉；必须统计和诊断。
```

---

## 19. 建议提交拆分

建议 Codex 每完成一个小任务提交一次：

```text
commit 1: project scaffold and cli
commit 2: source manager and diagnostics
commit 3: preprocessor directives
commit 4: textmacro expansion
commit 5: lexer
commit 6: parser ast basics
commit 7: library graph and symbol table
commit 8: phase1 codegen
commit 9: tests and fixtures
commit 10: input.j scan baseline and docs
```

---

## 20. Codex 起始提示词

可以直接把以下内容发给 Codex：

```text
你正在实现一个 C++20 编译器 vjassc，用于最终替代 JassHelper，将 vJass/Zinc 编译为 Warcraft 3 可运行的 JASS war3map.j。

当前只执行阶段 1，不要实现 struct/method/function interface/lambda/module/interface/delegate/operator lowering。

请阅读 docs/phase1_task_plan.md，然后按任务 1.1 到 1.10 顺序实现。

硬性要求：
1. 使用 CMake + C++20。
2. 生成可执行文件 vjassc。
3. 支持 CLI：--help、--version、--scan-only、--debug、--release、--emit-preprocessed、--emit-tokens、--emit-ast、--emit-stats、--import-path、--allow-unsupported。
4. 实现预处理：import、novjass、debug、textmacro/runtextmacro、zinc/endzinc。
5. 实现 JASS/Zinc lexer。
6. 实现基础 AST parser：globals/native/type/function/library/library_once/scope，Zinc 基础 function/if/while/for/local/call/set/return。
7. 实现 library requires/needs/uses 拓扑排序。
8. 实现 globals 合并、native/type 收集、public/private 最小改名、Zinc 基础 lowering、initializer helper、main 注入。
9. 对完整 samples/input.j 必须能 --scan-only --allow-unsupported 并输出 stats，不崩溃。
10. 对阶段 1 fixtures 必须能生成 expected JASS。

完成后运行：
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.stats.json

最后更新 docs/phase1_status.md，说明已实现功能、未实现功能、input.j 扫描结果和下一阶段建议。
```
