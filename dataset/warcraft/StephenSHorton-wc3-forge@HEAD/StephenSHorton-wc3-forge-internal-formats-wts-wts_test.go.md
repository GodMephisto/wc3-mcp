---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wts/wts_test.go"
ref: HEAD
---

package wts

import (
	"os"
	"testing"
)

func TestParse_basic(t *testing.T) {
	data := []byte("\xef\xbb\xbfSTRING 1\n{\nHello\n}\n\nSTRING 42 // comment\n{\nMulti\nline\nvalue\n}\n")
	s, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if got := s[1]; got != "Hello" {
		t.Errorf("[1] = %q, want %q", got, "Hello")
	}
	if got, want := s[42], "Multi\nline\nvalue"; got != want {
		t.Errorf("[42] = %q, want %q", got, want)
	}
}

func TestResolve(t *testing.T) {
	s := Strings{9661: "The Real Map Name"}
	cases := []struct{ in, want string }{
		{"TRIGSTR_9661", "The Real Map Name"},
		{"TRIGSTR_9999", "TRIGSTR_9999"}, // missing → ref unchanged
		{"Just a literal", "Just a literal"},
		{"trigstr_9661", "trigstr_9661"}, // case-sensitive per WC3 convention
		{"", ""},
	}
	for _, c := range cases {
		if got := s.Resolve(c.in); got != c.want {
			t.Errorf("Resolve(%q) = %q, want %q", c.in, got, c.want)
		}
	}
	// Nil-safe.
	var nilStrings Strings
	if got := nilStrings.Resolve("TRIGSTR_1"); got != "TRIGSTR_1" {
		t.Errorf("nil.Resolve = %q, want pass-through", got)
	}
}

func TestParse_enfo(t *testing.T) {
	path := `C:\Users\4step\maps-extracted\enfo-v2.64f\war3map.wts`
	data, err := os.ReadFile(path)
	if err != nil {
		t.Skipf("fixture not available: %v", err)
	}
	s, err := Parse(data)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	t.Logf("Parsed %d STRING entries", len(s))
	if name, ok := s[9661]; ok {
		t.Logf("STRING 9661 (map name candidate): %q", name)
	} else {
		t.Errorf("expected STRING 9661 to exist in enfos wts")
	}
	if author, ok := s[9664]; ok {
		t.Logf("STRING 9664 (author candidate): %q", author)
	}
}
