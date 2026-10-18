---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "tools/bench_phase21.ps1"
ref: HEAD
---

param(
    [string]$Exe = "build\vjassc.exe",
    [Alias("Input")]
    [string]$InputPath = "samples\input.j",
    [string]$Mode = "fast",
    [string[]]$ExtraArgs = @(),
    [int]$Warmup = 1,
    [int]$Repeat = 7,
    [string]$OutDir = "build",
    [string]$Report = "build\phase21.benchmark.report.json"
)

$ErrorActionPreference = "Stop"

function PercentileValue([long[]]$Values, [double]$Percentile) {
    if ($Values.Count -eq 0) {
        return 0
    }
    $sorted = $Values | Sort-Object
    $index = [Math]::Ceiling(($Percentile / 100.0) * $sorted.Count) - 1
    if ($index -lt 0) { $index = 0 }
    if ($index -ge $sorted.Count) { $index = $sorted.Count - 1 }
    return [long]$sorted[$index]
}

function Run-One([int]$Index, [bool]$IsWarmup) {
    $suffix = if ($IsWarmup) { "warmup$Index" } else { "run$Index" }
    $outPath = Join-Path $OutDir "phase21.$suffix.out.j"
    $perfPath = Join-Path $OutDir "phase21.$suffix.performance.json"
    $cmdArgs = @(
        $InputPath,
        "-o", $outPath,
        "--mode", $Mode,
        "--emit-performance-report", $perfPath
    ) + $ExtraArgs

    $sw = [Diagnostics.Stopwatch]::StartNew()
    & $Exe @cmdArgs
    $exit = $LASTEXITCODE
    $sw.Stop()
    if ($exit -ne 0) {
        throw "vjassc failed with exit code $exit on $suffix"
    }
    return [pscustomobject]@{
        index = $Index
        warmup = $IsWarmup
        totalMs = [long]$sw.ElapsedMilliseconds
        output = $outPath
        performanceReport = $perfPath
    }
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Report) | Out-Null

$Exe = (Resolve-Path $Exe).Path

for ($i = 1; $i -le $Warmup; $i++) {
    Run-One $i $true | Out-Null
}

$runs = @()
for ($i = 1; $i -le $Repeat; $i++) {
    $runs += Run-One $i $false
}

$totals = [long[]]($runs | ForEach-Object { $_.totalMs })
$summary = [pscustomobject]@{
    phase = 21
    kind = "benchmark-report"
    mode = $Mode
    warmup = $Warmup
    repeat = $Repeat
    extraArgs = $ExtraArgs
    totalMs = [pscustomobject]@{
        min = [long](($totals | Measure-Object -Minimum).Minimum)
        median = PercentileValue $totals 50
        p90 = PercentileValue $totals 90
        max = [long](($totals | Measure-Object -Maximum).Maximum)
    }
    runs = $runs
}

$summary | ConvertTo-Json -Depth 8 | Set-Content -Encoding UTF8 $Report
Write-Host "phase21 benchmark report: $Report"
