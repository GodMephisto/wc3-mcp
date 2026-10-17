---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "README.md"
ref: HEAD
---

<!-- markdownlint-disable MD004 MD033 -->
<div align="center">

# DarBLP Converter

DarBLP Converter is a modern Qt 6 desktop tool scaffold for converting texture files used by World of Warcraft and Warcraft III workflows.

![C++](https://img.shields.io/badge/C%2B%2B-00599C?style=flat-square&logo=c%2B%2B&logoColor=white)
[![License](https://img.shields.io/github/license/darhanger/BLP_Converter?style=flat-square)](https://github.com/darhanger/BLP_Converter/blob/main/License)
[![Last Release](https://img.shields.io/github/v/release/darhanger/BLP_Converter?style=flat-square)](https://github.com/darhanger/BLP_Converter)
[![GitHub All Releases](https://img.shields.io/github/downloads/darhanger/BLP_Converter/total?style=flat-square)](https://github.com/darhanger/BLP_Converter/releases)
[![Discord Server](https://img.shields.io/badge/Discord-7289DA?style=flat-square&logo=discord&logoColor=white)](https://discord.gg/ZKFkvrzaU4)
[![PayPal](https://img.shields.io/badge/PayPal-00457C?style=flat-square&logo=paypal&logoColor=white)](https://www.paypal.com/donate/?hosted_button_id=WMPGGC32C7U7U)
</div>
<br>

The current project is structured so the GUI, CLI, and codec layers are separate. PNG and TGA loading, preview, and conversion are wired through Qt image codecs. BLP2 support currently includes header parsing and metadata display; DXT payload decoding and BLP2 writing are isolated in TODO codec modules ready for the next implementation pass.

## Quick Start Windows

```bat
build.bat
```

If Qt is installed outside PATH, pass the Qt MSVC kit folder:

```bat
build.bat "C:\Qt\6.7.0\msvc2022_64"
```

The deployed app is written to `dist\DarBLPConverter\DarBLPConverter.exe`.
The deployed executable is a Windows GUI app and does not open a separate command prompt on launch.

More details: `docs\WINDOWS_BUILD.md`.

## Supported Formats

- PNG to TGA
- TGA to PNG
- PNG/TGA to BLP route prepared through `BlpWriter`
- BLP to PNG/TGA route prepared through `BlpReader`
- BLP2 header reading for metadata

Planned BLP work:

- BLP2 DXT1/DXT3/DXT5 decoding
- BLP2 DXT1/DXT3/DXT5 encoding
- Alpha-preserving BLP export
- Full mipmap payload generation

## GUI Features

- Dark Qt 6 interface with modern spacing, hover states, and rounded panels
- Drag and drop for files and folders
- Recursive folder import for `.png`, `.tga`, and `.blp`
- Model-backed file list with per-file status
- Texture preview with checkerboard transparency background
- Color, alpha, and mipmap preview modes for loaded PNG/TGA images
- Output settings for PNG, TGA, and BLP
- Batch conversion on a worker thread
- Progress bar and operation log

## CLI Examples

```bat
DarBLPConverter.exe convert input.png output.blp
DarBLPConverter.exe convert input.blp output.png
DarBLPConverter.exe batch .\input .\output --to blp --compression dxt5 --mipmaps
DarBLPConverter.exe batch .\input .\output --to png --keep-structure
```

Use `DarBLPConverter.exe --help` for command syntax.

## BLP Status

The codebase is ready for BLP2 codec development, but the DXT payload implementation is intentionally not completed in this scaffold. The following files contain the codec boundary:

- `src/blp/BlpReader.cpp`
- `src/blp/BlpWriter.cpp`
- `src/blp/BlpDecoder.cpp`
- `src/blp/BlpEncoder.cpp`
- `src/blp/DxtCodec.cpp`
- `src/blp/MipmapGenerator.cpp`

## Roadmap

1. Decode BLP2 DXT headers and mip offsets into `ImageData`.
2. Implement DXT1 and DXT5 block decompression.
3. Export BLP to PNG/TGA.
4. Import PNG/TGA to BLP2.
5. Add DXT3 and high-quality mipmap payload generation.
6. Add regression tests using sample textures.
