// src/Wc3.Commands/ScriptModernizeCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One rewritten native. Count is how many occurrences were rewritten.</summary>
public sealed record NativeRewrite(string From, string To, int Line, int Count);

/// <summary>
/// The result of a modernize pass. NewScript is null when nothing was rewritten, so a caller knows
/// the script was left exactly as it was.
/// </summary>
public sealed record ScriptModernizeResult(
    string ScriptFile,
    string TargetBuild,
    int Rewrites,
    IReadOnlyList<NativeRewrite> Changes,
    string? NewScript);

/// <summary>
/// Rewrites deprecated or renamed natives in the script to the target build's spelling, where a
/// mapping is known, and leaves everything else byte for byte as it was.
///
/// A mapping is only applied when the target build declares the new spelling (the same membership
/// test compat uses), so the pass can never rewrite a working call to a name that build does not
/// have. The pairs come from the curated <see cref="NativeRenames"/> table, and a name it does not
/// hold is untouched. A rewrite only touches identifier tokens outside strings and comments, so a
/// native name inside a message or a comment stays as the author wrote it.
/// </summary>
public static class ScriptModernizeCommand
{
    public static ScriptModernizeResult Execute(MapDocument doc, string target, string? schemasDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        var (file, source, isLua) = ScriptAnalysis.Read(doc);
        var result = Analyze(file, source, isLua, schema);
        if (result.Rewrites > 0) ScriptCommand.Write(doc, result.NewScript!);
        return result;
    }

    internal static ScriptModernizeResult Analyze(string scriptFile, string source, bool isLua, GameSchema schema)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (from, to) in NativeRenames.All)
        {
            if (!source.Contains(from, StringComparison.Ordinal)) continue;
            if (!TargetCompatCommand.TargetDeclares(to, schema)) continue;
            map[from] = to;
        }

        if (map.Count == 0)
            return new(scriptFile, schema.Build, 0, Array.Empty<NativeRewrite>(), null);

        var (text, hits) = ScriptAnalysis.RewriteIdentifiers(source, isLua, map);
        var changes = hits
            .GroupBy(h => (h.From, h.To))
            .Select(g => new NativeRewrite(g.Key.From, g.Key.To, g.Min(h => h.Line), g.Count()))
            .OrderBy(c => c.Line)
            .ThenBy(c => c.From, StringComparer.Ordinal)
            .ToList();
        return new(scriptFile, schema.Build, hits.Count, changes, text);
    }
}
