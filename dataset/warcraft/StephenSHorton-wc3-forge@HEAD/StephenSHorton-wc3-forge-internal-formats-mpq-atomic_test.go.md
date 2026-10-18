---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/atomic_test.go"
ref: HEAD
---

package mpq

import (
	"bytes"
	"os"
	"path/filepath"
	"strings"
	"testing"
)

// TestWriteFileAtomic_OverwriteAndNoResidue asserts the exported single-file
// atomic writer replaces an existing file with new (different-length) content
// and leaves no .tmp residue behind — the primitive the forge folder-save path
// reuses instead of a truncating os.WriteFile.
func TestWriteFileAtomic_OverwriteAndNoResidue(t *testing.T) {
	dir := t.TempDir()
	path := filepath.Join(dir, "war3map.w3e")

	if err := WriteFileAtomic(path, bytes.Repeat([]byte("A"), 1000)); err != nil {
		t.Fatalf("initial write: %v", err)
	}
	want := bytes.Repeat([]byte("B"), 10)
	if err := WriteFileAtomic(path, want); err != nil {
		t.Fatalf("overwrite: %v", err)
	}
	got, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read back: %v", err)
	}
	if !bytes.Equal(got, want) {
		t.Errorf("content mismatch: got %d bytes, want %d", len(got), len(want))
	}

	// No leftover temp file from the temp+rename dance.
	entries, err := os.ReadDir(dir)
	if err != nil {
		t.Fatalf("read dir: %v", err)
	}
	for _, e := range entries {
		if strings.HasSuffix(e.Name(), ".tmp") {
			t.Errorf("leaked temp file: %s", e.Name())
		}
	}
}
