---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/present_count_test.go"
ref: HEAD
---

package mpq

import (
	"os"
	"path/filepath"
	"testing"
)

// TestPresentFileCount verifies the hash-table-based file count that the
// repack do-no-harm guard relies on. The writer emits a (listfile), so an
// archive built from N caller files holds N+1 present files.
func TestPresentFileCount(t *testing.T) {
	entries := []FileEntry{
		{Name: "war3map.w3i", Data: []byte("info-bytes")},
		{Name: "war3map.j", Data: []byte("function main takes nothing returns nothing\nendfunction\n")},
		{Name: "war3mapImported\\custom.mdx", Data: []byte{0x4d, 0x44, 0x4c, 0x58}},
		{Name: "war3mapImported\\custom.blp", Data: make([]byte, 4096)},
	}
	tmp := filepath.Join(t.TempDir(), "present.w3x")
	if err := WriteFile(tmp, entries, nil); err != nil {
		t.Fatalf("WriteFile: %v", err)
	}
	a, err := Open(tmp)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer a.Close()

	got := a.PresentFileCount()
	want := len(entries) + 1 // + auto-generated (listfile)
	if got != want {
		t.Fatalf("PresentFileCount = %d, want %d (%d files + listfile)", got, want, len(entries))
	}

	// Sanity: every written file is actually readable back (no phantom count).
	for _, e := range entries {
		if _, err := a.Read(e.Name); err != nil {
			t.Errorf("Read(%q) after write: %v", e.Name, err)
		}
	}
	_ = os.Remove(tmp)
}
