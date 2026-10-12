// src/Wc3.Commands/ScriptUnusedFunctionsCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One function nothing reaches. Line is 1-based in war3map.j.</summary>
public sealed record UnusedFunction(string Name, int Line, string Signature);

public sealed record ScriptUnusedFunctionsResult(
    string ScriptFile,
    int Declared,
    int Unused,
    IReadOnlyList<UnusedFunction> Functions);

/// <summary>
/// Finds the functions a script declares that no other function calls and that nothing else
/// reaches.
///
/// A function is used when another function calls it or names it as a <c>code</c> value, when an
/// <c>InitTrig_*</c> function references it, or when a string literal passed to
/// <c>ExecuteFunc</c> names it. The engine's own entry points are never reported, whatever calls
/// them, since the game invokes them directly: <c>main</c>, <c>config</c>,
/// <c>InitCustomTriggers</c> and the map initialization functions are all excluded.
///
/// A function that only calls itself is reported, because the wording is "no OTHER function calls
/// it" and a self-recursive function with no external caller is unreachable. Call syntax is
/// shared with the call graph, see <see cref="JassCalls"/>.
/// </summary>
public static class ScriptUnusedFunctionsCommand
{
    public static ScriptUnusedFunctionsResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var name, out var script, out _))
            return new(name, 0, 0, Array.Empty<UnusedFunction>());
        return Analyze(name, script);
    }

    internal static ScriptUnusedFunctionsResult Analyze(string scriptFile, string script)
    {
        var functions = JassFunctionIndex.Parse(script);
        var code = JassComments.Strip(JassLines.Split(script));
        var edges = JassCalls.Edges(code, functions, out var executeFuncNames);

        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (caller, callees) in edges)
            foreach (var callee in callees)
            {
                if (callee != caller) used.Add(callee);                       // called by another
                if (caller.StartsWith("InitTrig_", StringComparison.Ordinal))
                    used.Add(callee);                                          // an InitTrig_ root
            }
        used.UnionWith(executeFuncNames);
        used.UnionWith(ScriptStripCommand.EngineEntryPoints);

        var unused = functions
            .Where(f => !used.Contains(f.Name))
            .Select(f => new UnusedFunction(f.Name, f.StartLine, f.Signature))
            .OrderBy(f => f.Line)
            .ToList();
        return new(scriptFile, functions.Count, unused.Count, unused);
    }
}
