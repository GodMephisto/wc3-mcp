---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "CHANGELOG.md"
ref: HEAD
---

# Changelog

All notable changes to BLP Converter are documented in this file.

## [1.2.1] - 2026-08-29

### Fixed
- **BLPView (Windows)**, Restart Explorer now clears the thumbnail cache after Explorer is down, so refreshing thumbnails actually takes effect. It was clearing while Explorer still held the cache files open.

### Changed
- **BLPView (Windows)**, the thumbnail provider now catches panics at the COM boundary and returns an error instead of unwinding, so a malformed .blp cannot take down Explorer. Release builds use unwind to support this.

## [1.2.0] - 2026-08-29

### Changed
- Relicensed to All Rights Reserved, updated the LICENSE file, README, and crate and package metadata.
- Refreshed the interface to a blue and silver palette with better text contrast, and regenerated every app icon to match.

### Added
- Indeterminate progress bar in the header while a batch is converting.
- Visible keyboard focus rings for better accessibility.

### Fixed
- Keep the document language attribute in sync with the selected locale.
- Version the saved settings store so future setting changes can migrate cleanly.

## [1.1.3] - 2026-08-03

### Fixed
- **Responsiveness**, scanning and conversion now run on a blocking worker thread instead of the main thread, so large batches and dropped folders no longer freeze the window or drag-and-drop.

### Changed
- Normalized user-facing text and docs to plain punctuation across all 8 locales and fixed a Korean export-label typo.

## [1.1.2] - 2026-06-23

### Fixed
- **BLPView thumbnails (Windows)**, detect broken legacy machine-wide BLPView registrations that block Explorer thumbnails.
- **BLPView install (Windows)**, install the thumbnail DLL beside the app executable for more reliable Explorer loading.
- **BLPView install (Windows)**, register full `SystemFileAssociations\.blp` metadata (`PerceivedType`, `Content Type`, `Application`).
- **BLPView restart (Windows)**, clear additional Explorer icon/thumbnail cache files and notify shell image updates.
- **BLPView thumbnail provider**, default thumbnail size when Explorer passes `cx = 0`.

## [1.1.1] - 2026-06-23

### Fixed
- **BLPView thumbnails (Windows)**, enable `DisableProcessIsolation` in release installs so Explorer can load the thumbnail handler reliably.
- **BLPView thumbnails (Windows)**, register the handler under `Explorer\FileExts\.blp\ShellEx` for Windows 10/11 compatibility.
- **BLPView thumbnails (Windows)**, clear Explorer thumbnail cache when restarting Explorer from Settings.
- **BLPView status (Windows)**, detect incomplete installs (missing DLL, approval list, or isolation flag) and prompt reinstall.
- **Release CI (Windows)**, fix intermittent `build.rs` file-lock error when bundling `blpview_thumb.dll`.

### Changed
- **BLPView (Linux/macOS)**, show the Settings section greyed out with a translated explanation instead of hiding it.

## [1.1.0] - 2026-06-23

### Added
- Cross-platform builds for Windows, Linux, and macOS via GitHub Actions.
- i18n for 8 languages (en, de, fr, es, pt-BR, ru, zh-CN, ko).
- DXT3 compression option and improved alpha-aware BLP encoding.
- Persisted conversion settings (compression, mipmaps, output folder).
- BLPView Windows Explorer thumbnail shell extension.

### Fixed
- Cross-platform path handling in conversion unit tests.
- Custom output folder filename collisions when batch converting.
- BLPView BGRA/alpha handling, registry cleanup, and uninstall behavior.

## [1.0.0] - 2026-06-23

### Added
- Initial release, BLP and PNG conversion with drag-and-drop UI.

[1.2.1]: https://github.com/Kkthnx/BLPConverter/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/Kkthnx/BLPConverter/compare/v1.1.3...v1.2.0
[1.1.3]: https://github.com/Kkthnx/BLPConverter/compare/v1.1.2...v1.1.3
[1.1.2]: https://github.com/Kkthnx/BLPConverter/compare/v1.1.1...v1.1.2
[1.1.1]: https://github.com/Kkthnx/BLPConverter/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/Kkthnx/BLPConverter/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Kkthnx/BLPConverter/releases/tag/v1.0.0
