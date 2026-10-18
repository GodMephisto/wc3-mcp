---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/dds/decode_test.go"
ref: HEAD
---

package dds

import "testing"

// TestDecode_BC1_Synthetic builds a minimal one-block BC1 DDS in memory and
// verifies the decoded 4×4 pixel block has the expected colors. Catches
// header-offset / block-size regressions without needing a fixture file.
func TestDecode_BC1_Synthetic(t *testing.T) {
	// 128-byte header + 8-byte block. The header just needs the magic,
	// width, height, and a DXT1 fourCC at the pixel-format offset.
	data := make([]byte, headerSize+8)
	copy(data[0:4], "DDS ")
	// height @ 12, width @ 16 — both 4 (single block).
	data[12], data[13], data[14], data[15] = 4, 0, 0, 0
	data[16], data[17], data[18], data[19] = 4, 0, 0, 0
	copy(data[pixelFormatFourCCOffset:pixelFormatFourCCOffset+4], "DXT1")

	// Block: c0 = white (RGB565 = 0xFFFF), c1 = black (0x0000),
	// indices = all 0 → every pixel decodes to c0 (white).
	block := data[headerSize:]
	block[0], block[1] = 0xFF, 0xFF
	block[2], block[3] = 0x00, 0x00
	// 16 pixels × 2 bits = 32 bits, all zero already.

	img, err := Decode(data)
	if err != nil {
		t.Fatalf("Decode: %v", err)
	}
	b := img.Bounds()
	if b.Dx() != 4 || b.Dy() != 4 {
		t.Fatalf("bounds = %v, want 4×4", b)
	}
	for y := 0; y < 4; y++ {
		for x := 0; x < 4; x++ {
			r, g, bl, a := img.At(x, y).RGBA()
			// All white (0xFFFF in 16-bit RGBA = 255 in 8-bit).
			if r>>8 != 0xFF || g>>8 != 0xFF || bl>>8 != 0xFF || a>>8 != 0xFF {
				t.Errorf("pixel (%d,%d) = (%d,%d,%d,%d), want (255,255,255,255)",
					x, y, r>>8, g>>8, bl>>8, a>>8)
			}
		}
	}
}
