// tests/Wc3.Tests/UabiRuntimeRepairTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Moving unit ability lists (uabi) into the script, the fix players report for disconnects on
/// maps with thousands of distinct uabi ids.
/// </summary>
public class UabiRuntimeRepairTests
{
    //   h000  uabi A000,A001        moved
    //   h001  uabi Aloc,A002        A002 moved, Aloc stays in the data
    //   h002  uabi A003             an ability morphs into it, so it stays
    //   h003  uabi in the SKIN      moved, and cleared in the skin layer
    private static byte[] Sample(string nl = "\r\n")
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        w3u.NewUnits.Add(Unit("h000", "A000,A001"));
        w3u.NewUnits.Add(Unit("h001", "Aloc,A002"));
        w3u.NewUnits.Add(Unit("h002", "A003"));
        var skin = new UnitObjectData(ObjectDataFormatVersion.v2);
        skin.NewUnits.Add(Unit("h003", "A004"));

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var morph = new LevelObjectModification { OldId = "Abrf".FromRawcode(), NewId = "A00M".FromRawcode() };
        morph.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "Btm1".FromRawcode(), Type = ObjectDataType.String, Value = "h002" });
        w3a.NewAbilities.Add(morph);

        string script = string.Join(nl, new[]
        {
            "globals",
            "    integer udg_x = 0",
            "endglobals",
            "function InitCustomTriggers takes nothing returns nothing",
            "endfunction",
            "function RunInitializationTriggers takes nothing returns nothing",
            "endfunction",
            "function main takes nothing returns nothing",
            "    local integer i = 0",
            "    call InitCustomTriggers()",
            "    call RunInitializationTriggers(  )",   // the spacing the World Editor writes
            "endfunction",
            "",
        });
        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3mapSkin.w3u"] = Serialize(w => w.Write(skin)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.Latin1.GetBytes(script),
        });
    }

    [Fact]
    public void Moves_the_lists_keeps_morph_targets_and_locust()
    {
        var doc = MapDocument.Load(Sample());
        var r = UabiRuntimeRepairCommand.Execute(doc, apply: true);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(3, r.UnitTypesMoved);
        Assert.Equal(4, r.ReferencesMoved);
        Assert.Equal(6, r.DistinctBefore);
        Assert.Equal(new[] { "A003", "Aloc" }.Length, r.DistinctAfter);
        Assert.Equal("h002", Assert.Single(r.Kept).Rawcode);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal("", Uabi(rebuilt, "h000"));
        Assert.Equal("Aloc", Uabi(rebuilt, "h001"));
        Assert.Equal("A003", Uabi(rebuilt, "h002"));
        Assert.Equal("", Uabi(rebuilt, "h003"));
    }

    [Fact]
    public void Script_gets_the_global_the_init_first_and_the_sweep_before_init_triggers()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(UabiRuntimeRepairCommand.Execute(doc, apply: true).Ok);
        var lines = Script(MapDocument.Load(doc.SaveToBytes())).Split("\r\n");

        int global = Array.FindIndex(lines, l => l.Trim() == "hashtable wc3ctl_uabi = null");
        Assert.Equal(Array.IndexOf(lines, "endglobals") - 1, global);

        int main = Array.IndexOf(lines, "function main takes nothing returns nothing");
        Assert.Equal("    local integer i = 0", lines[main + 1]);
        Assert.Equal("    call wc3ctl_uabi_Init()", lines[main + 2]);
        int sweep = Array.IndexOf(lines, "    call wc3ctl_uabi_Sweep()");
        Assert.Equal("    call RunInitializationTriggers(  )", lines[sweep + 1]);
        Assert.True(Array.FindIndex(lines, l => l.StartsWith("function wc3ctl_uabi_Init")) < main);

        // h000 is 0x68303030 and A001 is 0x41303031, written as the integers JASS compares.
        Assert.Contains($"    call SaveInteger(wc3ctl_uabi, {0x68303030}, 0, 2)", lines);
        Assert.Contains($"    call SaveInteger(wc3ctl_uabi, {0x68303030}, 2, {0x41303031})", lines);
        Assert.DoesNotContain(lines, l => l.Contains($"{0x68303032}"));   // h002 kept
    }

    [Fact]
    public void Bare_CR_script_keeps_its_line_ending()
    {
        var doc = MapDocument.Load(Sample("\r"));
        Assert.True(UabiRuntimeRepairCommand.Execute(doc, apply: true).Ok);
        string text = Script(MapDocument.Load(doc.SaveToBytes()));
        Assert.DoesNotContain("\n", text);
        Assert.Contains("\rcall wc3ctl_uabi_Init()".Replace("\rc", "\r    c"), text);
    }

    [Fact]
    public void A_second_run_refuses_rather_than_adding_twice()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(UabiRuntimeRepairCommand.Execute(doc, apply: true).Ok);
        var again = MapDocument.Load(doc.SaveToBytes());
        // Give a unit a list again, so there IS something to move, and the script already
        // carries the adder. Injecting a second copy would declare every function twice.
        Assert.True(ObjectSetCommand.Execute(again, "h000", "uabi", "A009").Ok);
        var r = UabiRuntimeRepairCommand.Execute(again, apply: true);
        Assert.False(r.Ok);
        Assert.Contains("already carries this repair", r.Message);
        Assert.Equal("A009", Uabi(again, "h000"));
    }

    [Fact]
    public void Only_the_named_ids_move_and_the_rest_of_each_list_stays()
    {
        var doc = MapDocument.Load(Sample());
        var r = UabiRuntimeRepairCommand.Execute(doc, apply: true, only: new[] { "A001", "A004" });
        Assert.True(r.Ok, r.Message);
        Assert.Equal(2, r.UnitTypesMoved);
        Assert.Equal(2, r.ReferencesMoved);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal("A000", Uabi(rebuilt, "h000"));        // A001 moved, A000 kept
        Assert.Equal("Aloc,A002", Uabi(rebuilt, "h001"));   // untouched, nothing named
        Assert.Equal("", Uabi(rebuilt, "h003"));
        var lines = Script(rebuilt).Split("\r\n");
        Assert.Contains($"    call SaveInteger(wc3ctl_uabi, {0x68303030}, 0, 1)", lines);
        Assert.Contains($"    call SaveInteger(wc3ctl_uabi, {0x68303030}, 1, {0x41303031})", lines);
    }

    [Fact]
    public void An_entry_that_is_not_moved_stays_exactly_as_written()
    {
        // GGGA's H08N lists 'A0S4m', five characters, a typo. The first version rebuilt each list
        // from its 4 character ids only and silently deleted it, measured as 175 references gone
        // when 174 were moved.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        w3u.NewUnits.Add(Unit("h000", "AInv,A0S4m,A001"));
        var map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.j"] = Encoding.Latin1.GetBytes("function main takes nothing returns nothing\r\nendfunction\r\n"),
        });
        var doc = MapDocument.Load(map);
        Assert.True(UabiRuntimeRepairCommand.Execute(doc, apply: true, only: new[] { "AInv" }).Ok);
        Assert.Equal("A0S4m,A001", Uabi(MapDocument.Load(doc.SaveToBytes()), "h000"));

        var doc2 = MapDocument.Load(map);
        Assert.True(UabiRuntimeRepairCommand.Execute(doc2, apply: true).Ok);
        Assert.Equal("A0S4m", Uabi(MapDocument.Load(doc2.SaveToBytes()), "h000"));
    }

    [Fact]
    public void Dry_run_changes_nothing()
    {
        var doc = MapDocument.Load(Sample());
        var r = UabiRuntimeRepairCommand.Execute(doc, apply: false);
        Assert.Equal(3, r.UnitTypesMoved);
        Assert.DoesNotContain(doc.Files, f => f.IsDirty);
    }

    [Theory]
    [InlineData("a\r\nb\r\nc", "\r\n")]
    [InlineData("a\rb\rc\r\"x\r\ny\"", "\r")]
    [InlineData("a\nb\nc", "\n")]
    public void Line_ending_is_the_one_the_script_mostly_uses(string s, string expected) =>
        Assert.Equal(expected, UabiRuntimeRepairCommand.LineEnding(s));

    private static string Uabi(MapDocument doc, string unit) =>
        ObjectKinds.ModsToDict(ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit))
            .Single(e => e.Id == unit.FromRawcode()).Mods)["uabi"];

    private static string Script(MapDocument doc) =>
        doc.TryReadFileByName("war3map.j", out var b) ? Encoding.Latin1.GetString(b) : "";

    private static SimpleObjectModification Unit(string id, string uabi)
    {
        var u = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = id.FromRawcode() };
        u.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = uabi });
        return u;
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
