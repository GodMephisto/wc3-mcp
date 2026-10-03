// tests/Wc3.Tests/TerrainCornerFieldsTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The string-keyed door onto the per-corner terrain setters. The brush commands cover areas and
/// cannot fix one corner, so this is the other half of terrain editing, and it was previously
/// reachable only from the GUI.
/// </summary>
public class TerrainCornerFieldsTests
{
    private static MapDocument Map() => BlankMap.Create();

    [Fact]
    public void Setting_the_ground_height_takes_effect_and_survives_a_reload()
    {
        var doc = Map();

        var r = TerrainCornerFields.SetField(doc, 4, 4, "GroundHeight", "3.5");
        Assert.True(r.Ok, r.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(3.5f, TerrainEditCommand.GetCorner(reloaded, 4, 4)!.GroundHeight, 3);
    }

    [Fact]
    public void Adding_a_delta_is_relative_to_what_is_there()
    {
        var doc = Map();
        Assert.True(TerrainCornerFields.SetField(doc, 2, 2, "GroundHeight", "1").Ok);

        Assert.True(TerrainCornerFields.SetField(doc, 2, 2, "AddGroundHeight", "2").Ok);

        Assert.Equal(3f, TerrainEditCommand.GetCorner(doc, 2, 2)!.GroundHeight, 3);
    }

    [Fact]
    public void A_negative_delta_lowers_the_corner()
    {
        var doc = Map();
        Assert.True(TerrainCornerFields.SetField(doc, 2, 2, "GroundHeight", "5").Ok);

        Assert.True(TerrainCornerFields.SetField(doc, 2, 2, "AddGroundHeight", "-2").Ok);

        Assert.Equal(3f, TerrainEditCommand.GetCorner(doc, 2, 2)!.GroundHeight, 3);
    }

    [Fact]
    public void Every_advertised_field_is_actually_handled()
    {
        // A field the CLI advertises and the switch does not handle is worse than not offering it.
        var doc = Map();

        foreach (var field in TerrainCornerFields.Fields)
        {
            var r = TerrainCornerFields.SetField(doc, 3, 3, field, "0");
            Assert.DoesNotContain("unknown field", r.Message);
        }
    }

    [Fact]
    public void An_unknown_field_names_the_ones_that_exist()
    {
        var r = TerrainCornerFields.SetField(Map(), 1, 1, "Nonsense", "1");

        Assert.False(r.Ok);
        Assert.Contains("unknown field", r.Message);
        Assert.Contains("GroundHeight", r.Message);
    }

    [Fact]
    public void A_value_that_does_not_parse_says_what_was_expected()
    {
        var r = TerrainCornerFields.SetField(Map(), 1, 1, "CliffLevel", "high");

        Assert.False(r.Ok);
        Assert.Contains("expected", r.Message);
    }

    [Fact]
    public void A_corner_outside_the_grid_is_refused_and_points_at_the_extents()
    {
        var r = TerrainCornerFields.SetField(Map(), 99999, 99999, "GroundHeight", "1");

        Assert.False(r.Ok);
        Assert.Contains("terrain info", r.Message);
    }

    [Fact]
    public void The_grid_extents_are_readable_so_a_caller_can_stay_in_bounds()
    {
        var info = TerrainEditCommand.GetInfo(Map());

        Assert.NotNull(info);
        Assert.True(info!.Width > 0);
        Assert.True(info.Height > 0);
    }

    [Fact]
    public void Editing_one_corner_leaves_its_neighbour_alone()
    {
        // A brush would touch both. This is the point of having a per-corner layer at all.
        var doc = Map();
        var before = TerrainEditCommand.GetCorner(doc, 5, 5)!.GroundHeight;

        Assert.True(TerrainCornerFields.SetField(doc, 4, 4, "GroundHeight", "7").Ok);

        Assert.Equal(before, TerrainEditCommand.GetCorner(doc, 5, 5)!.GroundHeight, 3);
        Assert.Equal(7f, TerrainEditCommand.GetCorner(doc, 4, 4)!.GroundHeight, 3);
    }
}
