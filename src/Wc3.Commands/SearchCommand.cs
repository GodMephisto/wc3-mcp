using Wc3.Model;

namespace Wc3.Commands;

public static class SearchCommand
{
    public static SearchResult Execute(MapDocument doc, string query)
    {
        var hits = new List<SearchHit>();
        foreach (var f in doc.Files.Where(f => f.FileName != null))
            if (f.FileName!.Contains(query, StringComparison.OrdinalIgnoreCase))
                hits.Add(new SearchHit(f.FileName!, "filename"));
        return new SearchResult(hits);
    }
}
