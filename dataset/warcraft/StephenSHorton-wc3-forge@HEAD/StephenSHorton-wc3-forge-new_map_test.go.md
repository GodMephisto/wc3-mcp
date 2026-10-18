---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "new_map_test.go"
ref: HEAD
---

package main

import (
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// TestNewMapRoundTrip builds the same files CreateNewMap writes (minus the CASC
// tileset lookup, which needs a WC3 install), packs them into a .w3x, then
// reopens and parses each one — asserting the map is structurally valid and,
// critically, that the terrain is FLAT at game-space Z = 0 (the bug-prone bit:
// zero-initialised tiles would sink the whole map to Z ≈ -2304).
func TestNewMapRoundTrip(t *testing.T) {
	const W, H = 64, 96
	path := filepath.Join(t.TempDir(), "Untitled.w3x")

	w3eBytes, err := w3e.Encode(buildNewTerrain(W, H, 'L', []string{"Ldrt", "Lgrs"}, []string{"CLdi"}))
	if err != nil {
		t.Fatalf("encode terrain: %v", err)
	}
	w3iBytes, err := w3i.Encode(buildNewInfo("Test Map", W, H, 'L'))
	if err != nil {
		t.Fatalf("encode info: %v", err)
	}
	doodadBytes, err := doodadsdoo.Encode(&doodadsdoo.File{Version: 8, SubVersion: 9})
	if err != nil {
		t.Fatalf("encode doodads: %v", err)
	}
	unitBytes, err := unitsdoo.Encode(&unitsdoo.File{Version: 8, SubVersion: 9})
	if err != nil {
		t.Fatalf("encode units: %v", err)
	}

	files := []mpq.FileEntry{
		{Name: "war3map.w3i", Data: w3iBytes},
		{Name: "war3map.w3e", Data: w3eBytes},
		{Name: "war3map.doo", Data: doodadBytes},
		{Name: "war3mapUnits.doo", Data: unitBytes},
	}
	if err := mpq.WriteFile(path, files, buildHM3WHeader("Test Map", 1)); err != nil {
		t.Fatalf("write .w3x: %v", err)
	}

	arc, err := mpq.Open(path)
	if err != nil {
		t.Fatalf("reopen .w3x: %v", err)
	}
	defer arc.Close()

	// --- map info ---
	rawI, err := arc.Read("war3map.w3i")
	if err != nil {
		t.Fatalf("read w3i: %v", err)
	}
	info, err := w3i.Parse(rawI)
	if err != nil {
		t.Fatalf("parse w3i: %v", err)
	}
	if info.Name != "Test Map" {
		t.Errorf("info.Name = %q, want %q", info.Name, "Test Map")
	}
	if info.FileVersion != w3i.FileVersionTFT {
		t.Errorf("info.FileVersion = %d, want %d", info.FileVersion, w3i.FileVersionTFT)
	}
	if info.Tileset != 'L' {
		t.Errorf("info.Tileset = %q, want 'L'", string(info.Tileset))
	}
	if len(info.Players) != 1 || len(info.Forces) != 1 {
		t.Errorf("players=%d forces=%d, want 1 and 1", len(info.Players), len(info.Forces))
	}

	// --- terrain ---
	rawE, err := arc.Read("war3map.w3e")
	if err != nil {
		t.Fatalf("read w3e: %v", err)
	}
	terr, err := w3e.Parse(rawE)
	if err != nil {
		t.Fatalf("parse w3e: %v", err)
	}
	if terr.Width != W+1 || terr.Height != H+1 {
		t.Errorf("terrain grid = %dx%d, want %dx%d", terr.Width, terr.Height, W+1, H+1)
	}
	if got := len(terr.Tiles); got != int(terr.Width)*int(terr.Height) {
		t.Fatalf("tiles = %d, want %d", got, int(terr.Width)*int(terr.Height))
	}
	for i, tp := range terr.Tiles {
		if z := tp.FinalZ(); z != 0 {
			t.Fatalf("tile %d FinalZ = %v, want 0 (map should be flat at sea level)", i, z)
		}
	}

	// --- empty entity files ---
	rawD, err := arc.Read("war3map.doo")
	if err != nil {
		t.Fatalf("read doo: %v", err)
	}
	doo, err := doodadsdoo.Parse(rawD)
	if err != nil {
		t.Fatalf("parse doo: %v", err)
	}
	if len(doo.Doodads) != 0 {
		t.Errorf("new map has %d doodads, want 0", len(doo.Doodads))
	}
}
