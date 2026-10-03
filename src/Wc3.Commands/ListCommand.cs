using Wc3.Model;

namespace Wc3.Commands;

public static class ListCommand
{
    /// <summary>
    /// Lists every archive entry. With <paramref name="typeUnnamed"/> the nameless entries
    /// additionally get typed from their leading bytes, so a protected map's listing says
    /// "BLP texture" instead of nothing. That sniff reads a bounded prefix per nameless entry
    /// (never the full decompression Load deferred) and caches on the entry, but on the worst
    /// protected maps it is still 65,000 bounded reads, measured at about 2.5 seconds cold.
    /// So it is opt-in, a caller that only wants names and sizes (the Studio status line, the
    /// Files panel's first paint) stays at the 1 ms sweep, and the callers that want content
    /// types (the ls verb, list_files, the Files panel's background pass) ask for them.
    /// </summary>
    public static FileListResult Execute(MapDocument doc, bool typeUnnamed = false) => new(
        // RawSize, not RawBytes.Length. A listing wants the SIZE of every entry, and asking for
        // the bytes to get it decompresses the whole archive, which Load deliberately deferred.
        // On a 240 MB map that is 1617 ms against 1 ms, paid every time anything lists files.
        doc.Files.Select(f => new FileEntryInfo(
            f.FileName, f.RawSize, f.IsKnown, f.IsParsed,
            typeUnnamed && f.FileName is null ? ContentTypeSniffer.Sniff(f).DisplayName : null,
            f.NameFromHarvest)).ToList());
}
