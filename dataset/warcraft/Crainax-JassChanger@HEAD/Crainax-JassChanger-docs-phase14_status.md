---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase14_status.md"
ref: HEAD
---

# Phase 14 Status

Phase 14 moved vjassc from standalone `samples/input.j` validation into the
real War3Lib/Xlimon compile path. The default map workflow still uses
JassHelper; vjassc is only selected by explicit backend config or the new ALPHA
task entries.

This is compile-chain validation, not a completed Warcraft III runtime pass.
Codex did not manually enter the game.

## Implemented

- War3Lib now supports `jasshelper`, `vjassc`, and `both` JASS compiler
  backends.
- The backend can be controlled by environment/config values:
  `WAR3_JASS_COMPILER`, `WAR3_JASS_COMPILER_SELECT`, `WAR3_VJASSC_EXE`,
  `WAR3_VJASSC_VALIDATE`, `WAR3_VJASSC_STRICT`, and
  `WAR3_ALLOW_VJASSC_NON_ALPHA`.
- The original JassHelper output path is preserved as
  `Output/5_jasshelper.j`; vjassc writes `Output/5_vjassc.j`.
- `both` mode writes `Output/compiler_backend_report.json` with structure
  metrics, backend timings, and selected-output metadata.
- vjassc validation writes `Output/vjassc.stats.json`,
  `Output/vjassc.validation.json`, `Output/vjassc.stdout.txt`,
  `Output/vjassc.stderr.txt`, and `Output/vjassc.cmd`.
- ALPHA-only task scripts were added for compile and launch flows:
  `TaskCompileAlphaWithVjassc.lua`, `TaskCompileCompareAlpha.lua`,
  `TaskStartMapVjasscAlpha.lua`, and `TaskStartMapCompareAlpha.lua`.
- Xlimon VSCode tasks now expose `🥭启动:内测-vjassc` and
  `🥭启动:内测-vjassc对比`.
- Runtime checklist files are generated in Xlimon output:
  `Output/vjassc_runtime_checklist.md` and `Output/runtime_notes.md`.
- JassChanger fixed a real Xlimon PJASS blocker where function-object
  `.evaluate`/`.execute` fallback calls did not lower receiver-field arguments
  such as `keyshop.playerU[pid]`.
- Unit-test vjassc launch and compare tasks were added for Xlimon so small
  fixtures can be used before testing the 100k-line ALPHA map.
- vjassc gained `-warn` / `--warn` struct diagnostics for allocation overflow,
  null destroy, and double free checks. The option is intentionally opt-in so
  production output stays close to JassHelper unless requested.
- Struct fixed-size arrays now use JassHelper-style per-instance base offsets
  and allocation limits, which fixed `tooltip`-style storage differences.
- Zinc library naming was aligned closer to JassHelper: public symbols keep
  public names, while private/default Zinc globals and structs are scoped.
- Struct `onInit` calls are now ordered by library dependency order. This fixed
  the `UILifeCycle` / `uiDragger` runtime regression where dragger destroy
  callbacks were not registered before Museum close.
- Function cycle breaking now prefers a no-argument `ExecuteFunc` edge when the
  opposite edge is a direct call with parameters and a `code` callback. This
  removes the `vjassc__bridge__s__syncBus_onDataSync` temp-argument bridge and
  keeps `syncBus.onInit` closer to JassHelper output.

## Validation Commands

JassChanger build and golden tests:

```bat
cmd.exe /d /s /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

Result:

```text
100% tests passed, 0 tests failed out of 1
```

Standalone vjassc PJASS validation:

```bat
build\vjassc.exe samples\input.j -o build\input.phase14.pjass.out.j --emit-stats build\input.phase14.pjass.stats.json --emit-validation-report build\input.phase14.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi
```

Result: exit code `0`, PJASS pass.

War3Lib default compile:

```bat
cmd /c "chcp 65001>nul && cd /d D:\War3\Library\War3Lib && lua lua/tasks/TaskCompile.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin"
```

Result: pass. Final output selected `Output/5_jasshelper.j`, preserving the
default workflow.

War3Lib ALPHA compare compile:

```bat
cmd /c "chcp 65001>nul && cd /d D:\War3\Library\War3Lib && lua lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin"
```

Result: pass. `jasshelper` and `vjassc` both completed; final output selected
`Output/5_jasshelper.j`.

War3Lib ALPHA vjassc-selected compile:

```bat
cmd /c "chcp 65001>nul && cd /d D:\War3\Library\War3Lib && lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin"
```

Result: pass. Final output selected `Output/5_vjassc.j`.

After this check, the normal default compile was run again so
`Output/output.j` returned to the JassHelper-selected default output.

## Latest Compare Report

The latest `Output/compiler_backend_report.json` after ALPHA compare mode:

```json
{
  "backend": "both",
  "selectedOutput": "jasshelper",
  "buildVersion": "内测版本",
  "jasshelper": {
    "ok": true,
    "elapsedMs": 12545,
    "lines": 116650,
    "functions": 6878
  },
  "vjassc": {
    "ok": true,
    "elapsedMs": 65362,
    "pjassOk": true,
    "lines": 115234,
    "functions": 6635
  },
  "diff": {
    "lineDelta": -1416,
    "functionDelta": -243,
    "globalDelta": 0,
    "nativeDelta": 0,
    "hasMainBoth": true,
    "hasConfigBoth": true,
    "hasInitCustomTriggersBoth": true
  }
}
```

The validation-only `InitTrig_japi` stub was emitted only in the PJASS helper
stub file, not as a function definition in `Output/5_vjassc.j` or
`Output/output.j`.

## Performance

War3Lib end-to-end timings from the latest compare report:

| Segment | ms |
| --- | ---: |
| syncMirrorMs | 401 |
| wave1Ms | 2135 |
| injectCodeBlockMs | 2494 |
| wave2Ms | 2640 |
| compileLuaMs | 3651 |
| jassCompilerMs | 78415 |
| totalCompileMs | 90099 |

vjassc internal timings:

| Segment | ms |
| --- | ---: |
| codegen | 53921 |
| syntaxLite | 9874 |
| pjass | 348 |
| comparison | 121 |
| total | 65241 |

The real-chain bottleneck is still vjassc code generation plus syntax-lite.
Phase 14 established the benchmark surface; it did not attempt the next
performance rewrite.

## Runtime State

- `Output/vjassc_runtime_checklist.md` exists for manual ALPHA game testing.
- `Output/runtime_notes.md` exists for user-observed runtime notes.
- User manual testing confirmed that the focused UNITTEST map can enter game
  after the `w3x2lni`/J-file fixes.
- User manual testing confirmed the Museum F2 open/close regression was fixed
  after struct `onInit` dependency ordering was corrected.
- Full ALPHA compatibility is still open; the next work should use the current
  unit-test and compare tasks to triage remaining behavior differences one by
  one.

## Known Issues

- PJASS pass does not prove Warcraft III runtime equivalence.
- vjassc output structure differs from JassHelper output by design; Phase 14
  only checks structure-level properties and PJASS validity.
- Full validation is still slow: latest vjassc total is `65241 ms`, with
  `53921 ms` in codegen and `9874 ms` in syntax-lite.
- ALPHA compatibility remains incomplete and should be solved incrementally from
  small unit tests before returning to the full 100k-line map.
