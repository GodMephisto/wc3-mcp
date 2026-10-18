---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mpq_lossless_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

// fixtureElementTD is a real 240-file custom map with unlisted custom imports
// (models/textures the editor can't name) plus FLAG_ENCRYPTED + a FLAG_KEY_
// ADJUSTED block — the worst case for the repack path. It is the map that, under
// the old name-based repack, shrank from 240 files to 14 and corrupted itself.
const fixtureElementTD = `C:\Users\4step\Documents\Warcraft III\Maps\Download\Element TD 4.3.w3x`

// snapshotPresent reads every present block of an archive into a map keyed by
// the (hashA,hashB) pair, capturing each one's raw stored bytes. This lets us
// compare two archives file-by-file WITHOUT knowing filenames — the only way to
// verify unlisted custom imports survived a repack.
func snapshotPresent(t *testing.T, path string) (count int, byHash map[[2]uint32][]byte, preheader []byte) {
	t.Helper()
	a, err := mpq.Open(path)
	if err != nil {
		t.Fatalf("open %q: %v", path, err)
	}
	defer a.Close()
	res, err := a.RawEntries()
	if err != nil {
		t.Fatalf("RawEntries %q: %v", path, err)
	}
	byHash = make(map[[2]uint32][]byte, len(res))
	for _, re := range res {
		byHash[[2]uint32{re.HashA, re.HashB}] = append([]byte(nil), re.Raw...)
	}
	count = a.PresentFileCount()
	if off := a.ArchiveOffset(); off > 0 {
		raw, _ := os.ReadFile(path)
		preheader = append([]byte(nil), raw[:off]...)
	}
	return count, byHash, preheader
}

// TestLosslessRepack_ElementTD_EditPreservesEverything is the forge-level proof
// of the lossless save: open the real 240-file Element TD .w3x as an MPQ-backed
// source, overwrite ONE file (war3map.w3i) via the source's write buffer, flush
// (repack in place), reopen, and assert:
//
//	(1) the edit took effect,
//	(2) the present-file count is unchanged (240),
//	(3) EVERY other file — including the unlisted custom imports the editor can't
//	    name — is byte-identical to the original (compared by hash),
//	(4) the HM3W preheader is preserved,
//	(5) a SECOND save (clean, force-repack) is stable: count unchanged and every
//	    non-edited file still byte-identical.
func TestLosslessRepack_ElementTD_EditPreservesEverything(t *testing.T) {
	if _, err := os.Stat(fixtureElementTD); err != nil {
		t.Skipf("fixture %q not available: %v", fixtureElementTD, err)
	}

	// Work on a throwaway copy.
	tmp := t.TempDir()
	mapPath := filepath.Join(tmp, "elementtd.w3x")
	copyFileT(t, fixtureElementTD, mapPath)

	origCount, origByHash, origPre := snapshotPresent(t, mapPath)
	if origCount < 200 {
		t.Fatalf("fixture present-count %d unexpectedly low; wrong map?", origCount)
	}

	// The edited file's identity (so we can exclude it from the byte-identical
	// comparison). We pick war3map.w3i because it's named + always present.
	_, w3iA, w3iB := mpq.HashName("war3map.w3i")
	w3iKey := [2]uint32{w3iA, w3iB}
	// The (listfile) is regenerated, so its raw bytes legitimately change.
	_, lfA, lfB := mpq.HashName("(listfile)")
	lfKey := [2]uint32{lfA, lfB}

	// Open as an MPQ-backed source and overwrite war3map.w3i.
	a, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("open archive: %v", err)
	}
	src := newMPQSource(a, mapPath)

	newW3I := []byte("FORGE LOSSLESS TEST W3I BYTES \x00\x01\x02\x03 — replaces the original")
	if err := src.write("war3map.w3i", newW3I); err != nil {
		t.Fatalf("write war3map.w3i: %v", err)
	}
	if err := src.flush(); err != nil {
		t.Fatalf("flush (lossless repack): %v", err)
	}
	_ = src.close()

	// --- Assertions after the first (edited) save --------------------------
	newCount, newByHash, newPre := snapshotPresent(t, mapPath)

	// (2) count unchanged.
	if newCount != origCount {
		t.Errorf("present-count after edit = %d, want %d", newCount, origCount)
	}

	// (4) preheader preserved.
	if !bytes.Equal(newPre, origPre) {
		t.Errorf("HM3W preheader changed across repack (got %d bytes, want %d)", len(newPre), len(origPre))
	}

	// (1) the edit took effect: read war3map.w3i back through the reader.
	ra, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("reopen for edit check: %v", err)
	}
	gotW3I, err := ra.Read("war3map.w3i")
	if err != nil {
		ra.Close()
		t.Fatalf("read war3map.w3i after repack: %v", err)
	}
	if !bytes.Equal(gotW3I, newW3I) {
		t.Errorf("war3map.w3i not updated after repack: got %q", gotW3I)
	}
	ra.Close()

	// (3) every OTHER file byte-identical (compare raw stored bytes by hash).
	// We skip the edited file and the regenerated listfile.
	missing := 0
	changed := 0
	for hash, want := range origByHash {
		if hash == w3iKey || hash == lfKey {
			continue
		}
		got, ok := newByHash[hash]
		if !ok {
			missing++
			continue
		}
		if !bytes.Equal(got, want) {
			changed++
		}
	}
	if missing != 0 {
		t.Errorf("%d original file(s) MISSING after lossless repack (dropped custom imports!)", missing)
	}
	if changed != 0 {
		t.Errorf("%d original file(s) CHANGED bytes after lossless repack (corruption!)", changed)
	}

	// --- (5) Second save: clean force-repack must be stable ----------------
	a2, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("reopen for second save: %v", err)
	}
	src2 := newMPQSource(a2, mapPath)
	if err := src2.forceRepackAll(); err != nil {
		t.Fatalf("forceRepackAll (second save): %v", err)
	}
	_ = src2.close()

	count3, byHash3, _ := snapshotPresent(t, mapPath)
	if count3 != origCount {
		t.Errorf("present-count after second save = %d, want %d", count3, origCount)
	}
	// Every non-listfile file (incl. the edited w3i, now a stable named entry)
	// must be byte-identical between save #1 and save #2.
	stableMissing, stableChanged := 0, 0
	for hash, want := range newByHash {
		if hash == lfKey {
			continue
		}
		got, ok := byHash3[hash]
		if !ok {
			stableMissing++
			continue
		}
		if !bytes.Equal(got, want) {
			stableChanged++
		}
	}
	if stableMissing != 0 || stableChanged != 0 {
		t.Errorf("second save not stable: %d missing, %d changed", stableMissing, stableChanged)
	}
}

// TestLosslessRepack_ElementTD_NoEditCleanSave proves a clean Save (no edits) on
// the 240-file map preserves all 240 files byte-identically — the exact failure
// the old name-based repack produced (240 -> 14). This is the regression gate.
func TestLosslessRepack_ElementTD_NoEditCleanSave(t *testing.T) {
	if _, err := os.Stat(fixtureElementTD); err != nil {
		t.Skipf("fixture %q not available: %v", fixtureElementTD, err)
	}
	tmp := t.TempDir()
	mapPath := filepath.Join(tmp, "elementtd-clean.w3x")
	copyFileT(t, fixtureElementTD, mapPath)

	origCount, origByHash, _ := snapshotPresent(t, mapPath)

	a, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("open: %v", err)
	}
	src := newMPQSource(a, mapPath)
	if err := src.forceRepackAll(); err != nil {
		t.Fatalf("forceRepackAll: %v", err)
	}
	_ = src.close()

	newCount, newByHash, _ := snapshotPresent(t, mapPath)
	if newCount != origCount {
		t.Fatalf("clean save changed present-count: %d -> %d (the 240->14 corruption)", origCount, newCount)
	}
	_, lfA, lfB := mpq.HashName("(listfile)")
	lfKey := [2]uint32{lfA, lfB}
	for hash, want := range origByHash {
		if hash == lfKey {
			continue
		}
		got, ok := newByHash[hash]
		if !ok {
			t.Errorf("file %v dropped by clean save", hash)
			continue
		}
		if !bytes.Equal(got, want) {
			t.Errorf("file %v changed bytes across clean save", hash)
		}
	}
}
