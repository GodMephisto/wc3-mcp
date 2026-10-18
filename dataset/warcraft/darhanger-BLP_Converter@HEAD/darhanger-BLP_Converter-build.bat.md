---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "build.bat"
ref: HEAD
---

@echo off
setlocal EnableExtensions

cd /d "%~dp0"
set "ROOT_DIR=%CD%"
set "QT_PATH=%~1"
set "DARBLP_PARENT_BUILD=1"

if not "%QT_PATH%"=="" (
    if not exist "%QT_PATH%\lib\cmake\Qt6" (
        echo [WARN] Qt path was provided, but Qt6 CMake files were not found:
        echo        %QT_PATH%\lib\cmake\Qt6
    )
    set "Qt6_DIR=%QT_PATH%\lib\cmake\Qt6"
    set "PATH=%QT_PATH%\bin;%PATH%"
)

echo.
echo ============================================================
echo DarBLP Converter Windows build
echo ============================================================
echo Root: %ROOT_DIR%
if not "%QT_PATH%"=="" echo Qt:   %QT_PATH%
echo.

call "%ROOT_DIR%\scripts\check_requirements.bat" "%QT_PATH%"
if errorlevel 1 goto failed

call "%ROOT_DIR%\scripts\setup_windows.bat" "%QT_PATH%"
if errorlevel 1 goto failed

call "%ROOT_DIR%\scripts\build_windows.bat" "%QT_PATH%"
if errorlevel 1 goto failed

call "%ROOT_DIR%\scripts\deploy_windows.bat" "%QT_PATH%"
if errorlevel 1 goto failed

set "DIST_DIR=%ROOT_DIR%\dist\DarBLPConverter"
set "EXE_PATH=%DIST_DIR%\DarBLPConverter.exe"

echo.
echo ============================================================
echo Build succeeded
echo ============================================================
echo EXE:  %EXE_PATH%
echo DIST: %DIST_DIR%
echo.
echo You can run the application from the EXE path above.
echo.
pause
endlocal
exit /b 0

:failed
set "EXIT_CODE=%ERRORLEVEL%"
if "%EXIT_CODE%"=="0" set "EXIT_CODE=1"
echo.
echo ============================================================
echo Build failed
echo ============================================================
echo Exit code: %EXIT_CODE%
echo.
echo What to try next:
echo   1. Read docs\WINDOWS_BUILD.md
echo   2. Install Qt 6.x MSVC 2022 64-bit if Qt is missing
echo   3. Re-run: build.bat "C:\Qt\6.7.0\msvc2022_64"
echo.
pause
endlocal
exit /b %EXIT_CODE%
