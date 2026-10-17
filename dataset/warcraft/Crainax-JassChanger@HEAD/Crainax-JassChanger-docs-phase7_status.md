---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase7_status.md"
ref: HEAD
---

# Phase 7 Status

Date: 2026-05-09

## Implemented

- added grouped PJASS error examples to the validation report, including generated-output excerpts
- stripped block comments before parser/directive processing while preserving strings and rawcodes
- parsed fixed array dimensions in type refs and field declarations
- lowered fixed and multi-dimensional global/local arrays to JASS `array` declarations
- flattened multi-dimensional array access for globals, locals, static fields, and struct instance fields
- widened Zinc global parsing to custom type declarations such as `mopupData MDXiaoan[6][3]`
- fixed `library_once` duplicate global emission by emitting sorted libraries once
- kept SymbolTable public rewrites off dotted member names so `thistype.onInit` is not rewritten as an unrelated public function
- carried library/scope context into extracted lambdas so private globals are renamed and flattened there too
- normalized generated struct support references such as `si__`, `s__`, and `sc__` against the collected canonical struct name
- lowered bare instance field and method references inside struct methods with an implicit `this`
- expanded syntax-lite checks for duplicate generated names, block-comment residue, chained array indexing, and output-source residue
- added phase-7 golden fixtures for comment filtering, multi-dimensional arrays, private lambda rewriting, and bare struct members

## Verification Commands

Build and tests:

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

Full real-sample codegen and validation report:

```bat
build\vjassc.exe samples\input.j -o build\input.phase7.out.j --emit-stats build\input.phase7.codegen.stats.json --emit-validation-report build\input.phase7.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

PJASS validation run:

```bat
build\vjassc.exe samples\input.j -o build\input.phase7.pjass.out.j --emit-stats build\input.phase7.pjass.stats.json --emit-validation-report build\input.phase7.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

## Output Stats

`build/input.phase7.validation.json` reports:

```json
{
  "bytes": 4356014,
  "lines": 105622,
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

`--check-output-syntax-lite` passes for `build/input.phase7.out.j`.

The report shows:

```json
{
  "ok": true,
  "issueCount": 0,
  "residualSourceForms": []
}
```

The remaining front-end warnings are source diagnostics rather than syntax-lite failures:

```text
unknown character warnings around samples/input.j:16216-16222
duplicate library_once 'YDTriggerSaveLoadSystem' ignored
duplicate native 'DzFrameGetName' ignored
duplicate native 'DzIsWindowActive' ignored
```

## Init Validation

The validation report still confirms `main`, `config`, struct init, function-interface init, and library init wiring. No init-validation issues were reported in the phase-7 validation run.

## PJASS Result

PJASS is executed and captured:

```json
{
  "requested": true,
  "ran": true,
  "ok": false,
  "exitCode": 1,
  "elapsedMs": 2870,
  "stdoutPath": "build/input.phase7.pjass.validation.pjass.stdout.txt",
  "stderrPath": "build/input.phase7.pjass.validation.pjass.stderr.txt"
}
```

`stderr` is empty. `stdout` starts by successfully parsing `common.j` and `blizzard.j`.

## PJASS Before/After

| Error Class | Phase 6 | Phase 7 |
| --- | ---: | ---: |
| duplicateDeclaration | 69 | 0 |
| invalidArraySyntax | many syntax cascades | 0 known syntax-lite residuals |
| syntaxError | 2768 | 2800 |
| other | 12515 | 8358 |
| undefinedVariable | 1973 | 763 |
| returnMismatch | 63 | 67 |
| undefinedFunction | 499 | 687 |
| expectedEndfunction | 153 | 161 |
| localOrder | 9 | 29 |
| invalidComparison | not split out | 38 |
| unknownType | cascaded | 0 in summary |

Phase 7 removed the declaration and multi-dimensional array blockers that prevented useful triage. PJASS still does not pass because the next blocker set is now exposed.

## First Remaining PJASS Blockers

1. Function declaration ordering and forward references:
   `sc__BaseAnim_baseanim_onDestroy` calls methods such as `s__BaseAnim_baseanim_isExist` and `s__BaseAnim_baseanim_delDelay` before PJASS has seen their definitions.
2. Lambda declaration ordering:
   generated functions such as `vjlambda__1` are referenced before their emitted definitions.
3. Nested struct member access after array/index lowering:
   examples like `s__BaseAnim_baseanim_DList[...].dID` still need a second pass that lowers the field access on the indexed struct instance.
4. Inline Zinc return forms:
   statements like `if (width > 0) return DzGetMouseXRelative()` currently lower to invalid JASS.
5. Comma-separated local declarations:
   PJASS still reports local-order/syntax issues around lines such as `local integer parent,child`.
6. Remaining unresolved symbols:
   top examples include `aby`/`acy`, `HASH_ABILITY`, `HASH_TIMER`, and generated helper references that require either declaration-order fixes or additional public/private lowering.

## JassHelper Comparison

The phase-7 run still uses `samples/output_jasshelper.j` for structure comparison. The generated output remains smaller and is not byte-for-byte compatible. The useful phase-7 signal is now duplicate generated names at zero and no syntax-lite multi-dimensional array residue.

## Performance Baseline

`build/input.phase7.codegen.stats.json` reports:

```json
{
  "codegen": 80983,
  "syntaxLite": 1086,
  "comparison": 327,
  "total": 79540
}
```

This remains much slower than desired. Correctness cleanup should continue before a dedicated optimization pass.

## Known Limitations

- PJASS does not pass yet
- function and lambda emission order still produces PJASS forward-reference failures
- nested struct field/method access on indexed expressions is incomplete
- inline Zinc single-line `if (...) return ...` forms are not fully lowered
- comma-separated local declaration expansion is incomplete
- PJASS errors are now grouped, but the `other` bucket still contains several distinct undeclared-function categories

## Next Phase Suggestion

Phase 8 should focus on declaration-order legality and nested struct member lowering before broad undefined-symbol cleanup:

1. emit lambdas before first use or add a dependency-aware function emission pass
2. order struct methods so callees are visible before callers, especially `onDestroy`
3. lower indexed struct instance member access like `arr[i].field` and `arr[i].method()`
4. split comma-separated local declarations and preserve initializer execution order
5. normalize inline Zinc return/control forms before PJASS
