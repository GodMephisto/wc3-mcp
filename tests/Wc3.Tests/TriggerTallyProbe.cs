// tests/Wc3.Tests/TriggerTallyProbe.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Does what the trigger reader reports actually tally with what the file holds.
///
/// Reported symptom, the trigger list does not match the map and "lots and lots of stuff"
/// is missing. `trigger read` on Anime_WOS2_0.29d reports 7 categories, 109 triggers and
/// ZERO variables, and zero variables on a 254MB map is not a plausible number when other
/// maps in the same library carry 526 and 816.
///
/// This compares the reader's output against the parsed MapTriggers directly, per item type,
/// so a category that is silently dropped shows up as a gap rather than as a smaller number
/// nobody questions.
/// </summary>
public class TriggerTallyProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerTallyProbe(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> Maps()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) yield break;
        foreach (var p in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length))
            yield return p;
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void What_the_reader_reports_against_what_the_tree_holds()
    {
        int mapsWithGap = 0, mapsSeen = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            MapTriggers? wtg;
            TriggerModel model;
            try
            {
                var doc = MapDocument.Load(path);
                wtg = doc.GetFile("war3map.wtg")?.Model as MapTriggers;
                if (wtg is null) continue;
                model = TriggerReadCommand.GetTriggers(doc);
            }
            catch (Exception ex)
            {
                _out.WriteLine($"{Trim(path),-40} threw {ex.GetType().Name}");
                continue;
            }

            mapsSeen++;

            // Ground truth, straight off the parsed tree.
            int rawItems = wtg.TriggerItems.Count;
            int rawCats = wtg.TriggerItems.OfType<TriggerCategoryDefinition>().Count();
            int rawTrigs = wtg.TriggerItems.OfType<TriggerDefinition>().Count();
            int rawVarItems = wtg.TriggerItems.Count(i => i.GetType().Name.Contains("Variable"));
            int rawVarList = wtg.Variables.Count;

            // What the reader hands a front-end.
            int gotCats = model.Categories.Count;
            int gotTrigs = model.Triggers.Count;
            int gotVars = model.Variables.Count;

            bool gap = gotCats != rawCats || gotTrigs != rawTrigs
                    || gotVars != Math.Max(rawVarList, rawVarItems);
            if (gap) mapsWithGap++;

            _out.WriteLine(
                $"{Trim(path),-40} sub={(wtg.SubVersion is null ? "no " : "yes")} "
                + $"items={rawItems,-5} "
                + $"cat {rawCats,-4}->{gotCats,-4} "
                + $"trig {rawTrigs,-5}->{gotTrigs,-5} "
                + $"var(list={rawVarList},tree={rawVarItems})->{gotVars,-5} "
                + (gap ? "GAP" : ""));

            // Anything in the tree that is neither a category, trigger nor variable is a type
            // the reader may not even have a bucket for, which is the likeliest way for items
            // to vanish without anybody noticing.
            var others = wtg.TriggerItems
                .Where(i => i is not TriggerCategoryDefinition && i is not TriggerDefinition)
                .GroupBy(i => i.GetType().Name)
                .ToList();
            foreach (var g in others)
                _out.WriteLine($"      unmodelled item type {g.Key} x{g.Count()}");
        }

        _out.WriteLine($"\n{mapsSeen} map(s) with a readable tree, {mapsWithGap} show a gap "
                     + "between the tree and what the reader reports");
        _out.WriteLine(mapsWithGap == 0
            ? "VERDICT: the reader reports everything the tree holds."
            : "VERDICT: the reader is DROPPING items. The columns above say which type and how "
            + "many, and a front-end can only ever show what the reader hands it.");
    }

    private static string Trim(string p)
    {
        string n = Path.GetFileName(p);
        return n.Length <= 38 ? n : n[..38];
    }
}
