---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase16_status.md"
ref: HEAD
---

# Phase 16 Status

Phase 16 focused on struct lowering performance without changing vJASS/Zinc
semantics. The implementation keeps PJASS green, adds finer performance
instrumentation, and reduces several lookup/scan counters, but it does not yet
meet the aggressive Phase 16 timing targets.

## Implemented

- Split `emitStructSupport` accounting into generated helpers, source methods,
  lifecycle support, and method body lowering.
- Added generated support counters; struct allocate/create/destroy/deallocate
  helpers are still emitted directly and do not enter generic body lowering.
- Added `GeneratedKind` metadata for generated struct support functions.
- Added `LineFeatures` and skip counters for line-level lowering gates.
- Narrowed the struct-wide static/member pass to explicit struct receiver
  candidates instead of scanning every struct for ordinary member access.
- Treated `.` as member syntax only when followed by an identifier, so real
  literals such as `1.0` do not force struct receiver lowering.
- Added an early no-bracket path for array access rewriting.
- Avoided lambda extraction scans on function/method bodies that cannot contain
  anonymous function syntax.
- Merged syntax-lite metrics collection into the main syntax-lite scan.
- Added Phase 16 regression fixtures for generated support, source method
  lowering, `onDestroy`, and direct `deallocate`.

## Standalone Results

Commands were run against `samples/input.j` with the release `build/vjassc.exe`.

| Metric | Phase 15 | Phase 16 | Delta |
| --- | ---: | ---: | ---: |
| standalone fast total | 20541 | 21296 | +755 |
| standalone fast codegen | 19955 | 20626 | +671 |
| standalone validate total | 23714 | 26449 | +2735 |
| standalone validate codegen | 20511 | 22863 | +2352 |
| standalone validate syntaxLite | 2200 | 2525 | +325 |
| standalone validate pjass | 298 | 373 | +75 |
| standalone full-validation total | 23065 | 26807 | +3742 |
| standalone full-validation codegen | 19584 | 22904 | +3320 |
| standalone full-validation comparison | 342 | 429 | +87 |
| structLookupCalls | 102092 | 90306 | -11786 |
| functionLookupCalls | 141163 | 125223 | -15940 |
| memberAccessScans | 17177 | 6420 | -10757 |

Correctness:

| Check | Result |
| --- | --- |
| `ctest --test-dir build --output-on-failure` | pass |
| validate syntax-lite | pass, 0 issues |
| validate init validation | pass, 0 issues |
| validate PJASS | pass, groupedCount 0 |
| full-validation syntax-lite | pass, 0 issues |
| full-validation PJASS | pass, groupedCount 0 |
| full-validation JassHelper reference | found |

## Phase 16 Pass Timings

Latest standalone fast artifact: `build/input.phase16.fast.stats.json`.

| Pass | ms |
| --- | ---: |
| emitFunctions | 18488 |
| emitStructSupport | 12934 |
| emitStructSupport.sourceMethods | 12848 |
| emitStructSupport.generatedHelpers | 0 |
| emitStructSupport.lifecycleSupport | 0 |
| lowerLambdas | 1786 |
| finalOutputValidationPrep | 2604 |
| sanitizeOutput | 1895 |
| functionOrdering | 708 |

Important counters:

| Counter | Value |
| --- | ---: |
| lineFeatureScans | 144792 |
| linesSkippedNoDotBracketCall | 96599 |
| linesSkippedNoCurrentStruct | 76388 |
| linesSkippedGeneratedSupport | 963 |
| generatedSupportLinesEmitted | 963 |
| generatedSupportLinesLowered | 0 |
| receiverChainAttempts | 4900 |
| receiverChainChanged | 417 |
| arrayAccessRewriteAttempts | 108677 |
| arrayAccessRewriteChanged | 7352 |
| functionOrderTokenScans | 208696 |
| functionOrderEdges | 32256 |
| functionOrderSccCount | 4 |

## War3Lib / Xlimon ALPHA

Validate command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result: pass. The task selected `Output/5_vjassc.j`, wrote
`Output/output.j`, and completed successfully.

Report summary from `Output/compiler_backend_report.json`:

| Metric | Phase 15 | Phase 16 |
| --- | ---: | ---: |
| vjassc internal total | 26601 | 23021 |
| vjassc internal codegen | 22641 | 19778 |
| jassCompilerMs | 27234 | 23611 |
| totalCompileMs | 37812 | 35066 |

The real ALPHA validate path improved but did not meet the Phase 16 target of
`totalCompileMs <= 32000`.

Compare command was also run with `WAR3_VJASSC_MODE=fast`, but the current
War3Lib `TaskCompileCompareAlpha.lua` still hardcodes `path.vjasscMode =
"full-validation"`. The report therefore recorded full-validation:

| Metric | Value |
| --- | ---: |
| jasshelper elapsedMs | 14263 |
| vjassc elapsedMs | 23013 |
| jassCompilerMs | 37765 |
| totalCompileMs | 51817 |

## Notes

- The useful changes are the receiver-candidate narrowing and lookup reduction:
  function and struct lookups are lower, and member access scans dropped by more
  than 60%.
- The ineffective part is that source method body lowering is still the dominant
  cost. Even after the scan reductions, `emitStructSupport.sourceMethods`
  remains around 12-13 seconds in the standalone run.
- The next optimization should avoid regex-based current-struct field/method
  rewriting inside source method bodies, likely by replacing that path with a
  token-level member rewriter and by caching per-method local/field plans.
