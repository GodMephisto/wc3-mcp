---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "CONTRIBUTING.md"
ref: HEAD
---

# Contributing to wc3toSC2

Thanks for contributing! This document covers the development workflow.

## Setup

```bash
git clone https://github.com/user/wc3toSC2.git
cd wc3toSC2
pip install -r requirements.txt
pip install -e ".[dev]"
```

You also need **Blender 4.4** with the **m3studio addon** installed (see README).

## Development

### Running tests

```bash
pytest tests/ -v
```

### Running the converter

```bash
# Test with the Naaru example
python convert.py examples/Naaru/Naaru.json --verbose

# Quick mode
python convert.py examples/Naaru/Naaru.mdx out/
```

### Code style

- Python 3.8+ compatible (no f-string `=` debugging, no `|` union syntax in runtime code)
- Type hints on all public function signatures
- Four-space indentation
- Docstrings for public modules and functions

### Project structure

| File | Role |
|---|---|
| `convert.py` | CLI orchestrator (run this) |
| `mdx.py` | MDX v800 binary parser |
| `blp.py` | BLP1 texture decoder |
| `textures.py` | BLP → DDS DXT5 converter |
| `build_m3.py` | Blender headless M3 builder |
| `tests/` | pytest test suite |

### Pull request checklist

- [ ] Tests pass: `pytest tests/ -v`
- [ ] Type hints are present on new functions
- [ ] New features are documented in README.md
- [ ] The Naaru example still converts successfully
- [ ] No hardcoded paths in tracked files

## Release process

1. Update version in `pyproject.toml`
2. Update `CHANGELOG.md`
3. Tag: `git tag vX.Y.Z`
4. Build executable: `pyinstaller wc3toSC2.spec`
5. Create GitHub release with the `.exe` artifact
