---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/session_import_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/imp"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// importFixtureMap builds a minimal folder-backed map (just the required
// war3map.w3i) in a tempdir and returns its path. Self-contained — the w3i is
// encoded in-memory so the test doesn't depend on any external fixture map.
func importFixtureMap(t *testing.T) string {
	t.Helper()
	dir := t.TempDir()
	info := &w3i.Info{
		FileVersion: w3i.FileVersionTFT,
		MapVersion:  1,
		Name:        "ImportFixture",
		Author:      "tests",
		Tileset:     'L',
	}
	b, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("w3i.Encode: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), b, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	return dir
}

// TestImportModel_FolderRoundTrip is the end-to-end smoke test for the import
// slice: open a folder-backed map, ImportModel a tiny fake MDX + one BLP,
// confirm ReadFile returns both, war3map.imp parses and Has() the new paths,
// the session goes dirty, and Save+reopen preserves everything on disk
// (lossless). Mirrors the folderSource round-trip shape of the save tests.
func TestImportModel_FolderRoundTrip(t *testing.T) {
	tmp := importFixtureMap(t)

	s := &Session{}
	var dirtyHistory []bool
	s.OnDirtyChanged(func(d bool) { dirtyHistory = append(dirtyHistory, d) })
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if s.IsDirty() {
		t.Fatalf("freshly-opened session is dirty")
	}

	mdxName := `war3mapImported\cube.mdx`
	blpName := `war3mapImported\cube.blp`
	mdxData := []byte("MDLX-fake-model-bytes")
	blpData := []byte("BLP1-fake-texture-bytes")

	res, err := s.ImportModel(ImportModelReq{
		ModelName: mdxName,
		MdxData:   mdxData,
		Textures:  []ImportTexture{{Name: blpName, Data: blpData}},
	})
	if err != nil {
		t.Fatalf("ImportModel: %v", err)
	}
	if res.ModelPath != mdxName {
		t.Errorf("ModelPath = %q, want %q", res.ModelPath, mdxName)
	}
	if len(res.TexturePaths) != 1 || res.TexturePaths[0] != blpName {
		t.Errorf("TexturePaths = %v, want [%q]", res.TexturePaths, blpName)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after ImportModel")
	}

	// ReadFile returns the buffered/written bytes pre-Save (preview path).
	if got, ok, err := s.ReadFile(mdxName); err != nil || !ok {
		t.Fatalf("ReadFile mdx: ok=%v err=%v", ok, err)
	} else if string(got) != string(mdxData) {
		t.Errorf("mdx bytes = %q, want %q", got, mdxData)
	}
	if got, ok, err := s.ReadFile(blpName); err != nil || !ok {
		t.Fatalf("ReadFile blp: ok=%v err=%v", ok, err)
	} else if string(got) != string(blpData) {
		t.Errorf("blp bytes = %q, want %q", got, blpData)
	}

	// Save, then verify war3map.imp on disk parses + Has both paths, and the
	// imported bytes survived to disk (lossless reopen).
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("expected clean after Save")
	}

	impBytes, err := os.ReadFile(filepath.Join(tmp, "war3map.imp"))
	if err != nil {
		t.Fatalf("read war3map.imp from disk: %v", err)
	}
	impFile, err := imp.Parse(impBytes)
	if err != nil {
		t.Fatalf("parse war3map.imp: %v", err)
	}
	if !impFile.Has(mdxName) {
		t.Errorf("war3map.imp missing %q (entries: %+v)", mdxName, impFile.Entries)
	}
	if !impFile.Has(blpName) {
		t.Errorf("war3map.imp missing %q (entries: %+v)", blpName, impFile.Entries)
	}

	// Reopen a fresh session and confirm the imported files + import table
	// persisted losslessly.
	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	if got, ok, err := s2.ReadFile(mdxName); err != nil || !ok {
		t.Fatalf("reopen ReadFile mdx: ok=%v err=%v", ok, err)
	} else if string(got) != string(mdxData) {
		t.Errorf("reopen mdx bytes = %q, want %q", got, mdxData)
	}
	if got, ok, err := s2.ReadFile(blpName); err != nil || !ok {
		t.Fatalf("reopen ReadFile blp: ok=%v err=%v", ok, err)
	} else if string(got) != string(blpData) {
		t.Errorf("reopen blp bytes = %q, want %q", got, blpData)
	}

	// Dirty bus: at least one true (ImportModel) then a false (Save).
	if len(dirtyHistory) < 2 {
		t.Errorf("expected >=2 dirty events, got %d: %v", len(dirtyHistory), dirtyHistory)
	} else {
		if !dirtyHistory[0] {
			t.Errorf("first dirty event = %v, want true", dirtyHistory[0])
		}
		if dirtyHistory[len(dirtyHistory)-1] {
			t.Errorf("last dirty event = %v, want false", dirtyHistory[len(dirtyHistory)-1])
		}
	}
}

// TestImportModel_NoMapLoaded asserts ImportModel errors cleanly when nothing
// is loaded, rather than panicking on a nil source.
func TestImportModel_NoMapLoaded(t *testing.T) {
	s := &Session{}
	if _, err := s.ImportModel(ImportModelReq{ModelName: `war3mapImported\x.mdx`, MdxData: []byte("x")}); err == nil {
		t.Fatalf("expected error when no map loaded")
	}
}

// TestImportModel_SecondImportReusesCachedTable confirms the lazy war3map.imp
// parse is cached on the Session (Add is idempotent, so re-importing the same
// path doesn't duplicate the entry) and a second distinct import appends.
func TestImportModel_SecondImportReusesCachedTable(t *testing.T) {
	tmp := importFixtureMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if _, err := s.ImportModel(ImportModelReq{ModelName: `war3mapImported\a.mdx`, MdxData: []byte("a")}); err != nil {
		t.Fatalf("ImportModel a: %v", err)
	}
	if _, err := s.ImportModel(ImportModelReq{ModelName: `war3mapImported\b.mdx`, MdxData: []byte("b")}); err != nil {
		t.Fatalf("ImportModel b: %v", err)
	}
	// Re-import a -> idempotent, no duplicate.
	if _, err := s.ImportModel(ImportModelReq{ModelName: `war3mapImported\a.mdx`, MdxData: []byte("a")}); err != nil {
		t.Fatalf("ImportModel a again: %v", err)
	}
	if s.imp == nil {
		t.Fatalf("expected cached imp table")
	}
	if !s.imp.Has(`war3mapImported\a.mdx`) || !s.imp.Has(`war3mapImported\b.mdx`) {
		t.Errorf("expected both paths registered, entries: %+v", s.imp.Entries)
	}
	count := 0
	for _, e := range s.imp.Entries {
		if canonicalImpPath(e.Path) == canonicalImpPath(`war3mapImported\a.mdx`) {
			count++
		}
	}
	if count != 1 {
		t.Errorf("path a registered %d times, want 1 (idempotent)", count)
	}
}

// canonicalImpPath mirrors imp's internal canonicalization for the dup check
// above (lowercase + slash-normalized).
func canonicalImpPath(p string) string {
	out := make([]rune, 0, len(p))
	for _, r := range p {
		if r == '\\' {
			r = '/'
		}
		if r >= 'A' && r <= 'Z' {
			r += 'a' - 'A'
		}
		out = append(out, r)
	}
	return string(out)
}
