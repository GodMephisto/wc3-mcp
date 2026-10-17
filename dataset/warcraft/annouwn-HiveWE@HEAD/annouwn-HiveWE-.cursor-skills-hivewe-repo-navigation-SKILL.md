---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/skills/hivewe-repo-navigation/SKILL.md"
ref: HEAD
---

---
name: hivewe-repo-navigation
description: >-
  Orients quickly inside HiveWE across UI, map state, rendering, and serialization.
  Use when finding where code lives, tracing call paths, or deciding which subsystem
  owns a change (base, file_formats, resources, or editor windows).
disable-model-invocation: true
---

# HiveWE Repo Navigation

## When to use

- Task starts with "find where this lives" or "understand this subsystem".
- Trace behavior across UI, map state, rendering, and serialization.
- Unsure whether a change belongs in `src/base/`, `src/file_formats/`, `src/resources/`, or an editor window.

## Entry points

| Start here | For |
|------------|-----|
| `src/main.cpp` | Startup, CASC, theme, first map load |
| `src/main_window/hivewe.cpp` | Ribbon actions, editor wiring |
| `src/base/map/map.ixx` | Map load/save orchestration |
| `src/base/hierarchy.ixx` | Asset lookup order |
| `src/models/` + editor folder | Tables/tree views |
| `tests/` | Existing parser/format coverage |

## Workflow

1. Identify owning layer: UI (`main_window`, `menus`, editors) vs core map (`src/base/`) vs formats (`src/file_formats/`, triggers) vs GPU (`src/resources/`, `glwidget.cpp`).
2. Confirm call path from UI action to data owner before editing.
3. Confirm save/load owner before changing serialized behavior.
4. Prefer **Codegraph MCP** (`codegraph_explore`) for symbol/call-path questions before wide grep loops or explore subagents.

## Common mistakes

- Patching `hivewe.cpp` when the bug is in `Map`, `Hierarchy`, or a parser.
- Treating `.ixx` as headers only — they are primary implementation units.
- Editing UI before checking if the Qt model already supports the behavior.
