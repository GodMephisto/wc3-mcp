---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/script_skymodel.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"fmt"
	"regexp"
	"strings"
)

// rewriteSetSkyModel returns the script source with `SetSkyModel("...")`
// updated (or inserted) to use newPath. The newPath argument should be in
// the editor's normalized form (lowercase forward-slash, e.g.
// "environment/sky/northrendsky/northrendsky.mdx"); this function converts
// it to the conventional WC3 script form (`Environment\\Sky\\...`).
//
// Behavior:
//   - If one or more SetSkyModel calls exist, every occurrence's argument is
//     replaced. (Maps occasionally call SetSkyModel multiple times — e.g. in
//     cinematics — so a single replacement could leave a stale call winning.
//     Replacing all keeps the editor's pick authoritative.)
//   - If no SetSkyModel call exists, one is inserted at the top of `main`
//     (JASS: after `function main takes nothing returns nothing`; Lua: after
//     `function main()`).
//   - If `main` can't be found, returns an error rather than guessing — the
//     caller surfaces this as a "couldn't write" toast and the user keeps
//     the pending change.
//
// Empty newPath is valid: it produces `SetSkyModel("")` which disables the
// sky at runtime.
//
// isLua picks between JASS and Lua syntax. The two share enough common ground
// (the SetSkyModel signature is identical) that the regex is shared; only the
// insertion site differs.
func rewriteSetSkyModel(src []byte, newPath string, isLua bool) ([]byte, error) {
	// Convert "environment/sky/foo/foo.mdx" → "Environment\\Sky\\Foo\\Foo.mdl"
	// for nicer-looking script output. WC3 itself is case-insensitive on
	// asset paths and accepts both .mdl/.mdx + forward/back slashes, but the
	// editor's conventional output is title-case + doubled-backslash + .mdl.
	scriptPath := toScriptSkyPath(newPath)
	newCall := fmt.Sprintf(`SetSkyModel("%s")`, scriptPath)
	if !isLua {
		// JASS calls require `call` prefix in statement position.
		newCall = "call " + newCall
	}

	// Replace any existing SetSkyModel call. We preserve the leading `call `
	// for JASS by matching it as part of the existing line.
	replaced := setSkyModelCallRE.ReplaceAllStringFunc(string(src), func(match string) string {
		// Carry over the indentation that preceded the match. Easier than
		// computing it from offsets — the regex captures leading whitespace.
		// The full match starts with optional indent + ("call " for JASS).
		// We re-emit the whole call with the same indent.
		indent := leadingIndent(match)
		return indent + strings.TrimLeft(newCall, " \t")
	})
	if replaced != string(src) {
		return []byte(replaced), nil
	}

	// No existing call — insert one. Find `function main` / `function main()`.
	insertRE := mainHeaderJassRE
	if isLua {
		insertRE = mainHeaderLuaRE
	}
	loc := insertRE.FindIndex(src)
	if loc == nil {
		return nil, fmt.Errorf("could not find main() function header to insert SetSkyModel call")
	}
	// Insert a new line right after the header line (after its trailing \n).
	// Find the first newline after loc[1].
	nl := bytes.IndexByte(src[loc[1]:], '\n')
	if nl < 0 {
		// Header is the last thing in the file — unusual but possible. Append.
		return append(append([]byte{}, src...), []byte("\n    "+strings.TrimLeft(newCall, " \t")+"\n")...), nil
	}
	insertAt := loc[1] + nl + 1
	// 4-space indent — matches the conventional WC3 script formatting.
	insertion := []byte("    " + strings.TrimLeft(newCall, " \t") + "\n")
	out := make([]byte, 0, len(src)+len(insertion))
	out = append(out, src[:insertAt]...)
	out = append(out, insertion...)
	out = append(out, src[insertAt:]...)
	return out, nil
}

// setSkyModelCallRE matches a complete SetSkyModel("…") statement at start of
// a line, optionally with leading `call ` (JASS). The (?m) flag lets ^ match
// after newlines so we catch every call. Group 1 is the indent + "call "
// prefix (or just indent for Lua); group 2 is the path argument.
//
// Permissive about quote style and whitespace; rejects only obviously-broken
// shapes (mismatched quotes, missing closing paren).
var setSkyModelCallRE = regexp.MustCompile(`(?im)^([ \t]*(?:call[ \t]+)?)SetSkyModel\s*\(\s*"[^"]*"\s*\)`)

// mainHeaderJassRE matches `function main takes nothing returns nothing` —
// the JASS entry-point that runs on map start. Anchored to start of line to
// avoid matching mid-comment occurrences.
var mainHeaderJassRE = regexp.MustCompile(`(?m)^[ \t]*function\s+main\s+takes\s+nothing\s+returns\s+nothing`)

// mainHeaderLuaRE matches `function main()` — the Lua entry-point. Allows
// optional whitespace before/inside parens to tolerate hand-formatted source.
var mainHeaderLuaRE = regexp.MustCompile(`(?m)^[ \t]*function\s+main\s*\(\s*\)`)

// leadingIndent returns just the run of whitespace at the start of s.
func leadingIndent(s string) string {
	for i := 0; i < len(s); i++ {
		c := s[i]
		if c != ' ' && c != '\t' {
			return s[:i]
		}
	}
	return s
}

// toScriptSkyPath converts a normalized editor path
// ("environment/sky/foo/foo.mdx") into the title-cased, double-backslashed
// .mdl form conventionally used in WC3 scripts
// ("Environment\\Sky\\Foo\\Foo.mdl"). WC3 is case-insensitive on asset paths
// and accepts both .mdl/.mdx + forward/back slashes; this is purely
// cosmetic — but matches what existing maps look like so a diff doesn't
// jump out as foreign.
//
// Empty input passes through (SetSkyModel("") is the "no sky" intent).
func toScriptSkyPath(p string) string {
	if p == "" {
		return ""
	}
	// Title-case each `/`-separated segment.
	parts := strings.Split(p, "/")
	for i, part := range parts {
		parts[i] = titleCaseAsciiWord(part)
	}
	titled := strings.Join(parts, `\\`)
	// Prefer .mdl extension (the convention in [SkyModels]).
	if strings.HasSuffix(strings.ToLower(titled), ".mdx") {
		titled = titled[:len(titled)-4] + ".mdl"
	}
	return titled
}

// titleCaseAsciiWord uppercases the first letter of each underscore-separated
// chunk so "lordaeronsummersky" → "Lordaeronsummersky" and "outland_sky" →
// "Outland_Sky". WC3 conventionally writes its sky filenames in CamelCase
// like "LordaeronSummerSky", which we can't recover without a dictionary —
// "Lordaeronsummersky" is close enough; case is irrelevant to the engine.
func titleCaseAsciiWord(s string) string {
	if s == "" {
		return s
	}
	out := make([]byte, 0, len(s))
	cap := true
	for i := 0; i < len(s); i++ {
		c := s[i]
		if c == '_' {
			cap = true
			out = append(out, c)
			continue
		}
		if cap && c >= 'a' && c <= 'z' {
			c = c - ('a' - 'A')
		}
		out = append(out, c)
		cap = false
	}
	return string(out)
}
