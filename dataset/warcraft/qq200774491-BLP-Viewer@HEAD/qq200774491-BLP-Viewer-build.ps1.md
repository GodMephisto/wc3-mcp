---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "build.ps1"
ref: HEAD
---

param(
    [ValidateSet("Debug", "Release", "RelWithDebInfo", "MinSizeRel")]
    [string]$Config = "Release",
    [string]$BuildDir = "",
    [string]$Generator = ""
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = $scriptRoot
if ([string]::IsNullOrWhiteSpace($BuildDir)) {
    $BuildDir = Join-Path $repoRoot "build"
} elseif (-not [System.IO.Path]::IsPathRooted($BuildDir)) {
    $BuildDir = Join-Path $repoRoot $BuildDir
}

function Require-Command {
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Missing required command: $Name"
    }
}

Require-Command "cmake"

Write-Host "Build dir: $BuildDir"

New-Item -ItemType Directory -Force $BuildDir | Out-Null
Push-Location $repoRoot

$cmakeArgs = @("-S", $repoRoot, "-B", $BuildDir, "-DCMAKE_BUILD_TYPE=$Config")
if (-not [string]::IsNullOrWhiteSpace($Generator)) {
    $cmakeArgs += @("-G", $Generator)
}

Write-Host "Configuring CMake..."
& cmake @cmakeArgs
if ($LASTEXITCODE -ne 0) {
    Pop-Location
    throw "CMake configure failed"
}

Write-Host "Building..."
& cmake --build $BuildDir --config $Config
Pop-Location
if ($LASTEXITCODE -ne 0) {
    throw "CMake build failed"
}
