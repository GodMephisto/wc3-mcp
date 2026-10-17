---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase5_codex_task_plan.md"
ref: HEAD
---

# JassChanger 阶段 5 Codex 实施计划

> 目标：从“`input.j` scan-only 无 unsupported”推进到“真实 `samples/input.j` 能完整生成一个 `.j` 输出文件”。
>
> 阶段 5 不再优先扩展 interface / delegate / operator / stub / super，而是集中解决 **真实项目中的 837 个 Zinc anonymous function / lambda 的全量 lowering、function-object 后端稳定化、full codegen 解锁**。

---

## 0. 当前项目状态

当前仓库已经完成：

- 阶段 1：项目骨架、预处理、lexer、顶层 parser、library 排序、基础 codegen、golden fixtures。
- 阶段 2：vJASS/Zinc `struct` / `method` parser 与基础 lowering。
- 阶段 3：`static if` 剪枝与 `module` / `implement` AST 级展开。
- 阶段 4：`function interface` parser/lowering、function-object `.execute/.evaluate/.name`、静态方法 function reference、非捕获 Zinc anonymous function 的小样例 lowering。

当前 `docs/phase4_status.md` 的真实输入基线为：

```json
{
  "functionInterfacesUnsupported": 0,
  "lambdas": 837,
  "diagnostics": {
    "errors": 0,
    "warnings": 0,
    "unsupported": 0
  }
}
```

但是 `samples/input.j` full codegen 仍被主动拒绝：

```text
unsupported: large lambda-heavy full codegen is not supported in phase 4
```

因此阶段 5 的核心不是“继续把 unsupported 数字清零”，而是：

```bash
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json
```

能够真正写出完整输出文件。

---

## 1. 阶段 5 总目标

### 1.1 必须达成

阶段 5 完成时，以下命令必须成功写出文件：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase5.scan.stats.json --emit-ast build/input.phase5.ast.txt --emit-expanded-ast build/input.phase5.expanded.ast.txt
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json
```

`build/input.phase5.out.j` 必须存在，且不能残留下列源语法：

```text
//! zinc
//! endzinc
struct
endstruct
method
endmethod
module
endmodule
implement
static if
function interface
function(...) { ... }
function (...) { ... }
```

### 1.2 允许暂不达成

阶段 5 不要求：

- 和 `output_jasshelper.j` byte-for-byte 一致；
- 必然通过 pjass；
- 必然能进 Warcraft III 运行；
- 支持完整 interface dispatch；
- 支持 delegate；
- 支持 operator overload；
- 支持 stub / super；
- 支持完整 closure capture 语义。

但是阶段 5 必须为后续 pjass 校验创造条件，即至少生成完整纯 JASS 候选文件。

---

## 2. 阶段 5 非目标

不要在阶段 5 混入以下大功能：

```text
interface dispatch
struct extends interface 多态表
delegate
operator [] / []= / virtual property
stub / super
byte-for-byte output_jasshelper matching
完整优化器
混淆器
地图 MPQ 打包
```

这些放到阶段 6/7。

阶段 5 的唯一主线是：

```text
真实 input.j 的 lambda-heavy full codegen 解锁
```

---

## 3. 当前代码需要重点关注的文件

阶段 5 主要会改动：

```text
src/parser/Ast.h
src/parser/Parser.cpp
src/main.cpp
src/codegen/Phase1Codegen.h
src/codegen/Phase1Codegen.cpp
src/sema/SymbolTable.*
src/util/PathUtil.*
CMakeLists.txt
tests/fixtures/*
docs/phase5_status.md
README.md
```

建议新增模块，避免继续把所有逻辑塞进 `Phase1Codegen.cpp`：

```text
src/lambda/LambdaAst.h
src/lambda/LambdaScanner.h
src/lambda/LambdaScanner.cpp
src/lambda/LambdaLowerer.h
src/lambda/LambdaLowerer.cpp
src/sema/FunctionSignatureTable.h
src/sema/FunctionSignatureTable.cpp
src/sema/NativeSignatureTable.h
src/sema/NativeSignatureTable.cpp
```

如果短期不想大重构，至少把 lambda 扫描、签名推断、capture 检测拆成独立 helper 文件。

---

## 4. 任务 A：建立真实 lambda inventory

### 4.1 目标

阶段 4 的 stats 只有：

```json
"lambdas": 837
```

阶段 5 必须把它拆细，知道 837 个 lambda 分布在哪些上下文中。

### 4.2 新增统计字段

在 `ParserStats` 或 codegen stats 中新增：

```cpp
size_t lambdas = 0;
size_t lambdasLowered = 0;
size_t lambdasCodeContext = 0;
size_t lambdasBoolexprContext = 0;
size_t lambdasFunctionInterfaceContext = 0;
size_t lambdasNativeCallbackContext = 0;
size_t lambdasMethodCallbackContext = 0;
size_t lambdasUnknownContext = 0;
size_t lambdasCapturing = 0;
size_t lambdasRejected = 0;
size_t lambdasGeneratedFunctions = 0;
```

### 4.3 新增可选报告

新增 CLI：

```bash
--emit-lambda-report build/input.lambda-report.json
```

报告格式示例：

```json
{
  "total": 837,
  "lowered": 837,
  "capturing": 0,
  "unknownContext": 0,
  "items": [
    {
      "id": 1,
      "source": "samples/input.j:376",
      "context": "function-interface-argument",
      "expectedType": "onLifeEnd",
      "generatedName": "vjlambda__0001",
      "captures": []
    }
  ]
}
```

如果不想新增 CLI，至少在 `--emit-stats` 中输出分类统计。

---

## 5. 任务 B：做统一 LambdaScanner

### 5.1 目标

当前阶段 4 已能处理小 fixture 的非捕获 lambda，但真实输入里 lambda 形态很多，需要一个统一 scanner/extractor。

必须识别以下形态：

```zinc
function() {
}

function () {
}

function(integer frame) {
}

function(player p) {
}

function() -> boolean {
    return true;
}

function () -> boolean{
    return true;
}

Condition(function () {
    return true;
})

Filter(function () -> boolean {
    return true;
})

TimerStart(t, 0.03, true, function () {
})

TriggerAddAction(tr, function () {
})

uiBtn.onEnter(function() {
})

uiBtn.spClick(function(integer frame) {
})
```

### 5.2 解析要求

LambdaScanner 必须能处理：

- 多行 lambda；
- 单行 lambda；
- 嵌套 `{}`；
- 字符串里的 `{` / `}`；
- rawcode，如 `'hfoo'`；
- 行注释里的 `{` / `}`；
- lambda 体内 `if/else/while/for`；
- lambda 体内又调用其他函数；
- fluent chain 中的 lambda 参数；
- 一行内多个 lambda。

### 5.3 数据结构建议

新增：

```cpp
struct LambdaDecl {
    size_t id = 0;
    SourceLocation loc;
    SyntaxMode mode = SyntaxMode::Zinc;
    std::vector<ParamDecl> params;
    TypeRef returnType;
    std::vector<LogicalLine> bodyLines;
    std::string generatedName;
    std::string expectedType;
    std::string contextKind;
    std::vector<std::string> captures;
};
```

如果当前 codegen 中已有 `LambdaInfo`，可以扩展它，不必重复定义。

---

## 6. 任务 C：Expected Type 推断

### 6.1 目标

同一个 lambda 在 JASS 输出中的替换方式取决于预期类型：

```text
code callback            -> function vjlambda__N
boolexpr / Condition     -> function vjlambda__N, 外层保持 Condition(...)
function interface value -> integer target id
ordinary direct call     -> generated function name 或 direct fallback
```

阶段 5 必须让 codegen 能判断 lambda 出现位置的 expected type。

### 6.2 建立 FunctionSignatureTable

从以下位置收集函数/方法签名：

```text
普通 function
library/scope 内 function，含 public/private 改名
struct static method
struct instance method
function interface declaration
Zinc type X extends function(...) -> Y
native declaration
```

建议结构：

```cpp
struct CallableSignature {
    std::string finalName;
    std::string sourceName;
    std::vector<std::string> paramTypes;
    std::string returnType;
    bool isNative = false;
    bool isMethod = false;
    bool isStaticMethod = false;
};
```

### 6.3 Native callback 签名

至少硬编码/推断以下常用 API：

```text
TimerStart(timer, real, boolean, code)
TriggerAddAction(trigger, code)
TriggerAddCondition(trigger, boolexpr)
Condition(code) -> boolexpr
Filter(code) -> boolexpr
ForForce(force, code)
ForGroup(group, code)
EnumDestructablesInRect(rect, boolexpr, code)
GroupEnumUnitsInRange(..., boolexpr)
GroupEnumUnitsInRangeEx(..., boolexpr)
DzFrameSetUpdateCallbackByCode(code)
DzTriggerRegisterMouseWheelEventByCode(..., code)
DzTriggerRegisterWindowResizeEventByCode(..., code)
DzTriggerRegisterMouseMoveEventByCode(..., code)
```

如果 native 已在 input.j 里声明，则优先从 native 签名解析；硬编码表只作为补充。

### 6.4 Function interface 参数推断

例如：

```zinc
public type escStackFunc extends function(player);
method push(escStackFunc f) -> integer;
escStack.push(function(player p) { ... });
```

需要根据 `push` 的参数类型知道 lambda 的 expected type 是 `escStackFunc`，然后替换为 interface target id。

### 6.5 Fluent chain 推断

对如下链式调用要能识别当前调用：

```zinc
icon[i].getClickBtn()
    .spEnter(function(integer frame) { ... })
    .spLeave(function(integer frame) { ... })
    .spClick(function(integer frame) { ... });
```

阶段 5 不要求完整表达式 AST，但至少要能在字符串/轻量 AST 层识别：

```text
.methodName(lambda)
```

并从方法签名表中找到 `methodName` 对应参数类型。

如果同名方法有多个候选：

- 优先当前 receiver struct 类型；
- 若无法推断 receiver 类型，但只有一个全局候选签名，使用它；
- 若多个候选且参数类型不同，报错，不要猜。

---

## 7. 任务 D：Lambda 函数生成

### 7.1 命名规则

建议使用稳定命名：

```text
vjlambda__0001
vjlambda__0002
...
```

或者按 container 加前缀：

```text
vjlambda__LibraryName__0001
vjlambda__StructName__0001
```

必须保证：

- 同一输入多次编译生成名称一致；
- 不依赖 unordered_map 遍历顺序；
- 不与用户函数冲突；
- 在 stats / lambda-report 里可追溯 source location。

### 7.2 生成位置

所有 lambda 生成函数建议输出在：

```text
native/type 后
struct support functions 前或后均可
普通用户函数前
```

关键是：被其他函数引用前必须已经定义。

### 7.3 参数与返回值

输入：

```zinc
function(integer frame) {
    BJDebugMsg(I2S(frame));
}
```

输出：

```jass
function vjlambda__0001 takes integer frame returns nothing
    call BJDebugMsg(I2S(frame))
endfunction
```

输入：

```zinc
function() -> boolean {
    return true;
}
```

输出：

```jass
function vjlambda__0002 takes nothing returns boolean
    return true
endfunction
```

### 7.4 局部变量规则

Zinc lambda 体内：

```zinc
function() {
    integer i; integer hid; unit u;
    i = 0;
}
```

必须转为：

```jass
function vjlambda__N takes nothing returns nothing
    local integer i
    local integer hid
    local unit u
    set i=0
endfunction
```

要求：

- 支持同一行多个 local 声明；
- local 必须移动到函数开头；
- 初始化表达式生成 `set`；
- 保持已有 Zinc body lowerer 行为。

---

## 8. 任务 E：Lambda 表达式替换规则

### 8.1 code 上下文

输入：

```zinc
TimerStart(t, 0.1, false, function() {
    BJDebugMsg("tick");
});
```

输出：

```jass
call TimerStart(t, 0.1, false, function vjlambda__0001)
```

### 8.2 Condition / Filter 上下文

输入：

```zinc
TriggerAddCondition(tr, Condition(function() -> boolean {
    return true;
}));
```

输出：

```jass
call TriggerAddCondition(tr, Condition(function vjlambda__0002))
```

### 8.3 function interface 上下文

输入：

```zinc
Button.onClick(function(integer frame) {
    BJDebugMsg(I2S(frame));
});
```

如果 `onClick` 参数类型是 `ClickCallback`，输出应类似：

```jass
call s__Button_onClick(Button, 17)
```

其中 `17` 是 `vjlambda__0003` 在 `ClickCallback` interface 中的 target id。

### 8.4 function interface 赋值

输入：

```zinc
ClickCallback cb = function(integer frame) {
    BJDebugMsg(I2S(frame));
};
```

输出：

```jass
local integer cb
set cb=17
```

### 8.5 `.name` 上下文

如果有：

```zinc
string s = callback.name;
```

对已知 function/interface target 应输出最终函数名字符串。

如果无法静态知道 target，报错或保守输出空字符串都不合适。推荐报错并给出位置。

---

## 9. 任务 F：Capture 检测与处理策略

### 9.1 为什么必须做

真实 `input.j` 里存在大量 lambda。JASS 没有真正闭包，所以不能随便把 lambda 提成全局函数，否则外层局部变量会丢失。

阶段 5 必须区分：

```text
非捕获 lambda：可以直接提成 function
捕获 lambda：必须明确处理或拒绝
```

### 9.2 Free variable 检测

对每个 lambda 计算：

```text
lambda 参数
lambda 局部变量
当前已知全局变量
当前已知 static 字段
当前 struct 字段/方法
函数名/native 名/type 名
JASS 内置事件函数，如 GetExpiredTimer/GetEnumUnit/GetTriggerPlayer
```

凡是不属于以上集合、又来自外层局部变量或外层参数的名称，都算 capture。

### 9.3 对 `this` 的特别规则

很多 timer/trigger lambda 会在体内自己恢复 `this`：

```zinc
function() {
    thistype this;
    this = LoadInteger(HASH_TIMER, GetHandleId(GetExpiredTimer()), KEY);
    ...
}
```

这不是 capture，应视为 lambda 内部局部变量。

### 9.4 对 struct 字段的特别规则

Zinc/JassHelper 允许在 struct method 中直接写字段名，编译后应转成：

```text
field -> s__Struct_field[this]
```

如果 lambda 内部声明/恢复了 `this`，则字段访问可以被正常 lowering。

如果 lambda 内部没有 `this`，但访问实例字段，则视为捕获外部 `this`，阶段 5 应报错，除非实现了安全 capture 存储。

### 9.5 阶段 5 的 capture 策略

建议阶段 5 先采用：

```text
目标：真实 input.j 中 capture 数量必须降到 0
```

也就是说，优先完善“局部变量/字段/全局/静态字段”的识别，避免误报 capture。

如果真实 input.j 仍有 capture：

- 不要静默生成错误 JASS；
- 输出 source location、变量名、lambda id；
- 在 `docs/phase5_status.md` 记录 capture blocker；
- 如果 capture 数量很少，再决定是否实现专门 lowering。

不建议阶段 5 做通用 closure 系统，因为异步 timer/trigger 回调里的 closure 生命周期很复杂。

---

## 10. 任务 G：Function interface 后端完善

### 10.1 当前问题

阶段 4 已经有 function interface wrapper，但文档说明：

```text
ordinary function objects use direct calls rather than a trigger/prototype backend
```

阶段 5 至少要保证 function interface 变量不是 direct-call fallback，而是走 interface dispatch。

### 10.2 Interface runtime 模型

每个 function interface：

```jass
function interface F takes integer a returns boolean
```

建议生成：

```jass
globals
    trigger array fi__F_trig
    integer fi__F_arg1
    boolean fi__F_result
endglobals

function fi__F_target_1 takes nothing returns boolean
    set fi__F_result = Target(fi__F_arg1)
    return false
endfunction

function fi__F_evaluate takes integer f, integer a returns boolean
    set fi__F_arg1 = a
    call TriggerEvaluate(fi__F_trig[f])
    return fi__F_result
endfunction

function fi__F_execute takes integer f, integer a returns nothing
    set fi__F_arg1 = a
    call TriggerExecute(fi__F_trig[f])
endfunction
```

具体可以沿用阶段 4 现有实现，不要求命名完全一致。

### 10.3 嵌套 evaluate 风险

阶段 4 已知限制：

```text
nested .evaluate uses fixed _tmp1 through _tmp8
```

阶段 5 要做两件事：

1. stats 记录最大嵌套深度；
2. 超过容量时报明确错误，而不是生成错误代码。

可新增：

```json
"functionInterfaceMaxEvaluateDepth": 3,
"functionInterfaceEvaluateTempLimit": 8
```

---

## 11. 任务 H：解除 large lambda-heavy full codegen guard

### 11.1 当前行为

阶段 4 对真实 input full codegen 主动拒绝：

```text
unsupported: large lambda-heavy full codegen is not supported in phase 4
```

阶段 5 完成 lambda lowering 后，必须删除或改造这个 guard。

### 11.2 新 guard 规则

只有以下情况可以拒绝 full codegen：

```text
仍有 DeclKind::Unsupported
有捕获 lambda 且未实现 capture lowering
有 unknown expected type lambda
有 function interface signature mismatch
有无法解析的 function target
有 codegen diagnostic error
```

不能再因为 lambda 数量多而拒绝。

### 11.3 验收命令

```bash
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json
```

成功标准：

```text
exit code = 0
build/input.phase5.out.j exists
stats.diagnostics.errors = 0
stats.diagnostics.unsupported = 0
stats.lambdas = 837
stats.lambdasLowered = 837
stats.lambdasUnknownContext = 0
stats.lambdasRejected = 0
```

如果 capture 不为 0，必须记录每个 capture 的位置和变量名；此时阶段 5 不算完成。

---

## 12. 任务 I：输出纯 JASS sanity check

生成 `input.phase5.out.j` 后，添加一个轻量 sanity checker。

### 12.1 新 CLI 可选项

```bash
--check-output-syntax-lite
```

或者直接在 test runner 中做。

### 12.2 检查内容

输出文件不应包含：

```text
//! zinc
//! endzinc
library
endlibrary
scope
endscope
struct
endstruct
method
endmethod
module
endmodule
implement
static if
function interface
function(
function (
->
```

注意：`function SomeName takes ...` 是合法 JASS，不应误判。

### 12.3 其他检查

```text
必须存在 globals/endglobals
必须存在 function main 或至少保留原 main
所有 local 声明应在函数顶部连续区域内，不能出现在可执行语句之后
不能输出空函数名
不能输出 unresolved lambda marker
不能输出 unsupported diagnostic marker
```

---

## 13. 任务 J：测试用例

在 `tests/fixtures` 新增以下 golden fixtures。

### 13.1 lambda code callback

```zinc
//! zinc
library T {
    function Init() {
        timer t = CreateTimer();
        TimerStart(t, 1.0, false, function() {
            BJDebugMsg("tick");
        });
    }
}
//! endzinc
```

预期：生成 `function vjlambda__... takes nothing returns nothing`，TimerStart 参数替换为 `function vjlambda__...`。

### 13.2 lambda Condition / Filter

```zinc
TriggerAddCondition(tr, Condition(function() -> boolean {
    return true;
}));
```

预期：`Condition(function vjlambda__...)`。

### 13.3 lambda function interface argument

```zinc
type ClickFn extends function(integer) -> nothing;
function Use(ClickFn f) { f.execute(1); }
function Test() { Use(function(integer x) { BJDebugMsg(I2S(x)); }); }
```

预期：lambda 作为 interface target id。

### 13.4 method chain lambda

```zinc
btn.spEnter(function(integer frame) {
    BJDebugMsg(I2S(frame));
}).spClick(function(integer frame) {
    BJDebugMsg("click");
});
```

预期：两个 lambda 都能推断 expected interface type。

### 13.5 static method function reference

```zinc
TimerStart(t, 0.1, true, function StructName.StaticCallback);
```

预期：替换为最终 generated static method function name。

### 13.6 lambda inside struct method with recovered this

```zinc
struct A {
    integer value;
    timer t;
    method start() {
        SaveInteger(H, GetHandleId(this.t), 1, this);
        TimerStart(this.t, 0.1, true, function() {
            thistype this;
            this = LoadInteger(H, GetHandleId(GetExpiredTimer()), 1);
            this.value = this.value + 1;
        });
    }
}
```

预期：不是 capture，能 lowering 字段访问。

### 13.7 negative：捕获外层局部变量

```zinc
function Test() {
    integer x = 1;
    TimerStart(CreateTimer(), 0.1, false, function() {
        BJDebugMsg(I2S(x));
    });
}
```

预期：报错 `capturing lambda is not supported`，包含变量名 `x` 和 source location。

### 13.8 negative：unknown expected type

```zinc
UnknownAccept(function() { BJDebugMsg("x"); });
```

预期：报错 `cannot infer expected type for lambda`。

### 13.9 input fragment fixture

从真实 input.j 里抽取几段：

```text
TimerStart(... function(){...})
ForGroup(... function(){...})
TriggerAddCondition(... Condition(function(){...}))
.spEnter(function(integer frame){...})
TriggerAddAction(... function(){ thistype this; this = LoadInteger(...); ... })
```

构造成小 fixture，确保真实形态被覆盖。

---

## 14. 任务 K：真实 input full codegen baseline

阶段 5 必须新增：

```text
docs/phase5_status.md
```

内容至少包括：

```md
# Phase 5 Status

Date: YYYY-MM-DD

## Implemented
...

## input.j Scan Baseline
...

## input.j Full Codegen Baseline
command...
exit code...
output file size...
output line count...
lambda stats...
remaining limitations...
```

建议记录：

```json
{
  "lambdas": 837,
  "lambdasLowered": 837,
  "lambdasCapturing": 0,
  "lambdasUnknownContext": 0,
  "functionInterfaces": 19,
  "functionInterfaceTargets": "filled",
  "functionInterfaceCalls": "filled",
  "diagnostics": {
    "errors": 0,
    "warnings": 0,
    "unsupported": 0
  },
  "output": {
    "bytes": 0,
    "lines": 0,
    "functions": 0,
    "globalsBlocks": 1
  },
  "timingMs": {
    "total": 0
  }
}
```

---

## 15. 任务 L：性能目标

阶段 5 重点是 correctness，但不能让性能退化太多。

目标：

```text
scan-only input.j: <= 1.5 秒
full codegen input.j: <= 3 秒
```

如果超过，先记录，不要为了优化牺牲正确性。

注意：避免对 10 万行文本做大量全文件 regex replace。推荐：

```text
一次扫描定位 lambda
按 source span 重写
字符串 builder 顺序输出
identifier intern / map lookup
避免 O(lambda_count * file_size) 的替换
```

---

## 16. 任务 M：工程清理建议

阶段 5 完成 full codegen 后，建议开始清理命名债。

### 16.1 可选重命名

当前类名仍是：

```cpp
Phase1Codegen
```

建议阶段 5 或阶段 6 改成：

```cpp
JassCodegen
LoweringCodegen
CompilerCodegen
```

如果阶段 5 时间不够，可以只在 `docs/phase5_status.md` 中保留 known limitation。

### 16.2 拆分 codegen

建议逐步拆分：

```text
StructLowering.cpp
FunctionInterfaceLowering.cpp
LambdaLowering.cpp
ZincBodyLowering.cpp
JassCodegen.cpp
```

现在 `Phase1Codegen.h` 已经包含 struct/function/lambda/interface 多套逻辑，继续堆会影响阶段 6。

---

## 17. 阶段 5 验收标准

### 17.1 必须通过

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase5.scan.stats.json
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json
```

### 17.2 产物必须满足

```text
build/input.phase5.out.j 存在
没有 unsupported diagnostics
没有 large lambda-heavy rejection
没有残留 Zinc/vJass 高级语法
lambdasLowered == lambdas
lambdasUnknownContext == 0
lambdasRejected == 0
```

### 17.3 如果无法完成

如果 full codegen 仍失败，Codex 必须在 `docs/phase5_status.md` 中列出：

```text
失败命令
exit code
失败原因
剩余 lambda 数
capture lambda 位置列表
unknown expected type lambda 位置列表
下一步最小修复建议
```

不要用“阶段 5 完成”描述未能生成 `input.phase5.out.j` 的状态。

---

## 18. 阶段 5 完成后的下一阶段预告

阶段 5 完成后，项目应进入：

```text
阶段 6：pjass 校验 + interface/delegate/operator/stub/super 补洞 + main/config/init 行为修正
阶段 7：对比 output_jasshelper.j、兼容性修正、性能优化、发布包装
```

如果阶段 5 无法一次性解决所有 lambda，则阶段 6 不应展开新语法，而应继续完成 lambda/callback full codegen。

---

## 19. 给 Codex 的关键提醒

1. **不要用正则全文件替换 lambda。** 真实 input 有 837 个 lambda，必须按 span / AST / logical line 精确替换。
2. **不要静默忽略 capture。** 捕获外层局部变量时必须报错或实现安全 capture lowering。
3. **不要因为 fixture 通过就宣布完成。** 阶段 5 的硬验收是 `samples/input.j` full codegen 写出文件。
4. **不要继续只做 scan-only。** scan-only 已经成功，阶段 5 要推进到完整输出。
5. **不要把未知 expected type 当 code。** 错误的 expected type 会生成可编译但行为错误的 JASS。
6. **保持输出可诊断。** 每个 lambda 要能从 generated name 追溯到原始 source line。
7. **优先正确性，后做 byte-for-byte。** 阶段 5 不追求和 JassHelper 输出完全一致。
