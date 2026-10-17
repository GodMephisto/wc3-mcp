---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/skills/hivewe-ui-development/SKILL.md"
ref: HEAD
---

---
name: hivewe-ui-development
description: >-
  Guides Qt editor changes in HiveWE — palettes, docking, shortcuts, models, and .ui files.
  Use when editing src/main_window, src/menus, object/trigger/model editors, or custom widgets.
disable-model-invocation: true
---

# HiveWE UI Development

## When to use

Main window, ribbon, palettes, dialogs, docked editors, minimap, signals/slots, `QShortcut`, `.ui` files, Qt item models.

## Workflow

1. Find owning window/palette from ribbon action in `src/main_window/hivewe.cpp`.
2. Determine driver: `.ui` layout, Qt model in `src/models/`, or palette/dialog class.
3. Reuse patterns:
   - `window_handler.create_or_raise<T>()`
   - `Palette` subclasses; sidebar orchestration in `hivewe.cpp`
   - `QShortcut` near owning widget; `ads::CDockManager` for docks
4. If UI reflects map data, trace `TableModel` or tree/list model before adding widget state.
5. QoL polish: active-state clarity, sidebar visibility vs brush context, concise tooltips, Warcraft-facing values.

## Verification checklist

- [ ] Open affected window/palette/editor
- [ ] Shortcuts, focus, docking/tabbing, open/close
- [ ] If editing map data: save and reload reflects change

## Common mistakes

- Widget-local state diverging from `Map` or Qt models.
- Bypassing `window_handler` for singleton editors.
- Missing `Qt::WA_DeleteOnClose` or parent ownership cleanup.
