// tests/Wc3.Tests/PathingValidateCommandTests.cs
using War3Net.Build.Environment; // MapPathingMap, PathingType, MapPathingMapFormatVersion
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>Hermetic coverage of <see cref="PathingCommand.Validate"/>. The pathing grid is a
/// small custom W x H map so a cell index is easy to reason about, while the blank map's
/// war3map.w3i supplies playable bounds of +/-2048 world units for the start-location check.</summary>
public class PathingValidateCommandTests
{
    private const int W = 8;
    private const int H = 8;

    private static MapDocument BlankWithPathing()
    {
        var doc = BlankMap.Create();
        var map = new MapPathingMap(MapPathingMapFormatVersion.v0)
        {
            Width = W,
            Height = H,
            Cells = Enumerable.Repeat(default(PathingType), W * H).ToList(),
        };
        doc.AddOrReplaceModelFile(PathingCommand.PathingFile, map);
        return doc;
    }

    private static void Block(MapDocument doc, int x, int y) =>
        PathingCommand.Paint(doc, x, y, radius: 0, PathingType.Walk, PathingCommand.BrushOp.Set);

    [Fact]
    public void Validate_is_clean_on_an_all_pathable_grid()
    {
        var doc = BlankWithPathing();
        var r = PathingCommand.Validate(doc);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(W, r.Width);
        Assert.Equal(H, r.Height);
        Assert.Empty(r.Issues);
    }

    [Fact]
    public void Validate_flags_an_enclosed_unpathable_pocket()
    {
        var doc = BlankWithPathing();
        PathingCommand.PaintRect(doc, x0: 3, y0: 3, x1: 4, y1: 4, PathingType.Walk);

        var r = PathingCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues, i => i.Kind == "enclosed-unpathable");
    }

    [Fact]
    public void Validate_does_not_flag_an_unpathable_border_strip()
    {
        var doc = BlankWithPathing();
        // A full left column of unpathable cells touches the border, so it is not enclosed.
        PathingCommand.PaintRect(doc, x0: 0, y0: 0, x1: 0, y1: H - 1, PathingType.Walk);

        var r = PathingCommand.Validate(doc);

        Assert.DoesNotContain(r.Issues, i => i.Kind == "enclosed-unpathable");
    }

    [Fact]
    public void Validate_flags_a_sealed_off_map_corner()
    {
        var doc = BlankWithPathing();
        // Isolate corner (0,0) with a wall so it is walkable but cut off.
        Block(doc, 1, 0);
        Block(doc, 0, 1);
        Block(doc, 1, 1);

        var r = PathingCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues,
            i => i.Kind == "unreachable-corner" && i.Detail.Contains("(0,0)"));
    }

    [Fact]
    public void Validate_flags_an_unpathable_map_corner()
    {
        var doc = BlankWithPathing();
        Block(doc, 0, 0);

        var r = PathingCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues,
            i => i.Kind == "unreachable-corner" && i.Detail.Contains("unpathable"));
    }

    [Fact]
    public void Validate_flags_a_start_location_on_an_unpathable_tile()
    {
        var doc = BlankWithPathing();
        // World (0,0) maps to the centre cell (4,4) of the 8x8 grid over +/-2048 units.
        PlacementCommand.PlaceStartLocation(doc, 0, 0f, 0f);
        Block(doc, 4, 4);

        var r = PathingCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues,
            i => i.Kind == "start-location-unpathable" && i.Detail.Contains("player 0"));
    }

    [Fact]
    public void Validate_is_clean_when_a_start_location_sits_on_pathable_ground()
    {
        var doc = BlankWithPathing();
        PlacementCommand.PlaceStartLocation(doc, 0, 0f, 0f);

        var r = PathingCommand.Validate(doc);

        Assert.True(r.Ok, r.Message);
    }

    [Fact]
    public void Validate_reports_a_missing_pathing_map()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 0x2F, 0x2F },
        }));
        Assert.Null(doc.GetFile(PathingCommand.PathingFile));

        var r = PathingCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Empty(r.Issues);
    }
}
