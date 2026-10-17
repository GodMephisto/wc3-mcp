---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase18_codex_task_plan.md"
ref: HEAD
---

# Phase 18 Codex 实施计划：Zinc/vJass 分流编译 + TokenCache 复用 + Fast Path 再提速

## 0. 当前背景

项目：`Crainax/JassChanger`

当前阶段：Phase 17 已完成。

Phase 17 已经完成一个重要里程碑：

- `vjassc` 已可在真实 War3Lib / Xlimon ALPHA 链路中替代 JassHelper。
- `samples/input.j` 继续 PJASS 通过，`groupedCount = 0`。
- War3Lib ALPHA fast compare 中，`vjassc fast elapsedMs = 7490 ms`，`jasshelper elapsedMs = 11514 ms`，说明 fast 模式已经比 JassHelper 快。
- standalone fast 已降到约 `7323 ms`。
- standalone validate 已降到约 `8862 ms`。
- source struct method lowering 从 Phase 16 的约 `12~13s` 降到 Phase 17 的约 `2.1~2.3s`。

Phase 17 主要提速来自：

- 减少 array rewrite 的无效尝试。
- 减少 function/struct lookup。
- 缩小 current-struct field/method rewrite 入口。
- function ordering 增加 prefilter。
- War3Lib compare 模式终于允许 `WAR3_VJASSC_MODE=fast` 生效。

当前剩余热点已经发生变化：

- `sourceMethods` 不再是唯一最大瓶颈，但仍有约 `2.1~2.3s`。
- `lineFeatureScans` 仍接近 `96847`。
- `functionOrderTokenScans` 仍约 `110035`。
- `sanitizeOutput` 仍约 `1.4~1.5s`。
- fast codegen 仍约 `6.5~6.7s`。

Phase 18 的目标不是继续零散 patch，而是开始把函数体/方法体 lowering 从“统一字符串兼容路径”转向“按语法模式分流 + 共享 token/cache + 更少全局扫描”的结构。

---

## 1. Phase 18 总目标

Phase 18 目标：

> 在保持 PJASS 通过、War3Lib ALPHA 可编译、fast 模式可用的前提下，引入 Zinc/vJass 分流编译基础设施，并通过 TokenCache 复用、sanitize 前移、function dependency edge 提前记录，进一步将 fast 模式压低到 5~6 秒级。

Phase 18 不是要一次性重写完整 AST/IR 编译器，不允许推翻当前已稳定通过 PJASS 和 ALPHA 链路的逻辑。

核心方向：

```text
SourceMode 分流
  ↓
ZincBodyLowerer / JassLikeBodyLowerer / GeneratedBodyLowerer
  ↓
LineTokenCache / BodyTokenCache
  ↓
现有 lowering 逻辑逐步复用 cache，而不是每个 pass 重新扫描
  ↓
JASS output
```

禁止方向：

```text
不要做 Zinc -> vJass 文本 -> 再 parse vJass 文本。
不要为了速度跳过 PJASS 或 syntax-lite 正确性检查。
不要删除 fast mode 的 statement-shape guard。
不要破坏 War3Lib/Xlimon ALPHA 当前可替代 JassHelper 的能力。
```

---

## 2. Phase 18 验收目标

### 2.1 Correctness 验收

必须全部满足：

```text
ctest pass
standalone fast compile pass
standalone validate syntax-lite pass
standalone validate PJASS pass, groupedCount = 0
standalone full-validation PJASS pass, groupedCount = 0
War3Lib ALPHA vjassc validate compile pass
War3Lib ALPHA fast compare compile pass
War3Lib ALPHA fast compare 中 vjassc 继续快于 JassHelper
```

### 2.2 性能最低目标

基于 Phase 17：

```text
standalone fast total:      7323 ms
standalone validate total:  8862 ms
standalone full total:      9558 ms
War3Lib fast vjassc:        7490 ms
War3Lib jasshelper:        11514 ms
```

Phase 18 最低目标：

```text
standalone fast total <= 6500 ms
standalone validate total <= 8200 ms
standalone full-validation total <= 9000 ms
War3Lib ALPHA fast vjassc elapsedMs <= 6800 ms
PJASS 继续通过
```

### 2.3 性能良好目标

```text
standalone fast total <= 5500 ms
standalone validate total <= 7500 ms
standalone full-validation total <= 8200 ms
War3Lib ALPHA fast vjassc elapsedMs <= 6000 ms
```

### 2.4 性能优秀目标

```text
standalone fast total <= 4500 ms
standalone validate total <= 6500 ms
War3Lib ALPHA fast vjassc elapsedMs <= 5000 ms
```

### 2.5 Counter 目标

Phase 17 baseline：

```text
arrayAccessRewriteAttempts: 18277
functionLookupCalls:       20270
structLookupCalls:         30501
functionOrderTokenScans:  110035
lineFeatureScans:          96847
```

Phase 18 最低目标：

```text
lineFeatureScans <= 70000
functionOrderTokenScans <= 80000
arrayAccessRewriteAttempts <= 15000
functionLookupCalls <= 18000
structLookupCalls <= 26000
sourceMethods <= 2000 ms
sanitizeOutput <= 1000 ms in fast mode
```

Phase 18 良好目标：

```text
lineFeatureScans <= 50000
functionOrderTokenScans <= 60000
arrayAccessRewriteAttempts <= 12000
sourceMethods <= 1600 ms
sanitizeOutput <= 700 ms in fast mode
```

---

## 3. 任务一：SourceMode / BodyMode 显式分流

### 3.1 背景

目前项目支持 Zinc 与 vJass/JASS 混合。Zinc 块由：

```jass
//! zinc
...
//! endzinc
```

包裹。理论上两类语法可以明确拆分，不需要在所有函数体/方法体里同时猜测两种语言语法。

当前很多 lowering 仍依赖统一字符串路径，需要判断：

```text
有没有 ;
有没有 {}
有没有 ->
有没有 call/set/local
有没有 inline if return/call/set
有没有 Zinc continuation
有没有 vJass endfunction/endif/endloop
```

Phase 18 需要建立 source mode 分流基础。

### 3.2 新增枚举

在合适位置加入：

```cpp
enum class BodyMode {
    JassLike,
    Zinc,
    Generated,
};
```

或复用现有 `SyntaxMode`，但需要区分 generated body。

### 3.3 要求

每个可 lower 的 body 都必须能拿到 `BodyMode`：

```text
普通 JASS/vJass function body -> JassLike
Zinc function body -> Zinc
vJass method body -> JassLike
Zinc method body -> Zinc
generated struct support body -> Generated
generated function-interface wrapper -> Generated
generated lambda wrapper -> Generated 或继承 lambda source mode，二者需明确
```

### 3.4 接口建议

```cpp
BodyMode bodyModeForFunction(const Decl& decl) const;
BodyMode bodyModeForMethod(const MethodDecl& method) const;
BodyMode bodyModeForGenerated(GeneratedKind kind) const;
```

### 3.5 验收

新增 stats：

```json
{
  "bodyModes": {
    "zincFunctions": N,
    "jassLikeFunctions": N,
    "zincMethods": N,
    "jassLikeMethods": N,
    "generatedBodies": N
  }
}
```

新增 fixture：

```text
tests/fixtures/phase18_body_mode_zinc_function.in.j
tests/fixtures/phase18_body_mode_vjass_function.in.j
tests/fixtures/phase18_body_mode_zinc_method.in.j
tests/fixtures/phase18_body_mode_generated_support.in.j
```

---

## 4. 任务二：BodyLowerer 分流骨架

### 4.1 新增三个 lowerer 概念

不要求一次拆成独立文件，但至少需要逻辑上分离：

```text
ZincBodyLowerer
JassLikeBodyLowerer
GeneratedBodyLowerer
```

可以先在 `Phase1Codegen` 内部做私有函数：

```cpp
std::vector<std::string> lowerBodyByMode(
    BodyMode mode,
    const std::vector<std::string>& lines,
    LoweringContext& ctx
) const;

std::vector<std::string> lowerZincBodyFast(...);
std::vector<std::string> lowerJassLikeBodyFast(...);
std::vector<std::string> lowerGeneratedBodyFast(...);
```

### 4.2 ZincBodyLowerer 只处理 Zinc 逻辑

Zinc body path 负责：

```text
semicolon removal
brace block lowering
inline if return/call/set
else-if splitting
multi-line continuation
arithmetic/logical continuation
function(...) lambda extraction
C-style local declaration
+= -= *= /=
while/for lowering
```

Zinc body path 不应该处理：

```text
vJass takes/returns
vJass call/set/local/endfunction
vJass endif/endloop parsing
```

### 4.3 JassLikeBodyLowerer 只处理 JASS/vJass 逻辑

JassLike body path 负责：

```text
call/set/local/return
if then / else / endif
loop / exitwhen / endloop
function reference
vJass local declaration hoisting
method body this/bare field rewrite
```

JassLike body path 不应该处理：

```text
Zinc brace matching
Zinc continuation joining
Zinc inline if-return normalization
Zinc ; cleanup beyond final sanitize guard
```

### 4.4 GeneratedBodyLowerer 极简化

Generated body path 负责：

```text
直接输出 compiler-generated JASS lines
只做必要的 name/function ordering registration
不做 Zinc/JassLike 猜测
不做 lambda extraction
不做 receiver-chain guessing
不做 array rewrite，除非生成器明确标记需要
```

### 4.5 验收

新增 counters：

```json
{
  "bodyLowerer": {
    "zincBodies": N,
    "jassLikeBodies": N,
    "generatedBodies": N,
    "zincLines": N,
    "jassLikeLines": N,
    "generatedLines": N,
    "generatedLinesSkippedGenericLowering": N
  }
}
```

目标：generated bodies 不进入 generic body lowering。

---

## 5. 任务三：LineTokenCache / BodyTokenCache

### 5.1 背景

Phase 17 仍有：

```text
lineFeatureScans: 96847
functionOrderTokenScans: 110035
```

说明不同 pass 仍在重复扫描行文本。Phase 18 要建立 token cache，供 expression lowering、array rewrite、member rewrite、function ordering、statement-shape guard 共享。

### 5.2 数据结构建议

```cpp
struct IdentifierToken {
    std::string_view text;
    size_t start = 0;
    size_t end = 0;
};

struct LineTokenCache {
    std::string_view line;
    bool hasDot = false;
    bool hasBracket = false;
    bool hasParen = false;
    bool hasCallWord = false;
    bool hasSetWord = false;
    bool hasLocalWord = false;
    bool hasReturnWord = false;
    bool hasFunctionWord = false;
    bool hasThisWord = false;
    bool hasThistypeWord = false;
    bool hasNameToken = false;
    bool hasExecuteEvaluate = false;
    bool hasBooleanOperators = false;
    bool hasStringOrRawcode = false;
    std::vector<IdentifierToken> identifiers;
    std::vector<size_t> parenPositions;
    std::vector<size_t> bracketPositions;
};

struct BodyTokenCache {
    BodyMode mode;
    std::vector<LineTokenCache> lines;
};
```

### 5.3 构建规则

必须保护：

```text
string literal
rawcode literal
line comment
block comment 已由前置预处理处理过，但仍不能误扫字符串里的符号
```

### 5.4 使用范围

优先替换这些 pass 的重复扫描：

```text
scanLineFeatures
lineNeedsExpressionLowering
lineNeedsStructRewrite
rewriteArrayAccesses prefilter
function ordering dependency extraction prefilter
statement-shape validation
.name rewrite prefilter
```

### 5.5 不要一次性重写 lowerExpression

Phase 18 只要求 token cache 复用，暂不要求把所有 expression lowering 改成 token AST。

### 5.6 Counters

新增：

```json
{
  "tokenCacheBuilds": N,
  "tokenCacheLines": N,
  "tokenCacheHits": N,
  "tokenCacheMisses": N,
  "featureScansAvoided": N,
  "functionOrderScansAvoided": N
}
```

### 5.7 验收目标

```text
lineFeatureScans <= 70000
functionOrderTokenScans <= 80000
```

良好目标：

```text
lineFeatureScans <= 50000
functionOrderTokenScans <= 60000
```

---

## 6. 任务四：Zinc/JassLike 分流后的 fast path 跳过

### 6.1 Zinc fast path

对于 Zinc body，如果 token cache 显示该行没有：

```text
;
{
}
(
)
.
[
]
function
return
call-like identifier
operator continuation
```

则不要进入复杂 Zinc statement lowering。

### 6.2 JassLike fast path

对于 JassLike body，如果 token cache 显示该行没有：

```text
.
[
]
function
this
thistype
execute/evaluate
known current-struct member token
```

则不要进入 struct/member/function-interface heavy lowering。

### 6.3 Generated fast path

Generated body 只做 append，不进入上述判断。

### 6.4 验收

新增 counters：

```json
{
  "zincFastPathLines": N,
  "jassLikeFastPathLines": N,
  "generatedFastPathLines": N,
  "heavyLoweringAvoidedByMode": N
}
```

---

## 7. 任务五：Array rewrite 精准过滤二次优化

### 7.1 背景

Phase 17 后：

```text
arrayAccessRewriteAttempts: 18277
arrayAccessRewriteChanged: 7351
```

命中率已经比 Phase 16 好很多，但仍有约 1.8 万次尝试。Phase 18 目标是进一步降低到 1.5 万以下，良好目标 1.2 万以下。

### 7.2 建立 knownArrayReceiverIndex

构建：

```cpp
std::unordered_set<NameId/StringId> knownGlobalArrayReceivers;
std::unordered_set<NameId/StringId> knownLocalArrayReceivers;
std::unordered_set<NameId/StringId> knownStructArrayFieldReceivers;
```

如果暂时没有 string interning，可以先用 `std::string_view` + stable storage 或 `std::string`。

### 7.3 改写入口条件

当前不应只靠 `hasBracket` 进入 array rewrite。

应改为：

```text
扫描 identifier 后，若下一个非空字符是 '['，且 identifier 在 known array receiver 集合中，才进入 rewriteArrayAccesses。
```

### 7.4 验收

```text
arrayAccessRewriteAttempts <= 15000
arrayAccessRewriteChanged 仍应约等于 Phase 17，不可明显下降
```

如果 changed 数下降，说明过滤错过了真实需要 rewrite 的数组访问，必须修复。

---

## 8. 任务六：sanitizeOutput 前移

### 8.1 背景

Phase 17 后 fast 仍有：

```text
sanitizeOutput: ~1427 ms
```

fast 模式不应该大规模扫描完整输出做修补。必要的合法化应尽量在生成行时完成。

### 8.2 分类 sanitize 工作

把 sanitize 拆成：

```text
RequiredForCorrectOutput
ValidationOnly
DebugReportOnly
```

RequiredForCorrectOutput 例子：

```text
移除不合法分号
&& -> and
|| -> or
! -> not
避免 bare invalid statement 输出
```

ValidationOnly 例子：

```text
检查 residue
统计 suspicious forms
生成 examples
```

DebugReportOnly 例子：

```text
详细 line example
PJASS grouped examples
```

### 8.3 emit-time sanitize

在 `writer_.writeln` 前，尽量调用轻量 `sanitizeGeneratedLineFast`，或确保每个 lowerer 已经输出合法 JASS。

目标：

```text
fast mode sanitizeOutput <= 1000 ms
良好：<= 700 ms
```

### 8.4 注意

不能删除 fast mode statement-shape guard。Phase 17 中 bare `123` 能被拒绝，这是正确行为，必须保持。

---

## 9. 任务七：Function ordering 记录边预研

### 9.1 背景

Phase 17 后：

```text
functionOrderTokenScans: 110035
functionOrdering: ~500 ms
```

这不再是最大瓶颈，但如果目标是接近 1~2 秒，最终必须去掉 output-based function dependency scan。

### 9.2 Phase 18 范围

Phase 18 不要求完全替换 function ordering。

只要求做预研和局部落地：

```text
在 lowering 阶段记录显式 call/function-ref edges。
生成 FunctionDependencyRecorder。
最终与 output scan 结果对比。
暂不完全依赖 recorder 排序，避免破坏 PJASS。
```

### 9.3 数据结构建议

```cpp
using FunctionId = uint32_t;

struct FunctionDependencyRecorder {
    std::vector<std::vector<FunctionId>> edges;
    void addEdge(FunctionId from, FunctionId to);
};
```

### 9.4 验收

生成 report：

```json
{
  "functionDependencyRecorder": {
    "recordedEdges": N,
    "outputScanEdges": N,
    "matchedEdges": N,
    "missingRecordedEdges": N,
    "extraRecordedEdges": N
  }
}
```

目标：recorded edges 覆盖 output scan edges 的 70% 以上。

Phase 19 再考虑全面替换。

---

## 10. 任务八：War3Lib/Xlimon 性能与 runtime 继续验证

### 10.1 War3Lib ALPHA validate

必须继续跑：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

要求：

```text
pass
Output/5_vjassc.j selected
Output/output.j written
PJASS pass
```

### 10.2 War3Lib ALPHA fast compare

必须继续跑：

```bat
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

要求：

```text
jasshelper ok
vjassc ok
vjassc fast elapsedMs < jasshelper elapsedMs
selectedOutput 仍按 compare 配置，不要误改默认输出
```

### 10.3 ALPHA 进游戏 smoke test

如果用户有时间，可让用户手动跑：

```bat
TaskStartMapVjasscAlpha.lua
```

Checklist：

```text
能进入游戏
主初始化无报错
F2/Museum UI 开关正常
DzAPI/YDWE/JAPI 相关初始化无明显异常
核心 UI click/enter callback 正常
存档/服务器 API mock 不崩
```

Codex 不需要承诺自己能进入游戏，只需生成 checklist 和记录点。

---

## 11. 任务九：文档与状态文件

新增：

```text
docs/phase18_status.md
```

必须记录：

```text
Implemented
Standalone Results
Counters Delta
Top Pass Timings
BodyMode Stats
TokenCache Stats
War3Lib / Xlimon ALPHA Results
Runtime Smoke Notes
Remaining Work
```

必须包含 Phase 17 baseline 与 Phase 18 对比。

README 更新：

```text
增加 Phase 18 一段说明。
Repository Layout 加入 phase18_status.md。
CLI 如果有新增模式或 report 字段，需要同步。
```

---

## 12. 推荐执行顺序

Codex 应按以下顺序做，不要跳跃：

```text
1. 添加 BodyMode / SourceMode 统计，不改变输出。
2. 添加 BodyLowerer 分流外壳，初始仍调用旧逻辑，确保输出不变。
3. 添加 LineTokenCache，并让 scanLineFeatures 使用 cache。
4. 让 function ordering prefilter 使用 token cache。
5. 让 array rewrite prefilter 使用 token cache + known array receiver。
6. GeneratedBodyLowerer 跳过所有 generic heavy lowering。
7. ZincBodyLowerer / JassLikeBodyLowerer 分别增加 fast path。
8. sanitizeOutput 拆分 Required/Validation/Debug。
9. War3Lib ALPHA validate 与 fast compare。
10. 写 phase18_status.md 和 README。
```

每完成一批必须跑：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

每个 major batch 后跑 standalone fast/validate。

最后跑 War3Lib ALPHA validate 和 fast compare。

---

## 13. 风险与注意事项

### 13.1 不要破坏当前 PJASS pass

Phase 17 已经 PJASS pass，任何性能优化都不能让 groupedCount 回升。

### 13.2 不要只优化 standalone，忽略 War3Lib

War3Lib / Xlimon ALPHA 是真实目标。Standalone 提速如果不反映到 War3Lib，就不算完整成功。

### 13.3 不要删除 statement-shape guard

Phase 17 已经修复 bare `123` 不应被静默接受的问题。Phase 18 必须继续保持。

### 13.4 不要做 Zinc -> vJass 文本中间层

只允许做：

```text
Zinc -> Zinc body lowerer / IR
JassLike -> JassLike body lowerer / IR
```

不允许：

```text
Zinc -> vJass text -> old vJass parser
```

这会增加中间输出、重新解析和新的兼容问题。

### 13.5 不要一次性并行化

Phase 18 可以为并行做准备，但不建议直接并行 body lowering。等 token cache / body lowerer 分流稳定后，Phase 19 再并行。

---

## 14. Phase 18 预期结果

完成后，项目应达到：

```text
vjassc fast 在 standalone 稳定进入 5~6 秒区间，至少低于 6.5 秒。
vjassc validate 保持 PJASS pass，并进入 7~8 秒区间。
War3Lib ALPHA fast 继续快于 JassHelper，并尽量进入 6 秒左右。
codegen pipeline 有明确 BodyMode / TokenCache 统计。
下一阶段可以继续做 function dependency recorder 全替换和并行 lowering。
```

如果性能没有明显改善，但 counters 明显下降，也可接受；Phase 18 的架构价值在于为 Phase 19 的并行和增量做基础。

---

## 15. Phase 19 预告，不要在 Phase 18 实现

Phase 19 建议方向：

```text
1. FunctionId graph 全面替换 output-based function ordering。
2. 并行 function/method/lambda body lowering。
3. Chunk hash / incremental cache 预研。
4. 更完整 FunctionBodyIR / MethodBodyIR。
```

Phase 18 不要提前做这些大改，避免破坏当前稳定性。
