---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/preprocess_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

func TestPreprocess_NoMacros(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    call BJDebugMsg("hi")
endfunction
`
	res := Preprocess(src)
	if res.Expanded != src {
		t.Errorf("expected passthrough; got:\n%s", res.Expanded)
	}
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.Macros) != 0 {
		t.Errorf("unexpected macros: %+v", res.Macros)
	}
}

func TestPreprocess_NoParamMacro(t *testing.T) {
	src := `//! textmacro Hello
    call BJDebugMsg("hi")
//! endtextmacro

function Init takes nothing returns nothing
//! runtextmacro Hello()
endfunction
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hi")`) {
		t.Errorf("expected expansion body, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "//! textmacro") {
		t.Errorf("definition should be stripped, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "//! runtextmacro") {
		t.Errorf("call site should be expanded, got:\n%s", res.Expanded)
	}
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
}

func TestPreprocess_OneParam(t *testing.T) {
	src := `//! textmacro Greet takes WHO
    call BJDebugMsg("hello $WHO$")
//! endtextmacro

//! runtextmacro Greet("world")
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hello world")`) {
		t.Errorf("expected param substitution, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_MultipleParams(t *testing.T) {
	src := `//! textmacro Assign takes NAME, VAL
    local integer $NAME$ = $VAL$
//! endtextmacro

//! runtextmacro Assign("counter", "42")
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `local integer counter = 42`) {
		t.Errorf("expected `local integer counter = 42`, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_RunOnce_SuppressesDuplicates(t *testing.T) {
	src := `//! textmacro Decl takes X
    local integer $X$ = 0
//! endtextmacro

//! runtextmacroonce Decl("a")
//! runtextmacroonce Decl("a")
`
	res := Preprocess(src)
	// First expands; second is suppressed (becomes a comment).
	count := strings.Count(res.Expanded, "local integer a = 0")
	if count != 1 {
		t.Errorf("expected exactly 1 expansion (got %d):\n%s", count, res.Expanded)
	}
	if !strings.Contains(res.Expanded, "runtextmacroonce suppressed") {
		t.Errorf("expected suppression comment, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_RunOnce_DifferentArgsExpandBoth(t *testing.T) {
	src := `//! textmacro Decl takes X
    local integer $X$ = 0
//! endtextmacro

//! runtextmacroonce Decl("a")
//! runtextmacroonce Decl("b")
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, "local integer a = 0") {
		t.Errorf("expected `a` expansion, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "local integer b = 0") {
		t.Errorf("expected `b` expansion, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "suppressed") {
		t.Errorf("did not expect suppression, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_NestedExpansion(t *testing.T) {
	src := `//! textmacro Inner takes WHO
    call BJDebugMsg("hi $WHO$")
//! endtextmacro

//! textmacro Outer takes NAME
//! runtextmacro Inner($NAME$)
//! endtextmacro

//! runtextmacro Outer("world")
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hi world")`) {
		t.Errorf("expected nested expansion, got:\n%s", res.Expanded)
	}
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
}

func TestPreprocess_InfiniteRecursionDetected(t *testing.T) {
	src := `//! textmacro Loopy
//! runtextmacro Loopy()
//! endtextmacro

//! runtextmacro Loopy()
`
	res := Preprocess(src)
	if len(res.Errors) == 0 {
		t.Fatalf("expected at least one error for infinite recursion")
	}
	foundDepth := false
	for _, e := range res.Errors {
		if strings.Contains(e.Message, "recursion") || strings.Contains(e.Message, "depth") {
			foundDepth = true
			break
		}
	}
	if !foundDepth {
		t.Errorf("expected recursion/depth error, got: %+v", res.Errors)
	}
}

func TestPreprocess_UnknownMacroIsNonFatal(t *testing.T) {
	src := `//! runtextmacro DoesNotExist(x)
function Foo takes nothing returns nothing
endfunction
`
	res := Preprocess(src)
	if len(res.Errors) == 0 {
		t.Fatalf("expected an error for unknown macro")
	}
	if !strings.Contains(res.Expanded, "unknown textmacro") {
		t.Errorf("expected unknown-textmacro placeholder, got:\n%s", res.Expanded)
	}
	// Subsequent code must still be present.
	if !strings.Contains(res.Expanded, "function Foo") {
		t.Errorf("subsequent source missing, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_UnknownDirective_PassThrough(t *testing.T) {
	src := `//! i //some literal injection
function Foo takes nothing returns nothing
endfunction
`
	res := Preprocess(src)
	// `//!` rewritten to plain `//` comment — must not contain bare `//!`.
	if strings.Contains(res.Expanded, "//!") {
		t.Errorf("expected //! to be rewritten, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "function Foo") {
		t.Errorf("subsequent source missing, got:\n%s", res.Expanded)
	}
	// Should record a non-fatal diagnostic for the directive.
	if len(res.Errors) == 0 {
		t.Errorf("expected at least one non-fatal directive error")
	}
}

func TestPreprocess_LiteralDollarPreservedWhenNotParam(t *testing.T) {
	src := `//! textmacro Greet takes WHO
    call BJDebugMsg("hi $WHO$ — call $999 now")
//! endtextmacro

//! runtextmacro Greet("world")
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hi world — call $999 now")`) {
		t.Errorf("expected $999 literal preserved, got:\n%s", res.Expanded)
	}
	// Also: `$undef$` shape (non-param dollar-delimited) — preserved verbatim.
	src2 := `//! textmacro X takes A
local integer $A$ = 0
local string s = "$NOTAPARAM$"
//! endtextmacro

//! runtextmacro X("foo")
`
	res2 := Preprocess(src2)
	if !strings.Contains(res2.Expanded, "$NOTAPARAM$") {
		t.Errorf("expected unmatched $NOTAPARAM$ preserved, got:\n%s", res2.Expanded)
	}
	if !strings.Contains(res2.Expanded, "local integer foo = 0") {
		t.Errorf("expected matched param substituted, got:\n%s", res2.Expanded)
	}
}

func TestPreprocess_LeadingWhitespaceAllowedOnDirective(t *testing.T) {
	src := `    //! textmacro Foo
        call BJDebugMsg("hi")
    //! endtextmacro

        //! runtextmacro Foo()
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hi")`) {
		t.Errorf("expected expansion despite leading whitespace, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "//! textmacro") {
		t.Errorf("definition should be stripped despite leading whitespace, got:\n%s", res.Expanded)
	}
}

func TestPreprocess_ArgCountMismatch_NonFatal(t *testing.T) {
	src := `//! textmacro Decl takes NAME, VALUE
    local integer $NAME$ = $VALUE$
//! endtextmacro

//! runtextmacro Decl("a")
`
	res := Preprocess(src)
	if len(res.Errors) == 0 {
		t.Fatalf("expected arg-count error")
	}
	if !strings.Contains(res.Expanded, "arg count mismatch") {
		t.Errorf("expected arg-mismatch placeholder, got:\n%s", res.Expanded)
	}
}

// Make sure expansion plays nicely with the downstream TranspileScript: a
// textmacro-only file should produce valid Lua, end-to-end.
func TestPreprocess_ThenTranspile_EndToEnd(t *testing.T) {
	src := `//! textmacro DeclInt takes NAME, VAL
    local integer $NAME$ = $VAL$
//! endtextmacro

function Init takes nothing returns nothing
//! runtextmacro DeclInt("a", "1")
//! runtextmacro DeclInt("b", "2")
    call BJDebugMsg("ready")
endfunction
`
	pp := Preprocess(src)
	if len(pp.Errors) != 0 {
		t.Fatalf("unexpected preprocess errors: %+v", pp.Errors)
	}
	lua, err := TranspileScript(pp.Expanded)
	if err != nil {
		t.Fatalf("transpile after preprocess failed: %v\nexpanded:\n%s", err, pp.Expanded)
	}
	if !strings.Contains(lua, "local a = 1") {
		t.Errorf("expected `local a = 1` in lua, got:\n%s", lua)
	}
	if !strings.Contains(lua, "local b = 2") {
		t.Errorf("expected `local b = 2` in lua, got:\n%s", lua)
	}
}

// TestStripBlockComments_SingleLine: `/* … */` on one line gets replaced
// with a space.
func TestStripBlockComments_SingleLine(t *testing.T) {
	src := `integer x = /* inline */ 5
`
	out := StripBlockComments(src)
	if strings.Contains(out, "/*") || strings.Contains(out, "*/") {
		t.Errorf("expected block comment stripped, got: %q", out)
	}
}

// TestStripBlockComments_MultiLine: multi-line block comments are removed,
// newline count preserved.
func TestStripBlockComments_MultiLine(t *testing.T) {
	src := `integer x = 0
/* this is
   multi line */
integer y = 1
`
	out := StripBlockComments(src)
	if strings.Contains(out, "/*") || strings.Contains(out, "multi line") {
		t.Errorf("expected multi-line block comment stripped, got:\n%s", out)
	}
	// Newline count should be preserved.
	if strings.Count(out, "\n") != strings.Count(src, "\n") {
		t.Errorf("expected newline count preserved (%d), got %d:\n%s",
			strings.Count(src, "\n"), strings.Count(out, "\n"), out)
	}
}

// TestStripBlockComments_NoComments_PassThrough: source without block
// comments round-trips unchanged.
func TestStripBlockComments_NoComments_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
endfunction
`
	out := StripBlockComments(src)
	if out != src {
		t.Errorf("expected passthrough, got:\n%s", out)
	}
}

// TestPreprocess_ConditionalIf_TakesFirstBranch: `//! if X / //! else /
// //! endif` keeps the if-body and drops the else-body. The condition is
// ignored.
func TestPreprocess_ConditionalIf_TakesFirstBranch(t *testing.T) {
	src := `//! if SOMETHING
local integer kept = 1
//! else
local integer dropped = 2
//! endif
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, "local integer kept = 1") {
		t.Errorf("expected if-branch kept, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "local integer dropped = 2") {
		t.Errorf("expected else-branch dropped, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "//! if") || strings.Contains(res.Expanded, "//! else") || strings.Contains(res.Expanded, "//! endif") {
		t.Errorf("expected conditional keywords stripped, got:\n%s", res.Expanded)
	}
}

// TestPreprocess_ConditionalIf_NoElse: if-only conditional keeps body.
func TestPreprocess_ConditionalIf_NoElse(t *testing.T) {
	src := `//! if X
integer kept = 1
//! endif
`
	res := Preprocess(src)
	if !strings.Contains(res.Expanded, "integer kept = 1") {
		t.Errorf("expected body kept, got:\n%s", res.Expanded)
	}
}

// TestPreprocess_ConditionalIf_Nested: nested conditionals work correctly.
func TestPreprocess_ConditionalIf_Nested(t *testing.T) {
	src := `//! if A
keep_outer = 1
//! if B
keep_inner = 2
//! endif
//! endif
`
	res := Preprocess(res_or(src))
	if !strings.Contains(res.Expanded, "keep_outer = 1") {
		t.Errorf("expected outer body kept, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "keep_inner = 2") {
		t.Errorf("expected inner body kept, got:\n%s", res.Expanded)
	}
}

// res_or is a tiny helper so the nested-conditional test reads cleanly
// without leaking the src var.
func res_or(s string) string { return s }

// TestPreprocess_ConditionalIf_MissingEndif: missing endif at EOF surfaces
// a diagnostic but partial output is still useful.
func TestPreprocess_ConditionalIf_MissingEndif(t *testing.T) {
	src := `//! if X
integer kept = 1
`
	res := Preprocess(src)
	if len(res.Errors) == 0 {
		t.Errorf("expected missing-endif diagnostic")
	}
}

// FindVJASSKeyword should NOT trip on textmacro-family keywords after Phase 1
// (the preprocessor consumes them). Library/scope/struct no longer block
// after Phases 2/3 either. Module/interface/define still block until Phase 4.
func TestFindVJASSKeyword_TextmacroFamilyIgnored(t *testing.T) {
	cases := []struct {
		name  string
		src   string
		found bool
	}{
		{"runtextmacro_in_source", `function F takes nothing returns nothing
//! runtextmacro Foo()
endfunction`, false},
		{"module_no_longer_blocks", `module X
endmodule`, false},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			_, found := FindVJASSKeyword(c.src)
			if found != c.found {
				t.Errorf("found=%v want=%v", found, c.found)
			}
		})
	}
}
