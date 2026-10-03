// tests/Wc3.Tests/HeroDefinitionTests.cs
using System.Text.Json;
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Pins the definition format, which is the project's central artifact: everything else is a verb
/// over it (import / install / package / lint). A silent shape change here would break stored
/// definitions, so the round-trip and the schema constant are asserted rather than assumed.
/// </summary>
public class HeroDefinitionTests
{
    private static HeroDefinition Sample() => new(
        HeroDefinition.CurrentSchemaVersion,
        "H0DA", "Shadow Nanaya", "Source.w3x", "wc3ctl hero export",
        new[]
        {
            new DefinitionObject("H0DA", "unit", "Shadow Nanaya", "Hpal", true, "root",
                new[] { new DefinitionField("uhpm", "3700"), new DefinitionField("umpm", "1000") }),
            new DefinitionObject("A1R1", "ability", "Neck Attack", "AHtb", true, "dependency",
                new[] { new DefinitionField("acdn", "6.0") }),
        },
        new[] { new DefinitionAsset(@"war3mapImported\Icon.blp", "icon", 4755, "abc123") },
        new[] { "Shadow Nanaya" },
        "script.j",
        new[] { "DarkShikiW_Start" },
        new[] { "integer si__DarkShikiSpells_F= 0" },
        new[] { new DefinitionRequirement("roster-registration", "target must register this", false) },
        new[] { "66 objects excluded" });

    [Fact]
    public void RoundTripsThroughJsonWithoutLoss()
    {
        var json = JsonSerializer.Serialize(Sample());
        var back = JsonSerializer.Deserialize<HeroDefinition>(json);

        Assert.NotNull(back);
        Assert.Equal("H0DA", back!.Id);
        Assert.Equal("Shadow Nanaya", back.Name);
        Assert.Equal(2, back.Objects.Count);
        Assert.Equal("script.j", back.ScriptFile);
        Assert.Single(back.Requires);
        Assert.Equal("roster-registration", back.Requires[0].Kind);
    }

    [Fact]
    public void CarriesObjectFieldsSoADefinitionStandsAlone()
    {
        // Without fields, an install would still need the source map, which defeats the format.
        var back = JsonSerializer.Deserialize<HeroDefinition>(JsonSerializer.Serialize(Sample()))!;
        var root = back.Objects.Single(o => o.Origin == "root");

        Assert.Equal("Hpal", root.BaseRawcode);   // install creates from the base
        Assert.Equal(2, root.Fields.Count);
        Assert.Equal("3700", root.Fields.Single(f => f.Code == "uhpm").Value);
    }

    [Fact]
    public void CarriesAnAssetHashSoCollisionsCanBeJudged()
    {
        // The porter overwrote six of a target's own assets. A hash lets install tell "already
        // exactly this file" from "a DIFFERENT file lives here", instead of clobbering.
        var back = JsonSerializer.Deserialize<HeroDefinition>(JsonSerializer.Serialize(Sample()))!;
        Assert.Equal("abc123", back.Assets[0].Sha256);
        Assert.Equal(4755, back.Assets[0].SizeBytes);
    }

    [Fact]
    public void SchemaVersionIsStatedSoAnOlderBuildCanRefuse()
    {
        Assert.Equal(1, HeroDefinition.CurrentSchemaVersion);
        Assert.Equal(HeroDefinition.CurrentSchemaVersion, Sample().SchemaVersion);
    }
}
