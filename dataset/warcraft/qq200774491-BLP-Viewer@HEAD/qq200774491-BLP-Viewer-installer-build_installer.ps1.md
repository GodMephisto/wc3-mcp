---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "installer/build_installer.ps1"
ref: HEAD
---

param(
    [ValidateSet("Debug", "Release", "RelWithDebInfo", "MinSizeRel")]
    [string]$Config = "Release"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$guiRoot = Resolve-Path (Join-Path $scriptRoot "..")
$buildDir = Join-Path $guiRoot "build\$Config"
$stageDir = Join-Path $scriptRoot "stage"
$issPath = Join-Path $scriptRoot "blp_viewer.iss"

if (-not (Test-Path -LiteralPath $buildDir)) {
    throw "Build output not found: $buildDir"
}

Remove-Item -LiteralPath $stageDir -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $stageDir | Out-Null

$exePath = Join-Path $buildDir "blp_viewer.exe"
if (-not (Test-Path -LiteralPath $exePath)) {
    throw "Missing blp_viewer.exe in $buildDir"
}

Copy-Item -LiteralPath $exePath -Destination $stageDir -Force

$thumbnailDll = Join-Path $buildDir "blp_thumbnail.dll"
if (Test-Path -LiteralPath $thumbnailDll) {
    Copy-Item -LiteralPath $thumbnailDll -Destination $stageDir -Force
}

$isccPath = (Get-Command "iscc" -ErrorAction SilentlyContinue).Source
if (-not $isccPath) {
    $isccPath = @(
        "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles(x86)\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    ) | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}
if (-not $isccPath) {
    Write-Warning "Inno Setup (iscc) not found. Install it to build the installer."
    Write-Host "Stage prepared at: $stageDir"
    exit 0
}

Write-Host "Building installer..."
& $isccPath $issPath
if ($LASTEXITCODE -ne 0) {
    throw "Installer build failed"
}

Write-Host "Done. Output in $scriptRoot\dist"
