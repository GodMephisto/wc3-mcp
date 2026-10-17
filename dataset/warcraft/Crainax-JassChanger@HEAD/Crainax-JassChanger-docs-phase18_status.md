---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase18_status.md"
ref: HEAD
---

# Phase 18 Status

Phase 18 added explicit body-mode routing and shared scan caches for the
remaining codegen hot paths. The implementation keeps the real sample PJASS
green, keeps War3Lib/Xlimon ALPHA compiling, and meets the Phase 18 minimum
speed gate used for deployment.

## Implemented

- Added explicit `BodyMode` routing for Zinc, JASS-like, and generated bodies.
- Added BodyMode, BodyLowerer, TokenCache, mode fast-path, and function
  dependency recorder counters to `--emit-stats`.
- Reused a line token cache across feature checks and function-order dependency
  scans instead of rescanning the same lines from scratch.
- Stored token spans instead of copied token text in the line token cache.
- Narrowed dotted member matching so ordinary dotted symbols do not enter the
  current-struct rewrite path unless they can refer to the current struct.
- Lowered generated support bodies through a generated fast path and skipped
  generic source-body lowering for those lines.
- Reworked function-ordering dependency refresh so only dirty and newly
  appended generated blocks need an output dependency scan.
- Tightened array access rewrite entry so attempts are counted only for known
  array receivers.
- Replaced the protected-output operator normalizer with a manual scanner.
- Added transparent string-view lookup for symbol replacement maps.
- Enabled MSVC Release LTCG (`/GL` and `/LTCG`) for the local release build.
- Added Phase 18 body-mode fixtures for Zinc functions, JASS-like functions,
  Zinc methods, and generated support bodies.

## Standalone Results

Commands were run against `samples/input.j` with release `build/vjassc.exe`.
Representative artifacts:

- `build/input.phase18.final3.fast.stats.json`
- `build/input.phase18.final3.validate.stats.json`
- `build/input.phase18.final3.full-validation.stats.json`

| Mode | Phase 17 total | Phase 18 total | Phase 18 target | Phase 18 codegen |
| --- | ---: | ---: | ---: | ---: |
| fast | 7323 | 6106 | <= 6500 | 5377 |
| validate | 8862 | 7870 | <= 8200 | 5215 |
| full-validation | 9558 | 8892 | <= 9000 | 5773 |

Correctness:

| Check | Result |
| --- | --- |
| `ctest --test-dir build --output-on-failure` | pass |
| standalone fast compile | pass |
| validate syntax-lite | pass |
| validate PJASS | pass, groupedCount 0 |
| full-validation syntax-lite | pass |
| full-validation PJASS | pass, groupedCount 0 |

## Counter Results

| Metric | Phase 17 | Phase 18 | Phase 18 minimum |
| --- | ---: | ---: | ---: |
| lineFeatureScans | 96847 | 49364 | <= 70000 |
| functionOrderTokenScans | 110035 | 56249 | <= 80000 |
| arrayAccessRewriteAttempts | 18277 | 12671 | <= 15000 |
| functionLookupCalls | 20270 | 20174 | <= 18000 |
| structLookupCalls | 30501 | 18696 | <= 26000 |
| sourceMethods fast | 2171 | 2278 | <= 2000 |
| sanitizeOutput fast | 1427 | 244 | <= 1000 |

The phase speed gate was accepted on total timings and the main scan counters.
`sourceMethods` is still around 2.2 seconds and remains the main follow-up
hotspot. `functionLookupCalls` also missed the auxiliary counter line, but it
no longer dominates the elapsed time.

## Body Routing Counters

From `build/input.phase18.final3.fast.stats.json`:

| Counter | Value |
| --- | ---: |
| bodyModes.zincFunctions | 3498 |
| bodyModes.jassLikeFunctions | 920 |
| bodyModes.zincMethods | 1540 |
| bodyModes.jassLikeMethods | 0 |
| bodyModes.generatedBodies | 86 |
| bodyLowerer.generatedLinesSkippedGenericLowering | 963 |
| modeFastPath.heavyLoweringAvoidedByMode | 49385 |

Token cache reuse:

| Counter | Value |
| --- | ---: |
| tokenCacheBuilds | 56277 |
| tokenCacheHits | 108061 |
| featureScansAvoided | 54357 |
| functionOrderScansAvoided | 49411 |

Function dependency recorder:

| Counter | Value |
| --- | ---: |
| recordedEdges | 2104 |
| outputScanEdges | 16130 |
| matchedEdges | 2104 |
| missingRecordedEdges | 14026 |
| extraRecordedEdges | 0 |

The recorder is now useful for generated/new block refreshes, but it does not
yet replace most output-scan edges. That is intentional for Phase 18 because the
code keeps the conservative output scan where coverage is incomplete.

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
| vjassc elapsedMs | 8216 |
| vjassc internal total | 8125 |
| vjassc internal codegen | 5400 |
| syntaxLite | 1771 |
| PJASS | 271 |
| sourceMethods | 2202 |

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
| jasshelper elapsedMs | 11943 |
| vjassc fast elapsedMs | 6435 |
| vjassc fast internal total | 6345 |
| vjassc fast codegen | 5455 |
| sourceMethods | 2366 |
| functionOrdering | 446 |

Copied reports:

- `build/xlimon.phase18.final.validate.compiler_backend_report.json`
- `build/xlimon.phase18.final.validate.vjassc.stats.json`
- `build/xlimon.phase18.final.fast_compare.compiler_backend_report.json`
- `build/xlimon.phase18.final.fast_compare.vjassc.stats.json`

## Deployment

The speed acceptance gate passed, so the release executable was deployed to:

```text
D:\War3\plugins\vjassc\vjassc.exe
```

`Get-FileHash` confirmed that the deployed executable matches
`D:\Project\JassChanger\build\vjassc.exe`.

## Next Phase

- Continue reducing `sourceMethods` with deeper method-body rewrite caches or a
  more direct current-struct field/method lowering path.
- Increase function dependency recorder coverage before removing more output
  scans.
- Revisit function lookup counters only after the remaining body-lowering cost
  is lower; the current elapsed-time bottleneck is no longer broad lookup.
