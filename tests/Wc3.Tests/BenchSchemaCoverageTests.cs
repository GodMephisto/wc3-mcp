// tests/Wc3.Tests/BenchSchemaCoverageTests.cs
// Hermetic tests for BenchSchemaCoverageCommand. The classification and the totals are pure folds
// over a LoadedSchema list, so they run with objects built in memory. The Execute path is checked
// against temporary schema folders written by GamedataFixtures, never data/schemas.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class BenchSchemaCoverageTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-bench-coverage", Guid.NewGuid().ToString("N"));

    public BenchSchemaCoverageTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private static LoadedSchema Schema(string build, int fieldCount)
    {
        IReadOnlyDictionary<string, IReadOnlyList<SchemaField>>? objects = fieldCount == 0
            ? null
            : new Dictionary<string, IReadOnlyList<SchemaField>>(StringComparer.Ordinal)
            {
                ["units/unitmetadata.slk"] = Enumerable.Range(0, fieldCount)
                    .Select(i => GamedataFixtures.Field("Field" + i, "int", "0", "1", true)).ToList(),
            };
        var schema = new GameSchema(1, build, "test", Array.Empty<SchemaInput>(),
            Array.Empty<string>(), objects, null, null);
        return new LoadedSchema(Path.Combine("dir", build), build, schema);
    }

    [Fact]
    public void Analyze_marks_real_era_derived_and_absent_and_totals_them()
    {
        var real = Schema("1.30.4.11274", 2);          // TFT, carries its own object fields
        var derived = Schema("1.31.1.12173", 0);       // TFT, no own fields, era covered by real
        var absent = Schema("Foo-v1.0", 0);            // unknown era, no real schema anywhere

        var r = BenchSchemaCoverageCommand.Analyze(new[] { real, derived, absent });

        Assert.Equal(3, r.Rows.Count);
        Assert.Equal("real", r.Rows.Single(x => x.Build == "1.30.4.11274").ObjectData);
        Assert.Equal("era-derived", r.Rows.Single(x => x.Build == "1.31.1.12173").ObjectData);
        Assert.Equal("absent", r.Rows.Single(x => x.Build == "Foo-v1.0").ObjectData);

        Assert.Equal(2, r.Rows.Single(x => x.Build == "1.30.4.11274").Fields);
        Assert.Equal(0, r.Rows.Single(x => x.Build == "1.31.1.12173").Fields);

        var t = r.Totals;
        Assert.Equal(3, t.Schemas);
        Assert.Equal(1, t.Real);
        Assert.Equal(1, t.EraDerived);
        Assert.Equal(1, t.Absent);
        Assert.Equal(2, t.Fields);
    }

    [Fact]
    public void Analyze_reports_the_era_label_from_the_build_name()
    {
        var r = BenchSchemaCoverageCommand.Analyze(new[] { Schema("1.30.4.11274", 1) });
        Assert.Equal("TFT", r.Rows[0].Era);
    }

    [Fact]
    public void Execute_reads_temporary_schema_folders()
    {
        GamedataFixtures.Save(_dir, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) });
        GamedataFixtures.Save(_dir, "TFT-v1.31.1.12173", natives: new[] { "N" });

        var r = BenchSchemaCoverageCommand.Execute(_dir);

        Assert.Equal(2, r.Rows.Count);
        Assert.Equal(1, r.Totals.Real);
        Assert.Equal(1, r.Totals.EraDerived);
        Assert.Equal(1, r.Totals.Fields);
    }
}
