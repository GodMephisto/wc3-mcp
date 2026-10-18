---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mpqchain/chain_test.go"
ref: HEAD
---

package mpqchain

import (
	"bytes"
	"errors"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

// fakeArchive is an in-memory archiveReader used to exercise Chain paths that
// a real on-disk MPQ can't easily produce — chiefly a file that is PRESENT
// (Has==true) but UNREADABLE (Read errors), and an archive with no listable
// entries. present maps the exact lookup name -> bytes; readErr[name], when
// set, makes Read fail even though Has reports the file present.
type fakeArchive struct {
	present map[string][]byte
	readErr map[string]error
	listing []string
}

func (f *fakeArchive) Has(name string) bool {
	_, ok := f.present[name]
	return ok
}

func (f *fakeArchive) Read(name string) ([]byte, error) {
	if err := f.readErr[name]; err != nil {
		return nil, err
	}
	if b, ok := f.present[name]; ok {
		return b, nil
	}
	return nil, errors.New("fakeArchive: not present")
}

func (f *fakeArchive) List() []string { return f.listing }
func (f *fakeArchive) Close() error   { return nil }

// writeArchive builds a real on-disk MPQ at <dir>/<name> holding the given
// (path -> bytes) files. Uses the package's own writer so the chain reads
// through the same code path production does.
//
// The mpq reader rejects files smaller than 512 bytes (the HM3W preheader
// floor in OpenReader), so we always include a filler file to push tiny
// fixtures past that threshold. The filler uses a unique per-archive name so
// it never collides with a real entry under test.
func writeArchive(t *testing.T, dir, name string, files map[string][]byte) {
	t.Helper()
	entries := []mpq.FileEntry{
		{Name: "_filler_" + name + ".bin", Data: make([]byte, 512)},
	}
	for p, d := range files {
		entries = append(entries, mpq.FileEntry{Name: p, Data: d})
	}
	if err := mpq.WriteFile(filepath.Join(dir, name), entries, nil); err != nil {
		t.Fatalf("write %s: %v", name, err)
	}
}

// TestOverrideOrder is the load-bearing correctness test: a file present in
// BOTH War3.mpq (base) and War3Patch.mpq (patch) must resolve to the PATCH
// bytes, because patch archives override base archives. A file present only
// in the base must still resolve. Getting the iteration direction backward
// silently serves stale base assets — this asserts the patch bytes
// specifically.
func TestOverrideOrder(t *testing.T) {
	dir := t.TempDir()

	baseBytes := []byte("BASE footman model")
	patchBytes := []byte("PATCH footman model -- this one wins")
	baseOnlyBytes := []byte("only in base archive")

	const shared = "units/human/footman/footman.mdx"
	const baseOnly = "units/human/peasant/peasant.mdx"

	writeArchive(t, dir, "War3.mpq", map[string][]byte{
		shared:   baseBytes,
		baseOnly: baseOnlyBytes,
	})
	writeArchive(t, dir, "War3Patch.mpq", map[string][]byte{
		shared: patchBytes,
	})

	c, err := Open(dir)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer c.Close()

	// Shared path must resolve to the PATCH bytes (override wins).
	got, ok, err := c.ReadFile(shared)
	if err != nil || !ok {
		t.Fatalf("ReadFile(%q): ok=%v err=%v", shared, ok, err)
	}
	if !bytes.Equal(got, patchBytes) {
		t.Fatalf("override order wrong: shared path resolved to %q, want patch bytes %q", got, patchBytes)
	}

	// Base-only path must still resolve from the base archive.
	got, ok, err = c.ReadFile(baseOnly)
	if err != nil || !ok {
		t.Fatalf("ReadFile(%q): ok=%v err=%v", baseOnly, ok, err)
	}
	if !bytes.Equal(got, baseOnlyBytes) {
		t.Fatalf("base-only path resolved to %q, want %q", got, baseOnlyBytes)
	}
}

// TestReadFileMiss confirms a path absent from every archive returns
// (nil, false, nil) — matching casc.Storage's contract so the asset
// handler's map-first ordering is preserved.
func TestReadFileMiss(t *testing.T) {
	dir := t.TempDir()
	writeArchive(t, dir, "War3.mpq", map[string][]byte{
		"units/human/footman/footman.mdx": []byte("x"),
	})

	c, err := Open(dir)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer c.Close()

	data, ok, err := c.ReadFile("does/not/exist.mdx")
	if err != nil {
		t.Fatalf("miss returned error: %v", err)
	}
	if ok || data != nil {
		t.Fatalf("miss returned ok=%v data=%v, want (nil,false,nil)", ok, data)
	}
}

// TestReadFileSlashEquivalence confirms forward- and back-slash paths both
// resolve (the mpq reader normalizes internally) — the asset handler passes
// lowercased forward-slash paths.
func TestReadFileSlashEquivalence(t *testing.T) {
	dir := t.TempDir()
	want := []byte("data")
	writeArchive(t, dir, "War3.mpq", map[string][]byte{
		`units\human\footman\footman.mdx`: want,
	})

	c, err := Open(dir)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer c.Close()

	got, ok, err := c.ReadFile("units/human/footman/footman.mdx")
	if err != nil || !ok {
		t.Fatalf("forward-slash lookup: ok=%v err=%v", ok, err)
	}
	if !bytes.Equal(got, want) {
		t.Fatalf("got %q want %q", got, want)
	}
}

// TestListByPrefix unions + de-dupes listfile entries across archives,
// lowercases + forward-slashes them, and filters by prefix — mirroring
// casc.Storage.ListByPrefix.
func TestListByPrefix(t *testing.T) {
	dir := t.TempDir()
	writeArchive(t, dir, "War3.mpq", map[string][]byte{
		"ReplaceableTextures/CommandButtons/BTNFootman.blp": []byte("a"),
		"units/human/footman/footman.mdx":                   []byte("b"),
	})
	writeArchive(t, dir, "War3Patch.mpq", map[string][]byte{
		"ReplaceableTextures/CommandButtons/BTNPeasant.blp": []byte("c"),
		// Duplicate of a base entry — must de-dupe.
		"ReplaceableTextures/CommandButtons/BTNFootman.blp": []byte("d"),
	})

	c, err := Open(dir)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer c.Close()

	got, err := c.ListByPrefix("replaceabletextures/commandbuttons/")
	if err != nil {
		t.Fatalf("ListByPrefix: %v", err)
	}
	want := []string{
		"replaceabletextures/commandbuttons/btnfootman.blp",
		"replaceabletextures/commandbuttons/btnpeasant.blp",
	}
	if len(got) != len(want) {
		t.Fatalf("ListByPrefix returned %v, want %v", got, want)
	}
	for i := range want {
		if got[i] != want[i] {
			t.Fatalf("ListByPrefix[%d] = %q, want %q (full: %v)", i, got[i], want[i], got)
		}
	}
}

// TestOpenNoArchives errors when the directory holds no Classic archives.
func TestOpenNoArchives(t *testing.T) {
	dir := t.TempDir()
	if _, err := Open(dir); err == nil {
		t.Fatal("Open on empty dir: want error, got nil")
	}
}

// TestIsClassicInstall covers the classifier: a dir with War3.mpq and no
// .build.info is Classic; a dir with .build.info is NOT (it's CASC); an
// empty dir is neither.
func TestIsClassicInstall(t *testing.T) {
	// Classic: War3.mpq present, no .build.info.
	classic := t.TempDir()
	writeArchive(t, classic, "War3.mpq", map[string][]byte{"x": []byte("y")})
	if !IsClassicInstall(classic) {
		t.Error("dir with War3.mpq and no .build.info should be Classic")
	}

	// Classic via War3Patch.mpq alone.
	patchOnly := t.TempDir()
	writeArchive(t, patchOnly, "War3Patch.mpq", map[string][]byte{"x": []byte("y")})
	if !IsClassicInstall(patchOnly) {
		t.Error("dir with War3Patch.mpq and no .build.info should be Classic")
	}

	// CASC: .build.info present -> NOT Classic even with a stray War3.mpq.
	casc := t.TempDir()
	writeArchive(t, casc, "War3.mpq", map[string][]byte{"x": []byte("y")})
	if err := os.WriteFile(filepath.Join(casc, ".build.info"), []byte("manifest"), 0o644); err != nil {
		t.Fatal(err)
	}
	if IsClassicInstall(casc) {
		t.Error("dir with .build.info should NOT be Classic (it's CASC)")
	}

	// Neither: empty dir.
	if IsClassicInstall(t.TempDir()) {
		t.Error("empty dir should not be Classic")
	}
	if IsClassicInstall("") {
		t.Error("empty path should not be Classic")
	}
}

// TestCaseInsensitiveArchiveNames confirms lowercase archive filenames (Wine
// / case-sensitive FS) are matched.
func TestCaseInsensitiveArchiveNames(t *testing.T) {
	dir := t.TempDir()
	writeArchive(t, dir, "war3.mpq", map[string][]byte{
		"units/human/footman/footman.mdx": []byte("z"),
	})
	if !IsClassicInstall(dir) {
		t.Fatal("lowercase war3.mpq should be recognized as Classic")
	}
	c, err := Open(dir)
	if err != nil {
		t.Fatalf("Open lowercase archive: %v", err)
	}
	defer c.Close()
	if _, ok, _ := c.ReadFile("units/human/footman/footman.mdx"); !ok {
		t.Fatal("read from lowercase-named archive failed")
	}
}

// TestReadFileSkipsUnreadableArchive covers ReadFile's read-error fall-through
// (chain.go): a higher-priority archive that HAS the file but fails to Read it
// (e.g. an unsupported-codec sector) must not fail the request — resolution
// continues to the next archive instead of 500-ing.
func TestReadFileSkipsUnreadableArchive(t *testing.T) {
	const name = "units/human/footman/footman.mdx"
	high := &fakeArchive{
		present: map[string][]byte{name: []byte("patch-but-corrupt")},
		readErr: map[string]error{name: errors.New("bad sector: unsupported codec")},
	}
	low := &fakeArchive{present: map[string][]byte{name: []byte("base-ok")}}
	c := &Chain{archives: []archiveReader{high, low}, names: []string{"War3Patch.mpq", "War3.mpq"}}

	got, ok, err := c.ReadFile(name)
	if err != nil {
		t.Fatalf("ReadFile returned error %v; the present-but-unreadable archive should be skipped, not propagated", err)
	}
	if !ok || string(got) != "base-ok" {
		t.Fatalf("ReadFile = (%q, %v); want (\"base-ok\", true) — must fall through past the unreadable patch archive", got, ok)
	}
}

// TestReadFileAllUnreadableIsMiss: when the only archive holding the file
// can't read it, ReadFile reports a clean miss (nil,false,nil), matching the
// casc.Storage contract, rather than surfacing the read error.
func TestReadFileAllUnreadableIsMiss(t *testing.T) {
	const name = "x.mdx"
	only := &fakeArchive{
		present: map[string][]byte{name: {0x01}},
		readErr: map[string]error{name: errors.New("boom")},
	}
	c := &Chain{archives: []archiveReader{only}, names: []string{"only"}}

	got, ok, err := c.ReadFile(name)
	if got != nil || ok || err != nil {
		t.Fatalf("ReadFile = (%v, %v, %v); want (nil, false, nil) when the only holder is unreadable", got, ok, err)
	}
}

// TestListByPrefixArchiveWithoutEntriesContributesNothing covers the
// ListByPrefix claim that an archive lacking listable entries (e.g. no
// (listfile)) simply contributes nothing to the union — it must not error or
// disturb the lower archive's entries. Also exercises backslash->forward-slash
// normalization + lowercasing of the surviving names.
func TestListByPrefixArchiveWithoutEntriesContributesNothing(t *testing.T) {
	withEntries := &fakeArchive{listing: []string{
		`ReplaceableTextures\CommandButtons\BTNFootman.blp`,
		"units/human/footman/footman.mdx",
	}}
	empty := &fakeArchive{} // List() == nil, like an archive with no (listfile)
	// empty is higher priority; it must not alter the union.
	c := &Chain{archives: []archiveReader{empty, withEntries}, names: []string{"patch", "base"}}

	got, err := c.ListByPrefix("replaceabletextures/commandbuttons/")
	if err != nil {
		t.Fatalf("ListByPrefix: %v", err)
	}
	want := []string{"replaceabletextures/commandbuttons/btnfootman.blp"}
	if len(got) != len(want) || got[0] != want[0] {
		t.Fatalf("ListByPrefix = %v; want %v (empty archive contributes nothing; names lowercased + forward-slashed)", got, want)
	}
}
