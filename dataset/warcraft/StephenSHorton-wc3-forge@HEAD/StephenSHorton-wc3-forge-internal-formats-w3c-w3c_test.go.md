---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3c/w3c_test.go"
ref: HEAD
---

package w3c

import (
	"bytes"
	"encoding/binary"
	"math"
	"testing"
)

func TestParse_Synthetic131(t *testing.T) {
	buf := build131Camera("OpeningCam", 1.5)
	f, err := Parse(buf)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(f.Cameras) != 1 {
		t.Fatalf("want 1 camera, got %d", len(f.Cameras))
	}
	c := f.Cameras[0]
	if c.Name != "OpeningCam" {
		t.Errorf("name=%q", c.Name)
	}
	if c.LocalRoll != 1.5 {
		t.Errorf("local_roll=%v", c.LocalRoll)
	}
	if !c.Has131Locals {
		t.Errorf("expected has_131_locals=true")
	}
}

func TestParse_SyntheticPre131(t *testing.T) {
	buf := buildPre131Camera("LegacyCam")
	f, err := Parse(buf)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(f.Cameras) != 1 {
		t.Fatalf("want 1 camera, got %d", len(f.Cameras))
	}
	if f.Cameras[0].Name != "LegacyCam" {
		t.Errorf("name=%q", f.Cameras[0].Name)
	}
}

func build131Camera(name string, localRoll float32) []byte {
	var buf bytes.Buffer
	wu32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		buf.Write(b[:])
	}
	wf32 := func(v float32) { wu32(math.Float32bits(v)) }
	wu32(0) // version
	wu32(1) // count
	for i := 0; i < 10; i++ {
		wf32(float32(i + 1))
	}
	// local pitch/yaw/roll
	wf32(0.1)
	wf32(0.2)
	wf32(localRoll)
	buf.WriteString(name)
	buf.WriteByte(0)
	return buf.Bytes()
}

func buildPre131Camera(name string) []byte {
	var buf bytes.Buffer
	wu32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		buf.Write(b[:])
	}
	wf32 := func(v float32) { wu32(math.Float32bits(v)) }
	wu32(0)
	wu32(1)
	for i := 0; i < 10; i++ {
		wf32(float32(i + 1))
	}
	buf.WriteString(name)
	buf.WriteByte(0)
	return buf.Bytes()
}
