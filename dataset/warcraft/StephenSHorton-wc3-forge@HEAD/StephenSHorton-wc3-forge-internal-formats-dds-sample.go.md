---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/dds/sample.go"
ref: HEAD
---

// Package dds extracts a representative average color from a DDS (DirectDraw
// Surface) texture without fully decompressing the image. WC3's Reforged
// CASC ships tileset textures as DDS (BC1/DXT1 or BC3/DXT5 — texture is
// 4×4-block compressed). The full decode requires bit-level decompression
// per block; for our use case (one solid color per tile, used as a
// vibe-accurate stand-in until proper UV-tiled rendering lands) we only
// need the block reference colors.
//
// BC1/DXT1 block layout (8 bytes per 4×4 pixel block):
//   bytes 0-1: color0 (16-bit RGB565, little-endian)
//   bytes 2-3: color1 (16-bit RGB565, little-endian)
//   bytes 4-7: 16×2-bit pixel indices into a 4-color palette
//
// BC3/DXT5 block layout (16 bytes per block): an 8-byte alpha block
// followed by an 8-byte BC1-shaped color block. Same color reference
// positions, just shifted +8.
//
// We sample reference colors from blocks scattered across the image and
// average them. For a tile texture that's mostly one substance (grass,
// snow, rock), this gives an honest representative color — close to what
// the human eye averages when looking at the texture from a distance.
package dds

import (
	"encoding/binary"
	"errors"
)

const (
	headerSize    = 128
	pixelFormatFourCCOffset = 84
	ddsMagic      = "DDS "
)

// AverageColor returns the average RGB (0..255) of the DDS image, sampled
// from the reference colors of compressed blocks scattered across the
// image. Returns an error for unsupported pixel formats.
func AverageColor(data []byte) (r, g, b uint8, err error) {
	if len(data) < headerSize+8 {
		return 0, 0, 0, errors.New("dds: too short")
	}
	if string(data[:4]) != ddsMagic {
		return 0, 0, 0, errors.New("dds: bad magic")
	}

	fourCC := string(data[pixelFormatFourCCOffset : pixelFormatFourCCOffset+4])
	blocks := data[headerSize:]

	var blockSize, colorOffset int
	switch fourCC {
	case "DXT1": // BC1 — 8 bytes/block, color at offset 0
		blockSize, colorOffset = 8, 0
	case "DXT3", "DXT5": // BC2/BC3 — 16 bytes/block, color at offset 8
		blockSize, colorOffset = 16, 8
	default:
		// "DX10" (BC7 etc.) requires DXGI format dispatch from extended header;
		// not handling it yet. Tilesets we've seen are all DXT1.
		return 0, 0, 0, errors.New("dds: unsupported pixel format " + fourCC)
	}

	if len(blocks) < blockSize {
		return 0, 0, 0, errors.New("dds: no blocks")
	}

	// Sample up to 64 blocks evenly spaced. More blocks = more accurate
	// average, but 64 is already plenty for "what color is this tile."
	const maxSamples = 64
	totalBlocks := len(blocks) / blockSize
	if totalBlocks == 0 {
		return 0, 0, 0, errors.New("dds: zero blocks")
	}
	samples := totalBlocks
	if samples > maxSamples {
		samples = maxSamples
	}
	step := totalBlocks / samples
	if step < 1 {
		step = 1
	}

	var rSum, gSum, bSum, count uint32
	for i := 0; i < samples; i++ {
		bIdx := i * step
		off := bIdx*blockSize + colorOffset
		if off+4 > len(blocks) {
			break
		}
		c0 := binary.LittleEndian.Uint16(blocks[off : off+2])
		c1 := binary.LittleEndian.Uint16(blocks[off+2 : off+4])
		r0, g0, b0 := rgb565(c0)
		r1, g1, b1 := rgb565(c1)
		// Average the two reference colors; for typical textures these
		// bound the block's dominant range.
		rSum += uint32(r0) + uint32(r1)
		gSum += uint32(g0) + uint32(g1)
		bSum += uint32(b0) + uint32(b1)
		count += 2
	}
	if count == 0 {
		return 0, 0, 0, errors.New("dds: no samples")
	}
	return uint8(rSum / count), uint8(gSum / count), uint8(bSum / count), nil
}

// rgb565 unpacks a 16-bit RGB565 color into 8-bit components by replicating
// the high bits into the low bits (standard expansion).
func rgb565(c uint16) (uint8, uint8, uint8) {
	r := uint8((c >> 11) & 0x1F)
	g := uint8((c >> 5) & 0x3F)
	b := uint8(c & 0x1F)
	// Bit-replicate to 8-bit. Simpler than "* 255 / 31" and gives correct
	// rounding (1F → FF, 00 → 00).
	r = (r << 3) | (r >> 2)
	g = (g << 2) | (g >> 4)
	b = (b << 3) | (b >> 2)
	return r, g, b
}
