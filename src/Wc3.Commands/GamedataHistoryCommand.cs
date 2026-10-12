// src/Wc3.Commands/GamedataHistoryCommand.cs
namespace Wc3.Commands;

/// <summary>One kind of declaration a name was found as, and the builds that carry it.</summary>
public sealed record GamedataHistoryKind(string Kind, IReadOnlyList<string> Builds);

/// <summary>
/// Where a name appears across every schema folder. Found is false when no build declares it in any
/// kind. FirstBuild and LastBuild are the oldest and newest build that declare it in any kind.
/// </summary>
public sealed record GamedataHistoryResult(
    string Name,
    bool Found,
    IReadOnlyList<GamedataHistoryKind> Kinds,
    IReadOnlyList<string> ObjectFiles,
    string? FirstBuild,
    string? LastBuild,
    int BuildsSearched);

/// <summary>
/// Reports whether one name is an object field code, a native, a type, a global or a function in any
/// recorded build, lists every build where each kind appears, and names the first and last build that
/// carry the name.
/// </summary>
public static class GamedataHistoryCommand
{
    /// <summary>The kinds, in the fixed order they are reported.</summary>
    public static readonly IReadOnlyList<string> KindOrder = new[]
    {
        "field", "native", "type", "global", "function",
    };

    public static GamedataHistoryResult Execute(string name, string? schemasDir = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("a name is required", nameof(name));

        var schemas = GamedataSupport.LoadAll(schemasDir);
        var kindBuilds = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var objectFiles = new List<string>();
        var buildsWithName = new List<string>();

        foreach (var one in schemas)
        {
            var slks = new List<string>();
            bool any = false;
            foreach (var kind in KindOrder)
            {
                bool hit = kind == "field"
                    ? HasObjectField(one.Schema, name, slks)
                    : HasScriptName(one.Schema, name, kind);
                if (!hit) continue;
                if (!kindBuilds.TryGetValue(kind, out var builds))
                {
                    builds = new List<string>();
                    kindBuilds[kind] = builds;
                }
                builds.Add(one.Build);
                any = true;
            }
            if (any) buildsWithName.Add(one.Build);
            foreach (var slk in slks)
                if (!objectFiles.Contains(slk)) objectFiles.Add(slk);
        }

        var kinds = KindOrder
            .Where(kindBuilds.ContainsKey)
            .Select(kind => new GamedataHistoryKind(kind, kindBuilds[kind]))
            .ToList();

        return new GamedataHistoryResult(
            name,
            kinds.Count > 0,
            kinds,
            objectFiles,
            buildsWithName.Count > 0 ? buildsWithName[0] : null,
            buildsWithName.Count > 0 ? buildsWithName[^1] : null,
            schemas.Count);
    }

    /// <summary>True when any object metadata file declares the code, filling in the file short names.</summary>
    private static bool HasObjectField(GameSchema schema, string name, List<string> slkFiles)
    {
        if (schema.Objects is null) return false;
        bool hit = false;
        foreach (var kv in schema.Objects)
        {
            if (!kv.Value.Any(f => string.Equals(f.Code, name, StringComparison.Ordinal))) continue;
            slkFiles.Add(kv.Key);
            hit = true;
        }
        return hit;
    }

    /// <summary>True when any script file declares the name in the named kind.</summary>
    private static bool HasScriptName(GameSchema schema, string name, string kind)
    {
        if (schema.Script is null) return false;
        foreach (var api in schema.Script.Values)
        {
            bool hit = kind switch
            {
                "native" => api.Natives.Any(n => string.Equals(n.Name, name, StringComparison.Ordinal)),
                "type" => api.Types.Any(t => string.Equals(t.Name, name, StringComparison.Ordinal)),
                "global" => api.Globals.Any(g => string.Equals(g.Name, name, StringComparison.Ordinal)),
                "function" => api.Functions.Any(f => string.Equals(f.Name, name, StringComparison.Ordinal)),
                _ => false,
            };
            if (hit) return true;
        }
        return false;
    }
}
