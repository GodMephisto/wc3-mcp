---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_convert.go"
ref: HEAD
---

// Package forge — Convert Map to Lua. Modernizes old JASS maps (FileVersion
// 25 / TFT-era) so they're editable on the current Reforged client and
// wc3-forge's Lua-only codegen.
//
// Two-step protocol:
//
//   1. CheckConvertToLua scans the loaded map's triggers for vJASS-specific
//      content that can't be auto-translated (the JassHelper preprocessor is
//      out of scope). Pure JASS — function/if/loop/set/call/return/globals —
//      is NOT a blocker; the jass2lua transpiler handles it.
//
//   2. ConvertToLua transpiles every pure-JASS section, concatenates with the
//      GUI codegen output, writes war3map.lua, deletes war3map.j, sets
//      info.Lua=true. Optionally backs up the map source first (folder copy
//      or .w3x file copy). Marks info + script dirty so Save persists.
//      Records ONE undo Command that restores the original war3map.j bytes +
//      info.Lua=false on Revert + drops the generated war3map.lua. The backup
//      file is NOT auto-deleted on Undo — it stays as the user's belt-and-
//      suspenders fallback.
//
// JASS-vs-vJASS triage is driven by jass2lua.FindVJASSKeyword (the keyword
// regex is centralized there alongside the transpiler so both stay in sync).
package forge

import (
	"errors"
	"fmt"
	"io"
	"io/fs"
	"os"
	"path/filepath"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/forge/jass2lua"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// ConvertBlocker describes one trigger / synthetic node that blocks
// auto-conversion. Kind discriminates the source:
//
//	"script_vjass"      — is_script=true trigger whose body uses vJASS keywords
//	"custom_text_vjass" — GUI trigger whose CustomText overlay uses vJASS
//	"map_header_vjass"  — hand-rolled war3map.j containing vJASS
//	"global_jass_vjass" — war3map.wct GlobalJASS block containing vJASS
//
// Reason is a short human-readable explanation the UI surfaces verbatim.
type ConvertBlocker struct {
	TriggerID   int32  `json:"trigger_id"`
	TriggerName string `json:"trigger_name"`
	Kind        string `json:"kind"`
	Reason      string `json:"reason"`
}

// ConvertToLuaCheckResult is the wire shape both CheckConvertToLua and
// ConvertToLua return. Empty Blockers means the map is safe to convert
// (or, for ConvertToLua, that the conversion succeeded).
type ConvertToLuaCheckResult struct {
	Blockers []ConvertBlocker `json:"blockers"`
}

// ErrConvertBlocked is the sentinel error ConvertToLua returns when blockers
// are present. The Wails wrapper translates this to a non-error response
// carrying the blocker list (the UI prefers a typed payload over an opaque
// error string for the blocker-list dialog).
var ErrConvertBlocked = errors.New("map has unconvertable vJASS content")

// ErrAlreadyLua is returned by ConvertToLua when the loaded map is already
// flagged as Lua (info.Lua == true). The UI disables the menu item in this
// case, so this is mostly belt-and-suspenders for direct MCP callers.
var ErrAlreadyLua = errors.New("map is already Lua (info.Lua == true) — nothing to convert")

// ErrBackupFailed is returned when the backup step fails. We refuse to proceed
// to a conversion the user can't easily undo.
var ErrBackupFailed = errors.New("backup failed; refusing to convert without a backup the user asked for")

// CheckConvertToLua scans the loaded map for vJASS-specific script content
// (the JassHelper preprocessor is out of scope; pure JASS is transpiled
// automatically). Returns an empty Blockers slice when the map is safe to
// convert.
//
// Blockers (we refuse on these):
//   - is_script triggers using vJASS keywords (library, struct, scope, etc.)
//   - GUI triggers whose CustomText overlay uses vJASS keywords
//   - Hand-rolled war3map.j containing vJASS
//   - war3map.wct GlobalJASS header containing vJASS
//
// NOT blockers (handled by the transpiler):
//   - is_script triggers with pure JASS (function/if/loop/set/call/etc.)
//   - GUI triggers with pure-JASS CustomText overlays
//   - Pure-JASS hand-rolled war3map.j
//   - Pure-JASS GlobalJASS
//   - Pure-GUI triggers with no script content
//   - Comment triggers
func (s *Session) CheckConvertToLua() (*ConvertToLuaCheckResult, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	if !s.loaded {
		return nil, fmt.Errorf("no map loaded")
	}
	res := &ConvertToLuaCheckResult{Blockers: []ConvertBlocker{}}

	// Hand-rolled war3map.j case: scan the synthesized Map Header trigger's
	// custom_text for vJASS. Pure-JASS hand-rolled maps are now convertible
	// (the transpiler handles them). The textmacro preprocessor runs FIRST
	// so a textmacro-only header expands to pure JASS and stops blocking.
	if s.triggerIsHandRolled {
		if s.triggers != nil && len(s.triggers.Triggers) > 0 {
			tr := &s.triggers.Triggers[0]
			if kw, found := scanVJASSAfterPreprocess(tr.CustomText); found {
				name := s.mapHeaderScriptName
				if name == "" {
					name = "war3map.j"
				}
				res.Blockers = append(res.Blockers, ConvertBlocker{
					TriggerID:   tr.ID,
					TriggerName: "Map Header (" + name + ")",
					Kind:        "map_header_vjass",
					Reason:      "war3map.j uses the vJASS keyword `" + kw + "`. vJASS requires the JassHelper preprocessor; auto-transpile to Lua isn't supported. Rewrite the script as pure JASS or Lua first.",
				})
			}
		}
		return res, nil
	}

	if s.triggers == nil {
		// No triggers and no hand-rolled script → trivially safe.
		return res, nil
	}

	// Per-trigger vJASS scan. We preprocess each section before scanning so
	// textmacro-only triggers no longer block the convert flow.
	for i := range s.triggers.Triggers {
		tr := &s.triggers.Triggers[i]
		if tr.IsComment {
			continue
		}
		if (tr.IsScript || tr.Classifier == wtg.ClassifierScript) && tr.CustomText != "" {
			if kw, found := scanVJASSAfterPreprocess(tr.CustomText); found {
				res.Blockers = append(res.Blockers, ConvertBlocker{
					TriggerID:   tr.ID,
					TriggerName: tr.Name,
					Kind:        "script_vjass",
					Reason:      "Custom-script trigger uses the vJASS keyword `" + kw + "`. vJASS needs the JassHelper preprocessor; rewrite as pure JASS or Lua, or delete this trigger before retrying.",
				})
			}
			continue
		}
		if tr.CustomText != "" {
			if kw, found := scanVJASSAfterPreprocess(tr.CustomText); found {
				res.Blockers = append(res.Blockers, ConvertBlocker{
					TriggerID:   tr.ID,
					TriggerName: tr.Name,
					Kind:        "custom_text_vjass",
					Reason:      "GUI trigger has a custom-script overlay using the vJASS keyword `" + kw + "`. Clear the overlay or rewrite it as pure JASS before retrying.",
				})
			}
		}
	}

	// GlobalJASS in war3map.wct — also subject to the vJASS gate.
	if s.triggersWct != nil && strings.TrimSpace(s.triggersWct.GlobalJASS) != "" {
		if kw, found := scanVJASSAfterPreprocess(s.triggersWct.GlobalJASS); found {
			res.Blockers = append(res.Blockers, ConvertBlocker{
				TriggerID:   -1,
				TriggerName: "war3map.wct GlobalJASS",
				Kind:        "global_jass_vjass",
				Reason:      "war3map.wct's global JASS header uses the vJASS keyword `" + kw + "`. Rewrite as pure JASS first.",
			})
		}
	}

	return res, nil
}

// scanVJASSAfterPreprocess runs every vJASS preprocessor pass in order
// (block-comment strip → textmacro+conditional expansion → library/scope
// wrapping → module extraction → interface stripping → define rewrites →
// struct expansion) and then scans the final expanded source for vJASS
// keywords the transpiler still can't handle.
//
// After Phase 4 the keyword list is empty (every known vJASS construct has
// a preprocessor pass), so this gate effectively returns false. The function
// is kept as the single source of truth for the pre-transpile pipeline:
// future safety-net keywords go in jass2lua.vJASSKeywords and this scan
// flips back on.
//
// We deliberately ignore preprocess errors here: an unknown `//!` directive,
// an unknown runtextmacro call, an unresolved `requires`, a struct parse
// error — none should itself cause the blocker check to fail (the user sees
// those as warnings in the diff UI).
func scanVJASSAfterPreprocess(src string) (string, bool) {
	if src == "" {
		return "", false
	}
	src = jass2lua.StripBlockComments(src)
	pp := jass2lua.Preprocess(src)
	ls := jass2lua.PreprocessLibScope(pp.Expanded)
	mods := jass2lua.PreprocessModules(ls.Expanded)
	withoutIfaces := jass2lua.PreprocessInterfaces(mods.Expanded)
	withoutDefines := jass2lua.PreprocessDefines(withoutIfaces)
	known := jass2lua.CollectTopLevelNames(withoutDefines, ls.PublicNames, nil)
	st := jass2lua.PreprocessStructs(withoutDefines, mods.Modules, known)
	return jass2lua.FindVJASSKeyword(st.Expanded)
}

// TranspilePreview is the read-only DTO returned by TranspilePreview() — every
// pure-JASS section the convert flow would translate, with both sides of the
// diff prepared. ConvertToLua's actual write path re-runs this internally to
// stay deterministic.
type TranspilePreview struct {
	Sections []TranspileSection `json:"sections"`
}

// TranspileSection is one piece of the preview. ID:
//
//	>=0 — trigger id (per-trigger script trigger or GUI custom_text overlay)
//	-1  — war3map.wct GlobalJASS header
//	-2  — hand-rolled war3map.j (entire file)
//
// Errors is non-nil only when the transpiler flagged trouble; UI uses it to
// warn the user before they hit Convert.
//
// PreprocessWarnings carries non-fatal diagnostics from the textmacro
// preprocessor (unknown macro, arg-count mismatch, unrecognized `//!`
// directive, recursion limit). Empty when the section didn't use macros
// (or used them cleanly). The UI surfaces these as a small warning bar
// above the diff editor; they do NOT block conversion.
//
// LibScopeWarnings is the analogous bucket for the Phase-2 library/scope
// preprocessor (unresolved requires, dep cycles, missing endlibrary,
// stray closer at top level). Independent from PreprocessWarnings so the
// UI can render them as separate amber bars and the user can tell which
// pass produced which diagnostic.
//
// StructWarnings is the analogous bucket for the Phase-3 struct preprocessor
// (unsupported `extends array`, `delegate` member, missing `endstruct`,
// unrecognized struct body line). Independent so the UI can show a third
// amber bar.
//
// InitOrder lists the libraries detected in this section in dep-respecting
// order. The aggregate across all sections drives the synthetic init
// function the codegen emits — see emit.go's
// emitLibScopeInitFunction call.
//
// StructInits lists the structs that defined `static method onInit`; the
// aggregate is threaded through to the codegen so each Foo.onInit() fires
// after library inits (and AFTER the struct snippet is itself defined,
// since the snippet creates the static method).
type TranspileSection struct {
	ID                 int32                  `json:"id"`
	Label              string                 `json:"label"`
	Kind               string                 `json:"kind"` // "script", "custom_text", "global_jass", "map_header"
	Original           string                 `json:"original"`
	Transpiled         string                 `json:"transpiled"`
	Diagnostics        []Diagnostic           `json:"diagnostics,omitempty"`
	Errors             []string               `json:"errors,omitempty"`
	PreprocessWarnings []string               `json:"preprocess_warnings,omitempty"`
	LibScopeWarnings   []string               `json:"libscope_warnings,omitempty"`
	ModuleWarnings     []string               `json:"module_warnings,omitempty"`
	StructWarnings     []string               `json:"struct_warnings,omitempty"`
	InitOrder          []jass2lua.LibraryInit `json:"init_order,omitempty"`
	StructInits        []jass2lua.StructInit  `json:"struct_inits,omitempty"`
}

// TranspilePreview returns a pure-read diff payload for the convert dialog.
// Empty Sections when the map has no transpilable script content (a pure-GUI
// map). The preview never writes to disk.
func (s *Session) TranspilePreview() (*TranspilePreview, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	if !s.loaded {
		return nil, fmt.Errorf("no map loaded")
	}
	out := &TranspilePreview{Sections: []TranspileSection{}}

	// P2#6: build the cross-section vJASS context ONCE from every section's
	// source, so a section that references a struct/module/textmacro/library
	// defined in ANOTHER section sees those definitions during its own
	// transpile (the flat war3map.j path already sees everything at once; this
	// mirrors that visibility for the per-section path). The context only
	// supplies VISIBILITY — each section still emits only its own struct/module
	// Lua, so the per-section output mapping is unchanged.
	ctx := jass2lua.BuildGlobalContext(s.collectSectionSourcesLocked())

	// Hand-rolled map_header path.
	if s.triggerIsHandRolled {
		if s.triggers != nil && len(s.triggers.Triggers) > 0 {
			tr := &s.triggers.Triggers[0]
			if strings.TrimSpace(tr.CustomText) != "" {
				sec := transpileSectionScript(tr.ID, "Map Header (war3map.j)", "map_header", tr.CustomText, ctx)
				out.Sections = append(out.Sections, sec)
			}
		}
		return out, nil
	}

	// Global JASS header (war3map.wct) — top of the diff so the user sees it first.
	if s.triggersWct != nil && strings.TrimSpace(s.triggersWct.GlobalJASS) != "" {
		sec := transpileSectionScript(-1, "war3map.wct GlobalJASS", "global_jass", s.triggersWct.GlobalJASS, ctx)
		out.Sections = append(out.Sections, sec)
	}

	if s.triggers == nil {
		return out, nil
	}

	// Per-trigger sections.
	for i := range s.triggers.Triggers {
		tr := &s.triggers.Triggers[i]
		if tr.IsComment {
			continue
		}
		if (tr.IsScript || tr.Classifier == wtg.ClassifierScript) && strings.TrimSpace(tr.CustomText) != "" {
			sec := transpileSectionScript(tr.ID, "Custom Script: "+tr.Name, "script", tr.CustomText, ctx)
			out.Sections = append(out.Sections, sec)
			continue
		}
		if strings.TrimSpace(tr.CustomText) != "" {
			// GUI trigger with custom_text overlay — auto-wrap as a function.
			sec := transpileSectionCustomText(tr.ID, tr.Name, tr.CustomText, ctx)
			out.Sections = append(out.Sections, sec)
		}
	}
	return out, nil
}

// collectSectionSourcesLocked returns the raw source text of every section the
// convert flow transpiles (custom-script triggers, GUI custom-text overlays,
// the war3map.wct GlobalJASS header, and a hand-rolled war3map.j). It is the
// input to jass2lua.BuildGlobalContext, which derives the cross-section
// struct/module/textmacro/library visibility set.
//
// Caller must hold s.mu (read or write). Order mirrors the transpile order so
// the global context's last-wins duplicate resolution is deterministic and
// matches the diff the user sees.
func (s *Session) collectSectionSourcesLocked() []string {
	var srcs []string
	if s.triggerIsHandRolled {
		if s.triggers != nil && len(s.triggers.Triggers) > 0 {
			tr := &s.triggers.Triggers[0]
			if strings.TrimSpace(tr.CustomText) != "" {
				srcs = append(srcs, tr.CustomText)
			}
		}
		return srcs
	}
	if s.triggersWct != nil && strings.TrimSpace(s.triggersWct.GlobalJASS) != "" {
		srcs = append(srcs, s.triggersWct.GlobalJASS)
	}
	if s.triggers != nil {
		for i := range s.triggers.Triggers {
			tr := &s.triggers.Triggers[i]
			if tr.IsComment {
				continue
			}
			if strings.TrimSpace(tr.CustomText) != "" {
				srcs = append(srcs, tr.CustomText)
			}
		}
	}
	return srcs
}

// transpileSectionScript runs the full preprocessor pipeline for a section
// whose body is a full JASS script (function decls + globals). Pipeline:
//
//	stripBlockComments → Preprocess (textmacros + //! if/else/endif)
//	  → PreprocessLibScope → PreprocessModules → PreprocessInterfaces
//	  → PreprocessDefines → PreprocessStructs (with modules + knownNames)
//	  → TranspileScript → SpliceStructLua
//
// Diagnostics from each pass surface separately on the section.
//
// Phase 5: appendNonEmpty filters defensively so a nil-but-not-empty error
// message (e.g. `fmt.Errorf("")` or a `Parse()` result with empty parse
// errors) never produces a blank bullet in the UI's transpiler-diagnostics
// list. This was the root cause of the "424 sections with empty error
// strings" observation: an unsurfaced parser-error branch could push
// blank entries into `p.errs`, and the helper join produced a non-nil
// error.Error() that was empty or all-whitespace.
func transpileSectionScript(id int32, label, kind, src string, ctx *jass2lua.GlobalContext) TranspileSection {
	stripped := jass2lua.StripBlockComments(src)
	// P2#6: seed cross-section textmacros so a `//! runtextmacro` whose
	// definition lives in another section expands here too.
	pp := jass2lua.PreprocessWithMacros(stripped, ctx.MacrosFor())
	ls := jass2lua.PreprocessLibScope(pp.Expanded)
	mods := jass2lua.PreprocessModules(ls.Expanded)
	withoutIfaces := jass2lua.PreprocessInterfaces(mods.Expanded)
	withoutDefines := jass2lua.PreprocessDefines(withoutIfaces)
	// Merge the section's own top-level names with the global dot-forcing set
	// (cross-section struct names + library publics) so the struct ref-rewriter
	// treats a struct defined elsewhere as dot (static) not colon (instance).
	known := ctx.KnownNamesFor(jass2lua.CollectTopLevelNames(withoutDefines, ls.PublicNames, nil))
	// Pass the GLOBAL module table so a struct's `implement Foo` inlines Foo's
	// body even when Foo is defined in another section (cross-trigger vJASS).
	// inlineImplements sanitizes the pasted body so interface/documentation
	// modules degrade gracefully instead of cascading — see ModulesFor.
	st := jass2lua.PreprocessStructs(withoutDefines, ctx.ModulesFor(mods.Modules), known)
	// P0#2: a "script" section whose post-preprocess body has no top-level
	// function/globals decl is a bare statement list (a GUI-style custom-script
	// trigger body), not a full JASS file. Feeding such a body to the top-level
	// parser (TranspileScript) rejects every if/call/set/loop as an unexpected
	// top-level token → walls of error(). Route it through the body-fragment
	// parser instead (exactly as custom_text sections do). True full-script
	// bodies (with `function Foo takes ...` / `globals`) still go to
	// TranspileScript and keep their existing behavior.
	var lua string
	var parseErrs []string
	var lexErr error
	if jass2lua.HasTopLevelDecl(st.Expanded) {
		lua, parseErrs, lexErr = jass2lua.TranspileScriptWithErrors(st.Expanded)
	} else {
		lua, parseErrs, lexErr = jass2lua.TranspileFunctionWithErrors(sectionFuncName(label, id), st.Expanded)
	}
	// P0#1 completion: struct method bodies are transpiled DURING the splice
	// (emitStructLuaInto → transpileMethodBodyWithErrors). Those per-statement
	// parse errors were previously swallowed (SpliceStructLua returned only a
	// string), so a section full of struct-method error() markers reported
	// section.Errors==0. Collect them here and fold them into the section's
	// errors alongside the top-level body's parse errors.
	var structErrs []string
	if lexErr == nil || lua != "" {
		lua, structErrs = jass2lua.SpliceStructLuaWithErrors(lua, st)
	}
	cleanLua, diags := extractDiagnostics(lua)
	sec := TranspileSection{
		ID:                 id,
		Label:              label,
		Kind:               kind,
		Original:           src,
		Transpiled:         cleanLua,
		Diagnostics:        diags,
		PreprocessWarnings: preprocessWarningsToStrings(pp.Errors),
		LibScopeWarnings:   libScopeWarningsToStrings(ls.Errors),
		ModuleWarnings:     moduleWarningsToStrings(mods.Errors),
		StructWarnings:     structWarningsToStrings(st.Errors),
		InitOrder:          ls.InitOrder,
		StructInits:        st.Inits,
	}
	// P0#1: surface the REAL error count. Each recovered parse error maps 1:1
	// to an inline error("jass2lua failed: ...") marker the emitter wrote, so
	// the section's Errors now reflects reality instead of under-reporting to 0.
	sec.Errors = appendErrorMessage(sec.Errors, lexErr)
	sec.Errors = appendErrorMessages(sec.Errors, parseErrs)
	sec.Errors = appendErrorMessages(sec.Errors, structErrs)
	return sec
}

// sectionFuncName derives a stable Lua function name for the bare-statement
// auto-wrap shim from a section's label/id. sanitizeName (in jass2lua) further
// scrubs the result, so this only needs to produce a non-empty seed.
func sectionFuncName(label string, id int32) string {
	if strings.TrimSpace(label) != "" {
		return label
	}
	return fmt.Sprintf("Section_%d", id)
}

// transpileSectionCustomText is the equivalent for GUI-overlay custom_text
// sections, which are bare statement lists rather than full scripts. The
// full preprocessor pipeline still runs (an overlay can contain a library
// or struct or module, though it's rare) before falling through to
// TranspileFunction. Splice struct Lua afterwards.
func transpileSectionCustomText(id int32, triggerName, src string, ctx *jass2lua.GlobalContext) TranspileSection {
	stripped := jass2lua.StripBlockComments(src)
	// P2#6: cross-section visibility — see transpileSectionScript.
	pp := jass2lua.PreprocessWithMacros(stripped, ctx.MacrosFor())
	ls := jass2lua.PreprocessLibScope(pp.Expanded)
	mods := jass2lua.PreprocessModules(ls.Expanded)
	withoutIfaces := jass2lua.PreprocessInterfaces(mods.Expanded)
	withoutDefines := jass2lua.PreprocessDefines(withoutIfaces)
	known := ctx.KnownNamesFor(jass2lua.CollectTopLevelNames(withoutDefines, ls.PublicNames, nil))
	st := jass2lua.PreprocessStructs(withoutDefines, ctx.ModulesFor(mods.Modules), known)
	lua, parseErrs, lexErr := jass2lua.TranspileFunctionWithErrors(triggerName+"_CustomText", st.Expanded)
	// P0#1 completion: also surface struct method-body transpile errors produced
	// during the splice (see transpileSectionScript for the full rationale).
	var structErrs []string
	if lexErr == nil || lua != "" {
		lua, structErrs = jass2lua.SpliceStructLuaWithErrors(lua, st)
	}
	cleanLua, diags := extractDiagnostics(lua)
	sec := TranspileSection{
		ID:                 id,
		Label:              "Custom Text: " + triggerName,
		Kind:               "custom_text",
		Original:           src,
		Transpiled:         cleanLua,
		Diagnostics:        diags,
		PreprocessWarnings: preprocessWarningsToStrings(pp.Errors),
		LibScopeWarnings:   libScopeWarningsToStrings(ls.Errors),
		ModuleWarnings:     moduleWarningsToStrings(mods.Errors),
		StructWarnings:     structWarningsToStrings(st.Errors),
		InitOrder:          ls.InitOrder,
		StructInits:        st.Inits,
	}
	// P0#1: surface the real per-statement error count, not a single joined
	// string (or nothing).
	sec.Errors = appendErrorMessage(sec.Errors, lexErr)
	sec.Errors = appendErrorMessages(sec.Errors, parseErrs)
	sec.Errors = appendErrorMessages(sec.Errors, structErrs)
	return sec
}

// appendErrorMessage is the Phase 5 defensive helper that filters empty /
// whitespace-only error strings before adding them to the section's Errors
// slice. The bug it fixes: parser-error aggregation paths can produce
// `fmt.Errorf("")` (e.g. join of `[]string{""}` or a snippet capture that
// loses its source bytes), and the unfiltered code path was leaking those
// blank strings into the UI as empty bullets. Net effect on Enfo's FFB
// preview: ~424 sections with `errors` non-empty but unreadable.
//
// We also defang trailing-newline / leading-newline cases so messages flow
// cleanly into the UI's <li> render without odd spacing.
func appendErrorMessage(dst []string, err error) []string {
	if err == nil {
		return dst
	}
	msg := strings.TrimSpace(err.Error())
	if msg == "" {
		return dst
	}
	return append(dst, msg)
}

// appendErrorMessages is the bulk variant used to surface the structured
// per-statement parse-error list returned by TranspileScriptWithErrors /
// TranspileFunctionWithErrors. Each entry corresponds 1:1 to an inline
// error("jass2lua failed: ...") marker in the emitted Lua, so appending them
// here makes the section's reported error count match reality (P0#1). Empty /
// whitespace-only entries are filtered defensively, consistent with the
// single-error helper.
func appendErrorMessages(dst []string, msgs []string) []string {
	for _, m := range msgs {
		m = strings.TrimSpace(m)
		if m == "" {
			continue
		}
		dst = append(dst, m)
	}
	return dst
}

// preprocessWarningsToStrings flattens PreprocessError diagnostics into the
// "line N: message" strings the UI surfaces. Returns nil for the no-warning
// case so the JSON omits the field entirely (json:"...,omitempty").
//
// Empty-message entries are dropped defensively (Phase 5) so a future
// preprocessor bug pushing an empty message can't paint blank bullets.
func preprocessWarningsToStrings(errs []jass2lua.PreprocessError) []string {
	if len(errs) == 0 {
		return nil
	}
	out := make([]string, 0, len(errs))
	for _, e := range errs {
		msg := strings.TrimSpace(e.Message)
		if msg == "" {
			continue
		}
		if e.Line > 0 {
			out = append(out, fmt.Sprintf("line %d: %s", e.Line, msg))
		} else {
			out = append(out, msg)
		}
	}
	if len(out) == 0 {
		return nil
	}
	return out
}

// libScopeWarningsToStrings flattens LibScopeError diagnostics into the
// "line N: message" strings the UI surfaces. Same shape as
// preprocessWarningsToStrings; kept as a separate function so changes to
// either pass's diagnostic shape don't ripple across both.
//
// Empty-message entries are filtered (Phase 5 — see appendErrorMessage).
func libScopeWarningsToStrings(errs []jass2lua.LibScopeError) []string {
	if len(errs) == 0 {
		return nil
	}
	out := make([]string, 0, len(errs))
	for _, e := range errs {
		msg := strings.TrimSpace(e.Message)
		if msg == "" {
			continue
		}
		if e.Line > 0 {
			out = append(out, fmt.Sprintf("line %d: %s", e.Line, msg))
		} else {
			out = append(out, msg)
		}
	}
	if len(out) == 0 {
		return nil
	}
	return out
}

// moduleWarningsToStrings flattens ModuleError diagnostics into the
// "line N: message" strings the UI surfaces. Same shape as the other
// per-pass helpers. Empty-message entries are filtered (Phase 5).
func moduleWarningsToStrings(errs []jass2lua.ModuleError) []string {
	if len(errs) == 0 {
		return nil
	}
	out := make([]string, 0, len(errs))
	for _, e := range errs {
		msg := strings.TrimSpace(e.Message)
		if msg == "" {
			continue
		}
		if e.Line > 0 {
			out = append(out, fmt.Sprintf("line %d: %s", e.Line, msg))
		} else {
			out = append(out, msg)
		}
	}
	if len(out) == 0 {
		return nil
	}
	return out
}

// structWarningsToStrings flattens StructError diagnostics into the
// "line N: message" strings the UI surfaces. Same shape as the other
// per-pass helpers. Empty-message entries are filtered (Phase 5).
func structWarningsToStrings(errs []jass2lua.StructError) []string {
	if len(errs) == 0 {
		return nil
	}
	out := make([]string, 0, len(errs))
	for _, e := range errs {
		msg := strings.TrimSpace(e.Message)
		if msg == "" {
			continue
		}
		if e.Line > 0 {
			out = append(out, fmt.Sprintf("line %d: %s", e.Line, msg))
		} else {
			out = append(out, msg)
		}
	}
	if len(out) == 0 {
		return nil
	}
	return out
}

// ConvertToLuaOptions is the new params shape for ConvertToLua. Backup=true
// duplicates the map's source (folder or .w3x file) before any conversion
// happens; on backup failure the operation refuses (no half-baked state).
type ConvertToLuaOptions struct {
	Backup bool `json:"backup"`
	// Overrides maps a section ID (a trigger ID for script/custom_text/
	// map_header sections, or -1 for the war3map.wct GlobalJASS header) to
	// user-edited Lua. When present for a section, that text is written verbatim
	// instead of the transpiler's output — this is how the convert dialog lets
	// the user hand-fix sections the transpiler couldn't translate cleanly. The
	// section's library/struct init metadata is still derived from the original
	// source, so cross-section init ordering is unaffected by an edit.
	Overrides map[int32]string `json:"overrides,omitempty"`
}

// ConvertToLua is the legacy wrapper (backup defaults to true). New callers
// should prefer ConvertToLuaWithOptions.
func (s *Session) ConvertToLua() (*ConvertToLuaCheckResult, error) {
	return s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: true})
}

// ConvertToLuaWithOptions runs the full convert flow:
//   - If opts.Backup, snapshot the map source (folder copy or .w3x file copy
//     to "<name>.backup<.w3x>"). Refuse on backup failure.
//   - Transpile each pure-JASS section (per-trigger custom_text + script
//     triggers + war3map.wct GlobalJASS).
//   - Concatenate the transpiled Lua with the GUI codegen output.
//   - Write war3map.lua, delete war3map.j, set info.Lua=true.
//
// Returns ErrConvertBlocked + the blocker list embedded in the result when
// CheckConvertToLua finds vJASS content. Returns ErrAlreadyLua if the map is
// already flagged as Lua. Returns ErrBackupFailed if the requested backup
// couldn't be created.
func (s *Session) ConvertToLuaWithOptions(opts ConvertToLuaOptions) (*ConvertToLuaCheckResult, error) {
	check, err := s.CheckConvertToLua()
	if err != nil {
		return nil, err
	}
	if len(check.Blockers) > 0 {
		return check, ErrConvertBlocked
	}

	s.mu.Lock()
	if !s.loaded {
		s.mu.Unlock()
		return nil, fmt.Errorf("no map loaded")
	}
	if s.info == nil {
		s.mu.Unlock()
		return nil, fmt.Errorf("no map info loaded")
	}
	if s.info.Lua {
		s.mu.Unlock()
		return nil, ErrAlreadyLua
	}
	src := s.source
	if src == nil {
		s.mu.Unlock()
		return nil, fmt.Errorf("no source for writing")
	}
	mapPath := s.path
	s.mu.Unlock()

	// Backup BEFORE any other work — the whole point is to give the user a
	// fallback if anything later goes wrong.
	if opts.Backup {
		if err := backupMapSource(mapPath); err != nil {
			return nil, fmt.Errorf("%w: %v", ErrBackupFailed, err)
		}
	}

	// Re-acquire lock for the rest of the flow.
	s.mu.Lock()
	originalJASS, hadJASS, readErr := src.read("war3map.j")
	if readErr != nil {
		s.mu.Unlock()
		return nil, fmt.Errorf("read war3map.j: %w", readErr)
	}
	if !hadJASS {
		originalJASS = nil
	}

	// Honor preserve-marker on existing war3map.lua.
	if existing, ok, _ := src.read("war3map.lua"); ok && len(existing) > 0 {
		first := existing
		for i, b := range existing {
			if b == '\n' {
				first = existing[:i]
				break
			}
		}
		if string(first) == PreserveScriptMarker {
			s.mu.Unlock()
			return nil, ErrPreserveScript
		}
	}

	// Build the per-trigger transpiled overlay (custom_texts that need
	// translation feed into CustomTexts as Lua, so the codegen treats them
	// as already-Lua and concatenates them verbatim). Each section runs
	// through every vJASS preprocessor pass (textmacro + library/scope +
	// struct) BEFORE the transpiler so a macro-only / library-only /
	// struct-only section produces real Lua, not error placeholders.
	//
	// Per-pass aggregates:
	//   - libInits     : vJASS library InitOrder across sections; codegen
	//                    threads these into _vjass_init_libs() in dep order.
	//   - structInits  : structs that defined `static method onInit`; codegen
	//                    appends Foo.onInit() calls AFTER the library inits.
	//
	// Each section also gets its struct-Lua snippets spliced into the
	// transpiled output at the marker positions PreprocessStructs left.
	// P2#6: cross-section vJASS context, built ONCE from every section so the
	// written war3map.lua matches the preview's cross-section visibility (a
	// struct/module/textmacro/library defined in one section is visible to the
	// others during transpile). MUST match TranspilePreview's context exactly so
	// the diff the user approved is what gets written.
	convertCtx := jass2lua.BuildGlobalContext(s.collectSectionSourcesLocked())
	customTextsLua := map[int32]string{}
	var libInits []jass2lua.LibraryInit
	var structInits []jass2lua.StructInit
	if s.triggers != nil {
		for i := range s.triggers.Triggers {
			tr := &s.triggers.Triggers[i]
			if tr.IsComment {
				continue
			}
			if strings.TrimSpace(tr.CustomText) == "" {
				continue
			}
			stripped := jass2lua.StripBlockComments(tr.CustomText)
			pp := jass2lua.PreprocessWithMacros(stripped, convertCtx.MacrosFor())
			ls := jass2lua.PreprocessLibScope(pp.Expanded)
			mods := jass2lua.PreprocessModules(ls.Expanded)
			withoutIfaces := jass2lua.PreprocessInterfaces(mods.Expanded)
			withoutDefines := jass2lua.PreprocessDefines(withoutIfaces)
			known := convertCtx.KnownNamesFor(jass2lua.CollectTopLevelNames(withoutDefines, ls.PublicNames, nil))
			st := jass2lua.PreprocessStructs(withoutDefines, convertCtx.ModulesFor(mods.Modules), known)
			libInits = append(libInits, ls.InitOrder...)
			structInits = append(structInits, st.Inits...)
			var lua string
			if (tr.IsScript || tr.Classifier == wtg.ClassifierScript) && jass2lua.HasTopLevelDecl(st.Expanded) {
				// True full-script body (function/globals at top level).
				lua, _ = jass2lua.TranspileScript(st.Expanded)
			} else {
				// P0#2: bare-statement bodies — whether classified script or
				// GUI custom_text — go through the body-fragment parser so
				// top-level if/call/set/loop transpile cleanly instead of
				// emitting walls of error(). Mirrors transpileSectionScript so
				// the written Lua matches the preview.
				lua, _ = jass2lua.TranspileFunction(tr.Name+"_CustomText", st.Expanded)
			}
			lua = jass2lua.SpliceStructLua(lua, st)
			if ov, ok := opts.Overrides[tr.ID]; ok {
				lua = ov // user hand-edited this section in the convert dialog
			} else {
				lua = cleanTranspiled(lua) // strip error markers to match the cleaned preview
			}
			customTextsLua[tr.ID] = lua
		}
	}

	in := CodegenInputs{
		Triggers:    s.triggers,
		Units:       s.units,
		Doodads:     s.doodads,
		Terrain:     s.terrain,
		Info:        s.info,
		Regions:     s.regions,
		Cameras:     s.cameras,
		TD:          TriggerDataSnapshot(),
		CustomTexts: customTextsLua,
		LibInits:    libInits,
		StructInits: structInits,
	}
	if s.triggersWct != nil && strings.TrimSpace(s.triggersWct.GlobalJASS) != "" {
		stripped := jass2lua.StripBlockComments(s.triggersWct.GlobalJASS)
		pp := jass2lua.PreprocessWithMacros(stripped, convertCtx.MacrosFor())
		ls := jass2lua.PreprocessLibScope(pp.Expanded)
		mods := jass2lua.PreprocessModules(ls.Expanded)
		withoutIfaces := jass2lua.PreprocessInterfaces(mods.Expanded)
		withoutDefines := jass2lua.PreprocessDefines(withoutIfaces)
		known := convertCtx.KnownNamesFor(jass2lua.CollectTopLevelNames(withoutDefines, ls.PublicNames, nil))
		st := jass2lua.PreprocessStructs(withoutDefines, convertCtx.ModulesFor(mods.Modules), known)
		in.LibInits = append(in.LibInits, ls.InitOrder...)
		in.StructInits = append(in.StructInits, st.Inits...)
		lua, _ := jass2lua.TranspileScript(st.Expanded)
		lua = jass2lua.SpliceStructLua(lua, st)
		if ov, ok := opts.Overrides[-1]; ok {
			lua = ov // user hand-edited the GlobalJASS section (id -1)
		} else {
			lua = cleanTranspiled(lua) // strip error markers to match the cleaned preview
		}
		in.GlobalJASS = lua
	}

	infoCopy := *s.info
	infoCopy.Lua = true
	in.Info = &infoCopy
	s.mu.Unlock()

	luaText, err := GenerateLuaScript(in)
	if err != nil {
		return nil, fmt.Errorf("generate war3map.lua: %w", err)
	}

	s.mu.Lock()
	if !s.loaded || s.info == nil || s.source == nil {
		s.mu.Unlock()
		return nil, fmt.Errorf("map closed during conversion")
	}
	cmd := &convertToLuaCmd{
		originalJASS:    originalJASS,
		hadOriginalJASS: hadJASS,
		generatedLua:    []byte(luaText),
		wasLua:          s.info.Lua,
		oldFileVersion:  s.info.FileVersion,
	}
	if err := cmd.Apply(s); err != nil {
		s.mu.Unlock()
		return nil, err
	}
	wasDirty := s.anyDirtyLocked()
	s.dirtyInfo = true
	s.mapHeaderScriptDirty = false
	s.recordCommand(cmd)
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "info", ID: 0, Field: "lua"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return &ConvertToLuaCheckResult{Blockers: []ConvertBlocker{}}, nil
}

// backupMapSource snapshots the map source so the user can roll back if the
// converted Lua misbehaves. The backup is NOT touched by Undo — it sits next
// to the original until the user manually deletes it.
//
// Naming:
//   - Folder source (e.g. `C:/path/to/MyMap`)        → `C:/path/to/MyMap.backup` (folder copy)
//   - .w3x file source (`C:/path/to/MyMap.w3x`)      → `C:/path/to/MyMap.backup.w3x` (file copy)
//
// If a backup with the chosen name already exists, we append `-N` until we
// find a free path. This costs a stat per attempt but is bounded — users
// rarely have more than a handful of historical backups around.
func backupMapSource(mapPath string) error {
	if mapPath == "" {
		return fmt.Errorf("no map path available for backup")
	}
	st, err := os.Stat(mapPath)
	if err != nil {
		return fmt.Errorf("stat map %q: %w", mapPath, err)
	}
	dest := pickBackupPath(mapPath, st.IsDir())
	if st.IsDir() {
		return copyDir(mapPath, dest)
	}
	return copyFile(mapPath, dest)
}

// pickBackupPath finds a non-colliding backup path for mapPath. For folders
// the suffix is `.backup`; for files we splice `.backup` before the original
// extension (`.backup.w3x` reads naturally and keeps the extension's MIME
// hint intact). Collisions append `-1`, `-2`, ... to the .backup token.
func pickBackupPath(mapPath string, isDir bool) string {
	if isDir {
		base := mapPath + ".backup"
		try := base
		for i := 1; pathExists(try); i++ {
			try = fmt.Sprintf("%s-%d", base, i)
		}
		return try
	}
	ext := filepath.Ext(mapPath)
	noExt := strings.TrimSuffix(mapPath, ext)
	base := noExt + ".backup" + ext
	try := base
	for i := 1; pathExists(try); i++ {
		try = fmt.Sprintf("%s.backup-%d%s", noExt, i, ext)
	}
	return try
}

func pathExists(p string) bool {
	_, err := os.Stat(p)
	return err == nil
}

func copyFile(src, dst string) error {
	in, err := os.Open(src)
	if err != nil {
		return err
	}
	defer in.Close()
	out, err := os.Create(dst)
	if err != nil {
		return err
	}
	defer out.Close()
	if _, err := io.Copy(out, in); err != nil {
		return err
	}
	return out.Sync()
}

func copyDir(src, dst string) error {
	return filepath.WalkDir(src, func(path string, d fs.DirEntry, err error) error {
		if err != nil {
			return err
		}
		rel, err := filepath.Rel(src, path)
		if err != nil {
			return err
		}
		target := filepath.Join(dst, rel)
		if d.IsDir() {
			return os.MkdirAll(target, 0o755)
		}
		// Make sure parent exists (WalkDir visits files before symlinks
		// sometimes; defensive).
		if err := os.MkdirAll(filepath.Dir(target), 0o755); err != nil {
			return err
		}
		return copyFile(path, target)
	})
}

// convertToLuaCmd captures everything needed to undo a Convert-to-Lua step:
//   - the original war3map.j bytes (nil + hadOriginalJASS=false when the map
//     had no .j file on disk to begin with)
//   - the generated war3map.lua bytes (so Apply/Redo regenerates byte-for-byte
//     instead of re-running the codegen, which is expensive and may produce
//     different output if other state has changed since the original convert)
//   - the wasLua flag (always false in practice — ConvertToLua refuses when
//     info.Lua is already true — but stored for symmetry)
//
// IMPORTANT: backup is NOT tracked by this command — undo restores the
// pre-conversion state in-place; the backup file is the user's belt-and-
// suspenders fallback and we never auto-delete it.
type convertToLuaCmd struct {
	originalJASS    []byte
	hadOriginalJASS bool
	generatedLua    []byte
	wasLua          bool
	oldFileVersion  int32
}

func (c *convertToLuaCmd) Label() string { return "Convert to Lua" }

func (c *convertToLuaCmd) Apply(s *Session) error {
	if s.source == nil || s.info == nil {
		return fmt.Errorf("convert: no map loaded")
	}
	if err := s.source.write("war3map.lua", c.generatedLua); err != nil {
		return fmt.Errorf("write war3map.lua: %w", err)
	}
	if err := s.source.delete("war3map.j"); err != nil {
		return fmt.Errorf("delete war3map.j: %w", err)
	}
	// These are DIRECT folder writes/deletes bypassing Save's batch commit, and
	// both names are in the change-detection baseline. Re-stamp war3map.lua (now
	// present with new bytes) and drop war3map.j (now gone) so a later Save
	// doesn't read our own conversion back as an external change/delete. The lock
	// is held by the command dispatcher; folder-only (no-op for MPQ). See
	// atomic_save.go.
	if fs, ok := s.source.(folderSource); ok {
		s.refreshBaselineEntryLocked(fs.root, "war3map.lua")
		s.refreshBaselineEntryLocked(fs.root, "war3map.j")
	}
	s.info.Lua = true
	if s.info.FileVersion < w3i.FileVersionRefV28 {
		s.info.FileVersion = w3i.FileVersionRefV28
	}
	s.dirtyInfo = true
	return nil
}

func (c *convertToLuaCmd) Revert(s *Session) error {
	if s.source == nil || s.info == nil {
		return fmt.Errorf("convert revert: no map loaded")
	}
	if c.hadOriginalJASS {
		if err := s.source.write("war3map.j", c.originalJASS); err != nil {
			return fmt.Errorf("restore war3map.j: %w", err)
		}
	} else {
		if err := s.source.delete("war3map.j"); err != nil {
			return fmt.Errorf("clear war3map.j: %w", err)
		}
	}
	if err := s.source.delete("war3map.lua"); err != nil {
		return fmt.Errorf("delete war3map.lua: %w", err)
	}
	// Symmetric to Apply: re-stamp/drop the baseline for both script names after
	// the direct folder writes/deletes so a later Save doesn't flag our undo as an
	// external change. refreshBaselineEntryLocked stamps a present file and drops a
	// now-absent one, so it's correct whether war3map.j was restored or cleared.
	if fs, ok := s.source.(folderSource); ok {
		s.refreshBaselineEntryLocked(fs.root, "war3map.j")
		s.refreshBaselineEntryLocked(fs.root, "war3map.lua")
	}
	s.info.Lua = c.wasLua
	s.info.FileVersion = c.oldFileVersion
	s.dirtyInfo = true
	return nil
}

func (c *convertToLuaCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "info", ID: 0, Field: "lua"}}
}

var _ Command = (*convertToLuaCmd)(nil)
