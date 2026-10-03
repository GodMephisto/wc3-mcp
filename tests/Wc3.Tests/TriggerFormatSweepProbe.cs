// tests/Wc3.Tests/TriggerFormatSweepProbe.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// TriggerItemCountsProbe answered its question on ONE map, and that map turned out to be
/// war3map.wtg format v7 with no sub-version and an EMPTY count dictionary. Proving the writer
/// ignores a dictionary that was empty proves very little. The counts almost certainly belong to
/// the newer sub-version format, which the read path already has a comment about (it mirrors
/// variables into the item tree).
///
/// So this sweeps the whole map library for wtg format versions, finds a map whose count
/// dictionary is actually populated, and reruns the corruption experiment there. Without this,
/// add and remove would ship correct for old maps and silently wrong for new ones, and the
/// Reforged World Editor writes the new format.
/// </summary>
public class TriggerFormatSweepProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerFormatSweepProbe(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> Maps()
    {
        string[] folders =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps", "Download"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps"),
        };
        foreach (var f in folders)
        {
            if (!Directory.Exists(f)) continue;
            foreach (var p in Directory.EnumerateFiles(f, "*.w3?", SearchOption.TopDirectoryOnly)
                         .OrderBy(p => new FileInfo(p).Length))
                yield return p;
        }
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Which_wtg_formats_exist_and_do_any_carry_populated_counts()
    {
        var populated = new List<(string Path, MapTriggers Wtg)>();
        int seen = 0, withWtg = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;
            seen++;
            MapTriggers? wtg = null;
            string note = "";
            try
            {
                var doc = MapDocument.Load(path);
                wtg = doc.GetFile("war3map.wtg")?.Model as MapTriggers;
            }
            catch (Exception ex) { note = $"load failed: {ex.GetType().Name}"; }

            if (wtg is null)
            {
                _out.WriteLine($"{Trim(path),-46} {"-",-6} {note}");
                continue;
            }
            withWtg++;

            int counts = wtg.TriggerItemCounts.Count;
            _out.WriteLine($"{Trim(path),-46} {wtg.FormatVersion,-6} "
                         + $"sub={wtg.SubVersion?.ToString() ?? "none",-6} "
                         + $"items={wtg.TriggerItems.Count,-5} "
                         + $"countsDict={counts}");

            if (counts > 0) populated.Add((path, wtg));
        }

        _out.WriteLine($"\n{seen} map(s) examined, {withWtg} with a parsed wtg, "
                     + $"{populated.Count} with a POPULATED count dictionary");

        if (populated.Count == 0)
        {
            _out.WriteLine("VERDICT: no map in this library carries populated counts, so the "
                         + "dictionary stays empty on every real input here and the earlier "
                         + "verdict holds for everything reachable. Add and remove will maintain "
                         + "it anyway, so a future sub-version map cannot be broken by omission.");
            return;
        }

        // The counts are emitted, so the next question is what they COUNT. If the dictionary is
        // a straight per-type tally of TriggerItems, an add or remove can adjust one entry by one
        // and stay correct. If it counts something else, adjusting it would push the header
        // further from the truth on every map here.
        _out.WriteLine("\ninvariant check, stored count vs actual tally per type:");
        int agree = 0, disagree = 0;
        foreach (var (p, w) in populated)
        {
            var tally = w.TriggerItems.GroupBy(i => i.Type)
                         .ToDictionary(g => g.Key, g => g.Count());
            var mismatches = new List<string>();
            foreach (var kv in w.TriggerItemCounts)
            {
                tally.TryGetValue(kv.Key, out int actual);
                if (kv.Value != actual) mismatches.Add($"{kv.Key} stored={kv.Value} actual={actual}");
            }
            foreach (var kv in tally)
                if (!w.TriggerItemCounts.ContainsKey(kv.Key))
                    mismatches.Add($"{kv.Key} stored=(absent) actual={kv.Value}");

            if (mismatches.Count == 0) { agree++; _out.WriteLine($"  {Trim(p),-46} agrees"); }
            else
            {
                disagree++;
                _out.WriteLine($"  {Trim(p),-46} {mismatches.Count} mismatch(es): "
                             + string.Join("; ", mismatches.Take(4)));
            }
            _out.WriteLine($"      Variables list = {w.Variables.Count}, "
                         + $"tree Variable items = "
                         + $"{(tally.TryGetValue(TriggerItemType.Variable, out int tv) ? tv : 0)}");
        }
        _out.WriteLine($"  => {agree} map(s) agree, {disagree} disagree");
        _out.WriteLine(disagree == 0
            ? "  INVARIANT HOLDS: the dictionary is a per-type tally of TriggerItems, so an add "
            + "or remove adjusts exactly one entry by one."
            : "  INVARIANT DOES NOT HOLD: the dictionary counts something other than a plain "
            + "per-type tally, so the add and remove rule needs deriving from the mismatches.");

        // Rerun the sharp experiment where the dictionary actually has contents.
        var (p2, wtg2) = populated[0];
        _out.WriteLine($"\nrerunning the corruption experiment on {Trim(p2)}");
        foreach (var kv in wtg2.TriggerItemCounts.OrderBy(k => (int)k.Key))
            _out.WriteLine($"  {kv.Key,-14} {kv.Value}");

        byte[] asRead = TriggerCommand.Serialize(wtg2);
        var original = new Dictionary<TriggerItemType, int>(wtg2.TriggerItemCounts);
        var firstKey = original.Keys.First();
        wtg2.TriggerItemCounts[firstKey] = 9999;
        byte[] corrupted = TriggerCommand.Serialize(wtg2);
        wtg2.TriggerItemCounts.Clear();
        foreach (var kv in original) wtg2.TriggerItemCounts[kv.Key] = kv.Value;

        int firstDiff = -1;
        for (int i = 0; i < Math.Min(asRead.Length, corrupted.Length); i++)
            if (asRead[i] != corrupted[i]) { firstDiff = i; break; }

        _out.WriteLine($"corrupted {firstKey} to 9999: {asRead.Length:N0} -> {corrupted.Length:N0} "
                     + $"bytes, first differing byte "
                     + $"{(firstDiff < 0 ? "none" : firstDiff.ToString("N0"))}");
        _out.WriteLine(firstDiff < 0 && asRead.Length == corrupted.Length
            ? "VERDICT: the writer ignores the counts even when they are populated."
            : "VERDICT: the writer EMITS the counts. Add and remove MUST maintain the dictionary.");
    }

    private static string Trim(string p)
    {
        string n = Path.GetFileName(p);
        return n.Length <= 44 ? n : n.Substring(0, 44);
    }
}
