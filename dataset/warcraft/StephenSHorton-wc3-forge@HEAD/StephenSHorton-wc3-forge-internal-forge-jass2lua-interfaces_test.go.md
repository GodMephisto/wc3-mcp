---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/interfaces_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestPreprocessInterfaces_StripsBlock: an `interface … endinterface` block
// is removed from the source and replaced by a marker comment.
func TestPreprocessInterfaces_StripsBlock(t *testing.T) {
	src := `interface Iterable
    method next takes nothing returns integer
    method hasNext takes nothing returns boolean
endinterface

function Use takes nothing returns nothing
    call BJDebugMsg("ok")
endfunction
`
	out := PreprocessInterfaces(src)
	for _, line := range strings.Split(out, "\n") {
		trim := strings.TrimSpace(line)
		if strings.HasPrefix(trim, "interface ") || trim == "endinterface" {
			t.Errorf("expected interface keywords stripped, found: %q", line)
		}
	}
	// The marker should mention the interface name.
	if !strings.Contains(out, "interface Iterable stripped") {
		t.Errorf("expected marker comment for interface Iterable, got:\n%s", out)
	}
	// The function after the interface still survives.
	if !strings.Contains(out, "function Use takes nothing") {
		t.Errorf("expected subsequent source preserved, got:\n%s", out)
	}
}

// TestPreprocessInterfaces_NoInterface_PassThrough: source without an
// interface keyword passes through unchanged.
func TestPreprocessInterfaces_NoInterface_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
endfunction
`
	out := PreprocessInterfaces(src)
	if out != src {
		t.Errorf("expected passthrough, got:\n%s", out)
	}
}

// TestPreprocessInterfaces_InterfaceTypeUsageIsPreserved: an interface used
// as a type elsewhere is preserved (we strip only the declaration, not
// references). The downstream parser tolerates unknown types as TokIdent.
func TestPreprocessInterfaces_InterfaceTypeUsageIsPreserved(t *testing.T) {
	src := `interface Foo
endinterface

function bar takes Foo x returns nothing
    call DoNothing()
endfunction
`
	out := PreprocessInterfaces(src)
	if !strings.Contains(out, "function bar takes Foo x") {
		t.Errorf("expected `Foo` reference preserved as a type, got:\n%s", out)
	}
}
