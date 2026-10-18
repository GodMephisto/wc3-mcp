---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/p2_parser_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestB1_StrayBlockRecovery_OneDiagnostic exercises the B1 block-aware recovery
// for the JassHelper `//! inject` pattern. After the preprocessor rewrites the
// `//!` to a plain `//`, the injected statements sit at file scope. The parser
// must NOT emit one "unexpected token" per statement (the 2,239-error cascade);
// it must fold the whole run into ONE StrayBlock with ONE diagnostic while the
// statements still transpile, and the well-formed function before it stays
// clean.
func TestB1_StrayBlockRecovery_OneDiagnostic(t *testing.T) {
	jass := strings.Join([]string{
		"function Setup takes nothing returns nothing",
		"    call DoNothing()",
		"endfunction",
		"// inject main",
		"local integer i = 0",
		"set i = i + 1",
		"call BJDebugMsg(\"hi\")",
		"if i > 0 then",
		"    call DoNothing()",
		"endif",
	}, "\n") + "\n"

	lua, parseErrs, lexErr := TranspileScriptWithErrors(jass)
	if lexErr != nil {
		t.Fatalf("unexpected lex error: %v", lexErr)
	}
	// Exactly ONE diagnostic for the whole injected run, not one per statement.
	if len(parseErrs) != 1 {
		t.Fatalf("expected exactly 1 block-level diagnostic, got %d: %v\nlua:\n%s", len(parseErrs), parseErrs, lua)
	}
	if !strings.Contains(parseErrs[0], "stray top-level statements") {
		t.Errorf("expected stray-block diagnostic, got: %v", parseErrs)
	}
	// The well-formed function before the stray run must still transpile cleanly.
	if !strings.Contains(lua, "function Setup()") {
		t.Errorf("preceding well-formed function should transpile, got:\n%s", lua)
	}
	// The injected statements transpile (no error() placeholder for readable code).
	for _, want := range []string{"BJDebugMsg(\"hi\")", "i = i + 1", "if i > 0 then"} {
		if !strings.Contains(lua, want) {
			t.Errorf("expected recovered statement %q in output, got:\n%s", want, lua)
		}
	}
	// The recovered run is wrapped in a do/end block so the Lua stays valid.
	if !strings.Contains(lua, "do") || !strings.Contains(lua, "end") {
		t.Errorf("expected do ... end wrapper for the stray block, got:\n%s", lua)
	}
	// No per-statement error() markers for statements the parser could read.
	if n := strings.Count(lua, `error("jass2lua failed`); n != 0 {
		t.Errorf("expected 0 inline error() markers for readable statements, got %d:\n%s", n, lua)
	}
}

// TestB1_BadOpenerInsideFunction_RestStillEmits is the assignment's B1 case:
// one bad opener inside a function must NOT torch the rest of the function. The
// surrounding well-formed statements still emit, and the failure is reported.
func TestB1_BadOpenerInsideFunction_RestStillEmits(t *testing.T) {
	// `set` with no lhs is a malformed statement; the statements around it are
	// well-formed and must survive.
	jass := strings.Join([]string{
		"function F takes nothing returns nothing",
		"    call BeforeOK()",
		"    set = 5", // bad: missing lhs
		"    call AfterOK()",
		"endfunction",
	}, "\n") + "\n"

	lua, parseErrs, lexErr := TranspileScriptWithErrors(jass)
	if lexErr != nil {
		t.Fatalf("unexpected lex error: %v", lexErr)
	}
	if len(parseErrs) == 0 {
		t.Fatalf("expected the bad statement to be reported, got 0 parseErrs")
	}
	if !strings.Contains(lua, "BeforeOK()") {
		t.Errorf("statement before the bad opener should still emit, got:\n%s", lua)
	}
	if !strings.Contains(lua, "AfterOK()") {
		t.Errorf("statement after the bad opener should still emit, got:\n%s", lua)
	}
	// The enclosing function frame is preserved.
	if !strings.Contains(lua, "function F()") {
		t.Errorf("function header should still emit, got:\n%s", lua)
	}
}

// TestB3_FragmentLeadingLineComment_NoSlashMisparse is the assignment's B3 case:
// a body fragment that begins with a `//` line comment (e.g. the editor's
// `//TESH.scrollpos=...` marker, or an asterisk banner) must transpile cleanly
// with NO `/`-operator misparse and NO error() markers.
func TestB3_FragmentLeadingLineComment_NoSlashMisparse(t *testing.T) {
	frag := "//TESH.scrollpos=10\ncall DoNothing()\n"
	lua, parseErrs, lexErr := TranspileFunctionWithErrors("Trig", frag)
	if lexErr != nil {
		t.Fatalf("unexpected lex error: %v", lexErr)
	}
	if len(parseErrs) != 0 {
		t.Fatalf("expected clean fragment, got %d parseErrs: %v\nlua:\n%s", len(parseErrs), parseErrs, lua)
	}
	if strings.Contains(lua, `error("jass2lua failed`) {
		t.Errorf("fragment should transpile clean, got error() marker:\n%s", lua)
	}
	if !strings.Contains(lua, "DoNothing()") {
		t.Errorf("the real statement after the comment should transpile, got:\n%s", lua)
	}
}

// TestB3_AsteriskBannerNotBlockComment guards the StripBlockComments fix: a `//`
// banner comment full of asterisks (which contains the substring `/*`) must NOT
// be treated as the start of a block comment. Previously the naive regex
// swallowed everything to the next `*/`, stranding a `/` that lexed as an
// operator and cascaded into "unexpected token /" errors.
func TestB3_AsteriskBannerNotBlockComment(t *testing.T) {
	src := strings.Join([]string{
		"//***************************************",
		"//   B A N N E R",
		"//***************************************",
		"function F takes nothing returns nothing",
		"    call DoNothing()",
		"endfunction",
	}, "\n") + "\n"

	stripped := StripBlockComments(src)
	// The banner lines must be preserved verbatim (not collapsed across lines).
	if !strings.Contains(stripped, "B A N N E R") {
		t.Fatalf("banner content was swallowed by StripBlockComments:\n%s", stripped)
	}
	if !strings.Contains(stripped, "function F takes nothing returns nothing") {
		t.Fatalf("function decl was swallowed by StripBlockComments:\n%s", stripped)
	}

	lua, parseErrs, lexErr := TranspileScriptWithErrors(stripped)
	if lexErr != nil {
		t.Fatalf("unexpected lex error (banner misparsed as block comment?): %v", lexErr)
	}
	if len(parseErrs) != 0 {
		t.Fatalf("expected clean transpile of banner+function, got %d errs: %v\nlua:\n%s", len(parseErrs), parseErrs, lua)
	}
	if !strings.Contains(lua, "function F()") || !strings.Contains(lua, "DoNothing()") {
		t.Errorf("function did not transpile after banner, got:\n%s", lua)
	}
}

// TestB3_StripBlockComments_RealBlockStillStripped confirms the scanner still
// strips genuine `/* ... */` block comments (single- and multi-line) and
// preserves newline counts for line-number alignment.
func TestB3_StripBlockComments_RealBlockStillStripped(t *testing.T) {
	// Single-line block comment becomes a space.
	if got := StripBlockComments("set x = /*c*/ 5"); strings.Contains(got, "/*") || strings.Contains(got, "c") {
		t.Errorf("single-line block comment not stripped: %q", got)
	}
	// Multi-line block comment is replaced with newlines preserving line count.
	src := "a\n/* line1\nline2 */\nb\n"
	got := StripBlockComments(src)
	if strings.Contains(got, "line1") || strings.Contains(got, "line2") {
		t.Errorf("multi-line block comment not stripped: %q", got)
	}
	if strings.Count(got, "\n") != strings.Count(src, "\n") {
		t.Errorf("newline count not preserved: src=%d got=%d (%q)", strings.Count(src, "\n"), strings.Count(got, "\n"), got)
	}
}

// TestB3_BlockCommentInsideStringPreserved confirms the scanner does not treat a
// `/*` inside a string literal as a block-comment opener.
func TestB3_BlockCommentInsideStringPreserved(t *testing.T) {
	src := "call F(\"a /* not a comment */ b\")\n"
	got := StripBlockComments(src)
	if !strings.Contains(got, "/* not a comment */") {
		t.Errorf("block-comment-looking text inside a string was stripped: %q", got)
	}
}

// TestB2_ZincGracefulDiagnostic confirms a Zinc `{` produces a clear,
// section-scoped "unsupported syntax (Zinc/JASS+)" diagnostic rather than a
// cryptic "unexpected character" lexer error, and fails the section gracefully.
func TestB2_ZincGracefulDiagnostic(t *testing.T) {
	_, err := Tokenize("LineAimer = {\n    Red = 1.0\n}\n")
	if err == nil {
		t.Fatalf("expected a lex error for Zinc `{`, got nil")
	}
	if !strings.Contains(err.Error(), "unsupported syntax (Zinc/JASS+)") {
		t.Errorf("expected clear Zinc/JASS+ diagnostic, got: %v", err)
	}
	if !strings.Contains(err.Error(), "Zinc") {
		t.Errorf("expected the diagnostic to name the Zinc dialect, got: %v", err)
	}
}

// TestB2_JassPlusGracefulDiagnostic confirms a JASS+ `!` produces the clear
// section-scoped diagnostic.
func TestB2_JassPlusGracefulDiagnostic(t *testing.T) {
	_, err := Tokenize("set x = a ! b\n")
	if err == nil {
		t.Fatalf("expected a lex error for JASS+ `!`, got nil")
	}
	if !strings.Contains(err.Error(), "unsupported syntax (Zinc/JASS+)") {
		t.Errorf("expected clear Zinc/JASS+ diagnostic, got: %v", err)
	}
	if !strings.Contains(err.Error(), "JASS+") {
		t.Errorf("expected the diagnostic to name the JASS+ dialect, got: %v", err)
	}
}

// TestB2_NotEqualStillLexes guards against the B2 diagnostic swallowing the
// legitimate `!=` operator (a `!` followed by `=`). `!=` must still lex as a
// single operator token, untouched by the Zinc/JASS+ bail-out.
func TestB2_NotEqualStillLexes(t *testing.T) {
	toks, err := Tokenize("if a != b then\n")
	if err != nil {
		t.Fatalf("`!=` should lex cleanly, got: %v", err)
	}
	found := false
	for _, tk := range toks {
		if tk.Kind == TokOp && tk.Value == "!=" {
			found = true
		}
	}
	if !found {
		t.Errorf("expected a `!=` operator token, got: %v", toks)
	}
}
