// tests/Wc3.Tests/TerrainFillTests.cs
using War3Net.Build.Environment;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The dispatcher that finally gives the eight bulk terrain operations a caller.
///
/// They were all built, seven of the eight were well tested, and not one of them reached a
/// front-end, so filling a region in the Studio meant clicking a brush repeatedly. What these
/// pin is that the dispatcher routes each tool to the RIGHT operation, because a fourteen-way
/// switch is exactly the kind of code where Blight quietly calls Ramp and every test still
/// passes if it only checks that something changed.
/// </summary>
public class TerrainFillTests
{
    private const int TileEdge = 8;
    private const int W = TileEdge + 1;
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument Doc() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
    private static MapEnvironment Env(MapDocument d) => (MapEnvironment)d.GetFile("war3map.w3e")!.Model!;

    [Theory]
    [InlineData(TerrainFillCommand.FillTool.Raise)]
    [InlineData(TerrainFillCommand.FillTool.Lower)]
    [InlineData(TerrainFillCommand.FillTool.SetHeight)]
    [InlineData(TerrainFillCommand.FillTool.Flatten)]
    [InlineData(TerrainFillCommand.FillTool.Paint)]
    [InlineData(TerrainFillCommand.FillTool.CliffRaise)]
    [InlineData(TerrainFillCommand.FillTool.CliffLower)]
    [InlineData(TerrainFillCommand.FillTool.CliffSet)]
    [InlineData(TerrainFillCommand.FillTool.Ramp)]
    [InlineData(TerrainFillCommand.FillTool.RampOff)]
    [InlineData(TerrainFillCommand.FillTool.Water)]
    [InlineData(TerrainFillCommand.FillTool.WaterRemove)]
    [InlineData(TerrainFillCommand.FillTool.Blight)]
    [InlineData(TerrainFillCommand.FillTool.BlightOff)]
    public void Every_tool_is_routed_and_none_falls_through_to_the_unknown_branch(
        TerrainFillCommand.FillTool tool)
    {
        var r = TerrainFillCommand.Fill(Doc(), 1, 1, 3, 3, tool, value: 0);
        Assert.True(r.Ok, $"{tool} failed: {r.Message}");
        Assert.DoesNotContain("unknown fill tool", r.Message);
    }

    [Fact]
    public void Water_sets_the_water_flag_and_blight_sets_the_blight_flag()
    {
        // The switch is wide enough that a mis-wired arm would go unnoticed by a test that only
        // asserted "something changed", so each of these checks the flag it names.
        var wet = Doc();
        TerrainFillCommand.Fill(wet, 2, 2, 4, 4, TerrainFillCommand.FillTool.Water, 1.5f);
        Assert.True(Env(wet).TerrainTiles[Idx(3, 3)].IsWater);
        Assert.False(Env(wet).TerrainTiles[Idx(3, 3)].IsBlighted);

        var sick = Doc();
        TerrainFillCommand.Fill(sick, 2, 2, 4, 4, TerrainFillCommand.FillTool.Blight);
        Assert.True(Env(sick).TerrainTiles[Idx(3, 3)].IsBlighted);
        Assert.False(Env(sick).TerrainTiles[Idx(3, 3)].IsWater);

        var ramped = Doc();
        TerrainFillCommand.Fill(ramped, 2, 2, 4, 4, TerrainFillCommand.FillTool.Ramp);
        Assert.True(Env(ramped).TerrainTiles[Idx(3, 3)].IsRamp);
        Assert.False(Env(ramped).TerrainTiles[Idx(3, 3)].IsWater);
    }

    [Fact]
    public void The_off_switches_undo_their_on_counterparts()
    {
        var doc = Doc();
        TerrainFillCommand.Fill(doc, 0, 0, 5, 5, TerrainFillCommand.FillTool.Blight);
        Assert.True(Env(doc).TerrainTiles[Idx(2, 2)].IsBlighted);
        TerrainFillCommand.Fill(doc, 0, 0, 5, 5, TerrainFillCommand.FillTool.BlightOff);
        Assert.False(Env(doc).TerrainTiles[Idx(2, 2)].IsBlighted);

        TerrainFillCommand.Fill(doc, 0, 0, 5, 5, TerrainFillCommand.FillTool.Water, 1f);
        Assert.True(Env(doc).TerrainTiles[Idx(2, 2)].IsWater);
        TerrainFillCommand.Fill(doc, 0, 0, 5, 5, TerrainFillCommand.FillTool.WaterRemove);
        Assert.False(Env(doc).TerrainTiles[Idx(2, 2)].IsWater);
    }

    [Fact]
    public void SetHeight_puts_the_requested_height_on_every_corner_in_the_rectangle()
    {
        var doc = Doc();
        var r = TerrainFillCommand.Fill(doc, 2, 2, 5, 5, TerrainFillCommand.FillTool.SetHeight, 3f);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(16, r.TilesChanged);   // 4x4 inclusive

        var env = Env(doc);
        float inside = env.TerrainTiles[Idx(3, 3)].Height;
        float outside = env.TerrainTiles[Idx(7, 7)].Height;
        Assert.NotEqual(inside, outside);
    }

    [Fact]
    public void Corners_may_be_given_in_any_order_and_an_off_grid_rectangle_is_clipped()
    {
        var a = Doc();
        var b = Doc();
        var ra = TerrainFillCommand.Fill(a, 1, 1, 4, 4, TerrainFillCommand.FillTool.Blight);
        var rb = TerrainFillCommand.Fill(b, 4, 4, 1, 1, TerrainFillCommand.FillTool.Blight);
        Assert.Equal(ra.TilesChanged, rb.TilesChanged);

        var clipped = TerrainFillCommand.Fill(Doc(), 6, 6, 99, 99, TerrainFillCommand.FillTool.Blight);
        Assert.True(clipped.Ok, clipped.Message);
        Assert.Equal(9, clipped.TilesChanged);   // 6..8 on both axes
    }

    [Fact]
    public void UsesValue_agrees_with_which_tools_actually_read_it()
    {
        // A front-end hides the value field on the strength of this, so if it drifts from the
        // switch the user is asked for a number that does nothing, or denied one that matters.
        Assert.True(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Raise));
        Assert.True(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Paint));
        Assert.True(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Water));
        Assert.True(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.CliffSet));

        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Flatten));
        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Ramp));
        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.RampOff));
        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.WaterRemove));
        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.Blight));
        Assert.False(TerrainFillCommand.UsesValue(TerrainFillCommand.FillTool.BlightOff));
    }
}
