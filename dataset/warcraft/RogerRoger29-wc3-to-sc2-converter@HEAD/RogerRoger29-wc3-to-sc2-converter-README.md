---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "README.md"
ref: HEAD
---

# WC3 to SC2 Model Converter

Convert **Warcraft 3 `.mdx` models (classic format, version 800)** into **StarCraft 2 `.m3` models**, preserving
mesh, skeleton, *all* animations, textures, team colour, particle emitters, hit-test volume and portrait camera.

It works on any WC3 v800 model: material, texture, team-colour and animation settings are **derived from the MDX
itself** (not hardcoded). A complete worked example — the **Naaru** — is included so you can test immediately.

There is **no third-party WC3 importer** in the pipeline. A hand-written MDX parser builds the scene directly in
Blender, then the **m3studio** addon writes the `.m3`.

---

## Quick Start

**Double-click `wc3toSC2.exe`**, drag your `.mdx` file onto the window, and click **Convert All**. Everything else is automatic — scale estimation, texture finding, animation matching, and Blender management.

For CLI use:
```
python convert.py examples/Naaru/Naaru.json
```

---

## What it produces

| Aspect | Handling |
|---|---|
| Geometry + UVs | 1:1 (WC3 and Blender are both Z-up); V flipped; edge-split for hard facet normals |
| Skinning | rigid (WC3 matrix groups to vertex groups, equal weights) |
| Skeleton + animations | every `SEQS` baked per-frame from `KGTR/KGRT/KGSC` (world-matrix accurate, auto-FPS) |
| Materials | blend mode, two-sided, unshaded — all derived from WC3 `filterMode` + `shadingFlags` |
| Team colour | WC3 replaceable textures (team colour / team glow) to SC2 `TEAMEMIS` emissive, with optional UV mask |
| Textures | each `.blp` to `.dds` (DXT5 + full mip chain); particle textures get additive-glow treatment |
| Animated fades | material alpha `KMTA` x geoset alpha `KGAO` baked into the diffuse (e.g. death dissolve) |
| Particles | `PRE2` emitters to SC2 particle systems, with `KP2V` emission gating |
| Bounds / hit-test | from `MODL` bounds (a tight sphere so the unit is selectable) |
| Portrait camera | from `CAMS` |

---

## Requirements

For the **.exe** version: none. Just double-click.

For the Python/CLI version:
1. **Blender 4.4** (tested on 4.4.3). The converter auto-detects the best frame rate per animation.
2. **m3studio addon** by Solstice245: https://github.com/Solstice245/m3studio
3. **Python 3.8+** with `pip install numpy pillow`

---

## GUI Features

The `.exe` provides a full graphical interface with:

- **Drag and drop** `.mdx` files onto the window
- **Model preview** via Blender headless render
- **Batch conversion** of entire folders
- **One-click Blender setup** — downloads and installs everything automatically
- **18 configurable settings** across animation quality, LOD, team color, normals, and pipeline options
- **Session recovery** — never lose your queue if the program closes
- **Auto-update** checking on startup
- **SC2 actor XML** generation for direct editor import

---

## Settings Reference

All features can be toggled in the GUI Settings tab:

| Group | Settings |
|---|---|
| Blender | Path, auto-detect, one-click download |
| Scale & Output | Scale, particle rate/size, output directory |
| Animation Quality | FPS mode (Auto/30/60/15/10), squad interpolation, keyframe reduction |
| Mesh & LOD | No LOD / LOD1 / LOD1+LOD2 |
| Team Color | TEAMEMIS (diffuse mask) / UV Mask (per-geoset) / Off |
| PBR & Normals | Normal map generation, strength |
| Pipeline | Multi-threaded textures, MDX cache, auto-alpha, auto-scale, fuzzy anims |
| Output | Actor XML, HTML report |

---

## Files

| File | Role |
|---|---|
| `main_window.py` | **double-click this** — PySide6 GUI with drag-drop, preview, one-click setup |
| `convert.py` | CLI orchestrator (config-driven or quick mode) |
| `mdx.py` | Warcraft 3 MDX (v800) parser |
| `blp.py` | BLP1 texture decoder (JPEG and palettized) |
| `textures.py` | BLP/image to DDS DXT5 with mip chain and particle glow treatment |
| `build_m3.py` | runs inside Blender: builds the scene and exports via m3studio |
| `diagnostics.py` | pre-flight checks: degenerate faces, bones, textures, particles |
| `healer.py` | auto-fixes: inverted alpha, bone hierarchy, particle lifespans |
| `discovery.py` | auto-detects textures, Blender path, optimal scale |
| `fuzzy_anims.py` | fuzzy-matches WC3 animation names to SC2 tokens |
| `presets/` | ready-to-use conversion profiles (human units, buildings, effects) |
| `config.example.json` | config template for CLI mode |
| `examples/Naaru/` | worked example with model and textures |

---

## Limitations

- **Format scope:** classic WC3 MDX **version 800**, and **BLP1** textures (JPEG-content and palettized).
- **Particles are approximate.** WC3 and SC2 particle systems are not 1:1; emitters are mapped sensibly but may need fine-tuning with `particle_rate_scale` and `particle_size_scale`.
- **Rotation interpolation** uses spherical cubic interpolation (squad) for hermite/bezier curves, preserving authored cubic motion. Linear tracks use slerp.
- **WC3 BLP-JPEG alpha is stored inverted**, and the decoder accounts for that. If transparency comes out backwards, toggle "Auto-detect inverted alpha" in Settings.
- **Textures must exist on disk** or be extractable from WC3 MPQ archives.

---

## Credits

- **m3studio** (M3 import/export) by **Solstice245** — https://github.com/Solstice245/m3studio
- Pipeline built and validated by porting the **Naaru** (a community WC3 model) to StarCraft 2.
