// tests/Wc3.Tests/TriggerTreeShapeProbe.cs
using War3Net.Build.Script;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Reports how real maps parent their trigger items, so a newly added category or trigger lands
/// where the World Editor expects rather than where I guessed.
///
/// The specific unknown is the RootCategory item that only sub-version maps carry. A new category
/// might belong under it, or at -1, and the two produce different trees. The same question applies
/// to the id space, since an added item needs an id nothing else uses, including the mirrored
/// Variable items that only exist in the sub-version format.
/// </summary>
public class TriggerTreeShapeProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerTreeShapeProbe(ITestOutputHelper output) => _out = output;

    public static IEnumerable<object[]> Candidates()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        foreach (var n in new[] { "Anime_WOS2_0.30d.w3x", "GGGA_V0.04g.w3x", "RATankD_516.2.w3x" })
            yield return new object[] { Path.Combine(dir, n) };
    }

    [Theory]
    [MemberData(nameof(Candidates))]
    [Trait("Category", "Corpus")]
    public void How_are_items_parented_and_which_ids_are_taken(string path)
    {
        if (!File.Exists(path)) { _out.WriteLine($"{Path.GetFileName(path)} absent, skipped"); return; }
        var doc = MapDocument.Load(path);
        if (doc.GetFile("war3map.wtg")?.Model is not MapTriggers wtg)
        { _out.WriteLine("no parsed wtg, skipped"); return; }

        _out.WriteLine($"{Path.GetFileName(path)}: {wtg.FormatVersion}, "
                     + $"sub={wtg.SubVersion?.ToString() ?? "none"}, "
                     + $"{wtg.TriggerItems.Count} item(s), {wtg.Variables.Count} variable(s)");

        var roots = wtg.TriggerItems.Where(i => i.Type == TriggerItemType.RootCategory).ToList();
        _out.WriteLine($"  RootCategory item(s): {roots.Count}"
                     + (roots.Count == 0 ? "" : $" -> id {string.Join(",", roots.Select(r => r.Id))}, "
                       + $"parentId {string.Join(",", roots.Select(r => r.ParentId))}, "
                       + $"name '{roots[0].Name}'"));

        foreach (var g in wtg.TriggerItems.GroupBy(i => i.Type).OrderBy(g => (int)g.Key))
        {
            var parents = g.GroupBy(i => i.ParentId)
                           .OrderByDescending(x => x.Count())
                           .Take(4)
                           .Select(x => $"{x.Key}({x.Count()})");
            _out.WriteLine($"  {g.Key,-14} n={g.Count(),-5} ids {g.Min(i => i.Id)}..{g.Max(i => i.Id)}"
                         + $"  parentId {string.Join(" ", parents)}");
        }

        int maxItemId = wtg.TriggerItems.Count == 0 ? 0 : wtg.TriggerItems.Max(i => i.Id);
        int maxVarId = wtg.Variables.Count == 0 ? 0 : wtg.Variables.Max(v => v.Id);
        var dupes = wtg.TriggerItems.GroupBy(i => i.Id).Where(g => g.Count() > 1).ToList();
        _out.WriteLine($"  max item id {maxItemId}, max variable id {maxVarId}, "
                     + $"duplicate item ids {dupes.Count}");

        // Any id referenced as a parent but carried by no item is a dangling parent, which tells
        // us whether the format tolerates one (and so whether -1 is really "root").
        var ids = wtg.TriggerItems.Select(i => i.Id).ToHashSet();
        var dangling = wtg.TriggerItems.Select(i => i.ParentId)
            .Where(p => p != -1 && !ids.Contains(p)).Distinct().ToList();
        _out.WriteLine($"  parentIds referencing no item: "
                     + (dangling.Count == 0 ? "none" : string.Join(",", dangling.Take(8))));

        // And the wct pairing, since removal safety depends on which slots carry code.
        var wct = doc.GetFile("war3map.wct")?.Model as MapCustomTextTriggers;
        int defs = wtg.TriggerItems.OfType<TriggerDefinition>().Count();
        if (wct is null) _out.WriteLine($"  wct: absent. {defs} definition(s) in the wtg.");
        else
        {
            int nonEmpty = wct.CustomTextTriggers
                .Count(t => !string.IsNullOrWhiteSpace(t.Code?.TrimEnd('\0')));
            _out.WriteLine($"  wct: {wct.CustomTextTriggers.Count} slot(s) for {defs} definition(s), "
                         + $"{nonEmpty} non-empty");
            int lastNonEmpty = -1;
            for (int i = 0; i < wct.CustomTextTriggers.Count; i++)
                if (!string.IsNullOrWhiteSpace(wct.CustomTextTriggers[i].Code?.TrimEnd('\0')))
                    lastNonEmpty = i;
            _out.WriteLine($"  last non-empty slot: {lastNonEmpty} "
                         + "(a definition at or after this ordinal can be removed without "
                         + "shifting any code)");
        }
    }
}
