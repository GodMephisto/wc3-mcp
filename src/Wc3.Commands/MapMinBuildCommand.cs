// src/Wc3.Commands/MapMinBuildCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One identifier the map uses, and the earliest build that declares it.</summary>
public sealed record MapBuildRequirement(string Name, string Earliest, int Line);

/// <summary>
/// The earliest build the open map runs on, from the same identifier union index the compat check
/// uses. MinimumBuild is null when the map has no script or uses no schema-known identifier, so
/// "no answer" is distinguishable from "runs on the oldest build".
/// </summary>
public sealed record MapMinBuildResult(
    bool Known, string? MinimumBuild, IReadOnlyList<MapBuildRequirement> Requirements,
    IReadOnlyList<string> Diagnostics)
{
    /// <summary>One-line summary for a status area, or null when there is nothing to say.</summary>
    public string? Summary => MinimumBuild is null
        ? null
        : $"Runs on build {MinimumBuild} or newer ({Requirements.Count} identifier(s) checked)";
}

/// <summary>
/// Finds the earliest build a map runs on. A map runs on a build when every identifier it uses is
/// declared there, so the earliest such build is the newest among the identifiers' own earliest
/// declaring builds, computed from the compat union index so the answer agrees with compat.
/// </summary>
public static class MapMinBuildCommand
{
    public static MapMinBuildResult Execute(MapDocument doc, string? schemasDir = null)
    {
        var diagnostics = new List<string>();
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out _, out var script, out var why))
        {
            diagnostics.Add(why);
            return new(false, null, Array.Empty<MapBuildRequirement>(), diagnostics);
        }

        // The same union index the compat check walks, so "when did this appear" agrees with a
        // compat finding. Each identifier's earliest declaring build is the head of its ordered
        // build list (oldest first, as TargetCompatCommand.LoadIdentifierIndex orders it).
        var index = TargetCompatCommand.LoadIdentifierIndex(schemasDir);
        var requirements = new List<MapBuildRequirement>();
        string? minimum = null;
        foreach (var (name, line, _) in TargetCompatCommand.UsedIdentifiers(script))
        {
            if (!index.Builds.TryGetValue(name, out var declaredBy) || declaredBy.Count == 0)
                continue;

            string earliest = declaredBy[0];
            requirements.Add(new MapBuildRequirement(name, earliest, line));
            if (minimum is null
                || GameBuildName.Compare(GameBuildName.Parse(earliest), GameBuildName.Parse(minimum)) > 0)
                minimum = earliest;
        }

        if (minimum is null)
        {
            diagnostics.Add(diagnostics.Count == 0
                ? "the map's script uses no identifier any schema declares"
                : "no schema-known identifier is used by the map's script");
            return new(false, null, requirements, diagnostics);
        }

        requirements.Sort((a, b) => GameBuildName.Compare(
            GameBuildName.Parse(a.Earliest), GameBuildName.Parse(b.Earliest)));
        return new(true, minimum, requirements, diagnostics);
    }
}
