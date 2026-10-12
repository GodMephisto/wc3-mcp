// src/Wc3.Commands/PlacedMissingRefsCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One placement whose rawcode resolves nowhere. Which file holds it, its kind,
/// creation number, the unresolved rawcode and where it sits.</summary>
public sealed record MissingRef(
    string File, string Kind, int CreationNumber, string Rawcode, float X, float Y);

/// <summary>Findings for placed missing-refs, with the count so a caller can branch
/// without walking the list.</summary>
public sealed record PlacedMissingRefsResult(
    bool Ok, string Message, int Count, IReadOnlyList<MissingRef> Missing);

/// <summary>
/// Lists placements whose rawcode is defined neither in the map's object data nor in the
/// base game data. Without a game dir only the map's object data can be consulted, so a
/// stock base-game type reads as unresolved too and the message says so. Read-only.
/// </summary>
public static class PlacedMissingRefsCommand
{
    /// <summary>The unresolved placements, unreduced, for callers that also need to edit them.</summary>
    public static IReadOnlyList<PlacedRef> FindMissing(MapDocument doc, string? gameDir)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return PlacementRefs.All(doc)
            .Where(r => !PlacementRefs.IsDefined(doc, r.Rawcode, gameDir))
            .ToList();
    }

    public static PlacedMissingRefsResult Execute(MapDocument doc, string? gameDir = null)
    {
        var missing = FindMissing(doc, gameDir)
            .Select(r => new MissingRef(r.File, r.Kind, r.CreationNumber, r.Rawcode, r.X, r.Y))
            .ToList();

        bool hasGameDir = gameDir is { Length: > 0 };
        string message;
        if (missing.Count == 0)
            message = hasGameDir
                ? "every placement resolves in the map's object data or the base game data"
                : "every placement resolves in the map's object data";
        else if (hasGameDir)
            message = $"{missing.Count} placement(s) reference a rawcode defined neither in the map's object data nor in the base game data";
        else
            message = $"{missing.Count} placement(s) reference a rawcode the map's object data does not define (no game dir, so the base-game check was skipped)";

        return new(true, message, missing.Count, missing);
    }
}
