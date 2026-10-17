---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "README.md"
ref: HEAD
---

# vjassc

`vjassc` is a C++20 phase-20 compiler prototype for lowering a supported subset of vJASS/Zinc to plain Warcraft III JASS.

Phase 1 built the compiler foundation: file loading, preprocessing, lexing, top-level parsing, library sorting, minimal public/private rewriting, basic Zinc function lowering, diagnostics, stats, and golden fixture tests.

Phase 2 added structured vJASS/Zinc `struct` and `method` parsing plus basic lowering for fields, methods, static fields, static methods, `thistype`, default allocate/create/destroy, `onDestroy`, and static `onInit`.

Phase 3 added pre-parse `static if` pruning, `LIBRARY_X` / `DEBUG_MODE` / constant boolean evaluation, and AST-level module expansion for vJASS `implement` plus Zinc `module` / `optional module` use.

Phase 4 added `function interface` parsing/lowering, function-object `.execute/.evaluate`, function-object `.name`, static-method function references, callback trigger wrappers, and non-capturing Zinc anonymous function lowering for the supported fixture subset.

Phase 5 unlocks full code generation for the real lambda-heavy `samples/input.j`, lowers nested/same-call anonymous functions, emits lambda/function-interface metrics, and adds a lightweight generated-output syntax check. It is still not a complete JassHelper replacement: interface dispatch, delegate, operator overloads, stub/super, capturing closures, byte-for-byte output matching, pjass validation, and runtime validation remain future work.

Phase 6 adds PJASS validation hooks, stronger output syntax-lite checks, initialization integrity validation, and structural comparison against the legacy JassHelper output. The current real-sample output passes syntax-lite and writes validation reports, but PJASS still fails on remaining lowering gaps; runtime validation and behavior matching remain future work.

Phase 7 reduces the first PJASS blocker set by filtering block comments before parse/lowering, lowering fixed and multi-dimensional arrays to JASS arrays, normalizing generated struct names, removing duplicate generated declarations, preserving public/private rewrites inside lambdas, and writing grouped PJASS examples into validation reports. The real sample now passes syntax-lite with zero duplicate function/global/native names, but PJASS still fails on remaining ordering and advanced struct/member lowering gaps.

Phase 8 legalizes more generated JASS by splitting comma-separated locals, normalizing inline Zinc one-line control forms, lowering indexed struct member access, moving lambda/function blocks before non-cyclic uses, and expanding syntax-lite/PJASS triage. The real sample still does not pass PJASS, but syntax-lite is green with comma-local, inline-Zinc, and indexed-struct residues at zero.

Phase 9 shrinks the remaining PJASS blocker set by lowering struct-returning method chains, adapting instance methods to function-interface signatures, propagating Zinc public/private block access to global rewrites, handling static `this.` field references, and adding PJASS blocker categories plus codegen pass timings. The real sample still does not pass PJASS, but grouped PJASS errors are reduced from the Phase 8 baseline of 3144 to 889 while syntax-lite and init validation remain green.

Phase 10 focuses on PJASS blocker compression for the real sample: multi-line Zinc method-chain continuations, `+` expression continuations, explicit function-interface references in ordinary call arguments, struct support return typing, and malformed discarded field-call statements. PJASS still does not pass, but grouped errors are reduced from the Phase 9 baseline of 889 to 360 while syntax-lite, init validation, duplicate-name checks, and local-order checks remain green.

Phase 11 adds PJASS provenance reporting and further real-sample lowering fixes: one-line Zinc function bodies, YDWE-style public globals in non-Zinc libraries, logical-operator continuations, balanced multi-dimensional struct receiver lowering, safer bare `destroy()` rewriting, and validation helpers for offline PJASS/output analysis. PJASS still does not pass, but grouped errors are reduced from the Phase 10 baseline of 360 to 97 while syntax-lite remains green and full validation runs under 90 seconds.

Phase 12 reduces the remaining PJASS blocker set and starts the first bounded performance pass: Zinc assignment/continuation/else-if fixes, array struct receiver rewrites, generated `deallocate` support, generated-lambda default returns, richer Phase 12 triage reporting, and regex/scanner hot-path cleanup. PJASS still does not pass, but grouped errors are reduced from the Phase 11 baseline of 97 to 21 while syntax-lite remains green and full validation runs under 60 seconds.

Phase 13 reaches PJASS pass for the real `samples/input.j` generated output by adding explicit validation-only external symbols, signature-aware cycle bridges, and residual function-interface callback adapters. Syntax-lite, init validation, and duplicate-name checks remain green. The generated output does not contain the `InitTrig_japi` validation stub, and runtime/map-load validation plus a second performance pass remain future work.

Phase 15 adds `--mode fast|validate|full-validation`, integrates `WAR3_VJASSC_MODE` into War3Lib, replaces the output writer with a reserved string-backed writer, and adds feature gates around struct/member lowering plus faster syntax-lite forward-reference scanning. The real sample now validates with PJASS in about 23 seconds internally, with fast mode skipping syntax-lite/PJASS/comparison entirely.

Phase 16 keeps PJASS green while splitting struct support timings and reducing several lookup/scan counters. It shows that generated struct helpers are no longer the main cost; source method body lowering remains the dominant hotspot.

Phase 17 adds a second performance pass around array/function/struct lookup paths, function-order dependency scanning, current-struct member hit lists, and fast-mode validation accounting. It also tightens fast-mode syntax safety so bare invalid statements such as `123` are rejected. The real sample remains PJASS green; standalone fast/validate/full-validation meet the Phase 17 timing targets, and War3Lib ALPHA fast compare now records vjassc fast ahead of JassHelper.

Phase 18 adds explicit Zinc/JASS-like/generated body-mode routing, reusable line token caches, generated-body fast paths, dirty-only function dependency refresh, and MSVC Release LTCG. The real sample remains PJASS green; standalone fast/validate/full-validation meet the Phase 18 minimum timing targets, War3Lib ALPHA validate passes, and War3Lib ALPHA fast compare records vjassc fast at 6.435 seconds versus JassHelper at 11.943 seconds.

Phase 19 adds another conservative performance pass: string-view lookups in hot maps, current-struct first-character candidate filters, token-level bare member rewrites, global array rewrite caching, single-pass Zinc simple-body lowering, faster syntax-lite guards, pre-reserved cold-compile caches, MethodPlan counters, and report-only incremental chunk state. The real sample remains PJASS green; standalone fast/validate/full-validation meet the Phase 19 speed gates, War3Lib ALPHA validate passes at 6.93 seconds, and War3Lib ALPHA fast compare records vjassc fast at 5.57 seconds versus JassHelper at 11.76 seconds.

Phase 20 adds deterministic generated-entity plan reporting, repeated-output stability coverage, dependency-recorder reconciliation, and conservative array/function-order hot-path work. Correctness remains green, but the strict Phase 20 speed gate was not reached locally, so the release executable was not deployed.

## Repository Layout

- `src/`: compiler implementation
- `tests/fixtures/`: golden cases for the supported phase-1 through phase-20 subset
- `samples/input.j`: large real input used for scan-only validation
- `samples/output_jasshelper.j`: legacy JassHelper output reference for later phases
- `jasshelper/`: old compiler package, kept for behavior comparisons when needed
- `docs/phase1_status.md`: phase-1 implementation status and scan baseline
- `docs/phase2_status.md`: phase-2 implementation status and scan baseline
- `docs/phase3_status.md`: phase-3 implementation status and scan baseline
- `docs/phase4_status.md`: phase-4 implementation status and scan baseline
- `docs/phase5_status.md`: phase-5 full-codegen status and real input baseline
- `docs/phase6_status.md`: phase-6 validation, PJASS, comparison, and performance baseline
- `docs/phase7_status.md`: phase-7 PJASS blocker cleanup status and remaining blockers
- `docs/phase8_status.md`: phase-8 declaration-order, local/control/member lowering, and PJASS triage status
- `docs/phase9_status.md`: phase-9 PJASS semantic blocker compression, validation report, and profiling status
- `docs/phase10_status.md`: phase-10 PJASS blocker compression, remaining blocker report, and profiling status
- `docs/phase11_status.md`: phase-11 PJASS provenance, blocker compression, and performance status
- `docs/phase12_status.md`: phase-12 PJASS convergence, triage, and performance status
- `docs/phase13_status.md`: phase-13 PJASS pass, environment stub policy, bridge/callback adapters, and remaining runtime/performance work
- `docs/phase15_status.md`: phase-15 compile-mode split, performance pass, and War3Lib mode integration
- `docs/phase16_status.md`: phase-16 struct-lowering instrumentation and lookup reduction status
- `docs/phase17_status.md`: phase-17 optimization, War3Lib compare, and remaining hotspot status
- `docs/phase18_status.md`: phase-18 body-mode routing, token-cache reuse, War3Lib compare, and deployment status
- `docs/phase19_status.md`: phase-19 performance pass, standalone/War3Lib acceptance, and deployment status
- `docs/phase20_status.md`: phase-20 deterministic plan reporting, dependency coverage, timing results, and skipped deployment

## Build

Windows with Visual Studio Build Tools, CMake, and Ninja:

```bat
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

Linux or WSL with a C++20 compiler:

```bash
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
ctest --test-dir build --output-on-failure
```

## CLI

```bash
vjassc <input.j> -o <output.j>
vjassc <input.j> -o <output.j> --mode fast
vjassc <input.j> -o <output.j> --mode validate --emit-validation-report build/validation.json --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
vjassc <input.j> -o <output.j> --mode full-validation --emit-validation-report build/validation.json --compare-jasshelper samples/output_jasshelper.j --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
vjassc <input.j> -o <output.j> --debug
vjassc <input.j> --scan-only --allow-unsupported
vjassc <input.j> --emit-preprocessed build/preprocessed.j
vjassc <input.j> --emit-tokens build/tokens.txt
vjassc <input.j> --emit-ast build/ast.txt
vjassc <input.j> --emit-expanded-ast build/expanded.ast.txt
vjassc <input.j> --emit-stats build/stats.json
vjassc <input.j> -o <output.j> --emit-validation-report build/validation.json --check-output-syntax-lite
vjassc <input.j> -o <output.j> --emit-performance-report build/performance.json
vjassc <input.j> -o <output.j> --emit-incremental-state build/incremental.state.json
vjassc <input.j> -o <output.j> --emit-incremental-report build/incremental.report.json --compare-incremental-state build/incremental.state.json
vjassc <input.j> -o <output.j> --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
vjassc <input.j> -o <output.j> --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --pjass-allow-external InitTrig_japi
vjassc --analyze-pjass-log build/input.phase11.pjass.validation.pjass.stdout.txt --emit-validation-report build/input.phase11.triage.json
vjassc --validate-existing-output build/input.phase11.pjass.out.j --emit-validation-report build/input.phase11.existing.validation.json --check-output-syntax-lite
vjassc --analyze-pjass-log build/input.phase12.pjass.validation.pjass.stdout.txt --emit-validation-report build/input.phase12.triage.json --emit-pjass-examples 50
vjassc --validate-existing-output build/input.phase12.pjass.out.j --emit-validation-report build/input.phase12.existing.validation.json --check-output-syntax-lite
vjassc --analyze-pjass-log build/input.phase13.pjass.validation.pjass.stdout.txt --emit-validation-report build/input.phase13.triage.json --emit-pjass-examples 50
vjassc --validate-existing-output build/input.phase13.pjass.out.j --emit-validation-report build/input.phase13.existing.validation.json --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --pjass-allow-external InitTrig_japi
vjassc --help
vjassc --version
```

Example:

```bash
build/vjassc samples/minimal_phase1.j -o build/minimal_phase1.out.j
build/vjassc samples/input.j --scan-only --allow-unsupported --emit-stats build/input.phase5.scan.stats.json
build/vjassc samples/input.j -o build/input.phase5.out.j --emit-stats build/input.phase5.codegen.stats.json --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase6.out.j --emit-stats build/input.phase6.codegen.stats.json --emit-validation-report build/input.phase6.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase7.out.j --emit-stats build/input.phase7.codegen.stats.json --emit-validation-report build/input.phase7.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase7.pjass.out.j --emit-stats build/input.phase7.pjass.stats.json --emit-validation-report build/input.phase7.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
build/vjassc samples/input.j -o build/input.phase8.out.j --emit-stats build/input.phase8.codegen.stats.json --emit-validation-report build/input.phase8.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase8.pjass.out.j --emit-stats build/input.phase8.pjass.stats.json --emit-validation-report build/input.phase8.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
build/vjassc samples/input.j -o build/input.phase9.out.j --emit-stats build/input.phase9.codegen.stats.json --emit-validation-report build/input.phase9.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase9.pjass.out.j --emit-stats build/input.phase9.pjass.stats.json --emit-validation-report build/input.phase9.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
build/vjassc samples/input.j -o build/input.phase10.out.j --emit-stats build/input.phase10.codegen.stats.json --emit-validation-report build/input.phase10.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite
build/vjassc samples/input.j -o build/input.phase10.pjass.out.j --emit-stats build/input.phase10.pjass.stats.json --emit-validation-report build/input.phase10.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j
build/vjassc samples/input.j -o build/input.phase11.out.j --emit-stats build/input.phase11.codegen.stats.json --emit-validation-report build/input.phase11.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --emit-pjass-examples 30
build/vjassc samples/input.j -o build/input.phase11.pjass.out.j --emit-stats build/input.phase11.pjass.stats.json --emit-validation-report build/input.phase11.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --emit-pjass-examples 30
build/vjassc samples/input.j -o build/input.phase12.out.j --emit-stats build/input.phase12.codegen.stats.json --emit-validation-report build/input.phase12.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --emit-pjass-examples 50
build/vjassc samples/input.j -o build/input.phase12.pjass.out.j --emit-stats build/input.phase12.pjass.stats.json --emit-validation-report build/input.phase12.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --emit-pjass-examples 50
build/vjassc samples/input.j -o build/input.phase13.pjass.out.j --emit-stats build/input.phase13.pjass.stats.json --emit-validation-report build/input.phase13.pjass.validation.json --compare-jasshelper samples/output_jasshelper.j --check-output-syntax-lite --validate-pjass --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --pjass-allow-external InitTrig_japi --emit-pjass-examples 50
build/vjassc samples/input.j -o build/input.phase15.fast.out.j --mode fast --emit-stats build/input.phase15.fast.stats.json
build/vjassc samples/input.j -o build/input.phase15.validate.out.j --mode validate --emit-stats build/input.phase15.validate.stats.json --emit-validation-report build/input.phase15.validate.validation.json --pjass jasshelper/pjass.exe --common jasshelper/common.j --blizzard jasshelper/blizzard.j --pjass-allow-external InitTrig_japi
build/vjassc tests/fixtures/phase4_function_interface_execute.in.j -o build/phase4_function_interface_execute.out.j
```

## Phase Boundary

Phase 19 can generate a complete plain-JASS candidate for the real `samples/input.j`, run syntax-lite validation, write grouped validation/provenance reports, run PJASS with explicit validation-only external symbols, validate main/init helper wiring, compare coarse output structure with `samples/output_jasshelper.j`, and emit codegen pass timings plus BodyMode/TokenCache/MethodPlan/function-dependency performance counters and read-only incremental chunk reports.

The current output is not yet a JassHelper replacement. PJASS passes for the real sample when `InitTrig_japi` is supplied through the explicit validation-only external symbol policy, but runtime/map-load validation and behavior matching remain future work. Phase 19 full validation is about 6.6 seconds in the representative standalone run, and fast mode is about 5.1 seconds because it skips validation-only work. War3Lib ALPHA validate passes, and War3Lib fast compare recorded vjassc fast at 5.57 seconds versus JassHelper at 11.76 seconds. `--allow-unsupported` is only for scan-only validation and statistics; it does not make partial code generation safe.
