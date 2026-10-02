// src/Wc3.Commands/Editing/TerrainSculptEdit.cs
using War3Net.Build.Environment; // MapEnvironment, TerrainTile
using Wc3.Model;

namespace Wc3.Commands.Editing;

/// <summary>
/// One reversible terrain-sculpt stroke: the corner tiles a whole brush stroke changed,
/// each captured before and after. A drag stroke applies many per-corner edits live, then
/// commits as a SINGLE <see cref="TerrainSculptEdit"/> so one Undo reverts the entire stroke
/// and one Redo re-applies it (never a half-stroke). Built by diffing the tile grid across
/// the stroke, so it holds only the tiles that actually changed - not the whole map.
///
/// The stroke has already run by the time this is recorded, so <see cref="Apply"/> restores
/// the "after" snapshot (a no-op on first record, the real work on redo) and
/// <see cref="Revert"/> restores "before". Tiles are reference types shared with the parsed
/// <see cref="MapEnvironment"/>, so restoring writes straight back into the live model.
/// </summary>
public sealed class TerrainSculptEdit : IMapEdit
{
    /// <summary>Every sculptable field of one corner tile, keyed by its grid index. Covers
    /// all Studio sculpt tools (height, cliff, texture+variation, water) plus blight/ramp so
    /// the snapshot is complete regardless of which tool ran.</summary>
    public readonly record struct TileState(
        int Index, float Height, int CliffLevel, int Texture, int Variation,
        bool IsWater, float WaterHeight, bool IsBlighted, bool IsRamp);

    private readonly IReadOnlyList<TileState> _before;
    private readonly IReadOnlyList<TileState> _after;
    private readonly string _label;

    public TerrainSculptEdit(string label, IReadOnlyList<TileState> before, IReadOnlyList<TileState> after)
    {
        _label = label;
        _before = before;
        _after = after;
    }

    public string Describe => _label;

    public void Apply(MapDocument doc) => Restore(doc, _after);
    public void Revert(MapDocument doc) => Restore(doc, _before);

    /// <summary>Snapshots one tile's sculptable state at its grid index.</summary>
    public static TileState Capture(int index, TerrainTile t) =>
        new(index, t.Height, t.CliffLevel, t.Texture, t.Variation,
            t.IsWater, t.WaterHeight, t.IsBlighted, t.IsRamp);

    private static void Restore(MapDocument doc, IReadOnlyList<TileState> states)
    {
        if (doc.GetFile(TerrainCommand.TerrainFile)?.Model is not MapEnvironment env)
            return;
        var tiles = env.TerrainTiles;
        if (tiles is null) return;
        foreach (var s in states)
        {
            if (s.Index < 0 || s.Index >= tiles.Count) continue;
            var t = tiles[s.Index];
            t.Height = s.Height;
            t.CliffLevel = s.CliffLevel;
            t.Texture = s.Texture;
            t.Variation = s.Variation;
            t.IsWater = s.IsWater;
            t.WaterHeight = s.WaterHeight;
            t.IsBlighted = s.IsBlighted;
            t.IsRamp = s.IsRamp;
        }
        doc.AddOrReplaceModelFile(TerrainCommand.TerrainFile, env);
    }
}
