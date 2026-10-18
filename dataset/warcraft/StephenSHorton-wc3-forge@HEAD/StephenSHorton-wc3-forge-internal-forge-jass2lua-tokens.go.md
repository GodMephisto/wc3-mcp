---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/tokens.go"
ref: HEAD
---

// Package jass2lua transpiles pure JASS source to Lua. It does NOT handle
// vJASS (libraries, scopes, structs, modules, textmacros, scope-private/public
// visibility, etc.) — those require the JassHelper preprocessor which is a
// separate problem.
//
// Pipeline:
//
//	tokens.go  — lexer; emits a flat []Token stream
//	parse.go   — Pratt-style expression parser + statement parser; emits an AST
//	emit.go    — AST → Lua text, with WC3-Lua-aware lowerings (FourCC literal,
//	             string concat, default initializers)
//
// The package is intentionally conservative: anything we can't parse cleanly
// becomes a `--[[ jass2lua: <reason> ]] error("jass2lua failed")` placeholder
// in the output so the user sees an actionable diff entry rather than a silent
// drop.
package jass2lua

import (
	"fmt"
	"strings"
	"unicode"
)

// TokenKind discriminates the token stream emitted by the lexer.
type TokenKind int

const (
	TokIdent      TokenKind = iota // bare identifier (variable, function, type name)
	TokKeyword                     // pure-JASS keyword (function, endfunction, if, then, etc.)
	TokType                        // type-keyword (integer, real, string, boolean, code, handle, plus user types)
	TokInt                         // integer literal (decimal or hex)
	TokReal                        // real literal
	TokString                      // quoted string literal (value is the unquoted bytes, escapes resolved)
	TokRaw                         // 'hpea'-style rawcode
	TokOp                          // operator or punctuation (one or two char)
	TokComment                     // // line comment (the // stripped, trailing newline NOT)
	TokEOL                         // end-of-line; significant in JASS statement framing
	TokEOF                         // end of input
)

// Token is one lexer output. Pos is a 1-based line for error messages.
type Token struct {
	Kind  TokenKind
	Value string
	Line  int
	Col   int
}

// pureKeywords is the canonical pure-JASS keyword set. Anything not in this
// list (but matching the vJASSKeywords list in the detector) is vJASS.
// Used by the parser's keyword classifier and indirectly by the detector
// (the detector reuses the wider vJASS list).
var pureKeywords = map[string]bool{
	"function": true, "endfunction": true,
	"if": true, "then": true, "else": true, "elseif": true, "endif": true,
	"loop": true, "endloop": true, "exitwhen": true,
	"set": true, "call": true, "return": true,
	"globals": true, "endglobals": true,
	"local": true, "constant": true, "native": true,
	"takes": true, "returns": true, "nothing": true,
	"array": true, "type": true, "extends": true,
	"true": true, "false": true, "null": true,
	"not": true, "and": true, "or": true,
	"debug": true, // pure JASS allows `debug call ...`
}

// baseTypes is the stock JASS primitive type set. User types (handle subtypes)
// are parsed as TokType too — we look them up dynamically against the
// in-flight `type Foo extends handle` decls plus a small built-in handle list.
var baseTypes = map[string]bool{
	"integer": true, "real": true, "string": true, "boolean": true,
	"code": true, "handle": true,
}

// builtinHandleTypes is a small whitelist of well-known WC3 handle types so
// the lexer can classify them as type-keywords even without seeing the
// declaration. Bigger types appear via `type Foo extends handle` in
// common.j (which we don't parse) — fall back to TokIdent for unknown
// type names; the parser tolerates that in declaration positions.
var builtinHandleTypes = map[string]bool{
	"unit": true, "player": true, "group": true, "trigger": true, "force": true,
	"location": true, "region": true, "rect": true, "destructable": true, "item": true,
	"timer": true, "timerdialog": true, "leaderboard": true, "multiboard": true,
	"camerasetup": true, "image": true, "ubersplat": true, "lightning": true, "effect": true,
	"weathereffect": true, "terraindeformation": true, "fogstate": true, "fogmodifier": true,
	"dialog": true, "button": true, "quest": true, "questitem": true, "defeatcondition": true,
	"gamecache": true, "hashtable": true, "boolexpr": true, "conditionfunc": true, "filterfunc": true,
	"event": true, "playerevent": true, "playerunitevent": true, "unitevent": true,
	"trackable": true, "framehandle": true, "originframetype": true,
	"sound": true, "soundtype": true, "musictheme": true, "ability": true, "buff": true,
	"agent": true,
}

// Tokenize lexes the whole JASS source into a flat []Token. EOF is always
// the last token. Newlines emit TokEOL — the parser relies on EOL for
// statement framing (JASS is line-oriented, unlike Lua).
func Tokenize(src string) ([]Token, error) {
	l := &lexer{src: src, line: 1, col: 1}
	var out []Token
	for {
		t, err := l.next()
		if err != nil {
			return nil, err
		}
		out = append(out, t)
		if t.Kind == TokEOF {
			return out, nil
		}
	}
}

type lexer struct {
	src      string
	pos      int
	line     int
	col      int
	atLineSt bool // tracks whether we're at line-start (for skipping `\` line-continuation, which JASS doesn't have but Lua does — guards us if input is weird)
}

func (l *lexer) peek(n int) byte {
	if l.pos+n >= len(l.src) {
		return 0
	}
	return l.src[l.pos+n]
}

func (l *lexer) advance() byte {
	if l.pos >= len(l.src) {
		return 0
	}
	c := l.src[l.pos]
	l.pos++
	if c == '\n' {
		l.line++
		l.col = 1
	} else {
		l.col++
	}
	return c
}

func (l *lexer) next() (Token, error) {
	// Skip horizontal whitespace (spaces, tabs, carriage returns) and NUL
	// bytes. NUL bytes appear in real-world maps' war3map.wct GlobalJASS
	// from file corruption, mid-edit save crashes, or save-game data sneaking
	// into the script. Stripping them is safer than aborting the whole
	// conversion.
	for l.pos < len(l.src) {
		c := l.src[l.pos]
		if c == ' ' || c == '\t' || c == '\r' || c == 0 {
			l.advance()
			continue
		}
		break
	}

	if l.pos >= len(l.src) {
		return Token{Kind: TokEOF, Line: l.line, Col: l.col}, nil
	}

	startLine, startCol := l.line, l.col
	c := l.src[l.pos]

	// Newline → TokEOL.
	if c == '\n' {
		l.advance()
		return Token{Kind: TokEOL, Value: "\n", Line: startLine, Col: startCol}, nil
	}

	// Line comment // ...
	if c == '/' && l.peek(1) == '/' {
		l.advance()
		l.advance()
		var b strings.Builder
		for l.pos < len(l.src) && l.src[l.pos] != '\n' {
			b.WriteByte(l.src[l.pos])
			l.advance()
		}
		return Token{Kind: TokComment, Value: b.String(), Line: startLine, Col: startCol}, nil
	}

	// String literal "..."
	if c == '"' {
		return l.lexString(startLine, startCol)
	}

	// Rawcode literal 'hpea'
	if c == '\'' {
		return l.lexRaw(startLine, startCol)
	}

	// Hex literal $1A2B
	if c == '$' {
		return l.lexHexDollar(startLine, startCol)
	}

	// Numeric literal (integer or real). 0x... is also handled here.
	if isDigit(c) || (c == '.' && isDigit(l.peek(1))) {
		return l.lexNumber(startLine, startCol)
	}

	// Identifier / keyword / type.
	if isIdentStart(c) {
		return l.lexIdent(startLine, startCol)
	}

	// Two-char operators: ==, !=, <=, >=, <<, >>
	if (c == '=' || c == '!' || c == '<' || c == '>') && l.peek(1) == '=' {
		l.advance()
		l.advance()
		return Token{Kind: TokOp, Value: string([]byte{c, '='}), Line: startLine, Col: startCol}, nil
	}
	// Bitwise shift: << / >>. Lexed as a single TokOp so the parser can
	// dispatch on the whole operator string in one shot.
	if (c == '<' && l.peek(1) == '<') || (c == '>' && l.peek(1) == '>') {
		l.advance()
		l.advance()
		return Token{Kind: TokOp, Value: string([]byte{c, c}), Line: startLine, Col: startCol}, nil
	}

	// One-char operator / punctuation.
	if isOpChar(c) {
		l.advance()
		return Token{Kind: TokOp, Value: string(c), Line: startLine, Col: startCol}, nil
	}

	// B2 — Zinc / JASS+ graceful handling. The characters below never
	// appear in pure JASS (nor in the vJASS subset we preprocess). They are the
	// tell-tale openers of syntaxes we deliberately do NOT support:
	//
	//   { }   Zinc (the C-style alternative vJASS dialect; also leaks through
	//         from unhandled `//! novjass` / Lua-table bodies).
	//   !     JASS+ (the `!`-prefixed operator dialect).
	//
	// Rather than the cryptic "unexpected character '{'", surface a clear,
	// section-scoped diagnostic so the convert dialog can tell the user this
	// section uses an unsupported dialect and was skipped — instead of a
	// mystery lexer error. We still fail the whole section (the caller turns a
	// lex error into a single error() placeholder), which is the intended
	// "fail that section gracefully" behavior.
	switch c {
	case '{', '}':
		return Token{}, fmt.Errorf("jass2lua: unsupported syntax (Zinc/JASS+): %q at line %d col %d — this section uses the Zinc dialect, which is not supported; convert it manually", string(c), startLine, startCol)
	case '!':
		return Token{}, fmt.Errorf("jass2lua: unsupported syntax (Zinc/JASS+): %q at line %d col %d — this section uses the JASS+ dialect, which is not supported; convert it manually", string(c), startLine, startCol)
	}
	return Token{}, fmt.Errorf("jass2lua: lex error at line %d col %d: unexpected character %q", startLine, startCol, c)
}

func (l *lexer) lexString(startLine, startCol int) (Token, error) {
	l.advance() // opening "
	var b strings.Builder
	for l.pos < len(l.src) {
		c := l.src[l.pos]
		if c == '"' {
			l.advance()
			return Token{Kind: TokString, Value: b.String(), Line: startLine, Col: startCol}, nil
		}
		if c == '\\' && l.pos+1 < len(l.src) {
			n := l.src[l.pos+1]
			switch n {
			case 'n':
				b.WriteByte('\n')
			case 't':
				b.WriteByte('\t')
			case 'r':
				b.WriteByte('\r')
			case '\\':
				b.WriteByte('\\')
			case '"':
				b.WriteByte('"')
			default:
				// Preserve unknown escape verbatim.
				b.WriteByte('\\')
				b.WriteByte(n)
			}
			l.advance()
			l.advance()
			continue
		}
		// A literal newline inside a string is ALLOWED: the World Editor
		// embeds raw newlines in quest/description/tooltip strings (e.g.
		// `call SetMapDescription("line1<NL>line2")`) and the WC3 engine
		// accepts them, so we must too. The byte is kept verbatim (luaStringLit
		// re-escapes it to \n on emit). Only a string that runs to EOF without
		// a closing quote is genuinely unterminated.
		b.WriteByte(c)
		l.advance()
	}
	return Token{}, fmt.Errorf("jass2lua: lex error at line %d col %d: unterminated string at EOF", startLine, startCol)
}

func (l *lexer) lexRaw(startLine, startCol int) (Token, error) {
	l.advance() // opening '
	var b strings.Builder
	for l.pos < len(l.src) {
		c := l.src[l.pos]
		if c == '\'' {
			l.advance()
			return Token{Kind: TokRaw, Value: b.String(), Line: startLine, Col: startCol}, nil
		}
		if c == '\n' {
			return Token{}, fmt.Errorf("jass2lua: lex error at line %d col %d: unterminated rawcode", startLine, startCol)
		}
		b.WriteByte(c)
		l.advance()
	}
	return Token{}, fmt.Errorf("jass2lua: lex error at line %d col %d: unterminated rawcode at EOF", startLine, startCol)
}

func (l *lexer) lexHexDollar(startLine, startCol int) (Token, error) {
	l.advance() // $
	startPos := l.pos
	for l.pos < len(l.src) && isHexDigit(l.src[l.pos]) {
		l.advance()
	}
	if l.pos == startPos {
		return Token{}, fmt.Errorf("jass2lua: lex error at line %d col %d: $ without hex digits", startLine, startCol)
	}
	// Re-emit as a 0x-prefixed integer for Lua.
	return Token{Kind: TokInt, Value: "0x" + l.src[startPos:l.pos], Line: startLine, Col: startCol}, nil
}

func (l *lexer) lexNumber(startLine, startCol int) (Token, error) {
	startPos := l.pos
	// 0x... hex.
	if l.src[l.pos] == '0' && (l.peek(1) == 'x' || l.peek(1) == 'X') {
		l.advance()
		l.advance()
		for l.pos < len(l.src) && isHexDigit(l.src[l.pos]) {
			l.advance()
		}
		return Token{Kind: TokInt, Value: l.src[startPos:l.pos], Line: startLine, Col: startCol}, nil
	}
	// Decimal integer or real.
	hasDot := false
	for l.pos < len(l.src) {
		c := l.src[l.pos]
		if isDigit(c) {
			l.advance()
			continue
		}
		if c == '.' && !hasDot {
			// Only consume the dot if it's part of a real (next char is digit
			// or we just finished digits and aren't followed by an ident — JASS
			// has no method-call syntax so a bare `1.foo` doesn't happen).
			hasDot = true
			l.advance()
			continue
		}
		break
	}
	val := l.src[startPos:l.pos]
	kind := TokInt
	if hasDot {
		kind = TokReal
	}
	return Token{Kind: kind, Value: val, Line: startLine, Col: startCol}, nil
}

func (l *lexer) lexIdent(startLine, startCol int) (Token, error) {
	startPos := l.pos
	for l.pos < len(l.src) {
		c := l.src[l.pos]
		if isIdentCont(c) {
			l.advance()
			continue
		}
		break
	}
	val := l.src[startPos:l.pos]
	switch {
	case pureKeywords[val]:
		return Token{Kind: TokKeyword, Value: val, Line: startLine, Col: startCol}, nil
	case baseTypes[val] || builtinHandleTypes[val]:
		return Token{Kind: TokType, Value: val, Line: startLine, Col: startCol}, nil
	}
	return Token{Kind: TokIdent, Value: val, Line: startLine, Col: startCol}, nil
}

func isDigit(c byte) bool     { return c >= '0' && c <= '9' }
func isHexDigit(c byte) bool  { return isDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') }
func isIdentStart(c byte) bool {
	return c == '_' || unicode.IsLetter(rune(c))
}
func isIdentCont(c byte) bool {
	return c == '_' || isDigit(c) || unicode.IsLetter(rune(c))
}
func isOpChar(c byte) bool {
	switch c {
	case '+', '-', '*', '/', '%', '=', '<', '>', '(', ')', '[', ']', ',', '.', ':':
		// `:` is NOT a JASS operator, but the vJASS struct preprocessor
		// rewrites instance-method calls to Lua-style colon syntax
		// (`recv:method(args)`) before the transpiler runs. Accepting `:` as
		// an op token lets that pre-rewritten source flow through; the
		// parser handles it identically to `.` and the emitter preserves
		// the colon. `:` is also the ternary RHS separator (Phase 5).
		return true
	case '?', '|', '&', '~', '^':
		// Phase 5: ternary (`?`) and bitwise (`|`, `&`, `~`, `^`) operators.
		// JASS proper doesn't have these but vJASS / BlizzardAPI maps in the
		// wild use them. The parser handles them as low-precedence operators
		// and the emitter maps them to Lua 5.3+ native operators.
		return true
	}
	return false
}
