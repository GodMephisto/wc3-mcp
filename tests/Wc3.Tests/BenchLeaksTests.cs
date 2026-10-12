// tests/Wc3.Tests/BenchLeaksTests.cs
// Hermetic tests for BenchLeaksCommand. A folder of two synthetic maps, one leaky and one clean,
// exercises the Execute sweep without any real map. The per-kind fold is also pinned over an
// in-memory result set via ScriptLeaksCommand.Analyze.
using System.Text;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class BenchLeaksTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-bench-leaks", Guid.NewGuid().ToString("N"));

    public BenchLeaksTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private void Map(string name, string script) =>
        File.WriteAllBytes(Path.Combine(_dir, name), SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));

    [Fact]
    public void Execute_sweeps_a_folder_and_counts_per_map_and_kind()
    {
        Map("leaky.w3x", "function main takes nothing returns nothing\n"
            + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\nendfunction\n");
        Map("clean.w3x", "function main takes nothing returns nothing\n"
            + "    call DestroyEffect(AddSpecialEffect(\"x.mdl\", 0, 0))\nendfunction\n");

        var r = BenchLeaksCommand.Execute(_dir);

        Assert.Equal(2, r.Maps.Count);
        var leaky = r.Maps.Single(m => m.Map == "leaky.w3x");
        Assert.True(leaky.Ok);
        Assert.Equal(1, leaky.Leaks);
        Assert.Equal("clean.w3x", r.Maps.Single(m => m.Leaks == 0).Map);

        Assert.Equal(1, r.TotalLeaks);
        var kind = Assert.Single(r.Kinds);
        Assert.Equal("discarded", kind.Rule);
        Assert.Equal(1, kind.Count);
    }

    [Fact]
    public void Summarize_folds_an_in_memory_result_set()
    {
        var one = ScriptLeaksCommand.Analyze("function Tick takes nothing returns nothing\n"
            + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\nendfunction\n"
            + "function main takes nothing returns nothing\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, function Tick)\nendfunction\n");
        var none = ScriptLeaksCommand.Analyze("function main takes nothing returns nothing\nendfunction\n");

        var r = BenchLeaksCommand.Summarize(new[] { ("a.w3x", one), ("b.w3x", none) });

        Assert.Equal(2, r.Maps.Count);
        Assert.Equal(1, r.TotalLeaks);
        Assert.Equal(1, r.Hot);
        Assert.Equal(0, r.Repeat);
        Assert.Equal("discarded", Assert.Single(r.Kinds).Rule);
    }
}
