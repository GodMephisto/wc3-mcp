---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase11_status.md"
ref: HEAD
---

# Phase 11 Status

Phase 11 targeted the remaining high-volume PJASS blocker groups from Phase 10 without adding dummy declarations or silently patching source semantics. PJASS still does not pass, but the grouped error count is now under the phase fallback target and the real-sample validation path is below the 90 second performance line.

## Implemented

- Zinc one-line functions with body text on the header line are now parsed with their inline body, preventing following functions from inheriting the wrong body.
- Non-Zinc `library` / `library_once` globals without explicit access are preserved as public JASS-style globals, matching YDWE-style helper globals such as `YDHT` instead of rewriting them as private library names.
- Zinc continuation joining now handles trailing/leading logical operators (`and`, `or`, `&&`, `||`) and keeps leading-dot chains alive across comment-only lines.
- Struct receiver lowering now uses balanced scanning for local/global generated struct receivers instead of greedy regular expressions, including multi-dimensional receivers such as `field[this][i].destroy()`.
- Bare `destroy()` inside instance methods is lowered with a scanner rather than a broad regex, so member calls such as `border.destroy()` do not trigger regex stack failures.
- Struct-typed Zinc parameters keep their original source type after signature rewriting, which allows member access like `sd.owner` and `dd.isExist()` to lower.
- Static field replacement now respects locals/params with the same name, avoiding accidental static-field shadow rewrites.
- Validation reporting now records Phase 11 PJASS grouped totals, symbol provenance buckets, callback-adapter counts, forward-cycle candidates, and pass-level performance hotspots.
- Added offline validation helpers: `--analyze-pjass-log`, `--validate-existing-output`, and `--emit-pjass-examples`.
- Added Phase 11 golden fixtures for environment globals, boolean interface IDs, raw code callbacks, wrapper returns, source missing-return preservation, ExecuteFunc init bridging, and environment-symbol reporting.

## Validation Commands

Build and tests were run through Visual Studio environment initialization:

```bat
cmd.exe /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build"
cmd.exe /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && ctest --test-dir build --output-on-failure"
```

Real input syntax-lite:

```bat
build\vjassc.exe samples\input.j -o build\input.phase11.out.j --emit-stats build\input.phase11.codegen.stats.json --emit-validation-report build\input.phase11.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --emit-pjass-examples 30
```

Real input PJASS:

```bat
build\vjassc.exe samples\input.j -o build\input.phase11.pjass.out.j --emit-stats build\input.phase11.pjass.stats.json --emit-validation-report build\input.phase11.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --emit-pjass-examples 30
```

The PJASS command exits non-zero because PJASS blockers remain; the report is still generated and is the Phase 11 measurement source.

## Syntax And Init

Latest real-sample syntax-lite result:

```json
{
  "syntaxLite.ok": true,
  "issueCount": 0,
  "methodChainCallResultResidues": 0,
  "commaLocalResidues": 0,
  "indexedStructMemberResidues": 0,
  "inlineZincControlResidues": 0,
  "forwardLambdaReferences": 0,
  "forwardFunctionReferences": 3,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0
}
```

Initialization validation has no issues. `main` calls struct, function-interface, and library init helpers in order. The generated output currently has 106647 lines, 6311 functions, 838 generated lambda functions, and 251 function-interface wrappers.

## PJASS Delta

Phase 10 latest grouped PJASS count: `360`.

Phase 11 latest grouped PJASS count: `97`.

| Group | Phase 10 | Phase 11 |
| --- | ---: | ---: |
| undefinedVariable | 150 | 8 |
| booleanIntegerMismatch | 66 | 0 |
| returnMissingValue | 48 | 44 |
| callbackCodeSignatureMismatch | 30 | 30 |
| arrayIndexMissing | 19 | 0 |
| other | 14 | 8 |
| syntaxError | 11 | 2 |
| undefinedFunction | 8 | 1 |
| unresolvedEnvironmentSymbol | 9 | 0 |
| unresolvedKnownSourceSymbol | 0 | 1 |
| forwardFunctionReference | 3 | 3 |
| expectedEndfunction | 1 | 0 |
| uninitializedVariable | 1 | 0 |

The result meets the Phase 11 fallback target:

- grouped PJASS count `97 <= 100`
- syntax-lite issue count `0`
- init validation issues `0`
- duplicate function/global/native names `0`
- total validation time `85705 ms <= 90000 ms`

It does not meet the stretch target of a full PJASS pass.

## Remaining Blockers

- `returnMissingValue` remains in source functions where not all control-flow paths return a value. Phase 11 intentionally does not synthesize default returns for source functions.
- `callbackCodeSignatureMismatch` remains where parameterized lambdas are passed to raw `code` callback positions without a known event-context adapter.
- `other` contains remaining call/statement shape issues such as `Call expected instead of set`.
- `undefinedVariable` is now small and mostly source/provenance cases such as `heroData`/`Set`.
- Three forward function references remain: SyncBus `onInit` to `onDataSync`, CombineSession `buildSelector` to `runStage`, and MopUpItem lambda to `MopUpItemCreate`.
- `InitTrig_japi` remains an external map-init symbol outside the generated source.

## Performance

Latest full PJASS run timing:

| Timing | Phase 10 ms | Phase 11 ms |
| --- | ---: | ---: |
| codegen | 102767 | 76563 |
| syntaxLite | 7427 | 7711 |
| pjass | 352 | 389 |
| comparison | 344 | 352 |
| total | 111511 | 85705 |

Largest Phase 11 codegen pass timings:

```json
{
  "emitFunctions": 64135,
  "emitStructSupport": 52253,
  "finalOutputValidationPrep": 11957,
  "sanitizeOutput": 9066,
  "lowerLambdas": 6627,
  "functionOrdering": 2891,
  "emitGlobals": 445
}
```

The main performance improvement came from skipping local/global struct receiver scans on lines without member access and avoiding the previous regex-heavy receiver path.

## PJASS Result

PJASS has not passed yet.

Current exact grouped count: `97`.

Recommended next order:

1. Add safe raw `code` callback adapters only for callback contexts whose event payload is known.
2. Decide a source-policy for missing returns by comparing legacy JassHelper behavior before emitting defaults.
3. Lower the remaining `Call expected instead of set` cases.
4. Handle the three true forward references with signature-aware bridge generation.
