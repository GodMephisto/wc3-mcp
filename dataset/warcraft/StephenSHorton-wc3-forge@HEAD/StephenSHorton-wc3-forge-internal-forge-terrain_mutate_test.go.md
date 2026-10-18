---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/terrain_mutate_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"math"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
)

// newTerrainSession builds a minimal loaded Session with a synthetic 3x3
// Reforged (v12) terrain grid and a two-entry ground palette. Avoids any
// on-disk fixture so the terrain-brush tests are hermetic. The Session is
// marked loaded with terrain set; other files are nil (the brush mutators only
// touch s.terrain).
func newTerrainSession(t *testing.T) *Session {
	t.Helper()
	const w, h = 3, 3
	tiles := make([]w3e.Tilepoint, w*h)
	for i := range tiles {
		tiles[i] = w3e.Tilepoint{
			GroundHeight:  0x2000, // game Z = 0
			GroundTexture: 0,      // palette slot 0 ("Ldrt")
		}
	}
	terrain := &w3e.File{
		Format:         w3e.Magic,
		Version:        12,
		Tileset:        'L',
		GroundTilesets: []string{"Ldrt", "Lgrs"},
		CliffTilesets:  []string{"CLdi"},
		Width:          w,
		Height:         h,
		CenterOffset:   [2]float32{-128, -128},
		Tiles:          tiles,
	}
	s := &Session{}
	s.loaded = true
	s.terrain = terrain
	return s
}

// TestSetTerrainTile_ThenGetTile verifies a tile edit lands and is observable
// via the read-back path (terrain.set_tile then terrain.get_tile semantics).
func TestSetTerrainTile_ThenGetTile(t *testing.T) {
	s := newTerrainSession(t)

	if err := s.SetTerrainTile(1, 2, "Lgrs"); err != nil {
		t.Fatalf("SetTerrainTile: %v", err)
	}

	info, err := s.GetTerrainTile(1, 2)
	if err != nil {
		t.Fatalf("GetTerrainTile: %v", err)
	}
	if info.GroundTileID != "Lgrs" {
		t.Errorf("ground_tile_id = %q, want %q", info.GroundTileID, "Lgrs")
	}
	if info.Col != 1 || info.Row != 2 {
		t.Errorf("col,row = %d,%d, want 1,2", info.Col, info.Row)
	}
	if !s.dirtyTerrain {
		t.Error("expected dirtyTerrain after SetTerrainTile")
	}
	// A neighboring corner must be untouched.
	other, err := s.GetTerrainTile(0, 0)
	if err != nil {
		t.Fatalf("GetTerrainTile(0,0): %v", err)
	}
	if other.GroundTileID != "Ldrt" {
		t.Errorf("untouched corner ground_tile_id = %q, want %q", other.GroundTileID, "Ldrt")
	}
}

// TestSetTerrainTile_NotInPalette asserts the FourCC-resolution error path:
// a tile not in the ground palette is rejected, and nothing is mutated.
func TestSetTerrainTile_NotInPalette(t *testing.T) {
	s := newTerrainSession(t)
	if err := s.SetTerrainTile(0, 0, "Xxxx"); err == nil {
		t.Fatal("expected error for FourCC not in palette")
	}
	if s.dirtyTerrain {
		t.Error("dirtyTerrain should stay false after a rejected SetTerrainTile")
	}
	info, err := s.GetTerrainTile(0, 0)
	if err != nil {
		t.Fatalf("GetTerrainTile: %v", err)
	}
	if info.GroundTileID != "Ldrt" {
		t.Errorf("corner mutated despite rejected set: got %q", info.GroundTileID)
	}
}

// TestSetTerrainTile_OutOfRange asserts coordinate bounds are enforced (no
// panic, clear error).
func TestSetTerrainTile_OutOfRange(t *testing.T) {
	s := newTerrainSession(t)
	for _, c := range []struct{ col, row int }{{-1, 0}, {0, -1}, {3, 0}, {0, 3}} {
		if err := s.SetTerrainTile(c.col, c.row, "Lgrs"); err == nil {
			t.Errorf("expected out-of-range error for (%d,%d)", c.col, c.row)
		}
	}
}

// TestSetTerrainHeight_RoundTrips verifies a height edit decodes back to the
// requested game-space Z (within quantization) via GetTerrainTile.
func TestSetTerrainHeight_RoundTrips(t *testing.T) {
	s := newTerrainSession(t)
	const wantZ float32 = 64.0
	if err := s.SetTerrainHeight(2, 1, wantZ); err != nil {
		t.Fatalf("SetTerrainHeight: %v", err)
	}
	info, err := s.GetTerrainTile(2, 1)
	if err != nil {
		t.Fatalf("GetTerrainTile: %v", err)
	}
	// z*4 must be integral for exact recovery; 64*4=256 fits int16 cleanly.
	if math.Abs(float64(info.Height-wantZ)) > 0.001 {
		t.Errorf("height = %v, want %v", info.Height, wantZ)
	}
	if !s.dirtyTerrain {
		t.Error("expected dirtyTerrain after SetTerrainHeight")
	}
}

// TestTerrainEdit_SurvivesEncodeParse is the load-bearing safety check: the
// edited w3e must re-encode and re-parse with both the tile-index and the
// height change intact. A writer that corrupts the heightfield is worse than
// no writer at all (see PRIME DIRECTIVE), so this gate must stay green.
func TestTerrainEdit_SurvivesEncodeParse(t *testing.T) {
	s := newTerrainSession(t)
	if err := s.SetTerrainTile(1, 1, "Lgrs"); err != nil {
		t.Fatalf("SetTerrainTile: %v", err)
	}
	if err := s.SetTerrainHeight(1, 1, 32.0); err != nil {
		t.Fatalf("SetTerrainHeight: %v", err)
	}

	encoded, err := w3e.Encode(s.terrain)
	if err != nil {
		t.Fatalf("w3e.Encode: %v", err)
	}
	reparsed, err := w3e.Parse(encoded)
	if err != nil {
		t.Fatalf("w3e.Parse: %v", err)
	}

	idx := 1*int(reparsed.Width) + 1 // (col=1,row=1)
	tp := reparsed.Tiles[idx]
	if int(tp.GroundTexture) >= len(reparsed.GroundTilesets) ||
		reparsed.GroundTilesets[tp.GroundTexture] != "Lgrs" {
		t.Errorf("after encode->parse, ground tile = idx %d (%v), want Lgrs",
			tp.GroundTexture, reparsed.GroundTilesets)
	}
	if math.Abs(float64(tp.Z()-32.0)) > 0.001 {
		t.Errorf("after encode->parse, height = %v, want 32.0", tp.Z())
	}
	// Unedited corner (0,0) must still be the baseline tile + flat ground.
	base := reparsed.Tiles[0]
	if reparsed.GroundTilesets[base.GroundTexture] != "Ldrt" || math.Abs(float64(base.Z())) > 0.001 {
		t.Errorf("unedited corner drifted: tile=%q z=%v",
			reparsed.GroundTilesets[base.GroundTexture], base.Z())
	}
}

// TestSetTerrainTile_UndoRestores verifies the undo command restores the prior
// palette index; redo re-applies.
func TestSetTerrainTile_UndoRestores(t *testing.T) {
	s := newTerrainSession(t)
	if err := s.SetTerrainTile(0, 1, "Lgrs"); err != nil {
		t.Fatalf("SetTerrainTile: %v", err)
	}
	if got, _ := s.GetTerrainTile(0, 1); got.GroundTileID != "Lgrs" {
		t.Fatalf("pre-undo tile = %q, want Lgrs", got.GroundTileID)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got, _ := s.GetTerrainTile(0, 1); got.GroundTileID != "Ldrt" {
		t.Errorf("after undo tile = %q, want Ldrt", got.GroundTileID)
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	if got, _ := s.GetTerrainTile(0, 1); got.GroundTileID != "Lgrs" {
		t.Errorf("after redo tile = %q, want Lgrs", got.GroundTileID)
	}
}

// TestSetTerrainHeight_UndoRestores verifies the height undo command restores
// the prior raw int16.
func TestSetTerrainHeight_UndoRestores(t *testing.T) {
	s := newTerrainSession(t)
	if err := s.SetTerrainHeight(2, 2, 100.0); err != nil {
		t.Fatalf("SetTerrainHeight: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	got, err := s.GetTerrainTile(2, 2)
	if err != nil {
		t.Fatalf("GetTerrainTile: %v", err)
	}
	if math.Abs(float64(got.Height)) > 0.001 {
		t.Errorf("after undo height = %v, want 0", got.Height)
	}
}

// TestSetTerrainTile_NoOp verifies setting a corner to its existing tile is a
// no-op that doesn't mark the session dirty or push history.
func TestSetTerrainTile_NoOp(t *testing.T) {
	s := newTerrainSession(t)
	if err := s.SetTerrainTile(0, 0, "Ldrt"); err != nil {
		t.Fatalf("SetTerrainTile: %v", err)
	}
	if s.dirtyTerrain {
		t.Error("no-op set should not mark terrain dirty")
	}
	if len(s.history) != 0 {
		t.Errorf("no-op set should not record history, got %d entries", len(s.history))
	}
}

// --- handler param-path coverage (mirrors the existing handler tests) -------

func TestHandleTerrainSetTile_InvalidParams(t *testing.T) {
	if _, err := handleTerrainSetTile(json.RawMessage(`{"col":"nope"}`)); err == nil {
		t.Fatal("expected error for malformed col")
	}
}

func TestHandleTerrainSetTile_MissingTileID(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = newTerrainSession(t)
	if _, err := handleTerrainSetTile(json.RawMessage(`{"col":0,"row":0}`)); err == nil {
		t.Fatal("expected error for missing ground_tile_id")
	}
}

func TestHandleTerrainGetTile_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	if _, err := handleTerrainGetTile(json.RawMessage(`{"col":0,"row":0}`)); err == nil {
		t.Fatal("expected error when no terrain loaded")
	}
}

// TestHandleTerrain_RoundTripViaWire drives the three handlers end to end the
// way an MCP client would: set_tile, set_height, then get_tile to confirm.
func TestHandleTerrain_RoundTripViaWire(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = newTerrainSession(t)

	if _, err := handleTerrainSetTile(json.RawMessage(`{"col":1,"row":0,"ground_tile_id":"Lgrs"}`)); err != nil {
		t.Fatalf("handleTerrainSetTile: %v", err)
	}
	if _, err := handleTerrainSetHeight(json.RawMessage(`{"col":1,"row":0,"height":48}`)); err != nil {
		t.Fatalf("handleTerrainSetHeight: %v", err)
	}
	res, err := handleTerrainGetTile(json.RawMessage(`{"col":1,"row":0}`))
	if err != nil {
		t.Fatalf("handleTerrainGetTile: %v", err)
	}
	info, ok := res.(TerrainTileInfo)
	if !ok {
		t.Fatalf("get_tile returned %T, want TerrainTileInfo", res)
	}
	if info.GroundTileID != "Lgrs" {
		t.Errorf("ground_tile_id = %q, want Lgrs", info.GroundTileID)
	}
	if math.Abs(float64(info.Height-48)) > 0.001 {
		t.Errorf("height = %v, want 48", info.Height)
	}
}
