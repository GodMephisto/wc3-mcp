---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase8_codex_task_plan.md"
ref: HEAD
---

# JassChanger 阶段 8 Codex 实施计划

> 目标项目：`Crainax/JassChanger`
> 当前阶段：Phase 7 已完成
> 阶段 8 主题：**声明顺序合法化 + indexed struct member 二次 lowering + Zinc 单行控制流修复 + local 声明拆分**
> 阶段 8 核心目标：继续保持真实 `samples/input.j` 可完整 codegen、syntax-lite 通过，并显著降低 PJASS 错误数量。

---

## 0. 当前状态摘要

Phase 7 已经完成第一批 PJASS blocker 清理：

```text
已解决：
- duplicateDeclaration: 69 -> 0
- duplicateFunctionNames: 0
- duplicateGlobalNames: 0
- duplicateNativeNames: 0
- block comment 泄漏已处理
- 多维数组声明/访问的基础 flattening 已实现
- syntax-lite 无残留 source forms
- main/config/init validation 仍通过
```

但 PJASS 仍未通过，当前主要错误来自第二层 blocker：

```text
Phase 7 PJASS remaining summary:
- syntaxError: 2800
- other: 8358
- undefinedVariable: 763
- returnMismatch: 67
- undefinedFunction: 687
- expectedEndfunction: 161
- localOrder: 29
- invalidComparison: 38
```

Phase 7 文档列出的第一批 remaining blockers：

```text
1. function declaration ordering / forward references
2. lambda declaration ordering
3. indexed struct instance member access: arr[i].field / arr[i].method()
4. inline Zinc return/control forms: if (cond) return expr
5. comma-separated local declarations: local integer parent,child
6. unresolved symbols: aby/acy, HASH_ABILITY, HASH_TIMER, generated helper refs
```

阶段 8 不应继续做 interface/delegate/operator/stub/super 等新大语法。当前优先级是：**让生成的 JASS 更接近 PJASS 可接受的普通 JASS**。

---

## 1. 阶段 8 总目标

### 1.1 必须达成

阶段 8 完成后，以下命令必须继续成功生成输出文件：

```bat
cmake --build build
ctest --test-dir build --output-on-failure

build\vjassc.exe samples\input.j ^
  -o build\input.phase8.out.j ^
  --emit-stats build\input.phase8.codegen.stats.json ^
  --emit-validation-report build\input.phase8.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite
```

并且 validation report 必须满足：

```text
syntaxLite.ok == true
duplicateFunctionNames == 0
duplicateGlobalNames == 0
duplicateNativeNames == 0
globalsBlocks == 1
hasMain == true
hasConfig == true
init validation issues == []
```

PJASS 命令也必须可以运行并被捕获：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase8.pjass.out.j ^
  --emit-stats build\input.phase8.pjass.stats.json ^
  --emit-validation-report build\input.phase8.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

### 1.2 PJASS 错误目标

理想目标：PJASS 直接通过。

如果本阶段无法直接通过，最低目标是显著降低第二层 blocker：

```text
必须保持：
- duplicateDeclaration == 0
- duplicateFunctionNames == 0
- duplicateGlobalNames == 0
- duplicateNativeNames == 0

建议目标：
- localOrder: 29 -> 0 或 <= 3
- expectedEndfunction: 161 -> <= 50
- syntaxError: 2800 -> <= 1000
- undefinedVariable: 763 -> <= 300
- undefinedFunction: 687 -> <= 300
- other: 8358 -> 明显下降，至少下降 30%
```

不要为了降低错误数而生成 dummy function/global。所有新增声明必须来自合法 lowering 或环境符号表，不允许伪造语义。

---

## 2. 阶段 8 禁止事项

本阶段不要做这些事：

```text
禁止：
- 不要实现完整 interface dispatch。
- 不要实现 delegate。
- 不要实现 operator overload 大功能，除非只是为了修一个 PJASS blocker 的极小局部 lowering。
- 不要实现 stub/super。
- 不要追求 byte-for-byte JassHelper 输出。
- 不要做大规模性能重构替代 correctness 修复。
- 不要用 dummy globals/functions 掩盖 undefined symbols。
- 不要关闭 PJASS validation 或降低 validation 标准。
```

允许做的小范围重构：

```text
允许：
- 拆分 Phase1Codegen 中与本阶段相关的 helper 函数。
- 增加 FunctionEmissionPlanner / OutputSyntaxLiteChecker / ArrayAccessLowerer 等独立模块。
- 为 validation report 增加更多字段。
- 添加更细的 pass timing。
```

---

## 3. 开发前基线确认

Codex 开始前先执行：

```bat
cmake --build build
ctest --test-dir build --output-on-failure

build\vjassc.exe samples\input.j ^
  -o build\baseline.phase7.out.j ^
  --emit-stats build\baseline.phase7.codegen.stats.json ^
  --emit-validation-report build\baseline.phase7.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite

build\vjassc.exe samples\input.j ^
  -o build\baseline.phase7.pjass.out.j ^
  --emit-stats build\baseline.phase7.pjass.stats.json ^
  --emit-validation-report build\baseline.phase7.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

记录：

```text
- PJASS summary
- 前 30 个 PJASS grouped examples
- codegen timing
- syntaxLite result
- duplicate name stats
```

将其作为阶段 8 前后对比基线。

---

## 4. 任务 A：增强 PJASS triage 与 validation report

### 4.1 目标

让 PJASS 错误更快定位到 root cause。现在 `other` bucket 很大，阶段 8 要继续拆分。

### 4.2 需要新增或增强的分类

在 PJASS parser / validation report 中增加分类：

```text
forwardFunctionReference
forwardLambdaReference
indexedStructMemberResidue
inlineZincControlResidue
commaSeparatedLocal
invalidLocalDeclaration
unknownType
invalidReturnSyntax
invalidIfSyntax
invalidCallSyntax
unresolvedGeneratedHelper
unresolvedSourceSymbol
```

### 4.3 需要输出的信息

每个分类至少输出：

```json
{
  "count": 0,
  "examples": [
    {
      "line": 12345,
      "function": "currentFunctionName",
      "text": "the output line",
      "nearby": ["previous", "line", "next"],
      "pjassMessage": "..."
    }
  ]
}
```

### 4.4 验收

`build/input.phase8.pjass.validation.json` 中必须能看到：

```text
- PJASS error summary
- grouped examples
- generated-output excerpts
- current function name
- line number
```

---

## 5. 任务 B：函数、struct method、lambda 的声明顺序合法化

### 5.1 背景

PJASS 要求函数在使用前定义。Phase 7 中典型错误：

```text
sc__BaseAnim_baseanim_onDestroy
  calls s__BaseAnim_baseanim_isExist
  calls s__BaseAnim_baseanim_delDelay
but callees are emitted later.

vjlambda__1 is referenced before emitted.
```

### 5.2 新增 FunctionEmissionPlanner

建议新增模块：

```text
src/codegen/FunctionEmissionPlanner.h
src/codegen/FunctionEmissionPlanner.cpp
```

核心数据结构：

```cpp
struct FunctionUnit {
    std::string name;
    std::string kind; // native? user? lambda? structSupport? interfaceWrapper? init? main? config?
    SourceLocation loc;
    std::vector<std::string> bodyLines;
    std::vector<std::string> dependencies;
    bool isMain = false;
    bool isConfig = false;
    bool isInitHelper = false;
};
```

### 5.3 依赖提取规则

从函数体中提取这些依赖：

```text
call Foo(...)
set x = Foo(...)
return Foo(...)
if Foo(...) then
loop / exitwhen Foo(...)
function Foo
Condition(function Foo)
Filter(function Foo)
TriggerAddAction(..., function Foo)
```

注意：

```text
- 不要把 native 当作必须排序的普通函数。
- 不要解析字符串里的 ExecuteFunc("Foo") 为硬依赖，除非已有专门规则。
- 不要把变量名误判为函数名。
- 不要把 BJ/native/common 函数误判为本输出函数依赖。
```

### 5.4 排序规则

生成函数时使用拓扑排序：

```text
如果 A 调用 B，则 B 必须排在 A 之前。
```

特殊规则：

```text
1. globals 永远在所有 function 之前。
2. native/type 声明仍在 function 之前。
3. main/config 尽量放在靠后位置，尤其 main 应在它直接 call 的 init helper 后。
4. vjassc__init_structs / vjassc__init_function_interfaces / vjassc__init_libraries 应在 main 前。
5. lambda 如果被 function reference 使用，必须在使用者之前。
6. struct onDestroy/create/destroy 等 support functions 也参与统一排序。
```

循环依赖处理：

```text
- 如果发现 A <-> B 直接调用循环，不要 silently output。
- 在 validation report 中记录 cycle。
- 优先保持原顺序并输出 warning，后续再处理。
```

实际 vJASS 常见互递归应通过 `.evaluate` 或 function interface wrapper 解决，不建议本阶段临时发明 trampoline。

### 5.5 两阶段 codegen 建议

当前 codegen 很可能边 lowering 边直接写 `CodeWriter`。阶段 8 建议调整为：

```text
1. collect/lower 所有函数为 FunctionUnit。
2. collect generated lambdas/interface wrappers/struct support functions。
3. build dependency graph。
4. topological order。
5. 最后统一 emit。
```

如果全量重构太大，最小方案：

```text
- 先把 generated lambda functions 提前 emit。
- 再把 struct support functions 按内部依赖排序。
- 再把 normal/library functions 按依赖排序。
```

但最终还是建议进入统一 FunctionUnit 模型。

### 5.6 验收

新增 syntax-lite 检查：

```text
forwardFunctionReference issue count
forwardLambdaReference issue count
```

阶段 8 目标：

```text
forwardFunctionReference 显著下降或清零
undefinedFunction 因前向引用导致的数量显著下降
```

---

## 6. 任务 C：lambda emission 顺序修复

### 6.1 目标

解决 `vjlambda__N` 被引用早于定义的问题。

### 6.2 要求

```text
- lambda ID 保持稳定、确定性。
- lambda function body 继续保留 private/public rename、array flattening、struct lowering。
- lambda 可以参与 FunctionEmissionPlanner。
- lambda 的 generated function 不能残留 Zinc 语法。
```

### 6.3 实现建议

当前阶段 5/6/7 已经支持 lambda extraction。阶段 8 改为：

```text
1. 对所有函数/方法体先做 lambda extraction，生成 LambdaInfo。
2. 记录每个使用者函数引用了哪些 vjlambda__N。
3. LambdaInfo 转成 FunctionUnit。
4. 让 FunctionEmissionPlanner 排序。
```

如果 lambda body 调用当前函数，可能形成循环。遇到循环时先记录在 report 中，不要生成 dummy。

### 6.4 fixture

新增：

```text
tests/fixtures/phase8_lambda_before_use.in.j
tests/fixtures/phase8_lambda_before_use.expected.j
tests/fixtures/phase8_lambda_calls_helper.in.j
tests/fixtures/phase8_lambda_calls_helper.expected.j
```

---

## 7. 任务 D：indexed struct member 二次 lowering

### 7.1 背景

Phase 7 已经做了多维数组 flatten，但还有类似：

```jass
s__BaseAnim_baseanim_DList[...].dID
```

JASS 不支持 `array[index].field`。必须继续降级为 struct field array access。

### 7.2 需要支持的形态

实例字段读取：

```jass
arr[i].field
objArray[i].field
this.list[i].field
s__SomeStruct_list[this].field
```

实例字段写入：

```jass
set arr[i].field = value
set this.list[i].field = value
```

实例方法调用：

```jass
call arr[i].method(a, b)
set x = arr[i].method(a, b)
```

嵌套访问：

```jass
arr[i].node.next.value
arr[i].field[j].subfield
```

阶段 8 至少要支持一层 indexed struct member：

```text
arr[i].field
arr[i].method(...)
```

多层可以通过循环 pass 逐层处理，但要加 max iteration 防止死循环。

### 7.3 类型来源

需要知道 `arr` 的元素类型。来源包括：

```text
- global declaration: SomeStruct array arr
- local declaration: local SomeStruct array arr
- struct instance field: SomeStruct array field
- static field: static SomeStruct array field
- lowered generated arrays that still preserve metadata
```

建议新增：

```text
StructTypeEnvironment
  globalTypes
  localTypes
  staticFieldTypes
  instanceFieldTypes
```

### 7.4 lowering 规则

如果 `expr` 的类型为 `StructType`，则：

```text
expr.field -> s__StructType_field[expr]
expr.method(a, b) -> s__StructType_method(expr, a, b)
```

如果 field 本身是 struct 类型，则下一轮继续处理：

```text
expr.field.sub -> s__FieldStruct_sub[s__StructType_field[expr]]
```

如果 field 是 fixed array：

```text
expr.field[k] -> s__StructType_field[expr * FIELD_SIZE + k]
```

如果已存在阶段 7 的 fixed array flattening 规则，复用它，不要分叉出第二套计算。

### 7.5 必须保护

```text
- 不要改字符串。
- 不要改 rawcode，如 'A000'。
- 不要改注释。
- 不要改 real literal 的小数点。
- 不要把 function object .evaluate/.execute 误判为 struct method。
```

### 7.6 fixture

新增：

```text
tests/fixtures/phase8_indexed_struct_field.in.j
tests/fixtures/phase8_indexed_struct_field.expected.j
tests/fixtures/phase8_indexed_struct_method.in.j
tests/fixtures/phase8_indexed_struct_method.expected.j
tests/fixtures/phase8_nested_indexed_struct_field.in.j
tests/fixtures/phase8_nested_indexed_struct_field.expected.j
```

### 7.7 syntax-lite 新增检测

检测残留：

```regex
\[[^\]]+\]\s*\.\s*[A-Za-z_$]
```

但要跳过字符串、rawcode、注释。

---

## 8. 任务 E：Zinc inline control / return forms 正规化

### 8.1 背景

当前 PJASS blocker：

```zinc
if (width > 0) return DzGetMouseXRelative();
```

这必须转为普通 JASS：

```jass
if (width > 0) then
    return DzGetMouseXRelative()
endif
```

### 8.2 支持范围

阶段 8 至少支持：

```zinc
if (cond) return expr;
if (cond) return;
if (cond) foo();
if (cond) x = y;
if (cond) x += y;
if (cond) break;
```

在 JASS 输出中分别转成：

```jass
if (cond) then
    return expr
endif

if (cond) then
    return
endif

if (cond) then
    call foo()
endif

if (cond) then
    set x = y
endif

if (cond) then
    set x = x + y
endif

if (cond) then
    exitwhen true
endif
```

注意 `break` 是否在 loop 里。若当前 lowerer 没有 loop context，先只处理 PJASS 日志中实际出现的形式。

### 8.3 不要破坏已有 block if

不要把这些误处理：

```zinc
if (cond) {
    return x;
}

if (cond)
{
    return x;
}
```

### 8.4 else / else-if

如果发现以下形式，也应处理：

```zinc
if (cond) return a; else return b;
```

阶段 8 可先实现：

```jass
if (cond) then
    return a
else
    return b
endif
```

如果实现复杂，可先检测并在 validation report 中分类，不要生成非法 JASS。

### 8.5 fixture

新增：

```text
tests/fixtures/phase8_zinc_inline_if_return.in.j
tests/fixtures/phase8_zinc_inline_if_return.expected.j
tests/fixtures/phase8_zinc_inline_if_call.in.j
tests/fixtures/phase8_zinc_inline_if_call.expected.j
tests/fixtures/phase8_zinc_inline_if_assignment.in.j
tests/fixtures/phase8_zinc_inline_if_assignment.expected.j
```

### 8.6 syntax-lite 检测

检测残留：

```text
if (...) return
if (...) call
if (...) set
```

仅检查输出函数体内；跳过合法 `if (...) then`。

---

## 9. 任务 F：comma-separated local declaration 拆分

### 9.1 背景

PJASS 仍报告：

```jass
local integer parent,child
```

普通 JASS 不接受逗号 local 声明。

### 9.2 拆分规则

输入：

```jass
local integer parent,child
```

输出：

```jass
local integer parent
local integer child
```

输入：

```jass
local integer a = 1, b = 2, c
```

输出时需满足 JASS local 必须在函数顶部：

```jass
local integer a
local integer b
local integer c
...
set a = 1
set b = 2
```

如果该 local 原本已经在函数顶部，`set` 初始化仍应放在原语义位置。

### 9.3 类型支持

支持：

```text
integer
real
boolean
string
code
handle
unit/timer/trigger/...
struct lowered type -> integer
function interface type -> integer
array local: local integer array a, b
```

JASS local array 一般不能初始化，若遇到：

```jass
local integer array a, b
```

输出：

```jass
local integer array a
local integer array b
```

### 9.4 解析注意

逗号可能出现在：

```text
function call 参数
字符串
rawcode
array index
泛型/模板样式文本
```

只拆 `local <type> <decl-list>` 这一行的顶层逗号，不要全局 split。

### 9.5 fixture

新增：

```text
tests/fixtures/phase8_comma_local_simple.in.j
tests/fixtures/phase8_comma_local_simple.expected.j
tests/fixtures/phase8_comma_local_initializer.in.j
tests/fixtures/phase8_comma_local_initializer.expected.j
tests/fixtures/phase8_comma_local_array.in.j
tests/fixtures/phase8_comma_local_array.expected.j
```

---

## 10. 任务 G：return mismatch 与 invalid comparison 初步清理

### 10.1 return mismatch

在完成 inline control 和 declaration ordering 后，再观察 `returnMismatch` 是否下降。

如果仍存在：

```text
- function returns nothing but has return expr
- function returns value but has bare return
- lambda wrapper return type 与 body 不一致
- function interface evaluate wrapper return 类型错误
```

要求：

```text
1. 在 validation report 中列出前 20 个 returnMismatch 的函数名、returnType、return line。
2. 不要盲目删除 return expr。
3. 先修 generated wrapper/lambda 的明显错误。
```

### 10.2 invalid comparison

当前有 `invalidComparison: 38`。

可能来源：

```text
- struct/int 与 primitive/handle 错比
- boolean expression lowering 错误
- null comparison 类型不一致
- function interface id 与 function object 混用
```

阶段 8 只做分类和明显 lowering 修复，不做完整类型系统。

---

## 11. 任务 H：unresolved symbol 第二轮排查

Phase 7 top examples：

```text
aby / acy
HASH_ABILITY
HASH_TIMER
generated helper references
```

### 11.1 原则

不要加 dummy：

```jass
// 禁止：为了过 PJASS 直接加
integer HASH_ABILITY
integer HASH_TIMER
```

必须追踪来源：

```text
- 是否来自 private/public rename 缺失？
- 是否来自 static if 剪枝误删？
- 是否来自 library ordering / dependency？
- 是否来自 local comma declaration 未拆？
- 是否来自 multidim flatten 后变量名变形？
- 是否来自 block comment 或宏展开？
```

### 11.2 要求

增加 `unresolvedSymbols` report：

```json
{
  "name": "HASH_ABILITY",
  "count": 12,
  "firstUseLine": 12345,
  "guessedCategory": "missingGlobalOrPrivateRewrite",
  "nearby": []
}
```

### 11.3 修复顺序

在完成 B/C/D/E/F 后再修 unresolved。因为许多 unresolved 是级联错误。

---

## 12. 任务 I：syntax-lite 增强

阶段 8 syntax-lite 必须继续绿色，并新增检测：

```text
1. duplicate function/global/native names
2. source syntax residue
3. block comment residue
4. chained array indexing residue
5. indexed struct member residue: arr[i].field
6. inline Zinc control residue: if (...) return/call/set without then
7. comma-separated local residue
8. forward function references, if feasible
9. lambda reference before definition, if feasible
```

输出到 validation report：

```json
"syntaxLite": {
  "ok": true,
  "issueCount": 0,
  "residualSourceForms": [],
  "forwardFunctionReferences": [],
  "commaLocalResidues": [],
  "indexedStructMemberResidues": []
}
```

---

## 13. 任务 J：性能打点，不做大优化

### 13.1 背景

Phase 7 codegen 约 80 秒，过慢。但 PJASS 未通过前，不建议做大规模性能优化。

### 13.2 本阶段只做低风险打点

在 stats 或 validation report 中新增细分 timings：

```json
"timingMs": {
  "preprocess": 0,
  "staticIf": 0,
  "lex": 0,
  "parse": 0,
  "moduleExpand": 0,
  "collectStructs": 0,
  "collectFunctionInterfaces": 0,
  "extractLambdas": 0,
  "lowerGlobals": 0,
  "lowerFunctions": 0,
  "functionEmissionPlanning": 0,
  "emitGlobals": 0,
  "emitFunctions": 0,
  "syntaxLite": 0,
  "comparison": 0,
  "pjass": 0,
  "total": 0
}
```

### 13.3 可做的低风险优化

如果发现明显 O(n²)：

```text
- 同一行/同一函数重复 regex_replace 很多次
- 每个函数都全量遍历所有函数表
- 每个表达式都全量遍历所有 struct/field
- validation 多次扫描完整 10 万行输出
```

允许改成 map/cache，但不要为了性能改语义。

### 13.4 不设硬性能目标

阶段 8 不以 1~2 秒为目标。阶段 8 目标仍是 PJASS 错误下降。

建议阶段 8 结束时记录：

```text
- total codegen ms
- 最慢 5 个 pass
- 是否比 phase7 更慢
```

---

## 14. 测试计划

### 14.1 必须新增 fixtures

```text
phase8_lambda_before_use
phase8_lambda_calls_helper
phase8_indexed_struct_field
phase8_indexed_struct_method
phase8_nested_indexed_struct_field
phase8_zinc_inline_if_return
phase8_zinc_inline_if_call
phase8_zinc_inline_if_assignment
phase8_comma_local_simple
phase8_comma_local_initializer
phase8_comma_local_array
```

### 14.2 negative fixtures

```text
phase8_function_dependency_cycle.negative.j
phase8_unknown_indexed_struct_type.negative.j
phase8_unsupported_inline_if_else_shape.negative.j
```

如果当前 test runner 不支持 negative expected diagnostics，可先将 negative cases 放入 `tests/fixtures_negative/` 并在 docs 中记录命令。

### 14.3 真实 input 验证

每次完成一个大任务后都跑：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase8.out.j ^
  --emit-stats build\input.phase8.codegen.stats.json ^
  --emit-validation-report build\input.phase8.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite
```

PJASS 不必每次都跑，但每完成 B/C/D/E/F 后至少跑一次：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase8.pjass.out.j ^
  --emit-stats build\input.phase8.pjass.stats.json ^
  --emit-validation-report build\input.phase8.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

---

## 15. 文档更新要求

新增：

```text
docs/phase8_status.md
```

必须包含：

```text
1. Implemented
2. Verification Commands
3. Output Stats
4. Syntax-Lite Result
5. Init Validation
6. PJASS Result
7. PJASS Before/After table: Phase 7 vs Phase 8
8. First Remaining PJASS Blockers
9. Performance Baseline with detailed timings
10. Known Limitations
11. Next Phase Suggestion
```

更新 README：

```text
- phase summary 增加 Phase 8
- CLI examples 增加 phase8 命令
- Phase Boundary 更新：当前是否 PJASS 通过；若未通过，列出剩余 blocker
```

---

## 16. 验收标准

### 16.1 最低合格

```text
- build 成功
- ctest 成功
- samples/input.j full codegen 成功
- syntax-lite 通过
- duplicate function/global/native 仍为 0
- PJASS 可运行并输出 grouped report
- 至少完成以下两类：
  1. lambda/function/struct method declaration ordering 有明显改善
  2. indexed struct member residue 或 comma local 或 inline Zinc control 至少两项清零
- docs/phase8_status.md 完整
```

### 16.2 良好

```text
- localOrder <= 3
- expectedEndfunction <= 50
- syntaxError <= 1000
- undefinedVariable <= 300
- undefinedFunction <= 300
- other 下降 30%+
```

### 16.3 优秀

```text
- PJASS 直接通过
或者
- PJASS 错误降到 <= 500 个，并且每个分类都有明确下一步修复方向
```

---

## 17. Codex 执行建议

建议按这个顺序执行：

```text
Step 1: 先增强 PJASS report 和 syntax-lite 检查。
Step 2: 修 comma-separated local declarations，因为范围小、收益直接。
Step 3: 修 inline Zinc if-return/control，因为会直接减少 syntaxError/expectedEndfunction。
Step 4: 修 indexed struct member lowering。
Step 5: 做 lambda emission order。
Step 6: 做 general function/struct method emission planner。
Step 7: 再看 unresolved symbols。
Step 8: 更新 docs/phase8_status.md 和 README。
```

不要反过来先做 unresolved symbols，因为很多 unresolved 很可能是前面几类错误造成的级联。

---

## 18. 阶段 8 完成后的推荐方向

如果阶段 8 后 PJASS 仍未通过，阶段 9 应继续从 PJASS report 出发，集中处理剩余最大的 2~3 类错误。

如果阶段 8 后 PJASS 通过，阶段 9 应进入：

```text
- Warcraft III/runtime loading validation
- JassHelper behavior comparison
- interface/delegate/operator/stub/super 补洞
- 性能优化专项
```
