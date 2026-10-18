---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "tools/bench_phase23.ps1"
ref: HEAD
---

param(
    [string]$Exe = "build\vjassc.exe",
    [Alias("Input")]
    [string]$InputPath = "samples\input.j",
    [ValidateSet("default-fast", "body-jobs-single", "parallel", "incremental-nochange", "incremental-smallchange", "validate")]
    [string]$Scenario = "default-fast",
    [string]$SmallChangeInput = "",
    [int]$Warmup = 1,
    [int]$Repeat = 7,
    [int]$Workers = 4,
    [string]$OutDir = "build\phase23-bench",
    [string]$CacheDir = "build\.vjassc-phase23-cache",
    [string]$Report = "build\phase23-benchmark-report.json"
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

function Build-Args([string]$RunInput, [string]$OutPath, [string]$PerfPath, [string]$IncReportPath) {
    $mode = if ($Scenario -eq "validate") { "validate" } else { "fast" }
    $cmdArgs = @(
        $RunInput,
        "-o", $OutPath,
        "--mode", $mode,
        "--emit-performance-report", $PerfPath
    )
    if ($Scenario -eq "body-jobs-single") {
        $cmdArgs += "--experimental-body-jobs-single-thread"
    }
    if ($Scenario -eq "parallel") {
        $cmdArgs += @("--experimental-parallel-lowering", "--parallel-workers", "$Workers")
    }
    if ($Scenario -eq "incremental-nochange" -or $Scenario -eq "incremental-smallchange") {
        $cmdArgs += @(
            "--experimental-incremental-cache", $CacheDir,
            "--incremental-mode", "reuse",
            "--emit-incremental-report", $IncReportPath
        )
    }
    return $cmdArgs
}

function Ensure-SmallChangeInput {
    if ($SmallChangeInput -ne "") {
        return $SmallChangeInput
    }
    $target = Join-Path $OutDir "phase23.smallchange.input.j"
    $text = Get-Content $InputPath -Raw
    if ($text -match "phase23_smallchange_marker") {
        Set-Content -Encoding UTF8 $target $text
        return $target
    }
    $text += "`nfunction phase23_smallchange_marker takes nothing returns nothing`nendfunction`n"
    Set-Content -Encoding UTF8 $target $text
    return $target
}

function Run-One([int]$Index, [bool]$IsWarmup) {
    $suffix = if ($IsWarmup) { "warmup$Index" } else { "run$Index" }
    $runInput = if ($Scenario -eq "incremental-smallchange") { Ensure-SmallChangeInput } else { $InputPath }
    $outPath = Join-Path $OutDir "phase23.$Scenario.$suffix.out.j"
    $perfPath = Join-Path $OutDir "phase23.$Scenario.$suffix.performance.json"
    $incPath = Join-Path $OutDir "phase23.$Scenario.$suffix.incremental.json"
    $cmdArgs = Build-Args $runInput $outPath $perfPath $incPath
    if ($Scenario -eq "incremental-smallchange") {
        $cmdArgs += @("--emit-generated-entity-plan", "$outPath.plan.json")
    }

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
        bodyCacheHits = if ($inc) { [long]$inc.incremental.bodyCacheHits } else { 0 }
        bodyCacheStores = if ($inc) { [long]$inc.incremental.bodyCacheStores } else { 0 }
        reusePercent = if ($inc) { [double]$inc.incremental.reusePercent } else { $null }
        output = $outPath
        performanceReport = $perfPath
        incrementalReport = if ($inc) { $incPath } else { $null }
    }
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Report) | Out-Null
$Exe = (Resolve-Path $Exe).Path

if ($Scenario -eq "incremental-nochange" -or $Scenario -eq "incremental-smallchange") {
    New-Item -ItemType Directory -Force -Path $CacheDir | Out-Null
    $primeOut = Join-Path $OutDir "phase23.cache-prime.out.j"
    $primePerf = Join-Path $OutDir "phase23.cache-prime.performance.json"
    $primeInc = Join-Path $OutDir "phase23.cache-prime.incremental.json"
    & $Exe @(Build-Args $InputPath $primeOut $primePerf $primeInc)
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
$summary = [pscustomobject]@{
    phase = 23
    kind = "benchmark-report"
    scenario = $Scenario
    warmup = $Warmup
    repeat = $Repeat
    workers = if ($Scenario -eq "parallel") { $Workers } else { 0 }
    cacheDir = if ($Scenario -like "incremental-*") { $CacheDir } else { "" }
    totalMs = [pscustomobject]@{
        min = [long](($totals | Measure-Object -Minimum).Minimum)
        median = PercentileValue $totals 50
        p75 = PercentileValue $totals 75
        p90 = PercentileValue $totals 90
        max = [long](($totals | Measure-Object -Maximum).Maximum)
    }
    codegenMs = [pscustomobject]@{
        median = PercentileValue $codegen 50
    }
    bodyCacheHitsMedian = PercentileValue ([long[]]($runs | ForEach-Object { $_.bodyCacheHits })) 50
    bodyCacheStoresMedian = PercentileValue ([long[]]($runs | ForEach-Object { $_.bodyCacheStores })) 50
    runs = $runs
}

$summary | ConvertTo-Json -Depth 8 | Set-Content -Encoding UTF8 $Report
Write-Host "phase23 benchmark report: $Report"
