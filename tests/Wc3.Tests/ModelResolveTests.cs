// tests/Wc3.Tests/ModelResolveTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Regression guard for the shared model resolver. Maps routinely reference a model as
/// ".mdl" while storing the binary under ".mdx" (and vice versa); every front-end must
/// resolve it via <see cref="RenderModelCommand.FindModelEntry"/>. The Studio preview
/// once used a weaker private copy that missed the swap and showed nothing.
/// </summary>
public class ModelResolveTests
{
    private static MapDocument MapWith(string storedName)
    {
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [storedName] = new byte[] { 1, 2, 3, 4 }, // presence is all the resolver checks
        }));
    }

    [Theory]
    [InlineData("war3mapImported\\hero.mdx", "war3mapImported\\hero.mdl")] // referenced .mdl, stored .mdx
    [InlineData("war3mapImported\\hero.mdl", "war3mapImported\\hero.mdx")] // referenced .mdx, stored .mdl
    [InlineData("war3mapImported\\hero.mdx", "war3mapImported/hero.mdx")]  // slash variant
    [InlineData("war3mapImported\\hero.mdx", "war3mapImported\\hero")]     // extensionless reference
    // Classic variation-style storage: no bare file, one model per variation — the
    // resolver falls back to the "<name>0" (variation 0) file.
    [InlineData("war3mapImported\\bush0.mdx", "war3mapImported\\bush")]     // extensionless reference
    [InlineData("war3mapImported\\bush0.mdx", "war3mapImported\\bush.mdx")] // .mdx reference
    [InlineData("war3mapImported\\bush0.mdl", "war3mapImported\\bush.mdl")] // .mdl reference
    public void Resolves_across_extension_swap_and_slash_variants(string stored, string referenced)
    {
        var doc = MapWith(stored);
        var entry = RenderModelCommand.FindModelEntry(doc, referenced);
        Assert.NotNull(entry);
        Assert.Equal(stored, entry!.FileName);
    }

    [Fact]
    public void Returns_null_for_a_model_not_in_the_map()
    {
        var doc = MapWith("war3mapImported\\hero.mdx");
        Assert.Null(RenderModelCommand.FindModelEntry(doc, "war3mapImported\\missing.mdl"));
    }

    [Fact]
    public void An_exact_file_beats_the_variation0_fallback()
    {
        // Both bush.mdx and bush0.mdx exist: the plain name must win — the variation
        // fallback only fires when every non-suffixed form misses.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapImported\\bush.mdx"] = new byte[] { 1, 2, 3, 4 },
            ["war3mapImported\\bush0.mdx"] = new byte[] { 5, 6, 7, 8 },
        }));
        var entry = RenderModelCommand.FindModelEntry(doc, "war3mapImported\\bush");
        Assert.Equal("war3mapImported\\bush.mdx", entry!.FileName);
    }
}
