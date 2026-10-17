---
type: vendor-doc
source_repo: "https://github.com/dc114154qq/war3-reforged-24-unit-selection"
source_path: "README.md"
ref: HEAD
---

# Warcraft III Reforged 24-Unit Selection

Standalone Windows plugin prototype for Warcraft III Reforged `2.0.4.23745`.
It raises the local selection limit to 24 and routes units 13-24 through a
second native order transaction without enlarging the game's fixed 12-unit
handler stacks or inline arrays.

## Status

This project is under active reverse engineering and is not yet complete.

Verified on the target build:

- 21 same-type units selected at once;
- synchronized selection remained 12 while mirror/local selection remained 21;
- a move order reached both native batches;
- the game remained alive during the 30-second movement monitor;
- DLL self-test scenarios `0-8` pass, including the DR3/VEH observer test.

Still pending:

- attack, point-target, unit-target, ability, and Shift-queue live tests;
- exact 24-unit selection and control groups;
- native subgroup and automatic caster arbitration acceptance;
- the 4x6 portrait layout;
- long-duration crash-stability testing.

See [the acceptance checklist](tools/WAR3_SELECTION_PERSISTENT_ACCEPTANCE.md)
for the completion contract.

## Design

The six native order handlers use fixed 12-entry working storage. Earlier
experiments that enlarged their stack frames and arrays caused delayed access
violations. The current route instead:

1. keeps the full 13-24 unit local selection intact;
2. calls each native handler once with the original synchronized batch;
3. transactionally switches the synchronized unit set to at most 12 extra
   units;
4. calls the same native handler again;
5. restores the original synchronized set in reverse order.

The production build disables direct executable-page selection patches. It
uses validated private writable order-handler pointers and persistent hardware
breakpoints for the selection-capacity path.

The plugin is loaded at normal game startup by a local DINPUT8 proxy. It does
not require per-launch DLL injection, a PID-specific runner, or direct launch
of `Warcraft III.exe`.

## Build

Requirements:

- Windows x64;
- Visual Studio 2022 Community with the C++ desktop toolchain;
- PowerShell;
- Python 3 and Pillow only for the optional QuickSave live-test runner.

Build the plugin, self-test DLL, self-test runner, and window bridge:

```powershell
.\tools\build_war3_selection_plugin.ps1
```

The build uses MSVC `/W4 /WX` and runs self-test scenarios `0-8`.

Build and smoke-test the DINPUT8 proxy:

```powershell
.\tools\build_war3_dinput8_proxy.ps1
```

Generated binaries stay under ignored `tools\dinput8-*-build` directories.

## Local Deployment

Exit Warcraft III before replacing files. Copy these locally built files into
the game's `_retail_\x86_64` directory:

- `tools\dinput8-marker-build\DINPUT8.dll`
- `tools\dinput8-marker-build\DINPUT8Original.dll`
- `tools\dinput8-plugin-build\War3SelectionPlugin.dll`

Then start the game with the Battle.net Play button. The proxy loads
`DINPUT8Original.dll` for the original API and automatically loads
`War3SelectionPlugin.dll`.

The plugin writes `war3_selection_auto_status.bin` beside the game executable
for local acceptance telemetry.

## Live Move Test

When the first QuickSave is already loaded:

```powershell
python .\tools\run_war3_quicksave.py `
  --no-launch `
  --skip-load `
  --live-move-test `
  --monitor-seconds 30
```

The runner does not inject a DLL. It uses the already loaded persistent plugin
and resolves the telemetry file beside the active `Warcraft III.exe`.

## Scope Boundaries

This repository does not modify the existing Warcraft III trainer, key-remap
tools, or the official `ClientSdk.dll`. It contains no Blizzard binaries,
saved games, crash dumps, screenshots, or locally built DLL/EXE artifacts.
