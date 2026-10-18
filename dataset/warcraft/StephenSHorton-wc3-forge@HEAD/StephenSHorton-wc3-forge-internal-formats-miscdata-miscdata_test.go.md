---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/miscdata/miscdata_test.go"
ref: HEAD
---

package miscdata

import (
	"bytes"
	"testing"
)

func TestParseEmpty(t *testing.T) {
	f, err := Parse(nil)
	if err != nil {
		t.Fatalf("Parse(nil): %v", err)
	}
	if len(f.Sections) != 0 {
		t.Errorf("expected 0 sections, got %d", len(f.Sections))
	}
}

func TestParseBOMAndSection(t *testing.T) {
	input := []byte("\xef\xbb\xbf[Misc]\nBoneDecayTime=0.1\nMaxHeroLevel=10\n")
	f, err := Parse(input)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(f.Sections) != 1 {
		t.Fatalf("expected 1 section, got %d", len(f.Sections))
	}
	if f.Sections[0].Name != "Misc" {
		t.Errorf("section name = %q, want %q", f.Sections[0].Name, "Misc")
	}
	if got, ok := f.Get("Misc", "BoneDecayTime"); !ok || got != "0.1" {
		t.Errorf("BoneDecayTime = %q ok=%v, want %q true", got, ok, "0.1")
	}
	if got, ok := f.Get("Misc", "MaxHeroLevel"); !ok || got != "10" {
		t.Errorf("MaxHeroLevel = %q ok=%v, want %q true", got, ok, "10")
	}
}

func TestParseComments(t *testing.T) {
	input := []byte("// header comment\n[Misc]\n; semicolon comment\nKey=val // trailing\nOther=v2 ; trailing too\n")
	f, err := Parse(input)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if got, _ := f.Get("Misc", "Key"); got != "val" {
		t.Errorf("Key value = %q, want %q (trailing // should be stripped)", got, "val")
	}
	if got, _ := f.Get("Misc", "Other"); got != "v2" {
		t.Errorf("Other value = %q, want %q (trailing ; should be stripped)", got, "v2")
	}
}

func TestParseDuplicateKeysFirstWins(t *testing.T) {
	input := []byte("[Misc]\nKey=first\nKey=second\n")
	f, _ := Parse(input)
	if got, _ := f.Get("Misc", "Key"); got != "first" {
		t.Errorf("dup key = %q, want %q (first wins)", got, "first")
	}
	if len(f.Sections[0].Entries) != 1 {
		t.Errorf("expected 1 entry after dedup, got %d", len(f.Sections[0].Entries))
	}
}

func TestParseListValuesAndImplicitSection(t *testing.T) {
	// No [Header] before the first entry — implicit "Misc" section.
	input := []byte("DamageBonusNormal=1.00,2.00,1.00,1.00,1.00,1.00,1.00,1.00\n")
	f, _ := Parse(input)
	if len(f.Sections) != 1 || f.Sections[0].Name != "Misc" {
		t.Fatalf("expected implicit [Misc] section, got %+v", f.Sections)
	}
	if got, _ := f.Get("Misc", "DamageBonusNormal"); got != "1.00,2.00,1.00,1.00,1.00,1.00,1.00,1.00" {
		t.Errorf("list value = %q", got)
	}
}

func TestRoundTrip(t *testing.T) {
	input := []byte("\xef\xbb\xbf[Misc]\nAgiAttackSpeedBonus=0.0\nBoneDecayTime=0.1\nMaxHeroLevel=5000\nDamageBonusNormal=1.00,2.00,1.00,1.00,1.00,1.00,1.00,1.00\n")
	f, err := Parse(input)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(out, input) {
		t.Errorf("round-trip mismatch:\n got  %q\n want %q", out, input)
	}
}

func TestSetUpdatesInPlace(t *testing.T) {
	f, _ := Parse([]byte("[Misc]\nA=1\nB=2\nC=3\n"))
	f.Set("Misc", "B", "99")
	if got, _ := f.Get("Misc", "B"); got != "99" {
		t.Errorf("Set: B = %q, want %q", got, "99")
	}
	// Position must be preserved.
	if f.Sections[0].Entries[1].Key != "B" {
		t.Errorf("Set should preserve entry position; got order %v", entryKeys(f.Sections[0]))
	}
}

func TestSetAppendsNew(t *testing.T) {
	f, _ := Parse([]byte("[Misc]\nA=1\n"))
	f.Set("Misc", "B", "2")
	if len(f.Sections[0].Entries) != 2 {
		t.Fatalf("expected 2 entries after Set, got %d", len(f.Sections[0].Entries))
	}
	if got, _ := f.Get("Misc", "B"); got != "2" {
		t.Errorf("Set: B = %q, want %q", got, "2")
	}
}

func TestSetCreatesSection(t *testing.T) {
	f := &File{}
	f.Set("Misc", "K", "v")
	if got, _ := f.Get("Misc", "K"); got != "v" {
		t.Errorf("Set on empty file failed; K=%q", got)
	}
}

func TestDelete(t *testing.T) {
	f, _ := Parse([]byte("[Misc]\nA=1\nB=2\nC=3\n"))
	if !f.Delete("Misc", "B") {
		t.Fatalf("Delete should return true for present key")
	}
	if _, ok := f.Get("Misc", "B"); ok {
		t.Errorf("Delete left B behind")
	}
	if len(f.Sections[0].Entries) != 2 {
		t.Errorf("expected 2 entries after Delete, got %d", len(f.Sections[0].Entries))
	}
	if f.Delete("Misc", "nope") {
		t.Errorf("Delete should return false for absent key")
	}
}

func entryKeys(s *Section) []string {
	out := make([]string, len(s.Entries))
	for i, e := range s.Entries {
		out[i] = e.Key
	}
	return out
}
