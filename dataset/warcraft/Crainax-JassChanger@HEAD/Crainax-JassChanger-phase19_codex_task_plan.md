---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase19_codex_task_plan.md"
ref: HEAD
---

# Phase 19 Codex 实施计划：依赖记录覆盖、MethodPlan 缓存、并行/增量编译可行性落地

> 项目：Crainax/JassChanger
> 目标阶段：Phase 19
> 主要目标：在保持 PJASS 通过、War3Lib/Xlimon ALPHA 可编译的前提下，继续压缩 fast/validate 编译耗时，并为并行 lowering 与分段/增量编译建立可验证的第一版基础设施。
> 当前建议优先级：**先做依赖记录覆盖 + MethodPlan/MethodBody 缓存 + token cache 复用深化；并行 lowering 做小范围实验；增量编译先做 after-CompileStep4 的 chunk hash/report，不要一上来替换正式输出。**

---

## 0. 当前项目状态基线

Phase 18 已经完成：

- real sample PJASS 继续通过，`groupedCount = 0`。
- War3Lib/Xlimon ALPHA validate 与 fast compare 都通过。
- vjassc fast 已经明显快于 JassHelper：`vjassc fast elapsedMs = 6435 ms`，`jasshelper elapsedMs = 11943 ms`。
- BodyMode routing 已落地：Zinc / JASS-like / generated bodies 分流。
- TokenCache 已初步复用，减少 line feature scan 与 function ordering token scan。
- sanitizeOutput fast 从约 1427ms 降到约 244ms。

Phase 18 代表性 standalone 数据：

```text
fast total:            7323 -> 6106 ms
validate total:        8862 -> 7870 ms
full-validation total: 9558 -> 8892 ms
```

Phase 18 代表性 counters：

```text
lineFeatureScans:             96847 -> 49364
functionOrderTokenScans:     110035 -> 56249
arrayAccessRewriteAttempts:   18277 -> 12671
functionLookupCalls:          20270 -> 20174
structLookupCalls:            30501 -> 18696
sourceMethods fast:            2171 -> 2278 ms
sanitizeOutput fast:           1427 -> 244 ms
```

Phase 18 body routing / token cache 数据：

```text
bodyModes.zincFunctions: 3498
bodyModes.jassLikeFunctions: 920
bodyModes.zincMethods: 1540
bodyModes.jassLikeMethods: 0
bodyModes.generatedBodies: 86
bodyLowerer.generatedLinesSkippedGenericLowering: 963
modeFastPath.heavyLoweringAvoidedByMode: 49385

tokenCacheBuilds: 56277
tokenCacheHits: 108061
featureScansAvoided: 54357
functionOrderScansAvoided: 49411
```

当前主要剩余瓶颈：

```text
1. sourceMethods 仍约 2.1~2.4 秒。
2. function dependency recorder 覆盖不足：recordedEdges 2104，但 outputScanEdges 16130，missingRecordedEdges 14026。
3. validate 模式 syntaxLite 仍约 1.7~2.0 秒。
4. fast 冷编译已到 6 秒级，继续靠单点 regex 优化收益会变小。
5. 若目标是 1~2 秒日常开发体验，需要引入并行 lowering 或增量/分段编译。
```

---

## 1. Phase 19 总目标

### 1.1 最低验收目标

```text
standalone fast total <= 5200 ms
standalone validate total <= 7000 ms
standalone full-validation total <= 8200 ms
War3Lib/Xlimon ALPHA fast vjassc elapsedMs <= 5800 ms
War3Lib/Xlimon ALPHA validate vjassc elapsedMs <= 7600 ms
PJASS 继续通过，groupedCount == 0
syntax-lite 继续通过，issueCount == 0
ALPHA 编译任务继续 pass
```

### 1.2 良好目标

```text
standalone fast total <= 4500 ms
standalone validate total <= 6200 ms
War3Lib/Xlimon ALPHA fast vjassc elapsedMs <= 5000 ms
sourceMethods <= 1500 ms
functionOrderTokenScans <= 25000
missingRecordedEdges <= 6000
syntaxLite validate <= 1300 ms
```

### 1.3 优秀目标

```text
standalone fast total <= 3500 ms
War3Lib/Xlimon ALPHA fast vjassc elapsedMs <= 4000 ms
parallel lowering experimental mode 可稳定通过 PJASS
incremental/chunk cache 能生成准确命中率报告，并证明小改场景可复用 70%+ chunks
```

---

## 2. 非目标与安全边界

Phase 19 不应做以下事情：

```text
- 不要推翻 Phase1Codegen 全部架构。
- 不要让 PJASS pass 退化。
- 不要取消 fast 模式 statement-shape guard。
- 不要把 incremental cache 直接作为默认正式输出路径。
- 不要让并行 lowering 改变 lambda id、function-interface target id、bridge id 的稳定性。
- 不要为了性能删除 validation/full-validation 需要的报告能力。
```

Phase 19 的正确节奏：

```text
先提升单线程冷编译结构效率；
再做并行 lowering 实验开关；
最后做增量/分段编译分析与只读缓存报告。
```

---

## 3. 任务 A：Function Dependency Recorder 覆盖率提升

### 3.1 当前问题

Phase 18 中：

```text
recordedEdges: 2104
outputScanEdges: 16130
matchedEdges: 2104
missingRecordedEdges: 14026
```

说明 dependency recorder 还只覆盖少量边，大量依赖仍靠 output scan 找。output scan 需要扫描大量函数输出，虽已减少，但仍是后续并行/增量的阻碍。

### 3.2 目标

将依赖边在 lowering / emit 阶段尽量记录下来，而不是最终输出后扫描。

最低目标：

```text
missingRecordedEdges <= 9000
functionOrderTokenScans <= 40000
```

良好目标：

```text
missingRecordedEdges <= 6000
functionOrderTokenScans <= 25000
```

### 3.3 实施内容

新增或扩展结构：

```cpp
struct DependencyRecorder {
    void beginFunction(FunctionId id);
    void recordCall(FunctionId target, DependencyKind kind, SourceLocation loc);
    void recordFunctionRef(FunctionId target, SourceLocation loc);
    void recordExecuteFunc(std::string_view targetName, SourceLocation loc);
    void endFunction();
};

enum class DependencyKind {
    DirectCall,
    FunctionReference,
    TriggerAction,
    TriggerCondition,
    ExecuteFuncString,
    InterfaceWrapper,
    LambdaWrapper,
    Bridge,
};
```

在以下位置记录依赖：

```text
- lowerStatementLine 处理 call Foo(...) 时。
- lowerExpression 处理 Foo(...) 调用时。
- lowerFunctionValue 处理 function Foo / function Struct.method 时。
- registerInterfaceTarget / function-interface wrapper 生成时。
- lambda lowering 生成 wrapper 调用时。
- bridge/cycle rewrite 生成 bridge target 时。
- emitStructMethod / emitJassFunction / emitZincFunction 输出函数块时。
```

### 3.4 不确定项

`ExecuteFunc("Foo")` 是否应计入强依赖？

建议：

```text
- 对 ExecuteFunc 字符串只记录 weak dependency，不参与必须前置排序。
- 对 JassHelper 风格 init ExecuteFunc 不强制排序。
- 对 cycle bridge 的 ExecuteFunc 保持现有策略。
```

### 3.5 验收

新增 stats：

```json
{
  "dependencyRecorder.recordedEdges": 0,
  "dependencyRecorder.outputScanEdges": 0,
  "dependencyRecorder.missingRecordedEdges": 0,
  "dependencyRecorder.weakExecuteFuncEdges": 0,
  "dependencyRecorder.coveragePercent": 0.0
}
```

要求：

```text
- PJASS 通过。
- function ordering 结果稳定。
- missingRecordedEdges 明显下降。
- 若 recorder 覆盖不完整，仍保留保守 output scan fallback。
```

---

## 4. 任务 B：MethodPlan / MethodBody rewrite cache

### 4.1 当前问题

Phase 18 后 `sourceMethods` 仍约 2.2~2.4 秒，是当前最大单项热点。

### 4.2 目标

为 struct method 建立一次性分析计划，避免每行重复判断：

```text
- 当前 struct 有哪些字段/方法。
- 当前 method 有哪些 params/local，会 shadow 字段。
- 哪些 bare identifiers 可能是 field/method/static field。
- 哪些行可能需要 this/bare field rewrite。
- 哪些行可能需要 array rewrite / receiver-chain rewrite / function-interface lowering。
```

### 4.3 新增结构

```cpp
struct MethodLoweringPlan {
    const StructInfo* currentStruct = nullptr;
    const MethodInfo* method = nullptr;

    std::unordered_set<NameId> params;
    std::unordered_set<NameId> locals;
    std::unordered_set<NameId> shadowedNames;

    std::unordered_map<NameId, const FieldInfo*> bareFieldCandidates;
    std::unordered_map<NameId, const MethodInfo*> bareMethodCandidates;
    std::unordered_map<NameId, const FieldInfo*> staticFieldCandidates;

    bool mayUseThis = false;
    bool mayUseThistype = false;
    bool mayUseBareField = false;
    bool mayUseBareMethod = false;
    bool mayUseArrayAccess = false;
    bool mayUseReceiverChain = false;
    bool mayUseLambda = false;
    bool mayUseFunctionInterface = false;
};
```

若暂时没有 `NameId`，可先用 `std::string_view` 或 interned string wrapper。

### 4.4 实施步骤

1. 在 `emitStructMethod` 之前建立 `MethodLoweringPlan`。
2. 先扫描 method body local declarations，构建 local/param shadow 集合。
3. 根据 current struct fields/methods 构建 candidate 集合。
4. 每行 lowering 前先看 plan + LineTokenCache：
   - 不含 candidate identifier，跳过 bare field/method rewrite。
   - 不含 `[` 且 method 没有 known array receiver，跳过 array rewrite。
   - 不含 `.` 且不含 `this` / `thistype`，跳过 receiver-chain rewrite。
5. 将 method plan 写入 stats。

### 4.5 验收

目标：

```text
sourceMethods <= 1700 ms
良好：sourceMethods <= 1400 ms
```

新增 stats：

```json
{
  "methodPlan.built": 0,
  "methodPlan.linesSkippedNoCandidate": 0,
  "methodPlan.bareFieldRewriteAttempts": 0,
  "methodPlan.bareFieldRewriteChanged": 0,
  "methodPlan.shadowSkips": 0
}
```

---

## 5. 任务 C：Token-level bare field / method rewriter

### 5.1 当前问题

当前 bare field / method rewrite 仍可能通过字符串扫描和多轮 rewrite 完成。即使 regex 降低了，仍会反复遍历 line。

### 5.2 目标

在 method body 内对 token 做语义判断：

```text
identifier 是 local/param -> 不改
identifier 是 currentStruct.field -> 改为 field array access
identifier 是 currentStruct.method 且 call form -> 改为 generated method call
identifier 是 static field -> 改为 generated static field
```

### 5.3 示例

输入：

```jass
set count = count + 1
call refresh(pid)
call destroy()
```

如果 `count` 是字段，`refresh` 是 method：

```jass
set s__Struct_count[this] = s__Struct_count[this] + 1
call s__Struct_refresh(this, pid)
call s__Struct_destroy(this)
```

如果 `count` 是 local：不改。

### 5.4 实施限制

Phase 19 不要求完整表达式 AST，但要求 token-level 安全：

```text
- 不改字符串/注释/rawcode 内 token。
- 不改 field 被 local/param shadow 的情况。
- 不改已生成的 s__/sc__/si__ 名称。
- 不改 namespace/global function 名称。
```

### 5.5 验收 fixtures

新增 fixtures：

```text
phase19_method_plan_bare_field.in.j
phase19_method_plan_shadow_local.in.j
phase19_method_plan_bare_method.in.j
phase19_method_plan_static_field.in.j
phase19_method_plan_string_rawcode_guard.in.j
```

---

## 6. 任务 D：Array rewrite 精准过滤

### 6.1 当前状态

Phase 18：

```text
arrayAccessRewriteAttempts: 12671
arrayAccessRewriteChanged: 7351
```

命中率已经比之前高，但仍可优化。

### 6.2 目标

只有当 `identifier[` 的 identifier 是 known array receiver 时才进入复杂 array rewrite。

建立集合：

```cpp
KnownArrayReceivers {
    global array shapes,
    local array shapes,
    struct fixed-array fields,
    generated struct array names,
}
```

### 6.3 验收

```text
arrayAccessRewriteAttempts <= 9500
arrayAccessRewriteChanged 保持约 7300，不应明显下降
```

若 changed 明显下降，说明过滤误伤，需要回退。

---

## 7. 任务 E：Syntax-lite 与 TokenCache 进一步共享

### 7.1 当前问题

validate 模式 syntaxLite 约 1.7~2.0 秒。

### 7.2 目标

减少 syntax-lite 独立重新扫描：

```text
- 复用 codegen 阶段已记录的 statement-shape flags。
- 复用 function table / duplicate name 信息。
- 复用 dependency recorder 的 forward reference 信息。
- full-validation 才做更完整的 output text scan。
```

### 7.3 模式区分

```text
fast:
  不跑 syntax-lite，但保留 cheap statement-shape guard。

validate:
  跑 core syntax-lite：duplicate、source residue、PJASS 必要辅助检查。

full-validation:
  跑完整 syntax-lite + examples + comparison。
```

### 7.4 验收

```text
validate syntaxLite <= 1300 ms
full-validation syntaxLite <= 1800 ms
```

---

## 8. 任务 F：并行 lowering 可行性实验

### 8.1 是否可以实施？

可以做 **实验版**，但不建议 Phase 19 直接默认开启。

原因：

```text
- 当前 codegen 仍有全局可变状态：lambda id、function-interface targets、generated wrappers、bridge requests。
- 若直接并行可能造成 id 顺序不稳定、输出顺序不稳定、runtime 差异。
- 但函数体/方法体本身 lower 是可并行的，只要预分配 id 和延迟合并。
```

### 8.2 Phase 19 建议范围

新增开关：

```bash
--experimental-parallel-lowering
--parallel-lowering-threads N
```

默认关闭。

### 8.3 并行实验对象

优先只并行 **普通 function body lowering**，暂不并行 struct methods / lambda / function interface runtime。

理由：

```text
普通 function 独立性更高。
struct methods 仍是 sourceMethods 热点，但依赖 currentStruct/global state 更多。
先用普通 function 验证架构与 determinism。
```

### 8.4 设计

主线程：

```text
- collect symbols
- collect functions
- allocate FunctionId
- allocate output slot
```

并行 worker：

```text
- 输入 FunctionLoweringJob
- 输出 FunctionLoweringResult
- 不直接写 writer_
- 不修改全局 functionInterfaces_
- 不分配 lambda id，若发现 lambda，记录 pending lambda request
```

合并阶段：

```text
- 按 originalIndex 合并 output
- 统一处理 pending lambda requests
- 统一记录 dependency edges
```

### 8.5 验收

实验开关下：

```text
- standalone fast PJASS 后续 validate-existing-output 通过。
- 输出与单线程 fast 在 normalized form 下等价，至少 function/global/native count 一致。
- 不能作为默认模式。
```

性能目标：

```text
普通 function lowering 并行后，fast 总耗时下降 >= 10% 才保留。
```

如果收益不足或不稳定，保留调研报告，不默认启用。

---

## 9. 任务 G：分段/增量编译可行性评估

### 9.1 是否可以实施？

可以开始做 **只读分析与 cache report**，但不建议 Phase 19 直接用增量输出替换正式输出。

原因：

```text
- Wave / Inject / CompileLua 之后的 CompileStep4 才是稳定输入。
- textmacro/static if/library ordering 可能影响 chunk 边界。
- function-interface target id、lambda id、bridge id 需要稳定编号策略。
```

### 9.2 增量边界

增量缓存应建立在 War3Lib 的：

```text
Output/4_luaexecute.j
```

而不是原始源码目录。

### 9.3 Chunk 划分策略

第一版只做分析：

```text
ChunkKind:
- top-level function
- struct method
- struct declaration metadata
- globals block
- native/type block
- library metadata
```

每个 chunk 记录：

```cpp
struct ChunkInfo {
    ChunkId id;
    ChunkKind kind;
    std::string ownerLibrary;
    std::string ownerStruct;
    std::string name;
    uint64_t contentHash;
    SourceSpan span;
    std::vector<FunctionId> deps;
};
```

### 9.4 Cache report

新增命令或选项：

```bash
vjassc input.j --emit-incremental-report build/incremental.json
```

或在 stats 中加入：

```json
{
  "incremental": {
    "chunksTotal": 0,
    "chunksHashStable": 0,
    "functions": 0,
    "methods": 0,
    "globals": 0,
    "estimatedReusableChunks": 0,
    "cacheKey": "..."
  }
}
```

### 9.5 模拟命中率

Phase 19 可做一项很有价值的功能：

```bash
vjassc input_old.j --emit-incremental-state old.json
vjassc input_new.j --compare-incremental-state old.json --emit-incremental-report report.json
```

如果暂时不做双输入 CLI，就先在 War3Lib 中记录上一轮 chunk state 到：

```text
Output/vjassc.incremental.state.json
```

下一轮对比并输出：

```text
Output/vjassc.incremental.report.json
```

### 9.6 Phase 19 验收

```text
- 只生成报告，不改变 output。
- 能统计 chunk 数量和 hash 稳定性。
- 能在连续两次未改源码的 ALPHA 编译中报告 95%+ chunks reusable。
- 能在小改一个 function 的场景中报告大部分 chunks reusable。
```

---

## 10. War3Lib / Xlimon 任务要求

继续跑：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

继续跑 fast compare：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

新增建议：

```text
- 输出 Output/vjassc.incremental.report.json。
- 输出 Output/vjassc.performance.phase19.json。
- report 中显示 BodyMode、TokenCache、DependencyRecorder、Incremental chunks。
```

---

## 11. 必须新增/更新文档

新增：

```text
docs/phase19_status.md
```

必须包含：

```text
1. Standalone fast / validate / full-validation 数据。
2. War3Lib/Xlimon ALPHA validate 和 fast compare 数据。
3. PJASS / syntax-lite / init validation 状态。
4. dependency recorder coverage。
5. sourceMethods 耗时。
6. syntaxLite 耗时。
7. incremental chunk report。
8. parallel lowering 实验结果，如实现。
9. 是否达到目标，未达到原因。
```

README 更新：

```text
- Phase 19 简述。
- 若新增 CLI，如 --emit-incremental-report / --experimental-parallel-lowering，加入 CLI 文档。
```

---

## 12. 推荐实施顺序

```text
Step 1: 保证 Phase 18 baseline 可复现。
Step 2: 实现 DependencyRecorder 覆盖提升。
Step 3: 实现 MethodLoweringPlan。
Step 4: Token-level bare field/method rewrite。
Step 5: Array rewrite 精准过滤。
Step 6: syntax-lite 共享 token/flags。
Step 7: War3Lib ALPHA validate + fast compare。
Step 8: 增量 chunk report。
Step 9: 并行 lowering 实验开关。
Step 10: docs/phase19_status.md。
```

不要把并行和增量放在最前面。先把单线程依赖记录和 method lowering 继续压缩，确保输出更稳定，再尝试并行和增量。

---

## 13. 验收命令

Standalone fast：

```bat
build\vjassc.exe samples\input.j -o build\input.phase19.fast.out.j --mode fast --emit-stats build\input.phase19.fast.stats.json
```

Standalone validate：

```bat
build\vjassc.exe samples\input.j -o build\input.phase19.validate.out.j --mode validate --emit-stats build\input.phase19.validate.stats.json --emit-validation-report build\input.phase19.validate.validation.json --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi
```

Standalone full-validation：

```bat
build\vjassc.exe samples\input.j -o build\input.phase19.full.out.j --mode full-validation --emit-stats build\input.phase19.full.stats.json --emit-validation-report build\input.phase19.full.validation.json --compare-jasshelper samples\output_jasshelper.j --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi --emit-pjass-examples 50
```

War3Lib ALPHA validate：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

War3Lib ALPHA fast compare：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

---

## 14. 风险与回退

### 风险 1：MethodPlan token rewriter 误改变量

回退策略：

```text
保留旧路径，通过 option 或内部 fallback 对比。
新增 shadow local/param fixtures。
```

### 风险 2：dependency recorder 缺边导致 PJASS forward reference

回退策略：

```text
保留 output scan fallback。
只有 coverage 足够后再考虑删除 output scan。
```

### 风险 3：并行 lowering 输出不稳定

回退策略：

```text
默认关闭。
只做实验结果，不进入 War3Lib 默认任务。
```

### 风险 4：增量 chunk 边界不稳定

回退策略：

```text
Phase 19 只输出 report，不使用 cache 结果生成最终 output。
```

---

## 15. Phase 19 成功标准总结

Phase 19 成功不是必须一次做到 1~2 秒，而是要做到：

```text
1. fast 冷编译继续降到 5 秒级。
2. validate 降到 7 秒以内或附近。
3. sourceMethods 明显低于 Phase 18。
4. dependency recorder 覆盖率明显提高。
5. 增量编译有可量化 chunk report。
6. 并行 lowering 有实验数据和明确下一步判断。
7. PJASS、syntax-lite、War3Lib ALPHA 继续稳定通过。
```

如果 Phase 19 达成这些，Phase 20 就可以正式选择：

```text
路线 A：并行 lowering 默认化。
路线 B：增量编译 cache 输出落地。
路线 C：继续 IR 化表达式 lowering。
```
