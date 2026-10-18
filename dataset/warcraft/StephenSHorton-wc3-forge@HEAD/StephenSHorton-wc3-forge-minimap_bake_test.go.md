---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "minimap_bake_test.go"
ref: HEAD
---

package main

import (
	"bytes"
	"image/png"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
	gw3blp "github.com/nielsAD/gowarcraft3/file/blp"
)

// flatTerrain builds a w3e with the given playable tile dimensions, a single
// Lordaeron ground tileset, all corners flat at layer 2. wTiles/hTiles are
// PLAYABLE tile counts; the vertex grid is one larger in each axis.
func flatTerrain(wTiles, hTiles int) *w3e.File {
	vw := wTiles + 1
	vh := hTiles + 1
	tiles := make([]w3e.Tilepoint, vw*vh)
	for i := range tiles {
		tiles[i] = w3e.Tilepoint{GroundHeight: 0x2000, LayerHeight: 2, GroundTexture: 0}
	}
	return &w3e.File{
		Format:         w3e.Magic,
		Version:        11,
		Tileset:        'L',
		GroundTilesets: []string{"Ldrt"},
		CliffTilesets:  []string{"CLdi"},
		Width:          uint32(vw),
		Height:         uint32(vh),
		CenterOffset:   [2]float32{-float32(wTiles) * 64, -float32(hTiles) * 64},
		Tiles:          tiles,
	}
}

func TestBakeMinimapImageDimensions(t *testing.T) {
	img := bakeMinimapImage(flatTerrain(4, 6))
	if got := img.Bounds().Dx(); got != 4 {
		t.Errorf("width = %d, want 4 (Width-1 cells)", got)
	}
	if got := img.Bounds().Dy(); got != 6 {
		t.Errorf("height = %d, want 6 (Height-1 cells)", got)
	}
}

func TestBakeMinimapImageDegenerate(t *testing.T) {
	// A 1×1 vertex grid has no real cells — must still return a valid 1×1 image
	// (and not panic on the cliff/water neighbor indexing).
	tp := []w3e.Tilepoint{{GroundHeight: 0x2000, LayerHeight: 2}}
	f := &w3e.File{
		Format: w3e.Magic, Version: 11, Tileset: 'L',
		GroundTilesets: []string{"Ldrt"}, Width: 1, Height: 1, Tiles: tp,
	}
	img := bakeMinimapImage(f)
	if img.Bounds().Dx() != 1 || img.Bounds().Dy() != 1 {
		t.Fatalf("degenerate image bounds = %v, want 1x1", img.Bounds())
	}
}

func TestBakeMinimapImageWaterTint(t *testing.T) {
	// A 2×2-tile map; flag the bottom-left cell's BL corner as water. That cell's
	// pixel must differ from a dry cell and skew bluer (water blends toward the
	// tileset's deep-water color).
	f := flatTerrain(2, 2)
	W := int(f.Width)
	f.Tiles[0].Flags |= 0x4 // HasWater on vertex (0,0) = BL of bottom-left cell

	img := bakeMinimapImage(f)
	// Cell (0,0) is the bottom-left cell → image bottom-left pixel (y = ch-1).
	ch := int(f.Height) - 1
	wr, wg, wb, _ := img.At(0, ch-1).RGBA()
	// A dry cell: top-right cell (1,1) → image (1, 0).
	dr, dg, db, _ := img.At(1, 0).RGBA()

	if wr == dr && wg == dg && wb == db {
		t.Fatalf("water cell pixel identical to dry cell (%d,%d,%d) — tint not applied", wr>>8, wg>>8, wb>>8)
	}
	// Deep-water fallback is bluish (B dominant), so the water cell should not be
	// LESS blue than the dry cell.
	if wb < db {
		t.Errorf("water cell blue=%d < dry cell blue=%d; expected water to skew bluer", wb>>8, db>>8)
	}
	_ = W
}

func TestBakeMinimapBLPRoundTrips(t *testing.T) {
	// The baked bytes must be a valid BLP1 that gowarcraft3 (the same decoder WC3
	// and the frontend lean on) can read back — otherwise the in-game minimap and
	// the panel would both fail to decode it.
	data, err := BakeMinimapBLP(flatTerrain(8, 8))
	if err != nil {
		t.Fatalf("BakeMinimapBLP: %v", err)
	}
	if len(data) == 0 {
		t.Fatal("BakeMinimapBLP returned empty bytes")
	}
	img, err := gw3blp.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("decoding baked BLP failed: %v", err)
	}
	b := img.Bounds()
	if b.Dx() <= 0 || b.Dy() <= 0 {
		t.Fatalf("decoded minimap has empty bounds %v", b)
	}
	// blp.Encode resizes to the nearest power of two; an 8-cell grid stays 8.
	if b.Dx() != 8 || b.Dy() != 8 {
		t.Errorf("decoded minimap bounds = %dx%d, want 8x8", b.Dx(), b.Dy())
	}
}

func TestBakeMinimapBLPNilTerrain(t *testing.T) {
	if _, err := BakeMinimapBLP(nil); err == nil {
		t.Error("expected error for nil terrain, got nil")
	}
}

// TestBakeMinimapImageFlatIsUniformOpaque guards the failure mode that shipped:
// the frontend showed a non-uniform, partly-transparent mess. A flat
// single-tileset map must bake to a single fully-opaque color across every
// pixel — anything else means the color/alpha math is wrong.
func TestBakeMinimapImageFlatIsUniformOpaque(t *testing.T) {
	img := bakeMinimapImage(flatTerrain(12, 12))
	b := img.Bounds()
	first := img.RGBAAt(b.Min.X, b.Min.Y)
	if first.A != 255 {
		t.Fatalf("pixel alpha = %d, want 255 (opaque)", first.A)
	}
	for y := b.Min.Y; y < b.Max.Y; y++ {
		for x := b.Min.X; x < b.Max.X; x++ {
			p := img.RGBAAt(x, y)
			if p != first {
				t.Fatalf("flat map not uniform: pixel (%d,%d)=%v != %v", x, y, p, first)
			}
		}
	}
}

// TestBakeMinimapPNGRoundTrips is the frontend-path guard: the panel renders a
// PNG, so the PNG must decode (via the standard library, same as the browser's
// native decoder) to the right size and an opaque image.
func TestBakeMinimapPNGRoundTrips(t *testing.T) {
	data, err := BakeMinimapPNG(flatTerrain(20, 16))
	if err != nil {
		t.Fatalf("BakeMinimapPNG: %v", err)
	}
	img, err := png.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("decode PNG: %v", err)
	}
	if got := img.Bounds().Dx(); got != 20 {
		t.Errorf("PNG width = %d, want 20", got)
	}
	if got := img.Bounds().Dy(); got != 16 {
		t.Errorf("PNG height = %d, want 16", got)
	}
	_, _, _, a := img.At(10, 8).RGBA()
	if a>>8 != 255 {
		t.Errorf("PNG center alpha = %d, want 255 (opaque)", a>>8)
	}
}

func TestBakeMinimapPNGNilTerrain(t *testing.T) {
	if _, err := BakeMinimapPNG(nil); err == nil {
		t.Error("expected error for nil terrain, got nil")
	}
}

// TestWriteNewMapEmbedsMinimap is the end-to-end check for the new-map fix: a
// map created by writeNewMap must contain a war3mapMap.blp that decodes, so the
// minimap panel (and WC3 in-game) finds a baked image instead of "No minimap".
// Runs without a CASC install — tileColor falls back to the tileset color, so
// the bake still succeeds and embeds a valid BLP.
func TestWriteNewMapEmbedsMinimap(t *testing.T) {
	path := filepath.Join(t.TempDir(), "fresh.w3x")
	if err := writeNewMap(NewMapParams{
		Name: "Fresh", Width: 16, Height: 16, Tileset: "L", Path: path,
	}); err != nil {
		t.Fatalf("writeNewMap: %v", err)
	}

	arc, err := mpq.Open(path)
	if err != nil {
		t.Fatalf("open written .w3x: %v", err)
	}
	defer arc.Close()

	if !arc.Has("war3mapMap.blp") {
		t.Fatalf("new map is missing war3mapMap.blp; List=%v", arc.List())
	}
	raw, err := arc.Read("war3mapMap.blp")
	if err != nil {
		t.Fatalf("read war3mapMap.blp: %v", err)
	}
	if _, err := gw3blp.Decode(bytes.NewReader(raw)); err != nil {
		t.Fatalf("embedded war3mapMap.blp does not decode: %v", err)
	}
}
