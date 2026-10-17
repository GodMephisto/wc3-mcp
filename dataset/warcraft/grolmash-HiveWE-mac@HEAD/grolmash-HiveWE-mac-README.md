---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "README.md"
ref: HEAD
---

# HiveWE
HiveWE is a Warcraft III World Editor (WE) that focuses on speed and ease of use. 
It improves massively on the vanilla WE, especially for large maps where the regular World Editor is often too slow and clunky.
The aim is to be a lightweight tool that only does what it needs to and relegates other tasks to high-quality apps.

[Thread on the Hiveworkshop](https://www.hiveworkshop.com/threads/introducing-hivewe.303183/)

Some of the benefits over the vanilla WE:
- Way faster loading times (32s -> 4s on a sample map)
- Renders your whole map at 120 fps
- Modern UI/UX
- Edit the pathing map directly
- Edit global tile pathing
- Import heightmaps
- Improved editing palettes
  - Directly edit water height
  - Brushes of size 1000+
  - Doodad variation control
- Unified global search (double shift)

## Features

- Edit the terrain
![HiveWE Screenshot](/Screenshots/HiveWE.png)
- Advanced Object Editor
![HiveWE Screenshot](/Screenshots/ObjectEditor.png)
- Directly edit the pathing map  
![Edit the Pathing Map](/Screenshots/PathingEditing.png)
- Edit triggers
![Edit triggers](/Screenshots/TriggerEditor.png)

## Download

See the [releases page](https://github.com/stijnherfst/HiveWE/releases) for binary downloads.

## Other Community Tools

Trigger editing: [WC3 Typescript](https://cipherxof.github.io/w3ts/)  
Model editing: [3DS Max Plugin](https://github.com/TaylorMouse/warcraft_III_reforged_tools)
or [Retera Model Studio](https://github.com/Retera/ReterasModelStudio)

## Build Instructions

### Windows

0. Requires Visual Studio 17.14 or higher (C++23)
1. Clone HiveWE somewhere 
`git clone https://github.com/stijnherfst/HiveWE.git`
2. Clone [vcpkg](https://github.com/microsoft/vcpkg) somewhere central (eg. "C:/")
`git clone https://github.com/Microsoft/vcpkg.git`
3. Run vcpkg/bootstrap-vcpkg.bat
4. Add a user environment variable to your system:
- `VCPKG_ROOT`: the location where vcpkg is installed (e.g. "C:\vcpkg")
5. Build
    ### Visual Studio or CLion
   1. Open Visual Studio as an **Administrator** and using the open folder button to open the HiveWE folder. (**Administrator required** for creating a symbolic link on Windows)
   2. Dependencies will be automatically compiled, might take about 15-20 minutes (mostly due to Qt)
    ### CMake
    ```shell
    # Configure and build
    cmake --preset Release
    cmake --build --preset Release
    
    # Run tests
    ctest --preset Release 
    ```

### macOS

Requires CMake 3.29 or newer, Ninja, Xcode Command Line Tools, vcpkg, Homebrew `llvm`, and Homebrew `ccache`. The macOS presets use `HOMEBREW_PREFIX` to find `clang++`, `clang-scan-deps`, and `ccache`; set it to your Homebrew prefix if your shell does not already export it.

```shell
git clone https://github.com/Microsoft/vcpkg.git
./vcpkg/bootstrap-vcpkg.sh
export VCPKG_ROOT="$PWD/vcpkg"
```

Configure and build with the macOS Ninja presets:

```shell
cmake --preset macOS-Debug-WithTests
cmake --build --preset macOS-Debug-App
cmake --build --preset macOS-Debug-Verify
ctest --preset macOS-Debug-WithTests

cmake --preset macOS-Release
cmake --build --preset macOS-Release
ctest --preset macOS-Release
```

Use `macOS-Debug-App` for quick app-only iteration, `macOS-Debug-Tests` when only the test binary is needed, and `macOS-Debug-Verify` when you need both the app and tests. All three build from the same `build/macOS-Debug-WithTests` tree so C++ module rebuilds are not repeated in separate Debug and WithTests directories. Use `macOS-Release-App` for app-only release packaging and `macOS-Release-Verify` when Release CTest evidence is needed. These presets are intentionally serial because the current Clang/Ninja C++ module scan can race header-imported modules in parallel builds.

The macOS configure also writes a local CLI launcher into each build tree. You can pass a map folder positionally:

```shell
build/macOS-Debug-WithTests/cli/hivewe "/path/to/unpacked-map-folder"
build/macOS-Debug-WithTests/cli/hivewe --warcraft-dir "/Applications/Warcraft III" "/path/to/unpacked-map-folder"
```

The CLI opens an unpacked Warcraft III map folder containing files such as `war3map.w3i`, `war3map.w3e`, and `war3map.doo`. If you have a `.w3x` or `.w3m` archive, unpack it first or use the GUI's Open Map (MPQ) option.

To make that local build available as `hivewe`, symlink the launcher into any directory on your `PATH`:

```shell
mkdir -p "$HOME/.local/bin"
ln -sf "$PWD/build/macOS-Debug-WithTests/cli/hivewe" "$HOME/.local/bin/hivewe"
hivewe "/path/to/unpacked-map-folder"
```

After `cmake --install`, the installed app bundle also contains `HiveWE.app/Contents/Resources/hivewe`, which can be symlinked the same way.

The macOS presets enable the native Metal renderer by default for the map canvas, model editor, and object-grid previews. The default macOS Metal configure path does not add Qt OpenGL, glad, or imgui link targets; those CMake targets are only enabled for the legacy OpenGL fallback. macOS configures still pin transitive QtGui/SOIL2 OpenGL lookups to Apple's framework so Metal builds do not accidentally link XQuartz `libGL`. The shared vcpkg manifest may still install the fallback packages so the OpenGL path can be built from the same dependency set. The non-macOS builds keep using the existing OpenGL renderer. Use `-DHIVEWE_ENABLE_METAL=OFF` with `cmake --preset macOS-*` to force the OpenGL path on macOS for debugging.

Metal runtime smoke checks:

```shell
python3 tools/run_metal_smoke.py build/macOS-Debug-WithTests/HiveWE.app --warcraft-dir "/Applications/Warcraft III" --require-current-source --require-clean-source
```

`ctest --preset macOS-Debug-WithTests` includes a Metal backend identity fingerprint, the Warcraft-free Metal renderer probes, a strict same-app Metal renderer-probe parity smoke, a strict same-app Metal preview-probe parity smoke, strict same-app save/reload and MPQ export/reload map-render parity smokes, a strict same-app shared-preview parity smoke with zero-difference thresholds, and always registers the full Metal runtime smoke; that full runtime test is skipped unless `HIVEWE_WARCRAFT_DIR` is available at test time. The full smoke gate above validates save/reload state, mutated save/reload state, MPQ export/unpack/reload state, saved-map file preservation/generated outputs, render-mode toggles, terrain/material/cliff/particle map-frame readback, click helpers, unit and doodad picking, model editor/object-grid previews, SD/HD/team-color/transparency model paths, renderer-probe readbacks, selection overlays, and particle color readback. `tools/run_metal_smoke.py` fingerprints the app with `--print-render-backend`, writes and validates `metal-smoke-provenance.json` with source/app/artifact SHA-256 data plus the required full-smoke command flags and recorded command values, verifies the expected PNG artifact manifest, dimensions, and non-flat content, and can require current clean-source provenance with `--require-current-source --require-clean-source` or a specific reviewed revision with `--require-source-head <sha> --require-clean-source`; it now requires `--warcraft-dir <path>` or `HIVEWE_WARCRAFT_DIR` so full smoke never silently uses saved Qt settings, stale artifacts, dirty-source artifacts, incomplete or mismatched smoke-mode commands, or a non-Metal runtime. Non-model smoke modes can run without Warcraft content by using the bundled minimal `data/warcraft/` fixture; the Warcraft-free map-canvas smoke now saves, reloads, mutates serialized map data, saves/reloads again, exports to MPQ, unpacks, reloads again, renders and picks the reloaded map through Metal, and renders synthetic model editor/object-grid previews after the map is loaded. OpenGL builds support `--smoke-model-previews` for matching real, synthetic material-heavy, and synthetic PE2 particle model-editor/object-grid preview artifacts, and both backends support `--smoke-shared-preview-probes` for a Warcraft-free synthetic preview artifact check. Use `tools/run_renderer_parity.py` to capture or compare matching OpenGL and Metal artifact directories and write a parity Markdown/JSON report with diff PNGs and capture provenance; default capture runs map render-mode, picking, and model-preview smoke and needs `--warcraft-dir` or `HIVEWE_WARCRAFT_DIR`, `--capture-only opengl|metal` records one backend at a time for cross-host parity evidence, captured artifact manifests include the reported renderer backend, source revision, executable hash, PNG dimensions, and SHA-256 fingerprints, `--map-after-save-reload` captures map-render parity artifacts and validates picking after a save/reload cycle, `--map-render-only` skips model-preview captures for a Warcraft-free map-canvas parity check but still validates picking, `--metal-renderer-probes-only` runs the Warcraft-free Metal renderer probe comparison, `--metal-preview-probes-only` runs the Warcraft-free Metal preview probe comparison, `--preview-probes-only` runs the Warcraft-free shared preview comparison, `--require-capture-manifest` enforces provenance when comparing externally collected artifacts, `--require-current-source` rejects stale captures from a different commit, `--require-matching-source` rejects OpenGL/Metal capture pairs from different commits, and `--require-clean-source` rejects captures from a dirty working tree. Add `--pr-gate` to capture or compare commands for maintainer-facing parity evidence; it enables the required manifest/current-clean-source checks, matching-source checks for final comparisons, backend identity checks, recorded picking smoke coverage and command provenance for map parity modes, post-save/reload map captures unless `--map-after-mpq-export-reload` is explicitly requested, required PNG fingerprints in every submitted capture manifest, and rejection of `--keep-existing-artifacts`; fresh comparisons clear prior parity JSON/Markdown/provenance files plus the previous diff PNG directory before writing new report evidence. Use `tools/package_renderer_capture.py` to validate and zip one backend capture directory for cross-host transfer or PR artifact storage, and pass those bundles back to `tools/run_renderer_parity.py --skip-capture` with `--opengl-artifact-archive` or `--metal-artifact-archive`; packaging rejects capture manifests that do not fingerprint the required PNGs or prove the requested capture commands, and each zip summary lists required artifacts, captured dimensions/SHA-256 prefixes, host/app provenance, source-pinned mode-specific comparison commands, and PR-gate reminders. The macOS CI job packages both the default Metal PR candidate bundle and a Metal MPQ export/reload map-render bundle when Warcraft content is configured, so the OpenGL host only needs to provide matching reference bundles for final parity reports. The final renderer-evidence job now fails automatically on pull requests that touch renderer, runtime, smoke, package, or evidence-sensitive paths if required cross-host artifacts are missing; manual workflow dispatch can also set `renderer_pr_evidence_required` to require the same complete evidence set. To reduce cross-host handoff mistakes, `tools/capture_renderer_pr_evidence.py --backend opengl|metal` runs the default PR capture, packages it, runs the MPQ export/reload map-render capture, packages it, and writes the source-pinned comparison/audit commands for the opposite backend; `tools/create_renderer_pr_handoff.py` assembles the Metal-side archives, package reports, full-smoke artifacts, audits, manifest, and a clean zip while preserving the full-smoke capture filename required by provenance. Use `--same-app-determinism` only for intentional same-backend smoke gates. After `cmake --install`, run `tools/check_macos_package.py` to smoke the installed app against its bundled test map and verify bundled resources plus the installed Metal backend identity; install packaging also strips and rejects unused bundled XQuartz `libGL`/X11 dylibs in Metal builds.

The package validator also runs strict `codesign` verification on the installed app and the app mounted from the generated DMG, and records that result in package reports used by the final evidence audit. `tools/prepare_renderer_pr_evidence.py` and `tools/create_renderer_pr_handoff.py` reject stale package-report schemas, reports missing that codesign gate, dirty or mismatched installed full-smoke source provenance, stale installed full-smoke PNG fingerprints, stale packaged capture PNG payloads, Metal captures or full-smoke runs from a different Debug package executable, OpenGL or Metal default/MPQ captures from different executables, wrong-backend capture archives, wrong default-vs-MPQ capture modes, and archives missing command provenance, picking coverage, or PNG fingerprints before final parity/handoff work begins.

For maintainer-facing Metal PR evidence, capture the Metal candidate from `build/macOS-Debug-WithTests/install/HiveWE.app` after `cmake --install` and preserve the Debug package report. The final PR audit rejects default and MPQ Metal capture manifests whose app path, executable path, executable size, executable SHA-256, or source head does not match that Debug package report.

For installed-bundle validation, use:

```shell
python3 tools/check_macos_package.py --build-dir build/macOS-Debug-WithTests --warcraft-dir "/Applications/Warcraft III"
python3 tools/check_macos_package.py --build-dir build/macOS-Debug-WithTests --skip-smoke --require-current-source --require-clean-source --require-build-type Debug --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-parity-smoke --map-canvas-smoke --map-render-parity-smoke --mpq-map-render-parity-smoke --shared-preview-probe-smoke --shared-preview-parity-smoke --preview-probe-smoke --preview-probe-parity-smoke --run-cpack --package-report-json /tmp/hivewe-debug-package-report.json --package-report-markdown /tmp/hivewe-debug-package-report.md
python3 tools/check_macos_package.py --build-dir build/macOS-Release --skip-smoke --require-current-source --require-clean-source --require-build-type Release --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-parity-smoke --map-canvas-smoke --map-render-parity-smoke --mpq-map-render-parity-smoke --shared-preview-probe-smoke --shared-preview-parity-smoke --preview-probe-smoke --preview-probe-parity-smoke --run-cpack --package-report-json /tmp/hivewe-release-package-report.json --package-report-markdown /tmp/hivewe-release-package-report.md
```

The package check skips the full installed-app smoke when no `--warcraft-dir` or `HIVEWE_WARCRAFT_DIR` is available, and otherwise runs it against the installed app's bundled `Contents/Resources/data/test map/` unless `--skip-smoke` is passed. Add `--require-full-smoke` for PR evidence runs that must fail instead of skipping when Warcraft content is absent or `--skip-smoke` is set. Use `--installed-smoke-artifact-dir` and `--installed-smoke-capture-frame` to keep the installed-app smoke PNGs for review. Use `--require-source-head <sha>` instead of `--require-current-source` when validating a detached installed app or DMG against a specific reviewed commit, and `--require-build-type Debug|Release|RelWithDebInfo` when the package evidence must prove a specific CMake configuration. Use `--package-report-json` and `--package-report-markdown` to preserve the package/DMG validation result for PR evidence audits.
With `--run-cpack`, the package check builds the current DragNDrop DMG from the selected build tree, verifies that exact image with `hdiutil`, mounts it read-only, and validates the bundled app contents.

The macOS build produces a `HiveWE.app` bundle. To create an install tree or DMG:

```shell
cmake --install build/macOS-Debug-WithTests --prefix build/macOS-Debug-WithTests/install
cmake --install build/macOS-Release --prefix build/macOS-Release/install
cpack --preset macOS-Debug-DMG
cpack --preset macOS-Release-DMG
```

See [docs/metal-renderer-status.md](docs/metal-renderer-status.md) for Metal coverage, smoke expectations, and remaining renderer risks.

If you run into any issues, then feel free to contact me at HiveWorkshop (eejin) or on Discord (eejin)

## Potential Contributions

Want to help with the development of HiveWE? Below is a list of features that you could implement. You can try one of these or just add something else you feel like HiveWE should have. Any contributions are welcome!

- Being able to change forces/teams
- Making HiveWE run faster
- An FDF frame editor
- Text colorizer
- Advanced terrain editing tools (e.g. flood fill, magic wand selection)
- Or any other functionality you think would be cool

If you have any questions, then don't be afraid to message me here, on the HiveWorkshop (eejin) or on Discord (eejin)
