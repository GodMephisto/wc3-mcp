---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/tool-reference.mdx"
ref: HEAD
---

# MCP tool reference

---
title: MCP tool reference
description: Everything Claude Code can do in wc3-forge today, grouped by surface.
---

wc3-forge exposes **152 MCP tools**. All entity tools speak **game coordinates**
(origin at map center, units = WC3 world units). All mutations flow through the
same history stack the GUI uses, so `Ctrl+Z` in the editor undoes Claude's edits
and vice versa.

## Map lifecycle

| Tool | Purpose |
| --- | --- |
| `map_open` | Open a map folder or `.w3x` |
| `map_new` | Create a fresh `.w3x` (name/size/tileset) and open it |
| `map_save` | Save in place |
| `map_save_as` | Export the current map to a new `.w3x` and switch to it |
| `map_extract_to_folder` | Unpack a `.w3x` into the editable folder workflow |
| `map_close` | Close the current map |
| `map_status` | Name + counts for the loaded map |

## Map info

`map_info_get`, `map_info_set` — name, author, description, suggested players,
and the Lua flag. On a localized map these fields are TRIGSTR-backed; edits
update the referenced `war3map.wts` entry instead of orphaning it.

## Placed units

`units_list`, `units_get`, `units_move`, `units_rotate`, `units_scale`,
`units_set_field` (player, HP/mana %, hero level, gold, …), `units_create`,
`units_delete`.

## Placed doodads

`doodads_list`, `doodads_get`, `doodads_move`, `doodads_rotate`, `doodads_scale`,
`doodads_create`, `doodads_delete`.

## Regions

`regions_list`, `regions_get`, `regions_create`, `regions_move`,
`regions_resize`, `regions_rename`, `regions_delete` — the `war3map.w3r` rect
editor, with a WebGL viewport overlay.

## Start locations

`start_locations_list`, `start_locations_create`, `start_locations_move`,
`start_locations_delete`.

## Terrain

`terrain_get_tile`, `terrain_set_tile`, `terrain_paint_tile`,
`terrain_set_height`, `terrain_brush_height`, `terrain_brush_cliff`,
`terrain_brush_ramp`, `terrain_brush_water`, `terrain_swap_tileset`.

## Object definitions (all 7 kinds)

For `<kind>` in **units / items / abilities / buffs / destructables / doodads /
upgrades**, read + write custom and stock objects:

`objects_<kind>_list`, `objects_<kind>_get`, `objects_<kind>_set_field`,
`objects_<kind>_create_custom`, `objects_<kind>_delete_custom`,
`objects_<kind>_fields_meta` — plus `objects_convert`.

## Gameplay constants

`gameplay_constants_get`, `gameplay_constants_apply` — per-map overrides in
`war3mapMisc.txt`.

## Imports

`imports_list`, `imports_add`, `imports_remove`, `imports_rename` — the
`war3map.imp` import table.

## Models & minimap

`models_import` (auto-convert OBJ / glTF / STL → MDX + textures and import them),
`minimap_bake` (render a terrain minimap, optionally persisted as `war3mapMap.blp`).

## Triggers

`triggers_tree`, `triggers_get`, `triggers_add_gui`, `triggers_generate_script`,
`triggers_convert_to_lua`, `triggers_test_map`, and more — the full GUI tree,
Monaco editor, GUI→Lua/JASS codegen, Convert-Map-to-Lua, and Test Map.

## Selection, view & window

- Selection: `selection_get`, `selection_set`, `selection_clear`.
- View + camera: `view_get_mode`, `view_set_mode`,
  `view_set_doodad_category_visible` (idempotent SET), `camera_set_view`.
- Window: `window_set_title`.

## History, diagnostics & multi-instance

- History: `history_undo`, `history_redo`, `history_list`,
  `history_begin_group`, `history_end_group`, `history_abort_group`.
- Diagnostics: `diagnostics_get`, `diagnostics_arm`.
- Multi-instance: `sessions_list`, `session_select`.
