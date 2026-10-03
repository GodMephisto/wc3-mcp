// src/Wc3.Commands/TerrainFillCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One entry point for every rectangle terrain and pathing operation.
/// </summary>
/// <remarks>
/// <para>Eight bulk operations existed here and reached no front-end at all, so every terrain
/// edit in the Studio was one brush dab and filling a region meant clicking repeatedly. Seven
/// of the eight were well tested, they simply had no caller.</para>
///
/// <para>They are exposed through ONE dispatcher rather than eight parallel verbs, for the
/// reason the reference implementation demonstrates. HiveWE does not carry a rectangle variant
/// of each tool, it carries one brush with a selection mode, so selecting a region and acting
/// on it is a single concept rather than a family of them. Eight near-identical verbs would
/// also be exactly the copy-paste this project's own reuse rule forbids, and a Studio drag
/// affordance would then have to duplicate the same eight-way switch a second time.</para>
///
/// <para>Coordinates are terrain CORNERS and the rectangle is inclusive on both, matching the
/// underlying operations. Corners may be given in any order.</para>
/// </remarks>
public static class TerrainFillCommand
{
    /// <summary>What to do over the rectangle. One flat list, because the caller is choosing a
    /// tool rather than choosing a file to edit.</summary>
    public enum FillTool
    {
        /// <summary>Raise ground height by Value.</summary>
        Raise,
        /// <summary>Lower ground height by Value.</summary>
        Lower,
        /// <summary>Set ground height to Value.</summary>
        SetHeight,
        /// <summary>Flatten ground to the region's average.</summary>
        Flatten,
        /// <summary>Paint ground texture, Value is an index into the map's ground tile list.</summary>
        Paint,
        /// <summary>Raise the cliff level by Value steps (default 1).</summary>
        CliffRaise,
        /// <summary>Lower the cliff level by Value steps (default 1).</summary>
        CliffLower,
        /// <summary>Set the cliff level to Value.</summary>
        CliffSet,
        /// <summary>Turn the ramp flag on.</summary>
        Ramp,
        /// <summary>Turn the ramp flag off.</summary>
        RampOff,
        /// <summary>Set water at height Value.</summary>
        Water,
        /// <summary>Clear the water flag.</summary>
        WaterRemove,
        /// <summary>Turn blight on.</summary>
        Blight,
        /// <summary>Turn blight off.</summary>
        BlightOff,
    }

    public sealed record FillResult(bool Ok, string Message, int TilesChanged = 0);

    /// <summary>Whether the tool reads <c>value</c> at all, so a front-end can hide the field.</summary>
    public static bool UsesValue(FillTool tool) => tool
        is FillTool.Raise or FillTool.Lower or FillTool.SetHeight or FillTool.Paint
        or FillTool.CliffRaise or FillTool.CliffLower or FillTool.CliffSet or FillTool.Water;

    /// <summary>
    /// Applies <paramref name="tool"/> over the inclusive corner rectangle. Never throws for an
    /// off-grid or inverted rectangle, the underlying operations clip and report.
    /// </summary>
    public static FillResult Fill(
        MapDocument doc, int x0, int y0, int x1, int y1, FillTool tool, float value = 1f)
    {
        int steps = (int)Math.Round(value);

        return tool switch
        {
            FillTool.Raise => From(TerrainCommand.DeformRect(
                doc, x0, y0, x1, y1, TerrainCommand.HeightOp.Raise, value)),
            FillTool.Lower => From(TerrainCommand.DeformRect(
                doc, x0, y0, x1, y1, TerrainCommand.HeightOp.Lower, value)),
            FillTool.SetHeight => From(TerrainCommand.DeformRect(
                doc, x0, y0, x1, y1, TerrainCommand.HeightOp.Set, value)),
            FillTool.Flatten => From(TerrainCommand.DeformRect(
                doc, x0, y0, x1, y1, TerrainCommand.HeightOp.Flatten)),

            FillTool.Paint => From(TerrainCommand.PaintRect(
                doc, x0, y0, x1, y1, steps)),

            FillTool.CliffRaise => From(TerrainCommand.CliffRect(
                doc, x0, y0, x1, y1, TerrainCommand.CliffOp.Raise, Math.Max(1, steps))),
            FillTool.CliffLower => From(TerrainCommand.CliffRect(
                doc, x0, y0, x1, y1, TerrainCommand.CliffOp.Lower, Math.Max(1, steps))),
            FillTool.CliffSet => From(TerrainCommand.CliffRect(
                doc, x0, y0, x1, y1, TerrainCommand.CliffOp.Set, steps)),

            FillTool.Ramp => From(TerrainCommand.RampRect(doc, x0, y0, x1, y1, on: true)),
            FillTool.RampOff => From(TerrainCommand.RampRect(doc, x0, y0, x1, y1, on: false)),

            FillTool.Water => From(TerrainCommand.WaterRect(
                doc, x0, y0, x1, y1, TerrainCommand.WaterOp.Set, value)),
            FillTool.WaterRemove => From(TerrainCommand.WaterRect(
                doc, x0, y0, x1, y1, TerrainCommand.WaterOp.Remove)),

            FillTool.Blight => From(TerrainCommand.BlightRect(doc, x0, y0, x1, y1, on: true)),
            FillTool.BlightOff => From(TerrainCommand.BlightRect(doc, x0, y0, x1, y1, on: false)),

            _ => new FillResult(false, $"unknown fill tool '{tool}'"),
        };
    }

    private static FillResult From(TerrainCommand.DeformResult r) => new(r.Ok, r.Message, r.TilesChanged);
    private static FillResult From(TerrainCommand.PaintResult r) => new(r.Ok, r.Message, r.TilesChanged);
}
