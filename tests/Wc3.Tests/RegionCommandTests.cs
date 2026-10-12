// tests/Wc3.Tests/RegionCommandTests.cs
using Color = System.Drawing.Color;
using War3Net.Build.Common;        // WeatherType
using War3Net.Build.Environment;   // MapRegions, Region
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>Hermetic coverage of <see cref="RegionCommand"/>, its create, to-rect and validate.
/// BlankMap.Create supplies a real war3map.w3i, so the playable bounds are ±2048 world units
/// for its 32-tile default.</summary>
public class RegionCommandTests
{
    // ---- create ----

    [Fact]
    public void Create_adds_region_from_corner_and_size()
    {
        var doc = BlankMap.Create();
        var r = RegionCommand.Create(doc, "Spawn", -256f, -128f, 512f, 256f);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.CreationNumber);

        var region = Assert.Single(PlacementCommand.ListRegions(doc));
        Assert.Equal("Spawn", region.Name);
        Assert.Equal(-256f, region.Left);
        Assert.Equal(-128f, region.Bottom);
        Assert.Equal(256f, region.Right);
        Assert.Equal(128f, region.Top);
    }

    [Fact]
    public void Create_rejects_duplicate_name_case_insensitively()
    {
        var doc = BlankMap.Create();
        Assert.True(RegionCommand.Create(doc, "Spawn", 0f, 0f, 10f, 10f).Ok);
        Assert.False(RegionCommand.Create(doc, "spawn", 0f, 0f, 10f, 10f).Ok);
        Assert.Single(PlacementCommand.ListRegions(doc));
    }

    [Theory]
    [InlineData(0f, 10f)]
    [InlineData(10f, 0f)]
    [InlineData(-5f, 10f)]
    public void Create_rejects_non_positive_size(float w, float h)
    {
        var doc = BlankMap.Create();
        Assert.False(RegionCommand.Create(doc, "Spawn", 0f, 0f, w, h).Ok);
        Assert.Empty(PlacementCommand.ListRegions(doc));
    }

    [Fact]
    public void Create_rejects_blank_name()
    {
        var doc = BlankMap.Create();
        Assert.False(RegionCommand.Create(doc, "  ", 0f, 0f, 10f, 10f).Ok);
    }

    [Fact]
    public void Create_with_weather_sets_a_weather_type()
    {
        var doc = BlankMap.Create();
        Assert.True(RegionCommand.Create(doc, "Rainy", 0f, 0f, 64f, 64f, weather: true).Ok);
        var model = (MapRegions)doc.GetFile(RegionCommand.FileName)!.Model!;
        Assert.NotEqual(WeatherType.None, model.Regions.Single().WeatherType);
    }

    // ---- to-rect ----

    [Fact]
    public void ToRect_returns_the_four_corner_values()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "Spawn", -256f, -128f, 512f, 256f);

        var r = RegionCommand.ToRect(doc, "Spawn");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(-256f, r.Left);
        Assert.Equal(-128f, r.Bottom);
        Assert.Equal(256f, r.Right);
        Assert.Equal(128f, r.Top);
        Assert.Contains("Rect(", r.Jass);
    }

    [Fact]
    public void ToRect_rejects_missing_region()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "Spawn", 0f, 0f, 64f, 64f);
        Assert.False(RegionCommand.ToRect(doc, "Ghost").Ok);
    }

    // ---- validate ----

    [Fact]
    public void Validate_is_clean_on_a_single_well_formed_region()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "Spawn", -256f, -256f, 512f, 512f);

        var r = RegionCommand.Validate(doc);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.RegionCount);
        Assert.Empty(r.Issues);
    }

    [Fact]
    public void Validate_flags_overlapping_regions()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "A", -256f, -256f, 512f, 512f);
        RegionCommand.Create(doc, "B", 0f, 0f, 512f, 512f);

        var r = RegionCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues, i => i.Kind == "overlap");
    }

    [Fact]
    public void Validate_does_not_flag_regions_that_only_share_an_edge()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "A", -256f, -256f, 256f, 256f); // right edge at 0
        RegionCommand.Create(doc, "B", 0f, -256f, 256f, 256f);    // left edge at 0

        var r = RegionCommand.Validate(doc);

        Assert.True(r.Ok, r.Message);
        Assert.DoesNotContain(r.Issues, i => i.Kind == "overlap");
    }

    [Fact]
    public void Validate_flags_zero_area_region()
    {
        var doc = BlankMap.Create();
        RegionCommand.Create(doc, "A", 0f, 0f, 64f, 64f);
        var model = (MapRegions)doc.GetFile(RegionCommand.FileName)!.Model!;
        model.Regions.Add(new Region
        {
            Name = "Flat",
            Left = 0f, Bottom = 0f, Right = 0f, Top = 64f,
            CreationNumber = 99,
            WeatherType = WeatherType.None,
            AmbientSound = string.Empty,
            Color = Color.FromArgb(255, 0, 0, 0),
        });

        var r = RegionCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues, i => i.Kind == "zero-area" && i.Name == "Flat");
    }

    [Fact]
    public void Validate_flags_region_outside_the_playable_bounds()
    {
        var doc = BlankMap.Create();
        // The blank map's playable half-extent is 32 tiles * 64 = 2048 world units.
        RegionCommand.Create(doc, "Far", 2000f, 0f, 200f, 64f); // right edge at 2200

        var r = RegionCommand.Validate(doc);

        Assert.False(r.Ok);
        Assert.Contains(r.Issues, i => i.Kind == "out-of-bounds" && i.Name == "Far");
    }

    [Fact]
    public void Validate_on_a_map_with_no_regions_is_clean()
    {
        var doc = BlankMap.Create();
        var r = RegionCommand.Validate(doc);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.RegionCount);
        Assert.Empty(r.Issues);
    }
}
