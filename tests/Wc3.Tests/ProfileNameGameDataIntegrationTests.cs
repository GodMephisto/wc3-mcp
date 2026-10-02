using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
namespace Wc3.Tests;

/// <summary>
/// Verifies the Profile TXT wiring against the live install through the normal
/// command path: standard items, abilities, buffs and upgrades resolve human names
/// (previously bare rawcodes, since their name fields live in profile TXTs rather
/// than SLKs). Expected strings confirmed against the install's enus profile files.
/// </summary>
public class ProfileNameGameDataIntegrationTests
{
    private const string Install = @"C:\Warcraft III";

    private static MapDocument EmptyMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>()));

    [Theory]
    [Trait("Category", "GameData")]
    [InlineData("item", "ratf", "Claws of Attack +15")]
    [InlineData("ability", "AHbz", "Blizzard")]
    [InlineData("buff", "BSTN", "Stunned")]
    [InlineData("upgrade", "Rhme", "Iron Forged Swords")]
    public void Standard_profile_backed_objects_resolve_names(string kind, string rawcode, string expected)
    {
        if (!Directory.Exists(Install)) return;
        var got = ObjectGetCommand.Execute(EmptyMap(), ObjectKinds.Parse(kind), rawcode, Install);
        Assert.True(got.Found, $"{kind} {rawcode} should resolve");
        Assert.Equal(expected, got.Name);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Item_tooltip_and_description_resolve_from_profile()
    {
        if (!Directory.Exists(Install)) return;
        var got = ObjectGetCommand.Execute(EmptyMap(), ObjectKind.Item, "ratf", Install);
        var fields = got.Fields.ToDictionary(f => f.Code, f => f, StringComparer.OrdinalIgnoreCase);
        Assert.Equal("Purchase Claws of Attack +15", fields["utip"].Value);
        Assert.Equal("Boosts attack damage by 15.", fields["ides"].Value);
        Assert.Equal("base", fields["unam"].Source);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Leveled_upgrade_names_expand_per_level()
    {
        if (!Directory.Exists(Install)) return;
        var got = ObjectGetCommand.Execute(EmptyMap(), ObjectKind.Upgrade, "Rhme", Install);
        var fields = got.Fields.ToDictionary(f => f.Code, f => f.Value, StringComparer.OrdinalIgnoreCase);
        Assert.Equal("Iron Forged Swords", fields["gnam"]);
        Assert.Equal("Iron Forged Swords", fields["gnam:1"]);
        Assert.Equal("Steel Forged Swords", fields["gnam:2"]);
        Assert.Equal("Mithril Forged Swords", fields["gnam:3"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Object_list_resolves_item_base_name_from_profile()
    {
        if (!Directory.Exists(Install)) return;
        // A modified standard item with no name delta: the list's name must come
        // from the base game's profile data.
        var w3t = new ItemObjectData(ObjectDataFormatVersion.v2);
        var item = new SimpleObjectModification { OldId = "ratf".FromRawcode() };
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "ilev".FromRawcode(), Type = ObjectDataType.Int, Value = 5 });
        w3t.BaseItems.Add(item);
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.w3t"] = Serialize(w => w.Write(w3t)) }));

        var listed = ObjectListCommand.Execute(doc, ObjectKind.Item, Install);
        var row = Assert.Single(listed.Items);
        Assert.Equal("Claws of Attack +15", row.Name);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Unit_and_doodad_names_still_resolve()
    {
        if (!Directory.Exists(Install)) return;
        var unit = ObjectGetCommand.Execute(EmptyMap(), ObjectKind.Unit, "hfoo", Install);
        Assert.Equal("Footman", unit.Name);
        var doodad = ObjectGetCommand.Execute(EmptyMap(), ObjectKind.Doodad, "AOhs", Install);
        Assert.Equal("Stump Hollow", doodad.Name);
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
