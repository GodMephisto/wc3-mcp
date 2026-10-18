---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/atomic_save_baseline_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"errors"
	"os"
	"path/filepath"
	"sync"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// writeHandRolledLuaFolderMap drops a minimal extracted Lua map into a fresh
// tempdir: a v28 (Reforged) war3map.w3i with the Lua flag set, plus a
// hand-authored war3map.lua. With no war3map.wtg present, Open synthesizes a
// hand-rolled "Map Header" trigger from the .lua — so SaveTriggerScript returns
// that text verbatim (no codegen / TriggerData dependency), giving a
// deterministic DIRECT write to war3map.lua we can drive in a test.
func writeHandRolledLuaFolderMap(t *testing.T, luaText string) string {
	t.Helper()
	dir := t.TempDir()

	info := &w3i.Info{
		FileVersion:    w3i.FileVersionRefV28, // 28 — carries the Lua flag
		Lua:            true,
		Name:           "Baseline Refresh Test",
		Author:         "tester",
		Description:    "fixture",
		PlayableWidth:  64,
		PlayableHeight: 64,
		Players:        []w3i.Player{{InternalNumber: 0, Name: "Player 1"}},
		Forces:         []w3i.Force{{Name: "Force 1", PlayerMasks: 0xFFFFFFFF}},
	}
	b, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("encode lua w3i: %v", err)
	}
	if parsed, perr := w3i.Parse(b); perr != nil {
		t.Fatalf("lua w3i did not round-trip: %v", perr)
	} else if !parsed.Lua {
		t.Fatalf("lua flag did not survive round-trip")
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), b, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.lua"), []byte(luaText), 0o644); err != nil {
		t.Fatalf("write lua: %v", err)
	}
	return dir
}

// TestSave_DirectWriteRefreshesBaseline is the regression for the self-inflicted
// false "external change": a DIRECT write that bypasses Save's batch commit
// (here SaveTriggerScript → war3map.lua) must refresh the change-detection
// baseline for the file it wrote, so the NEXT Save() does NOT mistake the
// editor's own write for a concurrent external edit and refuse it.
//
// We also assert the real detection still fires: after the direct write, a
// genuine out-of-band modification of a baselined file IS still refused.
func TestSave_DirectWriteRefreshesBaseline(t *testing.T) {
	dir := writeHandRolledLuaFolderMap(t, "-- hand authored\nprint('hi')\n")

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}

	// Sanity: Open recorded a baseline for war3map.lua (it's in baselineFileNames
	// and present on disk).
	s.mu.RLock()
	_, luaBaselined := s.srcBaseline["war3map.lua"]
	s.mu.RUnlock()
	if !luaBaselined {
		t.Fatalf("expected war3map.lua to be baselined at Open")
	}

	// Edit the hand-rolled Map Header so the direct write produces DIFFERENT
	// (longer) bytes than what's on disk — guaranteeing the file's size (hence
	// its stamp) changes, so the staleness check has something real to compare.
	// This also sets mapHeaderScriptDirty, so the subsequent Save() re-writes
	// war3map.lua and therefore RE-STATS it in Phase 3 — exactly the path the
	// bug fires on.
	newHeader := "-- hand authored, now edited\nprint('hi')\nprint('and more lines so the file grows')\n"
	if err := s.SetMapHeaderScript(newHeader); err != nil {
		t.Fatalf("SetMapHeaderScript: %v", err)
	}

	// DIRECT write that bypasses Save: SaveTriggerScript writes the edited
	// (longer) Map Header text straight to war3map.lua, changing its on-disk
	// stamp relative to the Open baseline.
	if n, err := s.SaveTriggerScript(); err != nil {
		t.Fatalf("SaveTriggerScript: %v", err)
	} else if n != len(newHeader) {
		t.Fatalf("SaveTriggerScript wrote %d bytes, want %d", n, len(newHeader))
	}

	// THE BUG: before the fix, SaveTriggerScript left the war3map.lua baseline
	// stale, so this Save re-stats war3map.lua, sees a changed stamp, and
	// spuriously refuses with ErrSourceChangedOnDisk. After the fix the direct
	// write refreshed the baseline, so Save succeeds.
	if err := s.Save(); err != nil {
		if errors.Is(err, ErrSourceChangedOnDisk) {
			t.Fatalf("Save spuriously refused as externally-changed after the editor's OWN direct write (baseline not refreshed): %v", err)
		}
		t.Fatalf("Save after direct write: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("session still dirty after a successful save")
	}

	// Now prove the REAL detection still works — the fix must not blind it.
	// Queue another info edit so war3map.w3i lands in the next Save's write set,
	// then genuinely modify war3map.w3i out-of-band (as a concurrent instance /
	// agent would). Save must still refuse with ErrSourceChangedOnDisk.
	if err := s.MutateInfo(func(i *w3i.Info) { i.Name = "Second Edit" }); err != nil {
		t.Fatalf("second MutateInfo: %v", err)
	}
	clobber := bytes.Repeat([]byte("Z"), 4096)
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), clobber, 0o644); err != nil {
		t.Fatalf("external clobber: %v", err)
	}
	err := s.Save()
	if !errors.Is(err, ErrSourceChangedOnDisk) {
		t.Fatalf("Save: want ErrSourceChangedOnDisk after a genuine external edit, got %v", err)
	}
	// The external bytes must be intact (the refused save touched nothing).
	if cur, _ := os.ReadFile(filepath.Join(dir, "war3map.w3i")); !bytes.Equal(cur, clobber) {
		t.Errorf("refused save still overwrote the externally-changed war3map.w3i")
	}
}

// TestSaveBaseline_ConcurrentNoRace stresses the change-detection baseline
// (s.srcBaseline) under the editor's designed dual-control-surface concurrency:
// the GUI and the MCP bridge both drive the one Session from independent
// goroutines. The race-prone pair is a Save reading the baseline (checkStaleness)
// running concurrently with a direct writer mutating it under the lock
// (SaveTriggerScript → refreshBaselineEntry, or another Save → refreshBaseline
// AfterCommit). An earlier version of checkStaleness aliased the live map and
// read it lock-free after releasing s.mu, which Go aborts as
// "fatal error: concurrent map read and map write".
//
// This is a no-crash / no-data-race test (run it under `go test -race`, as the
// macOS CI leg does), NOT a correctness test: concurrent folder saves legitimately
// refuse each other with ErrSourceChangedOnDisk, so all save errors are tolerated.
// It passes by completing without the runtime aborting and with -race clean.
func TestSaveBaseline_ConcurrentNoRace(t *testing.T) {
	dir := writeHandRolledLuaFolderMap(t, "-- concurrent\nprint('hi')\n")
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}

	const iters = 60
	var wg sync.WaitGroup

	// Dirtier: keep dirtyInfo set so Save reaches the encode + staleness path
	// (a clean folder save returns early without touching the baseline).
	wg.Add(1)
	go func() {
		defer wg.Done()
		for i := 0; i < iters; i++ {
			_ = s.MutateInfo(func(in *w3i.Info) { in.Name = "edit" })
		}
	}()

	// Two savers: each Save reads the baseline (checkStaleness) and, on a
	// successful folder commit, writes it (refreshBaselineAfterCommit).
	for g := 0; g < 2; g++ {
		wg.Add(1)
		go func() {
			defer wg.Done()
			for i := 0; i < iters; i++ {
				_ = s.Save()
			}
		}()
	}

	// Script saver: SaveTriggerScript is a DIRECT write that mutates the baseline
	// under the lock via refreshBaselineEntry — the writer side of the race.
	wg.Add(1)
	go func() {
		defer wg.Done()
		for i := 0; i < iters; i++ {
			_ = s.SetMapHeaderScript("-- concurrent\nprint('hi')\nprint('edit')\n")
			_, _ = s.SaveTriggerScript()
		}
	}()

	wg.Wait()
}
