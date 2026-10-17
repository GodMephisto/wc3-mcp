---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase4_status.md"
ref: HEAD
---

# Phase 4 Status

Date: 2026-05-08

## Implemented

- parsed vJASS `function interface` declarations as first-class AST declarations instead of unsupported blocks
- parsed Zinc-style `type X extends function(...) -> Y` as function-interface declarations
- lowered function-interface types to `integer` in globals, function parameters, method parameters, locals, and struct fields
- built codegen-time function-interface and function-signature tables for ordinary functions, public/private renamed functions, generated lambdas, and static methods
- lowered explicit and inferred function-interface values:
  - `local F f = F.Target`
  - `set f = F.Target`
  - `local F f = Target`
  - `Apply(1, F.Target)`
  - `Apply(1, Target)`
  - `local F f = Struct.StaticMethod`
- lowered interface `.execute(...)` and `.evaluate(...)` to trigger-array dispatch with argument/result globals
- generated per-interface wrapper functions and `vjassc__init_function_interfaces`
- lowered ordinary function-object `.execute/.evaluate` by direct-call fallback
- lowered function-object `.name` to the final generated function-name string for known function symbols
- lowered non-capturing Zinc anonymous functions to generated `vjlambda__N` functions for code values, trigger callbacks, and function-interface assignments
- rejected capturing Zinc lambdas with a clear diagnostic
- added phase-4 golden and negative fixtures

## Not Implemented

- large lambda-heavy full codegen for real `samples/input.j`
- closure/capture context for lambdas
- full expression AST and full type checker
- prototype backend for ordinary function objects; phase 4 uses direct-call fallback for ordinary functions
- interface dispatch, delegate, operator overloads, stub/super
- byte-for-byte JassHelper output matching
- pjass/runtime validation

## input.j Scan Baseline

Command:

```bash
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase4.stats.json --emit-ast build/input.phase4.ast.txt --emit-expanded-ast build/input.phase4.expanded.ast.txt
```

Observed baseline:

```json
{
  "libraries": 331,
  "libraryOnce": 9,
  "globalsBlocks": 503,
  "functions": 3584,
  "functionInterfaces": 19,
  "modules": 9,
  "moduleUses": 42,
  "staticIfs": 93,
  "staticIfResolvedTrue": 89,
  "staticIfResolvedFalse": 4,
  "staticIfPrunedLines": 5,
  "moduleExpansions": 41,
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 0,
  "staticIfUnsupported": 0,
  "functionInterfacesUnsupported": 0,
  "lambdas": 837,
  "diagnostics": {
    "errors": 0,
    "warnings": 0,
    "unsupported": 0
  },
  "timingMs": {
    "total": 588
  }
}
```

## Full Codegen Behavior

Small phase-4 fixtures full-codegen successfully.

`samples/input.j` full-codegen is intentionally rejected in phase 4 because the real input contains 837 anonymous lambda sites and needs a larger lambda/capture/codegen pass before safe full output:

```bash
build/vjassc samples/input.j -o build/input.phase4.out.j --emit-stats build/input.phase4.codegen.stats.json
```

Observed:

```text
exit code: 6
unsupported: large lambda-heavy full codegen is not supported in phase 4
```

No `build/input.phase4.out.j` is written in that rejection path.

## Commands Run

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase4.stats.json --emit-ast build/input.phase4.ast.txt --emit-expanded-ast build/input.phase4.expanded.ast.txt
build/vjassc samples/input.j -o build/input.phase4.out.j --emit-stats build/input.phase4.codegen.stats.json
```

## Known Limitations

- nested `.evaluate(...)` uses fixed per-interface temp globals `_tmp1` through `_tmp8`
- ordinary function objects use direct calls rather than a trigger/prototype backend
- function-interface target/call counters are present in JSON but not yet populated by the scan-only parser path
- diagnostics from some codegen-time lowering checks still use an unknown synthetic source location
- `Phase1Codegen` remains the legacy class/file name even though it now emits the phase-4 supported subset
