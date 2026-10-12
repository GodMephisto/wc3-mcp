using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>How many custom or modified objects of one Object Editor kind the map holds.</summary>
public sealed record ObjectKindCount(string Kind, int Count);

/// <summary>
/// The one-call map summary: metadata, every archive file, the custom object count per kind and
/// the load diagnostics. Diagnostics repeats <see cref="MapInfoResult.Diagnostics"/> so the
/// overview stands on its own.
/// </summary>
public sealed record MapOverviewResult(
    MapInfoResult Info,
    IReadOnlyList<FileEntryInfo> Files,
    IReadOnlyList<ObjectKindCount> ObjectCounts,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Composes the info, file-list and per-kind object-count reads into one result, so an agent
/// learns what a map is and what it holds in a single call instead of one call per tool. The
/// counts come from the same object-list handler every other caller uses, so the overview cannot
/// disagree with an explicit object list. No game data is opened, since a count needs only the
/// map's own entries, which keeps the call cheap and hermetic.
/// </summary>
public static class MapOverviewCommand
{
    public static MapOverviewResult Execute(MapDocument doc)
    {
        var info = InfoCommand.Execute(doc);
        var files = ListCommand.Execute(doc);
        var counts = ObjectKinds.All
            .Select(kind => new ObjectKindCount(
                kind.ToString(),
                ObjectListCommand.Execute(doc, kind, (GameDataContext?)null).Items.Count))
            .ToList();
        return new MapOverviewResult(info, files.Files, counts, info.Diagnostics);
    }
}
