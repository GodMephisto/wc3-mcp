// src/Wc3.Commands/PlaceGridCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Outcome of a bulk grid placement, whether it ran, how many copies landed,
/// and each copy's creation number in placement order.</summary>
public sealed record PlaceGridResult(
    bool Ok, string Message, int Placed, IReadOnlyList<int> CreationNumbers);

/// <summary>
/// Places a regular rows x cols grid of copies of one unit or doodad type, centred on
/// the map origin (the World Editor's world centre), reusing
/// <see cref="PlacementCommand.PlaceUnit"/> and <see cref="PlacementCommand.PlaceDoodad"/>
/// so a grid copy is built exactly like a single placement. The map is mutated in memory
/// only, the caller Saves the edited copy.
/// </summary>
public static class PlaceGridCommand
{
    public const string UnitKind = "unit";
    public const string DoodadKind = "doodad";

    /// <summary>Spacing between adjacent cells in world units, one terrain tile (128),
    /// the World Editor's own build grid.</summary>
    public const float DefaultSpacing = 128f;

    public static PlaceGridResult Execute(
        MapDocument doc,
        string rawcode,
        int rows,
        int cols,
        float spacing = DefaultSpacing,
        int ownerId = 0,
        string kind = UnitKind)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (rawcode is null || rawcode.Length != 4)
            return Fail($"invalid rawcode '{rawcode}', expected 4 characters");
        if (rows < 1)
            return Fail($"invalid rows {rows}, must be >= 1");
        if (cols < 1)
            return Fail($"invalid cols {cols}, must be >= 1");
        if (spacing <= 0f)
            return Fail($"invalid spacing {spacing}, must be > 0");

        bool isDoodad;
        switch ((kind ?? UnitKind).Trim().ToLowerInvariant())
        {
            case UnitKind: isDoodad = false; break;
            case DoodadKind: isDoodad = true; break;
            default: return Fail($"unknown kind '{kind}', expected {UnitKind} or {DoodadKind}");
        }
        if (!isDoodad && ownerId < 0)
            return Fail($"invalid owner id {ownerId}, must be >= 0");

        // Centre the block on the origin so "grid" means the middle of the map, not a
        // corner of it. A single row or column sits on the axis.
        float startX = -(cols - 1) * spacing / 2f;
        float startY = -(rows - 1) * spacing / 2f;

        var creation = new List<int>(rows * cols);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                float x = startX + c * spacing;
                float y = startY + r * spacing;
                if (isDoodad)
                {
                    var dr = PlacementCommand.PlaceDoodad(doc, rawcode, x, y);
                    if (!dr.Ok) return Fail(dr.Message, creation);
                    creation.Add(dr.CreationNumber);
                }
                else
                {
                    var ur = PlacementCommand.PlaceUnit(doc, rawcode, ownerId, x, y);
                    if (!ur.Ok) return Fail(ur.Message, creation);
                    creation.Add(ur.CreationNumber);
                }
            }
        }

        string noun = isDoodad ? "doodad" : "unit";
        return new(true,
            $"placed {creation.Count} {noun}(s) of {rawcode} in a {rows}x{cols} grid spaced {spacing:0.#}",
            creation.Count, creation);
    }

    private static PlaceGridResult Fail(string message, IReadOnlyList<int>? placed = null)
        => new(false, message, placed?.Count ?? 0, placed ?? Array.Empty<int>());
}
