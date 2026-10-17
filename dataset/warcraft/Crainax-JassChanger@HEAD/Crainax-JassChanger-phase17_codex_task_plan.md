---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase17_codex_task_plan.md"
ref: HEAD
---

# Phase 17 Codex 实施计划：Struct Method Fast Lowering + Fast Mode 精简

> 项目：`Crainax/JassChanger`
> 当前基线：Phase 16 之后，PJASS 继续通过，但 standalone 性能未达标；War3Lib/Xlimon ALPHA validate 链路有改善。
> 本阶段目标：不要新增大语法；保持 PJASS 绿色与 ALPHA 可编译，在 `Phase1Codegen` 的主要热点上做第二轮性能重构。

---

## 0. 当前结论

Phase 16 的性能结果说明：

- 正确性保持成功：`syntax-lite`、init validation、PJASS 均通过。
- lookup 与 scan 指标有改善：
  - `structLookupCalls`: `102092 -> 90306`
  - `functionLookupCalls`: `141163 -> 125223`
  - `memberAccessScans`: `17177 -> 6420`
- 但是 standalone 结果反而变慢：
  - fast total: `20541 -> 21296 ms`
  - validate total: `23714 -> 26449 ms`
  - full-validation total: `23065 -> 26807 ms`
- War3Lib/Xlimon ALPHA validate 链路有改善：
  - `vjassc internal total`: `26601 -> 23021 ms`
  - `jassCompilerMs`: `27234 -> 23611 ms`
  - `totalCompileMs`: `37812 -> 35066 ms`

所以 Phase 16 的方向部分正确，但局部实现产生了额外扫描成本。现在最大热点不是 PJASS，也不只是 regex，而是 **struct source method lowering** 与 **fast 模式仍然跑了过多输出级清理/验证准备**。

---

## 1. 本阶段硬目标

### 1.1 正确性硬目标

必须全部满足：

```text
ctest pass
samples/input.j --mode validate PJASS pass groupedCount == 0
samples/input.j --mode full-validation PJASS pass groupedCount == 0
syntaxLite issueCount == 0
init validation issues == 0
War3Lib / Xlimon ALPHA vjassc validate compile pass
War3Lib / Xlimon ALPHA vjassc selected output remains playable candidate
```

不得为了速度删除必要 lowering，不得回退 Phase 14/15/16 修复过的真实地图语义。

### 1.2 性能目标

最低目标：

```text
standalone fast total <= 17000 ms
standalone validate total <= 22000 ms
standalone full-validation total <= 23000 ms
emitStructSupport.sourceMethods <= 8000 ms
arrayAccessRewriteAttempts <= 60000
functionLookupCalls <= 100000
structLookupCalls <= 75000
War3Lib ALPHA validate totalCompileMs <= 30000 ms
```

良好目标：

```text
standalone fast total <= 14000 ms
standalone validate total <= 18000 ms
emitStructSupport.sourceMethods <= 5500 ms
arrayAccessRewriteAttempts <= 35000
War3Lib ALPHA validate totalCompileMs <= 26000 ms
```

优秀目标：

```text
standalone fast total <= 10000 ms
standalone validate total <= 14000 ms
War3Lib ALPHA fast jassCompilerMs <= current JassHelper elapsedMs
War3Lib ALPHA validate totalCompileMs <= 22000 ms
```

---

## 2. 基线命令

Codex 开始前必须先跑一次基线并记录，不要直接改代码。

### 2.1 Standalone fast

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase17.baseline.fast.out.j ^
  --mode fast ^
  --emit-stats build\input.phase17.baseline.fast.stats.json
```

### 2.2 Standalone validate

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase17.baseline.validate.out.j ^
  --mode validate ^
  --emit-stats build\input.phase17.baseline.validate.stats.json ^
  --emit-validation-report build\input.phase17.baseline.validate.validation.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

### 2.3 War3Lib / Xlimon ALPHA validate

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

### 2.4 War3Lib / Xlimon ALPHA fast

确认 War3Lib 不再被 compare task 硬编码成 `full-validation` 后，再跑：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

---

## 3. 问题分析

### 3.1 Phase 16 为什么没达标

Phase 16 减少了 lookup 与 member scan，但新增了 `LineFeatures` 和 gating 逻辑。当前 counters 显示：

```text
lineFeatureScans: 144792
arrayAccessRewriteAttempts: 108677
arrayAccessRewriteChanged: 7352
functionOrderTokenScans: 208696
receiverChainAttempts: 4900
receiverChainChanged: 417
```

这说明：

1. 很多行被扫描了，但最后没有改动。
2. array access rewrite 尝试次数远高于实际变更次数。
3. source struct method body lowering 仍然是主成本。
4. fast 模式仍跑了 `finalOutputValidationPrep` 和 `sanitizeOutput`，这对 fast 模式过重。
5. `emitStructSupport.generatedHelpers == 0 ms` 说明 generated helper 已跳过通用 lowering；真正慢的是 `emitStructSupport.sourceMethods`。

### 3.2 不要把问题简单归结为正则

正则确实曾经是主要瓶颈，但 Phase 15/16 已经砍掉了一部分 regex-heavy path。现在主要瓶颈是：

```text
source method body 多轮字符串 lowering
array access rewrite 对太多行尝试
struct/function lookup 仍过高
fast 模式包含 validation-oriented final scans
Phase1Codegen 超级类导致很多信息在 output 阶段重新推断
```

本阶段仍要继续清理 regex，但重点应是 **token-level method lowering plan** 与 **fast-mode pass 剥离**。

---

## 4. 任务 A：Struct Method Fast Lowerer

### 4.1 目标

把 `emitStructSupport.sourceMethods` 从 12~13 秒级压到 5~8 秒以内。

### 4.2 现状

Phase 16 已拆出：

```text
emitStructSupport.generatedHelpers
emitStructSupport.lifecycleSupport
emitStructSupport.sourceMethods
```

其中 generated helpers 没进入通用 lowering，基本成功。但 source methods 仍走复杂路径。

### 4.3 新增 MethodLoweringPlan

为每个 struct method 预生成 plan：

```cpp
struct MethodLoweringPlan {
    const StructInfo* currentStruct;
    const MethodInfo* method;
    std::unordered_set<std::string> localNames;
    std::unordered_map<std::string, TypeId> localTypes;
    std::unordered_set<std::string> shadowedFieldNames;
    std::unordered_set<std::string> shadowedMethodNames;
    bool hasAnyStructFieldUse;
    bool hasAnyStructMethodUse;
    bool hasAnyArrayAccess;
    bool hasAnyFunctionInterfaceUse;
    bool hasAnyLambda;
};
```

如果不想马上引入 `TypeId`，可以先用 `const StructInfo*` / `std::string_view`，但不要反复字符串查找。

### 4.4 生成 plan 的时机

在 `collectStructs` / `collectFunctions` 后，codegen 前执行：

```text
buildMethodLoweringPlans()
```

每个 method 只分析一次：

```text
- 参数名
- local 声明
- 是否遮蔽 struct field/method
- 是否包含 this / .field / .method / bare field / bare method
- 是否包含 []
- 是否包含 function(...) lambda
```

### 4.5 method body lowering 使用 plan

在 `emitStructMethod` 中：

```text
旧：每行 scanLineFeatures + 多轮 rewrite + 查 field/method/local
新：method plan 预判整函数；line token flags 决定是否进入具体 lowering
```

示例规则：

```text
若 method plan.hasAnyStructFieldUse == false 且 line 不含 this/.，跳过 struct expression rewrite。
若 method plan.hasAnyArrayAccess == false，整函数跳过 array access rewrite。
若 line 无 '['，跳过 array access rewrite。
若 line 无 '.' 且无 bare member token，跳过 receiver chain rewrite。
```

---

## 5. 任务 B：Token-level Bare Field / Method Rewriter

### 5.1 目标

替代 source method body 内 regex/broad string rewrite。

### 5.2 处理范围

只在 `currentStruct != nullptr` 的 method body 中启用。

需要处理：

```jass
set hp = hp + 1          // bare field
call destroy()           // bare instance method
call methodName(a, b)    // bare method
set this.hp = 1
call this.foo()
set .hp = 1
```

不应误伤：

```jass
local integer hp
set hp = hp + 1          // local shadow
call SomeFunction()
set x = 1.0              // real literal
call BJDebugMsg("hp")    // string literal
'hfoo'                   // rawcode
```

### 5.3 Token scanner 设计

新增：

```cpp
struct TokenView {
    enum Kind { Ident, Number, String, Rawcode, Symbol, Comment } kind;
    std::string_view text;
    size_t start;
    size_t end;
};

std::vector<TokenView> scanJassLineTokens(std::string_view line);
```

要求：

```text
- 保留原始 line，不创建大量 substring。
- string/rawcode/comment 内不参与 rewrite。
- `.` 后必须是 identifier 才是 member access。
- 数字小数点不视为 member access。
```

### 5.4 Rewrite 策略

基于 tokens 做替换段收集：

```cpp
struct Replacement { size_t start; size_t end; std::string text; };
```

最后一次性 apply replacements，避免多轮 string replace。

### 5.5 缓存

对 method line 可以缓存：

```cpp
unordered_map<LineCacheKey, LineTokenInfo>
```

但不要用整行大字符串做复杂 key。可先不做跨函数缓存，先确保每行在本 method 中只扫描一次。

---

## 6. 任务 C：Array Access Rewrite 快速过滤

### 6.1 问题

Phase 16：

```text
arrayAccessRewriteAttempts: 108677
arrayAccessRewriteChanged: 7352
```

尝试数过高，命中率只有约 6.7%。

### 6.2 目标

```text
arrayAccessRewriteAttempts <= 60000，良好 <= 35000
```

### 6.3 方案

在调用 `rewriteArrayAccesses` 前增加 cheap guard：

```text
line 必须含 '['
'[' 左侧最近 identifier 必须在 localArrayShapes 或 globalArrayShapes 或 struct fixed-array field 集合中
否则跳过
```

实现扫描函数：

```cpp
bool hasKnownArrayReceiver(std::string_view line,
                           const LocalArrayShapeMap* locals,
                           const GlobalArrayShapeMap& globals,
                           const StructInfo* currentStruct);
```

不要在这个 guard 内做完整 rewrite，只做一次轻量 token scan。

---

## 7. 任务 D：Fast Mode 剥离 validation-oriented passes

### 7.1 问题

Phase 16 fast artifact 里仍然有：

```text
finalOutputValidationPrep: 2604 ms
sanitizeOutput: 1895 ms
functionOrdering: 708 ms
```

其中 function ordering 是输出正确性所需，可保留；但 validation prep 与部分 sanitize 应检查是否真的必须。

### 7.2 目标

```text
fast 模式 finalOutputValidationPrep <= 300 ms
fast 模式 sanitizeOutput <= 800 ms
```

### 7.3 任务

将 pass 分类：

```cpp
enum class PassNecessity {
    RequiredForCorrectOutput,
    RequiredForPjassValidation,
    RequiredForReportOnly,
    RequiredForComparisonOnly,
};
```

fast 模式只执行：

```text
RequiredForCorrectOutput
```

validate 模式执行：

```text
RequiredForCorrectOutput + RequiredForPjassValidation
```

full-validation 执行全部。

### 7.4 注意

不要因为 fast 跳过 sanitize 导致输出不合法。正确做法是：

```text
把必要 sanitize 前移到生成时，成为 RequiredForCorrectOutput。
把只用于统计/检测的 final scan 留到 validate/full-validation。
```

---

## 8. 任务 E：Function / Struct Lookup 进一步压缩

### 8.1 当前数据

Phase 16：

```text
structLookupCalls: 90306
functionLookupCalls: 125223
```

比 Phase 15 好，但仍高。

### 8.2 目标

```text
structLookupCalls <= 75000
functionLookupCalls <= 90000
```

### 8.3 方案

1. 在 `scanLineFeatures` 中不要调用 `findStruct` / `findFunctionInfo`。
2. 将 localTypes 的 value 从 string 类型名改成：

```cpp
struct LocalTypeInfo {
    std::string name;
    const StructInfo* structInfo = nullptr;
    const FunctionInterfaceInfo* iface = nullptr;
};
```

3. 函数名 resolution 使用 `functionIndexByName_` 一次查找后传指针。
4. struct receiver chain 中连续访问时，把 receiver type 传递下去，不要每一段重新按字符串查 struct。

---

## 9. 任务 F：Function Ordering 轻量化

### 9.1 当前状态

Phase 16：

```text
functionOrdering: 708 ms
functionOrderTokenScans: 208696
functionOrderEdges: 32256
functionOrderSccCount: 4
```

708ms 不是最大问题，但如果要冲 5~10 秒，总要优化。

### 9.2 方案

1. 使用 `FunctionId` 整数图，避免 set<string>。
2. 在 function emission/lowering 时记录直接调用边，不要输出后再扫全部文本。
3. 对 generated bridge / lambda / function-interface wrapper 直接添加依赖边。
4. 只对未知/复杂行做 token fallback scan。

### 9.3 目标

```text
functionOrdering <= 300 ms
functionOrderTokenScans <= 100000
```

---

## 10. 任务 G：War3Lib 同链路 fast 对比修复

### 10.1 问题

Phase 16 记录：

```text
Compare command was also run with WAR3_VJASSC_MODE=fast,
but TaskCompileCompareAlpha.lua still hardcodes path.vjasscMode = "full-validation".
```

这会导致无法公平比较：

```text
jasshelper elapsedMs vs vjassc fast elapsedMs
```

### 10.2 任务

在 War3Lib 中修复 compare task：

```text
- 如果环境变量 WAR3_VJASSC_MODE 已设置，则不要覆盖。
- 如果没有设置，compare 默认 full-validation。
- report 中明确记录 requestedMode 与 effectiveMode。
```

### 10.3 新增命令

```bat
set WAR3_VJASSC_MODE=fast
lua lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

报告必须包含：

```json
{
  "jasshelper": { "elapsedMs": ... },
  "vjassc": { "elapsedMs": ..., "mode": "fast" },
  "diff": { ... }
}
```

---

## 11. 任务 H：文档与报告

新增或更新：

```text
docs/phase17_status.md
README.md
```

`docs/phase17_status.md` 必须包含：

```text
1. Phase 16 baseline
2. Phase 17 standalone fast/validate/full-validation
3. War3Lib ALPHA validate result
4. War3Lib ALPHA fast compare result
5. PJASS correctness status
6. Top pass timings
7. Counters delta
8. 是否快过 JassHelper
9. 下一阶段建议
```

必须明确回答：

```text
- standalone fast 是否快过 jasshelper？
- War3Lib ALPHA fast 是否快过 jasshelper？
- validate 模式是否可日常使用？
- 还有哪些热点？
```

---

## 12. 禁止事项

不得做：

```text
- 为了速度跳过必要 lowering 导致 PJASS 或 ALPHA 输出坏掉。
- 为了速度关闭 PJASS pass 的 correctness fix。
- 大规模重写 parser/AST。
- 一次性拆完整 Phase1Codegen 超级类。
- 让 fast 模式输出和 validate 模式输出语义不一致。
- 删除或弱化 War3Lib ALPHA safety fallback。
```

---

## 13. 推荐实施顺序

```text
Step 1: 记录 Phase 16 baseline。
Step 2: 修复 War3Lib compare fast mode hardcode。
Step 3: 实现 MethodLoweringPlan。
Step 4: 实现 token-level bare field/method rewriter。
Step 5: 优化 array access rewrite guard。
Step 6: fast mode 剥离 validation/report-only passes。
Step 7: 降低 function/struct lookup。
Step 8: function ordering 轻量化。
Step 9: 跑 standalone fast/validate/full-validation。
Step 10: 跑 War3Lib ALPHA validate 与 fast compare。
Step 11: 写 docs/phase17_status.md。
```

---

## 14. 最终验收清单

```text
[x] ctest pass
[x] standalone validate PJASS pass groupedCount == 0
[x] standalone full-validation PJASS pass groupedCount == 0
[x] standalone fast total <= 17000 ms (`7323 ms`)
[x] standalone validate total <= 22000 ms
[x] emitStructSupport.sourceMethods <= 8000 ms (`2070-2171 ms` standalone, `2308 ms` ALPHA validate)
[x] arrayAccessRewriteAttempts <= 60000
[x] War3Lib ALPHA validate totalCompileMs <= 30000 ms (`19633 ms`)
[x] War3Lib ALPHA fast compare 能真实记录 vjassc fast vs jasshelper
[x] docs/phase17_status.md 已提交
```

---

## 15. 成功后的下一阶段方向

如果 Phase 17 达成最低目标，则 Phase 18 可以继续：

```text
- 正式拆 Phase1Codegen 超级类
- 引入 CodegenIR / FunctionBodyIR
- 结构化 lowering pipeline
- 继续将 standalone fast 压到 5~8 秒
- 将 War3Lib vjassc fast 稳定压过 JassHelper
```

如果 Phase 17 达成优秀目标，则可以开始考虑让 ALPHA 的 vjassc fast 作为默认候选编译器，但正式版本仍应保留 jasshelper fallback。
