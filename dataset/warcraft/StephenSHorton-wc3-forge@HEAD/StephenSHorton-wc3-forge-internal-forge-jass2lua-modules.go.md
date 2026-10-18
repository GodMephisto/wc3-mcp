---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/modules.go"
ref: HEAD
---

package jass2lua

// modules.go — vJASS module preprocessor (Phase 4 of vJASS support).
//
// JassHelper modules are reusable mixin fragments: a `module Foo / endmodule`
// block declares fields + methods that get pasted into a struct body via
// `implement Foo` (or `implement optional Foo`). Each implementing struct
// receives its OWN copy of the module's contents — there's no shared object
// table at runtime.
//
// Source shape:
//
//	module HasLog
//	    method log takes string s returns nothing
//	        call BJDebugMsg("[" + this.name + "] " + s)
//	    endmethod
//	    integer logCount = 0
//	endmodule
//
//	struct Greeter
//	    string name = "Hi"
//	    implement HasLog       // pastes log method + logCount field into Greeter
//	endstruct
//
// Pipeline placement:
//
//	raw JASS
//	    → StripBlockComments   (Phase 4 — must come first)
//	    → Preprocess           (textmacro + //! if/else/endif — Phases 1+4)
//	    → PreprocessLibScope   (library/scope — Phase 2)
//	    → PreprocessModules    (this file — Phase 4 — BEFORE structs)
//	    → PreprocessInterfaces (Phase 4)
//	    → PreprocessDefines    (Phase 4)
//	    → PreprocessStructs    (Phase 3 — uses ModulesResult.Modules)
//	    → blocker scan + transpile
//
// Strategy: PreprocessModules parses `module … endmodule` blocks, stores the
// parsed fields + methods on the ModulesResult, and STRIPS them from the
// source. PreprocessStructs is then given the map and, when it sees an
// `implement Foo` line inside a struct body, inlines a copy of the module's
// content into that struct's body BEFORE the rest of struct parsing runs.

import (
	"fmt"
	"regexp"
	"strings"
)

// ModulesResult is the output of PreprocessModules.
type ModulesResult struct {
	// Expanded is the source with `module … endmodule` blocks stripped.
	// `implement` lines inside structs are NOT touched here — the struct
	// preprocessor handles them using the Modules map.
	Expanded string

	// Modules maps module name → ModuleDef. Empty if no modules.
	Modules map[string]ModuleDef

	// ModuleOrder preserves source-order for deterministic emission /
	// diagnostics.
	ModuleOrder []string

	// Errors is the non-fatal diagnostics bucket.
	Errors []ModuleError
}

// ModuleDef captures one parsed module.
type ModuleDef struct {
	Name    string
	Fields  []StructField // instance fields declared at module scope
	Statics []StructField // static fields declared at module scope
	Methods []StructMethod
	Line    int // 1-based opener line for diagnostics
	// Raw is the verbatim body text between module/endmodule. Kept so the
	// struct preprocessor can paste it directly when it inlines an implement.
	Raw string
}

// ModuleError is a non-fatal diagnostic surfaced to the UI.
type ModuleError struct {
	Line    int
	Message string
}

// Regex matchers for module openers/closers.
var (
	moduleOpenerRe = regexp.MustCompile(`^\s*module\s+([A-Za-z_][A-Za-z0-9_]*)\s*$`)
	moduleCloserRe = regexp.MustCompile(`^\s*endmodule\s*$`)
)

// PreprocessModules extracts every `module … endmodule` block from src,
// returning the stripped source + the parsed defs. Idempotent: a source with
// no modules round-trips with empty results.
//
// Conservative on errors (matches Phase 1/2/3 style): missing endmodule,
// duplicate module names, malformed openers — each emits a ModuleError +
// the block is dropped from the source, but processing continues.
func PreprocessModules(src string) ModulesResult {
	res := ModulesResult{
		Modules: map[string]ModuleDef{},
	}
	if src == "" {
		return res
	}
	var out strings.Builder
	out.Grow(len(src))
	lines := splitLinesKeepEnd(src)
	i := 0
	for i < len(lines) {
		line := lines[i]
		trim := strings.TrimRight(line, "\r\n")
		m := moduleOpenerRe.FindStringSubmatch(trim)
		if m == nil {
			out.WriteString(line)
			i++
			continue
		}
		name := m[1]
		defLine := i + 1
		// Collect body until endmodule.
		var bodyLines []string
		i++
		closed := false
		for i < len(lines) {
			inner := lines[i]
			innerTrim := strings.TrimRight(inner, "\r\n")
			if moduleCloserRe.MatchString(innerTrim) {
				closed = true
				i++
				break
			}
			bodyLines = append(bodyLines, inner)
			i++
		}
		if !closed {
			res.Errors = append(res.Errors, ModuleError{
				Line:    defLine,
				Message: fmt.Sprintf("module %q: missing `endmodule` before EOF", name),
			})
		}
		if _, dup := res.Modules[name]; dup {
			res.Errors = append(res.Errors, ModuleError{
				Line:    defLine,
				Message: fmt.Sprintf("duplicate module %q; second definition ignored", name),
			})
			// Preserve a marker so the diff stays readable.
			out.WriteString(fmt.Sprintf("// jass2lua: dropped duplicate module %s\n", name))
			continue
		}
		def := parseModuleBody(name, defLine, bodyLines, &res)
		res.Modules[name] = def
		res.ModuleOrder = append(res.ModuleOrder, name)
		// Strip the block; emit a single marker comment so the diff stays
		// grep-friendly.
		out.WriteString(fmt.Sprintf("// jass2lua: module %s stripped (pastes into structs via `implement %s`)\n", name, name))
	}
	res.Expanded = out.String()
	return res
}

// parseModuleBody parses the body of a module just like a struct body — we
// reuse the StructField + StructMethod types because module contents have
// identical syntax + semantics (they ARE going to be pasted into a struct).
//
// Implementation note: we delegate to the same field/method matchers the
// struct preprocessor uses. Anything the struct parser would treat as a
// field/method/static, the module parser also accepts.
func parseModuleBody(name string, startLine int, lines []string, res *ModulesResult) ModuleDef {
	def := ModuleDef{
		Name: name,
		Line: startLine,
		Raw:  strings.Join(lines, ""),
	}
	// Lean on parseStructBody by routing through a synthetic StructDef. Any
	// errors recorded on the StructResult get folded into module diagnostics
	// (re-labelled so the user sees `module Foo` instead of `struct Foo`).
	synthetic := &StructDef{Name: name, StartLine: startLine}
	stRes := &StructResult{}
	parseStructBody(synthetic, lines, stRes)
	def.Fields = synthetic.Fields
	def.Statics = synthetic.Statics
	def.Methods = synthetic.Methods
	for _, e := range stRes.Errors {
		res.Errors = append(res.Errors, ModuleError{
			Line:    e.Line,
			Message: strings.Replace(e.Message, "struct ", "module ", 1),
		})
	}
	return def
}
