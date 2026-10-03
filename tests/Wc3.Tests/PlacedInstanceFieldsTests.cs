// tests/Wc3.Tests/PlacedInstanceFieldsTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The string-keyed door onto the placed-instance editors, which is what lets the CLI and the MCP
/// reach them at all. The typed setters have their own tests; these cover the parsing, the
/// single-axis behaviour and the failure messages, because a front end passes strings and a wrong
/// parse here silently writes a wrong coordinate.
/// </summary>
public class PlacedInstanceFieldsTests
{
    private static (MapDocument Doc, int Cn) UnitFixture()
    {
        var doc = BlankMap.Create();
        var r = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 100f, y: 200f, rotation: 0f);
        Assert.True(r.Ok, r.Message);
        return (doc, r.CreationNumber);
    }

    private static (MapDocument Doc, int Cn) DoodadFixture()
    {
        var doc = BlankMap.Create();
        var r = PlacementCommand.PlaceDoodad(doc, "ATtr", x: 64f, y: 96f);
        Assert.True(r.Ok, r.Message);
        return (doc, r.CreationNumber);
    }

    [Fact]
    public void Setting_one_axis_leaves_the_other_alone()
    {
        // The setter takes both coordinates, so a field-keyed X edit has to read Y back or it
        // silently teleports the unit to y=0.
        var (doc, cn) = UnitFixture();

        Assert.True(PlacedInstanceFields.SetUnitField(doc, cn, "X", "500").Ok);

        var u = UnitInstanceCommand.Get(doc, cn)!;
        Assert.Equal(500f, u.X);
        Assert.Equal(200f, u.Y);
    }

    [Fact]
    public void A_position_tuple_sets_both()
    {
        var (doc, cn) = UnitFixture();

        Assert.True(PlacedInstanceFields.SetUnitField(doc, cn, "Position", "10, 20").Ok);

        var u = UnitInstanceCommand.Get(doc, cn)!;
        Assert.Equal(10f, u.X);
        Assert.Equal(20f, u.Y);
    }

    [Fact]
    public void One_number_scales_uniformly()
    {
        var (doc, cn) = UnitFixture();

        Assert.True(PlacedInstanceFields.SetUnitField(doc, cn, "Scale", "2").Ok);

        var s = UnitInstanceCommand.Get(doc, cn)!.Scale;
        Assert.Equal(2f, s.Sx);
        Assert.Equal(2f, s.Sy);
        Assert.Equal(2f, s.Sz);
    }

    [Theory]
    [InlineData("Owner", "3")]
    [InlineData("HeroLevel", "5")]
    [InlineData("Strength", "12")]
    [InlineData("Gold", "700")]
    [InlineData("HpPercent", "50")]
    [InlineData("Facing", "1.5")]
    [InlineData("TargetAcquisition", "600")]
    public void Every_advertised_unit_field_is_settable(string field, string value)
    {
        var (doc, cn) = UnitFixture();

        var r = PlacedInstanceFields.SetUnitField(doc, cn, field, value);

        Assert.True(r.Ok, $"{field} is advertised but did not accept '{value}': {r.Message}");
    }

    [Fact]
    public void Every_advertised_unit_field_is_actually_handled()
    {
        // A field in the list that the switch does not handle would be advertised by the CLI and
        // then refused at runtime, which is worse than not offering it.
        var (doc, cn) = UnitFixture();

        foreach (var field in PlacedInstanceFields.UnitFields)
        {
            var r = PlacedInstanceFields.SetUnitField(doc, cn, field, "1");
            Assert.DoesNotContain("unknown field", r.Message);
        }
    }

    [Fact]
    public void Every_advertised_doodad_field_is_actually_handled()
    {
        var (doc, cn) = DoodadFixture();

        foreach (var field in PlacedInstanceFields.DoodadFields)
        {
            var r = PlacedInstanceFields.SetDoodadField(doc, cn, field, "1");
            Assert.DoesNotContain("unknown field", r.Message);
        }
    }

    [Fact]
    public void An_unknown_field_names_the_ones_that_exist()
    {
        var (doc, cn) = UnitFixture();

        var r = PlacedInstanceFields.SetUnitField(doc, cn, "Nonsense", "1");

        Assert.False(r.Ok);
        Assert.Contains("unknown field", r.Message);
        Assert.Contains("Owner", r.Message);
    }

    [Fact]
    public void A_value_that_does_not_parse_says_what_was_expected()
    {
        var (doc, cn) = UnitFixture();

        var r = PlacedInstanceFields.SetUnitField(doc, cn, "Owner", "not a number");

        Assert.False(r.Ok);
        Assert.Contains("expected", r.Message);
    }

    [Fact]
    public void A_creation_number_that_does_not_exist_is_refused_not_ignored()
    {
        var (doc, _) = UnitFixture();

        var r = PlacedInstanceFields.SetUnitField(doc, 99999, "Owner", "1");

        Assert.False(r.Ok);
        Assert.Contains("99999", r.Message);
    }

    [Fact]
    public void A_doodad_position_accepts_two_or_three_components()
    {
        var (doc, cn) = DoodadFixture();

        Assert.True(PlacedInstanceFields.SetDoodadField(doc, cn, "Position", "1,2").Ok);
        var two = DoodadInstanceCommand.Get(doc, cn)!;
        Assert.Equal(1f, two.X);
        Assert.Equal(2f, two.Y);

        Assert.True(PlacedInstanceFields.SetDoodadField(doc, cn, "Position", "4,5,6").Ok);
        var three = DoodadInstanceCommand.Get(doc, cn)!;
        Assert.Equal(4f, three.X);
        Assert.Equal(6f, three.Z);
    }

    [Fact]
    public void Edits_survive_a_save_and_reload()
    {
        // The field door has to mark the file dirty like the typed setters do, or the edit lives
        // only in memory and the saved map is unchanged.
        var (doc, cn) = UnitFixture();
        Assert.True(PlacedInstanceFields.SetUnitField(doc, cn, "Owner", "4").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Equal(4, UnitInstanceCommand.Get(reloaded, cn)!.OwnerId);
    }
}
