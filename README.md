# wc3-mcp

Made by GodMephisto

[![CI](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)

An MCP server that lets AI assistants such as Claude read, audit and edit
Warcraft III `.w3x` and `.w3m` maps byte-faithfully (files you do not touch
keep their bytes).

## Requirements

See [REQUIREMENTS.md](REQUIREMENTS.md).

## Install

### From zip (no .NET needed)

Download the latest release zip from the Releases page, unzip it anywhere.

### Build from source

```
dotnet publish src/Wc3.Mcp/Wc3.Mcp.csproj -c Release \
  --self-contained -r win-x64 -o dist-mcp
```

CascLib.dll must stay beside Wc3.Mcp.exe.

## Register

### Claude Code

```
claude mcp add wc3 -- C:\tools\wc3-mcp\Wc3.Mcp.exe
```

Add a game_dir override when needed.

.mcp.json form (project scoped):

```json
{
  "mcpServers": {
    "wc3": {
      "command": "C:\\tools\\wc3-mcp\\Wc3.Mcp.exe"
    }
  }
}
```

Add an env block with a `WC3_GAME_DIR` key pointing to your Warcraft III
folder when a path override is needed.

### Claude Desktop

Put the same snippet under `mcpServers` in `%APPDATA%\\Claude\\claude_desktop_config.json`.

### Any other stdio MCP client

This is a standard MCP server over stdio. Point your client at the executable.

## Configuration

- The `WC3_GAME_DIR` environment variable sets the Warcraft III install used
  to resolve base-game data.
- Per-call, most tools accept an optional `game_dir` parameter.
- Per-call precedence picks the `game_dir` argument first, the `WC3_GAME_DIR`
  env var second, then auto-detection from the Windows registry and common paths.

The auto-detection order (read from `src/Wc3.GameData/GameInstall.cs`) hits
current user registry, local machine registry, then `C:\Warcraft III`,
`C:\Program Files (x86)\Warcraft III`, `C:\Program Files\Warcraft III`.

## Tools

48 tools across these categories:

| Name | Read/Write | Description |
|---|---|---|
| map_info | read-only | Map metadata including name, author, player count, playable dimensions and load diagnostics. |
| list_files | read-only | List the map archive internal files with name, size in bytes and parsed format flags. |
| script_leaks | read-only | Handle leaks in war3map.j including locations, groups, forces, timers and triggers ranked by frequency. |
| uabi_profile | read-only | Unit ability profiles across maps including unit types, references, ids and per-race breakdown. |
| replay_summary | read-only | Read `.w3g` replay files with map, length, players and disconnect events. |
| object_list | read-only | List custom or modified objects with rawcode, base rawcode and resolved name. |
| object_get | read-only | Merged object fields (base overlaid by map deltas) with names resolved. |
| object_set | writes | Set a field on an object and save to out_path. |
| object_new | writes | Create a new custom object derived from a base rawcode, save to out_path. |
| bundle_unit | read-only | Unit dependency closure including objects, asset files, strings, edges and JASS. |
| render_model | writes | Render an object model (`.mdx` or `.mdl`) to PNG at out_path. |
| port_unit | writes | Port a unit from a source map into a target map, auto-remapping collisions. |
| palette_doodad | read-only | List placeable doodad types, the palette for place_doodad. |
| place_doodad | writes | Place a doodad instance at (x, y) on the doodad layer. |
| place_region | writes | Define a rectangular region on the region layer. |
| place_unit | writes | Place a unit at world (x, y) owned by a player. |
| place_start_location | writes | Place or move a player start location at (x, y). |
| place_item | writes | Place a preplaced item at world (x, y). |
| terrain_stats | read-only | Terrain summary with tile count, min or max height, and cliff level. |
| terrain_deform | writes | Raise, lower, set, flatten or smooth ground height. |
| terrain_cliff | writes | Raise, lower or set cliff (stepped-terrain) level. |
| terrain_ramp | writes | Toggle the ramp (sloped cliff) flag. |
| terrain_paint | writes | Paint ground texture over a brush. |
| terrain_water | writes | Set, raise, lower or remove water. |
| terrain_blight | writes | Set or clear the blight flag. |
| deprotect_map | read-only | Recover file names from protected maps. |
| file_list | read-only | List archive entries with their current size. |
| file_get_text | read-only | Read an internal file decoded as UTF-8. |
| file_set | writes | Replace an internal file from a disk file. |
| audit_map | read-only | Behavioural audit of object data including tooltips, requirements and dangling references. |
| sound_list | read-only | List the map sound catalog with name, file, channel, volume, pitch and distance. |
| sound_add | writes | Add a new sound definition. |
| sound_set | writes | Set a field on a sound definition. |
| sound_remove | writes | Remove a sound definition. |
| camera_list | read-only | List cameras with name, target, rotation, angle of attack and field of view. |
| camera_add | writes | Add a camera at a target position. |
| camera_set | writes | Set a field on a camera. |
| camera_remove | writes | Remove a camera. |
| pathing_paint | writes | Paint pathing bits (Walk, Fly, Build, Blight, Water) over a brush. |
| map_info_get | read-only | Read editable scenario fields (name, author, fog, etc.). |
| map_info_set | writes | Set a scenario field. |
| player_list | read-only | List player slots with name, color, race, controller and start position. |
| player_set_force | writes | Move a player into a force (team). |
| force_list | read-only | List forces and teams with name, members, alliance and sharing flags. |
| force_set_flags | writes | Set force alliance or sharing flags. |
| new_map | writes | Create a blank World Editor openable map. |
| trigger_catalog_list | read-only | List GUI-trigger functions from the World Editor catalog. |
| trigger_catalog_describe | read-only | Show detail for one GUI-trigger function by name. |

## Safety

Writing tools require `out_path` to differ from the input map. The original is
never overwritten.

## Limits

Windows x64 only (native CascLib). Tools that query base-game data need a
Warcraft III Reforged install.

## Troubleshooting

- **Game not found**: set `WC3_GAME_DIR` or pass `game_dir` to the tool.
- **CascLib.dll missing**: when building from source, ensure the published
  folder contains CascLib.dll beside the exe.
- **Protected maps**: pass `listfiles` arguments to `deprotect_map` to supply
  community name dictionaries.

## Credits

Built on

| Library | Author |
|---|---|
| War3Net (War3Net.Build, War3Net.IO.Mpq, War3Net.Drawing.Blp) | Drake53 and contributors |
| CascLib | Ladislav Zezula |
| CascLib.NET | Kizari |
| ImageSharp | Six Labors and contributors |
| MCP C# SDK (ModelContextProtocol) | Model Context Protocol project |
| Microsoft.Extensions.Hosting | Microsoft, .NET Foundation |

Used directly: Luashine/jass-history (inventory change study), pjass (JASS syntax
checking), devoltz and Arakunido (SLK repair guide), and the Hive Workshop
community (command line arguments).

Learned from, among many others, HiveWE (stijnherfst), Warsmash and Retera's
Model Studio (Retera), WC3MapTranslator (ChiefOfGxBxL), Wurst, TheHelper and the
Hive Workshop. The full list of every project and community studied, 197 in all,
is in [CREDITS.md](CREDITS.md).

Warcraft III is made by Blizzard Entertainment. This project is unofficial, is
not affiliated with or endorsed by Blizzard, reads your own install and ships no
game files.

## License

Free for everyone. Non-commercial use needs no credit. Commercial use (selling
it, or using it in a paid product or service) must credit GodMephisto. See
LICENSE.
