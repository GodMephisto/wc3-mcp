---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mapfile_ops_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"
)

// TestSaveAsThenExtractRoundTrip exercises the full on-the-wire persist loop:
// open a folder map, edit it, SaveAs to a .w3x (folder → packaged archive,
// session repoints + goes MPQ-backed), then ExtractToFolder back out (archive →
// loose files). The edit must survive every hop.
func TestSaveAsThenExtractRoundTrip(t *testing.T) {
	src := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	s := &Session{}
	if err := s.Open(src); err != nil {
		t.Fatalf("Open folder: %v", err)
	}
	units := s.Units()
	if units == nil || len(units.Entities) < 1 {
		t.Skipf("fixture has %d entities, want ≥1", len(units.Entities))
	}
	cn := units.Entities[0].CreationNumber
	const moveX, moveY float32 = 1234, -5678
	if err := s.MoveUnit(cn, moveX, moveY, 0); err != nil {
		t.Fatalf("MoveUnit: %v", err)
	}

	// SaveAs → .w3x. Session should repoint to it and become MPQ-backed.
	w3x := filepath.Join(t.TempDir(), "exported.w3x")
	if err := s.SaveAs(w3x); err != nil {
		t.Fatalf("SaveAs: %v", err)
	}
	if fi, err := os.Stat(w3x); err != nil || fi.Size() == 0 {
		t.Fatalf("exported .w3x missing/empty: %v (size=%v)", err, fi)
	}
	if !pathsEqual(s.Path(), w3x) {
		t.Errorf("after SaveAs, Path()=%q want %q", s.Path(), w3x)
	}
	if _, isMPQ := s.source.(*mpqSource); !isMPQ {
		t.Errorf("after SaveAs to .w3x, session should be MPQ-backed")
	}
	// The edit must have packaged into the archive.
	if got := s.Units().Entities[0].Position; got[0] != moveX || got[1] != moveY {
		t.Errorf("edit lost across SaveAs: pos=%v want X=%v Y=%v", got, moveX, moveY)
	}

	// ExtractToFolder → loose files; the destination must not pre-exist.
	dest := filepath.Join(t.TempDir(), "roundtrip-extract")
	if err := s.ExtractToFolder(dest); err != nil {
		t.Fatalf("ExtractToFolder: %v", err)
	}
	for _, want := range []string{"war3map.w3i", "war3map.w3e", "war3mapUnits.doo"} {
		if _, err := os.Stat(filepath.Join(dest, want)); err != nil {
			t.Errorf("extracted folder missing %s: %v", want, err)
		}
	}

	// Re-open the extracted folder in a fresh session — the edit must persist.
	s2 := &Session{}
	if err := s2.Open(dest); err != nil {
		t.Fatalf("Open extracted folder: %v", err)
	}
	var found bool
	for i := range s2.Units().Entities {
		if s2.Units().Entities[i].CreationNumber == cn {
			found = true
			if p := s2.Units().Entities[i].Position; p[0] != moveX || p[1] != moveY {
				t.Errorf("edit lost across extract: pos=%v want X=%v Y=%v", p, moveX, moveY)
			}
		}
	}
	if !found {
		t.Errorf("unit cn=%d missing after round-trip", cn)
	}

	// Release the open .w3x / folder handles before t.TempDir cleanup (Windows
	// can't remove a file an mpqSource still holds open).
	s2.Close()
	s.Close()
}

// TestExtractToFolder_RejectsFolderBacked: a folder-backed map has nothing to
// extract.
func TestExtractToFolder_RejectsFolderBacked(t *testing.T) {
	src := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	s := &Session{}
	if err := s.Open(src); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.ExtractToFolder(filepath.Join(t.TempDir(), "x")); err == nil {
		t.Errorf("ExtractToFolder on a folder-backed map should error")
	}
}

// TestExtractToFolder_RejectsExistingDest: extraction must land in a fresh
// folder so it can't mingle with unrelated files.
func TestExtractToFolder_RejectsExistingDest(t *testing.T) {
	src := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	s := &Session{}
	if err := s.Open(src); err != nil {
		t.Fatalf("Open: %v", err)
	}
	w3x := filepath.Join(t.TempDir(), "m.w3x")
	if err := s.SaveAs(w3x); err != nil {
		t.Fatalf("SaveAs: %v", err)
	}
	existing := t.TempDir() // already exists
	if err := s.ExtractToFolder(existing); err == nil {
		t.Errorf("ExtractToFolder into an existing dir should error")
	}
	s.Close() // release the .w3x before t.TempDir cleanup
}

// TestSaveAs_RejectsNonArchivePath: SaveAs only writes .w3x/.w3m.
func TestSaveAs_RejectsNonArchivePath(t *testing.T) {
	src := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	s := &Session{}
	if err := s.Open(src); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.SaveAs(filepath.Join(t.TempDir(), "notamap.txt")); err == nil {
		t.Errorf("SaveAs to a .txt path should error")
	}
}
