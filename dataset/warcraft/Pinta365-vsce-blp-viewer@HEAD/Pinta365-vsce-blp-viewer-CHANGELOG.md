---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "CHANGELOG.md"
ref: HEAD
---

# Changelog

## [Unreleased]

## [0.0.5] - 2026-04-18

### Added
- BLP1 file support in preview (both JPEG and palettized RAW1 content)
- BLP version information in the metadata panel

### Changed
- Updated `@pinta365/blp` dependency to `^0.2.0` to use the new BLP1-capable decoder
- Metadata mapping now handles both BLP1 and BLP2 header structures
- README now documents BLP1 and BLP2 support in the feature and format lists

## [0.0.4] - 2026-03-10

### Added
- Image previews (BLP preview and metadata) in README

## [0.0.3] - 2026-03-10

### Added
- Metadata panel in preview showing format, alpha depth, mipmap count, and file size
- GitHub release notes now include commits since the previous tag

### Changed
- Replaced simple dimensions status text with a full metadata table below the image

## [0.0.2] - 2026-03-10

### Added
- MIT license
- `license` and `repository` fields in `package.json`

### Fixed
- Release script now creates annotated git tags so they are correctly pushed
- `.vscodeignore` tightened to exclude dev files and dependency samples from VSIX
- Automated marketplace publishing via GitHub Actions on release tags

## [0.0.1] - 2026-03-10

### Added
- Initial release
- BLP2 file preview via custom editor
- Support for DXT1, DXT3, DXT5, RAW1 (palettized), and RAW3 (uncompressed) formats
- Displays image dimensions
- GitHub Actions workflow to build and publish `.vsix` on release tags
- Release script (`npm run release`) to bump version, finalize changelog, and push tag
