// tests/Wc3.Tests/TriggerDataLoadCostProbe.cs
using System.Diagnostics;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// What wiring TriggerData into the wtg parser costs at load time.
///
/// The parser now prefers ReadMapTriggers(reader, TriggerData.Default) so that a trigger holding
/// any function can be read at all. TriggerData.Default is the stock World-Editor function table,
/// 38 events, 32 conditions, 677 actions and 571 calls, and something has to build it. Load
/// performance is a stated goal on this project, so a change that quietly added a second to every
/// map open would be a bad trade for a capability most maps do not exercise.
///
/// This separates the one-off cost of materialising the table from the per-map cost of using it.
/// </summary>
public class TriggerDataLoadCostProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerDataLoadCostProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void How_expensive_is_the_stock_function_table()
    {
        // First touch pays for building it. Everything after is a field read.
        var cold = Stopwatch.StartNew();
        var table = TriggerData.Default;
        cold.Stop();

        var warm = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++) _ = TriggerData.Default.TriggerActions.Count;
        warm.Stop();

        _out.WriteLine($"first access {cold.ElapsedMilliseconds}ms "
                     + $"({table.TriggerEvents.Count} events, {table.TriggerConditions.Count} "
                     + $"conditions, {table.TriggerActions.Count} actions, "
                     + $"{table.TriggerCalls.Count} calls)");
        _out.WriteLine($"1000 further accesses {warm.ElapsedMilliseconds}ms");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void What_does_the_table_add_to_parsing_a_real_wtg()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) { _out.WriteLine("library absent, skipped"); return; }

        _ = TriggerData.Default;   // pay the one-off cost before timing anything

        _out.WriteLine($"{"map",-44} {"bytes",10} {"plain",9} {"withTable",11}");
        _out.WriteLine(new string('-', 78));

        long totalPlain = 0, totalTable = 0;
        int measured = 0;

        foreach (var path in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length))
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            byte[] raw;
            try
            {
                var doc = MapDocument.Load(path);
                var entry = doc.GetFile("war3map.wtg");
                if (entry is null || entry.RawSize == 0) continue;
                raw = entry.RawBytes;
            }
            catch { continue; }

            long plain = TimeParse(raw, useTable: false);
            long withTable = TimeParse(raw, useTable: true);
            if (plain < 0 && withTable < 0) continue;

            measured++;
            if (plain >= 0) totalPlain += plain;
            if (withTable >= 0) totalTable += withTable;

            _out.WriteLine($"{Path.GetFileName(path),-44} {raw.Length,10:N0} "
                         + $"{(plain < 0 ? "threw" : plain + "us"),9} "
                         + $"{(withTable < 0 ? "threw" : withTable + "us"),11}");
        }

        _out.WriteLine($"\n{measured} wtg file(s): plain {totalPlain / 1000.0:F1}ms total, "
                     + $"with the table {totalTable / 1000.0:F1}ms total, "
                     + $"difference {(totalTable - totalPlain) / 1000.0:+#0.0;-#0.0;0}ms across the "
                     + "whole library");
    }

    /// <summary>Microseconds for one parse, or -1 when it throws.</summary>
    private static long TimeParse(byte[] raw, bool useTable)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            using var ms = new MemoryStream(raw);
            using var r = new BinaryReader(ms);
            _ = useTable ? r.ReadMapTriggers(TriggerData.Default) : r.ReadMapTriggers();
            sw.Stop();
            return sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
        }
        catch
        {
            return -1;
        }
    }
}
