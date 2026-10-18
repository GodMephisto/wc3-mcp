---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/tokens_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// tokensValues extracts the .Value slice of a token stream (minus the
// trailing EOF) for quick string-based diff assertions.
func tokenValues(toks []Token) []string {
	var out []string
	for _, t := range toks {
		if t.Kind == TokEOF {
			break
		}
		out = append(out, t.Value)
	}
	return out
}

// TestTokenize_NULByteStripped — Phase 5: real-world war3map.wct GlobalJASS
// blocks can contain stray NUL bytes (mid-save crashes, save-game data
// bleeding into the script). The lexer must silently strip them rather than
// abort the whole conversion.
func TestTokenize_NULByteStripped(t *testing.T) {
	withNul := "function Foo \x00 takes nothing returns nothing"
	clean := "function Foo   takes nothing returns nothing"
	a, err := Tokenize(withNul)
	if err != nil {
		t.Fatalf("NUL-bearing source must lex without error, got %v", err)
	}
	b, err := Tokenize(clean)
	if err != nil {
		t.Fatalf("clean source lex err: %v", err)
	}
	av := tokenValues(a)
	bv := tokenValues(b)
	if strings.Join(av, "|") != strings.Join(bv, "|") {
		t.Errorf("NUL strip should produce identical token values\n  withNul=%v\n  clean=%v", av, bv)
	}
}

// TestTokenize_MultipleNULs ensures repeated NUL bytes are all stripped.
func TestTokenize_MultipleNULs(t *testing.T) {
	src := "\x00\x00function\x00 Foo\x00\x00 takes\x00 nothing"
	toks, err := Tokenize(src)
	if err != nil {
		t.Fatalf("multi-NUL lex err: %v", err)
	}
	want := []string{"function", "Foo", "takes", "nothing"}
	got := tokenValues(toks)
	if strings.Join(got, "|") != strings.Join(want, "|") {
		t.Errorf("multi-NUL strip: got=%v want=%v", got, want)
	}
}

// TestTokenize_TernaryOps — `?` and `:` lex as TokOp.
func TestTokenize_TernaryOps(t *testing.T) {
	toks, err := Tokenize("a ? b : c")
	if err != nil {
		t.Fatalf("ternary lex err: %v", err)
	}
	want := []string{"a", "?", "b", ":", "c"}
	got := tokenValues(toks)
	if strings.Join(got, "|") != strings.Join(want, "|") {
		t.Errorf("ternary tokens: got=%v want=%v", got, want)
	}
}

// TestTokenize_BitwiseOps — `|`, `&`, `^`, `~`, `<<`, `>>` lex as ops.
func TestTokenize_BitwiseOps(t *testing.T) {
	cases := []struct {
		src  string
		want []string
	}{
		{"a | b", []string{"a", "|", "b"}},
		{"a & b", []string{"a", "&", "b"}},
		{"a ^ b", []string{"a", "^", "b"}},
		{"~a", []string{"~", "a"}},
		{"a << 2", []string{"a", "<<", "2"}},
		{"a >> 2", []string{"a", ">>", "2"}},
	}
	for _, c := range cases {
		toks, err := Tokenize(c.src)
		if err != nil {
			t.Errorf("%q lex err: %v", c.src, err)
			continue
		}
		got := tokenValues(toks)
		if strings.Join(got, "|") != strings.Join(c.want, "|") {
			t.Errorf("%q tokens: got=%v want=%v", c.src, got, c.want)
		}
	}
}

// TestTokenize_ShiftDistinctFromCompare ensures `<<` lexes as a single TokOp
// rather than two `<` tokens (which would clash with the comparison parser).
func TestTokenize_ShiftDistinctFromCompare(t *testing.T) {
	toks, err := Tokenize("a < b")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	// Expect 3 tokens (+ EOF): a, <, b
	if len(toks) != 4 {
		t.Errorf("expected 3 tokens + EOF for `a < b`, got %d: %+v", len(toks), tokenValues(toks))
	}
	toks2, err := Tokenize("a << b")
	if err != nil {
		t.Fatalf("lex err: %v", err)
	}
	// Expect 3 tokens (+ EOF): a, <<, b
	if len(toks2) != 4 {
		t.Errorf("expected 3 tokens + EOF for `a << b`, got %d: %+v", len(toks2), tokenValues(toks2))
	}
	if toks2[1].Value != "<<" {
		t.Errorf("expected `<<` op token, got %q", toks2[1].Value)
	}
}
