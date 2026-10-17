---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase11_codex_task_plan.md"
ref: HEAD
---

# JassChanger Phase 11 Codex 实施计划

> 目标读者：Codex / 继续开发 JassChanger 的自动化编码代理。  
> 项目：`Crainax/JassChanger`  
> 当前阶段：Phase 10 已完成。Phase 11 的目标是 **PJASS 通过优先**，同时开始 **低风险性能治理**，但不做大规模架构重写。

---

## 0. 当前状态摘要

Phase 10 已经能对真实 `samples/input.j` 生成完整 plain-JASS candidate，并且：

```text
syntax-lite: pass
init validation: pass
duplicateFunctionNames: 0
duplicateGlobalNames: 0
duplicateNativeNames: 0
localOrder: 0
methodChainCallResultResidues: 0
indexedStructMemberResidues: 0
inlineZincControlResidues: 0
```

Phase 10 的 PJASS grouped count 已从 Phase 9 的 `889` 降到 `360`，但 PJASS 仍未通过。

Phase 10 剩余主要错误组：

```text
undefinedVariable: 150
other: 99
returnMissingValue: 48
callbackCodeSignatureMismatch: 30
unresolvedEnvironmentSymbol: 9
undefinedFunction: 8
expectedEndfunction: 1
forwardFunctionReference: 3
```

Phase 10 运行耗时仍非常高：

```text
total: 111511 ms
codegen: 102767 ms
syntaxLite: 7427 ms
pjass: 352 ms
```

最大 codegen 热点：

```text
emitFunctions: 90521 ms
emitStructSupport: 69413 ms
finalOutputValidationPrep: 11854 ms
sanitizeOutput: 8861 ms
lowerLambdas: 8978 ms
functionOrdering: 2993 ms
```

注意：PJASS 自身只需要约几百毫秒，慢点主要在 full codegen 与 full-output scan。

---

## 1. Phase 11 总目标

Phase 11 不再以 “grouped PJASS count <= 500” 作为最低目标，因为 Phase 10 已经达成 `360`。Phase 11 的核心目标如下：

### 1.1 最高目标

```text
真实 samples/input.j 生成的 output 能通过 PJASS。
```

即：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase11.pjass.out.j ^
  --emit-stats build\input.phase11.pjass.stats.json ^
  --emit-validation-report build\input.phase11.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

### 1.2 若 PJASS 未能通过，Phase 11 最低交付标准

如果在安全边界内无法让 PJASS 通过，最低交付标准改为：

```text
grouped PJASS count <= 100
syntaxError <= 5
expectedEndfunction == 0
undefinedFunction <= 3
undefinedVariable <= 40
callbackCodeSignatureMismatch <= 10
unresolvedEnvironmentSymbol 全部有 provenance 分类
returnMissingValue 只剩用户源函数或明确需行为确认的函数
localOrder == 0
duplicate function/global/native == 0
syntax-lite 仍然通过
init validation 仍然通过
```

### 1.3 不允许的“假通过”

禁止为了降低 PJASS 错误而做以下事情：

```text
- 不允许随便添加 dummy global / dummy function / dummy native。
- 不允许给 HASH_* / yd_* / YDHT 等环境符号随便补 0/null。
- 不允许给所有 source function 自动补默认 return。
- 不允许删除源逻辑。
- 不允许为了让 PJASS 安静而把错误代码注释掉。
- 不允许牺牲 syntax-lite / init validation / duplicate-name checks。
```

---

## 2. 解决 Codex 低效迭代问题：改成“批量修复 + 分层验证”

当前问题：Codex 以前按“最低合格：grouped count <= 500”执行，导致每修一点就跑一次完整 PJASS。现在一次完整 codegen + validation 约 110 秒，若每次只减少几个错误，会极大浪费时间。

Phase 11 必须使用下面的迭代策略。

### 2.1 不再把 `<=500` 当作目标

Phase 10 已经是 `360`。Phase 11 的最低目标是 `<=100`，最高目标是 PJASS 通过。

在 prompt / task 执行中明确告诉 Codex：

```text
Do not stop at <=500. Phase 10 already achieved 360.
Your primary goal is PJASS pass. If not possible safely, reduce grouped count to <=100 and provide a classified residual report.
```

### 2.2 每一轮先离线分析，不立刻全量跑 PJASS

Codex 应优先读取上一轮已有文件：

```text
build/input.phase10.pjass.validation.json
build/input.phase10.pjass.validation.pjass.stdout.txt
build/input.phase10.pjass.validation.pjass.stderr.txt
build/input.phase10.pjass.out.j
```

如果这些文件不存在，再跑一次 full validation。

不要在没有代码改动前重复跑 full PJASS。

### 2.3 每类错误要“批处理”，不要修一个跑一次

每次选择一个 error group，先从 validation report / pjass stdout 提取 top 10~30 个例子，归纳为一个 lowering bug，然后一次性修这个 bug 的通用逻辑。

建议批处理顺序：

```text
Batch A: undefinedVariable + unresolvedEnvironmentSymbol provenance
Batch B: integer/boolean conversion mismatch in `other`
Batch C: callbackCodeSignatureMismatch raw-code adapter
Batch D: compiler-generated wrapper missing returns
Batch E: undefinedFunction + true forward cycles
Batch F: remaining syntaxError / expectedEndfunction edge cases
```

每个 batch 的流程：

```text
1. 从现有 PJASS log 收集同类 examples。
2. 写 focused fixture，覆盖该类模式。
3. 修 lowering / symbol resolution。
4. 跑 cmake build + ctest。
5. 跑小 fixture output / syntax-lite。
6. 累积到一批修复后，再跑一次 real-sample full PJASS。
```

### 2.4 Full PJASS checkpoint 规则

为避免“改一点跑一次”，Phase 11 规定：

```text
- 每个 batch 最多跑 2 次 full PJASS。
- 除非预计能减少 >=20 grouped errors，否则不要为了单个小修跑 full PJASS。
- 如果某个 batch 两次 full PJASS 后下降 < 10%，停止该 batch，转入下一类。
- 如果 syntax-lite 失败，可以立即修并跑 syntax-lite；不必立刻跑 PJASS。
- 如果 build/ctest 失败，必须先修 build/ctest，不跑 PJASS。
```

### 2.5 新增快速验证/分析工具，减少全量等待

Codex 应优先实现或增强这些工具，作为 Phase 11 的前置任务：

#### 2.5.1 `--analyze-pjass-log`

新增 CLI：

```bat
build\vjassc.exe --analyze-pjass-log build\input.phase10.pjass.validation.pjass.stdout.txt ^
  --emit-validation-report build\input.phase11.triage.json
```

用途：只解析已有 PJASS stdout，不重新 codegen，不重新跑 PJASS。

输出内容：

```json
{
  "pjassLogAnalysis": {
    "groupedCount": 360,
    "groups": {
      "undefinedVariable": {
        "count": 150,
        "topSymbols": [...],
        "examples": [...]
      }
    }
  }
}
```

#### 2.5.2 `--validate-existing-output`

新增 CLI：

```bat
build\vjassc.exe --validate-existing-output build\input.phase11.out.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --emit-validation-report build\input.phase11.existing.validation.json
```

用途：当只是调整 validation / PJASS 分类逻辑时，避免重新编译 input.j。

#### 2.5.3 `--emit-pjass-examples N`

增强 validation report，支持 top N examples：

```bat
--emit-pjass-examples 30
```

输出每个 group 的：

```text
- PJASS 原始错误行
- generated output line number
- current function name
- generated output excerpt
- suspected source location if available
- symbol name if available
- proposed provenance bucket
```

### 2.6 Codex 执行停止条件

Codex 不应该无边界“修到永远”。Phase 11 使用 bounded retry：

```text
优先停止条件：
- PJASS pass。

可接受停止条件：
- grouped count <= 100，并且剩余错误都被分类、带 examples、带下一步建议。

异常停止条件：
- 某一类错误连续两轮 full PJASS 无明显下降。
- 修复会要求 dummy declaration / unsafe default return / 删除源逻辑。
- 性能恶化超过 30% 且没有 correctness 改善。
```

---

## 3. Phase 11 任务 A：剩余 undefinedVariable / environment symbol provenance

### 3.1 当前问题

Phase 10 最大组是：

```text
undefinedVariable: 150
unresolvedEnvironmentSymbol: 9
```

已知例子：

```text
origin
keyId
YDHT
yd_NullTempGroup
yd_MapMaxX
yd_MapMinX
HASH_ABILITY
HASH_TIMER
```

### 3.2 目标

不要直接补 dummy。必须给每个高频符号建立 provenance 分类：

```text
SourceDeclaredButNotEmitted
PrivatePublicRewriteMiss
MacroExpansionMiss
StaticIfPrunedIncorrectly
EnvironmentProvidedByYDWE
CommonBlizzardProvided
GeneratedHelperMiss
TypoOrUnsupported
```

### 3.3 实现要求

1. 扩展 symbol collection，记录所有源声明：

```text
- globals
- public/private globals
- Zinc public/private block globals
- struct static fields
- generated helper globals
- imported/preprocessed lines
```

2. 在 validation report 中新增：

```json
"undefinedVariableProvenance": {
  "origin": {
    "count": 12,
    "bucket": "PrivatePublicRewriteMiss",
    "examples": [...]
  },
  "yd_MapMaxX": {
    "count": 3,
    "bucket": "EnvironmentProvidedByYDWE",
    "action": "requires environment policy"
  }
}
```

3. 修复明确属于 compiler bug 的类型：

```text
- 已在源中声明但未 emit。
- 源中 private/public rewrite 漏掉。
- lambda / wrapper / method chain 中没有带 container context。
- struct static field / generated helper field 未映射。
```

4. 对 YDWE / yd_ 环境符号只分类，不乱生成，除非能证明 JassHelper 输出中也生成同名声明。

### 3.4 验收

```text
undefinedVariable <= 40
unresolvedEnvironmentSymbol 全部有 provenance
不得出现 dummy HASH_* / yd_* / YDHT 声明
```

---

## 4. Phase 11 任务 B：integer/boolean conversion mismatch

### 4.1 当前问题

Phase 10 `other: 99` 主要包含：

```text
- integer/boolean mismatch
- code/function-interface conversion gaps
- 部分语义转换错误
```

### 4.2 目标

把 `other` 拆到更细，不再让它作为大黑箱。

新增分类：

```text
booleanIntegerMismatch
codeInterfaceConversion
invalidReturnExpression
invalidConditionExpression
invalidAssignmentConversion
unknownOther
```

### 4.3 修复策略

优先修 compiler-generated 的明显问题：

```text
- if 条件里出现 integer id，但 PJASS 要 boolean。
- boolean context 中 function-interface integer id 没有比较。
- execute/evaluate wrapper 返回值错放。
- Condition/Filter adapter 需要 boolean return。
```

可能的安全转换：

```jass
if someInteger then
```

不能直接输出。需要根据语义改为：

```jass
if someInteger != 0 then
```

但只对明确是 compiler-generated id / function-interface id / struct id 的表达式做此转换，不要对任意用户表达式乱改。

### 4.4 验收

```text
other <= 40
booleanIntegerMismatch 独立统计
invalidComparison 保持 0
syntax-lite 继续 pass
```

---

## 5. Phase 11 任务 C：raw `code` callback adapter

### 5.1 当前问题

Phase 10 仍有：

```text
callbackCodeSignatureMismatch: 30
```

典型问题：带参数 lambda 被当成 JASS `code` 使用。JASS 的 `code` 只能引用：

```jass
function X takes nothing returns nothing
```

不能直接：

```jass
function vjlambda__N takes integer x returns nothing
```

### 5.2 目标

只在上下文明确时生成安全 adapter。

### 5.3 适配规则

#### 5.3.1 function-interface 调度上下文

如果 lambda 实际作为 function-interface target 使用，不应按 raw `code` 处理，而应走 interface wrapper 参数 globals。

修复方向：

```text
- 检查 expected type 是否是 function interface。
- 如果是 interface，不输出 `function lambda` raw code。
- 注册 interface target id。
```

#### 5.3.2 TriggerAddAction / TimerStart / ForGroup / Enum callbacks

只有当 lambda 无参、returns nothing 时，才可以直接作为 raw code。

```zinc
function() { ... }
```

可降级为：

```jass
function vjlambda__N takes nothing returns nothing
    ...
endfunction
```

#### 5.3.3 带参数 lambda

如果调用点是 raw `code`，但 lambda 带参数：

```text
- 不要直接输出 raw function。
- 尝试识别是否有事件上下文来源。
- 如果能明确映射参数来源，生成 adapter。
- 如果不能明确映射，保留 PJASS 分类为 callbackCodeSignatureMismatch，并报告 source location。
```

禁止无依据地用全局临时变量填参数。

### 5.4 验收

```text
callbackCodeSignatureMismatch <= 10
如果仍有剩余，每个都必须有 source/callsite/currentFunction/examples
不得破坏 functionInterfaceWrappers
```

---

## 6. Phase 11 任务 D：returnMissingValue / missing return

### 6.1 当前问题

Phase 10：

```text
returnMissingValue: 48
```

### 6.2 原则

不要给用户源函数乱补默认 return。

优先处理：

```text
- compiler-generated wrappers
- function-interface evaluate wrappers
- lambda wrappers
- code adapters
- struct support helpers
```

### 6.3 安全默认值

仅对 compiler-generated function，如果所有路径缺失返回，可按返回类型补：

```text
integer -> 0
real -> 0.0
boolean -> false
string -> ""
handle/unit/timer/trigger/... -> null
struct/function-interface integer id -> 0
```

对 source function：

```text
- 只分类。
- 在 report 中标记 SourceMissingReturn。
- 不自动补，除非 JassHelper 参考输出明确也补了同等默认 return。
```

### 6.4 验收

```text
returnMissingValue <= 15
compilerGeneratedMissingReturn == 0
sourceMissingReturn 有明确 examples
```

---

## 7. Phase 11 任务 E：undefinedFunction 与 true forward cycles

### 7.1 当前问题

Phase 10：

```text
undefinedFunction: 8
forwardFunctionReference: 3
```

三组 true cycle：

```text
SyncBus onInit <-> onDataSync
CombineSession buildSelector <-> runStage
MopUpItem lambda <-> MopUpItemCreate
```

### 7.2 普通 undefinedFunction

先判断是否属于：

```text
- function emission order miss
- public/private rename miss
- method-chain lowering miss
- generated helper missing
- function-interface target adapter missing
```

能通过 reorder / rename / helper emission 修复的直接修。

### 7.3 true cycle 策略

JASS 不支持普通函数前向调用。对 true cycle 需要特殊策略。

#### 7.3.1 安全 ExecuteFunc bridge

仅对：

```text
函数签名 takes nothing returns nothing
```

可考虑：

```jass
call ExecuteFunc("Target")
```

这适合无参无返回 init / callback cycle。

#### 7.3.2 非无参无返回 cycle

不要生成错误 bridge。保留分类：

```text
UnbridgeableForwardCycle
```

并在 report 中列出：

```text
caller
callee
signature
source location
generated location
```

### 7.4 验收

```text
undefinedFunction <= 3
forwardFunctionReference <= 3 且全部分类为 true cycle 或已 bridge
```

---

## 8. Phase 11 任务 F：低风险性能治理

### 8.1 是否开始性能优化？

Phase 11 可以开始 **低风险性能治理**，但不要做大规模 codegen 架构重写。

优先做这些：

```text
- pass-level timing 保持输出。
- 为 emitFunctions / emitStructSupport 添加更细 timing。
- 缓存 findStruct/findMethod/findField/findFunctionInterface/findFunctionInfo。
- 避免每个函数 lowering 时全量遍历所有 structs/functions/interfaces。
- 将高频 regex_replace 改为手写 scanner 或预编译 regex。
- 合并 sanitizeOutput / syntax-lite 中重复的 full-output scan。
- 对 generated function dependency graph 做一次性构建，不要每轮重复扫描字符串。
```

### 8.2 禁止的大优化

```text
- 不要重写整个 parser。
- 不要重写整个 AST/IR。
- 不要大拆 Phase1Codegen 类作为 Phase 11 主目标。
- 不要牺牲 PJASS report 质量。
- 不要移除 validation checks。
```

### 8.3 性能目标

Phase 11 最低目标：

```text
codegen <= 80000 ms
syntaxLite <= 5000 ms
total <= 90000 ms
```

良好目标：

```text
codegen <= 60000 ms
total <= 70000 ms
```

注意：PJASS 通过优先于性能数字。

---

## 9. Phase 11 任务 G：validation report 改进

### 9.1 必须新增/保留字段

```json
{
  "phase": 11,
  "pjass": {
    "ok": false,
    "groupedCount": 100,
    "groups": {...},
    "examples": {...}
  },
  "provenance": {
    "undefinedVariables": {...},
    "environmentSymbols": {...}
  },
  "callbackAdapters": {
    "generated": 0,
    "rejected": 0,
    "unknownContext": 0
  },
  "forwardCycles": [...],
  "performance": {
    "passes": {...},
    "hotspots": [...]
  }
}
```

### 9.2 Report 必须能回答的问题

```text
- 还剩多少 PJASS grouped errors？
- 剩余 top 10 symbol 是什么？
- 哪些是 compiler bug，哪些是 environment dependency？
- 哪些 return 可以安全补，哪些不能？
- 哪些 callback 可以生成 adapter，哪些上下文未知？
- 性能最慢的 pass 是什么？
```

---

## 10. 建议执行顺序

Codex 应按以下顺序执行，不要跳到性能大重构：

```text
Step 1: 读取 Phase 10 validation / PJASS stdout，生成 Phase 11 triage baseline。
Step 2: 实现 --analyze-pjass-log 或等价的 offline log analyzer。
Step 3: 处理 undefinedVariable provenance，并修明确的 rewrite/emission 漏洞。
Step 4: 处理 integer/boolean conversion mismatch。
Step 5: 处理 raw code callback adapter，保守生成，只在上下文明确时生成。
Step 6: 处理 compiler-generated wrapper missing return。
Step 7: 处理 undefinedFunction / true forward cycles。
Step 8: 做低风险性能治理。
Step 9: 跑一次完整 real-sample PJASS checkpoint。
Step 10: 如果 grouped count > 100，针对最大 group 再做一轮 batch fix。
Step 11: 更新 docs/phase11_status.md 和 README。
```

---

## 11. 必须新增的 fixtures

新增至少这些 fixture：

```text
tests/fixtures/phase11_undefined_private_global_rewrite.in.j
tests/fixtures/phase11_boolean_integer_interface_id.in.j
tests/fixtures/phase11_code_callback_no_arg_ok.in.j
tests/fixtures/phase11_code_callback_param_rejected_or_adapted.in.j
tests/fixtures/phase11_generated_wrapper_return_default.in.j
tests/fixtures/phase11_source_missing_return_not_patched.in.j
tests/fixtures/phase11_execute_func_bridge_noarg_cycle.in.j
tests/fixtures/phase11_environment_symbol_report.in.j
```

每个 fixture 都要包含 `.expected.j` 或 negative expected diagnostics。

---

## 12. README 与 docs 更新要求

新增：

```text
docs/phase11_status.md
```

内容必须包括：

```text
- Implemented
- Validation Commands
- Syntax And Init
- PJASS Delta: Phase 10 -> Phase 11
- Remaining Blockers
- Provenance Summary
- Callback Adapter Summary
- Forward Cycle Summary
- Performance Delta
- Next Phase Recommendation
```

README 更新 Phase 11 段落：

```text
Phase 11 focuses on PJASS pass convergence and low-risk performance tuning. It adds provenance reporting for unresolved variables/environment symbols, handles safe callback/code adapters, fixes generated wrapper return gaps, classifies forward cycles, and reduces full validation time without broad codegen rewrites.
```

---

## 13. Phase 11 验收清单

### 必须通过

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

```bat
build\vjassc.exe samples\input.j -o build\input.phase11.out.j --emit-stats build\input.phase11.codegen.stats.json --emit-validation-report build\input.phase11.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

```bat
build\vjassc.exe samples\input.j -o build\input.phase11.pjass.out.j --emit-stats build\input.phase11.pjass.stats.json --emit-validation-report build\input.phase11.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

### 成功标准

优秀：

```text
PJASS ok == true
```

合格：

```text
grouped PJASS count <= 100
syntaxError <= 5
expectedEndfunction == 0
undefinedFunction <= 3
undefinedVariable <= 40
callbackCodeSignatureMismatch <= 10
localOrder == 0
syntaxLite.ok == true
init validation issues == []
duplicate names == 0
```

性能合格：

```text
codegen <= 80000 ms
total <= 90000 ms
```

---

## 14. 给 Codex 的关键提醒

```text
- 不要再以 <=500 为目标。Phase 10 已经是 360。
- 不要每改一个小点就跑 full PJASS。
- 先离线分析 PJASS log，再按 group 批量修复。
- 每个 batch 写 focused fixture。
- full PJASS 只在 batch checkpoint 跑。
- 不要 dummy declaration。
- 不要给源函数乱补 return。
- 不要大规模性能重构。
- PJASS pass 是最高目标；如果没过，必须把剩余错误压到 <=100 且全部分类清楚。
```
