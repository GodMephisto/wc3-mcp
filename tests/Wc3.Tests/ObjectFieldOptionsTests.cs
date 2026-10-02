// tests/Wc3.Tests/ObjectFieldOptionsTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The typed-editor option resolver derives a field's editor shape — type token, list-ness,
/// and the enumerated option set — from live base data, so a picker can only ever write a
/// token the game already uses (correct-by-construction). These lock the invariants the UI
/// relies on: no context degrades to free text, scalar enums are distinct + sorted, and
/// *List types are split on commas. Install-backed cases skip when the game is absent.
/// </summary>
public class ObjectFieldOptionsTests
{
    private const string Install = @"C:\Warcraft III";

    // Pure unit test (no install needed): a null context can resolve nothing, so the caller
    // must be told to fall back to a plain text editor — empty options, not a crash.
    [Fact]
    public void Null_context_yields_free_text_fallback()
    {
        var ok = ObjectKinds.TryGetFieldOptions(
            null, ObjectKind.Item, "icla", out var type, out var isList, out var options);

        Assert.False(ok);
        Assert.Equal("", type);
        Assert.False(isList);
        Assert.Empty(options);
    }

    // icla (item "Class") is an enumerated scalar (metadata type "itemClass", not a *List):
    // the option set must be non-empty, comma-free, distinct, and sorted ordinal-ignore-case.
    [Fact]
    [Trait("Category", "GameData")]
    public void Item_class_options_are_enumerated_distinct_and_sorted()
    {
        if (!Directory.Exists(Install)) return;
        var result = ObjectFieldOptionsCommand.Execute(ObjectKind.Item, "icla", gameDirOverride: Install);

        Assert.Null(result.Diagnostic);
        Assert.False(result.IsList, "itemClass is a scalar enum, not a *List type");
        Assert.NotEmpty(result.Options);
        Assert.DoesNotContain(result.Options, o => o.Contains(','));
        Assert.Equal(
            result.Options.OrderBy(o => o, StringComparer.OrdinalIgnoreCase).ToList(),
            result.Options);
        Assert.Equal(
            result.Options.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            result.Options.Count);
    }

    // btar (destructable "Targeted As") is a targetList: IsList must be reported true, and
    // every returned option is a single token from the split — never a comma-joined value.
    [Fact]
    [Trait("Category", "GameData")]
    public void Destructable_targeted_as_reports_split_list()
    {
        if (!Directory.Exists(Install)) return;
        var result = ObjectFieldOptionsCommand.Execute(ObjectKind.Destructable, "btar", gameDirOverride: Install);

        Assert.True(result.IsList, "targetList fields must report IsList");
        Assert.DoesNotContain(result.Options, o => o.Contains(','));
    }

    // A bogus field code has no metadata and no base values: the open still succeeds, so
    // this is a graceful "free text" signal (empty options), not an error.
    [Fact]
    [Trait("Category", "GameData")]
    public void Unknown_field_code_falls_back_to_empty_options()
    {
        if (!Directory.Exists(Install)) return;
        var result = ObjectFieldOptionsCommand.Execute(ObjectKind.Item, "zzzz", gameDirOverride: Install);

        Assert.Empty(result.Options);
    }
}
