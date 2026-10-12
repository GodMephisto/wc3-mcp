// src/Wc3.Commands/MpqExtractAllCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Every entry of an archive, selected for extraction, with counts for the report.</summary>
public sealed record MpqExtractAllResult(
    int TotalEntries,
    int NamedEntries,
    int UnnamedEntries,
    long TotalBytes,
    IReadOnlyList<ExtractedItem> Items);

/// <summary>
/// Selects every entry of an archive for extraction, named and unnamed alike. This is the
/// container-level counterpart of <c>extract</c>, and it never filters, because "all" is the
/// point, and the caller writes the bytes with the shared path-safety rule in
/// <see cref="ExtractPaths"/>.
///
/// Read-only over the map. Selecting the bytes is all that happens here, the front-end decides
/// where they land.
/// </summary>
public static class MpqExtractAllCommand
{
    public static MpqExtractAllResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var result = ExtractCommand.Execute(doc, new ExtractSelector { All = true });
        int named = result.Items.Count(i => i.Name is not null);
        return new MpqExtractAllResult(
            result.Items.Count,
            named,
            result.Items.Count - named,
            result.Items.Sum(i => (long)i.Bytes.Length),
            result.Items);
    }
}
