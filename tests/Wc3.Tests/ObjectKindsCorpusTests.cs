// tests/Wc3.Tests/ObjectKindsCorpusTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Real-map coverage for the multi-kind object commands: the Anime map carries
/// custom item (w3t) and ability (w3a) object data, so listing those kinds must
/// return real entries and their fields must merge. Skips when the map is absent.
/// </summary>
public class ObjectKindsCorpusTests
{
    private static readonly string MapPath =
        TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_lists_units_despite_null_wts_entries()
    {
        // Regression: this map's war3map.wts parses one entry (key 0) with a null
        // value; unit unam deltas referencing it made MapStrings.Resolve throw.
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);
        var units = ObjectListCommand.Execute(doc, ObjectKind.Unit, gameDirOverride: null);
        Assert.NotEmpty(units.Items);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_lists_custom_items_and_abilities()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);

        var items = ObjectListCommand.Execute(doc, ObjectKind.Item, gameDirOverride: null);
        Assert.NotEmpty(items.Items);

        var abilities = ObjectListCommand.Execute(doc, ObjectKind.Ability, gameDirOverride: null);
        Assert.NotEmpty(abilities.Items);

        // A listed ability's merged view must be Found with at least one map delta.
        var sample = abilities.Items[0].Rawcode;
        var got = ObjectGetCommand.Execute(doc, ObjectKind.Ability, sample, gameDirOverride: null);
        Assert.True(got.Found, $"listed ability {sample} should merge");
        Assert.Contains(got.Fields, f => f.Source == "map");
    }
}
