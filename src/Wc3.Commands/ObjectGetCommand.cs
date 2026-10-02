using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

public static class ObjectGetCommand
{
    /// <summary>
    /// Kind-agnostic pipeline (backward compatible): probes every Object Editor kind's
    /// map deltas, then every base store — first hit wins. Use the kind-aware overload
    /// to disambiguate rawcodes shared across kinds.
    /// </summary>
    public static MergedObjectResult Execute(MapDocument doc, string rawcode, string? gameDirOverride)
    {
        return GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic)
            ? Execute(doc, rawcode, ctx, Array.Empty<string>())
            : Execute(doc, rawcode, null, new[] { diagnostic });
    }

    /// <summary>
    /// Full pipeline for one kind: map deltas from war3map.* ⊕ war3mapSkin.* (skin wins
    /// per-field) ⊕ base game data. When the install/CASC is unavailable the result
    /// degrades to deltas-only plus a diagnostic (field names fall back to codes; the
    /// object name only resolves from a map delta).
    /// </summary>
    public static MergedObjectResult Execute(MapDocument doc, ObjectKind kind, string rawcode, string? gameDirOverride)
    {
        return GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic)
            ? Execute(doc, kind, rawcode, ctx, Array.Empty<string>())
            : Execute(doc, kind, rawcode, null, new[] { diagnostic });
    }

    /// <summary>Kind-agnostic core: map deltas of any kind, then any base store.</summary>
    internal static MergedObjectResult Execute(
        MapDocument doc, string rawcode, GameDataContext? ctx, IReadOnlyList<string> preDiagnostics)
    {
        if (rawcode.Length == 4)
        {
            int id = rawcode.FromRawcode();
            foreach (var kind in ObjectKinds.All)
            {
                var entry = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)).FirstOrDefault(e => e.Id == id);
                if (entry is not null) return MergeEntry(doc, kind, rawcode, entry, ctx, preDiagnostics);
            }
        }

        if (ctx is not null)
            foreach (var kind in ObjectKinds.All)
                if (ObjectKinds.TryGetBaseFields(ctx, kind, rawcode, out var baseFields))
                    return MergeStandard(kind, rawcode, baseFields, ctx, preDiagnostics);

        var none = new Dictionary<string, string>();
        return Merge(rawcode, null, definedInMap: false, name: null, none, none, code => code, preDiagnostics);
    }

    /// <summary>Core for one kind with an optional game-data context.</summary>
    internal static MergedObjectResult Execute(
        MapDocument doc, ObjectKind kind, string rawcode, GameDataContext? ctx, IReadOnlyList<string> preDiagnostics)
    {
        if (rawcode.Length == 4)
        {
            int id = rawcode.FromRawcode();
            var entry = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)).FirstOrDefault(e => e.Id == id);
            if (entry is not null) return MergeEntry(doc, kind, rawcode, entry, ctx, preDiagnostics);
        }

        // Not in the map: may still be a plain standard object of this kind.
        if (ctx is not null && ObjectKinds.TryGetBaseFields(ctx, kind, rawcode, out var baseFields))
            return MergeStandard(kind, rawcode, baseFields, ctx, preDiagnostics);

        var none = new Dictionary<string, string>();
        return Merge(rawcode, null, definedInMap: false, name: null, none, none, code => code, preDiagnostics);
    }

    /// <summary>A map-defined object: base fields (when a base resolves) ⊕ map/skin deltas.</summary>
    private static MergedObjectResult MergeEntry(
        MapDocument doc, ObjectKind kind, string rawcode, MapObjectEntry entry,
        GameDataContext? ctx, IReadOnlyList<string> diagnostics)
    {
        // A modified standard object is its own base; a from-scratch custom
        // object (OldId=0) has none — never rawcode-ify the zero id.
        string? baseRawcode = entry.OldId == 0 ? null : entry.OldId.ToRawcode();

        var deltaFields = ObjectKinds.ModsToDict(entry.Mods);

        IReadOnlyDictionary<string, string>? baseFields = null;
        if (baseRawcode is not null && ctx is not null
            && ObjectKinds.TryGetBaseFields(ctx, kind, baseRawcode, out var resolved))
            baseFields = resolved;

        // Object name: the map's name-field delta wins (TRIGSTR_ refs resolved via
        // war3map.wts); otherwise the base object's name when the kind has one.
        var strings = MapStrings.From(doc);
        string? name = ObjectKinds.DeltaName(deltaFields, ObjectKinds.Info(kind), strings)
            ?? ObjectKinds.BaseName(ctx, kind, baseRawcode);

        // Resolve TRIGSTR_ for DISPLAY (grid), keeping the raw value for editing/write-back.
        return Merge(rawcode, baseRawcode, definedInMap: true, name,
            baseFields ?? new Dictionary<string, string>(), deltaFields,
            ObjectKinds.FieldNameLookup(ctx, kind), diagnostics, strings.Resolve);
    }

    /// <summary>A standard object untouched by the map: base fields only.</summary>
    private static MergedObjectResult MergeStandard(
        ObjectKind kind, string rawcode, IReadOnlyDictionary<string, string> baseFields,
        GameDataContext ctx, IReadOnlyList<string> diagnostics)
    {
        return Merge(rawcode, rawcode, definedInMap: false, ObjectKinds.BaseName(ctx, kind, rawcode),
            baseFields, new Dictionary<string, string>(), ObjectKinds.FieldNameLookup(ctx, kind), diagnostics);
    }

    /// <summary>
    /// Pure merge: base fields overlaid by map deltas, each labeled with its source.
    /// A rawcode counts as Found when the map defines it (even with zero resolvable
    /// fields) or when any field resolved. Fields sort by display name, then bare
    /// code, then numeric level (leveled ability keys like "Ncl4:10").
    /// </summary>
    public static MergedObjectResult Merge(
        string rawcode, string? baseRawcode, bool definedInMap, string? name,
        IReadOnlyDictionary<string, string> baseFields,
        IReadOnlyDictionary<string, string> deltaFields,
        Func<string, string> nameLookup,
        IReadOnlyList<string> diagnostics,
        Func<string, string>? resolveDisplay = null)
    {
        string NameOf(string code)
        {
            var fieldName = nameLookup(code);
            return string.IsNullOrEmpty(fieldName) ? code : fieldName;
        }

        // Display value = TRIGSTR_ resolved (when a resolver is supplied); raw stays in Value.
        string Disp(string v) => resolveDisplay is null ? v : resolveDisplay(v);

        var merged = new Dictionary<string, MergedField>(StringComparer.OrdinalIgnoreCase);
        foreach (var (code, value) in baseFields)
            merged[code] = new MergedField(code, NameOf(code), value, "base") { Display = Disp(value) };
        foreach (var (code, value) in deltaFields)
            merged[code] = new MergedField(code, NameOf(code), value, "map") { Display = Disp(value) };

        var fields = merged.Values
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(f => BareCode(f.Code), StringComparer.OrdinalIgnoreCase)
            .ThenBy(f => LevelOf(f.Code))
            .ToList();
        return new MergedObjectResult(
            rawcode, definedInMap || fields.Count > 0, baseRawcode, name, fields, diagnostics);
    }

    private static string BareCode(string code)
    {
        int colon = code.IndexOf(':');
        return colon < 0 ? code : code[..colon];
    }

    private static int LevelOf(string code)
    {
        int colon = code.IndexOf(':');
        return colon >= 0 && int.TryParse(code[(colon + 1)..], out var level) ? level : 0;
    }
}
