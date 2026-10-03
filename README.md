# wc3-mcp

Made by GodMephisto

[![CI](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)

An MCP server that lets AI assistants such as Claude read, audit and edit
Warcraft III `.w3x` and `.w3m` maps byte-faithfully (files you do not touch
keep their bytes).

## What you need

- 64-bit Windows. The game data reader (CascLib) is a Windows library.
- Warcraft III Reforged, only for the tools that read base game data. Map tools
  work without it.
- Any AI app that speaks MCP. Nothing else, the release carries its own .NET.

## Install

Open PowerShell and run

```powershell
irm https://raw.githubusercontent.com/GodMephisto/wc3-mcp/main/install.ps1 | iex
```

That downloads the latest release, checks its SHA-256, unpacks it into
`%LOCALAPPDATA%\Programs\wc3-mcp`, adds that folder to your PATH and sets up
every supported AI app it finds. No admin rights. Run the same line again to
update. Restart your AI app afterwards.

Prefer to do it by hand? Download the zip from the
[Releases page](https://github.com/GodMephisto/wc3-mcp/releases), unzip it into
any folder you like, then from that folder run

```powershell
.\wc3-mcp.exe install --all
```

The setup records wherever the exe actually is, so any folder works. It also adds
that folder to your user PATH once, so from then on plain `wc3-mcp` works in any
new terminal. Pass `--no-path` to skip that, and use `.\wc3-mcp.exe` from its
folder instead.

## Set up your AI app

These need `wc3-mcp` on your PATH, which the one-line install and the first
`wc3-mcp.exe install` both arrange. Until then, run them as `.\wc3-mcp.exe ...`
from the folder the exe is in.

```
wc3-mcp install --all            set up every supported app found on this PC
wc3-mcp install cursor vscode    set up only the apps named
wc3-mcp uninstall --all          remove it again
wc3-mcp config <app>             print the settings to paste by hand
wc3-mcp clients                  list supported apps and whether each is set up
wc3-mcp doctor                   check the install, the game folder and the apps
```

| App | Name to use | Where it is written |
|---|---|---|
| Claude Code | `claude-code` | through `claude mcp add --scope user` |
| Claude Desktop | `claude-desktop` | `%APPDATA%\Claude\claude_desktop_config.json` (Store build too) |
| Cursor | `cursor` | `%USERPROFILE%\.cursor\mcp.json` |
| VS Code (GitHub Copilot) | `vscode` | `%APPDATA%\Code\User\mcp.json` |
| Windsurf | `windsurf` | `%USERPROFILE%\.codeium\windsurf\mcp_config.json` or `%APPDATA%\devin\mcp_config.json` |
| Gemini CLI | `gemini` | `%USERPROFILE%\.gemini\settings.json` |
| Codex CLI | `codex` | `%CODEX_HOME%` or `%USERPROFILE%\.codex\config.toml` |
| Cline | `cline` | `cline_mcp_settings.json` in VS Code's storage for Cline |
| LM Studio | `lmstudio` | `%USERPROFILE%\.lmstudio\mcp.json` |
| Zed | `zed` | `%APPDATA%\Zed\settings.json` |

Setup only changes the one `wc3` entry. Every other server and setting in the
file stays as it was, and the file is copied to `NAME.wc3-mcp.bak` first. A file
with comments in it (common in Zed and VS Code) is never rewritten, because that
would delete the comments. The settings to paste are printed instead.

### Any other MCP app

It is a standard stdio MCP server. Point your app at `wc3-mcp.exe` with no
arguments. `wc3-mcp config cursor` prints a ready-made JSON entry with the real
path filled in, which most apps accept as is.

## Configuration

The Warcraft III folder is found automatically from the Windows registry, then
`C:\Warcraft III`, `C:\Program Files (x86)\Warcraft III` and
`C:\Program Files\Warcraft III`. When yours is somewhere else, either

- install with `wc3-mcp install --all --game-dir "D:\Games\Warcraft III"`, which
  sets `WC3_GAME_DIR` in each app's entry, or
- pass `game_dir` to a single tool call.

A `game_dir` argument wins over `WC3_GAME_DIR`, which wins over the automatic
search. `wc3-mcp doctor` shows which folder it is using.

## Build from source

In PowerShell, with the .NET 8 SDK installed,

```powershell
git clone https://github.com/GodMephisto/wc3-mcp
cd wc3-mcp
dotnet test --filter "Category!=Corpus&Category!=GameData"
dotnet publish src/Wc3.Mcp/Wc3.Mcp.csproj -c Release --self-contained -r win-x64 -o dist-mcp
.\dist-mcp\wc3-mcp.exe install --all
```

The apps then start the exe inside your clone's `dist-mcp` folder, and that
folder is what goes on your PATH, so keep the clone where it is or rerun the last
line after moving it.

Exact SDK and package versions are in [REQUIREMENTS.md](REQUIREMENTS.md).
`CascLib.dll` must stay beside `wc3-mcp.exe`.

## Tools

49 tools across these categories:

| Name | Read/Write | Description |
|---|---|---|
| map_info | read-only | Map metadata including name, author, player count, playable dimensions and load diagnostics. |
| list_files | read-only | List the map archive internal files with name, size in bytes and parsed format flags. |
| script_leaks | read-only | Handle leaks in war3map.j including locations, groups, forces, timers and triggers ranked by frequency. |
| uabi_profile | read-only | Unit ability profiles across maps including unit types, references, ids and per-race breakdown. |
| replay_summary | read-only | Read `.w3g` replay files with map, length, players and disconnect events. |
| object_list | read-only | List custom or modified objects with rawcode, base rawcode and resolved name. |
| object_get | read-only | Merged object fields (base overlaid by map deltas) with names resolved. |
| unit_abilities | read-only | Every ability a unit has and where it comes from. Unit data, spellbook contents, a placed unit's own abilities and levels, morph forms, and abilities the script hands out (inferred, with the war3map.j line). |
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

- **Start with `wc3-mcp doctor`.** It shows where the exe is, whether CascLib.dll is beside
  it, which Warcraft III folder it uses and which apps are set up.
- **Game not found.** Run `wc3-mcp install --all --game-dir "<your Warcraft III folder>"`,
  or pass `game_dir` to the tool.
- **An app does not list the tools.** Restart it fully. Claude Desktop needs a quit from the
  tray, not just closing the window.
- **A settings file was not changed.** It has comments or is not valid JSON, so the settings
  to paste were printed instead. `wc3-mcp config <app>` prints them again.
- **Protected maps.** Pass `listfiles` to `deprotect_map` to supply community name lists.

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
