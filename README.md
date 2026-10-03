# wc3-mcp

Made by GodMephisto

[![CI](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)

An MCP server that lets AI assistants such as Claude read, audit and edit
Warcraft III `.w3x` and `.w3m` maps byte-faithfully (files you do not touch
keep their bytes).

It is the MCP server of [wc3ctl](https://github.com/GodMephisto/wc3ctl), which
also has a command line tool and a desktop editor. Install wc3ctl if you want
those as well. It carries the same server as `wc3ctl mcp serve`.

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

Using Claude Desktop only? Download `wc3-mcp-<version>-win-x64.mcpb` from the
Releases page and double-click it. Claude Desktop installs it as an extension.
Its one setting, the Warcraft III folder, can stay empty to have the install
detected. The server is also listed in the
[MCP Registry](https://registry.modelcontextprotocol.io) as
`io.github.GodMephisto/wc3-mcp`, for apps that install from there.

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

97 tools.

| Name | Read/Write | Description |
|---|---|---|
| asset_list | read-only | List the asset paths an icon or model object field can actually be set to, from the map's own imported files and from the base game. |
| audit_ability | read-only | Tells you, per ability, PASS or the exact broken link across a placed hero's whole runtime chain, far deeper than audit_hero, which only proves a cast... |
| audit_fidelity | read-only | Tells you whether a ported object's data came across faithfully, by comparing it and its whole custom-object dependency closure between the SOURCE map it... |
| audit_hero | read-only | Tells you whether a placed hero's abilities are actually wired up in the map's script, not merely present as object data. |
| audit_map | read-only | Behavioural audit of a map's object data, read-only. |
| audit_readiness | read-only | Tells you whether a placed hero's script would actually RUN correctly, not just fire, the gap audit_hero cannot see since it only proves a cast reaches a... |
| bundle_unit | read-only | Resolve everything a unit depends on, the porting preview. |
| camera_add | writes | Add a camera at a target position and save the edited map to out_path. |
| camera_list | read-only | List the map's cameras (war3map.w3c). |
| camera_remove | writes | Remove a camera and save the edited map to out_path. |
| camera_set | writes | Set a field on a camera and save the edited map to out_path. |
| deprotect_map | read-only | Recover the file names a protected map stripped. |
| diff | read-only | Compare two maps and report which internal files were added, removed or changed. |
| editor_catalog_get | read-only | Show one World Editor catalog's entries, each with its stored key (the token a map file actually carries), its resolved display name and every payload field. |
| editor_catalog_list | read-only | List the World Editor's catalog names from UI\WorldEditData.txt with entry counts (TileSets, SkyModels, LoadingScreens, SoundChannels, MapSizes, the brush... |
| file_get_text | read-only | Read one internal file decoded as UTF-8, with any leading BOM stripped. |
| file_list | read-only | List the archive's internal entries with their CURRENT size, which reflects a pending replacement rather than the stale original. |
| file_set | writes | Replace an internal file's bytes verbatim from a disk file and save the edited map to out_path. |
| force_list | read-only | List the map's forces/teams (war3map.w3i). |
| force_set_flags | writes | Set a force's alliance/sharing flags and save the edited map to out_path. |
| hero_install | writes | Install a hero definition into a map and save the result to out_path. |
| hero_lint | read-only | Validate a hero definition folder before installing it. |
| hero_roster | read-only | List the playable heroes a map registers, with each hero's rawcode, display name, the string tags from its registration call (typically a role and a... |
| imports_list | read-only | List the map's import table (war3map.imp) against what the archive actually holds, so a table entry with no file behind it and a file absent from the table... |
| lint | read-only | Pre-flight the map. |
| list_files | read-only | List the map archive's internal files, each with its name (null = unnamed/protected entry), size in bytes, whether wc3ctl knows/parses the format, and for... |
| map_info | read-only | Show a Warcraft III map's metadata. |
| map_info_get | read-only | Read the map's editable scenario fields (war3map.w3i). |
| map_info_set | writes | Set a scenario field and save the edited map to out_path. |
| new_map | writes | Create a blank, World-Editor-openable map (.w3x) at out_path. |
| object_field_options | read-only | The legal values for one object-data field, taken from what the base game data actually uses for it. |
| object_form | read-only | Get an object as an editable FORM rather than a flat field dump. |
| object_get | read-only | Get an object's merged fields (base game data overlaid by the map's deltas), each labeled 'base' or 'map', with field names resolved. |
| object_list | read-only | List the map's custom/modified objects of one Object Editor kind. |
| object_new | writes | Create a new custom object derived from a base rawcode (a fresh unused rawcode is allocated) and save the edited map to out_path. |
| object_set | writes | Set a field on an object of one Object Editor kind and save the edited map to out_path (only that kind's war3map.* file is re-serialized). |
| palette_doodad | read-only | List the doodad types placeable on a map, the palette to consult before place_doodad. |
| pathing_paint | writes | Paint pathing bits over a circular or square brush and save the edited map to out_path. |
| place_doodad | writes | Place a doodad instance at (x, y) on the map's doodad layer (war3map.doo) and save the edited map to out_path. |
| place_item | writes | Place a preplaced item of the given type at world (x, y) and save the edited map to out_path. |
| place_region | writes | Define a rectangular region on the map's region layer (war3map.w3r) and save the edited map to out_path. |
| place_start_location | writes | Place or move a player's start location at world (x, y) and save the edited map to out_path. |
| place_unit | writes | Place a unit of the given type, owned by a player, at world (x, y) and save the edited map to out_path. |
| placed_doodad_get | read-only | Show one placed doodad's full state by creation number. |
| placed_doodad_remove | writes | Remove a doodad already placed on the map and save the edited copy to out_path. |
| placed_doodad_set | writes | Set one field on a doodad already placed on the map and save the edited copy to out_path. |
| placed_doodads_list | read-only | List every doodad and destructable already PLACED on the map (war3map.doo), each with the creation number needed to edit or remove it, plus its type... |
| placed_unit_get | read-only | Show one placed unit's full state by creation number. |
| placed_unit_remove | writes | Remove a unit already placed on the map and save the edited copy to out_path. |
| placed_unit_set | writes | Set one field on a unit already placed on the map and save the edited copy to out_path. |
| placed_units_list | read-only | List every unit already PLACED on the map (war3mapUnits.doo), each with the creation number needed to edit or remove it, plus its type rawcode, owner,... |
| player_list | read-only | List the map's player slots (war3map.w3i). |
| player_set_force | writes | Move a player into a force (team) and save the edited map to out_path. |
| port_unit | writes | Port a unit (its custom-object closure + imported assets + strings, and best-effort its JASS trigger closure) from a source map into a target map,... |
| region_list | read-only | List the map's rectangular regions (war3map.w3r), each with its creation number, name, and bounds. |
| region_remove | writes | Remove a region by name and save the edited map to out_path. |
| render_model | writes | Render an object's model (map-imported .mdx/.mdl) to a PNG written at out_path. |
| repair_generated | writes | Repair a map that wc3ctl itself generated, whose preplaced-hero helper block is too narrow for the heroes installed into it, and save the repaired map to... |
| replay_summary | read-only | Read recorded games (.w3g) and report each one's map, length, players, and how and when every player left. |
| roundtrip | read-only | Verify the map survives a load and save byte for byte. |
| script_functions | read-only | List every function declared in the map's compiled script, with where it starts and ends. |
| script_leaks | read-only | Find handle leaks in a map's war3map.j (locations, groups, forces, effects, timers, text tags, lightning and triggers created and never destroyed), each... |
| script_references | read-only | Find every use of a name in the map's compiled script. |
| search | read-only | Search the map's contents for a string, ignoring case. |
| sound_add | writes | Add a new sound definition (keyed by name) to the map's sound catalog and save the edited map to out_path. |
| sound_list | read-only | List the map's sound catalog (war3map.w3s). |
| sound_remove | writes | Remove a sound definition from the map's sound catalog and save the edited map to out_path. |
| sound_set | writes | Set a field on a sound definition and save the edited map to out_path. |
| strings_list | read-only | List the map's string table (war3map.wts). |
| terrain_blight | writes | Set or clear the blight (corrupted ground) flag over a brush, saving the edited map to out_path. |
| terrain_cliff | writes | Raise/lower/set the cliff (stepped-terrain) level over a brush, saving the edited map to out_path. |
| terrain_corner_get | read-only | Show one terrain corner's full state. |
| terrain_corner_set | writes | Set one field on ONE terrain corner and save the edited copy to out_path. |
| terrain_deform | writes | Raise/lower/set/flatten/smooth ground height over a circular or square brush, saving the edited map to out_path. |
| terrain_fill | writes | Apply a terrain tool over an inclusive RECTANGLE of corners and save the edited map to out_path, instead of one brush dab at a time. |
| terrain_info | read-only | The terrain grid's extents in corners, plus the map's ground and cliff tile lists. |
| terrain_paint | writes | Paint a ground texture over a brush, saving the edited map to out_path. |
| terrain_ramp | writes | Toggle the ramp (sloped cliff transition) flag over a brush, saving the edited map to out_path. |
| terrain_stats | read-only | Summarize a map's terrain (war3map.w3e). |
| terrain_water | writes | Set/raise/lower/remove water over a brush, saving the edited map to out_path. |
| trigger_add | writes | Add a trigger to a category in the map's GUI trigger tree and save the edited map to out_path. |
| trigger_add_category | writes | Add a category to the map's GUI trigger tree and save the edited map to out_path. |
| trigger_add_eca | writes | Add an event, condition or action to a GUI trigger and save the edited map to out_path. |
| trigger_catalog_describe | read-only | Show full detail for one GUI-trigger function by name (e.g. |
| trigger_catalog_list | read-only | List GUI-trigger functions from the World-Editor catalog (UI\TriggerData.txt), optionally filtered by kind and/or a name/display-name search. |
| trigger_recover_from_script | writes | Rebuild a browsable GUI trigger tree (war3map.wtg) for a map that has NONE, by decompiling the compiled script the game runs, and save the result to out_path. |
| trigger_remove | writes | Remove a trigger item (a category, a trigger or a comment) from the GUI trigger tree and save the edited map to out_path. |
| trigger_remove_eca | writes | Remove one event, condition or action from a GUI trigger by its zero-based position, and save the edited map to out_path. |
| trigger_rename | writes | Rename a trigger item (a category, a trigger or a deleted stub) in the GUI trigger tree and save the edited map to out_path. |
| trigger_set_eca_enabled | writes | Enable or disable ONE event, condition or action within a trigger, the World Editor's per-line toggle rather than the whole-trigger one, and save the edited... |
| trigger_set_enabled | writes | Enable or disable a GUI trigger and save the edited map to out_path. |
| trigger_set_initially_on | writes | Set whether a GUI trigger starts switched on, and save the edited map to out_path. |
| trigger_set_run_on_map_init | writes | Set whether a GUI trigger runs on map initialization, and save the edited map to out_path. |
| triggers_read | read-only | Read the map's GUI trigger tree (war3map.wtg) plus the custom-text bodies (war3map.wct) and the script language. |
| uabi_profile | read-only | Measure unit ability lists (uabi) across many maps at once, one row per map. |
| unit_abilities | read-only | Every ability a unit has, each tagged with where it comes from. |
| validate | read-only | Check the map for structural problems. |

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
| War3Net (War3Net.Build, War3Net.IO.Mpq, War3Net.Drawing.Blp, War3Net.CodeAnalysis.Decompilers) | Drake53 and contributors |
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
