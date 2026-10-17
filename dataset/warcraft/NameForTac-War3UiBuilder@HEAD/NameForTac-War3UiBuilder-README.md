---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "README.md"
ref: HEAD
---

# War3UiBuilder

A Warcraft III custom UI editor. Design and export UI layouts for Warcraft III maps using a visual editor.

## Features

- **Visual canvas** — drag, resize, and arrange UI elements on an infinite canvas
- **Element tree** — hierarchical view of all UI elements with parent-child relationships
- **Property panel** — edit element properties (position, size, texture, text, etc.)
- **INI export** — export designs to Warcraft III `UI/{map}.ini` format
- **Image import** — import reference images as tracing backgrounds
- **Undo/Redo** — full undo/redo support for editing operations
- **Element types** — SIMPLEFRAME, BUTTON, TEXT, MODEL, and custom frames

## Requirements

- Qt 6.10.2+ (Core, Gui, Widgets)
- C++17 compiler (MinGW or MSVC)
- CMake or qmake

## Build

### Using qmake

```bash
qmake6 War3UiBuilder.pro
mingw32-make -f Makefile.Release
```

### Package

Run `deploy.bat` to build and collect all Qt dependencies into the `deploy/` directory.

## Usage

1. Launch the application
2. Create a new project or open an existing `.wui` project file
3. Add UI elements from the toolbar
4. Edit properties in the property panel
5. Export to INI format for use in Warcraft III maps

## Export Format

The editor generates `.ini` files compatible with Warcraft III's custom UI system (`UI/{map}.ini`). Supports both standard and War3-mode coordinate conversion.

## License

MIT
