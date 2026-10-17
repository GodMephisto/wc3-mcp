---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase12_status.md"
ref: HEAD
---

# Phase 12 Status

Phase 12 targeted PJASS convergence and the first bounded performance pass. PJASS still does not pass, but the real-sample grouped error count is down to 21 and the full validation run is now below the 60 second acceptance line.

## Implemented

- PJASS log parsing now handles Windows drive paths and can infer the generated output for offline `--analyze-pjass-log` runs.
- PJASS grouping now skips summary lines, de-duplicates repeated raw callback messages by generated call site, and writes Phase 12 triage buckets with provenance and suggested fix classes.
- Zinc statement lowering now distinguishes top-level assignments from function calls whose string literals contain `=`, preventing accidental `set BJDebugMsg(...)` output.
- Zinc continuations now handle arithmetic operators (`+`, `-`, `*`, `/`) in addition to the earlier logical and member-chain cases.
- Inline `} else if` splitting and `else if` lowering now preserve the conditional tail instead of gluing it into malformed `elseif` text.
- Array-backed struct receivers such as `Row[pos]` are recognized as generated integer receivers before member rewrite.
- Struct `deallocate` support is emitted only for structs that use it; `destroy` calls the generated deallocator when needed, and bare instance `deallocate()` lowers inside methods.
- Generated lambdas with non-`nothing` returns append a safe default return only when the generated body does not already end with a return. Source functions are not silently patched.
- Function-interface expected parameter types are propagated through static method arguments and recursive function-call argument lowering, reducing nested UI callback leakage.
- Performance instrumentation now reports codegen counters in both stats and performance JSON output.
- Hot regex work was reduced through regex caching, a fast path for lines without boolean/operator forms, output string reservation, and a manual identifier scanner for function ordering.
- Added Phase 12 golden fixtures for string-equals calls, arithmetic continuations, inline `else if`, array struct receivers, struct `deallocate`, generated lambda default returns, and method function-interface parameters.

## Validation Commands

Build and golden fixtures:

```bat
cmd.exe /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

Real input PJASS validation:

```bat
build\vjassc.exe samples\input.j -o build\input.phase12.pjass.out.j --emit-stats build\input.phase12.pjass.stats.json --emit-validation-report build\input.phase12.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --emit-pjass-examples 50
```

The PJASS command exits non-zero because PJASS blockers remain; the report is still generated and is the Phase 12 measurement source.

## Syntax And Init

Latest real-sample syntax-lite and init result:

```json
{
  "syntaxLite.ok": true,
  "issueCount": 0,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0,
  "init.issues": 0
}
```

Generated output metrics:

```json
{
  "lines": 115010,
  "functions": 6605,
  "generatedLambdaFunctions": 838,
  "structSupportFunctions": 1641,
  "functionInterfaceWrappers": 543
}
```

## PJASS Delta

Phase 11 baseline grouped PJASS count: `97`.

Phase 12 latest grouped PJASS count: `21`.

| Group | Phase 11 | Phase 12 |
| --- | ---: | ---: |
| returnMissingValue | 44 | 0 |
| callbackCodeSignatureMismatch | 30 | 15 |
| other | 8 | 0 |
| undefinedVariable | 8 | 0 |
| unresolvedKnownSourceSymbol | 1 | 0 |
| undefinedFunction | 1 | 1 |
| forwardFunctionReference | 3 | 5 |

The result meets the Phase 12 minimum acceptance line:

- grouped PJASS count `21 <= 30`
- syntax-lite issue count `0`
- init validation issues `0`
- duplicate function/global/native names `0`
- total validation time `57559 ms <= 60000 ms`

It does not meet the stretch target of a full PJASS pass.

## Remaining Blockers

- `callbackCodeSignatureMismatch`: 15 unique generated call sites still pass parameterized lambdas to raw `code` positions without a proven event/context adapter.
- `forwardFunctionReference`: 5 remaining references require signature-aware bridge generation or source-order handling:
  - `s__SyncBus_syncBus_onInit -> s__SyncBus_syncBus_onDataSync`
  - `s__Pet_CombineSession_buildSelector -> s__Pet_CombineSession_runStage`
  - `vjlambda__627 -> MopUpItem_MopUpItemCreate`
  - `DadiCarMovingHuanying -> CreateDadiCar`
  - `vjlambda__677 -> DadiCarMoving`
- `undefinedFunction`: `InitTrig_japi` remains an external map-init/environment symbol, not a generated source function.

## Performance

Latest full PJASS run timing:

| Timing | Phase 11 ms | Phase 12 ms |
| --- | ---: | ---: |
| codegen | 76563 | 48084 |
| syntaxLite | 7711 | 8187 |
| pjass | 389 | 343 |
| comparison | 352 | 336 |
| total | 85705 | 57559 |

Largest Phase 12 codegen pass timings:

```json
{
  "emitFunctions": 45750,
  "emitStructSupport": 35972,
  "lowerLambdas": 4777,
  "finalOutputValidationPrep": 1890,
  "sanitizeOutput": 1892,
  "functionOrdering": 291,
  "emitGlobals": 133
}
```

Performance counters from the latest run:

```json
{
  "linesVisited": 78442,
  "regexCalls": 0,
  "memberAccessScans": 17265,
  "structLookupCalls": 139617,
  "functionLookupCalls": 239115,
  "cachedRewriteHits": 361626,
  "cachedRewriteMisses": 17106
}
```

The largest win came from replacing the regex-heavy function-order dependency scan with a manual identifier scanner and avoiding expensive normalization paths on impossible lines.

## PJASS Result

PJASS has not passed yet.

Current exact grouped count: `21`.

Recommended next order:

1. Add signature-aware forward bridges for the five remaining forward references.
2. Resolve `InitTrig_japi` by documenting or loading the correct map environment source instead of emitting a fake declaration.
3. Add context-aware raw `code` adapters only for the 15 callback sites whose event payloads can be proven.
