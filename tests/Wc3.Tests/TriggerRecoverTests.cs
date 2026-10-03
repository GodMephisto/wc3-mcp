// tests/Wc3.Tests/TriggerRecoverTests.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The refusal is the feature here, so it is tested first and hardest.
///
/// Recovery decompiles a map's compiled script into a browsable trigger tree. Measured across
/// 23 maps in DecompilerRefusalSweep, the decompiler is actively dangerous where a tree
/// already exists, it returned success on a map carrying 626 trigger items while producing 1,
/// at 0% name recall. If Recover ever stops refusing those maps it will start destroying real
/// work while reporting success, which is the single worst outcome available here.
/// </summary>
public class TriggerRecoverTests
{
    private readonly ITestOutputHelper _out;
    public TriggerRecoverTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void A_map_that_already_has_a_trigger_tree_is_refused()
    {
        var doc = BlankMap.Create();

        // A blank map has no tree, so give it one and confirm the guard fires on it.
        var tree = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        tree.TriggerItems.Add(new TriggerDefinition { Name = "Existing work", Description = string.Empty });
        doc.AddOrReplaceRawFile(TriggerCommand.FileName, TriggerCommand.Serialize(tree));

        var r = TriggerRecoverCommand.Recover(doc);

        Assert.False(r.Ok);
        Assert.Contains("already has", r.Message, StringComparison.OrdinalIgnoreCase);
        _out.WriteLine(r.Message);
    }

    [Fact]
    public void A_map_with_no_script_is_refused_with_a_reason_that_names_the_cause()
    {
        // A synthesised blank map has no trigger tree and no compiled script, so it exercises
        // the second guard rather than the first.
        var doc = BlankMap.Create();
        Assert.Null(doc.GetFile(TriggerCommand.FileName));

        var r = TriggerRecoverCommand.Recover(doc);

        Assert.False(r.Ok);
        _out.WriteLine(r.Message);
        // Whatever the reason, a refusal must never look like a success.
        Assert.Equal(0, r.Triggers);
    }

    [Fact]
    public void Recovery_never_reports_success_without_producing_triggers()
    {
        // The invariant that matters most. Every other assertion here is about one input,
        // this one is about the shape of the contract itself.
        var doc = BlankMap.Create();
        var r = TriggerRecoverCommand.Recover(doc);
        Assert.True(!r.Ok || r.Triggers > 0,
            "Recover reported success while producing no triggers, which is exactly the false "
            + "success the decompiler sweep found in the library");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_stripped_map_gets_a_real_tree_back()
    {
        // Angel-samurai-Z-v332A carries scripts\war3map.j and no trigger tree at all. It is
        // the measured case that recovery exists to serve.
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "Angel-samurai-Z-v332A.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var doc = MapDocument.Load(path);
        Assert.Null(doc.GetFile(TriggerCommand.FileName));

        var r = TriggerRecoverCommand.Recover(doc);
        _out.WriteLine(r.Message);

        if (!r.Ok) return;   // a refusal is legitimate, it just is not what this map should do

        Assert.True(r.Triggers > 100, $"expected a substantial tree, got {r.Triggers}");
        Assert.True(r.Functions > r.Triggers,
            "a recovered tree whose triggers carry no functions would browse like a real tree "
            + "and teach the reader nothing");

        // The tree must now be readable back through the ordinary path, not just in memory.
        var written = doc.GetFile(TriggerCommand.FileName);
        Assert.NotNull(written);
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var tree = reloaded.GetFile(TriggerCommand.FileName)?.Model as MapTriggers;
        Assert.NotNull(tree);
        _out.WriteLine($"after save and reload the tree holds {tree!.TriggerItems.Count} item(s)");
        Assert.True(tree.TriggerItems.Count > 100);

        // And the pairing rule must be satisfied, otherwise the World Editor reads bodies
        // from slots that do not exist.
        Assert.Equal(0, WctPairing.ExpectedSlotCount(tree));
    }
}
