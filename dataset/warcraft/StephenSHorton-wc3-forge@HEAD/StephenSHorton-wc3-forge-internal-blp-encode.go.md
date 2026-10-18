---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/blp/encode.go"
ref: HEAD
---

// Package blp implements a pure-Go encoder for the BLP1/JPEG texture format
// used by Warcraft III (Classic / SD) model textures.
//
// gowarcraft3's file/blp package can DECODE this format but cannot encode it;
// this package provides the missing encoder. The wire layout is mirrored exactly
// from that decoder (the ground truth) so encoded output round-trips through it.
//
// BLP1/JPEG container (all little-endian):
//
//	"BLP1"              magic        (4 bytes)
//	uint32 compression  // 0 = JPEG
//	uint32 alphaBits    // 0 or 8 (decoder accepts only these)
//	uint32 width
//	uint32 height
//	uint32 flags        // pictureType-ish; 5 is a common value
//	uint32 hasMipmap    // 1 if a full mip chain is present
//	uint32 mipOffset[16] // ABSOLUTE byte offset of each mip's JPEG body
//	uint32 mipSize[16]   // byte length of each mip's JPEG body
//	uint32 jpegHeaderSize // length of the shared JPEG header blob (we emit 0)
//	byte   jpegHeader[jpegHeaderSize]
//	... concatenated per-mip JPEG bodies at mipOffset[i] ...
//
// We emit jpegHeaderSize = 0 and store each mip's complete JPEG stream
// (header + body) in its own mip slot. This is the simplest layout the
// gowarcraft3 decoder accepts (it concatenates the shared header with the
// mip body; with an empty shared header the mip body is a standalone JPEG).
//
// WC3 model textures require a FULL mip chain down to 1x1 (a mip0-only texture
// renders invisible in-game) and power-of-two dimensions, so Encode resizes the
// input to the nearest power of two and generates every mip level.
//
// WC3 JPEG-BLP stores color as BGR(A); the decoder swaps B<->R back on load, so
// the encoder swaps R<->B before handing pixels to image/jpeg.
package blp

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"image"
	"image/color"
	"image/jpeg"
)

const (
	// header size in bytes: magic(4) + 6 uint32 fields + 16 offsets + 16 sizes,
	// followed by the uint32 jpegHeaderSize field.
	headerFields = 4 + 6*4 + 16*4 + 16*4 // 156
	maxMips      = 16

	// jpegQuality is the quality passed to image/jpeg. 85 keeps the BGR-swap
	// round-trip well within lossy tolerance while staying compact.
	jpegQuality = 85

	compressionJPEG = 0
	flagsDefault    = 5
)

// Encode converts an image into a BLP1/JPEG texture WC3 can use as a model
// texture. The image is resized to the nearest power-of-two dimensions, a full
// mip chain down to 1x1 is generated, each mip is BGR-swapped and JPEG-encoded,
// and the result is wrapped in a BLP1 header.
//
// alphaBits is recorded as 0 (JPEG is opaque RGB); WC3 reads the texture as RGB.
func Encode(img image.Image) ([]byte, error) {
	if img == nil {
		return nil, fmt.Errorf("blp: nil image")
	}

	// Resize to the nearest power-of-two dimensions (mip0).
	w := nearestPow2(img.Bounds().Dx())
	h := nearestPow2(img.Bounds().Dy())
	if w < 1 {
		w = 1
	}
	if h < 1 {
		h = 1
	}
	mip0 := resize(img, w, h)

	// Build the full mip chain: each level halves both dimensions (min 1) until
	// both reach 1. Count = log2(max(w,h)) + 1.
	mips := buildMipChain(mip0)
	if len(mips) > maxMips {
		return nil, fmt.Errorf("blp: %d mip levels exceeds the %d-slot table (input too large)", len(mips), maxMips)
	}

	// JPEG-encode each mip after the R<->B (BGR) swap.
	bodies := make([][]byte, len(mips))
	for i, m := range mips {
		swapped := bgrSwap(m)
		var buf bytes.Buffer
		if err := jpeg.Encode(&buf, swapped, &jpeg.Options{Quality: jpegQuality}); err != nil {
			return nil, fmt.Errorf("blp: jpeg encode mip %d: %w", i, err)
		}
		bodies[i] = buf.Bytes()
	}

	// Assemble. jpegHeaderSize = 0, so mip bodies start at headerFields+4.
	var out bytes.Buffer
	out.Grow(headerFields + 4)

	out.WriteString("BLP1")
	writeU32(&out, compressionJPEG)
	writeU32(&out, 0) // alphaBits
	writeU32(&out, uint32(w))
	writeU32(&out, uint32(h))
	writeU32(&out, flagsDefault)
	writeU32(&out, 1) // hasMipmap

	// Mip bodies are laid out contiguously starting right after the
	// jpegHeaderSize field (which sits at headerFields, 4 bytes wide).
	dataStart := headerFields + 4
	offsets := make([]uint32, maxMips)
	sizes := make([]uint32, maxMips)
	cursor := dataStart
	for i, b := range bodies {
		offsets[i] = uint32(cursor)
		sizes[i] = uint32(len(b))
		cursor += len(b)
	}
	for i := 0; i < maxMips; i++ {
		writeU32(&out, offsets[i])
	}
	for i := 0; i < maxMips; i++ {
		writeU32(&out, sizes[i])
	}

	writeU32(&out, 0) // jpegHeaderSize = 0 (each mip body is a standalone JPEG)

	for _, b := range bodies {
		out.Write(b)
	}

	return out.Bytes(), nil
}

// Placeholder generates a solid neutral-gray BLP1 texture of the given
// power-of-two size, for STL / textureless imports so a TEXS entry always
// resolves to a valid texture. size is clamped to [1, 512] and rounded to the
// nearest power of two.
func Placeholder(size int) []byte {
	if size < 1 {
		size = 64
	}
	if size > 512 {
		size = 512
	}
	size = nearestPow2(size)

	neutral := color.RGBA{R: 0x80, G: 0x80, B: 0x80, A: 0xFF}
	img := image.NewRGBA(image.Rect(0, 0, size, size))
	for i := 0; i < len(img.Pix); i += 4 {
		img.Pix[i+0] = neutral.R
		img.Pix[i+1] = neutral.G
		img.Pix[i+2] = neutral.B
		img.Pix[i+3] = neutral.A
	}

	// Encode cannot fail for a valid solid power-of-two image; if it somehow
	// does, return nil so callers can fall back rather than panic.
	out, err := Encode(img)
	if err != nil {
		return nil
	}
	return out
}

func writeU32(buf *bytes.Buffer, v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	buf.Write(b[:])
}

// nearestPow2 returns the power of two closest to n (rounding to the nearer of
// the lower/upper power of two; ties round up). Always >= 1.
func nearestPow2(n int) int {
	if n <= 1 {
		return 1
	}
	lo := 1
	for lo*2 <= n {
		lo *= 2
	}
	hi := lo * 2
	if n-lo < hi-n {
		return lo
	}
	return hi
}

// buildMipChain produces mip0 plus every halved level down to 1x1.
func buildMipChain(mip0 *image.RGBA) []*image.RGBA {
	w := mip0.Rect.Dx()
	h := mip0.Rect.Dy()
	mips := []*image.RGBA{mip0}
	cur := mip0
	for w > 1 || h > 1 {
		nw := w / 2
		if nw < 1 {
			nw = 1
		}
		nh := h / 2
		if nh < 1 {
			nh = 1
		}
		cur = resize(cur, nw, nh)
		mips = append(mips, cur)
		w, h = nw, nh
	}
	return mips
}

// resize performs a simple box-sampled resize of src into a dw x dh RGBA image.
// Each destination pixel averages the block of source pixels it maps to, which
// handles both downscale (the common mip case) and upscale (pow2 enlargement).
func resize(src image.Image, dw, dh int) *image.RGBA {
	dst := image.NewRGBA(image.Rect(0, 0, dw, dh))
	b := src.Bounds()
	sw := b.Dx()
	sh := b.Dy()
	if sw == 0 || sh == 0 {
		return dst
	}

	for dy := 0; dy < dh; dy++ {
		// Source row range [sy0, sy1) covered by this destination row.
		sy0 := dy * sh / dh
		sy1 := (dy + 1) * sh / dh
		if sy1 <= sy0 {
			sy1 = sy0 + 1
		}
		for dx := 0; dx < dw; dx++ {
			sx0 := dx * sw / dw
			sx1 := (dx + 1) * sw / dw
			if sx1 <= sx0 {
				sx1 = sx0 + 1
			}

			var rsum, gsum, bsum, asum, count uint64
			for sy := sy0; sy < sy1; sy++ {
				for sx := sx0; sx < sx1; sx++ {
					r, g, bb, a := src.At(b.Min.X+sx, b.Min.Y+sy).RGBA()
					// RGBA() returns 16-bit; reduce to 8-bit.
					rsum += uint64(r >> 8)
					gsum += uint64(g >> 8)
					bsum += uint64(bb >> 8)
					asum += uint64(a >> 8)
					count++
				}
			}
			if count == 0 {
				count = 1
			}
			i := dst.PixOffset(dx, dy)
			dst.Pix[i+0] = uint8(rsum / count)
			dst.Pix[i+1] = uint8(gsum / count)
			dst.Pix[i+2] = uint8(bsum / count)
			dst.Pix[i+3] = uint8(asum / count)
		}
	}
	return dst
}

// bgrSwap returns a copy of src with the R and B channels swapped, producing the
// BGR ordering WC3 expects in JPEG-BLP. The decoder swaps them back on load.
func bgrSwap(src *image.RGBA) *image.RGBA {
	dst := image.NewRGBA(src.Rect)
	copy(dst.Pix, src.Pix)
	for i := 0; i+3 < len(dst.Pix); i += 4 {
		dst.Pix[i+0], dst.Pix[i+2] = dst.Pix[i+2], dst.Pix[i+0]
	}
	return dst
}
