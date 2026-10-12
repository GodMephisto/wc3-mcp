// src/Wc3.Commands/DocsReference.cs
using System.Text;

namespace Wc3.Commands;

/// <summary>One command line argument as the generated reference shows it. Variadic means it accepts
/// more than one value, so the usage token carries a trailing ellipsis.</summary>
public sealed record DocArgument(string Name, string? Description, bool Required, bool Variadic);

/// <summary>One command line option. Names is the full display token list, for example
/// "-o, --out <out>", already carrying the value placeholder when the option takes one.</summary>
public sealed record DocOption(string Names, string? Description, string? Default, bool Required);

/// <summary>One command in the tree, its own arguments and options, and its children. Runnable is
/// true when the command carries a handler, so a group that also runs directly is marked as both.</summary>
public sealed record DocCommand(
    string Path,
    string Name,
    string? Description,
    bool Runnable,
    IReadOnlyList<DocArgument> Arguments,
    IReadOnlyList<DocOption> Options,
    IReadOnlyList<DocCommand> Subcommands);

/// <summary>One MCP tool with its description and the four annotation hints. A null hint means the
/// tool left that annotation at its default.</summary>
public sealed record DocTool(
    string Name, string? Description,
    bool? ReadOnly, bool? Destructive, bool? Idempotent, bool? OpenWorld);

/// <summary>The line based difference between a committed document and a fresh render. Identical is
/// true when the two agree once line endings are normalised. Added and Removed count the lines that
/// appear only in the new text and only in the old text.</summary>
public sealed record DocFileDiff(bool Identical, int Added, int Removed);

/// <summary>
/// Renders the two reference documents from plain records, so the walk of the live surfaces lives
/// in the front-end that owns each surface while the formatting lives here and is testable on its
/// own. Nothing here reads a file or a command tree, which is what lets the CI guard regenerate the
/// same text in memory and compare it byte for byte.
///
/// The documents are deterministic. No date, no absolute path and no machine name appears in them,
/// so a re-render on any machine produces the committed bytes and a stale document fails the guard.
/// </summary>
public static class DocsReference
{
    /// <summary>Render the command reference. TopLevel is one entry per verb, GlobalOptions are the
    /// options every command inherits and are shown once.</summary>
    public static string RenderCli(IReadOnlyList<DocCommand> topLevel, IReadOnlyList<DocOption> globalOptions)
    {
        var sb = new StringBuilder();
        sb.Append("# wc3ctl command reference\n\n");
        sb.Append("Generated from the live command tree by `wc3ctl docs cli`. Do not edit by hand. ");
        sb.Append("Regenerate with `wc3ctl docs cli --force`, and the CI guard fails when this file is stale.\n\n");
        int nodes = topLevel.Sum(CountNodes);
        sb.Append($"The tree holds {topLevel.Count} top level verb(s) and {nodes} command path(s) in total.\n\n");

        sb.Append("## Global options\n\n");
        sb.Append("Present on every command.\n\n");
        foreach (var o in globalOptions) sb.Append(OptionLine(o)).Append('\n');
        sb.Append('\n');
        sb.Append("Every command also accepts `-h` and `--help`, and the root accepts `--version`.\n\n");

        foreach (var c in topLevel) AppendCommand(sb, c, 2);
        return sb.ToString();
    }

    /// <summary>Render the MCP reference, one section per tool in the order it is given.</summary>
    public static string RenderMcp(IReadOnlyList<DocTool> tools)
    {
        var sb = new StringBuilder();
        sb.Append("# wc3ctl MCP tool reference\n\n");
        sb.Append("Generated from the live tool surface by `wc3ctl docs mcp`. Do not edit by hand. ");
        sb.Append("Regenerate with `wc3ctl docs mcp --force`, and the CI guard fails when this file is stale.\n\n");
        sb.Append($"The server exposes {tools.Count} tool(s).\n\n");
        foreach (var t in tools)
        {
            sb.Append("## ").Append(t.Name).Append("\n\n");
            sb.Append(string.IsNullOrWhiteSpace(t.Description) ? "_No description._" : t.Description).Append("\n\n");
            sb.Append("Annotations. read-only ").Append(Hint(t.ReadOnly))
                .Append(", destructive ").Append(Hint(t.Destructive))
                .Append(", idempotent ").Append(Hint(t.Idempotent))
                .Append(", open-world ").Append(Hint(t.OpenWorld)).Append(".\n\n");
        }
        return sb.ToString();
    }

    /// <summary>Line based difference used for the write summary. Line endings are normalised first,
    /// so a document checked out with CRLF and a freshly rendered LF text compare equal.</summary>
    public static DocFileDiff Diff(string oldText, string newText)
    {
        var oldLines = Lines(oldText);
        var newLines = Lines(newText);
        var remaining = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var l in oldLines) remaining[l] = remaining.TryGetValue(l, out var n) ? n + 1 : 1;
        int added = 0;
        foreach (var l in newLines)
        {
            if (remaining.TryGetValue(l, out var n) && n > 0) remaining[l] = n - 1;
            else added++;
        }
        int removed = remaining.Values.Sum();
        return new DocFileDiff(added == 0 && removed == 0, added, removed);
    }

    private static List<string> Lines(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();

    private static int CountNodes(DocCommand c) => 1 + c.Subcommands.Sum(CountNodes);

    private static string Hint(bool? value) => value switch { true => "yes", false => "no", _ => "unset" };

    private static void AppendCommand(StringBuilder sb, DocCommand c, int level)
    {
        sb.Append(new string('#', level)).Append(' ').Append(c.Path).Append("\n\n");
        sb.Append(string.IsNullOrWhiteSpace(c.Description) ? "_No description._" : c.Description).Append("\n\n");

        sb.Append("Usage\n\n```\n").Append(Usage(c)).Append("\n```\n\n");

        if (c.Arguments.Count > 0)
        {
            sb.Append("Arguments\n\n");
            foreach (var a in c.Arguments) sb.Append(ArgumentLine(a)).Append('\n');
            sb.Append('\n');
        }
        if (c.Options.Count > 0)
        {
            sb.Append("Options\n\n");
            foreach (var o in c.Options) sb.Append(OptionLine(o)).Append('\n');
            sb.Append('\n');
        }
        if (c.Subcommands.Count > 0)
        {
            sb.Append("Subcommands\n\n");
            foreach (var s in c.Subcommands)
                sb.Append("- `").Append(s.Name).Append("`.")
                    .Append(' ').Append(string.IsNullOrWhiteSpace(s.Description) ? "No description." : s.Description)
                    .Append('\n');
            sb.Append('\n');
            foreach (var s in c.Subcommands) AppendCommand(sb, s, level + 1);
        }
    }

    private static string Usage(DocCommand c)
    {
        var parts = new List<string> { "wc3ctl", c.Path };
        foreach (var a in c.Arguments)
        {
            string inner = a.Variadic ? a.Name + "..." : a.Name;
            parts.Add(a.Required ? $"<{inner}>" : $"[<{inner}>]");
        }
        if (c.Options.Count > 0) parts.Add("[options]");
        return string.Join(' ', parts);
    }

    private static string ArgumentLine(DocArgument a)
    {
        string inner = a.Variadic ? a.Name + "..." : a.Name;
        string token = a.Required ? $"<{inner}>" : $"[<{inner}>]";
        string mark = a.Required ? " (required)" : " (optional)";
        string desc = string.IsNullOrWhiteSpace(a.Description) ? "No description." : a.Description;
        return $"- `{token}`{mark}. {desc}";
    }

    private static string OptionLine(DocOption o)
    {
        string mark = o.Required ? " (required)" : "";
        string desc = string.IsNullOrWhiteSpace(o.Description) ? "No description." : o.Description;
        string dflt = o.Default is null ? "" : $" Default `{o.Default}`.";
        return $"- `{o.Names}`{mark}. {desc}{dflt}";
    }
}
