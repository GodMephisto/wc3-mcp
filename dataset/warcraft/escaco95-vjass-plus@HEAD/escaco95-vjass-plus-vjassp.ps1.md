---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-plus"
source_path: "vjassp.ps1"
ref: HEAD
---

param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $ArgsFromCaller
)

$Required = '3.12'
$Vjassp = Join-Path $PSScriptRoot 'vjassp.py'

if (-not (Test-Path $Vjassp)) {
    Write-Error "vjassp.py not found: $Vjassp"
    exit 2
}

function Get-PythonMajorMinor([string] $exePath) {
    try {
        $out = & $exePath -c 'import sys; sys.stdout.write(str(sys.version_info[0])+\".\"+str(sys.version_info[1]))'
        return $out.Trim()
    } catch {
        return $null
    }
}

function Test-Python312([string] $exePath) {
    $mm = Get-PythonMajorMinor $exePath
    return ($mm -eq $Required)
}

$chosen = $null

# 1) Try PATH python
if (Test-Python312 'python') {
    Write-Host "Using python from PATH"
    $chosen = 'python'
}

# 2) Search recursively under current working directory if not found
if (-not $chosen) {
    Get-ChildItem -Path (Get-Location) -Recurse -File -Filter 'python.exe' -ErrorAction SilentlyContinue |
        ForEach-Object {
            Write-Host "Found Python executable: $($_.FullName)"
            if (Test-Python312 $_.FullName) {
                $chosen = $_.FullName
                break
            }
        }
}

if (-not $chosen) {
    Write-Error "Python $Required not found in PATH or under current directory."
    exit 1
}

# -------------------------------------------------
# 인자 처리: 첫 번째 인자가 유효 파일이 아니면 main.jp 검색
# -------------------------------------------------
$needSearch = $true
if ($ArgsFromCaller.Count -gt 0) {
    $firstArg = $ArgsFromCaller[0]
    if (Test-Path -LiteralPath $firstArg -PathType Leaf) {
        $needSearch = $false
    }
}

if ($needSearch) {
    $SearchRoot = $PSScriptRoot   # 스크립트 위치 기준 검색
    Write-Host "Searching for main.jp under: $SearchRoot"
    $mainCandidate = Get-ChildItem -Path $SearchRoot -Recurse -File -Filter 'main.jp' -ErrorAction SilentlyContinue |
                     Select-Object -First 1
    if ($mainCandidate) {
        Write-Host "Inserting discovered main.jp at arg[0]: $($mainCandidate.FullName)"
        if (-not $ArgsFromCaller) {
            $ArgsFromCaller = @()
        }
        # 0번 자리에 삽입 (기존 인자 뒤로 이동)
        $ArgsFromCaller = @($mainCandidate.FullName) + $ArgsFromCaller
    } else {
        Write-Host "main.jp not found under script directory; using original arguments."
    }
} else {
    Write-Host "First argument is an existing file: $($ArgsFromCaller[0])"
}

# Run vjassp.py with (가능한 경우 교체된) 인자
& $chosen $Vjassp @ArgsFromCaller
exit $LASTEXITCODE