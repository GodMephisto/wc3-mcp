namespace Wc3.GameData;

/// <summary>Index picks one element out of a multi-value profile entry (-1 = the whole
/// value). Repeat > 0 marks a per-level list (one element per level). Data > 0 tags an
/// ability data field with its column letter (1 = A, so Data + 2 reads DataB columns).
/// UseSpecific, when non-empty, is the comma list of rawcodes the field applies to (all
/// ability data fields carry one) and NotSpecific lists rawcodes it never applies to.</summary>
public sealed record ObjectFieldMeta(
    string Code, string SlkName, string Column, string DisplayName, string Type,
    int Index = 0, int Repeat = 0, int Data = 0, string UseSpecific = "", string NotSpecific = "");

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
            if (col.Length > 0)
                m._byCode[code] = new ObjectFieldMeta(code, slk, col, disp, type, index, repeat, data, use, not);
        }
        return m;
    }
}
