---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3e/encode.go"
ref: HEAD
---

package w3e

import (
	"encoding/binary"
	"fmt"
	"math"
)

// Encode serialises a File back to the on-wire war3map.w3e byte layout
// documented at the top of w3e.go. The encoding is the inverse of Parse;
// Parse(Encode(f)) must equal the original f for any File produced by Parse.
//
// Pitfalls that cost real time the first time around:
//   - Flags bit 3 (HasBoundary) and Tilepoint.Boundary are TWO different
//     boundary bits. The former lives in texture_and_flags (bit 9 on v12+,
//     bit 7 on v11); the latter lives in water_and_flags bit 14. Parse keeps
//     them separate on purpose — encode must write them back to their own
//     bit positions, not OR them together.
//   - GroundTilesets / CliffTilesets entries are exact 4-byte ASCII FourCCs;
//     we require len(s) == 4 and bail loudly otherwise rather than padding
//     silently, since a wrong-length FourCC bricks the map in WC3.
//   - On v12+ the ground texture index is 6 bits wide (0..63); on v11- it's
//     4 bits (0..15). A caller that swaps to a tileset with too many tiles
//     for the file version will be rejected here, not silently truncated.
func Encode(f *File) ([]byte, error) {
	if f == nil {
		return nil, fmt.Errorf("w3e: encode nil file")
	}
	if f.Format != Magic {
		return nil, fmt.Errorf("w3e: bad magic %q, want %q", f.Format[:], Magic[:])
	}
	if f.Width == 0 || f.Height == 0 {
		return nil, fmt.Errorf("w3e: zero dimension %dx%d", f.Width, f.Height)
	}
	if f.Width > maxDimension || f.Height > maxDimension {
		return nil, fmt.Errorf("w3e: hostile dimensions %dx%d (cap %d)", f.Width, f.Height, maxDimension)
	}
	wantTiles := int(f.Width) * int(f.Height)
	if len(f.Tiles) != wantTiles {
		return nil, fmt.Errorf("w3e: Tiles len = %d, want %d (= Width*Height)", len(f.Tiles), wantTiles)
	}
	if len(f.GroundTilesets) == 0 {
		return nil, fmt.Errorf("w3e: at least one ground tileset required")
	}

	bytesPerTilepoint := 7
	if f.Version >= 12 {
		bytesPerTilepoint = 8
	}
	maxGroundIdx := uint8(15)
	if f.Version >= 12 {
		maxGroundIdx = 63
	}

	if uint32(len(f.GroundTilesets)) > uint32(maxGroundIdx)+1 {
		return nil, fmt.Errorf("w3e: %d ground tilesets exceed version-%d cap of %d",
			len(f.GroundTilesets), f.Version, maxGroundIdx+1)
	}
	if len(f.CliffTilesets) > 16 {
		return nil, fmt.Errorf("w3e: %d cliff tilesets exceed cap of 16", len(f.CliffTilesets))
	}

	for i, id := range f.GroundTilesets {
		if len(id) != 4 {
			return nil, fmt.Errorf("w3e: GroundTilesets[%d] = %q (len %d), want 4-char FourCC", i, id, len(id))
		}
	}
	for i, id := range f.CliffTilesets {
		if len(id) != 4 {
			return nil, fmt.Errorf("w3e: CliffTilesets[%d] = %q (len %d), want 4-char FourCC", i, id, len(id))
		}
	}

	headerSize := 4 + 4 + 1 + 4 + 4 + 4*len(f.GroundTilesets) + 4 + 4*len(f.CliffTilesets) + 4 + 4 + 4 + 4
	out := make([]byte, 0, headerSize+wantTiles*bytesPerTilepoint)

	out = append(out, f.Format[:]...)
	out = binary.LittleEndian.AppendUint32(out, f.Version)
	out = append(out, f.Tileset)

	customFlag := uint32(0)
	if f.CustomTileset {
		customFlag = 1
	}
	out = binary.LittleEndian.AppendUint32(out, customFlag)

	out = binary.LittleEndian.AppendUint32(out, uint32(len(f.GroundTilesets)))
	for _, id := range f.GroundTilesets {
		out = append(out, id...)
	}
	out = binary.LittleEndian.AppendUint32(out, uint32(len(f.CliffTilesets)))
	for _, id := range f.CliffTilesets {
		out = append(out, id...)
	}

	out = binary.LittleEndian.AppendUint32(out, f.Width)
	out = binary.LittleEndian.AppendUint32(out, f.Height)
	out = binary.LittleEndian.AppendUint32(out, math.Float32bits(f.CenterOffset[0]))
	out = binary.LittleEndian.AppendUint32(out, math.Float32bits(f.CenterOffset[1]))

	var scratch [8]byte
	for i := range f.Tiles {
		tp := f.Tiles[i]

		if tp.GroundTexture > maxGroundIdx {
			return nil, fmt.Errorf("w3e: Tiles[%d].GroundTexture %d exceeds version-%d cap %d",
				i, tp.GroundTexture, f.Version, maxGroundIdx)
		}
		if tp.CliffTexture > 15 {
			return nil, fmt.Errorf("w3e: Tiles[%d].CliffTexture %d exceeds cap 15", i, tp.CliffTexture)
		}
		if tp.LayerHeight > 15 {
			return nil, fmt.Errorf("w3e: Tiles[%d].LayerHeight %d exceeds cap 15", i, tp.LayerHeight)
		}

		binary.LittleEndian.PutUint16(scratch[0:2], uint16(tp.GroundHeight))

		wf := tp.WaterLevel & 0x3FFF
		if tp.Boundary {
			wf |= 0x4000
		}
		binary.LittleEndian.PutUint16(scratch[2:4], wf)

		if f.Version >= 12 {
			tf := uint16(tp.GroundTexture) & 0x003F
			if tp.Flags&0x1 != 0 {
				tf |= 0x0040
			}
			if tp.Flags&0x2 != 0 {
				tf |= 0x0080
			}
			if tp.Flags&0x4 != 0 {
				tf |= 0x0100
			}
			if tp.Flags&0x8 != 0 {
				tf |= 0x0200
			}
			binary.LittleEndian.PutUint16(scratch[4:6], tf)
			scratch[6] = tp.TextureDetails
			scratch[7] = (tp.CliffTexture&0x0F)<<4 | (tp.LayerHeight & 0x0F)
			out = append(out, scratch[0:8]...)
		} else {
			tf := tp.GroundTexture & 0x0F
			if tp.Flags&0x1 != 0 {
				tf |= 0x10
			}
			if tp.Flags&0x2 != 0 {
				tf |= 0x20
			}
			if tp.Flags&0x4 != 0 {
				tf |= 0x40
			}
			if tp.Flags&0x8 != 0 {
				tf |= 0x80
			}
			scratch[4] = tf
			scratch[5] = tp.TextureDetails
			scratch[6] = (tp.CliffTexture&0x0F)<<4 | (tp.LayerHeight & 0x0F)
			out = append(out, scratch[0:7]...)
		}
	}

	return out, nil
}
