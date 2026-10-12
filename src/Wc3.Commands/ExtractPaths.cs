// src/Wc3.Commands/ExtractPaths.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Maps an internal archive name to a safe relative path under an output directory. This is the
/// one path-safety rule, shared by the CLI extract writer, the MCP extract tool and the Studio
/// files panel, so all three refuse the same escapes rather than each re-implementing the check.
///
/// A name is split on '\' and '/', '.' and '..' and empty segments are dropped (defeating a
/// traversal), invalid filename characters are replaced, and trailing dots and spaces are trimmed
/// because Windows silently rejects them. A name that reduces to nothing, and an unnamed entry,
/// go to <c>_unnamed\block_&lt;n&gt;.&lt;ext&gt;</c>.
/// </summary>
public static class ExtractPaths
{
    public static string SafeRelativePath(string? name, int blockIndex, string unnamedExtension = "bin")
    {
        if (name is null) return Path.Combine("_unnamed", $"block_{blockIndex}.{unnamedExtension}");
        var segments = name.Split('\\', '/')
            .Where(s => s.Length > 0 && s != "." && s != "..")
            .Select(Sanitize)
            .Where(s => s.Length > 0)
            .ToArray();
        return segments.Length == 0
            ? Path.Combine("_unnamed", $"block_{blockIndex}.bin")
            : Path.Combine(segments);
    }

    /// <summary>The relative path for one extracted item, sniffing an unnamed entry's extension
    /// from its bytes so a nameless BLP lands as a .blp rather than an anonymous .bin.</summary>
    public static string RelativePathFor(ExtractedItem item) =>
        SafeRelativePath(item.Name, item.BlockIndex,
            item.Name is null ? ContentTypeSniffer.Sniff(item.Bytes).Extension : "bin");

    private static string Sanitize(string segment)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(segment.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return cleaned.TrimEnd(' ', '.');   // Windows rejects trailing dots/spaces
    }
}
