// tests/Wc3.Tests/TriggerBodiesLintTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The trigger-bodies lint check, which exists because war3map.wct pairs script bodies to triggers
/// by position with nothing anchoring them, so a count drift silently attributes every later body
/// to the wrong trigger and produces no symptom at all until a human reads a script.
///
/// A check that never fires is worthless and a check that fires on healthy maps is worse, so this
/// pins both ends: it stays quiet on a well-formed map of each format, it fires on a map whose
/// halves genuinely disagree, and (in the corpus test) it stays quiet across the whole library.
/// </summary>
public class TriggerBodiesLintTests
{
    private readonly ITestOutputHelper _out;
    public TriggerBodiesLintTests(ITestOutputHelper output) => _out = output;

    private static byte[] Bytes(MapTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static byte[] Bytes(MapCustomTextTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static MapDocument Map(bool subVersion, int guiCount, int textCount, int slots)
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7,
            subVersion ? MapTriggersSubVersion.v4 : null);
        t.TriggerItems.Add(new TriggerCategoryDefinition
        { Id = 0x02000001, Name = "Cats", ParentId = -1 });
        for (int i = 0; i < guiCount; i++)
            t.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Gui)
            {
                Id = 0x03000001 + i, Name = $"Gui{i}", Description = string.Empty,
                ParentId = 0x02000001, IsEnabled = true,
            });
        for (int i = 0; i < textCount; i++)
            t.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Script)
            {
                Id = 0x03001001 + i, Name = $"Text{i}", Description = string.Empty,
                ParentId = 0x02000001, IsEnabled = true, IsCustomTextTrigger = true,
            });

        var wct = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, null)
        {
            GlobalCustomScriptComment = string.Empty,
            GlobalCustomScriptCode = new CustomTextTrigger { Code = string.Empty },
        };
        for (int i = 0; i < slots; i++)
            wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = $"// slot {i}" });

        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.wtg", Bytes(t));
        doc.AddOrReplaceRawFile("war3map.wct", Bytes(wct));
        return MapDocument.Load(doc.SaveToBytes());
    }

    private static LintCheck Check(MapDocument doc) =>
        LintCommand.Run(doc).Checks.Single(c => c.Name == "trigger-bodies");

    [Fact]
    public void Quiet_on_a_well_formed_sub_version_map()
    {
        // Sub-version: only custom-text triggers own a slot, so 3 text triggers need 3 slots
        // however many GUI triggers sit among them.
        var check = Check(Map(subVersion: true, guiCount: 4, textCount: 3, slots: 3));
        Assert.Equal(LintSeverity.Ok, check.Severity);
    }

    [Fact]
    public void Quiet_on_a_well_formed_classic_map()
    {
        // No sub-version: every definition owns a slot, so 4 GUI plus 3 text needs 7.
        var check = Check(Map(subVersion: false, guiCount: 4, textCount: 3, slots: 7));
        Assert.Equal(LintSeverity.Ok, check.Severity);
    }

    [Fact]
    public void Fires_when_the_two_halves_disagree_and_names_the_first_trigger_in_doubt()
    {
        var check = Check(Map(subVersion: true, guiCount: 2, textCount: 5, slots: 3));
        Assert.Equal(LintSeverity.Warning, check.Severity);
        Assert.Contains("disagree by 2", check.Summary);
        _out.WriteLine(check.Summary);
        foreach (var d in check.Detail) _out.WriteLine("  " + d);

        // Slots 0..2 are trustworthy, so the first body in doubt belongs to Text3.
        Assert.Contains(check.Detail, d => d.Contains("Text3"));
    }

    [Fact]
    public void Fires_when_applying_the_wrong_branch_of_the_rule()
    {
        // The exact shape the old reader assumed: a sub-version map laid out classic-style, with
        // an empty slot for each GUI trigger. Before the rule was measured this looked correct.
        var check = Check(Map(subVersion: true, guiCount: 4, textCount: 3, slots: 7));
        Assert.Equal(LintSeverity.Warning, check.Severity);
    }

    [Fact]
    public void Quiet_when_the_map_has_no_trigger_files_at_all()
    {
        Assert.Equal(LintSeverity.Ok, Check(BlankMap.Create()).Severity);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Quiet_across_the_whole_map_library()
    {
        // A check that fires on healthy maps is worse than no check. Every readable map in the
        // library must pass, otherwise the rule is wrong rather than the maps.
        string[] folders =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps", "Download"),
        };
        int seen = 0, ok = 0;
        var noisy = new List<string>();

        foreach (var folder in folders.Where(Directory.Exists))
        foreach (var path in Directory.EnumerateFiles(folder, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length))
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;
            LintCheck check;
            try
            {
                var doc = MapDocument.Load(path);
                if (doc.GetFile("war3map.wtg")?.Model is null) continue;
                check = Check(doc);
            }
            catch { continue; }

            seen++;
            if (check.Severity == LintSeverity.Ok) ok++;
            else noisy.Add($"{Path.GetFileName(path)}: {check.Summary}");
        }

        _out.WriteLine($"{ok} of {seen} map(s) with a readable wtg pass the check");
        foreach (var n in noisy) _out.WriteLine("  " + n);
        if (seen == 0) { _out.WriteLine("no maps available, skipped"); return; }
        Assert.Empty(noisy);
    }
}
