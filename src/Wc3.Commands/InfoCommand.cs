using Wc3.Model;
using War3Net.Build.Info;
using War3Net.Build.Script;

namespace Wc3.Commands;

public static class InfoCommand
{
    public static MapInfoResult Execute(MapDocument doc)
    {
        var info = doc.GetFile("war3map.w3i")?.Model as MapInfo;
        var strings = BuildTriggerStringTable(doc);
        var diags = doc.Diagnostics.Select(d => $"{d.Severity}: {d.FileName} — {d.Message}").ToList();
        return new MapInfoResult(
            Name: TriggerStringResolver.Resolve(info?.MapName ?? "(unknown)", strings),
            Author: TriggerStringResolver.Resolve(info?.MapAuthor ?? "(unknown)", strings),
            Players: info?.Players?.Count ?? 0,
            Width: info?.PlayableMapAreaWidth,
            Height: info?.PlayableMapAreaHeight,
            Diagnostics: diags);
    }

    private static IReadOnlyDictionary<int, string> BuildTriggerStringTable(MapDocument doc)
    {
        var wts = doc.GetFile("war3map.wts")?.Model as TriggerStrings;
        var table = new Dictionary<int, string>();
        foreach (var s in wts?.Strings ?? Enumerable.Empty<TriggerString>())
            table[(int)s.Key] = s.Value ?? string.Empty;
        return table;
    }
}
