// src/Wc3.Commands/CompatExplainCommand.cs
namespace Wc3.Commands;

/// <summary>
/// Everything known about one identifier against one target build. IsIdentifier is false when no
/// schema declares the name at all, which reads differently from a name that exists but this
/// target lacks. DeclaredInTarget is true when the target compiles it. AddedIn is the earliest
/// release build newer than the target that declares it, FirstSeen the earliest test build that
/// does. Builds lists every build that declares it, oldest first.
/// </summary>
public sealed record CompatExplainResult(
    string Name,
    bool IsIdentifier,
    string? Kind,
    bool DeclaredInTarget,
    string TargetBuild,
    IReadOnlyList<string> Builds,
    string? AddedIn,
    string? FirstSeen);

/// <summary>
/// Explain one identifier against a target build. Reuses the compat union index and the build
/// ordering rules, so its answer about when a name appeared matches what a compat finding says.
/// </summary>
public static class CompatExplainCommand
{
    public static CompatExplainResult Execute(string name, string target, string? schemasDir = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("an identifier name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(target))
            throw new ArgumentException("a target build is required", nameof(target));

        var index = TargetCompatCommand.LoadIdentifierIndex(schemasDir);
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);

        bool known = index.Kinds.TryGetValue(name, out var kind);
        var builds = index.Builds.TryGetValue(name, out var declaredBy)
            ? declaredBy
            : Array.Empty<string>();
        bool declared = known && TargetCompatCommand.TargetDeclares(name, schema);

        string? addedIn = null;
        string? firstSeen = null;
        if (known)
            (addedIn, firstSeen) = TargetCompatCommand.Introduced(builds, schema.Build);

        return new CompatExplainResult(
            name, known, kind, declared, schema.Build, builds, addedIn, firstSeen);
    }
}
