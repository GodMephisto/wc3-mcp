---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase21_codex_task_plan.md"
ref: HEAD
---

# Phase 21 Codex 实施计划：Parallel / Incremental Prototype

## 0. 阶段定位

Phase 21 不再以“继续压冷编译单次耗时”为主目标。

当前 Phase 20 已经证明：

- `vjassc` 正确性仍然是绿色：
  - `ctest` 通过。
  - syntax-lite 通过。
  - PJASS 通过，`groupedCount == 0`。
  - repeated fast output byte-identical。
- Phase 20 增加了后续并行/增量所需的基础：
  - generated entity plan。
  - dependency recorder report。
  - read-only incremental state/report。
  - no-change incremental report 可显示 `13255 / 13255` chunks reusable。
- 但 Phase 20 没有形成稳定冷编译净提速：
  - standalone fast `5116 ms -> 5341 ms / 5159 ms repeat`。
  - War3Lib ALPHA fast `5.53s`，仍通过但未达严格目标。
  - 测量误差已经足以吞掉小幅优化收益。

因此 Phase 21 的目标是：

```text
建立可验证、可回退、默认关闭的并行 lowering 与增量 body cache 原型。
```

本阶段不追求“冷编译继续下降几百毫秒”，而追求：

```text
1. no-change incremental 明显快。
2. small-change incremental 明显快。
3. parallel lowering 输出稳定、PJASS 通过。
4. 所有实验功能默认关闭，不影响当前稳定 ALPHA 工作流。
```

---

## 1. 当前基线

请 Codex 开始前先确认当前分支和基线。

当前建议基于：

```text
branch: codex/phase20-performance 或从 master 新建 phase21 分支
```

必须先跑：

```bat
cmd.exe /d /s /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

Standalone baseline：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase21.baseline.fast.out.j ^
  --mode fast ^
  --emit-stats build\input.phase21.baseline.fast.stats.json ^
  --emit-performance-report build\input.phase21.baseline.performance.json ^
  --emit-incremental-state build\input.phase21.baseline.incremental.state.json
```

Validate baseline：

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase21.baseline.validate.out.j ^
  --mode validate ^
  --emit-stats build\input.phase21.baseline.validate.stats.json ^
  --emit-validation-report build\input.phase21.baseline.validate.validation.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

War3Lib ALPHA baseline：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

---

## 2. 本阶段硬性原则

### 2.1 实验功能默认关闭

以下功能必须默认关闭：

```text
--experimental-recorded-order
--experimental-parallel-lowering
--experimental-incremental-cache
```

默认模式必须保持 Phase 20 的稳定行为。

### 2.2 不允许牺牲正确性换速度

以下情况一律算失败：

```text
PJASS 不通过。
syntax-lite 报错。
init validation 报错。
duplicate function/global/native 增加。
fast 输出不稳定。
parallel 输出与 single-thread 输出不一致。
incremental no-change 输出与 cold 输出不一致。
```

### 2.3 不要反复全量跑 War3Lib

为避免浪费时间和 token，Codex 执行顺序必须是：

```text
1. 小 fixture。
2. samples/input.j standalone fast。
3. samples/input.j standalone validate。
4. 必要时 War3Lib ALPHA fast。
5. 最后才 War3Lib ALPHA validate / compare。
```

除非已经完成一个 batch，不要每修几行就跑 War3Lib。

---

## 3. 新增/完善 CLI

### 3.1 并行 lowering 相关

新增：

```bat
--experimental-parallel-lowering
--parallel-workers <N>
```

规则：

```text
默认关闭。
N <= 1 时等价单线程。
N 未指定时可用 hardware_concurrency - 1，但至少 1。
输出必须与单线程 byte-identical，至少在 samples/input.j 上要一致。
```

### 3.2 依赖图排序相关

新增：

```bat
--experimental-recorded-order
--emit-dependency-report <path>
```

规则：

```text
默认仍保留 output-scan fallback。
experimental-recorded-order 使用 recorded dependency graph 做 ordering。
必须输出 dependency report：
  recordedEdges
  outputScanEdges
  missingRecordedEdges
  extraRecordedEdges
  weakExecuteFuncEdges
  coveragePercent
  strongEdgeCoveragePercent
```

### 3.3 增量 cache 相关

新增或完善：

```bat
--experimental-incremental-cache <path>
--incremental-mode report|reuse
--emit-incremental-state <path>
--emit-incremental-report <path>
--compare-incremental-state <path>
```

规则：

```text
report:
  只报告，不参与编译输出。

reuse:
  允许复用缓存 body lowering 结果，但必须满足严格 invalidation。
  默认关闭。
```

### 3.4 benchmark 辅助

新增：

```bat
--benchmark-repeat <N>
--benchmark-warmup <N>
--emit-benchmark-report <path>
```

如实现成本过高，可先写 `tools/bench_phase21.ps1`，但推荐 CLI 或脚本至少支持：

```text
warmup runs
repeat runs
min / median / p90 / max
```

后续性能验收必须看 median，不以单次运行作为判断依据。

---

## 4. Benchmark 规范

由于当前 5 秒级编译受 Windows 调度、磁盘缓存、CPU 频率影响明显，本阶段必须采用多次运行。

建议：

```text
warmup: 1
repeat: 5 或 7
metric: median
```

示例报告：

```json
{
  "mode": "fast",
  "warmup": 1,
  "repeat": 7,
  "totalMs": {
    "min": 4920,
    "median": 5070,
    "p90": 5350,
    "max": 5480
  },
  "codegenMs": {
    "median": 4480
  }
}
```

验收规则：

```text
小于 5% 的单次下降不算明确提速。
大于等于 8% 的 median 下降才算有效提速。
```

---

## 5. 任务一：Deterministic Generated Entity Ownership

### 5.1 目标

并行 lowering 之前必须先解决 ID 稳定性。

目前不能直接并行的原因：

```text
lambda id
function-interface target id
wrapper id
bridge id
method caller id
```

这些 ID 可能是在 lowering 时边遇到边注册。如果多个线程同时 lower，就会导致输出顺序和 ID 不稳定。

### 5.2 实施要求

新增 `GeneratedEntityPlan`，在 body lowering 前完成：

```cpp
struct GeneratedEntityPlan {
    std::vector<LambdaEntity> lambdas;
    std::vector<InterfaceTargetEntity> interfaceTargets;
    std::vector<WrapperEntity> wrappers;
    std::vector<BridgeEntity> bridges;
};
```

每个 entity 的 key 必须稳定：

```text
container path
source location
function/method/lambda ordinal
target final name
signature
body mode
```

### 5.3 验收

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.plan.a.j ^
  --mode fast ^
  --emit-generated-entity-plan build\phase21.plan.a.json

build\vjassc.exe samples\input.j ^
  -o build\phase21.plan.b.j ^
  --mode fast ^
  --emit-generated-entity-plan build\phase21.plan.b.json
```

要求：

```text
phase21.plan.a.j == phase21.plan.b.j
phase21.plan.a.json == phase21.plan.b.json
```

---

## 6. 任务二：Body Lowering Job Model

### 6.1 目标

将 function/method/lambda body lowering 抽象成可单线程/并行共用的 job。

### 6.2 数据结构建议

```cpp
enum class BodyJobKind {
    Function,
    StructMethod,
    Lambda,
    GeneratedSupport
};

struct BodyJob {
    int stableId;
    BodyJobKind kind;
    BodyMode mode;
    const Decl* decl;
    const MethodDecl* method;
    const StructInfo* currentStruct;
    const Decl* container;
    std::vector<std::string_view> sourceLines;
    GeneratedEntityPlanSlice generatedSlice;
};

struct BodyLoweringResult {
    int stableId;
    std::vector<std::string> outputLines;
    std::vector<DependencyEdge> dependencyEdges;
    std::vector<GeneratedRequest> generatedRequests;
    CodegenPerformanceCounters counters;
};
```

### 6.3 关键要求

Body job 不能直接修改这些全局状态：

```text
writer_
lambdas_
functionInterfaces_
functionInterfaceCalls_
functionObjectCalls_
global caches that mutate without lock
```

如果需要新 wrapper/request，写入 `BodyLoweringResult`，主线程最后合并。

### 6.4 验收

先只做单线程 job runner：

```text
existing lowering path output
job runner output
```

必须 byte-identical。

---

## 7. 任务三：Recorded Dependency Graph 替换 Output Scan 原型

### 7.1 当前问题

Phase 20 已经把 dependency recorder report 覆盖率变成 100%，但 `functionOrderTokenScans` 没降：

```text
functionOrderTokenScans: 56698 -> 56698
```

说明 output scan fallback 仍在运行。

### 7.2 目标

新增实验开关：

```bat
--experimental-recorded-order
```

在该模式下：

```text
使用 lowering 阶段记录的 dependencyEdges 构造 FunctionId graph。
不再对最终 output 做完整 dependency token scan。
保留 debug assert/report 对比能力。
```

### 7.3 依赖边类型

```cpp
enum class DependencyKind {
    DirectCall,
    FunctionReference,
    TriggerCallback,
    FunctionInterfaceTarget,
    ExecuteFuncWeak,
    GeneratedBridge,
};
```

强依赖：

```text
DirectCall
FunctionReference
TriggerCallback
FunctionInterfaceTarget
GeneratedBridge
```

弱依赖：

```text
ExecuteFuncWeak
```

弱依赖可记录但不一定参与 ordering。

### 7.4 算法

```text
1. FunctionId 分配。
2. lowering 时记录 edges。
3. Tarjan SCC 找循环。
4. SCC 间 Kahn topo sort。
5. SCC 内维持原始顺序或已有 bridge 策略。
```

### 7.5 验收

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.recorded-order.out.j ^
  --mode validate ^
  --experimental-recorded-order ^
  --emit-dependency-report build\phase21.recorded-order.deps.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

要求：

```text
PJASS pass。
syntax-lite pass。
duplicate checks pass。
functionOrderTokenScans 明显下降，目标 < 15000。
输出与 fallback 不要求 byte-identical，但结构指标应合理。
```

---

## 8. 任务四：Experimental Parallel Lowering

### 8.1 目标

实现默认关闭的并行 body lowering。

```bat
--experimental-parallel-lowering --parallel-workers 4
```

### 8.2 并行范围

可以并行：

```text
ordinary function body lowering
struct source method body lowering
lambda body lowering
dependency edge collection
```

暂不并行：

```text
globals
native/type emission
function-interface runtime assembly
struct generated support assembly
init helper
final function ordering
final output writing
```

### 8.3 实施建议

使用固定线程池，不要每个 job 一个 `std::async`：

```cpp
class ThreadPool {
public:
    explicit ThreadPool(size_t workerCount);
    template<class F> auto submit(F&& f);
};
```

如果实现线程池成本过高，可先用 work queue + `std::thread`。

### 8.4 合并顺序

并行结果必须按 `BodyJob.stableId` 排序合并：

```text
collect results
sort by stableId
merge output
merge dependencies
merge counters
```

### 8.5 验收

单线程 vs 并行：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.single.out.j ^
  --mode fast

build\vjassc.exe samples\input.j ^
  -o build\phase21.parallel.out.j ^
  --mode fast ^
  --experimental-parallel-lowering ^
  --parallel-workers 4
```

要求：

```text
byte-identical output。
parallel validate PJASS pass。
repeat 3 次 parallel output byte-identical。
```

### 8.6 性能目标

最低：

```text
parallel standalone fast median <= single-thread fast median * 0.85
```

良好：

```text
parallel standalone fast median <= single-thread fast median * 0.70
```

如果没有收益，不默认启用，只保留实验报告。

---

## 9. 任务五：Experimental Incremental Cache

### 9.1 目标

将 Phase 19/20 的 read-only incremental report 变成可选 reuse 原型。

```bat
--experimental-incremental-cache build\.vjassc-cache ^
--incremental-mode reuse
```

### 9.2 缓存范围

第一版只缓存 body lowering 结果：

```text
ordinary function body output
struct source method body output
lambda body output
dependency edges
body-level counters
```

暂不缓存：

```text
globals
type/native section
struct generated support
function-interface runtime final assembly
init helper
final ordering
final output
```

### 9.3 Chunk Key

建议：

```cpp
struct ChunkKey {
    std::string kind;          // function/method/lambda
    std::string finalName;
    std::string containerPath;
    std::string signatureHash;
    std::string sourceHash;
    std::string relevantSymbolHash;
    std::string compilerCacheVersion;
    std::string optionsHash;
};
```

其中 `relevantSymbolHash` 至少包含：

```text
current struct fields/methods
local/param signature
function interface signatures used by body
known array shapes used by body
private/public rewrite map version
```

第一版如果不确定 relevantSymbolHash，可保守使用 global symbol table hash。这样 reuse 少一点，但安全。

### 9.4 增量模式

```text
report:
  只报告 reuse，不复用输出。

reuse:
  尝试复用。
  如果不安全，fallback cold lowering。
```

### 9.5 no-change 验收

先建立 cache：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.inc.cold.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-cache ^
  --incremental-mode reuse ^
  --emit-incremental-state build\phase21.inc.state.json
```

再 no-change：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.inc.nochange.out.j ^
  --mode fast ^
  --experimental-incremental-cache build\.vjassc-cache ^
  --incremental-mode reuse ^
  --compare-incremental-state build\phase21.inc.state.json ^
  --emit-incremental-report build\phase21.inc.nochange.report.json
```

要求：

```text
cold output == nochange output。
reusePercent >= 95%。
PJASS validate pass after no-change incremental output.
```

### 9.6 small-change 验收

添加小 fixture 或复制 `samples/input.j` 后修改一个函数体。

要求：

```text
changedChunks > 0。
unchanged chunks reusable。
输出 PJASS pass。
只改动相关函数/依赖区域。
```

### 9.7 性能目标

no-change incremental：

```text
最低：standalone fast <= 2500 ms
良好：standalone fast <= 1500 ms
```

small-change incremental：

```text
最低：standalone fast <= 3500 ms
良好：standalone fast <= 2500 ms
```

如果未达成，但 correctness 稳定，可以作为 Phase 22 继续推进。

---

## 10. 任务六：War3Lib / Xlimon 接入实验模式

新增环境变量：

```text
WAR3_VJASSC_PARALLEL=0|1
WAR3_VJASSC_WORKERS=4
WAR3_VJASSC_INCREMENTAL=0|1
WAR3_VJASSC_INCREMENTAL_CACHE=...
WAR3_VJASSC_RECORDED_ORDER=0|1
```

规则：

```text
默认全部关闭。
仅 ALPHA 任务允许开启。
正式版本不允许默认开启 experimental。
```

War3Lib report 增加：

```json
{
  "vjasscExperimental": {
    "parallel": true,
    "workers": 4,
    "incremental": true,
    "recordedOrder": true
  },
  "incremental": {
    "chunkCount": 13255,
    "reusedChunks": 13240,
    "reusePercent": 99.8
  },
  "parallel": {
    "workers": 4,
    "jobs": 5900,
    "speedupRatio": 1.7
  }
}
```

---

## 11. 测试计划

### 11.1 Golden fixtures

新增：

```text
phase21_generated_entity_plan_stability
phase21_recorded_order_basic
phase21_recorded_order_cycle
phase21_parallel_output_stability
phase21_incremental_nochange
phase21_incremental_small_body_change
phase21_incremental_symbol_change_fallback
```

### 11.2 Standalone validation

必须通过：

```bat
ctest --test-dir build --output-on-failure
```

必须通过：

```bat
build\vjassc.exe samples\input.j ^
  -o build\phase21.validate.out.j ^
  --mode validate ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

### 11.3 Experimental matrix

至少跑：

```text
single fast
recorded-order fast
parallel fast workers=4
incremental no-change fast
parallel + incremental no-change fast
```

并记录：

```text
output equality
PJASS
timing median
cache reuse
dependency coverage
```

---

## 12. 验收目标

### 12.1 最低目标

```text
默认模式：
  correctness 不退化。
  fast median 不比 Phase 20 明显慢。

recorded-order：
  PJASS pass。
  functionOrderTokenScans < 15000。

parallel experimental：
  output byte-identical。
  PJASS pass。
  median speedup >= 15%。

incremental experimental:
  no-change output byte-identical。
  no-change reusePercent >= 95%。
  no-change standalone fast <= 2500 ms。
```

### 12.2 良好目标

```text
parallel:
  median speedup >= 30%。

incremental:
  no-change standalone fast <= 1500 ms。
  small-change standalone fast <= 2500 ms。

War3Lib ALPHA:
  incremental fast <= 3500 ms。
```

### 12.3 优秀目标

```text
parallel + incremental:
  no-change standalone fast <= 1200 ms。
  no-change War3Lib ALPHA vjassc compiler segment <= 2000 ms。
```

---

## 13. 输出文档要求

必须新增：

```text
docs/phase21_status.md
```

内容包括：

```text
Implemented
Standalone Results
Experimental Recorded Order
Experimental Parallel Lowering
Experimental Incremental Cache
War3Lib / Xlimon ALPHA Results
Correctness Matrix
Performance Median Report
Known Issues
Next Phase Recommendation
```

必须说明：

```text
哪些实验默认关闭
哪些可以考虑开启
哪些仍不建议部署
```

---

## 14. Codex 执行建议

不要一次性实现所有大功能后才测试。建议顺序：

```text
Batch 1:
  GeneratedEntityPlan + BodyJob 单线程模型。
  验证 output 稳定。

Batch 2:
  Recorded dependency graph experimental ordering。
  验证 PJASS。

Batch 3:
  Parallel lowering 实验。
  验证 byte-identical。

Batch 4:
  Incremental cache no-change reuse。
  验证 byte-identical。

Batch 5:
  Small-change incremental。
  验证 PJASS。

Batch 6:
  War3Lib ALPHA experimental integration。
```

每个 batch 都要有小 fixture，不要只靠 100k 行地图测试。

---

## 15. 重要提醒

本阶段的目标不是马上把默认 cold compile 压到 1 秒。

本阶段真正目标是建立：

```text
确定性 ID
可并行 body lowering
可复用 body cache
可记录 dependency graph
可回退实验开关
```

如果这些基础建立成功，Phase 22 才可以考虑：

```text
默认开启 recorded-order
默认开启 parallel lowering
ALPHA 默认开启 incremental cache
```

最终的 1–2 秒体验主要应来自：

```text
日常小改增量编译
```

而不是继续死磕单线程全量冷编译。
