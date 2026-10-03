// tests/Wc3.Tests/TriggerNotCompiledTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Pins the most important thing about GUI trigger authoring, which is what it does NOT do.
///
/// Warcraft III runs war3map.j, the compiled script. war3map.wtg is the World Editor's source for
/// that script, and nothing regenerates one from the other except the World Editor. So a trigger
/// authored by wc3ctl is real, is visible in the World Editor, and does absolutely nothing in game
/// until the map is opened and saved there.
///
/// That is the exact shape of failure this codebase keeps finding and guarding against, something
/// that looks correct and silently never runs. A user who authors a trigger, launches the map and
/// sees nothing happen would have no way to tell this from a bug. So the caveat travels in the
/// result of every edit that creates a trigger, and these tests fail if it stops doing so.
/// </summary>
public class TriggerNotCompiledTests
{
    private readonly ITestOutputHelper _out;
    public TriggerNotCompiledTests(ITestOutputHelper output) => _out = output;

    private const int CategoryId = 0x02000001;
    private const int TriggerId = 0x03000001;

    private static byte[] Bytes(MapTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static MapDocument Map()
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        t.TriggerItems.Add(new TriggerCategoryDefinition
        { Id = CategoryId, Name = "Cats", ParentId = -1 });
        t.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Gui)
        {
            Id = TriggerId, Name = "Existing", Description = string.Empty,
            ParentId = CategoryId, IsEnabled = true, IsInitiallyOn = true,
        });
        foreach (var type in Enum.GetValues<TriggerItemType>())
            t.TriggerItemCounts[type] = t.TriggerItems.Count(i => i.Type == type);

        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.wtg", Bytes(t));
        doc.AddOrReplaceRawFile("war3map.j", System.Text.Encoding.Latin1.GetBytes(
            "function main takes nothing returns nothing\nendfunction\n"));
        return MapDocument.Load(doc.SaveToBytes());
    }

    [Fact]
    public void Adding_a_trigger_says_it_will_not_run_until_the_editor_saves()
    {
        var doc = Map();
        var r = TriggerCommand.AddTrigger(doc, "Fresh", CategoryId);
        Assert.True(r.Ok, r.Message);
        _out.WriteLine(r.Message);
        Assert.Contains("World Editor", r.Message);
        Assert.Contains("war3map.j", r.Message);
    }

    [Fact]
    public void Adding_an_eca_says_the_same_thing()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Event,
            "MapInitializationEvent");
        Assert.True(r.Ok, r.Message);
        _out.WriteLine(r.Message);
        Assert.Contains("World Editor", r.Message);
        Assert.Contains("war3map.j", r.Message);
    }

    [Fact]
    public void And_the_claim_is_true_the_compiled_script_really_is_untouched()
    {
        // The caveat would be worse than useless if it were merely cautious boilerplate. This is
        // the measurement behind it, so that if wc3ctl ever DOES learn to regenerate the script,
        // this test fails and the now-false warning gets removed with it.
        var doc = Map();
        byte[] before = doc.GetFile("war3map.j")!.CurrentBytes.ToArray();

        Assert.True(TriggerCommand.AddTrigger(doc, "Fresh", CategoryId).Ok);
        int freshId = TriggerCommand.List(doc).Single(i => i.Name == "Fresh").Id;
        Assert.True(TriggerCommand.AddFunction(doc, freshId, TriggerFunctionType.Event,
            "MapInitializationEvent").Ok);
        Assert.True(TriggerCommand.AddFunction(doc, freshId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "hello" }).Ok);

        var saved = MapDocument.Load(doc.SaveToBytes());
        byte[] after = saved.GetFile("war3map.j")!.CurrentBytes;

        Assert.Equal(before, after);
        Assert.DoesNotContain("Fresh",
            System.Text.Encoding.Latin1.GetString(after), StringComparison.Ordinal);
        _out.WriteLine($"war3map.j unchanged at {after.Length} bytes, and does not mention the "
                     + "trigger, which is exactly what the warning tells the user");
    }

    [Fact]
    public void Renaming_or_re_flagging_an_existing_trigger_does_not_carry_the_note()
    {
        // The note belongs on edits that CREATE something a user might expect to run. Attaching
        // it to every trigger edit would train people to ignore it.
        var doc = Map();
        var rename = TriggerCommand.Rename(doc, TriggerId, "Renamed");
        Assert.True(rename.Ok);
        Assert.DoesNotContain("war3map.j", rename.Message);

        var flag = TriggerCommand.SetEnabled(doc, TriggerId, false);
        Assert.True(flag.Ok);
        Assert.DoesNotContain("war3map.j", flag.Message);
    }
}
