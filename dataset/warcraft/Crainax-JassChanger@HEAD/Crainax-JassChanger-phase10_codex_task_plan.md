---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase10_codex_task_plan.md"
ref: HEAD
---

# Phase 10 Codex 实施计划：冲刺 PJASS 通过 + 低风险性能治理

> 项目：`Crainax/JassChanger`
> 当前阶段：Phase 9 已完成
> 本阶段目标：优先让真实 `samples/input.j` 生成的 JASS 通过 PJASS；如果短期无法安全通过，则把 PJASS grouped count 压到可人工审查的低位，并产出明确 blocker 报告。
> 禁止目标：不要为了 PJASS 通过而添加无来源 dummy 变量、伪 native、伪函数；不要大规模重写 codegen 架构；不要牺牲语义正确性换取“表面通过”。

---

## 0. 当前状态摘要

Phase 9 已经把真实输入的 PJASS grouped count 从 Phase 8 的 `3144` 压到 `889`，并保持：

```text
syntax-lite: green
init validation: green
duplicateFunctionNames: 0
duplicateGlobalNames: 0
duplicateNativeNames: 0
localOrder: 0
undefinedFunction: 19
syntaxError: 303
expectedEndfunction: 11
```

Phase 9 后仍存在的主要 blocker：

```text
1. struct-returning method chain 仍有深层残留
   例如：s__Tooltip_tooltip_create().layoutTitle(...)
         s__UIText_uiText_setText(...).show(true)

2. nested generated array/index expression 仍可能生成 PJASS 非法形式
   例如：s__UIText_uiText_ui[s__UIText_uiText_setPoint(...)]

3. static / instance context 中仍有 this 泄漏

4. 参数化 lambda 被当作 raw code 传递，导致 code callback 签名不匹配

5. 部分 generated function 缺少所有路径 return

6. yd_* / HASH_ABILITY / HASH_TIMER 等环境符号来源未定

7. 仍有 3 个 true forward cycle candidates
```

Phase 9 还加入了 codegen pass timing，当前性能瓶颈大致为：

```text
codegen: ~109s
emitFunctions: ~96s
emitStructSupport: ~76s
finalOutputValidationPrep: ~12s
sanitizeOutput: ~9.6s
lowerLambdas: ~9.2s
functionOrdering: ~3.1s
```

本阶段仍以 correctness 为主，但允许做低风险性能优化。

---

## 1. 是否可以让 Codex “直接跑到 PJASS 通过为止”？

可以把 PJASS 通过设为 **最高目标**，但不要给 Codex 一个无边界的“无限修到通过”为指令。建议采用下面的边界：

```text
最高目标：让 samples/input.j 通过 PJASS。

若无法在本阶段安全通过，必须满足：
- grouped PJASS count <= 200，或相比 Phase 9 至少再下降 70%；
- 所有剩余错误都有清晰分类；
- 不允许 dummy declarations；
- 不允许跳过真实错误；
- 不允许删除源逻辑或输出逻辑来绕过 PJASS；
- 必须记录 blockers 和下一步建议。
```

Codex 执行方式应是 **bounded retry loop**，而不是无限循环：

```text
1. 运行 baseline。
2. 按优先级修复一个 blocker 类。
3. 运行 tests + full codegen + syntax-lite + PJASS。
4. 对比 grouped count 和具体 examples。
5. 若错误下降，继续下一类 blocker。
6. 若连续两轮同类修复没有下降，停止该类修复，转向下一类或写明原因。
7. 若 PJASS 通过，停止并更新文档。
```

---

## 2. Phase 10 硬目标

### 2.1 最低合格目标

```text
- build 成功
- ctest 全部通过
- samples/input.j full codegen 成功
- syntax-lite 通过
- init validation 通过
- duplicate function/global/native 仍为 0
- localOrder 仍为 0
- grouped PJASS count <= 500
- undefinedFunction <= 10
- syntaxError <= 150
- expectedEndfunction <= 5
```

### 2.2 良好目标

```text
- grouped PJASS count <= 200
- undefinedFunction <= 5
- undefinedVariable <= 80
- syntaxError <= 60
- returnMissingValue <= 20
- callbackCodeSignatureMismatch <= 5
- methodChainReceiverResidue <= 10
- forwardFunctionReference <= 3 且全部有处理策略
```

### 2.3 优秀目标

```text
- PJASS 通过
- validation report 中 pjass.ok == true
- 真实输出仍保持 syntax-lite green
- 生成结果不靠 dummy declarations / logic deletion / unsafe stubs
```

---

## 3. 本阶段禁止事项

```text
禁止 1：不要为 HASH_ABILITY / HASH_TIMER / yd_* 直接加 dummy global。
禁止 2：不要为了消除 PJASS 错误删除源函数、源 globals、lambda 或 struct support。
禁止 3：不要把带参数 lambda 直接强行转成 code。
禁止 4：不要隐藏 PJASS 错误分类。
禁止 5：不要大规模重写 parser / codegen 架构。
禁止 6：不要把 runtime behavior 明显改错，只为 PJASS 通过。
禁止 7：不要关闭 syntax-lite 或 PJASS validation。
```

---

## 4. 基线验证命令

Codex 开始前必须运行：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

真实输入 syntax-lite：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase10.out.j ^
  --emit-stats build\input.phase10.codegen.stats.json ^
  --emit-validation-report build\input.phase10.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite
```

真实输入 PJASS：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase10.pjass.out.j ^
  --emit-stats build\input.phase10.pjass.stats.json ^
  --emit-validation-report build\input.phase10.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

如果 PJASS 路径在本地不同，允许使用等价路径，但必须在 `docs/phase10_status.md` 记录实际命令。

---

## 5. 任务 A：method-chain continuation 完整 lowering

### 5.1 问题

Phase 9 已能处理一部分 struct-returning receiver chain，但仍有：

```jass
s__Tooltip_tooltip_create().layoutTitle(...)
s__UIText_uiText_setText(...).show(true)
UIHashTable_uiHashTable(frame).eventdata.get2()
foo().bar.baz()
foo().bar(...).baz(...)
arr[i].field.method().next
```

这些必须降级为 PJASS 可接受的普通函数调用 / 数组访问 / 临时变量。

### 5.2 设计原则

对任何 receiver expression：

```text
<struct-returning-expression> . <field-or-method-chain>
```

如果 receiver 不是一个简单变量名或 `this`，则必须创建临时 local：

```jass
local integer vjtmp_chain_1
set vjtmp_chain_1 = <struct-returning-expression>
```

然后继续降级：

```jass
call s__Type_method(vjtmp_chain_1, ...)
set x = s__Type_field[vjtmp_chain_1]
```

如果 chain 在 expression context 中使用，例如：

```jass
set x = foo().bar()
call Use(foo().bar.baz())
```

需要生成 prelude：

```jass
local integer vjtmp_chain_1
local integer vjtmp_chain_2
set vjtmp_chain_1 = foo()
set vjtmp_chain_2 = s__Foo_bar(vjtmp_chain_1)
set x = s__Bar_baz(vjtmp_chain_2)
```

### 5.3 具体实现任务

```text
A1. 扩展 lowerExpression / lowerStatementLine 中 receiver-chain 识别。
A2. 支持 call-result receiver：foo().method / foo().field。
A3. 支持 method-result receiver：foo().bar().baz。
A4. 支持 indexed receiver：arr[i].field.method()。
A5. 支持 generated helper receiver：s__Type_create().init(...)
A6. 支持 leading-dot continuation：上一表达式之后的 .method(...)
A7. 所有新增 local 必须 hoist 到函数 local 区，set 保留原执行点。
A8. 添加 syntax-lite residue：`\)\.[A-Za-z_]`、`]\.[A-Za-z_]`、`\.\w+\(` 残留。
A9. 添加 golden fixtures。
```

### 5.4 推荐 fixtures

`tests/fixtures/phase10_method_chain_call_result.in.j`

```jass
//! zinc
library T {
    struct Node {
        integer value;
        static method make(integer v) -> thistype {
            thistype this = thistype.allocate();
            this.value = v;
            return this;
        }
        method get() -> integer {
            return this.value;
        }
        method next() -> thistype {
            return this;
        }
    }

    function Run() {
        integer a = Node.make(8).get();
        integer b = Node.make(9).next().get();
    }
}
//! endzinc
```

预期：无 `.get` / `.next` 残留，输出普通 JASS。

---

## 6. 任务 B：nested generated array/index expression normalization

### 6.1 问题

仍可能出现：

```jass
s__UIText_uiText_ui[s__UIText_uiText_setPoint(...)]
```

这里的 index expression 调用了可能返回 struct 或进行副作用的方法，PJASS 可能报 syntax 或后续级联。

### 6.2 实现策略

对复杂 index expression 做临时变量提取：

```jass
local integer vjtmp_index_1
set vjtmp_index_1 = s__UIText_uiText_setPoint(...)
set x = s__UIText_uiText_ui[vjtmp_index_1]
```

适用范围：

```text
- array[functionCall(...)]
- array[methodCall(...)]
- array[foo().bar()]
- array[nested[index]] 中内层复杂表达式
```

不要对简单表达式过度临时化：

```text
array[i]
array[i + 1]
array[this]
array[someInteger]
```

可以保持原样。

### 6.3 具体实现任务

```text
B1. 添加 isComplexIndexExpression。
B2. 在 lowerExpression 中为复杂 index 生成 prelude temp。
B3. 在 statement lowering 中确保 temp local hoist。
B4. 更新 syntax-lite 检查，统计 complexIndexResidues。
B5. 添加 fixture：array[Struct.create().method()] / array[foo().bar()]。
```

---

## 7. 任务 C：raw `code` callback signature adapter

### 7.1 问题

JASS `code` 只能引用：

```jass
function F takes nothing returns nothing
```

但当前仍可能把带参数 lambda 作为 raw code 传递：

```text
Function vjlambda__108 must not take any arguments when used as code
```

### 7.2 必须区分三种场景

#### 场景 1：目标真实 expected type 是 function interface

如果调用点实际接受 function interface，不应该当 raw `code`，应走已有 interface wrapper。

```text
Action: 修复 expected type 推断，不生成 code reference。
```

#### 场景 2：目标真实 expected type 是 JASS code，且 lambda 无参无返回

直接允许：

```jass
function vjlambda__N
```

#### 场景 3：目标真实 expected type 是 JASS code，但 lambda 有参数或返回值

不能直接传。必须做 adapter，或如果没有安全参数来源则明确报 diagnostics，不生成错误 JASS。

### 7.3 Adapter 策略

如果参数来自 function-interface dispatch globals：

```jass
function vjlambda__N takes integer a returns nothing
    ...
endfunction

function vjlambda__N_code_adapter takes nothing returns nothing
    call vjlambda__N(vjassc__callback_integer_1)
endfunction
```

如果参数来自 event context，例如 `GetTriggerUnit()`、`GetExpiredTimer()`，仅当代码模式可确定时生成 adapter。

如果无法确定参数来源：

```text
- 不要生成非法 JASS。
- 记录 callbackCodeSignatureMismatch。
- 生成明确 diagnostic，说明该 lambda 不能作为 raw code。
```

### 7.4 实现任务

```text
C1. 给每个 lambda 记录 signature、expected context、call site。
C2. 在 function argument lowering 中区分 expected code vs expected interface。
C3. 对 code expected context 校验 lambda 是否 takes nothing returns nothing。
C4. 为安全场景生成 `vjlambda__N_code_adapter`。
C5. 对不安全场景输出 diagnostics，不 silent bad code。
C6. validation report 输出 callback adapters generated / rejected。
C7. 添加 fixtures：
    - no-arg lambda to code OK
    - parameter lambda to function interface OK
    - parameter lambda to raw code rejected or adapted only with known context
```

---

## 8. 任务 D：`this` context leak 修复

### 8.1 问题

Phase 9 仍有 static/instance context 泄漏 `this`，导致 undefined variable。

### 8.2 规则

```text
- instance method 内：this 是第一个 integer 参数。
- static method 内：this 不存在。
- static method 中 this.staticField 应重写为 generated static field。
- static method 中 this.instanceField 是非法，必须 diagnostic。
- lambda inside instance method：如果使用 this，必须捕获或传递。
  当前项目不支持 capturing closure 时，不要静默生成裸 this。
```

### 8.3 实现任务

```text
D1. 给 LoweringContext 添加 context kind：Function / InstanceMethod / StaticMethod / LambdaFromInstance / LambdaFromStatic。
D2. static method 内检测裸 this。
D3. static method 内 `this.staticField` -> generated static field。
D4. lambda 内裸 this 若不是安全传参，记录 capturing lambda / reject。
D5. syntax-lite 增加裸 this residue 检查，但不要误报函数参数 this。
D6. fixture：static method this.staticField / this.instanceField / lambda with this。
```

---

## 9. 任务 E：return mismatch / missing return 修复

### 9.1 问题

Phase 9 仍有：

```text
returnMissingValue: 50
returnMismatch / missing return diagnostics
```

### 9.2 规则

JASS 函数：

```text
returns nothing -> 不能 return value
returns T       -> return 必须带 value
```

如果函数声明 returns T，但源代码存在路径不 return：

```text
- 优先按源语义修复 lowering 造成的遗漏。
- 不要随便加默认 return 0/null，除非能证明是 JassHelper 风格或源逻辑明确允许。
```

### 9.3 实现任务

```text
E1. validation report 中按 generated function 分类 return errors。
E2. 分离：returns nothing with value / returns value missing value / all-path missing return。
E3. 修复由 method-chain lowering 造成的 return 语句破坏。
E4. 修复 function-interface evaluate/execute 误用造成的 return mismatch。
E5. 对确实需要 fallback 的 generated wrapper：按 return type 生成安全默认值。
    - integer/real: 0 / 0.0
    - boolean: false
    - string: null
    - handle-like: null
    - struct/function-interface integer: 0
E6. 仅对 compiler-generated wrappers 允许默认 return；源函数不要自动补，除非 JassHelper 参考行为确认。
```

---

## 10. 任务 F：unresolved environment/source symbols provenance

### 10.1 问题

Phase 9 仍有：

```text
yd_* / HASH_ABILITY / HASH_TIMER / uiHT 等符号未解析
```

### 10.2 禁止策略

不要直接：

```jass
integer HASH_ABILITY = 0
integer HASH_TIMER = 0
```

除非有明确来源证明这些本来就是 compiler-generated 或 environment-provided constants。

### 10.3 实现任务

```text
F1. 添加 unresolved symbol provenance report。
F2. 对每个 unresolved symbol 记录：
    - first generated line
    - current function
    - source location if available
    - surrounding expression
    - 是否出现在 input.j
    - 是否出现在 output_jasshelper.j
    - 是否出现在 common.j / blizzard.j / jasshelper env
F3. 对 output_jasshelper.j 中存在的 helper/global，研究 JassHelper 如何生成。
F4. 对 input.j 中存在但输出没有的符号，查 private/public rewrite 或 static-if/module/lambda lowering。
F5. 对环境符号建立 allowlist，但只用于 validation 分类，不自动生成声明。
F6. 只有 provenance 明确时才新增生成逻辑。
```

### 10.4 验收

```text
- unresolvedEnvironmentSymbol 数量下降。
- 每个仍存在的 unresolved symbol 都有 provenance 类别。
- 不出现 dummy declaration。
```

---

## 11. 任务 G：true forward cycle strategy

### 11.1 当前已知 cycles

```text
1. SyncBus onInit <-> onDataSync
2. CombineSession buildSelector <-> runStage
3. MopUpItem lambda <-> MopUpItemCreate
```

### 11.2 原则

PJASS 不支持前向函数调用。简单排序无法解决循环。

可选策略：

```text
策略 A：function interface / trigger wrapper bridge
策略 B：ExecuteFunc bridge，仅限 takes nothing returns nothing
策略 C：拆 wrapper：A calls A_impl，B calls B_impl，根据签名重排
策略 D：保持错误并分类，等待语义更完整后处理
```

不要对带参数/返回值函数直接 ExecuteFunc，因为会改变语义。

### 11.3 实现任务

```text
G1. cycle detector 输出 cycle graph。
G2. 对每个 cycle 记录 function signature。
G3. 如果全 cycle 都是 takes nothing returns nothing，可用 ExecuteFunc bridge。
G4. 如果有参数或返回值，不自动 bridge；输出 report。
G5. 添加 fixture：simple no-arg cycle bridge / value-returning cycle rejected。
```

---

## 12. 任务 H：低风险性能治理

### 12.1 本阶段允许做的优化

```text
H1. 继续保留 pass-level timing。
H2. 对 emitFunctions / emitStructSupport / lowerLambdas / sanitizeOutput 加二级 timing。
H3. 把重复的 regex_replace 改为单次扫描函数。
H4. 给 struct/function/interface lookup 建 unordered_map 缓存。
H5. 对 repeated output full scan 做结果缓存。
H6. 避免每个函数 body 都遍历所有 structs/functions。
H7. CodeWriter 预估 reserve 容量。
H8. 避免大字符串反复拼接，使用 vector chunks 或 CodeWriter buffer。
```

### 12.2 本阶段禁止做的优化

```text
- 不要为了性能删除 validation report。
- 不要去掉 syntax-lite。
- 不要重写整个 codegen architecture。
- 不要把 correctness checks 延后到不可见状态。
```

### 12.3 性能目标

```text
最低：不比 Phase 9 更慢。
良好：total <= 80s。
优秀：total <= 45s。
```

如果 PJASS 通过但性能仍慢，可以接受；后续再开性能专项。

---

## 13. Validation report 增强

Phase 10 validation report 应新增：

```json
{
  "pjass": {
    "groupedCount": 0,
    "groups": {
      "methodChainReceiverResidue": 0,
      "callbackCodeSignatureMismatch": 0,
      "returnMissingValue": 0,
      "unresolvedEnvironmentSymbol": 0,
      "trueForwardCycle": 0
    }
  },
  "methodChains": {
    "lowered": 0,
    "residual": 0,
    "tempLocalsGenerated": 0
  },
  "callbackAdapters": {
    "generated": 0,
    "rejected": 0,
    "unsafeRawCode": 0
  },
  "unresolvedSymbols": [
    {
      "name": "HASH_ABILITY",
      "category": "environment|source|rewrite-gap|unknown",
      "firstLine": 0,
      "inInput": true,
      "inJassHelperOutput": true
    }
  ],
  "performance": {
    "emitFunctions": 0,
    "emitStructSupport": 0,
    "lowerLambdas": 0,
    "sanitizeOutput": 0
  }
}
```

---

## 14. docs/phase10_status.md 要求

Codex 完成后必须新增：

```text
docs/phase10_status.md
```

内容包括：

```text
1. Implemented
2. Validation commands
3. Syntax-lite result
4. Init validation result
5. PJASS before/after table: Phase 9 vs Phase 10
6. Remaining blockers
7. Performance before/after table
8. Whether PJASS passed
9. If PJASS did not pass: exact grouped count and next phase recommendation
```

README 也要更新 Phase 10 概述。

---

## 15. 推荐执行顺序

```text
Step 1: 跑 Phase 9 baseline，确认复现 889 grouped count 左右。
Step 2: 修 method-chain continuation。
Step 3: 修 nested generated index expression。
Step 4: 修 callback/code signature adapter。
Step 5: 修 this context leaks。
Step 6: 修 return mismatch / missing return。
Step 7: 做 unresolved symbol provenance，不乱补声明。
Step 8: 处理 safe true forward cycles。
Step 9: 做低风险性能优化。
Step 10: 更新 docs/phase10_status.md 和 README。
```

每完成一个 step 都运行：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\input.j -o build\input.phase10.out.j --emit-stats build\input.phase10.codegen.stats.json --emit-validation-report build\input.phase10.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

每两个 step 至少运行一次 PJASS。

---

## 16. 最终验收命令

```bat
cmake --build build
ctest --test-dir build --output-on-failure

build\vjassc.exe samples\input.j ^
  -o build\input.phase10.out.j ^
  --emit-stats build\input.phase10.codegen.stats.json ^
  --emit-validation-report build\input.phase10.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite

build\vjassc.exe samples\input.j ^
  -o build\input.phase10.pjass.out.j ^
  --emit-stats build\input.phase10.pjass.stats.json ^
  --emit-validation-report build\input.phase10.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

---

## 17. 成功判定

### PJASS 通过时

```text
- docs/phase10_status.md 写明 pjass.ok == true。
- README 写明 Phase 10 首次 PJASS pass。
- 下一阶段进入 runtime validation + behavior comparison + performance optimization。
```

### PJASS 未通过时

必须满足：

```text
- grouped PJASS count <= 200，或至少相比 Phase 9 再下降 70%。
- 剩余每类 blocker 有 examples 和 current function。
- 没有新增 duplicate declarations。
- syntax-lite green。
- init validation green。
- docs/phase10_status.md 给出 Phase 11 的精确建议。
```

---

## 18. 给 Codex 的最终指令摘要

```text
你正在执行 Phase 10。最高目标是让真实 samples/input.j 生成的 JASS 通过 PJASS。
如果无法安全通过，必须把 grouped PJASS count 压到 <= 200 或至少相比 Phase 9 再下降 70%，并输出清晰 blocker 报告。

优先修：
1. struct-returning method-chain continuation
2. nested generated array/index expressions
3. raw code callback signature adapters
4. this context leaks
5. return mismatch / missing return
6. unresolved environment/source symbol provenance
7. true forward cycles
8. low-risk performance improvements

禁止 dummy declarations，禁止删除源逻辑绕过 PJASS，禁止大规模性能重构。
每类修复后运行 tests、syntax-lite；每两类修复至少运行一次 PJASS。
最终更新 docs/phase10_status.md 和 README。
```
