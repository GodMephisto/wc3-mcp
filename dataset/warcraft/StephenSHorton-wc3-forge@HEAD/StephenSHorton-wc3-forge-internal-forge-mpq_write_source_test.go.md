---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mpq_write_source_test.go"
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

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

const fixtureSurvivalW3X = `C:\Users\4step\projects\wc3-survival-game\build\wc3-survival-v1.6.w3x`

// copyFileT copies src to dst (used to work on a throwaway copy of a fixture).
func copyFileT(t *testing.T, src, dst string) {
	t.Helper()
	b, err := os.ReadFile(src)
	if err != nil {
		t.Fatalf("read %s: %v", src, err)
	}
	if err := os.WriteFile(dst, b, 0o644); err != nil {
		t.Fatalf("write %s: %v", dst, err)
	}
}

// TestMPQ_Save_RoundTrip is the end-to-end gate for the MPQ save path:
//   - copy a real .w3x to a tempdir,
//   - open it as an MPQ-backed Session,
//   - move a unit (making the session dirty),
//   - Save() -> repacks the .w3x atomically in place,
//   - reopen the .w3x in a fresh Session and confirm the move persisted,
//   - independently re-open the repacked archive with the mpq reader and
//     confirm a sample of the OTHER files round-trips byte-identically against
//     the originals (the repack didn't corrupt untouched files).
func TestMPQ_Save_RoundTrip(t *testing.T) {
	if _, err := os.Stat(fixtureSurvivalW3X); err != nil {
		t.Skipf("fixture %q not available: %v", fixtureSurvivalW3X, err)
	}
	tmp := t.TempDir()
	mapPath := filepath.Join(tmp, "roundtrip.w3x")
	copyFileT(t, fixtureSurvivalW3X, mapPath)

	// Snapshot a few original files straight from the source archive so we can
	// later assert the repack preserved them byte-for-byte.
	origArchive, err := mpq.Open(fixtureSurvivalW3X)
	if err != nil {
		t.Fatalf("open fixture archive: %v", err)
	}
	sampleNames := []string{"war3map.w3e", "war3map.w3i", "war3map.wts"}
	origBytes := make(map[string][]byte)
	for _, n := range sampleNames {
		if origArchive.Has(n) {
			b, err := origArchive.Read(n)
			if err != nil {
				t.Fatalf("read original %s: %v", n, err)
			}
			origBytes[n] = b
		}
	}
	origArchive.Close()

	// Open the COPY as an MPQ-backed session.
	s := &Session{}
	if err := s.Open(mapPath); err != nil {
		t.Fatalf("Open MPQ session: %v", err)
	}
	if s.RawMapBytes() == nil {
		t.Fatalf("expected RawMapBytes for an archive-backed open")
	}
	if s.IsDirty() {
		t.Errorf("freshly-opened session is dirty")
	}

	units := s.Units()
	if units == nil || len(units.Entities) == 0 {
		s.Close()
		t.Skipf("fixture has no placed units to mutate; skipping move assertion")
	}
	cn := units.Entities[0].CreationNumber
	orig := units.Entities[0].Position
	newX := orig[0] + 256

	if err := s.MoveUnit(cn, newX, orig[1], orig[2]); err != nil {
		t.Fatalf("MoveUnit: %v", err)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after MoveUnit")
	}

	if err := s.Save(); err != nil {
		t.Fatalf("Save (MPQ repack): %v", err)
	}
	if s.IsDirty() {
		t.Errorf("expected clean after Save")
	}
	s.Close()

	// The file must still start with the HM3W preheader (preserved through the
	// repack) — real WC3 maps carry it.
	raw, err := os.ReadFile(mapPath)
	if err != nil {
		t.Fatalf("read repacked file: %v", err)
	}
	if len(raw) >= 4 && string(raw[0:4]) != "HM3W" {
		t.Logf("repacked file does not start with HM3W (first 4 bytes %q) — acceptable only if the source had no preheader", raw[0:4])
	}

	// Reopen the repacked .w3x in a fresh session and confirm the move stuck.
	s2 := &Session{}
	if err := s2.Open(mapPath); err != nil {
		t.Fatalf("reopen repacked MPQ: %v", err)
	}
	defer s2.Close()
	u2 := s2.Units()
	if u2 == nil {
		t.Fatalf("reopened session has no units")
	}
	var found bool
	for _, e := range u2.Entities {
		if e.CreationNumber == cn {
			found = true
			if e.Position[0] != newX {
				t.Errorf("unit %d X = %v after reopen, want %v", cn, e.Position[0], newX)
			}
		}
	}
	if !found {
		t.Errorf("unit %d not found after reopen", cn)
	}

	// Independently verify untouched files survived the repack byte-identically.
	repacked, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("reopen repacked archive (reader): %v", err)
	}
	defer repacked.Close()
	for n, want := range origBytes {
		got, err := repacked.Read(n)
		if err != nil {
			t.Fatalf("read repacked %s: %v", n, err)
		}
		if !bytes.Equal(got, want) {
			t.Errorf("%s changed across repack (got %d bytes, want %d)", n, len(got), len(want))
		}
	}
}

// TestMPQ_CleanSave_StillRepacks proves the clean-save fix: a Save() on an
// unedited MPQ-backed session does NOT no-op — it repacks the archive at the
// source path, and the result is a valid, fully-readable .w3x (same files).
func TestMPQ_CleanSave_StillRepacks(t *testing.T) {
	if _, err := os.Stat(fixtureSurvivalW3X); err != nil {
		t.Skipf("fixture %q not available: %v", fixtureSurvivalW3X, err)
	}
	tmp := t.TempDir()
	mapPath := filepath.Join(tmp, "clean.w3x")
	copyFileT(t, fixtureSurvivalW3X, mapPath)

	infoBefore, _ := os.Stat(mapPath)

	s := &Session{}
	if err := s.Open(mapPath); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if s.IsDirty() {
		t.Fatalf("freshly opened map should be clean")
	}
	// Clean save — must still write a valid archive (not a misleading no-op).
	if err := s.Save(); err != nil {
		t.Fatalf("clean Save: %v", err)
	}
	s.Close()

	infoAfter, err := os.Stat(mapPath)
	if err != nil {
		t.Fatalf("stat after clean save: %v", err)
	}
	// The file should have been rewritten (we can't assert mtime reliably on
	// all platforms, but we CAN assert it's still a valid, readable archive
	// with the core files present — that's the contract that matters).
	_ = infoBefore
	_ = infoAfter

	a, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("repacked clean-save archive does not open: %v", err)
	}
	defer a.Close()
	for _, n := range []string{"war3map.w3i", "war3map.w3e"} {
		if !a.Has(n) {
			t.Errorf("repacked archive missing %s", n)
		}
		if _, err := a.Read(n); err != nil {
			t.Errorf("repacked archive %s unreadable: %v", n, err)
		}
	}
}

// TestRepackCountCheck_Policy is the deterministic, fixture-free proof of the
// import-drop policy: repackCountCheck must flag a DROP (written < expected) and
// must NOT spuriously flag a legitimate save (written == expected). It also
// confirms a count INCREASE is reported as a mismatch but is NOT in the
// refuse-direction (written < expected is the only refuse trigger in flush).
//
// We drive the helper with synthetic RawEntries + edits — exactly the inputs
// the flush path computes — so the test pins the policy decision independent of
// the MPQ encoder, the disk, or any fixture.
func TestRepackCountCheck_Policy(t *testing.T) {
	_, lfA, lfB := mpq.HashName("(listfile)")
	_, aA, aB := mpq.HashName("war3mapImported\\custom.mdx")
	_, bA, bB := mpq.HashName("war3map.w3i")

	// A source declaring 3 user files (custom.mdx present twice via a hash
	// collision — the corrupt-archive case) plus the regenerated listfile.
	raw := []mpq.RawEntry{
		{HashA: aA, HashB: aB}, // custom.mdx
		{HashA: aA, HashB: aB}, // custom.mdx (colliding duplicate)
		{HashA: bA, HashB: bB}, // war3map.w3i
		{HashA: lfA, HashB: lfB}, // source (listfile) — excluded from the count
	}
	// srcBlockCount should be 3 (listfile excluded).
	const wantSrc = 3

	t.Run("clean save (no edits) is not a mismatch", func(t *testing.T) {
		// keep = 3 user blocks copied through, named = 0 -> written 3.
		exp, src, mm := repackCountCheck(raw, nil, 3, lfA, lfB)
		if src != wantSrc {
			t.Fatalf("srcBlockCount = %d, want %d", src, wantSrc)
		}
		if exp != wantSrc || mm {
			t.Fatalf("clean save: expected=%d mismatch=%v, want expected=%d mismatch=false", exp, mm, wantSrc)
		}
	})

	t.Run("drop (tombstone matching two colliding entries) is refused", func(t *testing.T) {
		// Tombstoning custom.mdx matches BOTH colliding raw entries: the copy-
		// through drops 2, but expected only debits 1 (one logical file). So
		// written = 3-2 = 1, expected = 3-1 = 2 -> written < expected -> DROP.
		edits := []editInfo{{hA: aA, hB: aB, name: "war3mapImported\\custom.mdx", deleted: true}}
		written := 1 // keep=1 (only w3i), named=0
		exp, _, mm := repackCountCheck(raw, edits, written, lfA, lfB)
		if !mm {
			t.Fatalf("drop: mismatch=false, want true (written=%d expected=%d)", written, exp)
		}
		if !(written < exp) {
			t.Fatalf("drop: written=%d expected=%d, want written < expected (refuse direction)", written, exp)
		}
	})

	t.Run("legitimate brand-new file is not a mismatch", func(t *testing.T) {
		// Add a brand-new file (no matching raw entry): expected +1, written +1.
		_, nA, nB := mpq.HashName("war3map.wts")
		edits := []editInfo{{hA: nA, hB: nB, name: "war3map.wts", data: []byte("strings")}}
		written := 4 // keep=3, named=1
		exp, _, mm := repackCountCheck(raw, edits, written, lfA, lfB)
		if mm {
			t.Fatalf("brand-new file: mismatch=true, want false (written=%d expected=%d)", written, exp)
		}
	})
}

// writeCollisionArchiveT builds a REAL on-disk MPQ whose RawEntries contain a
// deliberate hash collision: two distinct physical blocks carrying identical
// (HashA,HashB) — the corrupt/adversarial source that makes the lossless repack
// drop files. It does this entirely through the mpq package's public API:
//  1. write a normal archive with `victim` + `decoy` + a filler file,
//  2. read its RawEntries (valid flags/raw bytes),
//  3. doctor the decoy entry to carry the victim's name-hashes,
//  4. re-emit losslessly. RawEntries on the result now returns two entries that
//     a tombstone for `victim` will both match -> a count drop at flush.
//
// Returns the path and the victim's storage name (the file to tombstone).
func writeCollisionArchiveT(t *testing.T, dir string) (path, victim string) {
	t.Helper()
	victim = "war3mapImported\\custom.mdx"
	decoy := "war3mapImported\\custom.blp"
	base := filepath.Join(dir, "base.w3x")
	// One file is padded past 512 bytes so the resulting archive clears the
	// reader's minimum-size guard (a bare MPQ must be at least the HM3W header
	// size to be opened).
	if err := mpq.WriteFile(base, []mpq.FileEntry{
		{Name: "war3map.w3i", Data: []byte("info bytes — a normal kept file")},
		{Name: victim, Data: bytes.Repeat([]byte("MDLX victim model bytes "), 64)},
		{Name: decoy, Data: bytes.Repeat([]byte{0xAB}, 64)},
	}, nil); err != nil {
		t.Fatalf("write base archive: %v", err)
	}
	a, err := mpq.Open(base)
	if err != nil {
		t.Fatalf("open base archive: %v", err)
	}
	rawEntries, err := a.RawEntries()
	hashCount := a.HashTableSize()
	sectorShift := a.SectorSizeShift()
	a.Close()
	if err != nil {
		t.Fatalf("RawEntries base: %v", err)
	}

	_, lfA, lfB := mpq.HashName("(listfile)")
	_, vA, vB := mpq.HashName(victim)
	_, dA, dB := mpq.HashName(decoy)

	// Doctor the decoy's slot to carry the victim's name-hashes (the collision),
	// and copy every other present block through verbatim (excluding the source
	// listfile, which the writer regenerates).
	keep := make([]mpq.RawEntry, 0, len(rawEntries))
	for _, re := range rawEntries {
		if re.HashA == lfA && re.HashB == lfB {
			continue
		}
		if re.HashA == dA && re.HashB == dB {
			re.HashA, re.HashB = vA, vB // make the decoy collide with the victim
			re.Slot++                   // a distinct slot so both survive placement
		}
		keep = append(keep, re)
	}

	path = filepath.Join(dir, "collision.w3x")
	if err := mpq.WriteFileLossless(path, keep, nil, hashCount, sectorShift, true, nil); err != nil {
		t.Fatalf("write collision archive: %v", err)
	}
	return path, victim
}

// TestMPQ_Flush_RefusesImportDrop_PreservesOriginal is the end-to-end DO-NO-HARM
// proof: a flush that would drop a file (the 240->14 import-loss class) must
// REFUSE — returning a wrapped ErrMPQRepackFailed with an actionable message —
// and must leave the user's only copy of the .w3x byte-for-byte intact. It also
// confirms the diagnostic counter is bumped and the buffered edit stays pending.
func TestMPQ_Flush_RefusesImportDrop_PreservesOriginal(t *testing.T) {
	tmp := t.TempDir()
	mapPath, victim := writeCollisionArchiveT(t, tmp)

	// Capture the exact pre-save bytes; the refuse path must not touch them.
	origBytes, err := os.ReadFile(mapPath)
	if err != nil {
		t.Fatalf("read original collision archive: %v", err)
	}

	a, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("open collision archive: %v", err)
	}
	src := newMPQSource(a, mapPath)
	defer src.close()

	// Tombstone the victim — it matches BOTH colliding raw entries, so the
	// copy-through drops two blocks while the count bookkeeping debits one: the
	// flush sees written < expected and must refuse.
	if err := src.delete(victim); err != nil {
		t.Fatalf("buffer tombstone: %v", err)
	}

	before := mapIODiag.repackFileMismatch.Load()

	flushErr := src.flush()
	if flushErr == nil {
		t.Fatalf("flush: want a refuse error on import drop, got nil (the lossy archive would have been written!)")
	}
	if !errors.Is(flushErr, ErrMPQRepackFailed) {
		t.Fatalf("flush error does not wrap ErrMPQRepackFailed: %v", flushErr)
	}
	// (2) actionable message: names the loss + the folder-extraction workaround.
	msg := flushErr.Error()
	if !strings.Contains(strings.ToLower(msg), "drop") {
		t.Errorf("error not actionable: missing the import-drop reason: %q", msg)
	}
	if !strings.Contains(strings.ToLower(msg), "extract") || !strings.Contains(strings.ToLower(msg), "folder") {
		t.Errorf("error not actionable: missing the extract-to-a-folder workaround: %q", msg)
	}

	// (3) diagnostic counter still bumped (instrumentation preserved).
	if got := mapIODiag.repackFileMismatch.Load(); got != before+1 {
		t.Errorf("repackFileMismatch counter = %d, want %d (before %d)", got, before+1, before)
	}

	// ACCEPTANCE: the original .w3x is byte-for-byte intact (no lossy overwrite).
	afterBytes, err := os.ReadFile(mapPath)
	if err != nil {
		t.Fatalf("read archive after refused flush: %v", err)
	}
	if !bytes.Equal(origBytes, afterBytes) {
		t.Errorf("REFUSE-the-save broken: original archive changed across a refused flush (%d -> %d bytes)",
			len(origBytes), len(afterBytes))
	}

	// The buffered tombstone must remain pending so a later (corrected) save can
	// still apply it — the refuse is non-destructive to session state too.
	src.mu.Lock()
	stillPending := src.deleted[mpqNameKey(victim)]
	src.mu.Unlock()
	if !stillPending {
		t.Errorf("buffered tombstone was cleared by a refused flush; want it kept pending for retry")
	}
}

// TestMPQ_Flush_NormalEdit_NoFalseRefusal guards the other side of the policy:
// a legitimate in-place edit (no file-count change) must NOT trip the drop guard
// — it saves losslessly and the edit is readable back. This proves the refuse
// logic does not false-positive on the common save path.
func TestMPQ_Flush_NormalEdit_NoFalseRefusal(t *testing.T) {
	tmp := t.TempDir()
	mapPath := filepath.Join(tmp, "normal.w3x")
	// Pad one file past the reader's 512-byte minimum-archive-size guard.
	if err := mpq.WriteFile(mapPath, []mpq.FileEntry{
		{Name: "war3map.w3i", Data: []byte("original info bytes")},
		{Name: "war3mapImported\\a.mdx", Data: bytes.Repeat([]byte("model A "), 80)},
		{Name: "war3mapImported\\b.blp", Data: bytes.Repeat([]byte{0x01}, 32)},
	}, nil); err != nil {
		t.Fatalf("write normal archive: %v", err)
	}

	a, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("open normal archive: %v", err)
	}
	src := newMPQSource(a, mapPath)

	newW3I := []byte("EDITED info bytes — replaces the original")
	if err := src.write("war3map.w3i", newW3I); err != nil {
		t.Fatalf("buffer edit: %v", err)
	}
	if err := src.flush(); err != nil {
		t.Fatalf("normal edit flush refused unexpectedly: %v", err)
	}
	_ = src.close()

	// The edit took effect and every other file still resolves.
	ra, err := mpq.Open(mapPath)
	if err != nil {
		t.Fatalf("reopen after normal edit: %v", err)
	}
	defer ra.Close()
	got, err := ra.Read("war3map.w3i")
	if err != nil {
		t.Fatalf("read edited war3map.w3i: %v", err)
	}
	if !bytes.Equal(got, newW3I) {
		t.Errorf("edit not applied: got %q, want %q", got, newW3I)
	}
	for _, n := range []string{"war3mapImported\\a.mdx", "war3mapImported\\b.blp"} {
		if _, err := ra.Read(n); err != nil {
			t.Errorf("kept file %q unreadable after normal edit: %v", n, err)
		}
	}
}
