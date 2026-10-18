---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/terrain_brush_test.go"
ref: HEAD
---

package forge

import "testing"

// fixtureExtracted is the same Lordaeron survival map the history tests open.
const fixtureExtracted = `C:\Users\4step\projects\wc3-survival-game\map\extracted`

// centerCorner returns a corner index/coords comfortably inside the grid so a
// radius-1 brush footprint never clips an edge.
func centerCorner(t *testing.T, s *Session) (col, row int) {
	t.Helper()
	tr := s.Terrain()
	if tr == nil || tr.Width < 8 || tr.Height < 8 {
		t.Fatalf("fixture terrain too small: %+v", tr)
	}
	return int(tr.Width) / 2, int(tr.Height) / 2
}

// TestPaintTileBrush_FootprintAndUndo paints a radius-1 circle and checks every
// footprint corner flipped to the target tile, then that Undo/Redo round-trips
// the whole dab as one step.
func TestPaintTileBrush_FootprintAndUndo(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	tr := s.Terrain()
	if len(tr.GroundTilesets) < 2 {
		t.Skipf("fixture ground palette has <2 tiles (%v); can't paint a different tile", tr.GroundTilesets)
	}
	col, row := centerCorner(t, s)
	w := int(tr.Width)
	center := row*w + col

	// Pick a target FourCC different from the center corner's current tile.
	curIdx := tr.Tiles[center].GroundTexture
	target := tr.GroundTilesets[0]
	if uint8(0) == curIdx {
		target = tr.GroundTilesets[1]
	}

	// Snapshot the radius-1 circle footprint (center + 4 orthogonal neighbors).
	footprint := []int{center, center - 1, center + 1, center - w, center + w}
	before := make(map[int]uint8, len(footprint))
	for _, idx := range footprint {
		before[idx] = s.Terrain().Tiles[idx].GroundTexture
	}

	if err := s.PaintTileBrush(col, row, 1, "circle", target); err != nil {
		t.Fatalf("PaintTileBrush: %v", err)
	}
	// Expected palette index of the target FourCC (scan; the resolver helper
	// needs the lock held).
	var wantIdx uint8
	for i, id := range s.Terrain().GroundTilesets {
		if id == target {
			wantIdx = uint8(i)
			break
		}
	}
	for _, idx := range footprint {
		if got := s.Terrain().Tiles[idx].GroundTexture; got != wantIdx {
			t.Errorf("corner %d after paint = %d, want %d", idx, got, wantIdx)
		}
	}
	if !s.CanUndo() {
		t.Fatalf("expected an undo step after paint")
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	for _, idx := range footprint {
		if got := s.Terrain().Tiles[idx].GroundTexture; got != before[idx] {
			t.Errorf("corner %d after undo = %d, want %d (orig)", idx, got, before[idx])
		}
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	for _, idx := range footprint {
		if got := s.Terrain().Tiles[idx].GroundTexture; got != wantIdx {
			t.Errorf("corner %d after redo = %d, want %d", idx, got, wantIdx)
		}
	}
}

// TestCliffBrush_RaiseAndUndo raises the cliff layer over a single corner and
// checks LayerHeight incremented + a renderable cliff texture got set, then that
// Undo restores both.
func TestCliffBrush_RaiseAndUndo(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)
	w := int(s.Terrain().Width)
	center := row*w + col

	origLayer := s.Terrain().Tiles[center].LayerHeight
	origTex := s.Terrain().Tiles[center].CliffTexture
	if origLayer >= maxLayerHeight {
		t.Skipf("center corner already at max layer %d", origLayer)
	}

	if err := s.CliffBrush(col, row, 0, "circle", "raise", 0, ""); err != nil {
		t.Fatalf("CliffBrush: %v", err)
	}
	if got := s.Terrain().Tiles[center].LayerHeight; got != origLayer+1 {
		t.Errorf("layer after raise = %d, want %d", got, origLayer+1)
	}
	if got := s.Terrain().Tiles[center].CliffTexture; got > maxLayerHeight {
		t.Errorf("cliff texture after raise = %d, want a valid palette slot", got)
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := s.Terrain().Tiles[center].LayerHeight; got != origLayer {
		t.Errorf("layer after undo = %d, want %d", got, origLayer)
	}
	if got := s.Terrain().Tiles[center].CliffTexture; got != origTex {
		t.Errorf("cliff texture after undo = %d, want %d", got, origTex)
	}
}

// TestBrushSaveRoundTrip paints a tile + raises a cliff, saves the map, reopens
// it from disk, and asserts both edits survived the .w3e Encode/Parse round-trip
// — the end-to-end path the GUI brush + Save button exercise.
func TestBrushSaveRoundTrip(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	tr := s.Terrain()
	if len(tr.GroundTilesets) < 2 {
		t.Skipf("fixture ground palette has <2 tiles; can't paint a different tile")
	}
	col, row := centerCorner(t, s)
	w := int(tr.Width)
	center := row*w + col

	curIdx := tr.Tiles[center].GroundTexture
	targetIdx := uint8(0)
	if curIdx == 0 {
		targetIdx = 1
	}
	target := tr.GroundTilesets[targetIdx]
	origLayer := tr.Tiles[center].LayerHeight
	if origLayer >= maxLayerHeight {
		t.Skipf("center corner already at max layer")
	}

	if err := s.PaintTileBrush(col, row, 0, "circle", target); err != nil {
		t.Fatalf("PaintTileBrush: %v", err)
	}
	if err := s.CliffBrush(col, row, 0, "circle", "raise", 0, ""); err != nil {
		t.Fatalf("CliffBrush: %v", err)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}

	// Reopen from disk and confirm persistence.
	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("reopen: %v", err)
	}
	got := s2.Terrain().Tiles[center]
	if got.GroundTexture != targetIdx {
		t.Errorf("persisted ground tex = %d, want %d", got.GroundTexture, targetIdx)
	}
	if got.LayerHeight != origLayer+1 {
		t.Errorf("persisted layer = %d, want %d", got.LayerHeight, origLayer+1)
	}
}

// TestCliffBrush_StaircaseInvariant raises a single corner three levels and
// asserts the result is a valid 1-level staircase: no two 4-adjacent corners
// differ by more than one level. Without the ripple, the center would be 3
// levels above its neighbors — a multi-level step with no cliff model (invisible
// wall). Also confirms one Undo reverts the whole ripple.
func TestCliffBrush_StaircaseInvariant(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)
	w := int(s.Terrain().Width)
	h := int(s.Terrain().Height)
	center := row*w + col
	orig := int(s.Terrain().Tiles[center].LayerHeight)
	if orig+3 > maxLayerHeight {
		t.Skipf("center layer %d too high for a +3 raise", orig)
	}

	for i := 0; i < 3; i++ {
		if err := s.CliffBrush(col, row, 0, "circle", "raise", 0, ""); err != nil {
			t.Fatalf("CliffBrush raise %d: %v", i, err)
		}
	}
	if got := int(s.Terrain().Tiles[center].LayerHeight); got != orig+3 {
		t.Errorf("center layer = %d, want %d", got, orig+3)
	}
	// Whole-grid staircase check: every 4-adjacent pair within 1 level.
	tiles := s.Terrain().Tiles
	maxDiff := 0
	for r := 0; r < h; r++ {
		for c := 0; c < w; c++ {
			l := int(tiles[r*w+c].LayerHeight)
			if c+1 < w {
				if d := abs(l - int(tiles[r*w+c+1].LayerHeight)); d > maxDiff {
					maxDiff = d
				}
			}
			if r+1 < h {
				if d := abs(l - int(tiles[(r+1)*w+c].LayerHeight)); d > maxDiff {
					maxDiff = d
				}
			}
		}
	}
	if maxDiff > 1 {
		t.Errorf("max adjacent layer diff = %d, want ≤1 (staircase broken)", maxDiff)
	}

	// One Undo reverts the last raise (and its ripple) as a single step.
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := int(s.Terrain().Tiles[center].LayerHeight); got != orig+2 {
		t.Errorf("center layer after one undo = %d, want %d", got, orig+2)
	}
}

func abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

// TestBrushFootprint_CircleVsSquare checks the two shapes produce the expected
// corner counts for a radius-2 brush well inside the grid: a square is the full
// (2r+1)² block; a circle drops the four extreme corners.
func TestBrushFootprint_CircleVsSquare(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)

	s.mu.Lock()
	sq := s.terrainBrushCornersLocked(col, row, 2, "square")
	ci := s.terrainBrushCornersLocked(col, row, 2, "circle")
	// Fractional radius grows the footprint smoothly: at r=1.5 the circle reach
	// is 2.0, so it includes the corners at distance²≤4 (13) — between r=1's 9
	// and r=2's 21.
	ciHalf := s.terrainBrushCornersLocked(col, row, 1.5, "circle")
	s.mu.Unlock()

	if len(sq) != 25 {
		t.Errorf("square r=2 footprint = %d corners, want 25", len(sq))
	}
	// radius-2 euclidean: drops the 4 corners at distance √8 > 2 → 21.
	if len(ci) != 21 {
		t.Errorf("circle r=2 footprint = %d corners, want 21", len(ci))
	}
	if len(ciHalf) != 13 {
		t.Errorf("circle r=1.5 footprint = %d corners, want 13", len(ciHalf))
	}
}

// TestWaterBrush_AddRemoveAndUndo floods a single corner, checks the water flag
// flipped on with a surface above the ground, then that Undo restores the prior
// flag + level exactly, and finally that "remove" clears both.
func TestWaterBrush_AddRemoveAndUndo(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)
	w := int(s.Terrain().Width)
	center := row*w + col

	// Start from a known dry corner so the "add" path computes a fresh surface
	// (rather than preserving an existing pond). Snapshot the dry baseline.
	if s.Terrain().Tiles[center].HasWater() {
		if err := s.WaterBrush(col, row, 0, "circle", "remove", 0, false, false); err != nil {
			t.Fatalf("WaterBrush remove (baseline): %v", err)
		}
	}
	origFlags := s.Terrain().Tiles[center].Flags
	origWater := s.Terrain().Tiles[center].WaterLevel

	// --- add (auto height) ---
	if err := s.WaterBrush(col, row, 0, "circle", "add", 0, false, false); err != nil {
		t.Fatalf("WaterBrush add: %v", err)
	}
	tp := s.Terrain().Tiles[center]
	if !tp.HasWater() {
		t.Errorf("water flag not set after add")
	}
	if tp.WaterZ() <= tp.FinalZ() {
		t.Errorf("water surface %.1f not above ground %.1f after add", tp.WaterZ(), tp.FinalZ())
	}
	if !s.CanUndo() {
		t.Fatalf("expected an undo step after add")
	}

	// --- undo restores the dry baseline exactly ---
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := s.Terrain().Tiles[center].Flags; got != origFlags {
		t.Errorf("flags after undo = 0x%x, want 0x%x", got, origFlags)
	}
	if got := s.Terrain().Tiles[center].WaterLevel; got != origWater {
		t.Errorf("water level after undo = %d, want %d", got, origWater)
	}

	// --- remove (re-add first, then clear) leaves the flag off + level zeroed ---
	if err := s.WaterBrush(col, row, 0, "circle", "add", 0, false, false); err != nil {
		t.Fatalf("WaterBrush re-add: %v", err)
	}
	if err := s.WaterBrush(col, row, 0, "circle", "remove", 0, false, false); err != nil {
		t.Fatalf("WaterBrush remove: %v", err)
	}
	tp = s.Terrain().Tiles[center]
	if tp.HasWater() {
		t.Errorf("water flag still set after remove")
	}
	if tp.WaterLevel != 0 {
		t.Errorf("water level after remove = %d, want 0", tp.WaterLevel)
	}
}

// TestWaterBrush_FlatExplicitHeight paints a radius-2 footprint with an explicit
// surface height and asserts EVERY watered corner lands on that one flat level
// (the flat-per-stroke / fixed-height behavior), regardless of per-corner ground.
func TestWaterBrush_FlatExplicitHeight(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)
	w := int(s.Terrain().Width)

	// Clear any pre-existing water across the footprint so every corner is dry and
	// will take the explicit height (the preserve-visible branch is exercised
	// separately by the add/undo test).
	if err := s.WaterBrush(col, row, 2, "circle", "remove", 0, false, false); err != nil {
		t.Fatalf("WaterBrush remove (baseline): %v", err)
	}

	const surfaceZ float32 = 256.0
	if err := s.WaterBrush(col, row, 2, "circle", "add", surfaceZ, true, true); err != nil {
		t.Fatalf("WaterBrush add (explicit): %v", err)
	}
	wantRaw := gameZToRawWater(surfaceZ)

	// The radius-2 circle footprint center + neighbors must all carry the SAME
	// stored water level — a flat sheet, not a per-corner drape.
	center := row*w + col
	footprint := []int{center, center - 1, center + 1, center - w, center + w, center - 2, center + 2}
	for _, idx := range footprint {
		tp := s.Terrain().Tiles[idx]
		if !tp.HasWater() {
			t.Errorf("corner %d not watered after explicit add", idx)
		}
		if tp.WaterLevel != wantRaw {
			t.Errorf("corner %d water level = %d, want flat %d", idx, tp.WaterLevel, wantRaw)
		}
	}
}

// TestWaterBrush_FixedHeightOverwrite reproduces the reported bug: adding water
// at a higher fixed height over existing lower water must raise it (overwrite=
// true), while Auto mode (overwrite=false) must preserve the existing higher
// surface instead of lowering it.
func TestWaterBrush_FixedHeightOverwrite(t *testing.T) {
	tmp := copyFixtureToTemp(t, fixtureExtracted)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	col, row := centerCorner(t, s)
	center := row*int(s.Terrain().Width) + col

	low := gameZToRawWater(256)
	high := gameZToRawWater(700)

	// Lay down low fixed-height water.
	if err := s.WaterBrush(col, row, 0, "circle", "add", 256, true, true); err != nil {
		t.Fatalf("WaterBrush add low: %v", err)
	}
	if got := s.Terrain().Tiles[center].WaterLevel; got != low {
		t.Fatalf("after low add, water=%d want %d", got, low)
	}

	// Fixed-height add at a HIGHER level must overwrite the existing lower water.
	if err := s.WaterBrush(col, row, 0, "circle", "add", 700, true, true); err != nil {
		t.Fatalf("WaterBrush add high (overwrite): %v", err)
	}
	if got := s.Terrain().Tiles[center].WaterLevel; got != high {
		t.Errorf("fixed-height higher add did not overwrite: water=%d want %d", got, high)
	}

	// Auto mode (overwrite=false) over the now-high water must NOT lower it. The
	// auto surface here (ground+~122, ground≈0) is well below 700, so preserve.
	if err := s.WaterBrush(col, row, 0, "circle", "add", 0, false, false); err != nil {
		t.Fatalf("WaterBrush auto add: %v", err)
	}
	if got := s.Terrain().Tiles[center].WaterLevel; got != high {
		t.Errorf("auto add lowered existing higher water: water=%d want preserved %d", got, high)
	}
}
