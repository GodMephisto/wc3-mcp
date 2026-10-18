---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wts/encode_test.go"
ref: HEAD
---

package wts

import (
	"reflect"
	"strings"
	"testing"
)

// TestEncodeParseRoundTrip: Encode then Parse must reproduce the Strings map.
// (Encode is canonical, not byte-faithful to an arbitrary source file, so the
// round-trip invariant is map-level: Parse(Encode(m)) == m.)
func TestEncodeParseRoundTrip(t *testing.T) {
	m := Strings{
		9661: "Just another Warcraft III map",
		1:    "Author Name",
		42:   "A description\nthat spans\nmultiple lines",
		7:    "", // empty value
		100:  "with |cffff0000color|r codes kept verbatim",
	}
	data, err := Encode(m)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	got, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if !reflect.DeepEqual(map[uint32]string(got), map[uint32]string(m)) {
		t.Errorf("round-trip mismatch:\n got=%#v\nwant=%#v", got, m)
	}
}

// TestEncodeAscendingOrder: entries are emitted with ascending IDs regardless of
// map iteration order.
func TestEncodeAscendingOrder(t *testing.T) {
	data, err := Encode(Strings{30: "c", 10: "a", 20: "b"})
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	s := string(data)
	i10, i20, i30 := strings.Index(s, "STRING 10"), strings.Index(s, "STRING 20"), strings.Index(s, "STRING 30")
	if !(i10 >= 0 && i10 < i20 && i20 < i30) {
		t.Errorf("IDs not ascending: STRING 10@%d, 20@%d, 30@%d", i10, i20, i30)
	}
}

// TestEncodeMultiline: interior newlines survive Encode→Parse verbatim.
func TestEncodeMultiline(t *testing.T) {
	const v = "line one\nline two\nline three"
	data, _ := Encode(Strings{5: v})
	got, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if got[5] != v {
		t.Errorf("multiline value = %q, want %q", got[5], v)
	}
}

// TestRefID parses TRIGSTR_<n> references and rejects everything else.
func TestRefID(t *testing.T) {
	cases := []struct {
		in   string
		id   uint32
		want bool
	}{
		{"TRIGSTR_009", 9, true},
		{"TRIGSTR_0", 0, true},
		{"TRIGSTR_12345", 12345, true},
		{"My Map Name", 0, false},
		{"TRIGSTR_", 0, false},
		{"trigstr_9", 0, false}, // case-sensitive
		{"TRIGSTR_9 ", 0, false}, // anchored — no trailing junk
		{"", 0, false},
	}
	for _, c := range cases {
		id, ok := RefID(c.in)
		if ok != c.want || (ok && id != c.id) {
			t.Errorf("RefID(%q) = (%d, %v), want (%d, %v)", c.in, id, ok, c.id, c.want)
		}
	}
}
