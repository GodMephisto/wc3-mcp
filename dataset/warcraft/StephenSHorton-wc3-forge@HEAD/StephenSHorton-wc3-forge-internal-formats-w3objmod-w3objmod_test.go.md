---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3objmod/w3objmod_test.go"
ref: HEAD
---

package w3objmod

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"
)

// TestRoundTrip_RealFixtures asserts byte-for-byte Parse → Encode equality on
// real, committed object-modification tables lifted from the user's GPL
// survival map. This is the losslessness proof that lets Session.Save write
// these files back without corrupting a custom unit/destructible/etc.
//
// Two wire shapes are covered:
//   - war3map.w3b (destructibles) — the opt=false form (no per-mod level slots).
//     This fixture carries a real original-table edit ("LTbr" overriding "bfil"
//     = "Crystal"), exercising the non-opt object/modification/string path.
//   - war3map.w3a (abilities) — the opt=true form (per-mod level/dataPointer
//     slots). Survival's is an empty table (version 3, 0 originals, 0 customs),
//     which guards the opt-format header round-trip.
//
// All seven object-data kinds share these two wire shapes — w3b/w3u/w3t/w3h are
// opt=false, w3d/w3a/w3q are opt=true — so a real fixture for each shape is the
// byte-equal proof for all of them. (The synthetic tests in
// w3objmod_encode_test.go cover the int/float/string + leveled-field paths.)
//
// Before this fixture existed the only file-based test (TestParseEnfos) Skipf'd
// on an out-of-repo temp directory, so a clean checkout / CI never ran a
// real-file round-trip on object data.
func TestRoundTrip_RealFixtures(t *testing.T) {
	cases := []struct {
		name string
		file string
		opt  bool
	}{
		{"destructibles_w3b", "wc3_survival_v1_6.w3b", false},
		{"abilities_w3a", "wc3_survival_v1_6.w3a", true},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			path := filepath.Join("testdata", c.file)
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture %q: %v", path, err)
			}
			// nil FieldMap: Overrides keys stay as raw FourCCs, so Encode
			// reproduces the exact on-wire modification IDs (this is the same
			// shape Session carries object mods in).
			f, err := Parse(orig, c.opt, nil)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			out, err := Encode(f, c.opt, nil)
			if err != nil {
				t.Fatalf("Encode: %v", err)
			}
			if !bytes.Equal(orig, out) {
				n := len(orig)
				if len(out) < n {
					n = len(out)
				}
				div := -1
				for i := 0; i < n; i++ {
					if orig[i] != out[i] {
						div = i
						break
					}
				}
				t.Fatalf("round-trip not byte-equal: orig=%d bytes, out=%d bytes, first diff @ byte %d",
					len(orig), len(out), div)
			}
			t.Logf("%s v=%d: %d original edits, %d customs (byte-equal round-trip OK)",
				c.name, f.Version, len(f.OriginalEdits), len(f.Customs))
		})
	}
}
