// tests/Wc3.Tests/EditorEnumDataTests.cs
using Wc3.Commands;
using Wc3.GameData;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The enumerated field types come from the game's own UI\UnitEditorData.txt, which states both
/// which values a field may take and what each is called. Options used to be derived by collecting
/// the distinct values the base data happened to use, which missed legal values nothing used and
/// produced raw tokens instead of names.
/// </summary>
public class EditorEnumDataTests
{
    private const string Install = @"D:\Warcraft III";

    private static EditorEnumData Parse(string text) =>
        EditorEnumData.FromByteSources(WorldEditStrings.FromByteSources(null, null),
            System.Text.Encoding.UTF8.GetBytes(text));

    [Fact]
    public void A_section_becomes_an_ordered_option_set()
    {
        var data = Parse("[attackType]\n00=unknown,WESTRING_NONE\n01=normal,WESTRING_A\n"
                       + "02=pierce,WESTRING_B\nNumValues=3\n");

        Assert.True(data.TryGet("attackType", out var options));
        Assert.Equal(new[] { "unknown", "normal", "pierce" }, options.Select(o => o.Value));
    }

    [Fact]
    public void The_editors_own_order_is_preserved_rather_than_sorted()
    {
        // The order carries meaning. "None" first, then the types in the editor's sequence. Sorting
        // would put chaos before normal, which is not how anyone reads that list.
        var data = Parse("[t]\n00=zulu,W_A\n01=alpha,W_B\n02=mike,W_C\n");

        Assert.True(data.TryGet("t", out var options));
        Assert.Equal(new[] { "zulu", "alpha", "mike" }, options.Select(o => o.Value));
    }

    [Fact]
    public void Section_bookkeeping_is_not_an_option()
    {
        // NumValues and Sort describe the section. _Alt entries are alternate labels the editor
        // skips. Any of them appearing as a selectable value would be a value the game rejects.
        var data = Parse("[t]\nSort=1\n00=real,W_A\n00_Alt=bogus,W_B\nNumValues=1\n");

        Assert.True(data.TryGet("t", out var options));
        Assert.Equal(new[] { "real" }, options.Select(o => o.Value));
    }

    [Fact]
    public void An_unresolved_display_key_falls_back_to_the_value()
    {
        // Some values genuinely have no shipped label. A raw token is more use than a blank row,
        // and leaking a WESTRING key into a dropdown is worse than either.
        var data = Parse("[t]\n00=foot,WESTRING_DOES_NOT_EXIST\n");

        Assert.True(data.TryGet("t", out var options));
        Assert.Equal("foot", options[0].Value);
        Assert.Equal("foot", options[0].Label);
        Assert.DoesNotContain("WESTRING", options[0].Label);
    }

    [Fact]
    public void A_type_with_no_section_is_not_enumerated()
    {
        var data = Parse("[attackType]\n00=normal,W_A\n");

        Assert.False(data.TryGet("uhpm", out var none));
        Assert.Empty(none);
        Assert.False(data.TryGet("", out _));
    }

    [Fact]
    public void Empty_is_safe_to_query()
    {
        Assert.False(EditorEnumData.Empty.TryGet("attackType", out var options));
        Assert.Empty(options);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void The_retail_install_supplies_named_options_for_real_fields()
    {
        if (!Directory.Exists(Install)) return;

        // Attack type is the clearest case: eight values, each with a real name.
        var r = ObjectFieldOptionsCommand.Execute(ObjectKind.Unit, "ua1t", Install);

        Assert.Equal("attackType", r.Type);
        Assert.Contains(r.Options, o => o.Value == "normal" && o.DisplayName == "Normal");
        Assert.Contains(r.Options, o => o.Value == "hero");
        // Every option must carry a label, whether shipped or fallen back to its own value.
        Assert.All(r.Options, o => Assert.False(string.IsNullOrEmpty(o.Label)));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Primary_attribute_resolves_to_the_names_a_person_recognises()
    {
        if (!Directory.Exists(Install)) return;

        var r = ObjectFieldOptionsCommand.Execute(ObjectKind.Unit, "upra", Install);

        Assert.Contains(r.Options, o => o.Value == "STR" && o.DisplayName == "Strength");
        Assert.Contains(r.Options, o => o.Value == "AGI" && o.DisplayName == "Agility");
        Assert.Contains(r.Options, o => o.Value == "INT" && o.DisplayName == "Intelligence");
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void A_free_text_field_offers_nothing_rather_than_guessing()
    {
        if (!Directory.Exists(Install)) return;

        // Hit points is a number with bounds, not a closed set. Offering a dropdown of every value
        // some stock unit happens to have would be noise.
        var r = ObjectFieldOptionsCommand.Execute(ObjectKind.Unit, "uhpm", Install);

        Assert.DoesNotContain(r.Options, o => o.Value.Contains(','));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void No_field_type_needs_a_WorldEditData_section()
    {
        // The load path used to read WorldEditData.txt through the ENUM lens as well as the
        // catalog one, which misparsed most of its sections. That was harmless only because no
        // object metadata field type, across any of the seven kinds, names a section that lives
        // in WorldEditData rather than UnitEditorData, so nothing ever looked one up.
        //
        // The ingestion is gone. This pins the measurement it rested on, so if a patch introduces
        // a field type served only by a WorldEditData section, this fails and says so rather than
        // the option set quietly coming back empty.
        if (!Directory.Exists(Install)) return;

        Assert.True(Wc3.GameData.GameData.TryOpen(Install, out var ctx, out var why), why);

        var catalogOnly = ctx!.EditorCatalogs.Names
            .Where(n => !ctx.EditorEnums.TryGet(n, out _))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.NotEmpty(catalogOnly);   // the catalogs must actually be distinct from the enums

        var offenders = new List<string>();
        foreach (var kind in ObjectKinds.All)
        {
            var meta = ObjectKinds.FieldMeta(ctx, kind);
            if (meta is null) continue;
            foreach (var f in meta.Fields)
                if (f.Type.Length > 0 && catalogOnly.Contains(f.Type))
                    offenders.Add($"{kind} field {f.Code} has type '{f.Type}'");
        }

        Assert.True(offenders.Count == 0,
            "a field type is served only by a WorldEditData catalog, so its options now come back "
            + "empty. Serve that type from EditorCatalogs in ObjectKinds.FieldOptions. Offenders: "
            + string.Join(", ", offenders));
    }
}
