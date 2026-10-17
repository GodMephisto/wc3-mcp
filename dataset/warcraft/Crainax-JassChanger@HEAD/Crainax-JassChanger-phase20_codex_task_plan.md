---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase20_codex_task_plan.md"
ref: HEAD
---

# Phase 20 Codex 实施计划：Deterministic ID、并行 lowering 试验、增量缓存试验与继续提速

> 项目：`Crainax/JassChanger`  
> 目标输入：`samples/input.j` 与 War3Lib / Xlimon ALPHA 编译链路  
> 当前基线：Phase 19 之后，PJASS / syntax-lite / init validation 均为绿色，War3Lib ALPHA fast 已明显快于 JassHelper。  
> Phase 20 核心：**不要再只做零散热路径微优化；要为并行 lowering 和增量编译建立安全、确定性、可回退的基础。**

---

## 0. 背景与当前状态

Phase 19 已达到以下状态：

```text
Standalone:
  fast:            5116 ms
  validate:        6901 ms
  full-validation: 6599 ms

War3Lib / Xlimon ALPHA:
  vjassc validate: 6.93 s
  vjassc fast:     5.57 s
  jasshelper:      11.76 s
```

Phase 19 主要收益来自：

```text
- string_view 透明查找
- MethodPlan skip
- array rewrite cache
- syntax-lite cheap guards
- less trim/copy
- read-only incremental report
```

当前仍然存在的主要瓶颈：

```text
sourceMethods fast:       ~2009 ms
syntaxLite validate:      ~1012 ms
functionOrdering fast:    有改善，但仍依赖 output scan fallback
functionOrderTokenScans:  56698
missingRecordedEdges:     14085
DependencyRecorder 覆盖率: 约 13.23%
```

Phase 19 已经新增只读增量报告，但并没有把增量缓存用于真实输出；并行 lowering 也仍然关闭，因为 lambda / function-interface / bridge 等全局 ID 分配还不是完全确定性所有权模型。

---

## 1. Phase 20 总目标

Phase 20 的目标不是一次性把冷编译压到 1 秒，而是完成以下三件事：

```text
1. 建立 deterministic generated entity ID plan。
2. 提升 dependency recorder 覆盖率，为减少 output scan 和并行/增量做准备。
3. 引入实验性 parallel lowering 与 experimental incremental cache，但默认关闭，保证输出稳定。
```

### 1.1 最低验收目标

```text
Correctness:
  - ctest 通过
  - standalone fast / validate / full-validation 均可运行
  - validate / full-validation PJASS groupedCount == 0
  - syntax-lite issueCount == 0
  - War3Lib / Xlimon ALPHA validate 通过
  - War3Lib / Xlimon ALPHA fast compare 通过

Performance, serial default path:
  - standalone fast <= 4700 ms
  - standalone validate <= 6500 ms
  - War3Lib ALPHA fast vjassc elapsed <= 5200 ms
  - War3Lib ALPHA validate elapsed <= 6500 ms

Recorder / cache:
  - dependency recorder coverage >= 35%
  - missingRecordedEdges <= 11000
  - sourceMethods <= 1700 ms
  - read-only incremental no-change reuse remains 100%
```

### 1.2 良好目标

```text
Performance, serial default path:
  - standalone fast <= 4200 ms
  - standalone validate <= 6000 ms
  - War3Lib ALPHA fast <= 4800 ms

Dependency recorder:
  - coverage >= 50%
  - missingRecordedEdges <= 8000
  - functionOrderTokenScans <= 35000

Method lowering:
  - sourceMethods <= 1400 ms
```

### 1.3 优秀 / 实验目标

```text
Experimental parallel lowering:
  - --experimental-parallel-lowering 下 standalone fast <= 3500 ms
  - parallel 与 serial 输出 byte-identical，或结构级完全一致且 PJASS 通过

Experimental incremental cache:
  - no-change incremental fast <= 3000 ms
  - no-change incremental 输出与 cold fast 输出 byte-identical
  - changed chunk report 能准确列出 changed / reused / added / removed chunks
```

---

## 2. 强制边界

### 2.1 不允许破坏现有正确性

不得为了提速牺牲这些结果：

```text
- PJASS pass
- syntax-lite pass
- init validation pass
- duplicate function/global/native == 0
- ALPHA validate 编译通过
- ALPHA fast compare 编译通过
```

### 2.2 并行和增量必须默认关闭

新增功能必须通过显式 flag 启用：

```text
--experimental-parallel-lowering
--parallel-threads N
--experimental-incremental-cache <path>
--incremental-mode report|read|write|readwrite
```

默认行为仍然是当前稳定 serial compile。

### 2.3 不允许 nondeterministic output

同一输入、同一参数、同一环境下，连续两次输出必须稳定。

要求新增测试：

```text
build/vjassc samples/input.j -o build/repeat_a.j --mode fast
build/vjassc samples/input.j -o build/repeat_b.j --mode fast
compare hash(repeat_a.j, repeat_b.j)
```

对 parallel / incremental 也需要做同样测试。

---

## 3. 任务 A：建立 Deterministic Generated Entity ID Plan

### 3.1 背景

当前 parallel lowering 不能直接启用，核心原因是许多全局实体 ID 在 lowering 过程中动态注册：

```text
- lambda id
- function-interface target id
- function-object runtime interface id
- generated wrapper id
- bridge id
- method caller id
```

这些 ID 如果在并行线程中动态分配，输出顺序和编号会不稳定。

### 3.2 新增数据结构

建议新增：

```cpp
struct GeneratedEntityPlan {
    std::vector<PlannedLambda> lambdas;
    std::vector<PlannedInterfaceTarget> interfaceTargets;
    std::vector<PlannedRuntimeWrapper> runtimeWrappers;
    std::vector<PlannedCycleBridge> cycleBridges;
    std::vector<PlannedMethodCaller> methodCallers;
};
```

每个实体都要有稳定 key：

```text
lambda:
  source location + container final name + ordinal in body

function-interface target:
  interface final name + target final function name + signature

runtime wrapper:
  runtime interface prefix + target final name + signature

bridge:
  caller final name + callee final name + call kind + signature

method caller:
  target final name + signature
```

### 3.3 实施步骤

1. 新增 `GeneratedEntityCollector` pass。
2. 在真正 lowering 前预扫描所有函数、方法、lambda、interface target 使用点。
3. 生成稳定排序后的 ID plan。
4. 将原先 lowering 过程中动态分配 ID 的地方改为查询 plan。
5. 对 plan 输出调试报告：

```text
--emit-generated-entity-plan build/generated_entity_plan.json
```

### 3.4 验收

```text
- serial fast 输出稳定
- validate 输出稳定
- full-validation 输出稳定
- generated_entity_plan.json 连续两次一致
- 所有 golden fixtures 通过
- PJASS 继续通过
```

---

## 4. 任务 B：提升 DependencyRecorder 覆盖率

### 4.1 背景

Phase 19 记录：

```text
recordedEdges: 2148
outputScanEdges: 16232
missingRecordedEdges: 14085
coveragePercent: 13.23%
```

目前 function ordering 仍然依赖 output scan fallback。要继续提速，并为并行/增量铺路，必须让 dependency recorder 更接近真实依赖图。

### 4.2 需要记录的边类型

新增或完善 edge 类型：

```cpp
DirectCall          // call Foo(...)
FunctionReference   // function Foo
TriggerCallback     // TriggerAddAction(..., function Foo)
ConditionCallback   // Condition(function Foo), Filter(function Foo)
ExecuteFuncWeak     // ExecuteFunc("Foo")，弱边，可单独分类
InterfaceWrapper    // function-interface runtime wrapper -> target
LambdaWrapper       // generated lambda wrapper -> target / captured bridge
CycleBridge         // bridge -> real target
InitCall            // init helper -> library/struct/function-interface init
GeneratedSupport    // generated destroy/create/onInit -> helper target
```

### 4.3 实施步骤

1. 在 `lowerStatementLine` / `lowerExpression` / `rewriteCallArguments` 里记录边。
2. 在 function-interface runtime 生成时记录 wrapper 到 target 的边。
3. 在 lambda 生成时记录 lambda body 内 direct call / function ref 边。
4. 在 init helper emit 时记录 init helper 边。
5. 对 output scan 仍保留 fallback，但报告 diff：

```json
{
  "recordedEdges": 8000,
  "outputScanEdges": 16000,
  "matchedEdges": 7500,
  "missingRecordedEdges": 8500,
  "extraRecordedEdges": 500,
  "coveragePercent": 46.8,
  "missingByCategory": {
    "DirectCall": 3000,
    "FunctionReference": 1200,
    "GeneratedWrapper": 900
  }
}
```

### 4.4 验收

```text
最低：coverage >= 35%, missing <= 11000
良好：coverage >= 50%, missing <= 8000
优秀：coverage >= 70%, missing <= 5000
```

注意：Phase 20 不要求彻底移除 output scan，只要求 recorder 明显进步。

---

## 5. 任务 C：MethodLoweringPlan 与 MethodBody Rewrite Cache

### 5.1 背景

`sourceMethods` 目前仍约 2 秒，是主要热点之一。

Phase 19 已经有 MethodPlan 初步计数：

```text
built: 1540
linesSkippedNoCandidate: 20617
bareFieldRewriteAttempts: 12182
bareFieldRewriteChanged: 9130
shadowSkips: 23999
```

下一步要把 MethodPlan 从“辅助跳过”升级成“method body lowering 主入口”。

### 5.2 MethodLoweringPlan 内容

```cpp
struct MethodLoweringPlan {
    StructId structId;
    MethodId methodId;
    bool mayUseThis;
    bool mayUseThistype;
    bool mayUseBareField;
    bool mayUseBareMethod;
    bool mayUseStaticField;
    bool mayUseArrayRewrite;
    bool mayUseFunctionValue;
    bool mayUseLambda;
    SmallSet<NameId> params;
    SmallSet<NameId> locals;
    SmallMap<NameId, FieldId> bareFieldCandidates;
    SmallMap<NameId, MethodId> bareMethodCandidates;
    SmallMap<NameId, FieldId> staticFieldCandidates;
};
```

### 5.3 Rewrite cache

对 method body line 建立 cache：

```cpp
struct MethodLineRewriteKey {
    MethodId methodId;
    uint64_t lineHash;
    uint32_t localScopeHash;
    uint32_t featureBits;
};
```

缓存内容：

```cpp
struct MethodLineRewriteValue {
    std::vector<std::string> loweredLines;
    std::vector<DependencyEdge> edges;
    bool changed;
};
```

### 5.4 实施边界

先只缓存安全场景：

```text
- 不含 lambda
- 不生成 temp locals
- 不触发 function-interface target 注册
- 不触发 bridge/wrapper request
```

复杂行仍走旧路径。

### 5.5 验收

```text
sourceMethods <= 1700 ms
良好 <= 1400 ms
优秀 <= 1000 ms
PJASS 继续通过
```

---

## 6. 任务 D：Array Rewrite 精准过滤二次优化

### 6.1 背景

Phase 19：

```text
arrayAccessRewriteAttempts: 6313
```

这已经比早期好很多，但仍有进一步空间。

### 6.2 建立 KnownArrayReceiverIndex

```cpp
struct KnownArrayReceiverIndex {
    FlatSet<NameId> globalArrays;
    FlatSet<NameId> localArrays;
    FlatSet<NameId> structFieldArrays;
    FlatSet<NameId> generatedStructArrays;
};
```

只在 token 形态为：

```text
identifier [
```

且 `identifier` 命中 known array receiver 时进入 array rewrite。

### 6.3 验收

```text
arrayAccessRewriteAttempts <= 4000
arrayAccessRewriteChanged 不得异常下降
相关 fixtures 通过
```

---

## 7. 任务 E：实验性 Parallel Lowering

### 7.1 背景

Phase 19 没启用并行，因为 ID allocation 不稳定。Phase 20 在完成 GeneratedEntityPlan 后，可以做实验性并行。

### 7.2 CLI

新增：

```bash
--experimental-parallel-lowering
--parallel-threads 4
--parallel-kind functions|methods|all
```

默认关闭。

### 7.3 第一版并行范围

只并行这些：

```text
- 普通 top-level function body lowering
- struct source method body lowering
```

暂不并行：

```text
- globals emit
- types/natives emit
- init helper emit
- function-interface runtime emit
- final function ordering
- final output assembly
```

### 7.4 并行任务模型

```cpp
struct BodyLoweringJob {
    FunctionId functionId;
    BodyMode mode;
    SourceSpan bodySpan;
    const ReadOnlyCodegenIndex* index;
    const GeneratedEntityPlan* plan;
};

struct BodyLoweringResult {
    FunctionId functionId;
    std::vector<std::string> lines;
    std::vector<DependencyEdge> edges;
    std::vector<GeneratedRequest> requests;
    Diagnostics diagnostics;
};
```

所有全局写入必须延迟到主线程合并。

### 7.5 输出稳定性测试

要求：

```bash
vjassc input.j -o serial.j --mode fast
vjassc input.j -o parallel.j --mode fast --experimental-parallel-lowering --parallel-threads 4
```

比较：

```text
hash(serial.j) == hash(parallel.j)
```

如果不能 byte-identical，至少必须：

```text
- PJASS pass
- syntax-lite pass
- structure metrics identical
- explain differences in phase20_status.md
```

但优先追求 byte-identical。

### 7.6 验收

```text
parallel 默认关闭
parallel fast PJASS pass
parallel fast 输出稳定
parallel 4 threads standalone fast <= 3500 ms，作为实验目标
```

---

## 8. 任务 F：实验性 Incremental Cache

### 8.1 背景

Phase 19 已经有 read-only incremental report：

```text
chunkCount: 13255
reusedChunks: 13255
reusePercent: 100%
```

Phase 20 可以开始 experimental incremental cache，但不得默认启用。

### 8.2 CLI

```bash
--emit-incremental-state build/state.json
--compare-incremental-state build/old_state.json
--experimental-incremental-cache build/cache_dir
--incremental-mode report|read|write|readwrite
```

### 8.3 Chunk 类型

只缓存这些：

```text
- top-level function body lowered output
- struct source method body lowered output
- generated lambda body lowered output
- dependency edges
- local generated requests that are deterministic under GeneratedEntityPlan
```

暂不缓存：

```text
- globals block
- types/natives
- library sorting
- init helper
- function-interface runtime final assembly
- final ordering output
```

### 8.4 Cache key

```cpp
struct IncrementalCacheKey {
    std::string compilerVersion;
    std::string compileMode;
    uint64_t sourceChunkHash;
    uint64_t symbolEpochHash;
    uint64_t generatedEntityPlanHash;
    uint64_t optionsHash;
};
```

必须包含：

```text
- vjassc version/build hash
- mode: fast/validate/full-validation
- debug/warn options
- source chunk hash
- visible symbol table hash
- generated entity plan hash
- body mode
```

### 8.5 第一版增量策略

先做 no-change 安全复用：

```text
if all chunks reusable:
  use cached function/method/lambda lowered bodies
  still rebuild globals/init/runtime/final ordering
```

如果有 changed chunks，先只报告，不复用混合输出，除非显式 `--incremental-mode=readwrite`。

### 8.6 验收

```text
no-change incremental fast 输出 byte-identical
no-change incremental fast <= 3000 ms 实验目标
changed chunk report 正确显示 changed/reused/added/removed
默认编译不使用 incremental cache
```

---

## 9. 任务 G：Syntax-lite 与 validation 进一步瘦身

### 9.1 validate 模式

validate 模式保留：

```text
syntax-lite core checks
PJASS
init validation
```

full-validation 才保留：

```text
JassHelper comparison
examples
detailed provenance report
full residue buckets
```

### 9.2 复用 token cache

syntax-lite 对 output 的扫描可以复用：

```text
- function list
- duplicate names
- source residue flags
- invalid statement shape flags
```

若 codegen 时已经记录了合法性 flags，syntax-lite 不必重复完整扫描。

### 9.3 验收

```text
validate syntaxLite <= 800 ms
full-validation syntaxLite <= 1000 ms
syntax-lite issueCount 仍为 0
```

---

## 10. War3Lib / Xlimon 集成要求

### 10.1 新增环境变量

War3Lib 可以支持但默认不启用：

```text
WAR3_VJASSC_PARALLEL=0|1
WAR3_VJASSC_THREADS=4
WAR3_VJASSC_INCREMENTAL=off|report|read|write|readwrite
WAR3_VJASSC_INCREMENTAL_CACHE=Output/.vjassc-cache
```

### 10.2 ALPHA 任务验收

必须继续通过：

```bat
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

```bat
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

### 10.3 报告要求

`Output/compiler_backend_report.json` 新增：

```json
{
  "vjasscParallel": false,
  "vjasscThreads": 1,
  "vjasscIncrementalMode": "off",
  "vjasscIncremental": {
    "chunkCount": 0,
    "reusedChunks": 0,
    "changedChunks": 0,
    "reusePercent": 0
  },
  "vjasscDependencyRecorder": {
    "coveragePercent": 0,
    "missingRecordedEdges": 0
  }
}
```

---

## 11. 测试计划

### 11.1 Golden fixtures

新增 fixtures：

```text
phase20_deterministic_lambda_ids
phase20_function_interface_target_id_plan
phase20_dependency_recorder_direct_call
phase20_dependency_recorder_function_ref
phase20_dependency_recorder_trigger_callback
phase20_method_plan_cache_shadowing
phase20_incremental_no_change
phase20_parallel_output_stability
```

### 11.2 重复输出稳定性

必须加入测试脚本：

```text
serial fast repeat output hash same
parallel fast repeat output hash same
incremental no-change output hash same as cold fast
```

### 11.3 PJASS

所有 validate/full-validation 仍需 PJASS pass：

```text
pjass.ok == true
pjass.groupedCount == 0
```

---

## 12. 性能报告要求

新增或扩展：

```text
build/input.phase20.fast.stats.json
build/input.phase20.validate.stats.json
build/input.phase20.full.stats.json
build/input.phase20.performance.json
build/input.phase20.generated_entity_plan.json
build/input.phase20.incremental.state.json
build/input.phase20.incremental.report.json
```

`docs/phase20_status.md` 必须包含：

```text
1. Phase 19 baseline vs Phase 20 serial numbers
2. parallel experimental numbers, if implemented
3. incremental no-change numbers, if implemented
4. dependency recorder coverage table
5. sourceMethods timing
6. syntaxLite timing
7. War3Lib ALPHA validate timing
8. War3Lib ALPHA fast compare timing
9. correctness summary
10. deployment hash, if deployed
```

---

## 13. Recommended execution order for Codex

按以下顺序执行，不要跳步：

```text
Step 1: Capture Phase 19 baseline locally.
Step 2: Implement GeneratedEntityPlan.
Step 3: Add output stability tests.
Step 4: Improve DependencyRecorder coverage.
Step 5: Add MethodLoweringPlan cache for safe lines.
Step 6: Add experimental parallel lowering, default off.
Step 7: Add experimental incremental cache no-change path, default off.
Step 8: Optimize syntax-lite with token/cache reuse.
Step 9: Run standalone fast/validate/full-validation.
Step 10: Run War3Lib/Xlimon ALPHA validate and fast compare.
Step 11: Write docs/phase20_status.md.
```

---

## 14. Non-goals

Phase 20 不做：

```text
- 不默认启用并行 lowering
- 不默认启用增量缓存
- 不删除 output scan fallback
- 不大改 AST/parser
- 不改变 JassHelper-compatible runtime behavior
- 不把 validation-only stub 写入正式 output
- 不牺牲 PJASS pass 换速度
```

---

## 15. 最终验收清单

完成后至少满足：

```text
[ ] ctest pass
[ ] standalone fast pass
[ ] standalone validate PJASS pass
[ ] standalone full-validation PJASS pass
[ ] War3Lib ALPHA validate pass
[ ] War3Lib ALPHA fast compare pass
[ ] serial output deterministic
[ ] GeneratedEntityPlan report exists
[ ] DependencyRecorder coverage improved
[ ] read-only incremental report still works
[ ] experimental incremental no-change output stable, if implemented
[ ] experimental parallel output stable, if implemented
[ ] docs/phase20_status.md committed
```

---

## 16. 预期结论

如果 Phase 20 顺利，项目将从：

```text
fast 冷编译约 5.1s
validate 约 6.9s
```

推进到：

```text
serial fast 约 4.2~4.7s
serial validate 约 5.8~6.5s
parallel experimental fast 约 3~4s
incremental no-change experimental 约 2~3s
```

Phase 20 的真正价值不只是提速，而是建立：

```text
确定性 ID 分配
依赖边记录
可并行 body lowering
可增量 chunk cache
```

这四个基础能力。它们是后续接近 1~2 秒日常编译体验的关键。
