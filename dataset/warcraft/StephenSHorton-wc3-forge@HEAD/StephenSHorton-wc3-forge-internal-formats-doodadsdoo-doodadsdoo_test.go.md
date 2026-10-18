---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/doodadsdoo/doodadsdoo_test.go"
ref: HEAD
---

package doodadsdoo

import (
	"bytes"
	"encoding/binary"
	"encoding/json"
	"errors"
	"io"
	"math"
	"os"
	"path/filepath"
	"strings"
	"testing"
)

// Fixture 1: wc3-survival's war3map.doo. It's a flat arena map with very
// few (likely zero) placed doodads — primarily exercises the header + empty
// trailing special-doodad section path.
var fixturePathV16 = filepath.Join("testdata", "wc3_survival_v1_6.doo")

// Fixture 2: enfos v2.64f's war3map.doo. JASS-era map packed with terrain
// decoration — should produce hundreds of doodad entries. Exercises the full
// per-doodad code path including item-drop sets and the subversion-9/11
// skin_id heuristic.
var fixturePathEnfo = filepath.Join("testdata", "enfo_ffb_v2_64f.doo")

func TestParse_v1_6(t *testing.T) {
	data, err := os.ReadFile(fixturePathV16)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}

	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	if got, want := string(f.Format[:]), "W3do"; got != want {
		t.Errorf("Format = %q, want %q", got, want)
	}
	if f.Version != 7 && f.Version != 8 {
		t.Errorf("unexpected Version %d (expected 7 or 8)", f.Version)
	}
	if f.SubVersion != 9 && f.SubVersion != 11 {
		t.Errorf("unexpected SubVersion %d (expected 9 or 11)", f.SubVersion)
	}
	if len(f.Doodads) < 0 {
		// Tautological — the >= 0 check from the spec is to assert that
		// the file parses cleanly even when empty. The real assertion is
		// just "Parse didn't error".
		t.Errorf("negative doodad count? %d", len(f.Doodads))
	}

	t.Logf("Format=%q Version=%d SubVersion=%d Doodads=%d SpecialFormat=%d SpecialDoodads=%d",
		string(f.Format[:]), f.Version, f.SubVersion, len(f.Doodads), f.SpecialFormat, len(f.SpecialDoodads))

	// Per-doodad sanity (probably zero iterations on this fixture).
	for i, d := range f.Doodads {
		validateDoodad(t, i, d)
	}

	// Sample up to 5 doodads as JSON for visual sanity. (Empty fixture →
	// SpecialDoodads only, which is still informative.)
	logSample(t, f, 5)
}

func TestParse_enfo(t *testing.T) {
	data, err := os.ReadFile(fixturePathEnfo)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}

	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}

	if got, want := string(f.Format[:]), "W3do"; got != want {
		t.Errorf("Format = %q, want %q", got, want)
	}
	if f.Version != 7 && f.Version != 8 {
		t.Errorf("unexpected Version %d (expected 7 or 8)", f.Version)
	}
	if f.SubVersion != 9 && f.SubVersion != 11 {
		t.Errorf("unexpected SubVersion %d (expected 9 or 11)", f.SubVersion)
	}

	t.Logf("Format=%q Version=%d SubVersion=%d Doodads=%d SpecialFormat=%d SpecialDoodads=%d",
		string(f.Format[:]), f.Version, f.SubVersion, len(f.Doodads), f.SpecialFormat, len(f.SpecialDoodads))

	// Enfos is heavily decorated — should easily clear 50 (likely many hundreds).
	if len(f.Doodads) <= 50 {
		t.Errorf("expected >50 doodads in enfos fixture, got %d", len(f.Doodads))
	}

	for i, d := range f.Doodads {
		validateDoodad(t, i, d)
	}

	logSample(t, f, 5)
}

// validateDoodad asserts per-entity invariants used by both fixtures.
func validateDoodad(t *testing.T, i int, d Doodad) {
	t.Helper()

	if len(d.TypeID) != 4 {
		t.Errorf("doodad[%d].TypeID = %q (len %d), want exactly 4 bytes", i, d.TypeID, len(d.TypeID))
	}
	for j, c := range []byte(d.TypeID) {
		// FourCCs are printable ASCII. Random-set placeholders use letters+digits.
		if c < 0x20 || c > 0x7E {
			t.Errorf("doodad[%d].TypeID byte %d = 0x%02x (not printable ASCII)", i, j, c)
		}
	}

	// Game-coord sanity. Doodads can sit in the unplayable border so use a
	// looser bound than units: 16384 game units gives a couple maps of slack
	// either side and still catches scrambled scale=128 garbage.
	if abs32(d.Position[0]) >= 16384 || abs32(d.Position[1]) >= 16384 {
		t.Errorf("doodad[%d] position out of bounds: %v", i, d.Position)
	}

	if d.SkinID != "" && len(d.SkinID) != 4 {
		t.Errorf("doodad[%d].SkinID = %q (len %d), want 4 or empty", i, d.SkinID, len(d.SkinID))
	}
}

// logSample dumps up to n parsed doodads (and up to 3 special-doodads) as
// indented JSON for human eyeball inspection.
func logSample(t *testing.T, f *File, n int) {
	t.Helper()

	type sample struct {
		Format         string          `json:"format"`
		Version        uint32          `json:"version"`
		SubVersion     uint32          `json:"sub_version"`
		DoodadsTotal   int             `json:"doodads_total"`
		DoodadsSample  []Doodad        `json:"doodads_sample"`
		SpecialFormat  uint32          `json:"special_format"`
		SpecialsTotal  int             `json:"specials_total"`
		SpecialsSample []SpecialDoodad `json:"specials_sample"`
	}

	d := f.Doodads
	if len(d) > n {
		d = d[:n]
	}
	sd := f.SpecialDoodads
	if len(sd) > 3 {
		sd = sd[:3]
	}

	s := sample{
		Format:         string(f.Format[:]),
		Version:        f.Version,
		SubVersion:     f.SubVersion,
		DoodadsTotal:   len(f.Doodads),
		DoodadsSample:  d,
		SpecialFormat:  f.SpecialFormat,
		SpecialsTotal:  len(f.SpecialDoodads),
		SpecialsSample: sd,
	}
	pretty, err := json.MarshalIndent(s, "", "  ")
	if err != nil {
		t.Fatalf("marshal: %v", err)
	}
	t.Logf("Sample:\n%s", string(pretty))
}

func abs32(f float32) float32 {
	if f < 0 {
		return -f
	}
	return f
}

// X Hero Reborn 1.3: real-world subver-11 map authored with a pre-Reforged
// game version. Its doodads have NO skin_id even though the .doo subversion
// claims 11. This used to crash the parser with "unexpected EOF" inside
// item-drop chance reads (we'd consume the flags byte as skin_id and then
// stride straight into garbage). The peek heuristic now handles this.
func TestParse_xHeroReborn(t *testing.T) {
	path := filepath.Join("testdata", "x_hero_reborn_1.3.doo")
	data, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}
	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if got, want := string(f.Format[:]), "W3do"; got != want {
		t.Errorf("Format = %q, want %q", got, want)
	}
	if f.Version != 8 {
		t.Errorf("Version = %d, want 8", f.Version)
	}
	if f.SubVersion != 11 {
		t.Errorf("SubVersion = %d, want 11", f.SubVersion)
	}
	if len(f.Doodads) != 1346 {
		t.Errorf("Doodads = %d, want 1346", len(f.Doodads))
	}
	// Variant signature: SkinID is empty across the board (no skin_id stored).
	for i, d := range f.Doodads {
		if d.SkinID != "" {
			t.Errorf("doodad[%d] unexpected SkinID %q on this variant", i, d.SkinID)
			break
		}
	}
	for i, d := range f.Doodads {
		validateDoodad(t, i, d)
	}
}

// --- synthetic tests ------------------------------------------------------

// buildDoo writes a minimal valid .doo with the given parameters. Used as a
// baseline that the truncation table can chop bytes from.
//
// includeSkinID controls whether each doodad carries a 4-byte FourCC skin_id.
// version controls whether item-drop-set fields are present.
func buildDoo(t *testing.T, version, subversion uint32, includeSkinID bool, doodadCount int, dropSetCount, itemsPerSet int) []byte {
	t.Helper()
	var b []byte
	put32 := func(v uint32) { b = binary.LittleEndian.AppendUint32(b, v) }
	putF := func(v float32) { put32(math.Float32bits(v)) }
	put8 := func(v uint8) { b = append(b, v) }
	put4 := func(s string) {
		if len(s) != 4 {
			t.Fatalf("FourCC %q must be 4 bytes", s)
		}
		b = append(b, s...)
	}

	put4("W3do")
	put32(version)
	put32(subversion)
	put32(uint32(doodadCount))

	for i := 0; i < doodadCount; i++ {
		put4("DRrk")
		put32(0) // variation
		putF(0)
		putF(0)
		putF(0) // position
		putF(0) // rotation
		putF(1)
		putF(1)
		putF(1) // scale
		if includeSkinID {
			put4("ABCD")
		}
		put8(2)   // flags
		put8(100) // life
		if version >= 8 {
			put32(0xFFFFFFFF) // map_item_table = -1
			put32(uint32(dropSetCount))
			for s := 0; s < dropSetCount; s++ {
				put32(uint32(itemsPerSet))
				for k := 0; k < itemsPerSet; k++ {
					put4("ratf") // item id
					put32(100)   // chance
				}
			}
		}
		put32(uint32(i + 1)) // creation_number
	}

	// Trailing special-doodads section: format=0, count=0.
	put32(0)
	put32(0)
	return b
}

// TestParse_synthetic exercises the synthetic builder against the parser to
// confirm round-trip correctness on a few known shapes.
func TestParse_synthetic(t *testing.T) {
	cases := []struct {
		name          string
		version       uint32
		subversion    uint32
		includeSkinID bool
		count         int
	}{
		{"v8-subver11-with-skinid", 8, 11, true, 3},
		{"v8-subver11-no-skinid (X-Hero variant)", 8, 11, false, 3},
		{"v8-subver9-with-skinid", 8, 9, true, 3},
		{"v8-subver9-no-skinid", 8, 9, false, 3},
		{"v7-no-itemdrops", 7, 7, false, 2},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			data := buildDoo(t, tc.version, tc.subversion, tc.includeSkinID, tc.count, 0, 0)
			f, err := Parse(data)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			if len(f.Doodads) != tc.count {
				t.Errorf("Doodads = %d, want %d", len(f.Doodads), tc.count)
			}
			for i, d := range f.Doodads {
				if tc.includeSkinID && d.SkinID != "ABCD" {
					t.Errorf("doodad[%d] SkinID = %q, want %q", i, d.SkinID, "ABCD")
				}
				if !tc.includeSkinID && d.SkinID != "" {
					t.Errorf("doodad[%d] SkinID = %q, want empty", i, d.SkinID)
				}
				if d.CreationNumber != uint32(i+1) {
					t.Errorf("doodad[%d] CreationNumber = %d, want %d", i, d.CreationNumber, i+1)
				}
			}
		})
	}
}

// TestParse_synthetic_withDrops exercises the per-doodad item-drop-set path —
// where misalignment originally surfaced for X Hero Reborn.
func TestParse_synthetic_withDrops(t *testing.T) {
	data := buildDoo(t, 8, 11, true, 2, 3, 4) // 3 sets × 4 items each, per doodad
	f, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(f.Doodads) != 2 {
		t.Fatalf("Doodads = %d, want 2", len(f.Doodads))
	}
	for i, d := range f.Doodads {
		if got, want := len(d.ItemDrops), 3*4; got != want {
			t.Errorf("doodad[%d] ItemDrops = %d, want %d", i, got, want)
		}
	}
}

// TestParse_truncation builds a known-good file, chops it at every field
// boundary, and confirms each truncation yields a wrapped EOF error that
// names the field where the read failed. This proves bounds-check coverage
// on the doodad-level reads.
func TestParse_truncation(t *testing.T) {
	// Baseline includes skin_id and one drop-set with one item, so every
	// per-doodad code path runs.
	base := buildDoo(t, 8, 11, true, 1, 1, 1)

	// Each entry: a cut-length, and a substring we expect in the error.
	// Offsets are computed by walking the same layout buildDoo writes.
	type cut struct {
		length int
		want   string
	}
	cuts := []cut{
		{0, "magic"},
		{2, "magic"},
		{4, "version"},
		{8, "subversion"},
		{12, "doodad count"},
		// Per-doodad. First doodad starts at offset 16.
		{16, "type_id"},
		{20, "variation"},
		{24, "position[0]"},
		{28, "position[1]"},
		{32, "position[2]"},
		{36, "rotation"},
		{40, "scale[0]"},
		{44, "scale[1]"},
		{48, "scale[2]"},
		// 52 is start of skin_id. Cutting in the middle of skin_id is fine —
		// peek() requires all 4 bytes; if any are missing the peek returns
		// nil, skin_id is skipped, and we continue with whatever bytes ARE
		// available. So:
		{52, "flags"}, // 0 bytes after scale → peek nil, skin_id skipped, flags EOFs
		{53, "life"},  // 1 byte after scale → peek nil, skin_id skipped, flags reads, life EOFs
		{56, "flags"}, // full skin_id consumed → flags EOFs
		{57, "life"},
		{58, "map_item_table"},
		{62, "item_drop_set_count"},
		{66, "drop_set[0] count"},
		{70, "drop_set[0].item[0].id"},
		{74, "drop_set[0].item[0].chance"},
		{78, "creation_number"},
		// Past the doodad: into the trailing special section.
		{82, "read special_format"},
		{86, "read special_count"},
	}

	for _, tc := range cuts {
		t.Run("len="+itoa(tc.length), func(t *testing.T) {
			if tc.length > len(base) {
				t.Fatalf("baseline too short: %d vs cut %d", len(base), tc.length)
			}
			_, err := Parse(base[:tc.length])
			if err == nil {
				t.Fatalf("expected error, got nil")
			}
			if !errors.Is(err, io.ErrUnexpectedEOF) {
				t.Errorf("error = %v, want io.ErrUnexpectedEOF chain", err)
			}
			if !strings.Contains(err.Error(), tc.want) {
				t.Errorf("error = %q, want substring %q", err.Error(), tc.want)
			}
		})
	}
}

// TestParse_sanityCaps verifies that an obviously-garbage drop-set count
// surfaces as a misalignment error rather than wandering off the buffer.
func TestParse_sanityCaps(t *testing.T) {
	// Build a baseline where one doodad carries a deliberately-corrupt
	// drop-set count. We hand-craft the bytes so we can poison just that
	// field without disturbing the rest of the layout.
	var b []byte
	put32 := func(v uint32) { b = binary.LittleEndian.AppendUint32(b, v) }
	putF := func(v float32) { put32(math.Float32bits(v)) }
	put8 := func(v uint8) { b = append(b, v) }
	put4 := func(s string) { b = append(b, s...) }

	put4("W3do")
	put32(8)  // version
	put32(11) // subversion
	put32(1)  // doodad_count
	put4("DRrk")
	put32(0)
	putF(0)
	putF(0)
	putF(0)
	putF(0)
	putF(1)
	putF(1)
	putF(1)
	put4("ABCD") // skin_id
	put8(2)
	put8(100)
	put32(0xFFFFFFFF) // map_item_table = -1
	put32(99999)      // ← bogus drop-set count
	// Pad with enough zeros that we'd run a very long time without the cap.
	for i := 0; i < 4096; i++ {
		put8(0)
	}

	_, err := Parse(b)
	if err == nil {
		t.Fatal("expected error, got nil")
	}
	if !strings.Contains(err.Error(), "item_drop_set_count") || !strings.Contains(err.Error(), "sanity cap") {
		t.Errorf("error = %q, want item_drop_set_count + sanity cap mention", err.Error())
	}
}

// TestParse_invalidMagic asserts the header validation message is helpful.
func TestParse_invalidMagic(t *testing.T) {
	_, err := Parse([]byte("XXXX\x07\x00\x00\x00\x09\x00\x00\x00\x00\x00\x00\x00"))
	if err == nil {
		t.Fatal("expected error")
	}
	if !strings.Contains(err.Error(), "magic") {
		t.Errorf("error = %q, want magic mention", err.Error())
	}
}

// TestRoundTrip asserts that Parse → Encode reproduces the original .doo
// bytes for every shipped fixture. This is the gate on the save/edit slice:
// a write path that ISN'T byte-faithful would silently corrupt fields the
// editor doesn't understand (custom subversion variants, exotic random-set
// FourCCs, etc.) on every save.
//
// All three real-world fixtures exercise different code paths:
//   - wc3_survival_v1_6.doo: minimal (often 0 doodads), validates the
//     header + empty special-doodads path
//   - enfo_ffb_v2_64f.doo: hundreds of doodads with item drops + skin_ids,
//     exercises the per-set boundary preservation + skin-id presence flag
//   - x_hero_reborn_1.3.doo: subversion 11 WITHOUT skin_ids (the variant
//     that originally crashed the parser), validates the peek-decision
//     preservation flow per-entity
func TestRoundTrip(t *testing.T) {
	fixtures := []string{
		fixturePathV16,
		fixturePathEnfo,
		filepath.Join("testdata", "x_hero_reborn_1.3.doo"),
	}
	for _, path := range fixtures {
		t.Run(filepath.Base(path), func(t *testing.T) {
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture: %v", err)
			}
			f, err := Parse(orig)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			encoded, err := Encode(f)
			if err != nil {
				t.Fatalf("Encode: %v", err)
			}
			if !bytes.Equal(orig, encoded) {
				// Pinpoint the first diff so the failure mode is debuggable.
				// Without this, a 1-byte diff in a 200KB file is a needle
				// in a haystack.
				mismatch := -1
				n := len(orig)
				if len(encoded) < n {
					n = len(encoded)
				}
				for i := 0; i < n; i++ {
					if orig[i] != encoded[i] {
						mismatch = i
						break
					}
				}
				t.Fatalf("round-trip mismatch: orig=%d encoded=%d first diff at offset %d (orig=0x%02x encoded=0x%02x)",
					len(orig), len(encoded), mismatch,
					byteAtOr(orig, mismatch), byteAtOr(encoded, mismatch))
			}
		})
	}
}

// TestMutateRotation_RoundTrip verifies that changing a Doodad's Rotation,
// encoding, and re-parsing preserves all other bytes intact AND returns the
// new rotation value. Exercises all three real-world fixtures.
func TestMutateRotation_RoundTrip(t *testing.T) {
	fixtures := []string{
		fixturePathV16,
		fixturePathEnfo,
		filepath.Join("testdata", "x_hero_reborn_1.3.doo"),
	}
	for _, path := range fixtures {
		t.Run(filepath.Base(path), func(t *testing.T) {
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture: %v", err)
			}
			f, err := Parse(orig)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			if len(f.Doodads) == 0 {
				t.Skip("no doodads in fixture")
			}

			idx := len(f.Doodads) - 1
			origRot := f.Doodads[idx].Rotation
			newRot := origRot + 1.5707963 // +90° in radians
			f.Doodads[idx].Rotation = newRot

			encoded, err := Encode(f)
			if err != nil {
				t.Fatalf("Encode after rotation mutation: %v", err)
			}
			f2, err := Parse(encoded)
			if err != nil {
				t.Fatalf("Parse of mutated bytes: %v", err)
			}
			got := f2.Doodads[idx].Rotation
			if got != newRot {
				t.Errorf("round-tripped Rotation = %v, want %v", got, newRot)
			}
			// Re-encode f2 → should match encoded exactly (full round-trip).
			reenc, err := Encode(f2)
			if err != nil {
				t.Fatalf("re-Encode: %v", err)
			}
			if !bytes.Equal(encoded, reenc) {
				mismatch := -1
				n := len(encoded)
				if len(reenc) < n {
					n = len(reenc)
				}
				for i := 0; i < n; i++ {
					if encoded[i] != reenc[i] {
						mismatch = i
						break
					}
				}
				t.Fatalf("re-encode mismatch at offset %d", mismatch)
			}
		})
	}
}

// TestMutateScale_RoundTrip verifies that changing a Doodad's Scale,
// encoding, and re-parsing returns the new scale. Doodads store Scale RAW
// on disk (no /128 convention) so no scaleRaw invalidation is needed —
// this is the simple/safe path compared to unitsdoo.
func TestMutateScale_RoundTrip(t *testing.T) {
	fixtures := []string{
		fixturePathV16,
		fixturePathEnfo,
		filepath.Join("testdata", "x_hero_reborn_1.3.doo"),
	}
	for _, path := range fixtures {
		t.Run(filepath.Base(path), func(t *testing.T) {
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture: %v", err)
			}
			f, err := Parse(orig)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			if len(f.Doodads) == 0 {
				t.Skip("no doodads in fixture")
			}

			idx := len(f.Doodads) - 1
			newScale := [3]float32{1.5, 1.5, 1.5}
			f.Doodads[idx].Scale = newScale
			// No scaleRaw invalidation needed — doodads store Scale raw on
			// disk. Encode writes Scale verbatim. In contrast, unitsdoo has
			// the scaleRaw preservation gotcha requiring ClearScaleRaw.

			encoded, err := Encode(f)
			if err != nil {
				t.Fatalf("Encode after scale mutation: %v", err)
			}
			f2, err := Parse(encoded)
			if err != nil {
				t.Fatalf("Parse of mutated bytes: %v", err)
			}
			got := f2.Doodads[idx].Scale
			if got != newScale {
				t.Errorf("round-tripped Scale = %v, want %v", got, newScale)
			}
		})
	}
}

// byteAtOr returns b[i] or 0xEE if i is out of range. Used for round-trip
// diff diagnostics so a length mismatch (one side shorter) doesn't panic.
func byteAtOr(b []byte, i int) byte {
	if i < 0 || i >= len(b) {
		return 0xEE
	}
	return b[i]
}

func itoa(n int) string {
	// Tiny avoid-strconv shim so the test loop names stay terse.
	if n == 0 {
		return "0"
	}
	neg := n < 0
	if neg {
		n = -n
	}
	var buf [20]byte
	i := len(buf)
	for n > 0 {
		i--
		buf[i] = byte('0' + n%10)
		n /= 10
	}
	if neg {
		i--
		buf[i] = '-'
	}
	return string(buf[i:])
}
