---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/skills/hivewe-build-and-debug/SKILL.md"
ref: HEAD
---

---
name: hivewe-build-and-debug
description: >-
  Guides HiveWE local build, test, and debug on Windows with CMake presets and vcpkg.
  Use when diagnosing compile/link errors, preset issues, dependency failures, or
  Tracy profiling — or when the user asks to build or test.
disable-model-invocation: true
---

# HiveWE Build and Debug

## Key files

`CMakeLists.txt`, `CMakePresets.json`, `vcpkg.json`, `overlay-ports/`, `.github/workflows/compile.yml`, `tests/`

## Workflow

1. Prerequisites: VS 2022 17.14+, `VCPKG_ROOT` set.
2. **Default:** do not run `cmake --build` unless user explicitly re-allows. Fix pasted errors first.
3. Use presets, not ad hoc flags:
   - `cmake --preset Release` / `cmake --build --preset Release`
   - `ctest --preset Release`
4. `Debug` for runtime issues; `ReleaseTracing` for Tracy profiling.
5. Dependency failures → inspect `vcpkg.json` and `overlay-ports/` before source changes.
6. VS open-folder: post-build `data/` symlink may need Administrator.
7. `LNK1104` on `HiveWE.exe` → often a running editor holding the lock.

## Verification checklist

- [ ] Reconfigure after CMake/dependency edits
- [ ] Narrowest test first, then full preset if feasible
- [ ] Runtime/UI: user launches app and opens affected workflow
- [ ] If user builds manually: interpret pasted errors, give targeted next fix

## Common mistakes

- Custom CMake flags when a preset already encodes the config.
- Forgetting vcpkg overlay ports.
- Assuming Linux/macOS primary workflow — docs and CI are Windows-focused.
