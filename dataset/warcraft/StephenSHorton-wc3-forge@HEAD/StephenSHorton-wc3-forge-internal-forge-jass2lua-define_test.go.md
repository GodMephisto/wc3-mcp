---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/define_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestPreprocessDefines_PassThrough: source without `define` round-trips.
func TestPreprocessDefines_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
endfunction
`
	out := PreprocessDefines(src)
	if out != src {
		t.Errorf("expected passthrough, got:\n%s", out)
	}
}

// TestPreprocessDefines_TopLevel: top-level `define X = 5` becomes a
// `globals` block with the assignment.
func TestPreprocessDefines_TopLevel(t *testing.T) {
	src := `define MAX_ITEMS = 5

function Foo takes nothing returns nothing
endfunction
`
	out := PreprocessDefines(src)
	if strings.Contains(out, "define MAX_ITEMS") {
		t.Errorf("expected `define` rewritten, got:\n%s", out)
	}
	// Should now be inside a globals block.
	if !strings.Contains(out, "globals") || !strings.Contains(out, "integer MAX_ITEMS = 5") {
		t.Errorf("expected top-level define wrapped in globals, got:\n%s", out)
	}
}

// TestPreprocessDefines_InsideFunction: a define inside a function becomes
// `local integer NAME = EXPR`.
func TestPreprocessDefines_InsideFunction(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    define COUNT = 3
    call BJDebugMsg("ok")
endfunction
`
	out := PreprocessDefines(src)
	if strings.Contains(out, "define COUNT") {
		t.Errorf("expected `define` rewritten inside function, got:\n%s", out)
	}
	if !strings.Contains(out, "local integer COUNT = 3") {
		t.Errorf("expected `local integer COUNT = 3`, got:\n%s", out)
	}
}

// TestPreprocessDefines_ComplexExpr: define with a complex RHS expression
// still gets rewritten without mangling the expression.
func TestPreprocessDefines_ComplexExpr(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    define MASK = 7 * 11 + 1
endfunction
`
	out := PreprocessDefines(src)
	if !strings.Contains(out, "local integer MASK = 7 * 11 + 1") {
		t.Errorf("expected RHS preserved, got:\n%s", out)
	}
}

// TestPreprocessDefines_PostTranspile: a define-only source transpiles to
// valid Lua (smoke test).
func TestPreprocessDefines_PostTranspile(t *testing.T) {
	src := `define MAX = 10

function Use takes nothing returns nothing
    call BJDebugMsg("max")
endfunction
`
	out := PreprocessDefines(src)
	lua, err := TranspileScript(out)
	if err != nil {
		t.Fatalf("transpile failed: %v\nsrc:\n%s", err, out)
	}
	if !strings.Contains(lua, "MAX = 10") {
		t.Errorf("expected Lua global MAX = 10, got:\n%s", lua)
	}
}
