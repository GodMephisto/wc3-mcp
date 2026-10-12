// src/Wc3.Commands/PortDepsCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One object the closure needs, with what it is and whether the root really owns it.</summary>
public sealed record DepsObject(string Rawcode, string? Name, bool CustomToMap, bool CarriedByScriptClosure);

/// <summary>Objects of one kind, so a report can say "3 ability, 1 buff" without a reader grouping them.</summary>
public sealed record DepsObjectGroup(string Kind, IReadOnlyList<DepsObject> Objects);

/// <summary>One asset the closure needs, its category, whether the map holds it, and its byte size
/// when it does (a base-game asset present in no map has no size here).</summary>
public sealed record DepsAsset(string Path, string Category, bool PresentInMap, int? SizeBytes);

/// <summary>
/// The full dependency report for one rawcode, its objects grouped by kind, its assets with
/// sizes, the display strings and the script functions its closure needs. This is the readable
/// shape of a <see cref="UnitBundle"/>, the same resolution the port preview uses, presented
/// so a reader sees what a port would carry before running one.
/// </summary>
public sealed record PortDepsResult(
    string RootRawcode,
    string? RootName,
    string RootKind,
    IReadOnlyList<DepsObjectGroup> ObjectGroups,
    int ObjectCount,
    IReadOnlyList<DepsAsset> Assets,
    int AssetsInMap,
    int AssetsMissing,
    long BytesInMap,
    IReadOnlyList<string> Strings,
    int Functions,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Resolves the dependency closure of a rawcode and groups it for reading. Thin over
/// <see cref="BundleCommand"/> plus <see cref="BundleStructure"/>, so the deps report, the
/// bundle command and the port preview all speak of one closure, not three.
/// </summary>
public static class PortDepsCommand
{
    public static PortDepsResult Execute(
        MapDocument doc, string rawcode, ObjectKind rootKind, string? gameDirOverride)
        => Execute(doc, BundleCommand.ResolveObject(doc, rootKind, rawcode, gameDirOverride), rootKind);

    internal static PortDepsResult Execute(MapDocument doc, UnitBundle bundle, ObjectKind rootKind)
    {
        // An object is the root's own when the root reaches it through a real field; the rest
        // arrived through the script closure's synthetic seed edge (another hero's kit on a
        // shared arena script), so they are flagged rather than presented as this unit's.
        var carried = BundleStructure.CarriedByScriptClosure(bundle);

        var groups = bundle.Objects
            .GroupBy(o => o.Kind)
            .OrderBy(g => g.Key)
            .Select(g => new DepsObjectGroup(
                g.Key.ToString().ToLowerInvariant(),
                g.OrderBy(o => o.Rawcode, StringComparer.Ordinal)
                    .Select(o => new DepsObject(o.Rawcode, o.Name, o.CustomToMap, carried.Contains(o.Rawcode)))
                    .ToList()))
            .ToList();

        var assets = new List<DepsAsset>();
        long bytes = 0;
        int inMap = 0;
        foreach (var f in bundle.Files.OrderBy(f => f.Path, StringComparer.Ordinal))
        {
            int? size = null;
            if (f.PresentInMap)
            {
                var entry = RenderModelCommand.FindAssetEntry(doc, f.Path) ?? FindFile(doc, f.Path);
                if (entry is { CurrentBytes.Length: > 0 })
                {
                    size = entry.CurrentBytes.Length;
                    bytes += size.Value;
                    inMap++;
                }
            }
            assets.Add(new DepsAsset(f.Path, f.Category, f.PresentInMap, size));
        }

        var rootNode = bundle.Objects.FirstOrDefault(o => o.Rawcode == bundle.RootRawcode);
        string rootKindWord = (rootNode?.Kind ?? rootKind).ToString().ToLowerInvariant();

        return new PortDepsResult(
            bundle.RootRawcode, bundle.RootName, rootKindWord,
            groups, bundle.Objects.Count,
            assets, inMap, assets.Count - inMap, bytes,
            bundle.Strings, bundle.Functions.Count, bundle.Diagnostics);
    }

    /// <summary>The stored entry for a bundle file path, trying both slash spellings, so an
    /// asset present under a different slash than the reference still reports its size.</summary>
    private static MapFileEntry? FindFile(MapDocument doc, string path)
    {
        foreach (var p in new[] { path, path.Replace('/', '\\'), path.Replace('\\', '/') }.Distinct())
            if (doc.GetFile(p) is { } e) return e;
        return null;
    }
}
