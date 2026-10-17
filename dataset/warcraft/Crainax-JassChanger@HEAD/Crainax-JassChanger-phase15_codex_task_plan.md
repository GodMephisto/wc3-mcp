---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase15_codex_task_plan.md"
ref: HEAD
---

# Phase 15 Codex 实施计划：Performance Architecture Pass 1

## 0. 阶段背景

Phase 14 已经完成了从 standalone `samples/input.j` 到真实 `War3Lib / Xlimon` 编译链路的接入：

- War3Lib 支持 `jasshelper` / `vjassc` / `both` 三种后端。
- ALPHA 任务可以显式选择 vjassc。
- vjassc 后端已经能在真实 Xlimon 输入上通过 PJASS。
- 当前瓶颈已经不再是 PJASS，而是 vjassc 的 codegen 与 syntax-lite。

当前真实链路关键基线：

```json
{
  "jasshelper.elapsedMs": 12545,
  "vjassc.elapsedMs": 65362,
  "war3lib.totalCompileMs": 90099,
  "vjassc.codegen": 53921,
  "vjassc.syntaxLite": 9874,
  "vjassc.pjass": 348,
  "vjassc.comparison": 121,
  "vjassc.total": 65241
}
```

Phase 15 的目标不是继续修 PJASS，而是做第一轮真正性能架构优化，把 vjassc 从“正确但慢”推进到“可在日常 ALPHA 开发中使用”。

## 1. 阶段目标

### 1.1 最低验收目标

在 `Crainax/Xlimon` 的 ALPHA vjassc 编译链路上：

```text
- PJASS 继续通过。
- syntax-lite 继续通过。
- init validation 继续通过。
- duplicate function/global/native names 仍为 0。
- vjassc internal total <= 35000 ms。
- vjassc codegen <= 25000 ms。
- War3Lib vjassc-selected ALPHA total compile <= 60000 ms。
```

### 1.2 良好目标

```text
- vjassc internal total <= 25000 ms。
- vjassc codegen <= 18000 ms。
- syntaxLite <= 4000 ms。
- War3Lib vjassc-selected ALPHA total compile <= 45000 ms。
```

### 1.3 优秀目标

```text
- vjassc compile-only mode <= 10000 ms。
- vjassc validate mode <= 20000 ms。
- War3Lib ALPHA vjassc compile <= 30000 ms。
```

> 说明：不要在 Phase 15 强行追求 1~2 秒。当前首要目标是去掉架构级慢点，把 60 秒级压到 20~35 秒级。后续 Phase 16/17 再继续压到 10 秒、5 秒、最终 1~2 秒。

## 2. 必须保持的正确性边界

任何性能优化都必须保持：

```text
- samples/input.j standalone PJASS pass。
- War3Lib ALPHA vjassc-selected compile pass。
- both mode 仍可同时产出 5_jasshelper.j 与 5_vjassc.j。
- validation-only InitTrig_japi stub 仍不能进入正式 generated output。
- 不允许删除已有 validation/report 能力，只能改为可选、懒执行或缓存。
- 不允许为了性能改变 source semantics。
```

## 3. 当前性能问题判断

### 3.1 Phase1Codegen 已经变成“超级类”

当前 `Phase1Codegen` 同时负责：

```text
- globals 输出
- native/type 输出
- function 输出
- Zinc body lowering
- struct 收集与 struct lowering
- struct support function 输出
- lambda 提取与输出
- function interface runtime 输出
- function ordering
- cyclic bridge
- final output adaptation
- syntax/PJASS 统计辅助
- 多种缓存与性能计数
```

这会造成：

```text
- 各 pass 的边界不清晰。
- 生成函数也被走源码 lowering 路径。
- 多轮全量字符串扫描难以避免。
- 很多信息明明 AST/collection 阶段已知，却在最终 output string 上重新解析。
```

Phase 15 要开始拆分，但不要一次性大重写。先做性能收益最大的“边界重构”。

## 4. Phase 15 任务拆分

## Task A：建立三种编译模式，避免日常编译总跑 full validation

### A1. 新增 CLI 模式

新增或规范化：

```bash
vjassc input.j -o output.j --mode fast
vjassc input.j -o output.j --mode validate
vjassc input.j -o output.j --mode full-validation
```

等价行为：

```text
fast:
  只生成 output.j。
  不跑 syntax-lite。
  不跑 PJASS。
  不跑 JassHelper comparison。
  不生成详细 PJASS examples。

validate:
  生成 output.j。
  跑 syntax-lite。
  跑 PJASS，如果提供 pjass/common/blizzard。
  写基本 stats/report。

full-validation:
  生成 output.j。
  跑 syntax-lite。
  跑 PJASS。
  跑 JassHelper structural comparison。
  写详细 validation report、examples、performance counters。
```

### A2. War3Lib 对接

War3Lib 的 vjassc 后端要支持：

```text
WAR3_VJASSC_MODE=fast|validate|full-validation
```

ALPHA 默认建议：

```text
TaskStartMapVjasscAlpha: validate
TaskCompileCompareAlpha: full-validation
日常快速测试: fast
```

### A3. 验收

```text
- fast 输出文件可用。
- validate 仍 PJASS pass。
- full-validation 报告字段不减少。
- War3Lib ALPHA 任务可选择不同模式。
```

## Task B：拆出 Performance-oriented CodeWriter

当前 `CodeWriter` 基于 `std::ostringstream`，每次 `writeln` 都输出缩进和行文本。Phase 15 建议替换为可预留容量的 `FastCodeWriter`。

### B1. 新建 `FastCodeWriter`

建议结构：

```cpp
class FastCodeWriter {
public:
    explicit FastCodeWriter(size_t reserveBytes = 0);
    void writeln(std::string_view line = {});
    void writeRaw(std::string_view text);
    void indent();
    void dedent();
    std::string take();

private:
    std::string out_;
    int indent_ = 0;
    std::array<std::string, 16> indentCache_;
};
```

### B2. 预估 reserve

对真实 Xlimon：输出约 115k 行。先粗略 reserve：

```cpp
reserveBytes = inputBytes * 2 + generatedFunctionCount * 512 + structCount * 4096;
```

或者在 full validation 中记录上次输出大小，下次使用：

```text
lastOutputBytes -> reserveBytes
```

### B3. 验收

```text
- 输出完全保持合法。
- `emitFunctions` / `emitStructSupport` 至少下降 5%~10%。
```

## Task C：将 struct support 改为模板化直接生成，跳过通用 lowering

当前 `emitStructSupport` 是最大热点之一。struct support 本质是编译器生成代码，不应再走大量源码级 rewrite。

### C1. 为生成函数加 GeneratedFunctionKind

定义：

```cpp
enum class GeneratedFunctionKind {
    StructAllocate,
    StructCreate,
    StructDestroy,
    StructDeallocate,
    StructOnDestroyWrapper,
    StructOnInitWrapper,
    FunctionInterfaceWrapper,
    Lambda,
    CycleBridge,
    UserFunction
};
```

### C2. struct support 直接模板输出

对于：

```text
allocate
create
destroy
deallocate
onDestroy wrapper
onInit wrapper
fixed array init
```

禁止进入：

```text
rewriteStructExpression
rewriteReceiverChains
rewriteCallArguments
lowerStatementLine
sanitizeGeneratedLine 多轮处理
```

除非该 support body 中确实包含用户源码片段，例如用户自定义 `onDestroy` body。

### C3. 预计算 struct lifecycle plan

新增：

```cpp
struct StructLifecyclePlan {
    bool needsAllocator;
    bool needsCreate;
    bool needsDestroy;
    bool needsDeallocate;
    bool hasUserCreate;
    bool hasUserDestroy;
    bool hasOnDestroy;
    bool hasOnInit;
    int instanceLimit;
    int fixedArrayStride;
};
```

在 collectStruct 阶段一次性算好，后面只读。

### C4. 验收

```text
- struct support functions 数量不异常下降。
- PJASS pass。
- emitStructSupport 时间至少下降 30%。
```

## Task D：FunctionBodyIR：函数体只扫描一次

当前最大热点是 `emitFunctions`。根因大概率是每个函数体被多轮字符串 pass 处理：

```text
extract lambda
rewrite locals
rewrite arrays
rewrite struct expressions
rewrite receiver chains
rewrite function values
rewrite call args
sanitize
function ordering dependency scan
final validation prep
```

Phase 15 不要求完整 AST expression parser，但要实现轻量 FunctionBodyIR。

### D1. 新建轻量行 IR

```cpp
struct ProtectedSpan {
    size_t begin;
    size_t end;
    enum class Kind { String, Rawcode, LineComment } kind;
};

struct IdentifierUse {
    std::string_view text;
    size_t begin;
    size_t end;
    enum class Role {
        Unknown,
        CallCandidate,
        FunctionReference,
        Receiver,
        LocalDecl,
        SetTarget,
        TypeName
    } role;
};

struct CodeLineIR {
    std::string original;
    std::string codeOnly;
    std::vector<ProtectedSpan> protectedSpans;
    std::vector<IdentifierUse> identifiers;
    bool hasDot = false;
    bool hasBracket = false;
    bool hasParen = false;
    bool hasFunctionKeyword = false;
    bool hasLambdaKeyword = false;
    bool hasOperatorChars = false;
};

struct FunctionBodyIR {
    std::string functionName;
    std::vector<CodeLineIR> lines;
};
```

### D2. 一次扫描，多个 pass 复用

每个函数体只做一次：

```text
- string/rawcode/comment splitting
- identifier scanning
- dot/bracket/paren/operator feature flags
- function reference candidate detection
```

后续 pass 用 flags 快速跳过：

```text
if (!line.hasDot && !line.hasBracket) skip struct/array receiver pass
if (!line.hasFunctionKeyword && !line.hasLambdaKeyword) skip function value pass
if (!line.hasOperatorChars) skip operator normalize
```

### D3. 验收

```text
- linesVisited 不显著增加。
- memberAccessScans 明显下降。
- emitFunctions 时间至少下降 25%。
- PJASS pass。
```

## Task E：把 function ordering 移到 pre-output IR，不再解析最终 output

Phase 12 已经把 functionOrdering 降到几百 ms，但现在还有 cycle bridge、callback adapter 等可能在最终 output 上继续扫描。

### E1. 函数依赖从 FunctionBodyIR 生成

不要等所有 function 变成最终字符串后再从 output 抽依赖。

构建：

```cpp
struct FunctionNode {
    int id;
    std::string name;
    FunctionSignature sig;
    GeneratedFunctionKind kind;
    std::vector<int> deps;
};
```

### E2. 算法

使用：

```text
- name -> function id unordered_map
- adjacency vector<vector<int>>
- Tarjan SCC 找循环
- 对非循环图做 Kahn topo sort
- 对 SCC 内按策略插 bridge 或保留原序
```

复杂度：

```text
O(V + E)
```

不要使用 `std::set<std::string>` 作为依赖主结构。依赖收集阶段可以去重，但最终排序应使用整数 id。

### E3. 验收

```text
- forwardFunctionReferences 保持 0。
- bridge 行为和 Phase 13 一致。
- functionOrdering <= 200 ms。
```

## Task F：重写 function / struct lookup 缓存策略

Phase 12 counters：

```json
{
  "structLookupCalls": 139617,
  "functionLookupCalls": 239115,
  "cachedRewriteHits": 361626,
  "cachedRewriteMisses": 17106
}
```

lookup 次数太多，说明 rewrite pass 仍在反复询问同一个 token。

### F1. 建立 NameResolutionCache

```cpp
struct NameResolutionCacheKey {
    std::string_view name;
    const StructInfo* currentStruct;
    const Decl* container;
    enum class Kind { Struct, Function, Field, Method, Interface } kind;
};
```

实际实现为了 hash 简单，可以用 interned id：

```cpp
struct NameId { uint32_t value; };
struct ScopeId { uint32_t value; };
```

### F2. 引入 string interning

标识符大量重复，例如 struct/function/global 名。建议做：

```cpp
class StringInterner {
public:
    uint32_t intern(std::string_view s);
    std::string_view get(uint32_t id) const;
};
```

`StructInfo` / `FunctionInfo` / `FieldInfo` 后续逐步换成 id。

Phase 15 只要求先用于 hot path lookup，不要求全项目替换。

### F3. 验收

```text
- structLookupCalls 降低 40%+。
- functionLookupCalls 降低 40%+。
- cachedRewriteMisses 不上升。
```

## Task G：syntax-lite 改为 validation-only，并增量利用 codegen flags

真实链路里 syntaxLite 约 9874 ms。日常 fast 编译不应强制跑。

### G1. fast 模式跳过 syntax-lite

```text
--mode fast 不跑 syntax-lite。
--mode validate 才跑。
--mode full-validation 才跑 detailed syntax-lite + examples。
```

### G2. codegen 内部记录 residue flags

很多 syntax-lite 检查可以在生成阶段顺手记录，而不是最终再全文件扫描：

```cpp
struct GenerationResidueFlags {
    bool emittedStructKeyword;
    bool emittedMethodKeyword;
    bool emittedZincSemicolon;
    bool emittedCommaLocal;
    bool emittedInlineZincControl;
    bool emittedIndexedStructResidue;
};
```

最终 syntax-lite 只在 full-validation 里做严查。

### G3. 验收

```text
- fast mode 不跑 syntax-lite。
- validate/full-validation 仍保持现有检查。
- syntaxLite validate 时间 <= 5000 ms。
```

## Task H：War3Lib 性能报告集成

### H1. 报告字段

在 `Output/compiler_backend_report.json` 中加入：

```json
{
  "vjasscMode": "fast|validate|full-validation",
  "vjasscInternal": {
    "read": 0,
    "preprocess": 0,
    "parse": 0,
    "codegen": 0,
    "syntaxLite": 0,
    "pjass": 0,
    "comparison": 0,
    "total": 0,
    "passTimings": {},
    "counters": {}
  }
}
```

### H2. ALPHA 默认策略

```text
TaskStartMapVjasscAlpha:
  默认 validate。
  可通过环境变量切 fast。

TaskStartMapCompareAlpha:
  默认 full-validation。
```

### H3. 验收

```text
- Xlimon ALPHA vjassc-selected compile report 中能看到 vjassc 内部 passTimings。
- both mode 能比较 jasshelper/vjassc 总耗时。
```

## Task I：保留 runtime/manual testing 入口

虽然 Phase 15 主攻性能，仍要允许继续手动进游戏。

### I1. 保持 ALPHA vjassc 任务

不得破坏：

```text
TaskStartMapVjasscAlpha.lua
TaskStartMapCompareAlpha.lua
🥭启动:内测-vjassc
🥭启动:内测-vjassc对比
```

### I2. 每次性能重构后至少跑

```text
- standalone PJASS validation
- War3Lib ALPHA vjassc-selected compile
- 一次 ALPHA map launch，由用户手动确认是否能进游戏
```

## 5. 建议 Codex 执行顺序

```text
Step 1: 增加 --mode fast|validate|full-validation，并接入 War3Lib。
Step 2: 替换 CodeWriter 为 FastCodeWriter。
Step 3: struct support 模板化，跳过通用 lowering。
Step 4: FunctionBodyIR 一次扫描 + feature flags。
Step 5: function ordering 迁移到 integer-id dependency graph。
Step 6: lookup cache / string interning 热点优化。
Step 7: syntax-lite validation-only 和增量 flags。
Step 8: 更新 docs/phase15_status.md、README、性能报告。
```

## 6. 禁止事项

```text
- 不要牺牲 PJASS pass。
- 不要把 validation-only external stub 写入正式 output。
- 不要为了速度删除 both mode 或 validation report。
- 不要一次性大改所有 AST/IR；先做 FunctionBodyIR 热点路径。
- 不要改 War3Lib 默认 jasshelper 后端，除非用户明确切换。
- 不要把 source function 自动补 return 的旧逻辑带回来。
```

## 7. Phase 15 交付物

```text
- docs/phase15_status.md
- README Phase 15 更新
- 新增/更新 CLI 文档：--mode fast|validate|full-validation
- War3Lib 接入文档或配置说明
- performance report 示例
- 至少 4 个性能相关 regression/golden fixtures
- 最新 Xlimon ALPHA vjassc benchmark
```

## 8. Phase 15 成功判定

最低成功：

```text
- PJASS pass。
- ALPHA vjassc-selected compile pass。
- vjassc internal total <= 35000 ms。
- vjassc codegen <= 25000 ms。
```

良好成功：

```text
- vjassc validate <= 25000 ms。
- fast compile <= 15000 ms。
- War3Lib ALPHA total <= 45000 ms。
```

优秀成功：

```text
- fast compile <= 10000 ms。
- validate <= 20000 ms。
- ALPHA 可进入游戏，用户未发现新增 runtime regression。
```

