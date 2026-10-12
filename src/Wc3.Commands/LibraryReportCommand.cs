// src/Wc3.Commands/LibraryReportCommand.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Where the report landed and the headline counts a caller can show without reading it.</summary>
public sealed record LibraryReportResult(
    string Path,
    int Maps,
    int Loaded,
    int Failed,
    int Findings,
    int Invalid);

/// <summary>
/// Write a single Markdown report over a folder of maps, a summary line, a per-map table and the
/// aggregated findings, so a maintainer can attach one artifact to a review. The report is built
/// from the same probes the scan, audit and validate sweeps use, run over one load per map, so it
/// cannot disagree with those commands.
///
/// Maps are opened one at a time and released before the next, so the sweep never holds a whole
/// library in memory at once. The report states this in its own body.
/// </summary>
public static class LibraryReportCommand
{
    public static LibraryReportResult Execute(string folder, string outPath, string? gameDirOverride = null)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new ArgumentException("a report output path is required.", nameof(outPath));

        var scans = new List<LibraryScanMap>();
        var audits = new List<LibraryAuditMap>();
        var validations = new List<LibraryValidateMap>();

        // One load per map feeds all three probes, and the three lists stay index aligned because
        // the same loop appends to each, so the report rows join by position rather than by name.
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            string name = Path.GetFileName(path);
            long bytes = new FileInfo(path).Length;
            try
            {
                var doc = MapDocument.Load(path);
                scans.Add(LibraryScanCommand.Probe(name, bytes, doc));
                audits.Add(LibraryAuditCommand.Probe(name, doc, gameDirOverride));
                validations.Add(LibraryValidateCommand.Probe(name, doc));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                scans.Add(LibraryScanCommand.Fail(name, bytes, e.Message));
                audits.Add(LibraryAuditCommand.Fail(name, e.Message));
                validations.Add(LibraryValidateCommand.Fail(name, e.Message));
            }
        }

        var scan = LibraryScanCommand.Summarize(scans);
        var audit = LibraryAuditCommand.Summarize(audits);
        string markdown = Build(folder, scans, audits, validations, scan, audit);

        string full = Path.GetFullPath(outPath);
        string? dir = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(full, markdown);

        return new LibraryReportResult(
            full, scan.Maps.Count, scan.Loaded, scan.Failed, audit.TotalFindings,
            validations.Count(v => !v.Valid));
    }

    /// <summary>Render the report body. Pure, so the exact Markdown can be tested without a file.</summary>
    public static string Build(
        string folder,
        IReadOnlyList<LibraryScanMap> scans,
        IReadOnlyList<LibraryAuditMap> audits,
        IReadOnlyList<LibraryValidateMap> validations,
        LibraryScanResult scan,
        LibraryAuditResult audit)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Map library report");
        sb.AppendLine();
        sb.AppendLine("Folder " + folder);
        sb.AppendLine();
        sb.AppendLine($"Maps {scan.Maps.Count}, loaded {scan.Loaded}, failed {scan.Failed}, "
            + $"findings {audit.TotalFindings}, invalid {validations.Count(v => !v.Valid)}");
        sb.AppendLine();
        sb.AppendLine("Streaming is true. Maps are opened one at a time and not held open "
            + "concurrently, so the sweep never keeps a whole library in memory.");
        sb.AppendLine();

        sb.AppendLine("## Maps");
        sb.AppendLine();
        sb.AppendLine("| Map | Bytes | Target build | Loads | Entries | Diagnostics | Findings | Invalid |");
        sb.AppendLine("| --- | --- | --- | --- | --- | --- | --- | --- |");
        for (int i = 0; i < scans.Count; i++)
        {
            var s = scans[i];
            var a = audits[i];
            var v = validations[i];
            sb.AppendLine($"| {Cell(s.Map)} | {s.Bytes} | {Cell(s.TargetBuild ?? "unknown")} | "
                + $"{YesNo(s.Loads)} | {s.Entries} | {s.Diagnostics} | {a.Issues.Count} | {YesNo(!v.Valid)} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Findings by check");
        sb.AppendLine();
        if (audit.Checks.Count == 0)
        {
            sb.AppendLine("No findings.");
        }
        else
        {
            sb.AppendLine("| Check | Count |");
            sb.AppendLine("| --- | --- |");
            foreach (var c in audit.Checks)
                sb.AppendLine($"| {Cell(c.Check)} | {c.Count} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Validation failures");
        sb.AppendLine();
        bool anyFailed = false;
        for (int i = 0; i < validations.Count; i++)
        {
            var v = validations[i];
            if (v.Valid) continue;
            anyFailed = true;
            sb.AppendLine($"- {Cell(v.Map)}, {v.Errors} error(s)"
                + (v.Error is null ? "" : " " + Cell(v.Error)));
            foreach (var issue in v.Issues.Where(x => x.Severity == DiagnosticSeverity.Error))
                sb.AppendLine($"  - ({Cell(issue.Category)}) {Cell(issue.Message)}");
        }
        if (!anyFailed) sb.AppendLine("No map failed validation.");

        return sb.ToString();
    }

    private static string YesNo(bool value) => value ? "yes" : "no";

    /// <summary>Escape a table cell so a pipe in a name or message cannot break the table.</summary>
    private static string Cell(string value) => value.Replace("|", "\\|");
}
