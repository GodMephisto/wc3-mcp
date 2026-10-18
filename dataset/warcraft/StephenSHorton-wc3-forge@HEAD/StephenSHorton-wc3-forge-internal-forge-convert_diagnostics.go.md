---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/convert_diagnostics.go"
ref: HEAD
---

package forge

import (
	"regexp"
	"strings"
)

// Diagnostic is one untranslatable spot in a section's transpiled Lua, surfaced
// to the convert dialog so it can highlight the line + show inline commentary
// instead of leaving noisy marker text in the code. LuaLine is 1-based into the
// CLEANED transpiled text (the text after extractDiagnostics has removed the
// markers).
type Diagnostic struct {
	LuaLine int    `json:"lua_line"`
	Reason  string `json:"reason"`
	Source  string `json:"source,omitempty"`
}

// The emitter marks a statement it can't translate with a two-line pair:
//
//	--[[ jass2lua: <reason> | source: <snippet> ]]
//	error("jass2lua failed: <reason>")
//
// (Lex/whole-section failures emit just the error() line.) extractDiagnostics
// collapses each such marker to a single BLANK line and records a Diagnostic, so
// the displayed/written Lua is clean and the error shows only as an editor
// decoration anchored to that line.
var (
	diagCommentRe = regexp.MustCompile(`^\s*--\[\[ jass2lua:\s*(.*?)\s*\]\]\s*$`)
	diagGuardRe   = regexp.MustCompile(`^\s*error\("jass2lua failed`)
)

// extractDiagnostics strips the inline jass2lua error markers from transpiled
// Lua, replacing each with a blank line, and returns the cleaned text plus the
// list of diagnostics (with their 1-based line number in the cleaned text).
func extractDiagnostics(transpiled string) (string, []Diagnostic) {
	if !strings.Contains(transpiled, "jass2lua failed") {
		return transpiled, nil
	}
	lines := strings.Split(transpiled, "\n")
	clean := make([]string, 0, len(lines))
	var diags []Diagnostic
	for _, line := range lines {
		if diagGuardRe.MatchString(line) {
			reason, source := "", ""
			// The reason/source live in the preceding `--[[ jass2lua: … ]]`
			// comment, which we already appended to clean — pull it back out.
			if n := len(clean); n > 0 {
				if m := diagCommentRe.FindStringSubmatch(clean[n-1]); m != nil {
					reason, source = splitReasonSource(m[1])
					clean = clean[:n-1]
				}
			}
			if reason == "" {
				reason = guardReason(line)
			}
			clean = append(clean, "")
			diags = append(diags, Diagnostic{LuaLine: len(clean), Reason: reason, Source: source})
			continue
		}
		clean = append(clean, line)
	}
	return strings.Join(clean, "\n"), diags
}

// cleanTranspiled returns just the marker-stripped Lua (for the write path,
// which doesn't need the diagnostics).
func cleanTranspiled(transpiled string) string {
	c, _ := extractDiagnostics(transpiled)
	return c
}

// splitReasonSource splits a `<reason> | source: <snippet>` comment payload.
func splitReasonSource(s string) (reason, source string) {
	if i := strings.Index(s, " | source: "); i >= 0 {
		return strings.TrimSpace(s[:i]), strings.TrimSpace(s[i+len(" | source: "):])
	}
	return strings.TrimSpace(s), ""
}

// guardReason pulls the reason out of an `error("jass2lua failed: <reason>")`
// line when there was no preceding comment (e.g. a whole-section lex failure).
func guardReason(line string) string {
	const pre = `jass2lua failed: `
	i := strings.Index(line, pre)
	if i < 0 {
		return "transpile failed"
	}
	rest := line[i+len(pre):]
	// rest is the tail of a Go %q-quoted string; trim the closing `")`.
	rest = strings.TrimRight(strings.TrimSpace(rest), ")")
	rest = strings.TrimRight(rest, `"`)
	// unescape the common \" sequences from %q
	rest = strings.ReplaceAll(rest, `\"`, `"`)
	return strings.TrimSpace(rest)
}
