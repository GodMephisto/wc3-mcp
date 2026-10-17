---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "CREDITS.md"
ref: HEAD
---

# Credits

wc3-forge stands on the shoulders of prior Warcraft III open-source work. This file documents the projects whose code, designs, or hard-won format knowledge were used in building wc3-forge.

## HiveWE

[HiveWE](https://github.com/stijnherfst/HiveWE) by **Stijn Herfst** and contributors (GPL-3.0) is the single largest influence on wc3-forge. wc3-forge began as a Go rewrite of an in-house HiveWE C++ fork, and many subsystems are deliberate ports of HiveWE logic. wc3-forge is itself licensed under GPL-3.0-or-later for this reason.

Specific places where wc3-forge is a port of, mirrors, or directly references HiveWE:

- **File-format parsers** under `internal/formats/`:
  - `w3e` (terrain) — encoding/decoding, ground/cliff palette interpretation, cliff-edge palette remap via `CliffTypes.slk`.
  - `w3i` (map info) — game-version dispatch for subversion-dependent fields.
  - `doodadsdoo` / `unitsdoo` (placed doodads + units) — version/subversion handling, raw-scale storage convention, item-drop set boundary preservation, the `creation_number` identity scheme.
  - `wpm` (pathing map), `shd` (shadow map), `mpq` (archive container) — structural layout and edge cases.
  - `miscdata` — gameplay-constants table parsing.

- **Renderer** under `frontend/src/`:
  - `terrain.ts` — N-layer alpha-composite blending, per-cell variation byte selection, lighting equations ported from HiveWE's `terrain.frag` / `terrain.vert`.
  - `cliffs.ts` + `cliff-shader.ts` — cliff-mesh selection, ramp/edge handling, the `real_tile_texture` ramp exemption.
  - `water.ts` — animated water plane logic.
  - `pathing.ts` + `path-blockers.ts` — pathing-map rendering.
  - `scene-instances.ts` — model-instance lifecycle conventions; the rendering loop sits between `mdx-m3-viewer`'s `startFrame()` / `render()` calls, but the terrain/cliff/water passes are ours-via-HiveWE.

- **MCP bridge** (`internal/bridge/`) — the JSON-RPC 2.0 over NDJSON wire shape originated in the same in-house HiveWE C++ fork's `src/mcp_bridge/`. wc3-forge's Go implementation is an independent reimplementation but the wire contract is preserved verbatim so HiveWE-fork MCP clients keep working.

- **Cross-checking tool** (`cmd/dumpw3e/`) — a parser-replay used when wc3-forge's render diverges from HiveWE's; existence implies HiveWE-as-reference for visual correctness.

Inline source comments name the specific HiveWE symbol or file being ported whenever the relationship is direct (`exact port of HiveWE terrain.frag line 42-44`, `Mirrors HiveWE's real_tile_texture`, etc). Those comments are kept on purpose.

## mdx-m3-viewer

[mdx-m3-viewer](https://github.com/flowtsohg/mdx-m3-viewer) by **Ghostwolf** (MIT) provides the low-level WebGL rendering primitives wc3-forge's frontend builds on — `ModelViewer`, `Scene`, and the `mdx` / `blp` / `dds` / `tga` parser+handler modules. wc3-forge does **not** use the higher-level `War3MapViewer.loadMap` path; terrain, cliffs, water, and sky are drawn by our own shaders layered into the same scene.

## CascLib

[CascLib](https://github.com/ladislav-zezula/CascLib) by **Ladislav Zezula** (MIT) is vendored under `scripts/casclib/` and wrapped in `internal/casc` — bound at runtime via [`purego`](https://github.com/ebitengine/purego) (cgo-free `dlopen`) on macOS and `syscall.LazyDLL` on Windows, so no C toolchain is needed to build. It backs asset resolution from the user's Warcraft III install.

## StormLib

[StormLib](https://github.com/ladislav-zezula/StormLib) by **Ladislav Zezula** (MIT) — referenced for MPQ archive format details used in `internal/formats/mpq`.

## Warcraft III community format documentation

The following community resources informed format work and are worth crediting alongside the source projects that operationalized them:

- The [hiveworkshop.com](https://www.hiveworkshop.com) forums and modeling/mapping documentation.
- Various community-authored W3X / W3M / DOO / W3E / W3I format notes accumulated across the WC3 modding community over the last 20 years.

If you've contributed to WC3 open-source tooling and your work shows up in wc3-forge's behavior without being listed here, open an issue and I'll add you.

— Stephen Horton
