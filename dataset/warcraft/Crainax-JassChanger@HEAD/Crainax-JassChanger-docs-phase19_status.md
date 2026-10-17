---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase19_status.md"
ref: HEAD
---

# Phase 19 Status

Phase 19 is a performance pass on top of the accepted Phase 18 compiler. The
main work stayed on the conservative single-threaded path: fewer repeated scans,
less transient allocation, and tighter validation hot paths. PJASS, syntax-lite,
and War3Lib/Xlimon ALPHA validation remain green.

## Implemented

- Added first-character prefilters to scoped/public symbol rewriting.
- Switched struct, function, local-type, array-shape, and function-order lookup
  maps to transparent `string_view` lookup where the hot path only needs a view.
- Added first-character field/method candidate tables for current-struct feature
  scans.
- Replaced several bare field/method regex rewrites with token-level helpers.
- Added a cache for global array-access rewrites and pre-reserved the hot lookup
  caches used during cold compile.
- Tightened array rewrite entry so only known array receivers enter the costly
  rewrite path.
- Made Zinc simple-body lowering single-pass: simple bodies are lowered while
  being classified, and only complex bodies fall back to structural lowering.
- Reduced repeated trims/copies in `removeSemicolon`, `PathUtil::trim`, output
  statement-shape checking, and syntax-lite line iteration.
- Added cheap guards before syntax-lite regex checks for indexed-member,
  method-chain, and callback-lambda residue detection.
- Added Phase 19 performance JSON output with BodyMode, TokenCache,
  DependencyRecorder, MethodPlan, and incremental chunk summaries.
- Added read-only incremental state/report output. It hashes generated function
  chunks and reports reuse against a prior state without changing compiler
  output.
- Kept function-order output scanning as the conservative fallback. Dependency
  recorder coverage is reported, including weak `ExecuteFunc` edges, but it is
  not complete enough to replace the output scan.

## Standalone Results

Commands were run against `samples/input.j` with release `build/vjassc.exe`.
Representative artifacts:

- `build/input.phase19.final.fast.stats.json`
- `build/input.phase19.final.performance.json`
- `build/input.phase19.final.incremental.state.json`
- `build/input.phase19.final.incremental.report.json`
- `build/input.phase19.final.validate.stats.json`
- `build/input.phase19.final.validate.validation.json`
- `build/input.phase19.final.full.stats.json`
- `build/input.phase19.final.full.validation.json`

| Mode | Phase 18 total | Phase 19 target | Phase 19 total | Codegen | Syntax-lite | PJASS |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| fast | 6106 | <= 5200 | 5116 | 4486 | 0 | 0 |
| validate | 7870 | <= 7000 | 6901 | 4981 | 1012 | 349 |
| full-validation | 8892 | <= 8200 | 6599 | 4461 | 1019 | 301 |

Correctness:

| Check | Result |
| --- | --- |
| `ctest --test-dir build --output-on-failure` | pass |
| standalone fast compile | pass |
| validate syntax-lite | pass, issueCount 0 |
| validate PJASS | pass, groupedCount 0 |
| full-validation syntax-lite | pass, issueCount 0 |
| full-validation PJASS | pass, groupedCount 0 |
| full-validation reference presence | pass |
| read-only incremental no-change reuse | pass, 100% chunks reusable |

## Counter Results

| Metric | Phase 18 | Phase 19 | Phase 19 note |
| --- | ---: | ---: | --- |
| sourceMethods fast | 2278 | 2009 | improved, still main hotspot |
| sanitizeOutput fast | 244 | 74 | improved |
| syntaxLite validate | 1771 | 1012 | meets <= 1300 goal |
| functionOrdering fast | 446 | 256 | improved elapsed time |
| lineFeatureScans | 49364 | 49668 | roughly unchanged |
| functionOrderTokenScans | 56249 | 56698 | coverage work remains |
| arrayAccessRewriteAttempts | 12671 | 6313 | improved entry filtering/cache |
| arrayAccessRewriteChanged | 7351 | 5398 | cached changed rewrites are not recounted as fresh attempts |
| tokenCacheBuilds | 56277 | 56608 | roughly unchanged |
| tokenCacheHits | 108061 | 108636 | roughly unchanged |

Function dependency recorder:

| Counter | Phase 18 | Phase 19 |
| --- | ---: | ---: |
| recordedEdges | 2104 | 2148 |
| outputScanEdges | 16130 | 16232 |
| matchedEdges | 2104 | 2147 |
| missingRecordedEdges | 14026 | 14085 |
| extraRecordedEdges | 0 | 1 |
| weakExecuteFuncEdges | 0 | 94 |
| coveragePercent | 13.04 | 13.23 |

Dependency recorder coverage is now visible in stats and reports, but it did
not improve enough to remove the output-scan fallback. That fallback is still
required and intentionally preserved.

MethodPlan counters:

| Counter | Phase 19 |
| --- | ---: |
| built | 1540 |
| linesSkippedNoCandidate | 20617 |
| bareFieldRewriteAttempts | 12182 |
| bareFieldRewriteChanged | 9130 |
| shadowSkips | 23999 |

## War3Lib / Xlimon ALPHA

Validate command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Validate result: pass. The task selected vjassc output.

| Metric | Value |
| --- | ---: |
| vjassc elapsed | 6.93 seconds |
| target | <= 7.60 seconds |
| final task result | pass |

Fast compare command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Fast compare result: pass. The compare task selected JassHelper output for the
final map output while recording vjassc fast-mode timing.

| Metric | Value |
| --- | ---: |
| vjassc fast elapsed | 5.57 seconds |
| target | <= 5.80 seconds |
| jasshelper elapsed | 11.76 seconds |
| final task result | pass |

## Incremental And Parallel Notes

The Phase 19 speed gate was reached without enabling parallel lowering and
without using incremental output for compilation. New report-only CLI was added:

```text
--emit-performance-report <path>
--emit-incremental-state <path>
--emit-incremental-report <path>
--compare-incremental-state <path>
```

The final no-change report used `build/input.phase19.final.incremental.state.json`
as prior state and produced:

| Metric | Value |
| --- | ---: |
| chunkCount | 13255 |
| reusedChunks | 13255 |
| changedChunks | 0 |
| addedChunks | 0 |
| removedChunks | 0 |
| reusePercent | 100 |

Parallel lowering remains disabled because lambda/interface id allocation still
needs a deterministic ownership plan before output can be safely split.

## Deployment

The speed acceptance gate passed, so the release executable was deployed to:

```text
D:\War3\plugins\vjassc\vjassc.exe
```

`Get-FileHash` confirmed that the deployed executable matches
`D:\Project\JassChanger\build\vjassc.exe`.

SHA256:

```text
CF9D69E1CF5E4A288EB4EC72B27581A882D86FA40D364F6C3685CAC6FED0A5D0
```

## Remaining Work

- Build real dependency recorder coverage so the output scan can shrink safely.
- Keep parallel lowering experimental and disabled until lambda/interface id
  allocation is deterministic.
