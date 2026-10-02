namespace Wc3.GameData;

/// <summary>
/// Resolves a base unit rawcode (e.g. "hfoo") to its default field values by joining
/// unitmetadata.slk with the unit data SLKs. Metadata "slk" values name the table
/// (e.g. "UnitBalance" -> unitbalance.slk); fields whose slk is "Profile" live in TXT
/// profile files and are not resolved in v1. The join itself lives in the generic
/// ObjectDataStore; the unit table set stays explicit so item fields (which share
/// unitmetadata.slk) resolve via the Items store instead.
/// </summary>
public sealed class BaseUnitStore
{
    private const string Dir = @"war3.w3mod:units\";
    private static readonly string[] UnitSlkNames = { "UnitData", "UnitBalance", "UnitWeapons", "UnitUI", "UnitAbilities" };

    private readonly UnitMetadata _meta;
    private readonly ObjectDataStore _store;

    private BaseUnitStore(UnitMetadata meta, ObjectDataStore store) { _meta = meta; _store = store; }

    /// <summary>Field-code metadata (display names) for consumers building lookups.</summary>
    public UnitMetadata Metadata => _meta;

    public static BaseUnitStore Build(IGameDataSource src)
    {
        var metaBytes = src.ReadFile(Dir + "unitmetadata.slk")
            ?? throw new InvalidDataException("unitmetadata.slk not found in game data");
        var metaTable = SlkTable.Parse(metaBytes);
        var store = ObjectDataStore.Build(src, ObjectMetadata.FromSlk(metaTable), ObjectDataStore.UnitsDirSlk, UnitSlkNames);
        return new BaseUnitStore(UnitMetadata.FromSlk(metaTable), store);
    }

    /// <summary>Field-code -> base value for the given unit; false if no field resolves.</summary>
    public bool TryGetUnit(string rawcode, out IReadOnlyDictionary<string, string> fieldsByCode)
        => _store.TryGet(rawcode, out fieldsByCode);

    /// <summary>Field metadata including the SLK `type` token (for typed editors).</summary>
    public ObjectMetadata FieldMetadata => _store.Metadata;

    /// <summary>Every unit rawcode in the base data (the unit catalog).</summary>
    public IEnumerable<string> Rawcodes => _store.Rawcodes;

    /// <summary>Distinct base values a field takes across all units (enum option set).</summary>
    public IEnumerable<string> DistinctValues(string fieldCode) => _store.DistinctValues(fieldCode);
}
