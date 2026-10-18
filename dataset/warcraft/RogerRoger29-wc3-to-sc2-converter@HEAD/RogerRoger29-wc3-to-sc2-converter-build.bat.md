---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "build.bat"
ref: HEAD
---

@echo off
echo === WC3 to SC2 Converter - Build Script ===
echo.

echo [1/4] Cleaning stale Python processes...
taskkill /F /IM python.exe >nul 2>&1
echo       Done.

echo [2/4] Running tests...
python -m pytest tests/ -q --tb=line
if %ERRORLEVEL% NEQ 0 (
    echo       TESTS FAILED - aborting build
    exit /b 1
)
echo       All tests passed.

echo [3/4] Building executable...
python -m PyInstaller wc3toSC2.spec --noconfirm
if %ERRORLEVEL% NEQ 0 (
    echo       BUILD FAILED
    exit /b 1
)
echo       Build complete.

echo [4/4] Verifying...
python -c "import os; s=os.path.getsize('dist/wc3toSC2.exe'); print(f'      wc3toSC2.exe: {s/1024/1024:.1f} MB')"

echo.
echo === Build successful ===
echo Output: dist\wc3toSC2.exe
