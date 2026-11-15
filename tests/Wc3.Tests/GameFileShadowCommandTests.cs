// tests/Wc3.Tests/GameFileShadowCommandTests.cs
// The shadow check rests on two small rules, that a store path and a map path normalise to the
// same string, and that the editor's own object data is not a shadow. Both are tested here so a
// change to either cannot quietly turn the report into noise or silence.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class GameFileShadowCommandTests
{
    private const string Bs = "\\";

    [Theory]
    [InlineData("war3.w3mod:_de.w3mod:replaceabletextures" + Bs + "commandbuttons" + Bs + "btn3m1.dds",
        "replaceabletextures" + Bs + "commandbuttons" + Bs + "btn3m1.dds")]
    [InlineData("war3.w3mod:ui" + Bs + "console" + Bs + "human" + Bs + "humanuitile05.dds",
        "ui" + Bs + "console" + Bs + "human" + Bs + "humanuitile05.dds")]
    [InlineData("UI" + Bs + "Console" + Bs + "Human" + Bs + "HumanUITile05.dds",
        "ui" + Bs + "console" + Bs + "human" + Bs + "humanuitile05.dds")]
    [InlineData("/units/unitui.slk", "units" + Bs + "unitui.slk")]
    public void Store_paths_and_map_paths_normalise_the_same(string input, string expected)
        => Assert.Equal(expected, GameFileShadowCommand.Normalize(input));

    [Fact]
    public void A_colon_that_is_not_a_layer_prefix_is_left_alone()
    {
        var name = "sound" + Bs + "thing:one.wav";
        Assert.Equal(name.ToLowerInvariant(), GameFileShadowCommand.Normalize(name));
    }

    [Theory]
    [InlineData("units" + Bs + "abilitydata.slk", true)]
    [InlineData("units" + Bs + "campaignabilitystrings.txt", true)]
    [InlineData("war3map.j", true)]
    [InlineData("doodads" + Bs + "doodads.slk", true)]
    [InlineData("ui" + Bs + "console" + Bs + "human" + Bs + "humanuitile05.dds", false)]
    [InlineData("replaceabletextures" + Bs + "commandbuttons" + Bs + "btnx.blp", false)]
    [InlineData("ui" + Bs + "feedback" + Bs + "cooldown" + Bs + "ui-cooldown-indicator.mdx", false)]
    public void Editor_data_is_not_a_shadow(string normalized, bool expected)
        => Assert.Equal(expected, GameFileShadowCommand.IsEditorData(normalized));

    [Fact]
    public void Without_an_install_the_check_refuses_and_says_why()
    {
        var doc = BlankMap.Create();
        var r = GameFileShadowCommand.Detect(doc, null);
        Assert.False(r.Ok);
        Assert.Contains("--game-dir", r.Message);
        Assert.Equal(0, r.Conflicts);
    }
}
