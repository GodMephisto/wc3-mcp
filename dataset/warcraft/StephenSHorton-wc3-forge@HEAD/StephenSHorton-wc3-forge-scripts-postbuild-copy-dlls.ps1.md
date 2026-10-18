---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/postbuild-copy-dlls.ps1"
ref: HEAD
---

# Wails postBuildHook: copy vendored CASC DLLs next to the compiled binary.
#
# The Go side (`internal/casc/casc.go`) loads CascLib.dll via
# `syscall.NewLazyDLL`. It searches the executable's directory first, so the
# DLLs must sit next to `wc3-forge.exe` in `build/bin/` for the binary to be
# runnable straight out of the box. Without this hook the user has to remember
# to copy `scripts/casclib/*.dll` into `build/bin/` after every fresh build,
# and forgetting it produces the cryptic
#   "Failed to load CascLib.dll: The specified module could not be found."
# at runtime (with zero units rendered).
#
# Wails v2 runs postBuildHooks via `shell.RunCommand` (no shell expansion)
# with `build/bin/` as the working directory, so we resolve sources relative
# to $PSScriptRoot and copy them into the cwd by default. The caller may pass
# an explicit destination as the first argument (Wails passes the binary
# path via `${bin}`; we take its parent directory).

[CmdletBinding()]
param(
  # Wails substitutes `${bin}` with the path of the compiled binary.
  # If passed, we use its parent directory as the destination.
  [Parameter(Position = 0)]
  [string]$BinPath
)

$ErrorActionPreference = 'Stop'

$srcDir = Join-Path $PSScriptRoot 'casclib'
if (-not (Test-Path $srcDir)) {
  Write-Error "CASC source directory not found: $srcDir"
  exit 1
}

if ($BinPath) {
  $destDir = Split-Path -Parent $BinPath
  if (-not $destDir) {
    # Bare filename — fall back to current working directory.
    $destDir = (Get-Location).Path
  }
} else {
  # No arg = Wails cwd, which is `build/bin/`.
  $destDir = (Get-Location).Path
}

if (-not (Test-Path $destDir)) {
  New-Item -ItemType Directory -Path $destDir -Force | Out-Null
}

$dlls = Get-ChildItem -Path $srcDir -Filter '*.dll' -File
if ($dlls.Count -eq 0) {
  Write-Error "No DLLs found in $srcDir"
  exit 1
}

foreach ($dll in $dlls) {
  Copy-Item -Path $dll.FullName -Destination $destDir -Force
  Write-Host "Copied $($dll.Name) -> $destDir"
}
