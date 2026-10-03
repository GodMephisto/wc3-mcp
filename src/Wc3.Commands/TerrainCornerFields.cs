// src/Wc3.Commands/TerrainCornerFields.cs
using System.Globalization;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Field-name dispatch for editing one terrain corner, so the CLI and MCP reach the per-corner
/// setters the same way the GUI does.
///
/// <see cref="TerrainCommand"/> already covers brush work over a radius, which is the right shape
/// for a command line and the wrong shape for fixing one corner. Both layers matter and neither
/// replaces the other. Same pattern as <see cref="PlacedInstanceFields"/>, so the field list lives
/// in one place and a front end cannot offer a field the layer does not have.
/// </summary>
public static class TerrainCornerFields
{
    /// <summary>Editable fields on a single terrain corner.</summary>
    public static readonly IReadOnlyList<string> Fields = new[]
    {
        "GroundHeight", "AddGroundHeight", "WaterHeight", "GroundTexture", "CliffLevel",
    };

    /// <summary>
    /// Sets one field on the corner at (<paramref name="col"/>, <paramref name="row"/>).
    /// </summary>
    public static TerrainEditCommand.TerrainEditResult SetField(
        MapDocument doc, int col, int row, string field, string value)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (TerrainEditCommand.GetCorner(doc, col, row) is null)
            return new TerrainEditCommand.TerrainEditResult(false,
                $"no terrain corner at ({col}, {row}). Use 'terrain info' for the map's extents.");

        switch (field.ToLowerInvariant())
        {
            case "groundheight":
                return Real(value, out var h)
                    ? TerrainEditCommand.SetGroundHeight(doc, col, row, h)
                    : Bad(field, value, "a height");
            // A relative nudge, which is what raising ground by a step actually means and what a
            // caller would otherwise have to read-modify-write by hand.
            case "addgroundheight":
                return Real(value, out var d)
                    ? TerrainEditCommand.AddGroundHeight(doc, col, row, d)
                    : Bad(field, value, "a height delta");
            case "waterheight":
                return Real(value, out var w)
                    ? TerrainEditCommand.SetWaterHeight(doc, col, row, w)
                    : Bad(field, value, "a water height");
            case "groundtexture":
                return Int(value, out var t)
                    ? TerrainEditCommand.SetGroundTexture(doc, col, row, t)
                    : Bad(field, value, "a tile index into the map's ground tile list");
            case "clifflevel":
                return Int(value, out var c)
                    ? TerrainEditCommand.SetCliffLevel(doc, col, row, c)
                    : Bad(field, value, "a cliff level");
            default:
                return new TerrainEditCommand.TerrainEditResult(false,
                    $"unknown field '{field}', expected one of: {string.Join("|", Fields)}");
        }
    }

    private static bool Int(string s, out int v) =>
        int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

    private static bool Real(string s, out float v) =>
        float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);

    private static TerrainEditCommand.TerrainEditResult Bad(string field, string value, string expected) =>
        new(false, $"'{value}' is not valid for {field}, expected {expected}");
}
