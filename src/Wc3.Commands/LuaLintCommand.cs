// src/Wc3.Commands/LuaLintCommand.cs
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One problem the lint found, with the line it is on.</summary>
public sealed record LuaLintFinding(string Kind, int Line, string Detail);

/// <summary>Everything the lint checked and the problems it found.</summary>
public sealed record LuaLintResult(
    string ScriptFile,
    string TargetBuild,
    int Lines,
    int Calls,
    IReadOnlyList<LuaLintFinding> Findings,
    IReadOnlyDictionary<string, int> Summary);

/// <summary>
/// Checks a Lua map script against the target build's native and Blizzard.j signatures, the checks
/// the game itself cannot do at save time. Lua's on-save syntax check cannot see a wrong parameter
/// type, so passing <c>nil</c> where a native wants a string is a runtime error that aborts the
/// statement, and a spell aborting mid-cast is how a caster ends up stuck. This reports
/// <list type="bullet">
/// <item><c>nil-string</c>, a literal <c>nil</c> passed to a parameter the schema types as string.</item>
/// <item><c>argument-count</c>, a call whose argument count does not match the declaration.</item>
/// <item><c>unknown-function</c>, a call to a global that is neither the map's own function, a
/// native, a schema function, nor a Lua builtin, which is a converter that dropped a function.</item>
/// </list>
/// The names are read from the parsed tree, so only a real call is checked. Uses the same schema
/// resolution as compat and lua-api.
/// </summary>
public static class LuaLintCommand
{
    public static LuaLintResult Execute(MapDocument doc, string target = "3.0.0.24268", string? schemasDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        var (file, source, isLua) = LuaScriptReader.Read(doc);
        if (!isLua)
            return new(file, schema.Build, 0, 0, Array.Empty<LuaLintFinding>(), new Dictionary<string, int>());
        var chunk = LuaScriptReader.TryParse(source, out var error);
        if (chunk is null)
            throw new InvalidOperationException(error?.ToString() ?? "the Lua script did not parse");

        var stringParams = new Dictionary<string, HashSet<int>>(StringComparer.Ordinal);
        var arity = new Dictionary<string, int>(StringComparer.Ordinal);
        void Add(string name, IReadOnlyList<JassParameter> ps)
        {
            var s = new HashSet<int>();
            for (int i = 0; i < ps.Count; i++)
                if (ps[i].Type == "string") s.Add(i);
            if (s.Count > 0) stringParams[name] = s;
            arity[name] = ps.Count;
        }
        if (schema.Script is not null)
            foreach (var api in schema.Script.Values)
            {
                foreach (var n in api.Natives) Add(n.Name, n.Parameters);
                foreach (var f in api.Functions) Add(f.Name, f.Parameters);
            }

        var own = new HashSet<string>(LuaScriptIndex.Functions(chunk, source).Select(f => f.Name), StringComparer.Ordinal);
        var findings = new List<LuaLintFinding>();
        int calls = 0;
        LuaScriptIndex.Walk(chunk.Body, null, (stmt, _) =>
        {
            foreach (var e in LuaScriptIndex.TopExprs(stmt))
                foreach (var call in LuaScriptReader.Calls(e))
                {
                    var name = LuaScriptReader.CalleeName(call.Func);
                    if (name is null || name.Contains('.')) continue;
                    calls++;
                    if (arity.TryGetValue(name, out int want))
                    {
                        if (call.Args.Count != want)
                            findings.Add(new("argument-count", call.Line,
                                $"{name} declares {want} parameter(s), {call.Args.Count} given"));
                        if (stringParams.TryGetValue(name, out var sp))
                            foreach (var i in sp)
                                if (i < call.Args.Count && call.Args[i] is LiteralExpr { Kind: "nil" })
                                    findings.Add(new("nil-string", call.Line,
                                        $"{name} argument {i + 1} is nil but the parameter is a string"));
                    }
                    else if (!own.Contains(name) && !LuaApiNames.NonNative.Contains(name))
                    {
                        findings.Add(new("unknown-function", call.Line,
                            $"{name} is called but is not defined, not a native, and not a schema function"));
                    }
                }
        });

        var summary = findings
            .GroupBy(f => f.Kind, StringComparer.Ordinal)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
        return new(file, schema.Build, source.Count(c => c == '\n') + 1, calls, findings, summary);
    }
}
