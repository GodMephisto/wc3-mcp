// tests/Wc3.Tests/PortToolkitCommandTests.cs
// Hermetic command-layer coverage for the port/bundle toolkit, the dependency report, the
// dry-run preview, the into-version downgrade, and the bundle diff and merge. Every test builds
// synthetic maps in memory, so none needs a WC3 install or a real .w3x on disk.
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Script;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

[Collection("Serial")]
public class PortToolkitCommandTests
{
    // ---- port deps ----------------------------------------------------------

    [Fact]
    public void Port_deps_groups_the_closure_and_sizes_the_imported_asset()
    {
        var doc = SourceMap();
        var r = PortDepsCommand.Execute(doc, "H000", ObjectKind.Unit, gameDirOverride: null);

        Assert.Equal("H000", r.RootRawcode);
        Assert.Equal("unit", r.RootKind);
        var kinds = r.ObjectGroups.Select(g => g.Kind).ToList();
        Assert.Contains("unit", kinds);
        Assert.Contains("ability", kinds);

        var icon = Assert.Single(r.Assets, a => a.Path == @"war3mapImported\raidenicon.blp");
        Assert.True(icon.PresentInMap);
        Assert.Equal(8, icon.SizeBytes);
        Assert.Equal(8, r.BytesInMap);
    }

    // ---- port preview -------------------------------------------------------

    [Fact]
    public void Port_preview_reports_the_plan_and_writes_nothing()
    {
        var source = SourceMap();
        var target = TargetMap();
        var r = PortPreviewCommand.Execute(source, "H000", target, gameDirOverride: null);

        Assert.NotEmpty(r.Objects);
        Assert.NotEmpty(r.CopiedFiles);

        // The target must be byte-identical apart from MPQ bookkeeping, with no injected w3u entry.
        var reloaded = MapDocument.Load(target.SaveToBytes());
        var w3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        Assert.Single(w3u.NewUnits);
        Assert.Equal("H000".FromRawcode(), w3u.NewUnits[0].NewId);
    }

    // ---- port into version --------------------------------------------------

    [Fact]
    public void Port_into_version_rewrites_undeclared_object_fields_and_flags_script_identifiers()
    {
        var source = SourceMap();
        var target = TargetMapWithScript();
        var schemaDir = BuildSchema();

        var r = PortIntoVersionCommand.Execute(
            source, "H000", target, schemaDir, schemasDir: null,
            gameDirOverride: null, includeScript: false);

        Assert.Equal("smoke-build", r.TargetBuild);
        Assert.NotEmpty(r.Port.Objects);

        // every field the unit set other than unam is undeclared, so all of them are removed.
        var removed = r.FieldsRemoved.Where(f => f.Kind == "unit" && f.Rawcode == "H000").ToList();
        Assert.Contains(removed, f => f.FieldCode == "uhpm");
        Assert.Contains(removed, f => f.FieldCode == "uico");
        Assert.DoesNotContain(removed, f => f.FieldCode == "unam");
        Assert.All(removed, f => Assert.Equal("Raiden Ei", f.Name));

        // DeclaredNative is in the schema; UndeclaredNative is not, so only the latter is flagged.
        Assert.Contains(r.ScriptIdentifiersFlagged, i => i.Name == "UndeclaredNative");
        Assert.DoesNotContain(r.ScriptIdentifiersFlagged, i => i.Name == "DeclaredNative");

        // Read back, the ported H000 keeps unam and has lost uhpm.
        var reloaded = MapDocument.Load(target.SaveToBytes());
        var w3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        var hero = w3u.NewUnits.Single(u => u.NewId == "H000".FromRawcode());
        Assert.Contains(hero.Modifications, m => m.Id == "unam".FromRawcode());
        Assert.DoesNotContain(hero.Modifications, m => m.Id == "uhpm".FromRawcode());
    }

    // ---- bundle diff --------------------------------------------------------

    [Fact]
    public void Bundle_diff_lists_the_added_object()
    {
        var mapA = SourceMap();
        var bundleA = BundleCommand.ResolveUnit(mapA, "H000", gameDirOverride: null);

        var mapB = SourceMap(withSecondAbility: true);
        var bundleB = BundleCommand.ResolveUnit(mapB, "H000", gameDirOverride: null);

        var r = BundleDiffCommand.Execute(bundleA, bundleB);

        Assert.Contains(r.Added, m => m.Id == "A001");
        Assert.DoesNotContain(r.Removed, m => m.Id == "H000");
        Assert.True(r.Shared >= 2); // H000 and A000 are in both
    }

    // ---- bundle merge -------------------------------------------------------

    [Fact]
    public void Bundle_merge_unions_two_clean_bundles()
    {
        var bundleA = BundleCommand.ResolveUnit(SourceMap(), "H000", gameDirOverride: null);
        var bundleB = BundleCommand.ResolveUnit(
            SourceMap(withSecondAbility: true), "H000", gameDirOverride: null);

        var r = BundleMergeCommand.Execute(bundleA, bundleB);

        Assert.True(r.Ok);
        Assert.NotNull(r.Merged);
        Assert.Contains(r.Merged!.Objects, o => o.Rawcode == "A001");
        Assert.Contains(r.Merged.Objects, o => o.Rawcode == "H000");
    }

    [Fact]
    public void Bundle_merge_refuses_a_rawcode_defined_as_two_kinds()
    {
        var a = new UnitBundle("H000", "Hero",
            new[] { new BundleNode("H000", ObjectKind.Unit, "Hero", true) },
            Array.Empty<BundleFile>(), Array.Empty<string>(), Array.Empty<BundleEdge>(),
            Array.Empty<string>(), Array.Empty<BundleFunction>());
        var b = new UnitBundle("H000", "Other",
            new[] { new BundleNode("H000", ObjectKind.Ability, "Other", true) },
            Array.Empty<BundleFile>(), Array.Empty<string>(), Array.Empty<BundleEdge>(),
            Array.Empty<string>(), Array.Empty<BundleFunction>());

        var r = BundleMergeCommand.Execute(a, b);

        Assert.False(r.Ok);
        Assert.Null(r.Merged);
        var conflict = Assert.Single(r.Conflicts);
        Assert.Equal("H000", conflict.Rawcode);
        Assert.Contains("unit", conflict.Detail);
        Assert.Contains("ability", conflict.Detail);
    }

    // ---- bundle json round trip ---------------------------------------------

    [Fact]
    public void Bundle_json_round_trips_through_the_artifact_format()
    {
        var bundle = BundleCommand.ResolveUnit(SourceMap(), "H000", gameDirOverride: null);
        var reopened = BundleJson.Deserialize(BundleJson.Serialize(bundle));

        Assert.Equal(bundle.RootRawcode, reopened.RootRawcode);
        Assert.Equal(bundle.Objects.Select(o => o.Rawcode), reopened.Objects.Select(o => o.Rawcode));
        Assert.Equal(bundle.Files.Select(f => f.Path), reopened.Files.Select(f => f.Path));
    }

    // ---- fixtures -----------------------------------------------------------

    /// <summary>Hero H000 (custom unit) with ability A000 and an imported icon, plus a leveled A000.
    /// With <paramref name="withSecondAbility"/> it also defines A001 and lists it on H000.</summary>
    private static MapDocument SourceMap(bool withSecondAbility = false)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(Str("unam", "TRIGSTR_1"));
        hero.Modifications.Add(Str("uhpm", 999));
        hero.Modifications.Add(Str("uabi", withSecondAbility ? "A000,A001" : "A000"));
        hero.Modifications.Add(Str("uico", @"war3mapImported\raidenicon.blp"));
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Naginata" });
        w3a.NewAbilities.Add(abil);
        if (withSecondAbility)
            w3a.NewAbilities.Add(new LevelObjectModification
            { OldId = "AHtb".FromRawcode(), NewId = "A001".FromRawcode() });

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.wts"] = Wts((1u, "Raiden Ei")),
            [@"war3mapImported\raidenicon.blp"] = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        }));
    }

    /// <summary>A target defining its own H000 (forcing a remap) and A000.</summary>
    private static MapDocument TargetMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var other = new SimpleObjectModification { OldId = "Hamg".FromRawcode(), NewId = "H000".FromRawcode() };
        other.Modifications.Add(Str("unam", "Someone Else"));
        w3u.NewUnits.Add(other);
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
        }));
    }

    /// <summary>A target whose script calls one native the schema will declare and one it will not.</summary>
    private static MapDocument TargetMapWithScript()
    {
        const string script = @"function Trig_Smoke_Actions takes nothing returns nothing
    call DeclaredNative()
    call UndeclaredNative()
endfunction
";
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));
    }

    /// <summary>A schema folder declaring the unit field unam and the native DeclaredNative only.</summary>
    private static string BuildSchema()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"wc3ctl_schema_{Guid.NewGuid():N}");
        var objects = new Dictionary<string, IReadOnlyList<SchemaField>>
        {
            ["units/unitmetadata.slk"] = new[]
            {
                new SchemaField("unam", "UnitBalance", "Name", "string", "WESTRING", "Name",
                    "text", 0, 1, 0, "", "", "", "", true, true, true, true, true),
            },
        };
        var script = new Dictionary<string, JassApi>
        {
            ["scripts/common.j"] = new JassApi(
                Array.Empty<JassTypeDecl>(),
                new[] { new JassNativeDecl("DeclaredNative", Array.Empty<JassParameter>(), "nothing", false) },
                Array.Empty<JassGlobalDecl>(),
                Array.Empty<JassFunctionDecl>()),
        };
        GameSchemaCommand.Save(
            new GameSchema(1, "smoke-build", "test", Array.Empty<SchemaInput>(), Array.Empty<string>(),
                objects, null, script),
            dir);
        return dir;
    }

    private static SimpleObjectDataModification Str(string code, string value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.String, Value = value };

    private static SimpleObjectDataModification Str(string code, int value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.Int, Value = value };

    private static byte[] Ser(Action<BinaryWriter> write)
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
}
