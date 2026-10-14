// src/Wc3.Commands/CiSchemaCommand.cs
namespace Wc3.Commands;

/// <summary>
/// One schema folder's parse outcome. Parsed is false when the folder held a manifest.json but
/// loading it threw, and Error carries the message in that case. Build falls back to the folder
/// name when the manifest never loaded.
/// </summary>
public sealed record CiSchemaBuildResult(string Build, string Dir, bool Parsed, string? Error);

/// <summary>
/// Outcome of the schema gate. Passed is true when every folder parsed and the real-object-data
/// count met the floor, ExitCode is 0 or 1, Gates names what was checked, Builds is the per-folder
/// parse outcome, Coverage is the coverage measurement over the folders that parsed, and MinReal is
/// the floor that was applied.
/// </summary>
public sealed record CiSchemaResult(
    bool Passed,
    int ExitCode,
    IReadOnlyList<CiGateLine> Gates,
    IReadOnlyList<CiSchemaBuildResult> Builds,
    SchemaCoverageResult Coverage,
    string SchemasDir,
    int MinReal);

/// <summary>
/// The schema dataset CI gate. It checks the dataset for regressions, that every schema folder still
/// parses, and that the number of builds carrying real object data is at least a floor. Each folder
/// is loaded on its own so one broken build is named rather than aborting the whole check, which is
/// what loading the corpus in one pass would do. Read-only.
/// </summary>
public static class CiSchemaCommand
{
    /// <summary>
    /// Run the schema gate. schemasDir overrides where the schemas are found. minReal is the least
    /// number of builds with real object data that passes, 0 when no floor is wanted.
    /// </summary>
    public static CiSchemaResult Execute(string? schemasDir = null, int minReal = 0)
    {
        if (minReal < 0)
            throw new ArgumentOutOfRangeException(nameof(minReal), "minimum real count cannot be negative");

        string dir = GameSchemaCommand.ResolveSchemasDir(schemasDir);
        var loaded = new List<LoadedSchema>();
        var builds = new List<CiSchemaBuildResult>();

        foreach (var sub in Directory.EnumerateDirectories(dir).OrderBy(d => d, StringComparer.Ordinal))
        {
            if (!File.Exists(Path.Combine(sub, "manifest.json"))) continue;
            try
            {
                var schema = GameSchemaCommand.Load(sub, GameSchemaCommand.SchemaPart.Objects);
                loaded.Add(new LoadedSchema(sub, schema.Build, schema));
                builds.Add(new CiSchemaBuildResult(schema.Build, sub, true, null));
            }
            catch (Exception ex)
            {
                builds.Add(new CiSchemaBuildResult(Path.GetFileName(sub), sub, false, ex.Message));
            }
        }

        var coverage = BenchSchemaCoverageCommand.Analyze(loaded);
        var parseFailures = builds.Where(b => !b.Parsed).ToList();

        bool anyBuild = builds.Count > 0;
        bool parsePass = anyBuild && parseFailures.Count == 0;
        bool realPass = coverage.Totals.Real >= minReal;

        var gates = new List<CiGateLine>
        {
            new(
                "schema-parse",
                parsePass,
                anyBuild
                    ? (parsePass
                        ? $"{builds.Count} build(s) parsed"
                        : $"{parseFailures.Count} of {builds.Count} build(s) failed to parse")
                    : "no schema folder found",
                parseFailures.Select(b => b.Build + "  " + b.Error).ToList()),
            new(
                "schema-real",
                realPass,
                $"{coverage.Totals.Real} build(s) with real object data, minimum {minReal}",
                coverage.Rows
                    .Where(r => r.ObjectData == BenchSchemaCoverageCommand.Real)
                    .Select(r => r.Build + "  " + r.Era + "  " + r.Fields + " field(s)")
                    .ToList()),
        };

        bool passed = parsePass && realPass;
        return new CiSchemaResult(passed, passed ? 0 : 1, gates, builds, coverage, dir, minReal);
    }
}
