---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "CHANGELOG.md"
ref: HEAD
---

# Changelog

This is the changelog for the Lufion fork of HiveWE. Versions follow the fork's
own numbering; `v0.20` is the first packaged release of the fork and builds on
upstream HiveWE `v0.10`.

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
  
hivecpp changes:

	// Wrap the palette stack in a scroll area so a tall palette scrolls instead of
	// forcing the sidebar (and the main window) to a large minimum height. This is
	// what lets the window be resized vertically on short screens.
	pathing_host = create_host("pathingSidebarHost");
	region_host = create_host("regionSidebarHost");
	added code below
	auto* sidebar_scroll = new QScrollArea(sidebar_root);
	sidebar_scroll->setWidget(sidebar_body);
	sidebar_scroll->setWidgetResizable(true);
	sidebar_scroll->setFrameShape(QFrame::NoFrame);
	sidebar_scroll->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
	sidebar_scroll->setVerticalScrollBarPolicy(Qt::ScrollBarAsNeeded);
	sidebar_scroll->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);
	sidebar_scroll->setStyleSheet("QScrollArea { background: transparent; border: 0; }");

	sidebar_layout->addWidget(sidebar_body, 1);
	to
	sidebar_layout->addWidget(sidebar_scroll, 1);

	sidebar_preview_placeholder->setMinimumHeight(150);
	to
	// Cap the placeholder so it can shrink on short windows instead of forcing a
	// large minimum height on the sidebar (and therefore the main window).
	sidebar_preview_placeholder->setMinimumHeight(60);
	sidebar_preview_placeholder->setMaximumHeight(150);

	minimap->setFixedHeight(180);
	minimap->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
	to
	minimap->setMinimumHeight(130);
	minimap->setMaximumHeight(180);
	minimap->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);

	sidebar_minimap_frame->setFixedHeight(196);
	sidebar_minimap_frame->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);

	to
	// Adaptive height: keeps the 196px look when there is room but can shrink on
	// short windows so it does not force the whole window's minimum height.
	sidebar_minimap_frame->setMinimumHeight(150);
	sidebar_minimap_frame->setMaximumHeight(196);
	sidebar_minimap_frame->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);

	added
		context = ui->widget;
	// Reasonable floor so the window can be resized but not collapsed to nothing.
	setMinimumSize(800, 600); <- this
	setup_palette_sidebar();

	massive changes here:
		sidebar_preview_title->setText(title.isEmpty() ? "Model Preview" : title);
	if (!sidebar_preview && model) {
		sidebar_preview = new ModelEditorGLWidget(sidebar_preview_frame, nullptr, true);
		sidebar_preview->setMinimumHeight(150);
		// Adaptive: keep a modest minimum so the preview can shrink on short windows.
		sidebar_preview->setMinimumHeight(80);
		sidebar_preview->setMaximumHeight(220);
		sidebar_preview->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
		sidebar_preview->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
		if (auto* preview_layout = qobject_cast<QVBoxLayout*>(sidebar_preview_frame->layout())) {
			preview_layout->insertWidget(1, sidebar_preview);
		}

## v0.21

Highlights since `v0.20`:

### Map Protector
- New **Map Protector** window (Config ribbon) that exports a hardened copy of the current map or an external `.w3x` / `.w3m` / folder. The source is never modified.
- MPQ hardening: remove listfile, remove attributes, per-file encryption, and optional junk-file injection.
- Trigger/script hardening: remove GUI trigger data (`war3map.wtg`), and optional trigger-string inlining (strip `war3map.wts` after resolving `TRIGSTR_*` in the script and map info).
- Optional metadata sanitization (author, description, loading-screen text, map name).
- Asset path obfuscation: rename custom imports to flat random-hex names and rewrite matching references in object data, map info, MDX internals, and script string literals, with a dangling-reference safety check before packing.
- Fixes for listfile-less archives (core-file extraction + `war3map.imp` recovery), StormLib fabricated-name packing crashes, and several object-data field types (`model` / `icon` / `modelList` / `pathingTexture`, plus icon/model paths that omit extensions).

### Object Data Export / Import
- Export Object Editor modifications (units, items, abilities, buffs, upgrades, destructibles, doodads) as a binary or human-readable text package from the Home ribbon.
- Import with validation and conflict preview, per-object overwrite controls, and transactional apply with rollback on failure.
- Also supports importing raw `war3map.w3*` files from another map folder.
- Newly created objects from an import now appear correctly in a new Object Editor tab.

### Scenario Properties
- New **Players / Forces** editor (Map ribbon) matching World Editor fields: controller, race, fixed start location, color, and force alliance flags.
- Always shows all 24 player slots; inactive (None) rows are dimmed.
- Players/Forces can be exported/imported as `scenario.json` alongside Object Data packages, with a field-level change preview.
- Fixed force share-vision bit loading and race lobby-selectability generation.

### Asset Manager
- Reworked into a stronger unused-file cleanup tool: stats header, type column, All / Unused filter, and bulk delete to the recycle bin.
- Full MDX scan (not only Object Editor–reachable models), override detection for stock game paths, and case-insensitive used/unused matching.

### Rendering
- Fixed opaque SD models rendering invisible (alpha test incorrectly applied to Filtermode None).
- Fixed black models caused by missing mipmaps on non-DDS (TGA/PNG) textures.
- Fixed tiling textures collapsing due to limited UV packing range (now half-float UVs).

### Fixes & Polish
- Window title version updated to **0.21**.
- Doodad and unit palettes now sort alphabetically.
- Fixed Object Editor left-panel search focus stealing.
- Fixed `TRIGSTR` next-id tracking that could collide with newly created strings.
- Opening listfile-less `.w3x` maps is more reliable (extracts core `war3map.*` files by name).

### Running
Download the zip, extract it, and run `HiveWE.exe`. A Warcraft III installation is required.

## v0.20

First packaged release of the fork. Highlights since upstream HiveWE `v0.10`:

### Object Editor
- Complete visual redesign into a modern, cleaner style with a matching color scheme.
- New **Custom** folder on every tab that collects all custom object data, for easier work in heavily customized maps.
- Folders are collapsed by default; a single click opens the right-side data panel instead of spawning new tabs.
- Active object data is sorted alphabetically, stacked values are shown inline on one line, object IDs are shown next to names, and selected-object data has proper column headers with full-row click/highlight.
- Arrow-key navigation that also updates the right-side settings panel.
- Folder search for the icon settings.
- Ability object improvements: fixed hotkey fields, multi-select Option field, and a Buffs field that lets you add/remove buffs from a searchable list.
- Copy/paste objects with Ctrl+C / Ctrl+V, F2 rename fixes, and a fix for duplicate objects syncing renames.
- Fewer clicks needed to open value fields such as ability target type.
- Editor state (active tab, open/closed folders) is now remembered.
- Custom object images are read and displayed correctly.
- Fixed a crash when scrolling through certain assets, and added the **O** shortcut to open the editor.

### Regions
- New Regions palette with colored regions (toggle with **R**; regions are only shown while the palette is open).
- Region movement is wired into undo/redo, with cursor feedback for drag/move.
- Overlapping regions prioritize the smaller region on click; double-clicking a region in the list moves the camera to it. Fixed incorrect rect coordinates.

### Terrain & Palettes
- Palettes reworked into a sidebar with a full visual redesign.
- Terrain copy/paste (use with caution).
- Smooth tool is now gradual instead of instantly flattening.
- The add-water tool accounts for differing terrain heights.
- Brush size can be changed with Shift+Scroll.

### Doodads & Units
- Selected doodads are highlighted; drag placement now requires movement and no longer fires immediately, and the pre-placement click helper was removed.
- Scale doodads with Shift+Scroll (works with multiple selected).
- Doodad palette gained a custom-doodads tab; the unit palette can filter by custom units.
- Item Drop Table window (under the Units tab and when double-clicking pre-placed units).
- Fixed doodad Z placement that previously mismatched in-game and differed from the World Editor.
- Fixed water pathing for units.

### Sidebar, Preview & Navigation
- Doodad/unit preview in the sidebar; arrow keys switch the previewed model.
- Minimap moved into the sidebar and is toggleable (off by default).
- Reworked panel/sidebar behavior: selection mode is on by default when switching panels, and editors are brought to the foreground when clicked.
- Player start locations are now shown.

### Model Editor & Rendering
- Added a model editor browser and made the model editor view much larger.
- Animation dropdown to choose which animation effects play, plus improved model-preview sequence selection.
- SD/HD editable mesh rendering, particle rendering and visibility fixes, full sequence tokenization, and an SD model grid view.

### MDX / MDL
- MDX/MDL round-trip tests plus reader/writer fixes.
- Added MDX camera and texture animations.
- Fixed PREM1 loading and removed MDL block comments.

### Tooltip Editor
- New tooltip editor that reads values from existing objects and hides the raw WC3 color strings for clarity, with color-picker UX improvements.

### Maps & General
- New-map creation popup that previews the selected tileset's tiles, similar to the default World Editor.
- Saving shows a fading confirmation message.
- Quick access to the last three opened maps under **Files**.
- Config tab listing all editor shortcuts, grouped by category.
- Building the project now launches it automatically (and kills old instances) to speed up iteration.
- New application icon, header redesign, and assorted visual/coordinate fixes in the debug view.

### Running
Download the zip, extract it, and run `HiveWE.exe`. A Warcraft III installation is required.
