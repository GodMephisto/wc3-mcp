---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3e/encode_test.go"
ref: HEAD
---

package w3e

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"
)

// TestRoundTrip_Fixtures takes each committed, self-contained terrain fixture
// (both wire layouts — v12 Reforged and v11 Classic), re-encodes it, and
// asserts byte-for-byte equality. A drift of even one bit here means a tileset
// swap would corrupt the map on save. The fixtures live in testdata/ so this
// proof RUNS on a clean checkout / in CI rather than Skipf'ing on an
// out-of-repo path.
func TestRoundTrip_Fixtures(t *testing.T) {
	for _, path := range []string{fixtureV12, fixtureV11} {
		t.Run(filepath.Base(path), func(t *testing.T) {
			data, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture %q: %v", path, err)
			}

			f, err := Parse(data)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}

			encoded, err := Encode(f)
			if err != nil {
				t.Fatalf("Encode: %v", err)
			}

			if !bytes.Equal(encoded, data) {
				// Locate the first divergence so the failure is debuggable.
				n := len(data)
				if len(encoded) < n {
					n = len(encoded)
				}
				var firstDiff = -1
				for i := 0; i < n; i++ {
					if encoded[i] != data[i] {
						firstDiff = i
						break
					}
				}
				t.Fatalf("round-trip drift: encoded %d bytes, original %d bytes, first diff at byte %d",
					len(encoded), len(data), firstDiff)
			}
		})
	}
}

// TestRoundTrip_AfterSwap exercises the path that matters for tileset swap:
// rewrite GroundTilesets / CliffTilesets to a different set, remap every
// Tilepoint.GroundTexture / CliffTexture, then assert Parse(Encode(f)) sees
// exactly the post-swap state.
func TestRoundTrip_AfterSwap(t *testing.T) {
	data, err := os.ReadFile(fixtureV12)
	if err != nil {
		t.Fatalf("read fixture %q: %v", fixtureV12, err)
	}
	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	// Replace tileset palettes with a 2-entry set so the index space is tiny.
	f.Tileset = 'A'
	f.GroundTilesets = []string{"Agrs", "Adrt"}
	f.CliffTilesets = []string{"CAa1"}
	for i := range f.Tiles {
		f.Tiles[i].GroundTexture = uint8(i % 2)
		f.Tiles[i].CliffTexture = 0
	}

	encoded, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	round, err := Parse(encoded)
	if err != nil {
		t.Fatalf("Parse(Encode): %v", err)
	}

	if round.Tileset != 'A' {
		t.Errorf("Tileset = %q, want %q", string(round.Tileset), "A")
	}
	if got, want := round.GroundTilesets, []string{"Agrs", "Adrt"}; !equalStrings(got, want) {
		t.Errorf("GroundTilesets = %v, want %v", got, want)
	}
	if got, want := round.CliffTilesets, []string{"CAa1"}; !equalStrings(got, want) {
		t.Errorf("CliffTilesets = %v, want %v", got, want)
	}
	for i := range round.Tiles {
		if got, want := round.Tiles[i].GroundTexture, uint8(i%2); got != want {
			t.Fatalf("Tiles[%d].GroundTexture = %d, want %d", i, got, want)
			break
		}
	}
}

func equalStrings(a, b []string) bool {
	if len(a) != len(b) {
		return false
	}
	for i := range a {
		if a[i] != b[i] {
			return false
		}
	}
	return true
}
