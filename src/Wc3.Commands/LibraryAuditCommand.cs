// src/Wc3.Commands/LibraryAuditCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map's behavioural audit from a folder sweep, with its issues and error total.</summary>
public sealed record LibraryAuditMap(
    string Map,
    bool Ok,
    int Errors,
    int Warnings,
    IReadOnlyList<AuditIssue> Issues,
    string? Error);

/// <summary>How many findings of one audit check the whole sweep produced.</summary>
public sealed record LibraryCheckTotal(string Check, int Count);

/// <summary>
/// A folder audit holding every map's rows, the findings grouped by check, and the totals. Maps are
/// ordered by errors then findings then name, so the library reads ranked by how much it needs
/// fixing rather than in folder order.
/// </summary>
public sealed record LibraryAuditResult(
    IReadOnlyList<LibraryAuditMap> Maps,
    IReadOnlyList<LibraryCheckTotal> Checks,
    int TotalFindings,
    int MapsWithFindings);

/// <summary>
/// Run the behavioural audit over every map in a folder and report the findings per map and the
/// totals per check, a sweep rather than one map. Each map goes through
/// <see cref="AuditCommand.Execute"/>, so the checks are the same ones the single-map audit applies.
/// This command only runs them across the folder and groups the results.
///
/// Maps are opened one at a time and released before the next, so a sweep never holds a whole
/// library in memory at once.
/// </summary>
public static class LibraryAuditCommand
{
    public static LibraryAuditResult Execute(string folder, string? gameDirOverride = null)
    {
        var maps = new List<LibraryAuditMap>();
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            string name = Path.GetFileName(path);
            try
            {
                maps.Add(Probe(name, MapDocument.Load(path), gameDirOverride));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                maps.Add(Fail(name, e.Message));
            }
        }
        return Summarize(maps);
    }

    /// <summary>One already-loaded map's audit row. Pure, so the report can reuse one load.</summary>
    public static LibraryAuditMap Probe(string name, MapDocument doc, string? gameDirOverride = null)
    {
        var r = AuditCommand.Execute(doc, gameDirOverride);
        int errors = r.Issues.Count(i => i.Severity == DiagnosticSeverity.Error);
        return new LibraryAuditMap(name, r.Ok, errors, r.Issues.Count - errors, r.Issues, null);
    }

    /// <summary>A map that could not be loaded, with the failure reason and no findings.</summary>
    public static LibraryAuditMap Fail(string name, string error) =>
        new(name, false, 0, 0, Array.Empty<AuditIssue>(), error);

    /// <summary>
    /// Fold the per-map audits into the per-check totals and the finding counts. Pure. Maps are
    /// ranked by errors then findings then name, so the worst map reads first, and checks are
    /// ordered by count then name, so the biggest source of findings reads first.
    /// </summary>
    public static LibraryAuditResult Summarize(IReadOnlyList<LibraryAuditMap> maps)
    {
        var ranked = maps
            .OrderByDescending(m => m.Errors)
            .ThenByDescending(m => m.Issues.Count)
            .ThenBy(m => m.Map, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var checks = maps.SelectMany(m => m.Issues)
            .GroupBy(i => i.Check, StringComparer.Ordinal)
            .Select(g => new LibraryCheckTotal(g.Key, g.Count()))
            .OrderByDescending(c => c.Count)
            .ThenBy(c => c.Check, StringComparer.Ordinal)
            .ToList();

        return new LibraryAuditResult(
            ranked, checks, maps.Sum(m => m.Issues.Count), maps.Count(m => m.Issues.Count > 0));
    }
}
