---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/define.go"
ref: HEAD
---

package jass2lua

// define.go — vJASS `define` preprocessor (Phase 4).
//
// JassHelper's `define` is a Pascal-style constant declaration:
//
//	define MAX_ITEMS = 5
//	define HERO_ID = 'H001'
//	define DAMAGE_FN = MyDamageFunc()
//
// At the JassHelper level this is purely a textual substitution. We have a
// simpler problem: WC3-Lua treats top-level assignments as globals and
// `local` for local scope. We map:
//
//	define X = <expr>        (at top level)        → X = <expr>
//	define X = <expr>        (inside a function)   → local X = <expr>
//
// Scope-detection heuristic: walk line-by-line, tracking whether we're
// inside a `function ... endfunction` block. Anything outside is top-level;
// anything inside gets `local`.
//
// We deliberately don't try to perform textual substitution of the
// identifier across the source — Lua's runtime resolves identifiers
// dynamically, so a `define X = 5` at top-level scope becomes a global `X = 5`
// that all subsequent code references through the normal global lookup.

import (
	"regexp"
	"strings"
)

// defineRe matches a `define NAME = EXPR` line. The expression is captured
// greedily through end-of-line — JASS expressions don't have multi-line
// continuation, so this is safe.
//
// Group 1: NAME, Group 2: EXPR.
var defineRe = regexp.MustCompile(`^\s*define\s+([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.+?)\s*$`)

// PreprocessDefines rewrites `define X = <expr>` lines to plain JASS-style
// assignments (`X = <expr>` at top level; `local X = <expr>` inside a
// function body). The result feeds into the JASS transpiler, which emits
// the equivalent Lua text directly.
//
// We use a light line-by-line scan + a function-depth counter to pick the
// right scope. The counter is incremented on `function` / `method` openers
// and decremented on `endfunction` / `endmethod`. This is intentionally
// permissive — globals blocks, struct blocks, etc., don't move the counter
// (a struct-level `define` shouldn't happen in real source, and a globals
// block `define` is also unconventional; both fall through as top-level).
func PreprocessDefines(src string) string {
	if !strings.Contains(src, "define") {
		return src
	}
	var out strings.Builder
	out.Grow(len(src))
	lines := splitLinesKeepEnd(src)
	depth := 0
	for _, line := range lines {
		trim := strings.TrimRight(line, "\r\n")
		bare := strings.TrimSpace(trim)
		// Track function/method depth BEFORE rewriting.
		// Note: opener detection here is intentionally cheap (prefix-match);
		// false positives on a string literal containing `function` are
		// rare and would only mis-scope a nearby `define`. Accept the
		// trade-off for simplicity.
		if strings.HasPrefix(bare, "function ") || strings.HasPrefix(bare, "method ") ||
			strings.HasPrefix(bare, "static method ") || strings.HasPrefix(bare, "private function ") ||
			strings.HasPrefix(bare, "public function ") || strings.HasPrefix(bare, "constant function ") {
			depth++
		}
		// Rewrite a `define` line if present.
		if m := defineRe.FindStringSubmatch(trim); m != nil {
			indent := trim[:len(trim)-len(strings.TrimLeft(trim, " \t"))]
			name, expr := m[1], m[2]
			eol := line[len(trim):]
			if depth > 0 {
				// Inside a function: `local NAME = EXPR`. JASS requires a type
				// after `local`, so emit as `integer` and let the transpiler
				// figure it out (TranspileScript drops the type in Lua output).
				out.WriteString(indent + "local integer " + name + " = " + expr + eol)
			} else {
				// Top-level: emit a globals-block-style assignment. JASS
				// doesn't allow bare top-level assignments, so wrap as a
				// `globals` block with a single decl. This survives the
				// transpiler cleanly.
				out.WriteString(indent + "globals\n")
				out.WriteString(indent + "    integer " + name + " = " + expr + "\n")
				out.WriteString(indent + "endglobals" + eol)
			}
			continue
		}
		// Track closers AFTER rewriting (so a `define` on the SAME line as
		// the closer — extremely rare — still picks up the local scope).
		if bare == "endfunction" || bare == "endmethod" {
			if depth > 0 {
				depth--
			}
		}
		out.WriteString(line)
	}
	return out.String()
}
