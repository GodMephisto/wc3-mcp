using Wc3.Model;
using War3Net.Build.Info;
using War3Net.Build.Script;

namespace Wc3.Commands;

public static class InfoCommand
{
    public static MapInfoResult Execute(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.w3i");
        var info = entry?.Model as MapInfo;
        // A w3i that is present but did not parse used to read as name "(unknown)" and 0 players
        // with nothing else said, which is how 21 maps saved by the 3.0 editor (w3i format 39)
        // looked empty. Refuse instead, with the parser's reason, as map_info_get does.
        if (entry is not null && info is null)
        {
            var why = doc.Diagnostics.FirstOrDefault(d => string.Equals(d.FileName, "war3map.w3i", StringComparison.OrdinalIgnoreCase))?.Message;
            throw new InvalidOperationException($"war3map.w3i could not be parsed{(why is null ? "" : $" ({why})")}, so the map info is unknown.");
        }
        var strings = BuildTriggerStringTable(doc);
        var diags = doc.Diagnostics.Select(d => $"{d.Severity}: {d.FileName}, {d.Message}").ToList();
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
