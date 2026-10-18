---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/structs_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestPreprocessStructs_PassThrough: source with no struct blocks round-trips
// unchanged + empty results.
func TestPreprocessStructs_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    call BJDebugMsg("hi")
endfunction
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.Structs) != 0 {
		t.Errorf("unexpected structs: %+v", res.Structs)
	}
	if res.Expanded != src {
		t.Errorf("expected pass-through, got:\n%s", res.Expanded)
	}
}

// TestPreprocessStructs_MinimalField: one field + one method.
func TestPreprocessStructs_MinimalField(t *testing.T) {
	src := `struct Foo
    integer x = 0
    method greet takes nothing returns nothing
        call BJDebugMsg("hi")
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.Structs) != 1 {
		t.Fatalf("expected 1 struct, got %d", len(res.Structs))
	}
	s, ok := res.Structs["Foo"]
	if !ok {
		t.Fatalf("expected struct Foo, got %v", StructNamesSorted(res))
	}
	if len(s.Fields) != 1 || s.Fields[0].Name != "x" || s.Fields[0].Type != "integer" || s.Fields[0].Default != "0" {
		t.Errorf("unexpected fields: %+v", s.Fields)
	}
	if len(s.Methods) != 1 || s.Methods[0].Name != "greet" || s.Methods[0].Static {
		t.Errorf("unexpected methods: %+v", s.Methods)
	}
	// Marker emitted.
	if !strings.Contains(res.Expanded, "__VJASS_STRUCT__ Foo") {
		t.Errorf("expected marker comment in expanded, got:\n%s", res.Expanded)
	}
	// struct/endstruct keywords stripped.
	for _, line := range strings.Split(res.Expanded, "\n") {
		trim := strings.TrimSpace(line)
		if trim == "endstruct" || strings.HasPrefix(trim, "struct ") {
			t.Errorf("expected struct keywords stripped, found: %q", line)
		}
	}
}

// TestPreprocessStructs_Inheritance: `extends Bar` → emitted Lua uses
// `setmetatable({}, {__index = Bar})`.
func TestPreprocessStructs_Inheritance(t *testing.T) {
	src := `struct Foo extends Bar
    integer y = 1
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if s.Extends != "Bar" {
		t.Errorf("expected Extends=Bar, got %q", s.Extends)
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "setmetatable({}, {__index = Bar})") {
		t.Errorf("expected metatable inheritance, got:\n%s", lua)
	}
}

// TestPreprocessStructs_StaticFields: static field becomes class-level Lua.
func TestPreprocessStructs_StaticFields(t *testing.T) {
	src := `struct Foo
    static integer count = 0
    integer instance_x = 0
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	if len(s.Statics) != 1 || s.Statics[0].Name != "count" {
		t.Errorf("expected one static `count`, got: %+v", s.Statics)
	}
	if len(s.Fields) != 1 || s.Fields[0].Name != "instance_x" {
		t.Errorf("expected one instance `instance_x`, got: %+v", s.Fields)
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "Foo.count = 0") {
		t.Errorf("expected static field assignment, got:\n%s", lua)
	}
	// Instance field appears in allocate() table.
	if !strings.Contains(lua, "instance_x = 0") {
		t.Errorf("expected instance field default in allocate(), got:\n%s", lua)
	}
}

// TestPreprocessStructs_StaticMethod: dot-call syntax for static methods.
func TestPreprocessStructs_StaticMethod(t *testing.T) {
	src := `struct Foo
    static method staticOp takes integer x returns integer
        return x + 1
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	if len(s.Methods) != 1 || !s.Methods[0].Static {
		t.Fatalf("expected one static method, got: %+v", s.Methods)
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "function Foo.staticOp(x)") {
		t.Errorf("expected dot-call static method signature, got:\n%s", lua)
	}
}

// TestPreprocessStructs_UserCreate: user-defined `create` is preserved; no
// auto-synthesis.
func TestPreprocessStructs_UserCreate(t *testing.T) {
	src := `struct Foo
    integer x = 0
    static method create takes integer initial returns Foo
        local Foo self = Foo.allocate()
        set self.x = initial
        return self
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	if !s.HasCreate {
		t.Errorf("expected HasCreate=true")
	}
	lua := EmitStructLua(s)
	// User's create body should appear; the auto-synthesized one-liner
	// should NOT.
	if !strings.Contains(lua, "function Foo.create(initial)") {
		t.Errorf("expected user create signature, got:\n%s", lua)
	}
	// Count occurrences of `function Foo.create` — should be exactly 1.
	if c := strings.Count(lua, "function Foo.create"); c != 1 {
		t.Errorf("expected exactly 1 create definition, got %d:\n%s", c, lua)
	}
}

// TestPreprocessStructs_AutoCreate: without a user `create`, one is
// synthesized.
func TestPreprocessStructs_AutoCreate(t *testing.T) {
	src := `struct Foo
    integer x = 0
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	if s.HasCreate {
		t.Errorf("expected HasCreate=false for no-user-create struct")
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "function Foo.create()") {
		t.Errorf("expected auto-synthesized Foo.create(), got:\n%s", lua)
	}
}

// TestPreprocessStructs_OnInit: `static method onInit` populates Inits.
func TestPreprocessStructs_OnInit(t *testing.T) {
	src := `struct Foo
    static method onInit takes nothing returns nothing
        call BJDebugMsg("Foo loaded")
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Inits) != 1 {
		t.Fatalf("expected 1 onInit, got %d: %+v", len(res.Inits), res.Inits)
	}
	if res.Inits[0].StructName != "Foo" || res.Inits[0].InitMethod != "onInit" {
		t.Errorf("expected (Foo, onInit), got %+v", res.Inits[0])
	}
}

// TestPreprocessStructs_ThistypeSubstitution: `thistype` inside a method body
// becomes the struct name.
func TestPreprocessStructs_ThistypeSubstitution(t *testing.T) {
	src := `struct Foo
    static method make takes nothing returns Foo
        return thistype.allocate()
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	lua := EmitStructLua(s)
	if strings.Contains(lua, "thistype") {
		t.Errorf("expected thistype substituted, got:\n%s", lua)
	}
	if !strings.Contains(lua, "Foo.allocate()") {
		t.Errorf("expected Foo.allocate() in emitted method, got:\n%s", lua)
	}
}

// TestPreprocessStructs_ThisToSelf: `this.x` inside a method body becomes
// `self.x`.
func TestPreprocessStructs_ThisToSelf(t *testing.T) {
	src := `struct Foo
    integer x = 0
    method bump takes nothing returns nothing
        set this.x = this.x + 1
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	lua := EmitStructLua(s)
	if strings.Contains(lua, "this.x") {
		t.Errorf("expected `this.x` rewritten to `self.x`, got:\n%s", lua)
	}
	if !strings.Contains(lua, "self.x") {
		t.Errorf("expected `self.x` in emitted method, got:\n%s", lua)
	}
}

// TestPreprocessStructs_MultipleMethods: instance + static methods coexist.
func TestPreprocessStructs_MultipleMethods(t *testing.T) {
	src := `struct Foo
    method instanceM takes nothing returns nothing
    endmethod
    static method staticM takes nothing returns nothing
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	s := res.Structs["Foo"]
	if len(s.Methods) != 2 {
		t.Fatalf("expected 2 methods, got %d", len(s.Methods))
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "function Foo:instanceM(") {
		t.Errorf("expected instance method colon syntax, got:\n%s", lua)
	}
	if !strings.Contains(lua, "function Foo.staticM(") {
		t.Errorf("expected static method dot syntax, got:\n%s", lua)
	}
}

// TestPreprocessStructs_EmptyStruct: struct with no fields/methods still
// produces a valid Lua snippet (with synthesized create + destroy).
func TestPreprocessStructs_EmptyStruct(t *testing.T) {
	src := `struct Foo
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors for empty struct: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "function Foo.allocate()") {
		t.Errorf("expected synthesized allocate, got:\n%s", lua)
	}
	if !strings.Contains(lua, "function Foo.create()") {
		t.Errorf("expected synthesized create, got:\n%s", lua)
	}
	if !strings.Contains(lua, "function Foo:destroy()") {
		t.Errorf("expected synthesized destroy, got:\n%s", lua)
	}
}

// TestPreprocessStructs_ExtendsArrayWarning: `extends array` produces a
// warning + treated as plain struct.
func TestPreprocessStructs_ExtendsArrayWarning(t *testing.T) {
	src := `struct Foo extends array
    integer x
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) == 0 {
		t.Errorf("expected a warning for extends array")
	}
	gotWarn := false
	for _, e := range res.Errors {
		if strings.Contains(e.Message, "extends array") {
			gotWarn = true
			break
		}
	}
	if !gotWarn {
		t.Errorf("expected an `extends array` warning in: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if s.Extends != "" {
		t.Errorf("expected Extends cleared for `extends array`, got %q", s.Extends)
	}
}

// TestPreprocessStructs_DelegateField: `delegate` member is parsed without
// warning (Phase 4 added metamethod fallthrough; no longer noisy) and the
// field is tagged for the emit phase.
func TestPreprocessStructs_DelegateField(t *testing.T) {
	src := `struct Foo
    delegate Bar parent
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("expected no delegate warning in Phase 4, got: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 1 || !s.Fields[0].Delegate {
		t.Errorf("expected one delegate field, got: %+v", s.Fields)
	}
	if len(s.Delegates) != 1 || s.Delegates[0] != "parent" {
		t.Errorf("expected Delegates=[\"parent\"], got: %+v", s.Delegates)
	}
	// Emitted Lua should use the metamethod __index = function() pattern.
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "Foo.__index = function") {
		t.Errorf("expected delegate __index metamethod, got:\n%s", lua)
	}
	if !strings.Contains(lua, `rawget(t, "parent")`) {
		t.Errorf("expected rawget for delegate field, got:\n%s", lua)
	}
}

// TestPreprocessStructs_ReadonlyAllowed: `readonly` member is parsed
// without warning and behaves like a normal field.
func TestPreprocessStructs_ReadonlyAllowed(t *testing.T) {
	src := `struct Foo
    readonly integer id
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("expected no errors for readonly, got: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 1 || !s.Fields[0].Readonly {
		t.Errorf("expected one readonly field, got: %+v", s.Fields)
	}
}

// TestPreprocessStructs_DotVsColonDisambiguation: outside-struct method-call
// rewriting. `Foo.method()` stays dot; `instance.method()` becomes colon.
func TestPreprocessStructs_DotVsColonDisambiguation(t *testing.T) {
	src := `struct Foo
    method greet takes nothing returns nothing
    endmethod
endstruct

function caller takes nothing returns nothing
    local Foo inst = Foo.create()
    call Foo.create()
    call inst.greet()
endfunction
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	// Foo.create stays dot (Foo is a known struct).
	if !strings.Contains(res.Expanded, "Foo.create()") {
		t.Errorf("expected `Foo.create()` dot preserved, got:\n%s", res.Expanded)
	}
	// inst.greet becomes inst:greet (instance receiver).
	if !strings.Contains(res.Expanded, "inst:greet()") {
		t.Errorf("expected `inst.greet()` rewritten to `inst:greet()`, got:\n%s", res.Expanded)
	}
}

// TestSpliceStructLua: marker comments in the transpiled Lua get replaced
// with the actual struct Lua snippet.
func TestSpliceStructLua(t *testing.T) {
	src := `struct Foo
    integer x = 0
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	// The marker would have been transpiled to `-- __VJASS_STRUCT__ Foo`.
	lua := "before\n" + StructLuaMarker + "Foo\nafter\n"
	out := SpliceStructLua(lua, res)
	if strings.Contains(out, StructLuaMarker+"Foo") {
		t.Errorf("expected marker replaced, still present:\n%s", out)
	}
	if !strings.Contains(out, "function Foo.allocate()") {
		t.Errorf("expected splice to include allocate(), got:\n%s", out)
	}
}

// TestSpliceStructLuaWithErrors_SurfacesMethodBodyErrors is the P0#1
// completion regression: a struct whose method body contains an unparseable
// statement must yield a NON-ZERO surfaced error count (these errors were
// previously swallowed because SpliceStructLua returned only a string, so the
// inline error() markers inside method bodies never reached section.Errors).
func TestSpliceStructLuaWithErrors_SurfacesMethodBodyErrors(t *testing.T) {
	// `garbage tokens here` is not a valid JASS statement; the body-fragment
	// parser recovers it into a RawStmt → inline error() marker.
	src := `struct Foo
    method bork takes nothing returns nothing
        garbage tokens here
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := StructLuaMarker + "Foo\n"
	out, errs := SpliceStructLuaWithErrors(lua, res)
	inline := strings.Count(out, "error(")
	if inline == 0 {
		t.Fatalf("expected inline error() marker in spliced struct method body, got:\n%s", out)
	}
	if len(errs) == 0 {
		t.Fatalf("P0#1 gap: struct method body had %d inline error() markers but SpliceStructLuaWithErrors surfaced 0", inline)
	}
	// The surfaced error must identify the struct + method so the UI's
	// diagnostics list points at the right place.
	if !strings.Contains(errs[0], "struct Foo method bork") {
		t.Errorf("expected surfaced error to name `struct Foo method bork`, got: %q", errs[0])
	}
	// Plain SpliceStructLua must still produce identical Lua (emitted output
	// is unchanged — we only added the error channel).
	if plain := SpliceStructLua(lua, res); plain != out {
		t.Errorf("SpliceStructLua output drifted from SpliceStructLuaWithErrors:\nplain:\n%s\nwith-errors:\n%s", plain, out)
	}
}

// TestSpliceStructLuaWithErrors_CleanMethodZeroErrors confirms a struct with a
// well-formed method body surfaces ZERO errors (no false positives).
func TestSpliceStructLuaWithErrors_CleanMethodZeroErrors(t *testing.T) {
	src := `struct Foo
    method greet takes nothing returns nothing
        call BJDebugMsg("hi")
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := StructLuaMarker + "Foo\n"
	out, errs := SpliceStructLuaWithErrors(lua, res)
	if strings.Contains(out, "error(") {
		t.Errorf("clean struct method should not emit error() markers, got:\n%s", out)
	}
	if len(errs) != 0 {
		t.Errorf("clean struct method should surface 0 errors, got %d: %v", len(errs), errs)
	}
}

// TestPreprocessStructs_TwoStructsOrder: source order is preserved in
// StructOrder.
func TestPreprocessStructs_TwoStructsOrder(t *testing.T) {
	src := `struct A
endstruct

struct B
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.StructOrder) != 2 {
		t.Fatalf("expected 2 structs, got %d", len(res.StructOrder))
	}
	if res.StructOrder[0] != "A" || res.StructOrder[1] != "B" {
		t.Errorf("expected source order [A, B], got %v", res.StructOrder)
	}
}

// TestPreprocessStructs_MissingEndstruct: warning emitted, body still
// captured for what we got.
func TestPreprocessStructs_MissingEndstruct(t *testing.T) {
	src := `struct Foo
    integer x = 0
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) == 0 {
		t.Errorf("expected missing-endstruct warning")
	}
	if _, ok := res.Structs["Foo"]; !ok {
		t.Errorf("expected partial struct still registered")
	}
}

// TestPreprocessStructs_NoFalseTripOnStringStruct: a function body that has
// the word "struct" inside a string literal must NOT be parsed as a struct
// block.
func TestPreprocessStructs_NoFalseTripOnStringStruct(t *testing.T) {
	src := `function F takes nothing returns nothing
    call BJDebugMsg("struct Foo")
endfunction
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Structs) != 0 {
		t.Errorf("expected no structs parsed (string literal), got: %+v", res.Structs)
	}
	if res.Expanded != src {
		t.Errorf("expected pass-through, got:\n%s", res.Expanded)
	}
}

// TestPreprocessStructs_StaticIfBodyKept: `static if SOMETHING / endif`
// inside a struct body strips the keyword pair and keeps the body.
func TestPreprocessStructs_StaticIfBodyKept(t *testing.T) {
	src := `struct Foo
    static if DEBUG_MODE
    integer debugCounter = 0
    endif
    integer x = 0
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	// Both fields should be present.
	names := map[string]bool{}
	for _, f := range s.Fields {
		names[f.Name] = true
	}
	if !names["debugCounter"] {
		t.Errorf("expected debugCounter field kept (static-if always-take-first); fields=%+v", s.Fields)
	}
	if !names["x"] {
		t.Errorf("expected x field; fields=%+v", s.Fields)
	}
}

// TestPreprocessStructs_DebugMethodPrefix: `debug method foo` strips the
// `debug` prefix and treats as a regular method.
func TestPreprocessStructs_DebugMethodPrefix(t *testing.T) {
	src := `struct Foo
    debug method dump takes nothing returns nothing
        call BJDebugMsg("dump")
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Methods) != 1 || s.Methods[0].Name != "dump" {
		t.Errorf("expected one `dump` method (debug prefix stripped), got: %+v", s.Methods)
	}
}

// TestPreprocessStructs_BlockCommentsStripped: `/* … */` block comments
// inside a struct are stripped by the global StripBlockComments pass before
// PreprocessStructs sees the source.
func TestPreprocessStructs_BlockCommentsStripped(t *testing.T) {
	src := `struct Foo
    /* this is
       a multi-line
       comment */
    integer x = 0
endstruct
`
	cleaned := StripBlockComments(src)
	res := PreprocessStructs(cleaned, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors after StripBlockComments: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 1 || s.Fields[0].Name != "x" {
		t.Errorf("expected single field x, got: %+v", s.Fields)
	}
}

// TestRewriteStructRefs_KnownNamesForceDot: a method-call receiver matching
// a known top-level name (library public, top-level function, global) stays
// dot — not rewritten to colon. Regression test for the Phase 3 issue where
// enum-like globals got colon'd.
func TestRewriteStructRefs_KnownNamesForceDot(t *testing.T) {
	src := `struct Foo
    method greet takes nothing returns nothing
    endmethod
endstruct

function caller takes nothing returns nothing
    call Foo_doThing.run()
    call inst.greet()
endfunction
`
	known := map[string]bool{
		"Foo_doThing": true,
	}
	res := PreprocessStructs(src, nil, known)
	if !strings.Contains(res.Expanded, "Foo_doThing.run()") {
		t.Errorf("expected known name to keep dot, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "inst:greet()") {
		t.Errorf("expected unknown receiver to swap to colon, got:\n%s", res.Expanded)
	}
}

// TestCollectTopLevelNames covers the parsing logic for top-level functions,
// globals, and library publics — the inputs to the dot-forcing set.
func TestCollectTopLevelNames(t *testing.T) {
	src := `globals
    integer myGlobal = 0
endglobals

function MyFunc takes nothing returns nothing
endfunction

native MyNative takes nothing returns nothing
`
	out := CollectTopLevelNames(src, []string{"Lib_pub"}, map[string]StructDef{"MyStruct": {}})
	for _, want := range []string{"myGlobal", "MyFunc", "MyNative", "Lib_pub", "MyStruct"} {
		if !out[want] {
			t.Errorf("expected %q in known top-level names, got: %+v", want, out)
		}
	}
}

// ---------------------------------------------------------------------------
// Phase 5 — real-world cleanup tests.
// ---------------------------------------------------------------------------

// TestPreprocessStructs_TrailingFieldComment verifies a field decl with a
// trailing inline `// comment` is parsed, not flagged as unrecognized.
func TestPreprocessStructs_TrailingFieldComment(t *testing.T) {
	src := `struct Foo
    real av        // alpha value
    integer life  // remaining life
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 2 {
		t.Fatalf("expected 2 fields, got %d: %+v", len(s.Fields), s.Fields)
	}
	if s.Fields[0].Name != "av" || s.Fields[0].Type != "real" {
		t.Errorf("bad field 0: %+v", s.Fields[0])
	}
	if s.Fields[1].Name != "life" || s.Fields[1].Type != "integer" {
		t.Errorf("bad field 1: %+v", s.Fields[1])
	}
}

// TestPreprocessStructs_BareFieldDecl verifies a field decl without `= default`
// parses correctly; the type's zero is used at emit time.
func TestPreprocessStructs_BareFieldDecl(t *testing.T) {
	src := `struct Foo
    unit s
    integer n
    real r
    string label
    boolean flag
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 5 {
		t.Fatalf("expected 5 fields, got %d", len(s.Fields))
	}
	lua := EmitStructLua(s)
	// Each type's default initializer should appear in the allocator.
	for _, want := range []string{"s = nil", "n = 0", "r = 0.0", `label = ""`, "flag = false"} {
		if !strings.Contains(lua, want) {
			t.Errorf("expected %q in emit, got:\n%s", want, lua)
		}
	}
}

// TestPreprocessStructs_StaticConstant — `static constant <type> NAME = EXPR`
// parses as a static field with the constant nature documented (no Lua
// enforcement). Both `static constant` and bare `constant` forms are accepted.
func TestPreprocessStructs_StaticConstant(t *testing.T) {
	src := `struct Foo
    static constant real INTERVAL = 0.03125
    constant integer COUNT = 5
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Statics) != 1 {
		t.Errorf("expected 1 static, got %d: %+v", len(s.Statics), s.Statics)
	}
	// Bare constant becomes an instance field with the default — JassHelper's
	// semantics here are arguable; we don't promote it to a static because
	// without `static`, the constant lives in instance scope. Either way, no
	// "unrecognized body line" error should fire.
	if s.Statics[0].Name != "INTERVAL" || s.Statics[0].Default != "0.03125" {
		t.Errorf("bad static: %+v", s.Statics[0])
	}
}

// TestPreprocessStructs_StaticIfElseEndif — full `static if … else … endif`
// blocks inside a struct body. Always-take-first means the if-body parses;
// the else-body is dropped; no stray `else` / `endif` lines escape.
func TestPreprocessStructs_StaticIfElseEndif(t *testing.T) {
	src := `struct Foo
    static if DEBUG_MODE then
        integer x = 1
    else
        integer x = 2
    endif
    integer y = 10
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Fields) != 2 {
		t.Fatalf("expected 2 fields (x=1 + y=10), got %d: %+v", len(s.Fields), s.Fields)
	}
	if s.Fields[0].Default != "1" {
		t.Errorf("expected first branch (x=1), got %+v", s.Fields[0])
	}
}

// TestPreprocessStructs_StaticIfNested — nested static-if blocks. Inner ones
// behave correctly within outer ones; depth tracking via stack.
func TestPreprocessStructs_StaticIfNested(t *testing.T) {
	src := `struct Foo
    static if A
        static if B
            integer x = 1
        else
            integer x = 2
        endif
    else
        integer x = 3
    endif
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	// Always-take-first → outer if true → inner if true → x = 1.
	if len(s.Fields) != 1 || s.Fields[0].Default != "1" {
		t.Errorf("expected x=1 from nested first branches, got: %+v", s.Fields)
	}
}

// TestPreprocessStructs_OperatorIndex — `static method operator [] takes …`
// parses as a method with Operator="[]" and emits a __index metamethod.
func TestPreprocessStructs_OperatorIndex(t *testing.T) {
	src := `struct Foo
    static method operator [] takes player p returns thistype
        return 0
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Methods) != 1 {
		t.Fatalf("expected 1 method, got %d", len(s.Methods))
	}
	if s.Methods[0].Operator != "[]" {
		t.Errorf("expected Operator=`[]`, got %q", s.Methods[0].Operator)
	}
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "Foo._op_index") {
		t.Errorf("expected fallback Foo._op_index method, got:\n%s", lua)
	}
	if !strings.Contains(lua, "Foo.__index = function") {
		t.Errorf("expected __index metamethod reassignment, got:\n%s", lua)
	}
}

// TestPreprocessStructs_OperatorComparison — `<` / `==` etc. parse and emit
// as __lt / __eq metamethods.
func TestPreprocessStructs_OperatorComparison(t *testing.T) {
	src := `struct Foo
    static method operator < takes thistype a, thistype b returns boolean
        return true
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "Foo.__lt = Foo._op_lt") {
		t.Errorf("expected __lt metamethod wiring, got:\n%s", lua)
	}
}

// TestPreprocessStructs_OperatorPlus — arithmetic operator overload maps to
// __add.
func TestPreprocessStructs_OperatorPlus(t *testing.T) {
	src := `struct Vec
    static method operator + takes thistype a, thistype b returns thistype
        return 0
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Vec"]
	lua := EmitStructLua(s)
	if !strings.Contains(lua, "Vec.__add = Vec._op_add") {
		t.Errorf("expected __add metamethod wiring, got:\n%s", lua)
	}
}

// TestPreprocessStructs_InlineCommentOnStaticField — a static field with a
// trailing comment parses + retains its value.
func TestPreprocessStructs_InlineCommentOnStaticField(t *testing.T) {
	src := `struct Foo
    static integer counter = 0   // global counter
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	if len(res.Errors) != 0 {
		t.Fatalf("unexpected errors: %+v", res.Errors)
	}
	s := res.Structs["Foo"]
	if len(s.Statics) != 1 || s.Statics[0].Name != "counter" || s.Statics[0].Default != "0" {
		t.Errorf("expected counter=0 static, got %+v", s.Statics)
	}
}

// --- Implicit-this member access (receiver-omitted `.field`) -----------------
//
// vJASS struct methods allow `.field` as shorthand for `this.field`. The
// rewriteMethodBody pass expands a receiver-omitted `.` to `self.` so the
// downstream parser's member-access grammar handles it. These tests pin the
// expansion in every syntactic position the real Enfo structs use (the bug
// that produced 91/72 inline error() stubs on Dialog System / NaturesShadow).

// TestImplicitThis_SetAssign: `set .x = .y` (LHS + RHS implicit-this) becomes
// `self.x = self.y` and emits ZERO error() stubs.
func TestImplicitThis_SetAssign(t *testing.T) {
	src := `struct Foo
    real x = 0.
    real y = 0.
    method move takes nothing returns nothing
        set .x = .y
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := EmitStructLua(res.Structs["Foo"])
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("expected no error() stub, got:\n%s", lua)
	}
	if !strings.Contains(lua, "self.x = self.y") {
		t.Errorf("expected `self.x = self.y`, got:\n%s", lua)
	}
}

// TestImplicitThis_InCondition: `.field` inside an `if` condition expands.
func TestImplicitThis_InCondition(t *testing.T) {
	src := `struct Foo
    integer n = 0
    method check takes nothing returns nothing
        if .n == 0 then
            set .n = 1
        endif
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := EmitStructLua(res.Structs["Foo"])
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("expected no error() stub, got:\n%s", lua)
	}
	if !strings.Contains(lua, "if self.n == 0 then") {
		t.Errorf("expected `if self.n == 0 then`, got:\n%s", lua)
	}
}

// TestImplicitThis_Chained: `.i.rs` (implicit-this head, then a normal nested
// member) expands ONLY the leading dot: `self.i.rs`. The second dot is
// preceded by ident `i` (a receiver tail) and is left alone.
func TestImplicitThis_Chained(t *testing.T) {
	src := `struct Foo
    real x = 0.
    method step takes nothing returns nothing
        set .x = .x + .i.rs
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := EmitStructLua(res.Structs["Foo"])
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("expected no error() stub, got:\n%s", lua)
	}
	if !strings.Contains(lua, "self.x = self.x + self.i.rs") {
		t.Errorf("expected `self.x = self.x + self.i.rs`, got:\n%s", lua)
	}
}

// TestImplicitThis_CallArg: `.field` as a function-call argument expands.
func TestImplicitThis_CallArg(t *testing.T) {
	src := `struct Foo
    unit s = null
    method pos takes nothing returns nothing
        set .x = GetUnitX(.s)
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := EmitStructLua(res.Structs["Foo"])
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("expected no error() stub, got:\n%s", lua)
	}
	if !strings.Contains(lua, "self.x = GetUnitX(self.s)") {
		t.Errorf("expected `self.x = GetUnitX(self.s)`, got:\n%s", lua)
	}
}

// TestImplicitThis_ExplicitReceiverUntouched: a normal `dat.next` (explicit
// receiver) must NOT have `self` injected — only receiver-omitted dots do.
func TestImplicitThis_ExplicitReceiverUntouched(t *testing.T) {
	src := `struct Foo
    method link takes Foo dat, Foo dat2 returns nothing
        set dat.next = dat2.next
    endmethod
endstruct
`
	res := PreprocessStructs(src, nil, nil)
	lua := EmitStructLua(res.Structs["Foo"])
	if strings.Contains(lua, "jass2lua failed") {
		t.Fatalf("expected no error() stub, got:\n%s", lua)
	}
	if !strings.Contains(lua, "dat.next = dat2.next") {
		t.Errorf("expected explicit-receiver `dat.next = dat2.next`, got:\n%s", lua)
	}
	if strings.Contains(lua, "self.dat") || strings.Contains(lua, "dat = self") {
		t.Errorf("explicit receiver wrongly got `self` injected:\n%s", lua)
	}
}

// TestSanitizeModuleBodyForPaste covers the cross-trigger inliner's body
// sanitizer: a module body pasted into a struct must shed constructs that are
// invalid as struct members (top-level globals/function/interface/nested-struct
// blocks), interface-style bodyless method signatures (no endmethod before the
// next opener), and valueless constant declarations — while keeping real
// methods (opener..endmethod) and real field declarations. Without this, a
// documentation/interface module (e.g. Enfo's 1080-line MisssileStruct) makes
// the struct parser over-collect and cascade errors.
func TestSanitizeModuleBodyForPaste(t *testing.T) {
	raw := "static method onCollide takes integer m, integer u returns boolean\n" + // bodyless interface sig -> dropped
		"static method launch takes integer m returns nothing\n" + // real method ...
		"call doThing()\n" +
		"endmethod\n" + // ... opener..endmethod kept
		"constant real FOO\n" + // valueless constant decl -> dropped
		"integer count = 0\n" + // real field -> kept
		"globals\n" + // top-level block -> dropped whole
		"integer g = 1\n" +
		"endglobals\n" +
		"function helper takes nothing returns nothing\n" + // free function -> dropped whole
		"call x()\n" +
		"endfunction\n"

	out := strings.Join(sanitizeModuleBodyForPaste(raw), "")

	// Dropped:
	if strings.Contains(out, "onCollide") {
		t.Errorf("bodyless interface method signature should be dropped:\n%s", out)
	}
	if strings.Contains(out, "constant real FOO") {
		t.Errorf("valueless constant decl should be dropped:\n%s", out)
	}
	if strings.Contains(out, "endglobals") || strings.Contains(out, "integer g = 1") {
		t.Errorf("top-level globals block should be dropped:\n%s", out)
	}
	if strings.Contains(out, "function helper") || strings.Contains(out, "call x()") {
		t.Errorf("module free function should be dropped:\n%s", out)
	}

	// Kept:
	if !strings.Contains(out, "method launch") || !strings.Contains(out, "call doThing()") {
		t.Errorf("real method (opener..endmethod) should be kept:\n%s", out)
	}
	if !strings.Contains(out, "integer count = 0") {
		t.Errorf("real field declaration should be kept:\n%s", out)
	}
}

// TestPreprocessStructs_StripsKeywordForwardDecl covers vJASS `keyword`
// forward-declarations: JassHelper emits nothing for them, but left in the
// source they reach the statement parser as `unexpected token "keyword"`.
// PreprocessStructs replaces them with a marker comment while leaving real
// struct declarations intact.
func TestPreprocessStructs_StripsKeywordForwardDecl(t *testing.T) {
	src := "private keyword roflcopter\n" +
		"struct Foo\n" +
		"    integer x = 0\n" +
		"endstruct\n"
	res := PreprocessStructs(src, nil, nil)
	if strings.Contains(res.Expanded, "keyword roflcopter") {
		t.Errorf("keyword forward-decl should be stripped:\n%s", res.Expanded)
	}
	if _, ok := res.Structs["Foo"]; !ok {
		t.Errorf("real struct Foo should still be parsed; got structs: %v", res.Structs)
	}
}
