---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/encode_test.go"
ref: HEAD
---

package wtg

import (
	"bytes"
	"encoding/binary"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

// roundTripMap reads war3map.wtg from the map at mapPath and asserts
// Parse → Encode produces byte-equal output. Skips when the fixture isn't
// available so CI doesn't fail on machines without the test maps.
func roundTripMap(t *testing.T, mapPath string) {
	t.Helper()
	if _, err := os.Stat(mapPath); err != nil {
		t.Skipf("test map not available: %v", err)
	}
	td := loadTriggerData(t)
	arc, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("mpq.Open: %v", err)
	}
	defer arc.Close()
	if !arc.Has("war3map.wtg") {
		t.Skip("map has no war3map.wtg")
	}
	orig, err := arc.Read("war3map.wtg")
	if err != nil {
		t.Fatalf("read war3map.wtg: %v", err)
	}
	parsed, err := Parse(orig, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	out, err := Encode(parsed, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		// On mismatch, find the first divergent byte to make the failure
		// actionable. Dump nearby bytes for context.
		div := firstDiff(orig, out)
		t.Fatalf("round-trip not byte-equal: orig=%d bytes, out=%d bytes, first diff @ byte %d\n  orig[%d..%d]=%x\n  out [%d..%d]=%x",
			len(orig), len(out), div,
			max0(div-8), min0(div+16, len(orig)),
			orig[max0(div-8):min0(div+16, len(orig))],
			max0(div-8), min0(div+16, len(out)),
			out[max0(div-8):min0(div+16, len(out))],
		)
	}
}

func TestEncodeRoundTripFFB(t *testing.T) {
	roundTripMap(t, `C:\Users\4step\Documents\Warcraft III\Maps\Download\Enfo FFB - v2.64f.w3x`)
}

func TestEncodeRoundTripSecretValley(t *testing.T) {
	roundTripMap(t, `C:\Users\4step\Documents\Warcraft III\Maps\Download\Season8\(2)SecretValley_S2_v2.0.w3x`)
}

// TestEncodeRoundTripSurvival exercises a real, committed war3map.wtg lifted
// from the user's GPL survival map. This proves Parse → Encode is byte-equal
// on a real-world Reforged Lua map (the FFB / SecretValley tests above cover
// JASS maps but Skipf when their .w3x isn't on the machine). The fixture is
// small (678 bytes) and lives in testdata/, so this RUNS on a clean checkout.
func TestEncodeRoundTripSurvival(t *testing.T) {
	roundTripRawWTG(t, filepath.Join("testdata", "wc3_survival_v1_6.wtg"))
}

// TestEncodeMinimalRoundTrip exercises Encode against a hand-built minimal
// wtg so the test suite has at least one always-on round-trip case (the
// real-map tests skip on machines without the .w3x fixtures).
func TestEncodeMinimalRoundTrip(t *testing.T) {
	var orig []byte
	put32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		orig = append(orig, b[:]...)
	}
	cstr := func(s string) {
		orig = append(orig, []byte(s)...)
		orig = append(orig, 0)
	}
	orig = append(orig, []byte("WTG!")...)
	put32(0x80000004)        // version
	put32(7)                 // sub_version
	for i := 0; i < 7; i++ { // 7 deleted-id blocks (all zero)
		put32(0)
		put32(0)
	}
	put32(0) // unknown1
	put32(0) // unknown2
	put32(2) // trig_def_ver
	put32(1) // variable_count
	// One variable: name=udg_x, type=integer, unknown=0, is_array=0,
	// array_size=0 (sub_version 7), is_initialized=1, initial_value="0",
	// id=100, parent_id=200.
	cstr("udg_x")
	cstr("integer")
	put32(0)
	put32(0)
	put32(0)
	put32(1)
	cstr("0")
	put32(100)
	put32(200)
	put32(2) // element_count: 1 category + 1 GUI trigger
	put32(uint32(ClassifierCategory))
	put32(200) // id
	cstr("MyCategory")
	put32(0) // is_comment (sub_version 7)
	put32(1) // open_state
	put32(0) // parent_id
	put32(uint32(ClassifierGUI))
	cstr("MyTrigger")
	cstr("")
	put32(0)   // is_comment
	put32(42)  // id
	put32(1)   // is_enabled
	put32(0)   // is_script
	put32(1)   // initially_on inverted on disk (0 means initially_on=true after Parse)
	put32(0)   // run_on_initialization
	put32(200) // parent_id
	put32(0)   // eca_count

	td := loadTriggerData(t)
	parsed, err := Parse(orig, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	// Sanity: the parsed initially_on should be FALSE because we wrote 1
	// on disk (inverted).
	if parsed.Triggers[0].InitiallyOn {
		t.Errorf("expected InitiallyOn=false (on-disk 1 → inverted), got true")
	}
	out, err := Encode(parsed, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		div := firstDiff(orig, out)
		t.Fatalf("minimal round-trip not byte-equal at byte %d\n  orig=%x\n  out =%x",
			div, orig[max0(div-4):min0(div+8, len(orig))], out[max0(div-4):min0(div+8, len(out))])
	}
}

// firstDiff returns the index of the first byte that differs between a and b,
// or min(len(a), len(b)) if one is a prefix of the other.
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

func max0(x int) int {
	if x < 0 {
		return 0
	}
	return x
}

func min0(x, cap int) int {
	if x > cap {
		return cap
	}
	return x
}

func roundTripRawWTG(t *testing.T, path string) {
	t.Helper()
	td := loadTriggerData(t)
	orig, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read %s: %v", path, err)
	}
	parsed, err := Parse(orig, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	out, err := Encode(parsed, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		div := firstDiff(orig, out)
		t.Fatalf("round-trip not byte-equal: orig=%d bytes, out=%d bytes, first diff @ byte %d", len(orig), len(out), div)
	}
}
