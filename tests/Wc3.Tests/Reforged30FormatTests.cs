// tests/Wc3.Tests/Reforged30FormatTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Info;
using War3Net.Build.Widget;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Maps saved by the Reforged 3.0 World Editor (w3i format 39, doo format 13), which War3Net 6.0.3
/// refused, and the repairs War3Net 6.1.1's bundled trigger table needs before it loads at all.
/// </summary>
public class Reforged30FormatTests
{
    [Fact]
    public void The_stock_trigger_table_loads_and_carries_the_3_0_calls()
    {
        var table = TriggerDataSource.Default;
        Assert.True(table.TriggerCalls.ContainsKey("UnitItemInEquipmentSlotBJ"));
        Assert.True(table.TriggerActions.ContainsKey("SetSoundFacialAnimationSetFilepathAction"));
    }

    [Fact]
    public void A_key_defined_twice_in_a_section_keeps_its_first_definition()
    {
        var text = TriggerDataSource.FirstDefinitionWins("[TriggerParams]\nA=1,first\nA=1,second\n[Other]\nA=1,third\n");
        Assert.Equal("[TriggerParams]\nA=1,first\n[Other]\nA=1,third\n", text);
    }

    [Fact]
    public void A_property_of_a_function_the_section_never_defines_is_dropped()
    {
        var text = TriggerDataSource.FirstDefinitionWins(
            "[TriggerActions]\nFooAction=0,sound\n_FooAction_Defaults=_\n_Foov_Defaults=_,_\n");
        Assert.Equal("[TriggerActions]\nFooAction=0,sound\n_FooAction_Defaults=_\n", text);
    }

    [Fact]
    public void A_defaults_line_that_lost_its_equals_sign_gets_it_back()
    {
        var text = TriggerDataSource.FirstDefinitionWins("[TriggerActions]\nBar=0,real\n_Bar_Defaults0\n");
        Assert.Equal("[TriggerActions]\nBar=0,real\n_Bar_Defaults=0\n", text);
    }

    [Fact]
    public void A_call_flag_other_than_0_or_1_reads_as_1()
    {
        var text = TriggerDataSource.FirstDefinitionWins("[TriggerCalls]\nGetX=0,2,item,unit\nGetY=0,0,unit\n[TriggerActions]\nZ=0,2\n");
        Assert.Equal("[TriggerCalls]\nGetX=0,1,item,unit\nGetY=0,0,unit\n[TriggerActions]\nZ=0,2\n", text);
    }

    [Fact]
    public void A_format_39_w3i_and_a_format_13_doo_parse_and_round_trip()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        var info = (MapInfo)doc.GetFile("war3map.w3i")!.Model!;
        info.FormatVersion = MapInfoFormatVersion.v39;
        var w3i = MapInfoCommand.Serialize(info);
        doc.AddOrReplaceRawFile("war3map.w3i", w3i);

        var doodads = new MapDoodads(MapWidgetsFormatVersion.v13, MapWidgetsSubVersion.v11, true);
        byte[] doo;
        using (var ms = new MemoryStream())
        {
            using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, true)) w.Write(doodads);
            doo = ms.ToArray();
        }
        doc.AddOrReplaceRawFile("war3map.doo", doo);

        var saved = MapDocument.Load(doc.SaveToBytes());
        Assert.DoesNotContain(saved.Diagnostics, d => d.Message.Contains("Parse failed"));
        var back = Assert.IsType<MapInfo>(saved.GetFile("war3map.w3i")!.Model);
        Assert.Equal(MapInfoFormatVersion.v39, back.FormatVersion);
        Assert.Equal(w3i, MapInfoCommand.Serialize(back));
        Assert.IsType<MapDoodads>(saved.GetFile("war3map.doo")!.Model);
        Assert.Equal(InfoCommand.Execute(MapDocument.Load(BlankMap.CreateArchiveBytes())).Players, InfoCommand.Execute(saved).Players);
    }

    [Fact]
    public void Info_refuses_a_w3i_that_does_not_parse_instead_of_printing_unknown()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        doc.AddOrReplaceRawFile("war3map.w3i", [99, 0, 0, 0, 1, 2, 3]);
        var saved = MapDocument.Load(doc.SaveToBytes());
        var e = Assert.Throws<InvalidOperationException>(() => InfoCommand.Execute(saved));
        Assert.Contains("war3map.w3i could not be parsed", e.Message);
    }
}
