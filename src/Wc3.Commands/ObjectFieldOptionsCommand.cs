using Wc3.GameData;

namespace Wc3.Commands;

/// <summary>
/// The editor shape for a single object field: its metadata type token, whether it is a
/// multi-value list, and the enumerated option set with each value's display name. When the
/// game install is unavailable, or the field is genuinely free text or a number, Options is
/// empty and the caller should fall back to a plain text editor.
/// </summary>
public sealed record ObjectFieldOptionsResult(
    string Type, bool IsList, IReadOnlyList<EnumOption> Options, string? Diagnostic = null);

/// <summary>
/// Resolves the option set for a field so a front end can pick a typed editor (dropdown,
/// multiselect or text) without hardcoding Warcraft III's type table.
///
/// Two sources, in this order. The game ships <c>UI\UnitEditorData.txt</c>, which states the
/// closed set for each enumerated field type together with the names the World Editor shows for
/// them. Only when a field's type names no such section does this fall back to collecting the
/// distinct values the base data happens to use.
///
/// The order matters, and it used to be the other way round. Deriving a closed set by observation
/// is wrong in two directions: it misses legal values no stock object uses, and it yields raw
/// tokens rather than names, so a team colour field offered "-1, 0, 1, 2" where the editor offers
/// "Match Owning Player" and the player colours. A value the game defines but nothing happens to
/// use is exactly the value someone opens an editor to set.
/// </summary>
public static class ObjectFieldOptionsCommand
{
    public static ObjectFieldOptionsResult Execute(ObjectKind kind, string fieldCode, string? gameDirOverride)
    {
        if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
            return new ObjectFieldOptionsResult("", false, Array.Empty<EnumOption>(), diagnostic);

        var options = ObjectKinds.FieldOptions(ctx, kind, fieldCode, out var type, out var isList);
        return new ObjectFieldOptionsResult(type, isList, options);
    }
}
