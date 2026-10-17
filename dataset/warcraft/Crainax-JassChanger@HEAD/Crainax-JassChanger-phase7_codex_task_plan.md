---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase7_codex_task_plan.md"
ref: HEAD
---

# JassChanger 阶段 7 Codex 详细实施计划

> 项目：`Crainax/JassChanger`
> 当前阶段：Phase 6 已完成
> 本阶段主题：**PJASS 阻断错误消除：声明级冲突、多维数组 flatten、注释泄漏、struct 生成名一致性、级联 undefined 修复**
> 目标输入：`samples/input.j`
> 参考输出：`samples/output_jasshelper.j`
> 工具链：C++20 + CMake + Ninja + PJASS

---

## 0. 阶段 7 的核心目标

阶段 6 已经完成了完整 codegen、syntax-lite、validation report、PJASS 调用和 JassHelper 结构级对比；但真实 `samples/input.j` 的生成文件仍然无法通过 PJASS。

阶段 7 不再扩展新语法，不做 interface/delegate/operator/stub/super，不做大规模性能优化。阶段 7 只围绕 **PJASS 当前第一批阻断错误** 修复：

1. 和 `common.j` / `blizzard.j` / PJASS 环境重复的 declaration。
2. block comment 泄漏进 parser / global detection / output lowering。
3. 多维数组声明和访问没有完整降级。
4. struct support 生成名大小写或命名不一致。
5. duplicate generated function/global names。
6. `local` 声明位置、`return` 类型、undefined function/variable 的第一批级联错误。

阶段 7 的最终目标是让真实输入的 PJASS 错误从 phase 6 的一万级级联错误下降到可人工审查的规模。理想目标是 PJASS 直接通过；如果不能直接通过，至少必须显著压低错误数量并在 `docs/phase7_status.md` 中保留清晰的剩余 blocker 分类。

---

## 1. 当前 Phase 6 基线

Codex 开始前必须先确认当前仓库能复现 Phase 6 基线。

### 1.1 构建与测试

Windows：

```bat
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

Linux / WSL：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

### 1.2 生成 phase 7 baseline 输出

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase7.baseline.out.j ^
  --emit-stats build\input.phase7.baseline.stats.json ^
  --emit-validation-report build\input.phase7.baseline.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite
```

### 1.3 运行 PJASS baseline

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase7.baseline.pjass.out.j ^
  --emit-stats build\input.phase7.baseline.pjass.stats.json ^
  --emit-validation-report build\input.phase7.baseline.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j
```

预期当前仍失败。不要因为 PJASS 失败而认为阶段 7 启动失败。

---

## 2. 阶段 7 边界

### 2.1 必须做

- 保持 `samples/input.j` 能完整 codegen。
- 保持 `--check-output-syntax-lite` 通过。
- 保持 validation report 可写出。
- 保持 PJASS 可以被执行并捕获 stdout/stderr。
- 修复 phase 6 记录的首批 PJASS blocker。
- 添加针对每类修复的 fixture。
- 更新 README 和 `docs/phase7_status.md`。

### 2.2 不要做

- 不要开始实现完整 interface dispatch。
- 不要实现 delegate。
- 不要实现 operator overload 全体系。
- 不要实现 stub/super。
- 不要做 JassHelper byte-for-byte 输出匹配。
- 不要以性能优化为主线。
- 不要跳过 PJASS log 中最早的 declaration/syntax blocker 去随机修后面的 undefined。

### 2.3 可以做的小型重构

可以做下列低风险重构，但必须保持测试通过：

- 增加 `ArrayLowerer` / `CommentSanitizer` / `GeneratedNameRegistry` 等独立模块。
- 增强 validation report 的 PJASS summary。
- 增强 syntax-lite 检查。
- 给 `Phase1Codegen` 增加局部辅助类，但不要在阶段 7 做大规模文件重命名。

`Phase1Codegen` 命名债可以继续记录为已知问题，除非改名不会影响本阶段目标。

---

## 3. 本阶段总验收标准

阶段 7 完成时，至少满足：

```text
build passes
ctest passes
samples/input.j full codegen succeeds
--check-output-syntax-lite passes
validation report generated
PJASS runs and stdout/stderr captured
```

并且针对 phase 6 的 PJASS 分类，至少达到：

```text
duplicateDeclaration: 0 或接近 0
localOrder: 0
expectedEndfunction: 明显下降，理想为 0
multi-dimensional array syntax errors: 0
struct generated-name consistency issues: 0
syntaxError: 显著下降
undefinedVariable / undefinedFunction: 显著下降
```

理想最终验收：

```text
PJASS ok: true
exitCode: 0
```

如果 PJASS 仍未通过，必须在 `docs/phase7_status.md` 中列出剩余错误分类、数量、前 20 个具体样例、推测根因和下一阶段建议。

---

## 4. Workstream A：PJASS log triage 工具增强

### 4.1 目标

Phase 6 已经有 PJASS 错误分类，但阶段 7 需要更精细的分类和定位。不要只看总数，要能知道每类错误的第一批具体位置。

### 4.2 实施任务

1. 增强 validation report 中的 `pjass` 节点。
2. 对 PJASS stdout 做 fingerprint 分类。
3. 每类错误记录：
   - `count`
   - `firstLine`
   - `firstMessage`
   - `examples[]`，最多 20 条
   - `generatedOutputLine`
   - `generatedOutputExcerpt`，上下各 3 行
4. 识别并单独分类：
   - duplicate declaration
   - syntax error
   - undefined variable
   - undefined function
   - expected endfunction
   - local declaration after statement
   - return type mismatch
   - invalid array syntax
   - invalid comparison
   - unknown type
5. 在 `build/input.phase7.pjass.validation.json` 中保留分类。

### 4.3 建议输出 JSON 结构

```json
{
  "pjass": {
    "requested": true,
    "ran": true,
    "ok": false,
    "exitCode": 1,
    "summary": {
      "duplicateDeclaration": 0,
      "syntaxError": 0,
      "undefinedVariable": 0
    },
    "groups": [
      {
        "kind": "invalidArraySyntax",
        "count": 12,
        "examples": [
          {
            "message": "...",
            "generatedLine": 12345,
            "excerpt": ["..."]
          }
        ]
      }
    ]
  }
}
```

### 4.4 验收

- PJASS 失败时仍能稳定写 validation report。
- report 中能快速定位前 20 个错误，不需要人工翻完整 stdout。
- `ctest` 通过。

---

## 5. Workstream B：common/blizzard/PJASS 环境重复声明处理

### 5.1 背景

Phase 6 PJASS 首批 blocker 里有 duplicate declaration，最早出现 `YDHT`、`YDLOC`。同时阶段 6 仍有 duplicate function/global 统计：

```text
duplicateFunctionNames: 85
duplicateGlobalNames: 7
```

vJass/JassHelper 的行为之一是把 native 移到正确位置，并避免与基础环境重复冲突。因此阶段 7 需要比 phase 6 更严格地区分：

- `common.j` / `blizzard.j` 已存在的 native/type/function/global。
- `samples/input.j` 自己的重复声明。
- `library_once` 合法忽略。
- 编译器生成名重复。

### 5.2 实施任务

#### B1. 建立环境符号扫描器

新增或扩展：

```text
src/validation/EnvironmentSymbols.h
src/validation/EnvironmentSymbols.cpp
```

或放到现有 validation/pjass 模块中。

需要能从 `--common`、`--blizzard` 指定文件中扫描：

```text
native NAME takes ... returns ...
function NAME takes ... returns ...
globals ... endglobals
    TYPE NAME
    TYPE array NAME
endglobals
type NAME extends BASE
```

注意：

- 只需要可靠提取名字，不必完整语义分析。
- 忽略注释、字符串、rawcode。
- common/blizzard 中重复的符号可记录 warning，但不要影响生成。

#### B2. 输出阶段过滤重复 native/type/global

对于生成文件中要输出的 native/type/global：

- 如果与环境完全同名且同类，默认不再输出。
- 如果同名但不同类，保留并在 validation report 标记为 high-risk。
- 对 Dz/JAPI/custom native 不在 common/blizzard 中的，必须继续输出。

不要因为名字看起来像 YD/Dz 就随意删除。必须以环境符号表为准。

#### B3. 自身重复声明检查

在 output validation 中新增：

```json
"duplicateDeclarations": {
  "functions": [...],
  "globals": [...],
  "natives": [...],
  "types": [...]
}
```

并要求：

```text
duplicateFunctionNames == 0
duplicateGlobalNames == 0
```

如果生成名重复，进入 Workstream E 修复。

### 5.3 Fixtures

新增测试：

```text
tests/fixtures/phase7_env_duplicate_native.in.j
tests/fixtures/phase7_env_duplicate_native.expected.j
tests/fixtures/phase7_duplicate_generated_global.in.j
tests/fixtures/phase7_library_once_duplicate.in.j
```

### 5.4 验收

- PJASS `duplicateDeclaration` 明显下降，目标为 0。
- `duplicateFunctionNames` 和 `duplicateGlobalNames` 为 0。
- DzAPI/native 不被误删。
- `samples/input.j` full codegen 仍通过 syntax-lite。

---

## 6. Workstream C：block comment 泄漏修复

### 6.1 背景

Phase 6 首批 blocker 里提到 `samples/input.j:42294` 附近 block-comment line 仍可能泄漏进 generated global analysis。

这类问题会导致：

- 注释内容被误认为 global declaration。
- 注释里的 `[`、`]`、`function`、`struct` 等干扰 parser/lowering。
- PJASS 看到非法源文本。

### 6.2 实施任务

#### C1. 新增统一 CommentSanitizer

建议新增：

```text
src/preprocess/CommentSanitizer.h
src/preprocess/CommentSanitizer.cpp
```

或在 `Preprocessor` 中集中处理。

要求：

- 支持 `//` 行注释。
- 支持 `/* ... */` block comment。
- 支持跨行 block comment。
- 不处理字符串内部的 `//`、`/*`、`*/`。
- 不处理 rawcode literal 内部的字符，例如 `'A000'`。
- 尽量保留 LogicalLine 的 source location。
- 可以把注释替换为空白，不要让后续 parser/lowering 看到注释内部内容。

#### C2. 应用位置

CommentSanitizer 必须至少作用于：

```text
pre-parse unsupported scan
static-if symbol collection
parser declaration detection
global/native/function detection
codegen line rewriting
syntax-lite validation
multi-dimensional array detection
```

不要只在一个地方修。阶段 6 的问题很可能来自不同模块各自用 trim/regex 处理文本。

#### C3. 保护字符串/rawcode

必须新增测试保证：

```jass
call BJDebugMsg("/* not comment */")
set s = "// not comment"
set id = 'A/*x'
```

不会被破坏。

### 6.3 Fixtures

新增：

```text
tests/fixtures/phase7_block_comment_globals.in.j
tests/fixtures/phase7_block_comment_zinc.in.j
tests/fixtures/phase7_block_comment_strings_rawcodes.in.j
```

### 6.4 验收

- output 中不残留 block comment 内容到 globals/function body。
- `input.j:42294` 附近问题消失。
- PJASS 中因注释泄漏造成的 syntax errors 消失。
- syntax-lite 继续通过。

---

## 7. Workstream D：多维数组 deterministic flattening

### 7.1 背景

PJASS 发现类似：

```text
[9][16]
[8][4]
[14][6]
```

这说明生成输出里仍有普通 JASS 不支持的多维数组声明或访问。JASS 最终只能接受一维 array，因此阶段 7 必须做 deterministic flattening。

### 7.2 支持范围

阶段 7 至少支持真实 `input.j` 中出现的多维数组形态，包括：

```jass
integer a[9][16]
real b[8][4]
string c[14][6]
a[i][j]
set a[i][j] = value
call Foo(a[i][j])
```

还要考虑：

```jass
static integer a[9][16]
struct field[9][16]
local integer a[9][16]
this.field[i][j]
obj.field[i][j]
```

如果真实输入没有 3D，可以先实现 2D，但代码结构要允许 3D。

### 7.3 数据结构建议

当前 AST 里可能只有 `isArray`、`isFixedArray`、`fixedArraySize`。阶段 7 建议扩展为：

```cpp
struct ArrayShape {
    bool isArray = false;
    std::vector<int> dimensions;   // [9, 16]
    bool dynamicLast = false;
};
```

然后在：

```cpp
FieldDecl
GlobalDecl or raw global line representation
LocalDecl or lowering context
```

中记录 shape。

如果当前 parser 仍不适合完整 AST 化，也可以先在 codegen/lowering 阶段建立：

```cpp
std::unordered_map<std::string, ArrayShape> globalArrayShapes;
std::unordered_map<std::string, ArrayShape> localArrayShapes;
std::unordered_map<std::string, ArrayShape> structFieldArrayShapes;
```

但必须保证所有后续表达式 rewriting 能查到 shape。

### 7.4 Flatten 规则

二维：

```text
A[i][j] -> A[(i) * DIM2 + (j)]
```

三维：

```text
A[i][j][k] -> A[((i) * DIM2 + (j)) * DIM3 + (k)]
```

固定常量：

```jass
integer a[9][16]
```

输出：

```jass
integer array a
constant integer a__dim0 = 9
constant integer a__dim1 = 16
constant integer a__size = 144
```

是否输出 dim constants 可选，但建议输出，便于调试和 validation。若担心名字冲突，使用编译器前缀：

```jass
constant integer vjassc__arr_a_dim0 = 9
constant integer vjassc__arr_a_dim1 = 16
constant integer vjassc__arr_a_size = 144
```

### 7.5 Struct field flattening

对于 struct instance field：

```jass
struct S
    integer grid[9][16]
endstruct
```

建议存储：

```jass
integer array s__S_grid
constant integer s__S_grid_dim0 = 9
constant integer s__S_grid_dim1 = 16
constant integer s__S_grid_size = 144
```

访问：

```jass
this.grid[i][j]
```

降级：

```jass
s__S_grid[this * 144 + (i) * 16 + (j)]
```

对象访问：

```jass
obj.grid[i][j]
```

降级：

```jass
s__S_grid[obj * 144 + (i) * 16 + (j)]
```

如果已有 struct field fixed-array 使用另一套 indexing，必须统一，不要新旧规则混用。

### 7.6 Static struct field flattening

```jass
static integer grid[9][16]
```

静态字段不需要 `this * size`：

```jass
s__S_grid[(i) * 16 + (j)]
```

### 7.7 Local multidimensional arrays

```jass
local integer a[9][16]
```

输出：

```jass
local integer array a
```

访问同样 flatten。不要在 JASS local declaration 中输出 `[9][16]`。

### 7.8 Parser 和 codegen 注意事项

- 不要在字符串、注释、rawcode 中改写 `[x][y]`。
- 要能处理括号表达式：`a[i + 1][j * 2]`。
- 要能处理 function call index：`a[GetI()][GetJ()]`。
- 不要误改普通 array + index 后接其他 bracket 的非数组表达式，除非 shape 表确认该变量是多维数组。
- 不要生成超过 JASS 支持的非法 local declaration。

### 7.9 Fixtures

新增：

```text
tests/fixtures/phase7_multidim_global_zinc.in.j
tests/fixtures/phase7_multidim_local_zinc.in.j
tests/fixtures/phase7_multidim_struct_field.in.j
tests/fixtures/phase7_multidim_static_struct_field.in.j
tests/fixtures/phase7_multidim_expression_indices.in.j
```

每个 fixture 的 expected 输出必须不包含：

```text
[9][16]
[8][4]
[14][6]
```

### 7.10 Validation 增强

syntax-lite 中新增检查：

```text
No declaration contains multiple bracket dimensions.
No expression contains chained indexing on a known multidimensional source name.
```

如果输出中仍有 `[number][number]`，validation report 必须 fail。

### 7.11 验收

- PJASS 中由 `[9][16]` 等引发的 syntax errors 消失。
- syntax-lite 新规则通过。
- `samples/input.j` full codegen 通过。

---

## 8. Workstream E：struct generated-name consistency

### 8.1 背景

Phase 6 记录了类似：

```text
sc__BaseAnim_baseanim_onDestroy
si__baseanim_V
```

这说明 struct generated name 在不同路径中大小写或命名策略不一致。

vJass/JASS 是区分大小写的，不能把 `BaseAnim` 和 `baseanim` 当成同一个名字。

### 8.2 实施任务

#### E1. 建立唯一 canonical struct generated-name

在 collect struct 时生成一次：

```cpp
StructInfo.generatedName
StructInfo.prefix
StructInfo.originalName
```

之后所有地方只引用 `StructInfo.generatedName`，不要重新 lowercase、重新拼接、重新猜测。

禁止在后续函数中用类似：

```cpp
toLower(structName)
normalize(name)
```

重新构造 struct support 名。

#### E2. 统一命名前缀

明确当前编译器最终使用的命名模型，例如：

```text
si__<StructGenerated>_F
si__<StructGenerated>_I
si__<StructGenerated>_V
s__<StructGenerated>_<field>
sc__<StructGenerated>_<method>
```

无论实际采用什么前缀，必须保证：

- declaration 和 reference 完全一致。
- field array declaration 和 access 完全一致。
- onDestroy wrapper declaration 和 call 完全一致。
- static method code reference 完全一致。

#### E3. 生成名 registry

新增 `GeneratedNameRegistry`：

```cpp
class GeneratedNameRegistry {
public:
    std::string reserveFunction(std::string desired, SourceLocation loc);
    std::string reserveGlobal(std::string desired, SourceLocation loc);
    bool containsFunction(std::string_view name) const;
    bool containsGlobal(std::string_view name) const;
};
```

如果发生冲突：

- 对用户函数/global，按 vJass 规则报错或按 existing policy。
- 对编译器生成物，添加 deterministic suffix。
- suffix 必须同步更新所有 reference。

#### E4. Validation 增强

在 output validation 中检查：

```text
All generated struct globals referenced are declared.
All generated struct support functions called are declared.
All generated onDestroy functions called are declared.
```

可以先用 regex 近似检查：

```text
set/call references starting with si__, s__, sc__
```

### 8.3 Fixtures

新增：

```text
tests/fixtures/phase7_struct_case_consistency.in.j
tests/fixtures/phase7_struct_onDestroy_case.in.j
tests/fixtures/phase7_struct_static_method_reference_case.in.j
```

### 8.4 验收

- `duplicateFunctionNames` 降到 0。
- `duplicateGlobalNames` 降到 0。
- PJASS 中相关 undefined function/variable 明显下降。
- `sc__BaseAnim_baseanim_onDestroy` / `si__baseanim_V` 这类大小写不一致消失。

---

## 9. Workstream F：local-order 和 function body JASS 合法性

### 9.1 背景

Phase 6 仍有：

```text
localOrder: 9
expectedEndfunction: 153
returnMismatch: 63
```

这些可能部分是前面语法错误级联，但也可能是 lowering 中 local hoisting 和 return 处理不完整。

### 9.2 实施任务

#### F1. 加强 local hoisting

对每个输出 function：

- 扫描所有 local declaration。
- 所有 local declaration 必须位于首个非 local statement 之前。
- 如果 lowering 插入 temp local，必须加入函数顶部 local 区。
- 原本带 initializer 的 local：

输入：

```jass
local integer i = Foo()
```

输出：

```jass
local integer i
...
set i = Foo()
```

initializer 的 `set` 必须保留在原执行位置，而不是无脑放到顶部。

#### F2. 处理 Zinc block 内 local

Zinc 允许块内声明，但 JASS 不允许。所有 Zinc block local 都要提升到 function 顶部，同时赋值保留在原位置。

#### F3. 处理 generated lambda locals

`vjlambda__N` 函数也必须经过同样 local hoisting。

#### F4. expected-endfunction 检查

syntax-lite 中加入：

```text
function/endfunction balanced
no nested function declaration
no endfunction missing
no stray endif/endloop at top level
```

#### F5. return mismatch 初筛

建立简单函数签名表：

```text
function name returns TYPE
```

检查：

- `returns nothing` 中不能有 `return expr`。
- 非 nothing 函数不应出现空 `return`。
- function interface wrapper 的 evaluate 必须 return result。
- execute wrapper returns nothing。

不要用默认值偷偷修复用户代码；先定位 generated wrapper 和 lowering 错误。

### 9.3 Fixtures

新增：

```text
tests/fixtures/phase7_local_order_zinc_block.in.j
tests/fixtures/phase7_local_order_generated_lambda.in.j
tests/fixtures/phase7_local_initializer_position.in.j
tests/fixtures/phase7_return_wrapper_evaluate.in.j
```

### 9.4 验收

- syntax-lite local-order check 为 0。
- PJASS `localOrder` 为 0。
- PJASS `expectedEndfunction` 显著下降。
- PJASS `returnMismatch` 显著下降，目标为 0。

---

## 10. Workstream G：第一轮 undefined function / variable 修复

### 10.1 背景

Phase 6 中：

```text
undefinedVariable: 1973
undefinedFunction: 499
```

这些大概率很多是级联错误。阶段 7 不要一开始就修它们。必须先完成 Workstream B/C/D/E/F。

### 10.2 实施顺序

1. 跑 PJASS。
2. 重新生成 error summary。
3. 只处理剩余 undefined 中排名最高的 20 个 unique symbol。
4. 每个 symbol 先判断：
   - 是否因为 declaration 被误删？
   - 是否因为 generated name 不一致？
   - 是否因为 public/private rename 不一致？
   - 是否因为 struct field/method lowering 不完整？
   - 是否因为 function interface wrapper 没生成？
   - 是否因为 JassHelper 特性尚未支持？

### 10.3 不要做

- 不要通过随意声明 dummy global/function 来压 PJASS 错误。
- 不要为了 PJASS 通过而生成空函数替代真实逻辑。
- 不要把 undefined 全部当作 common/blizzard 问题。

### 10.4 验收

- undefinedVariable / undefinedFunction 相比 phase 6 显著下降。
- 每个剩余 top undefined 都在 `docs/phase7_status.md` 有分类。

---

## 11. Workstream H：syntax-lite 和 validation report 强化

### 11.1 新增 syntax-lite 检查项

在 `--check-output-syntax-lite` 中增加：

```text
1. no residual source forms
2. exactly one globals block
3. no multidimensional declarations remain
4. no local after executable statement
5. function/endfunction balanced
6. if/then/endif rough balance
7. loop/endloop rough balance
8. no nested function declarations
9. no generated-name duplicate functions/globals
10. no block comment content leaked into declarations
11. no semicolons in final JASS statements unless inside string/rawcode/comment
```

### 11.2 validation report 增强

新增：

```json
{
  "syntaxLite": {
    "ok": true,
    "issues": [],
    "multiDimensionalResiduals": [],
    "localOrderIssues": [],
    "functionBalanceIssues": [],
    "duplicateGeneratedNames": []
  },
  "arrayLowering": {
    "multiDimDeclarations": 0,
    "multiDimAccessesLowered": 0,
    "residualMultiDimPatterns": 0
  },
  "generatedNames": {
    "duplicateFunctions": 0,
    "duplicateGlobals": 0,
    "unresolvedGeneratedFunctionRefs": 0,
    "unresolvedGeneratedGlobalRefs": 0
  }
}
```

### 11.3 验收

- `--check-output-syntax-lite` 能在不跑 PJASS 时提前抓住多维数组、local-order、函数平衡等问题。
- report 数据能支撑下一轮修复。

---

## 12. Workstream I：JassHelper 结构级对比维持

阶段 7 不追求 byte-for-byte，但要继续记录结构级差异。

### 12.1 必须保留指标

```text
generatedFunctions
referenceFunctions
functionDelta
generatedLines
referenceLines
lineDelta
generatedGlobals
referenceGlobals
```

### 12.2 新增可选指标

```text
structSupportFunctionCount
functionInterfaceWrapperCount
lambdaFunctionCount
nativeCount
libraryInitializerCount
structInitializerCount
```

### 12.3 注意

不要为了让函数数接近 JassHelper 而生成无意义 dummy 函数。对比只是辅助判断。

---

## 13. 开发顺序建议

Codex 必须按这个顺序推进，避免修级联错误浪费时间：

```text
1. baseline reproduce
2. PJASS log triage enhancement
3. common/blizzard duplicate declaration filtering
4. block comment sanitizer
5. multi-dimensional array flattening
6. struct generated-name consistency
7. duplicate generated names registry
8. local-order / return mismatch cleanup
9. rerun PJASS and classify remaining undefined
10. docs/phase7_status.md + README update
```

每完成一个 workstream，都跑：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\input.j -o build\input.phase7.out.j --emit-stats build\input.phase7.codegen.stats.json --emit-validation-report build\input.phase7.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

每完成 B/C/D/E/F 后，额外跑 PJASS：

```bat
build\vjassc.exe samples\input.j -o build\input.phase7.pjass.out.j --emit-stats build\input.phase7.pjass.stats.json --emit-validation-report build\input.phase7.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

---

## 14. docs/phase7_status.md 要求

阶段 7 完成后新增：

```text
docs/phase7_status.md
```

内容必须包含：

1. Implemented 列表。
2. Verification Commands。
3. Output Stats。
4. Syntax-Lite Result。
5. Init Validation。
6. PJASS Result。
7. PJASS error summary before/after。
8. First remaining PJASS blockers，如果还有。
9. JassHelper Comparison。
10. Performance Baseline。
11. Known Limitations。
12. Next Phase Suggestion。

### 14.1 PJASS before/after 表格

必须包含类似：

```text
| Error Class | Phase 6 | Phase 7 |
| --- | ---: | ---: |
| duplicateDeclaration | 69 | 0 |
| syntaxError | 2768 | ... |
| undefinedVariable | 1973 | ... |
| returnMismatch | 63 | ... |
| undefinedFunction | 499 | ... |
| expectedEndfunction | 153 | ... |
| localOrder | 9 | 0 |
```

### 14.2 如果 PJASS 通过

写明：

```text
PJASS ok: true
exitCode: 0
```

并建议下一阶段进入 runtime validation / behavior comparison / performance。

### 14.3 如果 PJASS 未通过

写明：

```text
PJASS ok: false
remaining blockers:
  1. ...
  2. ...
```

不要含糊写“还有一些错误”。

---

## 15. README 更新要求

README 中新增 Phase 7 摘要：

```text
Phase 7 reduces PJASS blockers by fixing environment duplicate declarations, block-comment filtering, multidimensional array flattening, generated-name consistency, and local/return legality checks. The compiler still is/is not a full JassHelper replacement depending on PJASS result.
```

并更新 CLI 示例，保留：

```bat
build\vjassc.exe samples\input.j -o build\input.phase7.out.j --emit-validation-report build\input.phase7.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

PJASS 示例也保留。

---

## 16. 阶段 7 完成后的项目状态判断

如果阶段 7 达到 PJASS 通过：

```text
整体完成度可提升到 85%~88%
下一阶段应做 runtime validation、behavior diff、remaining unsupported advanced vJass features、performance optimization。
```

如果阶段 7 未 PJASS 通过但错误数大幅下降：

```text
整体完成度约 82%~84%
下一阶段继续 PJASS blocker cleanup。
```

如果阶段 7 只增强 report，但 PJASS 错误数量没有明显下降：

```text
整体完成度仍约 80%
阶段 7 视为未完成核心目标。
```

---

## 17. Codex 注意事项

- 不要伪造 PJASS pass。
- 不要删除 PJASS stdout/stderr。
- 不要用 dummy 函数或 dummy globals 掩盖错误。
- 不要让 `--allow-unsupported` 影响 full codegen 安全性。
- 不要破坏 phase1~phase6 fixtures。
- 不要让 syntax-lite 变成摆设；发现问题必须 fail。
- 每一个 PJASS blocker 修复都要有最小 fixture。
- 每次修复都要更新 validation report 指标。
- 最终必须提交 `docs/phase7_status.md`。

---

## 18. 最终交付清单

阶段 7 完成时应提交：

```text
src/... new or modified files
CMakeLists.txt if new cpp files are added
tests/fixtures/phase7_*.in.j
tests/fixtures/phase7_*.expected.j
docs/phase7_status.md
README.md
```

最终命令必须可运行：

```bat
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\input.j -o build\input.phase7.out.j --emit-stats build\input.phase7.codegen.stats.json --emit-validation-report build\input.phase7.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
build\vjassc.exe samples\input.j -o build\input.phase7.pjass.out.j --emit-stats build\input.phase7.pjass.stats.json --emit-validation-report build\input.phase7.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```
