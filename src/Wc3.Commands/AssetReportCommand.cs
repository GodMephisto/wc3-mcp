// src/Wc3.Commands/AssetReportCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One imported entry, with the small facts that decide where it belongs.</summary>
public sealed record AssetReportEntry(string Path, int SizeBytes, string Type, bool Empty, bool Referenced);

/// <summary>One problem found against one file or one object.</summary>
public sealed record AssetReportFinding(string Kind, string Path, int SizeBytes, string Detail);

/// <summary>Imported bytes and count for one file type.</summary>
public sealed record AssetReportTypeCount(string Type, int Count, long Bytes);

/// <summary>
/// Everything the map carries and everything wrong with it, in one answer.
/// <see cref="EntryList"/> is every imported file with its size and whether anything references
/// it, which is the "log everything" half. <see cref="Findings"/> is every fault the asset checks
/// and the model scan know how to find, which is the "and whatever" half. <see cref="Summary"/>
/// counts the findings by kind so a reader sees the shape before the list.
/// </summary>
public sealed record AssetReportResult(
    string Map,
    int Entries,
    long TotalBytes,
    int ImportedEntries,
    IReadOnlyList<AssetReportTypeCount> ByType,
    IReadOnlyList<AssetReportEntry> EntryList,
    IReadOnlyList<AssetReportFinding> Findings,
    IReadOnlyList<War3LogFault> EngineFaults,
    IReadOnlyDictionary<string, int> Summary);

/// <summary>
/// One report over a map's imported files, composing the checks that already exist rather than
/// re-deriving any of them. It runs asset_audit (oversized, non power of two texture, model with
/// no source, empty), asset_unused (referenced nowhere), asset_duplicates (byte-identical groups)
/// and asset_model_lint (geoset without material, missing texture, zero duration sequence,
/// bounding box out of range), and lists every entry so the whole archive is accounted for, not
/// only the faults. Read-only.
///
/// The engine does not log what it loads, and its loading screen runs before the script, so this
/// static report is the complete answer a map can give about its own contents. The engine faults
/// it can still write are in War3Log.txt, and a formatting reader over it is a separate verb.
/// </summary>
public static class AssetReportCommand
{
    public static AssetReportResult Execute(MapDocument doc, string map = "(map)",
        long maxBytes = AssetAuditCommand.DefaultMaxBytes, bool includeModelLint = true,
        string? logPath = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var referenced = AssetReferences.ReferencedSpellings(doc);
        var entries = AssetReferences.ImportedEntries(doc);

        var list = new List<AssetReportEntry>(entries.Count);
        var byType = new Dictionary<string, (int Count, long Bytes)>(StringComparer.OrdinalIgnoreCase);
        var sizeByPath = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        long total = 0;
        foreach (var entry in entries)
        {
            var name = entry.FileName!;
            int size = entry.CurrentSize;
            total += size;
            sizeByPath[name] = size;
            var type = Path.GetExtension(name).ToLowerInvariant();
            if (type.Length == 0) type = "(none)";
            byType[type] = byType.TryGetValue(type, out var c) ? (c.Count + 1, c.Bytes + size) : (1, size);
            list.Add(new AssetReportEntry(name, size, type, size == 0,
                referenced.Contains(AssetReferences.Normalize(name))));
        }

        int SizeOf(string path) => sizeByPath.TryGetValue(path, out var s) ? s : 0;
        var findings = new List<AssetReportFinding>();

        foreach (var f in AssetAuditCommand.Execute(doc, maxBytes).Findings)
            findings.Add(new AssetReportFinding(f.Kind.ToString(), f.Path, SizeOf(f.Path), f.Detail));

        foreach (var u in AssetUnusedCommand.Execute(doc).Unused)
            findings.Add(new AssetReportFinding("Unreferenced", u.Path, u.SizeBytes,
                "no object field and no script literal names it"));

        foreach (var group in AssetDuplicatesCommand.Execute(doc).Groups)
            foreach (var path in group.Paths)
                findings.Add(new AssetReportFinding("Duplicate", path, group.SizeBytes,
                    $"{group.Paths.Count - 1} other file(s) hold the same bytes, {group.RecoverableBytes} recoverable"));

        if (includeModelLint)
        {
            var lint = AssetModelLintCommand.Execute(doc);
            foreach (var m in lint.Findings)
                findings.Add(new AssetReportFinding(m.Kind.ToString(), m.Path, SizeOf(m.Path), m.Detail));
            foreach (var d in lint.Diagnostics)
                findings.Add(new AssetReportFinding("ModelUnreadable", d, 0, "the model could not be parsed at all"));
        }

        findings.Sort((a, b) => string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase));

        // The engine's own faults, from War3Log.txt, folded into the same summary so one report
        // carries both what the map holds and what the engine actually complained about.
        var engineFaults = logPath is null ? Array.Empty<War3LogFault>() : War3LogFaults.Read(logPath);
        var summary = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var g in findings.GroupBy(f => f.Kind, StringComparer.Ordinal))
            summary[g.Key] = g.Count();
        foreach (var g in engineFaults.GroupBy(f => "Engine:" + f.Kind, StringComparer.Ordinal))
            summary[g.Key] = g.Sum(f => f.Count);
        summary = summary.OrderByDescending(kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

        var types = byType
            .Select(kv => new AssetReportTypeCount(kv.Key, kv.Value.Count, kv.Value.Bytes))
            .OrderByDescending(t => t.Bytes)
            .ToList();

        return new AssetReportResult(map, list.Count, total, entries.Count,
            types, list, findings, engineFaults, summary);
    }
}
