---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/libscope_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestPreprocessLibScope_PassThrough covers a source with no library/scope
// blocks: should round-trip unchanged with no diagnostics + empty InitOrder.
func TestPreprocessLibScope_PassThrough(t *testing.T) {
	src := `function Foo takes nothing returns nothing
    call BJDebugMsg("hi")
endfunction
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.InitOrder) != 0 {
		t.Errorf("unexpected InitOrder for no-library source: %+v", res.InitOrder)
	}
	if !strings.Contains(res.Expanded, `call BJDebugMsg("hi")`) {
		t.Errorf("expected pure-JASS to pass through, got:\n%s", res.Expanded)
	}
}

// TestPreprocessLibScope_SingleLibraryPublicPrivate exercises the v1 mangling
// scheme. A library with one public + one private function:
//   - public:  `public function Bar` → `function Foo_Bar`
//   - private: `private function Baz` → `function Foo___Baz`
//   - references inside the body resolve to the mangled names.
func TestPreprocessLibScope_SingleLibraryPublicPrivate(t *testing.T) {
	src := `library Foo
    public function Bar takes nothing returns nothing
        call Baz()
    endfunction
    private function Baz takes nothing returns nothing
        call BJDebugMsg("baz")
    endfunction
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	// Decl renames.
	if !strings.Contains(res.Expanded, "function Foo_Bar takes") {
		t.Errorf("expected public decl renamed to Foo_Bar, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "function Foo___Baz takes") {
		t.Errorf("expected private decl renamed to Foo___Baz, got:\n%s", res.Expanded)
	}
	// `public` / `private` keywords stripped.
	if strings.Contains(res.Expanded, "public function") {
		t.Errorf("expected `public` keyword stripped, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "private function") {
		t.Errorf("expected `private` keyword stripped, got:\n%s", res.Expanded)
	}
	// Reference inside Bar's body to Baz must be rewritten to Foo___Baz.
	if !strings.Contains(res.Expanded, "call Foo___Baz()") {
		t.Errorf("expected internal reference renamed to Foo___Baz(), got:\n%s", res.Expanded)
	}
	// library/endlibrary keywords stripped (excluding the marker comments
	// we emit for grep-ability; those start with `// jass2lua:`).
	for _, line := range strings.Split(res.Expanded, "\n") {
		trim := strings.TrimSpace(line)
		if strings.HasPrefix(trim, "//") {
			continue
		}
		if strings.HasPrefix(trim, "library ") {
			t.Errorf("expected non-comment library opener stripped, found line: %q\nfull:\n%s", line, res.Expanded)
		}
		if trim == "endlibrary" {
			t.Errorf("expected non-comment endlibrary stripped, found line: %q\nfull:\n%s", line, res.Expanded)
		}
	}
	// InitOrder: one library, no initializer.
	if len(res.InitOrder) != 1 {
		t.Fatalf("expected 1 library in InitOrder, got %d: %+v", len(res.InitOrder), res.InitOrder)
	}
	if res.InitOrder[0].Name != "Foo" {
		t.Errorf("expected library name Foo, got %q", res.InitOrder[0].Name)
	}
	if res.InitOrder[0].InitFunc != "" {
		t.Errorf("expected empty InitFunc, got %q", res.InitOrder[0].InitFunc)
	}
}

// TestPreprocessLibScope_Initializer asserts the `initializer Foo` clause is
// captured into LibraryInit.InitFunc.
func TestPreprocessLibScope_Initializer(t *testing.T) {
	src := `library Foo initializer InitFoo
    private function InitFoo takes nothing returns nothing
        call BJDebugMsg("init")
    endfunction
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.InitOrder) != 1 {
		t.Fatalf("expected 1 library, got %d", len(res.InitOrder))
	}
	if res.InitOrder[0].InitFunc != "InitFoo" {
		t.Errorf("expected InitFunc=InitFoo, got %q", res.InitOrder[0].InitFunc)
	}
}

// TestPreprocessLibScope_DependencyOrder asserts B-requires-A means A comes
// first in InitOrder.
func TestPreprocessLibScope_DependencyOrder(t *testing.T) {
	src := `library B requires A
endlibrary

library A
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.InitOrder) != 2 {
		t.Fatalf("expected 2 libraries, got %d", len(res.InitOrder))
	}
	if res.InitOrder[0].Name != "A" || res.InitOrder[1].Name != "B" {
		t.Errorf("expected order [A, B], got [%s, %s]", res.InitOrder[0].Name, res.InitOrder[1].Name)
	}
}

// TestPreprocessLibScope_Cycle asserts a cyclic dependency emits all involved
// libraries + a cycle diagnostic.
func TestPreprocessLibScope_Cycle(t *testing.T) {
	src := `library A requires B
endlibrary

library B requires A
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) == 0 {
		t.Fatalf("expected at least one error for the cycle")
	}
	foundCycle := false
	for _, e := range res.Errors {
		if strings.Contains(e.Message, "cycle") {
			foundCycle = true
			break
		}
	}
	if !foundCycle {
		t.Errorf("expected a cycle diagnostic, got: %+v", res.Errors)
	}
	if len(res.InitOrder) != 2 {
		t.Errorf("expected both libraries still in InitOrder (degraded), got %d: %+v", len(res.InitOrder), res.InitOrder)
	}
}

// TestPreprocessLibScope_OptionalRequiresMissingIsNonError asserts
// `requires optional Foo` where Foo doesn't exist produces no error.
func TestPreprocessLibScope_OptionalRequiresMissingIsNonError(t *testing.T) {
	src := `library Foo requires optional NonExistent
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("expected no errors for missing optional requires, got: %+v", res.Errors)
	}
	if len(res.InitOrder) != 1 {
		t.Fatalf("expected 1 library, got %d", len(res.InitOrder))
	}
}

// TestPreprocessLibScope_RequiredMissingIsError asserts a non-optional
// `requires NonExistent` emits a diagnostic.
func TestPreprocessLibScope_RequiredMissingIsError(t *testing.T) {
	src := `library Foo requires NonExistent
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) == 0 {
		t.Errorf("expected an error for missing required library")
	}
	// Library still emitted (degraded behavior).
	if len(res.InitOrder) != 1 {
		t.Errorf("expected 1 library still in InitOrder, got %d", len(res.InitOrder))
	}
}

// TestPreprocessLibScope_Scope asserts a scope block mangles every decl to
// `<Scope>___<name>` (no public exposure).
func TestPreprocessLibScope_Scope(t *testing.T) {
	src := `scope MyScope
    function Helper takes nothing returns nothing
        call BJDebugMsg("helper")
    endfunction
endscope
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if !strings.Contains(res.Expanded, "function MyScope___Helper takes") {
		t.Errorf("expected scope decl renamed to MyScope___Helper, got:\n%s", res.Expanded)
	}
	for _, line := range strings.Split(res.Expanded, "\n") {
		trim := strings.TrimSpace(line)
		if strings.HasPrefix(trim, "//") {
			continue
		}
		if strings.HasPrefix(trim, "scope ") {
			t.Errorf("expected non-comment scope opener stripped, found line: %q\nfull:\n%s", line, res.Expanded)
		}
		if trim == "endscope" {
			t.Errorf("expected non-comment endscope stripped, found line: %q\nfull:\n%s", line, res.Expanded)
		}
	}
	// Scopes don't appear in InitOrder.
	if len(res.InitOrder) != 0 {
		t.Errorf("scopes should not produce InitOrder entries, got: %+v", res.InitOrder)
	}
}

// TestPreprocessLibScope_LibraryWithNoInitNoPublic asserts the no-init,
// no-public case still emits one InitOrder entry with empty InitFunc.
func TestPreprocessLibScope_LibraryWithNoInitNoPublic(t *testing.T) {
	src := `library Empty
    private function _wat takes nothing returns nothing
    endfunction
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.InitOrder) != 1 {
		t.Fatalf("expected 1 library in InitOrder, got %d", len(res.InitOrder))
	}
	if res.InitOrder[0].InitFunc != "" {
		t.Errorf("expected empty InitFunc, got %q", res.InitOrder[0].InitFunc)
	}
}

// TestPreprocessLibScope_InternalRefMangled asserts a self-reference inside
// the library body uses the mangled name. (Same as PublicPrivate test but
// isolated — clearer failure message if this specific behavior breaks.)
func TestPreprocessLibScope_InternalRefMangled(t *testing.T) {
	src := `library Foo
    private function Helper takes nothing returns integer
        return 42
    endfunction

    public function Bar takes nothing returns integer
        return Helper()
    endfunction
endlibrary
`
	res := PreprocessLibScope(src)
	if !strings.Contains(res.Expanded, "return Foo___Helper()") {
		t.Errorf("expected internal call rewritten to Foo___Helper(), got:\n%s", res.Expanded)
	}
}

// TestPreprocessLibScope_MultipleLibrariesTopo asserts a multi-library file
// produces a valid topological order. We don't pin the exact ordering for
// independent libraries (the implementation uses alphabetical secondary
// ordering for stability) but we check the partial order: every required
// library appears before its requirer.
func TestPreprocessLibScope_MultipleLibrariesTopo(t *testing.T) {
	src := `library D requires B, C
endlibrary

library C requires A
endlibrary

library B requires A
endlibrary

library A
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.InitOrder) != 4 {
		t.Fatalf("expected 4 libraries, got %d", len(res.InitOrder))
	}
	pos := map[string]int{}
	for i, l := range res.InitOrder {
		pos[l.Name] = i
	}
	// Partial-order assertions.
	if pos["A"] > pos["B"] {
		t.Errorf("A must come before B; got order: %+v", names(res.InitOrder))
	}
	if pos["A"] > pos["C"] {
		t.Errorf("A must come before C; got order: %+v", names(res.InitOrder))
	}
	if pos["B"] > pos["D"] {
		t.Errorf("B must come before D; got order: %+v", names(res.InitOrder))
	}
	if pos["C"] > pos["D"] {
		t.Errorf("C must come before D; got order: %+v", names(res.InitOrder))
	}
}

func names(libs []LibraryInit) []string {
	out := make([]string, len(libs))
	for i, l := range libs {
		out[i] = l.Name
	}
	return out
}

// TestPreprocessLibScope_NeedsAndUsesSynonyms asserts JassHelper's `needs`
// and `uses` synonyms are treated as `requires`.
func TestPreprocessLibScope_NeedsAndUsesSynonyms(t *testing.T) {
	src := `library B needs A
endlibrary

library A
endlibrary
`
	res := PreprocessLibScope(src)
	if len(res.Errors) != 0 {
		t.Errorf("unexpected errors: %+v", res.Errors)
	}
	if len(res.InitOrder) < 2 || res.InitOrder[0].Name != "A" {
		t.Errorf("expected A before B with `needs`, got %+v", names(res.InitOrder))
	}

	src2 := `library B uses A
endlibrary

library A
endlibrary
`
	res2 := PreprocessLibScope(src2)
	if len(res2.InitOrder) < 2 || res2.InitOrder[0].Name != "A" {
		t.Errorf("expected A before B with `uses`, got %+v", names(res2.InitOrder))
	}
}

// TestPreprocessLibScope_GlobalsInside asserts public/private inside a
// globals block also get mangled.
func TestPreprocessLibScope_GlobalsInside(t *testing.T) {
	src := `library Foo
    globals
        public integer publicVar = 0
        private integer privateVar = 0
    endglobals
endlibrary
`
	res := PreprocessLibScope(src)
	if !strings.Contains(res.Expanded, "integer Foo_publicVar") {
		t.Errorf("expected public global renamed to Foo_publicVar, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "integer Foo___privateVar") {
		t.Errorf("expected private global renamed to Foo___privateVar, got:\n%s", res.Expanded)
	}
	if strings.Contains(res.Expanded, "public integer") || strings.Contains(res.Expanded, "private integer") {
		t.Errorf("expected visibility keywords stripped, got:\n%s", res.Expanded)
	}
}

// TestPreprocessLibScope_StringContentsNotMunged asserts the tokenizer-based
// rewrite does NOT rewrite identifier-shaped substrings inside quoted
// strings.
func TestPreprocessLibScope_StringContentsNotMunged(t *testing.T) {
	src := `library Foo
    private function Helper takes nothing returns nothing
        call BJDebugMsg("Helper says hi")
    endfunction
endlibrary
`
	res := PreprocessLibScope(src)
	if !strings.Contains(res.Expanded, `"Helper says hi"`) {
		t.Errorf("expected `Helper` inside string preserved, got:\n%s", res.Expanded)
	}
	if !strings.Contains(res.Expanded, "function Foo___Helper takes") {
		t.Errorf("expected decl still renamed, got:\n%s", res.Expanded)
	}
}

// TestPreprocessLibScope_ThenTranspile_EndToEnd checks a library-only source
// flows through Preprocess + libscope + TranspileScript without errors and
// produces valid-looking Lua.
func TestPreprocessLibScope_ThenTranspile_EndToEnd(t *testing.T) {
	src := `library Foo initializer InitFoo
    private function InitFoo takes nothing returns nothing
        call BJDebugMsg("init foo")
    endfunction
endlibrary
`
	pp := Preprocess(src)
	ls := PreprocessLibScope(pp.Expanded)
	if len(ls.Errors) != 0 {
		t.Fatalf("unexpected libscope errors: %+v", ls.Errors)
	}
	lua, err := TranspileScript(ls.Expanded)
	if err != nil {
		t.Fatalf("transpile after libscope failed: %v\nexpanded:\n%s", err, ls.Expanded)
	}
	if !strings.Contains(lua, "function Foo___InitFoo()") {
		t.Errorf("expected mangled function in lua output, got:\n%s", lua)
	}
}

// TestFindVJASSKeyword_LibraryAndScopeIgnored asserts FindVJASSKeyword no
// longer trips on library/scope after Phase 2 keyword-list pruning.
// Struct still blocks.
func TestFindVJASSKeyword_LibraryAndScopeIgnored(t *testing.T) {
	cases := []struct {
		name  string
		src   string
		found bool
	}{
		{
			"library_no_longer_blocks",
			`library X
endlibrary`,
			false,
		},
		{
			"scope_no_longer_blocks",
			`scope X
endscope`,
			false,
		},
		{
			"private_no_longer_blocks",
			`private function foo takes nothing returns nothing
endfunction`,
			false,
		},
		{
			"public_no_longer_blocks",
			`public function foo takes nothing returns nothing
endfunction`,
			false,
		},
		{
			"requires_no_longer_blocks",
			`library X requires Y
endlibrary`,
			false,
		},
		{
			// struct no longer blocks after Phase 3 — but the keyword scan
			// runs on the post-libscope source, NOT post-struct. So a bare
			// `struct ... endstruct` in the input here would still trip the
			// scan since this test exercises ONLY the libscope-to-keyword
			// gate. The real convert flow runs PreprocessStructs in between
			// (see scanVJASSAfterPreprocess in triggers_convert.go).
			"struct_still_trips_keyword_scan_in_this_test",
			`struct S
    integer x
endstruct`,
			false, // struct is removed from vJASSKeywords gate (Phase 3)
		},
		{
			"module_no_longer_blocks",
			`module M
endmodule`,
			false,
		},
		{
			"interface_no_longer_blocks",
			`interface I
endinterface`,
			false,
		},
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

// TestPreprocessLibScope_MultiLineRequiresClause covers a `requires` clause
// that lands on body lines instead of the opener tail — what happens when the
// opener interleaves /* */ comments to document each requirement and
// StripBlockComments has blanked them, leaving a standalone `requires` line and
// bare `optional Name` items. Those tokens can't begin a valid JASS statement,
// so the body must not carry them through to the transpiler; the real decls
// stay.
func TestPreprocessLibScope_MultiLineRequiresClause(t *testing.T) {
	src := "library Foo\n" +
		"requires\n" +
		"optional Bar\n" +
		"optional Baz ,\n" +
		"needs Qux\n" +
		"globals\n" +
		"    integer x = 0\n" +
		"endglobals\n" +
		"endlibrary\n"
	res := PreprocessLibScope(src)
	for _, ln := range strings.Split(res.Expanded, "\n") {
		trimmed := strings.TrimSpace(ln)
		for _, bad := range []string{"requires", "needs Qux", "optional Bar", "optional Baz"} {
			if strings.HasPrefix(trimmed, bad) {
				t.Errorf("dangling requires line %q should have been dropped, got line: %q", bad, ln)
			}
		}
	}
	if !strings.Contains(res.Expanded, "integer x = 0") {
		t.Errorf("real globals decl should survive:\n%s", res.Expanded)
	}
}
