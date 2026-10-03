using System.Globalization;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The seven Object Editor types, each backed by its own map file pair.</summary>
public enum ObjectKind { Unit, Item, Ability, Destructable, Doodad, Buff, Upgrade }

/// <summary>Per-kind wiring: map/skin file names, the object-name field code, plus
/// adapters over the per-kind stores and metadata in <see cref="GameDataContext"/>.</summary>
internal sealed record ObjectKindInfo(ObjectKind Kind, string MapFile, string SkinFile, string NameCode);

/// <summary>
/// A map object-data entry normalized across the three War3Net modification shapes
/// (Simple / Level / Variation). Id is the object's own rawcode (NewId for customs,
/// OldId for modified standards); OldId is the base (0 = base-less). Mod keys are the
/// bare field code, or "code:N" for per-level/per-variation values (N > 0).
/// </summary>
internal sealed record MapObjectEntry(int Id, int OldId, IReadOnlyList<KeyValuePair<string, string>> Mods);

public static class ObjectKinds
{
    // Name field codes confirmed against the live metadata SLKs: units/items share
    // "unam"; the rest follow the type's field-code prefix letter. Units resolve base
    // names via UnitNameTable; destructables/doodads carry SLK-backed WESTRING refs;
    // items/abilities/buffs/upgrades keep names in Profile TXTs (unresolved in v1).
    internal static ObjectKindInfo Info(ObjectKind kind) => kind switch
    {
        ObjectKind.Unit => new(kind, "war3map.w3u", "war3mapSkin.w3u", "unam"),
        ObjectKind.Item => new(kind, "war3map.w3t", "war3mapSkin.w3t", "unam"),
        ObjectKind.Ability => new(kind, "war3map.w3a", "war3mapSkin.w3a", "anam"),
        ObjectKind.Destructable => new(kind, "war3map.w3b", "war3mapSkin.w3b", "bnam"),
        ObjectKind.Doodad => new(kind, "war3map.w3d", "war3mapSkin.w3d", "dnam"),
        ObjectKind.Buff => new(kind, "war3map.w3h", "war3mapSkin.w3h", "fnam"),
        ObjectKind.Upgrade => new(kind, "war3map.w3q", "war3mapSkin.w3q", "gnam"),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    public static readonly IReadOnlyList<ObjectKind> All = (ObjectKind[])Enum.GetValues(typeof(ObjectKind));

    /// <summary>Parses a CLI kind token (case-insensitive full name, e.g. "ability").</summary>
    public static ObjectKind Parse(string token) =>
        token.Length > 0 && token.All(char.IsLetter)
        && Enum.TryParse<ObjectKind>(token, ignoreCase: true, out var kind)
            ? kind
            : throw new ArgumentException(
                $"unknown object kind '{token}' — expected one of: " +
                string.Join("|", All.Select(k => k.ToString().ToLowerInvariant())));

    /// <summary>
    /// The kind's entries from war3map.* overlaid by the Reforged war3mapSkin.* twin:
    /// union by object rawcode, per-field skin-wins, in first-seen order. Missing or
    /// unparsed files contribute nothing.
    /// </summary>
    internal static IReadOnlyList<MapObjectEntry> MergedEntries(MapDocument doc, ObjectKindInfo info)
    {
        var order = new List<int>();
        var byId = new Dictionary<int, MapObjectEntry>();
        foreach (var file in new[] { info.MapFile, info.SkinFile })
        {
            foreach (var e in Entries(doc.GetFile(file)?.Model))
            {
                if (byId.TryGetValue(e.Id, out var existing))
                    // Same object in both layers: skin mods append, so they win
                    // when the consumer folds Mods into a last-wins dictionary.
                    byId[e.Id] = existing with
                    {
                        OldId = existing.OldId != 0 ? existing.OldId : e.OldId,
                        Mods = existing.Mods.Concat(e.Mods).ToList(),
                    };
                else { byId[e.Id] = e; order.Add(e.Id); }
            }
        }
        return order.Select(id => byId[id]).ToList();
    }

    /// <summary>Folds an entry's mods into a last-wins dictionary (skin overlays map).</summary>
    internal static Dictionary<string, string> ModsToDict(IReadOnlyList<KeyValuePair<string, string>> mods)
    {
        var dict = new Dictionary<string, string>(mods.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in mods) dict[key] = value;
        return dict;
    }

    // Normalization across the three modification shapes (confirmed by reflection
    // against War3Net.Build.Core 6.0.3):
    //   w3u/w3t/w3b/w3h → Base*/New* : List<SimpleObjectModification>
    //     { OldId, NewId, Modifications: List<SimpleObjectDataModification { Id, Type, Value }> }
    //   w3a/w3q → List<LevelObjectModification>
    //     { ..., Modifications: List<LevelObjectDataModification { Level, Pointer, Id, Type, Value }> }
    //   w3d → List<VariationObjectModification>
    //     { ..., Modifications: List<VariationObjectDataModification { Variation, Pointer, Id, Type, Value }> }
    // int ids are little-endian rawcodes via ToRawcode/FromRawcode.
    private static IEnumerable<MapObjectEntry> Entries(object? model) => model switch
    {
        UnitObjectData m => m.BaseUnits.Concat(m.NewUnits).Select(Entry),
        ItemObjectData m => m.BaseItems.Concat(m.NewItems).Select(Entry),
        AbilityObjectData m => m.BaseAbilities.Concat(m.NewAbilities).Select(Entry),
        DestructableObjectData m => m.BaseDestructables.Concat(m.NewDestructables).Select(Entry),
        DoodadObjectData m => m.BaseDoodads.Concat(m.NewDoodads).Select(Entry),
        BuffObjectData m => m.BaseBuffs.Concat(m.NewBuffs).Select(Entry),
        UpgradeObjectData m => m.BaseUpgrades.Concat(m.NewUpgrades).Select(Entry),
        _ => Enumerable.Empty<MapObjectEntry>(),
    };

    private static MapObjectEntry Entry(SimpleObjectModification m) => new(
        m.NewId != 0 ? m.NewId : m.OldId, m.OldId,
        m.Modifications.Select(x => KeyValuePair.Create(x.Id.ToRawcode(), FormatValue(x.Value))).ToList());

    private static MapObjectEntry Entry(LevelObjectModification m) => new(
        m.NewId != 0 ? m.NewId : m.OldId, m.OldId,
        m.Modifications.Select(x => KeyValuePair.Create(Key(x.Id, x.Level), FormatValue(x.Value))).ToList());

    private static MapObjectEntry Entry(VariationObjectModification m) => new(
        m.NewId != 0 ? m.NewId : m.OldId, m.OldId,
        m.Modifications.Select(x => KeyValuePair.Create(Key(x.Id, x.Variation), FormatValue(x.Value))).ToList());

    private static string Key(int id, int levelOrVariation) =>
        levelOrVariation == 0 ? id.ToRawcode() : $"{id.ToRawcode()}:{levelOrVariation}";

    internal static string FormatValue(object? value) =>
        Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";

    /// <summary>Base-game default fields for a rawcode from the kind's store.</summary>
    internal static bool TryGetBaseFields(
        GameDataContext ctx, ObjectKind kind, string rawcode, out IReadOnlyDictionary<string, string> fields)
    {
        switch (kind)
        {
            case ObjectKind.Unit: return ctx.Units.TryGetUnit(rawcode, out fields);
            case ObjectKind.Ability: return ctx.Abilities.TryGetAbility(rawcode, out fields);
            case ObjectKind.Item: return ctx.Items.TryGet(rawcode, out fields);
            case ObjectKind.Destructable: return ctx.Destructables.TryGet(rawcode, out fields);
            case ObjectKind.Doodad: return ctx.Doodads.TryGet(rawcode, out fields);
            case ObjectKind.Buff: return ctx.Buffs.TryGet(rawcode, out fields);
            case ObjectKind.Upgrade: return ctx.Upgrades.TryGet(rawcode, out fields);
            default: fields = new Dictionary<string, string>(); return false;
        }
    }

    /// <summary>Field-name pipeline generalized over kinds: metadata displayName (a
    /// WESTRING key) → localized English string; fallbacks: the WESTRING key itself,
    /// then the raw key. Leveled keys ("code:N") look up by the bare code.</summary>
    internal static Func<string, string> FieldNameLookup(GameDataContext? ctx, ObjectKind kind) => key =>
    {
        int colon = key.IndexOf(':');
        var code = colon < 0 ? key : key[..colon];
        return ctx is not null && TryGetFieldDisplayName(ctx, kind, code, out var disp)
            ? (ctx.Strings.TryGet(disp, out var s) ? s : disp)
            : key;
    };

    private static bool TryGetFieldDisplayName(GameDataContext ctx, ObjectKind kind, string code, out string display)
    {
        display = "";
        switch (kind)
        {
            case ObjectKind.Unit:
                if (ctx.Units.Metadata.TryGet(code, out var um)) display = um.DisplayName;
                break;
            case ObjectKind.Ability:
                if (ctx.Abilities.Metadata.TryGet(code, out var am)) display = am.DisplayName;
                break;
            default:
                if (SimpleStore(ctx, kind).Metadata.TryGet(code, out var om)) display = om.DisplayName;
                break;
        }
        return display.Length > 0;
    }

    /// <summary>
    /// The legal values for a field, with their display names, preferring the enumeration the game
    /// ships over anything inferred.
    /// </summary>
    /// <remarks>
    /// Two sources, and the order matters. UnitEditorData.txt states the closed set for an
    /// enumerated type together with the names the World Editor shows. Only when the field's type
    /// names no such section does this fall back to collecting the distinct values the base data
    /// happens to use, which is a guess at a closed set and is wrong in two directions: it misses
    /// legal values no stock object uses, and it yields raw tokens, so a team colour field reads
    /// "-1, 0, 1, 2" instead of "None, Red, Blue, Teal".
    /// Empty means the field is genuinely free text or a number.
    /// </remarks>
    internal static IReadOnlyList<EnumOption> FieldOptions(
        GameDataContext? ctx, ObjectKind kind, string fieldCode, out string type, out bool isList)
    {
        type = "";
        isList = false;
        if (ctx is null) return Array.Empty<EnumOption>();

        int colon = fieldCode.IndexOf(':');
        var code = colon < 0 ? fieldCode : fieldCode[..colon];

        if (FieldMeta(ctx, kind) is { } meta && meta.TryGet(code, out var fm)) type = fm.Type;
        isList = type.EndsWith("List", StringComparison.OrdinalIgnoreCase);

        // The authoritative closed set, when the type names one.
        if (ctx.EditorEnums.TryGet(type, out var shipped) && shipped.Count > 0)
            return shipped;

        // Otherwise fall back to observation, and label each value with itself.
        TryGetFieldOptions(ctx, kind, fieldCode, out _, out _, out var observed);
        return observed.Select(v => new EnumOption(v, v)).ToList();
    }

    /// <summary>The kind's field metadata (the *MetaData.slk view), or null with no game data.
    /// One accessor so the form builder and the option lookup cannot read different tables.</summary>
    internal static ObjectMetadata? FieldMeta(GameDataContext? ctx, ObjectKind kind) => ctx switch
    {
        null => null,
        _ => kind switch
        {
            ObjectKind.Unit => ctx.Units.FieldMetadata,
            ObjectKind.Ability => ctx.Abilities.FieldMetadata,
            _ => SimpleStore(ctx, kind).Metadata,
        },
    };

    private static ObjectDataStore SimpleStore(GameDataContext ctx, ObjectKind kind) => kind switch
    {
        ObjectKind.Item => ctx.Items,
        ObjectKind.Destructable => ctx.Destructables,
        ObjectKind.Doodad => ctx.Doodads,
        ObjectKind.Buff => ctx.Buffs,
        ObjectKind.Upgrade => ctx.Upgrades,
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    /// <summary>
    /// A field's editor type token (from the kind's metadata SLK) plus the enumerated
    /// option set, generalized over every kind. Options are the distinct base-data values
    /// the field takes across all objects of the kind — correct-by-construction, so a
    /// dropdown/multiselect can only ever write a token the game already uses. For list
    /// types ("*List") the options are the individual comma-separated tokens; otherwise
    /// the whole values. Empty options ⇒ caller should fall back to free text. Leveled
    /// keys ("code:N") resolve by the bare code.
    /// </summary>
    internal static bool TryGetFieldOptions(
        GameDataContext? ctx, ObjectKind kind, string fieldCode,
        out string type, out bool isList, out IReadOnlyList<string> options)
    {
        type = "";
        isList = false;
        options = Array.Empty<string>();
        if (ctx is null) return false;

        int colon = fieldCode.IndexOf(':');
        var code = colon < 0 ? fieldCode : fieldCode[..colon];

        ObjectMetadata meta;
        Func<string, IEnumerable<string>> distinct;
        switch (kind)
        {
            case ObjectKind.Unit: meta = ctx.Units.FieldMetadata; distinct = ctx.Units.DistinctValues; break;
            case ObjectKind.Ability: meta = ctx.Abilities.FieldMetadata; distinct = ctx.Abilities.DistinctValues; break;
            default: var s = SimpleStore(ctx, kind); meta = s.Metadata; distinct = s.DistinctValues; break;
        }

        if (meta.TryGet(code, out var fm)) type = fm.Type;
        isList = type.EndsWith("List", StringComparison.OrdinalIgnoreCase);

        var values = distinct(code);
        var tokens = isList
            ? values.SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            : values;
        options = tokens
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return true;
    }

    /// <summary>
    /// The base game's name for a standard object, when resolvable: units via the
    /// localized name table; other kinds via the store's name field, whose SLK-backed
    /// values are WESTRING refs (an unresolvable ref is not a name → null). Kinds whose
    /// names live in Profile TXTs simply never resolve here.
    /// </summary>
    internal static string? BaseName(GameDataContext? ctx, ObjectKind kind, string? baseRawcode)
    {
        if (ctx is null || baseRawcode is null) return null;
        if (kind == ObjectKind.Unit)
            return ctx.UnitNames.TryGetName(baseRawcode, out var unitName) ? unitName : null;
        if (!TryGetBaseFields(ctx, kind, baseRawcode, out var fields)
            || !fields.TryGetValue(Info(kind).NameCode, out var raw) || raw.Length == 0)
            return null;
        if (ctx.Strings.TryGet(raw, out var resolved)) return resolved;
        return raw.StartsWith("WESTRING", StringComparison.OrdinalIgnoreCase) ? null : raw;
    }

    /// <summary>The object name from the map's name-field delta (bare code, else level 1
    /// for leveled kinds), TRIGSTR_ references resolved against war3map.wts.</summary>
    internal static string? DeltaName(
        IReadOnlyDictionary<string, string> deltaFields, ObjectKindInfo info, MapStrings strings)
    {
        var name = deltaFields.TryGetValue(info.NameCode, out var bare) ? bare
            : deltaFields.TryGetValue(info.NameCode + ":1", out var lvl1) ? lvl1 : null;
        if (name is null) return null;
        var resolved = strings.Resolve(name);
        return resolved.Length == 0 ? null : resolved;
    }
}
