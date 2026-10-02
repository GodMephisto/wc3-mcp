// src/Wc3.Commands/PaletteCommand.cs
using System.Text;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// Enumerates the doodad types placeable on a map — the "palette" a caller consults
/// before <see cref="PlacementCommand.PlaceDoodad"/>. The base-game doodad catalog
/// (installed GameData) is unioned with the map's own object-data (war3map.w3d overlaid
/// by war3mapSkin.w3d): custom doodads (New*) and modified base doodads (Base*). Each
/// entry carries its four-char rawcode, a resolved display name (the map's name-field
/// delta wins over the base game's name; null when unresolvable), its source, the base
/// it derives from, and — for units — its icon art path (the map's 'uico' delta wins
/// over the base skin profile; decode via <see cref="IconPng(MapDocument, string,
/// string?)"/>). Deterministic order: base catalog by rawcode, then map-only
/// customs by rawcode. Without a WC3 install the base catalog is unavailable, so the
/// palette is map-only and the open diagnostic is surfaced (never throws).
/// </summary>
public static class PaletteCommand
{
    /// <summary>CLI/default overload: opens game data by locating the install (or override).</summary>
    public static DoodadPaletteResult DoodadPalette(MapDocument doc, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag);
        return DoodadPalette(doc, ctx, string.IsNullOrEmpty(diag) ? null : diag);
    }

    /// <summary>Core with an explicit (optionally null) game-data context — null = map-only,
    /// no base catalog and no base-name fallback. Reused by Studio, which holds an open ctx.</summary>
    internal static DoodadPaletteResult DoodadPalette(MapDocument doc, GameDataContext? ctx, string? openDiag = null)
    {
        var (entries, diagnostics, msg) =
            BuildPalette(doc, ctx, ObjectKind.Doodad, ctx?.Doodads.Rawcodes, "doodad", openDiag);
        return new DoodadPaletteResult(true, msg, entries, diagnostics);
    }

    /// <summary>CLI default overload for the unit palette — locates the install (or override).</summary>
    public static UnitPaletteResult UnitPalette(MapDocument doc, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag);
        return UnitPalette(doc, ctx, string.IsNullOrEmpty(diag) ? null : diag);
    }

    /// <summary>Unit palette — same union semantics as <see cref="DoodadPalette(MapDocument,
    /// GameDataContext?, string?)"/> over the unit catalog (war3map.w3u overlaid by
    /// war3mapSkin.w3u), with base names via the localized unit name table.</summary>
    internal static UnitPaletteResult UnitPalette(MapDocument doc, GameDataContext? ctx, string? openDiag = null)
    {
        var (entries, diagnostics, msg) =
            BuildPalette(doc, ctx, ObjectKind.Unit, ctx?.Units.Rawcodes, "unit", openDiag);
        return new UnitPaletteResult(true, msg, entries, diagnostics);
    }

    /// <summary>CLI default overload for the item palette. Locates the install (or override).</summary>
    public static ItemPaletteResult ItemPalette(MapDocument doc, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag);
        return ItemPalette(doc, ctx, string.IsNullOrEmpty(diag) ? null : diag);
    }

    /// <summary>Item palette over the item catalog (war3map.w3t overlaid by war3mapSkin.w3t).
    /// Base names resolve through the item profile TXTs. Icons come from the map's 'iico'
    /// delta or the item skin profile's Art. Items place via
    /// <see cref="PlacementCommand.PlaceItem"/>.</summary>
    internal static ItemPaletteResult ItemPalette(MapDocument doc, GameDataContext? ctx, string? openDiag = null)
    {
        var (entries, diagnostics, msg) =
            BuildPalette(doc, ctx, ObjectKind.Item, ctx?.Items.Rawcodes, "item", openDiag);
        return new ItemPaletteResult(true, msg, entries, diagnostics);
    }

    /// <summary>CLI default overload for the destructable palette. Locates the install (or override).</summary>
    public static DestructablePaletteResult DestructablePalette(MapDocument doc, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag);
        return DestructablePalette(doc, ctx, string.IsNullOrEmpty(diag) ? null : diag);
    }

    /// <summary>Destructable palette over the destructable catalog (war3map.w3b overlaid by
    /// war3mapSkin.w3b). Base names are SLK-backed WESTRING refs, resolved when the editor
    /// strings know them. Destructables place through
    /// <see cref="PlacementCommand.PlaceDoodad"/> because war3map.doo holds doodads and
    /// destructables alike.</summary>
    internal static DestructablePaletteResult DestructablePalette(
        MapDocument doc, GameDataContext? ctx, string? openDiag = null)
    {
        var (entries, diagnostics, msg) =
            BuildPalette(doc, ctx, ObjectKind.Destructable, ctx?.Destructables.Rawcodes, "destructable", openDiag);
        return new DestructablePaletteResult(true, msg, entries, diagnostics);
    }

    /// <summary>Kind-generic palette builder: unions the base-game catalog (<paramref
    /// name="baseRawcodes"/>) with the map's own object-data for <paramref name="kind"/>,
    /// folding the map's name/source in where they overlap. Deterministic order: base catalog
    /// by rawcode, then map-only codes by rawcode.</summary>
    private static (List<PaletteEntry> Entries, List<string> Diagnostics, string Message) BuildPalette(
        MapDocument doc, GameDataContext? ctx, ObjectKind kind,
        IEnumerable<string>? baseRawcodes, string noun, string? openDiag)
    {
        var info = ObjectKinds.Info(kind);
        var strings = MapStrings.From(doc);
        var diagnostics = new List<string>();
        if (openDiag != null) diagnostics.Add(openDiag);

        // Icon art (units only for now): base icon art lives in Reforged's skin-profile
        // TXT, not the SLK stores — same story as the model-file field in
        // RenderModelCommand — so the profile is parsed once for the whole catalog.
        var iconField = kind == ObjectKind.Unit ? UnitIconField : null;
        var skinIcons = iconField is null ? null : LoadSkinIcons(ctx);
        var mapIcon = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Map object-data, indexed by code so base-catalog rows can adopt the map's name
        // and source. A New* custom has Id != OldId (its own distinct code); a Base*
        // modification has Id == OldId (the same code as the base doodad it edits).
        var mapName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var mapSource = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var mapBase = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in ObjectKinds.MergedEntries(doc, info))
        {
            var code = e.Id.ToRawcode();
            var baseRawcode = e.OldId == 0 ? null : e.OldId.ToRawcode();
            var mods = ObjectKinds.ModsToDict(e.Mods);
            mapSource[code] = e.Id != e.OldId ? "map-custom" : "map-modified";
            mapBase[code] = baseRawcode;
            var name = ObjectKinds.DeltaName(mods, info, strings)
                ?? ObjectKinds.BaseName(ctx, kind, baseRawcode);
            if (name != null) mapName[code] = name;
            if (iconField != null && mods.TryGetValue(iconField, out var art)
                && !string.IsNullOrWhiteSpace(art))
                mapIcon[code] = FirstArtPath(art);
        }

        // The entry's icon: its own icon-field delta wins, then its own skin art
        // (base/map-modified rows), then the skin art of the base it derives from
        // (map-custom rows that never overrode the icon).
        string? IconOf(string code, string? baseRawcode) =>
            mapIcon.TryGetValue(code, out var delta) ? delta
            : skinIcons is not null && skinIcons.TryGetValue(code, out var own) ? own
            : skinIcons is not null && baseRawcode is not null
                && skinIcons.TryGetValue(baseRawcode, out var inherited) ? inherited
            : null;

        var entries = new List<PaletteEntry>();
        var emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Base-game catalog (sorted), with the map's name/source folded in where they overlap.
        var baseCodes = baseRawcodes?.ToList() ?? new List<string>();
        foreach (var code in baseCodes.OrderBy(c => c, StringComparer.OrdinalIgnoreCase))
        {
            if (!emitted.Add(code)) continue; // Rawcodes is already Distinct; guard anyway
            var name = mapName.TryGetValue(code, out var mn) ? mn : ObjectKinds.BaseName(ctx, kind, code);
            var source = mapSource.TryGetValue(code, out var ms) ? ms : "base";
            var baseRawcode = source == "map-modified" ? code : (string?)null;
            entries.Add(new PaletteEntry(code, name, source, baseRawcode, IconOf(code, baseRawcode)));
        }

        // Map-only codes absent from the base catalog (all of them when base is unavailable).
        foreach (var code in mapSource.Keys.OrderBy(c => c, StringComparer.OrdinalIgnoreCase))
        {
            if (!emitted.Add(code)) continue;
            entries.Add(new PaletteEntry(
                code, mapName.TryGetValue(code, out var mn) ? mn : null, mapSource[code], mapBase[code],
                IconOf(code, mapBase[code])));
        }

        var msg = baseCodes.Count == 0
            ? $"{entries.Count} placeable {noun} type(s) — map only (base catalog unavailable)"
            : $"{entries.Count} placeable {noun} type(s): {baseCodes.Count} base-game, {mapSource.Count} defined/overridden by map";
        return (entries, diagnostics, msg);
    }

    /// <summary>Object-data field that names a unit's command-button icon ("Interface Icon").</summary>
    private const string UnitIconField = "uico";

    /// <summary>Reforged's home for base unit icon art (the SLK stores never carry it —
    /// 'uico' is a Profile-backed field, see <see cref="BaseUnitStore"/>).</summary>
    private const string UnitSkinProfile = @"units\unitskin.txt";

    /// <summary>
    /// Every "Art" value of the unit skin profile keyed by section rawcode, parsed once
    /// per palette build (per-lookup parsing would be O(file) × catalog size). Null when
    /// the install/CASC or the profile is unavailable. First key per section wins, like
    /// RenderModelCommand's INI lookup.
    /// </summary>
    private static Dictionary<string, string>? LoadSkinIcons(GameDataContext? ctx)
    {
        if (ctx is null || !ctx.TryReadFile(UnitSkinProfile, out var bytes)) return null;
        var icons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? section = null;
        foreach (var raw in Encoding.UTF8.GetString(bytes).Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith('['))
            {
                int end = line.IndexOf(']');
                section = end > 1 ? line[1..end].Trim() : null;
                continue;
            }
            if (section is null || line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal))
                continue;
            int eq = line.IndexOf('=');
            if (eq <= 0 || !line[..eq].Trim().Equals("Art", StringComparison.OrdinalIgnoreCase))
                continue;
            var value = FirstArtPath(line[(eq + 1)..]);
            if (value.Length > 0 && !icons.ContainsKey(section))
                icons[section] = value;
        }
        return icons;
    }

    /// <summary>Art fields can list several paths comma-separated — take the first.</summary>
    private static string FirstArtPath(string value) => value.Split(',')[0].Trim().Trim('"');

    /// <summary>
    /// Front-end hook: decodes a palette entry's icon art path (see
    /// <see cref="PaletteEntry.IconPath"/>) to PNG bytes for display. The map's own files
    /// win (imported icons override base art), then the base game via CASC — Reforged
    /// repacks classic .blp/.tga icons as .dds at the same path, so both are tried and
    /// the container is sniffed. Null when the path resolves nowhere or the bytes don't
    /// decode — never throws (a palette without icons must still render).
    /// </summary>
    public static byte[]? IconPng(MapDocument doc, string iconPath, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);
        return IconPng(doc, iconPath, ctx);
    }

    /// <summary>Core with an explicit (optionally null = map-only) game-data context.</summary>
    internal static byte[]? IconPng(MapDocument doc, string iconPath, GameDataContext? ctx)
    {
        if (string.IsNullOrWhiteSpace(iconPath)) return null;
        try
        {
            var entry = doc.GetFile(iconPath)
                ?? doc.GetFile(iconPath.Replace('/', '\\'))
                ?? doc.GetFile(iconPath.Replace('\\', '/'));
            byte[]? bytes = entry is { RawBytes.Length: > 0 } ? entry.RawBytes : null;
            if (bytes is null && ctx is not null)
                foreach (var candidate in IconCandidates(iconPath))
                    if (ctx.TryReadFile(candidate, out var cascBytes)) { bytes = cascBytes; break; }
            if (bytes is null) return null;

            if (DdsDecoder.LooksLikeDds(bytes))
                return TexturePng.Encode(DdsDecoder.Decode(bytes));
            try
            {
                return TexturePng.Encode(BlpDecoder.Decode(bytes));
            }
            catch
            {
                // Not BLP/DDS — .tga (and friends) go through the ImageSharp-based converter.
                return TextureConvert.Convert(bytes, Path.GetExtension(iconPath), ".png");
            }
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Icon references keep classic .blp/.tga names, but Reforged's CASC stores
    /// those files repacked as .dds at the same path — try both (as model textures do).</summary>
    private static IEnumerable<string> IconCandidates(string path)
    {
        yield return path;
        var ext = Path.GetExtension(path);
        if (ext.Equals(".blp", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".tga", StringComparison.OrdinalIgnoreCase))
            yield return Path.ChangeExtension(path, ".dds");
    }
}
