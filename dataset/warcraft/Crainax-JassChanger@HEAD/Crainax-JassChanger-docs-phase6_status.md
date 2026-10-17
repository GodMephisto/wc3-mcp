---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase6_status.md"
ref: HEAD
---

# Phase 6 Status

Date: 2026-05-08

## Implemented

- added `--validate-pjass`, `--pjass`, `--common`, `--blizzard`, and `--pjass-timeout-ms`
- added `--emit-validation-report` with output metrics, syntax-lite results, init validation, PJASS results, comparison data, and timings
- added PJASS path resolution and stdout/stderr capture under the build directory
- added PJASS error classification for duplicate declarations, syntax errors, undefined functions/variables, return mismatch, expected-endfunction, and local-order failures
- added init validation for `main`, `config`, `vjassc__init_structs`, `vjassc__init_function_interfaces`, and `vjassc__init_libraries`
- added structural comparison against `samples/output_jasshelper.j`
- strengthened output cleanup for semicolons, Zinc boolean operators, range-for lowering, chained method calls, `thistype.typeid`, array-struct field access, and static method function references
- added phase-6 golden fixtures for Zinc range-for, chained method calls, and `thistype` array/typeid lowering

## Verification Commands

Build and tests:

```bat
cmake --build build
ctest --test-dir build --output-on-failure
```

Full real-sample codegen and validation report:

```bat
build\vjassc.exe samples\input.j -o build\input.phase6.out.j --emit-stats build\input.phase6.codegen.stats.json --emit-validation-report build\input.phase6.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
```

PJASS validation run:

```bat
build\vjassc.exe samples\input.j -o build\input.phase6.pjass.out.j --emit-stats build\input.phase6.pjass.stats.json --emit-validation-report build\input.phase6.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

## Output Stats

`build/input.phase6.validation.json` reports:

```json
{
  "bytes": 4082924,
  "lines": 105547,
  "globalsBlocks": 1,
  "globalDeclarations": 3539,
  "natives": 549,
  "functions": 6124,
  "generatedLambdaFunctions": 838,
  "functionInterfaceWrappers": 61,
  "duplicateFunctionNames": 85,
  "duplicateGlobalNames": 7,
  "hasMain": true,
  "hasConfig": true
}
```

## Syntax-Lite Result

`--check-output-syntax-lite` passed for `build/input.phase6.out.j`.

The validation report shows:

```json
{
  "ok": true,
  "issueCount": 0,
  "residualSourceForms": []
}
```

Observed warnings are unchanged from the earlier full-sample runs:

```text
duplicate library_once 'YDTriggerSaveLoadSystem' ignored
duplicate native 'DzFrameGetName' ignored
duplicate native 'DzIsWindowActive' ignored
```

## Init Validation

The validation report confirms:

```json
{
  "hasMain": true,
  "hasConfig": true,
  "hasStructInit": true,
  "hasFunctionInterfaceInit": true,
  "hasLibraryInit": true,
  "mainCallsStructInit": true,
  "mainCallsFunctionInterfaceInit": true,
  "mainCallsLibraryInit": true,
  "structBeforeFunctionInterface": true,
  "functionInterfaceBeforeLibrary": true,
  "libraryInitializerCount": 4,
  "structInitializerCount": 54,
  "issues": []
}
```

## PJASS Result

PJASS is available and was executed:

```json
{
  "requested": true,
  "ran": true,
  "ok": false,
  "exitCode": -1073741819,
  "elapsedMs": 12895,
  "stdoutPath": "build/input.phase6.pjass.validation.pjass.stdout.txt",
  "stderrPath": "build/input.phase6.pjass.validation.pjass.stderr.txt"
}
```

`stderr` is empty. `stdout` starts by successfully parsing `common.j` and `blizzard.j`, then reports generated-output failures. The current summary is:

```json
{
  "duplicateDeclaration": 69,
  "syntaxError": 2768,
  "other": 12515,
  "undefinedVariable": 1973,
  "returnMismatch": 63,
  "undefinedFunction": 499,
  "expectedEndfunction": 153,
  "localOrder": 9
}
```

First concrete blockers observed in the PJASS log:

- duplicate declarations against the PJASS environment, starting with `YDHT` and `YDLOC`
- multi-dimensional array declarations and accesses are not fully lowered yet, for example global shapes like `[9][16]`, `[8][4]`, and `[14][6]`
- a block-comment line in the source around `samples/input.j:42294` is still able to leak into generated global analysis
- some generated struct names and method references are inconsistent, for example `sc__BaseAnim_baseanim_onDestroy` and `si__baseanim_V`
- later PJASS failures cascade into undefined variable/function and primitive type comparison errors

This means phase 6 wires PJASS into the workflow and records actionable failure data, but the generated output does not pass PJASS yet.

## JassHelper Comparison

Compared against `samples/output_jasshelper.j`:

```json
{
  "generatedFunctions": 6124,
  "referenceFunctions": 6864,
  "functionDelta": -740,
  "generatedLines": 105547,
  "referenceLines": 116561,
  "lineDelta": -11014,
  "generatedGlobals": 3539,
  "referenceGlobals": 3817
}
```

The generated output is still structurally smaller than the JassHelper reference, especially around wrapper/support functions and global lowering.

## Performance Baseline

The latest syntax-lite validation run reports:

```json
{
  "codegen": 65470,
  "syntaxLite": 1160,
  "comparison": 296,
  "total": 67698
}
```

The PJASS run reports total time around 79301 ms, including 12895 ms inside PJASS. Codegen is now much slower than the phase-5 baseline and needs a dedicated optimization pass after correctness blockers are reduced.

## Known Limitations

- PJASS does not pass yet
- multi-dimensional arrays need deterministic flattening in declarations and expressions
- block comments need to be filtered consistently before declaration detection and output lowering
- duplicate generated names and common-environment declarations need stricter handling
- struct support names need case-consistent generation and lookup
- interface/delegate/operator/stub/super coverage is still incomplete
- byte-for-byte JassHelper matching, Warcraft III runtime loading, and behavior matching remain future work
- `Phase1Codegen` is still the legacy class/file name

## Next Phase Suggestion

Phase 7 should be driven directly from `build/input.phase6.pjass.validation.pjass.stdout.txt`:

1. fix declaration-level PJASS blockers first: duplicate environment globals, leaked block-comment declarations, and multi-dimensional array declarations
2. add deterministic multi-dimensional array expression flattening with focused fixtures
3. fix generated struct support name consistency and unresolved method references
4. rerun PJASS after each blocker class drops, keeping `--check-output-syntax-lite` green
5. defer broad performance work until PJASS syntax blockers are under control
