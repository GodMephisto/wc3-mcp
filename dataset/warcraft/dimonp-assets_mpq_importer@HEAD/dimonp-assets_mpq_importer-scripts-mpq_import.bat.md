---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "scripts/mpq_import.bat"
ref: HEAD
---

@echo off
setlocal enabledelayedexpansion

:: Argument check
if "%~2"=="" (
    echo Usage: %~0 ^<path to input mpq archive^> ^<path to output folder^>
    echo Example: %~0 "path/to/war3.mpq" "output/folder"
    echo.
    echo This script processes files.
    exit /b 1
)

set "INPUT_MPQ=%~1"
set "OUTPUT_DIR=%~2"

:: Find executables
call :find_executable "importer.exe" importer_path
call :find_executable "merger.exe" merger_path

if "%importer_path%"=="" (
    echo Error. Importer executable not found. >&2
    exit /b 1
)
if "%merger_path%"=="" (
    echo Error. Merger executable not found. >&2
    exit /b 1
)

set "compression=BC3"

echo Using importer at: %importer_path%

:: Extract W3E map files
"%importer_path%" --filter="*.w3m" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"
"%importer_path%" --filter="*.w3x" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"

:: Import terrain textures to DDS with mipmaps
"%importer_path%" --dds --compression=%compression% --filter="*TerrainArt*.blp" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"
"%importer_path%" --dds --compression=%compression% --filter="*ReplaceableTextures*Cliff*.blp" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"

:: Import terrain MDX meshes
"%importer_path%" --filter="*Doodads*Terrain*Cliffs*.mdx" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"
"%importer_path%" --filter="*Doodads*Terrain*CliffTrans*.mdx" --input="%INPUT_MPQ%" --output="%OUTPUT_DIR%"

:: Create directory
if not exist "%OUTPUT_DIR%\Geosets" mkdir "%OUTPUT_DIR%\Geosets"

:: Merge ground cliffs and ramps
"%merger_path%" --name="ground" --pattern="Cliffs([a-zA-Z0-9]{5})\.obj" --input="%OUTPUT_DIR%\Doodads\Terrain\Cliffs" --output="%OUTPUT_DIR%\Geosets"
"%merger_path%" --ramp --append --name="ground" --pattern="CliffTrans([a-zA-Z0-9]{5})\.obj" --input="%OUTPUT_DIR%\Doodads\Terrain\CliffTrans" --output="%OUTPUT_DIR%\Geosets"

:: Merge city cliffs and ramps
"%merger_path%" --name="city" --pattern="CityCliffs([a-zA-Z0-9]{5})\.obj" --input="%OUTPUT_DIR%\Doodads\Terrain\CityCliffs" --output="%OUTPUT_DIR%\Geosets"
"%merger_path%" --ramp --append --name="city" --pattern="CityCliffTrans([a-zA-Z0-9]{5})\.obj" --input="%OUTPUT_DIR%\Doodads\Terrain\CityCliffTrans" --output="%OUTPUT_DIR%\Geosets"

:: Clean up temporary files
if exist "%OUTPUT_DIR%\Doodads" rd /s /q "%OUTPUT_DIR%\Doodads"
del /q "%OUTPUT_DIR%\Geosets\*.mtl" 2>nul

echo Done.
exit /b 0

:: Function to find an executable in PATH or current directory recursively
:find_executable
set "%~2="
:: 1. Check if the executable is in the system's PATH
for %%i in (%~1) do set "found=%%~$PATH:i"
if defined found (
    set "%~2=%found%"
    exit /b 0
)
:: 2. Recursive search in the current directory and subdirectories
for /r %%i in (%~1) do (
    if exist "%%i" (
        set "%~2=%%i"
        exit /b 0
    )
)
exit /b 0
