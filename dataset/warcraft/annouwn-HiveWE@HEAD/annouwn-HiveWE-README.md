---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "README.md"
ref: HEAD
---

# HiveWE (Fork)

 HiveWE (Fork)

This is a personal fork of [Lufion1420/HiveWE](https://github.com/Lufion1420/HiveWE),
which is itself a fork of [stijnherfst/HiveWE](https://github.com/stijnherfst/HiveWE).

---

## Changes in this fork

## v0.21.1

### Bug Fixes

#### Sidebar & Window Resize (UI)

Fixed a long-standing issue where the sidebar forced the main window to a large
minimum height, making it impossible to resize the window vertically on short or
small screens.

**Changes:**
- Wrapped the palette stack in a `QScrollArea` so tall palettes scroll instead
  of expanding the sidebar and forcing the window to grow
- Made the sidebar preview placeholder height adaptive (60–150px) instead of
  a fixed 150px minimum, so it can shrink on short screens
- Made the minimap height adaptive (130–180px) instead of a fixed 180px
- Made the minimap frame height adaptive (150–196px) instead of a fixed 196px
- Made the model preview widget height adaptive (80–220px) instead of fixed
- Set a reasonable minimum window size of 800×600 so the window cannot be
  collapsed to nothing

### Shortcuts
- `R` — toggle the Region Palette
- `O` — open the Object Editor
- `Shift + Scroll` — increase/decrease brush size (terrain)
- `Shift + Scroll` — scale selected doodad(s)
- Object Editor: `Ctrl + C` / `Ctrl + V` — copy/paste selected object settings or whole objects
- Object Editor: `F2` — rename the selected object entry inline
- Object Editor: `Alt + Left` / `Alt + Right` — navigate Back / Forward through opened objects
- Object Editor: `Tab` — switch focus between the object tree and the details pane
- Object Editor: `Ctrl + F` — focus the left browser search; `Ctrl + Shift + F` — focus the right-side field search
- Added a Config tab listing all usable shortcuts in the editor
- Config window commands further categorized for clarity
- Descriptions on hover added for some header items under `View`, including shortcut hints

### Build Process
- Attempts to automatically launch the project after building and kills any previously running instance first (note: automatic launch is currently not working as intended — the old instance is killed but starting the new one fails with an error)

---

## AI-assisted development

Cursor agent guidance lives in [`AGENTS.md`](AGENTS.md) (index) and [`.cursor/`](.cursor/) (rules, skills, plans). Scoped rules load automatically when editing matching files; invoke skills explicitly for subsystem workflows.
