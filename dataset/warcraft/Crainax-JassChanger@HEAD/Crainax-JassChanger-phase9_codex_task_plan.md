---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase9_codex_task_plan.md"
ref: HEAD
---

# Phase 9 Codex Implementation Plan — PJASS 语义 blocker 压缩与方法链 / 回调适配

Project: `Crainax/JassChanger`  
Compiler binary: `vjassc`  
Language target: `vJASS/Zinc/JASS -> plain Warcraft III JASS`  
Phase owner: Codex  
Phase goal: **把 Phase 8 剩余 PJASS 错误从 3144 继续压缩到可审查规模，重点修复 method-chain receiver、callback/code signature adapter、unresolved symbol provenance、return mismatch 和 true cycle 策略。**

---

## 0. 当前状态摘要

Phase 8 已经取得明显进展：

```text
Phase 7 broad PJASS grouped count: 12903
Phase 8 broad PJASS grouped count: 3144
```

Phase 8 已完成：

```text
- dependency-aware final function ordering
- lambda before first non-cyclic use
- comma-separated local declaration splitting
- inline Zinc if return/call/set lowering
- indexed struct member lowering: arr[i].field / arr[i].method(...)
- syntax-lite residue buckets
- grouped PJASS examples with generated line text and current function name
```

Phase 8 当前关键指标：

```json
{
  "syntaxLite.ok": true,
  "commaLocalResidues": 0,
  "indexedStructMemberResidues": 0,
  "inlineZincControlResidues": 0,
  "forwardLambdaReferences": 0,
  "forwardFunctionReferences": 3,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0,
  "pjass.ok": false,
  "pjass.groupedCount": 3144
}
```

Phase 8 剩余主要 PJASS blocker：

```text
1. function-interface/code callback signature mismatch
   Example: Function vjlambda__108 must not take any arguments when used as code

2. method-chain receiver result lowering incomplete
   Example: UIHashTable_uiHashTable(...).ui.bind(...)
   Example: UIHashTable_uiHashTable(frame).eventdata.get2()

3. unresolved source/environment symbols
   Example: uiHT, HASH_ABILITY, HASH_TIMER

4. uninitialized-variable diagnostics caused by method-chain lowering gaps

5. true cyclic forward references
   Example: syncBus.onInit <-> onDataSync
   Example: CombineSession.buildSelector <-> runStage
   Example: MopUpItemCreate <-> vjlambda__627
```

Phase 9 不是新语法阶段。Phase 9 是 **PJASS blocker 压缩阶段**。

---

## 1. Phase 9 总目标

### 1.1 硬目标

Phase 9 完成后必须保持：

```text
- samples/input.j full codegen 成功
- --check-output-syntax-lite 通过
- duplicateFunctionNames == 0
- duplicateGlobalNames == 0
- duplicateNativeNames == 0
- commaLocalResidues == 0
- indexedStructMemberResidues == 0
- inlineZincControlResidues == 0
- forwardLambdaReferences == 0
- main/config/init validation 无 issue
```

并且必须显著降低 PJASS 错误：

```text
最低合格：
- broad grouped PJASS count <= 1500
- undefinedFunction <= 100
- syntaxError <= 400
- expectedEndfunction <= 20
- localOrder == 0

良好：
- broad grouped PJASS count <= 800
- undefinedFunction <= 50
- undefinedVariable <= 300
- returnMismatch <= 60
- uninitializedVariable <= 150

优秀：
- PJASS 直接通过
  或 broad grouped PJASS count <= 300 且剩余 blocker 分类清晰
```

### 1.2 阶段 9 不做的内容

除非是 PJASS blocker 必需，否则不要在 Phase 9 做这些：

```text
- 完整 interface dispatch
- delegate 完整语义
- operator overload 全量兼容
- stub/super 完整兼容
- byte-for-byte JassHelper output matching
- Warcraft III runtime 行为验证
- 大规模性能优化 / 架构重写
```

性能只做 profiling 与低风险优化，不做大规模重构。

---

## 2. 开发环境与基线复现

### 2.1 构建

Windows：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

如果需要全新构建：

```bat
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

### 2.2 Phase 8 基线命令

Full codegen + syntax-lite + comparison：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase9.out.j ^
  --emit-stats build\input.phase9.codegen.stats.json ^
  --emit-validation-report build\input.phase9.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite
```

PJASS validation：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase9.pjass.out.j ^
  --emit-stats build\input.phase9.pjass.stats.json ^
  --emit-validation-report build\input.phase9.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

### 2.3 每次修复后必须记录

每次修复一个 blocker class 后，记录：

```text
- total runtime
- codegen runtime
- syntaxLite runtime
- pjass runtime
- grouped PJASS count
- each PJASS category count
- first 5 examples for remaining largest categories
```

---

## 3. Task A — PJASS triage report 继续细化

Phase 8 report 已经可用，但 Phase 9 需要进一步把 `other` 和 callback/method-chain 相关错误拆开。

### 3.1 新增分类

在 validation report 中新增或强化这些分类：

```text
callbackCodeSignatureMismatch
methodChainReceiverResidue
methodChainUninitialized
unresolvedPublicGlobal
unresolvedPrivateGlobal
unresolvedStructType
unresolvedArrayStruct
unresolvedEnvironmentSymbol
returnValueInNothingFunction
returnMissingValue
returnWrongTypeLikely
trueFunctionCycle
```

### 3.2 每个分类必须带样例

每个分类至少输出：

```json
{
  "generatedLine": 12345,
  "currentFunction": "functionName",
  "message": "PJASS message",
  "lineText": "generated output line text",
  "nearbyLines": ["...", "..."],
  "suspectedSource": "optional source location if known"
}
```

### 3.3 验收

`build/input.phase9.pjass.validation.json` 中必须能回答：

```text
- 哪些错误是 callback signature mismatch？
- 哪些错误是 method chain 没降级？
- 哪些错误是未解析 symbol？
- 哪些错误是 return mismatch？
- 哪些错误是真循环导致的 forward reference？
```

---

## 4. Task B — method-chain receiver result lowering

Phase 8 已经解决：

```text
arr[i].field
arr[i].method(...)
music[1002].play()
```

Phase 9 要解决更复杂的 receiver：

```text
foo().field
foo().method(...)
foo().field.method(...)
foo().field.get2()
arr[i].field.method(...)
UIHashTable_uiHashTable(frame).eventdata.get2()
UIHashTable_uiHashTable(frame).ui.bind(...)
```

### 4.1 真实 blocker 例子

源里存在类似结构：

```zinc
library UIHashTable {
    public hashtable HASH_UI = InitHashtable();
    integer frame = 0;

    public function uiHashTable(integer f) -> uiHT {
        frame = f;
        return uiHT[0];
    }

    struct uiHT [] {
        uiHTEvent eventdata;
        uiHTFrame ui;
    }

    struct uiHTFrame [] {
        method bind(integer typeID, integer ui) {
            SaveInteger(HASH_UI, frame, 1820, typeID);
            SaveInteger(HASH_UI, frame, 1821, ui);
        }
    }
}
```

调用形态：

```zinc
uiHashTable(frame).ui.bind(typeID, ui)
uiHashTable(frame).eventdata.get2()
```

不能输出为：

```jass
UIHashTable_uiHashTable(frame).ui.bind(...)
```

必须降级成普通函数/数组访问。

### 4.2 设计要求

实现一个独立的 receiver-chain lowering 逻辑，建议命名：

```text
StructChainLowerer
ReceiverChainLowerer
MethodChainLowerer
```

不要只用单个 regex 硬替换。

最低要求：

```text
- 识别 receiver expression 的静态类型
- receiver 可以是 local/global/field/array access/function call/static method call
- 对每一段 .field / .method() 做类型推进
- 如果 receiver 是复杂表达式，必须只求值一次
- 必须保持副作用顺序
- 必须能插入 temp local 与 set prelude
```

### 4.3 降级模型

#### 4.3.1 function-return struct + field

输入：

```zinc
uiHashTable(frame).ui
```

假设：

```text
uiHashTable(integer) -> uiHT
uiHT.ui -> uiHTFrame
```

输出模型：

```jass
local integer vjtmp_uiht
set vjtmp_uiht = UIHashTable_uiHashTable(frame)
// expression becomes s__uiHT_ui[vjtmp_uiht]
```

#### 4.3.2 function-return struct + field + method

输入：

```zinc
uiHashTable(frame).ui.bind(typeID, ui)
```

输出模型：

```jass
local integer vjtmp_uiht
local integer vjtmp_ui
set vjtmp_uiht = UIHashTable_uiHashTable(frame)
set vjtmp_ui = s__uiHT_ui[vjtmp_uiht]
call s__uiHTFrame_bind(vjtmp_ui, typeID, ui)
```

如果 `uiHT` 是 array struct，需要使用该 array struct 的既有 lowering 规则，不要生成 allocator。

#### 4.3.3 nested method return chain

输入：

```zinc
foo().bar().baz
```

输出模型：

```jass
local integer vjtmp1
local integer vjtmp2
set vjtmp1 = foo()
set vjtmp2 = s__Foo_bar(vjtmp1)
// expression becomes s__Bar_baz[vjtmp2]
```

### 4.4 Type source

类型信息来源：

```text
- FunctionInfo return type
- MethodInfo return type
- StructInfo field type
- localTypes table
- global symbol table
- function interface table where applicable
```

如果类型未知：

```text
- 不要猜
- 不要生成 dummy
- 在 validation report 标记为 methodChainUnknownType
- 保留原 PJASS 失败样例
```

### 4.5 作用域和 local 插入规则

所有 temp local 必须放在函数 local 区域顶部。执行顺序用 `set` 放在原表达式位置前。

错误示例：

```jass
call Foo(local integer tmp) // illegal
```

正确示例：

```jass
function X takes nothing returns nothing
    local integer vjtmp1
    set vjtmp1 = Foo()
    call Bar(vjtmp1)
endfunction
```

### 4.6 Syntax-lite 增强

新增 residue 检查：

```text
call-result member residue:    )\.[A-Za-z_]
indexed-result member residue: \]\.[A-Za-z_]
function-chain residue:        \)\.[A-Za-z_][A-Za-z0-9_]*\(
field-chain residue:           \.[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_]
```

但必须保护：

```text
- strings
- rawcodes
- comments
- decimal numbers like 1.23
```

### 4.7 Fixtures

新增 fixture：

```text
tests/fixtures/phase9_method_chain_function_receiver.in.j
tests/fixtures/phase9_method_chain_function_receiver.expected.j

tests/fixtures/phase9_method_chain_nested_field_method.in.j
tests/fixtures/phase9_method_chain_nested_field_method.expected.j

tests/fixtures/phase9_method_chain_array_struct_receiver.in.j
tests/fixtures/phase9_method_chain_array_struct_receiver.expected.j
```

至少覆盖：

```zinc
uiHashTable(frame).ui.bind(1, 2);
uiHashTable(frame).eventdata.get2();
music[1002].play();
foo().bar.baz();
```

---

## 5. Task C — callback/code/function-interface signature adapter

Phase 8 的第一大 blocker 是：

```text
Function vjlambda__108 must not take any arguments when used as code
```

JASS 的 `code` 只能指向：

```jass
function X takes nothing returns nothing
```

而 vJASS/Zinc 的 lambda/function interface 可能带参数或返回值。Phase 9 必须区分：

```text
- 真正的 JASS code callback
- function interface target id
- boolexpr condition/filter callback
- custom bind/register 函数的 callback 参数
```

### 5.1 建立 LambdaUseContext

为每个 lambda / function reference 记录：

```cpp
struct LambdaUseContext {
    SourceLocation loc;
    std::string generatedLambdaName;
    FunctionSignature lambdaSignature;
    std::string expectedTypeName;
    ExpectedCallbackKind expectedKind;
    std::string containingFunction;
    std::string callsiteCallee;
    size_t callsiteArgumentIndex;
};
```

`ExpectedCallbackKind` 至少包含：

```text
Unknown
CodeAction              // nothing -> nothing
CodeCondition           // nothing -> boolean, used by Condition/Filter-like wrapper
FunctionInterface       // vJASS function interface integer id
OrdinaryFunctionObject  // direct-call fallback or function object
CustomKnownCallback     // known custom callback registry
Invalid
```

### 5.2 Known callback APIs

建立或扩展已知 API 表：

```text
TriggerAddAction(trigger, code)
TriggerAddCondition(trigger, boolexpr)
Condition(code) -> conditionfunc/boolexpr depending model
Filter(code) -> filterfunc/boolexpr depending model
TimerStart(timer, real, boolean, code)
ForGroup(group, code)
EnumItemsInRect(..., boolexpr, code)
GroupEnumUnitsInRange(..., boolexpr)
DzTriggerRegister... / DzFrame... callbacks if source uses them
```

不要只看参数名；必须根据 callee signature 或 known API table 推断 expected kind。

### 5.3 Function interface 不应当被当作 code

如果某个函数参数类型是 function interface，例如：

```jass
function interface Callback takes integer x returns nothing
function Register takes Callback cb returns nothing
```

调用：

```zinc
Register(function(integer x) { ... });
```

应生成：

```jass
call Register(vjassc_function_interface_target_id)
```

不要生成：

```jass
call Register(function vjlambda__N) // wrong if Register expects integer function-interface id
```

### 5.4 真 code callback 的适配规则

#### 5.4.1 Lambda 无参 returns nothing

输入：

```zinc
TimerStart(t, 1.0, false, function() { BJDebugMsg("x"); });
```

输出：

```jass
function vjlambda__N takes nothing returns nothing
    call BJDebugMsg("x")
endfunction
call TimerStart(t, 1.0, false, function vjlambda__N)
```

#### 5.4.2 Lambda 有参数，但 callsite 是 true code

输入：

```zinc
TimerStart(t, 1.0, false, function(integer x) { ... });
```

这在 JASS code callback 下没有参数来源。必须：

```text
- 如果 Codex 能证明这是 custom known callback 并知道如何提供参数，则生成 adapter
- 否则报明确 diagnostic，不要生成非法 code
```

Diagnostic 示例：

```text
lambda with parameters cannot be used as raw JASS code callback; expected nothing -> nothing
```

#### 5.4.3 Condition/Filter wrapper

输入：

```zinc
TriggerAddCondition(t, Condition(function() -> boolean { return true; }));
```

输出：

```jass
function vjlambda__N takes nothing returns boolean
    return true
endfunction
call TriggerAddCondition(t, Condition(function vjlambda__N))
```

如果 lambda 带参数，同样必须有 known context，否则 reject。

### 5.5 Adapter wrapper 生成

对于需要从 function-interface 转成 code 的合法场景，生成 wrapper：

```jass
function vjadapter__N takes nothing returns nothing
    call sc__Interface_execute(vjadapter__N_target, ...context args...)
endfunction
```

但必须满足：

```text
- 参数来源明确
- 全局 temp 不冲突
- 不破坏 multiplayer async/sync
- 不使用未初始化值
```

如果参数来源不明确，不要生成假 adapter。

### 5.6 Stats

新增统计：

```json
{
  "callbackAdaptersGenerated": 0,
  "codeCallbacksDirect": 0,
  "codeCallbacksRejected": 0,
  "functionInterfaceLambdas": 0,
  "functionInterfaceMistakenAsCode": 0
}
```

### 5.7 Fixtures

新增：

```text
phase9_callback_code_noarg.in.j
phase9_callback_code_noarg.expected.j
phase9_callback_function_interface_param.in.j
phase9_callback_function_interface_param.expected.j
phase9_callback_invalid_code_param.in.j
phase9_callback_invalid_code_param.expected.err
phase9_callback_condition_boolean.in.j
phase9_callback_condition_boolean.expected.j
```

---

## 6. Task D — unresolved globals / source symbol provenance

Phase 8 剩余 unresolved symbols：

```text
uiHT
HASH_ABILITY
HASH_TIMER
```

这些不能用 dummy declaration 解决。必须追来源。

### 6.1 添加 SymbolProvenanceReport

为每个 PJASS undefined symbol 生成 provenance：

```json
{
  "symbol": "HASH_TIMER",
  "kind": "undefinedVariable",
  "generatedLine": 12345,
  "currentFunction": "X",
  "lineText": "call SaveInteger(HASH_TIMER, ...)",
  "sourceCandidates": [
    {
      "sourceLocation": "samples/input.j:3467",
      "declKind": "global",
      "container": "library HashTable",
      "access": "public",
      "generatedName": "HashTable_HASH_TIMER",
      "emitted": true
    }
  ],
  "suspectedCause": "public/global rename missing at use site"
}
```

### 6.2 重点检查 HashTable public block

源中存在：

```zinc
library HashTable {
    public{
        hashtable HASH_TYPEID = InitHashtable();
        hashtable HASH_TIMER = InitHashtable();
        hashtable HASH_GROUP = InitHashtable();
        hashtable HASH_EFFECT = InitHashtable();
        hashtable HASH_TRIGGER = InitHashtable();
        hashtable HASH_ITEM = InitHashtable();
        hashtable HASH_ABILITY = InitHashtable();
        hashtable HASH_DIALOG = InitHashtable();
    }
}
```

期望：

```text
- library 内部可用 HASH_TIMER / HASH_ABILITY
- library 外部应重写为 HashTable_HASH_TIMER / HashTable_HASH_ABILITY
- lambda、method-chain lowering、adapter wrapper 中也必须执行同样 rewrite
```

### 6.3 检查 public/private block parsing

Zinc 支持：

```zinc
public { ... }
private { ... }
```

Phase 9 要确认：

```text
- public{ 无空格形式能解析
- public { 有空格形式能解析
- block 内多行 global declarations 都带 public access
- block 内无分号/奇怪格式不导致漏解析
```

新增 fixture：

```text
phase9_zinc_public_block_globals.in.j
phase9_zinc_public_block_globals.expected.j
phase9_zinc_public_block_no_space.in.j
phase9_zinc_public_block_no_space.expected.j
```

### 6.4 uiHT 来源

源中存在：

```zinc
library UIHashTable {
    public function uiHashTable(integer f) -> uiHT { ... }

    struct uiHT [] {
        uiHTEvent eventdata;
        uiHTFrame ui;
    }
}
```

`uiHT` unresolved 可能由：

```text
- array struct type rewrite 漏了
- function return type rewrite 漏了
- method-chain lowering type inference 漏了
- public/private rename 漏了
```

不要直接声明 `integer uiHT`。正确做法是：

```text
- struct type as JASS integer
- return type uiHT -> integer
- field access uiHT.ui -> corresponding array field
```

### 6.5 验收

Phase 9 后：

```text
HASH_ABILITY 不应作为 undefinedVariable 出现
HASH_TIMER 不应作为 undefinedVariable 出现
uiHT 不应作为 unknown type/undefined variable 出现
```

如果仍出现，validation report 必须给出 source candidate 与 suspectedCause。

---

## 7. Task E — return mismatch 分类与修复

Phase 8 returnMismatch 从 67 增到 118，说明旧错误修掉后新问题暴露，或 method-chain/callback adapter 引入了返回类型错配。

### 7.1 分类

把 return mismatch 拆成：

```text
returnValueInNothingFunction
returnMissingValueInNonNothingFunction
returnBooleanUsedAsNothing
returnFunctionInterfaceEvaluateMismatch
returnMethodChainUnknownResult
returnLambdaAdapterMismatch
```

### 7.2 修复规则

#### 7.2.1 Zinc inline return

确保：

```zinc
if (cond) return expr;
```

在返回值函数中：

```jass
if (cond) then
    return expr
endif
```

在 returns nothing 函数中，如果 `expr` 非空，应报错，不要输出非法 JASS。

#### 7.2.2 `.execute` / `.evaluate`

规则：

```text
- .execute 用于 returns nothing
- .evaluate 用于有返回值
- 如果返回值被忽略，可以允许 evaluate 作为 statement，但必须丢弃结果合法
- 如果 expected expression 需要值，不可用 execute
```

#### 7.2.3 adapter return

Condition/Filter adapter 必须 returns boolean。Action/code adapter 必须 returns nothing。

### 7.3 Fixture

```text
phase9_return_inline_if_value.in.j
phase9_return_inline_if_value.expected.j
phase9_return_callback_condition.in.j
phase9_return_callback_condition.expected.j
phase9_return_invalid_nothing_value.in.j
phase9_return_invalid_nothing_value.expected.err
```

---

## 8. Task F — true cyclic forward reference strategy

Phase 8 剩余 3 个 forwardFunctionReferences，且文档标记为 true cycles。

### 8.1 不要用普通重排解决真循环

函数 A 调 B，B 调 A，单纯排序永远不能让两边都“先定义”。需要语义策略。

### 8.2 分类 true cycles

为每个 cycle 输出：

```json
{
  "cycle": ["A", "B"],
  "edges": [
    {"from": "A", "to": "B", "callKind": "direct", "signature": "..."},
    {"from": "B", "to": "A", "callKind": "direct", "signature": "..."}
  ],
  "recommendedStrategy": "executeFunc | functionInterfaceThunk | requiresSourceChange | unresolved"
}
```

### 8.3 支持简单 no-arg/nothing cycle

如果某个 cycle edge 是：

```text
callee takes nothing returns nothing
call result not needed
```

可以把其中一个边降成：

```jass
call ExecuteFunc("calleeName")
```

但必须注意：

```text
- ExecuteFunc 新线程语义不同
- 只可用于 no-arg returns nothing
- 不可用于需要返回值或参数的调用
- validation report 必须记录 semantic deviation
```

### 8.4 参数/返回值 cycle

如果 cycle 需要参数或返回值：

```text
- 优先考虑已有 function-interface runtime / prototype wrapper
- 如果无法无损处理，保留为 known blocker
- 不要生成 dummy function
```

### 8.5 验收

最低目标：

```text
- forwardFunctionReferences 保持 <= 3
- validation report 明确列出 true cycles 和策略
```

良好目标：

```text
- no-arg/nothing cycles 被 ExecuteFunc bridge 解决
- forwardFunctionReferences <= 1
```

优秀目标：

```text
- forwardFunctionReferences == 0
```

---

## 9. Task G — Performance profiling, not broad optimization

Phase 8 timing：

```json
{
  "codegen": 102711,
  "syntaxLite": 6923,
  "pjass": 673,
  "total": 111611
}
```

PJASS 很快，慢点在 codegen 和全输出扫描。Phase 9 必须加 profiling，但不要进行大规模架构重写。

### 9.1 添加 pass-level timers

至少输出：

```json
{
  "codegenPasses": {
    "collectStructs": 0,
    "collectFunctions": 0,
    "collectFunctionInterfaces": 0,
    "lowerLambdas": 0,
    "lowerZincBodies": 0,
    "lowerStructExpressions": 0,
    "methodChainLowering": 0,
    "functionOrdering": 0,
    "emitGlobals": 0,
    "emitFunctions": 0,
    "emitStructSupport": 0,
    "emitFunctionInterfaceRuntime": 0,
    "finalOutputValidationPrep": 0
  }
}
```

### 9.2 查找高风险慢点

重点检查：

```text
- 每行都遍历所有 structs/functions/globals 的 O(N*M)
- 对整个输出反复 regex_replace
- 多次全文件 string copy
- function ordering 中 repeated graph rebuild
- symbol lookup 使用 vector linear scan 而不是 unordered_map
- method-chain lowering 重复解析同一表达式
```

### 9.3 允许的低风险优化

```text
- 编译 regex 为 static const
- 建立 name -> info map
- 避免重复 trim / split
- CodeWriter reserve
- 仅对 changed lines 做 rewrite
- syntax-lite 扫描合并为单 pass
```

### 9.4 不允许的优化

```text
- 不要为了速度关闭 validation
- 不要跳过 PJASS report
- 不要改变 lowering 语义但无测试
- 不要大规模重写 parser/codegen 架构
```

---

## 10. Task H — Syntax-lite 与 validation report 更新

Phase 9 syntax-lite 必须新增：

```text
methodChainCallResultResidues
callbackCodeSignatureResidues
unresolvedKnownSourceSymbols
returnMismatchLikelyResidues
trueFunctionCycles
```

输出示例：

```json
{
  "methodChainCallResultResidues": 0,
  "callbackCodeSignatureResidues": 0,
  "unresolvedKnownSourceSymbols": 0,
  "returnMismatchLikelyResidues": 0,
  "trueFunctionCycles": 3
}
```

---

## 11. Task I — Tests and fixtures

### 11.1 Must add golden fixtures

```text
phase9_method_chain_function_receiver
phase9_method_chain_nested_receiver
phase9_method_chain_array_struct_receiver
phase9_callback_code_noarg
phase9_callback_function_interface_param
phase9_callback_condition_boolean
phase9_zinc_public_block_globals
phase9_zinc_public_block_no_space
phase9_return_inline_if_value
phase9_cycle_noarg_execute_func_bridge
```

### 11.2 Must add negative fixtures

```text
phase9_callback_invalid_code_param
phase9_method_chain_unknown_type
phase9_return_invalid_nothing_value
phase9_cycle_value_return_unresolved
```

### 11.3 Regression fixtures from previous phases

All previous fixtures must continue passing:

```bat
ctest --test-dir build --output-on-failure
```

Do not weaken existing expected outputs just to pass tests.

---

## 12. Acceptance checklist

Phase 9 is complete only if all are true:

```text
[ ] Build succeeds
[ ] All ctest fixtures pass
[ ] samples/input.j full codegen succeeds
[ ] syntax-lite passes
[ ] duplicateFunctionNames == 0
[ ] duplicateGlobalNames == 0
[ ] duplicateNativeNames == 0
[ ] commaLocalResidues == 0
[ ] indexedStructMemberResidues == 0
[ ] inlineZincControlResidues == 0
[ ] methodChainCallResultResidues == 0 or each residue is explained
[ ] callbackCodeSignatureMismatch count is reduced or each case classified
[ ] HASH_ABILITY/HASH_TIMER/uiHT no longer appear as unexplained undefineds
[ ] PJASS grouped count <= 1500, ideally <= 800 or PJASS passes
[ ] validation report includes current function and generated line for top examples
[ ] codegen pass timing is included in stats/report
[ ] docs/phase9_status.md is written
[ ] README is updated
```

---

## 13. Required docs update

Create `docs/phase9_status.md` with:

```markdown
# Phase 9 Status

## Implemented

## Verification Commands

## Output Stats

## Syntax-Lite Result

## Init Validation

## PJASS Result

## PJASS Before/After

## Remaining PJASS Blockers

## Method Chain Lowering Notes

## Callback Adapter Notes

## Symbol Provenance Notes

## True Cycle Notes

## Performance Baseline

## Known Limitations

## Next Phase Suggestion
```

README must add one paragraph:

```text
Phase 9 focuses on remaining PJASS semantic blockers: method-chain receiver lowering, callback/code signature adaptation, source symbol provenance, return mismatch classification, and true cyclic forward references. The real sample still may not pass PJASS unless stated in docs/phase9_status.md.
```

---

## 14. Suggested implementation order

Do not start with broad undefined-symbol cleanup. Use this order:

```text
1. Reproduce Phase 8 PJASS baseline locally.
2. Add report/profiling improvements first.
3. Implement method-chain receiver result lowering.
4. Fix HASH_ABILITY/HASH_TIMER/uiHT provenance and public/global rewrite gaps exposed by method-chain work.
5. Implement callback/code/function-interface signature adapter classification.
6. Fix clear return mismatch classes.
7. Classify true cycles and implement only safe no-arg/nothing bridges.
8. Rerun PJASS and compare counts.
9. Update docs and README.
```

Reason: method-chain lowering will likely reduce uninitializedVariable, returnMismatch, undefinedVariable, and other categories at the same time. Callback adapter work should come after expected type inference is reliable.

---

## 15. Final expected outcome

At the end of Phase 9, the compiler should still not claim full JassHelper replacement unless PJASS passes. The expected improvement is:

```text
- PJASS errors much lower than Phase 8
- method-chain and callback signature blockers isolated or fixed
- known source symbols traced instead of guessed
- remaining errors small enough to drive Phase 10 toward PJASS pass / runtime validation
```

If PJASS passes in Phase 9, Phase 10 should switch to:

```text
- Warcraft III runtime loading validation
- behavior comparison against output_jasshelper.j
- interface/delegate/operator/stub/super gap audit
- performance optimization toward 1-2 seconds
```
