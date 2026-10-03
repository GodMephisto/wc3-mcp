// tests/Wc3.Tests/ObjectFormGameDataTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The form against the retail metadata, which is where the grouping, the ordering, the bounds
/// and the authoritative layer actually come from. Needs a Warcraft III install.
/// </summary>
public class ObjectFormGameDataTests
{
    private const string Install = @"D:\Warcraft III";
    private static readonly string Map = TestCorpus.Map(@"GGGA_V0.04g.w3x");

    private static ObjectForm? Form(string rawcode, ObjectKind kind)
    {
        if (!Directory.Exists(Install) || !File.Exists(Map)) return null;
        return ObjectFormCommand.Execute(MapDocument.Load(Map), kind, rawcode, Install);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void A_unit_comes_back_grouped_the_way_the_editor_groups_it()
    {
        if (Form("H006", ObjectKind.Unit) is not { } form) return;

        var keys = form.Groups.Select(g => g.Key).ToList();
        Assert.Contains("art", keys);
        Assert.Contains("combat", keys);
        Assert.Contains("stats", keys);
        // A form, not a dump. Several groups, none of them holding everything.
        Assert.True(form.Groups.Count >= 5, $"expected several groups, got {form.Groups.Count}");
        Assert.All(form.Groups, g => Assert.True(g.Fields.Count < form.FieldCount,
            $"group {g.Key} holds every field, so nothing was actually grouped"));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Inapplicable_fields_are_hidden_rather_than_listed()
    {
        if (Form("H006", ObjectKind.Unit) is not { } form) return;

        // The kind defines far more fields than any one object uses, and the metadata says which
        // apply. Hiding the rest is most of the difference between a dump and a form.
        Assert.True(form.HiddenFieldCount > 0,
            "no field was hidden, so the applicability flags are not being read");
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void The_layer_is_authoritative_and_matches_the_measured_split()
    {
        if (Form("H006", ObjectKind.Unit) is not { } form) return;

        var byCode = form.Groups.SelectMany(g => g.Fields)
            .ToDictionary(f => f.Code, StringComparer.OrdinalIgnoreCase);

        // umdl is netsafe=1, so the metadata answers it rather than the fallback measurement.
        Assert.True(byCode.TryGetValue("umdl", out var model), "the working hero should carry a model");
        Assert.Equal(ObjectLayer.Skin, model!.Layer);
        Assert.True(model.LayerIsAuthoritative);

        Assert.True(byCode.TryGetValue("uhpm", out var hp));
        Assert.Equal(ObjectLayer.Map, hp!.Layer);
        Assert.True(hp.LayerIsAuthoritative);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Numeric_bounds_arrive_and_reject_an_out_of_range_edit()
    {
        if (Form("H006", ObjectKind.Unit) is not { } form) return;

        var bounded = form.Groups.SelectMany(g => g.Fields)
            .Where(f => f.MaxValue is not null).ToList();
        Assert.NotEmpty(bounded);

        var f = bounded[0];
        var over = (double.Parse(f.MaxValue!, System.Globalization.CultureInfo.InvariantCulture) + 1)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        Assert.NotNull(f.Validate(over));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void An_ability_groups_too_and_carries_its_own_categories()
    {
        if (Form("A051", ObjectKind.Ability) is not { } form) return;
        Assert.True(form.Groups.Count >= 3, $"expected several groups, got {form.Groups.Count}");
        Assert.Contains(form.Groups, g => g.Key is "art" or "stats" or "data");
    }
}
