---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3e/w3e.go"
ref: HEAD
---

// Package w3e parses Warcraft III terrain environment files (war3map.w3e).
//
// The .w3e file describes the terrain heightmap, tile texture indices, cliff
// data, water levels, and various per-vertex flags (ramp, blight, water,
// boundary) for a Warcraft III map. Vertex counts on the wire are one more
// than the tile count along each axis (a 64-tile map has 65 vertex columns
// of corner data), and a border of unplayable padding is included in the
// stored width/height — most modern Reforged maps end up at 67×67 vertices
// for a 64×64-tile playable area plus the 1-tile border padding on each side
// that wc3 maps always carry.
//
// File layout (all little-endian):
//
//	4   magic         "W3E!"
//	4   version       uint32, usually 11; Reforged sometimes writes higher
//	1   tileset       ASCII letter ('L' Lordaeron, 'A' Ashenvale, …, 'X' Reforged)
//	4   custom_flag   uint32, nonzero = custom tile palette
//	4   n_ground      uint32, count of ground tileset FourCCs
//	4*n FourCCs       each 4 bytes ASCII
//	4   n_cliff       uint32, count of cliff tileset FourCCs
//	4*n FourCCs       each 4 bytes ASCII
//	4   width         uint32, vertex count along X
//	4   height        uint32, vertex count along Y
//	8   center_offset 2 × float32 — game coords of vertex (0, 0); bottom-left
//	7*W*H tilepoints  packed per-vertex data, row-major, row 0 = bottom row
//
// Per-tilepoint structure — TWO wire layouts, switched on `version`:
//
// Version >= 12 (Reforged, 8 bytes/vertex):
//
//	int16  ground_height          — raw; game Z = (raw - 0x2000) / 4.0
//	uint16 water_and_flags        — bits 0..13 = water height raw (same scale)
//	                                bit 14 = boundary; bit 15 = unused
//	uint16 texture_and_flags      — bits 0..5  = ground texture index (0..63)
//	                                bit 6  = ramp
//	                                bit 7  = blight
//	                                bit 8  = water
//	                                bit 9  = boundary
//	                                (bits 10..15 unused)
//	uint8  variation              — bits 0..4 = ground variation (0..31)
//	                                bits 5..7 = cliff variation  (0..7)
//	uint8  misc                   — high nibble = cliff texture (0..15)
//	                                low nibble  = layer height   (0..15)
//
// Version < 12 (Classic/TFT, 7 bytes/vertex):
//
//	int16  ground_height          — same encoding as above
//	uint16 water_and_flags        — same as above
//	uint8  texture_and_flags      — low nibble = ground texture index (0..15)
//	                                bit 4 = ramp
//	                                bit 5 = blight
//	                                bit 6 = water
//	                                bit 7 = boundary
//	uint8  variation              — same 5+3 split as above
//	uint8  misc                   — same as above
//
// The 0x2000 baseline (decimal 8192) and the /4 elevation scaling are the
// same convention the WC3 engine itself uses — JASS GetTerrainCliffLevel and
// GetLocationZ are derived from these same packed bytes.
//
// Coordinate convention: per-tilepoint positions are derived externally as
//
//	game_pos = CenterOffset + (col*128, row*128)
//
// We deliberately do NOT bake game-space x/y into each Tilepoint — keeping
// them as grid indices means callers can stride through the slice without
// allocating per-vertex Vec2s, and the file's offset is a single value
// stored on File.
//
// Reference:
//   - HiveWE: src/base/terrain.ixx (Terrain::load — canonical C++ reader)
//   - WC3MapTranslator (TS, MIT): lib/translators/TerrainTranslator.ts
package w3e

import (
	"encoding/binary"
	"errors"
	"fmt"
	"math"
)

// Magic is the 4-byte file signature: ASCII "W3E!".
var Magic = [4]byte{'W', '3', 'E', '!'}

// maxDimension caps the vertex count along either axis. The largest WC3
// playable map is 480×480 tiles = 481×481 vertices + a border, so a few
// hundred is the realistic ceiling. We cap at 4096 to defeat protector
// maps that lie about their dimensions and try to provoke gigabyte
// allocations on parse (HiveWE applies a similar cap in regions.ixx).
const maxDimension = 4096

// File is the decoded contents of a war3map.w3e file.
type File struct {
	Format         [4]byte    // "W3E!"
	Version        uint32     // typically 11
	Tileset        byte       // ASCII letter: 'L'=Lordaeron Summer, 'A'=Ashenvale, 'X'=Reforged, etc.
	CustomTileset  bool       // true if the map uses a custom tile palette
	GroundTilesets []string   // FourCCs (4-byte ASCII), one per ground type slot, max 16
	CliffTilesets  []string   // FourCCs (4-byte ASCII), one per cliff type slot, max 16
	Width          uint32     // vertex count along X (= playable_width + 1 + 2*border)
	Height         uint32     // vertex count along Y
	CenterOffset   [2]float32 // game coords of vertex (0, 0); typically (-N*64, -N*64)
	Tiles          []Tilepoint // length = Width * Height, row-major (row 0 = bottom)
}

// Tilepoint is the raw packed per-vertex data, post-decode. Both wire
// layouts (v11 and v12+) normalise into this same struct — Flags carries
// the ramp/blight/water/boundary bits in a uniform low-nibble layout
// regardless of source format.
//
// Use the helper methods (Z, WaterZ, GroundTexIdx, CliffTexIdx, and the
// Has* flag predicates) to get derived values.
type Tilepoint struct {
	GroundHeight   int16  // raw 16-bit; subtract 0x2000 and divide by 4 for game-space Z
	WaterLevel     uint16 // raw 14-bit water height (low bits of the file's water_and_flags word)
	Flags          uint8  // normalised: 0x1 ramp, 0x2 blight, 0x4 water, 0x8 boundary
	GroundTexture  uint8  // ground tileset index — 0..15 (v11) or 0..63 (v12+)
	TextureDetails uint8  // ground variation (low 5 bits) + cliff variation (top 3 bits)
	CliffTexture   uint8  // cliff tileset index 0..15
	LayerHeight    uint8  // cliff layer 0..15
	// Boundary is the bit-14 marker pulled out of the water_and_flags word
	// during parse. It travels separately because callers want it even when
	// they aren't otherwise inspecting Flags. (HiveWE stores it as
	// corner_map_edge.) Distinct from the boundary bit inside Flags, which
	// some maps set independently.
	Boundary bool
}

// Z returns the game-space ground elevation in WC3 world units, EXCLUDING
// the cliff layer contribution. This is the "small float" wobble around a
// layer's nominal Z (range typically ±32 studs from level). For the actual
// height of a rendered terrain corner you want FinalZ, which adds the
// cliff-layer offset on top.
//
// Encoding: stored as `(game_z * 4 + 0x2000)` in an unsigned-ish int16 so
// that flat ground (Z = 0) lands on 8192 and the full elevation range
// (±2 cliff levels of ±128) fits comfortably.
func (t Tilepoint) Z() float32 {
	return float32(int32(t.GroundHeight)-0x2000) / 4.0
}

// FinalZ returns the rendered corner Z: per-corner small float wobble plus
// the cliff-layer offset. WC3 cliffs are integer LayerHeight steps; each
// step is 128 studs (one tile width). Layer 2 is the "nominal ground"
// reference (per HiveWE's `corner_final_ground_height = corner_height +
// corner_layer_height - 2.0`, then scaled by 128 for our stud space).
//
// Without this, terrain renders as a flat slab even on maps with cliffs —
// the per-corner Z() values alone don't capture the multi-level elevation.
func (t Tilepoint) FinalZ() float32 {
	return t.Z() + (float32(t.LayerHeight)-2.0)*128.0
}

// WaterZ returns the game-space water surface elevation, same encoding
// as Z. Only meaningful when HasWater() is true.
func (t Tilepoint) WaterZ() float32 {
	return float32(int32(t.WaterLevel)-0x2000) / 4.0
}

// GroundTexIdx is the ground-tileset slot index into File.GroundTilesets.
// Range is 0..15 on legacy (v11-) maps and 0..63 on Reforged (v12+) maps —
// the parser already masks to the correct width based on file version, so
// this accessor is just a named pass-through that documents intent.
func (t Tilepoint) GroundTexIdx() uint8 { return t.GroundTexture }

// CliffTexIdx is the cliff-tileset slot index (0..15) into File.CliffTilesets.
// A value of 15 commonly means "no cliff" (see HiveWE Terrain::real_tile_texture).
func (t Tilepoint) CliffTexIdx() uint8 { return t.CliffTexture & 0x0F }

// HasRamp reports whether this vertex is part of a cliff ramp.
func (t Tilepoint) HasRamp() bool { return t.Flags&0x1 != 0 }

// HasBlight reports whether this vertex is blighted (undead corruption).
func (t Tilepoint) HasBlight() bool { return t.Flags&0x2 != 0 }

// HasWater reports whether this vertex has water above it.
func (t Tilepoint) HasWater() bool { return t.Flags&0x4 != 0 }

// HasBoundary reports whether this vertex sits inside the playable boundary
// (flagged 1) versus the unplayable border padding (flagged 0). Note that
// the file also stores a redundant boundary bit inside the water_and_flags
// word — we surface that separately as Tilepoint.Boundary.
func (t Tilepoint) HasBoundary() bool { return t.Flags&0x8 != 0 }

// Parse decodes a war3map.w3e byte slice into a File. It returns an error
// for malformed inputs (bad magic, truncated data, hostile dimensions).
// Caller retains ownership of data; nothing in the returned File aliases
// the input slice.
func Parse(data []byte) (*File, error) {
	r := &reader{buf: data}

	var f File

	magic, err := r.readBytes(4)
	if err != nil {
		return nil, fmt.Errorf("w3e: read magic: %w", err)
	}
	copy(f.Format[:], magic)
	if f.Format != Magic {
		return nil, fmt.Errorf("w3e: bad magic %q, want %q", f.Format[:], Magic[:])
	}

	if f.Version, err = r.u32(); err != nil {
		return nil, fmt.Errorf("w3e: read version: %w", err)
	}

	tilesetByte, err := r.u8()
	if err != nil {
		return nil, fmt.Errorf("w3e: read tileset: %w", err)
	}
	f.Tileset = tilesetByte

	// HiveWE reads custom_tileset as a 4-byte int but only the low bit is
	// ever used by Blizzard's editor. Treat any nonzero value as "custom".
	customFlag, err := r.u32()
	if err != nil {
		return nil, fmt.Errorf("w3e: read custom_tileset: %w", err)
	}
	f.CustomTileset = customFlag != 0

	// Ground tilesets.
	nGround, err := r.u32()
	if err != nil {
		return nil, fmt.Errorf("w3e: read ground_tileset_count: %w", err)
	}
	if nGround > 64 {
		// Sanity bound — Blizzard's editor caps at 16, but custom maps
		// occasionally have more. Anything past 64 is almost certainly
		// a protector trick or a corrupt file.
		return nil, fmt.Errorf("w3e: implausible ground_tileset_count %d", nGround)
	}
	f.GroundTilesets = make([]string, nGround)
	for i := range f.GroundTilesets {
		b, err := r.readBytes(4)
		if err != nil {
			return nil, fmt.Errorf("w3e: read ground tileset[%d]: %w", i, err)
		}
		f.GroundTilesets[i] = string(b)
	}

	// Cliff tilesets.
	nCliff, err := r.u32()
	if err != nil {
		return nil, fmt.Errorf("w3e: read cliff_tileset_count: %w", err)
	}
	if nCliff > 64 {
		return nil, fmt.Errorf("w3e: implausible cliff_tileset_count %d", nCliff)
	}
	f.CliffTilesets = make([]string, nCliff)
	for i := range f.CliffTilesets {
		b, err := r.readBytes(4)
		if err != nil {
			return nil, fmt.Errorf("w3e: read cliff tileset[%d]: %w", i, err)
		}
		f.CliffTilesets[i] = string(b)
	}

	// Dimensions (vertex counts, NOT tile counts — a 64×64-tile playable
	// area has 65 vertices per axis before the 1-tile border padding adds
	// two more, giving 67×67 vertices on the wire).
	if f.Width, err = r.u32(); err != nil {
		return nil, fmt.Errorf("w3e: read width: %w", err)
	}
	if f.Height, err = r.u32(); err != nil {
		return nil, fmt.Errorf("w3e: read height: %w", err)
	}
	if f.Width == 0 || f.Height == 0 {
		return nil, fmt.Errorf("w3e: zero dimension %dx%d", f.Width, f.Height)
	}
	if f.Width > maxDimension || f.Height > maxDimension {
		return nil, fmt.Errorf("w3e: hostile dimensions %dx%d (cap %d)", f.Width, f.Height, maxDimension)
	}

	// Center offset — game coordinates of vertex (0,0), i.e. the
	// bottom-left corner of the map.
	ox, err := r.f32()
	if err != nil {
		return nil, fmt.Errorf("w3e: read center_offset.x: %w", err)
	}
	oy, err := r.f32()
	if err != nil {
		return nil, fmt.Errorf("w3e: read center_offset.y: %w", err)
	}
	f.CenterOffset = [2]float32{ox, oy}

	// Tilepoints — 8 bytes/vertex on v12+ (Reforged), 7 bytes on v11 and earlier.
	// We pick the layout based on the version field we already parsed.
	total := int(f.Width) * int(f.Height)
	bytesPerTilepoint := 7
	if f.Version >= 12 {
		bytesPerTilepoint = 8
	}
	tpBytes, err := r.readBytes(total * bytesPerTilepoint)
	if err != nil {
		return nil, fmt.Errorf("w3e: read %d tilepoints (need %d bytes at %d B/vertex, version %d): %w",
			total, total*bytesPerTilepoint, bytesPerTilepoint, f.Version, err)
	}

	f.Tiles = make([]Tilepoint, total)
	for i := 0; i < total; i++ {
		off := i * bytesPerTilepoint

		// Bytes 0-1: int16 ground_height (LE).
		gh := int16(binary.LittleEndian.Uint16(tpBytes[off+0 : off+2]))

		// Bytes 2-3: uint16 water_and_flags (LE).
		//   bits 0..13 = water height raw (same encoding as ground_height)
		//   bit  14    = boundary (HiveWE corner_map_edge)
		//   bit  15    = unused/reserved
		wf := binary.LittleEndian.Uint16(tpBytes[off+2 : off+4])
		water := wf & 0x3FFF
		boundaryBit := wf&0x4000 != 0

		// The remaining 3-4 bytes diverge between v11 and v12+:
		//
		//   v12+ (8 bytes total):  uint16 texture_and_flags + uint8 variation + uint8 misc
		//   v11- (7 bytes total):  uint8  texture_and_flags + uint8 variation + uint8 misc
		//
		// Both produce the same normalised Tilepoint — only the bit widths
		// of the texture_index and flag positions change. We unify into the
		// same Flags low-nibble layout (0x1 ramp / 0x2 blight / 0x4 water /
		// 0x8 boundary) so callers don't have to think about versions.
		var (
			flags     uint8
			groundTex uint8
			variation uint8
			cliffTex  uint8
			layer     uint8
		)
		if f.Version >= 12 {
			// uint16 texture_and_flags
			//   bits 0..5 = ground texture index (0..63)
			//   bit 6 ramp / bit 7 blight / bit 8 water / bit 9 boundary
			tf := binary.LittleEndian.Uint16(tpBytes[off+4 : off+6])
			groundTex = uint8(tf & 0x003F)
			if tf&0x0040 != 0 {
				flags |= 0x1
			}
			if tf&0x0080 != 0 {
				flags |= 0x2
			}
			if tf&0x0100 != 0 {
				flags |= 0x4
			}
			if tf&0x0200 != 0 {
				flags |= 0x8
			}
			// uint8 variation: low 5 bits ground, top 3 bits cliff
			vb := tpBytes[off+6]
			variation = vb // keep both nibbles packed (decoded by accessors below if added)
			// uint8 misc: high nibble cliff_texture, low nibble layer_height
			mb := tpBytes[off+7]
			cliffTex = (mb >> 4) & 0x0F
			layer = mb & 0x0F
		} else {
			// Legacy: uint8 texture_and_flags
			//   low nibble = ground texture index (0..15)
			//   bit 4 ramp / bit 5 blight / bit 6 water / bit 7 boundary
			tf := tpBytes[off+4]
			groundTex = tf & 0x0F
			if tf&0x10 != 0 {
				flags |= 0x1
			}
			if tf&0x20 != 0 {
				flags |= 0x2
			}
			if tf&0x40 != 0 {
				flags |= 0x4
			}
			if tf&0x80 != 0 {
				flags |= 0x8
			}
			vb := tpBytes[off+5]
			variation = vb
			mb := tpBytes[off+6]
			cliffTex = (mb >> 4) & 0x0F
			layer = mb & 0x0F
		}

		f.Tiles[i] = Tilepoint{
			GroundHeight:   gh,
			WaterLevel:     water,
			Flags:          flags,
			GroundTexture:  groundTex,
			TextureDetails: variation,
			CliffTexture:   cliffTex,
			LayerHeight:    layer,
			Boundary:       boundaryBit,
		}
	}

	return &f, nil
}

// ---- internal byte reader -------------------------------------------------

type reader struct {
	buf []byte
	pos int
}

var errEOF = errors.New("unexpected end of file")

func (r *reader) readBytes(n int) ([]byte, error) {
	if n < 0 {
		return nil, fmt.Errorf("negative read size %d", n)
	}
	if r.pos+n > len(r.buf) {
		return nil, errEOF
	}
	out := r.buf[r.pos : r.pos+n]
	r.pos += n
	return out, nil
}

func (r *reader) u8() (uint8, error) {
	b, err := r.readBytes(1)
	if err != nil {
		return 0, err
	}
	return b[0], nil
}

func (r *reader) u32() (uint32, error) {
	b, err := r.readBytes(4)
	if err != nil {
		return 0, err
	}
	return binary.LittleEndian.Uint32(b), nil
}

func (r *reader) f32() (float32, error) {
	v, err := r.u32()
	if err != nil {
		return 0, err
	}
	return math.Float32frombits(v), nil
}

