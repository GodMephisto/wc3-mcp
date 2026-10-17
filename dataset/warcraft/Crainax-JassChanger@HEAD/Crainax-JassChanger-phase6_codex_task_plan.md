---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase6_codex_task_plan.md"
ref: HEAD
---

# JassChanger 阶段 6 实施计划：PJASS 验证、真实输出修复与初始化完整性

> 交付对象：Codex
> 项目：`Crainax/JassChanger`
> 当前阶段：Phase 5 已完成
> 阶段 6 目标：把真实 `samples/input.j` 从“可生成纯 JASS 候选文件”推进到“可被 PJASS 验证、初始化顺序可信、输出结构可对比”的阶段。

---

## 0. 当前项目状态

Phase 5 已经完成一个非常关键的里程碑：真实 `samples/input.j` 已经可以 full codegen，并写出完整的纯 JASS 候选文件。

当前已知基线：

```json
{
  "functionInterfaces": 19,
  "functionInterfaceTargets": 61,
  "functionInterfaceCalls": 50,
  "functionObjectCalls": 33,
  "lambdas": 838,
  "lambdasLowered": 838,
  "lambdasGeneratedFunctions": 838,
  "lambdasCapturing": 0,
  "lambdasUnknownContext": 0,
  "lambdasRejected": 0,
  "diagnostics": {
    "errors": 0,
    "warnings": 3,
    "unsupported": 0
  },
  "output": {
    "bytes": 3777710,
    "lines": 91995,
    "functions": 6124,
    "globalsBlocks": 1
  },
  "timingMs": {
    "total": 11672
  }
}
```

Phase 5 输出已通过 `--check-output-syntax-lite`，并且输出中不再残留以下源级语法：

```text
//! zinc
//! endzinc
struct/endstruct
method/endmethod
module/endmodule
implement
static if
function interface
function(...)
```

但是当前输出仍然只是 **plain-JASS candidate**，还没有经过：

```text
1. PJASS 验证
2. Warcraft III 实机加载验证
3. main/config/init 语义顺序确认
4. 与 samples/output_jasshelper.j 的结构级对比
5. 性能优化
```

阶段 6 不应急着继续实现 interface/delegate/operator/stub/super。阶段 6 的核心是：**让当前已经能生成的真实输出变得可信。**

---

## 1. 阶段 6 总目标

阶段 6 的最终目标是让下面命令成为主验收命令：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure

build/vjassc samples/input.j \
  -o build/input.phase6.out.j \
  --emit-stats build/input.phase6.codegen.stats.json \
  --check-output-syntax-lite
```

如果项目中存在可用的 PJASS、`common.j`、`blizzard.j`，还要支持：

```bash
build/vjassc samples/input.j \
  -o build/input.phase6.out.j \
  --emit-stats build/input.phase6.codegen.stats.json \
  --check-output-syntax-lite \
  --validate-pjass \
  --pjass path/to/pjass.exe \
  --common path/to/common.j \
  --blizzard path/to/blizzard.j \
  --emit-validation-report build/input.phase6.validation.json
```

如果当前运行环境没有 PJASS，也必须提供：

```text
1. 可复现的 PJASS 调用脚本
2. docs/phase6_status.md 中记录 PJASS 未执行的原因
3. validation report 的占位字段
4. 用户可在 Windows 本地直接执行的命令
```

---

## 2. 阶段 6 非目标

阶段 6 **不要主动实现**下面功能，除非 PJASS 报错明确证明它们已经残留到输出里，且必须修复才能通过验证：

```text
- interface dispatch
- delegate
- operator overloads
- stub/super
- capturing closures
- byte-for-byte 匹配 JassHelper
- 完整性能优化到 1~2 秒
- Warcraft III 实机自动化运行
```

这些应放到后续阶段。

阶段 6 只做：

```text
- 输出语法可信
- PJASS 验证闭环
- 初始化顺序可信
- 输出结构可观测
- 修复真实 input 输出中的阻断性问题
```

---

## 3. 开发环境准备

### 3.1 基础构建环境

Windows 推荐：

```bat
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

Linux / WSL 推荐：

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

### 3.2 PJASS 准备

检查项目内是否已有 PJASS 或 JassHelper 包：

```bash
find . -iname "pjass*"
find . -iname "common.j"
find . -iname "blizzard.j"
```

Windows PowerShell：

```powershell
Get-ChildItem -Recurse -Filter "pjass*"
Get-ChildItem -Recurse -Filter "common.j"
Get-ChildItem -Recurse -Filter "blizzard.j"
```

如果项目内没有，则在 `docs/phase6_status.md` 里记录：

```text
PJASS not bundled / not found in this checkout.
Manual validation command documented but not executed.
```

不要因为找不到 PJASS 就阻塞所有阶段 6 工作。Codex 仍然要完成 CLI、脚本、报告结构和 syntax-lite 强化。

---

## 4. 阶段 6 任务拆分

---

# Task 1：新增 PJASS 验证入口

## 目标

让 `vjassc` 支持可选的外部 PJASS 验证。

新增 CLI 参数：

```text
--validate-pjass
--pjass <path>
--common <path>
--blizzard <path>
--emit-validation-report <path>
```

建议同时支持：

```text
--pjass-timeout-ms <number>
```

默认超时可设为：

```text
30000 ms
```

## 行为规则

### 情况 A：用户没有传 `--validate-pjass`

正常 codegen，不调用 PJASS。

### 情况 B：用户传了 `--validate-pjass` 但没传路径

尝试自动查找：

```text
./jasshelper/pjass.exe
./jasshelper/pjass/pjass.exe
./tools/pjass.exe
./pjass.exe
```

`common.j` 和 `blizzard.j` 尝试查找：

```text
./jasshelper/common.j
./jasshelper/blizzard.j
./jasshelper/war3/common.j
./jasshelper/war3/blizzard.j
./common.j
./blizzard.j
```

如果找不到，报清晰错误：

```text
PJASS validation requested, but pjass/common.j/blizzard.j was not found.
Pass --pjass, --common, and --blizzard explicitly.
```

### 情况 C：用户传了完整路径

执行类似：

```bash
pjass common.j blizzard.j build/input.phase6.out.j
```

注意 Windows 下 `.exe` 路径可能包含空格，必须正确 quote。

## 实现建议

新增文件：

```text
src/validation/PjassRunner.h
src/validation/PjassRunner.cpp
src/validation/ValidationReport.h
```

CMake 加入：

```cmake
src/validation/PjassRunner.cpp
```

`PjassRunner` 建议结构：

```cpp
struct PjassOptions {
    std::filesystem::path pjassPath;
    std::filesystem::path commonPath;
    std::filesystem::path blizzardPath;
    std::filesystem::path scriptPath;
    long long timeoutMs = 30000;
};

struct PjassResult {
    bool ran = false;
    bool ok = false;
    int exitCode = -1;
    std::string commandLine;
    std::string stdoutText;
    std::string stderrText;
    long long elapsedMs = 0;
};
```

Windows 调外部进程可先用 `std::system` 做最小实现，但建议尽量捕获 stdout/stderr。若捕获复杂，可先将输出重定向到临时文件：

```bat
pjass.exe common.j blizzard.j input.j > pjass.stdout.txt 2> pjass.stderr.txt
```

Linux/WSL 同理。

## 验收

```bash
build/vjassc samples/input.j \
  -o build/input.phase6.out.j \
  --validate-pjass \
  --pjass path/to/pjass.exe \
  --common path/to/common.j \
  --blizzard path/to/blizzard.j \
  --emit-validation-report build/input.phase6.validation.json
```

若 PJASS 可用，validation report 必须记录：

```json
{
  "pjass": {
    "ran": true,
    "ok": true,
    "exitCode": 0,
    "elapsedMs": 1234
  }
}
```

若失败，必须记录错误输出，而不是只返回 exit code。

---

# Task 2：新增 validation report

## 目标

生成统一的验证报告，方便后续阶段定位问题。

新增 CLI：

```text
--emit-validation-report <path>
```

报告结构建议：

```json
{
  "input": "samples/input.j",
  "output": "build/input.phase6.out.j",
  "syntaxLite": {
    "ran": true,
    "ok": true,
    "residualSourceForms": [],
    "globalsBlocks": 1,
    "functions": 6124,
    "natives": 551,
    "lines": 91995,
    "bytes": 3777710
  },
  "pjass": {
    "ran": true,
    "ok": false,
    "exitCode": 1,
    "commandLine": "...",
    "stdoutPreview": "...",
    "stderrPreview": "...",
    "errorCountGuess": 42
  },
  "comparison": {
    "jasshelperReference": "samples/output_jasshelper.j",
    "referenceFound": true,
    "generatedFunctions": 6124,
    "referenceFunctions": 6864,
    "generatedGlobalsBlocks": 1,
    "referenceGlobalsBlocks": 1
  }
}
```

不要在 JSON 里塞完整 PJASS 大输出。完整日志写到：

```text
build/input.phase6.pjass.stdout.txt
build/input.phase6.pjass.stderr.txt
```

JSON 只保存 preview 和统计。

---

# Task 3：强化 `--check-output-syntax-lite`

## 当前状态

Phase 5 的 syntax-lite 已经能检查源级语法残留，并确认输出中不再出现：

```text
//! zinc
//! endzinc
struct/endstruct
method/endmethod
module/endmodule
implement
static if
function interface
function(...)
```

## 阶段 6 增强目标

新增检查项：

```text
1. globals 块数量必须为 1
2. 不允许 endglobals 缺失
3. 不允许 function 嵌套 function
4. 不允许 endfunction 数量不匹配
5. 不允许 local 出现在非 local 语句之后
6. 不允许 Zinc 残留符号：
   - {
   - }
   - ;
   - ->
   - =>
7. 不允许 call 用在有返回值的赋值位置
8. 不允许 set 调用函数式语法错误
9. 不允许 return value 出现在 returns nothing 函数中
10. 不允许 return 空值出现在非 nothing 函数中
11. 不允许 private/public/static method 关键字残留
12. 不允许 thistype 残留
13. 不允许 `.execute(` / `.evaluate(` / `.name` 残留
14. 不允许匿名函数残留 `function(` / `function (`
15. 不允许 `//!` 指令残留，除普通注释说明外
```

注意：这些是 lightweight 检查，不需要完整 parser。实现时要跳过：

```text
- 字符串
- rawcode 字面量，如 'hfoo'
- 行注释
```

## 输出

如果 syntax-lite 失败，必须：

```text
1. 输出失败项名称
2. 输出前 20 个位置
3. 返回非零 exit code
4. 写入 validation report
```

---

# Task 4：PJASS 错误分类器

## 目标

PJASS 一旦失败，需要快速知道是哪一类问题，而不是只看到一长串错误。

实现一个简单分类器：

```text
Undefined function
Undefined variable
Expected endfunction
Expected globals
Invalid type
Duplicate declaration
Return type mismatch
Local declaration after statement
Syntax error around token
Native/type/global ordering issue
```

如果 PJASS 输出格式无法稳定解析，至少按关键词猜测：

```cpp
if contains("Undeclared") or contains("not declared")
if contains("Expected")
if contains("local")
if contains("return")
...
```

报告示例：

```json
"pjassErrorSummary": {
  "undefinedFunction": 12,
  "undefinedVariable": 5,
  "localOrder": 2,
  "returnMismatch": 1,
  "other": 9
}
```

## 用途

后续修复顺序按数量排序：

```text
1. local 位置错误
2. 未定义函数/变量
3. 函数顺序或 wrapper 缺失
4. return 类型错误
5. JASS 残留语法
```

---

# Task 5：main/config/init 注入完整性验证

## 背景

JassHelper 替代器最终必须保证初始化顺序正确。当前已有：

```text
- library initializer helper
- struct onInit
- function interface init helper
- main injection
```

但阶段 5 尚未系统验证这些 helper 是否都被调用、顺序是否合理。

## 目标

新增 init validation pass，检查输出中：

```text
1. 是否存在 function main
2. 是否存在 function config
3. 是否存在 vjassc__init_libraries
4. 是否存在 vjassc__init_function_interfaces
5. 是否存在 struct onInit helper
6. main 中是否调用必要 init helper
7. library initializer 顺序是否符合 dependency graph
8. struct onInit 是否在 library initializer 前执行
9. function interface target registry 是否在可能调用前初始化
```

## 推荐顺序

建议目标顺序：

```jass
function main takes nothing returns nothing
    call SetCameraBounds(...)
    call SetDayNightModels(...)
    call NewSoundEnvironment(...)
    call InitSounds()
    call CreateRegions()
    call InitBlizzard()

    call vjassc__init_structs()
    call vjassc__init_function_interfaces()
    call vjassc__init_libraries()

    call InitGlobals()
    call InitCustomTriggers()
    call RunInitializationTriggers()
endfunction
```

具体要兼容当前 `input.j` 原始 main 的结构。不要硬重写整个 main，优先在合适位置注入。

如果原始 main 没有 `InitBlizzard`，则在 main 末尾或首个非 local 语句后插入。

## 验收

`validation report` 中新增：

```json
"init": {
  "hasMain": true,
  "hasConfig": true,
  "hasStructInit": true,
  "hasFunctionInterfaceInit": true,
  "hasLibraryInit": true,
  "mainCallsStructInit": true,
  "mainCallsFunctionInterfaceInit": true,
  "mainCallsLibraryInit": true,
  "libraryInitializerCount": 331
}
```

---

# Task 6：输出结构级对比报告

## 目标

不要 byte-to-byte 比较 JassHelper 输出，而是先做结构级对比。

输入：

```text
generated: build/input.phase6.out.j
reference: samples/output_jasshelper.j
```

新增 CLI 可选：

```text
--compare-jasshelper samples/output_jasshelper.j
```

或新增工具：

```bash
build/jass_metrics build/input.phase6.out.j samples/output_jasshelper.j
```

如果不想新增可执行文件，可以在 `vjassc` 内实现：

```bash
build/vjassc samples/input.j \
  -o build/input.phase6.out.j \
  --compare-jasshelper samples/output_jasshelper.j \
  --emit-validation-report build/input.phase6.validation.json
```

## 结构指标

至少统计：

```text
1. bytes
2. lines
3. globalsBlocks
4. global declarations count
5. native declarations count
6. type declarations count
7. function count
8. generated lambda function count
9. struct support function count
10. function interface wrapper count
11. init helper names
12. main/config presence
13. source-form residue count
```

参考输出中常见前缀：

```text
s__
sc__
sa__
st__
jasshelper__
```

本项目当前前缀：

```text
si__
s___
vjlambda__
vjassc__
```

不要要求前缀一致，只做存在性和数量趋势判断。

## 报告示例

```json
"comparison": {
  "generated": {
    "bytes": 3777710,
    "lines": 91995,
    "functions": 6124,
    "globalsBlocks": 1
  },
  "jasshelper": {
    "bytes": 5920000,
    "lines": 116000,
    "functions": 6864,
    "globalsBlocks": 1
  },
  "delta": {
    "functions": -740,
    "lines": -24005
  },
  "notes": [
    "Generated output has fewer functions than JassHelper; verify wrapper generation and init helpers."
  ]
}
```

---

# Task 7：修复 PJASS 阻断错误

## 目标

一旦 Task 1~4 能跑 PJASS，就开始修复真实错误。

修复优先级：

### 7.1 local declaration order

JASS 要求 local 声明在函数顶部。Phase 5 已做 hoisting，但 PJASS 可能仍发现：

```text
- lambda 生成函数里的 local 未 hoist
- method lowering 后 extra local 插入位置不对
- function interface wrapper 里的 temp local 位置不对
- Zinc lowered body 中局部变量顺序不对
```

要求：

```text
所有 generated function 都必须保证 local 声明在首个可执行语句之前。
```

### 7.2 undefined symbol

常见来源：

```text
- function interface target 未注册
- static method function reference 未生成 wrapper
- module 展开后名称改写不完整
- private/public 名称映射遗漏
- struct field/method 访问改写遗漏
- lambda 函数生成顺序错误
```

要求：

```text
PJASS 不应出现 undefined function / undefined variable。
```

### 7.3 return mismatch

检查：

```text
returns nothing 函数中不能 return value
returns integer/real/string/boolean/handle/code 函数必须合理 return
function interface evaluate wrapper 的返回类型必须一致
execute wrapper 必须 returns nothing
```

### 7.4 native/type/global ordering

如果 PJASS 报 native/type/global 顺序问题，根据 PJASS 反馈调整输出布局。

目标布局建议：

```text
type declarations
globals
    ...
endglobals
native declarations
function declarations
```

或 PJASS 要求的等价布局。以 PJASS 结果为准。

### 7.5 duplicate declarations

允许重复 native 被去重。其他重复声明必须定位来源：

```text
- library_once duplicate
- module duplicate expansion
- lambda generated name collision
- function interface wrapper duplicate
- struct support duplicate
```

---

# Task 8：输出产物健康检查

## 目标

即使 PJASS 通过，也要做额外健康检查。

新增或扩展 `--check-output-syntax-lite`：

```text
1. globalsBlocks == 1
2. no source forms
3. no anonymous function source
4. no Zinc syntax
5. no vJASS declarations
6. no duplicate function names
7. no duplicate global names
8. no duplicate native names
9. all generated helper functions are called or intentionally unused
10. all function interface target ids have registered functions
```

不要把所有未调用函数都当错误，因为地图里很多 trigger 函数通过字符串或事件调用。

但下面 helper 应该必须被调用：

```text
vjassc__init_libraries
vjassc__init_function_interfaces
vjassc__init_structs 或实际 struct init helper
```

---

# Task 9：性能观测，不做大规模优化

## 当前问题

Phase 5 full codegen 总耗时约：

```text
11672 ms
```

这个明显高于最终目标：

```text
1~2 秒
```

但阶段 6 优先级是正确性，不是最终性能。

## 阶段 6 性能目标

最低目标：

```text
不比 Phase 5 明显退化。
```

建议目标：

```text
full codegen <= 10 秒
```

拉伸目标：

```text
full codegen <= 7 秒
```

## 增加分段计时

当前 stats 已有：

```text
read
preprocess
staticIf
lex
parse
moduleExpand
total
```

阶段 6 增加：

```text
codegenCollect
codegenLowering
codegenWrite
syntaxLite
pjass
comparison
```

示例：

```json
"timingMs": {
  "read": 40,
  "preprocess": 80,
  "staticIf": 20,
  "lex": 170,
  "parse": 130,
  "moduleExpand": 10,
  "codegenCollect": 500,
  "codegenLowering": 7000,
  "codegenWrite": 300,
  "syntaxLite": 120,
  "pjass": 1500,
  "total": 9800
}
```

## 优化建议

只做低风险优化：

```text
1. 避免每行重复构造大量 regex
2. 预编译常用 regex，或改成手写扫描
3. function/interface/struct 查找使用 unordered_map
4. 避免对整个大文件反复 regex_replace
5. 避免每个 lambda 都全局扫描函数表
6. 避免 O(functions * lines) 的重复 rewrite
```

不要为了性能重写 parser 或 lowering 主架构。

---

# Task 10：工程命名债处理，可选但建议做

## 问题

当前 `Phase1Codegen` 已经承担 phase 1~5 的全部 lowering/codegen，文件名和类名都不准确。

阶段 6 可以做一次**不改语义**的重命名：

```text
src/codegen/Phase1Codegen.h   -> src/codegen/JassCodegen.h
src/codegen/Phase1Codegen.cpp -> src/codegen/JassCodegen.cpp
class Phase1Codegen          -> class JassCodegen
```

CMake 同步修改。

如果担心影响太大，可以保留兼容别名：

```cpp
using Phase1Codegen = JassCodegen;
```

## 进一步拆分，谨慎做

如果 Codex 能安全拆分，可以拆成：

```text
src/codegen/JassCodegen.cpp
src/codegen/StructLowering.cpp
src/codegen/FunctionInterfaceLowering.cpp
src/codegen/LambdaLowering.cpp
src/codegen/OutputSyntaxLite.cpp
```

但阶段 6 不强制。不要因为重构引入行为变化。

---

# Task 11：新增测试

## 11.1 Golden fixtures

新增：

```text
tests/fixtures/phase6_local_hoist_lambda.in.j
tests/fixtures/phase6_local_hoist_lambda.expected.j

tests/fixtures/phase6_function_interface_nested_evaluate.in.j
tests/fixtures/phase6_function_interface_nested_evaluate.expected.j

tests/fixtures/phase6_static_method_callback_order.in.j
tests/fixtures/phase6_static_method_callback_order.expected.j

tests/fixtures/phase6_init_injection_main_has_initblizzard.in.j
tests/fixtures/phase6_init_injection_main_has_initblizzard.expected.j

tests/fixtures/phase6_init_injection_main_no_initblizzard.in.j
tests/fixtures/phase6_init_injection_main_no_initblizzard.expected.j

tests/fixtures/phase6_no_source_forms_after_codegen.in.j
tests/fixtures/phase6_no_source_forms_after_codegen.expected.j
```

## 11.2 Negative fixtures

新增：

```text
tests/fixtures/phase6_negative_capturing_lambda.in.j
tests/fixtures/phase6_negative_unknown_function_interface_target.in.j
tests/fixtures/phase6_negative_duplicate_generated_name.in.j
tests/fixtures/phase6_negative_bad_local_order.in.j
```

## 11.3 Optional PJASS tests

PJASS 不一定在 CI 可用，因此不要让缺少 PJASS 导致默认 ctest 失败。

可新增：

```text
ctest -L pjass
```

仅当环境变量存在时启用：

```text
VJASSC_PJASS
VJASSC_COMMON_J
VJASSC_BLIZZARD_J
```

CMake 可判断：

```cmake
if(DEFINED ENV{VJASSC_PJASS})
    add_test(NAME pjass_input COMMAND ...)
    set_tests_properties(pjass_input PROPERTIES LABELS pjass)
endif()
```

---

# Task 12：更新文档

新增：

```text
docs/phase6_status.md
```

必须包含：

```text
1. Phase 6 implemented list
2. input.j full codegen command
3. output stats
4. syntax-lite result
5. PJASS result
6. if PJASS unavailable: why unavailable and exact manual command
7. comparison with output_jasshelper.j
8. known limitations
9. performance baseline
10. next phase suggestion
```

README 更新：

```text
Phase 6 adds PJASS validation hooks, stronger output syntax-lite checks,
initialization integrity validation, and structural comparison against
the legacy JassHelper output.
```

如果 PJASS 通过，不要夸大为“完全替代 JassHelper”。只能说：

```text
Generated output passes PJASS for the current sample input.
Runtime validation and behavior matching remain future work.
```

---

## 5. 阶段 6 验收标准

### 必须完成

```text
1. cmake build 成功
2. ctest 成功
3. samples/input.j full codegen 成功
4. --check-output-syntax-lite 成功
5. build/input.phase6.out.j 被实际写出
6. 输出为单 globals 块
7. 输出没有阶段 1~5 已处理过的源级语法残留
8. docs/phase6_status.md 完整更新
9. validation report 能生成
```

### 有 PJASS 时必须完成

```text
1. --validate-pjass 能执行 PJASS
2. PJASS stdout/stderr 被保存
3. validation report 记录 PJASS 结果
4. 如果 PJASS 失败，必须分类错误并列出下一步修复计划
5. 如果 PJASS 通过，记录通过命令和版本/路径
```

### 无 PJASS 时必须完成

```text
1. 自动检测失败信息清晰
2. docs/phase6_status.md 写明 PJASS 未执行
3. 提供 Windows 本地可执行命令
4. validation report 记录 pjass.ran=false
```

---

## 6. 阶段 6 成功后的项目状态预期

如果阶段 6 完成并且 PJASS 通过，整体项目完成度可从当前约 75% 提升到：

```text
约 82%~85%
```

如果阶段 6 只能完成 syntax-lite 和 validation report，但 PJASS 仍未通过，整体完成度约：

```text
约 78%~80%
```

剩余阶段建议：

```text
阶段 7：interface/delegate/operator/stub/super 兼容性补洞，按真实输出/PJASS/差异报告决定优先级
阶段 8：Warcraft III 实机加载验证、行为测试、JassHelper 结构级差异修正
阶段 9：性能优化到 1~2 秒、发布包装、CLI 稳定化
```

如果真实 `input.j` 不使用 interface/delegate/operator/stub/super，阶段 7 可以缩短，直接进入实机和性能阶段。

---

## 7. 给 Codex 的重要注意事项

```text
1. 不要删除 samples/input.j 或 samples/output_jasshelper.j。
2. 不要把 --allow-unsupported 变成 partial codegen。
3. 不要因为 PJASS 不存在就跳过 validation report 实现。
4. 不要做 byte-to-byte JassHelper 匹配。
5. 不要在阶段 6 主动实现大量新语法。
6. 不要让捕获 lambda 静默通过。
7. 不要让 unknown function interface target 静默变成 0。
8. 不要为了性能牺牲输出正确性。
9. 所有新增生成名必须 deterministic。
10. 如果修复 PJASS 错误需要改 lowering，必须新增 fixture。
```

---

## 8. 推荐执行顺序

```text
Step 1: 建立 phase6 branch
Step 2: 运行 phase5 baseline，确认现状
Step 3: 实现 validation report
Step 4: 实现/接入 PJASS runner
Step 5: 强化 syntax-lite
Step 6: 加 init validation
Step 7: 加结构级 output_jasshelper 对比
Step 8: 跑 input.j full codegen + syntax-lite
Step 9: 跑 PJASS，如可用
Step 10: 按 PJASS 错误分类修复
Step 11: 增加 fixtures
Step 12: 更新 docs/phase6_status.md 和 README
Step 13: 最后跑完整验收命令
```

最终交付时请在回复或 commit message 中明确：

```text
- input.j 是否能 full codegen
- syntax-lite 是否通过
- PJASS 是否执行
- PJASS 是否通过
- 若 PJASS 失败，错误分类统计是什么
- 输出行数/函数数/globals 数
- 总耗时
- 下一阶段建议
```
