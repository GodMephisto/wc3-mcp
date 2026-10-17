---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase22_entry_audit.md"
ref: HEAD
---

# Phase 22 Entry Audit

## Snapshot

- Branch: `codex/phase22-parallel-incremental`
- Base commit at audit start: `b1743a8`
- Task plan: `phase22_codex_task_plan.md`
- Build baseline: `cmake --build build` passed
- Test baseline: `ctest --test-dir build --output-on-failure` passed

## Phase 21 Surface Check

| CLI surface | Status |
|---|---|
| `--experimental-recorded-order` | present |
| `--experimental-parallel-lowering` | present |
| `--parallel-workers <N>` | present |
| `--experimental-incremental-cache <path>` | present |
| `--incremental-mode report|reuse` | present |
| `--emit-incremental-state <path>` | present |
| `--emit-incremental-report <path>` | present |
| `--compare-incremental-state <path>` | present |
| `--emit-dependency-report <path>` | present |
| `--emit-generated-entity-plan <path>` | present |
| `--emit-benchmark-report <path>` | present |

## Baseline Results

| Check | Result |
|---|---|
| default fast | pass, elapsed 7092 ms, internal total 6610 ms |
| default validate + PJASS | pass, elapsed 8390 ms |
| recorded-order fast | pass, `functionOrderTokenScans=0` in the old report |
| parallel workers=4 fast | pass, output byte-identical to default |
| incremental cold | pass, 6040 ms |
| incremental no-change with report | pass, 532 ms, `reusePercent=100` |
| incremental no-change no report | pass, 379 ms |
| no-change output equality | cold output equals hot output |

## Audit Findings

- Generated entity plan existed and was deterministic for lambdas and function-interface targets.
- Struct lifecycle/generated support entries were not represented in the plan before Phase 22.
- Parallel lowering was a deterministic metadata path, not a real concurrent body runner.
- Incremental cache could short-circuit no-change runs, but the hot path still read the cached output into memory before writing the output.
- Recorded-order baseline was unsafe: its old coverage report compared recorded edges against the recorded dependency set after recorded ordering was already selected. A validate run exposed PJASS forward-reference failures, so Phase 22 must verify recorded edges against a conservative output scan and fallback when coverage is incomplete.
- War3Lib/Xlimon ALPHA baseline can run through `TaskCompileCompareAlpha.lua`, but current War3Lib scripts do not pass Phase 22 experimental env flags through to `vjassc`.

## Known Issues At Entry

- Recorded-order cannot be promoted until missing strong edges are closed.
- Parallel lowering cannot be promoted until the implementation moves beyond metadata and shows median speedup.
- Incremental small-change still uses full cold compile on changed input, then reports chunk reuse/invalidations.
- War3Lib env propagation is outside this repository and remains a follow-up unless that repo is changed separately.
