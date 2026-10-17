---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "docs/WINDOWS_BUILD.md"
ref: HEAD
---

# Windows Build Guide

DarBLP Converter is prepared for Windows 10/11 builds through one entry point:

```bat
build.bat
```

If Qt is installed but CMake cannot find it, pass the Qt MSVC kit path:

```bat
build.bat "C:\Qt\6.7.0\msvc2022_64"
```

The script writes the final application to:

```text
dist\DarBLPConverter\DarBLPConverter.exe
```

The Windows target is configured as a GUI subsystem executable. Running the deployed app opens only the DarBLP Converter window and does not spawn a separate command prompt.

## What build.bat Does

1. Runs `scripts\check_requirements.bat`.
2. Runs `scripts\setup_windows.bat`.
3. Runs `scripts\build_windows.bat`.
4. Runs `scripts\deploy_windows.bat`.
5. Prints the final EXE and `dist` paths.

## Required Tools

- Git
- CMake 3.21 or newer
- Visual Studio 2022 Build Tools with the C++ workload
- Qt 6.x MSVC 2022 64-bit
- `windeployqt`

## Automatic Installation With winget

If Git, CMake, or Visual Studio Build Tools are missing, `check_requirements.bat` checks for `winget` and asks:

```text
Install missing dependencies with winget? [Y/N]
```

Nothing is installed without confirmation.

The script may install:

```bat
winget install --id Git.Git -e --source winget
winget install --id Kitware.CMake -e --source winget
winget install --id Microsoft.VisualStudio.2022.BuildTools -e --source winget --override "--wait --passive --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended"
```

Node.js is not required and is not installed.

## Installing Qt 6

Qt is handled separately because the required component is specific: Qt 6.x MSVC 2022 64-bit.

Recommended setup:

1. Download Qt Online Installer from `https://www.qt.io/download`.
2. Install `Qt 6.x` with the `MSVC 2022 64-bit` component.
3. Add these variables for the current terminal, adjusting the version:

```bat
set "Qt6_DIR=C:\Qt\6.x.x\msvc2022_64\lib\cmake\Qt6"
set "PATH=C:\Qt\6.x.x\msvc2022_64\bin;%PATH%"
```

For permanent variables, use Windows Environment Variables settings and add the Qt `bin` folder to PATH.

Verify:

```bat
windeployqt --version
```

Alternative one-time build:

```bat
build.bat "C:\Qt\6.7.0\msvc2022_64"
```

This passes the following option to CMake:

```bat
-DQt6_DIR="C:\Qt\6.7.0\msvc2022_64\lib\cmake\Qt6"
```

## Manual Build Commands

```bat
scripts\check_requirements.bat
scripts\setup_windows.bat
scripts\build_windows.bat "C:\Qt\6.7.0\msvc2022_64"
scripts\deploy_windows.bat "C:\Qt\6.7.0\msvc2022_64"
```

## Troubleshooting

If `cl.exe` is not found, install Visual Studio 2022 Build Tools with the C++ workload. The build script can activate MSVC automatically when Build Tools are installed.

If CMake cannot find Qt, use:

```bat
build.bat "C:\Qt\6.7.0\msvc2022_64"
```

If `windeployqt` is missing, add the Qt `bin` folder to PATH or pass the Qt path to `build.bat`.

If deployment succeeds, run:

```bat
dist\DarBLPConverter\DarBLPConverter.exe
```
