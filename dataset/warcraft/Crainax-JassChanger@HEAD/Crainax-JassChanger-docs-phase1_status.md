---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase1_status.md"
ref: HEAD
---

# Phase 1 Status

Date: 2026-05-08

## Implemented

- CMake C++20 project generating `vjassc`.
- CLI: `--help`, `--version`, `--scan-only`, `--debug`, `--release`, `--emit-preprocessed`, `--emit-tokens`, `--emit-ast`, `--emit-stats`, `--import-path`, `--allow-unsupported`.
- Source loading, newline normalization, source locations, and diagnostics.
- Preprocessor support for `//! import`, `//! novjass`, debug-line release/debug behavior, textmacros, runtextmacros, and Zinc mode boundaries.
- JASS-like and Zinc lexer for the phase-1 token set.
- Parser for top-level globals/native/type/function/library/library_once/scope plus basic Zinc libraries/functions/globals.
- Library dependency sorting and `library_once` duplicate handling.
- Deterministic public/private rewriting for functions and globals.
- Codegen for merged globals, deduped natives, type declarations, vJASS functions, basic Zinc functions, initializer helper, and main injection.
- Golden fixture test runner covering tasks 01 through 09.

## Not Implemented

阶段 1 不是完整 JassHelper 替代。

阶段 1 不能 lowering `struct` / `method` / `function interface` / `lambda`。

阶段 1 能对完整 `input.j` 扫描统计，但不能完整生成最终 `war3map.j`。

Additional unsupported or intentionally incomplete areas:

- `thistype`, allocation, create/destroy, member access lowering.
- `module`, `interface`, `delegate`, operator overloads.
- `static if` evaluation.
- Full Zinc semantic and type checking.
- Byte-for-byte matching against `output_jasshelper.j`.

## Validation

Commands run successfully:

```bat
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\minimal_phase1.j -o build\minimal_phase1.out.j --emit-preprocessed build\minimal_phase1.preprocessed.j --emit-tokens build\minimal_phase1.tokens.txt --emit-ast build\minimal_phase1.ast.txt --emit-stats build\minimal_phase1.stats.json
build\vjassc.exe samples\input.j --scan-only --allow-unsupported --emit-stats build\input.stats.json
```

CTest result:

```text
100% tests passed, 0 tests failed out of 1
```

## `input.j` Scan Baseline

`build/input.stats.json` was generated with exit code `0`.

```json
{
  "files": 1,
  "bytes": 4253580,
  "lines": 104238,
  "zincBlocks": 307,
  "libraries": 329,
  "libraryOnce": 9,
  "scopes": 0,
  "globalsBlocks": 494,
  "natives": 551,
  "types": 0,
  "functions": 3575,
  "structsUnsupported": 162,
  "methodsUnsupported": 1370,
  "modulesUnsupported": 51,
  "staticIfUnsupported": 93,
  "functionInterfacesUnsupported": 10,
  "textmacros": 3,
  "runtextmacros": 18,
  "diagnostics": {
    "errors": 0,
    "warnings": 1686,
    "unsupported": 1686
  },
  "timingMs": {
    "read": 0,
    "preprocess": 83,
    "lex": 158,
    "parse": 121,
    "total": 363
  }
}
```

Line and byte counts reflect normalized LF text after source loading. The warnings are expected phase-1 unsupported diagnostics.

## Next Phase Suggestions

- Add real struct/method AST and lowering before attempting full `input.j` codegen.
- Implement `static if` condition evaluation and conditional parse pruning.
- Add module expansion and `implement` handling.
- Begin comparing selected lowered samples against `jasshelper/output.j` from the legacy package.
