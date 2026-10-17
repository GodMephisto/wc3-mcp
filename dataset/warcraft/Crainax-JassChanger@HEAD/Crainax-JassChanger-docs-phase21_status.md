---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase21_status.md"
ref: HEAD
---

# Phase 21 Status

## Implemented

- Added default-off CLI switches:
  - `--experimental-recorded-order`
  - `--experimental-parallel-lowering`
  - `--parallel-workers <N>`
  - `--experimental-incremental-cache <dir>`
  - `--incremental-mode report|reuse`
  - `--emit-dependency-report <path>`
  - `--emit-benchmark-report <path>`
- Extended generated entity plan output to phase 21 with stable keys and body job model metadata.
- Added recorded dependency ordering as an experimental function-order path.
- Added experimental no-change incremental cache reuse. The cache is key-guarded by input text and relevant CLI options, default-off, and only used in `reuse` mode.
- Added `tools/bench_phase21.ps1` for warmup/repeat median benchmark runs.
- Added golden-runner checks for recorded-order reports, parallel experiment metadata, benchmark report output, and incremental no-change cache reuse.

## Standalone Results

Commands run against `samples/input.j`:

- `cmake --build build`: pass.
- `ctest --test-dir build --output-on-failure`: pass.
- Default fast baseline: pass.
- Default validate + PJASS with `InitTrig_japi` external stub: pass.
- Generated entity plan repeat stability: output hash equal and plan JSON hash equal.

Baseline fast single-run report:

| Metric | Value |
| --- | ---: |
| totalMs | 5187 |
| codegenMs | 4605 |
| functionOrderTokenScans | 56698 |
| chunks | 13255 |

## Experimental Recorded Order

Command:

```bat
build\vjassc.exe samples\input.j -o build\phase21.recorded-order.out.j --mode validate --experimental-recorded-order --emit-dependency-report build\phase21.recorded-order.deps.json --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi
```

Result:

| Metric | Value |
| --- | ---: |
| PJASS groupedCount | 0 |
| recordedEdges | 2220 |
| outputScanEdges | 2220 |
| coveragePercent | 100 |
| functionOrderTokenScans | 0 |

This mode is still default-off. It passed standalone validation, but it should remain experimental until it is exercised on more real maps and cycle-heavy fixtures.

## Experimental Parallel Lowering

Command:

```bat
build\vjassc.exe samples\input.j -o build\phase21.parallel.out.j --mode fast --experimental-parallel-lowering --parallel-workers 4 --emit-performance-report build\phase21.parallel.performance.json
```

Correctness:

- Single-thread fast output equals experimental parallel output byte-for-byte.
- Three repeated experimental parallel outputs are byte-identical.
- Experimental parallel validate + PJASS passes.

Benchmark median, `tools/bench_phase21.ps1`, warmup 1, repeat 5:

| Mode | Median |
| --- | ---: |
| default fast | 5500 ms |
| experimental parallel metadata | 5738 ms |

The current parallel path is a deterministic job/report scaffold, not real concurrent body lowering. It does not meet the phase speedup target and should not be enabled by default.

## Experimental Incremental Cache

No-change cache command pair:

```bat
build\vjassc.exe samples\input.j -o build\phase21.inc.cold.out.j --mode fast --experimental-incremental-cache build\.vjassc-cache-phase21 --incremental-mode reuse --emit-incremental-state build\phase21.inc.state.json
build\vjassc.exe samples\input.j -o build\phase21.inc.nochange.out.j --mode fast --experimental-incremental-cache build\.vjassc-cache-phase21 --incremental-mode reuse --compare-incremental-state build\phase21.inc.state.json --emit-incremental-report build\phase21.inc.nochange.report.json
```

No-change result:

| Metric | Value |
| --- | ---: |
| output equality | pass |
| cacheHit | true |
| reusedChunks | 13255 |
| reusePercent | 100 |
| validate existing output + PJASS | pass |
| benchmark median | 353 ms |

Small-change result:

| Metric | Value |
| --- | ---: |
| changedChunks | 1 |
| reusedChunks | 13254 |
| reusePercent | 99.9925 |
| cacheHit | false |
| validate existing output + PJASS | pass |

This cache is default-off. The no-change path meets the phase target. The small-change path reports correct chunk reuse and safely falls back to cold compilation, but it does not yet reuse body lowering output for faster small-change builds.

## War3Lib / Xlimon ALPHA Results

Command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result:

| Metric | Value |
| --- | ---: |
| task exit | 0 |
| vjassc result | success |
| vjassc elapsed | 5.58s |
| vjassc stats totalMs | 5496 |
| vjassc stats codegenMs | 4868 |

The compare task still reports a later jasshelper failure in `currentmapscript.j`; `vjassc` itself completed successfully.

## Correctness Matrix

| Check | Result |
| --- | --- |
| `ctest` | pass |
| default validate + PJASS | pass |
| recorded-order validate + PJASS | pass |
| parallel validate + PJASS | pass |
| repeated fast output stability | pass |
| generated entity plan stability | pass |
| incremental no-change output equality | pass |
| incremental no-change PJASS | pass |
| incremental small-change PJASS | pass |

## Performance Median Report

`tools/bench_phase21.ps1`, warmup 1, repeat 5:

| Scenario | Median | Target |
| --- | ---: | --- |
| default fast | 5500 ms | no clear regression vs phase 20 |
| experimental parallel | 5738 ms | fail, no speedup |
| incremental no-change | 353 ms | pass, <= 2500 ms |

## Known Issues

- `--experimental-parallel-lowering` does not yet run body jobs concurrently. It only preserves deterministic metadata and output stability.
- Small-change incremental currently reports chunk reuse and avoids unsafe cache hits, but still cold-compiles changed input.
- War3Lib task scripts do not yet pass phase21 experimental flags from environment. The local War3Lib checkout has unrelated dirty files, so this phase did not edit that repository.
- Recorded-order has passed standalone `samples/input.j`, but should remain default-off until more cycle-heavy and real-map fixtures are covered.

## Next Phase Recommendation

- Move body lowering writes behind a real `BodyJob` result object so ordinary functions, source methods, and lambdas can run on worker threads.
- Add a body-level cache artifact keyed by function/method/lambda source and symbol hashes, instead of reusing final output only for no-change.
- Add War3Lib environment plumbing after isolating the existing dirty War3Lib changes, then validate `WAR3_VJASSC_RECORDED_ORDER`, `WAR3_VJASSC_PARALLEL`, and `WAR3_VJASSC_INCREMENTAL` in ALPHA only.
- Keep all phase21 experimental switches disabled by default.
