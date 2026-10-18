---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/libscope.go"
ref: HEAD
---

package jass2lua

// libscope.go — vJASS library + scope preprocessor (Phase 2 of vJASS support).
//
// JassHelper supports two "namespace wrapper" constructs:
//
//	library Foo [requires|needs|uses Bar, Baz] [initializer Init]
//	    [private|public] <decls>
//	endlibrary
//
//	scope Foo
//	    <decls>           // implicitly private-by-default
//	endscope
//
// Both wrap a group of decls. Libraries publish their `public` decls to the
// outer namespace (mangled with a single underscore: `Foo_public_thing`) and
// keep `private` ones inside the block (mangled with three underscores:
// `Foo___private_thing` — three underscores so the name can't collide with
// the public convention). Scope is the private-only variant of library —
// every decl is automatically `Foo___thing`. Scopes can't be `require`d.
//
// This file implements that preprocessor pass. After Phase 2 runs, a map
// whose only vJASS surface was library + scope (the bulk of vJASS in the
// wild — ~70-80% of public-vault content) flows through the transpiler
// cleanly. Surviving keywords (struct, module, interface, define) stay as
// blockers and get unlocked by Phases 3/4.
//
// Pipeline placement:
//
//	raw JASS
//	    → Preprocess           (textmacro expansion — Phase 1)
//	    → PreprocessLibScope   (this file — Phase 2)
//	    → scanVJASS            (struct/module/interface/define gate)
//	    → blocker OR transpile
//
// Conservative on errors: unresolved `requires`, name collisions, malformed
// visibility keywords, cycles — every one produces a LibScopeError + a
// comment marker in the output, but processing continues. Matches Phase 1's
// non-fatal style.

import (
	"fmt"
	"regexp"
	"sort"
	"strings"
)

// LibScopeResult is the output of the library/scope preprocessor.
type LibScopeResult struct {
	// Expanded is the source with library/scope blocks resolved: wrapper
	// keywords stripped, declarations renamed to their mangled forms,
	// references inside the block rewritten. The result is flat JASS that
	// the downstream transpiler can consume without further vJASS knowledge.
	Expanded string

	// InitOrder lists every library, sorted into a dependency-respecting
	// order (libraries with no requires come first; libraries listed in a
	// requires clause come before the libraries that require them). The
	// transpiler/codegen consults this to emit init-function calls at map
	// start in the right order.
	//
	// Scopes are NOT listed here — they have no initializer surface and
	// can't be required.
	InitOrder []LibraryInit

	// PublicNames is the flat set of every mangled public symbol that
	// libraries export to the outer scope (e.g. "Foo_Bar"). The struct
	// preprocessor consults this for the dot-vs-colon receiver heuristic:
	// a method call whose receiver is a known library public name stays
	// dot. Private mangled names are NOT included (they're not used as
	// receivers outside their library by definition).
	PublicNames []string

	// Errors lists non-fatal diagnostics surfaced to the UI as warnings.
	Errors []LibScopeError
}

// LibraryInit describes one library after Phase 2 resolution. Public/Private
// list the post-mangling symbol names so the codegen + downstream tooling
// can introspect what each library exposed.
type LibraryInit struct {
	Name     string   // library name as written in source
	InitFunc string   // initializer function name (empty if none declared)
	Public   []string // public symbol names (post-mangling: still "Name_thing")
	Private  []string // private symbol names (post-mangling: "Name___thing")
}

// LibScopeError is one non-fatal diagnostic from the library/scope pass.
// Line is 1-based; 0 when we couldn't pin it to a specific line.
type LibScopeError struct {
	Line    int
	Message string
}

// ---------------------------------------------------------------------------
// Regex helpers — opener / closer matchers for library + scope blocks.
// ---------------------------------------------------------------------------

// libraryOpenerRe matches the various `library` opener shapes:
//
//	library Foo
//	library Foo initializer Init
//	library Foo requires Bar, Baz
//	library Foo requires optional Bar, Baz initializer Init
//	library Foo needs Bar uses Baz       (synonyms for requires; mixed-case use is rare)
//
// JassHelper accepts "library Foo_Init", "library Foo  initializer  Init",
// `requires`/`needs`/`uses` interchangeably, and the `optional` qualifier on
// individual requires. We keep this regex permissive on whitespace; the
// keyword-tail (everything after the name) is fed through requireSplitterRe
// + initFuncRe for the structured pieces.
var libraryOpenerRe = regexp.MustCompile(`^\s*library\s+([A-Za-z_][A-Za-z0-9_]*)\s*(.*)$`)

// libraryCloserRe matches `endlibrary` (with optional leading whitespace).
var libraryCloserRe = regexp.MustCompile(`^\s*endlibrary\s*$`)

// scopeOpenerRe matches `scope Foo` (and the very rare `scope Foo initializer
// Init` — JassHelper allows this even though scopes are private-only).
var scopeOpenerRe = regexp.MustCompile(`^\s*scope\s+([A-Za-z_][A-Za-z0-9_]*)\s*(.*)$`)

// scopeCloserRe matches `endscope`.
var scopeCloserRe = regexp.MustCompile(`^\s*endscope\s*$`)

// initFuncRe extracts the `initializer Foo` clause from a library/scope
// opener's tail. Group 1 is the init function name.
var initFuncRe = regexp.MustCompile(`\binitializer\s+([A-Za-z_][A-Za-z0-9_]*)`)

// requireClauseRe matches `requires`, `needs`, or `uses` plus their tail
// (comma-separated list, possibly with `optional` qualifiers). We capture
// only the tail; parseRequireList walks it.
var requireClauseRe = regexp.MustCompile(`\b(?:requires|needs|uses)\s+([^\n]+?)(?:\s+initializer\b|$)`)

// A multi-line `requires` clause leaves dangling lines in the library body.
// JassHelper lets the opener interleave /* */ comments to document each
// requirement, e.g.
//
//	library Foo /* banner */ requires /* doc */ optional A, /* doc */ optional B
//
// After StripBlockComments blanks the comments (preserving newlines), the
// `requires` keyword and each `optional Name` item land on separate body lines
// instead of the opener tail, so parseOpenerTail never sees them. None of these
// tokens can begin a valid JASS statement, so we drop such lines from the body
// (replacing with a marker to keep line numbers stable) before transpiling.
var (
	danglingRequiresRe = regexp.MustCompile(`^\s*(?:requires|needs|uses)\b`)
	danglingOptionalRe = regexp.MustCompile(`^\s*optional\s+[A-Za-z_][A-Za-z0-9_]*\s*,?\s*$`)
)

// privatePublicRe matches a `private` or `public` visibility prefix on a
// decl line. Group 1 is the keyword; the rest of the line is the bare decl.
//
// Targets:
//
//	private function foo takes …
//	public  integer x = 0           (inside globals)
//	public  constant integer Y = 1
//
// We deliberately leave more exotic visibility forms (per-decl modifiers
// outside globals/function/native) to fall through unmodified — they're rare
// and the safer behavior is to round-trip them as comments rather than
// guess wrong.
var privatePublicRe = regexp.MustCompile(`^(\s*)(private|public)\s+(.*)$`)

// declAtTopOfBlockRe matches a function-declaration line so we can detect
// the decl name (used when the line carries no `private`/`public` prefix —
// scope decls are private-by-default but library decls without a prefix are
// already at outer-namespace scope). Group 1 = function name.
var funcDeclRe = regexp.MustCompile(`^\s*(?:constant\s+)?function\s+([A-Za-z_][A-Za-z0-9_]*)`)

// globalDeclRe matches one decl line inside a globals block. Group 1 is
// the optional `constant`, group 2 the type, group 3 the name. We don't
// care about array sizes / initializers for the rename pass.
var globalDeclRe = regexp.MustCompile(`^\s*(constant\s+)?([A-Za-z_][A-Za-z0-9_]*)\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)`)

// ---------------------------------------------------------------------------
// PreprocessLibScope — entry point.
// ---------------------------------------------------------------------------

// PreprocessLibScope handles vJASS library + scope blocks. Call it AFTER the
// textmacro preprocessor and BEFORE the vJASS keyword scan / transpiler.
//
// What it does, in order:
//
//  1. Scans the source for library/scope blocks (line-based; handles nested
//     scopes inside libraries — rare but legal).
//  2. For each block, walks its body to collect declared decl names tagged
//     with visibility (public/private). Library decls without a `private`
//     prefix stay at outer scope (no mangling). Scope decls are
//     private-by-default.
//  3. Builds a mangling table:
//         private/scope decl   `foo` → `<Block>___foo`
//         public library decl  `bar` → `<Block>_bar`
//     and rewrites every reference inside the block body using the JASS
//     tokenizer (so references inside comments + strings are NOT mangled).
//  4. Topologically sorts the libraries by their `requires` graph.
//     `requires optional X` is dropped from the graph when X doesn't exist
//     (the library still runs). Cycles emit all involved libraries + a
//     LibScopeError; the order within the cycle is arbitrary but stable
//     (alphabetical for determinism).
//  5. Strips the `library`/`scope`/`endlibrary`/`endscope` keywords from
//     the source.
//
// Top-level decls (outside any library/scope) are emitted verbatim with no
// mangling — they're already global.
func PreprocessLibScope(src string) LibScopeResult {
	res := LibScopeResult{}
	lines := splitLinesKeepEnd(src)

	// Pass 1 — locate every block + extract its body. Nested scope-in-library
	// is supported by maintaining a stack of "in-flight" blocks.
	blocks, topLevel, parseErrs := extractLibScopeBlocks(lines)
	res.Errors = append(res.Errors, parseErrs...)

	// Pass 2 — for each block, build the mangling table + rewrite its body.
	known := map[string]*libBlock{}
	for _, b := range blocks {
		if b.kind == blockLibrary {
			known[b.name] = b
		}
	}
	for _, b := range blocks {
		buildSymbolTable(b, &res)
	}
	for _, b := range blocks {
		rewriteBlockBody(b, &res)
	}

	// Pass 3 — topological sort over the library requires graph.
	ordered := topoSortLibraries(blocks, known, &res)
	for _, b := range ordered {
		init := LibraryInit{
			Name:     b.name,
			InitFunc: b.initFunc,
			Public:   sortedNames(b.publicNames),
			Private:  sortedNames(b.privateNames),
		}
		res.InitOrder = append(res.InitOrder, init)
		// Flat surface for the struct preprocessor's dot-vs-colon heuristic.
		res.PublicNames = append(res.PublicNames, init.Public...)
	}

	// Pass 4 — emit. Top-level lines stay verbatim; library/scope blocks emit
	// their rewritten body with a small "begin/end namespace" marker comment
	// so the output is grep-friendly.
	res.Expanded = assembleOutput(lines, topLevel, blocks, ordered)
	return res
}

// ---------------------------------------------------------------------------
// Block extraction.
// ---------------------------------------------------------------------------

type blockKind int

const (
	blockLibrary blockKind = iota
	blockScope
)

// libBlock is one parsed library or scope, with the body lines collected.
// requires + initFunc only populate for library blocks.
type libBlock struct {
	kind     blockKind
	name     string
	initFunc string

	// requires: each element is a (name, optional) pair. We keep them ordered
	// for stable output.
	requires []libRequire

	// bodyLines is the raw line-stream (with EOL bytes attached) for the
	// block's body — i.e. everything between the opener line and the closer
	// line, EXCLUDING those keyword lines.
	bodyLines []string

	// rewrittenBody is filled in by rewriteBlockBody.
	rewrittenBody string

	// publicNames maps the original decl name → mangled name (Block_name).
	// privateNames maps original → mangled (Block___name).
	// scopeDecls (only for scope blocks) maps original → mangled (Block___name).
	publicNames  map[string]string
	privateNames map[string]string

	// startLine is the 1-based line of the opener (for diagnostics).
	startLine int

	// idx is the position of the block in the source order — used as the
	// secondary key in topological sort for deterministic ordering.
	idx int
}

type libRequire struct {
	name     string
	optional bool
}

// extractLibScopeBlocks walks the line stream, identifying library/scope
// blocks. Returns the list of blocks (each with its body lines) plus the
// list of "outer" line indices — indices into the input `lines` for lines
// that are NOT inside any block (those pass through verbatim). On
// unrecoverable error (e.g. missing endlibrary at EOF) the block is still
// returned with whatever body was collected so far, plus a LibScopeError.
//
// Nesting policy: a scope can live inside a library; a library inside a
// library is treated as flat (the inner library's opener is just text — vJASS
// itself errors on this and we don't try to be more clever). Nested scopes
// inside scopes are similarly flat.
func extractLibScopeBlocks(lines []string) ([]*libBlock, []int, []LibScopeError) {
	var blocks []*libBlock
	var topLevel []int
	var errs []LibScopeError

	// Stack of "currently open" blocks. We support nesting one level deep
	// (scope inside library) — beyond that, the inner opener falls through
	// as body text.
	var stack []*libBlock

	for i := 0; i < len(lines); i++ {
		line := strings.TrimRight(lines[i], "\r\n")

		// If we're inside a block, prioritize matching its closer.
		if len(stack) > 0 {
			top := stack[len(stack)-1]
			closed := false
			switch top.kind {
			case blockLibrary:
				if libraryCloserRe.MatchString(line) {
					closed = true
				}
			case blockScope:
				if scopeCloserRe.MatchString(line) {
					closed = true
				}
			}
			if closed {
				stack = stack[:len(stack)-1]
				continue
			}

			// Check for a nested scope inside a library (legal).
			if top.kind == blockLibrary {
				if m := scopeOpenerRe.FindStringSubmatch(line); m != nil {
					child := &libBlock{
						kind:      blockScope,
						name:      m[1],
						startLine: i + 1,
						idx:       len(blocks),
					}
					parseOpenerTail(child, m[2])
					blocks = append(blocks, child)
					stack = append(stack, child)
					continue
				}
				// Nested library opener inside a library: not supported.
				// Surface a diagnostic, fall through as body text.
				if m := libraryOpenerRe.FindStringSubmatch(line); m != nil {
					errs = append(errs, LibScopeError{
						Line:    i + 1,
						Message: fmt.Sprintf("nested library %q inside library %q is not supported; flattening", m[1], top.name),
					})
					top.bodyLines = append(top.bodyLines, lines[i])
					continue
				}
			}

			// Regular body line.
			top.bodyLines = append(top.bodyLines, lines[i])
			continue
		}

		// Not inside any block — look for an opener.
		if m := libraryOpenerRe.FindStringSubmatch(line); m != nil {
			b := &libBlock{
				kind:      blockLibrary,
				name:      m[1],
				startLine: i + 1,
				idx:       len(blocks),
			}
			parseOpenerTail(b, m[2])
			blocks = append(blocks, b)
			stack = append(stack, b)
			continue
		}
		if m := scopeOpenerRe.FindStringSubmatch(line); m != nil {
			b := &libBlock{
				kind:      blockScope,
				name:      m[1],
				startLine: i + 1,
				idx:       len(blocks),
			}
			parseOpenerTail(b, m[2])
			blocks = append(blocks, b)
			stack = append(stack, b)
			continue
		}
		// Stray endlibrary / endscope at top level — diagnostic + skip.
		if libraryCloserRe.MatchString(line) {
			errs = append(errs, LibScopeError{
				Line:    i + 1,
				Message: "stray `endlibrary` outside any library block; skipping",
			})
			continue
		}
		if scopeCloserRe.MatchString(line) {
			errs = append(errs, LibScopeError{
				Line:    i + 1,
				Message: "stray `endscope` outside any scope block; skipping",
			})
			continue
		}
		// Plain top-level line.
		topLevel = append(topLevel, i)
	}

	// Any blocks left on the stack at EOF are unterminated — surface a
	// diagnostic but keep what we collected so the output is still useful.
	for _, b := range stack {
		errs = append(errs, LibScopeError{
			Line:    b.startLine,
			Message: fmt.Sprintf("%s %q: missing `end%s` before EOF", blockKindName(b.kind), b.name, blockKindName(b.kind)),
		})
	}

	// Attach topLevel as raw indices; the caller looks them up against `lines`.
	return blocks, topLevel, errs
}

func blockKindName(k blockKind) string {
	if k == blockLibrary {
		return "library"
	}
	return "scope"
}

// parseOpenerTail walks the after-name portion of a library/scope opener
// line, populating b.initFunc and b.requires as appropriate.
func parseOpenerTail(b *libBlock, tail string) {
	tail = strings.TrimSpace(tail)
	if tail == "" {
		return
	}
	if m := initFuncRe.FindStringSubmatch(tail); m != nil {
		b.initFunc = m[1]
	}
	if b.kind == blockLibrary {
		if m := requireClauseRe.FindStringSubmatch(tail); m != nil {
			b.requires = parseRequireList(m[1])
		}
	}
}

// parseRequireList splits a comma-separated list of require entries,
// honoring the `optional` qualifier per entry. Examples:
//
//	"Bar, Baz"               → [{Bar, false}, {Baz, false}]
//	"optional Bar"           → [{Bar, true}]
//	"Bar, optional Baz, Qux" → [{Bar, false}, {Baz, true}, {Qux, false}]
func parseRequireList(raw string) []libRequire {
	var out []libRequire
	for _, part := range strings.Split(raw, ",") {
		p := strings.TrimSpace(part)
		if p == "" {
			continue
		}
		optional := false
		if strings.HasPrefix(p, "optional ") {
			optional = true
			p = strings.TrimSpace(strings.TrimPrefix(p, "optional"))
		}
		// Strip trailing junk past the identifier (e.g. an inline initializer
		// keyword the requireClauseRe failed to slice cleanly).
		if idx := strings.IndexFunc(p, func(r rune) bool {
			return !(r == '_' || (r >= 'a' && r <= 'z') || (r >= 'A' && r <= 'Z') || (r >= '0' && r <= '9'))
		}); idx >= 0 {
			p = p[:idx]
		}
		if p == "" {
			continue
		}
		out = append(out, libRequire{name: p, optional: optional})
	}
	return out
}

// ---------------------------------------------------------------------------
// Symbol-table construction.
// ---------------------------------------------------------------------------

// buildSymbolTable walks the block body and records every decl name with
// its visibility. Mangling is computed eagerly so rewriteBlockBody can do
// a straight lookup.
//
// Visibility rules:
//   - scope block: every decl is private (mangled `Block___name`).
//   - library block:
//       * `private function foo`        → private
//       * `public function bar`         → public
//       * `function baz` (no prefix)    → top-level (no mangling — stays a global)
//       * inside `globals`:
//             `private integer x`       → private
//             `public integer y`        → public
//             `integer z`               → top-level (no mangling)
//
// We deliberately handle the common forms only — exotic forms (e.g. a
// `public` on a `type` decl) round-trip unchanged. Real-world maps lean
// almost entirely on the function + globals forms.
func buildSymbolTable(b *libBlock, res *LibScopeResult) {
	b.publicNames = map[string]string{}
	b.privateNames = map[string]string{}
	inGlobals := false
	for _, line := range b.bodyLines {
		trim := strings.TrimRight(line, "\r\n")
		bare := strings.TrimSpace(trim)
		if bare == "globals" {
			inGlobals = true
			continue
		}
		if bare == "endglobals" {
			inGlobals = false
			continue
		}
		// Visibility prefix?
		if m := privatePublicRe.FindStringSubmatch(trim); m != nil {
			vis := m[2]
			rest := m[3]
			name := extractDeclName(rest, inGlobals)
			if name == "" {
				continue
			}
			b.assignSymbol(name, vis == "public")
			continue
		}
		// No visibility prefix — for scope blocks, treat as private. For
		// library blocks, this is a top-level decl (no mangling).
		if b.kind == blockScope {
			name := extractDeclName(trim, inGlobals)
			if name == "" {
				continue
			}
			b.assignSymbol(name, false)
		}
		_ = res
	}
}

// assignSymbol records (and mangles) one decl name on the block. Collisions
// (the same logical name declared twice with the same visibility) are
// allowed — the mangled name is identical anyway. Cross-visibility collisions
// (`private foo` + `public foo` in the same block) keep the first definition.
func (b *libBlock) assignSymbol(name string, public bool) {
	if _, ok := b.publicNames[name]; ok {
		return
	}
	if _, ok := b.privateNames[name]; ok {
		return
	}
	if public {
		b.publicNames[name] = b.name + "_" + name
	} else {
		b.privateNames[name] = b.name + "___" + name
	}
}

// extractDeclName pulls the declared identifier from a single line. For
// non-globals lines we look for `function`/`constant function`/`native`.
// For globals-context lines we expect `[constant] <type> [array] <name>`.
// Returns "" when the line isn't a decl we can handle.
func extractDeclName(line string, inGlobals bool) string {
	if inGlobals {
		if m := globalDeclRe.FindStringSubmatch(line); m != nil {
			return m[3]
		}
		return ""
	}
	if m := funcDeclRe.FindStringSubmatch(line); m != nil {
		return m[1]
	}
	return ""
}

// ---------------------------------------------------------------------------
// Body rewrite.
// ---------------------------------------------------------------------------

// rewriteBlockBody tokenizes the block's body and rewrites identifier
// references that match one of the block's declared symbols. The decl lines
// themselves also get their declared name rewritten (so `public function
// foo` becomes `function Block_foo`). The `private`/`public` keyword is
// stripped on the way out.
//
// Strings + comments are NOT rewritten (the tokenizer skips them — we walk
// the raw bytes guided by token positions).
//
// On lex error we fall back to a regex-based rename that at least preserves
// the source; the diagnostic surfaces in res.Errors.
func rewriteBlockBody(b *libBlock, res *LibScopeResult) {
	// Drop dangling multi-line `requires` clause lines (see danglingRequiresRe).
	for idx, ln := range b.bodyLines {
		t := strings.TrimSpace(stripInlineComment(strings.TrimRight(ln, "\r\n")))
		if danglingRequiresRe.MatchString(t) || danglingOptionalRe.MatchString(t) {
			b.bodyLines[idx] = "// jass2lua: requires-clause continuation dropped\n"
		}
	}
	body := strings.Join(b.bodyLines, "")
	// Strip visibility keywords + rename the immediate decl on the same line.
	// We do this line-by-line BEFORE the token-based reference rewrite — the
	// decl rename is simpler at the syntactic level and the token rewrite
	// catches everything else (calls, set-targets, expression refs).
	body = renameVisibilityDecls(body, b)
	// Now rewrite all references to mangled names using the tokenizer (so
	// string contents + comments are preserved). Lex failure falls back to a
	// best-effort substitution.
	rewritten, err := tokenRewriteRefs(body, b)
	if err != nil {
		res.Errors = append(res.Errors, LibScopeError{
			Line:    b.startLine,
			Message: fmt.Sprintf("%s %q: lex error during reference rewrite (%v); falling back to coarse substitution", blockKindName(b.kind), b.name, err),
		})
		rewritten = regexRewriteRefs(body, b)
	}
	b.rewrittenBody = rewritten
}

// renameVisibilityDecls rewrites the decl-introducing lines. It strips the
// `private`/`public` keyword and substitutes the declared name with its
// mangled form. For scope blocks (no visibility keywords on lines), it
// substitutes the decl-name on every recognized decl line.
//
// We operate line-by-line because the visibility keyword + the decl name
// always live on the same line in JASS / vJASS (function signatures span
// multiple lines, but the name is on the opener).
func renameVisibilityDecls(body string, b *libBlock) string {
	lines := splitLinesKeepEnd(body)
	var out strings.Builder
	inGlobals := false
	for _, line := range lines {
		trim := strings.TrimRight(line, "\r\n")
		bare := strings.TrimSpace(trim)
		eol := line[len(trim):]
		if bare == "globals" {
			inGlobals = true
			out.WriteString(line)
			continue
		}
		if bare == "endglobals" {
			inGlobals = false
			out.WriteString(line)
			continue
		}
		if m := privatePublicRe.FindStringSubmatch(trim); m != nil {
			indent := m[1]
			vis := m[2]
			rest := m[3]
			name := extractDeclName(rest, inGlobals)
			rewrittenRest := rest
			if name != "" {
				var mangled string
				if vis == "public" {
					mangled = b.publicNames[name]
				} else {
					mangled = b.privateNames[name]
				}
				if mangled != "" {
					rewrittenRest = replaceFirstIdent(rest, name, mangled)
				}
			}
			out.WriteString(indent + rewrittenRest + eol)
			continue
		}
		// Scope: rename the decl name on the line (no visibility keyword).
		if b.kind == blockScope {
			name := extractDeclName(trim, inGlobals)
			if name != "" {
				if mangled := b.privateNames[name]; mangled != "" {
					rewritten := replaceFirstIdent(trim, name, mangled)
					out.WriteString(rewritten + eol)
					continue
				}
			}
		}
		out.WriteString(line)
	}
	return out.String()
}

// replaceFirstIdent replaces the first occurrence of `name` in `s` (matched
// as a whole-word identifier) with `replacement`. Used for the decl-name
// substitution where we know the name appears at most once on the line.
func replaceFirstIdent(s, name, replacement string) string {
	idx := indexIdentInString(s, name)
	if idx < 0 {
		return s
	}
	return s[:idx] + replacement + s[idx+len(name):]
}

// indexIdentInString returns the byte index of the first whole-word
// occurrence of `name` in `s`, or -1 if absent. Whole-word means the
// preceding + following byte (if any) must not be ident-cont.
func indexIdentInString(s, name string) int {
	start := 0
	for start < len(s) {
		idx := strings.Index(s[start:], name)
		if idx < 0 {
			return -1
		}
		pos := start + idx
		before := byte(0)
		after := byte(0)
		if pos > 0 {
			before = s[pos-1]
		}
		if pos+len(name) < len(s) {
			after = s[pos+len(name)]
		}
		if !isIdentCont(before) && !isIdentCont(after) {
			return pos
		}
		start = pos + 1
	}
	return -1
}

// tokenRewriteRefs uses the JASS tokenizer to walk body and rewrite every
// TokIdent whose value matches one of the block's declared names. Strings,
// comments, rawcodes, numbers, and operators all pass through verbatim.
//
// We build the output by interleaving: for each token, copy any source bytes
// preceding it (whitespace, newlines, etc), then either emit the token's
// rewritten value or the original token text.
func tokenRewriteRefs(body string, b *libBlock) (string, error) {
	toks, err := Tokenize(body)
	if err != nil {
		return "", err
	}
	// Build a quick lookup: original → mangled.
	rename := make(map[string]string, len(b.publicNames)+len(b.privateNames))
	for k, v := range b.publicNames {
		rename[k] = v
	}
	for k, v := range b.privateNames {
		rename[k] = v
	}
	if len(rename) == 0 {
		// Nothing to rewrite. Avoid the byte-walk cost.
		return body, nil
	}
	// Walk the source by tracking line/col positions ourselves. The lexer
	// emits Line/Col but no byte offset, so we maintain a running cursor and
	// step it forward to each token's position. Newlines + horizontal
	// whitespace are echoed; tokens are either echoed or rewritten.
	var out strings.Builder
	out.Grow(len(body))
	pos := 0
	line, col := 1, 1
	for _, t := range toks {
		if t.Kind == TokEOF {
			// Emit any trailing bytes after the last token.
			if pos < len(body) {
				out.WriteString(body[pos:])
			}
			break
		}
		// Advance the cursor to the token's start.
		for pos < len(body) && (line < t.Line || (line == t.Line && col < t.Col)) {
			c := body[pos]
			out.WriteByte(c)
			pos++
			if c == '\n' {
				line++
				col = 1
			} else {
				col++
			}
		}
		// Determine the token's source length so we can step the cursor past
		// it. For most tokens the lexer's Value matches the source bytes 1:1.
		// Strings, rawcodes, hex-dollar literals, and comments don't — for
		// those we re-derive the length from the source bytes.
		srcLen := tokenSourceLen(body, pos, t)
		if srcLen <= 0 {
			// Defensive fallback — shouldn't happen for well-formed input.
			srcLen = len(t.Value)
			if srcLen == 0 {
				srcLen = 1
			}
		}
		// Decide what to emit.
		switch t.Kind {
		case TokIdent:
			if mangled, ok := rename[t.Value]; ok {
				out.WriteString(mangled)
			} else {
				out.WriteString(body[pos : pos+srcLen])
			}
		default:
			out.WriteString(body[pos : pos+srcLen])
		}
		// Step the cursor past the token in the source.
		for k := 0; k < srcLen && pos < len(body); k++ {
			c := body[pos]
			pos++
			if c == '\n' {
				line++
				col = 1
			} else {
				col++
			}
		}
	}
	// Defensive: copy any unread tail (TokEOF should have done it above but
	// the loop may have exited via the EOF branch only after at least one
	// token).
	if pos < len(body) {
		// Only copy if we didn't already exit via the EOF arm.
		out.WriteString(body[pos:])
	}
	return out.String(), nil
}

// tokenSourceLen returns the length of the source bytes corresponding to a
// token starting at `start`. For most token kinds the lexer's `Value` and
// the source byte length match; for quoted strings, rawcodes, hex-dollar
// literals, and comments they differ and we re-scan.
func tokenSourceLen(body string, start int, t Token) int {
	if start >= len(body) {
		return 0
	}
	switch t.Kind {
	case TokString:
		// Walk until the matching unescaped quote.
		if body[start] != '"' {
			return len(t.Value)
		}
		i := start + 1
		for i < len(body) {
			if body[i] == '"' && body[i-1] != '\\' {
				return i - start + 1
			}
			if body[i] == '\n' {
				return i - start // unterminated; stop at EOL
			}
			i++
		}
		return i - start
	case TokRaw:
		if body[start] != '\'' {
			return len(t.Value)
		}
		i := start + 1
		for i < len(body) && body[i] != '\'' && body[i] != '\n' {
			i++
		}
		if i < len(body) && body[i] == '\'' {
			return i - start + 1
		}
		return i - start
	case TokComment:
		// `//` + body up to newline (NOT consuming the newline).
		i := start
		for i < len(body) && body[i] != '\n' {
			i++
		}
		return i - start
	case TokInt:
		// Could be `$ABCD` hex-dollar or `0xABCD` or decimal. Walk until the
		// first non-ident-cont (and stop after the leading `$` / `0x` are
		// consumed naturally by isIdentCont).
		i := start
		if body[i] == '$' {
			i++
			for i < len(body) && isHexDigit(body[i]) {
				i++
			}
			return i - start
		}
		for i < len(body) {
			c := body[i]
			if isDigit(c) || c == 'x' || c == 'X' || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') {
				i++
				continue
			}
			break
		}
		return i - start
	case TokReal:
		i := start
		for i < len(body) {
			c := body[i]
			if isDigit(c) || c == '.' {
				i++
				continue
			}
			break
		}
		return i - start
	case TokEOL:
		// Always a single byte.
		return 1
	case TokOp:
		return len(t.Value)
	default:
		return len(t.Value)
	}
}

// regexRewriteRefs is the fallback when tokenization fails. It does a
// whole-word substitution over the body for every declared name. Strings
// and comments WILL be munged in this path (which is why we prefer the
// tokenizer-driven path) — but a lex failure means the source isn't valid
// JASS anyway, so the surviving diff is for the user's eyes only.
func regexRewriteRefs(body string, b *libBlock) string {
	type pair struct{ from, to string }
	var pairs []pair
	for k, v := range b.publicNames {
		pairs = append(pairs, pair{k, v})
	}
	for k, v := range b.privateNames {
		pairs = append(pairs, pair{k, v})
	}
	// Sort by from-length desc so longer names rewrite first (avoids
	// `Foo_bar` being partially clobbered by a later `bar` rewrite).
	sort.Slice(pairs, func(i, j int) bool { return len(pairs[i].from) > len(pairs[j].from) })
	for _, p := range pairs {
		re := regexp.MustCompile(`\b` + regexp.QuoteMeta(p.from) + `\b`)
		body = re.ReplaceAllString(body, p.to)
	}
	return body
}

// ---------------------------------------------------------------------------
// Topological sort.
// ---------------------------------------------------------------------------

// topoSortLibraries returns the libraries in dependency order: every required
// library appears before the library that requires it. Cycles emit all
// involved libraries + a LibScopeError (alphabetical ordering within the
// cycle for determinism).
//
// `requires optional X` is dropped from the graph when X doesn't exist.
// Scopes are filtered out before sorting (they have no requires + no init
// surface).
func topoSortLibraries(blocks []*libBlock, known map[string]*libBlock, res *LibScopeResult) []*libBlock {
	// Build adjacency: requires `Bar` means Bar must come before us, so we
	// add an edge Bar → Self in the dep graph and use Kahn's algorithm.
	var libs []*libBlock
	for _, b := range blocks {
		if b.kind == blockLibrary {
			libs = append(libs, b)
		}
	}
	// Stable initial order: source order. The sort below preserves this for
	// libraries with the same indegree, giving deterministic output.
	indeg := make(map[string]int, len(libs))
	adj := make(map[string][]string, len(libs))
	for _, b := range libs {
		indeg[b.name] += 0 // ensure presence
	}
	for _, b := range libs {
		for _, r := range b.requires {
			dep, exists := known[r.name]
			if !exists {
				if r.optional {
					// Dropped silently — no diagnostic.
					continue
				}
				res.Errors = append(res.Errors, LibScopeError{
					Line:    b.startLine,
					Message: fmt.Sprintf("library %q requires unknown library %q", b.name, r.name),
				})
				continue
			}
			adj[dep.name] = append(adj[dep.name], b.name)
			indeg[b.name]++
		}
	}

	// Kahn's algorithm with a min-heap-ish ordering: we always pop the
	// alphabetically smallest zero-indegree library (so output is stable
	// across runs regardless of map insertion order).
	zero := make([]string, 0, len(libs))
	for name, d := range indeg {
		if d == 0 {
			zero = append(zero, name)
		}
	}
	sort.Strings(zero)

	var orderedNames []string
	visited := map[string]bool{}
	for len(zero) > 0 {
		// Pop the lexicographically smallest zero-indegree node.
		n := zero[0]
		zero = zero[1:]
		visited[n] = true
		orderedNames = append(orderedNames, n)
		nexts := append([]string(nil), adj[n]...)
		sort.Strings(nexts)
		for _, m := range nexts {
			indeg[m]--
			if indeg[m] == 0 {
				// Insert maintaining sort.
				inserted := false
				for i := range zero {
					if zero[i] > m {
						zero = append(zero[:i], append([]string{m}, zero[i:]...)...)
						inserted = true
						break
					}
				}
				if !inserted {
					zero = append(zero, m)
				}
			}
		}
	}

	// Anything not visited is part of a cycle — emit them all + diagnostic.
	if len(orderedNames) < len(libs) {
		var cyc []string
		for _, b := range libs {
			if !visited[b.name] {
				cyc = append(cyc, b.name)
			}
		}
		sort.Strings(cyc)
		res.Errors = append(res.Errors, LibScopeError{
			Line:    0,
			Message: fmt.Sprintf("library dependency cycle detected involving: %s", strings.Join(cyc, ", ")),
		})
		orderedNames = append(orderedNames, cyc...)
	}

	// Map back to *libBlock pointers (preserving order).
	byName := map[string]*libBlock{}
	for _, b := range libs {
		byName[b.name] = b
	}
	ordered := make([]*libBlock, 0, len(libs))
	for _, n := range orderedNames {
		if b, ok := byName[n]; ok {
			ordered = append(ordered, b)
		}
	}
	return ordered
}

// ---------------------------------------------------------------------------
// Emission.
// ---------------------------------------------------------------------------

// assembleOutput rebuilds the flat post-Phase-2 source. Strategy:
//   - Top-level lines come out first, in source order (preserves leading
//     globals + free-standing functions + comments). Lines that belonged to
//     a library/scope block are NOT in topLevel.
//   - After the top-level emission, libraries emit in dependency order; each
//     gets a `-- jass2lua: begin/end library Foo` comment marker so the
//     downstream tooling can grep + the user can see where each block went.
//   - Scopes follow in source-appearance order (independent of library
//     dependencies — scopes can't be required).
//
// We hoist libraries / scopes to the bottom of the file rather than trying
// to preserve their exact original positions. JASS doesn't care about decl
// order (functions are hoisted at parse time), so the only observable
// difference is what the user sees in the diff. Hoisting also makes the
// dep-sort result obvious in the output.
func assembleOutput(lines []string, topLevel []int, blocks []*libBlock, orderedLibs []*libBlock) string {
	var out strings.Builder
	// Emit top-level lines in source order.
	for _, idx := range topLevel {
		out.WriteString(lines[idx])
	}
	// Libraries in dependency order.
	for _, b := range orderedLibs {
		out.WriteString(fmt.Sprintf("\n// jass2lua: begin library %s\n", b.name))
		out.WriteString(b.rewrittenBody)
		if !strings.HasSuffix(b.rewrittenBody, "\n") {
			out.WriteByte('\n')
		}
		out.WriteString(fmt.Sprintf("// jass2lua: end library %s\n", b.name))
	}
	// Scopes in source order.
	for _, b := range blocks {
		if b.kind != blockScope {
			continue
		}
		out.WriteString(fmt.Sprintf("\n// jass2lua: begin scope %s\n", b.name))
		out.WriteString(b.rewrittenBody)
		if !strings.HasSuffix(b.rewrittenBody, "\n") {
			out.WriteByte('\n')
		}
		out.WriteString(fmt.Sprintf("// jass2lua: end scope %s\n", b.name))
	}
	return out.String()
}

// sortedNames returns the keys of a name-map in sorted order. Used so the
// emitted LibraryInit.Public/Private slices are deterministic.
func sortedNames(m map[string]string) []string {
	if len(m) == 0 {
		return nil
	}
	out := make([]string, 0, len(m))
	for _, v := range m {
		out = append(out, v)
	}
	sort.Strings(out)
	return out
}
