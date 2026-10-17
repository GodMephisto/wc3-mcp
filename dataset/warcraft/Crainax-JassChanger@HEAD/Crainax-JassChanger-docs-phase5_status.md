---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase5_status.md"
ref: HEAD
---

# Phase 5 Status

Date: 2026-05-08

## Implemented

- removed the phase-4 guard that rejected large lambda-heavy full codegen
- extended lambda extraction to JASS/vJASS function bodies, nested lambda bodies, and same-call suffix lambdas
- skipped anonymous-function extraction inside line comments, strings, and rawcode literals
- kept lambda capture rejection, with local-shadowing and struct-field false-positive fixes
- allowed function-interface integer/runtime values such as saved callback ids instead of treating every value as a direct function target
- allowed statement-form `.evaluate(...)` on non-returning function interfaces by lowering it through the existing trigger backend
- hoisted JASS local declarations to the function top while keeping initializer `set` statements at the original execution point
- added codegen lambda/function-interface metrics to `--emit-stats`
- added `--check-output-syntax-lite`
- added phase-5 fixtures for same-call lambdas, nested lambdas, and ignored comment lambdas

## input.j Scan Baseline

Command:

```bash
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase5.scan.stats.json --emit-ast build/input.phase5.ast.txt --emit-expanded-ast build/input.phase5.expanded.ast.txt
```

Observed baseline:

```json
{
  "functionInterfaces": 19,
  "lambdas": 837,
  "diagnostics": {
    "errors": 0,
    "warnings": 0,
    "unsupported": 0
  },
  "timingMs": {
    "total": 577
  }
}
```

## input.j Full Codegen Baseline

Command:

```bash
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json --check-output-syntax-lite
```

Observed baseline:

```json
{
  "functionInterfaces": 19,
  "functionInterfaceTargets": 61,
  "functionInterfaceCalls": 50,
  "functionObjectCalls": 33,
  "lambdas": 838,
  "lambdasLowered": 838,
  "lambdasGeneratedFunctions": 838,
  "lambdasCapturing": 0,
  "lambdasUnknownContext": 0,
  "lambdasRejected": 0,
  "diagnostics": {
    "errors": 0,
    "warnings": 3,
    "unsupported": 0
  },
  "output": {
    "bytes": 3777710,
    "lines": 91995,
    "functions": 6124,
    "globalsBlocks": 1
  },
  "timingMs": {
    "total": 11672
  }
}
```

Warnings:

```text
duplicate library_once 'YDTriggerSaveLoadSystem' ignored
duplicate native 'DzFrameGetName' ignored
duplicate native 'DzIsWindowActive' ignored
```

The scan-only parser still reports the historical pre-codegen inventory count of 837. Full codegen reports 838 generated lambdas because the lowering scanner now also accounts for a nested/suffix anonymous function that is only visible while extracting lambda bodies.

## Sanity Check

`--check-output-syntax-lite` passed for `build/input.phase5.out.j`.

The output does not contain these source forms outside comments/strings:

```text
//! zinc
//! endzinc
struct/endstruct
method/endmethod
module/endmodule
implement
static if
function interface
function(...)
```

## Known Limitations

- full codegen currently takes about 12 seconds on the real input, above the phase-5 performance target
- generated output is a pure-JASS candidate but has not been validated by pjass or in Warcraft III
- interface dispatch, delegate, operator overload, stub/super, and byte-for-byte JassHelper matching remain later-phase work
- `Phase1Codegen` is still the legacy class/file name
