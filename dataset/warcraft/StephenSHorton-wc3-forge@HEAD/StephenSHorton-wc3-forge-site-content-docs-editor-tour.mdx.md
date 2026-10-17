---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/editor-tour.mdx"
ref: HEAD
---

# Editor tour

---
title: Editor tour
description: A quick orientation to the wc3-forge window and its main surfaces.
---

wc3-forge is a single window with a 3D viewport at its center and panels around
it. The GUI and the MCP bridge are two views of the **same** editor session.

## The viewport

A WebGL viewport renders terrain, cliffs, water, sky, units, doodads, and
start-location markers. It uses [mdx-m3-viewer](https://github.com/flowtsohg/mdx-m3-viewer)
for low-level MDX/BLP/DDS work; terrain, cliffs, water, and sky are drawn by
wc3-forge's own shaders layered into the same scene.

- **SD / HD assets** — start in Classic (SD) by default, or launch with
  `--reforged` for HD.
- **Camera** — Claude can pin or pan the camera with `camera_set_view` to confirm
  edits visually.

## Editing surfaces

- **Terrain** — paint tiles and raise/lower height.
- **Units & doodads** — select, move, rotate, scale, create, and delete placed
  entities.
- **Object Editor** — edit definitions for all 7 kinds (units, items, abilities,
  buffs, destructables, doodads, upgrades), both stock and custom.
- **Trigger Editor** — a GUI trigger tree plus a Monaco code view with WC3
  IntelliSense, GUI→Lua/JASS codegen, Convert-Map-to-Lua, and Test Map.

## The Agent Console

Press `Ctrl+` `` ` `` to open the Agent Console. It streams every MCP bridge call
in real time — the method name, parameters, duration, and result — so you can
watch exactly what an agent is doing as it works.

## Saving & testing

- **Save in place** writes back to the loaded folder or packaged `.w3x` / MPQ
  archive, losslessly.
- **Test Map** launches Warcraft III with the current map for older JASS maps and
  modern maps alike.
