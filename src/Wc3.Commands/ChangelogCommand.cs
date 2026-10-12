// src/Wc3.Commands/ChangelogCommand.cs
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>One commit as the changelog sees it. Type is the Conventional Commit type or "other".</summary>
public sealed record ChangelogEntry(string Type, string Scope, string Subject, string Hash);

/// <summary>One heading with the subjects under it.</summary>
public sealed record ChangelogGroup(string Type, IReadOnlyList<ChangelogEntry> Entries);

/// <summary>The release notes, the group list and the rendered markdown.</summary>
public sealed record ChangelogResult(
    bool Ok,
    string From,
    string To,
    IReadOnlyList<ChangelogGroup> Groups,
    string Markdown,
    string? Message);

/// <summary>
/// Generates release notes from git log between two refs, grouped by Conventional Commit type.
/// The types feat, fix, perf, docs and chore come first in that order, every other recognised type
/// follows alphabetically, and a subject that is not a Conventional Commit lands under "other".
/// Read-only over git, it only reads the log.
/// </summary>
public static partial class ChangelogCommand
{
    public const string OtherType = "other";

    /// <summary>The five types named first, in the order the release notes present them.</summary>
    private static readonly string[] Preferred = { "feat", "fix", "perf", "docs", "chore" };

    private const char FieldSeparator = '\u001f';

    [GeneratedRegex(@"^(?<type>[a-zA-Z]+)(\((?<scope>[^)]*)\))?(?<bang>!)?:\s*(?<subject>.+)$")]
    private static partial Regex Conventional();

    public static ChangelogResult Execute(string repoDir, string? from, string? to)
    {
        if (!Directory.Exists(repoDir))
            return Fail(from ?? string.Empty, to ?? string.Empty, $"no directory at {repoDir}");

        string toRef = string.IsNullOrWhiteSpace(to) ? "HEAD" : to!;
        string? fromRef = string.IsNullOrWhiteSpace(from) ? LatestTag(repoDir, toRef) : from;
        string range = fromRef is { Length: > 0 } ? $"{fromRef}..{toRef}" : toRef;

        var (exit, stdout, stderr) = RunGit(repoDir, "log", "--no-merges", $"--format=%H{FieldSeparator}%s", range);
        if (exit != 0)
            return Fail(fromRef ?? string.Empty, toRef,
                string.IsNullOrWhiteSpace(stderr) ? "git log failed" : stderr.Trim());

        var entries = stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseEntry)
            .ToList();
        var groups = Group(entries);
        return new ChangelogResult(true, fromRef ?? string.Empty, toRef, groups,
            Render(groups, fromRef, toRef), null);
    }

    /// <summary>Parse one "hash\u001fsubject" line into an entry, unparseable subjects as "other".</summary>
    public static ChangelogEntry ParseEntry(string line)
    {
        int sep = line.IndexOf(FieldSeparator);
        string hash = sep >= 0 ? line[..sep] : line;
        string subject = sep >= 0 ? line[(sep + 1)..] : string.Empty;
        string shortHash = hash.Length > 7 ? hash[..7] : hash;

        var m = Conventional().Match(subject);
        if (!m.Success)
            return new ChangelogEntry(OtherType, string.Empty, subject, shortHash);

        return new ChangelogEntry(
            m.Groups["type"].Value.ToLowerInvariant(),
            m.Groups["scope"].Value,
            m.Groups["subject"].Value.Trim(),
            shortHash);
    }

    /// <summary>Group the entries, the five named types first then the rest, "other" last.</summary>
    public static IReadOnlyList<ChangelogGroup> Group(IReadOnlyList<ChangelogEntry> entries)
    {
        var order = Preferred
            .Concat(entries.Select(e => e.Type)
                .Where(t => !Preferred.Contains(t, StringComparer.Ordinal) && t != OtherType)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(t => t, StringComparer.Ordinal))
            .Append(OtherType)
            .ToList();

        var groups = new List<ChangelogGroup>();
        foreach (var type in order)
        {
            var items = entries.Where(e => string.Equals(e.Type, type, StringComparison.Ordinal)).ToList();
            if (items.Count > 0) groups.Add(new ChangelogGroup(type, items));
        }
        return groups;
    }

    private static ChangelogResult Fail(string from, string to, string message) =>
        new(false, from, to, Array.Empty<ChangelogGroup>(), string.Empty, message);

    private static string Render(IReadOnlyList<ChangelogGroup> groups, string? from, string? to)
    {
        var sb = new StringBuilder();
        sb.Append("# Release notes\n\n");
        sb.Append(from is { Length: > 0 }
            ? $"Commits from {from} to {to}.\n"
            : $"Commits up to {to}.\n");
        foreach (var group in groups)
        {
            sb.Append('\n').Append("## ").Append(group.Type).Append('\n');
            foreach (var entry in group.Entries)
            {
                sb.Append("* ").Append(entry.Subject);
                if (entry.Scope.Length > 0) sb.Append(" (").Append(entry.Scope).Append(')');
                sb.Append('\n');
            }
        }
        return sb.ToString();
    }

    private static string? LatestTag(string repoDir, string toRef)
    {
        var (exit, stdout, _) = RunGit(repoDir, "describe", "--tags", "--abbrev=0", toRef + "^");
        return exit == 0 && !string.IsNullOrWhiteSpace(stdout) ? stdout.Trim() : null;
    }

    private static (int Exit, string Stdout, string Stderr) RunGit(string repoDir, params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            CreateNoWindow = true,
            WorkingDirectory = repoDir,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);
        try
        {
            using var p = Process.Start(psi) ?? throw new InvalidOperationException("could not start git");
            p.StandardInput.Close();
            var errTask = p.StandardError.ReadToEndAsync();
            string stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return (p.ExitCode, stdout, errTask.Result);
        }
        catch (Exception ex)
        {
            return (127, string.Empty, ex.Message);
        }
    }
}
