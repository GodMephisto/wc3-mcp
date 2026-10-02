using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PaletteCommandTests
{
    // ---- map-only path (ctx = null): hermetic, needs no WC3 install ----

    [Fact]
    public void DoodadPalette_BlankMap_MapOnly_IsEmpty()
    {
        var doc = BlankMap.Create();
        var res = PaletteCommand.DoodadPalette(doc, ctx: null);
        Assert.True(res.Ok);
        Assert.Empty(res.Entries);
        Assert.Contains("map only", res.Message);
    }

    [Fact]
    public void DoodadPalette_CustomDoodad_ListedAsMapCustom()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Doodad, "ATtr");
        Assert.True(created.Ok);

        var res = PaletteCommand.DoodadPalette(doc, ctx: null);
        Assert.True(res.Ok);
        var entry = Assert.Single(res.Entries);
        Assert.Equal(created.NewRawcode, entry.Rawcode);
        Assert.Equal("map-custom", entry.Source);
        Assert.Equal("ATtr", entry.BaseRawcode);   // derives from the base it was created off
    }

    [Fact]
    public void DoodadPalette_SurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Doodad, "ATtr");
        Assert.True(created.Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var res = PaletteCommand.DoodadPalette(reloaded, ctx: null);
        var entry = Assert.Single(res.Entries);
        Assert.Equal(created.NewRawcode, entry.Rawcode);
        Assert.Equal("map-custom", entry.Source);
    }

    [Fact]
    public void DoodadPalette_MultipleCustoms_SortedByRawcode()
    {
        var doc = BlankMap.Create();
        var a = ObjectNewCommand.Execute(doc, ObjectKind.Doodad, "ATtr");
        var b = ObjectNewCommand.Execute(doc, ObjectKind.Doodad, "ABtr");
        Assert.True(a.Ok && b.Ok);

        var res = PaletteCommand.DoodadPalette(doc, ctx: null);
        Assert.Equal(2, res.Entries.Count);
        var codes = res.Entries.Select(e => e.Rawcode).ToList();
        Assert.Equal(codes.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToList(), codes);
        Assert.All(res.Entries, e => Assert.Equal("map-custom", e.Source));
    }

    // ---- base-catalog path: runs only when a WC3 install is present (soft no-op otherwise) ----

    [Fact]
    public void DoodadPalette_WithInstall_IncludesBaseCatalog()
    {
        if (!GameData.GameData.TryOpen(null, out var ctx, out _)) return; // no install → skip

        var doc = BlankMap.Create();
        var res = PaletteCommand.DoodadPalette(doc, ctx);
        Assert.True(res.Ok);
        Assert.NotEmpty(res.Entries);                                   // stock doodads present
        Assert.Contains(res.Entries, e => e.Source == "base");
        Assert.All(res.Entries, e => Assert.Equal(4, e.Rawcode.Length)); // rawcodes are 4 chars
    }

    // ---- unit palette: same union semantics over the unit catalog (ObjectKind.Unit) ----

    [Fact]
    public void UnitPalette_BlankMap_MapOnly_IsEmpty()
    {
        var doc = BlankMap.Create();
        var res = PaletteCommand.UnitPalette(doc, ctx: null);
        Assert.True(res.Ok);
        Assert.Empty(res.Entries);
        Assert.Contains("map only", res.Message);
        Assert.Contains("unit", res.Message);   // noun is "unit", not "doodad"
    }

    [Fact]
    public void UnitPalette_CustomUnit_ListedAsMapCustom()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo"); // Footman
        Assert.True(created.Ok);

        var res = PaletteCommand.UnitPalette(doc, ctx: null);
        Assert.True(res.Ok);
        var entry = Assert.Single(res.Entries);
        Assert.Equal(created.NewRawcode, entry.Rawcode);
        Assert.Equal("map-custom", entry.Source);
        Assert.Equal("hfoo", entry.BaseRawcode);   // derives from the base it was created off
    }

    [Fact]
    public void UnitPalette_SurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        var created = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(created.Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var res = PaletteCommand.UnitPalette(reloaded, ctx: null);
        var entry = Assert.Single(res.Entries);
        Assert.Equal(created.NewRawcode, entry.Rawcode);
        Assert.Equal("map-custom", entry.Source);
    }

    [Fact]
    public void UnitPalette_WithInstall_IncludesBaseCatalog()
    {
        if (!GameData.GameData.TryOpen(null, out var ctx, out _)) return; // no install → skip

        var doc = BlankMap.Create();
        var res = PaletteCommand.UnitPalette(doc, ctx);
        Assert.True(res.Ok);
        Assert.NotEmpty(res.Entries);                                    // stock units present
        Assert.Contains(res.Entries, e => e.Source == "base");
        Assert.All(res.Entries, e => Assert.Equal(4, e.Rawcode.Length)); // rawcodes are 4 chars
    }
}
