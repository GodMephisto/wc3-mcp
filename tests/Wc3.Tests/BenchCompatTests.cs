// tests/Wc3.Tests/BenchCompatTests.cs
// Hermetic tests for BenchCompatCommand. The Execute path runs two synthetic maps against two
// tiny target schemas built in a temp folder, so no test needs data/schemas. The aggregate fold is
// also pinned over an in-memory CompatMatrixResult.
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class BenchCompatTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-bench-compat", Guid.NewGuid().ToString("N"));

    public BenchCompatTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private void Schema(string build, params string[] natives)
    {
        var jass = new JassApi(
            Array.Empty<JassTypeDecl>(),
            natives.Select(n => new JassNativeDecl(n, Array.Empty<JassParameter>(), "boolean", false)).ToArray(),
            Array.Empty<JassGlobalDecl>(),
            Array.Empty<JassFunctionDecl>());
        var schema = new GameSchema(1, build, build,
            Array.Empty<SchemaInput>(), new[] { "script" },
            null, (SchemaTriggers?)null,
            new Dictionary<string, JassApi>(StringComparer.Ordinal) { ["scripts/common.j"] = jass });
        var dir = Path.Combine(_dir, build);
        Directory.CreateDirectory(dir);
        GameSchemaCommand.Save(schema, dir);
    }

    private string Map(string script)
    {
        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));
        return path;
    }

    [Fact]
    public void Execute_reports_a_matrix_and_an_aggregate()
    {
        Schema("TFT-v1.30.1.10211");
        Schema("MyBuild", "BenchNative");

        var caller = Map("function main takes nothing returns nothing\n    call BenchNative()\nendfunction\n");
        var plain = Map("function main takes nothing returns nothing\nendfunction\n");

        var r = BenchCompatCommand.Execute(
            new[] { caller, plain }, new[] { "TFT-v1.30.1.10211", "MyBuild" }, _dir);

        Assert.Equal(2, r.Maps.Count);
        Assert.Equal(2, r.Targets.Count);
        Assert.Equal(4, r.Cells.Count);

        var fail = r.Cells.Single(c => c.Map == Path.GetFileName(caller) && c.Target == "TFT-v1.30.1.10211");
        Assert.False(fail.Pass);
        Assert.Equal(1, fail.Errors);

        Assert.Equal(4, r.Aggregate.Cells);
        Assert.Equal(3, r.Aggregate.Passed);
        Assert.Equal(1, r.Aggregate.Failed);
        Assert.Equal(1, r.Aggregate.Errors);
    }

    [Fact]
    public void Analyze_folds_an_in_memory_matrix()
    {
        var matrix = new CompatMatrixResult(
            new[] { "a.w3x", "b.w3x" }, new[] { "T1", "T2" },
            new[]
            {
                new CompatMatrixCell("a.w3x", "T1", false, 2, 1, 3),
                new CompatMatrixCell("a.w3x", "T2", true, 0, 0, 0),
                new CompatMatrixCell("b.w3x", "T1", false, 1, 0, 1),
                new CompatMatrixCell("b.w3x", "T2", true, 0, 2, 2),
            });

        var r = BenchCompatCommand.Analyze(matrix);

        Assert.Equal(4, r.Aggregate.Cells);
        Assert.Equal(2, r.Aggregate.Passed);
        Assert.Equal(2, r.Aggregate.Failed);
        Assert.Equal(3, r.Aggregate.Errors);
        Assert.Equal(3, r.Aggregate.Warnings);
        Assert.Equal(6, r.Aggregate.Findings);
    }

    [Fact]
    public void Execute_requires_a_map_and_a_target()
    {
        Assert.Throws<ArgumentException>(() =>
            BenchCompatCommand.Execute(Array.Empty<string>(), new[] { "x" }, _dir));
        Assert.Throws<ArgumentException>(() =>
            BenchCompatCommand.Execute(new[] { "y" }, Array.Empty<string>(), _dir));
    }
}
