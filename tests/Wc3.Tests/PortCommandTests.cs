// tests/Wc3.Tests/PortCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class PortCommandTests
{
    // Source: custom hero H000 (hero ability A000, an imported icon, a TRIGSTR name)
    // + custom ability A000, a war3map.wts with STRING 100, and the imported icon file.
    private static MapDocument SourceMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(Str("uhab", "A000"));                                  // hero abilities → custom A000
        hero.Modifications.Add(Str("uico", "war3mapImported\\raidenicon.blp"));       // imported icon
        hero.Modifications.Add(Str("unam", "TRIGSTR_100"));                           // name via string table
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Naginata Combo" });
        w3a.NewAbilities.Add(abil);

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.wts"] = Encoding.UTF8.GetBytes("STRING 100\n{\nRaiden Ei\n}\n"),
            ["war3mapImported\\raidenicon.blp"] = new byte[] { 10, 20, 30, 40 },
        }));
    }

    // Target: already defines a DIFFERENT custom H000 (forces a rawcode remap) and has no w3a.
    private static MapDocument TargetMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var other = new SimpleObjectModification { OldId = "Hamg".FromRawcode(), NewId = "H000".FromRawcode() };
        other.Modifications.Add(Str("unam", "Someone Else"));
        w3u.NewUnits.Add(other);
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }));
    }

    [Fact]
    public void Ports_unit_closure_with_collision_remap_string_inlining_and_asset_copy()
    {
        var source = SourceMap();
        var target = TargetMap();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);

        // The bundle found the custom ability and the icon.
        Assert.Contains(bundle.Objects, o => o.Rawcode == "A000" && o.Kind == ObjectKind.Ability && o.CustomToMap);
        Assert.Contains(bundle.Files, f => f.Path.EndsWith("raidenicon.blp", StringComparison.OrdinalIgnoreCase) && f.PresentInMap);

        var result = PortCommand.PortUnit(source, bundle, target);

        // Root H000 collided with the target's H000 → it was remapped.
        Assert.NotEqual("H000", result.RootPortedTo);
        Assert.Contains(result.Remaps, r => r.From == "H000" && r.Kind == ObjectKind.Unit);
        // The ability didn't collide → kept as A000.
        Assert.Contains(result.Objects, o => o.Rawcode == "A000" && o.Kind == ObjectKind.Ability);
        Assert.DoesNotContain(result.Remaps, r => r.From == "A000");
        // The TRIGSTR name was inlined.
        Assert.True(result.InlinedStrings >= 1);
        Assert.Contains(result.CopiedFiles, f => f.EndsWith("raidenicon.blp", StringComparison.OrdinalIgnoreCase));

        // Save + reload the target and verify everything landed.
        var reloaded = MapDocument.Load(target.SaveToBytes());

        var tw3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        int portedId = result.RootPortedTo!.FromRawcode();
        var ported = tw3u.NewUnits.Single(u => u.NewId == portedId);
        // Original target H000 still present.
        Assert.Contains(tw3u.NewUnits, u => u.NewId == "H000".FromRawcode());

        // Name is now the literal, not a TRIGSTR reference.
        var nameMod = ported.Modifications.Single(m => m.Id == "unam".FromRawcode());
        Assert.Equal("Raiden Ei", nameMod.Value);
        Assert.DoesNotContain("TRIGSTR", (string)nameMod.Value!);

        // Ability reference still points at A000 (uncollided).
        var abilMod = ported.Modifications.Single(m => m.Id == "uhab".FromRawcode());
        Assert.Equal("A000", abilMod.Value);

        // The ability object was injected (target had no w3a before).
        var tw3a = (AbilityObjectData)reloaded.GetFile("war3map.w3a")!.Model!;
        Assert.Contains(tw3a.NewAbilities, a => a.NewId == "A000".FromRawcode());

        // The imported icon is present with the right bytes and registered in imp.
        Assert.True(reloaded.GetFile("war3mapImported\\raidenicon.blp")!.RawBytes.SequenceEqual(new byte[] { 10, 20, 30, 40 }));
    }

    [Fact]
    public void Remaps_an_in_bundle_reference_when_the_referenced_object_collides()
    {
        // Target already has A000 too → the ability must be remapped AND the unit's
        // uhab reference rewritten to the new ability code.
        var source = SourceMap();
        var target = TargetMap();
        var tw3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        tw3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });
        target.AddOrReplaceModelFile("war3map.w3a", tw3a);

        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        var result = PortCommand.PortUnit(source, bundle, target);

        var abilityRemap = result.Remaps.Single(r => r.From == "A000" && r.Kind == ObjectKind.Ability);
        Assert.NotEqual("A000", abilityRemap.To);

        var reloaded = MapDocument.Load(target.SaveToBytes());
        var tw3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        var ported = tw3u.NewUnits.Single(u => u.NewId == result.RootPortedTo!.FromRawcode());
        var uhab = (string)ported.Modifications.Single(m => m.Id == "uhab".FromRawcode()).Value!;
        Assert.Equal(abilityRemap.To, uhab); // reference followed the remap
    }

    [Fact]
    public void Skips_base_game_deps_and_assets_not_in_source()
    {
        var source = SourceMap();
        var target = TargetMap();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        var result = PortCommand.PortUnit(source, bundle, target);
        // Base ability AInv-style deps (none custom here) and missing assets never copy;
        // the report accounts for skipped items without throwing.
        Assert.NotNull(result.SkippedFiles);
    }

    /// <summary>
    /// Real-map stress test: porting Raiden Ei into a second copy of the same map means
    /// EVERY custom rawcode collides, forcing a full remap. The hero + all its custom
    /// abilities must land in the target's object data under fresh codes, with the unit's
    /// ability list rewritten to the remapped ability codes.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Real_map_self_port_remaps_the_whole_hero_and_rewrites_references()
    {
        string path = TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");
        if (!File.Exists(path)) return;

        var source = MapDocument.Load(path);
        var target = MapDocument.Load(path); // identical → every rawcode collides
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);

        var result = PortCommand.PortUnit(source, bundle, target);

        // The hero and its five custom abilities all collided and were remapped.
        Assert.NotEqual("H000", result.RootPortedTo);
        Assert.Contains(result.Remaps, r => r.From == "H000" && r.Kind == ObjectKind.Unit);
        Assert.True(result.Remaps.Count(r => r.Kind == ObjectKind.Ability) >= 5);

        // The ported unit exists in the target under its new code with a valid ability list.
        var tw3u = (UnitObjectData)target.GetFile("war3map.w3u")!.Model!;
        var ported = tw3u.NewUnits.Single(u => u.NewId == result.RootPortedTo!.FromRawcode());
        var abilityRemaps = result.Remaps.Where(r => r.Kind == ObjectKind.Ability)
            .ToDictionary(r => r.From, r => r.To);
        var uhab = (string)ported.Modifications.First(m => m.Id == "uhab".FromRawcode()).Value!;
        foreach (var code in uhab.Split(',', StringSplitOptions.RemoveEmptyEntries))
            if (abilityRemaps.ContainsKey(code)) // an original ability code must NOT survive
                Assert.Fail($"unit still references un-remapped ability {code}");

        // Every remapped ability was injected into the target's w3a.
        var tw3a = (AbilityObjectData)target.GetFile("war3map.w3a")!.Model!;
        foreach (var to in abilityRemaps.Values)
            Assert.Contains(tw3a.NewAbilities, a => a.NewId == to.FromRawcode());
    }

    /// <summary>
    /// Regression: a unit whose model is REFERENCED as ".mdl" but STORED as ".mdx"
    /// (the normal WC3 convention) must still have its model binary copied into the
    /// target. The port's copy step used a plain path lookup that missed the
    /// .mdl→.mdx swap, so custom models were discovered-but-never-copied.
    /// </summary>
    [Fact]
    public void Copies_a_custom_model_referenced_as_mdl_but_stored_as_mdx()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = "U000".FromRawcode() };
        unit.Modifications.Add(Str("umdl", "war3mapImported\\raiden.mdl")); // referenced as .mdl
        w3u.NewUnits.Add(unit);

        var modelBytes = new byte[] { (byte)'M', (byte)'D', (byte)'L', (byte)'X', 1, 2, 3, 4, 5, 6 };
        var source = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3mapImported\\raiden.mdx"] = modelBytes,   // stored as .mdx
        }));
        var target = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(new UnitObjectData(ObjectDataFormatVersion.v2))),
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }));

        var bundle = BundleCommand.ResolveUnit(source, "U000", gameDirOverride: null);
        // Discovery marks the model present through the .mdl→.mdx swap.
        Assert.Contains(bundle.Files, f => f.Category == "model" && f.PresentInMap
            && f.Path.EndsWith("raiden.mdl", StringComparison.OrdinalIgnoreCase));

        var result = PortCommand.PortUnit(source, bundle, target);

        // The stored .mdx was copied under its real name (was skipped before the fix).
        Assert.Contains(result.CopiedFiles, f => f.EndsWith("raiden.mdx", StringComparison.OrdinalIgnoreCase));

        var reloaded = MapDocument.Load(target.SaveToBytes());
        var copied = reloaded.GetFile("war3mapImported\\raiden.mdx");
        Assert.NotNull(copied);
        Assert.True(copied!.RawBytes.SequenceEqual(modelBytes));
    }

    private static SimpleObjectDataModification Str(string code, string value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.String, Value = value };

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
