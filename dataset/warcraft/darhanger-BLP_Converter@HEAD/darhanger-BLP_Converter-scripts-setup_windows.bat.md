---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "scripts/setup_windows.bat"
ref: HEAD
---

@echo off
setlocal EnableExtensions

cd /d "%~dp0\.."
set "ROOT_DIR=%CD%"

echo.
echo [2/5] Preparing folders and project assets...

if not exist "%ROOT_DIR%\build" mkdir "%ROOT_DIR%\build"
if not exist "%ROOT_DIR%\dist" mkdir "%ROOT_DIR%\dist"
if not exist "%ROOT_DIR%\external" mkdir "%ROOT_DIR%\external"

if exist "%ROOT_DIR%\.gitmodules" (
    echo Updating Git submodules...
    git submodule update --init --recursive
    if errorlevel 1 (
        echo [ERROR] Failed to update Git submodules.
        if not defined DARBLP_PARENT_BUILD pause
        endlocal
        exit /b 1
    )
) else (
    echo No Git submodules configured.
)

echo CMake FetchContent dependencies, if added later, will be downloaded during CMake configure.

if not exist "%ROOT_DIR%\resources\icons" mkdir "%ROOT_DIR%\resources\icons"
if not exist "%ROOT_DIR%\resources\icons\app.png" call :create_placeholder_icons
if not exist "%ROOT_DIR%\resources\icons\app.ico" call :create_placeholder_icons

if exist "%ROOT_DIR%\resources\icons\app.png" (
    echo [OK] resources\icons\app.png
) else (
    echo [WARN] resources\icons\app.png is missing.
)

if exist "%ROOT_DIR%\resources\icons\app.ico" (
    echo [OK] resources\icons\app.ico
) else (
    echo [WARN] resources\icons\app.ico is missing.
)

echo [OK] Setup complete.
endlocal
exit /b 0

:create_placeholder_icons
echo Creating placeholder app icons...
powershell -NoProfile -ExecutionPolicy Bypass -Command "Add-Type -AssemblyName System.Drawing; $d = Join-Path (Get-Location) 'resources\icons'; New-Item -ItemType Directory -Force -Path $d | Out-Null; $png = Join-Path $d 'app.png'; $ico = Join-Path $d 'app.ico'; $s = 256; $bmp = New-Object System.Drawing.Bitmap $s, $s; $g = [System.Drawing.Graphics]::FromImage($bmp); $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; $g.Clear([System.Drawing.Color]::FromArgb(22,24,29)); $orange = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(242,142,38)); $yellow = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(250,215,96)); $dark = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(22,24,29)); $font = New-Object System.Drawing.Font 'Segoe UI', 28, ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel); $fmt = New-Object System.Drawing.StringFormat; $fmt.Alignment = [System.Drawing.StringAlignment]::Center; $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center; $g.FillEllipse($orange, 24, 24, 208, 208); $g.FillEllipse($yellow, 58, 58, 92, 92); $g.FillRectangle($dark, 84, 118, 108, 34); $g.DrawString('BLP', $font, $dark, (New-Object System.Drawing.RectangleF 0, 160, 256, 44), $fmt); $bmp.Save($png, [System.Drawing.Imaging.ImageFormat]::Png); $icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon()); $fs = New-Object System.IO.FileStream $ico, ([System.IO.FileMode]::Create); $icon.Save($fs); $fs.Close(); $icon.Dispose(); $g.Dispose(); $bmp.Dispose();"
if errorlevel 1 echo [WARN] Could not create placeholder icons automatically.
goto :eof
