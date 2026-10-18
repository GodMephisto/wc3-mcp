---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/session_open_safety_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
)

// writeValidW3I drops a minimal-but-valid war3map.w3i into dir so a folder
// Session.Open succeeds. Mirrors importFixtureMap's builder in
// session_import_test.go — FileVersionTFT (v25) with just Name/Author/Tileset
// Encodes to a file w3i.Parse round-trips.
func writeValidW3I(t *testing.T, dir string) {
	t.Helper()
	info := &w3i.Info{
		FileVersion: w3i.FileVersionTFT,
		MapVersion:  1,
		Name:        "OpenSafetyFixture",
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
}

// TestOpen_DistinguishesAbsentFromCorrupt covers the Phase-0 "double-clicking a
// corrupt map shows a reason, not a blank window" bar at the error-message
// level: each failure mode Open can hit must return a DISTINCT, user-meaningful
// error so the --open/file-association toast can tell them apart. (The toast
// itself forwards Open's message verbatim — see main.go startupOpenError.)
func TestOpen_DistinguishesAbsentFromCorrupt(t *testing.T) {
	t.Run("nonexistent path", func(t *testing.T) {
		s := &Session{}
		err := s.Open(filepath.Join(t.TempDir(), "does-not-exist.w3x"))
		if err == nil {
			t.Fatalf("expected error opening a nonexistent path, got nil")
		}
		// stat failure — the "absent" case a user gets when a file-association
		// target was moved/deleted.
		if !strings.Contains(err.Error(), "stat ") {
			t.Errorf("nonexistent path should surface a stat error, got: %v", err)
		}
		if s.IsLoaded() {
			t.Errorf("session must not be loaded after a failed Open")
		}
	})

	t.Run("folder without war3map.w3i", func(t *testing.T) {
		dir := t.TempDir()
		// A real directory, but not a map — the "absent" case for someone who
		// pointed Open at the wrong folder.
		if err := os.WriteFile(filepath.Join(dir, "readme.txt"), []byte("not a map"), 0o644); err != nil {
			t.Fatalf("write decoy file: %v", err)
		}
		s := &Session{}
		err := s.Open(dir)
		if err == nil {
			t.Fatalf("expected error opening a w3i-less folder, got nil")
		}
		if !strings.Contains(err.Error(), "has no war3map.w3i") {
			t.Errorf("w3i-less folder should say so, got: %v", err)
		}
	})

	t.Run("corrupt war3map.w3i", func(t *testing.T) {
		dir := t.TempDir()
		// Present but unparseable: a too-short / garbage w3i. This is the
		// "corrupt" case, which must be distinguishable from the "absent" cases
		// above — Open wraps the parse failure as "parse war3map.w3i: …".
		if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), []byte{0x01, 0x02}, 0o644); err != nil {
			t.Fatalf("write corrupt w3i: %v", err)
		}
		s := &Session{}
		err := s.Open(dir)
		if err == nil {
			t.Fatalf("expected error opening a corrupt w3i, got nil")
		}
		if !strings.Contains(err.Error(), "parse war3map.w3i") {
			t.Errorf("corrupt w3i should surface a parse error, got: %v", err)
		}
		if s.IsLoaded() {
			t.Errorf("session must not be loaded after a failed Open")
		}
	})

	t.Run("valid map opens clean", func(t *testing.T) {
		dir := t.TempDir()
		writeValidW3I(t, dir)
		s := &Session{}
		if err := s.Open(dir); err != nil {
			t.Fatalf("valid map should open, got: %v", err)
		}
		if !s.IsLoaded() {
			t.Errorf("session should be loaded after a successful Open")
		}
	})
}

// TestOpen_RecoversParserPanic proves the Phase-0 guarantee that a parser
// panic on the GUI / --open / new-window load path becomes a returned error
// ("map appears corrupt or unsupported: …") instead of vanishing the window.
//
// The production format parsers are all bounds-checked today (truncated input
// returns errors, not panics), so we can't reliably craft a malformed *file*
// that panics through Session.Open — the recover() is defense-in-depth for the
// remaining unguarded count-bearing parsers and any future regression. To
// exercise the wrapper deterministically we drive Open through openWithSource,
// the same body Session.Open runs, with a fileSource whose read panics. This
// asserts the recover converts the panic to the contract error AND that the
// session is left unloaded (not half-swapped) afterwards.
func TestOpen_RecoversParserPanic(t *testing.T) {
	s := &Session{}
	err := s.openWithSource("panicfixture.w3x", panicSource{}, nil, "folder")
	if err == nil {
		t.Fatalf("expected a recovered error from a panicking source, got nil")
	}
	if !strings.Contains(err.Error(), "map appears corrupt or unsupported") {
		t.Errorf("recovered panic should surface the corrupt/unsupported message, got: %v", err)
	}
	if !strings.Contains(err.Error(), "boom") {
		t.Errorf("recovered error should carry the panic detail, got: %v", err)
	}
	if s.IsLoaded() {
		t.Errorf("session must not be loaded after a recovered Open panic")
	}
}

// panicSource is a fileSource whose first read panics — standing in for a
// parser blowing up on a corrupt map inside Session.Open. Used only by
// TestOpen_RecoversParserPanic.
type panicSource struct{}

func (panicSource) read(string) ([]byte, bool, error) { panic("boom: simulated parser panic") }
func (panicSource) write(string, []byte) error        { return nil }
func (panicSource) delete(string) error               { return nil }
func (panicSource) flush() error                      { return nil }
func (panicSource) close() error                      { return nil }
