---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/atomic_save_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"errors"
	"os"
	"path/filepath"
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// minimalW3I encodes a tiny-but-valid war3map.w3i so Open() can parse it. We
// round-trip it through Parse in the test that uses it to guarantee the bytes
// are loadable before relying on Open.
func minimalW3I(t *testing.T) []byte {
	t.Helper()
	info := &w3i.Info{
		FileVersion:    w3i.FileVersionTFT, // 25 — classic TFT, smallest tail we encode
		Name:           "Atomic Save Test",
		Author:         "tester",
		Description:    "fixture",
		PlayableWidth:  64,
		PlayableHeight: 64,
		Players:        []w3i.Player{{InternalNumber: 0, Name: "Player 1"}},
		Forces:         []w3i.Force{{Name: "Force 1", PlayerMasks: 0xFFFFFFFF}},
	}
	b, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("encode minimal w3i: %v", err)
	}
	if _, err := w3i.Parse(b); err != nil {
		t.Fatalf("minimal w3i did not round-trip through Parse: %v", err)
	}
	return b
}

// writeFolderMap drops a minimal extracted map (war3map.w3i + war3mapUnits.doo)
// into a fresh tempdir and returns the dir. The units file holds one entity so
// MoveUnit has something to mutate.
func writeFolderMap(t *testing.T) (dir string, unitCN uint32) {
	t.Helper()
	dir = t.TempDir()

	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), minimalW3I(t), 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}

	units := &unitsdoo.File{
		Format:     [4]byte{'W', '3', 'd', 'o'},
		Version:    8,
		SubVersion: 9, // sub9 hand-constructed: no skin_id chunk required
		Entities: []unitsdoo.Entity{{
			TypeID:         "hfoo",
			Position:       [3]float32{100, 200, 0},
			Scale:          [3]float32{1, 1, 1},
			HitPointsPct:   -1,
			ManaPct:        -1,
			RandomData:     make([]byte, 4),
			CreationNumber: 1,
		}},
	}
	ub, err := unitsdoo.Encode(units)
	if err != nil {
		t.Fatalf("encode units: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3mapUnits.doo"), ub, 0o644); err != nil {
		t.Fatalf("write units: %v", err)
	}
	return dir, 1
}

// TestSave_AbortedEncodeLeavesOriginalsAndDirtyFlags is acceptance (a): an
// encode failure mid-save must leave EVERY original file byte-identical on disk
// and EVERY dirty flag still set, so the user can retry without having lost work
// or produced an inconsistent map (some files new, some stale).
//
// We arrange two dirty files: war3mapUnits.doo (encodes fine) and war3map.w3i
// (its in-memory FileVersion is set to 0, which w3i.Encode rejects). The
// all-or-nothing encode phase encodes units first, then info — info fails, so
// Save must abort before writing ANY byte.
func TestSave_AbortedEncodeLeavesOriginalsAndDirtyFlags(t *testing.T) {
	dir := t.TempDir()

	// Real on-disk originals.
	origUnits := []byte("ORIGINAL-UNITS-BYTES-DO-NOT-TOUCH")
	origInfo := []byte("ORIGINAL-INFO-BYTES-DO-NOT-TOUCH")
	if err := os.WriteFile(filepath.Join(dir, "war3mapUnits.doo"), origUnits, 0o644); err != nil {
		t.Fatalf("seed units: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), origInfo, 0o644); err != nil {
		t.Fatalf("seed info: %v", err)
	}

	s := &Session{
		loaded: true,
		path:   dir,
		source: folderSource{root: dir},
		// A valid, encodable units file.
		units: &unitsdoo.File{Format: [4]byte{'W', '3', 'd', 'o'}, Version: 8, SubVersion: 9, Entities: []unitsdoo.Entity{{
			TypeID: "hfoo", Scale: [3]float32{1, 1, 1}, HitPointsPct: -1, ManaPct: -1, RandomData: make([]byte, 4), CreationNumber: 1,
		}}},
		// FileVersion 0 makes w3i.Encode fail — our injected encode error.
		info: &w3i.Info{FileVersion: 0, Name: "boom"},
	}
	s.dirtyUnits = true
	s.dirtyInfo = true

	err := s.Save()
	if err == nil {
		t.Fatal("expected Save to fail on the w3i encode error, got nil")
	}
	if !strings.Contains(err.Error(), "war3map.w3i") {
		t.Errorf("error should name the failing file, got: %v", err)
	}

	// Originals untouched (the whole point of all-or-nothing).
	gotUnits, _ := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo"))
	if !bytes.Equal(gotUnits, origUnits) {
		t.Errorf("war3mapUnits.doo was modified despite aborted save:\n got %q\nwant %q", gotUnits, origUnits)
	}
	gotInfo, _ := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	if !bytes.Equal(gotInfo, origInfo) {
		t.Errorf("war3map.w3i was modified despite aborted save:\n got %q\nwant %q", gotInfo, origInfo)
	}

	// Dirty flags intact so the user can retry.
	s.mu.RLock()
	du, di := s.dirtyUnits, s.dirtyInfo
	s.mu.RUnlock()
	if !du || !di {
		t.Errorf("dirty flags cleared on aborted save: dirtyUnits=%v dirtyInfo=%v (want both true)", du, di)
	}

	// And no temp files leaked into the map directory.
	assertNoTempFiles(t, dir)
}

// TestSave_AtomicWriteAndBackup is acceptance (b)+(c-of-backup): a successful
// folder save replaces the file via temp+rename (no partial/.tmp residue) AND
// leaves a .bak holding the prior on-disk contents.
func TestSave_AtomicWriteAndBackup(t *testing.T) {
	dir, cn := writeFolderMap(t)

	priorUnits, err := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo"))
	if err != nil {
		t.Fatalf("read prior units: %v", err)
	}

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.MoveUnit(cn, 500, 600, 0); err != nil {
		t.Fatalf("MoveUnit: %v", err)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}

	// The .bak holds the PRIOR bytes (recoverable prior version).
	bak, err := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo"+BackupSuffix))
	if err != nil {
		t.Fatalf("expected war3mapUnits.doo%s after overwrite: %v", BackupSuffix, err)
	}
	if !bytes.Equal(bak, priorUnits) {
		t.Errorf(".bak does not hold the prior contents: got %d bytes, want %d", len(bak), len(priorUnits))
	}

	// The live file parses and carries the new position (no torn write).
	live, err := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo"))
	if err != nil {
		t.Fatalf("read live units: %v", err)
	}
	parsed, err := unitsdoo.Parse(live)
	if err != nil {
		t.Fatalf("live units file is unparseable (torn write?): %v", err)
	}
	if len(parsed.Entities) != 1 || parsed.Entities[0].Position != [3]float32{500, 600, 0} {
		t.Errorf("live units file did not capture the move: %+v", parsed.Entities)
	}

	// No .tmp residue.
	assertNoTempFiles(t, dir)
}

// TestSave_RefusesStaleFile is acceptance (c): a file changed on disk after
// Open (another editor / agent) is detected and the save is refused by default
// with ErrSourceChangedOnDisk; passing SaveOptions{Force:true} overrides it.
func TestSave_RefusesStaleFile(t *testing.T) {
	dir, cn := writeFolderMap(t)

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.MoveUnit(cn, 777, 888, 0); err != nil {
		t.Fatalf("MoveUnit: %v", err)
	}

	// Simulate another instance/agent rewriting war3mapUnits.doo AFTER our Open.
	// Use a clearly different size so the stamp differs even on coarse-mtime
	// filesystems.
	clobber := bytes.Repeat([]byte("X"), 4096)
	if err := os.WriteFile(filepath.Join(dir, "war3mapUnits.doo"), clobber, 0o644); err != nil {
		t.Fatalf("external clobber: %v", err)
	}

	err := s.Save()
	if !errors.Is(err, ErrSourceChangedOnDisk) {
		t.Fatalf("Save: want ErrSourceChangedOnDisk, got %v", err)
	}
	// The external bytes must NOT have been overwritten by our refused save.
	if cur, _ := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo")); !bytes.Equal(cur, clobber) {
		t.Errorf("refused save still touched the file on disk")
	}
	// And our edit is still pending (dirty) for a later forced retry.
	if !s.IsDirty() {
		t.Errorf("session went clean after a refused save")
	}

	// Force overrides: the save now goes through and backs up the external bytes.
	if err := s.SaveWith(SaveOptions{Force: true}); err != nil {
		t.Fatalf("forced Save: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("session still dirty after a successful forced save")
	}
	bak, err := os.ReadFile(filepath.Join(dir, "war3mapUnits.doo"+BackupSuffix))
	if err != nil {
		t.Fatalf("forced overwrite should back up the clobbered bytes: %v", err)
	}
	if !bytes.Equal(bak, clobber) {
		t.Errorf(".bak after forced save should hold the external bytes")
	}
	// A second save with no external change must succeed (baseline refreshed).
	if err := s.MoveUnit(cn, 111, 222, 0); err != nil {
		t.Fatalf("second MoveUnit: %v", err)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("second Save after baseline refresh should succeed, got: %v", err)
	}
}

// TestFolderSourceWrite_IsAtomic asserts the single-file folder write path no
// longer truncates in place: a write to an existing file replaces it via
// temp+rename, leaving the result fully written and no .tmp residue. (The
// direct-writer guarantee that protects ImportModel / Convert-to-Lua.)
func TestFolderSourceWrite_IsAtomic(t *testing.T) {
	dir := t.TempDir()
	fs := folderSource{root: dir}

	if err := fs.write("war3map.w3e", bytes.Repeat([]byte("A"), 1000)); err != nil {
		t.Fatalf("first write: %v", err)
	}
	// Overwrite with different-length content.
	want := bytes.Repeat([]byte("B"), 50)
	if err := fs.write("war3map.w3e", want); err != nil {
		t.Fatalf("overwrite: %v", err)
	}
	got, err := os.ReadFile(filepath.Join(dir, "war3map.w3e"))
	if err != nil {
		t.Fatalf("read back: %v", err)
	}
	if !bytes.Equal(got, want) {
		t.Errorf("overwrite content mismatch: got %d bytes, want %d", len(got), len(want))
	}
	assertNoTempFiles(t, dir)

	// Nested-path write still works (war3mapImported\ creation).
	if err := fs.write(`war3mapImported\foo.mdx`, []byte("mdx")); err != nil {
		t.Fatalf("nested write: %v", err)
	}
	if b, _ := os.ReadFile(filepath.Join(dir, "war3mapImported", "foo.mdx")); string(b) != "mdx" {
		t.Errorf("nested write content = %q", b)
	}
}

// assertNoTempFiles fails if any sibling temp file (left by a torn or aborted
// atomic write) survives in the map directory.
func assertNoTempFiles(t *testing.T, dir string) {
	t.Helper()
	entries, err := os.ReadDir(dir)
	if err != nil {
		t.Fatalf("read dir: %v", err)
	}
	for _, e := range entries {
		n := e.Name()
		if strings.HasSuffix(n, ".tmp") || strings.HasPrefix(n, ".forgesave-") || strings.HasPrefix(n, ".mpqwrite-") {
			t.Errorf("leaked temp file in map dir: %s", n)
		}
	}
}
