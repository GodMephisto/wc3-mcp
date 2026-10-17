---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase3_codex_task_plan.md"
ref: HEAD
---

# vJass/Zinc -> JASS 编译器：阶段 3 任务计划

> 交付对象：Codex / 编码代理
> 目标仓库：`Crainax/JassChanger`
> 推荐语言：继续使用 C++20
> 阶段 3 总目标：在阶段 2 已完成 `struct/method` 基础 lowering 的基础上，实现 **static if / optional library 条件剪枝** 与 **module / implement 展开**，并补齐阶段 2 遗留的工程命名、测试命名和状态文档。
> 阶段 3 成功标准：完整 `samples/input.j` 的 scan-only 中 `staticIfUnsupported == 0`、`modulesUnsupported == 0`，同时保持 `structsUnsupported == 0`、`methodsUnsupported == 0`；小型 fixture 中带 `static if`、`module`、`implement` 的代码可以生成纯 JASS；完整 `input.j` 仍允许因 `function interface/lambda` 等后续阶段内容而拒绝完整 codegen。

---

## 0. 当前状态与阶段 3 边界

### 0.1 阶段 2 已完成能力

当前仓库已经进入 phase-2 状态，已实现：

```text
- vJASS / Zinc struct parser
- vJASS / Zinc method parser
- field parser，包括 static field、多声明、数组字段、固定长度字段
- struct deterministic generated name
- struct allocator globals
- instance field arrays
- static fields
- methods / static methods
- thistype
- default allocate/create/destroy
- onDestroy
- static onInit
- targeted field/method/create/allocate/destroy/code-reference rewriting
- Zinc method body lowering through existing Zinc body lowerer
- array struct support without allocator globals
- phase2 golden fixtures 和部分 negative fixtures
```

阶段 2 的真实 `samples/input.j` scan-only 基线：

```json
{
  "files": 1,
  "bytes": 4253580,
  "lines": 104238,
  "zincBlocks": 307,
  "libraries": 329,
  "libraryOnce": 9,
  "globalsBlocks": 502,
  "natives": 551,
  "functions": 3576,
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "functionInterfacesUnsupported": 10,
  "diagnostics": {
    "errors": 0,
    "warnings": 154,
    "unsupported": 154
  },
  "timingMs": {
    "read": 41,
    "preprocess": 79,
    "lex": 173,
    "parse": 127,
    "total": 391
  }
}
```

### 0.2 阶段 3 要解决的问题

阶段 3 只处理当前最阻塞的两类 unsupported：

```text
1. static if / optional library 条件剪枝
2. module / implement 展开
```

阶段 3 完成后，完整 `input.j` 的 unsupported 应主要剩下：

```text
- function interface
- Zinc anonymous function / lambda
- function type / prototype wrapper
- interface / delegate / operator / stub / super 等后续高级特性
```

### 0.3 阶段 3 明确不做

不要在阶段 3 中混入以下内容：

```text
不做：
- function interface lowering
- lambda / anonymous function lowering
- function pointer/prototype trigger wrapper
- code -> boolexpr 自动包装
- interface dispatch / typeid / getType
- delegate
- operator [] / []= / getter / setter
- stub / super
- 泛型、new/delete/let、JassForge 扩展语法
- 完整 expression AST
- 字节级匹配 output_jasshelper.j
- 完整 input.j 最终 war3map.j 生成
```

如果这些语法出现在完整 `input.j` 中，阶段 3 可以继续报告 unsupported，但不能再因为 `static if` 或 `module` 自身而 unsupported。

---

## 1. 阶段 3 交付物

Codex 完成后，仓库至少应新增或更新：

```text
README.md
CMakeLists.txt
docs/phase3_status.md

tests/CMakeLists.txt
tests/golden_runner.cpp
tests/fixtures/phase3_static_if_*.input.j
tests/fixtures/phase3_static_if_*.expected.j
tests/fixtures/phase3_module_*.input.j
tests/fixtures/phase3_module_*.expected.j
tests/fixtures/phase3_negative_*.input.j

src/main.cpp
src/parser/Ast.h
src/parser/Parser.h
src/parser/Parser.cpp
src/sema/SymbolTable.h
src/sema/SymbolTable.cpp
src/sema/LibraryGraph.h
src/sema/LibraryGraph.cpp
src/codegen/Phase1Codegen.h         # 可先保留，但建议重命名
src/codegen/Phase1Codegen.cpp       # 可先保留，但建议重命名
```

推荐新增：

```text
src/condition/StaticIfPruner.h
src/condition/StaticIfPruner.cpp
src/condition/BoolExpr.h
src/condition/BoolExpr.cpp

src/sema/ModuleTable.h
src/sema/ModuleTable.cpp
src/sema/ModuleExpander.h
src/sema/ModuleExpander.cpp

src/codegen/LoweringCodegen.h       # 推荐由 Phase1Codegen 重命名而来
src/codegen/LoweringCodegen.cpp
```

如果为了减少重构风险，阶段 3 可以暂时继续使用 `Phase1Codegen` 文件名，但必须在 `docs/phase3_status.md` 中说明这是遗留命名债。推荐做法是本阶段完成重命名：

```text
Phase1Codegen -> LoweringCodegen 或 CompilerCodegen
phase1_fixtures -> compiler_fixtures 或 golden_fixtures
```

---

## 2. 开发环境与基线验证

### 2.1 拉取仓库并创建阶段 3 分支

```bash
git clone https://github.com/Crainax/JassChanger.git
cd JassChanger
git status
git branch -vv
git log --oneline --decorate -5

git checkout -b phase3-static-if-module
```

如果当前环境已经有仓库：

```bash
cd JassChanger
git fetch origin
git checkout master
git pull --ff-only
git checkout -b phase3-static-if-module
```

### 2.2 构建与测试基线

Windows / MSVC：

```bat
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

Linux / WSL / Codex：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

真实输入扫描：

```bash
build/vjassc samples/input.j \
  --scan-only \
  --allow-unsupported \
  --emit-stats build/input.phase2.stats.json \
  --emit-ast build/input.phase2.ast.txt
```

Codex 必须先确认阶段 2 基线可以通过，再开始阶段 3 修改。

---

## 3. 阶段 3 的真实输入重点

完整 `input.j` 中的 `static if` 主要是 Zinc 风格，而不是传统 vJASS 风格。常见形态包括：

```zinc
static if (LIBRARY_UILifeCycle) {uiLifeCycle.onCreateCB(this,thistype.typeid,ui);}
static if (LIBRARY_UIHashTable) {uiHashTable(ui).ui.bind(thistype.typeid,this);}
```

也有多行带 `else` 的形态：

```zinc
static if (LIBRARY_DIY_RES__DiyResUIGold) {
    uiRes1Icon.setTexture(DIY_RES_UI_GOLD_STRING);
} else {
    uiRes1Icon.setTexture("UI\\Feedback\\Resources\\ResourceGold.blp");
}
```

完整 `input.j` 中的 module 主要也是 Zinc 风格。模块声明形态：

```zinc
library UIBaseModule requires UIUtils {
    public module uiBaseModule {
        method setPoint(integer anchor, integer relative, integer relativeAnchor, real offsetX, real offsetY) -> thistype {
            if (!this.isExist()) {return this;}
            DzFrameSetPoint(ui, anchor, relative, relativeAnchor, offsetX, offsetY);
            return this;
        }
        optional module extendResize;
    }
}
```

模块使用形态：

```zinc
public struct uiBtn {
    integer ui;
    integer id;
    method isExist() -> boolean { return (this != null && si__uiBtn_V[this] == -1); }
    optional module uiLifeCycle;
    module uiBaseModule;
    module uiEventModule;
    optional module extendDrag;
}
```

因此阶段 3 的实现优先级必须是：

```text
1. Zinc static if block / inline form
2. Zinc public/private module declaration
3. Zinc module / optional module use inside struct or module
4. vJASS static if then/elseif/else/endif
5. vJASS module / implement / implement optional
```

---

## 4. 总体编译管线调整

阶段 2 当前大致是：

```text
SourceManager
  -> Preprocessor
  -> Lexer
  -> Parser
  -> Codegen 内部 build SymbolTable / collectStructs / LibraryGraph / emit
```

阶段 3 推荐调整为：

```text
SourceManager
  -> Preprocessor
  -> StaticIfSymbolCollector
  -> StaticIfPruner
  -> Lexer
  -> Parser
  -> ModuleTableBuilder
  -> ModuleExpander
  -> SymbolTable
  -> LibraryGraph
  -> LoweringCodegen
```

### 4.1 为什么 static if 要在 Parser 前处理

`static if` 的 false 分支可能包含：

```text
- 不存在的库函数调用
- 不存在的类型
- 不存在的 module
- 未来 unsupported 语法
```

如果 Parser 先把所有分支都解析进去，再做剪枝，会出现两个问题：

```text
1. false 分支仍然被统计成 unsupported，导致 input.j 的 unsupported 降不下来。
2. false 分支里的缺失符号可能污染 SymbolTable / ModuleTable。
```

因此 `StaticIfPruner` 应该放在 `Preprocessor` 后、`Parser` 前。它输入 `std::vector<LogicalLine>`，输出剪枝后的 `std::vector<LogicalLine>`。

### 4.2 为什么 module expansion 要在 Codegen 前处理

module 的语义接近“把模块成员复制进 struct”。阶段 2 的 struct lowering 已经可以处理字段和方法，所以阶段 3 最稳的做法是：

```text
Parser 解析 module 声明与 module use
ModuleExpander 把 module 成员 clone 到目标 struct AST
Struct lowering 继续处理展开后的普通字段和方法
```

不要在 Codegen 中特殊处理 module。module 应该是 AST 级别展开。

---

## 5. 任务 A：阶段 2 工程小债务清理

这些任务建议先做，但不要花太多时间。

### 5.1 重命名 Codegen

当前文件仍叫：

```text
src/codegen/Phase1Codegen.h
src/codegen/Phase1Codegen.cpp
```

但代码已经输出 phase2，并承担 phase2 struct lowering。建议重命名为：

```text
src/codegen/LoweringCodegen.h
src/codegen/LoweringCodegen.cpp
```

类名同步改为：

```cpp
class LoweringCodegen
```

`CMakeLists.txt` 同步更新。

如果 Codex 判断重命名风险过高，可以保留旧名，但要在 `docs/phase3_status.md` 标记为待清理。

### 5.2 重命名测试目标

当前 `tests/CMakeLists.txt` 的 CTest 名称仍是：

```text
phase1_fixtures
```

建议改成：

```text
golden_fixtures
```

或拆分为：

```text
phase1_fixtures
phase2_fixtures
phase3_static_if_fixtures
phase3_module_fixtures
```

最小改动建议：把 CTest 名改为 `golden_fixtures`，让 runner 根据文件名前缀分组打印。

### 5.3 增强 stats 字段

在 `emitStatsJson` 中增加：

```json
{
  "modules": 0,
  "moduleUses": 0,
  "staticIfs": 0,
  "staticIfResolvedTrue": 0,
  "staticIfResolvedFalse": 0,
  "staticIfPrunedLines": 0,
  "moduleExpansions": 0
}
```

保留旧字段：

```json
{
  "modulesUnsupported": 0,
  "staticIfUnsupported": 0
}
```

这样便于和 phase2 基线比较。

---

## 6. 任务 B：static if / optional library 条件剪枝

### 6.1 新增数据结构

推荐新增：

```cpp
struct StaticIfSymbols {
    std::unordered_set<std::string> libraries;
    std::unordered_map<std::string, bool> boolConstants;
    bool debugMode = false;
};

struct StaticIfStats {
    size_t staticIfs = 0;
    size_t resolvedTrue = 0;
    size_t resolvedFalse = 0;
    size_t prunedLines = 0;
};
```

推荐类：

```cpp
class StaticIfSymbolCollector {
public:
    StaticIfSymbols collect(const std::vector<LogicalLine>& lines,
                            bool debugMode,
                            Diagnostics& diagnostics);
};

class StaticIfPruner {
public:
    StaticIfPruneResult prune(const std::vector<LogicalLine>& lines,
                              const StaticIfSymbols& symbols,
                              Diagnostics& diagnostics);
};
```

### 6.2 收集 library 常量

vJASS 规则中，每个存在的 library 都应该生成：

```jass
constant boolean LIBRARY_LibraryName = true
```

阶段 3 不需要在源码中真的插入这行给 static if 用，只要在 `StaticIfSymbols.libraries` 中记录即可。

收集规则：

```text
- 扫描所有 preprocessed LogicalLine
- JASS-like 模式：识别 library X / library_once X
- Zinc 模式：识别 library X { ... }
- 对 library_once 重复声明：第一份有效，重复忽略
- LIBRARY_X 在 static if 表达式中表示 X 是否存在
- DEBUG_MODE 表示当前 CLI 是否 --debug
```

注意：

```text
- requires optional X 不会自动创建 X。
- 如果 X 不存在，LIBRARY_X 为 false。
- 如果 X 存在，不管它是不是 optional dependency，LIBRARY_X 都为 true。
```

### 6.3 收集 constant boolean

阶段 3 至少支持这些形式：

vJASS：

```jass
globals
    constant boolean ENABLE_A = true
    private constant boolean ENABLE_B = false
endglobals
```

Zinc：

```zinc
constant boolean ENABLE_A = true;
public constant boolean ENABLE_B = false;
private constant boolean ENABLE_C = DEBUG_MODE;
```

表达式初始化的支持范围：

```text
必须支持：
- true
- false
- DEBUG_MODE
- LIBRARY_X
- not / !
- and / &&
- or / ||
- 括号

可以暂不支持：
- 函数调用
- 比较表达式
- 数字到 boolean
- 字符串
- 任意变量表达式
```

未知普通 identifier 的处理建议：

```text
- 如果是 LIBRARY_ 前缀：不存在则 false。
- 如果不是 LIBRARY_ 且不在 boolConstants：报错或 warning。
```

为了兼容真实地图，推荐先 warning 并按 false 处理；但在 negative fixture 中要覆盖此行为。

### 6.4 布尔表达式解析器

不要用一条 regex 解析 static if 条件。实现一个小型递归下降 parser。

语法：

```text
Expr        := OrExpr
OrExpr      := AndExpr (('or' | '||') AndExpr)*
AndExpr     := NotExpr (('and' | '&&') NotExpr)*
NotExpr     := ('not' | '!')* Primary
Primary     := 'true' | 'false' | 'DEBUG_MODE' | Identifier | '(' Expr ')'
Identifier  := [A-Za-z_][A-Za-z0-9_]*
```

返回：

```cpp
struct BoolEvalResult {
    bool ok;
    bool value;
    std::string error;
};
```

必须支持：

```text
static if LIBRARY_A then
static if not LIBRARY_A then
static if LIBRARY_A and LIBRARY_B then
static if (LIBRARY_A and not LIBRARY_B) then
static if (DEBUG_MODE || LIBRARY_DebugLib) { ... }
```

### 6.5 支持 vJASS static if

输入：

```jass
static if LIBRARY_A then
    call A_Do()
elseif LIBRARY_B then
    call B_Do()
else
    call Fallback()
endif
```

输出给后续 Parser 的 logical lines：

```jass
call A_Do()
```

或：

```jass
call B_Do()
```

或：

```jass
call Fallback()
```

实现要点：

```text
- 支持 nested static if。
- 支持 elseif。
- 支持 else。
- 删除 static if / elseif / else / endif 控制行。
- false 分支的所有行都不能进入 Parser。
- source location 尽量保留原始行号。
```

### 6.6 支持 Zinc static if

完整 `input.j` 中优先需要支持这些形态：

单行：

```zinc
static if (LIBRARY_A) { callA(); }
static if (LIBRARY_A) { callA(); } else { callB(); }
```

多行：

```zinc
static if (LIBRARY_A) {
    callA();
} else {
    callB();
}
```

函数体内单行无 `call` 的 Zinc 语句：

```zinc
static if (LIBRARY_UILifeCycle) {uiLifeCycle.onCreateCB(this, thistype.typeid, ui);}
```

推荐实现方式：

```text
- 在 LogicalLine 层实现一个 Zinc static if block 捕获器。
- 使用字符级 brace depth，忽略字符串、rawcode、注释中的大括号。
- 对单行 `{ body }` 形式，把 body 拆成新的 LogicalLine。
- 对多行 block，保留 active branch 内部行。
- 不要把 static if 自身当作普通 Zinc if 降级。
```

注意：

```text
- Zinc lambda 中也有 `{}`，不能被 static if 捕获误伤。
- 只在行首或语句开头识别 `static if`。
- 不要在字符串内替换。
```

### 6.7 static if 与 unsupported 统计

完成后，Parser 的 `preScanUnsupported` 不应再把 active 后的 `static if` 统计为 unsupported。预期：

```json
{
  "staticIfUnsupported": 0,
  "staticIfs": 93,
  "staticIfResolvedTrue": "按真实输入计算",
  "staticIfResolvedFalse": "按真实输入计算"
}
```

如果某个 `static if` 条件表达式语法不支持，则可以保留 unsupported，但必须在 `docs/phase3_status.md` 中列出具体行和表达式。

---

## 7. 任务 C：module / implement AST 解析

### 7.1 AST 扩展

在 `Ast.h` 中建议新增：

```cpp
enum class DeclKind {
    GlobalBlock,
    Native,
    TypeDecl,
    Function,
    Library,
    Scope,
    Struct,
    Module,
    Unsupported,
};

struct ModuleUseDecl {
    std::string name;
    SourceLocation loc;
    bool optional = false;
    SyntaxMode mode = SyntaxMode::JassLike;
};
```

扩展 `Decl`：

```cpp
struct Decl {
    ...
    std::vector<FieldDecl> fields;
    std::vector<MethodDecl> methods;
    std::vector<ModuleUseDecl> moduleUses;
    std::string moduleOriginName;    // clone 后用于诊断，可选
};
```

扩展 `ParserStats`：

```cpp
size_t modules = 0;
size_t moduleUses = 0;
size_t moduleExpansions = 0;
```

保留：

```cpp
size_t modulesUnsupported = 0;
```

但阶段 3 后普通 module 不应再进入 unsupported。

### 7.2 vJASS module 声明解析

支持：

```jass
module M
    integer x
    static integer y

    method foo takes nothing returns nothing
    endmethod

    static method bar takes nothing returns nothing
    endmethod

    implement OtherModule
    implement optional OptionalModule
endmodule
```

解析成：

```text
DeclKind::Module
  name = M
  fields = [...]
  methods = [...]
  moduleUses = [OtherModule, OptionalModule(optional)]
```

限制：

```text
- module 内不能直接声明 function。
- module 内不能直接声明 library/scope。
- module 内暂不支持 operator/stub/super/interface/delegate。
- 遇到上述高级语法，继续 unsupported 或 error，不要静默吞掉。
```

### 7.3 vJASS implement 解析

在 struct 或 module 体内支持：

```jass
implement M
implement optional M
```

解析为 `ModuleUseDecl`。

### 7.4 Zinc module 声明解析

支持完整 `input.j` 中出现的风格：

```zinc
public module uiBaseModule {
    method setPoint(integer anchor, integer relative, integer relativeAnchor, real offsetX, real offsetY) -> thistype {
        if (!this.isExist()) {return this;}
        DzFrameSetPoint(ui, anchor, relative, relativeAnchor, offsetX, offsetY);
        return this;
    }

    optional module extendResize;
}
```

解析规则：

```text
- public/private module Name { ... }
- module Name { ... }
- module 内可包含 field、method、static method、module use
- Zinc module 体用 brace depth 捕获
- module declaration 和 struct declaration 类似，但不会生成 allocator
```

### 7.5 Zinc module use 解析

在 struct 或 module 体内支持：

```zinc
module uiBaseModule;
optional module uiLifeCycle;
```

解析为 `ModuleUseDecl`。

注意真实 `input.j` 中大量使用：

```zinc
optional module uiLifeCycle;
module uiBaseModule;
module uiEventModule;
optional module extendDrag;
```

这是阶段 3 必须覆盖的核心形态。

---

## 8. 任务 D：ModuleTable 与作用域查找

### 8.1 ModuleTable 结构

推荐新增：

```cpp
struct ModuleInfo {
    const Decl* decl = nullptr;
    const Decl* container = nullptr;
    std::string name;
    std::string access;
    std::string qualifiedName;
    SourceLocation loc;
};

class ModuleTable {
public:
    void build(const Program& program, Diagnostics& diagnostics);
    const ModuleInfo* resolve(std::string_view name,
                              const Decl* currentContainer,
                              Diagnostics& diagnostics,
                              SourceLocation loc,
                              bool optional) const;
};
```

### 8.2 查找规则

推荐阶段 3 查找策略：

```text
1. 当前 library/scope 内的 private/public/default module 优先。
2. 全局 module 次之。
3. 其他 library/scope 中的 public/default module 可见。
4. 其他 library/scope 中的 private module 不可见。
5. 如果同名 public/default module 多个可见，报歧义。
6. optional module 未找到：忽略并 warning 或记录 stats。
7. 非 optional module 未找到：error。
```

真实输入中常见：

```zinc
library UIBaseModule {
    public module uiBaseModule { ... }
}

library UIButton requires UIBaseModule {
    public struct uiBtn {
        module uiBaseModule;
    }
}
```

因此即使 module 声明在另一个 library 中，只要是 public 并且名称唯一，就应能解析。

### 8.3 重复 module 声明

vJass 教程中提到重复 module/ISM 可以被忽略。阶段 3 推荐：

```text
- 同一 qualifiedName 重复：第一份有效，后续 warning 并忽略。
- 不同 scope/library 中同名 module：允许，但跨作用域 raw name 查找可能歧义。
- 重复 module use：同一 struct 中对同一个 resolved module 重复使用，忽略第二次。
```

### 8.4 循环 module use

例如：

```jass
module A
    implement B
endmodule
module B
    implement A
endmodule
```

处理：

```text
- 非 optional 循环：error，提示 module expansion cycle。
- optional 不应掩盖真实循环；如果两个模块都存在，即使 optional 也应该报 cycle。
```

---

## 9. 任务 E：ModuleExpander

### 9.1 总体行为

新增 `ModuleExpander`，在 Parser 之后、Codegen 之前运行。

接口建议：

```cpp
struct ModuleExpansionStats {
    size_t expansions = 0;
    size_t optionalMissing = 0;
    size_t duplicateUsesIgnored = 0;
};

class ModuleExpander {
public:
    Program expand(const Program& program,
                   const ModuleTable& modules,
                   Diagnostics& diagnostics,
                   ModuleExpansionStats& stats);
};
```

也可以原地修改 `Program&`，但建议返回 expanded copy，便于保留 `--emit-ast` 前后差异。

可新增 CLI：

```bash
--emit-expanded-ast build/expanded.ast.txt
```

非必须，但强烈推荐，方便后续调试。

### 9.2 展开顺序

对于 struct：

```jass
struct A
    integer x
    implement M1
    integer y
    implement M2
endstruct
```

推荐阶段 3 采用“源码顺序展开”：

```text
A.fields/methods 原有顺序
遇到 implement M1 时，把 M1 成员插入当前位置
遇到 implement M2 时，把 M2 成员插入当前位置
```

如果阶段 2 AST 暂时无法保留字段、方法、module use 的混合顺序，可以采用次优策略：

```text
- 先追加所有 module fields，再追加 struct own fields
- 先追加所有 module methods，再追加 struct own methods
```

但必须在 `docs/phase3_status.md` 中声明这个限制。推荐尽快引入 `StructMember` union 保留顺序：

```cpp
struct StructMember {
    enum class Kind { Field, Method, ModuleUse, Unsupported } kind;
    FieldDecl field;
    MethodDecl method;
    ModuleUseDecl moduleUse;
};
```

### 9.3 成员 clone 规则

module 成员 clone 到 struct 后：

```text
- field 变成目标 struct 的 field。
- method 变成目标 struct 的 method。
- static method 变成目标 struct 的 static method。
- thistype 在 lowering 时绑定为目标 struct。
- module 内访问 this、.field、无前缀 field，应当视作目标 struct 成员访问。
- module 内 module use 递归展开。
```

`source location` 应保留 module 原始行，用于诊断。

### 9.4 成员冲突规则

阶段 3 推荐规则：

```text
- 同一 struct 中重复使用同一个 module：忽略第二次。
- 不同 module 添加同名 field：error。
- 不同 module 添加同名 method：error。
- module 添加的 field/method 与 struct 自己已有同名成员：error。
- 如果重复来自同一个 module 的重复使用：ignore。
```

真实 vJass 中私有 module 成员有更细微的 name mangling 行为，但阶段 3 不要过度实现。先保证不静默生成冲突 JASS。

### 9.5 optional module

```zinc
optional module uiLifeCycle;
```

行为：

```text
- 找到模块：正常展开。
- 找不到模块：不报 error；stats.optionalMissing += 1；可 warning，也可静默。
```

推荐：默认静默，`--verbose` 或 `--emit-stats` 记录即可。真实地图中 optional module 很多，默认 warning 会刷屏。

### 9.6 module onInit

vJass 规则中，模块里的 `onInit` 会对每个使用该模块的 struct 执行一次。阶段 3 至少支持：

```jass
module M
    private static method onInit takes nothing returns nothing
        call BJDebugMsg("M init")
    endmethod
endmodule

struct A
    implement M
endstruct
```

输出模型可类似：

```jass
function s__A_M_onInit takes nothing returns nothing
    call BJDebugMsg("M init")
endfunction

function vjassc__init_structs takes nothing returns nothing
    call s__A_M_onInit()
endfunction
```

如果阶段 2 当前 `StructInfo` 只支持一个 `onInit`，需要改成多个 init hooks：

```cpp
std::vector<const MethodInfo*> onInitMethods;
```

或：

```cpp
std::vector<std::string> structInitializers;
```

确保顺序：

```text
module onInit 按 module 展开顺序执行
struct 自己的 onInit 按源码位置执行
struct initializers 仍应在 library initializer 之前执行
```

### 9.7 module onDestroy

如果模块中定义 `method onDestroy`，阶段 3 推荐支持：

```jass
module M
    method onDestroy takes nothing returns nothing
        call BJDebugMsg("M destroy")
    endmethod
endmodule

struct A
    implement M
    method onDestroy takes nothing returns nothing
        call BJDebugMsg("A destroy")
    endmethod
endstruct
```

输出 destroy 时应调用所有 destroy hooks。

推荐规则：

```text
- module onDestroy 按展开顺序执行。
- struct 自己的 onDestroy 最后执行，或按源码位置执行。
- 如果实现难度较大，先实现 module onDestroy + struct onDestroy 都收集进 destroy hook list。
```

真实 `input.j` 当前 module 中未明显出现 onDestroy，但这是 vJass 兼容性重要点，建议做 fixture。

### 9.8 module create 特殊处理

vJass 文档提到 `create` 在 module 中有特殊处理。阶段 3 建议最小支持：

```text
- 如果 module 提供 static method create，而 struct 自己没有 create：作为 struct create 使用。
- 如果 struct 自己已有 create，同时 module 也有 create：error。
- 多个 module 同时提供 create：error。
```

不要在阶段 3 做更复杂的 create override 兼容。

---

## 10. 任务 F：Parser 与 Codegen 集成细节

### 10.1 Parser 不再把 module/static if 直接 unsupported

修改 `Parser::preScanUnsupported`：

```text
- static if 应由 StaticIfPruner 先删除。
- module 声明与 module use 应进入 AST。
- 只有 module 内部出现尚未支持的 operator/stub/super/interface/delegate 时，才报 unsupported。
```

### 10.2 Codegen 不直接 emit module

`emitDeclGlobals`、`emitTypeOrNative`、`emitDeclFunctions` 遇到 `DeclKind::Module` 时应跳过。

原因：module 是编译期概念，不直接生成 JASS。

### 10.3 Struct lowering 使用 expanded Program

Codegen 的 `collectStructs` 必须基于展开后的 Program。检查：

```text
- module fields 已进入 target struct fields
- module methods 已进入 target struct methods
- module onInit 已进入 target struct init hooks
- module onDestroy 已进入 target struct destroy hooks
```

### 10.4 SymbolTable 使用 expanded Program

`SymbolTable::build(program)` 应在 ModuleExpander 后执行，否则 module 添加的成员不会被 rewrite。

### 10.5 `--emit-ast` 与 `--emit-expanded-ast`

建议行为：

```text
--emit-ast             输出 parse 后、module 展开前 AST
--emit-expanded-ast    输出 static if prune + module expand 后 AST
```

如果不新增 `--emit-expanded-ast`，则 `--emit-ast` 输出 expanded AST，并在 README 说明。

---

## 11. 任务 G：测试计划

### 11.1 static if 正例 fixtures

新增至少这些：

```text
phase3_static_if_library_true.input.j
phase3_static_if_library_false.input.j
phase3_static_if_else.input.j
phase3_static_if_elseif.input.j
phase3_static_if_nested.input.j
phase3_static_if_debug_mode.input.j
phase3_static_if_global_constant.input.j
phase3_static_if_zinc_inline.input.j
phase3_static_if_zinc_multiline.input.j
phase3_static_if_prunes_unsupported.input.j
```

示例 1：library true

```jass
library A
endlibrary
library B
    function Test takes nothing returns nothing
        static if LIBRARY_A then
            call BJDebugMsg("A")
        else
            call BJDebugMsg("missing")
        endif
    endfunction
endlibrary
```

期望输出只包含：

```jass
call BJDebugMsg("A")
```

示例 2：Zinc inline

```zinc
//! zinc
library A {}
library B {
    function Test() {
        static if (LIBRARY_A) { BJDebugMsg("A"); }
    }
}
//! endzinc
```

期望 JASS 中有：

```jass
call BJDebugMsg("A")
```

示例 3：false branch 含 unsupported，应被剪掉

```jass
library A
    function Test takes nothing returns nothing
        static if LIBRARY_Missing then
            function interface F takes nothing returns nothing
        else
            call BJDebugMsg("ok")
        endif
    endfunction
endlibrary
```

期望：

```text
- no functionInterfacesUnsupported
- output contains BJDebugMsg("ok")
```

### 11.2 static if 负例 fixtures

新增：

```text
phase3_negative_static_if_missing_endif.input.j
phase3_negative_static_if_bad_expr.input.j
phase3_negative_static_if_unmatched_else.input.j
phase3_negative_static_if_zinc_bad_brace.input.j
```

### 11.3 module 正例 fixtures

新增至少这些：

```text
phase3_module_vjass_simple.input.j
phase3_module_vjass_optional_missing.input.j
phase3_module_vjass_nested.input.j
phase3_module_zinc_simple.input.j
phase3_module_zinc_public_cross_library.input.j
phase3_module_zinc_optional_missing.input.j
phase3_module_zinc_nested.input.j
phase3_module_static_field.input.j
phase3_module_oninit.input.j
phase3_module_ondestroy.input.j
phase3_module_repeated_use_ignored.input.j
```

示例 1：vJASS simple

```jass
module M
    method foo takes nothing returns nothing
        call BJDebugMsg("foo")
    endmethod
endmodule

struct A
    implement M
endstruct

function Test takes nothing returns nothing
    local A a = A.create()
    call a.foo()
endfunction
```

期望：

```text
- no module unsupported
- generated function for A.foo exists
- call a.foo() rewritten to generated A foo function
```

示例 2：Zinc cross-library public module

```zinc
//! zinc
library MLib {
    public module M {
        method foo() {
            BJDebugMsg("foo");
        }
    }
}

library UseLib requires MLib {
    public struct A {
        module M;
    }
    function Test() {
        A a = A.create();
        a.foo();
    }
}
//! endzinc
```

期望：

```text
- M is resolved from MLib
- A gets foo method
- Test lowers to normal JASS call
```

示例 3：nested module

```zinc
//! zinc
library Demo {
    module A {
        method a() { BJDebugMsg("a"); }
    }
    module B {
        module A;
        method b() { BJDebugMsg("b"); }
    }
    struct S {
        module B;
    }
}
//! endzinc
```

期望：`S` 同时拥有 `a` 和 `b`。

### 11.4 module 负例 fixtures

新增：

```text
phase3_negative_module_missing.input.j
phase3_negative_module_cycle.input.j
phase3_negative_module_duplicate_field.input.j
phase3_negative_module_duplicate_method.input.j
phase3_negative_module_private_cross_library.input.j
phase3_negative_module_function_inside.input.j
```

### 11.5 真实 input.j 扫描测试

新增一个非 CTest 或 CTest 可选的 smoke test：

```bash
build/vjassc samples/input.j \
  --scan-only \
  --allow-unsupported \
  --emit-stats build/input.phase3.stats.json \
  --emit-ast build/input.phase3.ast.txt
```

验收字段：

```json
{
  "errors": 0,
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 0,
  "staticIfUnsupported": 0
}
```

预期仍可能有：

```json
{
  "functionInterfacesUnsupported": 10
}
```

如果 lambda 统计新增，则预期也会有 `lambdasUnsupported > 0`。

---

## 12. 任务 H：性能要求

阶段 2 真实输入 scan-only 约 391ms。阶段 3 增加 static if pruner 和 module expansion 后，目标：

```text
优先目标：total <= 600ms
可接受目标：total <= 1000ms
不接受：超过 2000ms 且无解释
```

实现建议：

```text
- static if 条件解析器只解析出现的条件，不全文件 token 化。
- module table 用 unordered_map。
- module expansion clone 时避免大字符串重复复制过多，bodyLines 可以先浅拷贝或保留 source span。
- 不要在每一行 rewrite 时全量遍历所有 module。
- 对 module expansion 做 visited set，避免重复展开。
```

新增 stats：

```json
"timingMs": {
  "read": 41,
  "preprocess": 79,
  "staticIf": 20,
  "lex": 173,
  "parse": 127,
  "moduleExpand": 20,
  "total": 500
}
```

具体数值不要求完全一致，但要输出字段。

---

## 13. 任务 I：文档更新

### 13.1 README.md

更新 README：

```text
- 当前是 phase-3 compiler prototype
- Phase 3 adds static if pruning and module expansion
- Still not complete JassHelper replacement
- Remaining unsupported: function interface, lambda, interface/delegate/operator/stub/super
```

CLI 示例增加：

```bash
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase3.stats.json
build/vjassc tests/fixtures/phase3_module_zinc_simple.input.j -o build/phase3_module_zinc_simple.out.j
```

### 13.2 docs/phase3_status.md

新增文件：

```markdown
# Phase 3 Status

Date: YYYY-MM-DD

## Implemented
- static if symbol collection
- LIBRARY_X / DEBUG_MODE / constant boolean evaluation
- vJASS static if then/elseif/else/endif pruning
- Zinc static if block/inline pruning
- module parser: vJASS and Zinc
- module use parser: implement / implement optional / module / optional module
- ModuleTable and ModuleExpander
- module expansion into struct fields/methods
- module onInit/onDestroy handling
- real input scan baseline

## Not Implemented
- function interface lowering
- lambda lowering
- interface/delegate/operator/stub/super
- full expression AST
- byte-for-byte JassHelper matching

## input.j Scan Baseline
{ ... }

## Known Limitations
- explain any static if expressions not supported, if any
- explain any module special cases not fully compatible, if any
```

### 13.3 docs/phase2_status.md

不要覆盖 phase2 状态，只在 README 指向 phase3。

---

## 14. 验收标准

Codex 完成后必须满足：

```text
1. cmake configure 成功。
2. cmake build 成功。
3. ctest --output-on-failure 全部通过。
4. phase1/phase2 fixtures 不回归。
5. phase3 static if fixtures 通过。
6. phase3 module fixtures 通过。
7. phase3 negative fixtures 能正确失败并包含可读 diagnostics。
8. samples/input.j scan-only 成功。
9. input.phase3.stats.json 中：
   - errors == 0
   - structsUnsupported == 0
   - methodsUnsupported == 0
   - modulesUnsupported == 0
   - staticIfUnsupported == 0
10. 完整 samples/input.j codegen 仍然因后续 unsupported 安全拒绝，不允许生成半残 war3map.j。
11. README.md 与 docs/phase3_status.md 已更新。
```

---

## 15. 推荐提交顺序

建议 Codex 按以下 commit 顺序执行：

```text
commit 1: cleanup codegen/test naming and stats fields
commit 2: add BoolExpr parser and StaticIfSymbolCollector
commit 3: add StaticIfPruner for vJASS static if
commit 4: add Zinc static if block/inline pruning
commit 5: add module AST and parser support
commit 6: add ModuleTable and resolver
commit 7: add ModuleExpander and integrate with codegen
commit 8: add fixtures and negative tests
commit 9: add input.j phase3 scan baseline and docs
```

如果 Codex 不能多 commit，至少要在最终 PR 描述中按以上模块说明改动。

---

## 16. Codex 注意事项

### 16.1 不要用粗暴正则完成所有逻辑

可以用 regex 做简单 header 提取，但以下内容不能只靠 regex：

```text
- static if 条件表达式
- Zinc static if brace block
- module recursive expansion
- module cycle detection
- string/rawcode/comment 保护
```

### 16.2 不要修改阶段 1/2 的语义边界

保持：

```text
--allow-unsupported 只服务 scan-only。
真正 codegen 遇到 unsupported 必须拒绝。
```

### 16.3 不要为了 input.j 强行吞错误

如果完整 `input.j` 出现阶段 3 范围外问题，可以保持 unsupported。不能为了 stats 好看而把未知语法静默删除。

### 16.4 不要在阶段 3 做 function interface/lambda

虽然完整 `input.j` 中有大量匿名函数和 function type，但这些属于下一阶段。阶段 3 的目标是消除 module/static if blocker。

### 16.5 module 展开后仍然走 struct lowering

不要写一套独立的 module codegen。正确路径是：

```text
module members -> clone into struct AST -> existing struct lowering
```

---

## 17. 阶段 3 完成后的下一阶段预告

阶段 3 完成后，下一阶段建议进入：

```text
阶段 4：function interface / function type / lambda / anonymous function lowering
```

阶段 4 的目标将是处理：

```text
- function interface F takes ... returns ...
- type Xxx extends function(...)
- .execute / .evaluate
- Zinc function(...) { ... } anonymous function
- TriggerAddCondition / Condition(function() { ... })
- code / boolexpr callback wrapper
- prototype trigger arrays / sa__ / sc__ / st__ 风格包装
```

这会是完整 `input.j` codegen 前的下一个关键 blocker。
