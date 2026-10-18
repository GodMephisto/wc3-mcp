---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "README.md"
ref: HEAD
---

# BLP Converter

[![Release](https://img.shields.io/github/v/release/Kkthnx/BLPConverter?style=flat-square&label=release)](https://github.com/Kkthnx/BLPConverter/releases/latest)

A compact desktop app for converting World of Warcraft BLP textures to and from PNG.

## What it does

- Two-panel drag and drop: BLP on the left becomes PNG, PNG on the right becomes BLP.
- Batch converts multiple files or whole folders in parallel with a Rust backend.
- PNG to BLP with RAW, DXT1, DXT3, or DXT5 compression, mipmap generation, and power-of-two validation.
- Optional BLPView Explorer thumbnail provider for `.blp` files on Windows.
- Runs on Windows, Linux, and macOS (Tauri 2), with the interface in 8 languages.

## Get it

Download the installer for your OS from the [Releases page](https://github.com/Kkthnx/BLPConverter/releases/latest):

| Platform | Files |
|----------|-------|
| Windows 10/11 | `*-setup.exe` (NSIS, recommended), `*.msi` |
| Linux | `*.deb`, `*.AppImage` |
| macOS | `*.dmg` (Intel + Apple Silicon) |

No admin needed for normal use on Windows. The BLPView thumbnail install writes to the current user registry only.

## Quick start

1. Install and launch BLP Converter.
2. Drop `.blp` files on the left panel to export PNG.
3. Drop `.png` files on the right panel to encode BLP.
4. Open Settings (gear icon) to set language, output folder, compression, and mipmaps.

Converted files land beside the originals unless you pick a different output folder.

## Support the work

Free forever. If it saved you time, chip in:

- [Ko-fi](https://ko-fi.com/kkthnx)
- [Buy Me a Coffee](https://buymeacoffee.com/kkthnx)
- [PayPal](https://www.paypal.com/paypalme/kkthnxtv)

Every bit goes back into building more.

## Community

- Glitch Haven Discord: https://discord.gg/FaVGhTYmck
- Website: https://kkthnx.com

## License

Copyright (c) 2026 Kkthnx. All rights reserved. You can download and run the official released builds for personal use. The source is published for reference, not for redistribution or reuse without written permission. See [LICENSE](LICENSE).
