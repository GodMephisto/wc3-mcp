---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/compiler_pipeline.md"
ref: HEAD
---

# Compiler Pipeline

## 1. Read

`SourceManager` loads files into memory, normalizes CRLF/CR to LF, stores file ids, and provides line text for diagnostics.

## 2. Preprocess

`Preprocessor` turns source files into logical lines with syntax mode metadata:

- resolves `//! import` with duplicate-import protection
- removes `//! novjass` blocks
- handles release/debug `debug` lines
- expands `//! textmacro` / `//! runtextmacro`
- switches between JASS-like and Zinc mode with `//! zinc` / `//! endzinc`

The output can be written with `--emit-preprocessed`.

## 3. Lex

`Lexer` tokenizes logical lines while preserving `SourceLocation` and syntax mode. It supports the phase-1 JASS/vJASS and Zinc token set, including strings, rawcodes, comments, braces, assignment operators, arrows, and boolean operators.

The output can be written with `--emit-tokens`.

## 4. Parse

`Parser` builds a lightweight AST for:

- `globals`
- `native`
- `type`
- `function`
- `function interface`
- `library` / `library_once`
- `scope`
- vJASS/Zinc `struct`, `method`, and `module`
- basic Zinc `library` / `function` / globals

Unsupported declarations are counted and represented as `Unsupported` nodes where they affect code generation.

The output can be written with `--emit-ast`.

## 5. Symbol And Library Analysis

`SymbolTable` builds deterministic public/private rename maps for functions, function-interface types, and globals inside libraries/scopes.

`LibraryGraph` validates and topologically sorts library dependencies, respecting optional requirements and `library_once` duplicate handling.

`ModuleExpander` expands supported vJASS/Zinc module uses into their owning structs before codegen.

## 6. Codegen

`Phase1Codegen` is still the legacy class name, but it now emits the phase-6 supported subset:

1. one merged `globals` block with `LIBRARY_` constants
2. struct support globals plus function-interface trigger/argument/result globals
3. type declarations and deduplicated natives
4. struct support functions and methods
5. functions in sorted library order
6. generated anonymous lambda functions from Zinc and vJASS bodies
7. function-interface wrapper functions and `vjassc__init_function_interfaces`
8. `vjassc__init_structs` and `vjassc__init_libraries`
9. `main` with init helper injection when present

If unsupported language constructs, capturing lambdas, unknown lambda contexts, function-interface signature errors, or output syntax-lite failures are present, normal codegen fails instead of producing invalid success.

## 7. Validate

Phase 6 added optional validation after output is written. Phase 15 groups the
common combinations behind compile modes:

- `--mode fast` writes output only and skips syntax-lite, PJASS, and
  JassHelper comparison
- `--mode validate` writes output, runs syntax-lite/init validation, and runs
  PJASS when PJASS paths are available
- `--mode full-validation` keeps syntax-lite, PJASS, validation reports, and
  JassHelper structural comparison

- `--check-output-syntax-lite` rejects known residual vJASS/Zinc source forms, anonymous-function syntax, invalid local ordering, invalid return shape, and unbalanced function/globals structure
- `--emit-validation-report` writes output metrics, syntax-lite findings, initialization integrity, PJASS results, JassHelper comparison, and timing data
- `--validate-pjass` runs `pjass.exe common.j blizzard.j <output>` when the executable and support files are available, saving stdout/stderr and classifying common failure categories
- `--compare-jasshelper <path>` compares coarse structure against the legacy JassHelper output without attempting byte-for-byte matching

PJASS is a validation hook. The real `samples/input.j` output currently passes
syntax-lite and PJASS when `InitTrig_japi` is supplied as a validation-only
external stub.
