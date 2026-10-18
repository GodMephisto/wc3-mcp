---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/context.go"
ref: HEAD
---

package jass2lua

// context.go — cross-section (cross-trigger) vJASS preprocess context (P2#6).
//
// The per-trigger Convert-to-Lua path preprocesses each WTG section in
// ISOLATION: every section runs its own StripBlockComments → Preprocess →
// PreprocessLibScope → PreprocessModules → PreprocessStructs pipeline seeing
// ONLY its own text. That breaks vJASS library systems, where a struct /
// module / textmacro / library defined in section A is referenced by section
// B — section B can't see A's definitions, so:
//
//   - `implement Foo` in B fails (module Foo lives in A) → empty paste.
//   - `Bar.method()` in B (Bar is a struct from A) is mis-rewritten dot→colon
//     because B's struct preprocessor doesn't know Bar is a struct.
//   - `//! runtextmacro M(...)` in B fails (textmacro M defined in A).
//
// The FLAT war3map.j path is clean precisely because it sees ALL definitions
// at once. GlobalContext mirrors that visibility for the per-section path:
// build it ONCE from the concatenation of every section's source, then thread
// it into each section's transpile so the section sees the full set of
// definitions WITHOUT changing which section emits which struct/module Lua
// (each section still emits only its own — GlobalContext supplies visibility,
// not output).
//
// Determinism: the context is built from the sections in the exact order the
// caller passes them. Same-named definitions resolve last-wins (mirrors the
// concatenation the flat path would see). The struct dot/colon receiver set
// and the textmacro table are the two surfaces that materially change a
// section's transpiled output; both are seeded from the global view here.

import "strings"

// GlobalContext is the shared cross-section preprocess context. Build it with
// BuildGlobalContext from every section's raw source, then pass it to the
// per-section transpile helpers (TranspileSectionWithContext-style call sites
// in triggers_convert.go).
//
// All fields are safe to read concurrently AFTER construction (the maps are
// never mutated post-build). The zero value (all-nil) is valid and behaves
// exactly like the old isolated path — every accessor tolerates nil.
type GlobalContext struct {
	// Macros is the union of every section's textmacro definitions. Seeded
	// into each section's PreprocessWithMacros so a cross-section
	// `//! runtextmacro` resolves. A section's own same-named definition wins.
	Macros map[string]Macro

	// Modules is the union of every section's `module … endmodule` defs.
	// Passed to PreprocessStructs so a struct's `implement Foo` inlines the
	// module body even when Foo was declared in a different section.
	Modules map[string]ModuleDef

	// KnownNames is the global dot-forcing set: every struct name, library
	// public, and top-level function/global declared across ALL sections.
	// Merged into each section's per-section knownNames so the struct
	// ref-rewriter (dot vs colon) treats a cross-section struct/library name
	// as dot (static) instead of mis-rewriting it to colon (instance).
	KnownNames map[string]bool
}

// BuildGlobalContext constructs the cross-section context from the raw source
// of every section (custom-script triggers, GUI custom-text overlays, the
// war3map.wct GlobalJASS header, a hand-rolled war3map.j — whatever the caller
// feeds). Order matters only for last-wins resolution of duplicate names; the
// resulting visibility set is order-independent.
//
// The build runs the same front of the per-section pipeline (block-comment
// strip → textmacro expand → libscope resolve → module extract) so the
// collected struct/module/known-name sets match what each section would see if
// every definition were concatenated into one flat script — which is exactly
// the flat war3map.j path's view.
func BuildGlobalContext(sections []string) *GlobalContext {
	ctx := &GlobalContext{
		Macros:     map[string]Macro{},
		Modules:    map[string]ModuleDef{},
		KnownNames: map[string]bool{},
	}
	if len(sections) == 0 {
		return ctx
	}

	// Pass A — gather textmacro definitions from every section FIRST, so the
	// per-section expansion in Pass B can resolve cross-section macro calls.
	// We only need the macro table here, not the expansion, so each section is
	// run through Preprocess in isolation purely to harvest its Macros map.
	for _, sec := range sections {
		if strings.TrimSpace(sec) == "" {
			continue
		}
		stripped := StripBlockComments(sec)
		pp := Preprocess(stripped)
		for name, mac := range pp.Macros {
			ctx.Macros[name] = mac
		}
	}

	// Pass B — for every section, expand (with the global macro table seeded),
	// resolve library/scope, extract modules, and harvest:
	//   * module defs        → ctx.Modules
	//   * library publics    → ctx.KnownNames
	//   * top-level names     → ctx.KnownNames (via CollectTopLevelNames)
	//   * struct names        → ctx.KnownNames (so cross-section structs are dot)
	//
	// We intentionally do NOT retain each section's struct DEFINITIONS for
	// emission — only the NAMES, for the dot/colon receiver heuristic. Each
	// section still parses + emits its own struct bodies at transpile time, so
	// no struct Lua is duplicated and the per-section output mapping is intact.
	for _, sec := range sections {
		if strings.TrimSpace(sec) == "" {
			continue
		}
		stripped := StripBlockComments(sec)
		pp := PreprocessWithMacros(stripped, ctx.Macros)
		ls := PreprocessLibScope(pp.Expanded)
		mods := PreprocessModules(ls.Expanded)
		for name, def := range mods.Modules {
			ctx.Modules[name] = def
		}
		withoutIfaces := PreprocessInterfaces(mods.Expanded)
		withoutDefines := PreprocessDefines(withoutIfaces)
		// Top-level names + library publics for this section.
		names := CollectTopLevelNames(withoutDefines, ls.PublicNames, nil)
		for n := range names {
			ctx.KnownNames[n] = true
		}
		// Struct names declared in this section. We run the struct preprocessor
		// with the modules accumulated so far (good enough for name discovery;
		// `implement` inlining only affects member parsing, not the struct's
		// own name). The names feed the cross-section dot-forcing set.
		st := PreprocessStructs(withoutDefines, ctx.Modules, names)
		for name := range st.Structs {
			ctx.KnownNames[name] = true
		}
	}

	return ctx
}

// MacrosFor returns the global macro table (nil-safe). The seed for a
// section's PreprocessWithMacros.
func (c *GlobalContext) MacrosFor() map[string]Macro {
	if c == nil {
		return nil
	}
	return c.Macros
}

// ModulesFor merges the section-local module table with the global one and
// returns the union (nil-safe). Local defs win on name collision. When the
// global context is nil, the section-local map is returned unchanged so the
// isolated path behaves exactly as before.
//
// This IS the production convert path: a struct's `implement Foo` inlines Foo's
// body even when Foo lives in another section (cross-trigger vJASS). Enabling
// it naively used to regress the Enfo map 514 → 1,366 (+852) because the only
// resolvable cross-section module there (MisssileStruct) is a 1,080-line
// interface/documentation module — bodyless `static method` signatures and
// valueless `constant` decls describing what an implementer SHOULD write.
// Pasted verbatim, the bodyless signatures made the struct parser over-collect
// every following declaration as a method "body", which the statement parser
// then rejected token-by-token. Two fixes made global inlining safe (Enfo now
// 513, BELOW the 514 baseline):
//
//   1. inlineImplements sanitizes the pasted body (sanitizeModuleBodyForPaste
//      in structs.go) — dropping bodyless signatures, top-level globals/
//      function/interface/nested-struct blocks, and valueless constants — so a
//      documentation module degrades to comments and a well-formed module
//      inlines cleanly.
//   2. parseSet accepts a call-expression assignment target
//      (`set ThisType(i).field = v`), which `extends array` allocator methods
//      emit; this also fixed 3 pre-existing baseline errors.
func (c *GlobalContext) ModulesFor(local map[string]ModuleDef) map[string]ModuleDef {
	if c == nil || len(c.Modules) == 0 {
		return local
	}
	out := make(map[string]ModuleDef, len(c.Modules)+len(local))
	for k, v := range c.Modules {
		out[k] = v
	}
	for k, v := range local {
		out[k] = v // local wins
	}
	return out
}

// ModulesForSection returns the SECTION-LOCAL module table unchanged, i.e.
// cross-section `implement` inlining is NOT performed. The production convert
// path now uses ModulesFor (global) — see its doc for why that became safe.
// This local-only variant is retained for callers that explicitly want module
// isolation and for the tests that contrast the two behaviors.
func (c *GlobalContext) ModulesForSection(local map[string]ModuleDef) map[string]ModuleDef {
	return local
}

// KnownNamesFor merges the section-local known-name set with the global
// dot-forcing set and returns the union (nil-safe). When the global context is
// nil, the section-local set is returned unchanged.
func (c *GlobalContext) KnownNamesFor(local map[string]bool) map[string]bool {
	if c == nil || len(c.KnownNames) == 0 {
		return local
	}
	out := make(map[string]bool, len(c.KnownNames)+len(local))
	for k, v := range c.KnownNames {
		if v {
			out[k] = true
		}
	}
	for k, v := range local {
		if v {
			out[k] = true
		}
	}
	return out
}
