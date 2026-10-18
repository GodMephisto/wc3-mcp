---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/package_folder_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// TestPackageFolderToW3X packs a tiny folder-backed session into a .w3x and
// asserts: the file starts with the HM3W magic, the embedded MPQ is openable,
// and the packaged files round-trip byte-for-byte (including a nested path).
func TestPackageFolderToW3X(t *testing.T) {
	root := t.TempDir()

	files := map[string]string{
		"war3map.lua":           "-- hand-authored\nprint('hi')\n",
		`war3mapImported\foo.txt`: "imported payload",
	}
	for name, content := range files {
		p := filepath.Join(root, filepath.FromSlash(name))
		if err := os.MkdirAll(filepath.Dir(p), 0o755); err != nil {
			t.Fatalf("mkdir for %q: %v", name, err)
		}
		if err := os.WriteFile(p, []byte(content), 0o644); err != nil {
			t.Fatalf("write %q: %v", name, err)
		}
	}

	// Minimal folder-backed session.
	s := &Session{
		loaded: true,
		path:   root,
		source: folderSource{root: root},
		info:   &w3i.Info{Name: "Test Map: Round/Trip", Players: make([]w3i.Player, 4)},
	}

	out := filepath.Join(filepath.Dir(root), "out", sanitizeMapFileName(s.info.Name)+".w3x")
	if err := s.PackageFolderToW3X(out); err != nil {
		t.Fatalf("PackageFolderToW3X: %v", err)
	}

	raw, err := os.ReadFile(out)
	if err != nil {
		t.Fatalf("read packaged .w3x: %v", err)
	}
	if !bytes.HasPrefix(raw, []byte("HM3W")) {
		t.Fatalf("packaged file does not start with HM3W magic; got %q", raw[:4])
	}

	a, err := mpq.Open(out)
	if err != nil {
		t.Fatalf("mpq.Open packaged .w3x: %v", err)
	}
	defer a.Close()

	for name, want := range files {
		got, err := a.Read(name)
		if err != nil {
			t.Fatalf("read %q from packaged archive: %v", name, err)
		}
		if string(got) != want {
			t.Errorf("file %q round-trip mismatch:\n got %q\nwant %q", name, got, want)
		}
	}
}

// TestPackageFolderToW3X_RejectsNonFolder ensures an MPQ-backed (or unloaded)
// session is refused rather than producing a bogus archive.
func TestPackageFolderToW3X_RejectsNonFolder(t *testing.T) {
	s := &Session{loaded: false}
	if err := s.PackageFolderToW3X(filepath.Join(t.TempDir(), "x.w3x")); err == nil {
		t.Fatal("expected error for unloaded session, got nil")
	}

	s2 := &Session{loaded: true, source: &mpqSource{}}
	if err := s2.PackageFolderToW3X(filepath.Join(t.TempDir(), "x.w3x")); err == nil {
		t.Fatal("expected error for MPQ-backed session, got nil")
	}
}

func TestSanitizeMapFileName(t *testing.T) {
	cases := map[string]string{
		"Plain":               "Plain",
		"With/Slash":          "With_Slash",
		`a<b>c:"d"e|f?g*h`:    "a_b_c__d_e_f_g_h",
		"   ":                 "map",
		`////`:                "____",
		"trailing.":           "trailing",
	}
	for in, want := range cases {
		if got := sanitizeMapFileName(in); got != want {
			t.Errorf("sanitizeMapFileName(%q) = %q, want %q", in, got, want)
		}
	}
}
