---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/parse_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestParseTernary_Basic — `a ? b : c` parses as TernaryExpr.
func TestParseTernary_Basic(t *testing.T) {
	toks, err := Tokenize("function F takes nothing returns integer\nreturn x ? 1 : 2\nendfunction\n")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	f := Parse(toks)
	if len(f.Items) != 1 {
		t.Fatalf("expected 1 top-level item, got %d (errs=%v)", len(f.Items), f.Errors)
	}
	fn, ok := f.Items[0].(*FuncDecl)
	if !ok {
		t.Fatalf("expected FuncDecl, got %T", f.Items[0])
	}
	if len(fn.Body) != 1 {
		t.Fatalf("expected 1 stmt, got %d", len(fn.Body))
	}
	ret, ok := fn.Body[0].(*ReturnStmt)
	if !ok {
		t.Fatalf("expected ReturnStmt, got %T", fn.Body[0])
	}
	if _, ok := ret.Value.(*TernaryExpr); !ok {
		t.Fatalf("expected return value to be TernaryExpr, got %T", ret.Value)
	}
}

// TestParseTernary_RightAssociative — `a ? b : c ? d : e` should bind as
// `a ? b : (c ? d : e)`.
func TestParseTernary_RightAssociative(t *testing.T) {
	toks, err := Tokenize("function F takes nothing returns integer\nreturn a ? b : c ? d : e\nendfunction\n")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	f := Parse(toks)
	if len(f.Errors) != 0 {
		t.Fatalf("unexpected parse errors: %v", f.Errors)
	}
	fn := f.Items[0].(*FuncDecl)
	ret := fn.Body[0].(*ReturnStmt)
	outer, ok := ret.Value.(*TernaryExpr)
	if !ok {
		t.Fatalf("expected outer TernaryExpr, got %T", ret.Value)
	}
	if _, ok := outer.Else.(*TernaryExpr); !ok {
		t.Errorf("expected right-associative: outer.Else should be TernaryExpr, got %T", outer.Else)
	}
}

// TestParseBitwise_Precedence — `a | b & c` should parse as `a | (b & c)`
// (& binds tighter than |, matching C-style).
func TestParseBitwise_Precedence(t *testing.T) {
	toks, err := Tokenize("function F takes nothing returns integer\nreturn a | b & c\nendfunction\n")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	f := Parse(toks)
	if len(f.Errors) != 0 {
		t.Fatalf("unexpected parse errors: %v", f.Errors)
	}
	fn := f.Items[0].(*FuncDecl)
	ret := fn.Body[0].(*ReturnStmt)
	or, ok := ret.Value.(*BinaryExpr)
	if !ok || or.Op != "|" {
		t.Fatalf("expected outer | BinaryExpr, got %T (%v)", ret.Value, ret.Value)
	}
	and, ok := or.R.(*BinaryExpr)
	if !ok || and.Op != "&" {
		t.Errorf("expected inner & BinaryExpr on RHS, got %T", or.R)
	}
}

// TestParseShift_Operators ensures `<<` / `>>` parse cleanly as BinaryExpr.
func TestParseShift_Operators(t *testing.T) {
	toks, err := Tokenize("function F takes nothing returns integer\nreturn x << 2\nendfunction\n")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	f := Parse(toks)
	if len(f.Errors) != 0 {
		t.Fatalf("unexpected parse errors: %v", f.Errors)
	}
	fn := f.Items[0].(*FuncDecl)
	ret := fn.Body[0].(*ReturnStmt)
	be, ok := ret.Value.(*BinaryExpr)
	if !ok || be.Op != "<<" {
		t.Errorf("expected << BinaryExpr, got %T", ret.Value)
	}
}

// TestParseUnaryTilde — `~x` parses as UnaryExpr(~).
func TestParseUnaryTilde(t *testing.T) {
	toks, err := Tokenize("function F takes nothing returns integer\nreturn ~x\nendfunction\n")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	f := Parse(toks)
	if len(f.Errors) != 0 {
		t.Fatalf("unexpected parse errors: %v", f.Errors)
	}
	fn := f.Items[0].(*FuncDecl)
	ret := fn.Body[0].(*ReturnStmt)
	un, ok := ret.Value.(*UnaryExpr)
	if !ok || un.Op != "~" {
		t.Errorf("expected unary ~ UnaryExpr, got %T (%v)", ret.Value, ret.Value)
	}
}

// --- vJASS data member access (`recv.field`) ---------------------------------
//
// JASS proper has no member-access syntax; the struct preprocessor leaves
// `recv.field` data access (NOT method calls — those go through the dot->colon
// rewrite) in the source. The parser must accept `recv.field` both as an
// assignable LVALUE and in expression position; the emitter writes it as Lua
// `recv.field`. These pin the documented design intent end-to-end.

// wrapBody transpiles a single statement inside a trivial function wrapper and
// returns the emitted Lua body line (trimmed). Fails on any parse/lex error.
func wrapBody(t *testing.T, stmt string) string {
	t.Helper()
	src := "function F takes nothing returns nothing\n" + stmt + "\nendfunction\n"
	lua, parseErrs, lexErr := TranspileScriptWithErrors(src)
	if lexErr != nil {
		t.Fatalf("lex error on %q: %v", stmt, lexErr)
	}
	if len(parseErrs) != 0 {
		t.Fatalf("parse errors on %q: %v\n%s", stmt, parseErrs, lua)
	}
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("unexpected error() stub on %q:\n%s", stmt, lua)
	}
	for _, ln := range strings.Split(lua, "\n") {
		tl := strings.TrimSpace(ln)
		if tl == "" || strings.HasPrefix(tl, "function F") || tl == "end" {
			continue
		}
		return tl
	}
	t.Fatalf("no body line emitted for %q:\n%s", stmt, lua)
	return ""
}

// TestMemberAccess_SetAssign — `set a.b = c.d` → `a.b = c.d` (LVALUE + RHS).
func TestMemberAccess_SetAssign(t *testing.T) {
	if got := wrapBody(t, "set a.b = c.d"); got != "a.b = c.d" {
		t.Errorf("got %q, want %q", got, "a.b = c.d")
	}
	// AST shape: the SetStmt target is a MemberExpr.
	toks, _ := Tokenize("function F takes nothing returns nothing\nset a.b = c.d\nendfunction\n")
	f := Parse(toks)
	fn := f.Items[0].(*FuncDecl)
	set, ok := fn.Body[0].(*SetStmt)
	if !ok {
		t.Fatalf("expected SetStmt, got %T", fn.Body[0])
	}
	if _, ok := set.Target.(*MemberExpr); !ok {
		t.Errorf("expected SetStmt.Target to be *MemberExpr, got %T", set.Target)
	}
}

// TestMemberAccess_InCondition — member access in an `if` condition.
func TestMemberAccess_InCondition(t *testing.T) {
	src := "function F takes nothing returns nothing\nif dat.s == null then\nset x = 1\nendif\nendfunction\n"
	lua, parseErrs, lexErr := TranspileScriptWithErrors(src)
	if lexErr != nil || len(parseErrs) != 0 {
		t.Fatalf("errors: lex=%v parse=%v\n%s", lexErr, parseErrs, lua)
	}
	if !strings.Contains(lua, "if dat.s == nil then") {
		t.Errorf("expected `if dat.s == nil then`, got:\n%s", lua)
	}
}

// TestMemberAccess_Chained — `a.b.c` chains into nested MemberExpr.
func TestMemberAccess_Chained(t *testing.T) {
	if got := wrapBody(t, "set q = a.b.c"); got != "q = a.b.c" {
		t.Errorf("got %q, want %q", got, "q = a.b.c")
	}
}

// TestMemberAccess_CallArg — member access as a call argument.
func TestMemberAccess_CallArg(t *testing.T) {
	if got := wrapBody(t, "set dat.x1 = GetUnitX(dat.s)"); got != "dat.x1 = GetUnitX(dat.s)" {
		t.Errorf("got %q, want %q", got, "dat.x1 = GetUnitX(dat.s)")
	}
}
