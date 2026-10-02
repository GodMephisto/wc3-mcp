namespace Wc3.Commands;

/// <summary>
/// The editor shape for a single object field: its metadata type token, whether it is a
/// multi-value list, and the enumerated option set (distinct base-data tokens). When the
/// game install is unavailable, or the field is free-text, Options is empty and the
/// caller should fall back to a plain text editor.
/// </summary>
public sealed record ObjectFieldOptionsResult(
    string Type, bool IsList, IReadOnlyList<string> Options, string? Diagnostic = null);

/// <summary>
/// Resolves the option set for a field so the UI can pick a typed editor (dropdown /
/// multiselect / text) without hard-coding WC3's type table. Options are derived from the
/// live base data, so they are correct-by-construction — a picker can only ever write a
/// token the game already uses.
/// </summary>
public static class ObjectFieldOptionsCommand
{
    public static ObjectFieldOptionsResult Execute(ObjectKind kind, string fieldCode, string? gameDirOverride)
    {
        if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
            return new ObjectFieldOptionsResult("", false, Array.Empty<string>(), diagnostic);

        ObjectKinds.TryGetFieldOptions(ctx, kind, fieldCode, out var type, out var isList, out var options);
        return new ObjectFieldOptionsResult(type, isList, options);
    }
}
