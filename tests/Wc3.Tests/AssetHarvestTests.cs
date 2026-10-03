// tests/Wc3.Tests/AssetHarvestTests.cs
// Covers MapDocument.HarvestAssetNames, the second-pass recovery that harvests candidate
// asset paths from a protected map's own (now readable, via ProtectedMapRecoveryTests'
// standard-name probe) script and object data, then probes the archive's hash table for
// those the same way, so an imported model/texture/sound gets a real name instead of
// staying an anonymous blob.
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Tests;

public class AssetHarvestTests
{
    [Fact]
    public void Load_alone_does_not_name_an_asset_only_the_script_references()
    {
        // Harvesting is a deliberate second pass, not part of Load, so a plain Load must
        // leave a script-referenced asset exactly as unnamed as any other author asset.
        var script = "globals\r\nendglobals\r\nfunction Cast takes nothing returns nothing\r\n"
            + "    call AddSpecialEffect(\"war3mapImported\\\\Foo.mdl\", 0, 0)\r\n"
            + "endfunction\r\n";
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(script),
                [@"war3mapImported\Foo.mdx"] = new byte[] { 1, 2, 3 },
            });

        var doc = MapDocument.Load(bytes);

        Assert.NotNull(doc.GetFile("war3map.j")); // phase 1 already recovers the script itself
        Assert.Null(doc.GetFile(@"war3mapImported\Foo.mdx"));
    }

    [Fact]
    public void Harvest_finds_a_script_referenced_model_despite_the_mdl_mdx_swap()
    {
        // Warcraft loads models extension-agnostically, the script names "Foo.mdl" but the
        // archive stores "Foo.mdx" (or vice versa), so the harvest must try the sibling
        // extension, not just the literal spelling the script uses.
        var script = "globals\r\nendglobals\r\nfunction Cast takes nothing returns nothing\r\n"
            + "    call AddSpecialEffect(\"war3mapImported\\\\Foo.mdl\", 0, 0)\r\n"
            + "endfunction\r\n";
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(script),
                [@"war3mapImported\Foo.mdx"] = new byte[] { 1, 2, 3 },
            });

        var doc = MapDocument.Load(bytes);
        int named = doc.HarvestAssetNames();

        Assert.True(named >= 1);
        var asset = doc.GetFile(@"war3mapImported\Foo.mdx");
        Assert.NotNull(asset);
        Assert.Equal(new byte[] { 1, 2, 3 }, asset!.RawBytes);
    }

    [Fact]
    public void Harvest_finds_an_asset_referenced_only_by_an_object_data_field()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var mod = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = "h000".FromRawcode() };
        mod.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\Bar.mdx" });
        w3u.NewUnits.Add(mod);
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) bw.Write(w3u);

        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.w3u"] = ms.ToArray(),
                [@"war3mapImported\Bar.mdx"] = new byte[] { 4, 5, 6 },
            });

        var doc = MapDocument.Load(bytes);
        // war3map.w3u itself is a standard name, phase 1 already recovers and parses it.
        Assert.IsType<UnitObjectData>(doc.GetFile("war3map.w3u")!.Model);
        Assert.Null(doc.GetFile(@"war3mapImported\Bar.mdx"));

        int named = doc.HarvestAssetNames();

        Assert.True(named >= 1);
        Assert.Equal(new byte[] { 4, 5, 6 }, doc.GetFile(@"war3mapImported\Bar.mdx")!.RawBytes);
    }

    [Fact]
    public void Harvest_on_a_map_with_no_recoverable_script_or_object_data_finds_nothing()
    {
        // Nothing to scan means nothing to find, harvesting an unnamed blob's own bytes is
        // not the point, only content this map itself already makes readable is a source.
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]> { ["mystery.mdx"] = new byte[] { 1 } });

        var doc = MapDocument.Load(bytes);
        Assert.Equal(0, doc.HarvestAssetNames());
        Assert.Null(doc.GetFile("mystery.mdx"));
    }

    [Fact]
    public void Harvest_does_not_mark_anything_dirty_or_change_saved_bytes()
    {
        var script = "globals\r\nendglobals\r\nfunction Cast takes nothing returns nothing\r\n"
            + "    call AddSpecialEffect(\"war3mapImported\\\\Foo.mdx\", 0, 0)\r\n"
            + "endfunction\r\n";
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(script),
                [@"war3mapImported\Foo.mdx"] = new byte[] { 1, 2, 3 },
            });

        var doc = MapDocument.Load(bytes);
        doc.HarvestAssetNames();
        var asset = doc.GetFile(@"war3mapImported\Foo.mdx");
        Assert.NotNull(asset);
        Assert.False(asset!.IsDirty);

        var saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);
        // The reload sees the same archive with the same stripped listfile, so the asset is
        // unnamed again until harvested a second time, that is expected, what matters is the
        // bytes underneath did not move or change across the save/reload round trip.
        int namedAgain = reloaded.HarvestAssetNames();
        Assert.True(namedAgain >= 1);
        Assert.Equal(new byte[] { 1, 2, 3 }, reloaded.GetFile(@"war3mapImported\Foo.mdx")!.RawBytes);
    }
}
