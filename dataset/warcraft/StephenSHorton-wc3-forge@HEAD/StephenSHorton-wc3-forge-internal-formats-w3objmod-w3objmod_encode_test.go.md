---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3objmod/w3objmod_encode_test.go"
ref: HEAD
---

package w3objmod

import (
	"reflect"
	"testing"
)

// TestEncode_RoundTrip_NoOpt covers the w3u/w3t/w3b shape (no per-mod
// level/dataPointer slots). Build a small File with one original-edit and
// one custom across int/float/string field types, encode, parse, and
// confirm the round-trip is value-equal.
func TestEncode_RoundTrip_NoOpt(t *testing.T) {
	fields := FieldMap{
		"unam": "name",
		"uhpm": "hp",
		"umvs": "spd",
	}
	f := &File{
		Version: 3,
		OriginalEdits: []OriginalEdit{
			{
				BaseID: "hpea",
				Overrides: Overrides{
					"name": "TestPeasant",
					"hp":   "500",
				},
			},
		},
		Customs: []CustomObject{
			{
				ID:     "h001",
				BaseID: "hpea",
				Overrides: Overrides{
					"name": "CustomGuy",
					"hp":   "200",
					"spd":  "1.5",
				},
			},
		},
	}
	data, err := Encode(f, false, fields)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}

	g, err := Parse(data, false, fields)
	if err != nil {
		t.Fatalf("re-Parse: %v", err)
	}
	if g.Version != f.Version {
		t.Errorf("Version = %d, want %d", g.Version, f.Version)
	}
	if !reflect.DeepEqual(g.OriginalEdits, f.OriginalEdits) {
		t.Errorf("OriginalEdits round-trip mismatch:\n got %+v\nwant %+v", g.OriginalEdits, f.OriginalEdits)
	}
	if !reflect.DeepEqual(g.Customs, f.Customs) {
		t.Errorf("Customs round-trip mismatch:\n got %+v\nwant %+v", g.Customs, f.Customs)
	}
}

// TestEncode_RoundTrip_Opt covers the w3d/w3a shape with the optional
// level/dataPointer slots between type and value.
func TestEncode_RoundTrip_Opt(t *testing.T) {
	fields := FieldMap{
		"dfil": "file",
		"dshd": "shadow",
	}
	f := &File{
		Version: 3,
		Customs: []CustomObject{
			{
				ID:     "D001",
				BaseID: "ATtr",
				Overrides: Overrides{
					"file":   "Doodads/Test/Test.mdx",
					"shadow": "Shadow.blp",
				},
			},
		},
	}
	data, err := Encode(f, true, fields)
	if err != nil {
		t.Fatalf("Encode (opt): %v", err)
	}
	g, err := Parse(data, true, fields)
	if err != nil {
		t.Fatalf("re-Parse (opt): %v", err)
	}
	if !reflect.DeepEqual(g.Customs, f.Customs) {
		t.Errorf("Customs round-trip mismatch:\n got %+v\nwant %+v", g.Customs, f.Customs)
	}
}

// TestEncode_RoundTrip_LeveledFields is the regression for the leveled-field
// collapse bug: a w3a ability with the SAME field FourCC carrying TWO distinct
// per-level values (level 1 = 100, level 2 = 250) must round-trip through
// Encode→Parse without collapsing into a single flat value. Before the fix the
// override model was map[string]string (no level dimension) and Encode
// hardcoded level_variation=0/data_pointer=0, so the two levels overwrote each
// other.
func TestEncode_RoundTrip_LeveledFields(t *testing.T) {
	fields := FieldMap{
		"Hbz1": "damage",
		"aaea": "areaofeffect",
	}
	f := &File{
		Version: 3,
		Customs: []CustomObject{
			{
				ID:     "A001",
				BaseID: "AHbz",
				// Base/level-0 slot stays in Overrides.
				Overrides: Overrides{
					"areaofeffect": "200",
				},
				// Two distinct per-level values for the SAME field FourCC.
				Levels: []LevelOverride{
					{FourCC: "damage", Level: 1, Value: "100"},
					{FourCC: "damage", Level: 2, Value: "250"},
				},
			},
		},
	}
	data, err := Encode(f, true, fields)
	if err != nil {
		t.Fatalf("Encode (opt, leveled): %v", err)
	}
	g, err := Parse(data, true, fields)
	if err != nil {
		t.Fatalf("re-Parse (opt, leveled): %v", err)
	}
	if len(g.Customs) != 1 {
		t.Fatalf("custom count = %d, want 1", len(g.Customs))
	}
	got := g.Customs[0]
	if !reflect.DeepEqual(got.Overrides, f.Customs[0].Overrides) {
		t.Errorf("base overrides mismatch:\n got %+v\nwant %+v", got.Overrides, f.Customs[0].Overrides)
	}
	if len(got.Levels) != 2 {
		t.Fatalf("level count = %d, want 2 (leveled values collapsed?)", len(got.Levels))
	}
	// The two per-level values must both survive, each tagged with its level.
	byLevel := map[uint32]string{}
	for _, lo := range got.Levels {
		if lo.FourCC != "damage" {
			t.Errorf("unexpected level field %q", lo.FourCC)
		}
		byLevel[lo.Level] = lo.Value
	}
	if byLevel[1] != "100" {
		t.Errorf("level 1 value = %q, want 100", byLevel[1])
	}
	if byLevel[2] != "250" {
		t.Errorf("level 2 value = %q, want 250 (collapsed to level 1?)", byLevel[2])
	}
}

// TestEncode_RoundTrip_DataPointer confirms a non-zero data_pointer on a
// leveled entry is preserved verbatim through the round-trip (a handful of
// multi-data abilities use the dp slot to index sub-fields).
func TestEncode_RoundTrip_DataPointer(t *testing.T) {
	f := &File{
		Version: 3,
		Customs: []CustomObject{
			{
				ID:        "A002",
				BaseID:    "AHfa",
				Overrides: Overrides{},
				Levels: []LevelOverride{
					{FourCC: "Idam", Level: 3, DataPointer: 2, Value: "42"},
				},
			},
		},
	}
	data, err := Encode(f, true, nil)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	g, err := Parse(data, true, nil)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(g.Customs) != 1 || len(g.Customs[0].Levels) != 1 {
		t.Fatalf("unexpected shape: %+v", g.Customs)
	}
	lo := g.Customs[0].Levels[0]
	if lo.FourCC != "Idam" || lo.Level != 3 || lo.DataPointer != 2 || lo.Value != "42" {
		t.Errorf("level override round-trip mismatch: %+v", lo)
	}
}

// TestEncode_EmptyFile encodes a nil/empty File and confirms it parses
// back as an empty table (no customs, no edits). Guard against new
// customers passing a nil pointer to a freshly-allocated Session.
func TestEncode_EmptyFile(t *testing.T) {
	data, err := Encode(&File{}, false, nil)
	if err != nil {
		t.Fatalf("Encode empty: %v", err)
	}
	g, err := Parse(data, false, nil)
	if err != nil {
		t.Fatalf("Parse empty: %v", err)
	}
	if len(g.OriginalEdits) != 0 || len(g.Customs) != 0 {
		t.Errorf("empty round-trip leaked entries: %+v", g)
	}
}

// TestEncode_RoundTripFromFourCCKeyed encodes a File whose Overrides keys
// are RAW FourCC strings (matching Parse with nil FieldMap). Mirrors how
// Session today carries unitMods — overrides come in as FourCC-keyed and
// translate-to-column happens at the MergedUnits layer.
func TestEncode_RoundTripFromFourCCKeyed(t *testing.T) {
	// nil FieldMap on Parse → Overrides keys ARE the FourCCs.
	f := &File{
		Version: 3,
		Customs: []CustomObject{
			{
				ID:     "h001",
				BaseID: "hpea",
				Overrides: Overrides{
					"unam": "RawKeyedCustom",
					"uhpm": "777",
				},
			},
		},
	}
	// Encode with nil fields — resolveModID must accept the 4-char keys as-is.
	data, err := Encode(f, false, nil)
	if err != nil {
		t.Fatalf("Encode nil-fields: %v", err)
	}
	g, err := Parse(data, false, nil)
	if err != nil {
		t.Fatalf("Parse nil-fields: %v", err)
	}
	if !reflect.DeepEqual(g.Customs, f.Customs) {
		t.Errorf("round-trip mismatch:\n got %+v\nwant %+v", g.Customs, f.Customs)
	}
}

// TestInferType ensures the value-type inference matches the wire format
// expectations: ints prefer TypeInt over TypeFloat, floats go to TypeFloat,
// strings go to TypeString. Empty values fall to TypeString to preserve
// the "no value" semantic instead of decoding as zero.
func TestInferType(t *testing.T) {
	cases := []struct {
		v    string
		want uint32
	}{
		{"100", TypeInt},
		{"-7", TypeInt},
		{"0", TypeInt},
		{"1.5", TypeFloat},
		{"3.14", TypeFloat},
		{"-0.5", TypeFloat},
		{"hello", TypeString},
		{"Units/Test.mdx", TypeString},
		{"", TypeString},
		{"1.0", TypeFloat}, // has decimal → float, even though numerically int
	}
	for _, c := range cases {
		if got := inferType(c.v); got != c.want {
			t.Errorf("inferType(%q) = %d, want %d", c.v, got, c.want)
		}
	}
}
