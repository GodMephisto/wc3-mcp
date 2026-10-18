---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/preprocess.go"
ref: HEAD
---

package jass2lua

// preprocess.go — vJASS textmacro preprocessor (Phase 1 of vJASS support).
//
// JassHelper supports a small "preprocessor" syntax embedded in `//!` line
// comments. The most important construct is the textmacro — a parameterized
// block of JASS that can be expanded at one or more call sites:
//
//	//! textmacro MyMacro takes A, B
//	    local integer $A$ = $B$
//	//! endtextmacro
//
//	//! runtextmacro MyMacro("foo", "0")     // → expands to: local integer foo = 0
//
// This file implements that preprocessor pass. After preprocessing, a map
// that ONLY used textmacros (no library/scope/struct/module) becomes pure
// JASS and flows through the existing TranspileScript pipeline unchanged.
// vJASS keywords that are NOT consumed by this pass (library, scope, struct,
// module, …) continue to block conversion (handled by Phases 2-4).
//
// Semantics implemented:
//   - `//! textmacro Name takes p1, p2, …` … `//! endtextmacro` — definition;
//     stripped from the output.
//   - `//! runtextmacro Name(arg1, arg2)` — expand verbatim; substitute
//     `$pN$` tokens in the body with the matching arg.
//   - `//! runtextmacroonce Name(...)` — same as runtextmacro but only fires
//     once per unique (name, args) tuple. Subsequent calls produce an empty
//     expansion + a comment noting the suppression.
//   - Recursive expansion: a macro body containing `//! runtextmacro Other(…)`
//     is expanded eagerly (bounded by a depth limit to catch self-recursion).
//   - Unknown `//!` directives (e.g. `//! i //…`, `//! external …`) become
//     pass-through comments + a non-fatal PreprocessError.
//
// Phase 4 additions:
//   - `//! if SOMETHING` / `//! else` / `//! endif` conditional compilation.
//     For v1 we ALWAYS take the `if` branch and drop the `else` branch — we
//     don't have a JassHelper-style condition evaluator. This is conservative
//     for the common pattern (`//! if DEBUG_MODE` … `//! else` … `//! endif`,
//     where the `if` body is usually the production path).
//   - `StripBlockComments` — separate helper that strips C-style `/* … */`
//     block comments BEFORE the textmacro pass. JassHelper accepts them; the
//     tokenizer doesn't. Multi-line capable.
//
// Intentionally NOT implemented (out of scope, no plausible map needs it):
//   - `//! import` / `//! zinc { … }` / `//! novjass { … }`
//   - JassHelper's `//! i` directive that injects literal JASS text into the
//     preprocess output — for now we just round-trip it as a comment.

import (
	"fmt"
	"regexp"
	"strings"
)

// PreprocessResult is the output of running the textmacro preprocessor.
type PreprocessResult struct {
	Expanded string            // The source with all textmacros expanded.
	Macros   map[string]Macro  // Definitions seen (for debugging / inspection).
	Calls    []MacroCall       // Call sites (for diagnostics).
	Errors   []PreprocessError // Non-fatal issues (unknown macro, arg count mismatch, etc).
}

// Macro is one parsed textmacro definition.
type Macro struct {
	Name   string
	Params []string
	Body   string // The raw body bytes between textmacro/endtextmacro.
	Line   int    // Definition line in source (for error reporting).
}

// MacroCall is one observed runtextmacro / runtextmacroonce call site.
type MacroCall struct {
	Name string
	Args []string
	Line int
	Once bool // true for runtextmacroonce
}

// PreprocessError is a non-fatal preprocessor diagnostic (unknown macro,
// arg-count mismatch, unrecognized `//!` directive, etc.). Surfaced through
// PreprocessResult.Errors and propagated to the UI as a per-section warning.
type PreprocessError struct {
	Line    int
	Message string
}

// maxExpandDepth bounds runtextmacro recursion. JassHelper itself has no
// fixed limit, but real-world macros never recurse beyond a handful of
// levels. 32 is comfortable headroom while still catching self-recursion
// from a typo'd macro body.
const maxExpandDepth = 32

// Regex helpers.

// textmacroDef matches `//! textmacro <Name> [takes <p1>, <p2>, …]`.
// Leading whitespace is allowed before //!. Params are optional (some macros
// take none — the `takes` clause is omitted entirely in that case). We're
// permissive on the takes-list separator since JassHelper accepts both
// `a, b` and `a,b`.
var textmacroDefRe = regexp.MustCompile(`^\s*//!\s*textmacro\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:takes\s+(.+))?$`)

// endtextmacro is the terminator for a definition block.
var endtextmacroRe = regexp.MustCompile(`^\s*//!\s*endtextmacro\s*$`)

// runtextmacroCallRe matches both `//! runtextmacro Name(...)` and the
// `runtextmacroonce` variant. Group 1 is empty for the plain form, "once"
// for the once form.
var runtextmacroCallRe = regexp.MustCompile(`^\s*//!\s*runtextmacro(once)?\s+([A-Za-z_][A-Za-z0-9_]*)\s*\((.*)\)\s*$`)

// otherDirectiveRe matches any other `//!` line (so we can pass it through
// with a diagnostic rather than crashing the preprocessor). We deliberately
// don't try to parse `//! i`, `//! external`, etc. — they round-trip as
// comments and the user is told.
var otherDirectiveRe = regexp.MustCompile(`^\s*//!\s*(\S+)`)

// Conditional-compilation regexes (Phase 4).
// `//! if SOMETHING` — opens a conditional block. SOMETHING is consumed but
// not evaluated; we always take the if-branch.
var condIfRe = regexp.MustCompile(`^\s*//!\s*if\b\s*(.*)$`)

// `//! else` — switches to the else-branch (which we drop).
var condElseRe = regexp.MustCompile(`^\s*//!\s*else\s*$`)

// `//! elseif` — same as else for our purposes (drop the rest of the block).
// We support it to avoid choking on maps that use it.
var condElseifRe = regexp.MustCompile(`^\s*//!\s*elseif\b`)

// `//! endif` — closes the conditional block.
var condEndifRe = regexp.MustCompile(`^\s*//!\s*endif\s*$`)

// paramSubRe captures `$ident$` tokens inside a macro body for arg substitution.
// We intentionally restrict to identifier-shaped names — `$1$` literal
// (used in some old macros as a dummy) would NOT match and is left as-is.
var paramSubRe = regexp.MustCompile(`\$([A-Za-z_][A-Za-z0-9_]*)\$`)

// StripBlockComments removes C-style block comments from src. This runs FIRST
// in the preprocessing pipeline because the JASS lexer doesn't recognize the
// /* opener and would error on it. Multi-line block comments are supported.
//
// We replace each block with a single newline-preserving spacer so downstream
// line-number diagnostics stay roughly aligned. (Inside a single-line block,
// the spacer is just empty; for multi-line blocks we preserve the newline
// count.)
//
// B3 fix: this used to be a naive (?s)/*.*?*/ regex (slashes escaped) that was
// neither line-comment- nor string-aware. JASS banner comments are full of
// asterisks (//***************), and //* contains the substring /*, so the
// regex treated a // line comment as the START of a block comment and swallowed
// everything up to the next */ (often dozens of lines away). That left a stray
// / at the start of the banner line, which the lexer then read as a / operator
// -- producing a cascade of "unexpected token /" parse errors. A /* inside a
// string literal had the same failure mode. The scanner below honors // line
// comments and "..." string literals so a /* inside either is left untouched.
func StripBlockComments(src string) string {
	if !strings.Contains(src, "/*") {
		return src
	}
	var b strings.Builder
	b.Grow(len(src))
	i := 0
	n := len(src)
	for i < n {
		c := src[i]
		// String literal: copy verbatim until the closing quote (honoring escape
		// sequences) so a /* inside a string is never treated as a block opener.
		if c == '"' {
			b.WriteByte(c)
			i++
			for i < n {
				ch := src[i]
				b.WriteByte(ch)
				i++
				if ch == '\\' && i < n {
					// Escaped char: copy the next byte too so an escaped quote
					// doesn't end the string early.
					b.WriteByte(src[i])
					i++
					continue
				}
				if ch == '"' || ch == '\n' {
					// Closing quote or unterminated-at-EOL: stop string scan.
					break
				}
			}
			continue
		}
		// Line comment // ... : copy verbatim until EOL. This is what stops a
		// //***** banner from being mistaken for a block-comment opener.
		if c == '/' && i+1 < n && src[i+1] == '/' {
			for i < n && src[i] != '\n' {
				b.WriteByte(src[i])
				i++
			}
			continue
		}
		// Block comment /* ... */ : replace with a newline-preserving spacer.
		if c == '/' && i+1 < n && src[i+1] == '*' {
			j := i + 2
			nl := 0
			for j < n {
				if src[j] == '\n' {
					nl++
				}
				if src[j] == '*' && j+1 < n && src[j+1] == '/' {
					j += 2
					break
				}
				j++
			}
			if nl == 0 {
				b.WriteByte(' ')
			} else {
				b.WriteString(strings.Repeat("\n", nl))
			}
			i = j
			continue
		}
		b.WriteByte(c)
		i++
	}
	return b.String()
}

// Preprocess parses the JASS source for textmacro definitions + call sites
// and returns the source with all call sites expanded.
//
// Conservative on errors: unknown macros, arg-count mismatches, and
// malformed `//!` lines produce a comment placeholder and a PreprocessError,
// but processing continues for the rest of the file. This matches
// JassHelper's non-fatal behavior and keeps the diff view usable.
//
// runtextmacroonce tracks an internal "fired" set keyed by (name, joined-args).
// Subsequent matching calls produce an empty expansion + a comment noting
// the suppression.
func Preprocess(jass string) PreprocessResult {
	return PreprocessWithMacros(jass, nil)
}

// PreprocessWithMacros is the cross-section variant of Preprocess. It seeds the
// macro table with `extra` BEFORE scanning the section for its own textmacro
// definitions, so a section that calls `//! runtextmacro Foo(...)` where `Foo`
// was defined in ANOTHER section still expands cleanly (P2#6: cross-trigger
// vJASS context — the per-section convert path used to preprocess each WTG
// section in isolation, so macros defined elsewhere were invisible).
//
// Section-local definitions take precedence: extractDefinitions overwrites any
// seeded entry of the same name, matching JassHelper's last-definition-wins
// behavior when the global concatenation order would otherwise differ. Passing
// nil/empty `extra` is identical to Preprocess.
func PreprocessWithMacros(jass string, extra map[string]Macro) PreprocessResult {
	res := PreprocessResult{
		Macros: map[string]Macro{},
	}
	// Seed cross-section macros first so a same-named local definition (added
	// by extractDefinitions below) overrides it.
	for k, v := range extra {
		res.Macros[k] = v
	}

	// Pass 0: process `//! if SOMETHING / //! else / //! endif` conditionals.
	// Always-take-first semantics — we keep the `if` body and drop the `else`
	// body. The condition itself is discarded (we don't have a JassHelper
	// evaluator). Nested conditionals supported.
	conditioned := stripConditionals(jass, &res)

	// Pass 1: scan for textmacro definitions. We strip them from the source
	// as we go so the remainder can be expanded without re-tripping the
	// definition matcher.
	stripped := extractDefinitions(conditioned, &res)

	// Pass 2: expand all call sites. runtextmacroonce dedupe lives here
	// (not in the recursive expander) so bodies referring to a once-macro
	// also honor the suppression.
	onceFired := map[string]bool{}
	expanded := expandCalls(stripped, res.Macros, onceFired, &res, 0)

	res.Expanded = expanded
	return res
}

// extractDefinitions consumes `//! textmacro Name [takes p, q]` … `//! endtextmacro`
// blocks from src, populates res.Macros, and returns the source with those
// blocks removed (replaced by a single blank line so line numbers downstream
// stay close to the original).
//
// Nested definitions are NOT supported (JassHelper itself forbids them at
// the same scope) — an inner `//! textmacro` inside a body is treated as
// part of the outer body. Maps in the wild don't nest definitions; the few
// that try are pathological.
func extractDefinitions(src string, res *PreprocessResult) string {
	var out strings.Builder
	lines := splitLinesKeepEnd(src)
	i := 0
	for i < len(lines) {
		line := lines[i]
		m := textmacroDefRe.FindStringSubmatch(strings.TrimRight(line, "\r\n"))
		if m == nil {
			out.WriteString(line)
			i++
			continue
		}
		// Found a definition opener. Collect body until endtextmacro.
		name := m[1]
		var params []string
		if rawParams := strings.TrimSpace(m[2]); rawParams != "" {
			for _, p := range strings.Split(rawParams, ",") {
				if t := strings.TrimSpace(p); t != "" {
					params = append(params, t)
				}
			}
		}
		defLine := i + 1
		var body strings.Builder
		i++ // step past the opener
		closed := false
		for i < len(lines) {
			inner := lines[i]
			if endtextmacroRe.MatchString(strings.TrimRight(inner, "\r\n")) {
				closed = true
				i++ // consume the endtextmacro
				break
			}
			body.WriteString(inner)
			i++
		}
		if !closed {
			res.Errors = append(res.Errors, PreprocessError{
				Line:    defLine,
				Message: fmt.Sprintf("textmacro %q: missing //! endtextmacro before EOF", name),
			})
			// Fall back to keeping the body in the output so users can see
			// what went wrong; the source already accumulated into `body`.
		}
		res.Macros[name] = Macro{
			Name:   name,
			Params: params,
			Body:   body.String(),
			Line:   defLine,
		}
		// Emit a single blank line so subsequent line-number-based errors
		// still roughly align with the user's source view.
		out.WriteString("\n")
	}
	return out.String()
}

// expandCalls scans src for runtextmacro / runtextmacroonce call sites and
// substitutes the expansion in-place. depth bounds recursion (a macro body
// can contain runtextmacro lines that trigger further expansion).
func expandCalls(src string, macros map[string]Macro, onceFired map[string]bool, res *PreprocessResult, depth int) string {
	if depth > maxExpandDepth {
		res.Errors = append(res.Errors, PreprocessError{
			Line:    0,
			Message: fmt.Sprintf("textmacro expansion exceeded depth %d — likely infinite recursion", maxExpandDepth),
		})
		return src + "\n// jass2lua: textmacro recursion limit reached\n"
	}
	var out strings.Builder
	lines := splitLinesKeepEnd(src)
	for idx, line := range lines {
		trimmed := strings.TrimRight(line, "\r\n")
		// runtextmacro / runtextmacroonce.
		if m := runtextmacroCallRe.FindStringSubmatch(trimmed); m != nil {
			once := m[1] == "once"
			name := m[2]
			args := parseCallArgs(m[3])
			call := MacroCall{Name: name, Args: args, Line: idx + 1, Once: once}
			res.Calls = append(res.Calls, call)

			mac, ok := macros[name]
			if !ok {
				res.Errors = append(res.Errors, PreprocessError{
					Line:    idx + 1,
					Message: fmt.Sprintf("runtextmacro %q: unknown macro", name),
				})
				out.WriteString(fmt.Sprintf("// jass2lua: unknown textmacro %q\n", name))
				continue
			}
			if len(args) != len(mac.Params) {
				res.Errors = append(res.Errors, PreprocessError{
					Line:    idx + 1,
					Message: fmt.Sprintf("runtextmacro %q: arg count %d does not match param count %d", name, len(args), len(mac.Params)),
				})
				out.WriteString(fmt.Sprintf("// jass2lua: arg count mismatch for %q\n", name))
				continue
			}
			if once {
				key := name + "|" + strings.Join(args, "|")
				if onceFired[key] {
					out.WriteString(fmt.Sprintf("// runtextmacroonce suppressed: %s(%s)\n", name, strings.Join(args, ", ")))
					continue
				}
				onceFired[key] = true
			}
			body := substituteParams(mac.Body, mac.Params, args)
			// Recurse: the substituted body may itself contain runtextmacro
			// lines that need expanding.
			body = expandCalls(body, macros, onceFired, res, depth+1)
			out.WriteString(body)
			// Ensure the expansion ends with a newline so downstream tokens
			// don't run together with the next source line.
			if !strings.HasSuffix(body, "\n") {
				out.WriteByte('\n')
			}
			continue
		}
		// Any other `//!` directive (e.g. `//! i …`, `//! external …`) —
		// pass through as a regular comment + diagnostic.
		if dm := otherDirectiveRe.FindStringSubmatch(trimmed); dm != nil {
			// Filter out the directives the matcher already handled in the
			// definition-extraction pass (textmacro / endtextmacro) and our
			// own call directives (runtextmacro*).
			switch dm[1] {
			case "textmacro", "endtextmacro", "runtextmacro", "runtextmacroonce":
				// Should not happen after extractDefinitions stripped the
				// definitions — but if a stray opener slipped through (e.g.
				// missing endtextmacro that we left in the source), pass it
				// through silently.
				out.WriteString(line)
				continue
			case "if", "else", "elseif", "endif":
				// Should not happen after stripConditionals consumed them; if
				// one slipped through (stray closer, etc.), pass it silently.
				out.WriteString(line)
				continue
			}
			// Don't add an error for very common pragma-style directives —
			// they're informational, not blockers. We still log so the UI
			// can surface them if it wants.
			res.Errors = append(res.Errors, PreprocessError{
				Line:    idx + 1,
				Message: fmt.Sprintf("unknown //! directive %q — passed through as comment", dm[1]),
			})
			// Rewrite `//!` to `//` so the downstream tokenizer treats it
			// as a plain comment rather than potentially choking. Preserve
			// the trailing newline / EOL bytes verbatim.
			rewritten := strings.Replace(line, "//!", "//", 1)
			out.WriteString(rewritten)
			continue
		}
		out.WriteString(line)
	}
	return out.String()
}

// substituteParams walks body and replaces every `$paramN$` token with its
// bound arg. Args not in the params list (i.e. literal `$thing$` in the
// source that doesn't correspond to a parameter) are preserved verbatim —
// JassHelper has the same behavior, and some maps use bare `$` literals in
// strings that we shouldn't munge.
func substituteParams(body string, params []string, args []string) string {
	if len(params) == 0 {
		return body
	}
	bind := make(map[string]string, len(params))
	for i, p := range params {
		bind[p] = args[i]
	}
	return paramSubRe.ReplaceAllStringFunc(body, func(tok string) string {
		// tok includes the leading + trailing `$`.
		inner := tok[1 : len(tok)-1]
		if v, ok := bind[inner]; ok {
			return v
		}
		return tok
	})
}

// parseCallArgs splits the argument list inside `runtextmacro Name(...)` on
// commas, respecting double-quoted strings (so `"a,b"` stays as one arg).
// JassHelper's tokenizer is similarly lax — it does NOT honor parens or
// brackets inside args; macro args are always flat strings.
func parseCallArgs(raw string) []string {
	raw = strings.TrimSpace(raw)
	if raw == "" {
		return nil
	}
	var args []string
	var cur strings.Builder
	inStr := false
	for i := 0; i < len(raw); i++ {
		c := raw[i]
		switch {
		case c == '"' && (i == 0 || raw[i-1] != '\\'):
			inStr = !inStr
			cur.WriteByte(c)
		case c == ',' && !inStr:
			args = append(args, strings.TrimSpace(cur.String()))
			cur.Reset()
		default:
			cur.WriteByte(c)
		}
	}
	if cur.Len() > 0 || len(args) > 0 {
		args = append(args, strings.TrimSpace(cur.String()))
	}
	// Strip surrounding double-quotes from each arg — JassHelper treats
	// `"foo"` and `foo` interchangeably for substitution, and stripping is
	// what produces the expected literal text in the expansion.
	for i, a := range args {
		if len(a) >= 2 && a[0] == '"' && a[len(a)-1] == '"' {
			args[i] = a[1 : len(a)-1]
		}
	}
	return args
}

// stripConditionals walks the source line-by-line, removing `//! if`,
// `//! else`, `//! elseif`, and `//! endif` directives and selecting the
// "if" branch's body verbatim. Nested conditionals are supported via a state
// stack — when we're inside a `dropped` arm, all nested arms also drop until
// the outermost endif.
//
// Semantics:
//   - `//! if SOMETHING` opens a new conditional; condition ignored, if-body kept.
//   - `//! else` switches the current conditional to the dropped arm.
//   - `//! elseif COND` is treated like `//! else` (always drop) since we
//     already took the if branch.
//   - `//! endif` closes the innermost conditional.
//
// Unmatched openers (no endif before EOF) get a PreprocessError; whatever was
// captured before EOF is included so the user still sees partial output.
func stripConditionals(src string, res *PreprocessResult) string {
	if !strings.Contains(src, "//!") {
		return src
	}
	var out strings.Builder
	out.Grow(len(src))
	lines := splitLinesKeepEnd(src)
	// Stack entries: false = currently keeping lines (if-branch); true =
	// currently dropping (else/elseif-branch). When a nested conditional
	// appears inside a dropped arm, the inner conditional is itself dropped
	// regardless of its own arm — propagate via "drop wins".
	type arm struct {
		dropping bool
		line     int // opener line for diagnostics
	}
	var stack []arm
	keeping := func() bool {
		for _, a := range stack {
			if a.dropping {
				return false
			}
		}
		return true
	}
	for idx, line := range lines {
		trim := strings.TrimRight(line, "\r\n")
		if m := condIfRe.FindStringSubmatch(trim); m != nil {
			// If we're already dropping, the nested arm is dropped regardless.
			if !keeping() {
				stack = append(stack, arm{dropping: true, line: idx + 1})
				continue
			}
			_ = m // condition consumed but ignored
			stack = append(stack, arm{dropping: false, line: idx + 1})
			continue
		}
		if condElseRe.MatchString(trim) || condElseifRe.MatchString(trim) {
			if len(stack) == 0 {
				res.Errors = append(res.Errors, PreprocessError{
					Line:    idx + 1,
					Message: "//! else / //! elseif without matching //! if; ignoring",
				})
				continue
			}
			// Flip the innermost arm to dropping. Once dropped, stays dropped.
			stack[len(stack)-1].dropping = true
			continue
		}
		if condEndifRe.MatchString(trim) {
			if len(stack) == 0 {
				res.Errors = append(res.Errors, PreprocessError{
					Line:    idx + 1,
					Message: "//! endif without matching //! if; ignoring",
				})
				continue
			}
			stack = stack[:len(stack)-1]
			continue
		}
		// Regular line: emit only if we're not in a dropped arm.
		if keeping() {
			out.WriteString(line)
		} else {
			// Preserve newline so line numbers stay aligned.
			if strings.HasSuffix(line, "\n") {
				out.WriteByte('\n')
			}
		}
	}
	// Any unclosed conditionals at EOF: surface a diagnostic.
	for _, a := range stack {
		res.Errors = append(res.Errors, PreprocessError{
			Line:    a.line,
			Message: "//! if: missing //! endif before EOF",
		})
	}
	return out.String()
}

// splitLinesKeepEnd splits s on '\n' boundaries, keeping the '\n' attached
// to each resulting line. This preserves byte-exact round-tripping for lines
// we pass through unchanged.
func splitLinesKeepEnd(s string) []string {
	if s == "" {
		return nil
	}
	var out []string
	start := 0
	for i := 0; i < len(s); i++ {
		if s[i] == '\n' {
			out = append(out, s[start:i+1])
			start = i + 1
		}
	}
	if start < len(s) {
		out = append(out, s[start:])
	}
	return out
}
