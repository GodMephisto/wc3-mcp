// src/Wc3.Commands/PlaceScatterCommand.cs
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Outcome of a scatter placement, whether it ran, how many copies landed,
/// and each copy's creation number in placement order.</summary>
public sealed record PlaceScatterResult(
    bool Ok, string Message, int Placed, IReadOnlyList<int> CreationNumbers);

/// <summary>
/// Places a fixed number of copies of one unit type at uniformly random positions, either
/// inside a named region or across the map's whole playable area. The positions come from
/// a seeded <see cref="Random"/>, so the same map, count, region and seed reproduce the
/// exact same layout, which is what makes a scatter testable and a build repeatable. Reuses
/// <see cref="PlacementCommand.PlaceUnit"/> so a scattered copy is built like any other.
/// </summary>
public static class PlaceScatterCommand
{
    public const int DefaultSeed = 0;

    /// <summary>World units per terrain tile, so the playable half-extent is the playable
    /// tile count times half a tile.</summary>
    private const float HalfTileWorld = 64f;

    public static PlaceScatterResult Execute(
        MapDocument doc,
        string rawcode,
        int count,
        string? region = null,
        int seed = DefaultSeed,
        int ownerId = 0)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (rawcode is null || rawcode.Length != 4)
            return Fail($"invalid unit rawcode '{rawcode}', expected 4 characters");
        if (count < 1)
            return Fail($"invalid count {count}, must be >= 1");
        if (ownerId < 0)
            return Fail($"invalid owner id {ownerId}, must be >= 0");

        if (!TryBounds(doc, region, out var minX, out var minY, out var maxX, out var maxY, out var why))
            return Fail(why);

        var rng = new Random(seed);
        var creation = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            float x = NextFloat(rng, minX, maxX);
            float y = NextFloat(rng, minY, maxY);
            var r = PlacementCommand.PlaceUnit(doc, rawcode, ownerId, x, y);
            if (!r.Ok) return Fail(r.Message, creation);
            creation.Add(r.CreationNumber);
        }

        string where = string.IsNullOrWhiteSpace(region) ? "across the map" : $"inside region '{region}'";
        return new(true, $"placed {count} unit(s) of {rawcode} {where} (seed {seed})",
            creation.Count, creation);
    }

    /// <summary>Region bounds when a region is named, else the playable area centred on
    /// the origin. A rejected region name or a map with no playable area is an error.</summary>
    private static bool TryBounds(
        MapDocument doc, string? region,
        out float minX, out float minY, out float maxX, out float maxY, out string why)
    {
        minX = minY = maxX = maxY = 0f;
        why = "";

        if (!string.IsNullOrWhiteSpace(region))
        {
            var match = PlacementCommand.ListRegions(doc).FirstOrDefault(
                r => string.Equals(r.Name, region, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                why = $"no region named '{region}'";
                return false;
            }
            minX = match.Left; minY = match.Bottom; maxX = match.Right; maxY = match.Top;
            return true;
        }

        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info)
        {
            why = $"map has no {MapInfoCommand.FileName}; name a region or use a map with map info";
            return false;
        }

        float halfX = info.PlayableMapAreaWidth * HalfTileWorld;
        float halfY = info.PlayableMapAreaHeight * HalfTileWorld;
        if (halfX <= 0f || halfY <= 0f)
        {
            why = "the map's playable area is empty";
            return false;
        }
        minX = -halfX; minY = -halfY; maxX = halfX; maxY = halfY;
        return true;
    }

    private static float NextFloat(Random rng, float lo, float hi)
        => lo + (float)rng.NextDouble() * (hi - lo);

    private static PlaceScatterResult Fail(string message, IReadOnlyList<int>? placed = null)
        => new(false, message, placed?.Count ?? 0, placed ?? Array.Empty<int>());
}
