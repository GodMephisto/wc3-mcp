using Wc3.Model;

namespace Wc3.Commands;

public static class ListCommand
{
    /// <summary>
    /// Lists every archive entry. With <paramref name="typeUnnamed"/> the nameless entries
    /// additionally get typed from their leading bytes, so a protected map's listing says
    /// "BLP texture" instead of nothing. That sniff reads a bounded prefix per nameless entry
    /// (never the full decompression Load deferred) and caches on the entry. The cache makes a
    /// second pass free, and the first pass is warmed up front by
    /// <see cref="MapDocument.PreSniffUnnamedEntries"/>, which reads the prefixes in parallel so a
    /// protected map's 64,738 of them no longer serialize into 22.6 seconds on the archive lock.
    /// So it is opt-in, a caller that only wants names and sizes (the Studio status line, the
    /// Files panel's first paint) stays at the 1 ms sweep, and the callers that want content
    /// types (the ls verb, list_files, the Files panel's background pass) ask for them.
    /// </summary>
    public static FileListResult Execute(MapDocument doc, bool typeUnnamed = false)
    {
        // Warm the per-entry sniff cache before the projection reads it, so each Sniff below is a
        // cache read rather than one bounded read serialized on the document's archive lock.
        if (typeUnnamed) doc.PreSniffUnnamedEntries();

        return new(
            // RawSize, not RawBytes.Length. A listing wants the SIZE of every entry, and asking for
            // the bytes to get it decompresses the whole archive, which Load deliberately deferred.
            // On a 240 MB map that is 1617 ms against 1 ms, paid every time anything lists files.
            doc.Files.Select(f => new FileEntryInfo(
                f.FileName, f.RawSize, f.IsKnown, f.IsParsed,
                typeUnnamed && f.FileName is null ? ContentTypeSniffer.Sniff(f).DisplayName : null,
                f.NameFromHarvest)).ToList());
    }

    /// <summary>A brief listing, the total file count and only the first page of files.</summary>
    public static FileListBriefResult ExecuteBrief(MapDocument doc, bool typeUnnamed = false)
    {
        var (page, total, truncated) = BriefListing.FirstPage(Execute(doc, typeUnnamed).Files);
        return new FileListBriefResult(total, truncated, page);
    }
}
