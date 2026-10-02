namespace Wc3.GameData;

/// <summary>
/// Generic base-object resolver: joins one Object Editor type's metadata SLK with its
/// data SLK(s) to map rawcode -> field-code -> default value. The per-type factories
/// encode the CASC paths and table-name -> file conventions confirmed against the live
/// install. Fields whose slk is "Profile" live in TXT profile files and resolve from an
/// optional <see cref="ProfileTxtStore"/> (when none is given they stay unresolved).
/// </summary>
public sealed class ObjectDataStore
{
    private const string UnitsDir = @"war3.w3mod:units\";
    private const string DoodadsDir = @"war3.w3mod:doodads\";

    private readonly ObjectMetadata _meta;
    private readonly Dictionary<string, SlkTable> _slks; // slk table name -> parsed table
    private readonly ProfileTxtStore _profile;
    // When set and the first code is missing, its value copies from the second code.
    // Buffs use this: WE shows Bufftip as the name when EditorName is absent.
    private readonly (string Code, string FallbackCode)? _nameFallback;
    // Data-SLK column holding the object's level count (abilities: "levels"). Caps the
    // leveled-field expansion so padded copies beyond the real levels stay hidden.
    private readonly string? _levelColumn;

    internal ObjectDataStore(
        ObjectMetadata meta, Dictionary<string, SlkTable> slks,
        ProfileTxtStore? profile = null, (string Code, string FallbackCode)? nameFallback = null,
        string? levelColumn = null)
    {
        _meta = meta;
        _slks = slks;
        _profile = profile ?? ProfileTxtStore.Empty;
        _nameFallback = nameFallback;
        _levelColumn = levelColumn;
    }

    /// <summary>A store that resolves nothing — used when a type's SLKs can't be read.</summary>
    public static ObjectDataStore Empty { get; } = new(new ObjectMetadata(), new(StringComparer.OrdinalIgnoreCase));

    /// <summary>Field-code metadata (display names) for consumers building lookups.</summary>
    public ObjectMetadata Metadata => _meta;

    public bool IsEmpty => _slks.Count == 0;

    /// <summary>
    /// Reads and joins the metadata SLK at <paramref name="metadataPath"/> with its data
    /// SLKs. Data tables come from <paramref name="slkNames"/> when given, otherwise from
    /// the metadata's distinct "slk" values; <paramref name="slkPath"/> maps a table name
    /// to its CASC path (null = skip). Missing data SLKs are skipped; a missing metadata
    /// SLK throws.
    /// </summary>
    public static ObjectDataStore Build(
        IGameDataSource src, string metadataPath, Func<string, string?> slkPath,
        IReadOnlyCollection<string>? slkNames = null, ProfileTxtStore? profile = null,
        (string Code, string FallbackCode)? nameFallback = null, string? levelColumn = null)
    {
        var metaBytes = src.ReadFile(metadataPath)
            ?? throw new InvalidDataException($"{metadataPath} not found in game data");
        return Build(src, ObjectMetadata.FromSlk(SlkTable.Parse(metaBytes)), slkPath, slkNames, profile, nameFallback, levelColumn);
    }

    internal static ObjectDataStore Build(
        IGameDataSource src, ObjectMetadata meta, Func<string, string?> slkPath,
        IReadOnlyCollection<string>? slkNames = null, ProfileTxtStore? profile = null,
        (string Code, string FallbackCode)? nameFallback = null, string? levelColumn = null)
    {
        var names = slkNames
            ?? (IReadOnlyCollection<string>)meta.Fields.Select(f => f.SlkName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var slks = new Dictionary<string, SlkTable>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            if (name.Length == 0 || name.Equals("Profile", StringComparison.OrdinalIgnoreCase)) continue;
            var path = slkPath(name);
            if (path is null) continue;
            var bytes = src.ReadFile(path);
            if (bytes != null) slks[name] = SlkTable.Parse(bytes);
        }
        return new ObjectDataStore(meta, slks, profile, nameFallback, levelColumn);
    }

    /// <summary>Field-code -> base value for the given object; false if no field resolves.</summary>
    public bool TryGet(string rawcode, out IReadOnlyDictionary<string, string> fieldsByCode)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        bool inCatalog = false;
        foreach (var fm in _meta.Fields)
        {
            if (!_slks.TryGetValue(fm.SlkName, out var table)) continue;
            if (!table.TryGetRow(rawcode, out var row)) continue;
            inCatalog = true;
            if (!AppliesTo(fm, rawcode)) continue;
            if (row.TryGetValue(fm.Column, out var val) && val.Length > 0)
                result[fm.Code] = val;
            else if (fm.Repeat > 0)
                AddLeveledSlkField(result, fm, row);
        }
        // Profile fields resolve only for rawcodes the data SLKs know. The ability
        // profile files double as the buff files, so an ungated join would leak
        // ability sections into buff queries and the other way round.
        if (inCatalog && !_profile.IsEmpty)
            foreach (var fm in _meta.Fields)
            {
                if (!fm.SlkName.Equals("Profile", StringComparison.OrdinalIgnoreCase)) continue;
                if (!AppliesTo(fm, rawcode)) continue;
                if (_profile.TryGetValue(rawcode, fm.Column, out var raw) && raw.Length > 0)
                    AddProfileField(result, fm, raw);
            }
        if (_nameFallback is { } fb && !result.ContainsKey(fb.Code)
            && result.TryGetValue(fb.FallbackCode, out var fallback))
            result[fb.Code] = fallback;
        fieldsByCode = result;
        return result.Count > 0;
    }

    /// <summary>
    /// Emits one profile value under the field's code. List-typed fields and index -1
    /// keep the whole value (a single quoted element still sheds its quotes). Otherwise
    /// the metadata index picks the element (x/y pairs share one key), and per-level
    /// lists (repeat > 0) additionally emit "code:N" keys matching the map-delta
    /// convention, the bare code keeping level 1 so name lookups stay flat.
    /// </summary>
    private static void AddProfileField(Dictionary<string, string> result, ObjectFieldMeta fm, string raw)
    {
        var elements = ProfileTxtStore.SplitElements(raw);
        if (fm.Index < 0 || fm.Type.EndsWith("List", StringComparison.OrdinalIgnoreCase))
        {
            result[fm.Code] = elements.Count == 1 ? elements[0] : raw;
            return;
        }
        if (fm.Index >= elements.Count) return;
        if (elements[fm.Index].Length > 0) result[fm.Code] = elements[fm.Index];
        if (fm.Repeat > 0 && elements.Count > 1)
            for (int level = 1; level <= elements.Count; level++)
                if (elements[level - 1].Length > 0)
                    result[$"{fm.Code}:{level}"] = elements[level - 1];
    }

    /// <summary>
    /// Metadata scoping: a non-empty useSpecific list restricts the field to the listed
    /// rawcodes (every ability data field carries one, e.g. Hbz1 applies to AHbz and its
    /// creep variants only, so Blizzard's dataa columns never surface under another
    /// ability's data codes) and notSpecific excludes the listed rawcodes. Generic
    /// fields leave both empty and apply everywhere.
    /// </summary>
    private static bool AppliesTo(ObjectFieldMeta fm, string rawcode)
    {
        if (fm.NotSpecific.Length > 0 && ListContains(fm.NotSpecific, rawcode)) return false;
        return fm.UseSpecific.Length == 0 || ListContains(fm.UseSpecific, rawcode);
    }

    private static bool ListContains(string commaList, string rawcode) =>
        commaList.Split(',').Any(t => t.Trim().Equals(rawcode, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Resolves a leveled SLK field (repeat > 0) whose per-level values live in numbered
    /// columns (Cool resolves from cool1..cool4). A nonzero metadata "data" tag appends
    /// its letter to the column base first (Data with data=2 reads DataB columns). Emits
    /// "code:N" keys matching the map-delta convention, the bare code keeping level 1 so
    /// flat lookups stay stable. The row's own level count, when the store knows that
    /// column, caps the expansion because the data SLKs pad the trailing column sets with
    /// copies of the last real level, and a single-level object stays bare only.
    /// </summary>
    private void AddLeveledSlkField(
        Dictionary<string, string> result, ObjectFieldMeta fm, IReadOnlyDictionary<string, string> row)
    {
        string column = fm.Data > 0 ? fm.Column + (char)('A' + fm.Data - 1) : fm.Column;
        int levels = fm.Repeat;
        if (_levelColumn is not null && row.TryGetValue(_levelColumn, out var count)
            && int.TryParse(count, out var n) && n > 0)
            levels = Math.Min(levels, n);
        string? last = null;
        for (int level = 1; level <= levels; level++)
        {
            if (!row.TryGetValue(column + level, out var v) || v.Length == 0) continue;
            if (level == 1) result[fm.Code] = v;
            if (levels > 1) result[$"{fm.Code}:{level}"] = v;
            last = v;
        }
        // Past the row's own level count the trailing columns are normally padding, copies of
        // the last real level, which is why they are dropped. Reforged 3.0.0 broke that
        // assumption. Breath of Fire (ANbf) declares 3 levels and its Area columns read
        // 90, 90, 90, 90 in 2.0.4 but 90, 90, 90, 60 in 3.0.0, so Area4 now carries a value
        // that is NOT a copy. Dropping it hid a real change from every caller, including a
        // custom ability that declares more levels than its base and therefore reaches it.
        // Padding is still dropped; a trailing column that DIFFERS is surfaced.
        for (int level = levels + 1; level <= fm.Repeat; level++)
        {
            if (!row.TryGetValue(column + level, out var v) || v.Length == 0) continue;
            if (last is not null && v == last) continue;      // genuine padding, stays hidden
            result[$"{fm.Code}:{level}"] = v;
        }
    }

    /// <summary>Every rawcode present in any loaded data SLK (the object catalog for this
    /// kind). Union across tables, case-insensitively de-duplicated.</summary>
    public IEnumerable<string> Rawcodes =>
        _slks.Values.SelectMany(t => t.RowKeys).Distinct(StringComparer.OrdinalIgnoreCase);

    /// <summary>Distinct non-empty base values a single field takes across every object of
    /// this kind — the correct-by-construction option set for an enumerated field (its
    /// tokens are exactly the ones the game already uses, so write-back can't corrupt).</summary>
    public IEnumerable<string> DistinctValues(string fieldCode)
    {
        if (!_meta.TryGet(fieldCode, out var fm)) yield break;
        if (!_slks.TryGetValue(fm.SlkName, out var table)) yield break;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in table.RowKeys)
            if (table.TryGetRow(key, out var row) && row.TryGetValue(fm.Column, out var v)
                && v.Length > 0 && seen.Add(v))
                yield return v;
    }

    /// <summary>Default table-name -> CASC path convention: lowercased + ".slk" under units\.</summary>
    public static string UnitsDirSlk(string name) => UnitsDir + name.ToLowerInvariant() + ".slk";

    // Per-type factories. Paths, data-table names, and rawcode key columns confirmed
    // against the live install:
    //   items:         unitmetadata.slk (item fields share it; slk "ItemData")
    //                    + units\itemdata.slk              (key column "itemID")
    //   destructables: units\destructablemetadata.slk ("DestructableData")
    //                    + units\destructabledata.slk      (key "DestructableID")
    //   doodads:       doodads\doodadmetadata.slk ("DoodadData")
    //                    + doodads\doodads.slk             (key "doodID")
    //   buffs:         units\abilitybuffmetadata.slk ("AbilityBuffData")
    //                    + units\abilitybuffdata.slk       (key "alias")
    //   upgrades:      units\upgrademetadata.slk ("UpgradeData")
    //                    + units\upgradedata.slk           (key "upgradeID")

    /// <summary>Item fields live in unitmetadata.slk; only the ItemData table is loaded so
    /// the store stays item-only (unit tables belong to BaseUnitStore).</summary>
    public static ObjectDataStore BuildItems(IGameDataSource src) => BuildItems(src, null);

    public static ObjectDataStore BuildItems(IGameDataSource src, ProfileTxtStore? profile) =>
        Build(src, UnitsDir + "unitmetadata.slk", UnitsDirSlk, new[] { "ItemData" }, profile);

    public static ObjectDataStore BuildDestructables(IGameDataSource src) =>
        Build(src, UnitsDir + "destructablemetadata.slk", UnitsDirSlk);

    /// <summary>Doodads live under doodads\; the "DoodadData" table's file is doodads.slk
    /// (not the lowercased table name).</summary>
    public static ObjectDataStore BuildDoodads(IGameDataSource src) =>
        Build(src, DoodadsDir + "doodadmetadata.slk",
            name => name.Equals("DoodadData", StringComparison.OrdinalIgnoreCase)
                ? DoodadsDir + "doodads.slk" : UnitsDirSlk(name));

    public static ObjectDataStore BuildBuffs(IGameDataSource src) => BuildBuffs(src, null);

    /// <summary>Buff sections live in the ability profile files, so callers pass the
    /// ability profile store. WE displays Bufftip (ftip) as the name when a buff has no
    /// EditorName (fnam), hence the name fallback.</summary>
    public static ObjectDataStore BuildBuffs(IGameDataSource src, ProfileTxtStore? profile) =>
        Build(src, UnitsDir + "abilitybuffmetadata.slk", UnitsDirSlk, null, profile,
            nameFallback: ("fnam", "ftip"));

    public static ObjectDataStore BuildUpgrades(IGameDataSource src) => BuildUpgrades(src, null);

    public static ObjectDataStore BuildUpgrades(IGameDataSource src, ProfileTxtStore? profile) =>
        Build(src, UnitsDir + "upgrademetadata.slk", UnitsDirSlk, null, profile);
}
