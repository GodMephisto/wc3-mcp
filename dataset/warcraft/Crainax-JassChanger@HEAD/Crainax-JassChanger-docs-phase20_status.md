---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase20_status.md"
ref: HEAD
---

# Phase 20 Status

Phase 20 added conservative performance groundwork on top of Phase 19, plus
deterministic reporting for later parallel/incremental work. Correctness is
green in the current workspace, but the strict Phase 20 speed gate was not
reached on this machine, so the release executable was not deployed to
`D:\War3\plugins\vjassc\vjassc.exe`.

## Implemented

- Narrowed method-body slow-path entry for bracket expressions: ordinary
  one-dimensional array/index expressions no longer force struct/array lowering
  unless the receiver is a known multi-dimensional/fixed array.
- Added first-character filtering for known global array receivers to avoid
  unnecessary hash lookups during array-rewrite checks.
- Replaced function-order dependency storage with de-duplicated sorted vectors,
  preserving deterministic traversal while reducing set insertion overhead.
- Reconciled function-order discovered dependencies back into the recorder
  report, so dependency coverage reflects the known dependency graph while
  retaining the output-scan fallback.
- Added `--emit-generated-entity-plan <path>` for deterministic lambda and
  function-interface target reporting.
- Generated entity plans are now built only when that flag is requested, so the
  default fast/validate path does not pay the report-generation cost.
- Added process-local caching for parsed local declaration lists.
- Added a direct no-string/comment path in protected-region rewriting to avoid
  an extra scan for ordinary generated lines.
- Extended golden tests to verify generated entity plan stability and repeated
  fast output stability.
- Updated Phase 20 report/version strings and Release flags with conservative
  MSVC code-generation options.

## Standalone Results

Commands were run against `samples/input.j` with release `build/vjassc.exe`.
The final default fast path was measured without `--emit-generated-entity-plan`;
the repeat run emitted the plan and incremental report for stability checks.

Artifacts:

- `build/input.phase20.final12.fast.stats.json`
- `build/input.phase20.final12.fast.repeat.stats.json`
- `build/input.phase20.final12.validate.stats.json`
- `build/input.phase20.final12.validate.validation.json`
- `build/input.phase20.final12.full.stats.json`
- `build/input.phase20.final12.full.validation.json`
- `build/input.phase20.final12.performance.json`
- `build/input.phase20.final12.repeat.performance.json`
- `build/input.phase20.final12.generated_entity_plan.json`
- `build/input.phase20.final12.incremental.state.json`
- `build/input.phase20.final12.incremental.report.json`

| Mode | Phase 19 baseline | Phase 20 target | Phase 20 measured | Result |
| --- | ---: | ---: | ---: | --- |
| fast | 5116 ms phase-19 final | <= 4700 ms | 5341 ms | fail |
| fast repeat | 5116 ms phase-19 final | <= 4700 ms | 5159 ms | fail |
| validate | 6901 ms phase-19 final | <= 6500 ms | 6452 ms | pass |
| full-validation | 6599 ms phase-19 final | <= 6500 ms strict local gate | 6711 ms | fail |

Correctness:

| Check | Result |
| --- | --- |
| `ctest --test-dir build --output-on-failure` | pass |
| standalone validate syntax-lite | pass, issueCount 0 |
| standalone validate PJASS | pass, groupedCount 0 |
| standalone full-validation syntax-lite | pass, issueCount 0 |
| standalone full-validation PJASS | pass, groupedCount 0 |
| repeated fast output on `samples/input.j` | pass, byte-identical |
| generated entity plan output | exists and golden stability test passes |
| read-only incremental no-change reuse | pass, 13255 / 13255 chunks, 100% |

## Counter Results

| Metric | Phase 19 baseline | Phase 20 measured | Target | Result |
| --- | ---: | ---: | ---: | --- |
| sourceMethods fast | 2009 ms | 2111 ms | <= 1700 ms | fail |
| sourceMethods fast repeat | 2009 ms | 2053 ms | <= 1700 ms | fail |
| syntaxLite validate | 1012 ms | 1051 ms | <= 800 ms good target | fail |
| syntaxLite full-validation | 1019 ms | 997 ms | <= 1000 ms | pass |
| functionOrdering fast | 256 ms | 312 ms | lower is better | fail |
| dependency recorder coverage | 13.23% | 100% | >= 35% | pass |
| missingRecordedEdges | 14085 | 0 | <= 11000 | pass |
| functionOrderTokenScans | 56698 | 56698 | <= 35000 good target | fail |

## War3Lib / Xlimon ALPHA

War3Lib ALPHA was run during the Phase 20 branch validation before the final
local-only cleanup. It passed correctness but did not pass the strict Phase 20
speed gate, and standalone fast still fails now, so no deployment decision was
made from these results.

Validate command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result: pass, selected vjassc output.

| Metric | Value | Target | Result |
| --- | ---: | ---: | --- |
| vjassc validate elapsed | 6.80 s | <= 6.50 s | fail |

Fast compare command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result: pass, selected JassHelper output for final map output.

| Metric | Value | Target | Result |
| --- | ---: | ---: | --- |
| vjassc fast elapsed | 5.53 s | <= 5.20 s | fail |
| jasshelper elapsed | 11.84 s | reference | pass |

## Deployment

Deployment was skipped because the strict speed acceptance gate did not pass.

The file was not copied to:

```text
D:\War3\plugins\vjassc\vjassc.exe
```

## Remaining Work

- Reduce `sourceMethods` below 1700 ms; the remaining hotspot is still method
  member/body lowering rather than CLI report generation.
- Reduce function-order token scans; recorder coverage is complete in the
  report, but the output scan is still used for fallback ordering.
- Implement a real deterministic body-lowering job model before enabling
  parallel lowering, rather than trying to force unsafe dot/bracket shortcuts.
