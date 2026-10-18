---
type: vendor-doc
source_repo: "https://github.com/wiselencave/war3-pfx-boilerplate"
source_path: "runWorldEditor.bat"
ref: HEAD
---

@echo off
for /f "tokens=2*" %%a in ('reg query "HKLM\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Warcraft III" /v "InstallLocation" 2^>nul') do set WC3_PATH=%%b
if not defined WC3_PATH (
    echo Warcraft III not found in registry
    pause
    exit /b 1
)
start "" "%WC3_PATH%\_retail_\x86_64\World Editor.exe" -launch -loadfile "%~dp0map.w3x"
