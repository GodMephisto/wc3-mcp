// src/Wc3.Commands/BenchSchemaCoverageCommand.cs
namespace Wc3.Commands;

/// <summary>
/// One schema folder's object-data coverage. ObjectData is real, era-derived or absent, and
/// Fields is the object fields this folder carries itself, so a derived row shows 0 while its
/// era holds the real field set.
/// </summary>
public sealed record SchemaCoverageRow(string Build, string Era, string ObjectData, int Fields);

/// <summary>Corpus totals for the schema-coverage table.</summary>
public sealed record SchemaCoverageTotals(int Schemas, int Real, int EraDerived, int Absent, int Fields);

/// <summary>Every schema folder's coverage plus the totals, oldest build first.</summary>
public sealed record SchemaCoverageResult(
    IReadOnlyList<SchemaCoverageRow> Rows, SchemaCoverageTotals Totals);

/// <summary>
/// Measure what object data a schemas corpus actually carries. One row per schema folder names
/// the build, its era and whether its object data is real, era-derived or absent, and the totals
/// count each state and the real fields.
///
/// real, this folder read the object metadata SLKs and holds fields of its own.
/// era-derived, this folder carries no object data, but a sibling in the same era does, so the
/// build is covered by its era rather than by itself.
/// absent, no folder in the era carries object data, so the build's object fields are unknown.
///
/// This reuses the schema resolution in <see cref="GameSchemaCommand"/> and the era labelling in
/// <see cref="GamedataSupport"/>, which reads the build name through <see cref="GameBuildName"/>.
/// </summary>
public static class BenchSchemaCoverageCommand
{
    public const string Real = "real";
    public const string EraDerived = "era-derived";
    public const string Absent = "absent";

    /// <summary>Resolve the schemas directory and measure every schema folder in it.</summary>
    public static SchemaCoverageResult Execute(string? schemasDir = null) =>
        Analyze(GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Objects));

    /// <summary>
    /// Classify and total a set of loaded schemas. An era is object-covered when any schema in it
    /// carries real object fields, and every schema in that era without its own fields is then
    /// era-derived. Pure, so a caller can measure a result set without touching disk.
    /// </summary>
    public static SchemaCoverageResult Analyze(IReadOnlyList<LoadedSchema> schemas)
    {
        var ownFields = schemas.ToDictionary(s => s.Dir, s => s.Schema.Objects?.Values.Sum(f => f.Count) ?? 0);
        var eraFields = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var s in schemas)
        {
            string era = GamedataSupport.Era(s.Build);
            if (ownFields[s.Dir] > 0 && (eraFields.TryGetValue(era, out var cur) ? ownFields[s.Dir] > cur : true))
                eraFields[era] = ownFields[s.Dir];
        }

        var rows = new List<SchemaCoverageRow>(schemas.Count);
        foreach (var s in schemas)
        {
            string era = GamedataSupport.Era(s.Build);
            int fields = ownFields[s.Dir];
            string state = fields > 0 ? Real
                : eraFields.TryGetValue(era, out var covered) && covered > 0 ? EraDerived
                : Absent;
            rows.Add(new SchemaCoverageRow(s.Build, era, state, fields));
        }

        int real = rows.Count(r => r.ObjectData == Real);
        int derived = rows.Count(r => r.ObjectData == EraDerived);
        int absent = rows.Count(r => r.ObjectData == Absent);
        int totalFields = rows.Sum(r => r.Fields);
        return new SchemaCoverageResult(rows, new SchemaCoverageTotals(
            rows.Count, real, derived, absent, totalFields));
    }
}
