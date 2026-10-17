---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "README.md"
ref: HEAD
---

# BLP Converter

A compact cross-platform desktop app for converting **World of Warcraft BLP textures to and from PNG**.

Drop files, convert in parallel, and on Windows optionally preview `.blp` files in File Explorer with the built-in BLPView shell extension.

[![Release](https://img.shields.io/github/v/release/Kkthnx/BLPConverter?style=flat-square&label=release)](https://github.com/Kkthnx/BLPConverter/releases/latest)
[![License](https://img.shields.io/badge/license-All%20Rights%20Reserved-blue?style=flat-square)](#license)

---

## Download

Get the latest release from [GitHub Releases](https://github.com/Kkthnx/BLPConverter/releases/latest).

| Platform | Files |
|----------|-------|
| **Windows 10/11** | `*-setup.exe` (NSIS, recommended), `*.msi`, includes optional BLPView Explorer thumbnails |
| **Linux** | `*.deb`, `*.AppImage` |
| **macOS** | `*.dmg` (Intel + Apple Silicon) |

No admin required for normal use on Windows. BLPView thumbnail install writes to the current user registry only.

### Publishing a new release

Push a version tag to trigger the build pipeline (see [RELEASING.md](RELEASING.md)):

```bash
git tag v1.1.0
git push origin v1.1.0
```

GitHub Actions builds all platforms and attaches installers to the release automatically.

---

## Features

- **Two-panel drag and drop**, BLP on the left converts to PNG, PNG on the right converts to BLP
- **Batch conversion**, drop multiple files or entire folders
- **Parallel processing**, Rust backend with Rayon
- **Smart output**, saves next to source files by default, with an optional custom output folder
- **PNG to BLP options**, RAW, DXT1, DXT3, or DXT5 compression plus mipmap generation
- **Alpha-aware encoding**, DXT formats automatically detect transparency in source PNGs
- **Power-of-two validation**, clear errors when PNG dimensions are invalid for WoW textures
- **8 languages**, English, Deutsch, Français, Español, Português (Brasil), Русский, 简体中文, 한국어
- **BLPView** *(Windows only)*, optional Explorer thumbnail provider for `.blp` files
- **Cross-platform**, Windows, Linux, and macOS via Tauri 2

---

## Usage

1. Launch **BLP Converter**
2. Drop **`.blp`** files on the **left** panel to export PNG
3. Drop **`.png`** files on the **right** panel to encode BLP
4. Open **Settings** (gear icon) to configure language, output folder, compression, and mipmaps

Converted files are written beside the originals unless you choose a different output folder in Settings.

### Compression guide

| Format | Best for |
|--------|----------|
| **DXT5** | Icons, UI, smooth transparency gradients *(default)* |
| **DXT3** | Sharp alpha edges, cut-out textures |
| **DXT1** | Opaque textures or 1-bit punch-through alpha, smallest size |
| **RAW** | Lossless / uncompressed BGRA output |

### Texture requirements (PNG to BLP)

WoW BLP textures require **power-of-two** dimensions (e.g. 64x64, 256x512). Non-conforming images are rejected with a clear error message.

---

## Build from source

### Requirements

- [Node.js](https://nodejs.org/) 18+
- [Rust](https://rustup.rs/) (stable, edition 2021)
- Platform-specific:
  - **Windows**: Visual Studio Build Tools (C++), WebView2
  - **Linux**: `libwebkit2gtk-4.1-dev`, `build-essential`, `libssl-dev`, etc. ([Tauri prerequisites](https://v2.tauri.app/start/prerequisites/))
  - **macOS**: Xcode Command Line Tools

### Commands

```bash
git clone https://github.com/Kkthnx/BLPConverter.git
cd BLPConverter
npm install

# Development
npm run tauri:dev

# Release build (installers in src-tauri/target/release/bundle/)
npm run tauri:build
```

On Windows, `tauri:dev` and `tauri:build` automatically build the BLPView shell extension DLL. On Linux and macOS this step is skipped.

### Other scripts

| Script | Purpose |
|--------|---------|
| `npm run build:icons` | Regenerate app icons *(Windows PowerShell)* |
| `npm run build:shell-ext` | Build BLPView DLL only *(Windows)* |

---

## Tech stack

| Layer | Stack |
|-------|-------|
| Desktop shell | [Tauri 2](https://tauri.app/) |
| Frontend | React 19, TypeScript, Tailwind CSS, Zustand, i18next |
| Backend | Rust, `image-blp`, `rayon`, `walkdir` |
| Shell extension | COM thumbnail provider (`blp-shell-ext`, Windows only) |

### BLP format support

Conversion is powered by the [`image-blp`](https://crates.io/crates/image-blp) crate, which supports:

- **BLP2** (World of Warcraft), RAW1, RAW3, JPEG, DXT1/3/5
- **BLP1** (Warcraft III), read support for palettized and JPEG variants

BLP to PNG exports mipmap level 0 as a transparent PNG. PNG to BLP outputs BLP2 with your chosen compression.

---

## Project structure

```
BLPConverter/
├── src/                 # React UI + i18n locales
├── src-tauri/           # Tauri + Rust conversion engine
├── blp-shell-ext/       # Windows Explorer BLPView DLL
└── scripts/             # Cross-platform build helpers
```

---

## Author

Made by **[Kkthnx](https://github.com/Kkthnx)**, WoW addon developer ([KkthnxUI](https://github.com/Kkthnx-Wow/KkthnxUI) and more).

---

## License

All Rights Reserved. Copyright (c) 2026 Kkthnx. See [LICENSE](LICENSE) for details.

You can download and run the official released builds for personal use. The source is here for reference, not for redistribution or reuse without written permission.
