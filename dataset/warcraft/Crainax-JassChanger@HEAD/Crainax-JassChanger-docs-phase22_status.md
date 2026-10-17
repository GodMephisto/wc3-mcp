---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase22_status.md"
ref: HEAD
---

# Phase 22 Status

## Implemented

- Added `--experimental-body-jobs-single-thread` as the stable single-thread body-job equivalence switch.
- Productized Phase 22 reports: performance/dependency/validation/incremental/benchmark reports now emit `phase: 22` and include experimental recorded/parallel/incremental metadata.
- Added generated entity plan coverage for struct lifecycle support functions (`StructAllocate`, `StructCreate`, `StructDestroy`, `StructDeallocate`).
- Fixed recorded-order safety: it now compares recorded edges against a conservative output scan and automatically falls back when strong edges are missing.
- Optimized no-change incremental cache hit path by copying the cached output file directly when no report or in-memory validation needs the output text.
- Added `tools/bench_phase22.ps1`.
- Extended golden tests for Phase 22 body-job equivalence, report fields, and generated support plan coverage.

## Entry Audit Summary

See `docs/phase22_entry_audit.md`.

## Correctness Matrix

| Check | Result |
|---|---|
| `cmake --build build` | pass |
| `ctest --test-dir build --output-on-failure` | pass |
| default validate + PJASS | pass |
| parallel validate + PJASS | pass |
| recorded-order validate + PJASS | pass by fallback to output scan |
| incremental small-change validate + PJASS | pass |
| War3Lib/Xlimon ALPHA compare task | pass, `vjassc` 5599 ms, jasshelper 11815 ms, total 27.97 s |

## Determinism Matrix

| Check | Result |
|---|---|
| body-jobs single output vs default | byte-identical |
| parallel workers 1/2/4/8 | byte-identical |
| parallel repeat x3 | byte-identical |
| incremental no-change output vs cold | byte-identical |
| generated entity plan repeated JSON | byte-identical |
| generated entity plan parallel JSON | byte-identical to default |

## Generated Entity Plan Coverage

- Repeated `samples/input.j` plan output was stable.
- Parallel plan output was stable against default.
- Struct generated support functions are now listed in `generatedSupport` with stable keys and signatures.

## BodyJob Productization

- `--experimental-body-jobs-single-thread` is wired as a no-output-change equivalence path.
- The current body-job model remains single-thread deterministic. Mutable codegen state is not yet safe to run as true parallel body jobs.

## Parallel Lowering Results

| Scenario | Median |
|---|---:|
| default fast | 5268 ms |
| parallel workers=4 | 5381 ms |

Parallel remains deterministic but is not faster. It stays `experimental`.

## Incremental Cache Results

| Scenario | Result |
|---|---:|
| cold incremental run | 6248 ms |
| no-change with report | 476 ms |
| no-change no report | 61 ms |
| benchmark no-change median | 239 ms |
| no-change reuse | 100% |
| small-change reuse | 99.9925%, `changedChunks=1`, `reusedChunks=13254` |

The direct-copy hot path is the main Phase 22 speed improvement.

## Recorded Dependency Graph Results

| Metric | Result |
|---|---:|
| recorded edges | 2220 |
| conservative output-scan edges | 16232 |
| matched edges | 2219 |
| missing recorded edges | 14013 |
| extra recorded edges | 1 |
| fallback | true |

Recorded-order is now safe, but not promotable.

## War3Lib / Xlimon ALPHA Results

- Command: `Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin`
- `WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe`
- `WAR3_VJASSC_MODE=fast`
- Result: pass
- Note: current War3Lib script did not propagate `WAR3_VJASSC_RECORDED_ORDER`, `WAR3_VJASSC_PARALLEL`, or `WAR3_VJASSC_INCREMENTAL` into the `vjassc` command. The repo was already dirty, so this phase documents the external follow-up instead of mixing War3Lib edits into the JassChanger commit.

## Benchmark Median Report

| Scenario | Median |
|---|---:|
| default fast | 5268 ms |
| parallel workers=4 | 5381 ms |
| incremental no-change | 239 ms |

## Feature Promotion Recommendation

| Feature | Status | Default Recommendation | Reason |
|---|---|---|---|
| recorded-order | experimental | manual only | safe fallback works, but strong edge coverage is only 13.67% on `samples/input.j` |
| parallel-lowering | experimental | manual only | deterministic, but median is slightly slower than default |
| incremental-cache | candidate | ALPHA/manual no-change use | no-change hot path is clearly faster and deterministic; small-change still cold-compiles changed input |

## Known Issues

- Recorded dependency recorder needs many more strong edges before output-scan fallback can be removed.
- True parallel body lowering is not implemented; current parallel switch is still a deterministic experiment surface.
- Small-change incremental does not yet reuse body lowering results, although it correctly reports chunk invalidation and validates the output.
- War3Lib environment-variable propagation needs a separate clean change in the War3Lib repository.

## Phase 23 Recommendation

- Close missing recorded edges for struct support, lambdas, function interface wrappers, and initializer/static-onInit calls.
- Move body lowering into an immutable job/result model before enabling true parallel workers.
- Add body-level cache records so small-change runs reuse unchanged lowering results instead of only reporting changed chunks.
- Update War3Lib compile scripts in a separate clean branch to pass Phase 22 experimental flags and reports through to `vjassc`.
