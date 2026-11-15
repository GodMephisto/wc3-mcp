// src/Wc3.Commands/TextureAuditCommand.cs
// Scans every .blp a map carries and reports the headers the engine mishandles. Read-only, no game
// install, no decode.
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Commands;

public sealed record TextureAuditResult(
    bool Ok,
    string Message,
    int Textures,
    int Findings,
    long Bytes,
    IReadOnlyList<BlpFinding> Items,
    IReadOnlyDictionary<string, int> Problems);

public static class TextureAuditCommand
{
    public static TextureAuditResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var items = new List<BlpFinding>();
        var kinds = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int textures = 0;
        long bytes = 0;

        foreach (var entry in doc.Files)
        {
            var name = entry.FileName ?? entry.RecoveredFileName;
            if (name is null || !name.EndsWith(".blp", StringComparison.OrdinalIgnoreCase)) continue;
            textures++;
            // The header is the first kilobyte, and the length check only needs the file's size, so
            // the pixels are never decompressed here.
            byte[] head;
            try { head = entry.ReadPrefix(BlpAudit.PrefixLength); }
            catch (Exception) { continue; }
            long length = entry.RawSize;
            bytes += length;

            var problem = BlpAudit.Inspect(head, length, out var w, out var h, out var kind);
            if (problem is null) continue;
            string label = kind switch
            {
                BlpProblem.NotBlp => "no BLP signature",
                BlpProblem.TruncatedHeader => "a header shorter than its own tables",
                BlpProblem.JpegHeaderTooLarge => "a JPEG header above 624 bytes",
                BlpProblem.MipPastEnd => "mip levels past the end of the file",
                BlpProblem.NoMipLevels => "no mip levels declared",
                BlpProblem.MissingFirstMip => "an empty first mip",
                BlpProblem.BadDimensions => "dimensions the engine will not read",
                BlpProblem.NotPowerOfTwo => "dimensions that are not a power of two",
                _ => "another defect",
            };
            kinds.TryGetValue(label, out var n);
            kinds[label] = n + 1;
            items.Add(new BlpFinding(name, problem, w, h, length));
        }

        items.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        var message = items.Count == 0
            ? $"all {textures} textures carry a header the engine reads correctly"
            : $"{items.Count} of {textures} textures carry a header the engine mishandles, {bytes:N0} bytes of texture in the map";

        return new TextureAuditResult(true, message, textures, items.Count, bytes, items, kinds);
    }
}
