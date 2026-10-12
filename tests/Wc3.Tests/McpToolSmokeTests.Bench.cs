// tests/Wc3.Tests/McpToolSmokeTests.Bench.cs
// The bench MCP tools, exercised through the protocol against temporary schema folders and
// synthetic maps, so the coverage check has a smoke test for each and no test needs data/schemas
// or a real map on disk.
using System.Text;

namespace Wc3.Tests;

public sealed class McpSmokeBenchTests : McpSmokeBase
{
    public McpSmokeBenchTests(SmokeFixture f) : base(f) { }

    /// <summary>A schemas root with one build carrying an object field and a native, and one build carrying only a script.</summary>
    private string BuildSchemas()
    {
        var root = Path.Combine(_f.Dir, "bench-gamedata-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        GamedataFixtures.Save(root, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) },
            natives: new[] { "BenchSmokeNative" });
        GamedataFixtures.Save(root, "TFT-v1.31.1.12173", natives: new[] { "OtherNative" });
        return root;
    }

    /// <summary>A folder holding a leaky map and a clean map.</summary>
    private string BuildMapFolder()
    {
        var folder = Path.Combine(_f.Dir, "bench-maps-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        Write(folder, "leaky.w3x", "function main takes nothing returns nothing\n"
            + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\nendfunction\n");
        Write(folder, "clean.w3x", "function main takes nothing returns nothing\n"
            + "    call DestroyEffect(AddSpecialEffect(\"x.mdl\", 0, 0))\nendfunction\n");
        return folder;
    }

    private static void Write(string folder, string name, string script) =>
        File.WriteAllBytes(Path.Combine(folder, name), SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));

    [Fact, Covers("bench_schema_coverage")]
    public async Task Bench_schema_coverage_reports_the_totals()
    {
        var root = BuildSchemas();
        var r = await Call("bench_schema_coverage", Args(("schemas_dir", root)));
        var rows = Rows(Prop(r, "rows"));
        Assert.Equal(2, rows.Count);
        var totals = Prop(r, "totals");
        Assert.Equal(2, Prop(totals, "schemas").GetInt32());
        Assert.Equal(1, Prop(totals, "real").GetInt32());
        Assert.Equal(1, Prop(totals, "eraDerived").GetInt32());
    }

    [Fact, Covers("bench_compat")]
    public async Task Bench_compat_reports_a_matrix_with_an_aggregate()
    {
        var root = BuildSchemas();
        var folder = BuildMapFolder();
        var caller = Path.Combine(folder, "caller.w3x");
        Write(folder, "caller.w3x", "function main takes nothing returns nothing\n"
            + "    call BenchSmokeNative()\nendfunction\n");

        var r = await Call("bench_compat", Args(
            ("maps", new[] { caller }), ("targets", new[] { "TFT-v1.30.1.10211" }), ("schemas_dir", root)));

        Assert.Single(Rows(Prop(r, "cells")));
        Assert.Equal(1, Prop(Prop(r, "aggregate"), "passed").GetInt32());
    }

    [Fact, Covers("bench_leaks")]
    public async Task Bench_leaks_sweeps_a_folder()
    {
        var r = await Call("bench_leaks", Args(("folder", BuildMapFolder())));
        Assert.Equal(2, Rows(Prop(r, "maps")).Count);
        Assert.Equal(1, Prop(r, "totalLeaks").GetInt32());
    }

    [Fact, Covers("bench_open")]
    public async Task Bench_open_sweeps_a_folder()
    {
        var r = await Call("bench_open", Args(("folder", BuildMapFolder())));
        Assert.Equal(2, Rows(Prop(r, "maps")).Count);
        Assert.NotEqual(0, Prop(r, "totalBytes").GetInt64());
    }

    [Fact, Covers("bench_perf_gate")]
    public async Task Bench_perf_gate_compares_two_files()
    {
        var baseline = Path.Combine(_f.Dir, "bench-baseline-" + Guid.NewGuid().ToString("N") + ".json");
        var run = Path.Combine(_f.Dir, "bench-run-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(baseline, "{ \"metrics\": { \"ttft_ms\": 100.0 } }");
        File.WriteAllText(run, "{ \"metrics\": { \"ttft_ms\": 200.0 } }");

        var r = await Call("bench_perf_gate", Args(
            ("baseline", baseline), ("run", run), ("max_regression_pct", 10.0)));

        Assert.False(Prop(r, "pass").GetBoolean());
        Assert.Equal(1, Prop(r, "regressed").GetInt32());
    }
}
