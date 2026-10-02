// tests/Wc3.Tests/BaseModelRenderTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Base-game (CASC) model preview: units whose model was never imported into the map
/// (a plain Footman) render from the install's game data. Install-dependent cases are
/// GameData-gated; the no-install degradation stays hermetic.
/// </summary>
public class BaseModelRenderTests
{
    private const string Install = @"C:\Warcraft III";

    private static MapDocument EmptyMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));

    [Fact]
    [Trait("Category", "GameData")]
    public void Context_reads_a_base_model_file_through_casc()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);

        // Plain game path — the context addresses it under war3.w3mod: like the stores do.
        Assert.True(ctx!.TryReadFile(@"units\human\footman\footman.mdx", out var bytes),
            "footman.mdx should be readable from CASC");
        Assert.True(bytes.Length > 1000, $"expected real model bytes, got {bytes.Length}");

        // Slash-agnostic, and unknown files are a clean false.
        Assert.True(ctx.TryReadFile("units/human/footman/footman.mdx", out _));
        Assert.False(ctx.TryReadFile(@"units\no\such\model.mdx", out _));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Renders_a_base_game_unit_model_from_casc()
    {
        if (!Directory.Exists(Install)) return;
        // hfoo is not in the map at all — its umdl resolves from base data and the
        // model + textures must come from CASC.
        var png = RenderModelCommand.Execute(EmptyMap(), ObjectKind.Unit, "hfoo", Install);
        Assert.True(png.Length > 500, $"expected a real PNG, got {png.Length} bytes");
        Assert.Equal(0x89, png[0]); // PNG signature
        Assert.Equal((byte)'P', png[1]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Renders_a_base_model_by_explicit_path_from_casc()
    {
        if (!Directory.Exists(Install)) return;
        // The umdl form maps use: extensionless — the .mdx candidate must be tried.
        var png = RenderModelCommand.ExecutePath(
            EmptyMap(), @"units\human\footman\footman", Install);
        Assert.True(png.Length > 500);
        Assert.Equal(0x89, png[0]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Decodes_a_reforged_dds_texture_from_casc()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);
        // Model references say "Textures\Footman.blp"; Reforged stores footman.dds —
        // the renderer's candidate swap depends on the .dds being there and decodable.
        Assert.True(ctx!.TryReadFile(@"textures\footman.dds", out var dds));
        Assert.True(Wc3.Modeling.DdsDecoder.LooksLikeDds(dds));
        var img = Wc3.Modeling.DdsDecoder.Decode(dds);
        Assert.True(img.Width >= 4 && img.Height >= 4);
        Assert.Contains(img.Rgba, b => b != 0); // real texels, not an all-black plane
    }

    [Fact]
    public void Without_an_install_a_non_imported_model_fails_with_a_clear_message()
    {
        // Hermetic: a bad --game-dir degrades to map-only resolution, and the error
        // says why the base-game fallback was unavailable.
        var ex = Assert.Throws<InvalidDataException>(() =>
            RenderModelCommand.ExecutePath(EmptyMap(), @"units\human\footman\footman.mdx", "Z:\\no_such"));
        Assert.Contains("not in the map", ex.Message);
        Assert.Contains("no game install", ex.Message);
    }
}
