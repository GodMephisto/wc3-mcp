namespace Wc3.Commands;

/// <summary>
/// Pure matching + ranking for searchable dropdowns: Studio's SearchableComboBox,
/// the object editor's list search, and any other front-end that filters
/// "Name (id)" style choices. No UI types here - Wc3.Commands stays
/// front-end-agnostic (Principle #3).
/// </summary>
public static class DropdownFilter
{
    /// <summary>
    /// Case-insensitive substring match on the display name OR the id. The query
    /// is trimmed first; an empty (or whitespace-only, or null) query matches
    /// everything. Null name/id never match a non-empty query.
    /// </summary>
    public static bool Matches(string? query, string? name, string? id)
    {
        var q = query?.Trim();
        if (string.IsNullOrEmpty(q))
            return true;
        return Contains(name, q) || Contains(id, q);
    }

    /// <summary>
    /// Sort key for filtered results - lower ranks first; sort stably so ties keep
    /// the caller's list order. Exact-id beats exact-name beats name-prefix beats
    /// id-prefix beats plain substring hits (name before id); no match at all is
    /// <see cref="int.MaxValue"/>. An empty query ranks everything 0, so ordering
    /// by rank leaves an unfiltered list untouched.
    /// </summary>
    public static int Rank(string? query, string? name, string? id)
    {
        var q = query?.Trim();
        if (string.IsNullOrEmpty(q))
            return 0;
        if (EqualsCi(id, q)) return 0;
        if (EqualsCi(name, q)) return 1;
        if (StartsWith(name, q)) return 2;
        if (StartsWith(id, q)) return 3;
        if (Contains(name, q)) return 4;
        if (Contains(id, q)) return 5;
        return int.MaxValue;
    }

    private static bool EqualsCi(string? text, string query) =>
        text is not null && string.Equals(text, query, StringComparison.OrdinalIgnoreCase);

    private static bool StartsWith(string? text, string query) =>
        text is not null && text.StartsWith(query, StringComparison.OrdinalIgnoreCase);

    private static bool Contains(string? text, string query) =>
        text is not null && text.Contains(query, StringComparison.OrdinalIgnoreCase);
}
