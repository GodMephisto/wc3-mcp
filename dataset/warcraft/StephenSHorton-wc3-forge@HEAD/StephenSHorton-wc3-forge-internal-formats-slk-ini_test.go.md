---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/slk/ini_test.go"
ref: HEAD
---

package slk

import "testing"

const sampleINI = "\xEF\xBB\xBF" + `[Hpal]
skinnableID=Hpal
file=Units\Human\Paladin\Paladin
defScale=1.000
modelScale=1.0
defScale:hd=0.85
// a comment we should skip
; another comment
red=255
green=224
blue=128

[LObr]
skinType=doodad
file=Doodads\Cinematic\Brazier\Brazier
numVar=1
`

func TestLoadINI(t *testing.T) {
	m := New()
	if err := m.LoadINI([]byte(sampleINI)); err != nil {
		t.Fatalf("LoadINI: %v", err)
	}
	hpal := m.Row("Hpal")
	if hpal == nil {
		t.Fatalf("missing Hpal")
	}
	if got := hpal.String("file"); got != `Units\Human\Paladin\Paladin` {
		t.Errorf("Hpal.file = %q", got)
	}
	if got := hpal.Number("modelScale"); got != 1.0 {
		t.Errorf("Hpal.modelScale = %v", got)
	}
	if got := hpal.String("defScale:hd"); got != "0.85" {
		t.Errorf("Hpal.defScale:hd = %q", got)
	}
	if got := hpal.Number("red"); got != 255 {
		t.Errorf("Hpal.red = %v", got)
	}
	lobr := m.Row("LObr")
	if lobr == nil {
		t.Fatalf("missing LObr")
	}
	if got := lobr.String("file"); got != `Doodads\Cinematic\Brazier\Brazier` {
		t.Errorf("LObr.file = %q", got)
	}
}

// Verify INI merge sits cleanly on top of an existing SLK row — same key,
// new columns added, no clobbering.
func TestMergeSLKAndINI(t *testing.T) {
	m := New()
	if err := m.Load([]byte(sampleSLK)); err != nil {
		t.Fatalf("Load: %v", err)
	}
	// sampleSLK has a Hpal row with file=Paladin\Paladin (col index 2).
	const extra = "[Hpal]\nmodelScale=1.5\nmoveHeight=10\n"
	if err := m.LoadINI([]byte(extra)); err != nil {
		t.Fatalf("LoadINI: %v", err)
	}
	r := m.Row("Hpal")
	if r == nil {
		t.Fatalf("merged Hpal missing")
	}
	if got := r.String("file"); got != `Units\Human\Paladin\Paladin` {
		t.Errorf("file lost after INI merge: %q", got)
	}
	if got := r.Number("modelScale"); got != 1.5 {
		t.Errorf("modelScale = %v", got)
	}
	if got := r.Number("moveHeight"); got != 10 {
		t.Errorf("moveHeight = %v", got)
	}
}
