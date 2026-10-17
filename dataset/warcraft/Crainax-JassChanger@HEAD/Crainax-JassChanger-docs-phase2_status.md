---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase2_status.md"
ref: HEAD
---

# Phase 2 Status

Date: 2026-05-08

## Implemented

- struct parser: vJASS and Zinc
- method parser: vJASS and Zinc
- field parser, including static fields, multi-declarations, array fields, and fixed-size fields
- deterministic generated struct names for top-level and library public/private structs
- basic struct lowering for allocator globals, instance field arrays, static fields, methods, static methods, `thistype`, default allocate/create/destroy, `onDestroy`, and static `onInit`
- targeted field/method/create/allocate/destroy/code-reference rewriting outside strings, rawcodes, and comments
- Zinc method body lowering through the existing Zinc body lowerer
- array struct support without allocator globals
- golden fixtures for vJASS structs, Zinc structs, library-scoped structs, fixed arrays, destroy/onDestroy, onInit, static code references, and a real input fragment
- negative fixtures for duplicate fields and method declarations outside structs

## Not Implemented

- module expansion
- static if evaluation
- function interface lowering
- lambda lowering
- interface/delegate/operator/stub/super
- deep chained member inference such as arbitrary `a.b.c` expressions
- full expression AST or byte-level JassHelper output matching

## input.j Scan Baseline

```json
{
  "files": 1,
  "bytes": 4253580,
  "lines": 104238,
  "zincBlocks": 307,
  "libraries": 329,
  "libraryOnce": 9,
  "globalsBlocks": 502,
  "natives": 551,
  "functions": 3576,
  "structsUnsupported": 0,
  "methodsUnsupported": 0,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "functionInterfacesUnsupported": 10,
  "diagnostics": {
    "errors": 0,
    "warnings": 154,
    "unsupported": 154
  },
  "timingMs": {
    "read": 41,
    "preprocess": 79,
    "lex": 173,
    "parse": 127,
    "total": 391
  }
}
```

## Known Limitations

- `samples/input.j` scan-only succeeds, but full codegen is still intentionally blocked while module/static-if/function-interface/lambda features remain unsupported.
- Body rewriting is targeted and fixture-backed, not a full expression AST. It handles the supported phase-2 forms but does not infer arbitrary nested member chains.
- Zinc lambdas are preserved only as unsupported future syntax; they do not break brace capture, but they are not lowered.
- `--allow-unsupported` only allows scan-only to finish and emit stats. Codegen still refuses programs that contain unsupported declarations.

## Commands Run

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase2.stats.json --emit-ast build/input.phase2.ast.txt
```
