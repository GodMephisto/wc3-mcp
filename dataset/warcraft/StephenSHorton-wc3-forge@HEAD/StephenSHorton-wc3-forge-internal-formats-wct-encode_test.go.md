---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wct/encode_test.go"
ref: HEAD
---

package wct

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg/data"
)

// roundTripWCT reads war3map.wct + war3map.wtg from the map at mapPath and
// asserts Parse(.wct) → Encode produces byte-equal output. The encoder needs
// both files because the per-trigger blob order is keyed off the wtg's
// category-then-trigger walk.
func roundTripWCT(t *testing.T, mapPath string) {
	t.Helper()
	if _, err := os.Stat(mapPath); err != nil {
		t.Skipf("test map not available: %v", err)
	}
	arc, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("mpq.Open: %v", err)
	}
	defer arc.Close()
	if !arc.Has("war3map.wct") {
		t.Skip("map has no war3map.wct")
	}
	origWCT, err := arc.Read("war3map.wct")
	if err != nil {
		t.Fatalf("read war3map.wct: %v", err)
	}
	f, err := Parse(origWCT)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	// Need the wtg's category-then-trigger ordering AND the per-trigger
	// CustomText bindings. We parse both, then bind, then encode.
	if !arc.Has("war3map.wtg") {
		t.Skip("map has no war3map.wtg (can't determine blob order)")
	}
	origWTG, err := arc.Read("war3map.wtg")
	if err != nil {
		t.Fatalf("read war3map.wtg: %v", err)
	}
	td, err := wtg.ParseTriggerData(data.TriggerDataTXT)
	if err != nil {
		t.Fatalf("ParseTriggerData: %v", err)
	}
	trig, err := wtg.Parse(origWTG, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("wtg.Parse: %v", err)
	}
	// Bind custom texts onto triggers in the same order Parse would.
	bindForTest(trig, f)

	out, err := Encode(f, trig)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(origWCT, out) {
		div := firstDiff(origWCT, out)
		t.Fatalf("round-trip not byte-equal: orig=%d bytes, out=%d bytes, first diff @ byte %d", len(origWCT), len(out), div)
	}
}

// bindForTest replicates forge.bindCustomTexts inline so wct tests don't
// pull in the forge package.
func bindForTest(t *wtg.Triggers, w *File) {
	skipComments := !w.IsPre131
	i := 0
	for _, cat := range t.Categories {
		for ti := range t.Triggers {
			tr := &t.Triggers[ti]
			if tr.ParentID != cat.ID {
				continue
			}
			if skipComments && tr.IsComment {
				continue
			}
			if i >= len(w.CustomTexts) {
				return
			}
			tr.CustomText = w.CustomTexts[i]
			i++
		}
	}
}

// roundTripRawWCT reads a raw war3map.wct + its sibling war3map.wtg from disk
// (a folder-extracted map) and asserts Parse(.wct) → Encode is byte-equal. The
// encoder needs the wtg to recover the per-trigger blob order. Unlike
// roundTripWCT (which Skipf's on a missing out-of-repo .w3x), this reads
// committed testdata fixtures and fails loudly if they're absent.
func roundTripRawWCT(t *testing.T, wctPath, wtgPath string) {
	t.Helper()
	origWCT, err := os.ReadFile(wctPath)
	if err != nil {
		t.Fatalf("read %s: %v", wctPath, err)
	}
	f, err := Parse(origWCT)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	origWTG, err := os.ReadFile(wtgPath)
	if err != nil {
		t.Fatalf("read %s: %v", wtgPath, err)
	}
	td, err := wtg.ParseTriggerData(data.TriggerDataTXT)
	if err != nil {
		t.Fatalf("ParseTriggerData: %v", err)
	}
	trig, err := wtg.Parse(origWTG, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("wtg.Parse: %v", err)
	}
	bindForTest(trig, f)
	out, err := Encode(f, trig)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(origWCT, out) {
		div := firstDiff(origWCT, out)
		t.Fatalf("round-trip not byte-equal: orig=%d bytes, out=%d bytes, first diff @ byte %d", len(origWCT), len(out), div)
	}
}

// TestEncodeWCTRoundTripSurvival proves Parse(.wct) → Encode is byte-equal on a
// real, committed war3map.wct lifted from the user's GPL survival map (a
// Reforged Lua map). The fixtures are small (.wct 310 B, .wtg 678 B) and live
// in testdata/, so this RUNS on a clean checkout — the FFB/SecretValley tests
// above Skipf when their .w3x isn't on the machine.
func TestEncodeWCTRoundTripSurvival(t *testing.T) {
	roundTripRawWCT(t,
		filepath.Join("testdata", "wc3_survival_v1_6.wct"),
		filepath.Join("testdata", "wc3_survival_v1_6.wtg"))
}

func TestEncodeWCTRoundTripFFB(t *testing.T) {
	roundTripWCT(t, `C:\Users\4step\Documents\Warcraft III\Maps\Download\Enfo FFB - v2.64f.w3x`)
}

func TestEncodeWCTRoundTripSecretValley(t *testing.T) {
	roundTripWCT(t, `C:\Users\4step\Documents\Warcraft III\Maps\Download\Season8\(2)SecretValley_S2_v2.0.w3x`)
}

// TestEncodeWCTMinimal exercises a hand-built minimal wct + matched wtg so
// the test suite has at least one always-on round-trip case.
func TestEncodeWCTMinimal(t *testing.T) {
	// Build a wct File manually + a matching wtg with one trigger.
	f := &File{
		Version:           0x80000004,
		SubVersion:        1,
		GlobalJASSComment: "Global JASS header (any text)",
		GlobalJASS:        "function Foo takes nothing returns nothing\nendfunction\n",
		CustomTexts:       []string{"// my custom script\n"},
	}
	trig := &wtg.Triggers{
		Version:    0x80000004,
		SubVersion: 7,
		Categories: []wtg.Category{
			{Classifier: wtg.ClassifierCategory, ID: 1, Name: "Main"},
		},
		Triggers: []wtg.Trigger{
			{Classifier: wtg.ClassifierScript, ID: 2, ParentID: 1, Name: "MyTrigger", CustomText: "// my custom script\n"},
		},
	}
	out, err := Encode(f, trig)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	// Re-parse and assert the resulting File round-trips.
	parsed, err := Parse(out)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if parsed.GlobalJASSComment != f.GlobalJASSComment {
		t.Errorf("GlobalJASSComment = %q, want %q", parsed.GlobalJASSComment, f.GlobalJASSComment)
	}
	if parsed.GlobalJASS != f.GlobalJASS {
		t.Errorf("GlobalJASS = %q, want %q", parsed.GlobalJASS, f.GlobalJASS)
	}
	if len(parsed.CustomTexts) != 1 || parsed.CustomTexts[0] != "// my custom script\n" {
		t.Errorf("CustomTexts = %v, want [\"// my custom script\\n\"]", parsed.CustomTexts)
	}
}

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
	if len(a) != len(b) {
		return n
	}
	return -1
}
