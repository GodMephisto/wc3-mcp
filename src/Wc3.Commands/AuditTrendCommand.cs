// src/Wc3.Commands/AuditTrendCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One check's change between two maps. Added are issues the second map has and the first does
/// not, Removed the reverse. The detail strings name each one, so the change is readable rather
/// than a count.
/// </summary>
public sealed record AuditTrendRow(
    string Check,
    int Added,
    int Removed,
    IReadOnlyList<string> AddedDetail,
    IReadOnlyList<string> RemovedDetail);

/// <summary>
/// The audit trend between two maps, grouped by check. Added and Removed are the totals across
/// every check. Diagnostics carries anything either audit said about a skipped check.
/// </summary>
public sealed record AuditTrendResult(
    string MapA,
    string MapB,
    IReadOnlyList<AuditTrendRow> Rows,
    int Added,
    int Removed,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Run the behavioural audit against two maps and report what the second added and removed,
/// grouped by check. Where audit answers what is wrong now, this answers what changed, which is
/// the regression view an edit or a port needs. Issues are matched by check, rawcode and message,
/// so a reordered field does not read as a change.
/// </summary>
public static class AuditTrendCommand
{
    public static AuditTrendResult Execute(
        string mapA, string mapB, string? gameDirOverride = null, IReadOnlyCollection<string>? only = null)
    {
        var a = AuditCommand.Execute(Load(mapA), gameDirOverride, only);
        var b = AuditCommand.Execute(Load(mapB), gameDirOverride, only);

        var inA = a.Issues.GroupBy(Key).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        var inB = b.Issues.GroupBy(Key).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        var checks = a.Issues.Select(i => i.Check)
            .Concat(b.Issues.Select(i => i.Check))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(c => c, StringComparer.Ordinal);

        var rows = new List<AuditTrendRow>();
        int added = 0, removed = 0;
        foreach (var check in checks)
        {
            var addedIssues = b.Issues.Where(i => i.Check == check && !inA.ContainsKey(Key(i))).ToList();
            var removedIssues = a.Issues.Where(i => i.Check == check && !inB.ContainsKey(Key(i))).ToList();
            if (addedIssues.Count == 0 && removedIssues.Count == 0) continue;

            added += addedIssues.Count;
            removed += removedIssues.Count;
            rows.Add(new AuditTrendRow(
                check, addedIssues.Count, removedIssues.Count,
                addedIssues.Select(Describe).ToList(),
                removedIssues.Select(Describe).ToList()));
        }

        var diagnostics = a.Diagnostics.Concat(b.Diagnostics)
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return new AuditTrendResult(mapA, mapB, rows, added, removed, diagnostics);
    }

    private static MapDocument Load(string mapPath)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("no map at " + mapPath, mapPath);
        return MapDocument.Load(mapPath);
    }

    /// <summary>Identity of an issue for matching. Check, rawcode and message, which is the full claim.</summary>
    private static string Key(AuditIssue i) =>
        i.Check + "\u0000" + i.Rawcode + "\u0000" + i.Message;

    private static string Describe(AuditIssue i) =>
        i.Rawcode + (i.Owner is { Length: > 0 } ? " (" + i.Owner + ")" : "") + " "
        + i.Message;
}
