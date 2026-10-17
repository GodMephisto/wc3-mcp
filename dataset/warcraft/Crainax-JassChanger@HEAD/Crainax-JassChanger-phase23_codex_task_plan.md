---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase23_codex_task_plan.md"
ref: HEAD
---

阶段23详细任务报告：真实并行 BodyJob + Body-Level 增量缓存落地
项目：Crainax/JassChanger
目标分支建议：codex/phase23-real-parallel-incremental
输出文件：phase23_task_report.txt

============================================================
一、阶段背景与当前状态
============================================================

当前 JassChanger 已经完成 Phase 22，并进入“并行 / 增量编译真正落地”的前置状态。

Phase 22 的核心结论：

1. 默认 fast 冷编译已经稳定在约 5 秒级。
2. vjassc 在 War3Lib/Xlimon ALPHA fast compare 中已经显著快于 JassHelper。
3. PJASS、syntax-lite、init validation 均保持通过。
4. recorded-order 已经有安全 fallback，但强边覆盖仍不足。
5. parallel-lowering 目前只是 deterministic metadata / job scaffold，并没有真正并发执行 body lowering。
6. incremental cache 当前能做到 no-change 极速复用，但 small-change 仍然 fallback 到 cold compile，并没有复用 body lowering 结果。
7. War3Lib 当前还没有把 Phase 22 的实验开关完整传入 vjassc，需要单独接入。

Phase 22 关键数据：

- default fast median: 约 5268 ms
- parallel workers=4 median: 约 5381 ms
- incremental no-change median: 约 239 ms
- no-change no-report hot path: 约 61 ms
- small-change reusePercent: 99.9925%，但 cacheHit=false，仍然 cold compile
- recorded edges: 2220
- conservative output-scan edges: 16232
- missing recorded edges: 14013
- recorded-order fallback: true

因此，Phase 23 不能继续做“冷编译微优化”，而应该真正推动两条路线：

A. 真正并行 BodyJob lowering
B. 真正 body-level incremental cache

============================================================
二、阶段23总目标
============================================================

Phase 23 的主目标不是继续追逐 5 秒级冷编译的几百毫秒波动，而是完成可验证、可回退、可渐进推广的并行与增量原型。

核心目标：

1. 将 function / struct source method / lambda 的 body lowering 封装成不可变输入、线程局部输出的 BodyJob。
2. 实现实验性真实并行 lowering，使 --experimental-parallel-lowering 不再只是 metadata path。
3. 实现 body-level incremental cache，使 small-change 能真正复用未变化函数/方法/lambda 的 lowering 结果，而不是只报告复用率。
4. 保持默认路径完全稳定，所有实验能力默认关闭。
5. 保持 PJASS pass、syntax-lite pass、init validation pass。
6. 接入 War3Lib ALPHA 环境变量，使并行/增量可以在 ALPHA 模式显式试用。
7. 生成详细报告，用于判断是否可在后续阶段推广默认启用。

============================================================
三、阶段边界
============================================================

本阶段应该做：

- BodyJob 数据模型
- BodyJobResult 数据模型
- 严格 deterministic ID ownership
- 并行 worker pool 原型
- 并行输出稳定性验证
- body-level incremental cache 原型
- small-change 实际复用
- War3Lib ALPHA 实验开关接入
- 详细 benchmark/report
- PJASS / syntax-lite / init / byte-identical 校验

本阶段不应该做：

- 不要默认启用 parallel-lowering
- 不要默认启用 recorded-order
- 不要默认启用 small-change incremental cache
- 不要为了速度牺牲 PJASS 或 runtime correctness
- 不要让 worker 线程写入全局 codegen 状态
- 不要一次性重写整个 Phase1Codegen
- 不要把 incremental cache 用于 globals/init/runtime wrapper 这类全局 assembly，除非已经严格验证依赖

============================================================
四、阶段23核心任务拆分
============================================================

------------------------------------------------------------
任务1：建立 BodyJob / BodyJobResult 模型
------------------------------------------------------------

目标：

把当前 function/method/lambda body lowering 从“直接写 writer_ / 修改全局状态”的模式，逐步改成：

BodyJob 输入不可变
BodyJobResult 输出线程局部
主线程按稳定顺序合并

建议新增结构：

enum class BodyJobKind {
    Function,
    StructMethod,
    Lambda,
    GeneratedFunction
};

struct BodyJobId {
    uint32_t value;
};

struct BodyJob {
    BodyJobId id;
    BodyJobKind kind;
    std::string stableKey;

    const Decl* decl = nullptr;
    const MethodDecl* methodDecl = nullptr;
    const StructInfo* currentStruct = nullptr;
    const Decl* container = nullptr;

    SyntaxMode syntaxMode;
    BodyMode bodyMode;

    std::vector<std::string> sourceLines;

    // 只读索引引用
    const CodegenIndex* index = nullptr;

    // 预分配实体引用
    std::vector<size_t> lambdaIds;
    std::vector<size_t> interfaceTargetIds;
    std::vector<size_t> bridgeIds;
};

struct BodyJobResult {
    BodyJobId id;
    std::string stableKey;
    bool ok = true;

    std::vector<std::string> outputLines;
    std::vector<std::string> localDeclarations;
    std::vector<DependencyEdge> dependencyEdges;
    std::vector<GeneratedRequest> generatedRequests;

    CodegenPerformanceCounters counters;
    std::vector<Diagnostic> diagnostics;
};

要求：

1. BodyJob 执行期间不得直接写 writer_。
2. BodyJob 执行期间不得修改 functionInterfaces_、lambdas_、functions_ 等全局向量。
3. 所有需要新增的 wrapper/bridge/interface target，必须在预分配阶段确定 ID。
4. Job 结果必须可以稳定排序合并。
5. 单线程 BodyJob 路径输出必须和现有默认路径 byte-identical。

验收：

- 新增 --experimental-body-jobs-single-thread。
- samples/input.j 下：
  - default fast output == body-jobs single-thread output
  - PJASS pass
  - generated entity plan JSON 相同或可解释一致
  - ctest pass

------------------------------------------------------------
任务2：Deterministic Generated Entity Ownership
------------------------------------------------------------

目标：

为并行和增量提前解决 ID 稳定问题。

需要预分配的实体：

1. lambda id
2. generated lambda function name
3. function-interface target id
4. function-interface wrapper name
5. runtime bridge id/name
6. method caller wrapper name
7. cycle bridge name
8. generated support function stable key

建议新增 GeneratedEntityPlan：

struct GeneratedEntityPlan {
    std::vector<LambdaEntity> lambdas;
    std::vector<InterfaceTargetEntity> interfaceTargets;
    std::vector<WrapperEntity> wrappers;
    std::vector<BridgeEntity> bridges;
    std::vector<GeneratedSupportEntity> generatedSupport;
};

要求：

1. 预扫描 AST / body token，不执行 lowering。
2. 所有 entity 都有 stableKey。
3. entity 输出顺序必须 deterministic。
4. 单线程、parallel workers=1/2/4/8 生成的 plan 必须 byte-identical。
5. 增量 cache key 必须包含相关 entity plan 版本或 hash。

验收：

- --emit-generated-entity-plan 输出稳定。
- repeated x3 JSON hash 一致。
- parallel workers=1/2/4/8 JSON hash 一致。
- 修改一个函数体时，只相关 body chunk hash 改变，entity plan 不应无意义大面积变化。

------------------------------------------------------------
任务3：真实并行 BodyJob Lowering
------------------------------------------------------------

目标：

让 --experimental-parallel-lowering 真正并发执行 body lowering，而不是只生成 metadata。

建议实现步骤：

1. 主线程 collect：
   - AST / symbols / struct index / function index / interface index
   - GeneratedEntityPlan
   - BodyJob list

2. worker pool 执行：
   - function body lowering
   - struct source method body lowering
   - lambda body lowering

3. 主线程合并：
   - 按 BodyJobId / stable order 合并 output lines
   - 合并 dependency edges
   - 合并 generated requests
   - 合并 diagnostics
   - 更新 performance counters

默认仍关闭：

--experimental-parallel-lowering
--parallel-workers 4

要求：

1. workers=1 输出必须等于默认输出。
2. workers=2/4/8 输出必须和 workers=1 byte-identical。
3. repeated x3 输出 byte-identical。
4. validate + PJASS pass。
5. 如果发现任何 nondeterministic 输出，自动 fallback 到 single-thread，并在 report 中记录。

建议先并行范围：

第一步只并行：
- 普通 function body
- struct source method body
- lambda body

暂不并行：
- globals
- native/type emission
- init helper
- function-interface runtime final assembly
- struct generated support
- final ordering
- final output validation

验收目标：

最低：
- parallel workers=4 PJASS pass
- output byte-identical
- median 不慢于 default fast 超过 10%

良好：
- parallel workers=4 median 比 default fast 快 >= 15%

优秀：
- parallel workers=4 median 比 default fast 快 >= 30%

注意：

如果 Phase 23 并行仍然没有提速，但输出稳定和 JobResult 模型完成，也可以接受。真实提速可能在 Phase 24 扩大并行范围后体现。

------------------------------------------------------------
任务4：Body-Level Incremental Cache
------------------------------------------------------------

目标：

把 Phase 22 的 no-change final output cache，推进为 small-change body-level cache。

当前状态：

- no-change 可以直接复用最终输出，非常快。
- small-change 能识别 changedChunks=1，但仍 cold compile。
- Phase 23 要让 small-change 真正复用 unchanged body lowering output。

建议缓存粒度：

1. FunctionBodyCache
2. StructMethodBodyCache
3. LambdaBodyCache

缓存内容：

struct BodyCacheEntry {
    std::string stableKey;
    std::string sourceHash;
    std::string symbolHash;
    std::string entityPlanHash;
    std::string compilerVersion;
    std::string optionsHash;

    std::vector<std::string> outputLines;
    std::vector<std::string> localDeclarations;
    std::vector<DependencyEdge> dependencyEdges;
    std::vector<GeneratedRequest> generatedRequests;
    CodegenPerformanceCounters counters;
};

cache key 必须包含：

- source body text hash
- body kind
- stable symbol identity
- current struct stable key
- relevant local/global/struct/function/interface symbol hash
- generated entity plan hash
- vjassc version
- relevant CLI options
- warnMode / debug / mode / compatibility flags

默认关闭：

--experimental-incremental-cache <dir>
--incremental-mode report|reuse

实现策略：

1. no-change：
   - 继续允许 final output direct-copy hot path。
   - 这是最快路径，不要破坏。

2. small-change：
   - 如果 final output cache miss，进入 body-level cache。
   - unchanged BodyJob 直接读取 cached BodyJobResult。
   - changed BodyJob 重新 lower。
   - 主线程重新 assembly。
   - PJASS 验证通过后写新 cache。

3. 失败回退：
   - 任意 cache entry 校验失败，fallback 到 cold BodyJob lowering。
   - 不允许使用不完整缓存生成正式输出。

验收：

- no-change:
  - output byte-identical
  - reusePercent=100
  - median <= 500ms

- one-function small-change:
  - changed body count <= 3
  - reused body count >= 95%
  - output PJASS pass
  - median 比 cold fast 快 >= 30%

- one-struct-method small-change:
  - PJASS pass
  - reused body count >= 95%

- one-lambda small-change:
  - PJASS pass
  - generated lambda IDs 稳定或有可解释变更

------------------------------------------------------------
任务5：Recorded Dependency Graph 推进
------------------------------------------------------------

目标：

尽量减少或替代 function-order output scan，为并行/增量服务。

Phase 22 现状：

- recorded edges: 2220
- conservative output-scan edges: 16232
- missing recorded edges: 14013
- fallback: true

Phase 23 目标：

1. 为 BodyJobResult 记录 dependency edges。
2. 记录普通 call edge。
3. 记录 function reference edge。
4. 记录 lambda generated target edge。
5. 记录 function-interface wrapper edge。
6. 记录 struct generated support edge。
7. 记录 initializer / onInit / static onInit edge。
8. 记录 bridge / method caller wrapper edge。
9. 对 ExecuteFunc("name") 标记 weak edge，不作为普通强排序边。

建议 edge 类型：

enum class DependencyEdgeKind {
    DirectCall,
    FunctionReference,
    LambdaCall,
    FunctionInterfaceWrapper,
    StructSupport,
    Initializer,
    ExecuteFuncWeak,
    Bridge,
    MethodCaller,
};

验收：

最低：
- recorded coverage >= 35%
- fallback 仍安全
- PJASS pass

良好：
- recorded coverage >= 60%
- missing recorded edges <= 6500

优秀：
- recorded coverage >= 80%
- output scan fallback 可在实验模式关闭并 PJASS pass

注意：

Recorded-order 默认仍关闭。只有在 coverage 足够且 real maps 多轮验证后再考虑推广。

------------------------------------------------------------
任务6：War3Lib / Xlimon ALPHA 实验接入
------------------------------------------------------------

目标：

让 War3Lib 能把 Phase 23 实验参数传给 vjassc，但只允许 ALPHA 显式启用。

需要在 War3Lib 单独 clean branch 修改，不要混在 JassChanger commit 中。

建议新增环境变量：

WAR3_VJASSC_RECORDED_ORDER=0|1
WAR3_VJASSC_PARALLEL=0|1
WAR3_VJASSC_PARALLEL_WORKERS=4
WAR3_VJASSC_INCREMENTAL=0|1
WAR3_VJASSC_INCREMENTAL_MODE=report|reuse
WAR3_VJASSC_INCREMENTAL_DIR=<path>
WAR3_VJASSC_EMIT_BENCHMARK=0|1

映射到 vjassc CLI：

if RECORDED_ORDER:
  --experimental-recorded-order

if PARALLEL:
  --experimental-parallel-lowering --parallel-workers N

if INCREMENTAL:
  --experimental-incremental-cache DIR --incremental-mode MODE

安全限制：

1. 只允许 path.buildVersion == "内测版本" 时启用。
2. 非 ALPHA 除非 WAR3_ALLOW_VJASSC_NON_ALPHA=1，否则拒绝。
3. 实验失败自动 fallback 到默认 vjassc fast/validate。
4. 默认仍不启用 parallel/recorded-order/incremental。

验收：

- TaskCompileCompareAlpha.lua 可以传递实验参数。
- TaskCompileAlphaWithVjassc.lua 可以显式启用 incremental no-change。
- ALPHA validate + PJASS pass。
- ALPHA fast compare pass。
- report 中记录实际使用的 experimental flags。

------------------------------------------------------------
任务7：Benchmark 方法标准化
------------------------------------------------------------

目标：

减少本地 4.x / 5.x 秒波动造成的误判。

要求新增或增强 bench 脚本：

tools/bench_phase23.ps1

功能：

1. warmup N 次。
2. repeat N 次。
3. 输出 min / median / p75 / p90 / max。
4. 支持场景：
   - default fast
   - body-jobs single-thread
   - parallel workers=2
   - parallel workers=4
   - incremental no-change
   - incremental small-change
   - validate
5. 输出 JSON report。

建议默认：

warmup=1
repeat=7

判断规则：

- 只看 median。
- 小于 5% 的提升不作为有效性能结论。
- 并行/增量功能以 correctness + determinism 优先，速度只作为第二指标。

============================================================
五、Phase 23 验收标准
============================================================

必须满足：

1. cmake build pass
2. ctest pass
3. default validate + PJASS pass
4. body-jobs single-thread output byte-identical
5. parallel workers=1/2/4 output byte-identical 或自动 fallback
6. incremental no-change output byte-identical
7. incremental small-change PJASS pass
8. War3Lib ALPHA default/fast compare 不回退

性能最低目标：

- default fast median 不明显回退，允许 +-10%
- incremental no-change median <= 500ms
- small-change incremental median 比 cold fast 快 >= 30%，如果 body cache 实现完成
- parallel workers=4 不比 default fast 慢 >10%

良好目标：

- parallel workers=4 比 default fast 快 >= 15%
- small-change incremental median <= 2500ms
- recorded dependency coverage >= 60%

优秀目标：

- parallel workers=4 比 default fast 快 >= 30%
- one-function small-change incremental <= 1500ms
- no-change incremental <= 250ms
- recorded-order experimental PJASS pass without fallback

============================================================
六、风险与禁止事项
============================================================

禁止：

1. 不允许 parallel worker 修改全局 mutable codegen 状态。
2. 不允许 incremental cache 在校验失败时继续生成正式输出。
3. 不允许 recorded-order 在 missing strong edges 时强行使用。
4. 不允许默认启用 experimental parallel/incremental。
5. 不允许为了 byte-identical 牺牲 PJASS 或 runtime 行为。
6. 不允许把 War3Lib 非 ALPHA 流程默认切到实验模式。

风险：

1. 并行会引入 nondeterministic 输出。
2. 增量 cache key 不完整会导致旧 body 误复用。
3. lambda/interface target ID 分配若不稳定，会导致 wrapper 对不上。
4. dependency recorder 覆盖不足会产生前向引用。
5. 小改动可能影响 init/global/runtime wrapper，不能只看 body hash。

缓解：

1. 所有实验默认关闭。
2. 所有实验输出都要 PJASS pass。
3. 所有实验必须支持 fallback。
4. 所有实验都写 report。
5. War3Lib 只允许 ALPHA 显式启用。

============================================================
七、建议执行顺序
============================================================

Step 1:
  完成 BodyJob / BodyJobResult 模型。
  先跑 single-thread equivalence。

Step 2:
  完成 deterministic entity ownership plan。
  验证 repeated / parallel plan JSON 稳定。

Step 3:
  实现并行 worker pool。
  只并行普通 function body，验证 byte-identical。

Step 4:
  扩大并行到 struct source method 和 lambda。
  验证 workers=1/2/4/8。

Step 5:
  实现 body-level incremental cache。
  no-change 保持 direct-copy hot path。
  small-change 开始复用 body cache。

Step 6:
  增强 dependency recorder edges。
  保留 fallback。

Step 7:
  接入 War3Lib ALPHA 环境变量。
  只在 clean branch 修改 War3Lib。

Step 8:
  完成 benchmark report 和 phase23_status.md。

============================================================
八、Phase 23 状态文档要求
============================================================

Codex 完成后必须写：

docs/phase23_status.md

内容必须包含：

1. Implemented 列表
2. CLI 新增或变更
3. Correctness Matrix
4. Determinism Matrix
5. Parallel Results
   - workers=1/2/4/8
   - output hash
   - median benchmark
6. Incremental Results
   - no-change
   - one-function small-change
   - one-method small-change
   - one-lambda small-change
   - reusePercent
   - cacheHit / bodyCacheHit
7. Dependency Recorder Results
   - recordedEdges
   - outputScanEdges
   - matchedEdges
   - missingEdges
   - coveragePercent
   - fallback
8. War3Lib / Xlimon ALPHA Results
9. Performance Median Report
10. Known Issues
11. Promotion Recommendation
   - parallel: default off / candidate / promotable
   - incremental: default off / ALPHA manual / promotable
   - recorded-order: default off / candidate / promotable

============================================================
九、结论
============================================================

Phase 23 可以开始实施并行和增量，但要明确：

1. 并行当前还没有真正执行 body lowering，Phase 23 才是实装阶段。
2. 增量当前只有 no-change final-output cache，Phase 23 要做 body-level small-change cache。
3. recorded-order 目前安全但覆盖低，仍不能默认启用。
4. War3Lib 需要单独 clean change 才能把实验参数传入 vjassc。
5. 所有实验能力都必须默认关闭，只在 ALPHA 显式启用。

如果 Phase 23 成功，项目将从“冷编译 5 秒级”进入：
- no-change 亚秒级
- small-change 1~3 秒级
- 并行 cold compile 3~4 秒级

这将比继续做单线程微优化更有价值。
