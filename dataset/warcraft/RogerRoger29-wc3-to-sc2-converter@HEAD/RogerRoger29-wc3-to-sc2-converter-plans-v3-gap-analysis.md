---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "plans/v3-gap-analysis.md"
ref: HEAD
---

# v2.0 → v3.0 Gap Analysis & Brainstorm

## Executive Summary

v2.0 is a **solid foundation** — 55 passing tests, a working GUI, self-correcting engine. But there are **16 major gaps** across five dimensions: User Experience, Conversion Quality, Distribution, Testing, and Ecosystem. Each gap below includes a concrete bridging strategy.

---

## Dimension 1: User Experience Gaps

### Gap 1: No Visual Model Preview
**Current state:** The GUI shows a tree of model names and a text log. Users cannot see what any model looks like before or after conversion.

**Why it matters:** This is a *visual* tool for *visual* assets. A modder needs to verify the model looks correct without opening SC2. Every professional 3D tool (Blender, Maya, 3ds Max, Noesis) has a viewport.

**Bridge strategy:**
- **Short-term:** Launch Blender in a minimal window, render one frame of the "Stand" animation to a temp PNG, display in the GUI's right panel via QLabel/QPixmap. This adds ~5 seconds per model but gives instant visual feedback.
- **Long-term:** Embed a lightweight OpenGL viewport (PyOpenGL + QOpenGLWidget) that renders the parsed MDX geometry directly — no Blender needed. Would require writing a minimal MDX renderer, but makes the tool truly self-contained.

### Gap 2: No Drag-and-Drop Onto the EXE Icon
**Current state:** Users must open the GUI first, then add models. They can't just drag `Footman.mdx` onto `wc3toSC2.exe` on the desktop.

**Why it matters:** This is the #1 "it just works" expectation for any converter tool. Windows passes the file path as `sys.argv[1]`.

**Bridge strategy:** In `main_window.py`, check `sys.argv[1:]` on startup. If arguments are `.mdx` files or a folder, auto-add them to the queue and optionally auto-start conversion. Add a `--silent` flag for fully unattended mode.

### Gap 3: No Undo / Session Recovery
**Current state:** If the app crashes or is closed mid-batch, all progress is lost. The history tab stores only completed conversions, not in-progress state.

**Why it matters:** Batch converting 50 models takes time. A crash at model 47 means redoing 46 conversions.

**Bridge strategy:** Persist the ModelJob queue to `%APPDATA%/wc3toSC2/session.json` after every status change. On startup, offer to resume. Store converted textures and intermediate build configs so completed stages aren't repeated.

### Gap 4: Configuration is Still JSON-First
**Current state:** The GUI settings tab is basic (Blender path, scale, toggles). For anything advanced (per-texture glow, custom anim names, attachment bone overrides), users must edit JSON.

**Why it matters:** The audience is SC2 modders, not developers. JSON editing is the #1 friction point cited in the README troubleshooting section.

**Bridge strategy:** Add an "Advanced" expandable panel per model in the queue that exposes: per-texture glow/alpha_invert toggles, anim name mapping table, attachment point editor, material override dropdowns. All stored in ModelJob, serialized transparently.

---

## Dimension 2: Conversion Quality Gaps

### Gap 5: Rotation Interpolation is Lossy
**Current state:** [`build_m3.py:316-325`](build_m3.py:316) uses slerp between quaternion keyframes, ignoring hermite/bezier tangents. WC3 models can specify cubic rotation curves.

**Why it matters:** Certain animations (spell casts with rapid spins, death animations with tumbling) look subtly wrong. The difference is visible to animators.

**Bridge strategy:** Implement proper squad (spherical cubic interpolation) for hermite/bezier quaternion tracks. This requires computing the cubic Bezier control quaternions from tangents and evaluating the curve on S³. Standard algorithm from Shoemake 1985. ~50 lines of math.

### Gap 6: 30 FPS Hardcoded
**Current state:** [`build_m3.py:297`](build_m3.py:297) `FPS = 30.0`. All animations are baked at 30fps regardless of the WC3 original frame rate. WC3 animations run at variable rates (typically 30fps for units, 60fps for effects).

**Why it matters:** Fast particle effects and rapid attacks can look choppy. Conversely, slow idles waste keyframes at 30fps.

**Bridge strategy:** Auto-detect optimal FPS per sequence by analyzing the minimum keyframe interval. For a sequence with keys every 33ms → 30fps. Keys every 16ms → 60fps. Keys every 100ms → 10fps. Bake at the appropriate rate. Expose as config override.

### Gap 7: No PBR/Advanced Material Generation
**Current state:** Materials are basic: diffuse texture + blend mode + optional team-color emissive. SC2 supports normal maps, specular maps, emissive maps, environment maps, roughness/metallic.

**Why it matters:** Modern SC2 mods (especially total conversions and HD projects) expect PBR materials. A flat diffuse model looks dated by 2026 standards.

**Bridge strategy:**
- Generate normal map from diffuse via Sobel filter (already in the v2-expansion roadmap)
- Detect and generate specular/roughness placeholder maps
- Allow users to supply companion textures (`_n.dds`, `_s.dds`, `_e.dds`) and auto-wire them into the M3 material
- Add material template "HD" that sets up all PBR layers

### Gap 8: Team Color is Crude
**Current state:** Team color is a TEAMEMIS emissive layer masked by the diffuse texture. This means the ENTIRE model glows in team color. In WC3, team color only applies to specific mesh regions (those using replaceableId 1/2 textures).

**Why it matters:** A Footman's armor glows, but so does his face. This looks wrong.

**Bridge strategy:** The MDX already knows which geosets use team-color materials. Generate a separate team-color mask texture (white where team color applies, black elsewhere) and use it as the emissive bitmap instead of the diffuse. This requires identifying which UV regions belong to team-color geosets and baking a mask.

### Gap 9: No LOD (Level of Detail) Generation
**Current state:** All geosets export at full resolution. SC2 supports multiple LOD levels for performance.

**Why it matters:** A converted WC3 hero with 2000 triangles is fine. A converted WC3 building with 8000 triangles at LOD0 is wasteful at distance.

**Bridge strategy:** Use mesh simplification (edge collapse via `trimesh` or `pymeshlab` Python bindings) to generate LOD1 (50% tris) and LOD2 (25% tris). Configure m3studio to export multi-LOD models. Optional toggle in settings.

### Gap 10: No Animation Compression
**Current state:** Every frame of every bone channel is keyed. A model with 30 bones × 7 animations × 100 frames = 21,000 keyframes. M3 file size balloons.

**Why it matters:** SC2 has file size limits for multiplayer maps. Bloated M3 files waste space.

**Bridge strategy:** Apply tolerance-based keyframe reduction after baking. If a bone's rotation changes by <0.001 radians between frame N and N+2, remove frame N+1. This is a standard technique (the "Douglas-Peucker for curves" equivalent). Can reduce keyframe count by 40-60% with no visual difference.

---

## Dimension 3: Distribution Gaps

### Gap 11: The Blender Problem is Unsolved
**Current state:** Users MUST install Blender 4.4 and the m3studio addon separately. This is the single biggest adoption barrier. The auto-detect finds Blender but can't install it or the addon.

**Why it matters:** Non-technical users (the primary audience for a double-click EXE) will bounce hard on "install Blender, then install this addon from GitHub, then configure the path." This is 3 steps too many.

**Bridge strategies (ordered by feasibility):**
1. **Auto-download Blender Portable:** The EXE detects missing Blender, offers to download `blender-4.4.0-windows-x64.zip` (~300MB), extracts to `%APPDATA%/wc3toSC2/blender/`. One click.
2. **Programmatic addon install:** Python can copy the m3studio folder into Blender's addons directory and enable it via `bpy.ops.preferences.addon_enable()`. Do this on first launch.
3. **Fully bundled distribution:** Ship a ~400MB ZIP containing the EXE + Blender Portable + m3studio pre-installed. "Extract and double-click." The gold standard for "no setup."

### Gap 12: No macOS/Linux Build
**Current state:** PyInstaller spec is Windows-only (`console=False` for GUI mode). The code has cross-platform Blender detection but no cross-platform packaging.

**Why it matters:** SC2 modding exists on all platforms (though Windows dominates). Linux users especially appreciate CLI + GUI tools.

**Bridge strategy:**
- CI matrix: build on `ubuntu-latest`, `macos-latest`, `windows-latest`
- Platform-specific PyInstaller specs (`.app` bundle on macOS, ELF binary on Linux)
- Test GUI on all platforms (Qt works everywhere)

### Gap 13: No Auto-Update Mechanism
**Current state:** Users must check GitHub for new versions. No in-app update notification or download.

**Why it matters:** v2.0 fixed bugs that v1.0 users will never know about unless they manually check. Critical for a tool targeting non-technical users.

**Bridge strategy:** On startup, `GET https://api.github.com/repos/RogerRoger29/wc3-to-sc2-converter/releases/latest`. Compare tag to current version. If newer, show a notification bar: "v2.1.0 available — download?" with a clickable link. Use `QNetworkAccessManager` for async HTTP.

---

## Dimension 4: Testing & Quality Gaps

### Gap 14: No Blender Integration Tests
**Current state:** All 55 tests are pure Python. The `build_m3.py` pipeline (which is 50% of the converter) has zero automated test coverage because it requires Blender.

**Why it matters:** A change to build_m3.py could silently break M3 export. The only way to catch it is manual testing.

**Bridge strategy:**
- CI installs Blender 4.4 headless (available on GitHub Actions runners)
- CI installs m3studio addon programmatically
- Integration test: convert Naaru.mdx → verify Naaru.m3 exists, has correct size, parses with a minimal M3 header reader
- Mark as `@pytest.mark.blender` and run only on CI (not local dev)

### Gap 15: No Fuzz Testing of MDX Parser
**Current state:** The parser is tested against the Naaru model and a few invalid files. It has never been tested against malformed/corrupted/fuzzed MDX data.

**Why it matters:** A malicious or corrupted `.mdx` could crash the parser, leak memory, or in theory exploit the Python process. Low risk but real for a tool that processes arbitrary files.

**Bridge strategy:**
- Add `hypothesis` or `atheris` fuzzing to the test suite
- Generate random binary data, feed to `mdx.parse()`, verify it either returns valid dict or raises an exception (never segfaults)
- Add bounds checking on all chunk sizes (already partially done with `assert` statements)
- Run in CI as a periodic job (not per-commit, too slow)

### Gap 16: No Performance Regression Testing
**Current state:** No benchmarks. No measurement of whether a change made conversion slower.

**Why it matters:** The animation baking loop is O(bones × frames). A naive change could make it O(bones² × frames) and nobody would know until users complain.

**Bridge strategy:**
- Add `pytest-benchmark` to dev dependencies
- Benchmark: parse Naaru.mdx (should be <50ms), convert 4 textures (should be <2s), full CLI dry-run (should be <3s)
- Run benchmarks in CI, fail if >20% regression
- Store historical results in a `benchmarks/` JSON file tracked in git

---

## Dimension 5: Ecosystem & Community Gaps

### Gap 17: No Preset Sharing / Model Database
**Current state:** Every user configures their own conversions from scratch. There's no way to share "I got the Footman working perfectly with these settings."

**Why it matters:** The WC3 model pool is finite and well-known. 90% of conversions will be standard units (Footman, Grunt, Peon, etc.). Community-maintained presets eliminate trial-and-error for the most common models.

**Bridge strategy:**
- `presets/` directory in the repo with JSON files per model/unit
- Each preset includes: scale, particle_rate_scale, anim_names mapping, per-texture settings
- Users can submit presets via PR
- GUI has a "Load Preset" dropdown that fetches the list from the repo
- Optional: online preset registry (simple JSON file on GitHub Pages)

### Gap 18: No SC2 Editor Integration
**Current state:** The converter produces `.m3` and `.dds` files. Users must manually import them into the SC2 editor. They must also create the Actor and Unit data entries.

**Why it matters:** The SC2 editor is the user's primary environment. Importing is friction. For bulk conversions (entire race rosters), manual import is impractical.

**Bridge strategy:**
- Generate SC2 "Component" XML that can be imported as a single unit
- Auto-place output files into the correct SC2 mod directory structure (`ModName/Assets/Textures/`, `ModName/Assets/Models/`)
- For advanced: SC2 editor plugin (C++ native plugin or Galaxy script) that adds "Import WC3 Model..." menu item
- The actor_gen.py already generates XML — wire it to output into the correct SC2 mod path

### Gap 19: No Video/Visual Documentation
**Current state:** README.md and START_HERE.txt are text-only. No screenshots of the GUI, no video walkthrough.

**Why it matters:** A 2-minute YouTube video showing "drag Footman.mdx → click Convert → import into SC2" would 10x adoption. Text docs are a barrier for visual learners.

**Bridge strategy:**
- Add GUI screenshots to README (dark theme Convert tab, Batch tab, report HTML)
- Record a 3-minute demo: Naaru conversion from drag-drop to SC2 editor import
- Host on YouTube, embed in README
- Add a "?" help button in the GUI that links to the video

### Gap 20: No SC2 Model Validation
**Current state:** The converter validates the MDX input but not the M3 output against SC2 engine requirements.

**Why it matters:** SC2 has hard limits: 65k triangles, 256 bones, 2048px textures. A model that exceeds these will crash the SC2 editor or cause rendering artifacts.

**Bridge strategy:**
- After export, parse the M3 header (simple binary read) to extract: vertex count, bone count, material count, texture dimensions
- Validate against known SC2 limits
- Add warnings to the conversion report
- Auto-downscale oversized textures
- Auto-warn on high triangle counts with LOD suggestion

---

## New Feature Brainstorm (Greenfield Ideas)

### F1: Web Converter (Zero Install)
A Flask/FastAPI web frontend: upload `.mdx` + `.blp` files → convert server-side → download `.m3` + `.dds`. No Blender, no Python, no install. Just a browser.

**Technical:** Server runs Blender headless. Rate-limited, queue-based. Could be a free tier (1 model/day) + paid tier. Host on a $20/mo VPS with Blender installed.

### F2: WC3 Map Extractor
Point the tool at a `.w3x` or `.w3m` map file → auto-extract all custom models and textures → batch convert all of them → output a ready-to-use SC2 mod folder.

**Technical:** `.w3x` files are MPQ archives. The mpq_reader.py already handles this. Just need to scan for all `.mdx` files inside.

### F3: Diff/Comparison Mode
Given an original WC3 model and a user's modified version, show what changed (new geosets, modified animations, different textures) and convert only the delta.

**Useful for:** Modders who iterate on models and want to re-convert only what changed.

### F4: Animation Retargeting
Given a WC3 skeleton, retarget animations from one model to another (e.g., apply Footman animations to a custom knight model with the same bone names).

**Technical:** Match bones by name, copy animation keyframes, adjust for scale differences.

### F5: SC2 Data Template Generator
Beyond actor XML — generate complete SC2 data entries: Unit, Actor, Model, Sound, Effect, Behavior. A "one-click import" that creates a fully functional SC2 unit from a WC3 model, ready to place in the editor.

### F6: Batch Portrait Generator
WC3 models often lack portrait cameras. Auto-generate a portrait model (head-focused camera, simplified mesh) from any unit model.

### F7: Material AI 
Use a simple ML classifier (or heuristics) to auto-detect material types from textures: "this looks like metal" → metallic template, "this looks like cloth" → cloth template, etc. Based on texture color histogram and naming conventions.

---

## Priority Matrix

| Priority | Gap/Feature | Impact | Effort | Dependencies |
|---|---|---|---|---|
| **P0** | G11: Blender auto-download | 🔴 Critical | Medium | None |
| **P0** | G1: Model preview in GUI | 🔴 Critical | Medium | Blender subprocess |
| **P0** | G2: Drag-drop onto EXE | 🔴 Critical | Low | None |
| **P0** | G14: Blender integration tests | 🔴 Critical | High | CI Blender install |
| **P1** | G5: Rotation interpolation fix | 🟡 High | Medium | Math |
| **P1** | G8: Team color mask | 🟡 High | Medium | UV baking |
| **P1** | G3: Session recovery | 🟡 High | Medium | QSettings |
| **P1** | G4: Advanced config in GUI | 🟡 High | High | PySide6 widgets |
| **P2** | G6: Variable FPS baking | 🟢 Medium | Low | None |
| **P2** | G10: Keyframe reduction | 🟢 Medium | Medium | None |
| **P2** | G7: PBR material support | 🟢 Medium | High | Normal map gen |
| **P2** | G13: Auto-update | 🟢 Medium | Low | GitHub API |
| **P3** | G17: Preset sharing | 🔵 Nice-to-have | Low | Community |
| **P3** | F2: Map extractor | 🔵 Nice-to-have | Low | mpq_reader.py |
| **P3** | F5: Data template gen | 🔵 Nice-to-have | Medium | actor_gen.py |

---

## Summary

The project is strong but has **three existential gaps** that prevent it from being "the tool everyone uses":

1. **The Blender problem** (G11) — too much setup for non-technical users
2. **No visual feedback** (G1) — users can't see what they're converting
3. **No integration tests on the Blender pipeline** (G14) — the core 50% of the tool is untested

Fixing these three alone would make v3.0 a **generational leap** from v2.0. The remaining 17 gaps are quality-of-life, quality-of-output, and ecosystem improvements that compound over time.
