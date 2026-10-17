---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase3_status.md"
ref: HEAD
---

# Phase 3 Status

Date: 2026-05-08

## Implemented

- pre-parse `static if` symbol collection for libraries, `DEBUG_MODE`, and constant booleans
- boolean expression evaluator for `true`, `false`, `DEBUG_MODE`, `LIBRARY_X`, `not` / `!`, `and` / `&&`, `or` / `||`, and parentheses
- vJASS `static if` / `elseif` / `else` / `endif` pruning, including nested blocks
- Zinc inline and block `static if (...) { ... } else { ... }` pruning before parser unsupported scans
- vJASS `module` declarations and `implement` / `implement optional` uses
- Zinc public/private/default `module` declarations and `module` / `optional module` uses
- `ModuleTable` visibility lookup for local, global, public/default cross-library, and private module rules
- `ModuleExpander` cloning module fields/methods into structs, including nested modules, optional missing modules, duplicate-use ignore, and cycle diagnostics
- module `onInit` / `onDestroy` hooks cloned into struct init/destroy flows
- phase-3 golden fixtures and negative fixtures for static-if and module behavior
- `--emit-expanded-ast` for inspecting AST after module expansion

## Not Implemented

- `function interface` lowering
- Zinc anonymous function / lambda lowering
- function type / prototype trigger wrapper lowering
- interface dispatch, delegate, operator overloads, stub/super
- full expression AST and byte-for-byte JassHelper output matching

## input.j Scan Baseline

Command:

```bash
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase3.stats.json --emit-ast build/input.phase3.ast.txt --emit-expanded-ast build/input.phase3.expanded.ast.txt
```

Observed baseline:

```json
{
  "libraries": 331,
  "libraryOnce": 9,
  "globalsBlocks": 503,
  "functions": 3584,
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
  "functionInterfacesUnsupported": 10,
  "diagnostics": {
    "errors": 0,
    "warnings": 10,
    "unsupported": 10
  },
  "timingMs": {
    "total": 799
  }
}
```

Full code generation for `samples/input.j` still rejects safely with exit code `6` because `function interface` remains future-phase syntax.

## Known Limitations

- `Phase1Codegen` remains a legacy file/class name even though it now emits the phase-3 supported subset.
- Module member order is normalized as expanded module fields/methods before the struct's own fields/methods; the AST does not yet preserve mixed source order across fields, methods, and module uses.
- Static-if expressions are intentionally limited to the supported boolean subset listed above; arbitrary calls, comparisons, numeric truthiness, and string expressions are future work.
- Optional missing modules are silently ignored during expansion and reflected only through expansion stats.

