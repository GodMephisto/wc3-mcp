---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3r/w3r_test.go"
ref: HEAD
---

package w3r

import (
	"bytes"
	"encoding/binary"
	"math"
	"testing"
)

// buildSyntheticW3R writes one region into a buffer per the wire format
// documented in the package doc comment, then asserts Parse round-trips
// every field.
func TestParse_Synthetic(t *testing.T) {
	var buf bytes.Buffer
	wu32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		buf.Write(b[:])
	}
	wf32 := func(v float32) { wu32(math.Float32bits(v)) }
	wcstr := func(s string) {
		buf.WriteString(s)
		buf.WriteByte(0)
	}

	wu32(5) // version
	wu32(2) // region count
	// Region 0
	wf32(100.5)
	wf32(-200)
	wf32(300)
	wf32(400)
	wcstr("Start Zone")
	wu32(7) // creation number
	buf.WriteString("RAhr")
	wcstr("amb1")
	buf.Write([]byte{255, 128, 64})
	buf.WriteByte(0) // padding
	// Region 1
	wf32(0)
	wf32(0)
	wf32(1)
	wf32(1)
	wcstr("Tiny")
	wu32(8)
	buf.WriteString("\x00\x00\x00\x00")
	wcstr("")
	buf.Write([]byte{0, 0, 0})
	buf.WriteByte(0)

	f, err := Parse(buf.Bytes())
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if f.Version != 5 {
		t.Errorf("version=%d", f.Version)
	}
	if len(f.Regions) != 2 {
		t.Fatalf("len(regions)=%d", len(f.Regions))
	}
	r0 := f.Regions[0]
	if r0.Left != 100.5 || r0.Bottom != -200 || r0.Right != 300 || r0.Top != 400 {
		t.Errorf("r0 bounds=%v", r0)
	}
	if r0.Name != "Start Zone" || r0.CreationNumber != 7 || r0.WeatherID != "RAhr" || r0.AmbientID != "amb1" {
		t.Errorf("r0 fields=%+v", r0)
	}
	if r0.Color != [3]uint8{255, 128, 64} {
		t.Errorf("r0 color=%v", r0.Color)
	}
	if f.Regions[1].Name != "Tiny" {
		t.Errorf("r1 name=%q", f.Regions[1].Name)
	}
}

func TestParse_TooSmall(t *testing.T) {
	if _, err := Parse([]byte{1, 2}); err == nil {
		t.Error("expected error for truncated header")
	}
}

func TestParse_OutrageousCount_TreatedAsZero(t *testing.T) {
	var buf bytes.Buffer
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], 5)
	buf.Write(b[:])
	binary.LittleEndian.PutUint32(b[:], 1_000_000_000) // CHSV "FUCK" trick
	buf.Write(b[:])
	f, err := Parse(buf.Bytes())
	if err != nil {
		t.Fatalf("unexpected err: %v", err)
	}
	if len(f.Regions) != 0 {
		t.Errorf("expected zero regions, got %d", len(f.Regions))
	}
}
