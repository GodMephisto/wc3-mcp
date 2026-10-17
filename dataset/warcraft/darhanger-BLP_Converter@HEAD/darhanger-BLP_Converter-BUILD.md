---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "BUILD.md"
ref: HEAD
---

# Build Instructions

## Requirements

- Windows 10 or newer
- CMake 3.21+
- C++17 compiler
- Qt 6 with `Core`, `Gui`, `Widgets`, and `Concurrent`

## Build With CMake

```bat
cmake -S . -B build -G "Visual Studio 17 2022" -A x64
cmake --build build --config Release
```

The executable is named `DarBLPConverter.exe`.
On Windows it is built as a GUI subsystem executable, so launching the app does not open an extra console window.

For the full Windows one-command flow, see `docs\WINDOWS_BUILD.md` or run:

```bat
build.bat
```

## Build With Qt Creator

1. Open `CMakeLists.txt`.
2. Select a Qt 6 kit.
3. Configure the project.
4. Build and run `DarBLPConverter.exe`.

## Build With Visual Studio

1. Open the folder in Visual Studio.
2. Make sure Qt 6 is visible through `CMAKE_PREFIX_PATH` or your environment.
3. Configure CMake.
4. Build the `darblp` CMake target. The produced executable is `DarBLPConverter.exe`.

If CMake cannot find Qt, pass the Qt install prefix:

```bat
cmake -S . -B build -G "Visual Studio 17 2022" -A x64 -DCMAKE_PREFIX_PATH=C:\Qt\6.7.0\msvc2022_64
```
