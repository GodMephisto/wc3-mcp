---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/modules_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestPreprocessModules_PassThrough: source with no modules round-trips
// unchanged + empty results.
func TestPreprocessModules_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    call BJDebugMsg("hi")
endfunction
`
	res := PreprocessModules(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.Modules) != 0 {
		t.Errorf("unexpected modules: %+v", res.Modules)
	}
	if res.Expanded != src {
		t.Errorf("expected passthrough, got:\n%s", res.Expanded)
	}
}

// TestPreprocessModules_MinimalModule: one module with one method + one
// field is parsed correctly.
func TestPreprocessModules_MinimalModule(t *testing.T) {
	src := `module HasLog
    method log takes string s returns nothing
        call BJDebugMsg(s)
    endmethod
    integer logCount = 0
endmodule
`
	res := PreprocessModules(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.Modules) != 1 {
		t.Fatalf("expected 1 module, got %d", len(res.Modules))
	}
	m, ok := res.Modules["HasLog"]
	if !ok {
		t.Fatalf("expected module HasLog, got %+v", res.Modules)
	}
	if len(m.Methods) != 1 || m.Methods[0].Name != "log" {
		t.Errorf("unexpected methods: %+v", m.Methods)
	}
	if len(m.Fields) != 1 || m.Fields[0].Name != "logCount" {
		t.Errorf("unexpected fields: %+v", m.Fields)
	}
	// Marker comment present + module/endmodule stripped.
	for _, line := range strings.Split(res.Expanded, "\n") {
		trim := strings.TrimSpace(line)
		if strings.HasPrefix(trim, "module ") || trim == "endmodule" {
			t.Errorf("expected module keywords stripped, found: %q", line)
		}
	}
}

// TestPreprocessStructs_ImplementModule: a struct that `implement`s a known
// module sees the module's methods + fields pasted into its body.
func TestPreprocessStructs_ImplementModule(t *testing.T) {
	src := `module HasGreet
    method greet takes nothing returns nothing
        call BJDebugMsg("hi")
    endmethod
    integer counter = 0
endmodule

struct Greeter
    string name = "Hi"
    implement HasGreet
endstruct
`
	mods := PreprocessModules(src)
	if len(mods.Errors) != 0 {
		t.Fatalf("module pass errors: %+v", mods.Errors)
	}
	st := PreprocessStructs(mods.Expanded, mods.Modules, nil)
	if len(st.Errors) != 0 {
		t.Errorf("struct pass errors: %+v", st.Errors)
	}
	s, ok := st.Structs["Greeter"]
	if !ok {
		t.Fatalf("expected struct Greeter, got %+v", st.Structs)
	}
	// Greeter should have greet method + counter field pasted in.
	gotGreet := false
	for _, m := range s.Methods {
		if m.Name == "greet" {
			gotGreet = true
			break
		}
	}
	if !gotGreet {
		t.Errorf("expected `greet` method pasted from module, got methods: %+v", s.Methods)
	}
	gotCounter := false
	for _, f := range s.Fields {
		if f.Name == "counter" {
			gotCounter = true
			break
		}
	}
	if !gotCounter {
		t.Errorf("expected `counter` field pasted from module, got fields: %+v", s.Fields)
	}
}

// TestPreprocessStructs_ImplementOptionalMissing: implement optional Foo
// where Foo is not defined → silent, no error.
func TestPreprocessStructs_ImplementOptionalMissing(t *testing.T) {
	src := `struct Bar
    integer y = 0
    implement optional MissingModule
endstruct
`
	st := PreprocessStructs(src, nil, nil)
	for _, e := range st.Errors {
		if strings.Contains(e.Message, "implement") || strings.Contains(e.Message, "MissingModule") {
			t.Errorf("expected silent skip for optional, got error: %+v", e)
		}
	}
	s, ok := st.Structs["Bar"]
	if !ok {
		t.Fatalf("expected struct Bar")
	}
	// y field still present, no module methods.
	if len(s.Fields) != 1 || s.Fields[0].Name != "y" {
		t.Errorf("expected only the y field, got: %+v", s.Fields)
	}
}

// TestPreprocessStructs_ImplementMissingRequired: implement (without
// optional) of an unknown module produces a diagnostic.
func TestPreprocessStructs_ImplementMissingRequired(t *testing.T) {
	src := `struct Bar
    implement DoesNotExist
endstruct
`
	st := PreprocessStructs(src, map[string]ModuleDef{}, nil)
	gotErr := false
	for _, e := range st.Errors {
		if strings.Contains(e.Message, "DoesNotExist") || strings.Contains(e.Message, "implement") {
			gotErr = true
			break
		}
	}
	if !gotErr {
		t.Errorf("expected an error for missing required module, got: %+v", st.Errors)
	}
}

// TestPreprocessStructs_MultipleStructsImplementSameModule: each struct
// gets its own copy of the module's methods.
func TestPreprocessStructs_MultipleStructsImplementSameModule(t *testing.T) {
	src := `module Shared
    method ping takes nothing returns nothing
        call BJDebugMsg("ping")
    endmethod
endmodule

struct A
    implement Shared
endstruct

struct B
    implement Shared
endstruct
`
	mods := PreprocessModules(src)
	st := PreprocessStructs(mods.Expanded, mods.Modules, nil)
	for _, name := range []string{"A", "B"} {
		s, ok := st.Structs[name]
		if !ok {
			t.Fatalf("expected struct %s", name)
		}
		got := false
		for _, m := range s.Methods {
			if m.Name == "ping" {
				got = true
				break
			}
		}
		if !got {
			t.Errorf("struct %s missing `ping` method from module", name)
		}
	}
}

// TestPreprocessStructs_ImplementMethodReferencesThis: a module method that
// uses `this.x` becomes `self.x` in the implementing struct's emitted Lua.
func TestPreprocessStructs_ImplementMethodReferencesThis(t *testing.T) {
	src := `module HasLog
    method log takes nothing returns nothing
        call BJDebugMsg(this.name)
    endmethod
endmodule

struct Greeter
    string name = "Hi"
    implement HasLog
endstruct
`
	mods := PreprocessModules(src)
	st := PreprocessStructs(mods.Expanded, mods.Modules, nil)
	s := st.Structs["Greeter"]
	lua := EmitStructLua(s)
	if strings.Contains(lua, "this.name") {
		t.Errorf("expected `this.name` rewritten to `self.name`, got:\n%s", lua)
	}
	if !strings.Contains(lua, "self.name") {
		t.Errorf("expected `self.name` in emitted method, got:\n%s", lua)
	}
}
