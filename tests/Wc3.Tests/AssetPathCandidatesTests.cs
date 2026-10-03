// tests/Wc3.Tests/AssetPathCandidatesTests.cs
using Wc3.Model;

namespace Wc3.Tests;

public class AssetPathCandidatesTests
{
    [Theory]
    [InlineData(@"war3mapImported\Foo.mdx", true)]
    [InlineData("Foo.mdl", true)]
    [InlineData(@"war3mapImported\BTNFoo", true)] // extensionless, but still a path
    [InlineData("A000", false)] // a bare rawcode, not a path
    [InlineData("Dark Strike", false)] // a display name, not a path
    public void LooksLikeAssetPath_matches_paths_not_plain_values(string token, bool expected) =>
        Assert.Equal(expected, AssetPathCandidates.LooksLikeAssetPath(token));

    [Fact]
    public void Expand_tries_the_sibling_model_extension()
    {
        var candidates = AssetPathCandidates.Expand(@"war3mapImported\Foo.mdl").ToList();
        Assert.Contains(@"war3mapImported\Foo.mdl", candidates);
        Assert.Contains(@"war3mapImported\Foo.mdx", candidates);
    }

    [Fact]
    public void Expand_tries_both_slash_conventions()
    {
        var candidates = AssetPathCandidates.Expand("war3mapImported/Foo.mdx").ToList();
        Assert.Contains(@"war3mapImported\Foo.mdx", candidates);
        Assert.Contains("war3mapImported/Foo.mdx", candidates);
    }

    [Fact]
    public void Expand_appends_every_extension_to_an_extensionless_reference()
    {
        var candidates = AssetPathCandidates.Expand(@"war3mapImported\BTNFoo").ToList();
        Assert.Contains(@"war3mapImported\BTNFoo.blp", candidates);
        Assert.Contains(@"war3mapImported\BTNFoo.tga", candidates);
    }

    [Fact]
    public void Expand_tries_sibling_sound_and_texture_extensions()
    {
        var sound = AssetPathCandidates.Expand(@"Sound\Hero_Foo_Q.wav").ToList();
        Assert.Contains(@"Sound\Hero_Foo_Q.mp3", sound);
        Assert.Contains(@"Sound\Hero_Foo_Q.flac", sound);

        var texture = AssetPathCandidates.Expand(@"Textures\Foo.tga").ToList();
        Assert.Contains(@"Textures\Foo.blp", texture);
        Assert.Contains(@"Textures\Foo.dds", texture);
    }

    [Fact]
    public void Expand_tries_the_other_two_icon_prefixes_and_their_extensions()
    {
        // A field only ever names the "BTN" (active) art, "DISBTN" (disabled) and "PASBTN"
        // (passive) are engine-derived siblings the map never spells out anywhere readable.
        var candidates = AssetPathCandidates.Expand(@"ReplaceableTextures\CommandButtons\BTNFoo.blp").ToList();
        Assert.Contains(@"ReplaceableTextures\CommandButtons\DISBTNFoo.blp", candidates);
        Assert.Contains(@"ReplaceableTextures\CommandButtons\PASBTNFoo.blp", candidates);
        // The sibling still gets the same extension treatment as the original.
        Assert.Contains(@"ReplaceableTextures\CommandButtons\DISBTNFoo.tga", candidates);
    }

    [Fact]
    public void Expand_does_not_touch_a_name_carrying_none_of_the_icon_prefixes()
    {
        var candidates = AssetPathCandidates.Expand(@"war3mapImported\Foo.blp").ToList();
        Assert.DoesNotContain(candidates, c => c.Contains("BTN", StringComparison.OrdinalIgnoreCase));
    }
}
