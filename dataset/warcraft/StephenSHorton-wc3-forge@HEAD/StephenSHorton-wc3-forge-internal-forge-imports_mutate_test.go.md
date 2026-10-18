---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/imports_mutate_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/imp"
)

// TestImportManager_AddListRemove exercises the general Import Manager surface
// on a folder-backed map: add an arbitrary file, confirm it lists + reads back,
// remove it, confirm it's gone from both the table and the source. Reuses
// importFixtureMap from session_import_test.go.
func TestImportManager_AddListRemove(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}

	// List on a fresh map with no imports is empty (not nil).
	if got := s.ListImports(); len(got) != 0 {
		t.Errorf("fresh map ListImports = %v, want empty", got)
	}

	iconData := []byte("BLP1-fake-icon-bytes")
	// Pass a forward slash to confirm normalization to backslash.
	norm, err := s.AddImportFile("war3mapImported/BTNFoo.blp", iconData)
	if err != nil {
		t.Fatalf("AddImportFile: %v", err)
	}
	if norm != `war3mapImported\BTNFoo.blp` {
		t.Errorf("AddImportFile returned %q, want backslash-normalized path", norm)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after AddImportFile")
	}

	list := s.ListImports()
	if len(list) != 1 {
		t.Fatalf("after add, ListImports len = %d, want 1: %+v", len(list), list)
	}
	if list[0].Path != norm || list[0].Size != len(iconData) || !list[0].Present {
		t.Errorf("listed entry = %+v, want {path:%q size:%d present:true}", list[0], norm, len(iconData))
	}
	if got, ok, err := s.ReadFile(norm); err != nil || !ok {
		t.Fatalf("ReadFile after add: ok=%v err=%v", ok, err)
	} else if string(got) != string(iconData) {
		t.Errorf("read-back bytes = %q, want %q", got, iconData)
	}

	// Remove it.
	if err := s.RemoveImport(norm); err != nil {
		t.Fatalf("RemoveImport: %v", err)
	}
	if got := s.ListImports(); len(got) != 0 {
		t.Errorf("after remove, ListImports = %+v, want empty", got)
	}
	if _, ok, _ := s.ReadFile(norm); ok {
		t.Errorf("file still present after RemoveImport")
	}
}

// TestImportManager_Rename moves an imported file and confirms the bytes follow,
// the old path is gone, and the table tracks the new path.
func TestImportManager_Rename(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	data := []byte("loading-screen-bytes")
	if _, err := s.AddImportFile(`war3mapImported\Old.blp`, data); err != nil {
		t.Fatalf("AddImportFile: %v", err)
	}
	if err := s.RenameImport(`war3mapImported\Old.blp`, `war3mapImported\New.blp`); err != nil {
		t.Fatalf("RenameImport: %v", err)
	}
	if _, ok, _ := s.ReadFile(`war3mapImported\Old.blp`); ok {
		t.Errorf("old path still readable after rename")
	}
	if got, ok, err := s.ReadFile(`war3mapImported\New.blp`); err != nil || !ok {
		t.Fatalf("ReadFile new path: ok=%v err=%v", ok, err)
	} else if string(got) != string(data) {
		t.Errorf("renamed bytes = %q, want %q", got, data)
	}
	list := s.ListImports()
	if len(list) != 1 || list[0].Path != `war3mapImported\New.blp` {
		t.Errorf("after rename, list = %+v, want single New.blp", list)
	}
	// Renaming an absent path errors.
	if err := s.RenameImport(`war3mapImported\NotThere.blp`, `war3mapImported\X.blp`); err == nil {
		t.Errorf("RenameImport(absent) succeeded, want error")
	}
}

// TestImportManager_PersistAcrossSaveReopen is the end-to-end persistence
// guarantee the acceptance criteria call out: add a file, Save, reopen a fresh
// session, and confirm the file + its war3map.imp entry survived on disk.
func TestImportManager_PersistAcrossSaveReopen(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	soundData := []byte("WAV-fake-sound-bytes")
	soundPath := `war3mapImported\zap.wav`
	if _, err := s.AddImportFile(soundPath, soundData); err != nil {
		t.Fatalf("AddImportFile: %v", err)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("expected clean after Save")
	}

	// war3map.imp on disk parses + Has the new path.
	impBytes, err := os.ReadFile(filepath.Join(tmp, "war3map.imp"))
	if err != nil {
		t.Fatalf("read war3map.imp: %v", err)
	}
	impFile, err := imp.Parse(impBytes)
	if err != nil {
		t.Fatalf("parse war3map.imp: %v", err)
	}
	if !impFile.Has(soundPath) {
		t.Errorf("war3map.imp missing %q (entries: %+v)", soundPath, impFile.Entries)
	}

	// Fresh reopen: file + table entry persisted.
	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	if got, ok, err := s2.ReadFile(soundPath); err != nil || !ok {
		t.Fatalf("reopen ReadFile: ok=%v err=%v", ok, err)
	} else if string(got) != string(soundData) {
		t.Errorf("reopen bytes = %q, want %q", got, soundData)
	}
	if list := s2.ListImports(); len(list) != 1 || list[0].Path != soundPath {
		t.Errorf("reopen ListImports = %+v, want single %q", list, soundPath)
	}
}

// TestImportManager_RejectsUnsafePath asserts traversal / absolute paths are
// rejected before any state change.
func TestImportManager_RejectsUnsafePath(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	for _, bad := range []string{
		`..\evil.txt`,
		`subdir\..\..\evil.txt`,
		`C:\Windows\system32\evil.dll`,
		`/etc/passwd`,
		"",
		"   ",
	} {
		if _, err := s.AddImportFile(bad, []byte("x")); err == nil {
			t.Errorf("AddImportFile(%q) succeeded, want rejection", bad)
		}
	}
}

// TestImportManager_NoMapLoaded asserts the mutators error cleanly with no map.
func TestImportManager_NoMapLoaded(t *testing.T) {
	s := &Session{}
	if _, err := s.AddImportFile(`war3mapImported\x.blp`, []byte("x")); err == nil {
		t.Errorf("AddImportFile with no map succeeded, want error")
	}
	if err := s.RemoveImport(`war3mapImported\x.blp`); err == nil {
		t.Errorf("RemoveImport with no map succeeded, want error")
	}
	if err := s.RenameImport(`war3mapImported\a.blp`, `war3mapImported\b.blp`); err == nil {
		t.Errorf("RenameImport with no map succeeded, want error")
	}
	if got := s.ListImports(); len(got) != 0 {
		t.Errorf("ListImports with no map = %v, want empty", got)
	}
}

// TestApplyGameplayConstants_RoundTrip exercises the canonical Session-level
// gameplay-constants apply/get that both surfaces now share.
func TestApplyGameplayConstants_RoundTrip(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	rows := []GameplayConstantRow{
		{Section: "Misc", Key: "MaxHeroLevel", Value: "10"},
		{Section: "", Key: "DamageBonusNormal", Value: "1.00,2.00,1.00"},
	}
	if err := s.ApplyGameplayConstants(rows); err != nil {
		t.Fatalf("ApplyGameplayConstants: %v", err)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after ApplyGameplayConstants")
	}
	got, err := s.GetGameplayConstants()
	if err != nil {
		t.Fatalf("GetGameplayConstants: %v", err)
	}
	// Empty section defaults to "Misc"; order preserved.
	want := map[string]string{"MaxHeroLevel": "10", "DamageBonusNormal": "1.00,2.00,1.00"}
	for _, r := range got {
		if r.Section != "Misc" {
			t.Errorf("row %q section = %q, want Misc", r.Key, r.Section)
		}
		if v, ok := want[r.Key]; ok {
			if r.Value != v {
				t.Errorf("%q value = %q, want %q", r.Key, r.Value, v)
			}
			delete(want, r.Key)
		}
	}
	if len(want) != 0 {
		t.Errorf("missing rows after round-trip: %v (got: %+v)", want, got)
	}
}
