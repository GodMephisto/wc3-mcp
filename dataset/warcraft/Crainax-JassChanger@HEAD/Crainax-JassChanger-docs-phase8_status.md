---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase8_status.md"
ref: HEAD
---

# Phase 8 Status

Date: 2026-05-09

## Implemented

- added dependency-aware final function ordering for generated JASS function blocks
- moved referenced lambda functions before their first non-cyclic uses
- split comma-separated local declarations and preserved initializer execution with follow-up `set` statements
- fixed Zinc inline `if (...) return/call/set` lowering by matching the real condition close paren
- added support for inline Zinc `break` and explicit `call` statements in simple lowering
- lowered indexed struct member access on local/global/generated struct arrays, including `arr[i].field`, `arr[i].method(...)`, `thistype` generated arrays, and `struct []` receivers such as `music[1002].play()`
- added syntax-lite residue buckets for comma locals, indexed struct members, inline Zinc controls, and forward function/lambda references
- expanded PJASS report examples with generated line text and current function name
- split PJASS categories for forward references, indexed-struct residue, inline-Zinc residue, comma locals, generated-helper references, and uninitialized variables
- added phase-8 golden fixtures for lambda ordering, indexed struct access, inline Zinc controls, and comma local declarations

## Verification Commands

Build and tests:

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

Full real-sample codegen and validation report:

```bat
build\vjassc.exe samples\input.j -o build\input.phase8.out.j --emit-stats build\input.phase8.codegen.stats.json --emit-validation-report build\input.phase8.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

PJASS validation run:

```bat
build\vjassc.exe samples\input.j -o build\input.phase8.pjass.out.j --emit-stats build\input.phase8.pjass.stats.json --emit-validation-report build\input.phase8.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

## Output Stats

`build/input.phase8.validation.json` reports:

```json
{
  "bytes": 4387328,
  "lines": 105645,
  "globalsBlocks": 1,
  "globalDeclarations": 3654,
  "natives": 549,
  "functions": 6119,
  "generatedLambdaFunctions": 838,
  "structSupportFunctions": 1639,
  "functionInterfaceWrappers": 61,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0,
  "hasMain": true,
  "hasConfig": true
}
```

## Syntax-Lite Result

`--check-output-syntax-lite` passes for `build/input.phase8.out.j`.

```json
{
  "ok": true,
  "issueCount": 0,
  "commaLocalResidues": 0,
  "indexedStructMemberResidues": 0,
  "inlineZincControlResidues": 0,
  "forwardFunctionReferences": 3,
  "forwardLambdaReferences": 0
}
```

The 3 forward function references are reported as non-fatal syntax-lite metadata because they are call cycles that cannot be solved by reordering alone.

## Init Validation

The validation report confirms `main`, `config`, struct init, function-interface init, and library init wiring. No init-validation issues were reported.

## PJASS Result

PJASS is executed and captured:

```json
{
  "requested": true,
  "ran": true,
  "ok": false,
  "exitCode": 1,
  "elapsedMs": 673,
  "stdoutPath": "build/input.phase8.pjass.validation.pjass.stdout.txt",
  "stderrPath": "build/input.phase8.pjass.validation.pjass.stderr.txt"
}
```

PJASS still fails, but the grouped report is much more actionable and includes generated output text plus the current function name for each example.

## PJASS Before/After

| Error Class | Phase 7 | Phase 8 |
| --- | ---: | ---: |
| other | 8358 | 1134 |
| syntaxError | 2800 | 708 |
| undefinedVariable | 763 | 656 |
| undefinedFunction | 687 | 159 |
| expectedEndfunction | 161 | 28 |
| localOrder | 29 | 0 |
| invalidComparison | 38 | 8 |
| returnMismatch | 67 | 118 |
| uninitializedVariable | not split | 330 |
| forwardFunctionReference | not split | 3 |
| duplicateDeclaration | 0 | 0 |

The broad PJASS grouped count dropped from 12903 to 3144. `other` dropped by about 86%, and the direct syntax blockers from comma locals, inline Zinc controls, and indexed struct members are no longer present in syntax-lite.

## First Remaining PJASS Blockers

1. Function-interface/code signature mismatch:
   examples include `Function vjlambda__108 must not take any arguments when used as code`.
2. Remaining chained method/member lowering:
   examples include `UIHashTable_uiHashTable(...).ui.bind(...)`, which needs another pass for method-chain receiver results.
3. Unresolved source symbols:
   examples include `uiHT`, `HASH_ABILITY`, and `HASH_TIMER`.
4. Uninitialized-variable diagnostics:
   examples currently involve method-chain values such as `UIHashTable_uiHashTable(frame).eventdata.get2()` that are not fully lowered.
5. True cyclic forward references:
   examples include `syncBus.onInit <-> onDataSync`, `CombineSession.buildSelector <-> runStage`, and `MopUpItemCreate <-> vjlambda__627`.

## Performance Baseline

`build/input.phase8.pjass.validation.json` reports:

```json
{
  "read": 67,
  "preprocess": 171,
  "staticIf": 269,
  "lex": 226,
  "parse": 260,
  "moduleExpand": 22,
  "codegen": 102711,
  "syntaxLite": 6923,
  "pjass": 673,
  "comparison": 346,
  "total": 111611
}
```

Phase 8 is slower than Phase 7 because dependency sorting and stronger syntax/PJASS analysis add full-output scans. Correctness still takes priority; the largest remaining cost is codegen.

## Known Limitations

- PJASS does not pass yet.
- Cyclic function references need a semantic strategy, not simple ordering.
- Method chains on returned struct instances remain incomplete.
- Function-interface callback signatures still need stricter adaptation.
- Source/environment symbols such as `HASH_ABILITY` and `HASH_TIMER` need provenance-backed resolution, not dummy declarations.

## Next Phase Suggestion

Phase 9 should focus on the largest remaining PJASS groups:

1. lower method-chain receiver results such as `foo().bar` and `arr[i].field.method()`
2. fix callback/code signature adaptation for lambdas with parameters
3. trace unresolved globals such as `uiHT`, `HASH_ABILITY`, and `HASH_TIMER` to source declarations or private rewrite gaps
4. classify/fix return mismatch after method-chain lowering reduces syntax cascades
5. decide how to represent true function cycles without dummy declarations
