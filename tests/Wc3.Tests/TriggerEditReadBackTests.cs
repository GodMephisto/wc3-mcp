// tests/Wc3.Tests/TriggerEditReadBackTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Trigger edits through a real save and reload.
///
/// Trigger-level authoring already exists, rename plus the enabled, initially-on and run-on-init
/// flags. What was never checked is whether any of it SURVIVES. Every existing trigger test asserts
/// against the in-memory model, and none saves and reloads, so a mutation that changed the object
/// and never reached the archive would pass all of them.
///
/// This is the foundation for ECA authoring, so it gets measured before anything is built on it.
/// </summary>
public class TriggerEditReadBackTests
{
    private readonly ITestOutputHelper _out;
    public TriggerEditReadBackTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    /// <summary>A map with a real GUI trigger tree, which most published maps do not have.</summary>
    private (MapDocument Doc, TriggerModel Model, string Name)? MapWithTriggers()
    {
        if (!Directory.Exists(Dir)) { _out.WriteLine("no maps folder, skipped"); return null; }

        foreach (var path in Directory.EnumerateFiles(Dir, "*.w3x")
                     .OrderBy(p => new FileInfo(p).Length))
        {
            if (new FileInfo(path).Length > 60L * 1024 * 1024) continue;
            MapDocument doc;
            TriggerModel model;
            try
            {
                doc = MapDocument.Load(path);
                model = TriggerReadCommand.GetTriggers(doc);
            }
            catch { continue; }

            if (model.Triggers.Count > 0)
            {
                _out.WriteLine($"using {Path.GetFileName(path)}: {model.Categories.Count} "
                             + $"categor(ies), {model.Triggers.Count} trigger(s), "
                             + $"{model.Variables.Count} variable(s)");
                return (doc, model, Path.GetFileName(path));
            }
        }
        _out.WriteLine("no map in the library has a GUI trigger tree, skipped");
        return null;
    }

    private static MapDocument Reload(MapDocument doc) => MapDocument.Load(doc.SaveToBytes());

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_renamed_trigger_keeps_its_new_name_through_a_save()
    {
        if (MapWithTriggers() is not { } m) return;

        // A GUI trigger, not a custom-text one, since those have no model writer by design.
        var subject = m.Model.Triggers.FirstOrDefault(t => !t.IsCustomText) ?? m.Model.Triggers[0];
        const string Probe = "wc3ctl renamed probe";

        var result = TriggerCommand.Rename(m.Doc, subject.Id, Probe);
        _out.WriteLine($"rename #{subject.Id} '{subject.Name}' -> '{Probe}': "
                     + $"ok={result.Ok} {result.Message}");
        Assert.True(result.Ok, result.Message);

        var back = TriggerReadCommand.GetTriggers(Reload(m.Doc));
        var found = back.Triggers.FirstOrDefault(t => t.Id == subject.Id);
        Assert.NotNull(found);
        Assert.Equal(Probe, found!.Name);

        // And the tree is otherwise intact, since a rewrite that dropped triggers would also
        // report the new name correctly.
        Assert.Equal(m.Model.Triggers.Count, back.Triggers.Count);
        Assert.Equal(m.Model.Categories.Count, back.Categories.Count);
        Assert.Equal(m.Model.Variables.Count, back.Variables.Count);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("Category", "Corpus")]
    public void An_enabled_flag_survives_a_save(bool enabled)
    {
        if (MapWithTriggers() is not { } m) return;
        var subject = m.Model.Triggers.FirstOrDefault(t => !t.IsCustomText) ?? m.Model.Triggers[0];

        var result = TriggerCommand.SetEnabled(m.Doc, subject.Id, enabled);
        Assert.True(result.Ok, result.Message);

        var back = TriggerReadCommand.GetTriggers(Reload(m.Doc))
            .Triggers.Single(t => t.Id == subject.Id);
        _out.WriteLine($"#{subject.Id} enabled -> {enabled}, read back {back.Enabled}");
        Assert.Equal(enabled, back.Enabled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("Category", "Corpus")]
    public void An_initially_on_flag_survives_a_save(bool on)
    {
        if (MapWithTriggers() is not { } m) return;
        var subject = m.Model.Triggers.FirstOrDefault(t => !t.IsCustomText) ?? m.Model.Triggers[0];

        Assert.True(TriggerCommand.SetInitiallyOn(m.Doc, subject.Id, on).Ok);

        var back = TriggerReadCommand.GetTriggers(Reload(m.Doc))
            .Triggers.Single(t => t.Id == subject.Id);
        _out.WriteLine($"#{subject.Id} initiallyOn -> {on}, read back {back.InitiallyOn}");
        Assert.Equal(on, back.InitiallyOn);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_trigger_edit_touches_only_the_trigger_file()
    {
        if (MapWithTriggers() is not { } m) return;
        var subject = m.Model.Triggers.FirstOrDefault(t => !t.IsCustomText) ?? m.Model.Triggers[0];

        var before = MapDocument.Load(Path.Combine(Dir, m.Name));
        Assert.True(TriggerCommand.Rename(m.Doc, subject.Id, "wc3ctl fidelity probe").Ok);
        var after = Reload(m.Doc);

        var changed = DiffCommand.Execute(before, after).Entries
            .Where(e => !RoundtripCommand.MpqSpecialFiles.Contains(e.Name))
            .Select(e => $"{e.Name}:{e.Change}")
            .ToList();
        _out.WriteLine($"changed: {string.Join(", ", changed)}");

        Assert.Equal(new[] { "war3map.wtg:modified" }, changed);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void An_untouched_trigger_tree_still_writes_byte_identically()
    {
        // The control. If a plain load-and-save already perturbs the tree, none of the above says
        // anything about the edits.
        if (MapWithTriggers() is not { } m) return;

        var before = MapDocument.Load(Path.Combine(Dir, m.Name));
        var result = RoundtripCommand.Execute(before);
        _out.WriteLine($"{m.Name} round-trip faithful={result.Faithful}, "
                     + $"{result.Mismatches.Count} mismatch(es)");
        foreach (var x in result.Mismatches.Take(4)) _out.WriteLine($"   {x}");
        Assert.True(result.Faithful, string.Join(", ", result.Mismatches.Take(4)));
    }
}
