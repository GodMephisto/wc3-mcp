---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "phase14_codex_task_plan.md"
ref: HEAD
---

# Phase 14 Codex 实施计划：War3Lib 接入 vjassc、ALPHA 实机验证与第二轮性能准备

## 0. 阶段定位

本阶段不是继续单独在 `JassChanger/samples/input.j` 上做 PJASS 收敛。Phase 13 已经证明：真实 `samples/input.j` 的 vjassc 输出在显式 validation-only external symbol `InitTrig_japi` 下可以通过 PJASS。

Phase 14 的目标是把 vjassc 接入真实地图开发链路：

```text
Xlimon 地图项目
  -> War3Lib / TaskStartMap
  -> Lua/compile/Compiler.lua
  -> Wave 第一次预处理
  -> InjectCodeBlock
  -> Wave 第二次预处理
  -> CompileLua
  -> LocalDzApi map config replacement
  -> JASS 编译后端
       当前：jasshelper.exe --debug --scriptonly common.j blizzard.j input.j output.j
       新增：vjassc.exe input.j -o output.j
  -> Output/output.j
  -> 地图 war3map.j / 启动测试
```

本阶段允许在 **ALPHA / 内测版本** 下尝试用 vjassc 替代 jasshelper，并允许人工进游戏测试功能是否正常。正式版本默认仍必须走 jasshelper，除非显式配置。

---

## 1. 当前项目事实

### 1.1 JassChanger 当前状态

当前 Phase 13 状态：

```text
syntax-lite: pass
init validation: pass
duplicate function/global/native: 0
PJASS: pass, with --pjass-allow-external InitTrig_japi
generated output: 不包含 InitTrig_japi validation stub
full validation total: about 57.8s
```

Phase 13 的 PJASS pass 是一个编译器验证里程碑，但还不是最终 JassHelper 替代里程碑。还必须做：

```text
- Warcraft III / 地图加载验证
- ALPHA 实机测试
- 和 jasshelper 输出做结构级/运行行为对比
- 性能分段测量
- 回退机制
```

### 1.2 War3Lib 当前编译流程

`War3Lib/Lua/compile/Compiler.lua` 当前最终 JASS 编译步骤大致是：

```lua
fileUtils.copyFile(path.CompileStep4, path.jasshelper .. "/input.j")
lfs.chdir(path.jasshelper)
os.execute("jasshelper.exe --debug --scriptonly common.j blizzard.j input.j output.j")
fileUtils.copyFile(path.jasshelper .. "/output.j", path.CompileStep5)
fileUtils.copyFile(path.CompileStep5, path.CompileResult)
```

`path.CompileStep4` 是 Wave、代码注入、Lua 遍历、DzAPI map config replacement 后的最终 vJass/Zinc 输入。vjassc 必须以 **这个文件** 作为输入，而不是更早阶段文件。

### 1.3 ALPHA / 内测版本

`War3Lib/Lua/path.lua` 中已有：

```lua
path.initAlpha = function()
    path.buildVersion = "内测版本"
    path.setMapName(path.mapName)
end
```

本阶段所有 vjassc 实机替代测试优先限定在 `path.buildVersion == "内测版本"` 或显式 ALPHA 任务入口中进行。

---

## 2. 阶段目标

### 2.1 最低目标

1. War3Lib 支持 JASS 编译后端配置：

```text
jasshelper  // 默认，保持原行为
vjassc      // 使用 JassChanger 输出
both        // 同时跑 jasshelper 和 vjassc，默认仍使用 jasshelper 输出
```

2. 不破坏当前 `TaskStartMap` / `StartCompile` 默认行为。
3. `both` 模式能产出：

```text
Output/5_jasshelper.j
Output/5_vjassc.j
Output/output.j
Output/vjassc.stats.json
Output/vjassc.validation.json
Output/compiler_backend_report.json
```

4. vjassc 输出在 War3Lib 真实链路中继续通过 PJASS：

```text
--validate-pjass
--pjass-allow-external InitTrig_japi
```

5. `InitTrig_japi` validation-only stub 不得写入正式 `Output/output.j` 或地图 `war3map.j`。

### 2.2 良好目标

1. 新增 ALPHA 专用 vjassc 测试入口，例如：

```text
TaskStartMapVjasscAlpha
TaskStartMapCompareAlpha
TaskCompileAlphaWithVjassc
```

实际名称可根据项目现有任务系统调整。

2. ALPHA 模式下可以选择把 `Output/5_vjassc.j` 作为 `Output/output.j`，并启动地图。
3. 生成 jasshelper/vjassc 结构级对比报告。
4. 记录完整编译链路分段耗时。

### 2.3 优秀目标

1. Xlimon ALPHA 地图能使用 vjassc 输出进入游戏。
2. 人工可玩基础流程：加载、初始化、UI、核心触发、DzAPI/YDWE/JAPI 相关功能不崩。
3. 如果 vjassc 输出无法进游戏，能自动保留失败输出和日志，并一键回退 jasshelper。
4. 真实链路性能报告能指出：Wave、Lua、DzAPI、vjassc、PJASS、文件复制各自耗时。

---

## 3. 非目标

Phase 14 暂不要求：

```text
- vjassc 默认替代 jasshelper
- byte-for-byte 匹配 output_jasshelper.j
- 一次性达到 1~2 秒编译目标
- 修复所有运行时行为差异
- 为了通过地图启动而伪造 InitTrig_japi 或其他环境函数
- 把 validation-only stub 写入正式输出
```

---

## 4. War3Lib 代码改造任务

### 4.1 新增编译器后端配置

建议在 `Lua/path.lua` 或新的配置模块中加入：

```lua
path.jassCompiler = path.jassCompiler or "jasshelper"
path.vjassc = path.root .. "/plugins/vjassc/vjassc.exe"
path.vjasscDir = path.root .. "/plugins/vjassc"
```

允许从环境变量覆盖：

```text
WAR3_JASS_COMPILER=jasshelper|vjassc|both
WAR3_VJASSC_EXE=...
WAR3_VJASSC_VALIDATE=0|1
WAR3_VJASSC_STRICT=0|1
```

推荐默认值：

```text
WAR3_JASS_COMPILER=jasshelper
WAR3_VJASSC_VALIDATE=1
WAR3_VJASSC_STRICT=0
```

### 4.2 新增输出路径

在 `path.init` 中增加，不要破坏现有 `CompileStep5`：

```lua
path.CompileStep5JassHelper = path.project .. "/Output/5_jasshelper.j"
path.CompileStep5Vjassc     = path.project .. "/Output/5_vjassc.j"
path.VjasscStats            = path.project .. "/Output/vjassc.stats.json"
path.VjasscValidation       = path.project .. "/Output/vjassc.validation.json"
path.CompilerBackendReport  = path.project .. "/Output/compiler_backend_report.json"
```

保留：

```lua
path.CompileStep5  = path.project .. "/Output/5_jasshelper.j"
path.CompileResult = path.project .. "/Output/output.j"
```

但在 vjassc 后端时，可以把 `path.CompileStep5` 视为最终选中输出路径，或者明确新增 `path.SelectedCompileOutput`。

### 4.3 抽象 JASS 编译阶段

在 `Compiler.lua` 中把 jasshelper 调用拆出来：

```lua
function compile:RunJassHelper(input, output)
    -- 原 jasshelper 行为
end

function compile:RunVjassc(input, output, options)
    -- 调用 JassChanger/vjassc
end

function compile:RunJassCompiler(input, selectedOutput)
    -- 根据 backend 分发
end
```

`StartCompile` 中原来的 jasshelper 块替换为：

```lua
local ok, msg = self:RunJassCompiler(path.CompileStep4, path.CompileStep5)
if not ok then
    -- 保持原有错误日志复制逻辑，并补充 vjassc 日志
    return false
end
```

### 4.4 jasshelper 后端要求

原行为必须保持：

```text
- 输入：path.CompileStep4
- 输出：path.CompileStep5JassHelper / path.CompileStep5
- 失败时复制 logs/compileerrors.txt
- 失败时复制 logs/currentmapscript.j
- 成功后 copy 到 path.CompileResult
```

### 4.5 vjassc 后端命令

建议命令：

```bat
vjassc.exe <CompileStep4> ^
  -o <CompileStep5Vjassc> ^
  --emit-stats <Output/vjassc.stats.json> ^
  --emit-validation-report <Output/vjassc.validation.json> ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass <path.jasshelper>/pjass.exe ^
  --common <path.jasshelper>/common.j ^
  --blizzard <path.jasshelper>/blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

注意：

```text
- --pjass-allow-external InitTrig_japi 仅用于 PJASS validation。
- vjassc 正式输出不得包含 InitTrig_japi stub。
- 如果 vjassc 失败，要保留 Output/4_luaexecute.j、Output/5_vjassc.j、validation json、stdout/stderr。
```

### 4.6 both 后端逻辑

`both` 模式用于对比，不默认替换 jasshelper：

```text
1. 跑 jasshelper -> Output/5_jasshelper.j
2. 跑 vjassc -> Output/5_vjassc.j
3. 生成 compiler_backend_report.json
4. 默认将 jasshelper 输出复制为 Output/output.j
5. 如果显式设置 WAR3_JASS_COMPILER_SELECT=vjassc，则使用 vjassc 输出
```

`both` 模式失败策略：

```text
- jasshelper 失败：保持原逻辑，整体失败。
- vjassc 失败：如果 strict=0，则警告但继续使用 jasshelper 输出。
- vjassc 失败：如果 strict=1，则整体失败。
```

### 4.7 ALPHA 保护策略

仅在以下条件之一满足时允许 vjassc 输出成为最终 `Output/output.j`：

```text
- path.buildVersion == "内测版本"
- 显式环境变量 WAR3_ALLOW_VJASSC_NON_ALPHA=1
```

如果不是 ALPHA 且请求 `vjassc`：

```text
- 打印警告
- 自动回退 jasshelper
- 或在 strict 模式下失败
```

---

## 5. Xlimon ALPHA 实机验证任务

### 5.1 新增 ALPHA 测试入口

根据现有任务系统新增一个入口，名称可调整：

```text
TaskStartMapVjasscAlpha
```

行为：

```text
1. 调用 path.initAlpha()
2. 设置 WAR3_JASS_COMPILER=vjassc 或内部 path.jassCompiler="vjassc"
3. StartCompile()
4. 将 Output/output.j 写入 ALPHA 地图 war3map.j
5. 启动地图
```

再新增一个对比入口：

```text
TaskStartMapCompareAlpha
```

行为：

```text
1. path.initAlpha()
2. WAR3_JASS_COMPILER=both
3. 同时生成 5_jasshelper.j 与 5_vjassc.j
4. 默认使用 jasshelper 输出启动地图
5. 保存 vjassc 输出和报告供对比
```

### 5.2 人工进入游戏测试 checklist

Codex 不需要自动判断游戏内行为，但要把人工 checklist 写入 `Output/vjassc_runtime_checklist.md`：

```text
[ ] 地图能进入加载界面
[ ] 地图能加载完成进入游戏
[ ] 没有脚本初始化崩溃
[ ] main/config/init 执行正常
[ ] struct onInit 执行正常
[ ] library initializer 执行正常
[ ] function interface / lambda callback 没有明显异常
[ ] UI 初始化正常
[ ] 英雄选择/出生流程正常
[ ] 定时器/周期系统正常
[ ] DzAPI 本地替换/测试环境行为正常
[ ] YDHT / YDWE helper 相关逻辑正常
[ ] JAPI / InitTrig_japi 相关逻辑没有缺失
[ ] 存档/房间展示相关测试逻辑没有报错
[ ] 运行 5 分钟无明显异常
```

### 5.3 运行时失败时保留证据

如果 ALPHA vjassc 地图启动失败，保留：

```text
Output/4_luaexecute.j
Output/5_vjassc.j
Output/5_jasshelper.j
Output/output.j
Output/vjassc.validation.json
Output/vjassc.stats.json
Output/compiler_backend_report.json
Output/runtime_notes.md
```

`runtime_notes.md` 至少包含：

```text
- 使用后端
- 是否 PJASS pass
- 是否进入地图
- 卡在哪一步
- 用户手动填写的现象
- 回退 jasshelper 是否正常
```

---

## 6. 结构对比报告

新增 `compiler_backend_report.json`，包含：

```json
{
  "backend": "both",
  "selectedOutput": "jasshelper",
  "buildVersion": "内测版本",
  "jasshelper": {
    "ok": true,
    "output": "Output/5_jasshelper.j",
    "elapsedMs": 0,
    "lines": 0,
    "functions": 0,
    "globalsBlocks": 0,
    "natives": 0
  },
  "vjassc": {
    "ok": true,
    "output": "Output/5_vjassc.j",
    "elapsedMs": 0,
    "pjassOk": true,
    "validation": "Output/vjassc.validation.json",
    "stats": "Output/vjassc.stats.json",
    "lines": 0,
    "functions": 0,
    "globalsBlocks": 0,
    "natives": 0
  },
  "diff": {
    "lineDelta": 0,
    "functionDelta": 0,
    "globalDelta": 0,
    "nativeDelta": 0,
    "hasMainBoth": true,
    "hasConfigBoth": true,
    "hasInitCustomTriggersBoth": true
  }
}
```

不要要求 byte-for-byte 一致。Phase 14 只做结构级对比。

---

## 7. 性能分段测量

### 7.1 War3Lib 链路耗时

在 `Compiler.lua` 中加入简单计时器，输出到 `compiler_backend_report.json`：

```text
syncMirrorMs
localDzApiGenerateMs
copyStep0Ms
wave1Ms
scanBuildStringMs
injectCodeBlockMs
wave2Ms
compileLuaMs
dzApiMapConfigMs
jassCompilerMs
copyBackMs
totalCompileMs
```

### 7.2 vjassc 内部耗时

读取 `vjassc.stats.json` 或 validation report 中已有 timing：

```text
read
preprocess
staticIf
lex
parse
moduleExpand
codegen
syntaxLite
pjass
comparison
total
```

### 7.3 性能目标

Phase 14 不强制达到最终性能目标，但需要建立真实链路 benchmark。

最低：

```text
- 能输出完整 War3Lib compile timing
- 能区分 Wave/Lua/DzAPI/JASS compiler 哪一段慢
```

良好：

```text
- vjassc 后端 codegen-only 模式能跑
- vjassc validation 模式能跑
- both 模式能比较 jasshelper/vjassc 总耗时
```

优秀：

```text
- ALPHA vjassc 后端总耗时不慢于当前 jasshelper 链路太多
- 或者明确证明主要瓶颈在 vjassc emitFunctions/emitStructSupport
```

---

## 8. JassChanger 侧可选改造

如果 War3Lib 接入时发现现有 CLI 不够用，可以在 JassChanger 中补充，但不要阻塞主目标。

### 8.1 fast / validate / full-validation 模式

建议新增：

```text
--mode fast
--mode validate
--mode full-validation
```

语义：

```text
fast:
  只生成输出，不跑 syntax-lite，不跑 PJASS，不跑 comparison，不生成重型 examples。

validate:
  生成输出 + syntax-lite + PJASS。

full-validation:
  生成输出 + syntax-lite + PJASS + comparison + full report + examples。
```

如果暂时不实现，也可以在 War3Lib 里通过是否传递参数来模拟。

### 8.2 输出正式/验证产物分离

确保：

```text
- .env-stubs.j 只用于 validation
- 不进入 -o output.j
- validation report 中记录 stub 路径
```

### 8.3 Exit code 规范

建议：

```text
0 = success
1 = CLI/config error
2 = compile/lowering error
3 = syntax-lite error
4 = PJASS validation failed
5 = output write failed
```

War3Lib 可以根据 exit code 打印更明确错误。

---

## 9. 验收命令示例

### 9.1 JassChanger 独立验证

```bat
build\vjassc.exe samples\input.j ^
  -o build\input.phase14.pjass.out.j ^
  --emit-stats build\input.phase14.pjass.stats.json ^
  --emit-validation-report build\input.phase14.pjass.validation.json ^
  --compare-jasshelper samples\output_jasshelper.j ^
  --check-output-syntax-lite ^
  --validate-pjass ^
  --pjass jasshelper\pjass.exe ^
  --common jasshelper\common.j ^
  --blizzard jasshelper\blizzard.j ^
  --pjass-allow-external InitTrig_japi
```

### 9.2 War3Lib both 模式

```bat
set WAR3_JASS_COMPILER=both
set WAR3_VJASSC_VALIDATE=1
lua TaskStartMap.lua
```

实际任务脚本名称按项目现有入口调整。

### 9.3 ALPHA vjassc 模式

```bat
set WAR3_JASS_COMPILER=vjassc
set WAR3_VJASSC_VALIDATE=1
set WAR3_BUILD_VERSION=ALPHA
lua TaskStartMapVjasscAlpha.lua
```

如项目任务系统不是这种形式，Codex 应按现有入口命名实现等价功能。

---

## 10. Codex 执行要求

### 10.1 不要破坏默认流程

默认 `StartCompile()` / `TaskStartMap` 仍应使用 jasshelper。

只有以下情况使用 vjassc：

```text
- 显式 WAR3_JASS_COMPILER=vjassc
- 显式 WAR3_JASS_COMPILER=both
- 新增 ALPHA vjassc 专用任务入口
```

### 10.2 不要伪造运行环境

禁止：

```text
- 把 InitTrig_japi 空函数写进正式 output.j
- 为了进游戏随便补 YD/JAPI/DzAPI dummy
- 删除源逻辑来消除运行时问题
```

允许：

```text
- validation-only external stub
- 在 PJASS validation 命令中传 --pjass-allow-external InitTrig_japi
- 在 report 中标记外部环境符号
```

### 10.3 保留回退

如果 vjassc 后端失败：

```text
- both 模式继续使用 jasshelper 输出
- vjassc 模式如果 strict=0，在 ALPHA 下可提示回退 jasshelper
- strict=1 才整体失败
```

### 10.4 写文档

新增：

```text
War3Lib/docs/vjassc_backend.md
War3Lib/docs/alpha_runtime_validation.md
JassChanger/docs/phase14_status.md
```

`phase14_status.md` 至少包含：

```text
- War3Lib 接入状态
- Xlimon ALPHA 测试状态
- jasshelper/vjassc both 模式结果
- PJASS pass 状态
- 是否进入游戏
- runtime checklist
- 性能分段
- 已知问题
```

---

## 11. 预期风险

### 11.1 PJASS pass 不等于游戏能进

可能出现：

```text
- 初始化顺序不同
- ExecuteFunc bridge 行为差异
- function interface callback 行为差异
- DzAPI/YDWE/JAPI 环境符号运行时缺失
- InitTrig_japi 在真实地图中来源不明确
- 某些 jasshelper 行为未完全模拟
```

### 11.2 输出结构和 jasshelper 不同

不要求 byte-to-byte。先验证是否能加载和运行。

### 11.3 性能未必立刻优于 jasshelper

当前 vjassc full validation 仍约 57.8 秒。接入真实链路后，应区分：

```text
- vjassc fast codegen 时间
- vjassc validation 时间
- War3Lib 总编译时间
- jasshelper 原链路时间
```

---

## 12. 最终交付物

Phase 14 完成时，应至少交付：

```text
1. War3Lib 支持 jasshelper/vjassc/both 后端
2. ALPHA vjassc 测试入口
3. both 模式输出 5_jasshelper.j 与 5_vjassc.j
4. compiler_backend_report.json
5. vjassc.validation.json / vjassc.stats.json
6. runtime checklist 文档
7. phase14_status.md
8. 默认 jasshelper 流程不变
```

---

## 13. 进入下一阶段的条件

完成 Phase 14 后，如果：

```text
- ALPHA vjassc 输出可以进入游戏
- 核心功能初步正常
- PJASS 继续 pass
```

则 Phase 15 可以做：

```text
- 行为差异修复
- 更深 runtime regression
- 正式性能优化，把编译从几十秒压到 10 秒以内
```

如果 ALPHA 进不了游戏，则 Phase 15 应优先做：

```text
- runtime failure triage
- InitTrig_japi / JAPI / YDWE 环境来源确认
- 初始化顺序和 callback bridge 行为修复
```

---

## 14. 当前执行进度与阶段总结

### 14.1 已完成

```text
- War3Lib 已接入 jasshelper / vjassc / both 三种最终 JASS 编译后端。
- Xlimon 已新增单元测试与内测版本的 vjassc 启动、编译、对比任务。
- both 模式已能同时生成 5_jasshelper.j 与 5_vjassc.j，并保留 report / stats / validation。
- UNITTEST vjassc 链路已能通过编译、PJASS、w3x2lni，并由用户确认可进入游戏。
- Museum F2 开关测试中的 uiDragger 销毁回调遗漏已修复。
- SyncBus 的 onDataSync 注册已优化为接近 JassHelper 的直接调用形式，不再生成 vjassc__bridge__s__syncBus_onDataSync。
```

### 14.2 JassChanger 已修复的问题

```text
- public/private 命名：公开符号尽量保持 JassHelper 名称，私有/default Zinc 符号才加作用域前缀。
- Logger/Trace 名称：避免 public Trace 和 logger_tr 被错误加 Logger_ 前缀。
- struct 固定数组：按 JassHelper 方式生成 per-instance base offset，例如 (this-1)*N。
- struct 分配上限：按最大固定数组成员尺寸限制实例数量，避免数组越界。
- -warn / --warn：为 struct allocate/destroy 生成调试提示，但默认不启用。
- struct onInit 顺序：按 library requires 拓扑排序，修复 uiLifeCycle 早于 uiDragger 的依赖要求。
- function cycle bridge：优先打断无参反向边，减少带 code 参数调用的全局临时 bridge。
- function-object 参数 lowering：补齐 .execute/.evaluate 中 receiver-field 参数改写。
- function interface OOS 风险：默认注册 TriggerAddCondition 并用 TriggerEvaluate 调用；仅显式 .execute() 的目标额外注册 TriggerAddAction 并保留 TriggerExecute。
- Zinc 前导点链：修复链式调用中间穿插纯注释行时 receiver 丢失的问题，避免 itemBtns.icons[i] 初始化漏掉 setTopRightPadding/setPoint/setTexture。
- function name .evaluate：兼容函数名直接 `.evaluate()` 的 JassHelper 式 method caller，避免无参循环调用退回 ExecuteFunc。
- function name .execute：对函数名 `.execute()` 使用 TriggerAddAction caller；只有同一目标也被 `.evaluate()` 时才同时注册 condition/action。
- function interface 注册裁剪：只对实际使用到 execute/evaluate 的目标注册 trigger；没有被调用的 function interface target 不再生成冗余 wrapper。
- function interface wrapper 命名：统一为 `vjfi__...__Target_wrapper`，condition/action 可共用同一个 wrapper，减少冗余函数。
- 循环断环 caller：所有需要打断循环依赖的普通函数调用改为 `TriggerEvaluate + TriggerAddCondition`，不再生成 `vjassc__bridge__...` + `ExecuteFunc`。
- 数组下标括号膨胀：数组下标 rewrite 已做幂等化，避免 `IFubenDiffCache[(2)]` 在多轮改写后膨胀成 `IFubenDiffCache[((((2))))]`。
- 裸 Zinc 链式调用：修复 `uiText.create(...).setAllPoint(...).setText(...)` 被拆成多次 `uiText.create` 的问题，AbilityLock footer 按钮文字恢复。
```

### 14.3 当前保留的三类任务语义

```text
单元测试:
  默认 jasshelper，作为稳定基准。

单元测试-vjassc:
  both 编译，最终选择 vjassc，strict=true，用于验证 vjassc 是否能真正进图。

单元测试-vjassc对比:
  both 编译，最终选择 jasshelper，用于生成两份 J 文件后人工对比。

内测版本:
  默认 jasshelper，作为 ALPHA 稳定基准。

内测-vjassc:
  vjassc 后端，最终选择 vjassc，用于真实 ALPHA 替代验证。

内测-vjassc对比:
  both 编译，最终选择 jasshelper，用于大图输出对比。
```

### 14.4 最新性能检测

检测时间：2026-05-09。
检测对象：`D:/War3/Maps/Xlimon/edit/ability/pool/AbilityLock_Test.j` 单元测试链路，输入为 `Output/4_luaexecute.j`。

#### 14.4.1 War3Lib both 编译链路

来自 `D:/War3/Maps/Xlimon/Output/compiler_backend_report.json`：

```text
buildVersion: 单元测试
backend: both
selectedOutput: vjassc
totalCompileMs: 33295
jassCompilerMs: 29480

jasshelper elapsedMs: 4432
vjassc elapsedMs:    24917
pjassOk:             true

wave1Ms:             494
injectCodeBlockMs:   565
wave2Ms:             755
compileLuaMs:        1478
dzApiMapConfigMs:    38
copyBackMs:          3
```

结构规模：

```text
4_luaexecute.j: 1.40 MB / 33247 lines
5_jasshelper.j: 1.93 MB / 39052 lines / 2631 functions
5_vjassc.j:     1.39 MB / 36580 lines / 2503 functions
```

结论：

```text
- 当前单元测试链路总耗时约 33.3s。
- 最终 JASS 编译段约 29.5s，是 War3Lib 链路主瓶颈。
- vjassc validation 模式约 24.9s，明显慢于 jasshelper 的约 4.4s。
- Wave / 注入 / Lua / DzAPI 合计约 3.8s，不是 Phase 15 首要瓶颈。
```

#### 14.4.2 vjassc 内部耗时

来自 `Output/vjassc.stats.json`：

```text
total:        24844 ms
read:         13 ms
preprocess:   34 ms
staticIf:     57 ms
lex:          49 ms
parse:        54 ms
moduleExpand: 4 ms
codegen:      22120 ms
syntaxLite:   2372 ms
pjass:        109 ms
comparison:   39 ms
```

codegen pass 细分：

```text
emitFunctions:               21254 ms
emitStructSupport:           18481 ms
lowerLambdas:                1825 ms
finalOutputValidationPrep:   767 ms
sanitizeOutput:              587 ms
functionOrdering:            180 ms
```

性能计数器：

```text
linesVisited:        23525
memberAccessScans:   6139
structLookupCalls:   45383
functionLookupCalls: 65807
cachedRewriteHits:   106875
cachedRewriteMisses: 4315
```

注意：`emitFunctions` 与 `emitStructSupport` 统计有包含/嵌套关系，二者不能简单相加。但它们共同指向同一类问题：函数输出阶段在 struct/method 支持逻辑上做了大量重复扫描、查找和字符串改写。

#### 14.4.3 vjassc fast 生成下限

命令：

```bat
build\vjassc.exe D:\War3\Maps\Xlimon\Output\4_luaexecute.j ^
  -o build\phase14_abilitylock_fast.out.j ^
  --emit-stats build\phase14_abilitylock_fast.stats.json
```

结果：

```text
外部 stopwatch: 22295 ms
stats total:    22099 ms
codegen:        21886 ms
syntaxLite:     0 ms
pjass:          0 ms
comparison:     0 ms
```

结论：

```text
- 即使关闭 syntax-lite / PJASS / comparison，vjassc 仍约 22.1s。
- validation 额外成本约 2.7s，其中 syntax-lite 约 2.4s，PJASS 约 0.1s。
- Phase 15 若要把编译压到 10s 内，不能只关验证；必须优化 codegen。
```

### 14.5 阶段 15 准备建议

```text
优先级 P0:
  优化 emitFunctions / emitStructSupport 路径。
  重点排查每个函数体 lowering 时是否重复遍历所有 structs / methods / fields。
  将热路径从“全量扫描 + regex/string rewrite”改为“按 token/候选名索引命中后再改写”。

优先级 P1:
  拆分 fast / validate / full-validation 模式。
  fast 模式默认跳过 syntax-lite / PJASS / comparison，但这只能节省约 2.7s。

优先级 P2:
  降低 sanitizeOutput / finalOutputValidationPrep 的整文件二次扫描成本。
  对 functionOrdering 的循环断环 caller 继续保持正确性优先，后续再做微优化。

阶段 15 性能目标建议:
  第一目标：AbilityLock 单测 fast 从 22s 降到 10s 以内。
  第二目标：AbilityLock validation 从 25s 降到 12s 以内。
  第三目标：内测大图 vjassc 输出进入游戏后，再用同样指标测完整 ALPHA。
```

### 14.6 下一步

```text
- 继续以 Xlimon 单元测试作为小样本，逐个定位 JassHelper / vjassc 行为差异。
- 每次差异优先对比 5_jasshelper.j 与 5_vjassc.j，确认是编译器差异还是源代码依赖了 JassHelper 特性。
- 小样本稳定后，再回到内测-vjassc测试完整 ALPHA 地图。
- 阶段 15 可正式启动性能专项：先优化 codegen 热路径，再做 full validation 的开关与降本。
```
