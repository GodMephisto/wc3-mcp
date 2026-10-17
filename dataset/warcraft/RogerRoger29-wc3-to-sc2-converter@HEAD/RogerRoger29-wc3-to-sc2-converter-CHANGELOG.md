---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "CHANGELOG.md"
ref: HEAD
---

# Changelog

All notable changes to the WC3 to SC2 Model Converter.

## [1.0.0] — Unreleased

### Added
- Initial release of the WC3 → SC2 model converter
- Pure-Python MDX v800 binary parser with full chunk support
- BLP1 texture decoder (JPEG + palettized)
- DDS DXT5 converter with full mip chain and particle glow treatment
- CLI orchestrator with JSON config and quick modes
- Blender headless M3 builder via m3studio addon
- Complete Naaru example model
- Configurable team colour emissive layer
- PRE2 particle emitter conversion with KP2V emission gating
- KMTA × KGAO animated fade baking
- Portrait camera from CAMS
- Hit-test tight sphere from MODL bounds
- Argument parser with --verbose and --quiet flags
- Structured logging throughout
- Type hints on all public function signatures
- JSON schema for config validation
- Comprehensive pytest test suite (36 tests)
- pyproject.toml with setuptools packaging
- PyInstaller .spec for single .exe distribution
- GitHub Actions CI/CD workflows (lint + test + build)
- Cross-platform Blender auto-detection (Windows/macOS/Linux)
- Binary search keyframe lookup (was O(n) linear)
- Dynamic PRE2 emitter parsing with KG track support
- ATCH attachment path/ID and KATV visibility parsing
- EVTS event object parsing
- Blender version guard (>= 4.4)
- m3studio addon validation with clear error messages
- MIT license
- CONTRIBUTING.md developer guide

### Fixed
- Removed hardcoded F: drive from Blender search paths
- Removed hardcoded Blender paths from example configs
- All silent `except: pass` blocks now report warnings
- Texture conversion failures caught and reported gracefully
