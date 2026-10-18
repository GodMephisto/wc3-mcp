---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/structs.go"
ref: HEAD
---

package jass2lua

// structs.go — vJASS struct preprocessor (Phase 3 of vJASS support).
//
// JassHelper structs are the headliner vJASS feature: an integer-handle-based
// OOP layer over JASS. Each struct gets a hidden allocator that hands out
// unique integer indices; instance "members" are really global arrays indexed
// by those handles; instance methods are functions whose first parameter is
// the handle (implicitly named `this`); inheritance is a single-parent chain.
//
// Source shape:
//
//	struct Foo extends Bar
//	    integer x = 0                    -- instance field (with default)
//	    static integer count = 0         -- static (class-level) field
//	    readonly integer id              -- readonly (ignored — same as field for our purposes)
//	    delegate Bar parent              -- delegate (warning; treated as plain field)
//
//	    method greet takes nothing returns nothing
//	        call BJDebugMsg(I2S(this.x))
//	    endmethod
//
//	    static method foo takes integer y returns integer
//	        return y + 1
//	    endmethod
//
//	    static method create takes integer initial returns Foo
//	        local Foo self = Foo.allocate()
//	        set self.x = initial
//	        return self
//	    endmethod
//
//	    method onDestroy takes nothing returns nothing
//	        call BJDebugMsg("bye")
//	    endmethod
//
//	    static method onInit takes nothing returns nothing
//	        call BJDebugMsg("Foo initialized")
//	    endmethod
//	endstruct
//
// Target Lua mapping (metatable-based OOP):
//
//	-- struct Foo extends Bar
//	Foo = (Bar and setmetatable({}, {__index = Bar})) or {}
//	Foo.__index = Foo
//	Foo.count = 0
//	function Foo.allocate()
//	    return setmetatable({x = 0}, Foo)
//	end
//	function Foo:greet()
//	    BJDebugMsg(I2S(self.x))
//	end
//	function Foo.foo(y)
//	    return y + 1
//	end
//	function Foo.create(initial)
//	    local self = Foo.allocate()
//	    self.x = initial
//	    return self
//	end
//	function Foo:onDestroy()
//	    BJDebugMsg("bye")
//	end
//	function Foo:destroy()
//	    if self.onDestroy then self:onDestroy() end
//	    setmetatable(self, nil)
//	end
//	function Foo.onInit()
//	    BJDebugMsg("Foo initialized")
//	end
//
// Pipeline placement:
//
//	raw JASS
//	    → Preprocess           (textmacro expansion — Phase 1)
//	    → PreprocessLibScope   (library/scope — Phase 2)
//	    → PreprocessStructs    (this file — Phase 3)
//	    → scanVJASS            (module/interface/define gate)
//	    → blocker OR transpile + struct-Lua splice
//
// Strategy: structs DON'T translate to JASS — they translate directly to Lua.
// So this pass:
//  1. Parses every `struct ... endstruct` block, capturing fields/methods.
//  2. STRIPS the entire block from the source and replaces it with a single
//     marker comment line:  `// __VJASS_STRUCT__ Foo`
//  3. Outside struct bodies, rewrites:
//     - `Foo.method(args)` (Foo is a known struct name) — leave alone (Lua
//       handles dot-call for static methods).
//     - `instance.method(args)` (receiver not a struct name) — rewrite to
//       `instance:method(args)` so Lua dispatches via __index.
//     - `instance.field` access on a non-struct-name receiver stays as-is.
//     - `set instance.field = value` — `set` is dropped by the transpiler
//       already; the dot survives. Lua semantics match.
//  4. Inside struct method bodies (which got STRIPPED out of the JASS source),
//     a separate rewrite happens during emit:
//     - `this` → `self`  (Lua self)
//     - `thistype` → containing struct name
//     - `set` prefix dropped (same as top-level rewriting)
//
// The caller (triggers_convert.go) takes the StructDef list from this result,
// asks emitStructDefsLua for the Lua snippets, then splices them at the
// marker positions in the transpiled output. Marker text passes through the
// transpiler verbatim — `// __VJASS_STRUCT__ Foo` becomes `-- __VJASS_STRUCT__
// Foo` in the Lua output, which gives us a deterministic splice anchor.
//
// What's intentionally not supported in v1 (Phase 4 / post-v1):
//   - `extends array` is treated identically to plain struct (auto-allocate).
//     The "array" marker would require user-managed indices and is rare.
//   - `delegate` is treated as a regular member; method-fallthrough is NOT
//     synthesized. A warning is emitted.
//   - `interface` / `module` / `define` — Phase 4.
//   - Struct member access type inference (we can't disambiguate `foo.bar`
//     as static vs instance with perfect accuracy; the heuristic is
//     receiver-name capitalization + matches-known-struct).

import (
	"fmt"
	"regexp"
	"sort"
	"strings"
)

// StructResult is the output of PreprocessStructs.
type StructResult struct {
	// Expanded is the source with all struct blocks stripped, replaced by
	// `// __VJASS_STRUCT__ <Name>` marker comments. References to struct
	// methods outside the bodies are rewritten to colon-syntax where the
	// heuristic identifies an instance receiver.
	Expanded string

	// Structs is the parsed struct definitions, keyed by name. The caller
	// uses these to emit Lua snippets at the marker positions.
	Structs map[string]StructDef

	// StructOrder preserves source-order so the emitted Lua has a stable
	// layout independent of map iteration.
	StructOrder []string

	// Inits is the list of structs with a `static method onInit` — the
	// codegen threads these into _vjass_init_libs() so each Foo.onInit()
	// fires at map load, AFTER library initializers.
	Inits []StructInit

	// Errors is the non-fatal diagnostics bucket (parse errors, unsupported
	// extends-array warning, delegate warning, etc.).
	Errors []StructError
}

// StructDef captures one parsed struct.
type StructDef struct {
	Name     string
	Extends  string // "" for no inheritance; "array" treated as "" + warning
	Fields   []StructField
	Statics  []StructField // static fields
	Methods  []StructMethod
	HasCreate    bool // user defined `static method create` — skip auto-synthesis
	HasOnDestroy bool // user defined `method onDestroy`
	HasDestroy   bool // user defined `method destroy` — skip auto-synthesis
	HasAllocate  bool // user defined `static method allocate` — skip auto-synthesis
	OnInit       string // method name (usually "onInit"); "" if none
	// Delegates is the list of delegate-typed field NAMES (in source order).
	// When a method-or-field lookup on an instance fails, we fall through to
	// these in declaration order. Phase 4 emits an `__index` metamethod for
	// every struct with at least one delegate.
	Delegates []string
	StartLine    int
	EndLine      int
}

// StructField is one instance or static member.
type StructField struct {
	Name     string
	Type     string
	Default  string // raw JASS source for the initializer, "" if none
	Array    bool
	Static   bool
	Delegate bool
	Readonly bool
}

// StructMethod is one instance or static method.
//
// Phase 5: operator-overload methods (`static method operator [] takes …`,
// `method operator < takes …`, etc.) populate Operator with the operator
// token. The emitter then maps to a Lua metamethod (`__index`, `__lt`, …)
// when possible, or to a regular method named `_op_<op>` with a warning when
// the mapping isn't supported.
type StructMethod struct {
	Name     string
	Static   bool
	Params   []StructParam
	Returns  string // "" for `returns nothing`
	Body     string // raw JASS body bytes (the contents between method/endmethod)
	Operator string // "[]", "<", "==", "+", etc.; "" for non-operator methods
}

// StructParam is one entry of a method's `takes` list.
type StructParam struct {
	Type string
	Name string
}

// StructInit names a struct + its onInit method for codegen threading.
type StructInit struct {
	StructName string
	InitMethod string
}

// StructError is a non-fatal diagnostic surfaced to the UI.
type StructError struct {
	Line    int
	Message string
}

// markerPrefix is the comment marker we splice into the source for each
// struct. The transpiler turns `// ...` into `-- ...` in the Lua output, so
// the splice anchor we look for downstream is `-- __VJASS_STRUCT__ <Name>`.
const structMarkerPrefix = "// __VJASS_STRUCT__ "

// StructLuaMarker is the splice anchor in the post-transpile Lua output.
const StructLuaMarker = "-- __VJASS_STRUCT__ "

// ---------------------------------------------------------------------------
// Regex helpers.
// ---------------------------------------------------------------------------

var (
	structOpenerRe = regexp.MustCompile(`^\s*struct\s+([A-Za-z_][A-Za-z0-9_]*)(?:\s+extends\s+([A-Za-z_][A-Za-z0-9_]*))?\s*$`)
	structCloserRe = regexp.MustCompile(`^\s*endstruct\s*$`)
	// keywordDeclRe matches a vJASS `keyword` forward-declaration line, e.g.
	// `private keyword roflcopter`. Stripped in PreprocessStructs (emits nothing).
	keywordDeclRe = regexp.MustCompile(`^\s*(?:private\s+|public\s+)?keyword\s+[A-Za-z_][A-Za-z0-9_]*\s*$`)

	// methodOpenerRe matches both instance + static methods (no operator).
	// Groups: 1=optional `static`, 2=name, 3=takes-list (or "nothing"),
	//         4=return type (or "nothing").
	methodOpenerRe = regexp.MustCompile(`^\s*(static\s+)?method\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes\s+(.+?)\s+returns\s+([A-Za-z_][A-Za-z0-9_]*)\s*$`)
	methodCloserRe = regexp.MustCompile(`^\s*endmethod\s*$`)

	// operatorOpenerRe matches operator-overload method openers (Phase 5):
	//   static method operator [] takes …          → array-read overload
	//   method operator []= takes …                → array-write overload
	//   method operator < takes …                  → comparison overload
	//   method operator + takes …                  → arithmetic overload
	//   method operator nameProperty takes …       → property-getter (vJASS)
	//   method operator nameProperty= takes …      → property-setter (vJASS)
	// Groups: 1=optional `static`, 2=operator token, 3=takes-list, 4=return type.
	//
	// Property-style operator overloads (`operator name`, `operator name=`) are
	// vJASS sugar: `x.name` invokes `operator name`; `set x.name = v` invokes
	// `operator name=`. The Lua mapping degrades these into normally-named
	// methods (`_op_name_get`, `_op_name_set`) since Lua __index/__newindex are
	// the only generic property hooks and they collide with method dispatch.
	operatorOpenerRe = regexp.MustCompile(`^\s*(static\s+)?method\s+operator\s+(\[\]=|\[\]|==|!=|<=|>=|<|>|\+|-|\*|/|%|=|[A-Za-z_][A-Za-z0-9_]*=?)\s+takes\s+(.+?)\s+returns\s+([A-Za-z_][A-Za-z0-9_]*)\s*$`)

	// fieldRe matches a struct field declaration. Groups:
	//   1=type, 2=name, 3=optional default expression.
	// Phase 5 changes:
	//   - The `=` clause + RHS is fully optional (bare decls like `unit s`
	//     default to the type's zero).
	//   - Trailing inline comments (`// …`) are stripped BEFORE the regex runs,
	//     so they don't pollute the default-expression capture.
	// We handle leading modifiers (static / readonly / delegate / constant)
	// in the parser body since regex can't capture all orderings cleanly.
	fieldRe = regexp.MustCompile(`^\s*([A-Za-z_][A-Za-z0-9_]*)\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)(?:\s*=\s*(.+?))?\s*$`)
)

// ---------------------------------------------------------------------------
// Entry point.
// ---------------------------------------------------------------------------

// PreprocessStructs parses + strips vJASS struct blocks. Call AFTER
// PreprocessLibScope + PreprocessModules and BEFORE the vJASS keyword gate /
// transpiler.
//
// modules (optional, may be nil) supplies the parsed module definitions so
// that `implement Foo` lines inside struct bodies inline the module's
// fields + methods. Pass nil for the common single-pass case (tests, etc.).
//
// knownNames (optional, may be nil) supplies a set of top-level names — library
// functions, top-level globals, etc. — that the dot-vs-colon rewrite consults
// to leave receivers like `Foo_doThing` alone (dot). Without it, the rewriter
// falls back to "anything not a struct name is colon", which mis-translates
// enum-like globals and library-exposed function calls.
//
// Conservative on errors: malformed struct opener, missing endstruct, unknown
// field modifier — every one produces a StructError + a comment marker in
// the output, but processing continues.
func PreprocessStructs(src string, modules map[string]ModuleDef, knownNames map[string]bool) StructResult {
	res := StructResult{
		Structs: map[string]StructDef{},
	}
	lines := splitLinesKeepEnd(src)

	// Drop vJASS `keyword` forward-declarations (`[private|public] keyword Foo`).
	// They pre-announce a struct/type name so it can be referenced before its
	// definition; JassHelper emits nothing for them and they are not valid
	// statements, so left in place they reach the statement parser as
	// `unexpected token "keyword"`. Replace (not remove) with a marker comment so
	// downstream line numbers stay stable.
	for idx, ln := range lines {
		if keywordDeclRe.MatchString(strings.TrimRight(ln, "\r\n")) {
			lines[idx] = "// jass2lua: vJASS keyword forward-declaration dropped\n"
		}
	}

	// Pass 1 — extract blocks; replace each block with a marker comment.
	// Module `implement Foo` inlining happens INSIDE extractStructBlocks
	// before parseStructBody runs, so the resolved struct sees the merged
	// surface.
	stripped, blocks := extractStructBlocks(lines, modules, &res)

	// Pass 2 — register parsed defs.
	for _, b := range blocks {
		if _, dup := res.Structs[b.Name]; dup {
			res.Errors = append(res.Errors, StructError{
				Line:    b.StartLine,
				Message: fmt.Sprintf("duplicate struct %q; second definition ignored", b.Name),
			})
			continue
		}
		res.Structs[b.Name] = b
		res.StructOrder = append(res.StructOrder, b.Name)
		if b.OnInit != "" {
			res.Inits = append(res.Inits, StructInit{
				StructName: b.Name,
				InitMethod: b.OnInit,
			})
		}
	}

	// Pass 3 — rewrite struct-method-call sites OUTSIDE struct bodies. We
	// tokenize the stripped source and emit dot/colon based on receiver
	// identity. Lex errors degrade to leaving the source alone (the downstream
	// transpiler will surface its own error).
	rewritten, err := rewriteStructRefs(stripped, res.Structs, knownNames)
	if err != nil {
		res.Errors = append(res.Errors, StructError{
			Line:    0,
			Message: fmt.Sprintf("struct ref-rewrite lex error: %v; leaving source unchanged", err),
		})
		res.Expanded = stripped
		return res
	}
	res.Expanded = rewritten
	return res
}

// EmitStructLua produces the Lua block for one struct. Used by the caller
// (triggers_convert.go) to splice into the transpiled output at each
// `-- __VJASS_STRUCT__ Foo` marker.
//
// The output is a self-contained Lua snippet (no surrounding indentation,
// trailing newline included).
func EmitStructLua(s StructDef) string {
	var b strings.Builder
	emitStructLua(&b, s)
	return b.String()
}

// EmitAllStructLua emits every struct's Lua snippet in StructOrder order.
// Useful for callers that want one big string rather than per-struct splices.
func EmitAllStructLua(res StructResult) string {
	var b strings.Builder
	for _, name := range res.StructOrder {
		s, ok := res.Structs[name]
		if !ok {
			continue
		}
		emitStructLua(&b, s)
		b.WriteByte('\n')
	}
	return b.String()
}

// SpliceStructLua replaces every `-- __VJASS_STRUCT__ <Name>` marker line in
// `lua` with the emitted Lua snippet for that struct. Markers for unknown
// names round-trip unchanged (defensive — shouldn't happen in practice).
//
// The marker line is replaced WHOLE; surrounding lines are preserved
// verbatim. This is the splice anchor the marker-comment strategy relies on.
func SpliceStructLua(lua string, res StructResult) string {
	out, _ := SpliceStructLuaWithErrors(lua, res)
	return out
}

// SpliceStructLuaWithErrors is the error-surfacing variant of SpliceStructLua.
// It performs the identical splice AND returns the per-statement parse errors
// recovered while transpiling each spliced struct's method bodies. Each error
// maps 1:1 to an inline `error("jass2lua failed: ...")` marker the emitter wrote
// inside a struct method, and is prefixed with the owning struct + method name.
//
// This closes the P0#1 gap: struct method-body transpile failures used to be
// emitted as inline error() markers but were swallowed (emitStructLua returned
// only a string), so section.Errors under-reported them to 0. Callers that want
// the section's reported error count to match reality (transpileSectionScript /
// transpileSectionCustomText) use this variant and append the returned errors.
func SpliceStructLuaWithErrors(lua string, res StructResult) (string, []string) {
	if len(res.Structs) == 0 {
		return lua, nil
	}
	var out strings.Builder
	out.Grow(len(lua) + 1024)
	var errs []string
	lines := splitLinesKeepEnd(lua)
	for _, line := range lines {
		trim := strings.TrimSpace(strings.TrimRight(line, "\r\n"))
		if strings.HasPrefix(trim, StructLuaMarker) {
			name := strings.TrimSpace(strings.TrimPrefix(trim, StructLuaMarker))
			s, ok := res.Structs[name]
			if !ok {
				out.WriteString(line)
				continue
			}
			emitStructLuaInto(&out, s, &errs)
			continue
		}
		out.WriteString(line)
	}
	return out.String(), errs
}

// ---------------------------------------------------------------------------
// Pass 1 — block extraction.
// ---------------------------------------------------------------------------

// extractStructBlocks walks `lines`, pulls out every `struct ... endstruct`
// block, and returns (a) the source with each block replaced by a single
// marker comment, and (b) the parsed defs in source order.
//
// modules (may be nil) supplies module defs for inline expansion when an
// `implement Foo` (or `implement optional Foo`) line appears inside a struct
// body. The module body is pasted in verbatim BEFORE parseStructBody runs;
// each implementing struct gets its own copy.
//
// Edge cases handled here (Phase 4):
//   - `implement Foo` / `implement optional Foo` — inline-paste module body.
//   - `static if SOMETHING` / `endif` inside struct body — strip the keyword
//     pair and emit body verbatim (always-take-first semantics; we don't
//     evaluate the condition).
//
// Nested structs are NOT supported (vJASS forbids them); an inner `struct`
// inside a struct body falls through as body text and the parser will surface
// a diagnostic.
func extractStructBlocks(lines []string, modules map[string]ModuleDef, res *StructResult) (string, []StructDef) {
	var out strings.Builder
	var blocks []StructDef
	i := 0
	for i < len(lines) {
		line := lines[i]
		trim := strings.TrimRight(line, "\r\n")
		m := structOpenerRe.FindStringSubmatch(trim)
		if m == nil {
			out.WriteString(line)
			i++
			continue
		}
		// Found a struct opener. Collect body until endstruct.
		startLine := i + 1
		def := StructDef{
			Name:      m[1],
			Extends:   m[2],
			StartLine: startLine,
		}
		if def.Extends == "array" {
			// `extends array` — treat like plain struct (auto-allocate) +
			// warning. Real-world usage is rare; full semantics (user-managed
			// indices) require user-managed indices we don't synthesize.
			res.Errors = append(res.Errors, StructError{
				Line:    startLine,
				Message: fmt.Sprintf("struct %q: `extends array` is treated as a normal struct (auto-allocated); user-managed indices not yet supported", def.Name),
			})
			def.Extends = ""
		}
		// Collect body lines + parse them.
		var bodyLines []string
		i++ // past opener
		closed := false
		for i < len(lines) {
			inner := lines[i]
			innerTrim := strings.TrimRight(inner, "\r\n")
			if structCloserRe.MatchString(innerTrim) {
				closed = true
				i++
				break
			}
			bodyLines = append(bodyLines, inner)
			i++
		}
		def.EndLine = i
		if !closed {
			res.Errors = append(res.Errors, StructError{
				Line:    startLine,
				Message: fmt.Sprintf("struct %q: missing `endstruct` before EOF", def.Name),
			})
		}
		// Phase 4 pre-passes on the body BEFORE parseStructBody walks it:
		//   1. Inline `implement Foo` lines from the modules map.
		//   2. Strip `static if … endif` keyword pairs (keep the body).
		//   3. Strip `debug ` prefix from `debug method` openers.
		bodyLines = inlineImplements(def.Name, bodyLines, modules, res, startLine)
		bodyLines = stripStaticIf(bodyLines)
		bodyLines = stripDebugMethodPrefix(bodyLines)
		parseStructBody(&def, bodyLines, res)
		// Emit the marker comment in place. Use a leading blank line so it's
		// easy to spot in the converted Lua.
		out.WriteString(structMarkerPrefix + def.Name + "\n")
		blocks = append(blocks, def)
	}
	return out.String(), blocks
}

// implementRe matches a struct-body `implement [optional] Foo` line.
//
//	implement Foo            → group1="", group2="Foo"
//	implement optional Foo   → group1="optional ", group2="Foo"
var implementRe = regexp.MustCompile(`^\s*implement\s+(optional\s+)?([A-Za-z_][A-Za-z0-9_]*)\s*$`)

// staticIfRe / staticElseifRe / staticElseRe / staticEndifRe match struct-body
// `static if … (elseif|else) … endif` blocks. JassHelper supports a static if
// (compile-time conditional) wrapping arbitrary member declarations. We always
// take the FIRST branch — there's no evaluator for the condition at this
// layer. Phase 5: extended to ALSO strip the else/elseif branches (Phase 3+4
// only stripped the bare opener+endif, which let `else` and `endif` lines
// leak through into parseStructBody as unrecognized).
var (
	staticIfRe     = regexp.MustCompile(`^\s*static\s+if\b`)
	staticElseifRe = regexp.MustCompile(`^\s*elseif\b`)
	staticElseRe   = regexp.MustCompile(`^\s*else\s*$`)
	staticEndifRe  = regexp.MustCompile(`^\s*endif\s*$`)
)

// debugMethodPrefixRe matches a `debug method` / `debug static method` opener.
// We strip the leading `debug ` so the rest is treated as a regular method.
var debugMethodPrefixRe = regexp.MustCompile(`^(\s*)debug\s+(static\s+method\b|method\b)`)

// inlineImplements walks bodyLines and replaces any `implement Foo` /
// `implement optional Foo` line with the body of module Foo (when present).
// Missing modules: `implement Foo` → diagnostic + marker comment;
// `implement optional Foo` → silent + marker comment.
//
// Each implementing struct gets its own copy of the module body — modules
// can be implemented by multiple structs without sharing.
func inlineImplements(structName string, bodyLines []string, modules map[string]ModuleDef, res *StructResult, structStartLine int) []string {
	var out []string
	for idx, line := range bodyLines {
		trim := strings.TrimRight(line, "\r\n")
		m := implementRe.FindStringSubmatch(trim)
		if m == nil {
			out = append(out, line)
			continue
		}
		isOptional := strings.TrimSpace(m[1]) == "optional"
		modName := m[2]
		mod, ok := modules[modName]
		if !ok {
			if isOptional {
				out = append(out, fmt.Sprintf("// jass2lua: implement optional %s — module not found; skipped\n", modName))
				continue
			}
			res.Errors = append(res.Errors, StructError{
				Line:    structStartLine + idx,
				Message: fmt.Sprintf("struct %q: implement %s — unknown module; pasting empty body", structName, modName),
			})
			out = append(out, fmt.Sprintf("// jass2lua: implement %s — module not found; pasting empty body\n", modName))
			continue
		}
		// Paste the module's body. Each implementing struct gets its own copy.
		// The body is SANITIZED first: a module body can legally contain
		// constructs that are NOT valid struct members (top-level globals /
		// functions / interfaces / nested structs) and — in interface-style
		// "documentation" modules — bodyless method signatures and valueless
		// constant decls that describe what an implementer SHOULD write. Pasted
		// verbatim into a struct those derail the struct body parser (a bodyless
		// `static method onX takes … returns …` makes collectMethodBody
		// over-collect every following declaration as its "body", which the
		// statement parser then rejects token-by-token). sanitizeModuleBodyForPaste
		// drops those so a well-formed module inlines cleanly and a
		// documentation module degrades to comments instead of an error cascade.
		out = append(out, fmt.Sprintf("// jass2lua: implement %s — module body pasted\n", modName))
		out = append(out, sanitizeModuleBodyForPaste(mod.Raw)...)
	}
	return out
}

// Helpers for sanitizeModuleBodyForPaste: openers/closers for the top-level
// constructs a module body may contain that are NOT valid inside a struct body.
var (
	moduleGlobalsOpenerRe   = regexp.MustCompile(`^\s*globals\s*$`)
	moduleGlobalsCloserRe   = regexp.MustCompile(`^\s*endglobals\s*$`)
	moduleFunctionOpenerRe  = regexp.MustCompile(`^\s*(?:private\s+|public\s+|constant\s+)*function\s+[A-Za-z_]`)
	moduleFunctionCloserRe  = regexp.MustCompile(`^\s*endfunction\s*$`)
	moduleInterfaceOpenerRe = regexp.MustCompile(`^\s*interface\s+[A-Za-z_]`)
	moduleInterfaceCloserRe = regexp.MustCompile(`^\s*endinterface\s*$`)
	// valuelessConstantRe matches an interface-style constant DECLARATION with
	// no initializer (`constant real Foo_BAR`). A real `constant x = v` field
	// has an `=` and is left for parseStructBody (it handles static constants).
	valuelessConstantRe = regexp.MustCompile(`^\s*constant\s+[A-Za-z_][A-Za-z0-9_]*\s+[A-Za-z_][A-Za-z0-9_]*\s*$`)
)

// sanitizeModuleBodyForPaste cleans a module's raw body so it can be pasted
// into a struct body without derailing parseStructBody / the statement parser.
// It drops constructs that are invalid as struct members — top-level
// globals/function/interface/nested-struct blocks, bodyless method signatures
// (no endmethod before the next opener), and valueless constant declarations —
// replacing each with a marker comment. Well-formed methods (opener … body …
// endmethod) and real field declarations pass through untouched.
func sanitizeModuleBodyForPaste(raw string) []string {
	lines := splitLinesKeepEnd(raw)
	var out []string
	i := 0
	for i < len(lines) {
		t := strings.TrimSpace(stripInlineComment(strings.TrimRight(lines[i], "\r\n")))
		switch {
		case t == "" || strings.HasPrefix(t, "//"):
			out = append(out, lines[i])
			i++
		case moduleGlobalsOpenerRe.MatchString(t):
			i = skipModuleBlock(lines, i+1, moduleGlobalsCloserRe)
			out = append(out, "\t// jass2lua: module globals block dropped from struct paste\n")
		case moduleInterfaceOpenerRe.MatchString(t):
			i = skipModuleBlock(lines, i+1, moduleInterfaceCloserRe)
			out = append(out, "\t// jass2lua: module interface block dropped from struct paste\n")
		case moduleFunctionOpenerRe.MatchString(t):
			i = skipModuleBlock(lines, i+1, moduleFunctionCloserRe)
			out = append(out, "\t// jass2lua: module free function dropped from struct paste\n")
		case structOpenerRe.MatchString(t):
			i = skipModuleBlock(lines, i+1, structCloserRe)
			out = append(out, "\t// jass2lua: nested struct dropped from struct paste\n")
		case methodOpenerRe.MatchString(t) || operatorOpenerRe.MatchString(t):
			if end := moduleMethodBodyEnd(lines, i+1); end >= 0 {
				out = append(out, lines[i:end+1]...) // opener … endmethod inclusive
				i = end + 1
			} else {
				out = append(out, "\t// jass2lua: bodyless interface method signature dropped from struct paste\n")
				i++
			}
		case valuelessConstantRe.MatchString(t):
			out = append(out, "\t// jass2lua: bodyless constant declaration dropped from struct paste\n")
			i++
		default:
			out = append(out, lines[i])
			i++
		}
	}
	return out
}

// skipModuleBlock returns the index just past the first line matching closer,
// starting the scan at start. If no closer is found the whole remainder is
// consumed (a malformed block can't be safely pasted anyway).
func skipModuleBlock(lines []string, start int, closer *regexp.Regexp) int {
	for j := start; j < len(lines); j++ {
		if closer.MatchString(strings.TrimSpace(lines[j])) {
			return j + 1
		}
	}
	return len(lines)
}

// moduleMethodBodyEnd returns the index of the endmethod closing a method whose
// opener sits just before start, or -1 if the method is bodyless — i.e. another
// method/operator opener or the struct closer appears before any endmethod
// (the hallmark of an interface-style signature-only declaration). Method
// bodies hold only statements, which never match a method opener, so this scan
// is unambiguous.
func moduleMethodBodyEnd(lines []string, start int) int {
	for j := start; j < len(lines); j++ {
		t := strings.TrimSpace(stripInlineComment(lines[j]))
		if methodCloserRe.MatchString(t) {
			return j
		}
		if methodOpenerRe.MatchString(t) || operatorOpenerRe.MatchString(t) || structCloserRe.MatchString(t) {
			return -1
		}
	}
	return -1
}

// stripStaticIf walks bodyLines and removes `static if SOMETHING …
// (elseif/else) … endif` blocks, KEEPING only the FIRST branch's body (no
// condition evaluation). Nested static-ifs are supported via a stack.
//
// State per open static-if (stack frame):
//   emitting=true  — we're inside the first branch (the one we always take)
//   emitting=false — we've passed `else` / `elseif`, so skip until `endif`
//
// Plain `endif` inside a struct body is rare outside this context; we only
// treat it as a closer when we're tracking an open `static if`.
func stripStaticIf(bodyLines []string) []string {
	if len(bodyLines) == 0 {
		return bodyLines
	}
	var out []string
	// stack[i] == true means we're currently emitting at depth i.
	var stack []bool
	for _, line := range bodyLines {
		trim := strings.TrimRight(line, "\r\n")
		if staticIfRe.MatchString(trim) {
			// New nested static-if. Inherit emitting-state from the parent
			// frame (if any) — a static-if inside a skipped branch stays
			// skipped.
			parentEmitting := true
			if len(stack) > 0 && !stack[len(stack)-1] {
				parentEmitting = false
			}
			stack = append(stack, parentEmitting)
			continue
		}
		if len(stack) > 0 && staticEndifRe.MatchString(trim) {
			stack = stack[:len(stack)-1]
			continue
		}
		if len(stack) > 0 && (staticElseRe.MatchString(trim) || staticElseifRe.MatchString(trim)) {
			// Transition off the first branch. We never re-enable emit; the
			// always-take-first policy means every later branch is dropped.
			stack[len(stack)-1] = false
			continue
		}
		// Check whether we're currently in a skipped region.
		if len(stack) > 0 && !stack[len(stack)-1] {
			continue
		}
		out = append(out, line)
	}
	return out
}

// stripDebugMethodPrefix walks bodyLines and removes a leading `debug ` from
// any method opener (`debug method foo …`, `debug static method bar …`).
// Pure-JASS already accepts `debug call …` as a statement; structs add the
// `debug method` form which we treat identically to a regular method.
func stripDebugMethodPrefix(bodyLines []string) []string {
	if len(bodyLines) == 0 {
		return bodyLines
	}
	out := make([]string, 0, len(bodyLines))
	for _, line := range bodyLines {
		// $1 = indent, $2 = `method` or `static method` keyword head.
		rewritten := debugMethodPrefixRe.ReplaceAllString(line, "$1$2")
		out = append(out, rewritten)
	}
	return out
}

// stripInlineComment removes a trailing `// …` comment from a struct-body
// line, returning the bytes BEFORE the comment marker. String literals don't
// matter at the field-decl level (field initializers are constants — strings
// containing `//` are rare). Phase 5 addition; lets us accept lines like
//   `real av        // alpha value`
// as a bare field decl + an annotation we currently drop on the floor.
func stripInlineComment(s string) string {
	if i := strings.Index(s, "//"); i >= 0 {
		return strings.TrimRight(s[:i], " \t")
	}
	return s
}

// parseStructBody walks the lines between `struct ... endstruct` and
// populates def.Fields / def.Statics / def.Methods / def.OnInit. Unrecognized
// lines produce a diagnostic but processing continues.
//
// Phase 5 additions:
//   - Inline `// comment` on field decls is stripped before field-regex.
//   - Bare field decls (`unit s`) without `= default` are accepted; type's
//     zero is used at emit time.
//   - `constant` and `static constant` field modifiers parse correctly.
//   - `static method operator [] takes …` (and other operator overloads)
//     parse as StructMethod with Operator set; emit maps to Lua metamethods.
func parseStructBody(def *StructDef, lines []string, res *StructResult) {
	i := 0
	for i < len(lines) {
		line := lines[i]
		trim := strings.TrimRight(line, "\r\n")
		bare := strings.TrimSpace(trim)
		if bare == "" || strings.HasPrefix(bare, "//") {
			i++
			continue
		}
		// Stray `implement` line — when extractStructBlocks didn't have a
		// modules map (parseStructBody is called directly without
		// inlineImplements) we silently drop them with a marker. We don't
		// emit a hard diagnostic so the no-module-context call sites (e.g.
		// module-parsing recursion) stay clean.
		if implementRe.MatchString(trim) {
			i++
			continue
		}
		// Strip trailing inline comments so method/field openers with
		// `// comment` tails match cleanly (Phase 5). The bare/full strings
		// were declared above — re-derive trimNoComment here for the
		// method/operator regex matches without altering `trim` (still used
		// for field-modifier matching below, which has its own strip pass).
		trimNoComment := stripInlineComment(trim)
		// Operator-overload method opener (Phase 5)?
		if om := operatorOpenerRe.FindStringSubmatch(trimNoComment); om != nil {
			method := StructMethod{
				Static:   strings.TrimSpace(om[1]) == "static",
				Operator: om[2],
				Name:     "_op_" + sanitizeOperatorName(om[2]),
			}
			method.Params = parseMethodParams(om[3])
			if om[4] != "nothing" {
				method.Returns = om[4]
			}
			collectMethodBody(def, &method, lines, &i, res)
			def.Methods = append(def.Methods, method)
			continue
		}
		// Method opener?
		if m := methodOpenerRe.FindStringSubmatch(trimNoComment); m != nil {
			method := StructMethod{
				Static: strings.TrimSpace(m[1]) == "static",
				Name:   strings.TrimSpace(m[2]),
			}
			method.Params = parseMethodParams(m[3])
			if m[4] != "nothing" {
				method.Returns = m[4]
			}
			collectMethodBody(def, &method, lines, &i, res)
			def.Methods = append(def.Methods, method)
			switch {
			case method.Static && method.Name == "create":
				def.HasCreate = true
			case method.Static && method.Name == "allocate":
				def.HasAllocate = true
			case !method.Static && method.Name == "onDestroy":
				def.HasOnDestroy = true
			case !method.Static && method.Name == "destroy":
				def.HasDestroy = true
			case method.Static && method.Name == "onInit":
				def.OnInit = "onInit"
			}
			continue
		}
		// Field declaration. Parse leading modifiers manually. Phase 5 adds
		// `constant` and `debug` (alongside the existing
		// static/readonly/delegate). `debug` on a field is a JassHelper
		// conditional — the field only exists in debug builds. We treat it
		// as a regular field (always emit it) since Lua has no per-mode
		// compilation concept here.
		modStatic := false
		modReadonly := false
		modDelegate := false
		modConstant := false
		// Strip a trailing inline `// …` comment FIRST so it doesn't pollute
		// the field-default capture or modifier matching.
		rest := stripInlineComment(trim)
		for {
			b := strings.TrimSpace(rest)
			if strings.HasPrefix(b, "static ") {
				modStatic = true
				rest = strings.TrimPrefix(b, "static ")
				continue
			}
			if strings.HasPrefix(b, "readonly ") {
				modReadonly = true
				rest = strings.TrimPrefix(b, "readonly ")
				continue
			}
			if strings.HasPrefix(b, "delegate ") {
				modDelegate = true
				rest = strings.TrimPrefix(b, "delegate ")
				continue
			}
			if strings.HasPrefix(b, "constant ") {
				modConstant = true
				rest = strings.TrimPrefix(b, "constant ")
				continue
			}
			if strings.HasPrefix(b, "debug ") {
				// `debug` on a field: JassHelper-conditional, drop the
				// prefix and treat as a normal field.
				rest = strings.TrimPrefix(b, "debug ")
				continue
			}
			rest = b
			break
		}
		fm := fieldRe.FindStringSubmatch(rest)
		if fm == nil {
			res.Errors = append(res.Errors, StructError{
				Line:    def.StartLine + i,
				Message: fmt.Sprintf("struct %q: unrecognized body line %q", def.Name, bare),
			})
			i++
			continue
		}
		field := StructField{
			Type:     fm[1],
			Name:     fm[2],
			Default:  strings.TrimSpace(fm[3]),
			Array:    strings.Contains(rest, "array "),
			Static:   modStatic,
			Readonly: modReadonly,
			Delegate: modDelegate,
		}
		if modDelegate && !modStatic {
			// Track the delegate name so emitStructLua wires up the
			// __index metamethod fallthrough.
			def.Delegates = append(def.Delegates, field.Name)
		}
		_ = modConstant // Lua has no enforced const; emit annotation only.
		if modStatic {
			def.Statics = append(def.Statics, field)
		} else {
			def.Fields = append(def.Fields, field)
		}
		i++
	}
}

// collectMethodBody reads lines from *i until `endmethod`, populating
// method.Body with the captured bytes. Records a StructError if `endmethod`
// is missing before the body runs out. Advances *i past the `endmethod` line
// (or to the end of lines if missing).
func collectMethodBody(def *StructDef, method *StructMethod, lines []string, i *int, res *StructResult) {
	var body strings.Builder
	*i++ // step past opener
	closed := false
	for *i < len(lines) {
		inner := lines[*i]
		if methodCloserRe.MatchString(strings.TrimRight(inner, "\r\n")) {
			closed = true
			*i++
			break
		}
		body.WriteString(inner)
		*i++
	}
	if !closed {
		opName := method.Name
		if method.Operator != "" {
			opName = "operator " + method.Operator
		}
		res.Errors = append(res.Errors, StructError{
			Line:    def.StartLine,
			Message: fmt.Sprintf("struct %q method %q: missing `endmethod`", def.Name, opName),
		})
	}
	method.Body = body.String()
}

// sanitizeOperatorName turns an operator token into an identifier-safe suffix
// for the fallback method name (e.g. `[]` → `index`, `<` → `lt`, `+` → `add`).
// Used when the emitter can't map the operator to a Lua metamethod and falls
// back to a regular named method.
//
// Property-style operator overloads (vJASS sugar — `operator name` /
// `operator name=`) map to `<name>_get` / `<name>_set` so the fallback method
// reads cleanly without colliding with regular method names.
func sanitizeOperatorName(op string) string {
	switch op {
	case "[]":
		return "index"
	case "[]=":
		return "newindex"
	case "<":
		return "lt"
	case "<=":
		return "le"
	case ">":
		return "gt"
	case ">=":
		return "ge"
	case "==":
		return "eq"
	case "!=":
		return "ne"
	case "=":
		return "assign"
	case "+":
		return "add"
	case "-":
		return "sub"
	case "*":
		return "mul"
	case "/":
		return "div"
	case "%":
		return "mod"
	}
	// Property-style operator (identifier-shaped op). `foo=` is the setter,
	// `foo` is the getter.
	if strings.HasSuffix(op, "=") {
		return strings.TrimSuffix(op, "=") + "_set"
	}
	return op + "_get"
}

// parseMethodParams parses a method's `takes` clause. JASS supports
// `nothing` (no params) and a comma-separated list of `<type> <name>` pairs.
func parseMethodParams(raw string) []StructParam {
	raw = strings.TrimSpace(raw)
	if raw == "" || raw == "nothing" {
		return nil
	}
	var out []StructParam
	for _, part := range strings.Split(raw, ",") {
		p := strings.TrimSpace(part)
		if p == "" {
			continue
		}
		fields := strings.Fields(p)
		if len(fields) < 2 {
			continue
		}
		// Last field is the param name; everything before is the type
		// (JASS doesn't allow multi-word types, but `code` etc. are single
		// tokens; this stays simple).
		name := fields[len(fields)-1]
		typ := strings.Join(fields[:len(fields)-1], " ")
		out = append(out, StructParam{Type: typ, Name: name})
	}
	return out
}

// ---------------------------------------------------------------------------
// Pass 3 — outside-struct reference rewrite (dot vs colon disambiguation).
// ---------------------------------------------------------------------------

// rewriteStructRefs tokenizes the source and rewrites method calls based on
// receiver identity:
//
//   - `Foo.method(args)` where `Foo` is a known struct name → leave as
//     `Foo.method(args)` (Lua static call).
//   - `something.method(args)` where `something` is NOT a known struct name →
//     rewrite the `.` to `:` so Lua dispatches via __index (instance call).
//
// Field access (`something.field` not followed by `(`) is NOT rewritten —
// Lua's dot works for both static and instance fields.
//
// We use the tokenizer so strings + comments + rawcodes don't get mangled.
// The token stream is walked window-by-window: detect IDENT, DOT, IDENT,
// optional whitespace, LPAREN. For non-struct receivers, swap the DOT for a
// COLON in the output.
func rewriteStructRefs(src string, structs map[string]StructDef, knownNames map[string]bool) (string, error) {
	if len(structs) == 0 || src == "" {
		return src, nil
	}
	toks, err := Tokenize(src)
	if err != nil {
		return "", err
	}
	// Build a quick lookup of known struct names. Struct names always force
	// the dot-form (Lua static call).
	known := make(map[string]bool, len(structs))
	for name := range structs {
		known[name] = true
	}
	// Build the dot-forcing set from knownNames (library publics, top-level
	// globals, top-level functions). A receiver that matches any of these
	// stays dot — it's not an instance.
	dotForcing := make(map[string]bool, len(knownNames))
	for k, v := range knownNames {
		if v {
			dotForcing[k] = true
		}
	}
	// Find DOTs to swap. We work in a positions list keyed by (line, col)
	// since the tokenizer doesn't emit byte offsets. To avoid an O(n²) walk
	// while applying swaps, we collect the dot positions in source order,
	// then walk the source byte-by-byte applying swaps when we hit the next
	// queued position.
	type swap struct {
		line, col int
	}
	var swaps []swap
	for i := 0; i < len(toks); i++ {
		t := toks[i]
		if t.Kind != TokIdent {
			continue
		}
		// Look ahead for the pattern: IDENT . IDENT (whitespace/EOL between
		// dot and second ident is unusual in JASS but we tolerate it).
		j := i + 1
		if j >= len(toks) {
			break
		}
		if !(toks[j].Kind == TokOp && toks[j].Value == ".") {
			continue
		}
		k := j + 1
		for k < len(toks) && (toks[k].Kind == TokEOL || toks[k].Kind == TokComment) {
			k++
		}
		if k >= len(toks) || toks[k].Kind != TokIdent {
			continue
		}
		// Look ahead for `(` to confirm this is a method call (vs bare field
		// access). Allow whitespace tokens (the lexer skips horizontal ws so
		// there's no whitespace token to step over — just check EOL).
		l := k + 1
		for l < len(toks) && (toks[l].Kind == TokEOL || toks[l].Kind == TokComment) {
			l++
		}
		isMethodCall := l < len(toks) && toks[l].Kind == TokOp && toks[l].Value == "("
		if !isMethodCall {
			continue
		}
		// Receiver name (t.Value): if it's a known struct, leave the dot. If
		// it's `thistype` (rare outside method bodies) leave alone (it'd be
		// rewritten by the struct method body emit path anyway). Otherwise
		// consult dotForcing — library publics, top-level globals, top-level
		// functions all stay dot. Anything else falls through to colon.
		recv := t.Value
		if known[recv] {
			continue
		}
		if dotForcing[recv] {
			// Known top-level name (library function, global, etc.) — dot.
			continue
		}
		// Skip rewrites on common false-positives: keywords-like identifiers
		// such as a struct's static name appearing on the LHS (`Foo` filtered
		// above already), or built-in identifiers we know aren't structs.
		// We're permissive here; the receiver heuristic is "anything that
		// looks like an instance variable".
		swaps = append(swaps, swap{line: toks[j].Line, col: toks[j].Col})
		// Advance past the consumed window minus 1 (the outer loop's i++
		// steps to t+1, but the next ident could itself participate as
		// receiver, so we don't skip ahead).
	}
	if len(swaps) == 0 {
		return src, nil
	}
	// Apply swaps by walking the source byte-by-byte and tracking line/col.
	var out strings.Builder
	out.Grow(len(src))
	swapIdx := 0
	line, col := 1, 1
	for pos := 0; pos < len(src); pos++ {
		c := src[pos]
		if swapIdx < len(swaps) && swaps[swapIdx].line == line && swaps[swapIdx].col == col {
			if c == '.' {
				out.WriteByte(':')
				swapIdx++
				// Advance position counter.
				col++
				continue
			}
			// Mis-located swap (e.g. token-position drift). Skip it.
			swapIdx++
		}
		out.WriteByte(c)
		if c == '\n' {
			line++
			col = 1
		} else {
			col++
		}
	}
	return out.String(), nil
}

// ---------------------------------------------------------------------------
// Lua emission.
// ---------------------------------------------------------------------------

// emitStructLua writes the Lua snippet for one struct into b.
func emitStructLua(b *strings.Builder, s StructDef) {
	emitStructLuaInto(b, s, nil)
}

// emitStructLuaInto is the error-collecting form of emitStructLua. When errs is
// non-nil, the per-statement parse errors recovered while transpiling each
// method body are appended to it (via emitStructMethodInto). Passing nil keeps
// the original emit-only behavior. This is the seam that lets the struct-splice
// path surface struct-method failures to section.Errors (P0#1 completion).
func emitStructLuaInto(b *strings.Builder, s StructDef, errs *[]string) {
	// Header comment + class table init.
	if s.Extends != "" {
		fmt.Fprintf(b, "-- struct %s extends %s\n", s.Name, s.Extends)
		fmt.Fprintf(b, "%s = (%s and setmetatable({}, {__index = %s})) or {}\n", s.Name, s.Extends, s.Extends)
	} else {
		fmt.Fprintf(b, "-- struct %s\n", s.Name)
		fmt.Fprintf(b, "%s = %s or {}\n", s.Name, s.Name)
	}
	if len(s.Delegates) == 0 {
		fmt.Fprintf(b, "%s.__index = %s\n", s.Name, s.Name)
	} else {
		// Delegate fallthrough: __index becomes a function so a missing
		// member falls through to the delegate's table. Emit once per
		// struct; check each delegate in declaration order. The struct's
		// own table is checked FIRST (rawget) so user-defined members
		// always win.
		fmt.Fprintf(b, "%s.__index = function(t, k)\n", s.Name)
		fmt.Fprintf(b, "\tlocal v = rawget(%s, k)\n", s.Name)
		b.WriteString("\tif v ~= nil then return v end\n")
		for _, dn := range s.Delegates {
			fmt.Fprintf(b, "\tlocal d_%s = rawget(t, %q)\n", dn, dn)
			fmt.Fprintf(b, "\tif d_%s ~= nil then\n", dn)
			fmt.Fprintf(b, "\t\tlocal dv = d_%s[k]\n", dn)
			b.WriteString("\t\tif dv ~= nil then return dv end\n")
			b.WriteString("\tend\n")
		}
		b.WriteString("\treturn nil\n")
		b.WriteString("end\n")
	}

	// Static fields (class-level).
	for _, f := range s.Statics {
		init := defaultForStructField(f)
		fmt.Fprintf(b, "%s.%s = %s\n", s.Name, f.Name, init)
	}

	// allocate(): always synthesized unless the user defined one.
	if !s.HasAllocate {
		b.WriteString(fmt.Sprintf("function %s.allocate()\n", s.Name))
		b.WriteString("\treturn setmetatable({")
		first := true
		for _, f := range s.Fields {
			if !first {
				b.WriteString(", ")
			}
			first = false
			fmt.Fprintf(b, "%s = %s", f.Name, defaultForStructField(f))
		}
		fmt.Fprintf(b, "}, %s)\n", s.Name)
		b.WriteString("end\n")
	}

	// User-defined methods.
	for _, m := range s.Methods {
		emitStructMethodInto(b, s, m, errs)
		// Operator overload (Phase 5): after emitting the named fallback
		// method, wire up the corresponding Lua metamethod when possible.
		if m.Operator != "" {
			emitOperatorMetamethod(b, s, m)
		}
	}

	// Auto-synthesize `create` if user didn't define one. The synthesized
	// create takes no args and just calls allocate.
	if !s.HasCreate {
		fmt.Fprintf(b, "function %s.create()\n", s.Name)
		fmt.Fprintf(b, "\treturn %s.allocate()\n", s.Name)
		b.WriteString("end\n")
	}

	// Auto-synthesize `destroy` if user didn't define one. The synthesized
	// destroy calls onDestroy (if defined on the instance) then clears the
	// metatable to release references.
	if !s.HasDestroy {
		fmt.Fprintf(b, "function %s:destroy()\n", s.Name)
		b.WriteString("\tif self.onDestroy then self:onDestroy() end\n")
		b.WriteString("\tsetmetatable(self, nil)\n")
		b.WriteString("end\n")
	}
}

// emitOperatorMetamethod wires a struct's operator overload to the Lua
// metamethod it most closely maps to. Called AFTER emitStructMethod has
// already emitted the named fallback method (e.g. `Foo._op_lt` for `<`).
//
// Mapping:
//   `[]`  → __index function delegating to user's _op_index, with fallback
//           to the existing Foo table for non-overloaded lookups. Static
//           operator only emits a comment (Lua metamethods are per-instance).
//   `<`   → __lt
//   `<=`  → __le
//   `==`  → __eq
//   `+` → __add, `-` → __sub, `*` → __mul, `/` → __div, `%` → __mod
//
// Other operators emit a comment noting the metamethod wasn't hooked up.
// The named fallback method stays available via `Foo:_op_<name>(...)`.
func emitOperatorMetamethod(b *strings.Builder, s StructDef, m StructMethod) {
	switch m.Operator {
	case "[]":
		// `__index` was already set to either `Foo` (no delegates) or a
		// function (delegates). To preserve method lookup, we wrap whichever
		// previous form was used and chain the user's overload after.
		//
		// We re-emit __index as a function that:
		//   1. Tries the user's `_op_index` overload (its return value, if
		//      non-nil, wins).
		//   2. Falls back to the existing Foo.<k> static-table lookup.
		fmt.Fprintf(b, "do\n")
		fmt.Fprintf(b, "\tlocal _prev_index = %s.__index\n", s.Name)
		fmt.Fprintf(b, "\t%s.__index = function(t, k)\n", s.Name)
		fmt.Fprintf(b, "\t\tlocal v = %s._op_index(t, k)\n", s.Name)
		fmt.Fprintf(b, "\t\tif v ~= nil then return v end\n")
		fmt.Fprintf(b, "\t\tif type(_prev_index) == 'function' then return _prev_index(t, k) end\n")
		fmt.Fprintf(b, "\t\treturn _prev_index[k]\n")
		fmt.Fprintf(b, "\tend\n")
		fmt.Fprintf(b, "end\n")
		return
	case "[]=":
		// `__newindex` — Lua-side instance-write hook. Phase 5 v1: route every
		// write through the user's `_op_newindex` overload.
		fmt.Fprintf(b, "%s.__newindex = %s._op_newindex\n", s.Name, s.Name)
		return
	case "<":
		fmt.Fprintf(b, "%s.__lt = %s._op_lt\n", s.Name, s.Name)
		return
	case "<=":
		fmt.Fprintf(b, "%s.__le = %s._op_le\n", s.Name, s.Name)
		return
	case "==":
		fmt.Fprintf(b, "%s.__eq = %s._op_eq\n", s.Name, s.Name)
		return
	case "+":
		fmt.Fprintf(b, "%s.__add = %s._op_add\n", s.Name, s.Name)
		return
	case "-":
		fmt.Fprintf(b, "%s.__sub = %s._op_sub\n", s.Name, s.Name)
		return
	case "*":
		fmt.Fprintf(b, "%s.__mul = %s._op_mul\n", s.Name, s.Name)
		return
	case "/":
		fmt.Fprintf(b, "%s.__div = %s._op_div\n", s.Name, s.Name)
		return
	case "%":
		fmt.Fprintf(b, "%s.__mod = %s._op_mod\n", s.Name, s.Name)
		return
	}
	// Unknown operator — emit a comment so the user knows the named method
	// exists but the metamethod hookup wasn't generated.
	fmt.Fprintf(b, "-- jass2lua: operator %q on struct %q has no Lua metamethod; available as %s.%s\n", m.Operator, s.Name, s.Name, m.Name)
}

// emitStructMethod writes one method as a Lua function. Instance methods use
// colon syntax (`function Foo:bar(...)`) so `self` is the implicit first arg;
// static methods use dot syntax (`function Foo.bar(...)`).
//
// The body is the raw JASS method body. We transpile it through the same
// jass2lua pipeline (re-wrapping as a function so the parser is happy), then
// extract the body lines. `this` / `thistype` rewrites are applied to the
// raw body BEFORE transpiling so the body looks like plain JASS to the
// transpiler.
func emitStructMethod(b *strings.Builder, s StructDef, m StructMethod) {
	emitStructMethodInto(b, s, m, nil)
}

// emitStructMethodInto is the error-collecting form of emitStructMethod. When
// errs is non-nil, every per-statement parse error the method body produced
// (each 1:1 with an inline `error(...)` marker in the emitted Lua) is appended,
// prefixed with the struct + method name so the section's diagnostics list
// identifies WHICH method failed. Passing nil preserves the original
// emit-only behavior for callers that don't want diagnostics.
func emitStructMethodInto(b *strings.Builder, s StructDef, m StructMethod, errs *[]string) {
	body := rewriteMethodBody(s.Name, m)
	// Build a JASS function wrapper around the body so the transpiler treats
	// it as a function decl. Instance methods get `self` as the first param
	// (we'll strip it from the signature when emitting Lua); static methods
	// just use the params verbatim.
	var params []string
	if !m.Static {
		// For instance methods we prepend a `self` synthetic param so the
		// transpiler sees it in scope. We'll emit `function Foo:method(...)`
		// without the self in the signature.
		params = append(params, "self")
	}
	for _, p := range m.Params {
		params = append(params, p.Name)
	}
	// Emit signature.
	if m.Static {
		fmt.Fprintf(b, "function %s.%s(%s)\n", s.Name, m.Name, strings.Join(params, ", "))
	} else {
		var sigParams []string
		for _, p := range m.Params {
			sigParams = append(sigParams, p.Name)
		}
		fmt.Fprintf(b, "function %s:%s(%s)\n", s.Name, m.Name, strings.Join(sigParams, ", "))
	}
	// Transpile the body. We wrap it as a function so the JASS parser
	// accepts the statement list, then extract the body lines.
	lua, parseErrs := transpileMethodBodyWithErrors(body)
	if errs != nil {
		for _, pe := range parseErrs {
			pe = strings.TrimSpace(pe)
			if pe == "" {
				continue
			}
			*errs = append(*errs, fmt.Sprintf("struct %s method %s: %s", s.Name, m.Name, pe))
		}
	}
	// Indent each non-empty line by one tab.
	for _, line := range splitLinesKeepEnd(lua) {
		if strings.TrimSpace(line) == "" {
			b.WriteString(line)
			continue
		}
		b.WriteByte('\t')
		b.WriteString(line)
	}
	if !strings.HasSuffix(lua, "\n") {
		b.WriteByte('\n')
	}
	b.WriteString("end\n")
}

// rewriteMethodBody applies struct-body-specific token rewrites to the raw
// JASS method body before it's handed to the transpiler:
//
//   - `thistype` → struct name (the containing struct)
//   - `this`     → `self` (so the transpiled Lua matches the colon-syntax
//     implicit `self` arg)
//   - implicit-`this` member access: a leading `.field` (receiver omitted) is
//     vJASS sugar for `this.field`. We expand a bare `.` (one whose previous
//     significant token can't be a receiver) to `self.`, so `set .x = .y` →
//     `set self.x = self.y`, `call .destroy()` → `call self.destroy()`,
//     `GetUnitX(.s)` → `GetUnitX(self.s)`. The downstream parser already has
//     member-access grammar (MemberExpr / MemberCallExpr) and emits the dot
//     verbatim, so this is the only missing piece for receiver-omitted access.
//     Chained access `.i.rs` becomes `self.i.rs` — only the FIRST dot is
//     receiver-omitted; the second is preceded by ident `i` (a receiver tail)
//     and is left alone.
//
// We use the tokenizer so strings/comments aren't touched. On lex error we
// fall back to the original body (the downstream transpiler will surface
// its own error).
func rewriteMethodBody(structName string, m StructMethod) string {
	if m.Body == "" {
		return ""
	}
	toks, err := Tokenize(m.Body)
	if err != nil {
		return m.Body
	}
	var out strings.Builder
	out.Grow(len(m.Body))
	pos := 0
	line, col := 1, 1
	// Track the previous significant token (EOL / comment skipped) so we can
	// decide whether a `.` is implicit-this. havePrev=false at body start, which
	// is itself a non-receiver position.
	var prevKind TokenKind
	var prevVal string
	havePrev := false
	for _, t := range toks {
		if t.Kind == TokEOF {
			if pos < len(m.Body) {
				out.WriteString(m.Body[pos:])
			}
			break
		}
		// Advance to token start.
		for pos < len(m.Body) && (line < t.Line || (line == t.Line && col < t.Col)) {
			c := m.Body[pos]
			out.WriteByte(c)
			pos++
			if c == '\n' {
				line++
				col = 1
			} else {
				col++
			}
		}
		srcLen := tokenSourceLen(m.Body, pos, t)
		if srcLen <= 0 {
			srcLen = len(t.Value)
			if srcLen == 0 {
				srcLen = 1
			}
		}
		// Implicit-this expansion: a `.` op whose previous significant token is
		// NOT a receiver tail is `this.field` with `this` elided. Inject `self`
		// before writing the dot.
		if t.Kind == TokOp && t.Value == "." && !isReceiverTail(havePrev, prevKind, prevVal) {
			out.WriteString("self")
		}
		// Apply rewrites at TokIdent positions.
		if t.Kind == TokIdent {
			switch t.Value {
			case "thistype":
				out.WriteString(structName)
			case "this":
				out.WriteString("self")
			default:
				out.WriteString(m.Body[pos : pos+srcLen])
			}
		} else {
			out.WriteString(m.Body[pos : pos+srcLen])
		}
		// Update prev for the NEXT iteration (skip EOL / comment tokens so a
		// statement that wraps across lines doesn't lose receiver context — JASS
		// statements don't wrap, but be defensive).
		if t.Kind != TokEOL && t.Kind != TokComment {
			prevKind = t.Kind
			prevVal = t.Value
			havePrev = true
		}
		for k := 0; k < srcLen && pos < len(m.Body); k++ {
			c := m.Body[pos]
			pos++
			if c == '\n' {
				line++
				col = 1
			} else {
				col++
			}
		}
	}
	if pos < len(m.Body) {
		out.WriteString(m.Body[pos:])
	}
	return out.String()
}

// isReceiverTail reports whether the previous significant token can be the tail
// of a member-access receiver, i.e. a `.` immediately after it is a normal
// `recv.field` (NOT receiver-omitted implicit-this). Receiver tails are:
//
//   - identifiers (`foo.field`, and chained `a.b.c` — the `b` ident before the
//     second dot is a tail);
//   - the closing brackets `)` and `]` (`f(x).field`, `arr[i].field`);
//   - literals (defensive — JASS has no numeric/string method syntax, so
//     `1.f` / `"x".f` don't appear, but treating them as tails can only ever
//     SUPPRESS a self-injection, never add a wrong one).
//
// Everything else — `set` / `call` / `if` / `then` / `return` keywords, `(`,
// `,`, `=`, arithmetic/comparison operators, the `.` itself, and body start
// (havePrev=false) — means a following `.` has no receiver and is implicit-this.
func isReceiverTail(havePrev bool, kind TokenKind, val string) bool {
	if !havePrev {
		return false
	}
	switch kind {
	case TokIdent, TokInt, TokReal, TokString, TokRaw:
		return true
	case TokOp:
		return val == ")" || val == "]"
	}
	return false
}

// transpileMethodBody runs the raw (already-rewritten) JASS body through the
// transpiler. We wrap the body as a function decl so the parser is happy,
// then strip the wrapping `function Wrapped()` and `end` lines off the
// result.
func transpileMethodBody(body string) string {
	lua, _ := transpileMethodBodyWithErrors(body)
	return lua
}

// transpileMethodBodyWithErrors is the error-surfacing variant of
// transpileMethodBody. It returns the stripped Lua body AND the per-statement
// parse errors the transpiler recovered (each maps 1:1 to an inline
// `error("jass2lua failed: ...")` marker the emitter wrote inside the method
// body). The struct-splice path threads these out so section.Errors finally
// counts struct-method failures instead of swallowing them (P0#1 completion).
func transpileMethodBodyWithErrors(body string) (string, []string) {
	wrapped := "function __vjass_struct_method takes nothing returns nothing\n" + body + "\nendfunction\n"
	lua, parseErrs, _ := TranspileScriptWithErrors(wrapped)
	// Strip the `function __vjass_struct_method()` opener and matching
	// `end` closer + the trailing blank line.
	lines := strings.Split(lua, "\n")
	var inner []string
	depth := 0
	started := false
	for _, ln := range lines {
		trim := strings.TrimSpace(ln)
		if !started {
			if strings.HasPrefix(trim, "function __vjass_struct_method") {
				started = true
				depth = 1
			}
			continue
		}
		if depth == 1 && trim == "end" {
			depth = 0
			break
		}
		inner = append(inner, ln)
	}
	// Trim leading tab indent the emitter added.
	for i, ln := range inner {
		inner[i] = strings.TrimPrefix(ln, "\t")
	}
	return strings.Join(inner, "\n") + "\n", parseErrs
}

// defaultForStructField returns the Lua literal to initialize a field. If the
// user wrote `integer x = 5` we use `5`; otherwise we fall back to the JASS
// type's default (0 / 0.0 / "" / false / nil).
func defaultForStructField(f StructField) string {
	if f.Array {
		return "{}"
	}
	if f.Default != "" {
		// Best-effort transpile of the initializer expression. Wrap as
		// `local x = <expr>` to get an expression context, then strip.
		wrapped := "function __vjass_init takes nothing returns nothing\nlocal " + f.Type + " x = " + f.Default + "\nendfunction\n"
		lua, err := TranspileScript(wrapped)
		if err == nil {
			// Pull out the `local x = <expr>` line.
			for _, line := range strings.Split(lua, "\n") {
				trim := strings.TrimSpace(line)
				if strings.HasPrefix(trim, "local x = ") {
					return strings.TrimPrefix(trim, "local x = ")
				}
			}
		}
		// Fallback: emit the raw default. Wrong for some types but visible.
		return f.Default
	}
	return defaultInitForType(f.Type)
}

// ---------------------------------------------------------------------------
// Sorted name iteration for deterministic output (used by tests).
// ---------------------------------------------------------------------------

// CollectTopLevelNames builds the dot-forcing set the struct preprocessor
// uses for receiver disambiguation. The set is:
//
//   - every top-level function name declared in `src` (parsed via the JASS
//     parser, so it only includes well-formed decls);
//   - every top-level global declared in a `globals` block; and
//   - every entry in `libraryPublics` (already mangled at libscope time).
//
// Anything in this set takes the dot-form in `recv.method(args)` rewrites,
// even if it's NOT a known struct name. This catches enum-like globals and
// library function calls that Phase 3 would have mis-rewritten to colon.
//
// libraryPublics may be nil. structs may be nil; the result still contains
// the source-derived top-level names.
func CollectTopLevelNames(src string, libraryPublics []string, structs map[string]StructDef) map[string]bool {
	out := map[string]bool{}
	for _, name := range libraryPublics {
		out[name] = true
	}
	for name := range structs {
		out[name] = true
	}
	// Quick line-based scan for top-level globals and functions. We don't
	// run the full parser here (it'd choke on the unstripped vJASS surface);
	// instead we do a cheap heuristic that matches the same patterns the
	// libscope pre-pass uses.
	lines := splitLinesKeepEnd(src)
	inGlobals := false
	inFunction := false
	for _, line := range lines {
		bare := strings.TrimSpace(strings.TrimRight(line, "\r\n"))
		switch {
		case bare == "globals":
			inGlobals = true
			continue
		case bare == "endglobals":
			inGlobals = false
			continue
		case strings.HasPrefix(bare, "function ") || strings.HasPrefix(bare, "constant function "):
			if !inGlobals && !inFunction {
				if m := funcDeclRe.FindStringSubmatch(bare); m != nil {
					out[m[1]] = true
				}
			}
			inFunction = true
			continue
		case strings.HasPrefix(bare, "native ") || strings.HasPrefix(bare, "constant native "):
			if !inGlobals {
				// Same shape as function for our purposes — pull the ident.
				if m := funcDeclRe.FindStringSubmatch(bare); m != nil {
					out[m[1]] = true
				} else {
					// Fall back: match `native FOO takes …`.
					fields := strings.Fields(bare)
					if len(fields) >= 2 {
						out[fields[1]] = true
					}
				}
			}
			continue
		case bare == "endfunction":
			inFunction = false
			continue
		}
		if inGlobals {
			if m := globalDeclRe.FindStringSubmatch(strings.TrimRight(line, "\r\n")); m != nil {
				out[m[3]] = true
			}
		}
	}
	return out
}

// StructNamesSorted returns the names in res.Structs sorted alphabetically.
func StructNamesSorted(res StructResult) []string {
	out := make([]string, 0, len(res.Structs))
	for name := range res.Structs {
		out = append(out, name)
	}
	sort.Strings(out)
	return out
}
