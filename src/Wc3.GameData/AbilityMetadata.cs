namespace Wc3.GameData;

public sealed record AbilityFieldMeta(string Code, string SlkName, string Column, string DisplayName);

/// <summary>
/// Maps 4-char ability field codes (the same ids used by war3map.w3a deltas, e.g. "aher")
/// to where the base value lives. Confirmed shape of abilitymetadata.slk (same as units):
/// row key = "ID" column = the field code; "field" = the column name inside the data SLK;
/// "slk" = which data file holds it (AbilityData, or "Profile" for TXT profile files);
/// "displayName" = a WESTRING key. Leveled fields (repeat >= 1) store per-level values in
/// numbered columns (e.g. Cool -> cool1..cool4). The join in ObjectDataStore expands them
/// to "code:N" keys, the same convention the map-authored w3a level deltas use.
/// </summary>
public sealed class AbilityMetadata
{
    private readonly Dictionary<string, AbilityFieldMeta> _byCode = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<AbilityFieldMeta> Fields => _byCode.Values.ToList();

    public bool TryGet(string code, out AbilityFieldMeta meta) => _byCode.TryGetValue(code, out meta!);

    public static AbilityMetadata FromSlk(SlkTable meta)
    {
        var m = new AbilityMetadata();
        foreach (var code in meta.RowKeys)
        {
            if (string.IsNullOrWhiteSpace(code) || !meta.TryGetRow(code, out var row)) continue;
            string slk = row.TryGetValue("slk", out var s) ? s : "";
            string col = row.TryGetValue("field", out var f) ? f : "";
            string disp = row.TryGetValue("displayname", out var d) ? d : code;
            if (col.Length > 0)
                m._byCode[code] = new AbilityFieldMeta(code, slk, col, disp);
        }
        return m;
    }
}
