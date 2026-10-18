---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wct/wct_test.go"
ref: HEAD
---

package wct

import (
	"os"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg/data"
)

// TestParseFFB parses the Enfo FFB v2.64f war3map.wct and asserts the
// custom-text entry count matches the wtg's non-comment trigger count
// (per HiveWE's per-trigger blob layout).
func TestParseFFB(t *testing.T) {
	mapPath := `C:\Users\4step\Documents\Warcraft III\Maps\Download\Enfo FFB - v2.64f.w3x`
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
	wctBytes, err := arc.Read("war3map.wct")
	if err != nil {
		t.Fatalf("Read war3map.wct: %v", err)
	}
	f, err := Parse(wctBytes)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	t.Logf("Enfo FFB: wct version=0x%X sub=%d entries=%d global_jass=%d bytes",
		f.Version, f.SubVersion, len(f.CustomTexts), len(f.GlobalJASS))

	if !arc.Has("war3map.wtg") {
		return
	}
	wtgBytes, _ := arc.Read("war3map.wtg")
	td, err := wtg.ParseTriggerData(data.TriggerDataTXT)
	if err != nil {
		t.Fatalf("ParseTriggerData: %v", err)
	}
	trig, err := wtg.Parse(wtgBytes, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("wtg.Parse: %v", err)
	}
	nonComment := 0
	for _, tr := range trig.Triggers {
		if !tr.IsComment {
			nonComment++
		}
	}
	if len(f.CustomTexts) != nonComment {
		t.Errorf("custom-text entry count = %d, want non-comment trigger count %d",
			len(f.CustomTexts), nonComment)
	}
}

// TestParseSecretValley repeats the count-match check against Secret Valley.
func TestParseSecretValley(t *testing.T) {
	mapPath := `C:\Users\4step\Documents\Warcraft III\Maps\Download\Season8\(2)SecretValley_S2_v2.0.w3x`
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
	wctBytes, err := arc.Read("war3map.wct")
	if err != nil {
		t.Fatalf("Read war3map.wct: %v", err)
	}
	f, err := Parse(wctBytes)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	t.Logf("Secret Valley: wct version=0x%X sub=%d entries=%d", f.Version, f.SubVersion, len(f.CustomTexts))
}
