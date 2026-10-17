---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "README.md"
ref: HEAD
---

# GrimForge Bot

A Warcraft 3 Reforged bot written in Python — Orc race, Blademaster focus.

Reads live game state directly from process memory, executes macro and micro faster than any human hand. No machine learning. No cheats. Pure engineering.

> *Forged in code. Sharpened in battle.*

---

## What it does

- **Memory reader** — reads gold, lumber, food, unit positions and building states directly from WC3 process memory via macOS Mach APIs
- **Sidecar system** — injects a Lua script into the WC3 map that exposes structured game state
- **Action queue** — priority-based input queue (MICRO > ARMY > MACRO > ECONOMY) with preemption between steps
- **Macro loop** — automated build order execution (peons, Great Hall, Barracks, tech)
- **Micro loop** — Blademaster ability usage and unit control
- **Input executor** — single-threaded input dispatch via CGEvent (keyboard) and CGEventPost (mouse)
- **Shorts pipeline** — automated YouTube Shorts production from bot sessions

## Architecture

```
SidecarScanner (memory reads)
       │
       ▼
   MacroLoop ──► ActionQueue ──► InputExecutor ──► WC3Input
   MicroLoop ──►               (single thread)
```

## Requirements

- macOS (Apple Silicon or Intel)
- Python 3.11+
- Warcraft 3 Reforged (windowed/borderless mode)
- StormLib (`brew install stormlib`) for map injection

```bash
pip install -r tools/shorts_pipeline/requirements.txt
```

## Project structure

```
src/wc3bot/
  action/       input primitives, build order
  core/         action queue, executor, macro/micro loops, coords
  observe/      memory reader, sidecar scanner, state parser
  race/orc/     orc-specific config, setup, build order
scripts/        diagnostic + test scripts
tools/
  shorts_pipeline/  automated YouTube Shorts production
maps/sidecar/   Lua sidecar map injection
```

## Status

Currently in Phase 1 — beating WC3 Insane AI consistently with the Orc race.

- [x] Live game state via memory reads
- [x] Sidecar Lua injection
- [x] ISO coordinate system + screen projection
- [x] Threading architecture (ActionQueue + InputExecutor)
- [x] T16 macro start sequence — 0.56s
- [ ] Blademaster micro loop
- [ ] Win rate ≥ 7/10 vs Insane AI

## Follow

| | |
|---|---|
| 📺 YouTube | [@GrimForgeBot](https://youtube.com/@GrimForgeBot) — every session recorded, every failure logged |
| 🔥 Ko-fi | [ko-fi.com/grimforge](https://ko-fi.com/grimforge) — keeps the forge burning |

## License

MIT
