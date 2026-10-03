// src/Wc3.Commands/FilePreviewCommand.cs
using System.Text;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// How a map file should be shown: decoded text, a PNG (for BLP/DDS images), or a hex
/// dump for opaque binary. Kind is "text" | "image" | "binary".
/// </summary>
public sealed record FilePreview(string Name, string Kind, string Info, string? Text, byte[]? Png);

/// <summary>
/// Produces a human-viewable preview of a single internal map file. Text formats decode
/// as UTF-8, .blp/.dds images decode + re-encode to PNG, everything else becomes a hex
/// dump, so a front-end only has to display, never parse.
/// </summary>
public static class FilePreviewCommand
{
    private const int MaxTextChars = 256 * 1024;   // keep the UI responsive on huge scripts
    private const int HexDumpBytes = 4 * 1024;     // first 4 KB as a hex dump

    private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3", ".wav", ".flac", ".ogg",
    };

    private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".j", ".lua", ".txt", ".wts", ".slk", ".ini", ".fdf", ".toc", ".xml",
        ".json", ".log", ".csv", ".md", ".htm", ".html", ".ai", ".pld", ".w3v",
    };

    public static FilePreview Execute(MapDocument doc, string name)
    {
        var entry = doc.GetFile(name)
            ?? throw new FileNotFoundException($"'{name}' is not in the map", name);
        return Of(entry.CurrentBytes, name);
    }

    /// <summary>Preview from raw bytes (used for unnamed entries too, with name = null).</summary>
    public static FilePreview Of(byte[] bytes, string? name)
    {
        string display = name ?? "(unnamed entry)";
        string ext = name is null ? "" : Path.GetExtension(name);

        if (AudioExtensions.Contains(ext))
            return new FilePreview(display, "audio",
                $"{ext.TrimStart('.').ToUpperInvariant()} audio, {bytes.Length:N0} bytes", null, null);

        if (string.Equals(ext, ".blp", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var tex = BlpDecoder.Decode(bytes);
                return new FilePreview(display, "image",
                    $"{tex.Width}x{tex.Height} BLP, {bytes.Length:N0} bytes", null, TexturePng.Encode(tex));
            }
            catch
            {
                // Not a decodable BLP after all — fall through to hex.
            }
        }

        if (string.Equals(ext, ".dds", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var tex = DdsDecoder.Decode(bytes);
                return new FilePreview(display, "image",
                    $"{tex.Width}x{tex.Height} DDS, {bytes.Length:N0} bytes", null, TexturePng.Encode(tex));
            }
            catch
            {
                // Not a decodable DDS after all, fall through to hex.
            }
        }

        if (TextExtensions.Contains(ext) || LooksLikeText(bytes))
        {
            var text = Encoding.UTF8.GetString(bytes);
            bool truncated = text.Length > MaxTextChars;
            if (truncated) text = text[..MaxTextChars];
            string info = truncated
                ? $"text, {bytes.Length:N0} bytes (showing first {MaxTextChars:N0} chars - Extract for the full file)"
                : $"text, {bytes.Length:N0} bytes";
            return new FilePreview(display, "text", info, text, null);
        }

        return new FilePreview(display, "binary",
            $"binary, {bytes.Length:N0} bytes (hex dump of first {Math.Min(bytes.Length, HexDumpBytes):N0} bytes)",
            HexDump(bytes, HexDumpBytes), null);
    }

    /// <summary>Sniff: mostly-printable + no NULs in the first block reads as text.</summary>
    private static bool LooksLikeText(byte[] bytes)
    {
        int n = Math.Min(bytes.Length, 512);
        if (n == 0) return false;
        int printable = 0;
        for (int i = 0; i < n; i++)
        {
            byte b = bytes[i];
            if (b == 0) return false; // NUL → binary
            if (b == '\t' || b == '\n' || b == '\r' || (b >= 0x20 && b < 0x7F) || b >= 0x80) printable++;
        }
        return printable >= n * 0.90;
    }

    private static string HexDump(byte[] bytes, int limit)
    {
        int n = Math.Min(bytes.Length, limit);
        var sb = new StringBuilder(n * 4);
        for (int off = 0; off < n; off += 16)
        {
            sb.Append(off.ToString("X8")).Append("  ");
            int rowEnd = Math.Min(off + 16, n);
            for (int i = off; i < off + 16; i++)
            {
                sb.Append(i < rowEnd ? bytes[i].ToString("X2") : "  ").Append(' ');
                if (i == off + 7) sb.Append(' ');
            }
            sb.Append(' ');
            for (int i = off; i < rowEnd; i++)
            {
                byte b = bytes[i];
                sb.Append(b >= 0x20 && b < 0x7F ? (char)b : '.');
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
