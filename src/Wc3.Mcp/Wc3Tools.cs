// src/Wc3.Mcp/Wc3Tools.cs
using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Mcp;

/// <summary>
/// The MCP tool surface: one thin wrapper per Wc3.Commands handler. Each tool
/// loads the map(s), calls the shared command layer and returns its result POCO
/// (the SDK serializes it to JSON) — no map parsing or logic lives here.
/// </summary>
[McpServerToolType]
public static class Wc3Tools
{
    private const string KindValues = "unit|item|ability|destructable|doodad|buff|upgrade";

    [McpServerTool(Name = "map_info", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Show a Warcraft III map's metadata: name, author, player count, playable dimensions and load diagnostics.")]
    public static MapInfoResult MapInfo(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => InfoCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "list_files", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map archive's internal files: name (null = unnamed/protected entry), size in bytes, and whether this server knows and parses the format.")]
    public static FileListResult ListFiles(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => ListCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_leaks", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Find handle leaks in a map's war3map.j (locations, groups, forces, effects, timers, "
        + "text tags, lightning and triggers created and never destroyed), each ranked by how often "
        + "its code runs, hot (periodic), repeat (any callback) or once (init), plus every periodic "
        + "timer and trigger with its period. Read-only.")]
    public static ScriptLeaksResult ScriptLeaks(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => ScriptLeaksCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "uabi_profile", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Measure unit ability lists (uabi) across many maps at once, one row per map: unit "
        + "types, references, distinct ids, hero and item abilities in normal lists, ids neither the "
        + "map nor the game defines, duplicates, and a per-race breakdown. Built to compare the "
        + "variants of a disconnect bisection without starting the game.")]
    public static UabiProfileReport UabiProfile(
        [Description("Maps, or folders searched recursively.")] string[] paths,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it hero, item, race and dangling columns use the map's own fields only.")] string? game_dir = null)
        => Run(() => UabiProfileCommand.Execute(paths, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "replay_summary", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Read recorded games (.w3g) and report each one's map, length, players, and how "
        + "and when every player left. A game where any player left with result 0x01 is flagged "
        + "as a disconnect, which is how a dropped or desynced game is scored without anyone "
        + "writing results down. Reforged autosaves every game under Documents\\Warcraft III\\"
        + "BattleNet\\<account>\\Replays\\Autosaved.")]
    public static ReplayReport ReplaySummary(
        [Description("A .w3g file, or a folder searched recursively. Default: every Battle.net account's replays.")] string? path = null,
        [Description("Only games whose map path contains this text.")] string? map = null)
        => Run(() => ReplayCommand.Execute(path, map));

    [McpServerTool(Name = "object_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map's custom/modified objects of one Object Editor kind: rawcode, base rawcode (null = created from scratch) and resolved name.")]
    public static ObjectListResult ObjectList(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to resolve base-game names.")] string? game_dir = null)
        => Run(() => ObjectListCommand.Execute(LoadMap(map), ParseKind(kind), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "object_get", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Get an object's merged fields (base game data overlaid by the map's deltas), each labeled 'base' or 'map', with field names resolved. Omit kind to auto-detect it from the rawcode.")]
    public static MergedObjectResult ObjectGet(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Four-character object rawcode, e.g. 'hfoo' or 'u000'.")] string rawcode,
        [Description("Object kind: " + KindValues + ". Default: probe every kind (map deltas first, then base game data).")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it only the map's own deltas resolve.")] string? game_dir = null)
        => Run(() => kind is null
            ? ObjectGetCommand.Execute(LoadMap(map), rawcode, ResolveGameDir(game_dir))
            : ObjectGetCommand.Execute(LoadMap(map), ParseKind(kind), rawcode, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "object_set", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on an object of one Object Editor kind and save the edited map to out_path (only that kind's war3map.* file is re-serialized). The input map is NEVER modified in place. Field syntax: a bare 4-char field code, or 'code:N' to select level N (ability/upgrade) or variation N (doodad).")]
    public static ObjectSetToolResult ObjectSet(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Four-character object rawcode, e.g. 'hfoo' or 'u000'.")] string rawcode,
        [Description("Field to set: a 4-char field code, optionally ':N' for level/variation.")] string field,
        [Description("New value (parsed to the field's type: int/real/bool/string).")] string value,
        [Description("Object kind (selects which war3map.* file is edited): " + KindValues + ".")] string kind = "unit")
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectSetCommand.Execute(doc, ParseKind(kind), rawcode, field, value);
            if (!r.Ok) throw new McpException(r.Message);
            if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
            doc.Save(full);
            return new ObjectSetToolResult(full, r.Message, r.Warning);
        });

    [McpServerTool(Name = "object_new", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Create a new custom object derived from a base rawcode (a fresh unused rawcode is allocated) and save the edited map to out_path. The input map is NEVER modified in place. Returns the new rawcode.")]
    public static ObjectNewToolResult ObjectNew(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Four-character base object rawcode to derive from, e.g. 'hfoo'.")] string base_rawcode,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit")
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectNewCommand.Execute(doc, ParseKind(kind), base_rawcode);
            if (!r.Ok) throw new McpException(r.Message);
            if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
            doc.Save(full);
            return new ObjectNewToolResult(full, r.Message, r.NewRawcode);
        });

    [McpServerTool(Name = "bundle_unit", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Resolve everything a unit depends on - the porting preview: referenced objects (with custom-to-map flags), asset files, trigger strings, dependency edges and the JASS trigger-function closure.")]
    public static UnitBundle BundleUnit(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Four-character unit rawcode, e.g. 'u000'.")] string rawcode,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to tell base-game references from custom ones.")] string? game_dir = null)
        => Run(() => BundleCommand.ResolveUnit(LoadMap(map), rawcode, ResolveGameDir(game_dir)));

    /// <summary>PNGs up to this size also return inline as MCP image content.</summary>
    private const int InlineImageLimit = 1_000_000;

    [McpServerTool(Name = "render_model", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Render an object's model (map-imported .mdx/.mdl) to a PNG written at out_path. Returns the saved path and byte size; small images also come back inline as MCP image content. The map itself is never modified.")]
    public static CallToolResult RenderModel(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Four-character object rawcode (e.g. 'u000'), or an internal model path (contains '\\', '/' or '.').")] string rawcode,
        [Description("Output PNG file path. Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Object kind: unit|item|destructable|doodad. Default: auto-detect from the rawcode.")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_path))
                throw new McpException("out_path is required");
            var doc = LoadMap(map);

            // Anything that can't be a rawcode is treated as an internal model path (CLI parity).
            bool isPath = rawcode.Length != 4 || rawcode.IndexOfAny(new[] { '\\', '/', '.' }) >= 0;
            byte[] png = isPath
                ? RenderModelCommand.Execute(doc, rawcode)
                : kind is null
                    ? RenderModelCommand.Execute(doc, rawcode, ResolveGameDir(game_dir))
                    : RenderModelCommand.Execute(doc, ParseKind(kind), rawcode, ResolveGameDir(game_dir));

            string full = Path.GetFullPath(out_path);
            if (string.Equals(full, Path.GetFullPath(map), StringComparison.OrdinalIgnoreCase))
                throw new McpException("out_path must not be the map file itself");
            if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
            File.WriteAllBytes(full, png);

            bool inline = png.Length <= InlineImageLimit;
            var info = new RenderModelToolResult(full, png.Length, inline);
            var content = new List<ContentBlock>
            {
                new TextContentBlock { Text = JsonSerializer.Serialize(info, Wc3McpServer.JsonOptions) },
            };
            if (inline)
                content.Add(new ImageContentBlock { MimeType = "image/png", Data = png });
            return new CallToolResult { Content = content };
        });

    [McpServerTool(Name = "port_unit", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Port a unit (its custom-object closure + imported assets + strings, and best-effort its JASS trigger closure) from a source map into a target map, auto-remapping rawcode collisions. WRITES A NEW FILE '<target>.ported.<ext>' next to the target map (overwritten if it exists from a previous run) - the source and target maps themselves are NEVER modified. Returns the full port report: remaps, ported objects, copied files, inlined strings, script info and warnings.")]
    public static PortUnitToolResult PortUnit(
        [Description("Map to port FROM (.w3x/.w3m). Read-only.")] string source_map,
        [Description("Four-character rawcode of the unit to port, e.g. 'u000'.")] string rawcode,
        [Description("Map to port INTO (.w3x/.w3m). Read-only - the result is saved as a sibling '<target>.ported.<ext>' file.")] string target_map,
        [Description("Also carry the unit's JASS trigger-function closure into the target script (best effort). Default true.")] bool include_script = true,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() =>
        {
            var source = LoadMap(source_map);
            var target = LoadMap(target_map); // fresh load - ported output never feeds back into inputs
            var bundle = BundleCommand.ResolveUnit(source, rawcode, ResolveGameDir(game_dir));
            var report = PortCommand.PortUnit(source, bundle, target, includeScript: include_script);

            string targetFull = Path.GetFullPath(target_map);
            string outPath = Path.Combine(Path.GetDirectoryName(targetFull) ?? ".",
                Path.GetFileNameWithoutExtension(targetFull) + ".ported" + Path.GetExtension(targetFull));
            target.Save(outPath);
            return new PortUnitToolResult(outPath, report);
        });

    [McpServerTool(Name = "palette_doodad", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the doodad types placeable on a map - the palette to consult before place_doodad. Unions the base-game doodad catalog (installed GameData) with the map's own object-data: custom New* doodads and modified Base* doodads. Each entry carries its four-char rawcode, a resolved display name (null when unresolvable), its source (base|map-custom|map-modified) and the base it derives from. Without a WC3 install the palette is map-only. The map is never modified.")]
    public static DoodadPaletteResult PaletteDoodad(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => PaletteCommand.DoodadPalette(LoadMap(map), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "place_doodad", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a doodad instance at (x, y) on the map's doodad layer (war3map.doo) and save the edited map to out_path. The input map is NEVER modified in place - out_path must differ from it. Returns the assigned creation number.")]
    public static PlacementToolResult PlaceDoodad(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Four-character doodad type rawcode from palette_doodad, e.g. 'ATtr'.")] string type_rawcode,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y,
        [Description("World Z height offset. Default 0.")] float z = 0f,
        [Description("Facing angle in radians. Default 0.")] float rotation = 0f,
        [Description("Uniform scale. Default 1.")] float scale = 1f,
        [Description("Doodad variation index. Default 0.")] int variation = 0)
        => Run(() => SavePlacement(map, out_path, doc =>
        {
            var r = PlacementCommand.PlaceDoodad(doc, type_rawcode, x, y, z, rotation, scale, variation);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_region", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Define a rectangular region on the map's region layer (war3map.w3r) and save the edited map to out_path. The input map is NEVER modified in place - out_path must differ from it. Bounds need right>left and top>bottom. Returns the assigned creation number.")]
    public static PlacementToolResult PlaceRegion(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Region name (non-empty).")] string name,
        [Description("West edge, min X (world coordinate).")] float left,
        [Description("South edge, min Y (world coordinate).")] float bottom,
        [Description("East edge, max X (world coordinate).")] float right,
        [Description("North edge, max Y (world coordinate).")] float top)
        => Run(() => SavePlacement(map, out_path, doc =>
        {
            var r = PlacementCommand.PlaceRegion(doc, name, left, bottom, right, top);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_unit", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a unit of the given type, owned by a player, at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place. Returns the assigned creation number (the id triggers use to reference the unit).")]
    public static PlacementToolResult PlaceUnit(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Four-character unit type rawcode, e.g. 'hfoo' or 'u000'.")] string type_rawcode,
        [Description("Owning player id (0-based; 0 = red). Neutral players use the high ids (e.g. 24 = neutral hostile).")] int owner_id,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y,
        [Description("Height above ground in world units. Default 0.")] float z = 0f,
        [Description("Facing angle in radians. Default 0.")] float rotation = 0f,
        [Description("Uniform scale. Default 1.")] float scale = 1f)
        => Run(() => SavePlacement(map, out_path, doc =>
        {
            var r = PlacementCommand.PlaceUnit(doc, type_rawcode, owner_id, x, y, z, rotation, scale);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_start_location", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place or move a player's start location at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place. A start location is stored as a preplaced 'sloc' unit owned by the player; the World Editor permits exactly one per player, so if this player already has one it is moved (its creation number preserved) rather than duplicated. Returns the start location's creation number.")]
    public static PlacementToolResult PlaceStartLocation(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Player whose start location this is (0-based; 0 = red).")] int player,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y)
        => Run(() => SavePlacement(map, out_path, doc =>
        {
            var r = PlacementCommand.PlaceStartLocation(doc, player, x, y);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_item", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a preplaced item of the given type at world (x, y) and save the edited map to out_path. Items share war3mapUnits.doo with units (WC3 spawns a ground item because the rawcode is an item), so the item has no owning player. The input map is NEVER modified in place. Returns the assigned creation number.")]
    public static PlacementToolResult PlaceItem(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Four-character item type rawcode, e.g. 'bspd' (Boots of Speed).")] string type_rawcode,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y,
        [Description("Height above ground in world units. Default 0.")] float z = 0f,
        [Description("Facing angle in radians. Default 0.")] float rotation = 0f,
        [Description("Uniform scale. Default 1.")] float scale = 1f)
        => Run(() => SavePlacement(map, out_path, doc =>
        {
            var r = PlacementCommand.PlaceItem(doc, type_rawcode, x, y, z, rotation, scale);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    // ---- terrain editing -------------------------------------------------

    private const string ShapeValues = "circle|square";

    // Compile-time mirror of SoundCommand.EditableFields (attribute args must be const).
    private const string SoundFieldValues =
        "Name|File|Eax|Volume|Pitch|PitchVariance|FadeIn|FadeOut|Priority|Channel|Flags|" +
        "MinDistance|MaxDistance|DistanceCutoff|ConeInside|ConeOutside|ConeOutsideVolume";

    [McpServerTool(Name = "terrain_stats", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Summarize a map's terrain (war3map.w3e): tile count, min/mean/max ground height and min/max cliff level. Read-only.")]
    public static TerrainCommand.StatsResult TerrainStats(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => TerrainCommand.Stats(LoadMap(map)));

    [McpServerTool(Name = "terrain_deform", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Raise/lower/set/flatten/smooth ground height over a circular or square brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainDeform(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Brush centre tile X (0-based column into war3map.w3e).")] int center_x,
        [Description("Brush centre tile Y (0-based row into war3map.w3e).")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Height operation: raise|lower|set|flatten|smooth.")] string op = "raise",
        [Description("Step for raise/lower; target for set; ignored by flatten/smooth. WC3 world units.")] float amount = 1f,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Deform(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.HeightOp>(op, "op"), amount,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_cliff", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Raise/lower/set the cliff (stepped-terrain) level over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainCliff(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Cliff operation: raise|lower|set.")] string op = "raise",
        [Description("Step count for raise/lower; the absolute cliff level for set.")] int level = 1,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Cliff(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.CliffOp>(op, "op"), level,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_ramp", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Toggle the ramp (sloped cliff transition) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainRamp(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("true = set the ramp flag; false = clear it.")] bool on = true,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Ramp(doc, center_x, center_y, radius, on,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_paint", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Paint a ground texture over a brush, saving the edited map to out_path. texture_index selects a slot in the map's tileset table. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainPaint(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Ground texture slot index in the map's tileset table.")] int texture_index,
        [Description("Optional tile variation; omit to choose the default.")] int? variation = null,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Paint(doc, center_x, center_y, radius, texture_index, variation,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_water", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set/raise/lower/remove water over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainWater(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Water operation: set|raise|lower|remove. set/raise/lower flag the tile as water; remove clears the flag.")] string op = "set",
        [Description("Absolute water height for set; delta for raise/lower; ignored by remove. WC3 world units.")] float amount = 0f,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Water(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.WaterOp>(op, "op"), amount,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_blight", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set or clear the blight (corrupted ground) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainBlight(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("true = blight the tiles; false = clear blight.")] bool on = true,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle")
        => Run(() => SaveTerrain(map, out_path, doc =>
        {
            var r = TerrainCommand.Blight(doc, center_x, center_y, radius, on,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "deprotect_map", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Recover the file names a protected map stripped. A protected map ships no "
        + "(listfile), so most entries have no name and cannot be looked up by name at all. "
        + "Measured on one real map, 793 of 851 entries were nameless and recovery reached 832. "
        + "Names come from the map's own object data, its script literals, the texture paths its "
        + "models declare, any listfiles supplied, and an asset-name sweep of every block. "
        + "Read-only. Pass out_path to write a copy whose listfile carries the recovered names.")]
    public static NameRecoveryResult DeprotectMap(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Optional output map path. When given, the recovered names are written into a copy; the input map is NEVER modified in place. Named entries are recompressed, every other entry stays byte-faithful.")] string? out_path = null,
        [Description("Paths to extra name dictionaries, one name per line. Community listfiles and sibling maps' listfiles both work.")] string[]? listfiles = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = NameRecoveryCommand.Execute(doc, listfiles ?? Array.Empty<string>());
            if (!string.IsNullOrWhiteSpace(out_path))
            {
                NameRecoveryCommand.ApplyNames(doc, r);
                doc.Save(ResolveOutPath(map, out_path));
            }
            return r;
        });

    [McpServerTool(Name = "file_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the archive's internal entries with their CURRENT size, which reflects a "
        + "pending replacement rather than the stale original. Name is null for an unnamed entry, "
        + "which is what a protected map produces.")]
    public static IReadOnlyList<FileState> FileList(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => FileEditCommand.ListFiles(LoadMap(map)));

    [McpServerTool(Name = "file_get_text", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Read one internal file decoded as UTF-8, with any leading BOM stripped. Use it "
        + "for the override text files, war3mapSkin.txt and friends, that carry a map's string and "
        + "art overrides.")]
    public static FileTextResult FileGetText(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Internal archive path, for example 'war3mapSkin.txt'.")] string internal_path)
        => Run(() => new FileTextResult(internal_path,
            FileEditCommand.ReadText(LoadMap(map), internal_path)));

    [McpServerTool(Name = "file_set", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Replace an internal file's bytes verbatim from a disk file and save the edited "
        + "map to out_path. The input map is NEVER modified in place. Bytes are staged unchanged, "
        + "so a file carrying a BOM or CRLF round-trips exactly, which is what keeps the archive "
        + "byte-faithful. Content comes from a disk path rather than an argument because these "
        + "payloads are multi-line and full of pipes and colour codes.")]
    public static FileSetResult FileSet(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Internal archive path to replace or add, for example 'war3mapSkin.txt'.")] string internal_path,
        [Description("Disk file whose exact bytes become the new payload.")] string from)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = FileEditCommand.AddOrReplace(doc, internal_path, File.ReadAllBytes(from));
            doc.Save(full);
            return new FileSetResult(full, r.Name, r.Replaced, r.SizeBytes);
        });

    [McpServerTool(Name = "audit_map", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Behavioural audit of a map's object data, read-only. Where 'validate' answers "
        + "whether a map can LOAD, this answers whether its abilities do what they CLAIM, by "
        + "comparing the author's own tooltips against the data and by resolving every object "
        + "reference. Checks: level-gap, level-tooltip, tooltip-claim, orphan-ability, "
        + "requirement, portrait-risk, dangling-reference, missing-model (model paths in the script "
        + "or object data that load no file, which the engine logs and retries on every creation). "
        + "'requirement' and 'dangling-reference' "
        + "need an installed game and say so in a diagnostic rather than reporting a false clean. "
        + "It CANNOT see a trigger's damage formula, whether a status effect lands, or timing, so "
        + "a clean result means no claim in the map contradicts its data, not that the map is correct.")]
    public static AuditResult AuditMap(
        [Description("Path to a .w3x/.w3m map file.")] string map,
        [Description("Restrict to one check by name. Default: run all eight.")] string? check = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the requirement and dangling-reference checks are skipped, with a diagnostic.")] string? game_dir = null)
        => Run(() => AuditCommand.Execute(LoadMap(map), ResolveGameDir(game_dir),
            check is null ? null : new[] { check }));

    [McpServerTool(Name = "sound_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map's sound catalog (war3map.w3s): each definition's name, file path, channel, flags, volume, pitch, priority and min/max distance.")]
    public static IReadOnlyList<SoundFields> SoundList(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => SoundCommand.List(LoadMap(map)));

    [McpServerTool(Name = "sound_add", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a new sound definition (keyed by name) to the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static SoundToolResult SoundAdd(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Sound definition name (the label triggers/UI reference). Must be unique.")] string name,
        [Description("Sound file path, e.g. 'Sound\\Ambient\\...'. Optional.")] string? file = null)
        => Run(() => SaveSound(map, out_path, doc => SoundCommand.Add(doc, name, file)));

    [McpServerTool(Name = "sound_set", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on a sound definition and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + SoundFieldValues + ".")]
    public static SoundToolResult SoundSet(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Sound definition name to edit.")] string name,
        [Description("Field to set: " + SoundFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type: int/real/bool/string).")] string value)
        => Run(() => SaveSound(map, out_path, doc => SoundCommand.Set(doc, name, field, value)));

    [McpServerTool(Name = "sound_remove", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a sound definition from the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static SoundToolResult SoundRemove(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Sound definition name to remove.")] string name)
        => Run(() => SaveSound(map, out_path, doc => SoundCommand.Remove(doc, name)));

    // ---- camera catalog (war3map.w3c) ------------------------------------

    // Compile-time mirror of CameraCommand.EditableFields (attribute args must be const).
    private const string CameraFieldValues =
        "Name|TargetX|TargetY|ZOffset|Rotation|AngleOfAttack|TargetDistance|Roll|" +
        "FieldOfView|FarClippingPlane|NearClippingPlane|LocalPitch|LocalYaw|LocalRoll";

    [McpServerTool(Name = "camera_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map's cameras (war3map.w3c): each camera's name, target position, rotation, angle of attack, distance and field of view.")]
    public static IReadOnlyList<CameraFields> CameraList(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => CameraCommand.List(LoadMap(map)));

    [McpServerTool(Name = "camera_add", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a camera at a target position and save the edited map to out_path. The input map is NEVER modified in place. Rejects a blank or duplicate name.")]
    public static EditToolResult CameraAdd(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Camera name (unique).")] string name,
        [Description("Camera target X (world coordinate).")] float target_x,
        [Description("Camera target Y (world coordinate).")] float target_y)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var r = CameraCommand.Add(doc, name, target_x, target_y);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "camera_set", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on a camera and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + CameraFieldValues + ".")]
    public static EditToolResult CameraSet(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Camera name to edit.")] string name,
        [Description("Field to set: " + CameraFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type).")] string value)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var r = CameraCommand.Set(doc, name, field, value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "camera_remove", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a camera and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult CameraRemove(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Camera name to remove.")] string name)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var r = CameraCommand.Remove(doc, name);
            return (r.Ok, r.Message);
        }));

    // ---- pathing map (war3map.wpm) ---------------------------------------

    private const string PathingFlagValues = "Walk|Fly|Build|Blight|Water";

    [McpServerTool(Name = "pathing_paint", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Paint pathing bits over a circular or square brush and save the edited map to out_path. The input map is NEVER modified in place. Flags (comma-separated): " + PathingFlagValues + " - a SET bit RESTRICTS that capability (Walk set = ground units cannot walk there). op: set|clear|toggle.")]
    public static EditToolResult PathingPaint(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Brush centre pathing-cell X (4 cells per terrain tile).")] int center_x,
        [Description("Brush centre pathing-cell Y.")] int center_y,
        [Description("Brush radius in pathing cells (>= 0).")] int radius,
        [Description("Comma-separated pathing bits: " + PathingFlagValues + ".")] string flags,
        [Description("How the bits combine: set|clear|toggle. Default set.")] string op = "set",
        [Description("Brush footprint: circle|square. Default circle.")] string shape = "circle")
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var flagText = (flags ?? "").Replace(" ", "");
            if (!Enum.TryParse<War3Net.Build.Environment.PathingType>(flagText, ignoreCase: true, out var parsed))
                throw new McpException($"invalid pathing flag(s) '{flags}'; expected a comma-separated subset of: " + PathingFlagValues);
            var r = PathingCommand.Paint(doc, center_x, center_y, radius, parsed,
                ParseEnum<PathingCommand.BrushOp>(op, "op"),
                ParseEnum<PathingCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Ok ? $"{r.Message} ({r.CellsChanged} cells)" : r.Message);
        }));

    // ---- scenario: map-info, players, forces (war3map.w3i) ---------------

    // Compile-time mirror of MapInfoCommand.EditableFields (attribute args must be const).
    private const string MapInfoFieldValues =
        "MapName|Author|Description|RecommendedPlayers|Tileset|LightEnvironment|GlobalWeather|" +
        "SoundEnvironment|WaterTintColor|FogStyle|FogStartZ|FogEndZ|FogDensity|FogColor";

    [McpServerTool(Name = "map_info_get", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Read the map's editable scenario fields (war3map.w3i): name, author, description, recommended players, tileset, environment, fog and the map-option flags. Richer than map_info; this is the field set map_info_set edits.")]
    public static MapInfoFields MapInfoGet(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => MapInfoCommand.Read(LoadMap(map)));

    [McpServerTool(Name = "map_info_set", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a scenario field and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + MapInfoFieldValues + " (and the map-option booleans).")]
    public static EditToolResult MapInfoSet(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Field to set: " + MapInfoFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type).")] string value)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            MapInfoCommand.Set(doc, field, value); // throws on bad field -> clean MCP error via Run
            return (true, $"set {field} = {value}");
        }));

    [McpServerTool(Name = "player_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map's player slots (war3map.w3i): each player's id, name, color, race, controller and whether its start position is fixed.")]
    public static IReadOnlyList<PlayerInfo> PlayerList(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => PlayerForceCommand.GetPlayers(LoadMap(map)));

    [McpServerTool(Name = "player_set_force", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Move a player into a force (team) and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult PlayerSetForce(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Player id (0-based).")] int player_id,
        [Description("Force index (0-based).")] int force_index)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var r = PlayerForceCommand.SetPlayerForce(doc, player_id, force_index);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "force_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the map's forces/teams (war3map.w3i): each force's index, name, member player ids and alliance/sharing flags.")]
    public static IReadOnlyList<ForceInfo> ForceList(
        [Description("Path to a .w3x/.w3m map file.")] string map)
        => Run(() => PlayerForceCommand.GetForces(LoadMap(map)));

    [McpServerTool(Name = "force_set_flags", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a force's alliance/sharing flags and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult ForceSetFlags(
        [Description("Path to the source .w3x/.w3m map file. Read-only; the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Force index (0-based).")] int force_index,
        [Description("Allied.")] bool allied,
        [Description("Allied victory.")] bool allied_victory,
        [Description("Shared vision.")] bool shared_vision,
        [Description("Shared unit control.")] bool shared_unit_control,
        [Description("Shared advanced unit control.")] bool shared_adv_unit_control)
        => Run(() => SaveEdit(map, out_path, doc =>
        {
            var r = PlayerForceCommand.SetForceFlags(doc, force_index, allied, allied_victory,
                shared_vision, shared_unit_control, shared_adv_unit_control);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "new_map", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Create a blank, World-Editor-openable map (.w3x) at out_path. No input map. Returns where it was written.")]
    public static EditToolResult NewMap(
        [Description("Output map file path to create.")] string out_path,
        [Description("Map name. Default 'Blank Map'.")] string? name = null,
        [Description("Playable size in tiles per edge. Default 32.")] int? tiles = null)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_path)) throw new McpException("out_path is required");
            var opts = new BlankMapOptions();
            if (!string.IsNullOrEmpty(name)) opts = opts with { MapName = name! };
            if (tiles is { } t) opts = opts with { TileEdge = t };
            byte[] bytes;
            try { bytes = BlankMap.CreateArchiveBytes(opts); }
            catch (Exception ex) { throw new McpException(ex.Message); }
            string full = Path.GetFullPath(out_path);
            if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
            File.WriteAllBytes(full, bytes);
            return new EditToolResult(full, $"created \"{opts.MapName}\" ({opts.TileEdge}x{opts.TileEdge} tiles, {bytes.Length} bytes)");
        });

    [McpServerTool(Name = "trigger_catalog_list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List GUI-trigger functions from the World-Editor catalog (UI\\TriggerData.txt), optionally filtered by kind and/or a name/display-name search. The catalog is read from an explicit file when given, otherwise from the installed game.")]
    public static TriggerCatalogListResult TriggerCatalogList(
        [Description("Path to an explicit TriggerData.txt. When omitted, the catalog is read from the installed game.")] string? file = null,
        [Description("Warcraft III install directory. When omitted, uses the WC3_GAME_DIR env var, else auto-detects.")] string? game_dir = null,
        [Description("Filter by kind: event|condition|action|call. Optional.")] string? kind = null,
        [Description("Case-insensitive substring filter over function name and display name. Optional.")] string? search = null)
        => Run(() =>
        {
            var catalog = TriggerCatalogCommand.Load(file, ResolveGameDir(game_dir), out var source);
            return TriggerCatalogCommand.List(catalog, source, kind, search);
        });

    [McpServerTool(Name = "trigger_catalog_describe", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Show full detail for one GUI-trigger function by name (e.g. 'DoNothing'): kind, game version, return type, argument types, display name, parameters layout and defaults. The catalog is read from an explicit file when given, otherwise from the installed game.")]
    public static TriggerCatalogDetail TriggerCatalogDescribe(
        [Description("Function name (e.g. DoNothing).")] string name,
        [Description("Path to an explicit TriggerData.txt. When omitted, the catalog is read from the installed game.")] string? file = null,
        [Description("Warcraft III install directory. When omitted, uses the WC3_GAME_DIR env var, else auto-detects.")] string? game_dir = null)
        => Run(() =>
        {
            var catalog = TriggerCatalogCommand.Load(file, ResolveGameDir(game_dir), out _);
            return TriggerCatalogCommand.Describe(catalog, name)
                ?? throw new McpException($"No such function: {name}");
        });

    // ---- shared plumbing -------------------------------------------------

    /// <summary>Expected failures become clean MCP tool errors, never stack traces.</summary>
    private static T Run<T>(Func<T> body)
    {
        try { return body(); }
        catch (McpException) { throw; }
        catch (Exception ex) { throw new McpException(ex.Message); }
    }

    private static MapDocument LoadMap(string map)
    {
        if (string.IsNullOrWhiteSpace(map))
            throw new McpException("map path is required");
        if (!File.Exists(map))
            throw new McpException($"map not found: {map}");
        try { return MapDocument.Load(map); }
        catch (Exception ex) { throw new McpException($"could not load '{map}': {ex.Message}"); }
    }

    /// <summary>Load the map, apply a placement mutation, and save the edited copy to out_path.
    /// The input map is never modified in place (out_path must differ) - mirroring the no-clobber
    /// stance of render_model and port_unit. A rejected placement surfaces as a clean MCP error.</summary>
    private static PlacementToolResult SavePlacement(
        string map, string outPath, Func<MapDocument, (bool Ok, string Message, int CreationNumber)> place)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new McpException("out_path is required");
        string full = Path.GetFullPath(outPath);
        if (string.Equals(full, Path.GetFullPath(map), StringComparison.OrdinalIgnoreCase))
            throw new McpException("out_path must not be the input map file itself");

        var doc = LoadMap(map);
        var (ok, message, creationNumber) = place(doc);
        if (!ok) throw new McpException(message);

        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        doc.Save(full);
        return new PlacementToolResult(full, message, creationNumber);
    }

    /// <summary>Load the map, apply a terrain edit, and save the edited copy to out_path.</summary>
    private static TerrainToolResult SaveTerrain(
        string map, string outPath, Func<MapDocument, (bool Ok, string Message, int TilesChanged)> edit)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new McpException("out_path is required");
        string full = Path.GetFullPath(outPath);
        if (string.Equals(full, Path.GetFullPath(map), StringComparison.OrdinalIgnoreCase))
            throw new McpException("out_path must not be the input map file itself");

        var doc = LoadMap(map);
        var (ok, message, tilesChanged) = edit(doc);
        if (!ok) throw new McpException(message);

        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        doc.Save(full);
        return new TerrainToolResult(full, message, tilesChanged);
    }

    /// <summary>Load the map, apply a sound-catalog mutation, and save the edited copy to out_path.</summary>
    private static SoundToolResult SaveSound(
        string map, string outPath, Func<MapDocument, SoundOpResult> edit)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new McpException("out_path is required");
        string full = Path.GetFullPath(outPath);
        if (string.Equals(full, Path.GetFullPath(map), StringComparison.OrdinalIgnoreCase))
            throw new McpException("out_path must not be the input map file itself");

        var doc = LoadMap(map);
        var r = edit(doc);
        if (!r.Ok) throw new McpException(r.Message);

        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        doc.Save(full);
        return new SoundToolResult(full, r.Message);
    }

    /// <summary>Load the map, apply an (Ok, Message) mutation, and save the edited copy to
    /// out_path. Shared by the camera, pathing, map-info, player and force write tools.</summary>
    private static EditToolResult SaveEdit(
        string map, string outPath, Func<MapDocument, (bool Ok, string Message)> edit)
    {
        string full = ResolveOutPath(map, outPath);
        var doc = LoadMap(map);
        var (ok, message) = edit(doc);
        if (!ok) throw new McpException(message);

        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        doc.Save(full);
        return new EditToolResult(full, message);
    }

    /// <summary>Parse a string into an enum (case-insensitive); clean MCP error on failure.</summary>
    private static TEnum ParseEnum<TEnum>(string value, string paramName) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            return parsed;
        throw new McpException(
            $"invalid {paramName} '{value}'; expected one of: " +
            string.Join(", ", Enum.GetNames<TEnum>()).ToLowerInvariant());
    }

    /// <summary>Validate an out_path (required, must differ from the input) and return its full path.</summary>
    private static string ResolveOutPath(string map, string outPath)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new McpException("out_path is required");
        string full = Path.GetFullPath(outPath);
        if (string.Equals(full, Path.GetFullPath(map), StringComparison.OrdinalIgnoreCase))
            throw new McpException("out_path must not be the input map file itself");
        return full;
    }

    private static ObjectKind ParseKind(string kind)
    {
        try { return ObjectKinds.Parse(kind); }
        catch (ArgumentException ex) { throw new McpException(ex.Message); }
    }

    /// <summary>Explicit argument wins; else the WC3_GAME_DIR env var; else auto-detect.</summary>
    private static string? ResolveGameDir(string? game_dir) =>
        !string.IsNullOrWhiteSpace(game_dir) ? game_dir
        : Environment.GetEnvironmentVariable("WC3_GAME_DIR") is { Length: > 0 } env ? env
        : null;
}

/// <summary>render_model outcome: where the PNG landed and whether it was also inlined.</summary>
public sealed record RenderModelToolResult(string SavedTo, int SizeBytes, bool ImageInline);

/// <summary>port_unit outcome: where the new .ported map was written plus the full port report.</summary>
public sealed record PortUnitToolResult(string SavedTo, PortResult Report);

/// <summary>place_doodad / place_region outcome: where the edited map was written, a
/// human-readable message, and the creation number assigned to the new instance.</summary>
public sealed record PlacementToolResult(string SavedTo, string Message, int CreationNumber);

/// <summary>terrain_* edit outcome: where the edited map was written, a message, and tiles changed.</summary>
public sealed record TerrainToolResult(string SavedTo, string Message, int TilesChanged);

/// <summary>object_set outcome: where the edited map was written, a message, and any non-fatal warning.</summary>
public sealed record ObjectSetToolResult(string SavedTo, string Message, string? Warning);

/// <summary>object_new outcome: where the edited map was written, a message, and the newly allocated rawcode.</summary>
public sealed record ObjectNewToolResult(string SavedTo, string Message, string? NewRawcode);

/// <summary>sound_* write outcome: where the edited map was written plus a human-readable message.</summary>
public sealed record SoundToolResult(string SavedTo, string Message);

/// <summary>file_get_text outcome, the internal path asked for and its decoded text.</summary>
public sealed record FileTextResult(string Name, string Text);

/// <summary>file_set outcome. Replaced is true when an existing entry's payload was
/// superseded and false when a new entry was added, which the caller cannot otherwise tell.</summary>
public sealed record FileSetResult(string SavedTo, string Name, bool Replaced, int SizeBytes);

/// <summary>Generic write outcome for camera/pathing/map-info/player/force/new tools:
/// where the edited (or created) map was written plus a human-readable message.</summary>
public sealed record EditToolResult(string SavedTo, string Message);
