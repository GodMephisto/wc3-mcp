// src/Wc3.Commands/LuaApiCommand.cs
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One native or function a Lua script calls that the target build does not declare.
/// <paramref name="Name"/> is the Lua spelling, <paramref name="JassSpelling"/> the JASS name the
/// schema was asked about.</summary>
public sealed record LuaApiEntry(string Name, string JassSpelling, string Kind, int Line, int Count);

/// <summary>The natives and functions a Lua script calls that the target build is missing.</summary>
public sealed record LuaApiResult(
    string ScriptFile,
    string TargetBuild,
    string Mapping,
    int Used,
    int Missing,
    IReadOnlyList<LuaApiEntry> Usages);

/// <summary>
/// Reports the natives and functions a Lua script calls that the target build does not declare, the
/// compile-time side of a port. The Lua twin of <see cref="ScriptApiUsageCommand"/>, sharing the
/// same target schema resolution (<see cref="TargetBuildSettings.LoadSchema"/>), the same union
/// index (<see cref="TargetCompatCommand.LoadIdentifierIndex"/>) and the same membership test
/// (<see cref="TargetCompatCommand.TargetDeclares"/>), so this command and compat can never
/// disagree about what a build declares.
///
/// The names are read from the parsed tree, not from a line scan, so only a real call is counted.
/// A name the map itself declares, a Lua library call, and a call to <c>FourCC</c> or a standard
/// library function are all skipped. The mapping from the Lua spelling to the JASS spelling is
/// <see cref="LuaApiNames"/>, described in the result so the answer says how it read the names.
/// </summary>
public static class LuaApiCommand
{
    public static LuaApiResult Execute(MapDocument doc, string target, string? schemasDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        var index = TargetCompatCommand.LoadIdentifierIndex(schemasDir);
        var (file, source, isLua) = LuaScriptReader.Read(doc);
        if (!isLua)
            return new(file, schema.Build, LuaApiNames.Description, 0, 0, Array.Empty<LuaApiEntry>());
        var chunk = LuaScriptReader.TryParse(source, out var error);
        if (chunk is null)
            return new(file, schema.Build, LuaApiNames.Description, 0, 0, Array.Empty<LuaApiEntry>());
        return Analyze(file, source, chunk, schema, index);
    }

    internal static LuaApiResult Analyze(
        string scriptFile, string source, LuaChunk chunk, GameSchema schema, TargetCompatCommand.IdentifierIndex index)
    {
        var own = new HashSet<string>(LuaScriptIndex.Functions(chunk, source).Select(f => f.Name), StringComparer.Ordinal);
        var seen = new Dictionary<string, (string Jass, string Kind, int Line, int Count)>(StringComparer.Ordinal);

        LuaScriptIndex.Walk(chunk.Body, null, (stmt, _) =>
        {
            foreach (var e in LuaScriptIndex.TopExprs(stmt))
                foreach (var call in LuaScriptReader.Calls(e))
                {
                    if (LuaScriptReader.CalleeName(call.Func) is not { } name) continue;
                    if (name.Contains('.')) continue;                 // a field call, not a native name
                    if (own.Contains(name)) continue;                 // the map's own function
                    if (LuaApiNames.NonNative.Contains(name)) continue;
                    string jass = LuaApiNames.ToJassSpelling(name);
                    if (!index.Kinds.TryGetValue(jass, out var kind)) continue;   // not a native or function in any build
                    if (kind != "native" && kind != "function") continue;
                    if (seen.TryGetValue(name, out var prev))
                        seen[name] = (jass, kind, prev.Line, prev.Count + 1);
                    else
                        seen[name] = (jass, kind, call.Line, 1);
                }
        });

        var usages = new List<LuaApiEntry>();
        int used = 0;
        foreach (var (name, info) in seen)
        {
            used++;
            if (TargetCompatCommand.TargetDeclares(info.Jass, schema)) continue;
            usages.Add(new LuaApiEntry(name, info.Jass, info.Kind, info.Line, info.Count));
        }
        usages = usages.OrderBy(u => u.Line).ThenBy(u => u.Name, StringComparer.Ordinal).ToList();
        return new LuaApiResult(scriptFile, schema.Build, LuaApiNames.Description, used, usages.Count, usages);
    }
}
