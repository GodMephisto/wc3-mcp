---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "scripts/check_requirements.bat"
ref: HEAD
---

@echo off
setlocal EnableExtensions

cd /d "%~dp0\.."
set "ROOT_DIR=%CD%"
set "QT_PATH=%~1"
set "INSTALL_ATTEMPTED=0"

if not "%QT_PATH%"=="" (
    set "Qt6_DIR=%QT_PATH%\lib\cmake\Qt6"
    set "PATH=%QT_PATH%\bin;%PATH%"
)

call :add_default_paths

:check_again
set "ERRORS=0"
set "MISSING_WINGET_IDS="
set "MISSING_WINGET_NAMES="
set "QT_MISSING=0"

echo.
echo [1/5] Checking Windows build requirements...
echo.

call :check_command git "Git" "Git.Git"
call :check_command cmake "CMake" "Kitware.CMake"
call :check_msvc
call :check_qt
call :check_windeployqt

if "%ERRORS%"=="0" (
    echo.
    echo [OK] All required tools are available.
    endlocal
    exit /b 0
)

echo.
echo [ERROR] Missing or incomplete build requirements were found.

if not "%MISSING_WINGET_IDS%"=="" if "%INSTALL_ATTEMPTED%"=="0" (
    call :offer_winget
    if errorlevel 1 goto manual_instructions
    set "INSTALL_ATTEMPTED=1"
    call :add_default_paths
    goto check_again
)

goto manual_instructions

:check_command
where %~1 >nul 2>nul
if errorlevel 1 (
    echo [MISS] %~2 was not found.
    call :mark_winget "%~2" "%~3"
    goto :eof
)

echo [OK] %~2:
"%~1" --version 2>nul
goto :eof

:check_msvc
where cl >nul 2>nul
if not errorlevel 1 (
    echo [OK] MSVC compiler:
    cl 2>&1 | findstr /I "Version" 2>nul
    goto :eof
)

call :find_vsdevcmd
if defined VSDEVCMD (
    echo [OK] Visual Studio Build Tools found.
    echo      cl.exe is not on PATH yet; build_windows.bat will activate:
    echo      %VSDEVCMD%
    goto :eof
)

echo [MISS] MSVC compiler / Visual Studio Build Tools were not found.
call :mark_winget "Visual Studio 2022 Build Tools" "Microsoft.VisualStudio.2022.BuildTools"
goto :eof

:check_qt
if not "%QT_PATH%"=="" (
    if exist "%QT_PATH%\lib\cmake\Qt6\Qt6Config.cmake" (
        echo [OK] Qt6_DIR from build.bat argument:
        echo      %QT_PATH%\lib\cmake\Qt6
        goto :eof
    )
)

if defined Qt6_DIR (
    if exist "%Qt6_DIR%\Qt6Config.cmake" (
        echo [OK] Qt6_DIR:
        echo      %Qt6_DIR%
        goto :eof
    )
)

where qmake >nul 2>nul
if errorlevel 1 goto qt_missing

set "QMAKE_QT_VERSION="
for /f "delims=" %%Q in ('qmake -query QT_VERSION 2^>nul') do set "QMAKE_QT_VERSION=%%Q"
if "%QMAKE_QT_VERSION:~0,2%"=="6." (
    echo [OK] Qt qmake:
    qmake --version
    goto :eof
)
echo [MISS] qmake was found, but it does not report Qt 6.

:qt_missing
echo [MISS] Qt 6 was not found.
echo        Qt is not installed automatically because the required MSVC 2022 64-bit
echo        component is best selected through the official Qt Online Installer.
set "QT_MISSING=1"
set /a ERRORS+=1
goto :eof

:check_windeployqt
where windeployqt >nul 2>nul
if not errorlevel 1 (
    echo [OK] windeployqt:
    windeployqt --version
    goto :eof
)

if not "%QT_PATH%"=="" if exist "%QT_PATH%\bin\windeployqt.exe" (
    echo [OK] windeployqt:
    echo      %QT_PATH%\bin\windeployqt.exe
    goto :eof
)

echo [MISS] windeployqt was not found.
set "QT_MISSING=1"
set /a ERRORS+=1
goto :eof

:mark_winget
set /a ERRORS+=1
set "MISSING_WINGET_NAMES=%MISSING_WINGET_NAMES% %~1;"
set "MISSING_WINGET_IDS=%MISSING_WINGET_IDS% %~2"
goto :eof

:offer_winget
where winget >nul 2>nul
if errorlevel 1 (
    echo.
    echo winget was not found. Install missing dependencies manually.
    exit /b 1
)

echo.
echo The following dependencies can be installed with winget:
echo %MISSING_WINGET_NAMES%
echo.
choice /C YN /M "Install missing dependencies with winget?"
if errorlevel 2 exit /b 1

for %%P in (%MISSING_WINGET_IDS%) do (
    if /I "%%P"=="Microsoft.VisualStudio.2022.BuildTools" (
        echo.
        echo Installing Visual Studio 2022 Build Tools with C++ workload...
        winget install --id %%P -e --source winget --override "--wait --passive --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended"
    ) else (
        echo.
        echo Installing %%P...
        winget install --id %%P -e --source winget
    )
    if errorlevel 1 (
        echo [ERROR] winget failed while installing %%P.
        exit /b 1
    )
)

echo.
echo winget installation finished. Re-checking requirements...
exit /b 0

:manual_instructions
echo.
echo Manual installation instructions:
echo.
echo Git:
echo   winget install --id Git.Git -e --source winget
echo.
echo CMake:
echo   winget install --id Kitware.CMake -e --source winget
echo.
echo Visual Studio Build Tools:
echo   winget install --id Microsoft.VisualStudio.2022.BuildTools -e --source winget --override "--wait --passive --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended"
echo.
echo Qt 6:
echo   1. Download Qt Online Installer from https://www.qt.io/download
echo   2. Install Qt 6.x MSVC 2022 64-bit
echo   3. Set:
echo      Qt6_DIR=C:\Qt\6.x.x\msvc2022_64\lib\cmake\Qt6
echo      PATH=C:\Qt\6.x.x\msvc2022_64\bin;%%PATH%%
echo   4. Verify:
echo      windeployqt --version
echo.
echo You can also run:
echo   build.bat "C:\Qt\6.7.0\msvc2022_64"
echo.
if not defined DARBLP_PARENT_BUILD pause
endlocal
exit /b 1

:add_default_paths
if exist "%ProgramFiles%\CMake\bin" set "PATH=%ProgramFiles%\CMake\bin;%PATH%"
if exist "%ProgramFiles%\Git\cmd" set "PATH=%ProgramFiles%\Git\cmd;%PATH%"
goto :eof

:find_vsdevcmd
set "VSDEVCMD="
where vswhere >nul 2>nul
if errorlevel 1 goto skip_vswhere

for /f "delims=" %%I in ('vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath 2^>nul') do (
    if exist "%%I\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=%%I\Common7\Tools\VsDevCmd.bat"
)

:skip_vswhere
if defined VSDEVCMD goto :eof
if exist "C:\PROGRA~2\Microsoft Visual Studio\2022\BuildTools\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=C:\PROGRA~2\Microsoft Visual Studio\2022\BuildTools\Common7\Tools\VsDevCmd.bat"
if exist "C:\PROGRA~2\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=C:\PROGRA~2\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
if exist "C:\PROGRA~2\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=C:\PROGRA~2\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsDevCmd.bat"
if exist "C:\PROGRA~2\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=C:\PROGRA~2\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat"
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\BuildTools\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=%ProgramFiles%\Microsoft Visual Studio\2022\BuildTools\Common7\Tools\VsDevCmd.bat"
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsDevCmd.bat"
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat" set "VSDEVCMD=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat"
goto :eof
