---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "pyproject.toml"
ref: HEAD
---

[build-system]
requires = ["setuptools>=61.0", "wheel"]
build-backend = "setuptools.backends._legacy:_Backend"

[project]
name = "wc3toSC2"
version = "2.1.0"
description = "Convert Warcraft 3 .mdx models to StarCraft 2 .m3 models — with premium GUI"
readme = "README.md"
license = {text = "MIT"}
requires-python = ">=3.8"
dependencies = [
    "numpy>=1.24.0",
    "Pillow>=10.0.0",
]

[project.optional-dependencies]
gui = ["PySide6>=6.5.0"]
dev = ["pytest>=7.0", "ruff"]

[project.scripts]
wc3toSC2 = "convert:main"
wc3toSC2-gui = "main_window:main"

[project.urls]
Homepage = "https://github.com/RogerRoger29/wc3-to-sc2-converter"
Issues = "https://github.com/RogerRoger29/wc3-to-sc2-converter/issues"

[tool.setuptools]
py-modules = [
    "convert", "mdx", "blp", "textures", "build_m3",
    "discovery", "diagnostics", "healer", "fuzzy_anims",
    "mpq_reader", "actor_gen", "main_window",
    "blender_manager", "auto_updater", "team_color_mask", "normal_map_gen",
]

[tool.setuptools.package-data]
"*" = ["config_schema.json", "config.example.json"]
