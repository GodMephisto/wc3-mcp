# wc3-mcp

Made by GodMephisto

[![CI](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml/badge.svg)](https://github.com/GodMephisto/wc3-mcp/actions/workflows/ci.yml)

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

## Context and RAG

Out of the box the server answers `docs_search` from the bundled Warcraft III references, the Big UI-Frame Tutorial and the Editor Ability Insight doc, lexical and offline, and serves each whole as an MCP resource.

For the full corpus and semantic search, install the local index. From your AI app, call the `rag_install` tool, or from a shell

```powershell
wc3ctl rag install
```

It fetches two files, once, and verifies each against a pinned digest. The embedding model `BAAI/bge-small-en-v1.5`, about 130 MB, and the corpus, about 61 MB from this repo's `dataset-v1` release. It then builds a local vector index and a graph. No database, no service, no key, and it is set low on CPU by default. After that `docs_search` is semantic and the graph tools `graph_neighbors`, `graph_hubs` and `graph_path` are live. `wc3ctl rag status` reports what is installed.

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
dotnet test --filter "Category!=Corpus&Category!=GameData&Category!=Doctor"
dotnet publish src/Wc3.Mcp/Wc3.Mcp.csproj -c Release --self-contained -r win-x64 -o dist-mcp
.\dist-mcp\wc3-mcp.exe install --all
```

The apps then start the exe inside your clone's `dist-mcp` folder, and that
folder is what goes on your PATH, so keep the clone where it is or rerun the last
line after moving it.

Exact SDK and package versions are in [REQUIREMENTS.md](REQUIREMENTS.md).
`CascLib.dll` must stay beside `wc3-mcp.exe`.

## Tools

251 tools.

| Name | Read/Write | Description |
|---|---|---|
| asset_audit | read-only | Flag suspicious imported assets. It reports entries larger than max_bytes, BLP textures whose width or height is not a power of two, a binary .mdx with no .m... |
| asset_convert_batch | writes | Convert every texture in a folder to one target format, reusing the shared converter, and write the copies under out_dir with their relative paths preserved.... |
| asset_duplicates | read-only | Group the map's imported files whose bytes are identical (by sha256) and report the space each duplicate group wastes. Zero-byte entries are excluded. Read-o... |
| asset_import_wire | writes | Add disk files to a map as imports and, when a file is a model or an icon and object names an object, point its matching model or icon field at the new impor... |
| asset_list | read-only | List the asset paths an icon or model object field can actually be set to, from the map's own imported files and from the base game. Ask this BEFORE setting... |
| asset_model_lint | read-only | Scan every imported model (.mdx/.mdl) in the map and report the structural problems a static check can find, a geoset with no material, a material naming a t... |
| asset_replace | writes | Replace an asset path everywhere it is referenced (object-data field values, script string literals and the war3map.imp import table) and save the edited cop... |
| asset_size_report | read-only | Break the map's imported bytes down by parent folder, largest total first, so the folders that dominate the map's weight are obvious. Read-only. |
| asset_thumbnails | writes | Render a thumbnail PNG for every model in a folder, or for every imported model inside every map in the folder, and write them under out_dir. Reuses the mode... |
| asset_unused | read-only | List the map's imported files that no object-data field value and no script string literal references, each with the size it holds. A lead to review before d... |
| audit_ability | read-only | Tells you, per ability, PASS or the exact broken link across a placed hero's whole runtime chain, far deeper than audit_hero, which only proves a cast reache... |
| audit_fidelity | read-only | Tells you whether a ported object's data came across faithfully, by comparing it and its whole custom-object dependency closure between the SOURCE map it was... |
| audit_hero | read-only | Tells you whether a placed hero's abilities are actually wired up in the map's script, not merely present as object data. For every ability it verifies the w... |
| audit_map | read-only | Behavioural audit of a map's object data, read-only. Where 'validate' answers whether a map can LOAD, this answers whether its abilities do what they CLAIM,... |
| audit_readiness | read-only | Tells you whether a placed hero's script would actually RUN correctly, not just fire, the gap audit_hero cannot see since it only proves a cast reaches a han... |
| audit_trend | read-only | Run the behavioural audit against two maps and report the findings the second added and removed, grouped by check. Where audit_map answers what is wrong now,... |
| bench_compat | read-only | Check every map against every target build and return a pass or fail matrix with the finding counts in each cell, plus the aggregate over the whole grid. Pas... |
| bench_leaks | read-only | Run the static leak detector over every map in a folder and report the counts per map and per leak kind, with the hot and repeat totals. The folder sweep the... |
| bench_open | read-only | Open every map in a folder through the loader and record the per-map open time in milliseconds, entry count and total decompressed bytes, reporting the slowe... |
| bench_perf_gate | read-only | Compare two recorded benchmark result JSON files and report whether any metric regressed by more than a threshold, for use as a CI gate. Each file is an obje... |
| bench_schema_coverage | read-only | Measure what object data a schemas corpus carries. One row per schema folder names the build, its era and whether the object data is real (the folder holds i... |
| bundle_diff | read-only | Compare two dependency bundles (the JSON written by 'bundle unit -o') by their object and asset members, listing what each adds, removes or renames. |
| bundle_merge | writes | Merge two dependency bundles into one, written as JSON at out_path. Refuses when the same rawcode is defined differently in the two bundles (a different kind... |
| bundle_unit | read-only | Resolve everything a unit depends on - the porting preview: referenced objects (with custom-to-map flags), asset files, trigger strings, dependency edges and... |
| camera_add | writes | Add a camera at a target position and save the edited map to out_path. The input map is NEVER modified in place. Rejects a blank or duplicate name. |
| camera_list | read-only | List the map's cameras (war3map.w3c): each camera's name, target position, rotation, angle of attack, distance and field of view. |
| camera_remove | writes | Remove a camera and save the edited map to out_path. The input map is NEVER modified in place. |
| camera_set | writes | Set a field on a camera and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: Name\|TargetX\|TargetY\|ZOffset\|Rotation... |
| camera_tour | writes | Add N cameras interpolated between two existing cameras as a smooth tour path, and save the edited map to out_path. Every float field is blended linearly and... |
| changelog | read-only | Generate release notes from git log between two refs, grouped by Conventional Commit type (feat, fix, perf, docs, chore and the rest), a heading per group wi... |
| ci_all | read-only | Run lint, validate and, when target is given, compat in one pass. Returns one summary line per gate and the overall pass or fail with an exit code (0 pass, 1... |
| ci_compat | read-only | Run the compat check as a CI gate. Returns one pass or fail with what was examined and an exit code (0 pass, 1 fail). Fails on an error-level finding, a nati... |
| ci_lint | read-only | Run the lint as a CI gate. Returns one pass or fail per check with what was examined and an exit code (0 pass, 1 fail). Fails on an error-level finding, or,... |
| ci_schema | read-only | Check the schema dataset for regressions as a CI gate. Returns one pass or fail per check with what was examined and an exit code (0 pass, 1 fail). Fails whe... |
| ci_validate | read-only | Run the validation as a CI gate. Returns one pass or fail with what was examined and an exit code (0 pass, 1 fail). Fails when the map cannot load or reports... |
| compat_explain | read-only | Explain one identifier against a target build. Reports whether the target declares it as a native, global, type or function, and if not, the earliest build t... |
| compat_matrix | read-only | Check several maps against several target builds and return a grid of pass or fail per cell, with the finding count in each cell. Pass means the target build... |
| deprotect_map | writes | Recover the file names a protected map stripped. A protected map ships no (listfile), so most entries have no name and cannot be looked up by name at all. Me... |
| diff | read-only | Compare two maps and report which internal files were added, removed or changed. The direct way to see what an edit actually did to a map, and to check that... |
| doctor_check | read-only | Will this map run on Reforged 3.0, and if not, why. A verdict plus findings, each with a code, a severity (blocker, warning, info), the affected files or mod... |
| doctor_compare | read-only | What differs between two versions of a map, file by file (added, removed, changed) with names recovered on protected archives, plus object, script and string... |
| doctor_fix | writes | Repair a COPY of a map for Reforged 3.0 with the doctor's Fix map: protection, fake file tables, scrambled ids, data the 3.0 engine refuses, broken SLK table... |
| doctor_inspect | read-only | Read-only doctor reports on one map. diag is what protects the map and what the World Editor needs, including whether it runs in SLK mode and the fix steps t... |
| editor_catalog_get | read-only | Show one World Editor catalog's entries, each with its stored key (the token a map file actually carries), its resolved display name and every payload field.... |
| editor_catalog_list | read-only | List the World Editor's catalog names from UI\WorldEditData.txt with entry counts (TileSets, SkyModels, LoadingScreens, SoundChannels, MapSizes, the brush pa... |
| file_get_text | read-only | Read one internal file decoded as UTF-8, with any leading BOM stripped. Use it for the override text files, war3mapSkin.txt and friends, that carry a map's s... |
| file_list | read-only | List the archive's internal entries with their CURRENT size, which reflects a pending replacement rather than the stale original. Name is null for an unnamed... |
| file_set | writes | Replace an internal file's bytes verbatim from a disk file and save the edited map to out_path. The input map is NEVER modified in place. Bytes are staged un... |
| force_list | read-only | List the map's forces/teams (war3map.w3i): each force's index, name, member player ids and alliance/sharing flags. |
| force_rename | writes | Rename a force (team) in war3map.w3i and save the edited map to out_path. The input map is NEVER modified in place. |
| force_set_flags | writes | Set a force's alliance/sharing flags and save the edited map to out_path. The input map is NEVER modified in place. |
| force_set_players | writes | Add or remove a player from a force (team) in war3map.w3i and save the edited map to out_path. Pass exactly one of add or remove. Adding clears the player fr... |
| gamedata_bounds | read-only | Report a field code's type, minimum and maximum bounds, whether it can be empty, and the object metadata file that defines it, in one build or every build wh... |
| gamedata_changelog | read-only | Diff two builds' object data field by field. Reports fields added, removed, retyped, or whose minimum or maximum bound changed, grouped by object metadata fi... |
| gamedata_coverage | read-only | List one row per schema folder with the build, its era, and which sections it holds (objects, triggers, script, enums), plus field and native counts, so a re... |
| gamedata_enum | read-only | Report the closed set of legal values for an enumerated object field type, from the build's UnitEditorData.txt, in one build or every build where it exists.... |
| gamedata_export_schemas | writes | Export the schema dataset. Copies every schema folder into a target directory, or packs the whole dataset into a single zip when out_path ends in .zip, for a... |
| gamedata_field_lookup | read-only | Find object field codes whose display name contains the query, in one build or all builds. Reports the code, display name, type and build for each match. |
| gamedata_history | read-only | Report whether a name is an object field code, a native, a type, a global or a function across every recorded build. Lists every build each kind appears in,... |
| gamedata_schema_diff | read-only | Compare two schema folders (produced by gamedata schema) and report added, removed and changed object fields, trigger functions, script types and natives, an... |
| gamedata_sources | read-only | Return the dataset's provenance index. For every schema folder it reports the build, its era, the sections present, the SHA-256 of each section file, and the... |
| hero_install | writes | Install a hero definition into a map and save the result to out_path. Remaps rawcodes that would collide, writes the hero's assets, and reports what the targ... |
| hero_lint | read-only | Validate a hero definition folder before installing it: assets present and hash-matched, objects installable, declared script functions present, no dangling... |
| hero_roster | read-only | List the playable heroes a map registers, with each hero's rawcode, display name, the string tags from its registration call (typically a role and a portrait... |
| imports_list | read-only | List the map's import table (war3map.imp) against what the archive actually holds, so a table entry with no file behind it and a file absent from the table b... |
| leak_ab | read-only | Diff two wc3watch sessions and report whether the handle and memory slope fell, each measured from its own forced garbage collection. The usual shape is one... |
| leak_benchmark | read-only | Run the leak detector over a set of synthetic maps, each with a known injected leak of one creator kind plus a clean control, and report true positive and fa... |
| leak_harness | writes | Emit a small in-map harness in the map's own language (jass or lua) that drives the named hot leak candidates on a short timer so the runtime handle and memo... |
| leak_join | read-only | Join a map's static leak list to a wc3watch session folder. Reads samples.csv and events.csv, fits the handle and memory slopes from the session's forced gar... |
| leak_list | read-only | The ranked static handle-leak list for a map's script plus the periodic timers and triggers that feed it, and every hot leak target by function. Read-only, a... |
| leak_model | read-only | Report the model-level leaks and waste across a map's imported models and textures, the join between the model audit and the import table. Finds an imported... |
| library_audit | read-only | Run the behavioural audit over every map in a folder and report the findings per map and the totals per check, so a whole library is ranked by how much it ne... |
| library_compare | read-only | Compare two folders of maps by file name and content hash, listing the maps only in the first folder, only in the second, and changed between them, plus the... |
| library_report | writes | Write one Markdown report over a folder of maps, a summary line, a per-map table and the aggregated findings, to out_path. The report is built from the same... |
| library_scan | read-only | Open every map in a folder through the loader and report per map its size in bytes, the build it targets if known, whether it loads, its archive entry count... |
| library_validate | read-only | Run the load validation over every map in a folder and list the ones that fail, with the reason. A map that cannot be opened counts as invalid. Opens one map... |
| lint | read-only | Pre-flight the map: does its script compile, does war3map.imp agree with what the archive holds, do referenced asset paths resolve, and is every entry a load... |
| lint_baseline | writes | Write the map's current lint findings to a baseline JSON file. A later gate, the CLI 'lint <map> --baseline <file>', then fails only on a finding this file d... |
| list_files | read-only | List the map archive's internal files, each with its name (null = unnamed/protected entry), size in bytes, whether wc3ctl knows/parses the format, and for un... |
| map_compat | read-only | Check a Warcraft III map against a target build (a schema folder path or build name). Reports compile-time incompatibilities (missing natives, globals, types... |
| map_info | read-only | Show a Warcraft III map's metadata: name, author, player count, playable dimensions and load diagnostics. |
| map_info_get | read-only | Read the map's editable scenario fields (war3map.w3i): name, author, description, recommended players, tileset, environment, fog and the map-option flags. Ri... |
| map_info_set | writes | Set a scenario field and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: MapName\|Author\|Description\|RecommendedPl... |
| map_optimize | writes | Shrink every imported model and texture inside a map in one pass and write a new map to out_path with only those files replaced, everything else byte for byt... |
| map_overview | read-only | One call per map: metadata (name, author, player count, dimensions), every archive file, the custom or modified object counts by Object Editor kind, and the... |
| mcp_audit_annotations | read-only | Audit the MCP tool surface itself. Reports every tool whose annotations disagree with its name (a mutating name marked read-only) and every tool that declare... |
| mcp_export_verify | read-only | Verify that a wc3-mcp checkout holds the same server source scripts/export-wc3-mcp.py would export from this repository, reporting every file that differs wi... |
| model_audit | read-only | Audit a .mdx or .mdl model for the resource waste and draw-nothing problems an optimiser can remove, and report what it holds. Finds duplicate textures, geos... |
| model_export_obj | writes | Export a map-imported model to Wavefront OBJ at out_path, writing an MTL and any map-resolvable textures converted to PNG beside it. The map itself is never... |
| model_import | writes | Import a foreign model file into a Warcraft III MDX at out_path. Accepts Wavefront OBJ (.obj), glTF 2.0 (.gltf and .glb), STL (.stl, binary and ASCII), PLY (... |
| model_lod | writes | Generate a level-of-detail chain for a .mdx or .mdl model, writing one decimated MDX per ratio into out_dir by running the optimiser once per ratio. ratios i... |
| model_optimize | writes | Decimate a .mdx or .mdl model's geometry with the quadric error metric, drop zero-area faces and duplicate textures, and write a smaller MDX at out_path, lea... |
| mpq_add | writes | Add or replace one internal archive file from a file on disk, byte-faithfully. Reports whether the name was added or replaced and whether the payload round-t... |
| mpq_bytes_diff | read-only | Compare two archives entry by entry by name. For each name present in both, report whether the payload SHA256 matches, and list names only one side carries.... |
| mpq_extract_all | writes | Extract every entry of a map archive to a directory, including unnamed protected blocks, through the same path-safety rule 'extract' uses. Read-only over the... |
| mpq_listfile | read-only | Report an archive's listfile coverage in three parts, the entries named by the listfile it ships, the entries recovered by the standard-name probe, and the e... |
| mpq_merge | writes | Merge two map archives into one, taking every readable named entry from both. A name in both is resolved by the preferred side and reported as a conflict. Wr... |
| mpq_name_recover | read-only | Resolve a dictionary of candidate names against the archive's hash table and report the names that hit unnamed blocks. Narrower than deprotect_map, which har... |
| mpq_rebuild | writes | Rebuild a map archive from scratch into a healthy one, re-placing every readable entry and dropping the blocks protection made unreadable or left nameless. R... |
| mpq_remove | writes | Remove one internal file by rebuilding the archive without it, and report whether the rebuild actually dropped the name. Writes a copy, the input map is neve... |
| mpq_strip_bookkeeping | writes | Save a copy of a map with the (listfile) and (attributes) bookkeeping entries dropped, then confirm the copy opens again. The input map is never overwritten.... |
| mpq_verify | read-only | Open every entry of a map archive and read it to its end, reporting any that fail with the error raised, plus a summary count. Use this to prove an archive i... |
| new_map | writes | Create a blank, World-Editor-openable map (.w3x) at out_path. No input map. Returns where it was written. |
| object_bulk_set | writes | Set one field on every object of a kind that matches every where clause, and save the edited copy to out_path. The input map is NEVER modified in place. A wh... |
| object_clone | writes | Deep-clone a map object under a new rawcode and save the edited map to out_path. The input map is NEVER modified in place. The clone copies the source's fiel... |
| object_deprecated | read-only | List object fields the target build's schema does not define but the map still sets, meaning the edit is ignored on that build. Read-only. The target is a sc... |
| object_export | read-only | Export the objects of one kind (base game data overlaid by the map's deltas) to a CSV file at out_path, one row per object, one column per field code, with a... |
| object_field_options | read-only | The legal values for one object-data field, taken from what the base game data actually uses for it. Ask this BEFORE object_set on an enumerated field, so th... |
| object_form | read-only | Get an object as an editable FORM rather than a flat field dump: fields grouped and ordered the way the World Editor groups them (Art, Combat, Stats, ...), f... |
| object_get | read-only | Get an object's merged fields (base game data overlaid by the map's deltas), each labeled 'base' or 'map', with field names resolved. Omit kind to auto-detec... |
| object_import | writes | Import a CSV of one kind back into the map, adding objects the map does not define and updating those it does, then save the edited copy to out_path. The inp... |
| object_list | read-only | List the map's custom/modified objects of one Object Editor kind: rawcode, base rawcode (null = created from scratch) and resolved name. Pass brief true to g... |
| object_migrate | writes | Rewrite a map's object data to a target build's field set, dropping every field the target does not define, and save the edited copy to out_path. The input m... |
| object_new | writes | Create a new custom object derived from a base rawcode (a fresh unused rawcode is allocated) and save the edited map to out_path. The input map is NEVER modi... |
| object_noop_fields | read-only | List object fields whose map value equals the base game value, so the edit changes nothing. Read-only. Needs a game_dir for the base values; without one the... |
| object_rawcode_suggest | read-only | Propose free four-character rawcodes for one Object Editor kind that collide with neither the map's objects nor the base game data. Read-only. Without a game... |
| object_rename | writes | Rename an object rawcode and save the edited copy to out_path. The input map is NEVER modified in place. With rewrite_refs true, every object-data field valu... |
| object_search | read-only | Full-text search across every custom or modified object's field values, in both the base game data and the map's own deltas. Read-only. Reports the kind, raw... |
| object_set | writes | Set a field on an object of one Object Editor kind and save the edited map to out_path (only that kind's war3map.* file is re-serialized). The input map is N... |
| palette_doodad | read-only | List the doodad types placeable on a map - the palette to consult before place_doodad. Unions the base-game doodad catalog (installed GameData) with the map'... |
| pathing_export_png | writes | Render a map's war3map.wpm pathing grid to a colour-coded PNG at out_path, one pixel per pathing cell with a legend strip of the pathing flags. Read-only, th... |
| pathing_import_png | writes | Set the pathing flags from a colour-coded PNG and save the edited map to out_path. Each cell's flags are replaced by the value its pixel decodes to. The imag... |
| pathing_paint | writes | Paint pathing bits over a circular or square brush and save the edited map to out_path. The input map is NEVER modified in place. Flags (comma-separated): Wa... |
| pathing_validate | read-only | Flag pathing problems in a map's war3map.wpm. It reports a pocket of unpathable tiles fully enclosed by pathable ones, a start location sitting on an unpatha... |
| place_doodad | writes | Place a doodad instance at (x, y) on the map's doodad layer (war3map.doo) and save the edited map to out_path. The input map is NEVER modified in place - out... |
| place_grid | writes | Place a regular rows x cols grid of copies of one unit or doodad type and save the edited copy to out_path. The grid is centred on the map origin. The input... |
| place_item | writes | Place a preplaced item of the given type at world (x, y) and save the edited map to out_path. Items share war3mapUnits.doo with units (WC3 spawns a ground it... |
| place_region | writes | Define a rectangular region on the map's region layer (war3map.w3r) and save the edited map to out_path. The input map is NEVER modified in place - out_path... |
| place_scatter | writes | Place N copies of a unit type at deterministic random positions and save the edited copy to out_path. Positions are uniform inside a named region or across t... |
| place_start_location | writes | Place or move a player's start location at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place. A start location is st... |
| place_unit | writes | Place a unit of the given type, owned by a player, at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place. Returns the... |
| placed_doodad_get | read-only | Show one placed doodad's full state by creation number: type, position, rotation, scale, variation and life percentage. |
| placed_doodad_remove | writes | Remove a doodad already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. |
| placed_doodad_set | writes | Set one field on a doodad already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. A position field takes 'x... |
| placed_doodads_list | read-only | List every doodad and destructable already PLACED on the map (war3map.doo), each with the creation number needed to edit or remove it, plus its type rawcode,... |
| placed_fix_refs | writes | Drop (or, with to_rawcode, rewrite) placements that reference a missing rawcode and save the edited copy to out_path, reporting each change. Refuses when not... |
| placed_missing_refs | read-only | List placements whose rawcode is defined neither in the map's object data nor in the base game data. Without a game_dir only the map's object data can be con... |
| placed_stats | read-only | Count the units and doodads already placed on the map, grouped by rawcode and by kind, with each type's resolved name where one is available. Read-only. |
| placed_unit_get | read-only | Show one placed unit's full state by creation number: type, owner, position, facing, scale, hero level and attributes, hit points and mana percentages, gold... |
| placed_unit_remove | writes | Remove a unit already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. |
| placed_unit_set | writes | Set one field on a unit already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. A position field takes 'x,y... |
| placed_units_list | read-only | List every unit already PLACED on the map (war3mapUnits.doo), each with the creation number needed to edit or remove it, plus its type rawcode, owner, positi... |
| player_list | read-only | List the map's player slots (war3map.w3i): each player's id, name, color, race, controller and whether its start position is fixed. |
| player_set_color | writes | Set a player's team color and save the edited map to out_path. war3map.w3i has no per-player color field, so this rewrites the SetPlayerColor call the map sc... |
| player_set_force | writes | Move a player into a force (team) and save the edited map to out_path. The input map is NEVER modified in place. |
| player_set_name | writes | Set a player's display name (war3map.w3i) and save the edited map to out_path. The input map is NEVER modified in place. |
| player_set_race | writes | Set a player's race (war3map.w3i) and save the edited map to out_path. Accepted values are human, orc, undead, nightelf and random. The input map is NEVER mo... |
| port_deps | read-only | Report the full dependency closure of a rawcode, its objects grouped by kind, its assets with their byte sizes, its display strings and its script functions.... |
| port_into_version | writes | Port a unit, then downgrade the result to a target build's schema. Object fields the build does not declare are removed and script identifiers it lacks are f... |
| port_preview | read-only | Dry run, listing exactly what a port of the rawcode into the target map would add or change, writing nothing. Same code path as port_unit, so the preview and... |
| port_unit | writes | Port a unit (its custom-object closure + imported assets + strings, and best-effort its JASS trigger closure) from a source map into a target map, auto-remap... |
| quickstart | read-only | Route a broad request to the right workflow. Returns the ten core workflows a map task usually falls into (inspect, port-hero, reforged-3-fix, black-portrait... |
| region_create | writes | Create a rectangular region from its south-west corner (x, y) plus a width w and height h, and save the edited map to out_path. The input map is NEVER modifi... |
| region_list | read-only | List the map's rectangular regions (war3map.w3r), each with its creation number, name, and bounds. Add regions with place_region. |
| region_remove | writes | Remove a region by name and save the edited map to out_path. The input map is NEVER modified in place. |
| region_to_rect | read-only | Resolve one named region to the four corner values a JASS trigger's Rect call needs (left, bottom, right, top), with a ready-to-copy Rect(...) line. Read-only. |
| region_validate | read-only | Flag overlapping regions, zero-area regions and regions that fall outside the playable bounds in a map's war3map.w3r. Regions sharing only an edge do not ove... |
| render_model | writes | Render an object's model (map-imported .mdx/.mdl) to a PNG written at out_path. Returns the saved path and byte size; small images also come back inline as M... |
| repair_generated | writes | Repair a map that wc3ctl itself generated, whose preplaced-hero helper block is too narrow for the heroes installed into it, and save the repaired map to out... |
| replay_diff | read-only | Compare two recorded games (.w3g). Reports whether they played the same map, the length difference, and each player's leave time difference, keyed by slot. R... |
| replay_find_map | read-only | The map path a replay (.w3g) records, and whether that map file exists in the Maps folder, resolving the .w3x or .w3m. Default folder is the user's Documents... |
| replay_players | read-only | Per-player table for one recorded game (.w3g). Reports name, slot, result, leave time and leave reason. Team and race are not carried by this replay parser,... |
| replay_stats | read-only | Aggregate stats for one recorded game (.w3g). Reports duration, player count, actions per minute per player when the record carries them, and the disconnect... |
| replay_summary | read-only | Read recorded games (.w3g) and report each one's map, length, players, and how and when every player left. A game where any player left with result 0x01 is f... |
| replay_timeline | read-only | Ordered event list for one recorded game (.w3g). Reports the game start, each chat line and player leave with its reason, and the game end time. Read-only. |
| roundtrip | read-only | Verify the map survives a load and save byte for byte. Any file that differs is one this toolkit does not reproduce faithfully, which is worth knowing BEFORE... |
| script_api_usage | read-only | List the natives and functions the map script uses that a target game build does not declare, the compile-time side of a port. Uses the same schema membershi... |
| script_callgraph | read-only | The call edges between the functions a map's war3map.j declares, caller to callee. Only edges between functions this script declares are included, so a nativ... |
| script_check | read-only | Check the map script for errors that stop it loading, war3map.j or war3map.lua. Lua is checked against Lua 5.3 exactly as luac 5.3.6 would, with its line and... |
| script_complexity | read-only | Measure each function's cyclomatic complexity as the number of if, loop, and and or branch keywords in its body, ranked worst first with the line. Answers wh... |
| script_conversion_notes | read-only | The rules the JASS to Lua converter applies and where it diverges, one row per construct, with the JASS form, the Lua it emits and whether that runs the same... |
| script_decompile | writes | Turn every Lua entry of a map that ships as precompiled Lua 5.3 bytecode (war3map.lua or a module it requires, common in YDWE maps) back into Lua source that... |
| script_duplicate_bodies | read-only | Find functions whose normalized bodies are identical, up to comments, whitespace and local variable names. A copied function is a maintenance trap, a fix in... |
| script_format | writes | Reprint a Lua map script and save the map to out_path. pretty is indented with comments kept, canonical is one fixed form for diffing two builds. The result... |
| script_functions | read-only | List every function declared in the map's compiled script, with where it starts and ends. Indentation-tolerant, so it finds declarations a naive line scan mi... |
| script_leaks | read-only | Find handle leaks in a map's war3map.j (locations, groups, forces, effects, timers, text tags, lightning and triggers created and never destroyed), each rank... |
| script_lua_api | read-only | List the natives and functions a Lua script calls that a target game build does not declare. Uses the same schema membership test as the compat check, and re... |
| script_lua_check | read-only | Parse every Lua file in a map and report the syntax errors with their line and column, the way luac 5.3 refuses them. Wider than script_check, which parses o... |
| script_lua_globals | read-only | List the globals a Lua map defines, where each is read and written, and any global that is written but never read. The Lua twin of script_unused_globals, par... |
| script_lua_leaks | read-only | Find handle leaks in a Lua map script. Lua collects the wrapper but the engine still owns the handle, so a creator dropped as a statement, a creator on a per... |
| script_lua_to_jass | writes | Best effort conversion of a Lua map back to JASS, saved to out_path. The JASS to Lua conversion is not reversible in general, so only the constructs that map... |
| script_modernize | writes | Rewrite deprecated or renamed natives in the map script to the target build's spelling where a known mapping exists, and save the map to out_path. A rewrite... |
| script_null_locals | read-only | Find the classic JASS handle leak: a local of a handle type (location, group, force, effect, timer, trigger, text tag, lightning) that the function destroys... |
| script_reachable | read-only | List the functions reachable from the map script's entry points, main and the InitTrig functions, and the dead functions nothing reaches, each with its decla... |
| script_references | read-only | Find every use of a name in the map's compiled script: direct calls, and the places it is passed as a code value (Condition, Filter, TriggerAddAction). The s... |
| script_semantic_diff | read-only | Compare two map scripts line by line after normalizing whitespace, dropping comments and ignoring pure renames of local variables, and report the real change... |
| script_to_lua | writes | Convert a JASS map to a Lua map and save it to out_path. The script becomes war3map.lua, which keeps the JASS rules Lua lacks (32 bit integer wrap, integer d... |
| script_unused_functions | read-only | Find the functions a map's war3map.j declares that no other function calls and nothing else reaches, excluding the engine's entry points (main, config, InitC... |
| script_unused_globals | read-only | Find the globals a map's war3map.j declares and never reads, the variable half of dead code. A global that is only ever assigned and never appears on the rig... |
| search | read-only | Search the map's contents for a string, ignoring case: archive file names, script lines (with the line number), string-table entries, and object data (custom... |
| sound_add | writes | Add a new sound definition (keyed by name) to the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place. |
| sound_list | read-only | List the map's sound catalog (war3map.w3s): each definition's name, file path, channel, flags, volume, pitch, priority and min/max distance. |
| sound_optimize | writes | Shrink a sound file by decoding it, resampling to a target rate (default 22050 Hz), optionally setting the channel count and encoding it. The format comes fr... |
| sound_remove | writes | Remove a sound definition from the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place. |
| sound_set | writes | Set a field on a sound definition and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: Name\|File\|Eax\|Volume\|Pitch\|... |
| strings_diff | read-only | Compare two maps' string tables (war3map.wts) and report entries added, removed and changed. Entries are matched by id, so a text change on the same key read... |
| strings_find | read-only | Find war3map.wts entries whose text contains the given text, case-insensitive, each with its id. Read-only. An empty query matches every entry. |
| strings_list | read-only | List the map's string table (war3map.wts). Every TRIGSTR_ reference in object data and in scripts resolves through here, so this is how to read what a toolti... |
| strings_missing | read-only | List the TRIGSTR_ keys the map script and the GUI trigger tree reference that have no war3map.wts entry behind them. A missing key shows the literal token TR... |
| strings_set | writes | Set one war3map.wts entry's text, or add the entry when the id is absent, and save the edited map to out_path. The file's own encoding and line endings are p... |
| strings_unused | read-only | List the war3map.wts entries no object-data field, map-info field, GUI trigger or script line references, each with its text. Read-only. An unreferenced entr... |
| target_get | read-only | View the current target build setting: which build maps are checked against when compat or lint omit --target, where it comes from (option, environment, sett... |
| target_list | read-only | List all available builds in the schemas directory, with their sections. Use --full_only to skip scripts-only builds. |
| target_set | writes | Set the target build that compat and lint use when --target is omitted. Validates the build exists in the schemas folder. |
| terrain_audit | read-only | Audit a map's terrain (war3map.w3e) for its size and its measurable waste: grid and tile counts, distinct ground and cliff textures in use, cliff/ramp/water/... |
| terrain_blight | writes | Set or clear the blight (corrupted ground) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place. |
| terrain_cliff | writes | Raise/lower/set the cliff (stepped-terrain) level over a brush, saving the edited map to out_path. The input map is NEVER modified in place. |
| terrain_corner_get | read-only | Show one terrain corner's full state: ground height, cliff level, water, ground and cliff texture indexes with variations, and the ramp, blight, boundary and... |
| terrain_corner_set | writes | Set one field on ONE terrain corner and save the edited copy to out_path. The input map is NEVER modified in place. For area work prefer terrain_deform, terr... |
| terrain_deform | writes | Raise/lower/set/flatten/smooth ground height over a circular or square brush, saving the edited map to out_path. The input map is NEVER modified in place. |
| terrain_export_heightmap | writes | Export a map's terrain heights as a 16-bit grayscale PNG of the (Width+1) by (Height+1) corner grid, one pixel per corner, to out_path. Read-only, the map is... |
| terrain_fill | writes | Apply a terrain tool over an inclusive RECTANGLE of corners and save the edited map to out_path, instead of one brush dab at a time. Corners may be given in... |
| terrain_import_heightmap | writes | Set the terrain tile heights from a grayscale PNG and save the edited map to out_path. The PNG dimensions must match the map's corner grid exactly or the cal... |
| terrain_info | read-only | The terrain grid's extents in corners, plus the map's ground and cliff tile lists. Texture fields on a corner are indexes into those lists, so read this firs... |
| terrain_optimize | writes | Shrink a map's war3map.w3e by dropping the declared ground and cliff tile types the terrain never references and remapping every corner's ground and cliff te... |
| terrain_paint | writes | Paint a ground texture over a brush, saving the edited map to out_path. texture_index selects a slot in the map's tileset table. The input map is NEVER modif... |
| terrain_ramp | writes | Toggle the ramp (sloped cliff transition) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place. |
| terrain_smooth | writes | Smooth the whole map: each pass replaces every corner height with the mean of itself and its in-grid four neighbours, saving the edited map to out_path. The... |
| terrain_stats | read-only | Summarize a map's terrain (war3map.w3e): tile count, min/mean/max ground height and min/max cliff level. Read-only. |
| terrain_water | writes | Set/raise/lower/remove water over a brush, saving the edited map to out_path. The input map is NEVER modified in place. |
| texture_atlas | writes | Pack two or more .blp, .dds or standard image textures into one power-of-two atlas and write it at out_path in the first input's format. Every input is decod... |
| texture_optimize | writes | Shrink a .blp, .dds or standard image texture to fit a maximum side, preserving aspect ratio, and write it at out_path in the same format. The input is never... |
| trigger_add | writes | Add a trigger to a category in the map's GUI trigger tree and save the edited map to out_path. The new trigger is created enabled and initially on, matching... |
| trigger_add_category | writes | Add a category to the map's GUI trigger tree and save the edited map to out_path. A category is a folder in the trigger tree, so this is always safe: categor... |
| trigger_add_eca | writes | Add an event, condition or action to a GUI trigger and save the edited map to out_path. Parameters are given in the order the function declares them; any lef... |
| trigger_catalog_describe | read-only | Show full detail for one GUI-trigger function by name (e.g. 'DoNothing'): kind, game version, return type, argument types, display name, parameters layout an... |
| trigger_catalog_list | read-only | List GUI-trigger functions from the World-Editor catalog (UI\TriggerData.txt), optionally filtered by kind and/or a name/display-name search. The catalog is... |
| trigger_dead | read-only | Find triggers that cannot do anything, listing each reason. Triggers with no action, and triggers whose events never fire, which is an event depending on a v... |
| trigger_diff | read-only | Compare two maps' GUI trigger trees and report triggers added, removed and renamed, plus event, condition or action changes within a trigger shared by both m... |
| trigger_export | read-only | Export a map's full GUI trigger tree as structured output. Each trigger carries its name, its enabled, initially-on and run-on-map-init flags, and its event,... |
| trigger_lint | read-only | Check a map's GUI trigger tree for three structural faults, a trigger that can never fire (no event and not run on map initialization), a trigger that regist... |
| trigger_recover_from_script | writes | Rebuild a browsable GUI trigger tree (war3map.wtg) for a map that has NONE, by decompiling the compiled script the game runs, and save the result to out_path... |
| trigger_remove | writes | Remove a trigger item (a category, a trigger or a comment) from the GUI trigger tree and save the edited map to out_path. Refuses rather than risk two silent... |
| trigger_remove_eca | writes | Remove one event, condition or action from a GUI trigger by its zero-based position, and save the edited map to out_path. Positions come from triggers_read,... |
| trigger_rename | writes | Rename a trigger item (a category, a trigger or a deleted stub) in the GUI trigger tree and save the edited map to out_path. The input map is NEVER modified... |
| trigger_set_code | writes | Set the custom-text JASS body of a custom-text trigger and save the edited map to out_path. The body is written into war3map.wct at the byte level, replacing... |
| trigger_set_eca_enabled | writes | Enable or disable ONE event, condition or action within a trigger, the World Editor's per-line toggle rather than the whole-trigger one, and save the edited... |
| trigger_set_enabled | writes | Enable or disable a GUI trigger and save the edited map to out_path. A disabled trigger is not compiled into the map script at all, so this is how you take o... |
| trigger_set_initially_on | writes | Set whether a GUI trigger starts switched on, and save the edited map to out_path. A trigger that is off at map start never fires until something turns it on... |
| trigger_set_run_on_map_init | writes | Set whether a GUI trigger runs on map initialization, and save the edited map to out_path. The input map is NEVER modified in place. |
| trigger_to_script | read-only | Render the map's GUI trigger tree (war3map.wtg) to JASS-style text, the way the World Editor's 'convert to custom text' presents one trigger. Read-only, it n... |
| trigger_var_add | writes | Add a global variable to the map's wtg and save the edited map to out_path. Provides a name, a type (for example integer, real, string, boolean, unit, group,... |
| trigger_var_list | read-only | List the map's global variables (the .wtg Variables list), each with its name, type, array flag, declared size and initial value. Read-only. |
| trigger_var_remove | writes | Remove a global variable from the map's wtg by name and save the edited map to out_path. Any trigger that referenced the variable is left pointing at nothing... |
| trigger_var_set | writes | Update an existing global variable in the map's wtg by name and save the edited map to out_path. Any argument left off keeps its current value, so one field... |
| triggers_read | read-only | Read the map's GUI trigger tree (war3map.wtg) plus the custom-text bodies (war3map.wct) and the script language: every category, every trigger with its event... |
| uabi_profile | read-only | Measure unit ability lists (uabi) across many maps at once, one row per map: unit types, references, distinct ids, hero and item abilities in normal lists, i... |
| unit_abilities | read-only | Every ability a unit has, each tagged with where it comes from. ObjectNormal and ObjectHero are the unit type's own lists (uabi, uhab). Spellbook is inside a... |
| validate | read-only | Check the map for structural problems: missing or empty files, entries that fail to load, and a script that cannot compile. Narrower than lint and a good fir... |
| validate_strict | read-only | A stricter validate. Runs the default checks and adds three that are too strict to be a default, a map with no (listfile), a script that declares no entry po... |
| version_info | read-only | Report the product version, the git short hash and UTC build time stamped into this build, and the name and version of the MCP server it carries. Read-only. |
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
