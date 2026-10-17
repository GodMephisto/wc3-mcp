---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase23_status.md"
ref: HEAD
---

# Phase 23 状态报告

日期：2026-05-12
分支：`codex/phase23-real-parallel-incremental`

## 完成范围

- 新增阶段 23 body-level incremental cache，入口仍为实验开关：
  `--experimental-incremental-cache <dir> --incremental-mode reuse`。
- 将 JASS function、Zinc function、struct source method、lambda 的 lowered body 输出拆成可缓存片段；命中时直接复用缓存 body lines，未命中时走原 lowering 并写入缓存。
- body cache key 使用 `vjassc-phase23-body-cache-v3`，包含：
  - body kind、owner stable key、body mode、fast/warn 模式；
  - body 原始文本；
  - 全局函数/结构体/字段/方法/function-interface 的签名上下文 hash。
- 加入安全门禁，遇到动态回调/函数构造风险时跳过缓存：
  `function`、`.execute`、`.evaluate`、`ExecuteFunc`、`Condition(`、`Filter(`、`TriggerAddAction`、`TriggerAddCondition`。
- performance report / incremental report 增加 body cache 计数：
  lookups、hits、misses、stores、bypassedUnsafe、reusedLines、storedLines。
- CLI/report phase 升级到 phase 23 / `0.23.0`。
- 新增 `tools/bench_phase23.ps1`，覆盖冷编译、body-jobs-single、parallel metadata、no-change incremental、small-change incremental、validate 场景。
- golden runner 增加 body cache 冷缓存写入断言，并清理测试 cache dir 避免旧缓存污染。

## 正确性验证

命令：

```powershell
cmd.exe /d /c "call ""C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

结果：

- `cmake --build build` 通过。
- `ctest --test-dir build --output-on-failure` 通过，`golden_fixtures` 1/1 passed。

样例输出一致性：

- default fast、`--experimental-body-jobs-single-thread`、`--experimental-parallel-lowering --parallel-workers 4` 输出 SHA256 相同：
  `ED1B2F29B6F7725041D61239EB80B67A4C0746D76B2BDD6D94C6C716EF184DD5`。

PJASS / syntax-lite：

- `samples/input.j` validate：`syntaxLite.ok=true`，`pjass.ok=true`，syntax issue count `0`。
- small-change body-cache 输出 validate：`syntaxLite.ok=true`，`pjass.ok=true`，syntax issue count `0`。

recorded-order：

- recorded edges：`2220`
- output-scan edges：`16232`
- matched edges：`2219`
- missing recorded edges：`14013`
- coverage：`13.6705%`
- fallback：`true`

结论：recorded-order 仍未满足推广条件，继续保留 fallback，不默认启用。

## 性能结果

基准输入：`samples/input.j`

最终二进制基准：

| 场景 | warmup | repeat | median | min | p75 | codegen median | 备注 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| default-fast | 1 | 3 | 5697 ms | 5595 ms | 5836 ms | 4866 ms | 默认冷编译 |
| parallel workers=4 | 1 | 3 | 5614 ms | 5613 ms | 5644 ms | 4878 ms | 仍是 deterministic metadata path |
| incremental no-change | 1 | 5 | 278 ms | 269 ms | 278 ms | 0 ms | 最终输出热缓存路径 |
| incremental small-change | 1 | 5 | 3633 ms | 3534 ms | 3634 ms | 2754 ms | body cache 命中 4263 |

small-change 说明：

- small-change 输入只改动一个 debug 字符串，避免改符号名造成无效对照。
- benchmark small-change 每轮追加 `--emit-generated-entity-plan`，避免最终输出热缓存直接命中，强制观察 body cache。
- 对比 default-fast median：`5697 ms -> 3633 ms`，提速约 `36.2%`。

状态对比报告：

- changed chunks：`37`
- reused chunks：`13148`
- reuse percent：`99.7194%`
- body cache hits：`4263`
- body cache misses：`0`
- body cache bypassed unsafe：`1695`

## War3Lib / Xlimon ALPHA 验证

命令：

```powershell
$env:WAR3_VJASSC_EXE='D:/Project/JassChanger/build/vjassc.exe'
$env:WAR3_VJASSC_MODE='fast'
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

结果：

- 内测版本 `VERSION_ALPHA` compile compare 通过。
- vjassc fast：`6.59 s`
- jasshelper：`12.53 s`
- 任务总耗时：`31.83 s`
- compare 模式按原工作流选中 `5_jasshelper.j` 作为最终输出。

## 推广结论

- 有速度提升，满足本阶段“有提升才部署 exe”的门槛。
- 可部署 `build/vjassc.exe` 到 `D:\War3\plugins\vjassc\vjassc.exe` 给本地使用。
- 默认编译路径未启用 body cache、parallel-lowering、recorded-order，默认行为保持稳定。
- body-level incremental cache 可以继续作为实验路径试用。

## 未推广项

- `--experimental-parallel-lowering` 本阶段仍没有升级为真正 worker-thread body lowering。当前只是 deterministic metadata / output stability 验证路径，未默认启用。
- recorded-order 强边覆盖仍不足，继续 fallback。
- body cache 对动态回调/函数构造体采取跳过策略，优先正确性，不追求全覆盖。
