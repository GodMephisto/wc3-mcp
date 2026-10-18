---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/write_lossless_test.go"
ref: HEAD
---

package mpq

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"
)

// elementTDPath is a real 240-file custom map with unlisted custom imports and
// FLAG_ENCRYPTED + FLAG_KEY_ADJUSTED blocks — the exact structure the lossless
// repack must preserve. Tests skip cleanly when the fixture isn't present.
const elementTDPath = `C:\Users\4step\Documents\Warcraft III\Maps\Download\Element TD 4.3.w3x`

// openElementTD opens the fixture or skips. Returns the archive (caller closes).
func openElementTD(t *testing.T) *Archive {
	t.Helper()
	if _, err := os.Stat(elementTDPath); err != nil {
		t.Skipf("fixture %q not available: %v", elementTDPath, err)
	}
	a, err := Open(elementTDPath)
	if err != nil {
		t.Fatalf("Open(%q): %v", elementTDPath, err)
	}
	return a
}

// readPreheader reads the original archive's preheader bytes (HM3W block).
func readPreheader(t *testing.T, a *Archive, path string) []byte {
	t.Helper()
	off := a.ArchiveOffset()
	if off == 0 {
		return nil
	}
	raw, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read preheader: %v", err)
	}
	return append([]byte(nil), raw[:off]...)
}

// TestLossless_CopyThroughElementTD is the core correctness gate: copy EVERY
// present block of the real 240-file Element TD map through the lossless writer
// verbatim (no edits), reopen, and assert:
//
//	(a) PresentFileCount is preserved (240),
//	(b) every standard file (war3map.j/.w3i/.w3e/...) reads back byte-identical,
//	(c) the (listfile) reads back (regenerated, present).
//
// This exercises the unnamed-custom-import preservation AND the key-adjusted
// offset-pinning path (Element TD has a key-adjusted unnamed block).
func TestLossless_CopyThroughElementTD(t *testing.T) {
	a := openElementTD(t)
	defer a.Close()

	origPresent := a.PresentFileCount()
	if origPresent < 200 {
		t.Fatalf("fixture present-count %d unexpectedly low; wrong map?", origPresent)
	}
	rawEntries, err := a.RawEntries()
	if err != nil {
		t.Fatalf("RawEntries: %v", err)
	}
	if len(rawEntries) != origPresent {
		t.Fatalf("RawEntries len %d != PresentFileCount %d", len(rawEntries), origPresent)
	}

	// Snapshot the bytes of every NAMED standard file from the original, to
	// compare after the round trip.
	stdNames := []string{"war3map.j", "war3map.w3i", "war3map.w3e", "war3map.wpm",
		"war3map.shd", "war3map.doo", "war3mapMap.blp", "war3map.wts"}
	orig := map[string][]byte{}
	for _, n := range stdNames {
		if a.Has(n) {
			b, err := a.Read(n)
			if err != nil {
				t.Fatalf("orig Read(%q): %v", n, err)
			}
			orig[n] = b
		}
	}

	pre := readPreheader(t, a, elementTDPath)

	// Copy EVERY entry through verbatim. We must NOT keep the source's
	// (listfile) raw entry: the writer regenerates one, so a copied-through
	// listfile would collide on slot. Identify the listfile by its name-hashes
	// and drop it from keep (it'll be regenerated as a named entry only if we
	// pass it — which we don't; List() falls back to the standard set).
	_, lfA, lfB := HashName(listfileName)
	keep := make([]RawEntry, 0, len(rawEntries))
	for _, re := range rawEntries {
		if re.HashA == lfA && re.HashB == lfB {
			continue // drop the source listfile; writer regenerates
		}
		keep = append(keep, re)
	}

	tmp := filepath.Join(t.TempDir(), "elementtd-copy.w3x")
	if err := WriteFileLossless(tmp, keep, nil, a.HashTableSize(), a.SectorSizeShift(), true, pre); err != nil {
		t.Fatalf("WriteFileLossless: %v", err)
	}

	b, err := Open(tmp)
	if err != nil {
		t.Fatalf("Open(copy): %v", err)
	}
	defer b.Close()

	// (a) present count preserved. We dropped 1 (source listfile) and the
	// writer added 1 (regenerated listfile) -> same count.
	if got := b.PresentFileCount(); got != origPresent {
		t.Errorf("PresentFileCount after copy = %d, want %d", got, origPresent)
	}

	// (b) standard named files byte-identical.
	for n, want := range orig {
		if !b.Has(n) {
			t.Errorf("copy is missing %q", n)
			continue
		}
		got, err := b.Read(n)
		if err != nil {
			t.Errorf("copy Read(%q): %v", n, err)
			continue
		}
		if !bytes.Equal(got, want) {
			idx, ctx := firstDiff(got, want)
			t.Errorf("%s: not byte-identical after copy (diff at %d: %s; got=%d want=%d)", n, idx, ctx, len(got), len(want))
		}
	}

	// (c) regenerated listfile present.
	if !b.Has(listfileName) {
		t.Errorf("copy missing regenerated %s", listfileName)
	}
}

// TestLossless_UnnamedCustomImportByteIdentical proves an UNLISTED custom file
// (one whose name List() does not know) survives the copy-through byte-for-byte.
// We locate such an entry by hash (any present entry whose hashes don't match a
// standard name) and compare its raw stored bytes before/after.
func TestLossless_UnnamedCustomImportByteIdentical(t *testing.T) {
	a := openElementTD(t)
	defer a.Close()

	rawEntries, err := a.RawEntries()
	if err != nil {
		t.Fatalf("RawEntries: %v", err)
	}

	// Build the set of hashes that ARE named (standard set + listfile).
	named := map[[2]uint32]bool{}
	for _, n := range append(append([]string{}, standardWC3Files...), listfileName) {
		_, hA, hB := HashName(n)
		named[[2]uint32{hA, hB}] = true
	}

	// Pick the FIRST present entry that is NOT named and has non-empty data —
	// that's a genuine unlisted custom import.
	var target RawEntry
	found := false
	for _, re := range rawEntries {
		if named[[2]uint32{re.HashA, re.HashB}] {
			continue
		}
		if len(re.Raw) == 0 {
			continue
		}
		target = re
		found = true
		break
	}
	if !found {
		t.Skip("no unlisted custom import found in fixture (unexpected for Element TD)")
	}
	t.Logf("unnamed custom: hashA=%08x hashB=%08x flags=%08x cSize=%d uSize=%d pinned=%v",
		target.HashA, target.HashB, target.Flags, len(target.Raw), target.USize, target.NeedsName)

	wantRaw := append([]byte(nil), target.Raw...)

	// Copy everything through (drop source listfile to avoid slot collision).
	_, lfA, lfB := HashName(listfileName)
	keep := make([]RawEntry, 0, len(rawEntries))
	for _, re := range rawEntries {
		if re.HashA == lfA && re.HashB == lfB {
			continue
		}
		keep = append(keep, re)
	}
	pre := readPreheader(t, a, elementTDPath)
	tmp := filepath.Join(t.TempDir(), "copy2.w3x")
	if err := WriteFileLossless(tmp, keep, nil, a.HashTableSize(), a.SectorSizeShift(), true, pre); err != nil {
		t.Fatalf("WriteFileLossless: %v", err)
	}

	b, err := Open(tmp)
	if err != nil {
		t.Fatalf("Open(copy): %v", err)
	}
	defer b.Close()

	// Find the same entry by hash in the rewritten archive and compare raw bytes.
	reb, err := b.RawEntries()
	if err != nil {
		t.Fatalf("reopened RawEntries: %v", err)
	}
	var gotRaw []byte
	for _, re := range reb {
		if re.HashA == target.HashA && re.HashB == target.HashB {
			gotRaw = re.Raw
			break
		}
	}
	if gotRaw == nil {
		t.Fatal("unnamed custom import disappeared after copy-through")
	}
	if !bytes.Equal(gotRaw, wantRaw) {
		idx, ctx := firstDiff(gotRaw, wantRaw)
		t.Errorf("unnamed custom import not byte-identical (diff at %d: %s; got=%d want=%d)",
			idx, ctx, len(gotRaw), len(wantRaw))
	}
}

// TestLossless_EditOneFilePreservesRest copies Element TD through while REPLACING
// war3map.w3i with new bytes, then asserts: the edit took effect, the present
// count is unchanged, and a sampled unlisted custom import is still byte-
// identical. This mirrors the forge-level repack-with-one-edit flow at the raw
// layer.
func TestLossless_EditOneFilePreservesRest(t *testing.T) {
	a := openElementTD(t)
	defer a.Close()

	origPresent := a.PresentFileCount()
	rawEntries, err := a.RawEntries()
	if err != nil {
		t.Fatalf("RawEntries: %v", err)
	}

	// Snapshot an unlisted custom import (by hash) for the after-comparison.
	named := map[[2]uint32]bool{}
	for _, n := range append(append([]string{}, standardWC3Files...), listfileName) {
		_, hA, hB := HashName(n)
		named[[2]uint32{hA, hB}] = true
	}
	var customHashes [2]uint32
	var customWant []byte
	for _, re := range rawEntries {
		if !named[[2]uint32{re.HashA, re.HashB}] && len(re.Raw) > 0 {
			customHashes = [2]uint32{re.HashA, re.HashB}
			customWant = append([]byte(nil), re.Raw...)
			break
		}
	}

	// Drop the entry we're replacing (war3map.w3i) and the source listfile from
	// keep; everything else copies through verbatim.
	_, w3iA, w3iB := HashName("war3map.w3i")
	_, lfA, lfB := HashName(listfileName)
	keep := make([]RawEntry, 0, len(rawEntries))
	for _, re := range rawEntries {
		if re.HashA == w3iA && re.HashB == w3iB {
			continue
		}
		if re.HashA == lfA && re.HashB == lfB {
			continue
		}
		keep = append(keep, re)
	}

	newW3I := []byte("BRAND NEW W3I BYTES \x00\x01\x02 for the edit test")
	named2 := []FileEntry{{Name: "war3map.w3i", Data: newW3I}}

	pre := readPreheader(t, a, elementTDPath)
	tmp := filepath.Join(t.TempDir(), "edited.w3x")
	if err := WriteFileLossless(tmp, keep, named2, a.HashTableSize(), a.SectorSizeShift(), true, pre); err != nil {
		t.Fatalf("WriteFileLossless: %v", err)
	}

	b, err := Open(tmp)
	if err != nil {
		t.Fatalf("Open(edited): %v", err)
	}
	defer b.Close()

	// Present count unchanged: we removed w3i + listfile (2) and added w3i +
	// regenerated listfile (2).
	if got := b.PresentFileCount(); got != origPresent {
		t.Errorf("PresentFileCount after edit = %d, want %d", got, origPresent)
	}

	// The edit took effect.
	gotW3I, err := b.Read("war3map.w3i")
	if err != nil {
		t.Fatalf("Read(war3map.w3i): %v", err)
	}
	if !bytes.Equal(gotW3I, newW3I) {
		t.Errorf("war3map.w3i not updated: got %q", gotW3I)
	}

	// The sampled custom import is still byte-identical.
	if customWant != nil {
		reb, _ := b.RawEntries()
		var gotCustom []byte
		for _, re := range reb {
			if re.HashA == customHashes[0] && re.HashB == customHashes[1] {
				gotCustom = re.Raw
				break
			}
		}
		if gotCustom == nil {
			t.Error("custom import lost after edit")
		} else if !bytes.Equal(gotCustom, customWant) {
			t.Error("custom import changed after editing an unrelated file")
		}
	}
}

// TestLossless_SynthRoundTrip is a fast no-fixture test: build a tiny lossless
// archive from named entries only (no keep), round-trip it, confirm reads match.
func TestLossless_SynthRoundTrip(t *testing.T) {
	named := []FileEntry{
		{Name: "war3map.w3i", Data: bytes.Repeat([]byte("info"), 100)},
		{Name: "war3map.j", Data: []byte("function main\nendfunction")},
		{Name: "war3mapImported\\custom.mdx", Data: []byte{0x4d, 0x44, 0x4c, 0x58, 0x01}},
		{Name: "empty.bin", Data: []byte{}},
	}
	body, err := BuildLossless(nil, named, 0, 0, true)
	if err != nil {
		t.Fatalf("BuildLossless: %v", err)
	}
	a, err := OpenReader(bytes.NewReader(body), int64(len(body)))
	if err != nil {
		t.Fatalf("OpenReader: %v", err)
	}
	defer a.Close()
	for _, f := range named {
		got, err := a.Read(f.Name)
		if err != nil {
			t.Fatalf("Read(%q): %v", f.Name, err)
		}
		if !bytes.Equal(got, f.Data) {
			t.Errorf("%s: round-trip mismatch", f.Name)
		}
	}
}
