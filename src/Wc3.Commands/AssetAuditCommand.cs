// src/Wc3.Commands/AssetAuditCommand.cs
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>Why an asset was flagged.</summary>
public enum AssetAuditKind
{
    /// <summary>Bigger than the size threshold.</summary>
    Oversized,
    /// <summary>A BLP texture whose width or height is not a power of two.</summary>
    NonPowerOfTwoTexture,
    /// <summary>A binary .mdx with no .mdl source beside it.</summary>
    ModelWithoutSource,
    /// <summary>A zero-byte entry.</summary>
    Empty,
}

/// <summary>One flagged asset and the reason, with a short human detail.</summary>
public sealed record AssetAuditFinding(string Path, AssetAuditKind Kind, string Detail);

/// <summary>
/// Suspicious imported assets, grouped by kind. <see cref="MaxBytes"/> is the oversized
/// threshold actually applied. Read-only.
/// </summary>
public sealed record AssetAuditResult(
    IReadOnlyList<AssetAuditFinding> Findings,
    int Scanned,
    long MaxBytes,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Flags imported assets worth a second look. Four checks, each one a shape that has cost a map
/// real load time or a real render.
///
/// An entry above the size threshold, since one oversized texture or sound dominates a map's
/// weight. A BLP texture whose width or height is not a power of two, which the engine pads or
/// refuses and which usually means a resized export slipped in. A binary .mdx with no matching
/// .mdl beside it, the tell for a half-imported model. And a zero-byte entry, which the game
/// fails to load with no message of its own.
///
/// Texture dimensions are read by decoding the header through <see cref="BlpDecoder"/>. That
/// reads the whole texture, which is heavier than the rest of the scan, so an unreadable
/// texture is reported in <see cref="AssetAuditResult.Diagnostics"/> rather than failing the run.
/// </summary>
public static class AssetAuditCommand
{
    /// <summary>Default oversized threshold, 4 MiB, above which a single import dominates a map.</summary>
    public const long DefaultMaxBytes = 4 * 1024 * 1024;

    public static AssetAuditResult Execute(MapDocument doc, long maxBytes = DefaultMaxBytes)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entries = AssetReferences.ImportedEntries(doc);
        var names = entries.Select(e => AssetReferences.Normalize(e.FileName!))
            .ToHashSet(StringComparer.Ordinal);
        var findings = new List<AssetAuditFinding>();
        var diagnostics = new List<string>();

        foreach (var entry in entries)
        {
            var name = entry.FileName!;
            long size = entry.CurrentSize;

            if (size == 0)
            {
                findings.Add(new AssetAuditFinding(name, AssetAuditKind.Empty, "zero-byte entry"));
                continue;
            }
            if (size > maxBytes)
                findings.Add(new AssetAuditFinding(name, AssetAuditKind.Oversized,
                    $"{size} bytes, over the {maxBytes}-byte threshold"));

            var stem = Wc3Path.GetFileNameWithoutExtension(name);
            var ext = Wc3Path.GetExtension(name).ToLowerInvariant();
            if (ext == ".blp")
            {
                if (TryTextureSize(entry, out int width, out int height))
                {
                    if (!IsPowerOfTwo(width) || !IsPowerOfTwo(height))
                        findings.Add(new AssetAuditFinding(name, AssetAuditKind.NonPowerOfTwoTexture,
                            $"{width}x{height} is not a power of two"));
                }
                else
                {
                    diagnostics.Add($"could not read the texture dimensions of '{name}'");
                }
            }
            else if (ext == ".mdx")
            {
                var directory = AssetReferences.FolderOf(name);
                var prefix = directory == "(root)" ? "" : directory + "\\";
                if (!names.Contains(AssetReferences.Normalize(prefix + stem + ".mdl")))
                    findings.Add(new AssetAuditFinding(name, AssetAuditKind.ModelWithoutSource,
                        "no matching .mdl beside it"));
            }
        }

        findings.Sort((a, b) =>
        {
            int byPath = string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
            return byPath != 0 ? byPath : a.Kind.CompareTo(b.Kind);
        });
        return new AssetAuditResult(findings, entries.Count, maxBytes, diagnostics);
    }

    private static bool IsPowerOfTwo(int value) => value > 0 && (value & (value - 1)) == 0;

    private static bool TryTextureSize(MapFileEntry entry, out int width, out int height)
    {
        width = height = 0;
        var bytes = entry.CurrentBytes;
        // Some maps ship PNG data under a .blp name. The engine decodes it, the BLP decoder
        // cannot, and reporting "unknown" for a valid texture is the false signal. Read the
        // IHDR width and height straight out of the PNG signature instead.
        if (bytes.Length >= 24
            && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
            && bytes[12] == 0x49 && bytes[13] == 0x48 && bytes[14] == 0x44 && bytes[15] == 0x52)
        {
            width = (bytes[16] << 24) | (bytes[17] << 16) | (bytes[18] << 8) | bytes[19];
            height = (bytes[20] << 24) | (bytes[21] << 16) | (bytes[22] << 8) | bytes[23];
            return width > 0 && height > 0;
        }
        try
        {
            var texture = BlpDecoder.Decode(entry.CurrentBytes);
            width = texture.Width;
            height = texture.Height;
            return width > 0 && height > 0;
        }
        catch
        {
            return false;
        }
    }
}
