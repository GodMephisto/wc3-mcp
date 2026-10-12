namespace Wc3.Commands;

/// <summary>
/// Shared paging for the brief listing modes. A brief listing answers with the total and a first
/// page, so an agent learns how big a list is without paying for every entry in its context.
/// </summary>
public static class BriefListing
{
    /// <summary>Entries in the first page of a brief listing.</summary>
    public const int PageSize = 50;

    /// <summary>The first page plus the total and whether the list was cut.</summary>
    public static (IReadOnlyList<T> Page, int Total, bool Truncated) FirstPage<T>(IReadOnlyList<T> all) =>
        all.Count <= PageSize
            ? (all, all.Count, false)
            : (all.Take(PageSize).ToList(), all.Count, true);
}

/// <summary>list_files brief outcome, the total file count and the first page of files.</summary>
public sealed record FileListBriefResult(int Total, bool Truncated, IReadOnlyList<FileEntryInfo> Files);

/// <summary>object_list brief outcome, the total object count and the first page of items.</summary>
public sealed record ObjectListBriefResult(int Total, bool Truncated, IReadOnlyList<ObjectListItem> Items);

/// <summary>placed_units_list brief outcome, the total unit count and the first page of units.</summary>
public sealed record PlacedUnitsBriefResult(int Total, bool Truncated, IReadOnlyList<UnitInstanceInfo> Units);
