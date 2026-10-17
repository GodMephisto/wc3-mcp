---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase22_codex_task_plan.md"
ref: HEAD
---

# Phase 22 Codex 实施计划：Parallel / Incremental 从原型进入可用化

## 0. 阶段定位

Phase 22 **不是从 0 开始实现并发与增量编译**。

本阶段默认基于 Phase 21 已经建立的这些基础继续推进：

```text
1. GeneratedEntityPlan 已经用于稳定生成实体 ID。
2. BodyJob / BodyLoweringResult 已经把 body lowering 拆成可调度任务。
3. Recorded Dependency Graph 已经有实验开关与 dependency report。
4. Parallel Lowering 已经有实验路径，至少可以验证输出稳定性。
5. Incremental Cache 已经有 body-level reuse 原型，默认关闭。
6. War3Lib / Xlimon ALPHA 已经可以通过环境变量接入实验功能。
```

Phase 22 的核心目标是：

```text
把 Phase 21 的并行/增量原型推进到“可稳定用于日常开发”的状态。
```

本阶段重点不再是证明“能不能做”，而是解决：

```text
1. 哪些功能可以默认启用。
2. 哪些功能仍保持 experimental。
3. no-change incremental 是否真正稳定变快。
4. small-change incremental 是否能正确失效并复用。
5. parallel lowering 是否在大图上稳定且确定性输出。
6. recorded-order 是否可以取代 output scan fallback。
7. War3Lib / Xlimon 的 ALPHA 流程是否能直接受益。
```

Phase 22 结束时，希望达到：

```text
默认 fast 工作流仍保持正确。
ALPHA / experimental 工作流可以安全开启 parallel + incremental。
日常小改编译速度明显接近 1~2 秒体验。
```

---

## 1. 当前 Phase 21 交接基线

请 Codex 从以下分支开始：

```text
branch: codex/phase21-parallel-incremental
```

如果该分支不存在，请先从 Phase 21 完成分支或当前 master 新建：

```bat
git checkout codex/phase21-parallel-incremental
```

或者：

```bat
git checkout -b codex/phase22-parallel-incremental-productize
```

### 1.1 开始前必须确认的 Phase 21 产物

先确认 CLI 是否已经支持以下选项：

```text
--experimental-recorded-order
--experimental-parallel-lowering
--parallel-workers <N>
--experimental-incremental-cache <path>
--incremental-mode report|reuse
--emit-incremental-state <path>
--emit-incremental-report <path>
--compare-incremental-state <path>
--emit-dependency-report <path>
--emit-generated-entity-plan <path>
```

如果某些选项在 Phase 21 分支中命名略有差异，本阶段不要强行重命名全部旧接口；先在代码中找到对应功能，再决定是否补 alias。

### 1.2 开始前必须跑的基线命令

编译与单测：

```bat
cmd.exe /d /s /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

Standalone fast baseline：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.fast.out.j ^
  --mode fast ^
  --emit-stats build\phase22.baseline.fast.stats.json ^
  --emit-performance-report build\phase22.baseline.performance.json ^
  --emit-incremental-state build\phase22.baseline.incremental.state.json
```

Standalone validate baseline：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.validate.out.j ^
  --mode validate ^
  --emit-stats build\phase22.baseline.validate.stats.json ^
  --emit-validation-report build\phase22.baseline.validation.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

Phase 21 experimental matrix baseline：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.recorded.out.j ^
  --mode fast ^
  --experimental-recorded-order ^
  --emit-dependency-report build\phase22.baseline.recorded.deps.json
```

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.parallel.out.j ^
  --mode fast ^
  --experimental-parallel-lowering ^
  --parallel-workers 4
```

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.inc.cold.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-phase22-cache ^
  --incremental-mode reuse ^
  --emit-incremental-state build\phase22.baseline.inc.state.json ^
  --emit-incremental-report build\phase22.baseline.inc.cold.report.json
```

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.baseline.inc.nochange.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-phase22-cache ^
  --incremental-mode reuse ^
  --compare-incremental-state build\phase22.baseline.inc.state.json ^
  --emit-incremental-report build\phase22.baseline.inc.nochange.report.json
```

要求记录：

```text
1. normal fast 时间。
2. recorded-order fast 时间与 functionOrderTokenScans。
3. parallel fast 时间与输出是否 byte-identical。
4. no-change incremental 时间与 reusePercent。
5. no-change incremental 输出是否与 cold 输出一致。
```

---

## 2. 本阶段硬性原则

### 2.1 默认稳定行为不能退化

默认不带实验参数时：

```text
vjassc samples/input.j --mode fast
```

必须保持 Phase 21 / Phase 20 已验证的稳定行为。

以下情况一律算失败：

```text
1. ctest 失败。
2. PJASS 不通过。
3. syntax-lite 报错。
4. init validation 报错。
5. duplicate function/global/native 增加。
6. fast repeated output 不稳定。
7. 默认 fast 输出明显变慢。
8. 默认 fast 输出发生不必要的大规模结构变化。
```

### 2.2 实验功能可逐步升级，但不能无验证默认开启

Phase 22 可以做“候选默认启用”评估，但不能直接把以下功能默认开启：

```text
--experimental-parallel-lowering
--experimental-incremental-cache
--experimental-recorded-order
```

除非满足本计划后文的 promoted 条件。

建议本阶段内部区分三种状态：

```text
experimental: 默认关闭，仅手动开关。
candidate: 默认关闭，但文档建议 ALPHA 可开启。
promoted: 可以在 ALPHA 默认开启，但 release 默认仍关闭。
```

### 2.3 并行与增量必须可回退

任何实验路径遇到不确定情况，必须自动 fallback 到稳定路径：

```text
1. incremental cache key 不匹配 -> cold lowering。
2. recorded-order coverage 不足 -> output scan fallback。
3. parallel job 内部异常 -> 禁止吞错，直接失败或回退单线程。
4. dependency report 显示强依赖缺失 -> 不允许使用 recorded-order 作为最终 ordering。
```

### 2.4 输出确定性优先于速度

Phase 22 的速度收益不能以不稳定输出换取。

并行路径必须满足：

```text
1. 同一输入重复运行输出 byte-identical。
2. workers=1 与 workers=4 输出 byte-identical，除非明确记录了允许差异。
3. incremental no-change 输出与 cold 输出 byte-identical。
4. small-change incremental 输出必须 PJASS pass，并且只改变合理区域。
```

---

## 3. Phase 22 目标概览

### 3.1 最低目标

```text
默认模式：
  correctness 不退化。
  fast median 不比 Phase 21 明显慢。

recorded-order：
  PJASS pass。
  functionOrderTokenScans < 8000。
  dependency report 能解释 fallback 原因。

parallel lowering：
  repeated output byte-identical。
  workers=4 median speedup >= 15%。
  validate 模式 PJASS pass。

incremental cache：
  no-change output byte-identical。
  no-change reusePercent >= 95%。
  no-change standalone fast <= 2000 ms。
  small-change incremental PJASS pass。
```

### 3.2 良好目标

```text
recorded-order：
  functionOrderTokenScans < 3000。
  strongEdgeCoveragePercent >= 99.5%。

parallel lowering：
  median speedup >= 30%。

incremental cache：
  no-change standalone fast <= 1500 ms。
  small-change standalone fast <= 2500 ms。

War3Lib / Xlimon ALPHA：
  incremental fast <= 3500 ms。
```

### 3.3 优秀目标

```text
parallel + incremental：
  no-change standalone fast <= 1200 ms。
  small-change standalone fast <= 1800 ms。
  War3Lib ALPHA vjassc compiler segment <= 2000 ms。

ALPHA 工作流：
  可以默认开启 recorded-order。
  可以默认开启 parallel lowering。
  incremental cache 作为 ALPHA 可选开启。
```

---

## 4. 任务一：Phase 21 产物审计与状态文档

### 4.1 目标

在继续开发前，先把 Phase 21 真实状态变成明确报告，避免 Phase 22 建立在错误假设上。

### 4.2 新增文档

新增：

```text
docs/phase22_entry_audit.md
```

内容包括：

```text
1. 当前分支名与 commit hash。
2. Phase 21 相关 CLI 是否存在。
3. GeneratedEntityPlan 是否稳定。
4. BodyJob runner 是否存在。
5. Parallel lowering 是否输出稳定。
6. Incremental cache 是否可复用。
7. Recorded-order 是否仍需要 output scan fallback。
8. 当前 standalone baseline。
9. 当前 War3Lib / Xlimon ALPHA baseline，如果可运行。
10. 已知失败项。
```

### 4.3 验收

必须提交：

```text
docs/phase22_entry_audit.md
```

并包含至少这些数据：

```text
ctest result
fast baseline ms
validate result
recorded-order functionOrderTokenScans
parallel output equality result
incremental no-change reusePercent
incremental no-change output equality result
```

---

## 5. 任务二：GeneratedEntityPlan 完善为“唯一 ID 真源”

### 5.1 背景

Phase 21 已经要求 GeneratedEntityPlan 稳定 lambda / wrapper / bridge / function interface target 等生成实体。

Phase 22 要进一步确保：

```text
所有并行 lowering 期间可能产生名字或 ID 的实体，都必须在 lowering 前被 plan 固定。
```

如果仍然有 lowering 过程中临时注册的实体，并行与增量都会不稳定。

### 5.2 需要纳入 plan 的实体

检查并补齐：

```text
1. lambda entity
2. function interface target entity
3. function object wrapper entity
4. method caller bridge entity
5. struct allocate/deallocate/create/destroy support entity
6. operator getter/setter lowering helper entity
7. default argument bridge entity
8. interpreter path call bridge entity
9. trigger callback wrapper entity
10. static onInit helper entity
11. module implement generated method entity
```

其中 Phase 21 可能已经覆盖 1~4，本阶段重点检查 5~11。

### 5.3 推荐数据结构

如果已有结构可复用，不必强行改名；但建议让 plan 至少表达以下字段：

```cpp
enum class GeneratedEntityKind {
    Lambda,
    FunctionInterfaceTarget,
    FunctionObjectWrapper,
    MethodCallerBridge,
    StructLifecycleSupport,
    OperatorBridge,
    DefaultArgumentBridge,
    InterpreterCallBridge,
    TriggerCallbackWrapper,
    StaticOnInitHelper,
    ModuleImplementedMember
};

struct GeneratedEntityKey {
    GeneratedEntityKind kind;
    std::string containerPath;
    std::string sourceFile;
    int sourceLine;
    int sourceColumn;
    int ordinalInContainer;
    std::string targetFinalName;
    std::string signatureHash;
    std::string bodyMode;
};

struct GeneratedEntityPlanEntry {
    int stableId;
    GeneratedEntityKey key;
    std::string finalName;
    std::string ownerDeclFinalName;
};
```

### 5.4 实施步骤

```text
1. 搜索 lowering 阶段所有 “nextId++ / push_back generated / makeUniqueName” 类型逻辑。
2. 分类哪些是可提前 plan 的生成物。
3. 在 semantic / pre-lowering 阶段收集 plan。
4. lowering 阶段只允许按 key 查询 plan entry，不允许新建未登记实体。
5. 对确实无法提前 plan 的实体，写入 docs/phase22_entry_audit.md 的 Known Issues。
```

### 5.5 新增测试

新增 fixture：

```text
phase22_generated_entity_plan_full_coverage
phase22_generated_entity_plan_parallel_stability
phase22_generated_entity_plan_incremental_stability
```

覆盖：

```text
1. lambda inside function。
2. function interface evaluate/execute。
3. struct method callback。
4. operator [] / []=。
5. interpreter nested call。
6. module implement 产生的方法。
7. default parameter bridge。
```

### 5.6 验收命令

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.plan.a.out.j ^
  --mode fast ^
  --emit-generated-entity-plan build\phase22.plan.a.json

build\vjassc.exe samples\input.j ^
  -o build\phase22.plan.b.out.j ^
  --mode fast ^
  --emit-generated-entity-plan build\phase22.plan.b.json
```

要求：

```text
phase22.plan.a.out.j == phase22.plan.b.out.j
phase22.plan.a.json == phase22.plan.b.json
```

并行路径：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.plan.parallel.out.j ^
  --mode fast ^
  --experimental-parallel-lowering ^
  --parallel-workers 4 ^
  --emit-generated-entity-plan build\phase22.plan.parallel.json
```

要求：

```text
phase22.plan.parallel.out.j == phase22.plan.a.out.j
phase22.plan.parallel.json == phase22.plan.a.json
```

---

## 6. 任务三：BodyJob 模型产品化

### 6.1 目标

Phase 21 已经把 body lowering 抽象成 job。Phase 22 要把它从原型改成稳定生产路径。

重点：

```text
1. 单线程 job runner 可以替代旧 lowering 路径。
2. 并行 job runner 与单线程 job runner 共用同一套逻辑。
3. BodyJob 不能直接写全局 writer 或全局 mutable state。
4. BodyLoweringResult 必须包含足够的输出、依赖边、cache 信息。
```

### 6.2 BodyJob 应覆盖的范围

必须覆盖：

```text
ordinary function body
struct source method body
operator method body
lambda body
default argument generated bridge body
function interface execute/evaluate wrapper body
interpreter call bridge body
```

暂不强制覆盖：

```text
globals emission
native/type emission
struct generated support globals
final function ordering
final output writing
```

这些仍可以主线程执行。

### 6.3 BodyJob 输入约束

BodyJob 内只能读：

```text
1. immutable AST。
2. immutable symbol table snapshot。
3. immutable generated entity plan。
4. immutable lowering options。
5. immutable container / visibility map。
```

BodyJob 不允许直接修改：

```text
writer_
lambdas_
functionInterfaces_
functionInterfaceCalls_
functionObjectCalls_
global generated wrapper vectors
global dependency graph
global codegen counters without lock
```

### 6.4 BodyLoweringResult 推荐字段

```cpp
struct BodyLoweringResult {
    int stableId;
    std::string finalName;
    BodyJobKind kind;
    std::vector<std::string> outputLines;
    std::vector<DependencyEdge> dependencyEdges;
    std::vector<GeneratedRequest> generatedRequests;
    std::vector<Diagnostic> diagnostics;
    BodyCacheRecord cacheRecord;
    CodegenPerformanceCounters counters;
};
```

### 6.5 合并规则

主线程合并时必须：

```text
1. 按 stableId 排序。
2. 先合并 diagnostics，遇到 error 终止。
3. 再合并 dependencyEdges。
4. 再合并 generatedRequests。
5. 最后合并 outputLines。
6. counters 只做加和或 max，不允许影响输出顺序。
```

### 6.6 验收

新增开关，如果已有类似开关可复用：

```bat
--experimental-body-jobs-single-thread
```

跑：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.legacy.out.j ^
  --mode fast

build\vjassc.exe samples\input.j ^
  -o build\phase22.bodyjobs.single.out.j ^
  --mode fast ^
  --experimental-body-jobs-single-thread
```

要求：

```text
phase22.legacy.out.j == phase22.bodyjobs.single.out.j
```

如果 Phase 21 已经没有 legacy path，则要求：

```text
workers=1 body job output == default output
```

---

## 7. 任务四：Parallel Lowering 稳定化

### 7.1 目标

Phase 21 的 parallel lowering 是原型。Phase 22 要让它满足日常 ALPHA 可用条件。

目标：

```text
1. workers=1、workers=2、workers=4、workers=8 输出稳定。
2. repeated parallel 输出 byte-identical。
3. validate 模式可用。
4. performance report 能显示并行 job 数、worker 数、等待时间。
```

### 7.2 CLI 保持

继续使用：

```bat
--experimental-parallel-lowering
--parallel-workers <N>
```

新增或完善 report 字段：

```json
{
  "parallelLowering": {
    "enabled": true,
    "workers": 4,
    "jobs": 5900,
    "completedJobs": 5900,
    "failedJobs": 0,
    "queueMs": 12,
    "workerTotalMs": 8300,
    "mergeMs": 41,
    "speedupRatioEstimate": 1.72
  }
}
```

### 7.3 线程池要求

使用固定线程池。

不要使用：

```cpp
std::async per job
```

建议：

```cpp
class ThreadPool {
public:
    explicit ThreadPool(size_t workerCount);
    template <class F>
    auto submit(F&& f);
    void wait();
};
```

或简单 work queue：

```text
1. 主线程创建 N 个 worker。
2. atomic nextIndex 分配 job。
3. 每个 worker 写入 results[stableId] 或 thread-local vector。
4. 主线程最后排序合并。
```

### 7.4 不允许并行的阶段

继续保持主线程：

```text
globals merging
native/type emission
library dependency sorting
struct support globals emission
final function ordering
final output writer
PJASS validation
```

### 7.5 验收矩阵

```bat
build\vjassc.exe samples\input.j -o build\phase22.parallel.w1.out.j --mode fast --experimental-parallel-lowering --parallel-workers 1
build\vjassc.exe samples\input.j -o build\phase22.parallel.w2.out.j --mode fast --experimental-parallel-lowering --parallel-workers 2
build\vjassc.exe samples\input.j -o build\phase22.parallel.w4.out.j --mode fast --experimental-parallel-lowering --parallel-workers 4
build\vjassc.exe samples\input.j -o build\phase22.parallel.w8.out.j --mode fast --experimental-parallel-lowering --parallel-workers 8
```

要求：

```text
w1 == w2 == w4 == w8
```

重复稳定性：

```bat
build\vjassc.exe samples\input.j -o build\phase22.parallel.repeat1.out.j --mode fast --experimental-parallel-lowering --parallel-workers 4
build\vjassc.exe samples\input.j -o build\phase22.parallel.repeat2.out.j --mode fast --experimental-parallel-lowering --parallel-workers 4
build\vjassc.exe samples\input.j -o build\phase22.parallel.repeat3.out.j --mode fast --experimental-parallel-lowering --parallel-workers 4
```

要求：

```text
repeat1 == repeat2 == repeat3
```

Validate：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.parallel.validate.out.j ^
  --mode validate ^
  --experimental-parallel-lowering ^
  --parallel-workers 4 ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

要求：

```text
PJASS pass
syntax-lite pass
init validation pass
```

---

## 8. 任务五：Incremental Cache 从 no-change 扩展到 small-change

### 8.1 目标

Phase 21 的 incremental cache 原型重点是 no-change。Phase 22 必须把 small-change 做实。

目标：

```text
1. no-change 能稳定复用 >= 95%。
2. 修改单个普通函数，只重编译必要 body。
3. 修改 struct 字段，会让相关 method / operator / generated support 正确失效。
4. 修改 function interface 签名，会让相关 lambda / wrapper / caller 正确失效。
5. 修改 public/private 可见性或 final name，会让相关调用者正确失效。
6. 缓存不安全时自动 fallback，而不是生成错误输出。
```

### 8.2 缓存范围

本阶段缓存：

```text
ordinary function body lowering result
struct method body lowering result
operator method body lowering result
lambda body lowering result
default argument bridge body
function interface wrapper body
body dependency edges
body-level counters
```

继续不缓存：

```text
complete final war3map.j
final function order
native/type section
globals section
library topo order
PJASS result
```

### 8.3 ChunkKey 必须增强

Phase 21 如果只用 sourceHash + finalName，本阶段需要增强。

推荐：

```cpp
struct ChunkKey {
    std::string compilerCacheVersion;
    std::string optionsHash;
    std::string kind;
    std::string finalName;
    std::string containerPath;
    std::string signatureHash;
    std::string sourceHash;
    std::string visibleSymbolHash;
    std::string generatedEntityPlanHash;
    std::string dependencyShapeHash;
};
```

### 8.4 relevant hash 内容

`visibleSymbolHash` 至少包含：

```text
1. 当前函数参数和返回类型。
2. 当前 struct 字段列表、字段类型、static/readonly/array shape。
3. 当前 struct method final name map。
4. function interface signatures。
5. public/private/protected rewrite map。
6. imported native/type declarations used by body。
7. operator overload table。
8. interpreter path function map。
9. module implement expansion version。
10. generated entity plan slice。
```

如果第一版无法做到精确，可以保守使用：

```text
globalSymbolTableHash
```

但必须在 incremental report 中标明：

```json
{
  "symbolInvalidationMode": "global-conservative"
}
```

### 8.5 增量报告字段

完善 `--emit-incremental-report`：

```json
{
  "incremental": {
    "enabled": true,
    "mode": "reuse",
    "cachePath": "build/.vjassc-phase22-cache",
    "chunkCount": 13255,
    "reusedChunks": 13240,
    "recompiledChunks": 15,
    "rejectedChunks": 0,
    "reusePercent": 99.8,
    "coldFallback": false,
    "symbolInvalidationMode": "precise-or-global-conservative",
    "rejectReasons": {
      "sourceHashChanged": 1,
      "signatureHashChanged": 0,
      "symbolHashChanged": 14,
      "optionsHashChanged": 0,
      "compilerVersionChanged": 0
    }
  }
}
```

### 8.6 no-change 验收

Cold run 建缓存：

```bat
rmdir /s /q build\.vjassc-phase22-cache

build\vjassc.exe samples\input.j ^
  -o build\phase22.inc.cold.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-phase22-cache ^
  --incremental-mode reuse ^
  --emit-incremental-state build\phase22.inc.cold.state.json ^
  --emit-incremental-report build\phase22.inc.cold.report.json
```

No-change run：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.inc.nochange.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-phase22-cache ^
  --incremental-mode reuse ^
  --compare-incremental-state build\phase22.inc.cold.state.json ^
  --emit-incremental-state build\phase22.inc.nochange.state.json ^
  --emit-incremental-report build\phase22.inc.nochange.report.json
```

要求：

```text
phase22.inc.cold.out.j == phase22.inc.nochange.out.j
reusePercent >= 95%
recompiledChunks <= 5% total chunks
PJASS validate pass after no-change output
```

### 8.7 small-change fixture

新增最小 fixture：

```text
tests/fixtures/phase22_incremental_small_change/input_v1.j
tests/fixtures/phase22_incremental_small_change/input_v2.j
```

v1 示例：

```jass
library IncSmall initializer Init
    globals
        integer G = 0
    endglobals

    private function A takes nothing returns nothing
        set G = G + 1
    endfunction

    private function B takes nothing returns nothing
        set G = G + 2
    endfunction

    private function Init takes nothing returns nothing
        call A()
        call B()
    endfunction
endlibrary
```

v2 只改 B：

```jass
    private function B takes nothing returns nothing
        set G = G + 3
    endfunction
```

验收：

```text
changedChunks > 0
reusedChunks > changedChunks
A 的 chunk 应复用
B 的 chunk 应重编译
最终输出 PJASS pass
```

### 8.8 symbol-change fixture

新增：

```text
phase22_incremental_struct_field_change
phase22_incremental_function_interface_signature_change
phase22_incremental_operator_change
phase22_incremental_visibility_change
```

要求：

```text
相关 chunk 必须失效。
无关 chunk 可以复用。
不能错误复用导致 PJASS 失败。
```

---

## 9. 任务六：Recorded Dependency Graph 取代 output scan fallback

### 9.1 目标

Phase 21 的 recorded dependency graph 已经有原型，但可能仍然运行 output scan fallback。

Phase 22 要实现：

```text
1. recorded-order 能独立完成大部分 function ordering。
2. output scan fallback 只在 coverage 不足或 debug compare 时运行。
3. dependency report 明确显示缺失边、弱边和 fallback 原因。
```

### 9.2 DependencyKind 完善

确认至少支持：

```cpp
enum class DependencyKind {
    DirectCall,
    MethodCall,
    StaticMethodCall,
    OperatorCall,
    FunctionReference,
    TriggerCallback,
    FunctionInterfaceTarget,
    FunctionInterfaceExecute,
    FunctionObjectCall,
    LambdaWrapper,
    GeneratedBridge,
    InitializerCall,
    StaticOnInitCall,
    ExecuteFuncWeak
};
```

### 9.3 强/弱依赖分类

强依赖参与 ordering：

```text
DirectCall
MethodCall
StaticMethodCall
OperatorCall
FunctionReference
TriggerCallback
FunctionInterfaceTarget
FunctionInterfaceExecute
FunctionObjectCall
LambdaWrapper
GeneratedBridge
InitializerCall
StaticOnInitCall
```

弱依赖记录但默认不强制排序：

```text
ExecuteFuncWeak
```

### 9.4 Recorded-order 算法

```text
1. 为所有 output function 分配 FunctionId。
2. 从 BodyLoweringResult 收集 dependencyEdges。
3. 加入 generated support 的显式边。
4. 加入 initializer / static onInit 的显式边。
5. 加入 function interface runtime assembly 的显式边。
6. 构建 directed graph。
7. Tarjan SCC 找循环。
8. SCC 间 Kahn topo sort。
9. SCC 内保持原始 stable order。
10. 如果 strong edge 缺失率超过阈值，fallback 到 output scan。
```

### 9.5 Dependency report 字段

```json
{
  "recordedOrder": {
    "enabled": true,
    "usedForFinalOrder": true,
    "fallbackToOutputScan": false,
    "functionCount": 6864,
    "recordedEdges": 24123,
    "strongRecordedEdges": 23500,
    "weakExecuteFuncEdges": 623,
    "outputScanEdges": 0,
    "missingRecordedEdges": 0,
    "extraRecordedEdges": 0,
    "coveragePercent": 100.0,
    "strongEdgeCoveragePercent": 100.0,
    "functionOrderTokenScans": 0,
    "sccCount": 120,
    "largestSccSize": 4
  }
}
```

如果仍需 compare mode：

```bat
--experimental-recorded-order-compare-output-scan
```

该模式允许同时跑 output scan 做对比，但最终报告必须说明没有用它排序。

### 9.6 验收

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.recorded.out.j ^
  --mode validate ^
  --experimental-recorded-order ^
  --emit-dependency-report build\phase22.recorded.deps.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

要求：

```text
PJASS pass
syntax-lite pass
init validation pass
functionOrderTokenScans < 8000，良好目标 < 3000
fallbackToOutputScan == false，若为 true 必须在 docs 解释原因
```

---

## 10. 任务七：Struct / Method / Operator 降级稳定性补强

### 10.1 目标

Phase 22 不重新做 struct 系统，但要补齐并行/增量后最容易出错的降级边界。

重点：

```text
1. struct lifecycle support 不能在并行时重复生成。
2. operator [] / []= 的调用改写必须记录 dependency edge。
3. static method / instance method 的 final name 必须进入 symbol hash。
4. onDestroy / destroy / deallocate 调用顺序必须稳定。
5. 泛型 struct 单态化结果必须进入 generated entity plan 与 cache key。
```

### 10.2 必测语法点

新增 fixture：

```text
phase22_struct_lifecycle_parallel
phase22_struct_operator_dependency
phase22_struct_static_method_cache
phase22_struct_generic_monomorph_cache
phase22_struct_onDestroy_incremental
```

覆盖示例：

```jass
struct TableLike
    integer array values

    method operator [] takes integer key returns integer
        return this.values[key]
    endmethod

    method operator []= takes integer key, integer value returns nothing
        set this.values[key] = value
    endmethod

    method onDestroy takes nothing returns nothing
        call BJDebugMsg("destroy")
    endmethod
endstruct
```

### 10.3 验收

```text
1. 默认 fast pass。
2. parallel workers=4 输出与默认一致。
3. incremental no-change 输出与 cold 一致。
4. small-change 修改 operator body 后，仅相关 chunk 失效。
5. recorded-order report 中能看到 OperatorCall 依赖边。
```

---

## 11. 任务八：Function Interface / Lambda / Delegate 缓存与并行安全

### 11.1 目标

你的 samples/input.j 里 function interface / lambda / wrapper 这类生成物数量很多，是并行和增量最容易出错的区域。

Phase 22 要确保：

```text
1. function interface target id 稳定。
2. lambda wrapper id 稳定。
3. evaluate / execute wrapper 输出稳定。
4. delegate method resolution 结果进入 cache key。
5. 跨 library private/public 可见性不被 cache 错误复用。
```

### 11.2 新增 fixtures

```text
phase22_fi_basic_evaluate
phase22_fi_lambda_parallel_stability
phase22_fi_signature_change_invalidates
phase22_delegate_method_resolution
phase22_delegate_visibility_cache
```

### 11.3 验收

```text
1. workers=1/4/8 输出一致。
2. no-change reusePercent 高。
3. 修改 function interface signature 后相关 wrapper 全部失效。
4. 修改 lambda body 后只重编译 lambda 相关 chunk。
5. PJASS pass。
```

---

## 12. 任务九：Interpreter / Textmacro / Static If 与增量兼容

### 12.1 Interpreter

Phase 21 已经支持 interpreter path 调用或至少已有基础。Phase 22 要确保它不会破坏 cache 与 recorded dependency。

要求：

```text
1. interpreter.('父').('子').Func() 必须记录 DirectCall 或 InterpreterCall edge。
2. function interpreter.('父').Func 必须记录 FunctionReference edge。
3. 完整 interpreter path 必须进入 ChunkKey 的 containerPath 或 visibleSymbolHash。
4. 输出注释 interpreter-start/end 不因 parallel 顺序变化而乱序。
```

新增 fixture：

```text
phase22_interpreter_nested_parallel
phase22_interpreter_function_ref_dependency
phase22_interpreter_path_change_invalidates
```

### 12.2 Textmacro

textmacro 展开理论上在 parse / pre-lowering 前完成。Phase 22 要确认：

```text
1. 展开后的 sourceHash 稳定。
2. 修改 textmacro 定义会让所有 runtextmacro 使用点失效。
3. optional runtextmacro 不存在时不会污染 cache。
4. macro 展开位置错误仍应报语法错误，而不是 cache 复用旧结果。
```

新增 fixture：

```text
phase22_textmacro_definition_change_invalidates
phase22_textmacro_optional_missing_cache
phase22_textmacro_expansion_location_error
```

### 12.3 Static if / optional library

要求：

```text
1. static if 剔除结果进入 sourceHash 或 preprocessedHash。
2. optional library 存在/不存在会影响 visibleSymbolHash。
3. LIBRARY_X 常量变化时相关 chunk 必须失效。
```

新增 fixture：

```text
phase22_static_if_optional_library_cache
phase22_static_if_branch_change_invalidates
```

---

## 13. 任务十：Native / DzAPI / 自定义 native 顺序稳定

### 13.1 目标

Phase 22 不需要重新实现 native 解析，但要确保并行/增量/recorded-order 不影响 native 输出位置。

要求输出顺序：

```text
1. globals block。
2. native declarations。
3. generated support globals/functions。
4. normal functions。
```

地图内自由声明 native 仍要被移动到正确位置。

### 13.2 DzAPI 兼容点

必须允许：

```text
1. common.j / blizzard.j 不认识但地图内声明的 native。
2. InitTrig_japi 这类外部入口通过 --pjass-allow-external 放行。
3. native 去重逻辑不能误删用户自定义 native。
4. repeated output native 顺序稳定。
```

### 13.3 新增 fixture

```text
phase22_native_custom_order
phase22_native_duplicate_dedup
phase22_native_dzapi_allow_external
```

### 13.4 验收

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.native.validate.out.j ^
  --mode validate ^
  --experimental-parallel-lowering ^
  --parallel-workers 4 ^
  --experimental-recorded-order ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

要求：

```text
PJASS pass
native duplicate count 不增加
output native section 稳定
```

---

## 14. 任务十一：Benchmark 工具与性能报告产品化

### 14.1 目标

Phase 21 已经提出 benchmark repeat/warmup。Phase 22 必须让性能数据可复现。

如果 CLI 已支持：

```bat
--benchmark-repeat <N>
--benchmark-warmup <N>
--emit-benchmark-report <path>
```

则完善它。

如果 CLI 未支持，则新增脚本：

```text
tools/bench_phase22.ps1
```

### 14.2 bench_phase22.ps1 参数

```powershell
param(
  [string]$Exe = "build/vjassc.exe",
  [string]$Input = "samples/input.j",
  [int]$Warmup = 1,
  [int]$Repeat = 7,
  [string]$OutDir = "build/phase22-bench",
  [switch]$RecordedOrder,
  [switch]$Parallel,
  [int]$Workers = 4,
  [switch]$Incremental,
  [string]$CacheDir = "build/.vjassc-phase22-cache"
)
```

输出：

```json
{
  "scenario": "parallel+incremental-nochange",
  "warmup": 1,
  "repeat": 7,
  "totalMs": {
    "min": 1100,
    "median": 1250,
    "p90": 1410,
    "max": 1490
  },
  "codegenMs": {
    "median": 900
  },
  "incremental": {
    "reusePercent": 99.6
  },
  "parallel": {
    "workers": 4,
    "jobs": 5900
  }
}
```

### 14.3 必测场景

```text
1. default fast。
2. recorded-order fast。
3. parallel fast workers=4。
4. incremental no-change fast。
5. recorded-order + parallel fast。
6. recorded-order + incremental no-change fast。
7. recorded-order + parallel + incremental no-change fast。
8. small-change incremental fast。
```

### 14.4 性能判断规则

```text
1. 只看 median，不看单次最优。
2. warmup 至少 1 次。
3. repeat 至少 5 次，建议 7 次。
4. 小于 5% 的差异视为噪声。
5. 大于等于 8% 的 median 改善才算明确收益。
```

---

## 15. 任务十二：War3Lib / Xlimon ALPHA 接入 Phase 22 组合开关

### 15.1 目标

Phase 21 已经计划了环境变量。Phase 22 要让它们进入 ALPHA 编译流程并产出报告。

继续支持或新增：

```text
WAR3_VJASSC_PARALLEL=0|1
WAR3_VJASSC_WORKERS=4
WAR3_VJASSC_INCREMENTAL=0|1
WAR3_VJASSC_INCREMENTAL_CACHE=...
WAR3_VJASSC_RECORDED_ORDER=0|1
WAR3_VJASSC_INCREMENTAL_MODE=report|reuse
```

### 15.2 ALPHA 默认策略

Phase 22 初期：

```text
全部默认关闭。
```

Phase 22 后期如果通过验收：

```text
recorded-order 可在 ALPHA 默认开启。
parallel 可在 ALPHA 默认开启。
incremental 仍建议手动开启，除非 no-change 和 small-change 都非常稳定。
```

### 15.3 War3Lib report 新增字段

```json
{
  "vjasscExperimental": {
    "recordedOrder": true,
    "parallel": true,
    "workers": 4,
    "incremental": true,
    "incrementalMode": "reuse"
  },
  "recordedOrder": {
    "usedForFinalOrder": true,
    "functionOrderTokenScans": 0,
    "strongEdgeCoveragePercent": 100.0
  },
  "parallel": {
    "jobs": 5900,
    "workers": 4,
    "speedupRatioEstimate": 1.7
  },
  "incremental": {
    "chunkCount": 13255,
    "reusedChunks": 13240,
    "reusePercent": 99.8,
    "recompiledChunks": 15
  }
}
```

### 15.4 验收命令

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
set WAR3_VJASSC_RECORDED_ORDER=1
set WAR3_VJASSC_PARALLEL=1
set WAR3_VJASSC_WORKERS=4
set WAR3_VJASSC_INCREMENTAL=1
set WAR3_VJASSC_INCREMENTAL_MODE=reuse
set WAR3_VJASSC_INCREMENTAL_CACHE=D:/Project/JassChanger/build/.vjassc-war3lib-cache
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

要求：

```text
ALPHA fast pass
生成的 war3map.j 可通过现有 compare/validate 流程
report 包含 recorded/parallel/incremental 字段
```

---

## 16. 测试计划

### 16.1 新增测试清单

```text
phase22_entry_audit_exists
phase22_generated_entity_plan_full_coverage
phase22_generated_entity_plan_parallel_stability
phase22_bodyjobs_single_matches_default
phase22_parallel_workers_stability
phase22_parallel_validate_pjass
phase22_incremental_nochange
phase22_incremental_small_body_change
phase22_incremental_struct_field_change
phase22_incremental_function_interface_signature_change
phase22_incremental_operator_change
phase22_incremental_visibility_change
phase22_recorded_order_no_fallback
phase22_recorded_order_cycle_scc
phase22_recorded_order_execute_func_weak
phase22_struct_lifecycle_parallel
phase22_struct_operator_dependency
phase22_fi_lambda_parallel_stability
phase22_delegate_method_resolution
phase22_interpreter_nested_parallel
phase22_textmacro_definition_change_invalidates
phase22_static_if_optional_library_cache
phase22_native_custom_order
phase22_native_duplicate_dedup
```

### 16.2 Standalone 必跑

```bat
ctest --test-dir build --output-on-failure
```

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase22.final.default.validate.out.j ^
  --mode validate ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

### 16.3 Experimental matrix 必跑

```text
1. default fast。
2. recorded-order fast。
3. parallel fast workers=4。
4. incremental no-change fast。
5. recorded-order + parallel fast。
6. recorded-order + incremental no-change fast。
7. parallel + incremental no-change fast。
8. recorded-order + parallel + incremental no-change fast。
9. small-change incremental。
```

每个场景记录：

```text
output equality
PJASS / validation
median timing
reusePercent
functionOrderTokenScans
dependency coverage
parallel worker count
```

---

## 17. 验收标准

### 17.1 Correctness

必须全部满足：

```text
ctest pass。
default validate PJASS pass。
parallel validate PJASS pass。
incremental no-change validate PJASS pass。
recorded-order validate PJASS pass。
no duplicate function/global/native regression。
syntax-lite pass。
init validation pass。
```

### 17.2 Determinism

必须全部满足：

```text
repeated default fast output byte-identical。
repeated parallel output byte-identical。
parallel workers=1/2/4/8 output byte-identical。
incremental no-change output == cold output。
generated entity plan repeated json identical。
```

### 17.3 Incremental

最低：

```text
no-change reusePercent >= 95%。
no-change standalone fast <= 2000 ms。
small-change incremental PJASS pass。
small-change reusedChunks > recompiledChunks。
```

良好：

```text
no-change standalone fast <= 1500 ms。
small-change standalone fast <= 2500 ms。
```

优秀：

```text
no-change standalone fast <= 1200 ms。
small-change standalone fast <= 1800 ms。
```

### 17.4 Parallel

最低：

```text
workers=4 median speedup >= 15%。
```

良好：

```text
workers=4 median speedup >= 30%。
```

优秀：

```text
workers=4 median speedup >= 40%。
```

### 17.5 Recorded-order

最低：

```text
functionOrderTokenScans < 8000。
PJASS pass。
```

良好：

```text
functionOrderTokenScans < 3000。
strongEdgeCoveragePercent >= 99.5%。
```

优秀：

```text
functionOrderTokenScans == 0 或接近 0。
fallbackToOutputScan == false。
```

---

## 18. 输出文档要求

必须新增：

```text
docs/phase22_status.md
```

内容包括：

```text
1. Implemented
2. Entry Audit Summary
3. Correctness Matrix
4. Determinism Matrix
5. Generated Entity Plan Coverage
6. BodyJob Productization
7. Parallel Lowering Results
8. Incremental Cache Results
9. Recorded Dependency Graph Results
10. War3Lib / Xlimon ALPHA Results
11. Benchmark Median Report
12. Feature Promotion Recommendation
13. Known Issues
14. Phase 23 Recommendation
```

### 18.1 Feature Promotion Recommendation 格式

```markdown
## Feature Promotion Recommendation

| Feature | Status | Default Recommendation | Reason |
|---|---|---|---|
| recorded-order | candidate/promoted | ALPHA on / release off | PJASS pass, scans reduced |
| parallel-lowering | candidate/promoted | ALPHA on / release off | deterministic, speedup 30% |
| incremental-cache | experimental/candidate | manual only | small-change still needs coverage |
```

---

## 19. Codex 执行顺序建议

不要一次性大改。按 batch 执行。

### Batch 1：入口审计与基线

```text
1. 跑 ctest / fast / validate。
2. 跑 Phase 21 experimental matrix。
3. 生成 docs/phase22_entry_audit.md。
```

完成后提交。

### Batch 2：GeneratedEntityPlan 全覆盖

```text
1. 搜索所有 lowering 时生成 ID 的位置。
2. 把遗漏生成物纳入 plan。
3. 增加 plan stability 测试。
4. 验证 parallel plan identical。
```

完成后提交。

### Batch 3：BodyJob 产品化

```text
1. 确保 single-thread job runner 等价默认路径。
2. 移除/隔离 body job 内全局 mutable 写入。
3. BodyLoweringResult 补齐 edges/cache/counters。
```

完成后提交。

### Batch 4：Parallel 稳定化

```text
1. workers=1/2/4/8 输出一致。
2. repeat 输出一致。
3. validate PJASS pass。
4. performance report 增加 parallel 字段。
```

完成后提交。

### Batch 5：Incremental no-change + small-change

```text
1. no-change 输出等价。
2. small-change fixture。
3. struct/signature/operator/visibility invalidation fixture。
4. incremental report 完善。
```

完成后提交。

### Batch 6：Recorded-order 无 fallback

```text
1. 补齐 dependency kinds。
2. generated support / initializer / FI wrapper edges。
3. Tarjan/Kahn ordering。
4. 降低 functionOrderTokenScans。
```

完成后提交。

### Batch 7：边缘语法兼容

```text
1. struct/operator。
2. function interface/lambda/delegate。
3. interpreter/textmacro/static if。
4. native/DzAPI。
```

完成后提交。

### Batch 8：War3Lib ALPHA 与最终文档

```text
1. 接入环境变量组合。
2. 跑 War3Lib ALPHA。
3. 跑 benchmark repeat。
4. 写 docs/phase22_status.md。
```

完成后提交。

---

## 20. Phase 23 预期方向

Phase 22 如果完成良好，Phase 23 可以考虑：

```text
1. ALPHA 默认开启 recorded-order。
2. ALPHA 默认开启 parallel lowering。
3. incremental cache 对 no-change / small-change 默认开启。
4. 优化 cache 持久化格式与跨进程复用。
5. 更细粒度 symbol dependency hash，减少保守失效。
6. 对 War3Lib / Xlimon 做真实编辑循环 benchmark。
7. 逐步将 experimental 参数改为 stable 参数。
```

最终目标：

```text
普通冷编译稳定 2~3 秒以内。
日常小改增量编译 1~2 秒以内。
no-change 增量编译 1 秒左右或以内。
输出 correctness 与 JassHelper 语义等价。
```

---

## 21. 特别提醒

本阶段不要把目标写成“重新做并发/增量编译”。

Phase 21 已经建立原型，因此 Phase 22 的关键词是：

```text
productize
stabilize
deterministic
invalidate correctly
measure with median
ALPHA integration
```

不要为了追求 benchmark 单次数字而牺牲：

```text
输出稳定性
PJASS 正确性
small-change 失效安全
fallback 可控性
War3Lib ALPHA 可回退
```

Phase 22 完成后，项目才真正具备把并发与增量编译用于日常开发的基础。
