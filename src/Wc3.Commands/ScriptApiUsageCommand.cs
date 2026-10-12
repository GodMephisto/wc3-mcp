// src/Wc3.Commands/ScriptApiUsageCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One native or function the script uses that the target build does not declare.</summary>
public sealed record ApiUsageEntry(string Name, string Kind, int Line, int Count);

/// <summary>
/// The natives and functions a script uses that a target build is missing, with the count checked.
/// </summary>
public sealed record ScriptApiUsageResult(
    string ScriptFile,
    string TargetBuild,
    int Used,
    int Missing,
    IReadOnlyList<ApiUsageEntry> Usages);

/// <summary>
/// Reports the natives and functions a script uses that the target build does not declare, the
/// compile-time side of a port.
///
/// It reuses the compat check's identifier scanner (<see cref="TargetCompatCommand.UsedIdentifiers"/>)
/// and the same union index and membership test (<see cref="TargetCompatCommand.LoadIdentifierIndex"/>,
/// <see cref="TargetCompatCommand.TargetDeclares"/>), so this command and <c>compat</c> can never
/// disagree about what a build declares. Identifiers the map itself declares are skipped, and an
/// unknown name that no schema declares in any build is skipped too, exactly as compat does.
/// </summary>
public static class ScriptApiUsageCommand
{
    public static ScriptApiUsageResult Execute(MapDocument doc, string target, string? schemasDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        var index = TargetCompatCommand.LoadIdentifierIndex(schemasDir);
        var (file, source, _) = ScriptAnalysis.Read(doc);
        return Analyze(file, source, schema, index);
    }

    internal static ScriptApiUsageResult Analyze(
        string scriptFile, string source, GameSchema schema, TargetCompatCommand.IdentifierIndex index)
    {
        var usages = new List<ApiUsageEntry>();
        int used = 0;
        foreach (var (name, line, count) in TargetCompatCommand.UsedIdentifiers(source))
        {
            // Unknown to every build, so not a native or a function and not a finding.
            if (!index.Kinds.TryGetValue(name, out var kind)) continue;
            // The spec is natives and functions, a missing global or type is compat's finding.
            if (kind != "native" && kind != "function") continue;

            used++;
            if (TargetCompatCommand.TargetDeclares(name, schema)) continue;
            usages.Add(new ApiUsageEntry(name, kind, line, count));
        }

        usages = usages
            .OrderBy(u => u.Line)
            .ThenBy(u => u.Name, StringComparer.Ordinal)
            .ToList();
        return new(scriptFile, schema.Build, used, usages.Count, usages);
    }
}
