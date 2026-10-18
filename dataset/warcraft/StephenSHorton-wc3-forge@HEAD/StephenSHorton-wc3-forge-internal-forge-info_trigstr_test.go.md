---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/info_trigstr_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wts"
)

// localizedFixture copies the folder fixture and rewrites its war3map.w3i so the
// map Name is a TRIGSTR token, with a matching war3map.wts — i.e. a localized
// map, which is exactly the case the divergence bug bit.
func localizedFixture(t *testing.T, nameToken string, id uint32, value string) string {
	t.Helper()
	dir := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	raw, err := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	if err != nil {
		t.Fatalf("read fixture w3i: %v", err)
	}
	info, err := w3i.Parse(raw)
	if err != nil {
		t.Fatalf("parse fixture w3i: %v", err)
	}
	info.Name = nameToken
	nb, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("encode w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), nb, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	wb, err := wts.Encode(wts.Strings{id: value})
	if err != nil {
		t.Fatalf("encode wts: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.wts"), wb, 0o644); err != nil {
		t.Fatalf("write wts: %v", err)
	}
	return dir
}

// TestMapInfoEdit_PreservesTrigstrAndWritesWts is the regression test for the
// w3i↔wts divergence: editing a TRIGSTR-backed Map Info field must update the
// referenced war3map.wts entry (and write the file) while leaving the w3i token
// intact — NOT inline a literal that orphans the wts entry.
func TestMapInfoEdit_PreservesTrigstrAndWritesWts(t *testing.T) {
	dir := localizedFixture(t, "TRIGSTR_005", 5, "Original Name")
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}

	// Open resolved the token to its display value.
	if got := s.Info().Name; got != "Original Name" {
		t.Fatalf("after Open, Name=%q want %q (resolve failed)", got, "Original Name")
	}

	// Edit the name.
	if _, err := s.SetMapInfo(map[string]any{"name": "Edited Name"}); err != nil {
		t.Fatalf("SetMapInfo: %v", err)
	}
	if got := s.Info().Name; got != "Edited Name" {
		t.Errorf("after edit, Name=%q want %q", got, "Edited Name")
	}
	// The wts entry tracked the edit (not orphaned).
	if got := s.Strings()[5]; got != "Edited Name" {
		t.Errorf("wts[5]=%q want %q (edit should update the referenced entry)", got, "Edited Name")
	}

	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	s.Close()

	// On disk: w3i keeps the TOKEN, the literal is NOT inlined, and wts is updated.
	rawW3i, _ := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	if !bytes.Contains(rawW3i, []byte("TRIGSTR_005")) {
		t.Errorf("saved w3i lost the TRIGSTR_005 token")
	}
	if bytes.Contains(rawW3i, []byte("Edited Name")) {
		t.Errorf("saved w3i inlined the literal name (orphaning the wts entry — the bug)")
	}
	rawWts, _ := os.ReadFile(filepath.Join(dir, "war3map.wts"))
	if !bytes.Contains(rawWts, []byte("Edited Name")) {
		t.Errorf("saved wts was not updated with the new name")
	}

	// Re-open: the new value resolves through the (token → updated wts) chain.
	s2 := &Session{}
	if err := s2.Open(dir); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	defer s2.Close()
	if got := s2.Info().Name; got != "Edited Name" {
		t.Errorf("after reopen, Name=%q want %q", got, "Edited Name")
	}
}

// TestMapInfoEdit_TrigstrUndoRevertsWts: undo of a localized Map Info edit
// reverts BOTH the displayed field and the underlying wts entry.
func TestMapInfoEdit_TrigstrUndoRevertsWts(t *testing.T) {
	dir := localizedFixture(t, "TRIGSTR_007", 7, "Before")
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer s.Close()
	if _, err := s.SetMapInfo(map[string]any{"name": "After"}); err != nil {
		t.Fatalf("SetMapInfo: %v", err)
	}
	if s.Strings()[7] != "After" {
		t.Fatalf("pre-undo wts[7]=%q want After", s.Strings()[7])
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := s.Info().Name; got != "Before" {
		t.Errorf("after undo, Name=%q want Before", got)
	}
	if got := s.Strings()[7]; got != "Before" {
		t.Errorf("after undo, wts[7]=%q want Before (undo must revert the wts entry too)", got)
	}
}

// TestMapInfoEdit_NonLocalized_NoWts: on a map with a LITERAL name (no token),
// an edit writes the literal into w3i and never touches/creates war3map.wts.
func TestMapInfoEdit_NonLocalized_NoWts(t *testing.T) {
	dir := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	// Force a literal name (no token) so there's nothing to capture.
	raw, _ := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	info, err := w3i.Parse(raw)
	if err != nil {
		t.Fatalf("parse: %v", err)
	}
	info.Name = "Literal Map Name"
	nb, _ := w3i.Encode(info)
	_ = os.WriteFile(filepath.Join(dir, "war3map.w3i"), nb, 0o644)
	_ = os.Remove(filepath.Join(dir, "war3map.wts")) // ensure non-localized

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer s.Close()
	if _, err := s.SetMapInfo(map[string]any{"name": "New Literal"}); err != nil {
		t.Fatalf("SetMapInfo: %v", err)
	}
	if got := s.Info().Name; got != "New Literal" {
		t.Errorf("Name=%q want New Literal", got)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	// No war3map.wts should have been created for a non-localized edit.
	if _, err := os.Stat(filepath.Join(dir, "war3map.wts")); err == nil {
		t.Errorf("non-localized edit should not create war3map.wts")
	}
	// The literal lands directly in w3i.
	rawW3i, _ := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	if !bytes.Contains(rawW3i, []byte("New Literal")) {
		t.Errorf("literal name not written to w3i")
	}
}
