---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "README.md"
ref: HEAD
---

# Assets MPQ importer

[![Testing](https://github.com/dimonp/assets_mpq_importer/actions/workflows/test.yml/badge.svg)](https://github.com/dimonp/assets_mpq_importer/actions/workflows/test.yml)
[![Test Coverage](https://github.com/dimonp/assets_mpq_importer/blob/gh-pages/coverage.svg?raw=true)](https://dimonp.github.io/assets_mpq_importer/)
[![CodeQL](https://github.com/dimonp/assets_mpq_importer/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/dimonp/assets_mpq_importer/actions/workflows/codeql-analysis.yml)

## About

The `Assets MPQ importer` is a toolset for extracting and converting assets from Warcraft III MPQ archives. It provides utilities to convert BLP textures to modern formats (PNG/DDS), MDX models to OBJ meshes, and extract map data files from Warcraft III maps.

The initial goal of the project is to extract and prepare resources for a warcraft 3 terrain project written for the Godot 4 engine.

This project consists of two main tools:

1. **importer** - Extracts and converts files from MPQ archives
2. **merger** - Processes and merges terrain cliffs and ramps 3D mesh files with transformations

## Features

### Importer

- Extract files from Warcraft III MPQ archives
- Convert BLP textures to PNG or DDS (BC1, BC3, BC7) with mipmaps
- Convert MDX models to Wavefront OBJ format
- Extract W3E, SHD, WPM, and DOO map files from W3M/W3X maps
- Flexible filtering options for selective extraction
- Support for multiple compression backends ([Nvidia Texture Tools](https://github.com/castano/nvidia-texture-tools) and [AMD Compressionator](https://github.com/GPUOpen-Tools/Compressonator))

### Merger

- Process cliff and ramp mesh files from OBJ format
- Apply transformations and scaling to meshes
- Split ramp meshes into appropriate parts
- Generate a multigroup mesh file with JSON metadata
  for later use in the W3Map renderer.

## Dependencies

### Necessary Dependencies

1. A C++ compiler that supports C++23
2. [CMake 3.31+](https://cmake.org/)

### Optional Dependencies

- [Doxygen](http://doxygen.nl/) for documentation generation
- [ccache](https://ccache.dev/) for faster rebuilds
- [Cppcheck](http://cppcheck.sourceforge.net/) for static analysis
- [include-what-you-use](https://include-what-you-use.org/) for include optimization

For a complete list of dependencies and installation instructions, see [Dependencies](README_dependencies.md).

## Build Instructions

### Quick Build

```bash
cmake -S . -B ./build -DCMAKE_TOOLCHAIN_FILE=<vcpkg/installation/folder>/scripts/buildsystems/vcpkg.cmake
cmake --build ./build
```

### Docker Build

If you have Docker installed, you can build and run the project using:

```bash
docker build -f ./.devcontainer/Dockerfile --tag=assets_mpq_importer:latest .
docker run -it assets_mpq_importer:latest
```

For more detailed build instructions, see [Building Details](README_building.md).

## Usage

### Importer usage

The importer tool extracts and converts files from MPQ archives:

```bash
# Basic usage
./importer -i path/to/archive.mpq -o output/directory

# Convert BLP textures to DDS with BC7 compression
./importer -i path/to/archive.mpq -o output/directory --dds --compression=bc7

# Extract specific file types
./importer -i path/to/archive.mpq -o output/directory --filter="*.mdx"

# Convert BLP textures to PNG
./importer -i path/to/archive.mpq -o output/directory --filter="*.blp"
```

### Merger usage

The merger tool processes and merges mesh files:

```bash
# Process cliff meshes
./merger -i input/directory -o output/directory -n ground -p "Cliffs([a-zA-Z0-9]{5})\.obj"

# Process ramp meshes and append to existing JSON
./merger -r -a -i input/directory -o output/directory -n ground -p "CliffTrans([a-zA-Z0-9]{5})\.obj"
```

### Complete Workflow Script

For a complete workflow of extracting and converting Warcraft III assets, use the provided scripts:

```bash
# Linux/macOS
./scripts/mpq_import.sh path/to/war3.mpq output/directory

# Windows
scripts\mpq_import.bat path/to/war3.mpq output/directory
```

## Documentation

For more information on specific aspects of the project:

- [Dependency Setup](README_dependencies.md)
- [Building Details](README_building.md)
- [Docker](README_docker.md)
