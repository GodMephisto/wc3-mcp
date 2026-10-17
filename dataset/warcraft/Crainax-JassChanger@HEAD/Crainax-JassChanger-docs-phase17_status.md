---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase17_status.md"
ref: HEAD
---

# Phase 17 Status

Phase 17 focused on the second performance pass for `Phase1Codegen`, War3Lib
mode routing, and keeping fast mode honest about invalid source/output shapes.
The final run keeps PJASS green, makes `123` fail instead of being silently
accepted, and reaches the Phase 17 timing targets.

## Implemented

- Added `CodegenOptions::fastMode` and stopped charging fast mode with
  `finalOutputValidationPrep`.
- Replaced the `InitTrig_*` missing-call regex pass with direct line scanning.
- Avoided broad function argument lookup for callees that cannot require
  function-interface/function-value lowering.
- Reduced struct/function lookup hot paths by using existing name indexes,
  member-hit line features, and small caches instead of repeated lookup calls.
- Narrowed current-struct field/method rewrite entry with line features,
  actual member hit lists, and cheap `this` / `thistype` / `.` guards.
- Counted array rewrite attempts only when a known array receiver is actually
  encountered.
- Added a function-ordering prefilter so dependency token scans skip lines
  without calls or `function` references.
- Removed a redundant no-expected-function-value lowering path and kept a
  second struct-rewrite pass only when a prior rewrite exposes generated member
  chains.
- Added fast-mode statement-shape validation and parser errors for bare numeric
  Zinc members, so a stray `123` is no longer silently accepted.
- Updated War3Lib vjassc tasks so `WAR3_VJASSC_MODE=fast` reaches compare mode
  instead of being forced back to `full-validation`.

## Standalone Results

Commands were run against `samples/input.j` with release `build/vjassc.exe`.
The baseline files are the Phase 17 pre-edit captures.

| Mode | Baseline total | Phase 17 total | Baseline codegen | Phase 17 codegen |
| --- | ---: | ---: | ---: | ---: |
| fast | 18410 | 7323 | 17786 | 6572 |
| validate | 20697 | 8862 | 17612 | 6263 |
| full-validation | 21536 | 9558 | 18062 | 6636 |

Correctness:

| Check | Result |
| --- | --- |
| `ctest --test-dir build --output-on-failure` | pass |
| validate syntax-lite | pass, 0 issues |
| validate init validation | pass, 0 issues |
| validate PJASS | pass, groupedCount 0 |
| full-validation syntax-lite | pass, 0 issues |
| full-validation init validation | pass, 0 issues |
| full-validation PJASS | pass, groupedCount 0 |
| bare `123` JASS statement fixture | rejected |
| bare `123` Zinc member fixture | rejected |

## Counters Delta

| Metric | Baseline | Phase 17 |
| --- | ---: | ---: |
| arrayAccessRewriteAttempts | 108677 | 18277 |
| arrayAccessRewriteChanged | 7352 | 7351 |
| functionLookupCalls | 125223 | 20270 |
| structLookupCalls | 90306 | 30501 |
| functionOrderTokenScans | 208696 | 110035 |
| lineFeatureScans | 144792 | 96847 |

## Top Pass Timings

Latest representative artifacts:

- `build/input.phase17.final5.fast.stats.json`
- `build/input.phase17.final5.validate.stats.json`
- `build/input.phase17.final5.full-validation.stats.json`

| Mode | sourceMethods | finalPrep | sanitize | functionOrdering |
| --- | ---: | ---: | ---: | ---: |
| fast | 2171 | 0 | 1427 | 532 |
| validate | 2070 | 1907 | 1406 | 501 |
| full-validation | 2124 | 2124 | 1559 | 565 |

## War3Lib / Xlimon ALPHA

The temporary `123` inserted in `edit/hero/data/HeroCondition.j` was removed
after confirming both jasshelper and vjassc catch it. With the corrected source,
ALPHA builds pass again.

Validate command:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
set WAR3_VJASSC_MODE=validate
lua Lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Validate result: pass. The task selected `Output/5_vjassc.j` and wrote
`Output/output.j`.

| Metric | Value |
| --- | ---: |
| vjassc elapsedMs | 9665 |
| vjassc internal total | 9577 |
| vjassc internal codegen | 6934 |
| sourceMethods | 2308 |
| syntaxLite | 1738 |
| PJASS | 267 |
| totalCompileMs | 19633 |

Fast compare command:

```bat
set WAR3_VJASSC_MODE=fast
lua Lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Fast compare result: pass. The compare task recorded real fast-mode vjassc
timing and selected JassHelper output for the final map output.

| Metric | Value |
| --- | ---: |
| jasshelper elapsedMs | 11514 |
| vjassc fast elapsedMs | 7490 |
| vjassc fast internal total | 7410 |
| vjassc fast codegen | 6695 |
| sourceMethods | 2249 |
| jassCompilerMs | 19383 |
| totalCompileMs | 28841 |

Copied reports:

- `build/xlimon.phase17.final.validate.compiler_backend_report.json`
- `build/xlimon.phase17.final.validate.vjassc.stats.json`
- `build/xlimon.phase17.final.fast_compare.compiler_backend_report.json`
- `build/xlimon.phase17.final.fast_compare.vjassc.stats.json`

## Answers

- standalone fast now meets the good target: `7323 ms`, below both `17000 ms`
  and `14000 ms`.
- War3Lib ALPHA fast is now faster than JassHelper in compare mode:
  `7490 ms` vs `11514 ms`.
- validate mode is usable as a daily candidate: ALPHA validate compiled, selected
  vjassc output, and standalone validate/full-validation paths are PJASS green.
- Remaining hotspots are now smaller: sanitize/output ordering and line-feature
  scans dominate more than source method lowering. `sourceMethods` is down to
  about 2.1-2.3 seconds in the representative runs.

## Next Phase

- Replace the remaining line-feature broad scan with a token cache that can be
  reused across expression and statement lowering.
- Move required sanitize work closer to generation so fast mode can reduce the
  remaining `sanitizeOutput` cost without weakening output legality.
- Keep fast mode's statement-shape guard; it is cheap enough and prevents
  invalid source like a bare `123` from being treated as a valid compile.
