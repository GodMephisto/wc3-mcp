// tests/Wc3.Tests/LeakWorkflowTests.cs
// Hermetic tests for the leak investigation workflow: the static list, the in-map harness, the join
// to a synthetic wc3watch session, the A/B diff, and the detector benchmark. Everything is built in
// memory or in a temp folder, no game and no WC3 install.
using System.Globalization;
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class LeakWorkflowTests
{
    private const string LeakScript =
        "function HotTick takes nothing returns nothing\n"
        + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\n"
        + "endfunction\n"
        + "function ColdOnce takes nothing returns nothing\n"
        + "    call AddSpecialEffect(\"y.mdl\", 0, 0)\n"
        + "endfunction\n"
        + "function main takes nothing returns nothing\n"
        + "    call TimerStart(CreateTimer(), 0.03, true, function HotTick)\n"
        + "    call ColdOnce()\n"
        + "endfunction\n";

    private static MapDocument MapWith(string script)
    {
        var doc = BlankMap.Create();
        FileEditCommand.WriteText(doc, "war3map.j", script);
        return doc;
    }

    [Fact]
    public void Leak_list_ranks_the_hot_effect_as_a_target()
    {
        var r = LeakListCommand.Execute(MapWith(LeakScript));
        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.Hot);
        var target = Assert.Single(r.Targets);
        Assert.Equal(("HotTick", "effect"), (target.Function, target.Handle));
        Assert.Contains(r.Periodic, p => p.Function == "HotTick" && p.HotLeaks == 1);
    }

    [Fact]
    public void A_periodic_timer_whose_callback_is_a_code_variable_is_joined()
    {
        // Wurst compiles a callback as a code variable, "set GC = function HotTick", then
        // TimerStart(..., true, GC). Matching only the inline "function Name" form dropped it, so a
        // Wurst map produced no periodic driver and the hot-leak join found nothing to attribute.
        string script =
            "function HotTick takes nothing returns nothing\n"
            + "    call AddSpecialEffect(\"x.mdl\", 0, 0)\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing\n"
            + "    set GC = function HotTick\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, GC)\n"
            + "endfunction\n";

        var r = LeakListCommand.Execute(MapWith(script));

        Assert.True(r.Ok, r.Message);
        Assert.Contains(r.Periodic, p => p.Function == "HotTick" && p.HotLeaks == 1);
    }

    [Fact]
    public void Leak_harness_jass_names_the_target_and_is_valid_syntax()
    {
        var list = LeakListCommand.Execute(MapWith(LeakScript));
        string outPath = Path.Combine(Path.GetTempPath(), $"wc3leak-{Guid.NewGuid():N}.j");
        try
        {
            var r = LeakHarnessCommand.Build(list, "jass", "HotTick", outPath);
            Assert.True(r.Ok, r.Message);
            Assert.Equal("jass", r.Language);
            Assert.Contains("call HotTick()", r.Source);
            Assert.Contains("TimerStart", r.Source);
            Assert.Contains("LeakHarnessSample", r.Source);
            Assert.True(File.Exists(outPath));
            Assert.Equal(r.Source, File.ReadAllText(outPath));
            // The harness is a fragment for a map that already has config and main, so it is checked
            // merged into a minimal map skeleton rather than on its own.
            string merged = "function config takes nothing returns nothing\nendfunction\n"
                + "function main takes nothing returns nothing\nendfunction\n" + r.Source;
            Assert.DoesNotContain(JassScriptCheck.Check(merged), i => i.Severity == DiagnosticSeverity.Error);
        }
        finally { File.Delete(outPath); }
    }

    [Fact]
    public void Leak_harness_lua_names_the_target_and_is_valid_syntax()
    {
        var list = LeakListCommand.Execute(MapWith(LeakScript));
        var r = LeakHarnessCommand.Build(list, "lua", "HotTick");
        Assert.True(r.Ok, r.Message);
        Assert.Contains("HotTick()", r.Source);
        Assert.Contains("TimerStart", r.Source);
        Assert.Null(Wc3.Commands.Lua.LuaParser.Check(r.Source));
    }

    [Fact]
    public void Leak_harness_for_an_unknown_target_refuses_and_names_the_available_ones()
    {
        var list = LeakListCommand.Execute(MapWith(LeakScript));
        var r = LeakHarnessCommand.Build(list, "jass", "Nope");
        Assert.False(r.Ok);
        Assert.Contains("Nope", r.Message);
        Assert.Contains("HotTick", r.Message);
    }

    [Fact]
    public void Leak_harness_accepts_a_handle_kind_as_the_target()
    {
        var list = LeakListCommand.Execute(MapWith(LeakScript));
        var r = LeakHarnessCommand.Build(list, "jass", "effect");
        Assert.True(r.Ok, r.Message);
        Assert.Contains("HotTick", r.Functions);
        Assert.Contains("call HotTick()", r.Source);
    }

    [Fact]
    public void Leak_join_measures_a_rising_slope_and_ranks_the_periodic_driver()
    {
        var folder = NewSession(risePerSecond: 5.0);
        try
        {
            var r = LeakJoinCommand.Execute(MapWith(LeakScript), folder);
            Assert.True(r.Ok, r.Message);
            Assert.Equal(40, r.SampleCount);
            Assert.True(r.HandleSlopePerMin > 0, $"slope was {r.HandleSlopePerMin}");
            Assert.NotNull(r.HandleCorrelation);
            Assert.Equal("HotTick", r.Drivers[0].Function);
            Assert.True(r.Drivers[0].PredictedPerMin > 0);
            Assert.NotEmpty(r.Windows);
            Assert.All(r.Windows, w => Assert.True(w.HandlesPerMin > 0));
            // A session with no events.csv still joins, on the samples alone.
            Assert.Null(r.GcProbe);
            Assert.Equal(0, r.EventsJoined);
        }
        finally { Directory.Delete(folder, recursive: true); }
    }

    [Fact]
    public void Leak_join_reads_events_and_names_the_candidate_with_runtime_evidence()
    {
        var folder = NewSessionWithEvents(risePerSecond: 5.0, gcProbeAtSecond: 10,
            logAtSecond: 35, logText: "trigger ran spawn 12 units");
        try
        {
            var r = LeakJoinCommand.Execute(MapWith(LeakScript), folder);
            Assert.True(r.Ok, r.Message);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 10), r.GcProbe);
            Assert.True(r.EventsJoined >= 2, $"events {r.EventsJoined}");
            Assert.Equal("HotTick", r.Drivers[0].Function);
            // The rising window near the game log line carries it, and names the nearest driver.
            var evidenced = Assert.Single(r.Windows, w => w.Near.Contains("trigger ran spawn"));
            Assert.Equal("HotTick", evidenced.Driver);
        }
        finally { Directory.Delete(folder, recursive: true); }
    }

    [Fact]
    public void Leak_join_measures_from_the_gc_probe_not_the_session_start()
    {
        // Flat until the forced collection, then a rise. Measured from the start the slope is
        // diluted by the flat part. Measured from the gc_probe it reflects the rise, 10 handles a
        // second is 600 a minute, the way analyze.py reports it.
        var folder = NewSessionWithEvents(risePerSecond: 0.0, gcProbeAtSecond: 20, riseAfterGc: 10.0);
        try
        {
            var r = LeakJoinCommand.Execute(MapWith(LeakScript), folder);
            Assert.True(r.Ok, r.Message);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 20), r.GcProbe);
            Assert.True(r.HandleSlopePerMin > 500,
                $"slope measured from the baseline was {r.HandleSlopePerMin}, expected about 600");
        }
        finally { Directory.Delete(folder, recursive: true); }
    }

    [Fact]
    public void Leak_join_reports_a_folder_with_no_samples_csv()
    {
        var folder = Path.Combine(Path.GetTempPath(), $"wc3leak-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        try
        {
            var r = LeakJoinCommand.Execute(MapWith(LeakScript), folder);
            Assert.False(r.Ok);
            Assert.Contains("samples.csv", r.Message);
        }
        finally { Directory.Delete(folder, recursive: true); }
    }

    [Fact]
    public void Leak_ab_reports_the_drop_and_names_the_disabled_candidate()
    {
        string a = NewSession(risePerSecond: 5.0);
        string b = NewSession(risePerSecond: 0.0);
        try
        {
            var r = LeakAbCommand.Execute(a, b, MapWith(LeakScript));
            Assert.True(r.Ok, r.Message);
            Assert.True(r.HandlesFell, r.Message);
            Assert.True(r.HandleDropPerMin > 0);
            Assert.Equal("HotTick", r.LikelyDisabled);
        }
        finally { Directory.Delete(a, recursive: true); Directory.Delete(b, recursive: true); }
    }

    [Fact]
    public void Leak_ab_measures_each_session_from_its_own_gc_probe_and_marks_the_change()
    {
        // A leaks after its forced collection, B is flat after its own. The drop is measured
        // between the two baselines, and the change is attributed to the disabled candidate.
        string a = NewSessionWithEvents(risePerSecond: 5.0, gcProbeAtSecond: 10,
            logAtSecond: 30, logText: "spawn");
        string b = NewSessionWithEvents(risePerSecond: 0.0, gcProbeAtSecond: 10);
        try
        {
            var r = LeakAbCommand.Execute(a, b, MapWith(LeakScript));
            Assert.True(r.Ok, r.Message);
            Assert.True(r.HandlesFell, r.Message);
            Assert.Equal("HotTick", r.LikelyDisabled);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 10), r.A.Baseline);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 10), r.B.Baseline);
        }
        finally { Directory.Delete(a, recursive: true); Directory.Delete(b, recursive: true); }
    }

    /// <summary>Writes a wc3watch session with samples.csv and events.csv. Handles rise by
    /// <paramref name="risePerSecond"/> across the session and by a further
    /// <paramref name="riseAfterGc"/> a second from the forced collection on. The gc_probe marks the
    /// baseline and a game log line is placed so a rising window can be tied to it.</summary>
    private static string NewSessionWithEvents(double risePerSecond, int gcProbeAtSecond,
        double riseAfterGc = 0.0, int? logAtSecond = null, string logText = "")
    {
        string folder = Path.Combine(Path.GetTempPath(), $"wc3leak-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        var start = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);

        var samples = new StringBuilder();
        samples.AppendLine("time,focus,minimized,wc3_priv_mb,wc3_handles");
        for (int i = 0; i < 40; i++)
        {
            double handles = 1000 + risePerSecond * i + Math.Max(0, i - gcProbeAtSecond) * riseAfterGc;
            double mem = 200 + 0.1 * i;
            samples.AppendLine($"{start.AddSeconds(i):yyyy-MM-dd HH:mm:ss.fff},1,0,"
                + $"{mem.ToString("0.0", CultureInfo.InvariantCulture)},"
                + $"{handles.ToString("0", CultureInfo.InvariantCulture)}");
        }
        File.WriteAllText(Path.Combine(folder, "samples.csv"), samples.ToString());

        var events = new StringBuilder();
        events.AppendLine("time,kind,detail");
        events.AppendLine($"{start:yyyy-MM-dd HH:mm:ss},game_start,pid 1");
        events.AppendLine($"{start.AddSeconds(gcProbeAtSecond):yyyy-MM-dd HH:mm:ss},gc_probe,operator pressed G");
        if (logAtSecond is { } sec)
            events.AppendLine($"{start.AddSeconds(sec):yyyy-MM-dd HH:mm:ss},gamelog,{logText}");
        File.WriteAllText(Path.Combine(folder, "events.csv"), events.ToString());
        return folder;
    }

    [Fact]
    public void Leak_benchmark_measures_a_true_positive_per_kind_and_no_false_positives()
    {
        var r = LeakBenchmarkCommand.Execute();
        Assert.True(r.Ok, r.Message);
        Assert.Equal(8, r.TruePositives);
        Assert.Equal(0, r.FalsePositives);
        Assert.Equal(0, r.FalseNegatives);
        Assert.Equal(1.0, r.Precision);
        Assert.Equal(1.0, r.Recall);
        Assert.Equal(9, r.Cases.Count);
    }

    /// <summary>Writes a minimal wc3watch samples.csv with a linear handle and memory series.</summary>
    private static string NewSession(double risePerSecond)
    {
        string folder = Path.Combine(Path.GetTempPath(), $"wc3leak-{Guid.NewGuid():N}");
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
}
