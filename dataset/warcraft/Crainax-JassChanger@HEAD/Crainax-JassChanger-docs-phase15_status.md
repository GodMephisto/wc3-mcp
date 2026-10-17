---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "docs/phase15_status.md"
ref: HEAD
---

# Phase 15 Status

Phase 15 is the first performance architecture pass after Phase 14 connected
vjassc to the real War3Lib/Xlimon ALPHA compile path. The goal was to keep the
PJASS-green output while making vjassc usable in normal ALPHA iteration.

## Implemented

- Added `--mode fast|validate|full-validation`.
- `fast` writes generated output and stats without syntax-lite, PJASS, or
  JassHelper comparison.
- `validate` writes output, runs syntax-lite/init validation, and runs PJASS
  when PJASS paths are available.
- `full-validation` keeps syntax-lite, PJASS, detailed report output, and
  JassHelper structure comparison.
- Replaced the old `std::ostringstream` writer with a reserved string-backed
  `FastCodeWriter`.
- Added codegen feature gates so ordinary lines skip struct/member regex
  lowering when they have no relevant syntax or current-struct member tokens.
- Skipped unnecessary `.name` function-reference rewriting when no `.name`
  token exists.
- Reworked syntax-lite forward-reference scanning to collect function and
  lambda forward references in one token pass instead of two regex passes.
- Added War3Lib `WAR3_VJASSC_MODE=fast|validate|full-validation` integration.
- War3Lib ALPHA vjassc launch/compile tasks default to `validate`; compare
  tasks default to `full-validation`.
- Added Phase 15 golden fixtures for fast mode, bare field rewriting, static
  method rewriting, and local shadowing.

## Standalone Validation

Build and tests:

```bat
cmd.exe /d /c 'call "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat" >nul && cmake --build build && ctest --test-dir build --output-on-failure'
```

Result: pass.

Fast compile:

```bat
build\vjassc.exe samples\input.j -o build\input.phase15.fast3.out.j --mode fast --emit-stats build\input.phase15.fast3.stats.json
```

Result:

```json
{
  "mode": "fast",
  "timingMs": {
    "codegen": 19955,
    "syntaxLite": 0,
    "pjass": 0,
    "comparison": 0,
    "total": 20541
  }
}
```

Validate compile:

```bat
build\vjassc.exe samples\input.j -o build\input.phase15.validate.out.j --mode validate --emit-stats build\input.phase15.validate.stats.json --emit-validation-report build\input.phase15.validate.validation.json --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi
```

Result:

```json
{
  "mode": "validate",
  "syntaxLite.ok": true,
  "init.issues": 0,
  "pjass.ok": true,
  "pjass.groupedCount": 0,
  "timingMs": {
    "codegen": 20511,
    "syntaxLite": 2200,
    "pjass": 298,
    "comparison": 0,
    "total": 23714
  }
}
```

Full validation:

```bat
build\vjassc.exe samples\input.j -o build\input.phase15.full.out.j --mode full-validation --emit-stats build\input.phase15.full.stats.json --emit-validation-report build\input.phase15.full.validation.json --compare-jasshelper samples\output_jasshelper.j --pjass jasshelper\pjass.exe --common jasshelper\common.j --blizzard jasshelper\blizzard.j --pjass-allow-external InitTrig_japi --emit-pjass-examples 50
```

Result:

```json
{
  "mode": "full-validation",
  "syntaxLite.ok": true,
  "pjass.ok": true,
  "pjass.groupedCount": 0,
  "comparison.referenceFound": true,
  "timingMs": {
    "codegen": 19584,
    "syntaxLite": 2200,
    "pjass": 326,
    "comparison": 342,
    "total": 23065
  }
}
```

## Performance Delta

Phase 14 baseline:

| Segment | ms |
| --- | ---: |
| codegen | 53921 |
| syntaxLite | 9874 |
| pjass | 348 |
| comparison | 121 |
| total | 65241 |

Phase 15 full-validation:

| Segment | ms |
| --- | ---: |
| codegen | 19584 |
| syntaxLite | 2200 |
| pjass | 326 |
| comparison | 342 |
| total | 23065 |

Hot path counters:

| Counter | Phase 14 | Phase 15 |
| --- | ---: | ---: |
| structLookupCalls | 139617 | 102092 |
| functionLookupCalls | 239115 | 141163 |
| cachedRewriteMisses | 17106 | 16735 |

## War3Lib Integration

New environment/config control:

```text
WAR3_VJASSC_MODE=fast|validate|full-validation
```

Task defaults:

| Task | Mode |
| --- | --- |
| `TaskCompileAlphaWithVjassc.lua` | `validate` |
| `TaskStartMapVjasscAlpha.lua` | `validate` |
| `TaskCompileCompareAlpha.lua` | `full-validation` |
| `TaskStartMapCompareAlpha.lua` | `full-validation` |

`Output/compiler_backend_report.json` now includes `vjasscMode` and a
`vjasscInternal` object with timing, pass timing, and counter maps.

War3Lib/Xlimon ALPHA vjassc-selected compile:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
lua lua/tasks/TaskCompileAlphaWithVjassc.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result: pass. The task selected `Output/5_vjassc.j`, wrote
`Output/output.j`, and completed in 38.42 seconds.

Report summary:

```json
{
  "backend": "vjassc",
  "selectedOutput": "vjassc",
  "vjasscMode": "validate",
  "vjassc.pjassOk": true,
  "vjasscInternal": {
    "codegen": 22641,
    "syntaxLite": 2914,
    "pjass": 356,
    "comparison": 0,
    "total": 26601
  },
  "war3libTimingMs": {
    "jassCompilerMs": 27234,
    "totalCompileMs": 37812
  }
}
```

War3Lib/Xlimon ALPHA compare compile:

```bat
set WAR3_VJASSC_EXE=D:/Project/JassChanger/build/vjassc.exe
lua lua/tasks/TaskCompileCompareAlpha.lua D:/War3 D:/War3/Maps/Xlimon D:/WE/KKWE_Plugin
```

Result: pass. The task generated both `Output/5_jasshelper.j` and
`Output/5_vjassc.j`, selected `Output/5_jasshelper.j` as configured, wrote
`Output/output.j`, and completed in 52.72 seconds.

Report summary:

```json
{
  "backend": "both",
  "selectedOutput": "jasshelper",
  "vjasscMode": "full-validation",
  "jasshelper.ok": true,
  "vjassc.ok": true,
  "vjassc.pjassOk": true,
  "vjasscInternal": {
    "codegen": 21891,
    "syntaxLite": 3000,
    "pjass": 378,
    "comparison": 401,
    "total": 26451
  },
  "war3libTimingMs": {
    "jassCompilerMs": 39911,
    "totalCompileMs": 52030
  }
}
```

## Remaining Work

- Function ordering is still output-string based and remains around 600-750 ms.
- `emitStructSupport` includes all struct method lowering and is still the main
  codegen hotspot at roughly 11 seconds.
- ALPHA launch/runtime behavior still needs a manual in-game smoke test when the
  generated output is selected for actual play.
