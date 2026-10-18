---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/convert_diagnostics_test.go"
ref: HEAD
---

package forge

import "testing"

func TestExtractDiagnostics(t *testing.T) {
	in := "" +
		"function Foo()\n" +
		"\t--[[ jass2lua: unexpected token \"static\" at line 4:13 | source: static method onX ]]\n" +
		"\terror(\"jass2lua failed: unexpected token \\\"static\\\" at line 4:13\")\n" +
		"\tlocal a = 1\n" +
		"\terror(\"jass2lua failed: lex error at line 9\")\n" + // lone guard (no comment)
		"end\n"

	clean, diags := extractDiagnostics(in)

	// Markers gone: no error()/--[[ jass2lua left in the cleaned text.
	if got := clean; contains(got, "jass2lua failed") || contains(got, "--[[ jass2lua") {
		t.Fatalf("cleaned text still has markers:\n%s", got)
	}
	if len(diags) != 2 {
		t.Fatalf("want 2 diagnostics, got %d: %+v", len(diags), diags)
	}

	// First diagnostic: reason + source from the comment, anchored to the blank
	// line that replaced the 2-line marker.
	d0 := diags[0]
	if d0.Reason != `unexpected token "static" at line 4:13` {
		t.Errorf("d0 reason = %q", d0.Reason)
	}
	if d0.Source != "static method onX" {
		t.Errorf("d0 source = %q", d0.Source)
	}
	lines := splitLinesTest(clean)
	if d0.LuaLine < 1 || d0.LuaLine > len(lines) || trimTest(lines[d0.LuaLine-1]) != "" {
		t.Errorf("d0 LuaLine=%d should point at a blank line; line=%q", d0.LuaLine, lines[d0.LuaLine-1])
	}

	// Second diagnostic: lone guard, reason parsed from the error() line.
	if diags[1].Reason != "lex error at line 9" {
		t.Errorf("d1 reason = %q", diags[1].Reason)
	}

	// Real code survives.
	if !contains(clean, "local a = 1") || !contains(clean, "function Foo()") {
		t.Errorf("real code lost:\n%s", clean)
	}
}

func TestExtractDiagnostics_NoMarkers(t *testing.T) {
	in := "function Foo()\n\tlocal a = 1\nend\n"
	clean, diags := extractDiagnostics(in)
	if clean != in || diags != nil {
		t.Errorf("clean section should pass through unchanged; diags=%v", diags)
	}
}

// tiny helpers to avoid importing strings in the test for trivial ops
func contains(s, sub string) bool { return indexOf(s, sub) >= 0 }
func indexOf(s, sub string) int {
	for i := 0; i+len(sub) <= len(s); i++ {
		if s[i:i+len(sub)] == sub {
			return i
		}
	}
	return -1
}
func splitLinesTest(s string) []string {
	var out []string
	start := 0
	for i := 0; i < len(s); i++ {
		if s[i] == '\n' {
			out = append(out, s[start:i])
			start = i + 1
		}
	}
	out = append(out, s[start:])
	return out
}
func trimTest(s string) string {
	for len(s) > 0 && (s[0] == ' ' || s[0] == '\t' || s[0] == '\r') {
		s = s[1:]
	}
	for len(s) > 0 && (s[len(s)-1] == ' ' || s[len(s)-1] == '\t' || s[len(s)-1] == '\r') {
		s = s[:len(s)-1]
	}
	return s
}
