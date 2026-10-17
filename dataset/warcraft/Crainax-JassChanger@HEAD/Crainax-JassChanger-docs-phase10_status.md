---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase10_status.md"
ref: HEAD
---

# Phase 10 Status

Phase 10 targeted the remaining real-sample PJASS blockers after Phase 9. PJASS does not pass yet, but the grouped error count is now below the phase hard target without adding dummy declarations or deleting source logic.

## Implemented

- Struct-returning receiver chains now run after protected string/rawcode/comment splitting, so generated calls such as `create().layout(...)` and nested argument chains lower without corrupting string arguments.
- Zinc body normalization now joins multi-line argument lists and `+` expression continuations before simple-statement lowering, preventing stray generated statements such as `call + "..."`.
- Leading-dot continuation tracking now handles bare struct receivers with indexed expressions and avoids emitting the receiver-only line when the next source line continues the chain.
- Generated struct support functions (`allocate`, default `create`, `destroy`) are registered as typed return sources for receiver-chain lowering.
- Local/global struct receiver array matching was narrowed so one argument cannot greedily consume following comma-separated arguments.
- Discarded field-call statements shaped like `call field[method(...)]` are normalized back to ordinary method calls.
- Explicit function-interface references such as `AfterBuffTime.ABB81` and `TaozhuangGet.tg_tz2` are lowered when they appear inside ordinary call arguments.
- Repeated legacy struct-prefix rewriting now avoids double-prefixing names such as same-name library/struct outputs.
- Added a Phase 10 golden fixture covering Zinc method-chain continuations, `+` continuations, and explicit function-interface argument lowering.

## Validation Commands

The local PowerShell environment needed Visual Studio variables for C++ rebuilds, so build and test were run through `vcvars64.bat`:

```bat
cmd.exe /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat`" >nul && cmake --build build"
cmd.exe /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat`" >nul && ctest --test-dir build --output-on-failure"
```

Real input syntax-lite:

```bat
build\vjassc.exe samples\input.j -o build\input.phase10.out.j --emit-stats build\input.phase10.codegen.stats.json --emit-validation-report build\input.phase10.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

Real input PJASS:

```bat
build\vjassc.exe samples\input.j -o build\input.phase10.pjass.out.j --emit-stats build\input.phase10.pjass.stats.json --emit-validation-report build\input.phase10.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

The PJASS command still exits non-zero because PJASS blockers remain; the validation report is generated and is the phase-10 measurement source.

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

Initialization validation has no issues. `main` calls struct, function-interface, and library init helpers in the expected order. The generated output currently has 106641 lines, 6309 functions, 838 generated lambda functions, and 251 function-interface wrappers.

## PJASS Delta

Phase 9 latest grouped PJASS count: `889`.

Phase 10 latest grouped PJASS count: `360`.

| Group | Phase 9 | Phase 10 |
| --- | ---: | ---: |
| syntaxError | 303 | 11 |
| undefinedVariable | 201 | 150 |
| other | 166 | 99 |
| methodChainReceiverResidue | 64 | 0 |
| returnMissingValue | 50 | 48 |
| callbackCodeSignatureMismatch | 30 | 30 |
| uninitializedVariable | 27 | 1 |
| undefinedFunction | 19 | 8 |
| expectedEndfunction | 11 | 1 |
| unresolvedEnvironmentSymbol | 9 | 9 |
| invalidComparison | 6 | 0 |
| forwardFunctionReference | 3 | 3 |

The result meets the phase-10 hard target:

- grouped PJASS count `360 <= 500`
- `undefinedFunction == 8 <= 10`
- `syntaxError == 11 <= 150`
- `expectedEndfunction == 1 <= 5`
- `localOrder == 0`

It does not meet the stretch target of PJASS pass or grouped count `<= 200`.

## Remaining Blockers

- `undefinedVariable` remains the largest group. The top examples include source/environment or rewrite-provenance cases such as `origin`/`keyId` in generated item ability helpers and `YDHT` helper globals.
- `other` remains mostly semantic conversion errors, including integer/boolean mismatches and code/function-interface conversion gaps.
- `returnMissingValue` remains in source functions with non-void return types and not all paths returning a value. Phase 10 did not add unsafe default returns to source functions.
- `callbackCodeSignatureMismatch` remains where parameterized lambdas are still passed to raw `code` callback positions.
- `unresolvedEnvironmentSymbol` remains for environment globals such as `yd_NullTempGroup`, `yd_MapMaxX`, and `yd_MapMinX`.
- Three true forward-cycle candidates remain: SyncBus `onInit` to `onDataSync`, CombineSession `buildSelector` to `runStage`, and MopUpItem lambda to `MopUpItemCreate`.

## Performance

Latest full PJASS run timing:

| Timing | Phase 9 ms | Phase 10 ms |
| --- | ---: | ---: |
| read | 42 | 40 |
| preprocess | 105 | 103 |
| staticIf | 174 | 188 |
| lex | 162 | 151 |
| parse | 168 | 159 |
| moduleExpand | 12 | 10 |
| codegen | 109463 | 102767 |
| syntaxLite | 7970 | 7427 |
| pjass | 519 | 352 |
| comparison | 404 | 344 |
| total | 118987 | 111511 |

Largest Phase 10 codegen pass timings:

```json
{
  "emitFunctions": 90521,
  "emitStructSupport": 69413,
  "finalOutputValidationPrep": 11854,
  "sanitizeOutput": 8861,
  "lowerLambdas": 8978,
  "functionOrdering": 2993,
  "emitGlobals": 365
}
```

Phase 10 stayed correctness-focused and did not do a broad performance rewrite. The measured total time improved mainly because fewer malformed outputs reached PJASS and fewer function-interface targets were left unresolved.

## PJASS Result

PJASS has not passed yet.

Current exact grouped count: `360`.

Recommended Phase 11 order:

1. Add provenance reporting and real generation rules for the remaining `undefinedVariable` / environment-symbol groups without dummy declarations.
2. Separate and fix integer/boolean conversion errors in the `other` group.
3. Implement safe raw `code` callback adapters only where the event context or interface dispatch source is known.
4. Address missing returns only for compiler-generated wrappers first; do not add default returns to source functions without confirming legacy behavior.
5. Handle the three true forward cycles with signature-aware bridges or leave them explicitly classified.
