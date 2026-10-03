// tests/Wc3.Tests/WctPairingRuleSweep.cs
using War3Net.Build.Script;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// WctPairingProbe found that the two candidate pairing rules each win on a different map, which
/// means the rule is conditional and the reader implements one branch of it unconditionally.
///
///   RATankD_516.2      sub=none   606 slots, 606 definitions, 42 empty == 42 gui   -> rule A
///   Anime_WOS2_0.30d   sub=v4     109 slots, 115 definitions,  0 empty == 109 ct   -> rule B
///   GGGA_V0.04g        sub=v4      79 slots,  79 definitions, all custom text      -> either
///
/// The obvious discriminator is the wtg sub-version. Three maps and one old-format sample is not
/// enough to pin a format rule, and pinning it wrong would mispair code bodies on the other side
/// of the branch instead. So this checks the structural prediction on EVERY readable map, without
/// relying on the name heuristic that scored the earlier probe.
///
/// The prediction is exact and falsifiable:
///   sub-version absent  =>  slots == definitions        (gui definitions hold empty slots)
///   sub-version present =>  slots == custom-text count  (gui definitions hold no slot at all)
/// </summary>
public class WctPairingRuleSweep
{
    private readonly ITestOutputHelper _out;
    public WctPairingRuleSweep(ITestOutputHelper output) => _out = output;

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
    public void Does_the_sub_version_decide_the_pairing_rule()
    {
        int oldOk = 0, oldBad = 0, newOk = 0, newBad = 0, ambiguous = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;
            MapTriggers? wtg;
            MapCustomTextTriggers? wct;
            try
            {
                var doc = MapDocument.Load(path);
                wtg = doc.GetFile("war3map.wtg")?.Model as MapTriggers;
                wct = doc.GetFile("war3map.wct")?.Model as MapCustomTextTriggers;
            }
            catch { continue; }
            if (wtg is null || wct is null) continue;

            var defs = wtg.TriggerItems.OfType<TriggerDefinition>().ToList();
            int ct = defs.Count(d => d.IsCustomTextTrigger || d.Type == TriggerItemType.Script);
            int gui = defs.Count - ct;
            int slots = wct.CustomTextTriggers.Count;
            int empty = wct.CustomTextTriggers
                .Count(t => string.IsNullOrWhiteSpace(t.Code?.TrimEnd('\0')));
            bool hasSub = wtg.SubVersion is not null;

            bool matchesA = slots == defs.Count;
            bool matchesB = slots == ct;
            string verdict = matchesA && matchesB ? "either (gui=0)"
                : matchesA ? "A"
                : matchesB ? "B"
                : "NEITHER";

            if (matchesA && matchesB) ambiguous++;
            else if (hasSub) { if (matchesB) newOk++; else newBad++; }
            else { if (matchesA) oldOk++; else oldBad++; }

            // The wct carries a sub-version of its own. If it ever disagrees with the wtg's,
            // the rule is anchored to the wrong field and this column will say so.
            string wctSub = wct.SubVersion?.ToString() ?? "none";
            string wtgSub = wtg.SubVersion?.ToString() ?? "none";
            if (wctSub != wtgSub) _out.WriteLine($"  !! ANCHOR DISAGREEMENT on {Path.GetFileName(path)}: "
                                               + $"wtg sub={wtgSub}, wct sub={wctSub}");

            _out.WriteLine($"{Path.GetFileName(path),-46} sub={(hasSub ? "v4" : "none"),-5} wctSub={wctSub,-5} "
                         + $"defs={defs.Count,-5} ct={ct,-5} gui={gui,-4} slots={slots,-5} "
                         + $"empty={empty,-4} fits={verdict}");
        }

        _out.WriteLine($"\nsub-version ABSENT : {oldOk} fit rule A, {oldBad} did not");
        _out.WriteLine($"sub-version PRESENT: {newOk} fit rule B, {newBad} did not");
        _out.WriteLine($"ambiguous (no gui triggers, both rules identical): {ambiguous}");

        _out.WriteLine(oldBad == 0 && newBad == 0
            ? "VERDICT: the sub-version decides the rule, with no counter-example. The reader must "
            + "branch on it: count every definition when the sub-version is absent, and count only "
            + "custom-text definitions when it is present."
            : "VERDICT: the sub-version does NOT fully decide the rule. The counter-examples above "
            + "need explaining before the reader is changed.");
    }
}
