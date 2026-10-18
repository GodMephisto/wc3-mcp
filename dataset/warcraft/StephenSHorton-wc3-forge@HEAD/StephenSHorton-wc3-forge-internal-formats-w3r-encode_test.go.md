---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3r/encode_test.go"
ref: HEAD
---

package w3r

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"
)

// Real-world fixture: wc3-survival's war3map.w3r (48 bytes) — version 5, one
// region "Region 000" with empty weather + ambient and a NON-ZERO trailing pad
// byte (0xFF). The pad byte is the reason Encode must round-trip the unexported
// Region.pad rather than assume zero.
var fixtureSurvival = filepath.Join("testdata", "wc3_survival_v1_6.w3r")

// TestRoundTrip_RealFixture asserts Encode is the byte-faithful inverse of
// Parse for the committed real-world fixture. This is the contract that lets
// the Region Editor write the file back without corrupting unmodified regions.
func TestRoundTrip_RealFixture(t *testing.T) {
	orig, err := os.ReadFile(fixtureSurvival)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}
	f, err := Parse(orig)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		t.Fatalf("Encode output != original (sizes orig=%d encoded=%d, first diff at offset %d)",
			len(orig), len(out), firstDiff(orig, out))
	}
	// Sanity on what the fixture actually contains, so the assertion above
	// isn't vacuously trivial.
	if f.Version != 5 || len(f.Regions) != 1 {
		t.Fatalf("fixture shape changed: version=%d regions=%d", f.Version, len(f.Regions))
	}
	if f.Regions[0].pad != 0xFF {
		t.Errorf("fixture pad byte = 0x%02x, expected 0xFF (round-trip would be vacuous otherwise)", f.Regions[0].pad)
	}
}

// TestRoundTrip_Synthetic covers paths the single real fixture doesn't:
// multiple regions, a non-empty weather FourCC, a non-empty ambient C-string,
// and a zero-pad region. It builds bytes per the wire format, then asserts both
// Parse∘Encode (bytes) and Encode∘Parse (struct) round-trip.
func TestRoundTrip_Synthetic(t *testing.T) {
	var buf bytes.Buffer
	w := &writer{}
	w.writeU32(5) // version
	w.writeU32(3) // region count
	// Region 0: full fields, pad 0x07.
	w.writeF32(100.5)
	w.writeF32(-200)
	w.writeF32(300)
	w.writeF32(400)
	w.writeCString("Start Zone")
	w.writeU32(7) // creation number
	w.writeBytes([]byte("RAhr"))
	w.writeCString("amb1")
	w.writeBytes([]byte{255, 128, 64})
	w.writeU8(7) // non-zero pad
	// Region 1: empty name/ambient, all-zero weather, pad 0.
	w.writeF32(0)
	w.writeF32(0)
	w.writeF32(1)
	w.writeF32(1)
	w.writeCString("")
	w.writeU32(8)
	w.writeBytes([]byte{0, 0, 0, 0})
	w.writeCString("")
	w.writeBytes([]byte{0, 0, 0})
	w.writeU8(0)
	// Region 2: negative creation number (int32) + max color, pad 0xFF.
	w.writeF32(-1)
	w.writeF32(-2)
	w.writeF32(-3)
	w.writeF32(-4)
	w.writeCString("Region with spaces")
	w.writeU32(0xFFFFFFFF) // -1 as int32
	w.writeBytes([]byte("LRaa"))
	w.writeCString("LRaa")
	w.writeBytes([]byte{255, 255, 255})
	w.writeU8(0xFF)
	buf.Write(w.bytes())

	orig := buf.Bytes()
	f, err := Parse(orig)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		t.Fatalf("Parse∘Encode != original (orig=%d encoded=%d, first diff at %d)",
			len(orig), len(out), firstDiff(orig, out))
	}

	// Encode∘Parse: re-parse the encoded bytes and confirm structural identity.
	f2, err := Parse(out)
	if err != nil {
		t.Fatalf("re-Parse: %v", err)
	}
	out2, err := Encode(f2)
	if err != nil {
		t.Fatalf("re-Encode: %v", err)
	}
	if !bytes.Equal(out, out2) {
		t.Fatalf("Encode∘Parse not stable (first diff at %d)", firstDiff(out, out2))
	}

	// Field-level spot checks so a future regression fails loudly.
	if f.Regions[2].CreationNumber != -1 {
		t.Errorf("region 2 creation number = %d, want -1", f.Regions[2].CreationNumber)
	}
	if f.Regions[0].WeatherID != "RAhr" || f.Regions[0].AmbientID != "amb1" {
		t.Errorf("region 0 weather/ambient = %q/%q", f.Regions[0].WeatherID, f.Regions[0].AmbientID)
	}
}

// TestRoundTrip_HandConstructed verifies Encode∘Parse stability for a File
// built entirely in code (no Parse to populate the pad byte). Such a File omits
// WeatherID; Encode must normalize it to 4 NUL bytes so the result re-parses.
func TestRoundTrip_HandConstructed(t *testing.T) {
	f := &File{
		Version: 5,
		Regions: []Region{
			{
				Left: 0, Bottom: 0, Right: 256, Top: 256,
				Name:           "MyRegion",
				CreationNumber: 0,
				WeatherID:      "", // unset → 4 NULs
				AmbientID:      "",
				Color:          [3]uint8{255, 255, 255},
			},
		},
	}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	f2, err := Parse(out)
	if err != nil {
		t.Fatalf("Parse of hand-constructed encode: %v", err)
	}
	if len(f2.Regions) != 1 {
		t.Fatalf("expected 1 region, got %d", len(f2.Regions))
	}
	r := f2.Regions[0]
	if r.Name != "MyRegion" || r.Color != ([3]uint8{255, 255, 255}) {
		t.Errorf("round-tripped region mismatch: %+v", r)
	}
	if r.WeatherID != "\x00\x00\x00\x00" {
		t.Errorf("WeatherID = %q, want 4 NUL bytes", r.WeatherID)
	}
	// Encode∘Parse must now be stable.
	out2, err := Encode(f2)
	if err != nil {
		t.Fatalf("re-Encode: %v", err)
	}
	if !bytes.Equal(out, out2) {
		t.Fatalf("Encode∘Parse not stable for hand-constructed file (first diff at %d)", firstDiff(out, out2))
	}
}

// TestRoundTrip_ZeroRegions covers the header-only (count=0) case — common for
// maps whose regions are all created at runtime via CreateRegion in JASS.
func TestRoundTrip_ZeroRegions(t *testing.T) {
	f := &File{Version: 5}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if len(out) != 8 {
		t.Fatalf("zero-region file = %d bytes, want 8 (version + count)", len(out))
	}
	f2, err := Parse(out)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if f2.Version != 5 || len(f2.Regions) != 0 {
		t.Errorf("round-trip mismatch: version=%d regions=%d", f2.Version, len(f2.Regions))
	}
}

func TestEncode_NilFile(t *testing.T) {
	if _, err := Encode(nil); err == nil {
		t.Error("expected error encoding nil file")
	}
}

func TestEncode_BadWeatherID(t *testing.T) {
	f := &File{Version: 5, Regions: []Region{{WeatherID: "AB"}}}
	if _, err := Encode(f); err == nil {
		t.Error("expected error for non-4-byte WeatherID")
	}
}

func TestEncode_EmbeddedNUL(t *testing.T) {
	f := &File{Version: 5, Regions: []Region{{Name: "bad\x00name"}}}
	if _, err := Encode(f); err == nil {
		t.Error("expected error for embedded NUL in name")
	}
}

// firstDiff returns the index of the first differing byte (or the shorter
// length when one is a prefix of the other). Mirrors the unitsdoo test helper.
func firstDiff(a, b []byte) int {
	n := len(a)
	if len(b) < n {
		n = len(b)
	}
	for i := 0; i < n; i++ {
		if a[i] != b[i] {
			return i
		}
	}
	return n
}
