---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "scripts/deploy_windows.bat"
ref: HEAD
---

@echo off
setlocal EnableExtensions

cd /d "%~dp0\.."
set "ROOT_DIR=%CD%"
set "QT_PATH=%~1"
set "DIST_DIR=%ROOT_DIR%\dist\DarBLPConverter"
set "DIST_EXE=%DIST_DIR%\DarBLPConverter.exe"

echo.
echo [4/5] Deploying Qt runtime...

if not "%QT_PATH%"=="" (
    set "PATH=%QT_PATH%\bin;%PATH%"
)

set "BUILT_EXE="
if exist "%ROOT_DIR%\build\Release\DarBLPConverter.exe" set "BUILT_EXE=%ROOT_DIR%\build\Release\DarBLPConverter.exe"
if not defined BUILT_EXE if exist "%ROOT_DIR%\build\DarBLPConverter.exe" set "BUILT_EXE=%ROOT_DIR%\build\DarBLPConverter.exe"
if not defined BUILT_EXE (
    for /f "delims=" %%F in ('dir /b /s "%ROOT_DIR%\build\DarBLPConverter.exe" 2^>nul') do (
        if not defined BUILT_EXE set "BUILT_EXE=%%F"
    )
)

if not defined BUILT_EXE (
    echo [ERROR] Built executable was not found in build directory.
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

if not exist "%DIST_DIR%" mkdir "%DIST_DIR%"
copy /Y "%BUILT_EXE%" "%DIST_EXE%" >nul
if errorlevel 1 (
    echo [ERROR] Failed to copy executable to dist.
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

set "WINDEPLOYQT="
if not "%QT_PATH%"=="" if exist "%QT_PATH%\bin\windeployqt.exe" set "WINDEPLOYQT=%QT_PATH%\bin\windeployqt.exe"
if not defined WINDEPLOYQT (
    for /f "delims=" %%W in ('where windeployqt 2^>nul') do (
        if not defined WINDEPLOYQT set "WINDEPLOYQT=%%W"
    )
)

if not defined WINDEPLOYQT (
    echo [ERROR] windeployqt was not found.
    echo Add Qt bin to PATH or run:
    echo   build.bat "C:\Qt\6.7.0\msvc2022_64"
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

echo Using windeployqt:
echo %WINDEPLOYQT%
"%WINDEPLOYQT%" --release --compiler-runtime "%DIST_EXE%"
if errorlevel 1 (
    echo [ERROR] windeployqt failed.
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

if exist "%ROOT_DIR%\README.md" copy /Y "%ROOT_DIR%\README.md" "%DIST_DIR%\README.md" >nul
if exist "%ROOT_DIR%\LICENSE" copy /Y "%ROOT_DIR%\LICENSE" "%DIST_DIR%\LICENSE" >nul

echo.
echo [OK] Deploy complete.
echo %DIST_EXE%
endlocal
exit /b 0
