---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase13_status.md"
ref: HEAD
---

# Phase 13 Status

Phase 13 targeted the final PJASS blocker set from Phase 12 for the real
`samples/input.j` generated output. The latest Phase 13 command now passes
PJASS with an explicit validation-only environment stub for `InitTrig_japi`.

This is still not runtime validation. The output needs Warcraft III/map-load
testing before it can be treated as a JassHelper replacement.

## Phase 12 Baseline

Phase 12 ended with syntax-lite and init checks green, but PJASS still failed:

```text
grouped PJASS count: 21
callbackCodeSignatureMismatch: 15
forwardFunctionReference: 5
undefinedFunction: 1 (InitTrig_japi)
full validation total: 57559 ms
```

## Implemented

- Added `--pjass-allow-external <name>` and `--allow-external-init <name>` for
  explicit validation-only external functions.
- PJASS validation now writes a separate `.env-stubs.j` file and passes it to
  PJASS before the generated script. These stubs are not emitted into the
  formal generated output.
- Validation reports now include `pjass.validationEnvStubPath`,
  `pjass.allowedExternalFunctions`, and the Phase 13 marker.
- Function ordering now detects cyclic generated function dependencies and
  rewrites safe cycle edges through signature-aware bridges.
- `takes nothing returns nothing` cyclic targets are rewritten through
  `ExecuteFunc` when the call site does not need parameters or a return value.
- Direct cyclic calls with parameters and `returns nothing` can use generated
  bridge argument globals plus a no-arg `ExecuteFunc` wrapper.
- Residual function-interface callback calls are adapted after codegen when a
  raw `function vjlambda__N` argument is passed to a generated integer callback
  slot and the lambda signature matches a known function-interface runtime.
- Ambiguous same-signature runtime interfaces use the callee/interface name hint
  to choose the closest generated runtime wrapper instead of guessing from the
  first signature match.
- Added Phase 13 golden fixtures for a no-arg forward bridge cycle, formal
  output preservation around external environment symbols, and a known
  function-interface callback adapter context.

## Validation Commands

Phase 12 offline triage was preserved:

```bat
build\vjassc.exe --analyze-pjass-log build\input.phase12.pjass.validation.pjass.stdout.txt --emit-validation-report build\input.phase13.triage.from-phase12.json --emit-pjass-examples 50
```

Existing-output validation remained usable:

```bat
build\vjassc.exe --validate-existing-output build\input.phase12.pjass.out.j --emit-validation-report build\input.phase13.existing-output.validation.json --check-output-syntax-lite
```

Final real input validation:

```bat
build\vjassc.exe samples\input.j -o build\input.phase13.pjass.out.j --emit-stats build\input.phase13.pjass.stats.json --emit-validation-report build\input.phase13.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi --emit-pjass-examples 50
```

Build and golden fixtures:

```bat
cmd.exe /d /s /c "call \"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat\" >nul && cmake --build build && ctest --test-dir build --output-on-failure"
```

## Syntax And Init

Latest real-sample validation result:

```json
{
  "phase": 13,
  "syntaxLite.ok": true,
  "syntaxLite.issueCount": 0,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0,
  "init.issues": 0,
  "pjass.ok": true,
  "pjass.groupedCount": 0,
  "pjass.allowedExternalFunctions": ["InitTrig_japi"]
}
```

Generated output metrics:

```json
{
  "lines": 115126,
  "functions": 6624,
  "generatedLambdaFunctions": 838,
  "functionInterfaceWrappers": 558
}
```

## PJASS Delta

| Group | Phase 12 | Phase 13 |
| --- | ---: | ---: |
| callbackCodeSignatureMismatch | 15 | 0 |
| forwardFunctionReference | 5 | 0 |
| undefinedFunction | 1 | 0 |
| total grouped PJASS errors | 21 | 0 |

PJASS now reports a successful parse for `common.j`, `blizzard.j`, the generated
validation environment stub, and `build\input.phase13.pjass.out.j`.

## Performance

Phase 13 is roughly flat with Phase 12 and did not meet the planned 45 second
minimum performance target. The PJASS correctness goal is complete; the next
phase still needs a real performance pass.

| Timing | Phase 12 ms | Phase 13 ms |
| --- | ---: | ---: |
| codegen | 48084 | 48401 |
| syntaxLite | 8187 | 8171 |
| pjass | 343 | 275 |
| comparison | 336 | 305 |
| total | 57559 | 57753 |

Largest Phase 13 codegen pass timings:

```json
{
  "emitFunctions": 46115,
  "emitStructSupport": 36657,
  "lowerLambdas": 4608,
  "finalOutputValidationPrep": 2130,
  "sanitizeOutput": 1558,
  "functionOrdering": 572
}
```

## Remaining Work

- Runtime/map-load validation has not been performed.
- Behavior matching against the legacy JassHelper output is still structural,
  not byte-for-byte or runtime-equivalent.
- Full validation total is still `57753 ms`, above the planned Phase 13
  `<= 45000 ms` line.
- The validation-only external stub mechanism currently supports explicit
  no-arg/`returns nothing` function names. Richer environment source files can be
  added later if more external symbols are needed.

## Readiness

Phase 13 reaches PJASS pass for the real generated output without adding fake
environment functions to the output script. It is a compiler-validation
milestone, not a final JassHelper replacement milestone. Phase 14 should focus
on Warcraft III runtime/map-load validation, behavior comparison, and a second
performance pass.
