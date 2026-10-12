// tests/Wc3.Tests/McpToolSmokeTests.Leaks.cs
// Leak workflow tools, driven through the MCP protocol like an AI client. A map with one hot
// periodic leak is built in memory, and two synthetic wc3watch sessions are written to temp
// folders, so every one of the five tools answers for real without a game or a WC3 install.
using System.Globalization;
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeLeakTests : McpSmokeBase
{
    public McpSmokeLeakTests(SmokeFixture f) : base(f) { }

    private string LeakMap()
    {
        var path = NewOut("leakmap");
        var doc = BlankMap.Create();
        FileEditCommand.WriteText(doc, "war3map.j",
            "function HotTick takes nothing returns nothing\n"
            + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, function HotTick)\n"
            + "endfunction\n");
        doc.Save(path);
        return path;
    }

    private string LeakSession(double risePerSecond)
    {
        var folder = Path.Combine(_f.Dir, $"session-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        var sb = new StringBuilder();
        sb.AppendLine("time,focus,minimized,wc3_priv_mb,wc3_handles");
        var start = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
        for (int i = 0; i < 40; i++)
        {
            double handles = 1000 + risePerSecond * i;
            double mem = 200 + 0.1 * i;
            sb.AppendLine($"{start.AddSeconds(i):yyyy-MM-dd HH:mm:ss.fff},1,0,"
                + $"{mem.ToString("0.0", CultureInfo.InvariantCulture)},"
                + $"{handles.ToString("0", CultureInfo.InvariantCulture)}");
        }
        File.WriteAllText(Path.Combine(folder, "samples.csv"), sb.ToString());
        return folder;
    }

    [Fact, Covers("leak_list")]
    public async Task Leak_list_returns_the_hot_target()
    {
        var map = LeakMap();
        var r = await Call("leak_list", Args(("map", map)));
        Assert.True(Prop(r, "ok").GetBoolean());
        var targets = Rows(Prop(r, "targets"));
        Assert.Contains(targets, t => Prop(t, "function").GetString() == "HotTick");
    }

    [Fact, Covers("leak_harness")]
    public async Task Leak_harness_writes_a_valid_jass_file()
    {
        var map = LeakMap();
        var outPath = NewOut("harness", ".j");
        var r = await Call("leak_harness", Args(
            ("map", map), ("lang", "jass"), ("target", "HotTick"), ("out_path", outPath)));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.True(File.Exists(outPath));
        Assert.Contains("call HotTick()", File.ReadAllText(outPath));
    }

    [Fact, Covers("leak_join")]
    public async Task Leak_join_measures_the_synthetic_session()
    {
        var r = await Call("leak_join", Args(("map", LeakMap()), ("session_folder", LeakSession(5.0))));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.True(Prop(r, "handleSlopePerMin").GetDouble() > 0);
    }

    [Fact, Covers("leak_ab")]
    public async Task Leak_ab_reports_the_drop_between_two_sessions()
    {
        var r = await Call("leak_ab", Args(
            ("session_a", LeakSession(5.0)), ("session_b", LeakSession(0.0)), ("map", LeakMap())));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.True(Prop(r, "handlesFell").GetBoolean());
        Assert.Equal("HotTick", Prop(r, "likelyDisabled").GetString());
    }

    [Fact, Covers("leak_benchmark")]
    public async Task Leak_benchmark_reports_measured_counts()
    {
        var r = await Call("leak_benchmark", Args());
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.Equal(8, Prop(r, "truePositives").GetInt32());
        Assert.Equal(0, Prop(r, "falsePositives").GetInt32());
    }
}
