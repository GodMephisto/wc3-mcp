---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/dds/decode.go"
ref: HEAD
---

package dds

import (
	"encoding/binary"
	"errors"
	"image"
)

// Decode returns the top-mip of a DDS file as an RGBA image. Supports BC1
// (DXT1) and BC3 (DXT5) — the two compressed formats Reforged CASC actually
// uses for tile textures. Alpha is decoded for BC3; BC1 yields fully-opaque
// pixels (we ignore BC1's optional 1-bit alpha encoding, since tile textures
// don't use it).
//
// We only need the top mip for thumbnail rendering. Lower mips are skipped.
func Decode(data []byte) (image.Image, error) {
	if len(data) < headerSize+8 {
		return nil, errors.New("dds: too short")
	}
	if string(data[:4]) != ddsMagic {
		return nil, errors.New("dds: bad magic")
	}
	// Header layout (we only need height + width here):
	//   offset 12: uint32 height
	//   offset 16: uint32 width
	height := int(binary.LittleEndian.Uint32(data[12:16]))
	width := int(binary.LittleEndian.Uint32(data[16:20]))
	if width <= 0 || height <= 0 || width > 8192 || height > 8192 {
		return nil, errors.New("dds: implausible dimensions")
	}

	fourCC := string(data[pixelFormatFourCCOffset : pixelFormatFourCCOffset+4])
	blocks := data[headerSize:]

	var blockSize int
	withAlpha := false
	switch fourCC {
	case "DXT1":
		blockSize = 8
	case "DXT3", "DXT5":
		blockSize = 16
		withAlpha = true
	default:
		return nil, errors.New("dds: unsupported pixel format " + fourCC)
	}

	bw := (width + 3) / 4
	bh := (height + 3) / 4
	need := bw * bh * blockSize
	if len(blocks) < need {
		return nil, errors.New("dds: truncated block data")
	}

	img := image.NewRGBA(image.Rect(0, 0, width, height))
	stride := img.Stride

	for by := 0; by < bh; by++ {
		for bx := 0; bx < bw; bx++ {
			off := (by*bw + bx) * blockSize
			block := blocks[off : off+blockSize]
			colorBlock := block
			if withAlpha {
				colorBlock = block[8:]
			}

			c0 := binary.LittleEndian.Uint16(colorBlock[0:2])
			c1 := binary.LittleEndian.Uint16(colorBlock[2:4])
			bits := binary.LittleEndian.Uint32(colorBlock[4:8])

			r0, g0, b0 := rgb565(c0)
			r1, g1, b1 := rgb565(c1)

			// Palette entries 2 and 3 interpolate. When c0 > c1 it's the
			// "4-color" mode (1/3, 2/3 mix); otherwise it's the "3-color +
			// transparent" mode, where the 4th palette entry is black/
			// transparent. For tile thumbnails we don't care about the
			// transparency distinction — render entry 3 as black in 3-color
			// mode, fully opaque always.
			pal := [4][4]uint8{
				{r0, g0, b0, 0xFF},
				{r1, g1, b1, 0xFF},
			}
			if c0 > c1 {
				pal[2] = [4]uint8{
					uint8((2*uint16(r0) + uint16(r1)) / 3),
					uint8((2*uint16(g0) + uint16(g1)) / 3),
					uint8((2*uint16(b0) + uint16(b1)) / 3),
					0xFF,
				}
				pal[3] = [4]uint8{
					uint8((uint16(r0) + 2*uint16(r1)) / 3),
					uint8((uint16(g0) + 2*uint16(g1)) / 3),
					uint8((uint16(b0) + 2*uint16(b1)) / 3),
					0xFF,
				}
			} else {
				pal[2] = [4]uint8{
					uint8((uint16(r0) + uint16(r1)) / 2),
					uint8((uint16(g0) + uint16(g1)) / 2),
					uint8((uint16(b0) + uint16(b1)) / 2),
					0xFF,
				}
				pal[3] = [4]uint8{0, 0, 0, 0xFF}
			}

			// Optional alpha plane (BC3). DXT5 alpha is two reference values
			// + 3-bit-per-pixel interpolated palette. We decode just enough
			// to drive the per-pixel alpha here; thumbnails very rarely use
			// it, but if some tileset starts shipping with it we don't want
			// surprises.
			var aPal [8]uint8
			var aBits uint64
			if withAlpha {
				a0 := block[0]
				a1 := block[1]
				aPal[0] = a0
				aPal[1] = a1
				if a0 > a1 {
					for i := 1; i < 7; i++ {
						aPal[i+1] = uint8((uint16(a0)*uint16(7-i) + uint16(a1)*uint16(i)) / 7)
					}
				} else {
					for i := 1; i < 5; i++ {
						aPal[i+1] = uint8((uint16(a0)*uint16(5-i) + uint16(a1)*uint16(i)) / 5)
					}
					aPal[6] = 0
					aPal[7] = 0xFF
				}
				aBits = uint64(block[2]) |
					uint64(block[3])<<8 |
					uint64(block[4])<<16 |
					uint64(block[5])<<24 |
					uint64(block[6])<<32 |
					uint64(block[7])<<40
			}

			for py := 0; py < 4; py++ {
				y := by*4 + py
				if y >= height {
					break
				}
				for px := 0; px < 4; px++ {
					x := bx*4 + px
					if x >= width {
						break
					}
					pixIdx := py*4 + px
					colorIdx := (bits >> uint(2*pixIdx)) & 0x3
					c := pal[colorIdx]
					if withAlpha {
						aIdx := uint((aBits >> uint(3*pixIdx)) & 0x7)
						c[3] = aPal[aIdx]
					}
					p := y*stride + x*4
					img.Pix[p+0] = c[0]
					img.Pix[p+1] = c[1]
					img.Pix[p+2] = c[2]
					img.Pix[p+3] = c[3]
				}
			}
		}
	}
	return img, nil
}
