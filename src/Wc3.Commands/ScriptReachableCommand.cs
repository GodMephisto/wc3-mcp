// src/Wc3.Commands/ScriptReachableCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One function in the reachability report. Line is its 1-based declaration line.</summary>
public sealed record ReachableFunction(string Name, int Line, string Signature);

/// <summary>
/// The functions reachable from the script's real entry points, and the dead ones nothing reaches.
/// </summary>
public sealed record ScriptReachableResult(
    string ScriptFile,
    string Language,
    int Reachable,
    int Dead,
    IReadOnlyList<ReachableFunction> ReachableFunctions,
    IReadOnlyList<ReachableFunction> DeadFunctions)
{
    /// <summary>Share of declared functions that nothing reaches.</summary>
    public double DeadPercent =>
        Reachable + Dead == 0 ? 0 : Math.Round(100.0 * Dead / (Reachable + Dead), 1);
}

/// <summary>
/// Walks the call graph from the two entry points a Warcraft III map always has, <c>main</c> and
/// the <c>InitTrig_*</c> functions, and reports what the walk reaches and what it does not.
///
/// The call edges come from <see cref="JassCalls"/>, the same extraction the call graph and the
/// unused-function finder use, so the three cannot disagree about what calls what. Unlike
/// unused-functions this pass roots only at the declared entry points, so a function reached solely
/// by a string literal (<c>ExecuteFunc("Name")</c>) is reported dead here. That is the honest answer
/// to "what does main reach", and <c>script roots</c> exists to find the string-dispatched names a
/// dead-code pass must also pin.
/// </summary>
public static class ScriptReachableCommand
{
    public static ScriptReachableResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = ScriptAnalysis.Read(doc);
        return Analyze(file, source, isLua);
    }

    internal static ScriptReachableResult Analyze(string scriptFile, string source, bool isLua)
    {
        var functions = ScriptAnalysis.Functions(source, isLua);
        var code = ScriptAnalysis.CodeLines(source, isLua);
        var edges = ScriptAnalysis.Edges(code, functions);

        // Roots. main is the engine's own entry point, and every InitTrig_* is one it calls
        // directly. A name reached only through a string literal is deliberately not a root here.
        var roots = new Queue<string>();
        foreach (var f in functions)
            if (f.Name == "main" || f.Name.StartsWith("InitTrig", StringComparison.Ordinal))
                roots.Enqueue(f.Name);

        var reachable = new HashSet<string>(StringComparer.Ordinal);
        while (roots.Count > 0)
        {
            var fn = roots.Dequeue();
            if (!reachable.Add(fn)) continue;
            if (!edges.TryGetValue(fn, out var callees)) continue;
            foreach (var callee in callees)
                if (!reachable.Contains(callee)) roots.Enqueue(callee);
        }

        var reach = new List<ReachableFunction>();
        var dead = new List<ReachableFunction>();
        foreach (var f in functions.OrderBy(f => f.StartLine))
        {
            var row = new ReachableFunction(f.Name, f.StartLine, f.Signature);
            if (reachable.Contains(f.Name)) reach.Add(row);
            else dead.Add(row);
        }

        return new(scriptFile, isLua ? "lua" : "jass", reach.Count, dead.Count, reach, dead);
    }
}
