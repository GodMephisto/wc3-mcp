---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase13_codex_task_plan.md"
ref: HEAD
---

# Phase 13 Codex 实施计划：PJASS Pass Finalization + 稳定性/性能收敛

> 项目：`Crainax/JassChanger` / `vjassc`  
> 阶段目标：基于 Phase 12 最新状态，将真实 `samples/input.j` 的完整生成物推进到 **PJASS 通过**。如果存在外部环境符号或无法安全自动适配的 raw `code` callback，必须给出可验证、可配置、可文档化的处理方式，禁止通过伪声明或静默改语义来“假通过”。

---

## 0. 当前状态基线

Phase 12 已经完成了第一轮 PJASS 收敛和性能治理：

```text
Phase 11 grouped PJASS count: 97
Phase 12 grouped PJASS count: 21
Phase 12 full validation total: 57559 ms
syntax-lite: green
init validation: green
duplicate function/global/native: 0
returnMissingValue: 0
undefinedVariable: 0
statement-shape PJASS buckets: 0
```

Phase 12 仍未通过 PJASS，剩余 blocker 集合非常小：

```text
1. callbackCodeSignatureMismatch: 15
   15 个唯一 generated call sites 仍然把带参数 lambda 传给 raw code 位置。

2. forwardFunctionReference: 5
   s__SyncBus_syncBus_onInit -> s__SyncBus_syncBus_onDataSync
   s__Pet_CombineSession_buildSelector -> s__Pet_CombineSession_runStage
   vjlambda__627 -> MopUpItem_MopUpItemCreate
   DadiCarMovingHuanying -> CreateDadiCar
   vjlambda__677 -> DadiCarMoving

3. undefinedFunction: 1
   InitTrig_japi 是外部 map-init / environment symbol，不是当前 generated source function。
```

Phase 13 的重点是：**不要继续泛泛修所有 lowering，也不要开始大规模架构重写；只收敛这 3 类问题，并保持性能不倒退。**

---

## 1. Phase 13 总目标

### 1.1 最高目标

```text
PJASS pass for samples/input.j
```

即命令：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase13.pjass.out.j ^
  --emit-stats build\input.phase13.pjass.stats.json ^
  --emit-validation-report build\input.phase13.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --emit-pjass-examples 50
```

应满足：

```json
{
  "pjass.ok": true,
  "syntaxLite.ok": true,
  "syntaxLite.issueCount": 0,
  "init.issues": 0,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0
}
```

### 1.2 最低合格目标

如果确实存在外部环境依赖导致 PJASS 不能在当前 `common.j/blizzard.j/generated.j` 三文件模式下通过，则最低合格目标是：

```text
grouped PJASS count <= 5
所有剩余 grouped errors 必须有明确 provenance、fix policy、是否外部环境依赖的判断。
full validation total <= 45s，不能比 Phase 12 的 57559 ms 更慢。
```

### 1.3 性能目标

Phase 13 不做大规模性能重构，但要继续做低风险优化：

```text
最低：full validation <= 45s
良好：full validation <= 35s
优秀：PJASS pass 且 full validation <= 30s
```

---

## 2. 重要约束

### 2.1 禁止伪修复

绝对禁止：

```text
- 为了 PJASS pass 给 InitTrig_japi 生成空函数，除非确认这是 JassHelper/地图环境等价行为。
- 给 HASH/YD/JAPI 环境符号乱补 0/null。
- 给用户源函数无脑补 return。
- 把带参数 lambda 强行当 raw code 使用。
- 删除源逻辑以消除 PJASS error。
- 隐藏 PJASS 错误分类，只报告 ok。
```

### 2.2 优先保持语义等价

Phase 13 的每个修复都要回答：

```text
这个修复是否只是让 PJASS 通过？
它是否改变游戏运行行为？
它是否接近 JassHelper 行为？
它是否只作用于 compiler-generated wrapper，而不是用户源逻辑？
```

若无法确认语义安全，宁可保留为 classified blocker，不要假修。

### 2.3 降低 Codex 迭代成本

不要每改一小处就全量 `input.j + PJASS`。执行顺序必须是：

```text
1. 先用 --analyze-pjass-log 分析上一轮 PJASS stdout。
2. 从 validation report 中抽取具体 examples。
3. 为同一类问题写 focused fixture。
4. 跑 focused fixture / golden tests。
5. 只在一个 batch 完成后跑 full PJASS。
```

---

## 3. 必须先做的准备工作

### 3.1 更新 Phase 13 文档骨架

新增：

```text
docs/phase13_status.md
```

内容至少包括：

```text
- Phase 12 baseline
- Phase 13 implemented changes
- validation commands
- PJASS result
- remaining blockers or pass result
- performance table Phase 12 vs Phase 13
- final JassHelper replacement readiness statement
```

### 3.2 保留 Phase 12 基线

不要覆盖 Phase 12 产物命名。Phase 13 使用：

```text
build/input.phase13.out.j
build/input.phase13.codegen.stats.json
build/input.phase13.validation.json
build/input.phase13.pjass.out.j
build/input.phase13.pjass.stats.json
build/input.phase13.pjass.validation.json
build/input.phase13.pjass.validation.pjass.stdout.txt
build/input.phase13.pjass.validation.pjass.stderr.txt
```

### 3.3 强制保留 offline analysis workflow

Phase 13 需要优先使用：

```bat
build\vjassc.exe ^
  --analyze-pjass-log build\input.phase12.pjass.validation.pjass.stdout.txt ^
  --emit-validation-report build\input.phase13.triage.from-phase12.json ^
  --emit-pjass-examples 50
```

并使用：

```bat
build\vjassc.exe ^
  --validate-existing-output build\input.phase12.pjass.out.j ^
  --emit-validation-report build\input.phase13.existing-output.validation.json ^
  --check-output-syntax-lite
```

确认 existing-output validation 工具仍可用。

---

## 4. 任务 A：Signature-aware forward bridge generation

### 4.1 背景

Phase 12 剩余 5 个 forward references：

```text
s__SyncBus_syncBus_onInit -> s__SyncBus_syncBus_onDataSync
s__Pet_CombineSession_buildSelector -> s__Pet_CombineSession_runStage
vjlambda__627 -> MopUpItem_MopUpItemCreate
DadiCarMovingHuanying -> CreateDadiCar
vjlambda__677 -> DadiCarMoving
```

JASS/PJASS 不允许函数调用尚未定义的函数。单纯排序无法解决真正循环引用。

### 4.2 目标

实现一个**签名感知 bridge 策略**，只对安全情况生成桥接函数。

### 4.3 处理策略

#### 情况 1：callee 是 `takes nothing returns nothing`

可以用 `ExecuteFunc` bridge：

```jass
function bridge__Target takes nothing returns nothing
    call ExecuteFunc("Target")
endfunction
```

调用点可改为：

```jass
call bridge__Target()
```

或在 caller 内直接：

```jass
call ExecuteFunc("Target")
```

但必须确认：

```text
- target 无参数
- target returns nothing
- 调用点不依赖返回值
```

#### 情况 2：callee 有返回值或参数

不能用 ExecuteFunc 直接桥接。需要选择：

```text
A. dependency-aware split 是否能解决？
B. 是否能通过 wrapper + global temp 参数/返回值安全桥接？
C. 是否必须保留为 classified blocker？
```

不要生成错误的 dummy wrapper。

### 4.4 实现建议

新增或扩展：

```text
src/sema/FunctionDependencyGraph.*
src/codegen/ForwardBridgePlanner.*
```

也可以先在当前 codegen 内实现，但要用清晰函数拆分：

```cpp
struct ForwardReference {
    std::string caller;
    std::string callee;
    FunctionSignature callerSig;
    FunctionSignature calleeSig;
    SourceLocation loc;
    bool isCycle;
};

struct ForwardBridgeDecision {
    enum class Kind {
        None,
        ReorderOnly,
        ExecuteFuncBridge,
        GlobalTempBridge,
        UnsafeKeepClassified
    };
};
```

### 4.5 必须新增 fixture

新增：

```text
tests/fixtures/phase13_forward_bridge_noarg.in.j
tests/fixtures/phase13_forward_bridge_noarg.expected.j
tests/fixtures/phase13_forward_cycle_return_unsafe.in.j
tests/fixtures/phase13_forward_cycle_return_unsafe.expected.diagnostics
```

覆盖：

```text
- A -> B -> A，均 nothing/nothing，可 bridge。
- A -> B -> A，B returns integer，不可乱 bridge。
- lambda -> function forward reference。
- struct onInit -> method forward reference。
```

### 4.6 验收

```text
forwardFunctionReference: 5 -> 0，或仅剩被明确判定 unsafe 的条目。
如果 PJASS 仍未过，validation report 必须列出每个剩余 forward reference 的签名和拒绝原因。
```

---

## 5. 任务 B：InitTrig_japi 环境符号策略

### 5.1 背景

Phase 12 剩余：

```text
undefinedFunction: InitTrig_japi
```

这是外部 map-init / environment symbol，不是当前 generated source function。

### 5.2 禁止做法

禁止直接生成：

```jass
function InitTrig_japi takes nothing returns nothing
endfunction
```

除非证明 JassHelper 在同等环境下确实会这样生成或地图运行时确实允许空实现。

### 5.3 目标

把环境符号处理做成**显式配置/显式输入**，而不是隐式 dummy。

### 5.4 实现方案

新增 CLI 选项之一或组合：

```text
--env-script <path>
--env-symbols <path>
--allow-external-init <name>
--pjass-allow-external InitTrig_japi
```

推荐优先实现：

```text
--env-symbols path/to/env_symbols.json
```

示例：

```json
{
  "externalFunctions": [
    {
      "name": "InitTrig_japi",
      "takes": [],
      "returns": "nothing",
      "kind": "map-init-external",
      "pjassStubPolicy": "declare-for-validation-only"
    }
  ]
}
```

然后在 PJASS validation 模式中有两种策略：

```text
1. validation-only stub：只在 PJASS 临时输入里附加声明，不写入正式 output.j。
2. environment-source：用户提供真实 env script，把它与 common/blizzard/output 一起传给 PJASS。
```

### 5.5 推荐实现细节

不要污染最终 `input.phase13.out.j`。实现 PJASS runner 时：

```text
common.j + blizzard.j + generated.j + validation_env_stubs.j
```

其中 `validation_env_stubs.j` 只在 build 目录生成，用于 PJASS 验证报告。

### 5.6 新增 fixture

```text
tests/fixtures/phase13_external_env_symbol.in.j
tests/fixtures/phase13_external_env_symbol.expected.validation.json
```

### 5.7 验收

```text
InitTrig_japi 不再作为 unresolved blocker。
validation report 清晰说明它是 external map-init symbol。
正式 generated output 不包含伪造的 InitTrig_japi 空函数。
```

---

## 6. 任务 C：Raw code callback adapter finalization

### 6.1 背景

Phase 12 剩余：

```text
callbackCodeSignatureMismatch: 15
```

这些是带参数 generated lambda 被传给 raw JASS `code` 位置。

JASS `code` 只能指向：

```jass
function X takes nothing returns nothing
```

因此：

```jass
function vjlambda__108 takes integer x returns nothing
```

不能直接作为：

```jass
function vjlambda__108
```

传给 raw `code` 参数。

### 6.2 目标

对 15 个 call sites 做分类，只在上下文明确时生成 adapter。目标是：

```text
callbackCodeSignatureMismatch: 15 -> 0
```

如果无法安全适配，必须保留为 classified unsafe callback，而不是伪修。

### 6.3 先做 call-site inventory

新增或增强 validation report：

```json
{
  "callbackCodeSignatureMismatchExamples": [
    {
      "generatedLine": 12345,
      "currentFunction": "...",
      "callee": "TimerStart",
      "argumentIndex": 4,
      "lambda": "vjlambda__108",
      "lambdaSignature": "takes integer returns nothing",
      "expected": "code",
      "sourceHint": "samples/input.j:xxxxx",
      "adapterDecision": "unknown"
    }
  ]
}
```

### 6.4 分类规则

把 raw `code` callback 分为：

```text
A. TimerStart / ForForce / ForGroup / EnumItems / EnumDestructables 等标准 no-arg callback
B. TriggerAddAction / TriggerAddCondition / Condition / Filter
C. Dz/JAPI/UI API callback
D. function-interface runtime 内部 wrapper
E. 无法识别的 raw code 参数
```

### 6.5 安全 adapter 策略

#### A. 标准 no-arg callback

如果 lambda 实际不需要外部参数，只是 lowering 误保留了参数，修 lambda signature 或生成 no-arg wrapper。

#### B. function-interface expected args 已知

如果调用点来自 function-interface dispatch，可用现有 function-interface argument globals：

```jass
function vjlambda__108_adapter takes nothing returns nothing
    call vjlambda__108(vjfi_arg_integer_1)
endfunction
```

但必须确认参数来源。

#### C. UI/DzAPI callback

不要猜参数。必须从 API 签名或调用点上下文证明 payload 来源。若无法证明，保留 unsafe classification。

### 6.6 adapter 命名

统一命名：

```text
vjlambda__N_code_adapter__M
```

避免重复名：

```text
duplicateFunctionNames 必须保持 0。
```

### 6.7 新增 fixture

```text
tests/fixtures/phase13_raw_code_noarg_lambda.in.j
tests/fixtures/phase13_raw_code_param_lambda_known_context.in.j
tests/fixtures/phase13_raw_code_param_lambda_unknown_context.in.j
```

### 6.8 验收

```text
callbackCodeSignatureMismatch: 15 -> 0，或 unsafe entries 被清楚记录。
PJASS grouped count 显著下降。
不允许在未知上下文中传 0/null 伪造参数。
```

---

## 7. 任务 D：最终 PJASS pass 验证流程

### 7.1 快速迭代流程

每一类 blocker 修复后，不要立即全量跑 3 次 PJASS。采用：

```text
1. focused fixtures
2. --validate-existing-output 如果只改 validator/report
3. --analyze-pjass-log 如果只改分类
4. full PJASS checkpoint
```

### 7.2 Full PJASS checkpoint 限制

Phase 13 中 full PJASS checkpoint 不应超过 4 次：

```text
Checkpoint 1: forward bridge batch 后
Checkpoint 2: InitTrig_japi env policy 后
Checkpoint 3: raw code callback adapter batch 后
Checkpoint 4: final verification
```

每次 checkpoint 必须记录：

```text
- grouped PJASS count
- per-group delta
- syntax-lite result
- init validation result
- duplicate name result
- total time
```

### 7.3 如果 PJASS 通过

生成：

```text
docs/phase13_status.md
build/input.phase13.pjass.out.j
build/input.phase13.pjass.validation.json
```

并在 status 中写明：

```text
PJASS passed.
This is still not runtime validation.
Next phase must run Warcraft III / map-load validation.
```

### 7.4 如果 PJASS 未通过

必须满足：

```text
grouped PJASS count <= 5
剩余每个错误都必须是无法无环境判断的 external/callback/cycle 问题。
```

---

## 8. 任务 E：性能继续压缩但不大拆架构

### 8.1 当前热点

Phase 12 热点：

```json
{
  "emitFunctions": 45750,
  "emitStructSupport": 35972,
  "lowerLambdas": 4777,
  "finalOutputValidationPrep": 1890,
  "sanitizeOutput": 1892,
  "functionOrdering": 291,
  "emitGlobals": 133
}
```

以及 counters：

```json
{
  "linesVisited": 78442,
  "regexCalls": 0,
  "memberAccessScans": 17265,
  "structLookupCalls": 139617,
  "functionLookupCalls": 239115,
  "cachedRewriteHits": 361626,
  "cachedRewriteMisses": 17106
}
```

### 8.2 Phase 13 性能目标

```text
最低：full validation <= 45s
良好：full validation <= 35s
优秀：full validation <= 30s
```

### 8.3 允许的低风险优化

```text
- 缓存 function lookup / struct lookup 的常见 key。
- 对没有 '.', '[', '(', 'function', 'call', 'set' 的行直接跳过 expensive rewrite。
- emitStructSupport 中避免重复收集/重复生成 support metadata。
- lowerLambdas 中缓存 lambda body lowering 结果。
- validation report 只在 --emit-validation-report 时做昂贵 example extraction。
- --emit-pjass-examples N 默认限制 N，不扫描过多 examples。
- CodeWriter reserve 更准确，减少 string realloc。
```

### 8.4 禁止的高风险优化

```text
- 不要大拆 AST/IR。
- 不要删除 syntax-lite / init validation。
- 不要为了速度跳过 PJASS grouping。
- 不要改动核心 lowering 语义但无 fixture。
```

---

## 9. 任务 F：README 和 Phase boundary 更新

### 9.1 README 更新

在 README 加入 Phase 13 摘要。

如果 PJASS 通过：

```text
Phase 13 achieves PJASS pass for the real samples/input.j generated output while keeping syntax-lite, init validation, and duplicate-name checks green. Runtime validation and behavior matching remain future work.
```

如果 PJASS 未通过但 <= 5：

```text
Phase 13 reduces grouped PJASS blockers to <= 5 and documents remaining external/callback/cycle blockers with provenance. PJASS pass remains the next target.
```

### 9.2 CLI 文档更新

加入或更新：

```text
--env-symbols <path>
--allow-external-init <name>
--pjass-allow-external <name>
--analyze-pjass-log ...
--validate-existing-output ...
```

只记录实际实现的选项，不要写未实现功能。

---

## 10. Phase 13 验收标准

### 10.1 最低验收

```text
- build passes
- ctest passes
- full input.j codegen succeeds
- syntax-lite ok
- init validation issues == 0
- duplicateFunctionNames == 0
- duplicateGlobalNames == 0
- duplicateNativeNames == 0
- grouped PJASS count <= 5
- every remaining PJASS error has provenance and decision
- full validation total <= 45s
```

### 10.2 良好验收

```text
- PJASS passes with optional explicit env symbol file or validation-only external symbol policy
- full validation total <= 35s
- generated output still does not contain fake InitTrig_japi unless user explicitly requested validation stub
```

### 10.3 优秀验收

```text
- PJASS passes without unsafe dummy declarations
- full validation total <= 30s
- Phase 13 status clearly separates:
  1. generated output correctness
  2. PJASS validation environment
  3. runtime validation still pending
```

---

## 11. Codex 执行顺序建议

Codex 应按下面顺序执行，不要反复随机修 PJASS：

```text
Step 1: 读取 docs/phase12_status.md 和最新 validation report。
Step 2: 用 --analyze-pjass-log 生成 Phase 13 triage baseline。
Step 3: 实现 forward bridge planner，写 fixtures，跑 ctest。
Step 4: 跑一次 full PJASS checkpoint。
Step 5: 实现 InitTrig_japi environment policy，写 fixtures，跑 ctest。
Step 6: 跑一次 full PJASS checkpoint。
Step 7: 实现 raw code callback adapter inventory + safe adapters，写 fixtures。
Step 8: 跑一次 full PJASS checkpoint。
Step 9: 做低风险性能优化，确保不改语义。
Step 10: final PJASS + validation + docs/README 更新。
```

---

## 12. 给 Codex 的特别注意事项

```text
- 当前目标是 PJASS pass，不是 byte-for-byte JassHelper matching。
- 当前目标不是 runtime validation；PJASS pass 后仍需进 War3 实机验证。
- InitTrig_japi 是环境符号，不能假装是编译器生成函数。
- raw code callback adapter 必须有上下文依据。
- true forward reference bridge 必须签名匹配。
- 不要因为剩余错误少就乱补默认 return 或 dummy globals。
- 每个修复都必须有 focused fixture。
- 每个 full PJASS checkpoint 都必须记录 grouped count delta。
```

---

## 13. Phase 13 完成后的下一阶段预期

如果 Phase 13 PJASS 通过，Phase 14 应转为：

```text
Warcraft III runtime/map-load validation + JassHelper behavior comparison + second performance pass
```

Phase 14 的方向：

```text
1. 用生成的 war3map.j 替换地图脚本并进游戏/编辑器加载。
2. 对比 JassHelper 输出的初始化顺序、关键 globals、struct allocator 行为。
3. 处理 runtime-only 问题。
4. 开始正式性能目标：60s -> 20s -> 10s -> 2s。
```

