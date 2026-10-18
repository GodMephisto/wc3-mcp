---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3i/w3i_test.go"
ref: HEAD
---

package w3i

import (
	"bytes"
	"encoding/json"
	"os"
	"path/filepath"
	"testing"
)

// Fixtures live under testdata/. Both are real extracted war3map.w3i bytes:
//
//   - wc3_survival_v1_6.w3i — Reforged Lua, file_version 33, 309 B.
//   - enfo_ffb_v2_64f.w3i   — Reforged JASS,  file_version 31, 906 B.
//     Exercises the v31 codepath (no min cam distance, includes enemy
//     priorities for players, has random-unit/item tables).
//
// These are vendored into the repo (kilobytes each) to keep tests
// hermetic — the larger fixture maps move around as siblings of this
// repo and absolute paths used to drift.
var (
	fixturePathV1_6 = filepath.Join("testdata", "wc3_survival_v1_6.w3i")
	fixturePathEnfo = filepath.Join("testdata", "enfo_ffb_v2_64f.w3i")
)

func TestParse_v1_6(t *testing.T) {
	data, err := os.ReadFile(fixturePathV1_6)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}

	info, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	if info.FileVersion < FileVersionTFT {
		t.Errorf("file_version %d unexpectedly low — fixture should be a modern Reforged map", info.FileVersion)
	}
	if info.Name == "" {
		t.Errorf("map name is empty")
	}
	if info.Author == "" {
		t.Errorf("map author is empty")
	}
	if info.PlayableWidth <= 0 || info.PlayableWidth >= 1024 {
		t.Errorf("playable width %d outside sane bounds (0,1024)", info.PlayableWidth)
	}
	if info.PlayableHeight <= 0 || info.PlayableHeight >= 1024 {
		t.Errorf("playable height %d outside sane bounds (0,1024)", info.PlayableHeight)
	}
	if len(info.Players) < 1 {
		t.Errorf("expected at least 1 player slot, got %d", len(info.Players))
	}

	// Pretty-print for visual sanity; only emitted with -v.
	pretty, err := json.MarshalIndent(info, "", "  ")
	if err != nil {
		t.Fatalf("marshal Info: %v", err)
	}
	t.Logf("Parsed war3map.w3i (file_version %d):\n%s", info.FileVersion, pretty)
}

// TestEncodeRoundTrip asserts that Encode is the byte-faithful inverse of
// Parse for real-world fixtures, mirroring the unitsdoo / doodadsdoo round-
// trip contract. This is the load-bearing test for the save path: every
// dirty-info Save flush goes Parse -> MutateInfo -> Encode -> disk, and any
// byte drift on the round trip would corrupt fields the editor doesn't
// surface (random-unit tables, prologue strings, force flags, etc.).
//
// Round-trip operates on RAW Parse output -- we do NOT call ResolveStrings
// here because that mutates Info.Name etc. in place from TRIGSTR_<n> to
// the resolved display string, breaking byte equality. The Session.Save
// path applies the same constraint by saving from the in-memory Info;
// today that Info IS post-ResolveStrings (which means any save will
// linearize TRIGSTR refs into literals -- see Option A note in
// encode.go). The test asserts the encoder itself is correct; the
// TRIGSTR collapse is a separate Session-level concern.
func TestEncodeRoundTrip(t *testing.T) {
	cases := []string{fixturePathV1_6, fixturePathEnfo}
	for _, path := range cases {
		t.Run(filepath.Base(path), func(t *testing.T) {
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture: %v", err)
			}
			info, err := Parse(orig)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			out, err := Encode(info)
			if err != nil {
				t.Fatalf("Encode: %v", err)
			}
			if !bytes.Equal(orig, out) {
				div := firstDiff(orig, out)
				lo := div - 16
				if lo < 0 {
					lo = 0
				}
				hiOrig := div + 32
				if hiOrig > len(orig) {
					hiOrig = len(orig)
				}
				hiOut := div + 32
				if hiOut > len(out) {
					hiOut = len(out)
				}
				t.Fatalf("Encode output != original\n"+
					"  sizes orig=%d encoded=%d\n"+
					"  first diff at offset %d\n"+
					"  orig[%d:%d] = % x\n"+
					"  out [%d:%d] = % x",
					len(orig), len(out), div,
					lo, hiOrig, orig[lo:hiOrig],
					lo, hiOut, out[lo:hiOut])
			}
		})
	}
}

func firstDiff(a, b []byte) int {
	n := len(a)
	if len(b) < n {
		n = len(b)
	}
	for i := 0; i < n; i++ {
		if a[i] != b[i] {
			return i
		}
	}
	return n
}
