---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "tools/bench_phase22.ps1"
ref: HEAD
---

param(
    [string]$Exe = "build\vjassc.exe",
    [Alias("Input")]
    [string]$InputPath = "samples\input.j",
    [int]$Warmup = 1,
    [int]$Repeat = 7,
    [string]$OutDir = "build\phase22-bench",
    [switch]$RecordedOrder,
    [switch]$Parallel,
    [int]$Workers = 4,
    [switch]$Incremental,
    [string]$CacheDir = "build\.vjassc-phase22-cache",
    [string]$Report = "build\phase22-benchmark-report.json"
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

function Scenario-Name {
    $parts = @()
    if ($RecordedOrder) { $parts += "recorded-order" }
    if ($Parallel) { $parts += "parallel" }
    if ($Incremental) { $parts += "incremental-nochange" }
    if ($parts.Count -eq 0) { return "default-fast" }
    return ($parts -join "+")
}

function Build-Args([string]$OutPath, [string]$PerfPath, [string]$IncReportPath) {
    $cmdArgs = @(
        $InputPath,
        "-o", $OutPath,
        "--mode", "fast",
        "--emit-performance-report", $PerfPath
    )
    if ($RecordedOrder) {
        $cmdArgs += "--experimental-recorded-order"
    }
    if ($Parallel) {
        $cmdArgs += @("--experimental-parallel-lowering", "--parallel-workers", "$Workers")
    }
    if ($Incremental) {
        $cmdArgs += @(
            "--experimental-incremental-cache", $CacheDir,
            "--incremental-mode", "reuse",
            "--emit-incremental-report", $IncReportPath
        )
    }
    return $cmdArgs
}

function Run-One([int]$Index, [bool]$IsWarmup) {
    $suffix = if ($IsWarmup) { "warmup$Index" } else { "run$Index" }
    $outPath = Join-Path $OutDir "phase22.$suffix.out.j"
    $perfPath = Join-Path $OutDir "phase22.$suffix.performance.json"
    $incPath = Join-Path $OutDir "phase22.$suffix.incremental.json"
    $cmdArgs = Build-Args $outPath $perfPath $incPath

    $sw = [Diagnostics.Stopwatch]::StartNew()
    & $Exe @cmdArgs
    $exit = $LASTEXITCODE
    $sw.Stop()
    if ($exit -ne 0) {
        throw "vjassc failed with exit code $exit on $suffix"
    }

    $perf = if (Test-Path $perfPath) { Get-Content $perfPath -Raw | ConvertFrom-Json } else { $null }
    $inc = if (Test-Path $incPath) { Get-Content $incPath -Raw | ConvertFrom-Json } else { $null }
    return [pscustomobject]@{
        index = $Index
        warmup = $IsWarmup
        totalMs = [long]$sw.ElapsedMilliseconds
        codegenMs = if ($perf) { [long]$perf.timingMs.codegen } else { 0 }
        reusePercent = if ($inc) { [double]$inc.incremental.reusePercent } else { $null }
        output = $outPath
        performanceReport = $perfPath
        incrementalReport = if ($Incremental) { $incPath } else { $null }
    }
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Report) | Out-Null
$Exe = (Resolve-Path $Exe).Path

if ($Incremental) {
    New-Item -ItemType Directory -Force -Path $CacheDir | Out-Null
    $primeOut = Join-Path $OutDir "phase22.cache-prime.out.j"
    $primePerf = Join-Path $OutDir "phase22.cache-prime.performance.json"
    $primeInc = Join-Path $OutDir "phase22.cache-prime.incremental.json"
    & $Exe @(Build-Args $primeOut $primePerf $primeInc)
    if ($LASTEXITCODE -ne 0) {
        throw "vjassc failed while priming incremental cache"
    }
}

for ($i = 1; $i -le $Warmup; $i++) {
    Run-One $i $true | Out-Null
}

$runs = @()
for ($i = 1; $i -le $Repeat; $i++) {
    $runs += Run-One $i $false
}

$totals = [long[]]($runs | ForEach-Object { $_.totalMs })
$codegen = [long[]]($runs | ForEach-Object { $_.codegenMs })
$reuse = @($runs | Where-Object { $null -ne $_.reusePercent } | ForEach-Object { $_.reusePercent })
$summary = [pscustomobject]@{
    phase = 22
    kind = "benchmark-report"
    scenario = Scenario-Name
    warmup = $Warmup
    repeat = $Repeat
    recordedOrder = [bool]$RecordedOrder
    parallel = [pscustomobject]@{
        enabled = [bool]$Parallel
        workers = if ($Parallel) { $Workers } else { 0 }
    }
    incremental = [pscustomobject]@{
        enabled = [bool]$Incremental
        cacheDir = if ($Incremental) { $CacheDir } else { "" }
        reusePercentMedian = if ($reuse.Count -gt 0) { [double](PercentileValue ([long[]]($reuse | ForEach-Object { [long]$_ })) 50) } else { $null }
    }
    totalMs = [pscustomobject]@{
        min = [long](($totals | Measure-Object -Minimum).Minimum)
        median = PercentileValue $totals 50
        p90 = PercentileValue $totals 90
        max = [long](($totals | Measure-Object -Maximum).Maximum)
    }
    codegenMs = [pscustomobject]@{
        median = PercentileValue $codegen 50
    }
    runs = $runs
}

$summary | ConvertTo-Json -Depth 8 | Set-Content -Encoding UTF8 $Report
Write-Host "phase22 benchmark report: $Report"
