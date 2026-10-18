---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wpm/wpm.go"
ref: HEAD
---

// Package wpm parses war3map.wpm — the WC3 static pathing map.
//
// The pathing map encodes per-quarter-cell movement constraints at 4× the
// terrain tile resolution: a terrain with T×T tiles produces a 4T×4T pathing
// grid. Each cell is a single byte whose bits flag what entity types cannot
// occupy that quarter-cell. Multiple bits may be set on the same cell.
//
// File layout (all little-endian):
//
//	4   magic     "MP3W"
//	4   version   uint32, expected 0
//	4   width     uint32, cells along X (typically terrain_tile_width  * 4)
//	4   height    uint32, cells along Y (typically terrain_tile_height * 4)
//	W*H cells     one byte per cell, row-major, row 0 = bottom row
//
// Cell flag bits (matching HiveWE's enum in src/base/pathing_map.ixx):
//
//	0x02 = unwalkable
//	0x04 = unflyable
//	0x08 = unbuildable
//	0x10 = harvestable
//	0x20 = blight
//	0x40 = water
//	0x80 = amphibious
//
// Reference:
//   - HiveWE: src/base/pathing_map.ixx (PathingMap::load)
//   - WC3MapTranslator (TS, MIT): lib/translators/PathingTranslator.ts
package wpm

import (
	"encoding/binary"
	"fmt"
)

const magic = "MP3W"

// Flag bits (re-exported as named constants for renderer use).
const (
	FlagUnwalkable  byte = 0x02
	FlagUnflyable   byte = 0x04
	FlagUnbuildable byte = 0x08
	FlagHarvestable byte = 0x10
	FlagBlight      byte = 0x20
	FlagWater       byte = 0x40
	FlagAmphibious  byte = 0x80
)

// File holds the parsed pathing map.
type File struct {
	// Width × Height in pathing cells (4× the terrain tile count in each axis
	// for well-formed maps; we trust whatever the file declares).
	Width  int
	Height int
	// Cells is row-major, length = Width * Height. Cell at (col, row) is
	// `Cells[row*Width + col]`. Row 0 is the bottom of the map.
	Cells []byte
}

// Parse decodes raw war3map.wpm bytes. Returns an error on header truncation
// or magic mismatch. A size-vs-declared-dims mismatch is recoverable: the file
// is returned with whatever cell bytes were available, zero-padded to the
// declared Width*Height.
func Parse(data []byte) (*File, error) {
	if len(data) < 16 {
		return nil, fmt.Errorf("wpm: too short (%d bytes)", len(data))
	}
	if string(data[:4]) != magic {
		return nil, fmt.Errorf("wpm: bad magic %q, want %q", string(data[:4]), magic)
	}
	version := binary.LittleEndian.Uint32(data[4:8])
	w := int(binary.LittleEndian.Uint32(data[8:12]))
	h := int(binary.LittleEndian.Uint32(data[12:16]))
	if w <= 0 || h <= 0 {
		return nil, fmt.Errorf("wpm: non-positive dimensions %dx%d", w, h)
	}
	// Guard against absurd dimensions from corrupted/protected files. 8192²
	// is far beyond any real WC3 map (480² playable max → 1920² pathing).
	const maxDim = 8192
	if w > maxDim || h > maxDim {
		return nil, fmt.Errorf("wpm: dimensions %dx%d exceed sanity cap", w, h)
	}
	want := w * h
	cells := make([]byte, want)
	have := len(data) - 16
	if have < want {
		copy(cells, data[16:])
		return &File{Width: w, Height: h, Cells: cells}, fmt.Errorf(
			"wpm: short payload (got %d cells, want %d) — zero-padded", have, want)
	}
	copy(cells, data[16:16+want])
	if version != 0 {
		// Soft-warn via err. Modern WC3 only writes version 0; non-zero may
		// indicate a future format change. We still return a usable File.
		return &File{Width: w, Height: h, Cells: cells}, fmt.Errorf(
			"wpm: unknown version %d (expected 0); parsed assuming v0 layout", version)
	}
	return &File{Width: w, Height: h, Cells: cells}, nil
}
