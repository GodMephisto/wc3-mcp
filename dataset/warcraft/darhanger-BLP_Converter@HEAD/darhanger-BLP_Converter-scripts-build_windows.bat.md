---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "scripts/build_windows.bat"
ref: HEAD
---

@echo off
setlocal EnableExtensions

cd /d "%~dp0\.."
set "ROOT_DIR=%CD%"
set "QT_PATH=%~1"

echo.
echo [3/5] Configuring and building DarBLP Converter...

if not "%QT_PATH%"=="" (
    set "Qt6_DIR=%QT_PATH%\lib\cmake\Qt6"
    set "PATH=%QT_PATH%\bin;%PATH%"
)

call :add_default_paths
call :ensure_msvc
if errorlevel 1 (
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

if not exist "%ROOT_DIR%\build" mkdir "%ROOT_DIR%\build"

if defined Qt6_DIR (
    echo Using Qt6_DIR=%Qt6_DIR%
    cmake -S "%ROOT_DIR%" -B "%ROOT_DIR%\build" -G "Visual Studio 17 2022" -A x64 "-DQt6_DIR=%Qt6_DIR%"
) else (
    cmake -S "%ROOT_DIR%" -B "%ROOT_DIR%\build" -G "Visual Studio 17 2022" -A x64
)

if errorlevel 1 (
    echo [ERROR] CMake configure failed.
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

cmake --build "%ROOT_DIR%\build" --config Release
if errorlevel 1 (
    echo [ERROR] Release build failed.
    if not defined DARBLP_PARENT_BUILD pause
    endlocal
    exit /b 1
)

echo [OK] Build complete.
endlocal
exit /b 0

:add_default_paths
if exist "%ProgramFiles%\CMake\bin" set "PATH=%ProgramFiles%\CMake\bin;%PATH%"
if exist "%ProgramFiles%\Git\cmd" set "PATH=%ProgramFiles%\Git\cmd;%PATH%"
goto :eof

:ensure_msvc
where cl >nul 2>nul
if not errorlevel 1 (
    echo [OK] MSVC compiler is already active.
    goto :eof
)

call :find_vsdevcmd
if not defined VSDEVCMD (
    echo [ERROR] Visual Studio 2022 Build Tools were not found.
    echo Install them with the C++ workload or run scripts\check_requirements.bat.
    exit /b 1
)

echo Activating MSVC environment:
echo %VSDEVCMD%
call "%VSDEVCMD%" -arch=x64 -host_arch=x64
if errorlevel 1 (
    echo [ERROR] Failed to activate Visual Studio developer environment.
    exit /b 1
)

where cl >nul 2>nul
if errorlevel 1 (
    echo [ERROR] cl.exe is still unavailable after activating MSVC.
    exit /b 1
)

exit /b 0

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
