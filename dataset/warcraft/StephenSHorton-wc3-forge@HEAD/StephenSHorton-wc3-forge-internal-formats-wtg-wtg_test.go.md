---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/wtg_test.go"
ref: HEAD
---

package wtg

import (
	"encoding/binary"
	"errors"
	"os"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg/data"
)

// loadTriggerData loads the embedded snapshot. Helper for the integration
// tests below that drive Parse() against real-world maps.
func loadTriggerData(t *testing.T) *TriggerData {
	t.Helper()
	td, err := ParseTriggerData(data.TriggerDataTXT)
	if err != nil {
		t.Fatalf("ParseTriggerData(embedded): %v", err)
	}
	if err := td.LoadTriggerStrings(data.TriggerStringsTXT); err != nil {
		t.Fatalf("LoadTriggerStrings(embedded): %v", err)
	}
	return td
}

// TestParseTriggerDataArgc spot-checks a few well-known function argcs against
// HiveWE's algorithm. Catches the easy regression where the section parser
// drifts off the comment-stripper or the TriggerCalls -1 fudge gets lost.
func TestParseTriggerDataArgc(t *testing.T) {
	td := loadTriggerData(t)
	if got := len(td.ArgumentCounts); got < 1000 {
		t.Errorf("ArgumentCounts has %d entries; expected several thousand", got)
	}
	cases := []struct {
		name string
		// argc HiveWE would compute. Determined by inspecting TriggerData.txt:
		// look up the row, count non-empty / non-numeric / non-"nothing"
		// tokens, subtract 1 if section is TriggerCalls.
		want int
	}{
		// [TriggerActions] CreateNUnitsAtLoc=0,integer,unitcode,player,location,real
		// → tokens: 0 (numeric), integer, unitcode, player, location, real → 5
		{name: "CreateNUnitsAtLoc", want: 5},
		// [TriggerActions] SetUnitLifeBJ=0,unit,real → tokens: 0, unit, real → 2
		{name: "SetUnitLifeBJ", want: 2},
		// [TriggerActions] DoNothing=0,nothing → tokens: 0 (numeric), nothing (skip) → 0
		{name: "DoNothing", want: 0},
		// [TriggerActions] CommentString=0,scriptcode → tokens: 0 (numeric), scriptcode → 1
		{name: "CommentString", want: 1},
		// [TriggerCalls] ParseTags=1,1,string,string → 1 (numeric), 1 (numeric),
		// string (return), string (arg) = 2 non-skip, minus 1 (return) = 1
		{name: "ParseTags", want: 1},
	}
	for _, c := range cases {
		got, ok := td.Argc(c.name)
		if !ok {
			t.Errorf("Argc(%q): not present", c.name)
			continue
		}
		if got != c.want {
			t.Errorf("Argc(%q) = %d, want %d", c.name, got, c.want)
		}
	}
}

// TestParseFFB drives the embedded TriggerData snapshot against Enfo's FFB
// v2.64f war3map.wtg. Skipped if the local map isn't available.
func TestParseFFB(t *testing.T) {
	mapPath := `C:\Users\4step\Documents\Warcraft III\Maps\Download\Enfo FFB - v2.64f.w3x`
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
		t.Skip("map has no war3map.wtg (hand-rolled-script map)")
	}
	wtgBytes, err := arc.Read("war3map.wtg")
	if err != nil {
		t.Fatalf("Read war3map.wtg: %v", err)
	}
	trig, err := Parse(wtgBytes, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	t.Logf("Enfo FFB: cats=%d vars=%d triggers=%d sub_ver=%d",
		len(trig.Categories), len(trig.Variables), len(trig.Triggers), trig.SubVersion)
	if len(trig.Triggers) == 0 {
		t.Errorf("expected at least one trigger; got 0")
	}
	if len(trig.Categories) == 0 {
		t.Errorf("expected at least one category; got 0")
	}
}

// TestParseSecretValley drives the same logic against Secret Valley v2.0 —
// a melee-derived map that exercises the standard 1.31 sub_version 7 path.
func TestParseSecretValley(t *testing.T) {
	mapPath := `C:\Users\4step\Documents\Warcraft III\Maps\Download\Season8\(2)SecretValley_S2_v2.0.w3x`
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
	wtgBytes, err := arc.Read("war3map.wtg")
	if err != nil {
		t.Fatalf("Read war3map.wtg: %v", err)
	}
	trig, err := Parse(wtgBytes, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	t.Logf("Secret Valley: cats=%d vars=%d triggers=%d", len(trig.Categories), len(trig.Variables), len(trig.Triggers))
}

// TestArgcMissingError verifies that an unknown ECA function name surfaces as
// ErrUnknownECAName, with a useful message, rather than silently misaligning
// the parse.
func TestArgcMissingError(t *testing.T) {
	// Build a hand-crafted minimal 1.31 wtg with one GUI trigger holding one
	// ECA of an unknown function name. The argc map is empty so we MUST trip
	// the unknown-name guard.
	var buf []byte
	put32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		buf = append(buf, b[:]...)
	}
	cstr := func(s string) {
		buf = append(buf, []byte(s)...)
		buf = append(buf, 0)
	}
	buf = append(buf, []byte("WTG!")...)
	put32(0x80000004)         // version
	put32(7)                  // sub_version
	for i := 0; i < 7; i++ {  // 7 deleted-id blocks (all zero)
		put32(0)
		put32(0)
	}
	put32(0)   // unknown1
	put32(0)   // unknown2
	put32(2)   // trig_def_ver
	put32(0)   // variable_count
	put32(1)   // element_count
	put32(uint32(ClassifierGUI))
	cstr("MyTrigger")    // name
	cstr("")             // description
	put32(0)             // is_comment
	put32(42)            // id
	put32(1)             // is_enabled
	put32(0)             // is_script
	put32(1)             // initially_on (inverted on disk, so 1=off)
	put32(0)             // run_on_initialization
	put32(0)             // parent_id
	put32(1)             // eca_count
	put32(uint32(ECAAction))
	cstr("NotAFunction") // name → not in argc
	put32(1)             // enabled

	_, err := Parse(buf, map[string]int{}) // empty argc
	if err == nil {
		t.Fatalf("expected error for unknown ECA name; got nil")
	}
	if !errors.Is(err, ErrUnknownECAName) {
		t.Errorf("expected ErrUnknownECAName; got %v", err)
	}
}

// TestArgcRequired verifies that Parse refuses a nil argc map outright.
func TestArgcRequired(t *testing.T) {
	if _, err := Parse([]byte("WTG!"), nil); err == nil {
		t.Errorf("expected error for nil argc; got nil")
	}
}
