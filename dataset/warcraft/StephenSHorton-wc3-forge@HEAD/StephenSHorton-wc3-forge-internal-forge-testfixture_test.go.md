---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/testfixture_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"
)

// committedMapFixtureDir is a small, in-repo extracted-map folder used by the
// safety-critical save-pipeline round-trip tests. It holds the minimum a folder
// open needs (war3map.w3i — REQUIRED) plus war3mapUnits.doo (10 entities,
// including non-sloc units the move/rotate/scale tests mutate) and war3map.w3e
// (a v12 terrain with a 6-ground / 2-cliff palette the tileset-swap test
// exercises). See internal/forge/testdata/extracted-map/README.md for
// provenance.
//
// Before this fixture existed every session-save round-trip test Skipf'd on an
// out-of-repo path (C:\Users\4step\projects\wc3-survival-game\map\extracted),
// so a clean checkout / CI never proved the save pipeline was lossless. A
// regression that corrupted a unit move, an Info field write, or a tileset
// swap on Save could have shipped green.
var committedMapFixtureDir = filepath.Join("testdata", "extracted-map")

// copyCommittedMapFixture copies the committed extracted-map folder to a fresh
// tempdir and returns its path. Unlike copyFixtureToTemp (which Skipf's on a
// missing out-of-repo path), this fails loudly if the in-repo fixture is
// missing — the whole point is that the test always RUNS.
func copyCommittedMapFixture(t *testing.T) string {
	t.Helper()
	src := committedMapFixtureDir
	if _, err := os.Stat(src); err != nil {
		t.Fatalf("committed map fixture %q missing (should be checked in): %v", src, err)
	}
	tmp := t.TempDir()
	entries, err := os.ReadDir(src)
	if err != nil {
		t.Fatalf("read fixture dir: %v", err)
	}
	for _, e := range entries {
		if e.IsDir() {
			continue
		}
		// Skip the human-facing README; it's not a map file.
		if e.Name() == "README.md" {
			continue
		}
		b, err := os.ReadFile(filepath.Join(src, e.Name()))
		if err != nil {
			t.Fatalf("read %s: %v", e.Name(), err)
		}
		if err := os.WriteFile(filepath.Join(tmp, e.Name()), b, 0o644); err != nil {
			t.Fatalf("write %s: %v", e.Name(), err)
		}
	}
	return tmp
}
