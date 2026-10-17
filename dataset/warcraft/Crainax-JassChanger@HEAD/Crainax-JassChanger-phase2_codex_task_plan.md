---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase2_codex_task_plan.md"
ref: HEAD
---

# vJass/Zinc -> JASS 编译器：阶段 2 任务计划

> 交付对象：Codex / 编码代理
> 目标仓库：`Crainax/JassChanger`
> 推荐语言：继续使用 C++20
> 阶段 2 总目标：在阶段 1 编译器骨架上，实现 **struct / method 的 AST 解析、符号建表、基础语义检查、JASS lowering 与测试体系**。
> 阶段 2 成功标准：对阶段 2 范围内的小型 vJASS/Zinc struct 用例可生成纯 JASS；对完整 `samples/input.j` 能稳定扫描，并将 `structsUnsupported` 与 `methodsUnsupported` 降到 0 或接近 0；仍然允许 `module/static if/function interface/lambda` 等后续阶段内容保留为 unsupported。

---

## 0. 当前项目状态与阶段 2 边界

阶段 1 已经完成以下基础能力：

```text
- CMake C++20 项目，生成 vjassc
- CLI：--help、--version、--scan-only、--debug、--release、--emit-preprocessed、--emit-tokens、--emit-ast、--emit-stats、--import-path、--allow-unsupported
- SourceManager / Diagnostics / Preprocessor / Lexer / Parser / SymbolTable / LibraryGraph / Phase1Codegen 分层
- import / novjass / debug / textmacro / runtextmacro / zinc mode 预处理
- JASS-like 与 Zinc 的阶段 1 lexer
- 顶层 globals/native/type/function/library/library_once/scope 解析
- library dependency sorting
- public/private 函数与 globals 基础改名
- 基础 Zinc function lowering
- golden fixture runner
- 完整 input.j 的 scan-only 统计
```

阶段 1 的真实输入扫描基线：

```json
{
  "lines": 104238,
  "zincBlocks": 307,
  "libraries": 329,
  "libraryOnce": 9,
  "globalsBlocks": 494,
  "natives": 551,
  "functions": 3575,
  "structsUnsupported": 162,
  "methodsUnsupported": 1370,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "functionInterfacesUnsupported": 10,
  "unsupported": 1686,
  "totalMs": 363
}
```

阶段 2 只解决其中最关键的一组 unsupported：

```text
本阶段要实现：
- struct 声明解析
- method / static method 声明解析
- struct 字段解析
- thistype / this / .field 基础处理
- struct 类型变量降级为 integer
- 实例字段降级为全局数组
- static 字段降级为普通全局变量或数组
- method 降级为普通 function
- static method 降级为普通 function
- create / allocate / destroy / deallocate 基础生命周期
- onDestroy 调用
- static method onInit 初始化收集与注入
- Zinc struct 基础语法
- vJASS struct 基础语法
- struct Name [] / extends array 风格的 array struct 基础支持
```

阶段 2 明确不做：

```text
不做：
- function interface / prototype wrapper
- lambda / anonymous function lowering
- module 展开与 implement
- static if 条件求值
- interface 多态 / typeid / getType
- delegate
- operator [] / []= 重载
- stub / super
- hooks
- 泛型、new/delete/let、JassForge 扩展语法
- 字节级匹配 output_jasshelper.j
- 完整 input.j 生成最终可运行 war3map.j
```

如果遇到 `module/static if/function interface/lambda/interface/delegate/operator/stub/super`，阶段 2 可以继续报告 unsupported，但不能因为 `struct` 或 `method` 自身导致 unsupported。

---

## 1. 阶段 2 交付物

Codex 完成后，仓库至少应新增或更新：

```text
src/parser/Ast.h 或 src/ast/Ast.h
src/parser/Parser.h
src/parser/Parser.cpp
src/sema/SymbolTable.h
src/sema/SymbolTable.cpp
src/sema/StructTable.h              # 可选，也可以合并进 SymbolTable
src/sema/StructTable.cpp            # 可选
src/codegen/StructLowering.h        # 推荐新增
src/codegen/StructLowering.cpp      # 推荐新增
src/codegen/Phase1Codegen.h         # 可改名 PhaseCodegen 或保留
src/codegen/Phase1Codegen.cpp
src/lexer/Token.h
src/lexer/Lexer.cpp                 # 仅在 token 支持不足时修改
src/cli/CliOptions.*                # 仅在需要新增选项时修改
tests/fixtures/phase2_*.input.j
tests/fixtures/phase2_*.expected.j
docs/phase2_status.md
README.md
CMakeLists.txt
```

推荐新增 `src/codegen/StructLowering.*`，不要把所有 struct lowering 堆进 `Phase1Codegen.cpp`。阶段 1 的 `Phase1Codegen` 已经开始承担 globals/native/functions/library 初始化等工作，阶段 2 若继续往里硬塞，会使后续 module/function interface/static if 难以维护。

---

## 2. 开发环境与基线验证

### 2.1 拉取仓库并创建阶段 2 分支

```bash
git clone https://github.com/Crainax/JassChanger.git
cd JassChanger
git status
git branch -vv
git log --oneline --decorate -5

git checkout -b phase2-struct-method-lowering
```

如果当前环境已经有仓库：

```bash
cd JassChanger
git fetch origin
git checkout master
git pull --ff-only
git checkout -b phase2-struct-method-lowering
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
  --emit-stats build/input.phase1.stats.json \
  --emit-ast build/input.phase1.ast.txt
```

Codex 必须先确认阶段 1 基线能通过，再开始修改。

---

## 3. 需要先修正或澄清的阶段 1 小问题

这些不是阶段 2 的主目标，但建议在阶段 2 开头处理，避免后续误解。

### 3.1 `--allow-unsupported` 行为澄清

当前代码中 `Phase1Codegen::generate` 对 unsupported 的判断类似：

```cpp
if (program.hasUnsupported() && !options_.scanOnly) {
    diagnostics_.error(SourceLocation{}, "phase1 unsupported declarations prevent code generation");
    return CodegenResult{false, {}};
}
```

但 `main.cpp` 又把 `allowUnsupported` 传给 codegen。

阶段 2 请选择其中一种设计，并在 README / docs 中写清楚：

方案 A，推荐：

```text
--allow-unsupported 只允许 scan-only 继续完成统计；
只要真正 codegen，就不能忽略 unsupported。
```

这种方案更安全。真实地图代码里如果还有 `module/static if/lambda`，强行 codegen 会生成不完整脚本，容易误判。

方案 B，不推荐但可接受：

```text
--allow-unsupported 允许 codegen 忽略 unsupported 声明块，生成部分输出。
```

如果选择方案 B，必须确保输出文件顶部写入明显注释：

```jass
// WARNING: generated with unsupported declarations omitted
```

### 3.2 `read` timing 目前为 0

`emitStatsJson` 有 `read` 字段，但当前没有单独计时。阶段 2 可任选其一：

```text
方案 A：移除 read 字段。
方案 B：在 SourceManager::loadFile 或 Preprocessor::run 中记录文件读取耗时。
```

推荐方案 B，因为项目目标强调速度，后续需要性能基线。

### 3.3 测试拆分不足

阶段 1 现在只有一个 CTest 目标 `phase1_fixtures`。阶段 2 不一定要重构测试 runner，但必须增加足够 fixtures，并让测试输出能定位失败用例。

推荐改进：

```text
- golden_runner 打印当前 fixture 名称
- 每个 fixture 一个 .input.j + .expected.j
- 支持 .args 文件，允许某些 fixture 传 --debug 或其他参数
- 支持 .expect-fail 文件，未来用于负例
```

---

## 4. 阶段 2 总体架构要求

阶段 1 的 Parser 很多地方是行级 `trim/find/startsWithWord/regex` 解析。这对顶层扫描够用，但对 struct/method 已经不够。

阶段 2 不要求一次性实现完整 Pratt Parser，但必须做到：

```text
1. struct/method header 不能只靠简单 find 硬拆。
2. 需要建立明确 AST 节点，而不是把 struct 当 raw lines 存起来。
3. 方法体可以暂时保留 raw logical lines，但方法签名、字段、static/instance/access 必须结构化。
4. body lowering 可以先做 targeted rewriting，不要求完整表达式 AST。
5. 所有重写必须避免改坏字符串字面量、rawcode、注释。
6. 后续阶段可继续在此 AST 上扩展 module、function interface、static if。
```

推荐阶段 2 的编译流程：

```text
Preprocessor
  -> LogicalLine[] with SyntaxMode
Lexer
  -> Token[] for diagnostics / future parser
Parser
  -> Program AST
     - GlobalBlock
     - Native
     - TypeDecl
     - Function
     - Library
     - Scope
     - Struct
       - FieldDecl[]
       - MethodDecl[]
       - unsupported inner declarations
SymbolTable / StructTable
  -> type symbols, container symbols, struct members, generated names
LibraryGraph
  -> sorted libraries
StructLowering
  -> generated globals + generated functions + body rewriting
Codegen
  -> merged globals / natives / functions / init injection
```

---

## 5. AST 数据结构设计

### 5.1 新增或扩展 DeclKind

当前 `DeclKind::Unsupported` 里包含 struct/method。阶段 2 要改为：

```cpp
enum class DeclKind {
    GlobalBlock,
    Native,
    TypeDecl,
    Function,
    Library,
    Scope,
    Struct,
    Unsupported
};
```

### 5.2 TypeRef

```cpp
struct TypeRef {
    std::string name;          // integer, real, unit, thistype, MyStruct, etc.
    SourceLocation loc;
    bool isArray = false;      // 用于声明层面的 array / []，不是字段固定数组长度
};
```

阶段 2 不要求完整类型系统，但要能判断：

```text
- 基础 JASS 类型
- 用户 type alias
- struct 类型
- thistype
- unknown custom type：先接受，按名字输出或按 struct table 判断
```

### 5.3 ParamDecl

```cpp
struct ParamDecl {
    TypeRef type;
    std::string name;
    SourceLocation loc;
};
```

vJASS 参数格式：

```jass
takes integer a, unit u returns nothing
```

Zinc 参数格式：

```jass
method move(integer x, real y) -> nothing { }
```

阶段 2 可暂不支持默认参数、`&` 引用参数、泛型参数。

### 5.4 StructDecl

```cpp
struct StructDecl {
    std::string name;              // 原始名
    std::string access;            // public/private/empty
    SyntaxMode mode;               // JassLike / Zinc
    SourceLocation loc;

    bool libraryPrivate = false;   // 可由 container + access 推导，也可不存
    bool isArrayStruct = false;    // Zinc `struct X []` 或 vJASS `extends array`
    std::string extendsName;       // 暂只识别 extends array；其他 extends 作为 unsupported

    std::vector<FieldDecl> fields;
    std::vector<MethodDecl> methods;
    std::vector<Decl> unsupportedChildren;

    std::string generatedName;     // name mangling 后的 struct 名
    std::string prefix;            // s__ + generatedName
};
```

### 5.5 FieldDecl

```cpp
struct FieldDecl {
    std::string name;
    TypeRef type;
    std::string access;          // public/private/empty
    SourceLocation loc;

    bool isStatic = false;
    bool isConstant = false;
    bool isReadonly = false;     // 解析即可，阶段 2 可不做完整赋值检查
    bool isArray = false;        // `type array x` 或 `type x[]`
    bool isFixedArray = false;   // `type x[50]`
    int fixedArraySize = 0;

    std::string initializer;     // 原始表达式文本，暂不做完整 expression AST
    std::string generatedName;   // s__Struct_field 或 s___Struct_field
};
```

必须支持一行多个字段：

```jass
integer x, y, z
static integer A = 1, B = 2
```

Zinc 中必须支持：

```jass
integer x;
integer dID,dTime,dNow;
static thistype DList[] , MList[];
static integer DNum = 0 , MNum = 0;
icon ic[50];
boolean alignWidth[50];
```

### 5.6 MethodDecl

```cpp
struct MethodDecl {
    std::string name;
    std::string access;          // public/private/empty
    SyntaxMode mode;
    SourceLocation loc;

    bool isStatic = false;
    bool isOperator = false;     // 阶段 2 识别但 unsupported
    bool isOnDestroy = false;
    bool isOnInit = false;

    std::vector<ParamDecl> params;
    TypeRef returnType;
    std::vector<LogicalLine> bodyLines;

    std::string generatedName;   // s__Struct_method 或 sc__Struct_onDestroy 等
};
```

阶段 2 不要求完整 method body AST，但 bodyLines 必须稳定保留源码位置，便于 diagnostics。

---

## 6. Parser 任务

### 6.1 取消 struct/method 的 unsupported 预扫描

当前 `preScanUnsupported` 会把 `struct` 和 `method` 计入 unsupported。阶段 2 要改成：

```text
- struct 不再 unsupported
- method 不再 unsupported，但 method outside struct 应报 error
- module/static if/function interface 继续 unsupported
- operator/stub/super/interface/delegate 可继续 unsupported
```

阶段 2 完成后，对完整 `input.j` 的 stats 应大致变为：

```json
{
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "functionInterfacesUnsupported": 10
}
```

如果因为某些特殊 struct/method 语法尚未支持，允许少量 method unsupported，但必须在 `docs/phase2_status.md` 说明原因和样例。

### 6.2 vJASS struct 解析

支持语法：

```jass
[public|private] struct Name [extends array]
    [fields]
    [methods]
endstruct
```

字段：

```jass
[public|private] [static] [constant] [readonly] type [array] name [= init]
[public|private] [static] type name[arraySize]
```

方法：

```jass
[public|private] [static] method Name takes ... returns ...
    ...
endmethod
```

示例：

```jass
struct Point
    real x = 0.0
    real y = 0.0

    method move takes real nx, real ny returns nothing
        set this.x = nx
        set this.y = ny
    endmethod
endstruct
```

解析要求：

```text
- method body 捕获到 endmethod
- struct body 捕获到 endstruct
- struct 内 function 不合法，报 error
- method 外出现 endmethod 报 error
- 缺少 endstruct / endmethod 报 error，并附 source location
- extends 非 array 暂时 unsupported 或 warning，不要崩溃
```

### 6.3 Zinc struct 解析

支持语法：

```jass
[public|private] struct Name [ [] ] [extends array] {
    fields;
    methods { ... }
}
```

真实 `input.j` 中存在很多 Zinc 结构体，且大量使用以下写法：

```jass
public struct baseanim {
    static thistype DList[] , MList[];
    static integer DNum = 0 , MNum = 0;
    integer ui;
    method isExist () -> boolean {return (this != null && si__baseanim_V[this] == -1);}
    static method create (integer ui) -> thistype {
        thistype this = allocate();
        this.ui = ui;
        return this;
    }
}
```

```jass
public struct unitAttrObserver [] {
    public static unit argsU = null;
    public static trigger attackIntervalCB = null;
    public static method registerAttackInterval (code func) {
        ...
    }
}
```

阶段 2 必须支持：

```text
- `struct Name { ... }`
- `struct Name [] { ... }`，视为 array struct / extends array
- `public struct` / `private struct`
- `method name(args) { ... }`，无 `->` 时返回 nothing
- `method name(args) -> type { ... }`
- `static method name(args) { ... }`
- `static method name(args) -> type { ... }`
- 一行 method：`method isExist() -> boolean { return ...; }`
- 多行 method
- 字段一行多个声明：`integer a,b,c;`
- 字段数组：`thistype list[];`
- 固定数组字段：`icon ic[50];`
- 注释与空行
- 字符串中的 `{`、`}` 不能影响 brace depth
- rawcode / 单引号中的 `{`、`}` 不能影响 brace depth
```

阶段 2 可继续不支持，但必须正确跳过/计数：

```jass
module SomeModule;
optional module SomeModule;
method operator [](...)
stub method ...
super.method(...)
function(...) { ... }  // lambda
```

### 6.4 Zinc brace depth 规则

Zinc method body 捕获不能只用 `line.count('{') - line.count('}')`。必须写一个小 scanner：

```text
状态：
- normal
- double quoted string
- single quoted rawcode
- line comment
- escaped char

只在 normal 状态下统计 `{` 和 `}`。
```

否则以下代码会错误结束：

```jass
DzFrameSetTexture(frame, "UI\\Widgets\\{abc}.blp", 0);
```

### 6.5 多声明拆分

Zinc 和 vJASS 都可能有多变量声明：

```jass
integer a,b,c;
static integer A = 1 , B = 2;
static thistype DList[] , MList[];
```

实现 `splitCommaListRespectingQuotesAndBrackets`：

```text
逗号分割时跳过：
- 字符串
- rawcode
- 小括号
- 中括号
- 花括号
```

字段解析不要把函数调用参数里的逗号切开。

---

## 7. SymbolTable / StructTable 任务

### 7.1 基础类型表

内建类型至少包括：

```text
integer real boolean string code handle nothing
unit player timer trigger effect group force rect location item destructable
widget image sound region hashtable boolexpr dialog button
```

未知类型不要立刻报错，因为地图里可能有 `type` 声明、平台 JAPI 类型、或后续 struct。先记录 unresolved，等第二遍建表后再决定。

### 7.2 struct 类型注册

对每个 `StructDecl` 注册：

```text
- 原始名称
- 当前 container 内可见名称
- public/private 修饰后的外部名称
- generatedName
- 是否 array struct
- 字段表
- 方法表
```

命名建议：

```text
顶层 public/default struct Point -> Point
library Lib public struct Point -> Lib_Point
library Lib private struct Point -> Lib___Point 或沿用现有 SymbolTable 的 private 规则
生成前缀 -> s__<generatedName>
```

如果阶段 1 已有 deterministic private prefix，继续沿用，不要引入随机数。JassHelper private 可能有随机化，但本项目当前更需要可测试、可复现。

### 7.3 struct 成员检查

至少检查：

```text
- 同一 struct 中字段重名 -> error
- 同一 struct 中方法重名 -> error，create/destroy/onDestroy/onInit 可按规则特殊处理
- 字段名与方法名同名 -> warning 或 error，推荐 error
- array struct 中如果调用 allocate/create/destroy 且未自定义 -> warning 或 error
- onDestroy 必须是 instance method，返回 nothing，无参数
- onInit 必须是 static method，返回 nothing，无参数
- create 若自定义，必须 static，返回 thistype 或本 struct 类型
- destroy 若自定义，允许 instance destroy() 或 static destroy(thistype)；阶段 2 可先不支持自定义 destroy，遇到时报 unsupported
```

### 7.4 this / thistype 上下文

在 method body lowering 中需要知道当前 struct：

```text
- `this` 的类型是当前 struct
- `thistype` 在类型位置等价当前 struct 类型
- `thistype` 在静态成员访问中等价当前 struct 名
- `.field` shorthand 等价 `this.field`
- `.method(args)` shorthand 等价 `this.method(args)`
```

---

## 8. Struct lowering 设计

### 8.1 生成命名规则

采用接近 JassHelper 的命名，方便后续与 `output_jasshelper.j` 对比：

```text
struct 类型 ID：       si__Point
allocator free list：  si__Point_F
allocator max index：  si__Point_I
allocator state：      si__Point_V
字段：                s__Point_x
静态字段：            s__Point_count
方法：                s__Point_move
allocate：            s__Point__allocate
create：              s__Point_create
onDestroy wrapper：   sc__Point_onDestroy
```

阶段 2 不要求与 JassHelper 完全同名，但建议尽量接近。真实 `output_jasshelper.j` 中大量使用 `s__ / si__ / sc__ / st__ / sa__` 前缀，后续阶段会依赖这一风格做差异分析。

### 8.2 struct 类型最终为 integer

所有 struct 类型在 JASS 输出中都应变成 `integer`：

```jass
local Point p
```

降级为：

```jass
local integer p
```

参数：

```jass
function Foo takes Point p returns Point
```

降级为：

```jass
function Foo takes integer p returns integer
```

Zinc：

```jass
Point p = Point.create();
```

降级为：

```jass
local integer p
set p = s__Point_create()
```

### 8.3 非 array struct 的 allocator

对普通 struct 生成：

```jass
globals
    constant integer si__Point=1
    integer si__Point_F=0
    integer si__Point_I=0
    integer array si__Point_V
endglobals
```

生成 allocate：

```jass
function s__Point__allocate takes nothing returns integer
    local integer this=si__Point_F
    if (this!=0) then
        set si__Point_F=si__Point_V[this]
    else
        set si__Point_I=si__Point_I+1
        set this=si__Point_I
    endif
    if (this>8190) then
        return 0
    endif
    set si__Point_V[this]=-1
    return this
endfunction
```

生成 deallocate：

```jass
function s__Point_deallocate takes integer this returns nothing
    if (this==0) then
        return
    endif
    set si__Point_V[this]=si__Point_F
    set si__Point_F=this
endfunction
```

如果 struct 没有自定义 create，生成默认 create：

```jass
function s__Point_create takes nothing returns integer
    local integer this=s__Point__allocate()
    if (this==0) then
        return 0
    endif
    // instance field default initializers here
    return this
endfunction
```

如果 struct 有自定义 static method create，按照普通 static method lowering 输出为 `s__Point_create`，但函数体内的 `allocate()` / `thistype.allocate()` 要改成 `s__Point__allocate()`。

### 8.4 destroy / onDestroy

如果存在：

```jass
method onDestroy takes nothing returns nothing
    set this.u = null
endmethod
```

生成：

```jass
function sc__Point_onDestroy takes integer this returns nothing
    set s__Point_u[this]=null
endfunction
```

默认 destroy：

```jass
function s__Point_destroy takes integer this returns nothing
    if (this==0) then
        return
    endif
    call sc__Point_onDestroy(this) // 仅当存在 onDestroy
    call s__Point_deallocate(this)
endfunction
```

调用改写：

```jass
call p.destroy()
```

变成：

```jass
call s__Point_destroy(p)
```

```jass
call Point.destroy(p)
```

变成：

```jass
call s__Point_destroy(p)
```

阶段 2 可不支持自定义 destroy 覆盖；如遇到 `method destroy` 或 `static method destroy`，先报 unsupported，并在 `phase2_status.md` 记录。

### 8.5 array struct / `struct Name []`

真实 `input.j` 中大量存在：

```jass
public struct uilayer [] {
    static integer lv [];
    static method onInit () { ... }
}
```

阶段 2 将其视作 vJASS `extends array` 风格：

```text
- 不生成 F/I/V allocator
- 不生成默认 create/destroy/allocate/deallocate
- 仍生成 si__Name 常量，用于 type id / 兼容输出
- 允许 static fields 和 static methods
- 若存在 instance fields，也可以生成 array 字段，但不自动分配实例
- 若代码调用 Name.create()/allocate()/destroy() 且没有自定义方法，报 error 或 unsupported
```

array struct 的 globals 示例：

```jass
globals
    constant integer si__uilayer=119
    integer array s__uilayer_lv
endglobals
```

### 8.6 字段 lowering

实例字段：

```jass
struct Point
    real x
    real y
endstruct
```

输出：

```jass
globals
    real array s__Point_x
    real array s__Point_y
endglobals
```

访问：

```jass
set this.x = nx
set p.y = 5.0
return p.x + p.y
```

输出：

```jass
set s__Point_x[this]=nx
set s__Point_y[p]=5.0
return s__Point_x[p] + s__Point_y[p]
```

静态字段：

```jass
static integer count = 0
static unit array units
```

输出：

```jass
integer s__Point_count=0
unit array s__Point_units
```

访问：

```jass
set Point.count = Point.count + 1
set thistype.count = thistype.count + 1
set count = count + 1       // 在 struct method 内可直接访问静态字段
```

输出：

```jass
set s__Point_count=s__Point_count+1
```

### 8.7 固定数组字段

真实输入中存在：

```jass
icon ic[50];
uiText text[50];
boolean alignWidth[50];
```

阶段 2 至少支持解析并生成可重写的线性数组模型。推荐：

```jass
globals
    constant integer s___tooltip_ic_size=50
    integer array s__tooltip_ic
endglobals
```

访问：

```jass
set this.ic[i] = 0
call this.ic[i].destroy()
```

输出：

```jass
set s__tooltip_ic[this*50+i]=0
call s__icon_destroy(s__tooltip_ic[this*50+i])
```

更安全的索引公式可用：

```jass
(this * s___tooltip_ic_size + i)
```

注意：JassHelper 真实输出里可能还会生成额外辅助数组，如 `s___tooltip_ic` 和 `s___tooltip_ic_size`。阶段 2 可先采用单数组线性化，只要生成 JASS 可运行、语义合理即可。后续再向 JassHelper 输出逼近。

### 8.8 字段默认值

实例字段默认值：

```jass
integer x = 1
string name = ""
```

在默认 create 或自定义 create 的 allocate 后应初始化。阶段 2 可采用：

```text
- 如果没有自定义 create：默认 create 中写默认字段赋值。
- 如果有自定义 create：不自动插入，除非实现能可靠找到 allocate 后的位置。
```

为了更接近 JassHelper，可在 `s__Point__allocate` 后统一调用 `s__Point__init_fields(this)`：

```jass
function s__Point__init_fields takes integer this returns nothing
    set s__Point_x[this]=1
    set s__Point_name[this]=""
endfunction
```

然后：

```jass
function s__Point_create takes nothing returns integer
    local integer this=s__Point__allocate()
    call s__Point__init_fields(this)
    return this
endfunction
```

自定义 create 中，如果代码调用 `allocate()`，阶段 2 可以不自动插入字段默认值。请在 `phase2_status.md` 说明该限制。

---

## 9. Method lowering 设计

### 9.1 instance method

输入：

```jass
method move takes real nx, real ny returns nothing
    set this.x = nx
    set this.y = ny
endmethod
```

输出：

```jass
function s__Point_move takes integer this, real nx, real ny returns nothing
    set s__Point_x[this]=nx
    set s__Point_y[this]=ny
endfunction
```

Zinc：

```jass
method move(real nx, real ny) {
    this.x = nx;
    this.y = ny;
}
```

输出同上。

### 9.2 static method

输入：

```jass
static method make takes nothing returns thistype
    return thistype.create()
endmethod
```

输出：

```jass
function s__Point_make takes nothing returns integer
    return s__Point_create()
endfunction
```

### 9.3 方法调用改写

实例方法：

```jass
call p.move(1.0, 2.0)
p.move(1.0, 2.0);        // Zinc
```

输出：

```jass
call s__Point_move(p, 1.0, 2.0)
```

静态方法：

```jass
call Point.make()
Point.make();
thistype.make();
```

输出：

```jass
call s__Point_make()
```

在当前 struct 内直接调用：

```jass
call move(1.0, 2.0)      // 若 move 是当前 struct instance method，JassHelper 是否允许要谨慎
move(1.0, 2.0);          // Zinc
```

阶段 2 推荐只支持显式 `this.move(...)` / `p.move(...)` / `Point.make(...)`。直接裸调用同名方法容易与普通函数冲突，可先不支持或只在无歧义时支持。

### 9.4 `.field` 与 `.method` shorthand

vJASS 文档允许在实例方法内部用：

```jass
set .x = 10
call .move(1, 2)
```

阶段 2 要支持：

```jass
set .x = 10
```

输出：

```jass
set s__Point_x[this]=10
```

### 9.5 `function Struct.staticMethod`

静态方法可以作为 code 值：

```jass
call TriggerAddAction(t, function Point.tick)
```

输出：

```jass
call TriggerAddAction(t, function s__Point_tick)
```

阶段 2 只支持无参、返回 nothing 的 static method 作为 code 引用。实例方法作为 code 暂不支持。

---

## 10. Body rewriting 规则

阶段 2 不要求完整表达式 AST，但必须实现可靠的 targeted rewriting。

### 10.1 不要改字符串和 rawcode

以下内容不能被替换：

```jass
call BJDebugMsg("this.x should not be replaced")
local integer id = 'hfoo'
```

实现方式：

```text
写一个 scanAndRewriteIdentifiers(text, context) 小工具：
- normal 状态识别 identifier / dot / brackets / call pattern
- string 状态原样复制
- rawcode 状态原样复制
- comment 状态原样复制或丢弃
```

不要用全局 regex 简单替换 `this.`。

### 10.2 类型位置改写

需要改写这些位置：

```text
- function returns 类型
- function takes 参数类型
- method returns 类型
- method 参数类型
- local 声明类型
- globals 字段类型
- Zinc 局部声明类型
```

规则：

```text
struct type / thistype -> integer
struct array -> integer array
基础类型 -> 原样
未知类型 -> 原样
```

### 10.3 表达式位置改写

要识别：

```text
this.field
.field
var.field
Struct.staticField
thistype.staticField
this.method(args)
.method(args)
var.method(args)
Struct.staticMethod(args)
thistype.staticMethod(args)
Struct.create(args)
Struct.allocate()
Struct.destroy(x)
x.destroy()
function Struct.staticMethod
```

阶段 2 可以暂不支持：

```text
a.b.c 深层链式自动推导
泛型调用
operator overload
lambda
interface dynamic dispatch
```

但真实 `input.j` 有类似：

```jass
DList[dID].dID = dID;
this.ic[i].destroy();
```

因此建议至少支持一层数组取值后的方法/字段：

```text
DList[dID].dID
```

如果 `DList` 是 `thistype` 数组，则 `DList[dID]` 的类型为当前 struct，可以改写为：

```jass
set s__baseanim_dID[s__baseanim_DList[dID]]=dID
```

这一步会显著提升对真实输入的覆盖率。

### 10.4 Zinc 局部变量声明仍要提升到函数顶部

阶段 1 已有 `lowerZincBody`，会把局部变量收集到函数顶部。阶段 2 的 method lowering 也要复用或扩展该逻辑：

输入：

```jass
method add(integer x) {
    integer y = x + 1;
    this.value = y;
}
```

输出：

```jass
function s__Foo_add takes integer this, integer x returns nothing
    local integer y
    set y=x+1
    set s__Foo_value[this]=y
endfunction
```

注意 struct 类型局部变量：

```jass
thistype this = allocate();
Point p = Point.create();
```

输出：

```jass
local integer this
local integer p
set this=s__Foo__allocate()
set p=s__Point_create()
```

---

## 11. 初始化顺序

### 11.1 static method onInit

输入：

```jass
struct A
    static method onInit takes nothing returns nothing
        call BJDebugMsg("A")
    endmethod
endstruct
```

输出：

```jass
function s__A_onInit takes nothing returns nothing
    call BJDebugMsg("A")
endfunction
```

并生成：

```jass
function vjassc__init_structs takes nothing returns nothing
    call s__A_onInit()
endfunction
```

### 11.2 注入 main

当前阶段 1 已有 `vjassc__init_libraries()` 注入。阶段 2 修改为：

```jass
function vjassc__init_structs takes nothing returns nothing
    ...
endfunction

function vjassc__init_libraries takes nothing returns nothing
    ...
endfunction
```

在 `main` 中：

```jass
call InitBlizzard()
call vjassc__init_structs()
call vjassc__init_libraries()
```

如果没有 `InitBlizzard()`，就在 `main` 开头或函数末尾 fallback 插入，但必须保持 deterministic。

vJass 规则中 struct onInit 通常早于 library initializer；阶段 2 按此规则实现。

---

## 12. Codegen 输出顺序

建议输出顺序：

```text
1. // Generated by vjassc phase2
2. globals
   - LIBRARY_xxx constants
   - struct type id / allocator globals
   - struct fields globals
   - user globals
3. endglobals
4. type declarations
5. native declarations
6. struct support functions
   - onDestroy wrappers
   - init field helpers
   - allocate/deallocate/create/destroy
   - methods/static methods/onInit
7. library functions in dependency order
8. root functions
9. vjassc__init_structs
10. vjassc__init_libraries
11. main with injected init calls
```

JASS 要求函数先定义后使用，因此 support functions 要尽量早于调用它们的普通函数。

---

## 13. 真实 input.j 中需要特别处理的语法形态

Codex 请不要只写玩具用例。真实 `input.j` 中至少存在这些写法：

```jass
public struct baseanim {
    static thistype DList[] , MList[] , AMList[];
    static integer DNum = 0 , MNum = 0;
    integer ui;
    method isExist () -> boolean {return (this != null && si__baseanim_V[this] == -1);}
    static method create (integer ui) -> thistype {
        thistype this = allocate();
        this.ui = ui;
        return this;
    }
    integer dID,dTime,dNow;
}
```

```jass
public struct unitAttrObserver [] {
    public static unit argsU = null;
    public static trigger attackIntervalCB = null;
    public static method registerAttackInterval (code func) {
        TriggerAddCondition(attackIntervalCB, Condition(func));
    }
}
```

```jass
public struct tooltip {
    static integer borderType = 0;
    uiBorder border;
    icon ic[50];
    uiText text[50];
    boolean alignWidth[50];
    integer iconCount;
    method onDestroy() {
        this.clear();
    }
}
```

```jass
private struct AbilityCDQueue [] {
    private static unit uList[];
    private static integer abilList[];
    private static integer size = 0;
    private static method ensureTimer() {
        TimerStart(thistype.tickTimer, 0.02, true, function () {
            // lambda: 阶段 2 仍 unsupported，但 brace capture 不能崩
        });
    }
}
```

要求：

```text
- 这些结构体至少能 parse 成 StructDecl。
- 对 lambda 所在 method，可把 method body 标记 containsUnsupportedLambda，但不能让 struct parser 断裂。
- module 行可作为 unsupported child，不要破坏后续字段/方法解析。
- static if 仍可 unsupported。
```

---

## 14. 测试计划

### 14.1 必须新增的 golden fixtures

新增以下 fixtures。命名可调整，但覆盖点不可减少。

#### 01. vJASS 基础 struct 字段

`tests/fixtures/phase2_struct_basic_vjass.input.j`

```jass
struct Point
    real x
    real y
endstruct

function Test takes nothing returns nothing
    local Point p = Point.create()
    set p.x = 1.0
    set p.y = 2.0
endfunction
```

预期：

```text
- globals 有 si__Point_F/I/V
- globals 有 real array s__Point_x/y
- local Point -> local integer
- Point.create() -> s__Point_create()
- p.x -> s__Point_x[p]
```

#### 02. vJASS method lowering

```jass
struct Point
    real x
    real y
    method move takes real nx, real ny returns nothing
        set this.x = nx
        set this.y = ny
    endmethod
endstruct
```

预期：

```jass
function s__Point_move takes integer this, real nx, real ny returns nothing
```

#### 03. static field / static method

```jass
struct Counter
    static integer value = 0
    static method add takes integer x returns nothing
        set value = value + x
    endmethod
endstruct
```

预期：

```text
integer s__Counter_value=0
function s__Counter_add takes integer x returns nothing
set s__Counter_value=s__Counter_value+x
```

#### 04. custom create / allocate / thistype

```jass
struct Node
    integer v
    static method create takes integer x returns thistype
        local thistype this = thistype.allocate()
        set this.v = x
        return this
    endmethod
endstruct
```

预期：

```text
local integer this
set this=s__Node__allocate()
set s__Node_v[this]=x
return this
```

#### 05. onDestroy / destroy call

```jass
struct Box
    unit u
    method onDestroy takes nothing returns nothing
        set this.u = null
    endmethod
endstruct

function Test takes nothing returns nothing
    local Box b = Box.create()
    call b.destroy()
endfunction
```

预期：

```text
sc__Box_onDestroy(this)
s__Box_destroy(b)
```

#### 06. static onInit

```jass
struct InitMe
    static method onInit takes nothing returns nothing
        call BJDebugMsg("init")
    endmethod
endstruct
```

预期：

```text
function s__InitMe_onInit takes nothing returns nothing
function vjassc__init_structs takes nothing returns nothing
call s__InitMe_onInit()
```

#### 07. library public/private struct

```jass
library LibA
    public struct Pub
        integer x
    endstruct

    private struct Priv
        integer x
    endstruct
endlibrary
```

预期：

```text
public struct 使用 LibA_Pub 或既定 public 名称
private struct 使用 deterministic private 名称
字段和方法跟随 generatedName
```

#### 08. Zinc 基础 struct

```jass
//! zinc
library Demo {
    public struct Foo {
        integer value;
        method add(integer x) {
            this.value += x;
        }
    }
}
//! endzinc
```

预期：

```text
integer array s__Demo_Foo_value 或 s__Foo_value，取决于现有命名规则
+= 降级为 set field = field + x
```

#### 09. Zinc array struct

```jass
//! zinc
library Demo {
    public struct Queue [] {
        static integer size = 0;
        static method clear() {
            size = 0;
        }
    }
}
//! endzinc
```

预期：

```text
不生成 F/I/V allocator
生成 static field 和 static method
```

#### 10. 固定数组字段

```jass
//! zinc
library Demo {
    public struct Bag {
        integer items[10];
        method setItem(integer i, integer v) {
            this.items[i] = v;
        }
    }
}
//! endzinc
```

预期：

```text
constant integer s___Bag_items_size=10
integer array s__Bag_items
set s__Bag_items[this*10+i]=v 或等价公式
```

#### 11. static method code reference

```jass
struct Tick
    static method run takes nothing returns nothing
    endmethod
endstruct

function Test takes trigger t returns nothing
    call TriggerAddAction(t, function Tick.run)
endfunction
```

预期：

```text
function s__Tick_run takes nothing returns nothing
function Tick.run -> function s__Tick_run
```

#### 12. 真实 input fragment

从 `samples/input.j` 复制一个小 struct，例如 `unitAttrObserver []` 或 `triangleXY []`，建立 fixture。要求能 parse + codegen。

### 14.2 负例 tests

新增或扩展 runner 支持 expect-fail 后，加入：

```text
- duplicate field
- duplicate method
- method outside struct
- missing endstruct
- missing endmethod
- invalid onDestroy signature
- invalid onInit signature
- array struct calls default create
- unknown member access
```

如果 runner 暂不支持 expect-fail，可先把负例放到 `tests/negative/` 并在 `docs/phase2_status.md` 标记“手动验证命令”。

### 14.3 完整 input.j 扫描验收

阶段 2 完成后必须运行：

```bash
build/vjassc samples/input.j \
  --scan-only \
  --allow-unsupported \
  --emit-stats build/input.phase2.stats.json \
  --emit-ast build/input.phase2.ast.txt
```

预期：

```text
- exit code 0
- 不崩溃
- structsUnsupported 从 162 降到 0 或接近 0
- methodsUnsupported 从 1370 降到 0 或接近 0
- modulesUnsupported/staticIfUnsupported/functionInterfacesUnsupported 可保留
- total time 建议 < 1000ms；若超过，记录原因
```

---

## 15. 性能要求

阶段 2 不要为了实现 struct 而让扫描速度大幅下降。

目标：

```text
samples/input.j --scan-only --allow-unsupported:
  阶段 1 total: 约 363ms
  阶段 2 目标: < 1000ms
  警戒线: > 2000ms 必须分析原因
```

实现建议：

```text
- 不要在大文件上反复 substring 大块文本
- bodyLines 可引用 LogicalLine，不要复制超大字符串块
- identifier 使用现有 StringInterner 或轻量缓存
- struct/member lookup 用 unordered_map
- rewrite body 时只处理需要输出的 supported fixtures；scan-only 不必做完整 body rewrite
- AST 输出可控，避免 --emit-ast 默认生成超大内容
```

---

## 16. 文档更新要求

新增 `docs/phase2_status.md`，至少包含：

```markdown
# Phase 2 Status

Date: YYYY-MM-DD

## Implemented
- struct parser: vJASS / Zinc
- method parser: vJASS / Zinc
- field parser
- struct symbol table
- basic struct lowering
- allocate/create/destroy/onDestroy/onInit
- method call rewrite
- field access rewrite
- array struct support

## Not Implemented
- module expansion
- static if
- function interface
- lambda lowering
- interface/delegate/operator/stub/super

## input.j Scan Baseline
粘贴 build/input.phase2.stats.json

## Known Limitations
列出仍未支持但真实 input.j 中出现的语法。

## Commands Run
粘贴 cmake/build/ctest/input scan 命令。
```

更新 README：

```text
- Phase 2 当前支持 struct/method 基础 lowering
- 仍不是完整 JassHelper 替代
- 完整 input.j 仍不能保证 codegen，除非剩余 unsupported 后续阶段完成
```

---

## 17. 验收清单

Codex 完成阶段 2 后，必须满足：

```text
[ ] CMake configure 成功
[ ] CMake build 成功
[ ] ctest --test-dir build --output-on-failure 成功
[ ] 所有阶段 1 fixtures 仍通过
[ ] 新增 phase2 fixtures 通过
[ ] README 已更新
[ ] docs/phase2_status.md 已新增
[ ] samples/input.j --scan-only 成功
[ ] input.phase2.stats.json 中 structsUnsupported 明显下降，目标为 0
[ ] input.phase2.stats.json 中 methodsUnsupported 明显下降，目标为 0
[ ] 对支持的 struct fixture，输出中不残留 struct/endstruct/method/endmethod/thistype
[ ] 对支持的 Zinc struct fixture，输出中不残留 `{}`/`;`/`->` 语法
[ ] 生成 JASS 的 globals 只有一个 globals 块
[ ] 生成 JASS 的方法都变成普通 function
[ ] onInit 被收集到 vjassc__init_structs
[ ] main 中按顺序注入 vjassc__init_structs 与 vjassc__init_libraries
[ ] 仍未支持的 module/static if/function interface/lambda 有清晰 diagnostics
```

---

## 18. 推荐实施顺序

不要一次写完所有东西。按以下顺序推进：

### Step 1：准备与测试框架增强

```text
1. 运行阶段 1 build/ctest/input scan。
2. 新建 docs/phase2_status.md 空模板。
3. 增强 golden_runner 输出失败 fixture 名。
4. 新增一个最小 phase2 fixture，但先标记为预期失败或先不加入 CTest。
```

### Step 2：AST 数据结构

```text
1. 新增 DeclKind::Struct。
2. 新增 StructDecl/FieldDecl/MethodDecl/TypeRef/ParamDecl。
3. emit-ast 能显示 struct/method/field。
4. 先只 parse，不 codegen。
```

### Step 3：vJASS struct parser

```text
1. 支持 struct/endstruct。
2. 支持 field。
3. 支持 method/endmethod。
4. 支持 static method。
5. 支持 thistype 出现在类型位置。
6. 修改 stats：struct/method 不再 unsupported。
7. 添加 vJASS parse fixtures。
```

### Step 4：Zinc struct parser

```text
1. 支持 struct Name { }。
2. 支持 struct Name [] { }。
3. 支持 fields;。
4. 支持 method(args) { }。
5. 支持 static method(args) -> type { }。
6. 支持一行 method body。
7. brace scanner 忽略字符串/rawcode。
8. 添加 Zinc parse fixtures。
```

### Step 5：StructTable / symbol resolution

```text
1. 注册 struct 类型。
2. 注册 fields/methods。
3. 生成 deterministic generatedName。
4. 检查重复成员。
5. 处理 container public/private。
```

### Step 6：生成 struct globals

```text
1. si__ constants。
2. allocator F/I/V。
3. static fields。
4. instance field arrays。
5. fixed array field globals。
6. array struct 不生成 allocator。
```

### Step 7：生成 allocate/create/destroy/onDestroy/onInit

```text
1. 普通 struct allocator。
2. 默认 create。
3. custom static create。
4. default destroy。
5. onDestroy wrapper。
6. onInit 收集。
```

### Step 8：method body lowering

```text
1. method signature -> function signature。
2. this/thistype type rewrite。
3. field access rewrite。
4. static field access rewrite。
5. method call rewrite。
6. create/allocate/destroy call rewrite。
7. Zinc local declaration lifting。
```

### Step 9：真实 input.j 扫描与修补

```text
1. 跑完整 input scan。
2. 查看 stats 中仍被判 unsupported 的 struct/method。
3. 逐类修补 parser，不要扩大阶段边界。
4. 不处理 module/static if/lambda lowering，只保证它们不破坏 struct parser。
```

### Step 10：文档与最终验收

```text
1. 更新 README。
2. 填写 docs/phase2_status.md。
3. 保存 input.phase2.stats.json 摘要。
4. 确保 ctest 全过。
```

---

## 19. Codex 注意事项

```text
- 不要重写整个项目。
- 不要删除阶段 1 功能。
- 不要把 module/static if/function interface/lambda 偷偷当成已实现。
- 不要用全局 regex 直接替换所有 `this.` 或 `Struct.`。
- 不要在有 unsupported 的真实 input.j 上声称已经能生成完整 war3map.j。
- 每完成一个小目标就跑 ctest。
- 每次修改 parser 后都跑 samples/input.j --scan-only。
- 所有新增语法必须有 fixture。
- 若必须暂时跳过某个真实语法，写入 docs/phase2_status.md 的 Known Limitations。
```

阶段 2 的交付目标不是“完全替代 JassHelper”，而是把 JassHelper 最核心的对象系统入口——`struct/method`——落到稳定 AST 和可测试 lowering 上。只要这一层打牢，阶段 3 再做 `static if/module`，阶段 4 再做 `function interface/lambda`，成功率会高很多。
