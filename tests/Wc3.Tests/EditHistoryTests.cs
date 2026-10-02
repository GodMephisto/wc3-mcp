// tests/Wc3.Tests/EditHistoryTests.cs
using War3Net.Build.Environment;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Commands.Editing;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Exercises the interactive undo/redo journal (<see cref="EditHistory"/>) and
/// the placement edits that wrap <see cref="PlacementCommand"/>. The headline
/// guarantee is byte-faithful reversibility: undo restores the exact prior
/// document, and redo reproduces the exact post-edit document.
/// </summary>
public class EditHistoryTests
{
    private static MapUnits? Units(MapDocument doc) =>
        doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;

    private static MapDoodads? Doodads(MapDocument doc) =>
        doc.GetFile(PlacementCommand.DoodadsFile)?.Model as MapDoodads;

    private static MapRegions? Regions(MapDocument doc) =>
        doc.GetFile(PlacementCommand.RegionsFile)?.Model as MapRegions;

    private static MapEnvironment Env(MapDocument doc) =>
        (MapEnvironment)doc.GetFile(TerrainCommand.TerrainFile)!.Model!;

    // Mirrors TerrainView.CommitSculptStroke: diff the grid against a pre-stroke snapshot
    // and record ONE edit for whatever the stroke changed.
    private static TerrainSculptEdit BuildSculptEdit(
        TerrainSculptEdit.TileState[] before, MapDocument doc)
    {
        var tiles = Env(doc).TerrainTiles;
        var b = new List<TerrainSculptEdit.TileState>();
        var a = new List<TerrainSculptEdit.TileState>();
        for (int i = 0; i < tiles.Count; i++)
        {
            var now = TerrainSculptEdit.Capture(i, tiles[i]);
            if (!now.Equals(before[i])) { b.Add(before[i]); a.Add(now); }
        }
        return new TerrainSculptEdit($"Sculpt {a.Count} tile(s)", b, a);
    }

    private static TerrainSculptEdit.TileState[] Snapshot(MapDocument doc)
    {
        var tiles = Env(doc).TerrainTiles;
        var snap = new TerrainSculptEdit.TileState[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) snap[i] = TerrainSculptEdit.Capture(i, tiles[i]);
        return snap;
    }

    [Fact]
    public void TerrainSculpt_Undo_RestoresHeights_Redo_ReappliesThem()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });
        var hist = new EditHistory();

        // A stroke: snapshot, raise a patch, then record the diff as one edit (the stroke
        // already ran, so Do's Apply is an idempotent no-op).
        var before = Snapshot(doc);
        var r = TerrainCommand.Deform(doc, 4, 4, radius: 2, TerrainCommand.HeightOp.Raise, amount: 3f);
        Assert.True(r.Ok, r.Message);
        int centerIdx = 4 * 9 + 4; // (Width+1) = 9 columns on an 8-tile map
        float raised = Env(doc).TerrainTiles[centerIdx].Height;

        hist.Do(doc, BuildSculptEdit(before, doc));
        Assert.Equal(raised, Env(doc).TerrainTiles[centerIdx].Height); // Do did not disturb the result

        // One Undo reverts the WHOLE stroke back to the pre-stroke height.
        Assert.True(hist.Undo(doc));
        Assert.Equal(before[centerIdx].Height, Env(doc).TerrainTiles[centerIdx].Height);

        // One Redo re-applies it exactly.
        Assert.True(hist.Redo(doc));
        Assert.Equal(raised, Env(doc).TerrainTiles[centerIdx].Height);
    }

    [Fact]
    public void TerrainSculpt_Water_RoundTripsThroughUndoRedo()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });
        var hist = new EditHistory();

        var before = Snapshot(doc);
        TerrainCommand.Water(doc, 4, 4, radius: 1, TerrainCommand.WaterOp.Set, amount: 1f);
        int idx = 4 * 9 + 4;
        Assert.True(Env(doc).TerrainTiles[idx].IsWater);

        hist.Do(doc, BuildSculptEdit(before, doc));
        Assert.True(hist.Undo(doc));
        Assert.False(Env(doc).TerrainTiles[idx].IsWater); // water flag cleared on undo
        Assert.True(hist.Redo(doc));
        Assert.True(Env(doc).TerrainTiles[idx].IsWater);  // and restored on redo
    }

    [Fact]
    public void PlaceDoodad_Undo_RemovesIt_Redo_RestoresSameCreationNumber()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        var edit = new PlaceDoodadEdit("LTlt", x: 128f, y: -256f);

        hist.Do(doc, edit);
        Assert.Single(Doodads(doc)!.Doodads);
        int cn = edit.CreationNumber;
        Assert.Equal(0, cn);

        Assert.True(hist.Undo(doc));
        Assert.Empty(Doodads(doc)!.Doodads);

        Assert.True(hist.Redo(doc));
        var d = Assert.Single(Doodads(doc)!.Doodads);
        Assert.Equal(cn, d.CreationNumber);
        Assert.Equal("LTlt".FromRawcode(), d.TypeId);
        Assert.Equal(128f, d.Position.X);
        Assert.Equal(-256f, d.Position.Y);
    }

    [Fact]
    public void Undo_RestoresByteIdenticalDocument()
    {
        // Seed one unit directly so the units file already exists — this isolates
        // the undo assertion from the "created an empty file" edge and lets us
        // compare raw bytes.
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 100f, y: 100f);
        byte[] before = doc.SaveToBytes();

        var hist = new EditHistory();
        hist.Do(doc, new PlaceUnitEdit("hpea", ownerId: 0, x: 200f, y: 200f));
        Assert.Equal(2, Units(doc)!.Units.Count);

        Assert.True(hist.Undo(doc));
        byte[] after = doc.SaveToBytes();

        Assert.Equal(before, after); // undo is byte-faithful
    }

    [Fact]
    public void Redo_RestoresByteIdenticalDocument()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 100f, y: 100f);

        var hist = new EditHistory();
        hist.Do(doc, new PlaceUnitEdit("hpea", ownerId: 0, x: 200f, y: 200f));
        byte[] placed = doc.SaveToBytes();

        hist.Undo(doc);
        Assert.True(hist.Redo(doc));
        byte[] redone = doc.SaveToBytes();

        Assert.Equal(placed, redone); // redo reproduces the exact post-edit bytes
    }

    [Fact]
    public void NewEdit_ClearsRedoStack()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();

        hist.Do(doc, new PlaceDoodadEdit("LTlt", 0f, 0f));
        hist.Do(doc, new PlaceDoodadEdit("LTlt", 64f, 0f));
        hist.Undo(doc);
        Assert.True(hist.CanRedo);

        hist.Do(doc, new PlaceDoodadEdit("LTlt", 128f, 0f)); // fresh edit
        Assert.False(hist.CanRedo);
        Assert.Equal(2, Doodads(doc)!.Doodads.Count);
    }

    [Fact]
    public void Undo_And_Redo_ReturnFalse_WhenStacksEmpty()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();

        Assert.False(hist.CanUndo);
        Assert.False(hist.CanRedo);
        Assert.False(hist.Undo(doc));
        Assert.False(hist.Redo(doc));
    }

    [Fact]
    public void MultiEdit_UndoAll_ThenRedoAll_PreservesCreationNumbers()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        var a = new PlaceUnitEdit("hfoo", 0, 0f, 0f);
        var b = new PlaceUnitEdit("hpea", 0, 64f, 0f);
        var c = new PlaceUnitEdit("hkni", 0, 128f, 0f);

        hist.Do(doc, a);
        hist.Do(doc, b);
        hist.Do(doc, c);
        int[] cns = { a.CreationNumber, b.CreationNumber, c.CreationNumber };
        Assert.Equal(new[] { 0, 1, 2 }, cns);

        Assert.True(hist.Undo(doc));
        Assert.True(hist.Undo(doc));
        Assert.True(hist.Undo(doc));
        Assert.Empty(Units(doc)!.Units);
        Assert.False(hist.CanUndo);

        Assert.True(hist.Redo(doc));
        Assert.True(hist.Redo(doc));
        Assert.True(hist.Redo(doc));
        var units = Units(doc)!.Units;
        Assert.Equal(3, units.Count);
        Assert.Equal(cns, units.Select(u => u.CreationNumber).OrderBy(n => n).ToArray());
    }

    [Fact]
    public void RemoveUnitEdit_DeletesById_Undo_RestoresIt()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 0f, y: 0f);   // cn 0
        var target = PlacementCommand.PlaceUnit(doc, "hpea", ownerId: 0, x: 64f, y: 0f); // cn 1
        Assert.Equal(2, Units(doc)!.Units.Count);

        var hist = new EditHistory();
        hist.Do(doc, new RemoveUnitEdit(target.CreationNumber));
        Assert.Single(Units(doc)!.Units);
        Assert.DoesNotContain(Units(doc)!.Units, u => u.CreationNumber == target.CreationNumber);

        Assert.True(hist.Undo(doc));
        Assert.Equal(2, Units(doc)!.Units.Count);
        Assert.Contains(Units(doc)!.Units, u => u.CreationNumber == target.CreationNumber);

        Assert.True(hist.Redo(doc));
        Assert.Single(Units(doc)!.Units);
    }

    [Fact]
    public void PlaceRegion_Undo_Redo_RoundTrips()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        var edit = new PlaceRegionEdit("spawn", left: 0f, bottom: 0f, right: 512f, top: 512f);

        hist.Do(doc, edit);
        Assert.Equal("spawn", Assert.Single(Regions(doc)!.Regions).Name);

        hist.Undo(doc);
        Assert.Empty(Regions(doc)!.Regions);

        hist.Redo(doc);
        Assert.Equal("spawn", Assert.Single(Regions(doc)!.Regions).Name);
    }

    [Fact]
    public void Labels_And_Depths_TrackStackState()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        Assert.Null(hist.NextUndoLabel);
        Assert.Null(hist.NextRedoLabel);

        hist.Do(doc, new PlaceDoodadEdit("LTlt", 0f, 0f));
        Assert.Equal("Place doodad LTlt", hist.NextUndoLabel);
        Assert.Equal(1, hist.UndoDepth);
        Assert.Equal(0, hist.RedoDepth);

        hist.Undo(doc);
        Assert.Null(hist.NextUndoLabel);
        Assert.Equal("Place doodad LTlt", hist.NextRedoLabel);
        Assert.Equal(0, hist.UndoDepth);
        Assert.Equal(1, hist.RedoDepth);
    }

    [Fact]
    public void Changed_Fires_OnEveryMutation()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        int fired = 0;
        hist.Changed += () => fired++;

        hist.Do(doc, new PlaceDoodadEdit("LTlt", 0f, 0f)); // 1
        hist.Undo(doc);                                    // 2
        hist.Redo(doc);                                    // 3
        hist.Clear();                                      // 4
        Assert.Equal(4, fired);

        int before = fired;
        Assert.False(hist.Undo(doc)); // no-op: must not fire
        Assert.False(hist.Redo(doc)); // no-op: must not fire
        hist.Clear();                 // already empty: must not fire
        Assert.Equal(before, fired);
    }

    [Fact]
    public void Clear_EmptiesBothStacks()
    {
        var doc = BlankMap.Create();
        var hist = new EditHistory();
        hist.Do(doc, new PlaceDoodadEdit("LTlt", 0f, 0f));
        hist.Undo(doc);
        Assert.True(hist.CanRedo);

        hist.Clear();
        Assert.False(hist.CanUndo);
        Assert.False(hist.CanRedo);
    }
}
