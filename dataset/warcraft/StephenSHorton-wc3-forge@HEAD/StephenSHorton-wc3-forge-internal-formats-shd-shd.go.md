---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/shd/shd.go"
ref: HEAD
---

// Package shd parses war3map.shd — the WC3 static shadow map.
//
// Format: raw byte array, no header. Dimensions are derived from the
// terrain: `(terrain_width - 1) * 4` columns × `(terrain_height - 1) * 4`
// rows, where terrain_width/height are vertex counts. Each cell quarters
// into 4×4 sub-cells in the shadow map, giving 16 shadow samples per
// terrain tile.
//
// Cell encoding: empirically 0x00 means "no shadow" and any non-zero
// value means "shadowed" (the WC3 engine treats it as a hard mask, not a
// gradient). We pass the raw byte through and let the renderer decide
// whether to threshold or smooth-blend.
//
// The file is written by the WC3 World Editor when the map author bakes
// doodad/building shadows. Maps without static shadows omit the file
// entirely.
package shd

import "fmt"

// File holds the parsed shadow map.
type File struct {
	// Width × Height in shadow cells (4× the terrain tile count in each axis).
	Width  int
	Height int
	// Cells is row-major, length = Width * Height. cell at (col, row) is
	// `Cells[row*Width + col]`. Row 0 is the bottom of the map (same
	// convention as Tilepoint).
	Cells []byte
}

// Parse decodes raw war3map.shd bytes. terrainW / terrainH are the vertex
// counts from w3e — the shadow map's expected size is `(terrainW-1)*4 *
// (terrainH-1)*4`. Mismatches log a warning but still produce a File with
// a zero-filled cells buffer at the expected size (so downstream rendering
// has predictable input even on broken maps).
func Parse(data []byte, terrainW, terrainH int) (*File, error) {
	if terrainW < 2 || terrainH < 2 {
		return nil, fmt.Errorf("shd: terrain too small (%dx%d)", terrainW, terrainH)
	}
	w := (terrainW - 1) * 4
	h := (terrainH - 1) * 4
	expected := w * h
	if len(data) != expected {
		// Don't fail: same recovery as HiveWE (println + zero-fill).
		out := &File{Width: w, Height: h, Cells: make([]byte, expected)}
		return out, fmt.Errorf("shd: size mismatch (got %d, want %d) — using zero-fill", len(data), expected)
	}
	cells := make([]byte, expected)
	copy(cells, data)
	return &File{Width: w, Height: h, Cells: cells}, nil
}
