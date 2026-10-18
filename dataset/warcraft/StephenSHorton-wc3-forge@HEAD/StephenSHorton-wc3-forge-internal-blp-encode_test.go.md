---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/blp/encode_test.go"
ref: HEAD
---

package blp

import (
	"bytes"
	"image"
	"image/color"
	"testing"

	gowblp "github.com/nielsAD/gowarcraft3/file/blp"
)

// gradient builds an n x n RGBA test image with a smooth color gradient so the
// BGR-swap correctness can be checked on a known top-left pixel.
func gradient(n int) *image.RGBA {
	img := image.NewRGBA(image.Rect(0, 0, n, n))
	for y := 0; y < n; y++ {
		for x := 0; x < n; x++ {
			img.Set(x, y, color.RGBA{
				R: uint8(x * 255 / max(n-1, 1)),
				G: uint8(y * 255 / max(n-1, 1)),
				B: 0x40,
				A: 0xFF,
			})
		}
	}
	return img
}

func approxEq(a, b uint8, tol int) bool {
	d := int(a) - int(b)
	if d < 0 {
		d = -d
	}
	return d <= tol
}

func TestEncodeMagic(t *testing.T) {
	data, err := Encode(gradient(8))
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if len(data) < 4 || string(data[:4]) != "BLP1" {
		t.Fatalf("magic = %q, want BLP1", data[:min(len(data), 4)])
	}
}

func TestEncodeRoundTripDecode(t *testing.T) {
	const n = 8
	src := gradient(n)
	data, err := Encode(src)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}

	// Ground-truth gate: the gowarcraft3 decoder must accept our bytes.
	decoded, err := gowblp.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("gowarcraft3 blp.Decode failed: %v", err)
	}

	if got := decoded.Bounds().Dx(); got != n {
		t.Errorf("decoded width = %d, want %d", got, n)
	}
	if got := decoded.Bounds().Dy(); got != n {
		t.Errorf("decoded height = %d, want %d", got, n)
	}

	// Top-left pixel of the gradient is (R=0, G=0, B=0x40). The decoder swaps
	// BGR back to RGB, so if our encoder swapped correctly the decoded pixel
	// matches the source within JPEG-lossy tolerance. A wrong/absent swap would
	// surface here as R and B being transposed (R~0x40, B~0).
	r, g, b, _ := decoded.At(0, 0).RGBA()
	r8, g8, b8 := uint8(r>>8), uint8(g>>8), uint8(b>>8)
	const tol = 24
	if !approxEq(r8, 0x00, tol) || !approxEq(g8, 0x00, tol) || !approxEq(b8, 0x40, tol) {
		t.Errorf("top-left pixel = (%#02x,%#02x,%#02x), want approx (0x00,0x00,0x40) — BGR swap likely wrong", r8, g8, b8)
	}
}

func TestEncodeFullMipChain(t *testing.T) {
	const n = 8 // log2(8)+1 = 4 mips: 8,4,2,1
	data, err := Encode(gradient(n))
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}

	offsets, sizes := readMipTables(t, data)

	wantMips := 4
	got := 0
	for i := range offsets {
		if offsets[i] != 0 && sizes[i] != 0 {
			got++
		}
	}
	if got != wantMips {
		t.Fatalf("mip count = %d, want %d (log2(%d)+1)", got, wantMips, n)
	}

	// Each present mip body must be a self-contained JPEG (starts with SOI
	// marker 0xFFD8), since jpegHeaderSize is 0.
	for i := 0; i < wantMips; i++ {
		off := offsets[i]
		sz := sizes[i]
		if int(off+sz) > len(data) {
			t.Fatalf("mip %d body [%d:%d] exceeds file size %d", i, off, off+sz, len(data))
		}
		body := data[off : off+sz]
		if len(body) < 2 || body[0] != 0xFF || body[1] != 0xD8 {
			t.Errorf("mip %d does not start with JPEG SOI marker", i)
		}
	}
}

func TestEncodeNonPow2Resized(t *testing.T) {
	// 6x6 should round to nearest pow2 (8x8: 6-4=2, 8-6=2 tie -> rounds up).
	data, err := Encode(gradient(6))
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	decoded, err := gowblp.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("Decode: %v", err)
	}
	w, h := decoded.Bounds().Dx(), decoded.Bounds().Dy()
	if w&(w-1) != 0 || h&(h-1) != 0 {
		t.Errorf("decoded dims %dx%d are not power-of-two", w, h)
	}
}

func TestPlaceholderSolid(t *testing.T) {
	data := Placeholder(64)
	if data == nil {
		t.Fatal("Placeholder returned nil")
	}
	if string(data[:4]) != "BLP1" {
		t.Fatalf("magic = %q, want BLP1", data[:4])
	}

	decoded, err := gowblp.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("gowarcraft3 Decode of placeholder failed: %v", err)
	}
	if decoded.Bounds().Dx() != 64 || decoded.Bounds().Dy() != 64 {
		t.Errorf("placeholder dims = %dx%d, want 64x64", decoded.Bounds().Dx(), decoded.Bounds().Dy())
	}

	// Every pixel should be ~neutral gray (0x80) within JPEG tolerance.
	const tol = 12
	b := decoded.Bounds()
	for _, p := range []image.Point{
		{X: b.Min.X, Y: b.Min.Y},
		{X: b.Max.X - 1, Y: b.Min.Y},
		{X: b.Min.X, Y: b.Max.Y - 1},
		{X: (b.Min.X + b.Max.X) / 2, Y: (b.Min.Y + b.Max.Y) / 2},
	} {
		r, g, bb, _ := decoded.At(p.X, p.Y).RGBA()
		r8, g8, b8 := uint8(r>>8), uint8(g>>8), uint8(bb>>8)
		if !approxEq(r8, 0x80, tol) || !approxEq(g8, 0x80, tol) || !approxEq(b8, 0x80, tol) {
			t.Errorf("placeholder pixel at %v = (%#02x,%#02x,%#02x), want approx neutral gray 0x80", p, r8, g8, b8)
		}
	}
}

// readMipTables extracts the 16-entry offset and size tables from a BLP1 header.
func readMipTables(t *testing.T, data []byte) (offsets, sizes [16]uint32) {
	t.Helper()
	if len(data) < headerFields {
		t.Fatalf("data too short: %d bytes", len(data))
	}
	// Tables begin after magic(4) + 6 uint32 fields = offset 28.
	base := 4 + 6*4
	for i := 0; i < 16; i++ {
		offsets[i] = le32(data, base+i*4)
	}
	base += 16 * 4
	for i := 0; i < 16; i++ {
		sizes[i] = le32(data, base+i*4)
	}
	return offsets, sizes
}

func le32(b []byte, off int) uint32 {
	return uint32(b[off]) | uint32(b[off+1])<<8 | uint32(b[off+2])<<16 | uint32(b[off+3])<<24
}
