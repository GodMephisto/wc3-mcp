// tests/Wc3.Tests/McpToolSmokeTests.Bundles.cs
// Bundle and port-into-version family. A synthetic source map with one custom unit, its custom
// ability and one imported asset is enough to exercise every member group; the blank fixture is
// the target. Bundles are written with the same BundleJson the CLI writes, so the diff and merge
// tools read exactly the artifact the CLI produces.
using System.Text;
using System.Text.Json;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeBundlesTests : McpSmokeBase
{
    public McpSmokeBundlesTests(SmokeFixture f) : base(f) { }

    /// <summary>A source map holding custom hero H000 with custom ability A000 (name-leveled) and
    /// an imported icon the unit's uico names, so the closure has an object, a leveled object and
    /// an asset.</summary>
    private string SourceMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uico".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\smokeicon.blp" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Smoke Strike", Level = 0, Pointer = 0 });
        w3a.NewAbilities.Add(abil);

        var path = Path.Combine(_f.Dir, $"bundle-src-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            [@"war3mapImported\smokeicon.blp"] = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        }));
        return path;
    }

    /// <summary>Write a resolved bundle to a JSON file the diff and merge tools can read.</summary>
    private string BundleFile(string map, string rawcode, string root, ObjectKind kind)
    {
        var bundle = BundleCommand.ResolveObject(
            MapDocument.Load(map), kind, rawcode, gameDirOverride: null);
        var path = Path.Combine(_f.Dir, $"bundle-{root}-{Guid.NewGuid():N}.json");
        BundleJson.Save(bundle, path);
        return path;
    }

    /// <summary>A schema folder naming only the unit field unam, so every other unit field is undeclared.</summary>
    private string SchemaFolder()
    {
        var dir = Path.Combine(_f.Dir, $"schema-{Guid.NewGuid():N}");
        var schema = new GameSchema(1, "smoke-build", "test",
            Array.Empty<SchemaInput>(), Array.Empty<string>(),
            new Dictionary<string, IReadOnlyList<SchemaField>>
            { ["units/unitmetadata.slk"] = new[] { SchemaFieldOf("unam") } },
            null, null);
        GameSchemaCommand.Save(schema, dir);
        return dir;
    }

    [Fact, Covers("port_deps")]
    public async Task Port_deps_groups_the_closure_and_sizes_the_asset()
    {
        var source = SourceMap();
        var r = await Call("port_deps", Args(("map", source), ("rawcode", "H000")));

        Assert.Equal("H000", Prop(r, "rootRawcode").GetString());
        var groups = Rows(Prop(r, "objectGroups"));
        Assert.Contains(groups, g => Prop(g, "kind").GetString() == "unit");
        Assert.Contains(groups, g => Prop(g, "kind").GetString() == "ability");

        var assets = Rows(Prop(r, "assets"));
        var icon = Assert.Single(assets, a => Prop(a, "path").GetString() == @"war3mapImported\smokeicon.blp");
        Assert.True(Prop(icon, "presentInMap").GetBoolean());
        Assert.Equal(8, Prop(icon, "sizeBytes").GetInt32());
    }

    [Fact, Covers("port_preview")]
    public async Task Port_preview_reports_what_a_port_would_do_without_writing()
    {
        var source = SourceMap();
        var before = Sha(Fixture);
        var r = await Call("port_preview", Args(("source_map", source), ("rawcode", "H000"), ("target_map", Fixture)));

        Assert.Equal(before, Sha(Fixture));
        Assert.NotEmpty(Rows(Prop(r, "objects")));
        Assert.Equal("H000", Prop(r, "rootRawcode").GetString());
    }

    [Fact, Covers("port_into_version")]
    public async Task Port_into_version_writes_a_downgraded_copy_and_leaves_the_inputs()
    {
        var source = SourceMap();
        var schema = SchemaFolder();
        var outPath = NewOut("into-version");
        var sourceBefore = Sha(source);
        var targetBefore = Sha(Fixture);

        var r = await Call("port_into_version", Args(
            ("source_map", source), ("rawcode", "H000"), ("target_map", Fixture),
            ("target", schema), ("out_path", outPath), ("include_script", false)));

        Assert.Equal(sourceBefore, Sha(source));
        Assert.Equal(targetBefore, Sha(Fixture));
        Assert.Equal(outPath, Prop(r, "savedTo").GetString());
        Assert.True(File.Exists(outPath));
        // The port only declared unam, so at least one field the unit set was removed.
        Assert.NotEmpty(Rows(Prop(Prop(r, "report"), "fieldsRemoved")));
        await Call("map_info", Args(("map", outPath)));
    }

    [Fact, Covers("bundle_diff")]
    public async Task Bundle_diff_lists_the_shared_and_the_added_member()
    {
        var source = SourceMap();
        var a = BundleFile(source, "H000", root: "H000", ObjectKind.Unit);
        // A second bundle rooted at the ability, so it shares the ability but not the unit.
        var b = BundleFile(source, "A000", root: "A000", ObjectKind.Ability);

        var r = await Call("bundle_diff", Args(("bundle_a", a), ("bundle_b", b)));

        Assert.True(Prop(r, "shared").GetInt32() >= 1);
        // The ability bundle adds nothing the unit bundle lacks (both carry H000 and A000),
        // so the unit bundle's own members are what the ability bundle is missing.
        var removed = Rows(Prop(r, "removed"));
        Assert.Contains(removed, m => Prop(m, "id").GetString() == "H000");
    }

    [Fact, Covers("bundle_merge")]
    public async Task Bundle_merge_writes_a_merged_bundle_and_refuses_a_conflict()
    {
        var source = SourceMap();
        var a = BundleFile(source, "H000", root: "H000", ObjectKind.Unit);
        var b = BundleFile(source, "A000", root: "A000", ObjectKind.Ability);
        var outPath = Path.Combine(_f.Dir, $"merged-{Guid.NewGuid():N}.json");

        var merged = await Call("bundle_merge", Args(("bundle_a", a), ("bundle_b", b), ("out_path", outPath)));
        Assert.Equal(outPath, Prop(merged, "savedTo").GetString());
        Assert.True(File.Exists(outPath));

        // A hand-written bundle whose H000 is an ability, not a unit, conflicts with the first.
        var conflictPath = Path.Combine(_f.Dir, $"conflict-{Guid.NewGuid():N}.json");
        var conflicting = new UnitBundle("H000", "Other",
            new[] { new BundleNode("H000", ObjectKind.Ability, "Other", true) },
            Array.Empty<BundleFile>(), Array.Empty<string>(), Array.Empty<BundleEdge>(),
            Array.Empty<string>(), Array.Empty<BundleFunction>());
        BundleJson.Save(conflicting, conflictPath);

        var message = await CallError("bundle_merge",
            Args(("bundle_a", a), ("bundle_b", conflictPath), ("out_path", NewOut("merge"))));
        Assert.Contains("H000", message);
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static SchemaField SchemaFieldOf(string code) =>
        new(code, "UnitBalance", "HP", "int", "WESTRING", code, "data", 0, 0, 0,
            "", "", "", "", true, true, true, true, true);
}
