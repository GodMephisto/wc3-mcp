// src/Wc3.Commands/ScriptCallGraphCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A directed call edge, caller to callee.</summary>
public sealed record CallEdge(string From, string To);

public sealed record ScriptCallGraphResult(
    string ScriptFile,
    int Functions,
    int Edges,
    IReadOnlyList<CallEdge> Calls);

/// <summary>
/// The call edges between functions a script declares, as data. The CLI renders them as a text
/// adjacency list or as Graphviz DOT; an MCP client gets the edge list.
///
/// Only edges between functions this script declares are included, so a native call or a call
/// through a local code value is not an edge. This is the same extraction the unused function
/// finder uses, see <see cref="JassCalls"/>, so the two cannot disagree about what calls what.
/// </summary>
public static class ScriptCallGraphCommand
{
    public static ScriptCallGraphResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var name, out var script, out _))
            return new(name, 0, 0, Array.Empty<CallEdge>());
        return Analyze(name, script);
    }

    internal static ScriptCallGraphResult Analyze(string scriptFile, string script)
    {
        var functions = JassFunctionIndex.Parse(script);
        var code = JassComments.Strip(JassLines.Split(script));
        var edges = JassCalls.Edges(code, functions, out _);

        var calls = new List<CallEdge>();
        foreach (var f in functions)
            foreach (var callee in edges[f.Name])
                calls.Add(new CallEdge(f.Name, callee));

        return new(scriptFile, functions.Count, calls.Count, calls);
    }
}
