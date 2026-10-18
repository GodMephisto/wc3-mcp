---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3e/water_regression_test.go"
ref: HEAD
---

package w3e

import (
	"os"
	"testing"
)

// TestParse_HasWater_Truthtable_Enfo verifies that Enfo's FFB v2.64f
// reports zero corners with HasWater() == true. This is a regression
// guard for a class of bugs where the renderer would emit thousands of
// water triangles on a map that has no water at all (e.g. a misread of
// the texture_and_flags water bit, or accidentally OR-ing in the boundary
// flag).
//
// Enfo's FFB has no water cells anywhere — confirmed by side-by-side
// inspection in HiveWE. If a future change to the bit-mask in Parse
// (legacy v11 path bit 6 = 0x40, or v12+ path bit 8 = 0x100) starts
// flipping the water flag on more corners than it should, this test will
// catch it before the renderer wastes a buildWater pass and the user
// sees phantom translucent surfaces around the islands.
//
// We skip when the fixture is not available so the test stays portable —
// the map is the user's personal extraction at the path below.
func TestParse_HasWater_Truthtable_Enfo(t *testing.T) {
	const enfoPath = `C:\Users\4step\maps-extracted\enfo-v2.64f\war3map.w3e`

	data, err := os.ReadFile(enfoPath)
	if err != nil {
		t.Skipf("Enfo fixture unavailable: %v", err)
	}

	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	// Sanity: this is the legacy v11 layout map we have on file. If the
	// fixture ever gets upgraded, this assertion documents which path
	// we're exercising; the bit-mask is different on v12+.
	if f.Version != 11 {
		t.Logf("Enfo fixture is version %d (was 11); bit-mask path differs", f.Version)
	}

	// Count corners with HasWater()=true.
	var trues int
	for i := range f.Tiles {
		if f.Tiles[i].HasWater() {
			trues++
		}
	}
	if trues != 0 {
		// Dump a few sample corners so future debuggers know where to look.
		shown := 0
		for i := range f.Tiles {
			if !f.Tiles[i].HasWater() {
				continue
			}
			tp := f.Tiles[i]
			t.Errorf("[%d] HasWater=true Flags=0x%02x GroundTex=%d WaterLevel=%d",
				i, tp.Flags, tp.GroundTexture, tp.WaterLevel)
			shown++
			if shown >= 4 {
				break
			}
		}
		t.Fatalf("Enfo's FFB should have 0 corners with HasWater()=true; got %d / %d",
			trues, len(f.Tiles))
	}

	// Also confirm the same predicate that the JS water mesh uses (any of
	// 4 corners has water → emit cell) yields zero cells. This is what
	// gates the wasted-render path in frontend/src/water.ts.
	W, H := int(f.Width), int(f.Height)
	var cells int
	for j := 0; j < H-1; j++ {
		for i := 0; i < W-1; i++ {
			bl := j*W + i
			br := bl + 1
			tl := bl + W
			tr := tl + 1
			if f.Tiles[bl].HasWater() || f.Tiles[br].HasWater() ||
				f.Tiles[tl].HasWater() || f.Tiles[tr].HasWater() {
				cells++
			}
		}
	}
	if cells != 0 {
		t.Fatalf("Enfo should emit 0 water cells (4-corner OR); got %d", cells)
	}
}

// TestParse_HasWater_BitMask_v11 documents the legacy v11 wire layout
// for the water flag: it lives at bit 6 (0x40) of the single
// texture_and_flags byte. Anything else in that byte (ramp 0x10, blight
// 0x20, boundary 0x80) must NOT count as water. This regression test
// constructs a one-tilepoint v11 file with each flag bit set in turn
// and asserts only the 0x40 case flips HasWater() true.
func TestParse_HasWater_BitMask_v11(t *testing.T) {
	build := func(flagsByte byte) []byte {
		b := []byte{}
		b = append(b, 'W', '3', 'E', '!')                  // magic
		b = append(b, 11, 0, 0, 0)                         // version 11
		b = append(b, 'L')                                 // tileset
		b = append(b, 0, 0, 0, 0)                          // custom_tileset
		b = append(b, 1, 0, 0, 0)                          // n_ground = 1
		b = append(b, 'L', 'g', 'r', 's')                  // FourCC
		b = append(b, 0, 0, 0, 0)                          // n_cliff = 0
		b = append(b, 1, 0, 0, 0)                          // width = 1
		b = append(b, 1, 0, 0, 0)                          // height = 1
		b = append(b, 0, 0, 0, 0, 0, 0, 0, 0)              // center_offset (0,0)
		// One tilepoint (7 bytes for v11):
		b = append(b, 0, 0x20)                              // ground_height raw 0x2000
		b = append(b, 0, 0x20)                              // water_and_flags raw 0x2000
		b = append(b, flagsByte)                            // texture_and_flags
		b = append(b, 0)                                    // variation
		b = append(b, 0)                                    // misc
		return b
	}

	cases := []struct {
		name      string
		flagsByte byte
		want      bool
	}{
		{"none", 0x00, false},
		{"ramp_bit_4", 0x10, false},
		{"blight_bit_5", 0x20, false},
		{"water_bit_6", 0x40, true},
		{"boundary_bit_7", 0x80, false},
		{"water_plus_boundary", 0x40 | 0x80, true},
		{"ramp_plus_boundary_no_water", 0x10 | 0x80, false},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			f, err := Parse(build(tc.flagsByte))
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			got := f.Tiles[0].HasWater()
			if got != tc.want {
				t.Fatalf("flags=0x%02x: HasWater=%v, want %v (Flags=0x%02x)",
					tc.flagsByte, got, tc.want, f.Tiles[0].Flags)
			}
		})
	}
}

// TestParse_HasWater_BitMask_v12 mirrors the v11 test for the Reforged
// (v12+) wire layout: texture_and_flags is a uint16 and the water bit
// is at bit 8 (0x0100). Other flag bits (ramp 0x40, blight 0x80,
// boundary 0x0200) and ground-texture bits (0..5) must NOT influence
// HasWater().
func TestParse_HasWater_BitMask_v12(t *testing.T) {
	build := func(tf uint16) []byte {
		b := []byte{}
		b = append(b, 'W', '3', 'E', '!')                  // magic
		b = append(b, 12, 0, 0, 0)                         // version 12
		b = append(b, 'X')                                 // tileset (Reforged)
		b = append(b, 0, 0, 0, 0)                          // custom_tileset
		b = append(b, 1, 0, 0, 0)                          // n_ground = 1
		b = append(b, 'L', 'g', 'r', 's')                  // FourCC
		b = append(b, 0, 0, 0, 0)                          // n_cliff = 0
		b = append(b, 1, 0, 0, 0)                          // width = 1
		b = append(b, 1, 0, 0, 0)                          // height = 1
		b = append(b, 0, 0, 0, 0, 0, 0, 0, 0)              // center_offset (0,0)
		// One tilepoint (8 bytes for v12+):
		b = append(b, 0, 0x20)                              // ground_height raw 0x2000
		b = append(b, 0, 0x20)                              // water_and_flags raw 0x2000
		b = append(b, byte(tf&0xFF), byte(tf>>8))           // texture_and_flags LE
		b = append(b, 0)                                    // variation
		b = append(b, 0)                                    // misc
		return b
	}

	cases := []struct {
		name string
		tf   uint16
		want bool
	}{
		{"none", 0x0000, false},
		{"ground_tex_only", 0x003F, false},            // all 6 ground-tex bits set
		{"ramp_bit_6", 0x0040, false},
		{"blight_bit_7", 0x0080, false},
		{"water_bit_8", 0x0100, true},
		{"boundary_bit_9", 0x0200, false},
		{"water_plus_boundary", 0x0100 | 0x0200, true},
		{"ramp_plus_blight_plus_boundary_no_water", 0x0040 | 0x0080 | 0x0200, false},
		{"all_bits_with_water", 0x03FF, true},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			f, err := Parse(build(tc.tf))
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			got := f.Tiles[0].HasWater()
			if got != tc.want {
				t.Fatalf("tf=0x%04x: HasWater=%v, want %v (Flags=0x%02x)",
					tc.tf, got, tc.want, f.Tiles[0].Flags)
			}
		})
	}
}
