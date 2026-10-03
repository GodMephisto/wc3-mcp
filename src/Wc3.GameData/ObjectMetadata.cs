namespace Wc3.GameData;

/// <summary>Index picks one element out of a multi-value profile entry (-1 = the whole
/// value). Repeat > 0 marks a per-level list (one element per level). Data > 0 tags an
/// ability data field with its column letter (1 = A, so Data + 2 reads DataB columns).
/// UseSpecific, when non-empty, is the comma list of rawcodes the field applies to (all
/// ability data fields carry one) and NotSpecific lists rawcodes it never applies to.</summary>
public sealed record ObjectFieldMeta(
    string Code, string SlkName, string Column, string DisplayName, string Type,
    int Index = 0, int Repeat = 0, int Data = 0, string UseSpecific = "", string NotSpecific = "",
    string Category = "", string Sort = "", string MinValue = "", string MaxValue = "",
    bool ForceNonNegative = false, bool CanBeEmpty = false, int StringExt = 0,
    bool UseHero = true, bool UseUnit = true, bool UseBuilding = true, bool UseItem = true,
    string NetSafe = "")
{
    /// <summary>Whether the field applies to an object of this shape, from the four use* flags.
    /// The World Editor hides the rest, which is how it shows a readable form instead of every
    /// field the kind defines.</summary>
    public bool AppliesTo(bool isHero, bool isBuilding, bool isItem) =>
        isItem ? UseItem : isBuilding ? UseBuilding : isHero ? UseHero : UseUnit;

    /// <summary>Which object-data layer this field is written to. "1" and "11" mean the
    /// war3mapSkin twin, "0" means war3map.*, empty means the metadata does not say.</summary>
    public bool IsSkinField => NetSafe is "1" or "11";

    /// <summary>
    /// Whether the field applies to one particular object, by rawcode, as opposed to the whole
    /// kind. Checked against the object's own code AND its base, because a custom object keeps
    /// its base's field set while the metadata names only the base.
    /// </summary>
    /// <remarks>
    /// This is what keeps an ability editable. 708 of the 777 ability fields carry a UseSpecific
    /// list, because an ability's data fields belong to its TYPE, so Channel's fields have
    /// nothing to do with Blizzard's. Ignoring the list showed 769 fields for one ability where
    /// the World Editor shows about 70, and that is not a longer form, it is an unusable one.
    ///
    /// A field with no UseSpecific is general and applies to every object of the kind.
    /// NotSpecific is an explicit exclusion and wins outright.
    /// </remarks>
    public bool AppliesToObject(string? rawcode, string? baseRawcode)
    {
        if (NotSpecific.Length > 0 && Names(NotSpecific, rawcode, baseRawcode)) return false;
        if (UseSpecific.Length == 0) return true;
        return Names(UseSpecific, rawcode, baseRawcode);
    }

    private static bool Names(string list, string? rawcode, string? baseRawcode)
    {
        foreach (var token in list.Split(',', StringSplitOptions.RemoveEmptyEntries
                                             | StringSplitOptions.TrimEntries))
        {
            if (rawcode is not null && token.Equals(rawcode, StringComparison.OrdinalIgnoreCase))
                return true;
            if (baseRawcode is not null
                && token.Equals(baseRawcode, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

/// <summary>
/// Type-agnostic view of the metadata SLK shape shared by every Object Editor type
/// (units, items, abilities, buffs, destructables, doodads, upgrades). Confirmed shape:
/// row key = "ID" column = the 4-char field code (matches w3u/w3t/w3a/w3h/w3b/w3d/w3q
/// mod codes); "field" = the column name inside the data SLK, or the profile key when
/// "slk" says "Profile" (resolved via <see cref="ProfileTxtStore"/>); "displayName" = a
/// WESTRING key.
/// </summary>
public sealed class ObjectMetadata
{
    private readonly Dictionary<string, ObjectFieldMeta> _byCode = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<ObjectFieldMeta> Fields => _byCode.Values.ToList();

    public bool TryGet(string code, out ObjectFieldMeta meta) => _byCode.TryGetValue(code, out meta!);

    public static ObjectMetadata FromSlk(SlkTable meta)
    {
        var m = new ObjectMetadata();
        foreach (var code in meta.RowKeys)
        {
            if (string.IsNullOrWhiteSpace(code) || !meta.TryGetRow(code, out var row)) continue;
            string slk = row.TryGetValue("slk", out var s) ? s : "";
            string col = row.TryGetValue("field", out var f) ? f : "";
            string disp = row.TryGetValue("displayname", out var d) ? d : code;
            string type = row.TryGetValue("type", out var t) ? t : "";
            int index = row.TryGetValue("index", out var ix) && int.TryParse(ix, out var i) ? i : 0;
            int repeat = row.TryGetValue("repeat", out var rp) && int.TryParse(rp, out var r) ? r : 0;
            int data = row.TryGetValue("data", out var dt) && int.TryParse(dt, out var dv) ? dv : 0;
            string use = row.TryGetValue("usespecific", out var us) ? us : "";
            string not = row.TryGetValue("notspecific", out var ns) ? ns : "";

            // The columns the World Editor builds its whole object form from. Without them a
            // client can only render one flat alphabetical list of every field the kind
            // defines, which for a unit is 273 rows and unusable.
            string Str(string c) => row.TryGetValue(c, out var v) ? v.Trim() : "";
            bool Flag(string c, bool fallback) =>
                row.TryGetValue(c, out var v) && int.TryParse(v.Trim(), out var n) ? n != 0 : fallback;
            int Num(string c) => row.TryGetValue(c, out var v) && int.TryParse(v.Trim(), out var n) ? n : 0;

            if (col.Length > 0)
                m._byCode[code] = new ObjectFieldMeta(code, slk, col, disp, type, index, repeat, data,
                    use, not,
                    Category: Str("category"), Sort: Str("sort"),
                    MinValue: Str("minval"), MaxValue: Str("maxval"),
                    ForceNonNegative: Flag("forcenonneg", false), CanBeEmpty: Flag("canbeempty", true),
                    StringExt: Num("stringext"),
                    // A column the ability metadata omits entirely means "applies", not "does not".
                    UseHero: Flag("usehero", true), UseUnit: Flag("useunit", true),
                    UseBuilding: Flag("usebuilding", true), UseItem: Flag("useitem", true),
                    NetSafe: Str("netsafe"));
        }
        return m;
    }
}
