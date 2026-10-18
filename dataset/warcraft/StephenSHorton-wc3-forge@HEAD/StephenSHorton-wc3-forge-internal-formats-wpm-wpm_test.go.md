---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wpm/wpm_test.go"
ref: HEAD
---

package wpm

import (
	"encoding/binary"
	"os"
	"path/filepath"
	"testing"
)

func buildHeader(w, h, version uint32) []byte {
	out := make([]byte, 16)
	copy(out, magic)
	binary.LittleEndian.PutUint32(out[4:8], version)
	binary.LittleEndian.PutUint32(out[8:12], w)
	binary.LittleEndian.PutUint32(out[12:16], h)
	return out
}

func TestParseHappy(t *testing.T) {
	// 4×4 cells with a known bit pattern.
	header := buildHeader(4, 4, 0)
	payload := []byte{
		FlagUnwalkable, 0, 0, FlagUnflyable,
		0, FlagUnbuildable | FlagUnwalkable, 0, 0,
		0, 0, FlagBlight, 0,
		FlagAmphibious, 0, 0, 0,
	}
	f, err := Parse(append(header, payload...))
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if f.Width != 4 || f.Height != 4 {
		t.Fatalf("dims: %dx%d, want 4x4", f.Width, f.Height)
	}
	if f.Cells[0] != FlagUnwalkable {
		t.Errorf("Cells[0] = %#x, want %#x", f.Cells[0], FlagUnwalkable)
	}
	if f.Cells[5] != FlagUnbuildable|FlagUnwalkable {
		t.Errorf("Cells[5] = %#x, want %#x", f.Cells[5], FlagUnbuildable|FlagUnwalkable)
	}
}

func TestParseBadMagic(t *testing.T) {
	bad := buildHeader(4, 4, 0)
	bad[0] = 'X'
	_, err := Parse(append(bad, make([]byte, 16)...))
	if err == nil {
		t.Fatal("expected magic error")
	}
}

func TestParseTooShort(t *testing.T) {
	if _, err := Parse([]byte{0x4d, 0x50, 0x33}); err == nil {
		t.Fatal("expected too-short error")
	}
}

func TestParseShortPayloadRecovers(t *testing.T) {
	// Declare 4×4 cells but only supply 8 bytes; expect a usable File with
	// the available bytes copied and the rest zero-padded.
	header := buildHeader(4, 4, 0)
	payload := []byte{FlagUnwalkable, 1, 2, 3, 4, 5, 6, 7}
	f, err := Parse(append(header, payload...))
	if err == nil {
		t.Fatal("expected short-payload warning")
	}
	if f == nil || len(f.Cells) != 16 {
		t.Fatalf("want 16 cells, got %v", f)
	}
	if f.Cells[7] != 7 || f.Cells[15] != 0 {
		t.Errorf("padding broken: %v", f.Cells)
	}
}

func TestParseFixture(t *testing.T) {
	// The wc3-survival fixture is the canonical end-to-end smoke test for
	// the parser. Walk up from the test cwd to find go.mod (repo root),
	// then resolve sideways to the sibling project. Skip cleanly when the
	// fixture isn't present (developer doesn't have wc3-survival-game
	// checked out alongside wc3-forge).
	dir, _ := os.Getwd()
	for i := 0; i < 12; i++ {
		if _, err := os.Stat(filepath.Join(dir, "go.mod")); err == nil {
			break
		}
		parent := filepath.Dir(dir)
		if parent == dir {
			break
		}
		dir = parent
	}
	// From the repo root (which may itself be inside a worktree), the
	// fixture lives at <projects-root>/wc3-survival-game/map/extracted/.
	// Worktrees nest the repo under .claude/worktrees/, so resolve sideways
	// from the projects root by stripping that nesting if present.
	projectsRoot := dir
	if base := filepath.Base(dir); base == "pathing-overlay" || filepath.Base(filepath.Dir(dir)) == "worktrees" {
		// Inside <repo>/.claude/worktrees/<name>/; the actual repo is 3 up.
		projectsRoot = filepath.Dir(filepath.Dir(filepath.Dir(filepath.Dir(dir))))
	} else {
		projectsRoot = filepath.Dir(dir)
	}
	abs := filepath.Join(projectsRoot, "wc3-survival-game", "map", "extracted", "war3map.wpm")
	data, err := os.ReadFile(abs)
	if err != nil {
		t.Skipf("fixture not available at %s: %v", abs, err)
	}
	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse fixture: %v", err)
	}
	if f.Width <= 0 || f.Height <= 0 {
		t.Fatalf("bad dims %dx%d", f.Width, f.Height)
	}
	if len(f.Cells) != f.Width*f.Height {
		t.Fatalf("cell count %d ≠ %d*%d", len(f.Cells), f.Width, f.Height)
	}
	// Count cells with any flag set; expect at least some unwalkable cells
	// for any real map.
	var unwalk int
	for _, c := range f.Cells {
		if c&FlagUnwalkable != 0 {
			unwalk++
		}
	}
	if unwalk == 0 {
		t.Errorf("fixture has zero unwalkable cells — fixture changed?")
	}
	t.Logf("fixture %dx%d, %d unwalkable cells (%.1f%%)",
		f.Width, f.Height, unwalk, 100*float64(unwalk)/float64(len(f.Cells)))
}
