// tests/Wc3.Tests/TriggerItemCountsProbe.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Settles whether MapTriggers.TriggerItemCounts is bookkeeping the writer recomputes, or a
/// header the writer emits as stored.
///
/// It matters because adding or removing a trigger item changes the true counts. If the writer
/// emits the stored dictionary, an add that leaves it alone writes a header that disagrees with
/// the body, and nothing in this repo would notice, because our own reader walks the item list
/// rather than trusting the header. The corruption would surface only in the World Editor, which
/// is the worst place to find it.
///
/// The experiment is sharp rather than inferential. Serialize an untouched model. Then corrupt
/// the dictionary deliberately, by a large obvious amount, and serialize again. Identical bytes
/// prove the writer ignores the dictionary. Different bytes prove it must be maintained, and the
/// diff offset says where the counts live.
/// </summary>
public class TriggerItemCountsProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerItemCountsProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Does_the_writer_emit_the_stored_item_counts()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) { _out.WriteLine("corpus map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        if (doc.GetFile("war3map.wtg")?.Model is not MapTriggers wtg)
        { _out.WriteLine("no parsed wtg, skipped"); return; }

        _out.WriteLine($"format {wtg.FormatVersion}, subVersion "
                     + $"{wtg.SubVersion?.ToString() ?? "(none)"}, "
                     + $"{wtg.TriggerItems.Count} item(s)");

        _out.WriteLine("stored TriggerItemCounts:");
        foreach (var kv in wtg.TriggerItemCounts.OrderBy(k => (int)k.Key))
            _out.WriteLine($"  {kv.Key,-14} {kv.Value}");

        _out.WriteLine("actual tally from TriggerItems:");
        foreach (var g in wtg.TriggerItems.GroupBy(i => i.Type).OrderBy(g => (int)g.Key))
            _out.WriteLine($"  {g.Key,-14} {g.Count()}");

        byte[] asRead = TriggerCommand.Serialize(wtg);

        // Deliberate, obvious corruption. If the writer reads the dictionary at all, 9999 will
        // show up in the bytes.
        var original = new Dictionary<TriggerItemType, int>(wtg.TriggerItemCounts);
        wtg.TriggerItemCounts[TriggerItemType.Category] = 9999;
        byte[] corrupted = TriggerCommand.Serialize(wtg);
        wtg.TriggerItemCounts.Clear();
        foreach (var kv in original) wtg.TriggerItemCounts[kv.Key] = kv.Value;

        int firstDiff = -1;
        for (int i = 0; i < Math.Min(asRead.Length, corrupted.Length); i++)
            if (asRead[i] != corrupted[i]) { firstDiff = i; break; }

        _out.WriteLine($"as-read {asRead.Length:N0} bytes, "
                     + $"after corrupting the dictionary {corrupted.Length:N0} bytes, "
                     + $"first differing byte {(firstDiff < 0 ? "none" : firstDiff.ToString("N0"))}");

        bool ignoresIt = asRead.Length == corrupted.Length && firstDiff < 0;
        _out.WriteLine(ignoresIt
            ? "VERDICT: the writer IGNORES the stored counts, so add and remove need not maintain "
            + "them. Maintaining them anyway is still correct and costs nothing."
            : "VERDICT: the writer EMITS the stored counts. Add and remove MUST update the "
            + "dictionary or they write a header that disagrees with the body.");
    }
}
