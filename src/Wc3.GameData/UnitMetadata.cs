namespace Wc3.GameData;

public sealed record UnitFieldMeta(string Code, string SlkName, string Column, string DisplayName);

/// <summary>
/// Maps 4-char object field codes (the same ids used by war3map.w3u deltas, e.g. "uhpm")
/// to where the base value lives. Confirmed shape of unitmetadata.slk:
/// row key = "ID" column = the field code; "field" = the column name inside the data SLK;
/// "slk" = which data file holds it (UnitData/UnitBalance/UnitWeapons/UnitUI/UnitAbilities,
/// or "Profile" for TXT profile files); "displayName" = a WESTRING key.
/// </summary>
public sealed class UnitMetadata
{
    private readonly Dictionary<string, UnitFieldMeta> _byCode = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<UnitFieldMeta> Fields => _byCode.Values.ToList();

    public bool TryGet(string code, out UnitFieldMeta meta) => _byCode.TryGetValue(code, out meta!);

    public static UnitMetadata FromSlk(SlkTable meta)
    {
        var m = new UnitMetadata();
        foreach (var code in meta.RowKeys)
        {
            if (string.IsNullOrWhiteSpace(code) || !meta.TryGetRow(code, out var row)) continue;
            string slk = row.TryGetValue("slk", out var s) ? s : "";
            string col = row.TryGetValue("field", out var f) ? f : "";
            string disp = row.TryGetValue("displayname", out var d) ? d : code;
            if (col.Length > 0)
                m._byCode[code] = new UnitFieldMeta(code, slk, col, disp);
        }
        return m;
    }
}
