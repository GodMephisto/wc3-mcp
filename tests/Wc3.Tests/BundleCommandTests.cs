// tests/Wc3.Tests/BundleCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Script;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage (ctx=null → map deltas only) for the unit dependency
/// resolver: object-ref recursion across kinds, custom flagging, asset-file
/// capture with model→texture expansion, TRIGSTR resolution, cycle and
/// false-positive-token handling.
/// </summary>
public class BundleCommandTests
{
    [Fact]
    public void Unit_closure_captures_ability_buff_model_texture_and_strings()
    {
        // H000 (unit) --uabi--> A000 (ability) --abuf:1--> B000 (buff),
        // plus a umdl model import whose MDX names one texture.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "TRIGSTR_1" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\hero.mdx" });
        w3u.NewUnits.Add(unit);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ability = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Strike", Level = 0, Pointer = 0 });
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "abuf".FromRawcode(), Type = ObjectDataType.String, Value = "B000", Level = 1, Pointer = 0 });
        w3a.NewAbilities.Add(ability);

        var w3h = new BuffObjectData(ObjectDataFormatVersion.v2);
        var buff = new SimpleObjectModification { OldId = "BSTN".FromRawcode(), NewId = "B000".FromRawcode() };
        buff.Modifications.Add(new SimpleObjectDataModification
        { Id = "fnam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Buff" });
        w3h.NewBuffs.Add(buff);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.w3h"] = Serialize(w => w.Write(w3h)),
            ["war3map.wts"] = Wts((1u, "Dark Paladin")),
            [@"war3mapImported\hero.mdx"] = TexsOnlyMdx(@"Textures\Hero.blp"),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Equal("H000", bundle.RootRawcode);
        Assert.Equal("Dark Paladin", bundle.RootName);

        // Nodes sorted by kind then rawcode; every one is map-defined → custom.
        Assert.Equal(new[]
        {
            new BundleNode("H000", ObjectKind.Unit, "Dark Paladin", CustomToMap: true),
            new BundleNode("A000", ObjectKind.Ability, "Dark Strike", CustomToMap: true),
            new BundleNode("B000", ObjectKind.Buff, "Dark Buff", CustomToMap: true),
        }, bundle.Objects);

        // The model import is in the map; its texture is referenced but not imported.
        Assert.Equal(new[]
        {
            new BundleFile(@"Textures\Hero.blp", "texture", PresentInMap: false),
            new BundleFile(@"war3mapImported\hero.mdx", "model", PresentInMap: true),
        }, bundle.Files);

        Assert.Equal(new[] { "Dark Paladin" }, bundle.Strings);

        Assert.Contains(new BundleEdge("H000", "A000", "uabi"), bundle.Edges);
        Assert.Contains(new BundleEdge("A000", "B000", "abuf:1"), bundle.Edges);
        Assert.Contains(new BundleEdge("H000", @"war3mapImported\hero.mdx", "umdl"), bundle.Edges);
        Assert.Contains(new BundleEdge(@"war3mapImported\hero.mdx", @"Textures\Hero.blp", "texture"), bundle.Edges);
        // A string edge records WHICH object wants the display string, so a front end can tell the
        // root's own names and tooltips from the ones the script closure carries in.
        Assert.Contains(new BundleEdge("H000", "Dark Paladin", "string"), bundle.Edges);
        Assert.Equal(5, bundle.Edges.Count);
    }

    [Fact]
    public void Ability_root_closure_captures_buff_model_and_texture()
    {
        // ResolveObject seeded at an ABILITY: A000 --abuf:1--> B000 (buff), plus an
        // aeat effect-art model import whose MDX names one texture. Same crawl as
        // the unit path, just rooted at a different kind.
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ability = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Strike", Level = 0, Pointer = 0 });
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "abuf".FromRawcode(), Type = ObjectDataType.String, Value = "B000", Level = 1, Pointer = 0 });
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "aeat".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\spell.mdx", Level = 0, Pointer = 0 });
        w3a.NewAbilities.Add(ability);

        var w3h = new BuffObjectData(ObjectDataFormatVersion.v2);
        var buff = new SimpleObjectModification { OldId = "BSTN".FromRawcode(), NewId = "B000".FromRawcode() };
        buff.Modifications.Add(new SimpleObjectDataModification
        { Id = "fnam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Buff" });
        w3h.NewBuffs.Add(buff);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.w3h"] = Serialize(w => w.Write(w3h)),
            [@"war3mapImported\spell.mdx"] = TexsOnlyMdx(@"Textures\Spell.blp"),
        }));

        var bundle = BundleCommand.ResolveObject(
            doc, ObjectKind.Ability, "A000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Equal("A000", bundle.RootRawcode);
        Assert.Equal("Dark Strike", bundle.RootName);
        Assert.Equal(new[]
        {
            new BundleNode("A000", ObjectKind.Ability, "Dark Strike", CustomToMap: true),
            new BundleNode("B000", ObjectKind.Buff, "Dark Buff", CustomToMap: true),
        }, bundle.Objects);
        Assert.Equal(new[]
        {
            new BundleFile(@"Textures\Spell.blp", "texture", PresentInMap: false),
            new BundleFile(@"war3mapImported\spell.mdx", "model", PresentInMap: true),
        }, bundle.Files);
        Assert.Contains(new BundleEdge("A000", "B000", "abuf:1"), bundle.Edges);
        Assert.Contains(new BundleEdge("A000", @"war3mapImported\spell.mdx", "aeat"), bundle.Edges);
        Assert.Contains(new BundleEdge(@"war3mapImported\spell.mdx", @"Textures\Spell.blp", "texture"), bundle.Edges);
    }

    [Fact]
    public void Missing_root_of_non_unit_kind_names_the_kind()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));

        var bundle = BundleCommand.ResolveObject(
            doc, ObjectKind.Ability, "A999", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Empty(bundle.Objects);
        Assert.Contains(bundle.Diagnostics,
            d => d.Contains("root ability") && d.Contains("A999") && d.Contains("not found"));
    }

    [Fact]
    public void Cyclic_references_terminate_with_each_node_once()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(unit);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ability = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "atgt".FromRawcode(), Type = ObjectDataType.String, Value = "H000", Level = 0, Pointer = 0 });
        w3a.NewAbilities.Add(ability);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Equal(2, bundle.Objects.Count);
        Assert.Contains(new BundleEdge("H000", "A000", "uabi"), bundle.Edges);
        Assert.Contains(new BundleEdge("A000", "H000", "atgt"), bundle.Edges);
    }

    [Fact]
    public void Unresolvable_and_numeric_tokens_are_skipped()
    {
        // Without game data "AHbz" cannot resolve (base store unavailable) and
        // "1200" is a 4-char numeric value, not an object — neither becomes a node.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "AHbz,A000" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 1200 });
        w3u.NewUnits.Add(unit);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Equal(new[] { "H000", "A000" }, bundle.Objects.Select(o => o.Rawcode));
        Assert.Equal(new BundleEdge("H000", "A000", "uabi"), Assert.Single(bundle.Edges));
    }

    [Fact]
    public void Missing_root_yields_empty_bundle_with_diagnostic()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));

        var bundle = BundleCommand.ResolveUnit(doc, "H999", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Empty(bundle.Objects);
        Assert.Empty(bundle.Files);
        Assert.Empty(bundle.Edges);
        Assert.Contains(bundle.Diagnostics, d => d.Contains("H999") && d.Contains("not found"));
    }

    [Fact]
    public void Modified_standard_unit_counts_as_custom_root()
    {
        // A base-list entry (NewId=0) modifies the standard Hpal — the map defines
        // deltas for it, so porting must carry it: CustomToMap and recursed.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = 0 };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.BaseUnits.Add(unit);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ability = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Frost Nova", Level = 0, Pointer = 0 });
        w3a.NewAbilities.Add(ability);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "Hpal", ctx: null, preDiagnostics: Array.Empty<string>());

        var root = bundle.Objects.Single(o => o.Rawcode == "Hpal");
        Assert.True(root.CustomToMap);
        Assert.Contains(bundle.Objects, o => o is { Rawcode: "A000", Kind: ObjectKind.Ability, CustomToMap: true });
    }

    [Fact]
    public void Extensionless_model_ref_captures_the_model_file_and_its_textures()
    {
        // Real-map convention (e.g. Nanaya Shiki H05Y): the unit's model field stores an
        // EXTENSIONLESS path — the game appends .mdx/.mdl at load — while the import is
        // keyed WITH the extension. The resolver must still treat it as a model so the
        // model file AND its textures are captured, not dropped as an unknown "other" file.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "TRIGSTR_1" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\Tohno" });
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.wts"] = Wts((1u, "Tohno")),
            [@"war3mapImported\Tohno.mdx"] = TexsOnlyMdx(@"war3mapImported\Tohno.blp"),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        // The model file is present in the map (resolved by appending .mdx to the ref)...
        Assert.Contains(bundle.Files, f => f is { Category: "model", PresentInMap: true }
            && f.Path == @"war3mapImported\Tohno");
        // ...and its texture was followed (dropped entirely if the ref stayed category "other").
        Assert.Contains(bundle.Files, f => f.Path == @"war3mapImported\Tohno.blp");
        Assert.Contains(bundle.Edges, e => e is { From: "H000", Via: "umdl" });
        Assert.Contains(bundle.Edges, e => e.Via == "texture" && e.To == @"war3mapImported\Tohno.blp");
    }

    [Fact]
    public void Mdl_reference_to_an_mdx_import_captures_the_textures()
    {
        // Regression guard: the field references ".mdl" but the binary is stored as ".mdx"
        // (a routine cross-extension mismatch). FindModelEntry swaps the extension, so
        // presence AND texture-following must keep working through the full AddFileRef path.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "TRIGSTR_1" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\Tohno.mdl" });
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.wts"] = Wts((1u, "Tohno")),
            [@"war3mapImported\Tohno.mdx"] = TexsOnlyMdx(@"war3mapImported\Tohno.blp"),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Contains(bundle.Files, f => f is { Category: "model", PresentInMap: true });
        Assert.Contains(bundle.Files, f => f.Path == @"war3mapImported\Tohno.blp");
    }

    [Fact]
    public void Extensionless_icon_reference_is_found_and_categorized_icon()
    {
        // A unit whose icon field stores the command-button path with NO extension (the game
        // appends .blp at load); the file is imported as ...BTNRaiden.blp. The bundle must find
        // it and mark it present, else the port drops the custom icon and the unit shows the
        // black-and-green missing box.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        {
            Id = "uico".FromRawcode(), Type = ObjectDataType.String,
            Value = @"ReplaceableTextures\CommandButtons\BTNRaiden",
        });
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            [@"ReplaceableTextures\CommandButtons\BTNRaiden.blp"] = new byte[] { 1, 2, 3, 4 },
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        var icon = bundle.Files.Single(f => f.Path == @"ReplaceableTextures\CommandButtons\BTNRaiden");
        Assert.True(icon.PresentInMap, "extensionless icon import must resolve to the stored .blp");
        Assert.Equal("icon", icon.Category);
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static byte[] Wts(params (uint Key, string Value)[] entries)
    {
        var wts = new TriggerStrings();
        foreach (var (key, value) in entries)
            wts.Strings.Add(new TriggerString { Key = key, Value = value });
        using var ms = new MemoryStream();
        using (var sw = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true)) sw.WriteTriggerStrings(wts);
        return ms.ToArray();
    }

    /// <summary>Minimal parseable MDX: MDLX magic + VERS + a TEXS chunk (no geometry).</summary>
    private static byte[] TexsOnlyMdx(params string[] texturePaths)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);
        w.Write("VERS"u8); w.Write(4u); w.Write(800u);
        w.Write("TEXS"u8); w.Write((uint)(268 * texturePaths.Length));
        foreach (var path in texturePaths)
        {
            w.Write(0u); // replaceableId
            var bytes = Encoding.ASCII.GetBytes(path);
            w.Write(bytes);
            w.Write(new byte[260 - bytes.Length]);
            w.Write(0u); // flags
        }
        w.Flush();
        return ms.ToArray();
    }
}
