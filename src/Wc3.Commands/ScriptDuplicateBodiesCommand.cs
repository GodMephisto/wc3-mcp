// src/Wc3.Commands/ScriptDuplicateBodiesCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One function in a duplicate group. Line is its 1-based declaration line.</summary>
public sealed record DuplicateBodyFunction(string Name, int Line);

/// <summary>A set of functions whose normalized bodies are identical.</summary>
public sealed record DuplicateBodyGroup(int Size, IReadOnlyList<DuplicateBodyFunction> Functions);

/// <summary>Functions grouped by normalized body, every group holding at least two functions.</summary>
public sealed record ScriptDuplicateBodiesResult(
    string ScriptFile,
    string Language,
    int Functions,
    int DuplicateGroups,
    int DuplicateFunctions,
    IReadOnlyList<DuplicateBodyGroup> Groups);

/// <summary>
/// Finds functions whose bodies are the same once comments and whitespace are removed and local
/// variables are normalized by declaration position.
///
/// A copied function is a maintenance trap, a fix in one copy silently misses the others, and a
/// large script accumulates them. Normalizing local names matters because the common copy is then
/// edited by hand, a variable renamed in one copy, and a byte comparison would miss the pair. The
/// signature line is dropped, since two functions with the same body but different names are
/// exactly the case this should catch.
/// </summary>
public static class ScriptDuplicateBodiesCommand
{
    public static ScriptDuplicateBodiesResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = ScriptAnalysis.Read(doc);
        return Analyze(file, source, isLua);
    }

    internal static ScriptDuplicateBodiesResult Analyze(string scriptFile, string source, bool isLua)
    {
        var functions = ScriptAnalysis.Functions(source, isLua);
        var lineStarts = ScriptCommand.ComputeLineStarts(source);

        var bodies = functions
            .Select(f => (Function: f, Body: ScriptAnalysis.CanonicalBody(source, lineStarts, f, isLua)))
            .Where(t => t.Body.Length > 0)
            .ToList();

        var groups = bodies
            .GroupBy(t => t.Body, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => new DuplicateBodyGroup(
                g.Count(),
                g.OrderBy(t => t.Function.StartLine)
                    .Select(t => new DuplicateBodyFunction(t.Function.Name, t.Function.StartLine))
                    .ToList()))
            .OrderByDescending(g => g.Size)
            .ThenBy(g => g.Functions[0].Line)
            .ToList();

        return new(
            scriptFile, isLua ? "lua" : "jass",
            functions.Count, groups.Count, groups.Sum(g => g.Size), groups);
    }
}
