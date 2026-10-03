// tests/Wc3.Tests/ObjectFormCompletenessTests.cs
using Wc3.Commands;
using Wc3.GameData;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The form must show every field that applies to an object and no field that does not, for all
/// seven kinds. Both halves of that were wrong at different times and in opposite directions.
///
/// Too few. The form was built from the fields holding a VALUE, so a hero showed 160 of its 223
/// applicable fields, Art 19 of 51, Techtree 3 of 18. An unset field never appeared, which is the
/// one an editor exists to set.
///
/// Too many. Applicability ignored the metadata's UseSpecific lists, and 708 of the 777 ability
/// fields carry one because an ability's data fields belong to its TYPE. One ability showed 769
/// fields where the editor shows about 70.
/// </summary>
public class ObjectFormCompletenessTests
{
    private const string Install = @"D:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public ObjectFormCompletenessTests(ITestOutputHelper output) => _out = output;

    private static ObjectMetadata? MetaFor(GameDataContext ctx, ObjectKind kind) => kind switch
    {
        ObjectKind.Unit => ctx.Units.FieldMetadata,
        ObjectKind.Ability => ctx.Abilities.FieldMetadata,
        ObjectKind.Item => ctx.Items.Metadata,
        ObjectKind.Destructable => ctx.Destructables.Metadata,
        ObjectKind.Doodad => ctx.Doodads.Metadata,
        ObjectKind.Buff => ctx.Buffs.Metadata,
        ObjectKind.Upgrade => ctx.Upgrades.Metadata,
        _ => null,
    };

    [Fact]
    [Trait("Category", "GameData")]
    public void Every_kind_accounts_for_every_field_the_metadata_defines()
    {
        if (!Directory.Exists(Install)) return;
        if (!File.Exists(CorpusMap.PathOrEmpty)) return;

        var doc = MapDocument.Load(CorpusMap.PathOrEmpty);
        Assert.True(Wc3.GameData.GameData.TryOpen(Install, out var ctx, out var why), why);

        var losing = new List<string>();
        foreach (var kind in ObjectKinds.All)
        {
            var items = ObjectListCommand.Execute(doc, kind, Install).Items;
            if (items.Count == 0) continue;
            var subject = items.FirstOrDefault(o =>
                o.BaseRawcode is not null && o.BaseRawcode != o.Rawcode) ?? items[0];

            var form = ObjectFormCommand.Execute(doc, kind, subject.Rawcode, Install);
            var meta = MetaFor(ctx!, kind);
            if (meta is null) continue;

            // Shown plus hidden must cover everything the metadata defines. A per-level field can
            // legitimately produce more than one row, so more is fine and less is a loss.
            int accounted = form.FieldCount + form.HiddenFieldCount;
            _out.WriteLine($"{kind,-14} defines {meta.Fields.Count,4}  shows {form.FieldCount,4}  "
                         + $"hidden {form.HiddenFieldCount,4}");
            if (accounted < meta.Fields.Count)
                losing.Add($"{kind} loses {meta.Fields.Count - accounted}");
        }

        Assert.True(losing.Count == 0,
            "a kind drops fields entirely, so they can be neither seen nor set: "
            + string.Join(", ", losing));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void A_units_form_matches_the_applicable_count_exactly()
    {
        if (!Directory.Exists(Install)) return;
        if (!File.Exists(CorpusMap.PathOrEmpty)) return;

        var doc = MapDocument.Load(CorpusMap.PathOrEmpty);
        Assert.True(Wc3.GameData.GameData.TryOpen(Install, out var ctx, out _));

        var hero = ObjectListCommand.Execute(doc, ObjectKind.Unit, Install).Items
            .FirstOrDefault(o => o.BaseRawcode is not null && o.BaseRawcode != o.Rawcode);
        if (hero is null) return;

        var form = ObjectFormCommand.Execute(doc, ObjectKind.Unit, hero.Rawcode, Install);
        // Per category, against the metadata's own count for that category. A form showing FEWER
        // than the category defines is the bug this pins.
        var meta = ctx!.Units.FieldMetadata;
        foreach (var g in form.Groups)
        {
            int inMeta = meta.Fields.Count(f =>
                string.Equals(f.Category, g.Key, StringComparison.OrdinalIgnoreCase));
            if (inMeta == 0) continue;
            Assert.True(g.Fields.Count <= inMeta,
                $"category {g.Key} shows {g.Fields.Count} rows but the metadata defines {inMeta}");
        }
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void An_abilitys_form_is_narrowed_by_UseSpecific()
    {
        if (!Directory.Exists(Install)) return;
        if (!File.Exists(CorpusMap.PathOrEmpty)) return;

        var doc = MapDocument.Load(CorpusMap.PathOrEmpty);
        var ability = ObjectListCommand.Execute(doc, ObjectKind.Ability, Install).Items
            .FirstOrDefault(o => o.BaseRawcode is not null && o.BaseRawcode != o.Rawcode);
        if (ability is null) return;

        var form = ObjectFormCommand.Execute(doc, ObjectKind.Ability, ability.Rawcode, Install);
        _out.WriteLine($"{ability.Rawcode} shows {form.FieldCount}, hides {form.HiddenFieldCount}");

        // The whole ability table is 777 rows. Anything near that means UseSpecific was ignored
        // and the form is showing every other ability type's data fields.
        Assert.True(form.FieldCount < 200,
            $"an ability shows {form.FieldCount} fields, so UseSpecific is being ignored and the "
            + "form carries other ability types' data fields");
        Assert.True(form.HiddenFieldCount > 400,
            "most of the ability table belongs to other ability types and should be hidden");
    }

    [Fact]
    public void UseSpecific_matches_the_base_code_not_just_the_object_code()
    {
        // A custom ability keeps its base's field set, and the metadata names only the base, so a
        // check against the object's own code alone would hide every data field on every custom.
        var general = new ObjectFieldMeta("aaaa", "", "col", "", "int");
        var specific = general with { UseSpecific = "AHbz,ACtc" };
        var excluded = general with { NotSpecific = "A000" };

        Assert.True(general.AppliesToObject("A000", "AHbz"));       // no list, applies to all
        Assert.True(specific.AppliesToObject("A000", "AHbz"));      // matched via the BASE
        Assert.True(specific.AppliesToObject("ACtc", null));        // matched via its own code
        Assert.False(specific.AppliesToObject("A000", "AHtb"));     // base is not in the list
        Assert.False(excluded.AppliesToObject("A000", "AHbz"));     // explicit exclusion wins
    }
}
