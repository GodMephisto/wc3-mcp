// tests/Wc3.Tests/TriggerAddFeasibilityProbe.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// A feasibility probe, deliberately not a guard. It answers ONE question that gates ECA authoring
/// and nothing else.
///
/// TriggerCommand's own remarks say "Edits here never add or remove items, so the model's
/// TriggerItemCounts stays consistent with the writer." That sentence is the gate. Adding a trigger,
/// removing one, or adding an event, condition or action all change the item or function count, and
/// nobody has ever checked whether War3Net's writer keeps its counts consistent when they do.
///
/// So this adds one category to a real map's tree, writes it, reads it back, and reports what
/// happens. A pass means add and remove are buildable on the existing writer. A failure names the
/// specific thing that needs fixing first, which is worth more than an estimate.
/// </summary>
public class TriggerAddFeasibilityProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerAddFeasibilityProbe(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Can_the_wtg_writer_survive_an_added_trigger_item()
    {
        var path = Path.Combine(Dir, "Random Farm TD 0.63 Beta.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var entry = doc.GetFile("war3map.wtg");
        if (entry?.Model is not MapTriggers triggers)
        { _out.WriteLine("no parsed MapTriggers, skipped"); return; }

        _out.WriteLine($"before: {triggers.TriggerItems.Count} item(s)");
        _out.WriteLine($"        counts by type: {DescribeCounts(triggers)}");

        int maxId = triggers.TriggerItems.Count == 0
            ? 0
            : triggers.TriggerItems.Max(i => i.Id);

        var added = new TriggerCategoryDefinition
        {
            Id = maxId + 1,
            Name = "wc3ctl probe category",
            IsComment = false,
            ParentId = -1,
        };
        triggers.TriggerItems.Add(added);
        _out.WriteLine($"added a category with id {added.Id}, now "
                     + $"{triggers.TriggerItems.Count} item(s)");

        // Serialize exactly the way TriggerCommand does, so this measures the real write path.
        byte[] bytes;
        try
        {
            bytes = TriggerCommand.Serialize(triggers);
        }
        catch (Exception ex)
        {
            _out.WriteLine($"WRITE FAILED: {ex.GetType().Name}: {ex.Message}");
            _out.WriteLine("=> adding an item needs writer work before ECA authoring can start.");
            return;   // a probe reports, it does not fail the suite
        }
        _out.WriteLine($"wrote {bytes.Length:N0} bytes "
                     + $"(was {entry.CurrentBytes.Length:N0})");

        doc.AddOrReplaceRawFile("war3map.wtg", bytes);

        MapTriggers reread;
        try
        {
            var reloaded = MapDocument.Load(doc.SaveToBytes());
            if (reloaded.GetFile("war3map.wtg")?.Model is not MapTriggers r)
            {
                _out.WriteLine("REREAD FAILED: the written wtg no longer parses as MapTriggers.");
                _out.WriteLine("=> the writer emits something the reader rejects once an item is added.");
                return;
            }
            reread = r;
        }
        catch (Exception ex)
        {
            _out.WriteLine($"RELOAD FAILED: {ex.GetType().Name}: {ex.Message}");
            return;
        }

        _out.WriteLine($"after:  {reread.TriggerItems.Count} item(s)");
        _out.WriteLine($"        counts by type: {DescribeCounts(reread)}");

        var found = reread.TriggerItems.FirstOrDefault(i => i.Id == added.Id);
        _out.WriteLine(found is null
            ? "VERDICT: the item did not survive the round trip."
            : $"VERDICT: the item survived as '{found.Name}'.");

        bool countGrew = reread.TriggerItems.Count == triggers.TriggerItems.Count;
        _out.WriteLine(countGrew
            ? "         item count matches, so the writer tracked the addition."
            : $"         item count is {reread.TriggerItems.Count}, expected "
              + $"{triggers.TriggerItems.Count}, so counts are NOT tracked.");

        // Read the tree through the normal command too, since that is what a front-end sees.
        var model = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));
        _out.WriteLine($"         TriggerReadCommand sees {model.Categories.Count} categor(ies), "
                     + $"{model.Triggers.Count} trigger(s)");
    }

    private static string DescribeCounts(MapTriggers t)
    {
        try
        {
            var counts = t.TriggerItemCounts;
            return counts is null
                ? "(null)"
                : string.Join(", ", counts.Select((c, i) => $"{i}={c}").Where(x => !x.EndsWith("=0")));
        }
        catch (Exception ex)
        {
            return $"(unreadable: {ex.GetType().Name})";
        }
    }
}
