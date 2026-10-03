# install.ps1, installs or updates wc3-mcp for the current Windows user.
#
#   irm https://raw.githubusercontent.com/GodMephisto/wc3-mcp/main/install.ps1 | iex
#
# It downloads the latest release, checks its SHA-256, unpacks it into
# %LOCALAPPDATA%\Programs\wc3-mcp, puts that folder on your user PATH and sets up every
# supported AI app it finds. No admin rights are needed. Run it again to update.
#
# To pass options, download it first, then run for example
#   .\install.ps1 -Version v0.1.0 -InstallDir D:\Tools\wc3-mcp -NoRegister
# or install a zip you already have, checked against NAME.sha256 beside it when present
#   .\install.ps1 -FromZip .\wc3-mcp-v0.1.0-win-x64.zip
# -NoPath leaves PATH alone, for a portable copy.
# or set $env:WC3_MCP_VERSION, $env:WC3_MCP_DIR or $env:WC3_MCP_NO_REGISTER=1 before the one-liner.

[CmdletBinding()]
param(
    [string]$Version = $(if ($env:WC3_MCP_VERSION) { $env:WC3_MCP_VERSION } else { 'latest' }),
    [string]$InstallDir = $(if ($env:WC3_MCP_DIR) { $env:WC3_MCP_DIR } else { Join-Path $env:LOCALAPPDATA 'Programs\wc3-mcp' }),
    [switch]$NoRegister = [bool]$env:WC3_MCP_NO_REGISTER,
    [string]$FromZip,
    [switch]$NoPath
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$repo = 'GodMephisto/wc3-mcp'

if (-not [Environment]::Is64BitOperatingSystem) { throw 'wc3-mcp needs 64-bit Windows.' }

function Test-Checksum([string]$zipPath, [string]$sumPath) {
    $expected = ((Get-Content -Path $sumPath -Raw) -split '\s+')[0].Trim().ToLowerInvariant()
    $actual = (Get-FileHash -Path $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($expected -ne $actual) { throw "Checksum mismatch for $zipPath. Expected $expected, got $actual." }
    Write-Host 'checksum ok'
}

$work = Join-Path ([IO.Path]::GetTempPath()) ("wc3-mcp-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
    if ($FromZip) {
        $zipPath = (Resolve-Path -Path $FromZip).Path
        Write-Host "installing $zipPath"
        if (Test-Path "$zipPath.sha256") { Test-Checksum $zipPath "$zipPath.sha256" }
        else { Write-Warning "No $([IO.Path]::GetFileName($zipPath)).sha256 beside the zip, so it was not verified." }
    } else {
        # Ask GitHub which release to fetch, and find its zip and checksum.
        $api = if ($Version -eq 'latest') { "https://api.github.com/repos/$repo/releases/latest" }
               else { "https://api.github.com/repos/$repo/releases/tags/$Version" }
        $release = Invoke-RestMethod -Uri $api -Headers @{ 'User-Agent' = 'wc3-mcp-installer' }
        $zip = $release.assets | Where-Object { $_.name -like '*-win-x64.zip' } | Select-Object -First 1
        $sum = $release.assets | Where-Object { $_.name -like '*-win-x64.zip.sha256' } | Select-Object -First 1
        if (-not $zip) { throw "Release $($release.tag_name) has no win-x64 zip." }
        Write-Host "wc3-mcp $($release.tag_name)"

        $zipPath = Join-Path $work $zip.name
        Invoke-WebRequest -Uri $zip.browser_download_url -OutFile $zipPath -UseBasicParsing
        if ($sum) {
            Invoke-WebRequest -Uri $sum.browser_download_url -OutFile "$zipPath.sha256" -UseBasicParsing
            Test-Checksum $zipPath "$zipPath.sha256"
        } else {
            Write-Warning 'This release has no checksum file, so the download was not verified.'
        }
    }

    $unpacked = Join-Path $work 'unpacked'
    Expand-Archive -Path $zipPath -DestinationPath $unpacked
    if (-not (Test-Path (Join-Path $unpacked 'wc3-mcp.exe'))) { throw 'The zip does not contain wc3-mcp.exe.' }

    # Replace the old copy. A running server holds its files open, so say which app to close.
    if (Test-Path $InstallDir) {
        try { Remove-Item -Path (Join-Path $InstallDir '*') -Recurse -Force }
        catch { throw "Could not replace $InstallDir. Close any AI app that is running wc3-mcp, then run this again." }
    } else {
        New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null
    }
    Copy-Item -Path (Join-Path $unpacked '*') -Destination $InstallDir -Recurse -Force
} finally {
    Remove-Item -Path $work -Recurse -Force -ErrorAction SilentlyContinue
}
$exe = Join-Path $InstallDir 'wc3-mcp.exe'
Write-Host "installed to $InstallDir"

# Put the folder on the user PATH once, so wc3-mcp works in any new terminal.
# PATH is read and written raw, keeping its registry type, so entries such as
# %USERPROFILE%\go\bin stay as written instead of being expanded into fixed paths.
$envKey = Get-Item -Path 'HKCU:\Environment'
$userPath = $envKey.GetValue('Path', '', 'DoNotExpandEnvironmentNames')
$kind = if ($envKey.GetValueNames() -contains 'Path') { $envKey.GetValueKind('Path') } else { 'ExpandString' }
if ($kind -ne 'String') { $kind = 'ExpandString' }
$parts = @($userPath -split ';' | Where-Object { $_ })
$present = $parts | Where-Object { [Environment]::ExpandEnvironmentVariables($_).TrimEnd('\') -ieq $InstallDir.TrimEnd('\') }
if ($NoPath) {
    Write-Host 'left your PATH unchanged (-NoPath)'
} elseif (-not $present) {
    Set-ItemProperty -Path 'HKCU:\Environment' -Name Path -Value (($parts + $InstallDir) -join ';') -Type $kind
    # Tell Explorer the environment changed, so new terminals see it.
    Add-Type -Namespace Wc3Mcp -Name Native -MemberDefinition '[DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr SendMessageTimeout(IntPtr h, uint m, UIntPtr w, string l, uint f, uint t, out UIntPtr r);'
    $r = [UIntPtr]::Zero
    [void][Wc3Mcp.Native]::SendMessageTimeout([IntPtr]0xFFFF, 0x1A, [UIntPtr]::Zero, 'Environment', 2, 5000, [ref]$r)
    Write-Host 'added to your PATH (open a new terminal to use it)'
}
$env:Path = "$env:Path;$InstallDir"

if ($NoRegister) {
    Write-Host 'Skipped app setup. Run  wc3-mcp install --all  when ready.'
} else {
    # PATH was handled above (or skipped with -NoPath), so the exe leaves it alone.
    & $exe install --all --no-path
}
Write-Host ''
& $exe doctor
