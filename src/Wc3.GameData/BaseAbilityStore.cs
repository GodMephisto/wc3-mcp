namespace Wc3.GameData;

/// <summary>
/// Resolves a base ability rawcode (e.g. "AHbz") to its default field values by joining
/// abilitymetadata.slk with the ability data SLKs. The data SLK set is derived from the
/// metadata's "slk" column (currently just AbilityData -> abilitydata.slk, keyed by "alias");
/// fields whose slk is "Profile" (names, tooltips, art) resolve from the ability
/// ProfileTxtStore when one is passed. The join itself lives in the generic ObjectDataStore.
/// </summary>
public sealed class BaseAbilityStore
{
    private const string Dir = @"war3.w3mod:units\";

    private readonly AbilityMetadata _meta;
    private readonly ObjectDataStore _store;

    private BaseAbilityStore(AbilityMetadata meta, ObjectDataStore store) { _meta = meta; _store = store; }

    /// <summary>Field-code metadata (display names) for consumers building lookups.</summary>
    public AbilityMetadata Metadata => _meta;

    public static BaseAbilityStore Build(IGameDataSource src) => Build(src, null);

    public static BaseAbilityStore Build(IGameDataSource src, ProfileTxtStore? profile)
    {
        var metaBytes = src.ReadFile(Dir + "abilitymetadata.slk")
            ?? throw new InvalidDataException("abilitymetadata.slk not found in game data");
        var metaTable = SlkTable.Parse(metaBytes);
        // "levels" is abilitydata.slk's per-row level count. It caps the leveled-field
        // expansion (cool1..cool4 and friends) at the ability's real level count.
        var store = ObjectDataStore.Build(
            src, ObjectMetadata.FromSlk(metaTable), ObjectDataStore.UnitsDirSlk, null, profile,
            levelColumn: "levels");
        return new BaseAbilityStore(AbilityMetadata.FromSlk(metaTable), store);
    }

    /// <summary>Field-code -> base value for the given ability; false if no field resolves.</summary>
    public bool TryGetAbility(string rawcode, out IReadOnlyDictionary<string, string> fieldsByCode)
        => _store.TryGet(rawcode, out fieldsByCode);

    /// <summary>Field metadata including the SLK `type` token (for typed editors).</summary>
    public ObjectMetadata FieldMetadata => _store.Metadata;

    /// <summary>Every ability rawcode in the base data (the ability catalog).</summary>
    public IEnumerable<string> Rawcodes => _store.Rawcodes;

    /// <summary>Distinct base values a field takes across all abilities (enum option set).</summary>
    public IEnumerable<string> DistinctValues(string fieldCode) => _store.DistinctValues(fieldCode);
}
