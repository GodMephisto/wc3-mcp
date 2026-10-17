---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase16_codex_task_plan.md"
ref: HEAD
---

# Phase 16 Codex 实施计划：Struct Lowering 拆分 + Fast Mode 深度优化

## 0. 阶段定位

Phase 15 已经完成第一轮性能架构优化：

- 新增 `--mode fast|validate|full-validation`。
- `fast` 跳过 syntax-lite、PJASS、JassHelper comparison。
- `validate` 保留 syntax-lite / init validation / PJASS。
- `full-validation` 保留详细 report 与 JassHelper 结构对比。
- 旧 `std::ostringstream` writer 已替换为 reserved string-backed writer。
- 普通行在无相关语法时跳过 struct/member regex lowering。
- syntax-lite forward-reference scanning 已从两轮 regex 改成一次 token pass。
- War3Lib 已支持 `WAR3_VJASSC_MODE=fast|validate|full-validation`。

Phase 15 性能基线：

```text
Standalone fast:
  codegen:    19955 ms
  total:      20541 ms

Standalone validate:
  codegen:    20511 ms
  syntaxLite: 2200 ms
  pjass:      298 ms
  total:      23714 ms

Standalone full-validation:
  codegen:    19584 ms
  syntaxLite: 2200 ms
  pjass:      326 ms
  comparison: 342 ms
  total:      23065 ms

War3Lib/Xlimon ALPHA vjassc-selected validate:
  vjassc internal total: 26601 ms
  jassCompilerMs:       27234 ms
  totalCompileMs:       37812 ms

War3Lib/Xlimon ALPHA compare full-validation:
  vjassc internal total: 26451 ms
  jassCompilerMs:       39911 ms
  totalCompileMs:       52030 ms
```

Phase 14 baseline：

```text
codegen:    53921 ms
syntaxLite: 9874 ms
pjass:      348 ms
comparison: 121 ms
total:      65241 ms
```

Phase 15 已经把 standalone full-validation 从 `65241 ms` 压到 `23065 ms`，约快 `2.83x`。但是目前 `vjassc validate` 还没有稳定快过 JassHelper。Phase 14/15 中 JassHelper 真实链路参考值约 `12545 ms`，而 vjassc validate 仍约 `23~26s`。

Phase 16 的目标是继续把 `fast` 和 `validate` 模式压下去，重点优化 `emitStructSupport` / struct method lowering，而不是继续泛泛替换正则。

---

## 1. 阶段目标

### 1.1 最低验收目标

必须全部满足：

```text
1. PJASS 继续通过。
2. syntax-lite 继续通过。
3. init validation 继续通过。
4. duplicate function/global/native names 继续为 0。
5. Standalone fast total <= 14000 ms。
6. Standalone validate total <= 17000 ms。
7. War3Lib/Xlimon ALPHA vjassc-selected validate totalCompileMs <= 32000 ms。
8. 不引入新的 runtime regression。
```

### 1.2 良好目标

```text
1. Standalone fast total <= 8000 ms。
2. Standalone validate total <= 12000 ms。
3. War3Lib/Xlimon ALPHA vjassc fast 的 jassCompilerMs <= 同链路 JassHelper elapsedMs。
4. `emitStructSupport` 或等价 struct lowering 总耗时下降至少 50%。
```

### 1.3 优秀目标

```text
1. Standalone fast total <= 5000 ms。
2. Standalone validate total <= 8000 ms。
3. War3Lib/Xlimon ALPHA vjassc validate 接近或快于 JassHelper。
4. Full-validation total <= 15000 ms。
```

---

## 2. 当前性能瓶颈判断

### 2.1 正则已经不是唯一主瓶颈

Phase 12 到 Phase 15 已经消除了大量 regex-heavy 路径：

- function-order dependency scan 已改为 manual identifier scanner。
- syntax-lite forward-reference scan 已改为单次 token pass。
- 普通行无相关语法时跳过 struct/member regex lowering。
- `.name` token 不存在时跳过 `.name` function-reference rewriting。

因此 Phase 16 不应只做“继续找正则替换”。剩余瓶颈更可能是：

```text
1. struct support / struct method lowering 混在一起。
2. generated support functions 仍经过不必要的通用 lowering。
3. 函数体行被多轮扫描和多轮字符串重写。
4. struct/function lookup 次数仍过高。
5. function ordering 仍基于输出文本块，而非编译期 FunctionId 图。
6. validate/full-validation 模式仍有最终输出扫描和 report 生成成本。
```

Phase 15 hot path 指出：

```text
emitStructSupport includes all struct method lowering and is still the main codegen hotspot at roughly 11 seconds.
```

所以 Phase 16 的核心应是 **拆分 struct support 与 source struct method lowering**。

---

## 3. 总体策略

Phase 16 不做大范围语义改动，不新增 vJass/Zinc 语法。只做性能架构优化，并保持输出通过 PJASS。

目标 pipeline：

```text
旧路径：
  struct support + source methods 混合 emit
  每行进入多轮 generic lowering
  最终 output 再扫描/修补

Phase 16 目标路径：
  collect 阶段预计算 StructInfo / MethodInfo / FunctionInfo
  generated struct support 使用模板直接输出
  source struct methods 进入 source-method lowering
  每个函数体先生成轻量 LineFeatures / FunctionBodyPlan
  lowering pass 根据 feature gates 跳过无关逻辑
  function ordering 使用整数 ID 图
  fast 模式跳过所有 validation-only scan
```

---

## 4. 任务 1：建立更细的性能剖析

### 4.1 新增 pass timing

在 codegen 内增加更细粒度 timing，不改变行为。

新增或细分这些计时项：

```text
emitStructSupport.total
emitStructSupport.generatedHelpers
emitStructSupport.sourceMethods
emitStructSupport.methodBodyLowering
emitStructSupport.generatedBodyLowering
emitStructSupport.fixedArraySupport
emitStructSupport.lifecycleSupport

emitFunctions.total
emitFunctions.jassFunctions
emitFunctions.zincFunctions
emitFunctions.lambdaEmit
emitFunctions.functionInterfaceRuntime
emitFunctions.bodyLowering

lowering.rewriteReceiverChains
lowering.rewriteStructExpression
lowering.rewriteArrayAccesses
lowering.rewriteCallArguments
lowering.lowerExpression
lowering.rewriteFunctionNames
lowering.rewriteForContainer

validation.syntaxLiteForwardScan
validation.syntaxLiteResidueScan
validation.finalOutputValidationPrep
```

### 4.2 新增 counters

新增 counters：

```text
lineFeatureScans
linesSkippedNoDotBracketCall
linesSkippedNoCurrentStruct
linesSkippedGeneratedSupport
structMethodLinesLowered
generatedSupportLinesEmitted
generatedSupportLinesLowered
receiverChainAttempts
receiverChainChanged
arrayAccessRewriteAttempts
arrayAccessRewriteChanged
functionOrderTokenScans
functionOrderEdges
functionOrderSccCount
```

### 4.3 交付要求

更新：

```text
docs/phase16_status.md
build/input.phase16.fast.stats.json
build/input.phase16.validate.stats.json
build/input.phase16.full.stats.json
Output/compiler_backend_report.json
```

`docs/phase16_status.md` 必须列出 Phase 15 -> Phase 16 的 timing delta。

---

## 5. 任务 2：拆分 emitStructSupport

### 5.1 当前问题

当前 `emitStructSupport` 同时承担：

```text
1. struct allocate/create/destroy/deallocate support。
2. struct fixed-size array support。
3. onDestroy/onInit wrappers。
4. 用户声明的 struct methods。
5. method body lowering。
```

这会导致 generated support functions 也可能进入通用 lowering 路径。

### 5.2 目标结构

拆成：

```cpp
void emitStructGeneratedSupport(const StructInfo& info);
void emitStructLifecycleSupport(const StructInfo& info);
void emitStructFixedArraySupport(const StructInfo& info);
void emitStructSourceMethods(const StructInfo& info);
void emitStructSourceMethod(const StructInfo& info, const MethodInfo& method);
```

或拆到独立文件：

```text
src/codegen/StructSupportEmitter.h/.cpp
src/codegen/StructMethodLowerer.h/.cpp
```

若一次拆文件风险太大，可先在 `Phase1Codegen.cpp` 内拆函数，Phase 17 再拆文件。

### 5.3 Generated support 直接模板输出

以下函数应直接模板化生成，不走通用 lowering：

```text
s__Struct__allocate
s__Struct_create/default create
s__Struct_destroy
s__Struct_deallocate
s__Struct_onDestroy wrapper
s__Struct_onInit wrapper
struct array fixed-storage helpers
```

模板输出时只允许做必要替换：

```text
- struct prefix
- generated field names
- instance limit
- recycle globals
- onDestroy target function name
```

禁止 generated support 进入这些逻辑，除非明确必要：

```text
- extractZincLambdas
- rewriteReceiverChains
- lowerExpression
- rewriteCallArguments
- inferUniqueFunctionInterfaceTypeForFunctionValue
- rewriteFunctionObject .execute/.evaluate
- syntax cleanup 猜测式重写
```

### 5.4 给 generated function 加类型标记

新增：

```cpp
enum class GeneratedKind {
    None,
    StructAllocate,
    StructCreate,
    StructDestroy,
    StructDeallocate,
    StructOnDestroyWrapper,
    StructOnInitWrapper,
    FunctionInterfaceWrapper,
    LambdaWrapper,
    CycleBridge,
};
```

函数体 lowering 根据 `GeneratedKind` 判断是否跳过特定 pass。

### 5.5 验收

新增 golden fixtures：

```text
tests/fixtures/phase16_struct_generated_support.in.j
tests/fixtures/phase16_struct_source_method_lowering.in.j
tests/fixtures/phase16_struct_on_destroy.in.j
tests/fixtures/phase16_struct_deallocate_direct.in.j
```

验证：

```text
1. PJASS 通过。
2. output 中 allocate/create/destroy/deallocate 行为不变。
3. fixed-size struct arrays 行为不变。
4. struct onInit dependency ordering 不回退。
5. emitStructSupport.sourceMethods 与 generatedHelpers timing 分离。
```

---

## 6. 任务 3：FunctionBodyPlan / LineFeatures 一次扫描

### 6.1 当前问题

大量函数体按行被多轮检查：

```text
- 是否有 .
- 是否有 []
- 是否有 function(...)
- 是否有 .evaluate/.execute
- 是否有 this
- 是否有 call/set/local/return
- 是否有 string/rawcode/comment 保护区
```

这些判断应该一次完成，然后 pass 根据 flags 跳过。

### 6.2 新增数据结构

建议：

```cpp
struct LineFeatures {
    std::string_view original;
    std::string masked; // strings/rawcodes/comments blanked only if needed

    bool hasDot = false;
    bool hasBracket = false;
    bool hasParen = false;
    bool hasCall = false;
    bool hasSet = false;
    bool hasLocal = false;
    bool hasReturn = false;
    bool hasFunctionKeyword = false;
    bool hasLambdaStart = false;
    bool hasThis = false;
    bool hasNameToken = false;        // .name
    bool hasExecuteEvaluate = false;  // .execute/.evaluate
    bool hasBooleanOperators = false; // && || ! and/or
    bool hasPossibleStructMember = false;
    bool hasStringOrRawcode = false;
};

struct FunctionBodyPlan {
    std::vector<LineFeatures> lines;
    bool containsLambda = false;
    bool containsStructMemberAccess = false;
    bool containsFunctionInterfaceCall = false;
    bool containsArrayAccess = false;
    bool containsZincOnlyForms = false;
};
```

### 6.3 应用规则

在 lowering 时：

```text
if (!features.hasDot && !features.hasBracket && !features.hasThis)
    skip struct receiver/member lowering

if (!features.hasFunctionKeyword && !features.hasLambdaStart)
    skip lambda/function-reference lowering

if (!features.hasExecuteEvaluate)
    skip function-object execute/evaluate lowering

if (!features.hasBooleanOperators)
    skip boolean/operator normalization

if (!features.hasNameToken)
    skip .name rewrite
```

### 6.4 注意事项

- Feature scan 必须保护 string/rawcode/comment，不允许把字符串里的 `.`、`function`、`=` 当语法。
- 不要把 `masked` 强制存每一行。可以 lazy：只有 line 含 `"`、`'`、`//` 时才生成 masked。
- 不要在 Phase 16 引入完整 parser；这里只做轻量 scanner。

### 6.5 验收

新增 counters：

```text
lineFeatureScans
linesSkippedNoDotBracketCall
linesSkippedNoCurrentStruct
```

目标：

```text
至少 60% 以上普通行不进入 struct receiver lowering。
```

---

## 7. 任务 4：Function ordering 改为整数 ID 图

### 7.1 当前问题

Phase 15 已经把 function ordering 的正则降掉，但它仍是 output-string based，约 `600~750 ms`。

### 7.2 目标算法

构建：

```cpp
using FunctionId = uint32_t;

struct FunctionNode {
    FunctionId id;
    std::string name;
    FunctionSignature sig;
    std::vector<FunctionId> deps;
    GeneratedKind generatedKind;
};
```

算法：

```text
1. 编译期 collect 所有 function name -> FunctionId。
2. 每个函数体用 LineFeatures token scan 提取 identifier call。
3. dependency 用 FunctionId 存储，不用 std::set<std::string>。
4. Tarjan SCC 找循环。
5. SCC 内按现有 bridge 策略处理。
6. SCC 外用 Kahn topo sort 排序。
7. 原始顺序作为 stable tie-breaker。
```

### 7.3 注意事项

- 必须保留现有 cycle bridge 语义。
- function references：`function Foo` 也算依赖。
- `ExecuteFunc("Foo")` 字符串不应当强制重排，除非已有逻辑处理。
- generated bridge functions 要纳入图，避免再做输出后 patch。

### 7.4 验收

```text
functionOrdering <= 200 ms
PJASS 通过
5 个历史 forward-cycle fixtures 继续通过
```

---

## 8. 任务 5：Lookup 降频与 ID 化

### 8.1 当前问题

Phase 15 counters：

```text
structLookupCalls:   102092
functionLookupCalls: 141163
cachedRewriteMisses: 16735
```

Lookup 次数仍偏高。

### 8.2 优化策略

#### 8.2.1 struct/function name interning

新增简单 interner：

```cpp
using SymbolId = uint32_t;

class StringInterner {
public:
    SymbolId intern(std::string_view text);
    std::string_view text(SymbolId id) const;
};
```

优先在内部 indexes 使用 ID，不要求一次性改完整 AST。

#### 8.2.2 StructIndex

新增：

```cpp
struct StructIndex {
    std::unordered_map<std::string, StructId> bySourceName;
    std::unordered_map<std::string, StructId> byGeneratedName;
    std::unordered_map<std::string, StructId> byPrefix;
};
```

#### 8.2.3 FunctionIndex

新增：

```cpp
struct FunctionIndex {
    std::unordered_map<std::string, FunctionId> bySourceName;
    std::unordered_map<std::string, FunctionId> byFinalName;
};
```

#### 8.2.4 Local type cache

对每个函数体的 localTypes 和 receiver types 建立局部缓存，避免每行重复通过字符串推断 struct。

### 8.3 验收

目标：

```text
structLookupCalls <= 60000
functionLookupCalls <= 80000
cachedRewriteMisses 不上升
PJASS 通过
```

---

## 9. 任务 6：Fast mode 进一步瘦身

### 9.1 目标

`--mode fast` 应只做：

```text
read / preprocess / staticIf / lex / parse / moduleExpand / codegen / write output / write stats
```

不做：

```text
syntax-lite
PJASS
comparison
validation report examples
JassHelper structure comparison
full output residue scan
PJASS grouped report prep
```

### 9.2 检查点

确认 fast mode 下：

```text
syntaxLite == 0
pjass == 0
comparison == 0
finalOutputValidationPrep 尽可能为 0 或接近 0
```

如果 `finalOutputValidationPrep` 在 fast 下仍有耗时，需要拆成：

```text
requiredFinalization
validationOnlyPrep
```

### 9.3 War3Lib 集成

增加一个专门对比任务或配置：

```text
TaskCompileCompareAlphaFastVjassc.lua
```

或在现有 compare task 中支持：

```bat
set WAR3_VJASSC_MODE=fast
lua lua/tasks/TaskCompileCompareAlpha.lua ...
```

报告必须包含：

```json
{
  "jasshelper": { "elapsedMs": ... },
  "vjassc": {
    "mode": "fast",
    "elapsedMs": ...,
    "internal": { "codegen": ..., "total": ... }
  }
}
```

### 9.4 验收

```text
War3Lib both + vjassc fast 能跑出同链路 jasshelper/vjassc 时间对比。
```

---

## 10. 任务 7：Syntax-lite 单 pass 化

### 10.1 当前问题

Validate 模式 syntax-lite 约 `2.2~3.0s`。虽然不是最大瓶颈，但可以继续压。

### 10.2 目标

把 syntax-lite 合并为一次 output line scan：

```text
- duplicate function/global/native check
- residual source forms
- comma locals
- indexed struct residues
- inline Zinc control residues
- forward references metadata
```

不要每个检查重新扫一遍 output。

### 10.3 验收

```text
syntaxLite <= 1200 ms
validate 模式 PJASS 通过
```

---

## 11. 任务 8：保持运行时安全

Phase 14 已经接入真实 War3Lib/Xlimon ALPHA 链路。Phase 16 优化不能破坏运行时。

每次大改后至少执行：

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

并保留：

```text
Output/5_vjassc.j
Output/output.j
Output/vjassc.stats.json
Output/vjassc.validation.json
Output/compiler_backend_report.json
Output/runtime_notes.md
```

如用户进行 ALPHA 进游戏测试，应记录：

```text
- 是否进入地图
- 是否完成初始化
- Museum/UI/F2 open-close 是否正常
- 英雄选择/基础 UI 是否正常
- DzAPI/YDWE/JAPI 相关功能是否报错
- 是否出现 handle/null/destroy 类异常
```

---

## 12. 执行顺序建议

Codex 请按以下顺序执行，避免一次大重构失控：

```text
Step 1: 增加更细 timing/counters，不改行为。
Step 2: 拆 emitStructSupport 的 generated support 与 source methods。
Step 3: generated support 模板化，跳过通用 lowering。
Step 4: 引入 LineFeatures / FunctionBodyPlan，先只用于 skip gates。
Step 5: function ordering 改为 FunctionId 图。
Step 6: lookup 降频与 ID 化。
Step 7: fast mode validation-only prep 清理。
Step 8: syntax-lite 单 pass 化。
Step 9: War3Lib fast-vs-jasshelper 同链路 benchmark。
Step 10: 写 docs/phase16_status.md。
```

每个 Step 完成后至少跑：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\input.j -o build\input.phase16.fast.out.j --mode fast --emit-stats build\input.phase16.fast.stats.json
```

关键 Step 后跑：

```bat
build\vjassc.exe samples\input.j -o build\input.phase16.validate.out.j --mode validate --emit-stats build\input.phase16.validate.stats.json --emit-validation-report build\input.phase16.validate.validation.json --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi
```

最终必须跑 War3Lib ALPHA validate。

---

## 13. 文档交付要求

新增：

```text
docs/phase16_status.md
```

必须包含：

```text
1. Phase 15 baseline。
2. Phase 16 standalone fast / validate / full-validation timing。
3. Phase 16 War3Lib ALPHA vjassc validate timing。
4. Phase 16 War3Lib both mode jasshelper vs vjassc fast timing。
5. PJASS 是否通过。
6. syntax-lite 是否通过。
7. runtime/manual test 记录。
8. pass timings。
9. performance counters。
10. 哪些优化有效，哪些无效。
```

建议表格：

```text
| Metric | Phase 15 | Phase 16 | Delta |
| --- | ---: | ---: | ---: |
| standalone fast total | 20541 | ... | ... |
| standalone validate total | 23714 | ... | ... |
| standalone codegen | 19955/20511 | ... | ... |
| syntaxLite | 2200 | ... | ... |
| War3Lib vjassc selected totalCompileMs | 37812 | ... | ... |
| War3Lib jassCompilerMs | 27234 | ... | ... |
| structLookupCalls | 102092 | ... | ... |
| functionLookupCalls | 141163 | ... | ... |
```

---

## 14. 禁止事项

```text
1. 不允许为了速度跳过实际输出所需的 lowering。
2. 不允许破坏 PJASS pass。
3. 不允许把 validation-only InitTrig_japi stub 写入正式 output.j。
4. 不允许默认切换 War3Lib 正式版本到 vjassc。
5. 不允许大规模删除 runtime checklist/report。
6. 不允许为了性能移除 critical diagnostics。
7. 不允许无 fixture 地修改 struct allocation/destroy/deallocate 语义。
```

---

## 15. 最终验收命令

### 15.1 JassChanger standalone fast

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase16.fast.out.j ^
  --mode fast ^
  --emit-stats build\input.phase16.fast.stats.json
```

### 15.2 JassChanger standalone validate

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase16.validate.out.j ^
  --mode validate ^
  --emit-stats build\input.phase16.validate.stats.json ^
  --emit-validation-report build\input.phase16.validate.validation.json ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

### 15.3 War3Lib/Xlimon ALPHA vjassc-selected validate

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

### 15.4 War3Lib/Xlimon ALPHA compare fast

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

### 15.5 War3Lib/Xlimon ALPHA compare full-validation

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=full-validation
lua lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

---

## 16. 预期结论

如果 Phase 16 成功，预计会出现以下结果：

```text
1. vjassc fast 首次接近或超过 JassHelper。
2. validate 模式接近 JassHelper。
3. full-validation 保持可接受，但不是日常开发默认路径。
4. emitStructSupport 不再是 10s+ 热点。
5. 下一阶段可以进入更深层的 IR/codegen 架构优化，目标 5s 以内。
```
