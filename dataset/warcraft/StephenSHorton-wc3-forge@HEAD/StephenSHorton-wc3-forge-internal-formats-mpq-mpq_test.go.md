---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/mpq_test.go"
ref: HEAD
---

package mpq

import (
	"crypto/sha256"
	"encoding/hex"
	"os"
	"testing"
)

// Test fixtures: real .w3x maps + a directory of pre-extracted files
// (produced by HiveWE's scripts/extract_stormlib.py against the same
// .w3x via StormLib). Our reader's output must be byte-for-byte
// identical to StormLib's.
const (
	fixtureEnfo            = `C:\Users\4step\Documents\Warcraft III\Maps\Download\Enfo FFB - v2.64f.w3x`
	fixtureEnfoExtracted   = `C:\Users\4step\maps-extracted\enfo-v2.64f`
	fixtureSurvival        = `C:\Users\4step\projects\wc3-survival-game\build\wc3-survival-v1.6.w3x`
	fixtureSurvivalExtract = `C:\Users\4step\projects\wc3-survival-game\map\extracted`
)

func sha256Hex(b []byte) string {
	sum := sha256.Sum256(b)
	return hex.EncodeToString(sum[:])
}

// firstDiff returns the byte index of the first divergence and a small
// hex window around it, or (-1, "") if a and b are equal. Used for
// loud failure messages on extraction mismatch.
func firstDiff(a, b []byte) (int, string) {
	n := len(a)
	if len(b) < n {
		n = len(b)
	}
	for i := 0; i < n; i++ {
		if a[i] != b[i] {
			lo := i - 8
			if lo < 0 {
				lo = 0
			}
			hi := i + 8
			if hi > n {
				hi = n
			}
			return i, "got=" + hex.EncodeToString(a[lo:hi]) + " want=" + hex.EncodeToString(b[lo:hi])
		}
	}
	if len(a) != len(b) {
		return n, "length differs"
	}
	return -1, ""
}

// needFixture skips the calling test if the given path doesn't exist.
// The MPQ tests run against personal fixtures (real .w3x files the
// author has locally + StormLib-pre-extracted siblings to byte-compare
// against) that aren't and shouldn't be checked into the repo. Without
// this guard, anyone checking out the repo on a fresh box — macOS, a
// new Windows machine, CI — hits hardcoded-path failures instead of a
// clean skip.
func needFixture(t *testing.T, path string) {
	t.Helper()
	if _, err := os.Stat(path); err != nil {
		t.Skipf("fixture not present at %q (set up locally; not checked in)", path)
	}
}

func TestOpenEnfo(t *testing.T) {
	needFixture(t, fixtureEnfo)
	a, err := Open(fixtureEnfo)
	if err != nil {
		t.Fatalf("Open(%s): %v", fixtureEnfo, err)
	}
	defer a.Close()

	// A handful of files that should exist in any sane WC3 map.
	for _, name := range []string{
		"war3map.w3i",
		"war3map.w3e",
		"war3map.wts",
		"war3mapUnits.doo",
	} {
		if !a.Has(name) {
			t.Errorf("Has(%q) = false; expected true", name)
		}
	}

	// Forward-slash equivalence.
	if !a.Has("war3map/w3i") && a.Has("war3map.w3i") {
		// Cosmetic: this isn't a real WC3 path, just confirming our
		// normalizePath doesn't accidentally substring-match. Skip.
		t.Logf("forward-slash variant 'war3map/w3i' correctly not found")
	}
}

func TestExtractMatchesStormLib_w3i(t *testing.T) {
	needFixture(t, fixtureEnfo)
	needFixture(t, fixtureEnfoExtracted)
	a, err := Open(fixtureEnfo)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer a.Close()

	for _, name := range []string{
		"war3map.w3i",
		"war3mapUnits.doo",
		"war3map.w3e",
		"war3map.wts",
	} {
		t.Run(name, func(t *testing.T) {
			got, err := a.Read(name)
			if err != nil {
				t.Fatalf("Read(%q): %v", name, err)
			}
			ref, err := os.ReadFile(fixtureEnfoExtracted + `\` + name)
			if err != nil {
				t.Fatalf("read reference: %v", err)
			}
			if sha256Hex(got) != sha256Hex(ref) {
				idx, ctx := firstDiff(got, ref)
				t.Fatalf("%s: sha256 mismatch (got=%s want=%s, first diff at byte %d: %s, sizes got=%d want=%d)",
					name, sha256Hex(got), sha256Hex(ref), idx, ctx, len(got), len(ref))
			}
		})
	}
}

func TestExtractMatchesStormLib_survival(t *testing.T) {
	needFixture(t, fixtureSurvival)
	needFixture(t, fixtureSurvivalExtract)
	a, err := Open(fixtureSurvival)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer a.Close()

	for _, name := range []string{
		"war3map.w3i",
		"war3mapUnits.doo",
		"war3map.w3e",
		"war3map.lua",
	} {
		t.Run(name, func(t *testing.T) {
			got, err := a.Read(name)
			if err != nil {
				t.Fatalf("Read(%q): %v", name, err)
			}
			ref, err := os.ReadFile(fixtureSurvivalExtract + `\` + name)
			if err != nil {
				t.Fatalf("read reference: %v", err)
			}
			if sha256Hex(got) != sha256Hex(ref) {
				idx, ctx := firstDiff(got, ref)
				t.Fatalf("%s: sha256 mismatch (got=%s want=%s, first diff at byte %d: %s, sizes got=%d want=%d)",
					name, sha256Hex(got), sha256Hex(ref), idx, ctx, len(got), len(ref))
			}
		})
	}
}

func TestList(t *testing.T) {
	needFixture(t, fixtureEnfo)
	a, err := Open(fixtureEnfo)
	if err != nil {
		t.Fatalf("Open: %v", err)
	}
	defer a.Close()

	names := a.List()
	if len(names) == 0 {
		t.Fatal("List() returned 0 names")
	}

	want := []string{"war3map.w3i", "war3map.w3e", "war3map.wts", "war3mapUnits.doo"}
	set := make(map[string]bool)
	for _, n := range names {
		set[n] = true
	}
	for _, w := range want {
		if !set[w] {
			t.Errorf("List() missing %q (got %d entries: %v)", w, len(names), names)
		}
	}
	t.Logf("List() = %d files: %v", len(names), names)
}
