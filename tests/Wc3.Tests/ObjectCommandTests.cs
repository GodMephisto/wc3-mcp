// tests/Wc3.Tests/ObjectCommandTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
namespace Wc3.Tests;

public class ObjectCommandTests
{
    [Fact]
    public void Merges_base_and_delta_with_source_labels()
    {
        // No real map needed for the merge core: exercise the pure Merge with fakes.
        var baseFields = new Dictionary<string, string> { ["unam"] = "Footman", ["uhpm"] = "420" };
        var result = ObjectGetCommand.Merge(
            rawcode: "H001", baseRawcode: "hfoo", definedInMap: true, name: "Super Footman",
            baseFields: baseFields,
            deltaFields: new Dictionary<string, string> { ["uhpm"] = "999" },
            nameLookup: code => code == "unam" ? "Name" : code == "uhpm" ? "Hit Points" : code,
            diagnostics: Array.Empty<string>());

        Assert.True(result.Found);
        Assert.Equal("hfoo", result.BaseRawcode);
        Assert.Equal("Super Footman", result.Name);
        var hp = result.Fields.Single(f => f.Code == "uhpm");
        Assert.Equal("999", hp.Value);
        Assert.Equal("map", hp.Source);
        Assert.Equal("base", result.Fields.Single(f => f.Code == "unam").Source);
        // Base-only field retained; output ordered by display name.
        Assert.Equal(new[] { "Hit Points", "Name" }, result.Fields.Select(f => f.Name).ToArray());
    }

    [Fact]
    public void Map_defined_unit_with_zero_fields_is_still_found()
    {
        // Base-less custom unit with no mods and no base data: presence in the
        // map's w3u alone must yield Found=true.
        var result = ObjectGetCommand.Merge(
            rawcode: "H001", baseRawcode: null, definedInMap: true, name: null,
            baseFields: new Dictionary<string, string>(),
            deltaFields: new Dictionary<string, string>(),
            nameLookup: code => code,
            diagnostics: Array.Empty<string>());

        Assert.True(result.Found);
        Assert.Null(result.BaseRawcode);
        Assert.Null(result.Name);
        Assert.Empty(result.Fields);
    }

    [Fact]
    public void Unknown_rawcode_with_zero_fields_is_not_found()
    {
        var result = ObjectGetCommand.Merge(
            rawcode: "Xxxx", baseRawcode: null, definedInMap: false, name: null,
            baseFields: new Dictionary<string, string>(),
            deltaFields: new Dictionary<string, string>(),
            nameLookup: code => code,
            diagnostics: Array.Empty<string>());

        Assert.False(result.Found);
        Assert.Empty(result.Fields);
    }

    [Fact]
    public void Leveled_ability_delta_keys_order_numerically_within_a_field()
    {
        // Ability deltas key per-level values as "code:N"; same display name for
        // every level, so ordering must fall through to the numeric level (10 > 2).
        var result = ObjectGetCommand.Merge(
            rawcode: "A000", baseRawcode: "ANcl", definedInMap: true, name: "Custom Spell",
            baseFields: new Dictionary<string, string>(),
            deltaFields: new Dictionary<string, string>
            {
                ["Ncl4:10"] = "1.0",
                ["Ncl4:2"] = "0.2",
                ["Ncl4:1"] = "0.1",
                ["alev"] = "10",
            },
            nameLookup: code => code.StartsWith("Ncl4") ? "Casting Time" : "Levels",
            diagnostics: Array.Empty<string>());

        Assert.Equal(new[] { "Ncl4:1", "Ncl4:2", "Ncl4:10", "alev" }, result.Fields.Select(f => f.Code).ToArray());
        Assert.Equal("Custom Spell", result.Name);
    }

    [Fact]
    public void Ability_deltas_resolve_from_w3a_with_leveled_keys_and_anam_name()
    {
        // Hermetic full pipeline (ctx=null → deltas-only): a synthetic map whose w3a
        // defines A000 (base ANcl) with a non-leveled anam and a level-2 aran.
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ability = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A000".FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Fireball", Level = 0, Pointer = 0 });
        ability.Modifications.Add(new LevelObjectDataModification
        { Id = "aran".FromRawcode(), Type = ObjectDataType.Unreal, Value = 600f, Level = 2, Pointer = 0 });
        w3a.NewAbilities.Add(ability);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.w3a"] = Serialize(w => w.Write(w3a)) }));
        var result = ObjectGetCommand.Execute(doc, "A000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.True(result.Found);
        Assert.Equal("ANcl", result.BaseRawcode);
        Assert.Equal("Fireball", result.Name);
        Assert.Equal("600", result.Fields.Single(f => f.Code == "aran:2").Value);
        Assert.All(result.Fields, f => Assert.Equal("map", f.Source));
    }

    [Fact]
    public void Unit_name_resolves_from_map_unam_delta_without_game_data()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Paladin" });
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.w3u"] = Serialize(w => w.Write(w3u)) }));

        var got = ObjectGetCommand.Execute(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.Equal("Dark Paladin", got.Name);
        Assert.Equal("Hpal", got.BaseRawcode);

        var listed = ObjectListCommand.Execute(doc, gameDirOverride: "Z:\\no_such");
        var item = Assert.Single(listed.Items);
        Assert.Equal(new ObjectListItem("H000", "Hpal", "Dark Paladin"), item);
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
