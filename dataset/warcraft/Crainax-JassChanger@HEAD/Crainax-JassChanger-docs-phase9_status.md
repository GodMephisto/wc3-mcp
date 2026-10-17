---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase9_status.md"
ref: HEAD
---

# Phase 9 Status

Phase 9 focuses on shrinking the remaining PJASS semantic blocker set from the Phase 8 baseline, not on adding a broad new syntax surface.

## Implemented

- Zinc `public { ... }` / `private { ... }` access blocks now parse as scoped children, and global lines inside those blocks inherit the block access for public/private symbol rewriting.
- Struct methods are registered as function-interface targets with the implicit `this` parameter, so instance methods can satisfy interface signatures such as `NodeHandler takes Node, integer`.
- Function-call receiver chains on struct-returning expressions are lowered, including nested argument cases like `TakeValue(Node.make(8).get())`.
- Leading-dot chains can continue from standalone struct expressions, covering real-sample lines shaped like `icon[i].getClickBtn()` followed by `.spEnter(...)`.
- Static methods rewrite `this.staticField` to the generated static field name.
- Bare static-method calls inside the current struct rewrite to the generated method name.
- Validation reports now include phase-9 triage buckets for method-chain call-result residues, callback code-signature candidates, known source/environment symbols, likely missing returns, true forward-cycle candidates, grouped PJASS categories, and codegen pass timings.
- Golden fixtures cover phase-9 receiver-chain, nested receiver-chain, instance-method interface, Zinc public block globals, and static `this.` field rewrites.

## Validation Commands

```bat
cmake --build build
ctest --test-dir build --output-on-failure
build\vjassc.exe samples\input.j -o build\input.phase9.out.j --emit-stats build\input.phase9.codegen.stats.json --emit-validation-report build\input.phase9.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite
build\vjassc.exe samples\input.j -o build\input.phase9.pjass.out.j --emit-stats build\input.phase9.pjass.stats.json --emit-validation-report build\input.phase9.pjass.validation.json --compare-jasshelper samples\output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j
```

The PJASS command still exits non-zero because PJASS blockers remain; the validation report is generated and is the phase-9 measurement source.

## Syntax And Init

Latest real-sample syntax-lite result:

```json
{
  "syntaxLite.ok": true,
  "issueCount": 0,
  "commaLocalResidues": 0,
  "indexedStructMemberResidues": 0,
  "inlineZincControlResidues": 0,
  "forwardLambdaReferences": 0,
  "forwardFunctionReferences": 3,
  "duplicateFunctionNames": 0,
  "duplicateGlobalNames": 0,
  "duplicateNativeNames": 0
}
```

Initialization validation has no issues. `main` calls struct, function-interface, and library init helpers in the expected order. The generated output currently has 106131 lines, 6201 functions, 838 generated lambda functions, and 143 function-interface wrappers.

## PJASS Delta

Phase 8 baseline grouped PJASS count: `3144`.

Phase 9 latest grouped PJASS count: `889`.

| Group | Phase 8 | Phase 9 |
| --- | ---: | ---: |
| syntaxError | 708 | 303 |
| undefinedVariable | 656 | 201 |
| other | 1134 | 166 |
| methodChainReceiverResidue | 0 | 64 |
| returnMissingValue | 118 | 50 |
| callbackCodeSignatureMismatch | 0 | 30 |
| uninitializedVariable | 330 | 27 |
| undefinedFunction | 159 | 19 |
| expectedEndfunction | 28 | 11 |
| unresolvedEnvironmentSymbol | 0 | 9 |
| invalidComparison | 8 | 6 |
| forwardFunctionReference | 3 | 3 |

The count meets the phase-9 hard target:

- grouped PJASS count `889 <= 1500`
- `undefinedFunction == 19 <= 100`
- `syntaxError == 303 <= 400`
- `expectedEndfunction == 11 <= 20`
- `localOrder == 0`

## Remaining Blockers

- Method chains still remain when the receiver is the result of a mutating chain call, for example `s__Tooltip_tooltip_create().layoutTitle(...)` or `s__UIText_uiText_setText(...).show(true)`.
- Some generated nested array/index expressions still produce PJASS syntax errors, for example `s__UIText_uiText_ui[s__UIText_uiText_setPoint(...)]`.
- Some static/instance contexts still leak `this`, causing undefined-variable groups around generated methods.
- Callback code-signature mismatches remain where a lambda with parameters is passed as raw `code`.
- Missing-return diagnostics remain in generated functions that PJASS requires to return on all paths.
- `yd_` environment symbols remain unresolved in the PJASS environment.
- Three true forward-cycle candidates remain: SyncBus `onInit` to `onDataSync`, CombineSession `buildSelector` to `runStage`, and MopUpItem lambda to `MopUpItemCreate`.

## Performance

Latest full PJASS run timing:

```json
{
  "read": 42,
  "preprocess": 105,
  "staticIf": 174,
  "lex": 162,
  "parse": 168,
  "moduleExpand": 12,
  "codegen": 109463,
  "syntaxLite": 7970,
  "pjass": 519,
  "comparison": 404,
  "total": 118987
}
```

The largest codegen pass timings are:

```json
{
  "emitFunctions": 96209,
  "emitStructSupport": 76178,
  "finalOutputValidationPrep": 12818,
  "sanitizeOutput": 9646,
  "lowerLambdas": 9278,
  "functionOrdering": 3172,
  "emitGlobals": 409
}
```

The data points to `emitStructSupport`, lambda lowering, and final output sanitizing/ordering as the expensive paths. Phase 9 only instruments these paths and avoids a broad optimization rewrite.

## Next Phase Candidates

- Lower method chains that continue after any struct-returning method call, not just the first function-call receiver.
- Normalize nested generated array/index expressions before PJASS.
- Decide a real strategy for raw `code` callbacks that currently receive parameterized lambdas.
- Add an environment-symbol policy for `yd_*` and other external globals expected from the map runtime.
- Add explicit bridge handling for the three true forward cycles.
