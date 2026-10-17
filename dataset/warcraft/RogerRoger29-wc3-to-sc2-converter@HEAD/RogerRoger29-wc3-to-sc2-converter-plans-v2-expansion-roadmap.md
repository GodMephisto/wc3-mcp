---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "plans/v2-expansion-roadmap.md"
ref: HEAD
---

# WC3 → SC2 Converter — v2.0 Expansion Roadmap

## Vision

Transform the current CLI pipeline into a **self-contained, self-correcting executable** that handles virtually any WC3 model automatically — detecting and fixing issues without user intervention, while still exposing power-user controls when needed.

---

## Pillar 1: Zero-Config Executable Experience

### 1.1 Drag-and-Drop EXE

The executable accepts `.mdx` files dropped onto it. No config file needed.

**Architecture:**
```
wc3toSC2.exe MyModel.mdx
  → auto-detects all textures in same folder / Textures/ subfolder
  → estimates optimal scale from model bounds
  → detects Blender installation (or uses bundled portable)
  → converts everything → outputs MyModel.m3 + all .dds into out/
  → opens the output folder when done
```

**Implementation:**
- New `discovery.py` module: scans for companion `.blp`/`.png`/`.tga` files near the `.mdx`, searches parent directories, common WC3 texture paths, and optionally MPQ archives.
- New `scale_estimator.py`: analyzes `MODL` bounds and `SEQS` movement speed to recommend scale. Uses heuristics table (hero ≈ 0.04, building ≈ 0.06, doodad ≈ 0.03).
- PyInstaller bundles Python runtime + numpy + Pillow + all `.py` files into single `.exe`.
- Optional: bundle a stripped Blender 4.4 portable (~120MB compressed) with m3studio pre-installed, so the user needs nothing else. Fallback: download-on-demand.

### 1.2 Guided First-Run Wizard

If the `.exe` is run without arguments, launch an interactive wizard:

```
====================================
 WC3 to SC2 Model Converter v2.0
====================================

  [1] Convert a single model (drag .mdx here)
  [2] Convert a folder of models
  [3] Convert from a WC3 MPQ archive
  [4] Settings & diagnostics
  [5] Exit

  Choice: _
```

Each option walks the user through with plain-language prompts and file picker dialogs (tkinter or native Windows).

### 1.3 Portable Blender Bundle

**Option A — embedded:** Ship a minimal Blender 4.4 portable (no UI, no Python packages besides m3studio). ~120MB compressed, ~300MB extracted. The `.exe` extracts on first run.

**Option B — download-on-demand:** If Blender isn't found, offer to download and set it up:
```
Blender 4.4 not found. Download and install automatically? (Y/n)
→ downloads blender-4.4.0-windows-x64.zip
→ extracts to %APPDATA%/wc3toSC2/blender/
→ installs m3studio addon programmatically
```

**Option C — config-based:** Keep current auto-detect + user config. Best for existing Blender users.

---

## Pillar 2: Self-Diagnostic Engine

### 2.1 Pre-Flight Analyzer (`diagnostics.py`)

Runs before conversion. Produces a `REPORT.json` and optional `REPORT.html`.

**Checks performed:**

| Check | What it detects | Auto-fix |
|---|---|---|
| MDX validity | Bad magic, truncated chunks, version mismatch | Reject with clear error |
| Texture resolution | Missing BLP files, broken references | Search broader paths; generate placeholder |
| Texture format | BLP2 (WoW) vs BLP1, corrupted JPEG headers | Reject BLP2; attempt JPEG repair |
| Alpha channel | Inverted alpha (common WC3 quirk) | Auto-detect via histogram analysis → auto-invert |
| UV range | UVs outside [0,1] (tiling issues in SC2) | Warn; clamp option |
| Zero-area faces | Degenerate triangles | Remove or warn |
| Zero-length normals | Broken normals | Regenerate from face normals |
| Orphaned bones | Bones with no vertices, no children, no tracks | Remove or flag |
| Missing root bone | No single root in hierarchy | Auto-create root |
| Bone scale | Zero or extreme scale values | Clamp to sane range |
| Animation range | SEQS with 0-length or overlapping ranges | Flag; merge option |
| Material refs | Geoset referencing missing material ID | Fallback to material 0 |
| Particle size | Extreme particle sizes relative to model | Auto-clamp |
| Emission rate | Zero or near-zero emission rates | Warn; suggest particle_rate_scale |

### 2.2 Post-Conversion Validator

After Blender exports the `.m3`, validate the output:

- **File integrity:** verify `.m3` is parseable, not truncated
- **Texture references:** all referenced `.dds` paths exist in output
- **Size sanity:** model size within SC2 engine limits (warn if > 50MB)
- **Animation count:** verify all SEQS produced M3 animation groups
- **Bone count:** verify skeleton didn't lose bones in export
- **Material count:** verify all materials made it through

### 2.3 Conversion Report

Generate a human-readable HTML report:

```
═══════════════════════════════════════
  Conversion Report: Footman.mdx
═══════════════════════════════════════

  Model: Footman
  Scale: 0.05 (auto-estimated)
  Duration: 12.3s

  ✓ MDX parsed (v800, 1450 vertices, 28 bones)
  ✓ 4 textures converted (footman.dds, footman_weapon.dds, ...)
  ✓ 28 bones, 7 animations baked
  ⚠ tex[2] alpha appears inverted — auto-corrected
  ⚠ 3 degenerate faces removed
  ✓ Hit-test sphere: r=1.42
  ✓ Portrait camera: OK
  ✓ M3 exported: out/Footman.m3 (2.1 MB)

  Warnings (2):
    - tex[2] alpha was inverted; fixed automatically
    - 3 degenerate triangles in geoset[1]; removed

  Suggestions:
    - Particle effect "breath" has low emission rate;
      try --particle-rate-scale 3.0 for better visibility
```

---

## Pillar 3: Self-Healing Conversion

### 3.1 Alpha Auto-Correction

WC3 BLP textures often store alpha inverted (transparent = 255, opaque = 0). The current converter requires manual `alpha_invert: true` in config.

**Auto-detect algorithm:**
1. Load texture, separate alpha channel
2. Sample border pixels (typically transparent in WC3 textures)
3. If > 80% of border pixels have alpha > 200, alpha is likely inverted
4. Auto-invert and log the action

### 3.2 Scale Auto-Estimation

Current: hardcoded `scale: 0.04` in example configs. Users must guess.

**Heuristic approach:**
1. Read `MODL` bounds (min/max) and `boundsRadius`
2. Compare against known WC3 model categories:
   - Hero units: bounds ~100-200 units → scale ~0.04
   - Buildings: bounds ~300-600 units → scale ~0.06
   - Doodads: bounds ~20-80 units → scale ~0.03
   - Effects: bounds ~10-30 units → scale ~0.02
3. Use `SEQS` moveSpeed as additional signal (heroes walk faster)
4. Output: recommended scale with confidence level
5. User can override with `--scale` flag

### 3.3 Animation Name Fuzzy Matching

Current: rigid `DEFAULT_ANIM_NAMES` table. Unknown names pass through verbatim.

**Fuzzy approach:**
1. Levenshtein distance against known WC3→SC2 mappings
2. If "Attack - 1" maps to "Attack", then "Atack - 1" (typo) should too
3. Strip numbers: "Attack 3" → "Attack 03"
4. Common variants: "Stand-1", "Stand 1", "STAND_1" all → "Stand"
5. Log all fuzzy matches for user review

### 3.4 Texture Fallback Chain

When a texture can't be found:
1. Search by basename in model directory
2. Search in `Textures/` subdirectory
3. Search in parent directories (for MPQ-extracted structures)
4. Search user-provided texture library path
5. Search WC3 install directory (if detected via registry)
6. Generate a 64x64 magenta placeholder with the texture name burned in
7. Log the full search path and what was used

### 3.5 Bone Hierarchy Repair

Common issues with community models:
- Orphaned bones (no parent, not root)
- Circular parent references
- Missing root bone

**Auto-repair:**
1. Detect cycles in parent chain → break at the cycle point
2. Orphaned bones → re-parent to root
3. No root bone → create a dummy root at (0,0,0) with identity transform

### 3.6 Degenerate Geometry Cleanup

- Zero-area faces: remove silently (already handled by edge-split)
- Duplicate vertices: merge within tolerance
- Unused vertices: remove (not referenced by any face)
- Inverted normals: detect and flip based on winding order consistency

---

## Pillar 4: Intelligent Material System

### 4.1 Material Templates

Instead of purely deriving materials from WC3 flags, offer named templates:

| Template | Behavior |
|---|---|
| `standard` | Current behavior: derive from filterMode |
| `teamcolor` | Force team-color emissive on all geosets |
| `glow` | Force additive blend + unshaded + glow treatment |
| `metallic` | Specular + environment map setup |
| `transparent_glass` | Alpha blend + refraction hint |

Templates can be specified per-material in config or auto-detected from texture naming conventions (e.g., `_glow.blp` → glow template).

### 4.2 Normal Map Generation (Optional)

For high-quality ports:
1. Detect if model has team-color areas (replaceableId 1/2)
2. Generate a tangent-space normal map from the diffuse using Sobel edge detection
3. Output `modelname_n.dds` alongside `modelname.dds`
4. Configure SC2 material to use normal map

### 4.3 Emission Map Extraction

WC3 team-color textures (replaceableId 1/2) don't have file data — they're runtime color fills.

**SC2 approach:** Extract the diffuse texture, create an emission mask where team-color geometry appears (via the geoset/material association), and produce an emissive texture that maps to the TEAMEMIS layer.

---

## Pillar 5: Particle System Intelligence

### 5.1 Auto-Tuning

Current: particles often look wrong because WC3 and SC2 particle systems differ fundamentally. Users must manually tune `particle_rate_scale` and `particle_size_scale`.

**Auto-tune algorithm:**
1. For each emitter, compare its `emissionRate` × `lifespan` against model bounds
2. If the total particle volume is < 5% of model volume at any time → rate is too low
3. If individual particle `scaling[0]` > 50% of model bounds → particles are too large
4. Auto-suggest scale factors and apply with `--auto-tune-particles`
5. Generate preview render (Blender can render a frame) to validate visually

### 5.2 Particle Presets

Common WC3 effects map to SC2 presets:

| WC3 Effect | SC2 Preset | Settings |
|---|---|---|
| Fire (orange glow, upward velocity) | `fire` | gravity=-0.5, additive, orange color ramp |
| Smoke (gray, slow rise) | `smoke` | gravity=-0.1, alpha blend, gray color ramp |
| Magic (blue/purple glow) | `magic` | additive, blue→purple color, billboard |
| Blood (red, falls) | `blood` | gravity=1.0, modulate, red, short life |
| Dust (brown, ground-level) | `dust` | gravity=0, modulate, brown, wide spread |

Auto-detect based on texture name, color values, and gravity direction.

---

## Pillar 6: Batch & Power Features

### 6.1 Directory Batch Mode

```
wc3toSC2.exe --batch "C:\My Models\" --recursive
```

- Finds all `.mdx` files recursively
- Queues conversions with progress bar
- Parallel: one Blender instance handles sequential models, textures convert in parallel
- Generates a summary CSV: model name, status, warnings, output size, duration
- Failed models get detailed error reports in individual subdirectories

### 6.2 MPQ Archive Extraction

```
wc3toSC2.exe --mpq war3.mpq --extract "Units\Human\Footman\*"
```

- Reads WC3 MPQ archives directly (using a pure-Python MPQ reader like `mpyq` or stormlib bindings)
- Extracts `.mdx` + all referenced `.blp` textures
- Converts the extracted set automatically
- Useful for bulk-porting entire race rosters

### 6.3 Watch Mode

```
wc3toSC2.exe --watch "C:\My Models\"
```

- Monitors a directory for new/modified `.mdx` files
- Auto-converts on change (like a build system)
- Useful during model editing workflow

---

## Pillar 7: SC2 Integration

### 7.1 Actor Data Generation

Auto-generate SC2 actor/data XML for quick import:

```xml
<CActorModel id="Footman">
  <Model value="Assets\Units\Human\Footman\Footman.m3"/>
  <Scale value="1.0"/>
  <HostAttachments>
    <HostAttachment ActorAttachment="Ref_Origin"/>
  </HostAttachments>
</CActorModel>
```

Output: `Footman_actor.xml` that can be pasted directly into the SC2 editor.

### 7.2 Model Preview / Thumbnail

Generate a `.png` preview of the converted model:
1. Blender renders a single frame from the "Stand" animation
2. Outputs `ModelName_preview.png` at 256x256
3. Can be used as SC2 editor thumbnail

### 7.3 SC2 Limits Compliance

Auto-check and warn:
- Model triangle count > 65k (SC2 soft limit)
- Texture resolution > 2048 (SC2 recommended max)
- Bone count > 256
- Animation keyframe density
- Material layer count > 4

---

## Pillar 8: Plugin & Config System

### 8.1 Preset System

Ship with presets for popular WC3 model packs:

```json
// presets/human_units.json
{
  "name": "Human Units",
  "scale": 0.05,
  "particle_rate_scale": 2.0,
  "team_color": true,
  "anim_names": {
    "Stand - 1": "Stand",
    "Attack - 1": "Attack",
    ...
  }
}
```

Users apply: `wc3toSC2.exe Footman.mdx --preset human_units`

### 8.2 User Config Profiles

Save/load conversion profiles:
```
wc3toSC2.exe --save-profile my_settings
wc3toSC2.exe MyModel.mdx --profile my_settings
```

### 8.3 Post-Processing Hooks

Allow users to run custom Python scripts after conversion:
```
wc3toSC2.exe MyModel.mdx --post-hook my_custom_fixes.py
```

The hook receives the parsed MDX dict and the build config, can modify before Blender runs.

---

## Pillar 9: Error Recovery & Resilience

### 9.1 Graceful Degradation

If a feature fails, don't abort the whole conversion:

| Feature fails | Fallback |
|---|---|
| Particle conversion | Skip particles, export model without them |
| Animation baking | Export static model (no animations) + warn |
| Texture conversion | Reference the texture path; user supplies DDS later |
| Camera export | Skip camera; model still works |
| Team color emissive | Skip emissive layer; team color won't glow |
| Blender crash | Retry once, then offer to skip animations/particles |

### 9.2 Crash Recovery

- Save intermediate state after each stage (MDX parse → texture convert → Blender → export)
- On crash, resume from last successful stage
- Never lose completed texture conversions

### 9.3 Telemetry (Opt-In)

- Anonymous error reporting (what model format, what failed, stack trace)
- Helps identify common failure patterns across the user base
- Strictly opt-in with clear disclosure

---

## Pillar 10: Implementation Priority

### Tier 1 — Foundation (v2.0)
- [ ] `diagnostics.py` — pre-flight analyzer with 15+ checks
- [ ] Auto alpha-invert detection
- [ ] Scale auto-estimation from bounds
- [ ] Texture fallback chain with placeholder generation
- [ ] Degenerate geometry cleanup
- [ ] Graceful degradation (skip failing features)
- [ ] Conversion report (HTML + JSON)
- [ ] Drag-and-drop `.exe` (auto-detect everything)
- [ ] PyInstaller with embedded Python runtime

### Tier 2 — Intelligence (v2.1)
- [ ] Animation name fuzzy matching
- [ ] Bone hierarchy auto-repair
- [ ] Particle auto-tuning
- [ ] Material templates
- [ ] Batch directory mode
- [ ] Conversion profiles / presets

### Tier 3 — Power (v2.2)
- [ ] SC2 actor data generation
- [ ] Model preview rendering
- [ ] MPQ archive extraction
- [ ] Watch mode
- [ ] Normal map generation
- [ ] Portable Blender bundle or download-on-demand

### Tier 4 — Ecosystem (v3.0)
- [ ] Plugin/hook system
- [ ] Community preset marketplace
- [ ] GUI application (tkinter or web-based)
- [ ] Telemetry (opt-in)
- [ ] CI/CD artifact publishing (auto-build `.exe` on tag)
