---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/write_test.go"
ref: HEAD
---

package mpq

import (
	"bytes"
	"math/rand"
	"os"
	"path/filepath"
	"testing"
)

// TestEncryptDecryptRoundTrip proves encryptBlock is the exact inverse of
// decryptBlock (and vice-versa) on the same key, for random uint32-aligned
// data. This is the cryptographic foundation the table writers depend on: if
// this fails, every archive we write would have an undecryptable hash/block
// table and the reader could never open them.
func TestEncryptDecryptRoundTrip(t *testing.T) {
	rng := rand.New(rand.NewSource(1))
	for _, n := range []int{4, 16, 64, 256, 1024} {
		data := make([]byte, n)
		for i := range data {
			data[i] = byte(rng.Intn(256))
		}
		orig := append([]byte(nil), data...)
		key := rng.Uint32()

		// encrypt then decrypt -> identity
		work := append([]byte(nil), orig...)
		encryptBlock(work, key)
		if bytes.Equal(work, orig) && n >= 16 {
			t.Errorf("n=%d: encryptBlock produced identical bytes (no encryption happened)", n)
		}
		decryptBlock(work, key)
		if !bytes.Equal(work, orig) {
			t.Fatalf("n=%d: decrypt(encrypt(x)) != x", n)
		}

		// decrypt then encrypt -> identity (the cipher is invertible both ways)
		work2 := append([]byte(nil), orig...)
		decryptBlock(work2, key)
		encryptBlock(work2, key)
		if !bytes.Equal(work2, orig) {
			t.Fatalf("n=%d: encrypt(decrypt(x)) != x", n)
		}
	}
}

// sampleFiles returns a spread of payloads chosen to stress the reader:
//   - a tiny text file
//   - a binary file with a recognisable header (mimics war3map.w3e)
//   - an empty file (uSize==0 short-circuit path)
//   - a file whose size is NOT a multiple of 4 (alignment edge)
//   - a file larger than one logical sector (4096 bytes) to confirm
//     single-unit storage doesn't accidentally get sectored on read
func sampleFiles(t *testing.T) []FileEntry {
	t.Helper()
	big := make([]byte, 10000)
	rng := rand.New(rand.NewSource(42))
	for i := range big {
		big[i] = byte(rng.Intn(256))
	}
	return []FileEntry{
		{Name: "war3map.w3i", Data: []byte("HELLO WORLD this is a fake w3i\x00\x01\x02\x03")},
		{Name: "war3map.w3e", Data: append([]byte("W3E!"), big...)},
		{Name: "war3map.j", Data: []byte("function main takes nothing returns nothing\nendfunction\n")},
		{Name: "empty.txt", Data: []byte{}},
		{Name: "odd.bin", Data: []byte{0xDE, 0xAD, 0xBE}}, // length 3, not 4-aligned
		{Name: "war3mapUnits.doo", Data: bytes.Repeat([]byte{0xAB, 0xCD}, 2049)}, // 4098 bytes > sector
	}
}

// TestWriteRoundTrip is the authoritative correctness gate: build an archive,
// open it with THIS package's reader, and assert every file reads back
// byte-identical. If this fails, the writer MUST NOT be wired into map.save.
func TestWriteRoundTrip(t *testing.T) {
	files := sampleFiles(t)

	archive, err := Build(files)
	if err != nil {
		t.Fatalf("Build: %v", err)
	}
	if len(archive) < headerSizeV0 {
		t.Fatalf("archive too small: %d bytes", len(archive))
	}
	// Header magic must be at offset 0.
	if !bytes.Equal(archive[0:4], magicMPQ[:]) {
		t.Fatalf("archive does not start with MPQ magic: % x", archive[0:4])
	}

	a, err := OpenReader(bytes.NewReader(archive), int64(len(archive)))
	if err != nil {
		t.Fatalf("OpenReader on freshly-written archive: %v", err)
	}
	defer a.Close()

	for _, f := range files {
		t.Run(f.Name, func(t *testing.T) {
			if !a.Has(f.Name) {
				t.Fatalf("Has(%q) = false after writing it", f.Name)
			}
			got, err := a.Read(f.Name)
			if err != nil {
				t.Fatalf("Read(%q): %v", f.Name, err)
			}
			if !bytes.Equal(got, f.Data) {
				idx, ctx := firstDiff(got, f.Data)
				t.Fatalf("%s: round-trip mismatch (first diff at byte %d: %s, sizes got=%d want=%d)",
					f.Name, idx, ctx, len(got), len(f.Data))
			}
		})
	}

	// The generated (listfile) must be present and enumerate every file.
	if !a.Has(listfileName) {
		t.Fatalf("archive is missing the generated %s", listfileName)
	}
	lf, err := a.Read(listfileName)
	if err != nil {
		t.Fatalf("Read(%s): %v", listfileName, err)
	}
	for _, f := range files {
		if !bytes.Contains(lf, []byte(f.Name)) {
			t.Errorf("(listfile) does not mention %q; got:\n%s", f.Name, lf)
		}
	}

	// List() should surface the names too (union with standard set).
	listed := make(map[string]bool)
	for _, n := range a.List() {
		listed[n] = true
	}
	for _, f := range files {
		if !listed[f.Name] {
			t.Errorf("List() missing %q (got %v)", f.Name, a.List())
		}
	}
}

// TestWriteFileAtomic exercises the on-disk WriteFile path + atomic replace,
// then reopens via Open (the full file path, not a reader) to confirm the
// header-at-offset-0 archive opens from disk and round-trips.
func TestWriteFileAtomic(t *testing.T) {
	files := sampleFiles(t)
	dir := t.TempDir()
	path := filepath.Join(dir, "out.w3x")

	// Pre-seed the destination with junk so we confirm the replace happens
	// and the junk is gone (no append, no leftover).
	if err := os.WriteFile(path, []byte("OLD CONTENTS THAT MUST BE REPLACED"), 0o644); err != nil {
		t.Fatalf("seed: %v", err)
	}

	if err := WriteFile(path, files, nil); err != nil {
		t.Fatalf("WriteFile: %v", err)
	}

	// No stray temp files left behind.
	entries, _ := os.ReadDir(dir)
	for _, e := range entries {
		if filepath.Ext(e.Name()) == ".tmp" {
			t.Errorf("stray temp file left behind: %s", e.Name())
		}
	}

	a, err := Open(path)
	if err != nil {
		t.Fatalf("Open(%q): %v", path, err)
	}
	defer a.Close()
	for _, f := range files {
		got, err := a.Read(f.Name)
		if err != nil {
			t.Fatalf("Read(%q): %v", f.Name, err)
		}
		if !bytes.Equal(got, f.Data) {
			t.Fatalf("%s: on-disk round-trip mismatch", f.Name)
		}
	}
}

// TestWriteFilePreservingHeader confirms a prepended HM3W preheader is
// preserved AND the reader auto-detects it and still extracts every file.
func TestWriteFilePreservingHeader(t *testing.T) {
	files := sampleFiles(t)
	dir := t.TempDir()
	path := filepath.Join(dir, "with-hm3w.w3x")

	// Build a fake 512-byte HM3W preheader.
	pre := make([]byte, hm3wHeaderSize)
	copy(pre[0:4], magicHM3W[:])
	copy(pre[8:], []byte("Fake Map Name"))

	if err := WriteFile(path, files, pre); err != nil {
		t.Fatalf("WriteFile with preheader: %v", err)
	}

	raw, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read back: %v", err)
	}
	if !bytes.Equal(raw[0:4], magicHM3W[:]) {
		t.Fatalf("preheader not preserved; first 4 bytes = % x", raw[0:4])
	}

	a, err := Open(path)
	if err != nil {
		t.Fatalf("Open(%q) with HM3W: %v", path, err)
	}
	defer a.Close()
	for _, f := range files {
		got, err := a.Read(f.Name)
		if err != nil {
			t.Fatalf("Read(%q): %v", f.Name, err)
		}
		if !bytes.Equal(got, f.Data) {
			t.Fatalf("%s: round-trip mismatch (HM3W variant)", f.Name)
		}
	}
}

// TestBuildRejectsDuplicates confirms case-insensitive duplicate names are
// rejected rather than silently producing an ambiguous archive. The two names
// fold to the same hash key (case-only difference; the separator is identical).
func TestBuildRejectsDuplicates(t *testing.T) {
	_, err := Build([]FileEntry{
		{Name: `war3map\foo`, Data: []byte("a")},
		{Name: `WAR3MAP/FOO`, Data: []byte("b")}, // same after fold+slash-normalise
	})
	if err == nil {
		t.Fatal("expected duplicate-name rejection, got nil")
	}
}

// TestBuildSkipsCallerListfile confirms a caller-supplied (listfile) is ignored
// in favour of the generated one (no duplicate-name error, exactly one entry).
// We pad with a >512-byte payload so the produced archive clears the reader's
// minimum-size floor (OpenReader rejects anything smaller than an HM3W block).
func TestBuildSkipsCallerListfile(t *testing.T) {
	archive, err := Build([]FileEntry{
		{Name: "war3map.w3i", Data: bytes.Repeat([]byte("data"), 200)},
		{Name: "(listfile)", Data: []byte("bogus\r\n")},
	})
	if err != nil {
		t.Fatalf("Build: %v", err)
	}
	a, err := OpenReader(bytes.NewReader(archive), int64(len(archive)))
	if err != nil {
		t.Fatalf("OpenReader: %v", err)
	}
	defer a.Close()
	lf, err := a.Read(listfileName)
	if err != nil {
		t.Fatalf("Read listfile: %v", err)
	}
	if bytes.Contains(lf, []byte("bogus")) {
		t.Errorf("generated listfile leaked caller content: %s", lf)
	}
	if !bytes.Contains(lf, []byte("war3map.w3i")) {
		t.Errorf("generated listfile missing real file: %s", lf)
	}
}
