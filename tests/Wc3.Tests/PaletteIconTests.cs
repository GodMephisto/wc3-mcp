using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Icon-art resolution on palette entries (PaletteEntry.IconPath) and the IconPng
/// decode hook. Hermetic tests cover the map-only path (a 'uico' delta wins with no
/// install at all); the base-catalog path — icons from Reforged's units\unitskin.txt
/// via CASC — soft-skips without a WC3 install, like the other GameData tests.
/// </summary>
public class PaletteIconTests
{
    private const string FootmanIcon = @"ReplaceableTextures\CommandButtons\BTNFootman.blp";

    // ---- map-only path (ctx = null): hermetic, needs no WC3 install ----

    [Fact]
    public void UnitPalette_MapOnly_UicoDelta_BecomesIconPath()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(created.Ok);
        var set = ObjectSetCommand.Execute(doc, created.NewRawcode!, "uico", FootmanIcon);
        Assert.True(set.Ok, set.Message);

        var res = PaletteCommand.UnitPalette(doc, ctx: null);
        Assert.True(res.Ok);
        var entry = Assert.Single(res.Entries);
        Assert.Equal(FootmanIcon, entry.IconPath);
    }

    [Fact]
    public void UnitPalette_MapOnly_NoIconDelta_IconPathIsNull()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(created.Ok);

        var res = PaletteCommand.UnitPalette(doc, ctx: null);
        var entry = Assert.Single(res.Entries);
        Assert.Null(entry.IconPath);   // no delta and no install → nothing to resolve
    }

    [Fact]
    public void DoodadPalette_Entries_CarryNoIconPath()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Doodad, "ATtr");
        Assert.True(created.Ok);

        var res = PaletteCommand.DoodadPalette(doc, ctx: null);
        var entry = Assert.Single(res.Entries);
        Assert.Null(entry.IconPath);   // doodads have no icon art — null by design
    }

    [Fact]
    public void IconPng_UnresolvablePath_ReturnsNull_NeverThrows()
    {
        var doc = BlankMap.Create();
        Assert.Null(PaletteCommand.IconPng(doc, @"ReplaceableTextures\CommandButtons\BTNNope.blp", ctx: null));
        Assert.Null(PaletteCommand.IconPng(doc, "", ctx: null));
        Assert.Null(PaletteCommand.IconPng(doc, "   ", ctx: null));
    }

    // ---- base-catalog path: needs a WC3 install (soft no-op otherwise) ----

    [Fact]
    [Trait("Category", "GameData")]
    public void UnitPalette_WithInstall_TypicalUnits_CarryIconPath()
    {
        if (!GameData.GameData.TryOpen(null, out var ctx, out _)) return; // no install → skip

        var doc = BlankMap.Create();
        var res = PaletteCommand.UnitPalette(doc, ctx);
        Assert.True(res.Ok);

        // Everyday units: Footman, Peasant, Grunt — all base rows with skin-profile art.
        foreach (var code in new[] { "hfoo", "hpea", "ogru" })
        {
            var entry = res.Entries.FirstOrDefault(e => e.Rawcode == code);
            Assert.NotNull(entry);
            Assert.False(string.IsNullOrWhiteSpace(entry!.IconPath),
                $"{code} should carry icon art from units\\unitskin.txt");
        }
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void IconPng_WithInstall_DecodesFootmanIcon_ToPng()
    {
        if (!GameData.GameData.TryOpen(null, out var ctx, out _)) return; // no install → skip

        var doc = BlankMap.Create();
        var png = PaletteCommand.IconPng(doc, FootmanIcon, ctx);
        Assert.NotNull(png);
        // PNG magic: 0x89 'P' 'N' 'G'.
        Assert.True(png!.Length > 8);
        Assert.Equal(0x89, png[0]);
        Assert.Equal((byte)'P', png[1]);
        Assert.Equal((byte)'N', png[2]);
        Assert.Equal((byte)'G', png[3]);
    }

    // ---- real map + install: end to end over a big custom-unit catalog ----

    [Fact]
    [Trait("Category", "Corpus")]
    public void UnitPalette_CorpusMap_WithInstall_EntriesCarryIcons()
    {
        string map = TestCorpus.Map(@"Anime_WOS2_0.27d3.w3x");
        if (!File.Exists(map)) return;                                    // map not on this machine
        if (!GameData.GameData.TryOpen(null, out var ctx, out _)) return; // no install → skip

        var doc = MapDocument.Load(map);
        var res = PaletteCommand.UnitPalette(doc, ctx);
        Assert.True(res.Ok);
        Assert.NotEmpty(res.Entries);
        Assert.Contains(res.Entries, e => e.Source == "base" && e.IconPath != null);
        // hfoo is always in the base catalog and always has command-button art.
        var footman = res.Entries.FirstOrDefault(e => e.Rawcode == "hfoo");
        Assert.NotNull(footman);
        Assert.NotNull(footman!.IconPath);
    }
}
