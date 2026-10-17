---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase4_codex_task_plan.md"
ref: HEAD
---

# JassChanger 阶段 4 Codex 实施计划

> 项目：`Crainax/JassChanger`
> 阶段：Phase 4
> 主题：`function interface` / 函数对象 / Zinc anonymous function / callback lowering
> 目标：继续向 JassHelper 替代器推进，消除当前 `input.j` 中剩余的主要 unsupported：`function interface`，并建立后续 lambda/callback/prototype wrapper 的基础。

---

## 0. 当前项目状态

当前项目已经完成：

```text
Phase 1:
  - C++20/CMake 工程骨架
  - CLI
  - SourceManager / Diagnostics
  - Preprocessor
  - JASS/Zinc lexer
  - 顶层 parser
  - library dependency sort
  - globals/native/function 基础 codegen
  - golden fixture runner

Phase 2:
  - vJASS/Zinc struct parser
  - vJASS/Zinc method parser
  - struct field / static field / fixed array field
  - thistype
  - method/static method lowering
  - default allocate/create/destroy/deallocate
  - onDestroy/onInit
  - struct body rewrite

Phase 3:
  - static if symbol collect
  - static if pruning
  - LIBRARY_X / DEBUG_MODE / constant boolean
  - vJASS module / implement
  - Zinc module / optional module
  - ModuleTable
  - ModuleExpander
  - --emit-expanded-ast
```

当前 `docs/phase3_status.md` 的 `input.j` 基线大致为：

```json
{
  "modules": 9,
  "moduleUses": 42,
  "staticIfs": 93,
  "staticIfResolvedTrue": 89,
  "staticIfResolvedFalse": 4,
  "moduleExpansions": 41,
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 0,
  "staticIfUnsupported": 0,
  "functionInterfacesUnsupported": 10,
  "diagnostics": {
    "errors": 0,
    "warnings": 10,
    "unsupported": 10
  }
}
```

阶段 4 的核心目标是：

```text
functionInterfacesUnsupported == 0
```

并为 Zinc anonymous function / lambda / callback 建立可扩展的 lowering 框架。

---

## 1. 阶段 4 范围

### 1.1 本阶段必须完成

```text
1. 解析 function interface 声明。
2. 建立 FunctionInterfaceTable。
3. 将 function interface 类型降级为 integer。
4. 支持 interface 对象赋值：
   - local F f = F.Target
   - set f = F.Target
   - local F f = Target           // 在可推断上下文中
   - 函数参数中传入 F.Target
5. 支持 .execute / .evaluate：
   - call f.execute(args)
   - set x = f.evaluate(args)
   - return f.evaluate(args)
   - 表达式中嵌套 f.evaluate(...)
6. 支持普通函数对象：
   - call Foo.execute(args)
   - set x = Foo.evaluate(args)
   - Foo.name
7. 支持静态方法作为 function interface/code/function object：
   - local F f = Struct.staticMethod
   - function Struct.staticMethod
8. 支持最小 Zinc anonymous function：
   - function() { ... }
   - function(args) -> type { ... }
   - 可用于 code / function interface / callback 参数。
9. 生成 trigger/prototype wrapper。
10. 新增 golden fixtures 与 negative fixtures。
11. 更新 README、docs/phase4_status.md。
12. 完整 input.j scan-only 成功，并输出 phase4 stats。
```

### 1.2 本阶段不要做

不要在阶段 4 里混入以下功能，除非它们是 function interface/lambda 必需的最小支撑：

```text
- interface dispatch
- delegate
- operator [] / []=
- stub / super
- full expression AST
- full Zinc type checker
- byte-for-byte JassHelper output matching
- pjass integration
- map packaging
- full main/config template replacement
```

这些放到后续阶段。

---

## 2. 语义背景

### 2.1 function interface 的基本语义

vJASS 中：

```jass
function interface Action takes unit u returns nothing

function KillTarget takes unit u returns nothing
    call KillUnit(u)
endfunction

function Test takes unit u returns nothing
    local Action a = Action.KillTarget
    call a.execute(u)
endfunction
```

语义：

```text
Action 是一种函数接口类型。
Action 类型的变量可以保存任何满足签名：
    takes unit returns nothing
的函数指针。
无返回值接口使用 .execute(args) 调用。
有返回值接口使用 .evaluate(args) 调用。
```

有返回值例子：

```jass
function interface RealFunc takes real x returns real

function Square takes real x returns real
    return x * x
endfunction

function Test takes real x returns real
    local RealFunc f = RealFunc.Square
    return f.evaluate(x)
endfunction
```

### 2.2 普通函数对象

vJASS 也允许普通函数像对象一样被调用：

```jass
function Calc takes integer x returns integer
    return x + 1
endfunction

function Test takes nothing returns integer
    return Calc.evaluate(1)
endfunction
```

以及：

```jass
function Later takes effect fx, real t returns nothing
    call TriggerSleepAction(t)
    call DestroyEffect(fx)
endfunction

function Test takes effect fx returns nothing
    call Later.execute(fx, 3.0)
endfunction
```

阶段 4 需要支持这类形式，至少做到语义可运行。

### 2.3 `.name`

vJASS 支持函数对象 `.name`：

```jass
scope S
    public function Foo takes nothing returns nothing
    endfunction

    function Test takes nothing returns nothing
        call ExecuteFunc(Foo.name)
    endfunction
endscope
```

阶段 4 应该把 `Foo.name` 重写成最终生成函数名字符串：

```jass
call ExecuteFunc("S_Foo")
```

注意：对于 private/public/library/scope/struct static method，必须使用 codegen 后的真实名字。

---

## 3. 总体实现方案

阶段 4 建议新增这些模块：

```text
src/sema/FunctionSignature.h
src/sema/FunctionSignature.cpp

src/sema/FunctionTable.h
src/sema/FunctionTable.cpp

src/sema/FunctionInterfaceTable.h
src/sema/FunctionInterfaceTable.cpp

src/lowering/FunctionInterfaceLowerer.h
src/lowering/FunctionInterfaceLowerer.cpp

src/lowering/LambdaExtractor.h
src/lowering/LambdaExtractor.cpp

src/lowering/PrototypeBackend.h
src/lowering/PrototypeBackend.cpp
```

如果你不想新增 `lowering/` 目录，也可以暂时放在 `src/sema/` 或 `src/codegen/`，但建议阶段 4 开始拆出 lowering 层，避免 `Phase1Codegen.cpp` 继续膨胀。

推荐最终管线：

```text
Preprocessor
  ↓
StaticIfSymbolCollector / StaticIfPruner
  ↓
Lexer
  ↓
Parser
  ↓
ModuleExpander
  ↓
FunctionInterfaceTable build
  ↓
LambdaExtractor
  ↓
FunctionInterfaceLowerer / FunctionObjectLowerer
  ↓
Codegen
```

实际可以先这样：

```text
Parser 解析 function interface / lambda 占位
ModuleExpander 后统一 lowering
Codegen 接收 lowered Program
```

---

## 4. AST 扩展

### 4.1 新增 DeclKind

在 `src/parser/Ast.h`：

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
    FunctionInterface,
    LambdaFunction,        // 可选：也可以单独放 Program.generatedFunctions
    Unsupported,
};
```

### 4.2 FunctionInterfaceDecl

可以作为 `Decl` 的字段，也可以独立结构。

建议：

```cpp
struct FunctionInterfaceDecl {
    std::string name;
    SourceLocation loc;
    std::string access;
    SyntaxMode mode = SyntaxMode::JassLike;
    std::vector<ParamDecl> params;
    TypeRef returnType;
    std::string generatedPrefix;
};
```

如果继续复用 `Decl`：

```cpp
struct Decl {
    ...
    std::vector<ParamDecl> params;  // function interface 使用
    TypeRef returnType;             // function / method / function interface 可共享
};
```

但当前 `Decl` 的 function 是用 `lines` 保存 header/body，不建议一次性大改。阶段 4 可以新增：

```cpp
std::vector<ParamDecl> interfaceParams;
TypeRef interfaceReturnType;
```

### 4.3 LambdaDecl

用于 Zinc anonymous function。

```cpp
struct LambdaDecl {
    std::string generatedName;
    SourceLocation loc;
    SyntaxMode mode = SyntaxMode::Zinc;
    std::vector<ParamDecl> params;
    TypeRef returnType;
    std::vector<LogicalLine> bodyLines;

    // Phase 4 最小实现：
    bool capturesLocalVariables = false;
    std::vector<std::string> capturedNames;
};
```

阶段 4 可以先只支持非捕获 lambda，发现捕获局部变量时报 unsupported：

```text
unsupported: capturing lambda is not supported in phase 4
```

---

## 5. Parser 任务

### 5.1 vJASS function interface

支持：

```jass
function interface F takes nothing returns nothing
function interface F takes unit u returns integer
public function interface F takes real x returns real
private function interface F takes integer a, integer b returns nothing
```

注意：

```text
function interface 允许出现在：
- 顶层
- library 内
- scope 内

不要输出到最终 JASS。
它只是编译器类型声明。
```

Parser 任务：

```text
1. 在 preScanUnsupported 中不要再把 function interface 计入 unsupported。
2. 在 parseJassRange 中识别 function interface。
3. 解析 name / params / returnType / access。
4. 计入 stats.functionInterfaces。
5. 保留作用域 container，用于 private/public 名字解析。
```

### 5.2 Zinc function interface / function type

Zinc 中可能出现两类：

```jass
function interface F takes integer x returns boolean
```

以及现代写法：

```jass
type Predicate extends function(integer) -> boolean;
```

阶段 4 必须至少支持第一类，因为 JassHelper/vJASS 核心是 `function interface`。

第二类如果当前项目已经作为 TypeDecl 处理，可以先标记为 future；但如果 `input.j` 里出现，要做最小解析。

建议阶段 4 支持：

```jass
type Predicate extends function(integer, real) -> boolean;
```

把它降级为 FunctionInterfaceDecl 的等价物：

```text
Predicate:
  params: integer, real
  returns: boolean
```

### 5.3 Zinc anonymous function

支持形态：

```jass
function() {
}

function(integer a, integer b) -> boolean {
    return a < b;
}
```

使用上下文：

```jass
code c = function() {
    call BJDebugMsg("x")
};

call TriggerAddAction(t, function() {
    call BJDebugMsg("x")
});

local Predicate p = function(integer x) -> boolean {
    return x > 0;
};
```

Parser 任务：

```text
1. Zinc parser 遇到 function(...) { ... } 时，不要误认为普通 function declaration。
2. 需要能 capture brace block。
3. 生成 LambdaDecl 或把 lambda 替换成占位 token：
   __vjassc_lambda_0001
4. 将 lambda body 作为 generated function 保存。
5. 支持 no-return type 默认 nothing。
```

阶段 4 最小策略：

```text
- 不捕获局部变量。
- 支持使用全局变量。
- 支持使用参数。
- 支持调用函数。
- 支持 return。
- 不支持 let/static local/capturing local。
```

---

## 6. FunctionSignature / FunctionTable

### 6.1 FunctionSignature

新增：

```cpp
struct FunctionSignature {
    std::vector<TypeRef> params;
    TypeRef returnType;
};

bool sameSignature(const FunctionSignature& a, const FunctionSignature& b);
bool canAssignToInterface(const FunctionSignature& functionSig,
                          const FunctionSignature& interfaceSig);
```

比较规则阶段 4 先简单处理：

```text
- 参数数量必须一致。
- 参数类型字符串必须一致，经过 rewriteTypeName 后比较。
- return type 必须一致。
- thistype 在 struct method 内转换成 struct generated name 或 integer。
- struct/interface type 最终都按 integer 存，但语义上先按原名比较。
```

### 6.2 FunctionTable

收集：

```text
1. 普通 function。
2. library/scope 内 function，使用最终 rename 名。
3. struct static method。
4. module expansion 后的 method。
5. generated lambda function。
```

存储：

```cpp
struct FunctionInfo {
    std::string sourceName;
    std::string qualifiedName;       // 例如 Lib_Foo / Lib___PrivateFoo / s__Struct_method
    std::string visibleName;
    const Decl* container = nullptr;
    const Decl* ownerStruct = nullptr;
    bool isStaticMethod = false;
    bool isPrivate = false;
    FunctionSignature signature;
    SourceLocation loc;
};
```

需要支持查找：

```text
- Foo
- Lib_Foo
- InterfaceName.Foo
- Struct.staticMethod
- function Struct.staticMethod
- private/public scope 内直接 Foo
```

### 6.3 FunctionInterfaceTable

收集：

```cpp
struct FunctionInterfaceInfo {
    std::string name;
    std::string generatedName;
    const Decl* container = nullptr;
    FunctionSignature signature;
    SourceLocation loc;

    // codegen helper names
    std::string triggerArrayName;
    std::string argGlobalPrefix;
    std::string resultGlobalName;
    std::string countGlobalName;
};
```

查找：

```text
- local F f
- function parameter type F
- global F x
- F.Target
```

---

## 7. JASS 类型降级规则

所有 function interface 类型在最终 JASS 中都是 `integer`。

例子：

```jass
function interface Filter takes unit u returns boolean

globals
    Filter GlobalFilter
endglobals

function Test takes Filter f returns nothing
    local Filter x = f
endfunction
```

降级：

```jass
globals
    integer GlobalFilter
endglobals

function Test takes integer f returns nothing
    local integer x = f
endfunction
```

要改的地方：

```text
1. rewriteTypeName 增加 FunctionInterfaceTable 查询。
2. globals 输出。
3. local declaration rewriting。
4. function param rewriting。
5. function return type rewriting。
6. method param rewriting。
7. struct fields 如果是 function interface 类型，也降级 integer array。
```

---

## 8. PrototypeBackend 设计

### 8.1 为什么需要 trigger/prototype backend

JASS 没有真正的函数指针。function interface 需要把“函数值”降级成整数 ID，并通过 trigger wrapper 间接调用。

阶段 4 推荐生成语义模型：

```text
interface value = integer id
id -> trigger
execute/evaluate:
  1. 把参数写入全局临时变量。
  2. TriggerExecute(trigger[id])。
  3. 如果有返回值，从返回全局变量读取。
```

### 8.2 每个 function interface 生成的 globals

例如：

```jass
function interface RealFunc takes real x returns real
```

生成：

```jass
globals
    trigger array vjfi__RealFunc_trigger
    integer vjfi__RealFunc_count = 0

    real vjfi__RealFunc_arg0
    real vjfi__RealFunc_result
endglobals
```

对于多参数：

```jass
function interface Action takes unit u, integer i returns nothing
```

生成：

```jass
globals
    trigger array vjfi__Action_trigger
    integer vjfi__Action_count = 0

    unit vjfi__Action_arg0
    integer vjfi__Action_arg1
endglobals
```

### 8.3 每个目标函数生成 wrapper

输入：

```jass
function Double takes real x returns real
    return x * 2.0
endfunction
```

分配给：

```jass
local RealFunc f = RealFunc.Double
```

生成：

```jass
function vjfi__RealFunc__Double__wrapper takes nothing returns nothing
    set vjfi__RealFunc_result = Double(vjfi__RealFunc_arg0)
endfunction
```

无返回值：

```jass
function vjfi__Action__KillTarget__wrapper takes nothing returns nothing
    call KillTarget(vjfi__Action_arg0)
endfunction
```

### 8.4 每个目标函数生成 getter/register

```jass
globals
    integer vjfi__RealFunc__Double_id = 0
endglobals

function vjfi__RealFunc__Double takes nothing returns integer
    if vjfi__RealFunc__Double_id == 0 then
        set vjfi__RealFunc_count = vjfi__RealFunc_count + 1
        set vjfi__RealFunc__Double_id = vjfi__RealFunc_count
        set vjfi__RealFunc_trigger[vjfi__RealFunc__Double_id] = CreateTrigger()
        call TriggerAddAction(vjfi__RealFunc_trigger[vjfi__RealFunc__Double_id], function vjfi__RealFunc__Double__wrapper)
    endif
    return vjfi__RealFunc__Double_id
endfunction
```

赋值：

```jass
local RealFunc f = RealFunc.Double
```

降级：

```jass
local integer f = vjfi__RealFunc__Double()
```

这样避免必须统一集中初始化，也避免 main/config 注入复杂化。

### 8.5 execute/evaluate

```jass
call f.execute(u)
```

降级：

```jass
set vjfi__Action_arg0 = u
call TriggerExecute(vjfi__Action_trigger[f])
```

```jass
set y = f.evaluate(x)
```

降级：

```jass
set vjfi__RealFunc_arg0 = x
call TriggerExecute(vjfi__RealFunc_trigger[f])
set y = vjfi__RealFunc_result
```

---

## 9. `.evaluate(...)` 表达式抽取

这是阶段 4 的难点。

### 9.1 简单语句

输入：

```jass
set y = f.evaluate(x)
```

输出：

```jass
set vjfi__RealFunc_arg0 = x
call TriggerExecute(vjfi__RealFunc_trigger[f])
set y = vjfi__RealFunc_result
```

### 9.2 return

输入：

```jass
return f.evaluate(x)
```

输出：

```jass
set vjfi__RealFunc_arg0 = x
call TriggerExecute(vjfi__RealFunc_trigger[f])
return vjfi__RealFunc_result
```

### 9.3 嵌套表达式

输入：

```jass
return F.evaluate(F.evaluate(x) * F.evaluate(x))
```

输出模型：

```jass
local real vjfi_tmp_1
local real vjfi_tmp_2
local real vjfi_tmp_3

set vjfi__RealFunc_arg0 = x
call TriggerExecute(vjfi__RealFunc_trigger[F])
set vjfi_tmp_1 = vjfi__RealFunc_result

set vjfi__RealFunc_arg0 = x
call TriggerExecute(vjfi__RealFunc_trigger[F])
set vjfi_tmp_2 = vjfi__RealFunc_result

set vjfi__RealFunc_arg0 = vjfi_tmp_1 * vjfi_tmp_2
call TriggerExecute(vjfi__RealFunc_trigger[F])
set vjfi_tmp_3 = vjfi__RealFunc_result

return vjfi_tmp_3
```

阶段 4 需要实现一个小型表达式扫描器，而不是简单 regex。

建议接口：

```cpp
struct ExtractedCall {
    std::string replacementVar;
    std::vector<std::string> preludeLines;
};

std::string extractEvaluateCalls(
    std::string expression,
    const FunctionContext& ctx,
    std::vector<std::string>& localDecls,
    std::vector<std::string>& preludeLines);
```

要求：

```text
- 从内到外抽取 `.evaluate(...)`。
- 正确处理括号嵌套。
- 忽略字符串、rawcode、注释。
- 每个函数体内生成唯一 temp。
- temp 类型等于 interface return type。
```

阶段 4 不需要完整 Pratt parser，但这个抽取器必须比 regex 稳定。

---

## 10. Function object lowering

### 10.1 普通函数 `.execute/.evaluate`

输入：

```jass
call Work.execute(a, b)
set x = Calc.evaluate(i)
```

有两种可选策略：

#### 策略 A：直接调用

```jass
call Work(a, b)
set x = Calc(i)
```

优点：

```text
- 实现简单
- 性能好
```

缺点：

```text
- 不符合 vJASS 函数对象使用 trigger 的语义
- 不能规避 300000 bytecode 限制
- execute 新线程语义不准确
```

#### 策略 B：生成 internal prototype

把普通函数对象视为匿名 function interface。

```text
signature: takes a,b returns nothing/integer
prototype name: vjfo__<hash-of-signature>
```

推荐阶段 4 采用策略 B，但可以先让 `--compat jasshelper` 之外默认用 A。当前项目还没有 compat 模式，因此建议：

```text
默认策略：B
如果实现复杂，可以先用 A，但必须在 docs/phase4_status.md 标为 known limitation。
```

### 10.2 `.name`

输入：

```jass
call ExecuteFunc(Foo.name)
```

输出：

```jass
call ExecuteFunc("FinalGeneratedFooName")
```

注意：

```text
- library public/private 函数要用 rewrite 后名字。
- static method 要用 generated static method function name。
- lambda `.name` 如果出现，也返回 generated lambda name。
```

---

## 11. LambdaExtractor

### 11.1 非捕获 code lambda

输入：

```jass
//! zinc
function Test() {
    code c = function() {
        BJDebugMsg("hello");
    };
}
//! endzinc
```

输出：

```jass
function vjlambda__0001 takes nothing returns nothing
    call BJDebugMsg("hello")
endfunction

function Test takes nothing returns nothing
    local code c = function vjlambda__0001
endfunction
```

### 11.2 TriggerAddAction

输入：

```jass
call TriggerAddAction(t, function() {
    call BJDebugMsg("x")
})
```

输出：

```jass
function vjlambda__0002 takes nothing returns nothing
    call BJDebugMsg("x")
endfunction

call TriggerAddAction(t, function vjlambda__0002)
```

### 11.3 Condition / boolexpr

输入：

```jass
call TriggerAddCondition(t, Condition(function() -> boolean {
    return true;
}))
```

输出：

```jass
function vjlambda__0003 takes nothing returns boolean
    return true
endfunction

call TriggerAddCondition(t, Condition(function vjlambda__0003))
```

### 11.4 function interface lambda

输入：

```jass
local Predicate p = function(integer x) -> boolean {
    return x > 0;
}
```

输出：

```jass
function vjlambda__0004 takes integer x returns boolean
    return x > 0
endfunction

local integer p = vjfi__Predicate__vjlambda__0004()
```

### 11.5 捕获局部变量

输入：

```jass
integer base = 10;
Predicate p = function(integer x) -> boolean {
    return x > base;
};
```

阶段 4 建议先报 unsupported：

```text
capturing lambda is not supported in phase 4
```

但要保证：

```text
- 诊断清楚。
- stats.lambdaCapturingUnsupported++。
- 不要 silently 生成错误 JASS。
```

后续如需支持闭包，要单独开阶段做 closure context。

---

## 12. 函数引用与 callback 特殊处理

### 12.1 `function Foo`

已有基础支持时，要扩展：

```jass
function Foo
function Struct.staticMethod
function Library_PublicFunc
function interpreter path // 若未来有
```

阶段 4 必须支持：

```text
- 普通 function Foo。
- static method function Struct.Method。
- generated lambda function vjlambda__N。
```

### 12.2 TriggerAddAction

```jass
call TriggerAddAction(t, SomeFunction)
```

如果 `SomeFunction` 是 lambda / function value，按上下文转换为 `function generatedName`。

但标准 JASS 通常要求：

```jass
call TriggerAddAction(t, function SomeFunction)
```

阶段 4 可支持 Zinc 语法糖：

```zinc
TriggerAddAction(t, function() { ... });
```

### 12.3 TriggerAddCondition / Condition / Filter

支持：

```jass
Condition(function BoolFunc)
Filter(function BoolFunc)
Condition(function() -> boolean { return true; })
Filter(function() -> boolean { return true; })
```

如果 lambda 返回不是 boolean，应报错。

---

## 13. Codegen 改动点

### 13.1 Globals

在 `emitGlobals` 增加：

```text
- 每个 function interface 的 trigger array。
- 每个 interface 的 count。
- 每个 interface 的 arg globals。
- 每个 interface 的 result global。
- 每个 target function getter cache id。
```

### 13.2 Functions

输出顺序建议：

```text
1. 普通 type/native。
2. function interface wrapper functions。
3. lambda generated functions。
4. struct support functions。
5. original/lowered functions。
6. init helper/main。
```

要保证：

```text
- wrapper 调用的 target function 已经在 JASS 中可见。
```

JASS 要求函数先声明后使用。可以通过以下方式解决：

```text
方案 A:
  wrapper functions 放在 target functions 后面。
  getter functions 也放在 target functions 后面。
  原函数体里使用 getter 时，getter 必须已声明。
  这会产生顺序困难。

方案 B:
  使用 execute/evaluate wrapper 之前，先 forward wrapper? JASS 不支持 forward declaration。

方案 C:
  把所有 original functions 先输出，再输出 wrappers，再输出使用 getter 的 rewritten functions？
  不可行，因为 rewritten functions 本身就是 original functions。

方案 D 推荐:
  对函数体中出现 interface assignment 的地方，不调用 getter function，而直接使用预先初始化的 id。
  但 id 初始化需要 main helper。

方案 E 推荐给阶段 4:
  生成 getter function 时，必须出现在所有用户函数之前。
  getter function 内引用 wrapper code。
  wrapper function 也必须在 getter 前。
  wrapper function 内调用 target function，JASS 要求 target function 已声明，可能失败。

解决策略：
  1. 对 wrapper 调用 target function 时，如果 target function 可能在后面，使用 `ExecuteFunc` 不适合带参数和返回。
  2. 因此阶段 4 需要重新排序函数输出：
     - 先输出所有普通 functions / static methods / generated lambdas 的 target body。
     - 再输出 function interface wrappers/getters。
     - 再输出使用这些 wrappers 的 functions 会冲突。
```

这里是关键设计点。

### 13.3 推荐解决方案：分两层函数输出

把用户函数分为：

```text
raw target body function:
  FinalName__impl

public callable wrapper:
  FinalName
```

但这会大改所有函数调用，不建议阶段 4 全量做。

### 13.4 阶段 4 实用策略

考虑当前项目已使用 library sorting 和 codegen rewrite，阶段 4 建议：

```text
1. 先保持 wrapper/getter 输出在所有用户函数之前。
2. wrapper 内不要直接调用目标函数名，而是调用目标函数的 `.evaluate/.execute` direct fallback？
   不可行，递归。
3. 改为 interface assignment 不调用 getter，而是使用 compile-time numeric id。
4. 在 `vjassc__init_libraries` 或新增 `vjassc__init_function_interfaces` 中创建 triggers。
```

具体：

```jass
globals
    trigger array vjfi__RealFunc_trigger
    integer constant vjfi__RealFunc__Double_id = 1
endglobals

function vjfi__RealFunc__Double__wrapper takes nothing returns nothing
    set vjfi__RealFunc_result = Double(vjfi__RealFunc_arg0)
endfunction

function vjassc__init_function_interfaces takes nothing returns nothing
    set vjfi__RealFunc_trigger[1] = CreateTrigger()
    call TriggerAddAction(vjfi__RealFunc_trigger[1], function vjfi__RealFunc__Double__wrapper)
endfunction
```

赋值：

```jass
local integer f = 1
```

这样 wrapper 函数可以放在目标函数之后，init helper 也放后面并由 main 注入。

最终输出顺序：

```text
globals
native/type
user functions and struct methods
generated lambda functions
function interface wrappers
vjassc__init_function_interfaces
vjassc__init_libraries
main
```

缺点：

```text
- id 是编译期分配。
- 需要收集所有 interface target references。
```

优点：

```text
- 避免 getter 函数顺序问题。
- 简化 JASS 函数声明顺序。
- 适合阶段 4。
```

请采用这个策略。

### 13.5 init helper 注入

当前已有 init helper。阶段 4 新增：

```jass
function vjassc__init_function_interfaces takes nothing returns nothing
    ...
endfunction
```

并在总 init 中调用：

```jass
call vjassc__init_function_interfaces()
```

顺序建议：

```text
1. struct onInit
2. function interface trigger init
3. library initializers
```

如果现有 init helper 是：

```jass
function vjassc__init_libraries takes nothing returns nothing
    ...
endfunction
```

则可以：

```jass
function vjassc__init_runtime takes nothing returns nothing
    call vjassc__init_function_interfaces()
    call vjassc__init_libraries()
endfunction
```

阶段 4 不必重构名字，但要确保 main 注入会执行 function interface trigger 初始化。

---

## 14. Signature inference

### 14.1 显式 interface target

容易：

```jass
local F f = F.Target
set f = F.Target
call TakesF(F.Target)
```

### 14.2 裸函数值

```jass
call TakesF(Target)
```

需要知道 `TakesF` 的参数类型是 `F`。

实现：

```text
1. FunctionTable 记录 TakesF 参数类型。
2. 解析 function call 时，如果第 i 个实参是 identifier，且目标参数类型是 function interface，
   则把 identifier 当作 function target。
3. 检查 target signature 是否匹配 F。
4. 替换为 target id。
```

### 14.3 本地变量初始化

```jass
local F f = Target
```

这里左侧类型给出 F，右侧裸 Target 可以推断。

### 14.4 set 语句

```jass
set f = Target
```

需要知道局部变量 `f` 的类型是 F。当前项目已有 localTypes 机制用于 struct field rewriting，可扩展：

```text
- seedFunctionLocalTypes 增加 function interface 类型。
- rewriteLocalDeclLine 记录 local f -> F。
- rewrite assignment 时查 localTypes[f]。
```

---

## 15. 统计字段

在 ParserStats 或新增 CompileStats 里加入：

```cpp
size_t functionInterfaces = 0;
size_t functionInterfaceTargets = 0;
size_t functionInterfaceCalls = 0;
size_t functionObjectCalls = 0;
size_t lambdas = 0;
size_t lambdasLowered = 0;
size_t lambdasCapturingUnsupported = 0;
size_t prototypeWrappers = 0;
```

`emitStatsJson` 输出这些字段。

阶段 4 验收时希望：

```json
{
  "functionInterfacesUnsupported": 0,
  "functionInterfaces": 10,
  "lambdas": <observed>,
  "lambdasLowered": <observed>,
  "prototypeWrappers": <observed>
}
```

---

## 16. Diagnostics

必须有清晰错误：

```text
1. unknown function interface type
2. unknown function target
3. function target signature mismatch
4. execute used on non-nothing return interface
5. evaluate used on nothing return interface
6. wrong argument count for execute/evaluate
7. lambda assigned to incompatible type
8. capturing lambda unsupported
9. function interface target not visible due to private scope
10. ambiguous target function
```

不要 silently 生成错误 JASS。

---

## 17. Golden fixtures

新增以下 fixtures。

### 17.1 phase4_function_interface_execute

输入：

```jass
function interface Action takes unit u returns nothing

function KillTarget takes unit u returns nothing
    call KillUnit(u)
endfunction

function Test takes unit u returns nothing
    local Action a = Action.KillTarget
    call a.execute(u)
endfunction
```

检查：

```text
- Action 不输出为 JASS function。
- local Action 降级 integer。
- 生成 trigger array / arg global。
- 生成 wrapper。
- execute 降级为 arg set + TriggerExecute。
```

### 17.2 phase4_function_interface_evaluate

```jass
function interface RealFunc takes real x returns real

function Double takes real x returns real
    return x * 2.0
endfunction

function Test takes real x returns real
    local RealFunc f = RealFunc.Double
    return f.evaluate(x)
endfunction
```

### 17.3 phase4_nested_evaluate

```jass
function interface RealFunc takes real x returns real

function Double takes real x returns real
    return x * 2.0
endfunction

function Test takes real x returns real
    local RealFunc f = RealFunc.Double
    return f.evaluate(f.evaluate(x) + 1.0)
endfunction
```

### 17.4 phase4_interface_as_function_param

```jass
function interface F takes integer x returns integer

function Inc takes integer x returns integer
    return x + 1
endfunction

function Apply takes integer x, F f returns integer
    return f.evaluate(x)
endfunction

function Test takes nothing returns integer
    return Apply(1, F.Inc)
endfunction
```

### 17.5 phase4_bare_function_value

```jass
function interface F takes integer x returns integer

function Inc takes integer x returns integer
    return x + 1
endfunction

function Apply takes integer x, F f returns integer
    return f.evaluate(x)
endfunction

function Test takes nothing returns integer
    return Apply(1, Inc)
endfunction
```

### 17.6 phase4_static_method_interface

```jass
function interface Printer takes integer x returns nothing

struct S
    static method Print takes integer x returns nothing
        call BJDebugMsg(I2S(x))
    endmethod

    static method Test takes nothing returns nothing
        local Printer p = S.Print
        call p.execute(5)
    endmethod
endstruct
```

### 17.7 phase4_function_object_evaluate

```jass
function Inc takes integer x returns integer
    return x + 1
endfunction

function Test takes nothing returns integer
    return Inc.evaluate(1)
endfunction
```

### 17.8 phase4_function_name

```jass
library L
    public function Foo takes nothing returns nothing
    endfunction

    function Test takes nothing returns nothing
        call ExecuteFunc(Foo.name)
    endfunction
endlibrary
```

Expected:

```jass
call ExecuteFunc("L_Foo")
```

### 17.9 phase4_zinc_lambda_code

```jass
//! zinc
library L {
    function Test() {
        code c = function() {
            BJDebugMsg("x");
        };
    }
}
//! endzinc
```

### 17.10 phase4_zinc_lambda_trigger_action

```jass
//! zinc
library L {
    function Test(trigger t) {
        TriggerAddAction(t, function() {
            BJDebugMsg("x");
        });
    }
}
//! endzinc
```

### 17.11 phase4_zinc_lambda_interface

```jass
//! zinc
library L {
    function interface Pred takes integer x returns boolean

    function Test() {
        Pred p = function(integer x) -> boolean {
            return x > 0;
        };
    }
}
//! endzinc
```

### 17.12 phase4_negative_signature_mismatch

```jass
function interface F takes integer x returns integer

function Bad takes real x returns integer
    return 0
endfunction

function Test takes nothing returns nothing
    local F f = F.Bad
endfunction
```

Expected diagnostic.

### 17.13 phase4_negative_evaluate_on_nothing

```jass
function interface F takes nothing returns nothing

function A takes nothing returns nothing
endfunction

function Test takes nothing returns integer
    local F f = F.A
    return f.evaluate()
endfunction
```

Expected diagnostic.

### 17.14 phase4_negative_capturing_lambda

```jass
//! zinc
library L {
    function Test() {
        integer base = 1;
        code c = function() {
            BJDebugMsg(I2S(base));
        };
    }
}
//! endzinc
```

Expected unsupported or error.

---

## 18. input.j 验收

阶段 4 完成后执行：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure

build/vjassc samples/input.j \
  --scan-only \
  --allow-unsupported \
  --emit-stats build/input.phase4.stats.json \
  --emit-ast build/input.phase4.ast.txt \
  --emit-expanded-ast build/input.phase4.expanded.ast.txt
```

必须满足：

```text
errors == 0
functionInterfacesUnsupported == 0
structsUnsupported == 0
methodsUnsupported == 0
modulesUnsupported == 0
staticIfUnsupported == 0
```

如果 lambda 仍有不支持形态，需要输出独立字段：

```json
{
  "lambdasCapturingUnsupported": N
}
```

并且在 docs 中解释。

---

## 19. full codegen 验收

阶段 4 不强制完整 `input.j` full codegen 成功。

但需要做两个检查：

### 19.1 小型 full codegen 成功

所有 phase4 fixtures 必须可 `-o` 生成 JASS。

### 19.2 input.j full codegen 行为

执行：

```bash
build/vjassc samples/input.j -o build/input.phase4.out.j --emit-stats build/input.phase4.codegen.stats.json
```

允许两种结果：

```text
A. 如果 remaining unsupported 仍存在：
   - 安全拒绝 codegen
   - exit code 6
   - 明确列出剩余 unsupported 类型

B. 如果没有 unsupported：
   - 生成 output
   - 后续阶段再做 pjass/war3 runtime 校验
```

禁止：

```text
- 静默生成明显残留 function interface / lambda 的 JASS。
- 生成不能解释原因的坏 output。
```

---

## 20. 性能目标

当前 phase3 total 约 799ms。阶段 4 新增 function interface/lambda lowering 后：

```text
input.j scan-only total <= 1300ms
小型 full codegen <= 100ms
```

不要在大文件上做 O(N^2) 字符串扫描。

建议：

```text
- FunctionTable 使用 unordered_map。
- FunctionInterfaceTable 使用 unordered_map。
- LambdaExtractor 单 pass。
- Expression extraction 限定在包含 ".evaluate(" / ".execute(" / "function(" 的行。
```

---

## 21. 文档更新

新增：

```text
docs/phase4_status.md
```

内容包含：

```text
1. Implemented
2. Not Implemented
3. input.j Scan Baseline
4. function interface stats
5. lambda stats
6. Known Limitations
7. Commands Run
```

README 更新：

```text
Phase 4 adds function interface, function object, function reference, callback, and non-capturing Zinc anonymous function lowering.
```

Phase Boundary 更新：

```text
Still not complete JassHelper replacement:
- interface dispatch
- delegate
- operator overload
- stub/super
- capturing closure
- byte-for-byte output matching
- pjass validation
```

---

## 22. Codex 注意事项

```text
1. 不要把 function interface 当成 native/type 原样输出。
2. 不要用简单 regex 处理嵌套 evaluate 表达式。
3. 不要在字符串、rawcode、注释里替换 .evaluate/.execute/.name。
4. 不要忽略 private/public/scope/library 名字改写。
5. 不要让 interface target wrapper 早于目标函数调用而导致 JASS 顺序错误。
6. 推荐使用 compile-time id + init helper 注册 trigger 的方案。
7. 不要支持捕获 lambda 时假装成功；捕获局部变量必须诊断。
8. 每个新增能力都要有 golden fixture。
9. 每个不支持形态都要有 negative fixture。
10. 完成后必须更新 docs/phase4_status.md。
```

---

## 23. 阶段 4 完成标准

阶段 4 完成时，仓库应满足：

```text
- README 显示 phase-4。
- docs/phase4_status.md 存在。
- CMake 包含新增 FunctionInterface/Lambda/Prototype 模块。
- ctest 全部通过。
- functionInterfacesUnsupported == 0。
- input.j scan-only 成功。
- phase4 fixtures full codegen 成功。
- function interface 类型正确降级 integer。
- execute/evaluate 可运行。
- static method 可赋给 function interface。
- non-capturing Zinc lambda 可降级成 generated function。
- 捕获 lambda 有明确 unsupported/error。
```

---

## 24. 建议提交信息

```bash
git add .
git commit -m "phase 4 function interface and lambda lowering"
git push
```
