---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/index.mdx"
ref: HEAD
---

# Introduction

---
title: Introduction
description: A native Warcraft III map editor with an embedded MCP server, driven by Claude Code as much as by hand.
---

**wc3-forge** is a native Warcraft III map editor (Go + TypeScript, a single
[Wails](https://wails.io) binary) with an embedded MCP server. The GUI and Claude
Code talk to the same editor session — every edit you make by hand can also be
made by an agent, and vice versa, and they share one undo stack.

<Callout type="warn" title="Status: alpha">
  wc3-forge is under active development. Most surfaces are wired through both the
  GUI and MCP, but some rough edges remain.
</Callout>

## What it does today

Read + write across most surfaces, exposed through both the GUI and MCP:

- **Placed units and doodads** — move / rotate / scale, plus create and delete.
- **Terrain** — tile and height editing.
- **A full Object Editor** for all 7 definition kinds: units, items, abilities,
  buffs, destructables, doodads, and upgrades (read + write, custom and stock).
- **A complete Trigger Editor** — GUI tree + Monaco code view + WC3 IntelliSense,
  GUI→Lua/JASS codegen, native JASS-map editing, Convert-Map-to-Lua, and Test Map.
- **Saves in place**, including packaged `.w3x` / MPQ archives.

Older JASS maps open, edit, save, and Test Map directly — no conversion required.

## Why an MCP-native editor?

Most editors are built for a human at a mouse. wc3-forge is built so that the
**same session** can be driven by a person and by [Claude Code](https://claude.com/claude-code)
at the same time. A JSON-RPC `units.move` and a viewport drag end up in the same
mutator and emit the same change events, so:

- Claude can read the map, reason about it, and make precise edits.
- You keep full manual control and a live view of everything the agent does.
- Both share one history stack — `Ctrl+Z` undoes Claude's edits and vice versa.

## Next steps

- [Install wc3-forge](/docs/installation)
- [Set up Claude Code](/docs/claude-code)
- [Take the editor tour](/docs/editor-tour)
- [Browse the MCP tool reference](/docs/tool-reference)
