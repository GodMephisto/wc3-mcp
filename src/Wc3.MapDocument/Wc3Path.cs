// src/Wc3.MapDocument/Wc3Path.cs
namespace Wc3.Model;

/// <summary>
/// Path operations for names that live inside a map archive, which use the backslash as their
/// separator on every platform.
/// </summary>
/// <remarks>
/// <see cref="System.IO.Path"/> treats only '/' as a separator when it is not running on Windows,
/// so <c>Path.GetFileName(@"ReplaceableTextures\CommandButtons\BTNFoo.blp")</c> returns the whole
/// string on macOS and Linux rather than the file name. Every operation built on that then quietly
/// does nothing, an icon sibling is never generated, a stem is never separated from its extension,
/// a sibling model file is never looked for. None of it throws, the asset is just reported absent
/// while it sits in the archive.
///
/// Measured on macOS with the full hermetic suite, fifteen tests failed and the ones that were not
/// environment dependent all traced here, the asset candidate expansion, the sound label, the
/// model export names and the replay map match.
///
/// These are used for archive-internal names only. A real filesystem path still belongs to
/// <see cref="System.IO.Path"/>, which is correct for the platform it is running on.
/// </remarks>
public static class Wc3Path
{
    private static bool IsSeparator(char c) => c == '\\' || c == '/';

    /// <summary>The text after the last separator, or the whole string when there is none.</summary>
    public static string GetFileName(string? path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        int i = path.Length;
        while (i > 0 && !IsSeparator(path[i - 1])) i--;
        return path[i..];
    }

    /// <summary>Everything up to and including the last separator, or an empty string.</summary>
    public static string GetDirectoryPrefix(string? path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        int i = path.Length;
        while (i > 0 && !IsSeparator(path[i - 1])) i--;
        return path[..i];
    }

    /// <summary>
    /// Everything before the last separator, without a trailing one, or an empty string. Matches
    /// <see cref="System.IO.Path.GetDirectoryName(string)"/> for a rooted archive name.
    /// </summary>
    public static string GetDirectoryName(string? path)
    {
        var prefix = GetDirectoryPrefix(path);
        return prefix.Length == 0 ? "" : prefix.TrimEnd('\\', '/');
    }

    /// <summary>
    /// The extension of the last segment, including the dot, or an empty string. A dot in a
    /// directory is not an extension, exactly as <see cref="System.IO.Path.GetExtension(string)"/>.
    /// </summary>
    public static string GetExtension(string? path)
    {
        var name = GetFileName(path);
        int dot = name.LastIndexOf('.');
        return dot <= 0 ? "" : name[dot..];
    }

    /// <summary>The last segment without its extension.</summary>
    public static string GetFileNameWithoutExtension(string? path)
    {
        var name = GetFileName(path);
        int dot = name.LastIndexOf('.');
        return dot <= 0 ? name : name[..dot];
    }

    /// <summary>Replace the extension of the last segment, or append one when it has none.</summary>
    public static string ChangeExtension(string? path, string? extension)
    {
        if (string.IsNullOrEmpty(path)) return "";
        var ext = extension ?? "";
        if (ext.Length > 0 && ext[0] != '.') ext = "." + ext;

        var name = GetFileName(path);
        int dot = name.LastIndexOf('.');
        var stem = dot <= 0 ? name : name[..dot];
        return GetDirectoryPrefix(path) + stem + ext;
    }
}
