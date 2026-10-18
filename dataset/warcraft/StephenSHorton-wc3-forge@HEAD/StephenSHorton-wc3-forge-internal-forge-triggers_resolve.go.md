---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_resolve.go"
ref: HEAD
---

package forge

import (
	"fmt"
	"log"
	"regexp"
	"strconv"
	"strings"
	"sync"

	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// Trigger Editor entity-name resolution — Phase 1b.
//
// ECAs reference units/destructables/regions/cameras by codegen-generated
// global names:
//
//   gg_unit_<FourCC>_<NNNN>    — unit by creation_number NNNN, of type FourCC
//   gg_dest_<FourCC>_<NNNN>    — destructible/doodad by creation_number NNNN
//   gg_rct_<name>              — region named <name> (war3map.w3r — not parsed yet)
//   gg_cam_<name>              — camera named <name> (war3map.w3c — not parsed yet)
//
// HiveWE scrapes these from `custom_text` STRINGS to know which globals to
// emit (triggers.ixx L795-809). We do the OPPOSITE: parse them out of
// Parameter.Value and look up entity display names so the Trigger Editor
// shows "Paladin (1)" instead of "gg_unit_Hpal_0001".
//
// Region + camera resolution is deferred — neither war3map.w3r nor war3map.w3c
// has a parser in wc3-forge yet (TODO Phase 1c or later). Until then, those
// references fall back to the raw identifier; we log the first miss per
// (session, id) so we can audit how often the gap bites real maps.

// gg* — covers all four reference shapes in one pass. The capture groups are:
//
//   group 1: "unit" | "dest" | "rct" | "cam"
//   group 2: FourCC + "_" + creation_number for unit/dest;
//            raw name for rct/cam.
//
// Anchored to start + end of string because trigger ECA parameter values are
// the whole identifier (no embedding inside JASS expressions — those land as
// custom-script text, not as GUI parameters).
var ggRefRE = regexp.MustCompile(`^gg_(unit|dest|rct|cam)_(.+)$`)

// unitDestRefRE breaks the "<FourCC>_<NNNN>" tail of gg_unit_/gg_dest_ into
// its two halves. FourCCs are always exactly 4 chars; the suffix is one or
// more decimal digits (HiveWE assigns them sequentially, so even the millionth
// placed unit fits fine in u32).
var unitDestRefRE = regexp.MustCompile(`^(.{4})_(\d+)$`)

// unresolvedLogged tracks (sessionPath, raw) pairs we've already logged so the
// "couldn't resolve gg_rct_foo" warning fires once per startup, not once per
// triggers.get call. The key includes the session path so reopening a fresh
// map gets a fresh warning set. Reset by Session.Close via the lifecycle hook.
var (
	unresolvedLoggedMu sync.Mutex
	unresolvedLogged   = map[string]struct{}{}
)

// resetUnresolvedLog wipes the per-session memo. Wired off Session.OnMapChanged
// so each map open starts fresh — otherwise a long-running editor accumulates
// stale keys forever.
func resetUnresolvedLog() {
	unresolvedLoggedMu.Lock()
	unresolvedLogged = map[string]struct{}{}
	unresolvedLoggedMu.Unlock()
}

// init subscribes the unresolved-log reset to the session's map-changed bus
// so each map open starts with a clean dedupe set. Subscribing here (rather
// than in main's startup or from each open call) keeps the reset path in the
// same file as the dedupe state.
func init() {
	Current.OnMapChanged(func(loaded bool) {
		resetUnresolvedLog()
	})
}

// logUnresolvedOnce emits a single warning per (sessionPath, raw) pair. The
// session-path scope means swapping maps refreshes the dedupe set — a region
// missing from map A might be present in map B, and we want to surface that
// gap when B loads.
func logUnresolvedOnce(sessionPath, raw string) {
	key := sessionPath + "|" + raw
	unresolvedLoggedMu.Lock()
	_, seen := unresolvedLogged[key]
	if !seen {
		unresolvedLogged[key] = struct{}{}
	}
	unresolvedLoggedMu.Unlock()
	if !seen {
		log.Printf("triggers: unresolved entity ref %q (no parser yet for this kind)", raw)
	}
}

// resolveEntityName returns the human-readable display name for a gg_*_
// reference, or "" if raw isn't one (so callers can branch cleanly on empty).
//
// Examples:
//
//	gg_unit_Hpal_0001 → "Paladin (1)"
//	gg_dest_LTlt_0042 → "Summer Tree Wall (42)"
//	gg_rct_StartZone  → "" (no w3r parser yet; logged once)
//	gg_cam_OpeningCam → "" (no w3c parser yet; logged once)
//	udg_PlayerScore   → "" (not a gg_*_ reference at all)
//
// The (NNNN) suffix on resolved results disambiguates duplicates — a map can
// place many Paladins, and the trigger may reference any one of them.
func resolveEntityName(s *Session, raw string) string {
	m := ggRefRE.FindStringSubmatch(raw)
	if m == nil {
		return ""
	}
	kind := m[1]
	tail := m[2]
	switch kind {
	case "unit":
		return resolveUnitOrDestRef(s, tail, true /*units*/, raw)
	case "dest":
		return resolveUnitOrDestRef(s, tail, false /*destructibles/doodads*/, raw)
	case "rct":
		return resolveRegionRef(s, tail, raw)
	case "cam":
		return resolveCameraRef(s, tail, raw)
	}
	return ""
}

// resolveRegionRef looks up the region named `tail` (the underscore-suffix of
// gg_rct_) inside the loaded war3map.w3r. Returns the region's Name (with no
// "(N)" suffix — regions are unique by name in HiveWE's editor, unlike unit
// placements). Returns "" when the regions file is absent OR no match exists;
// in the latter case we log once so a stale gg_rct_ reference surfaces.
//
// HiveWE codegen replaces spaces in region names with underscores when emitting
// the gg_rct_ global. We mirror that by checking both verbatim AND with
// underscores → spaces substitution.
func resolveRegionRef(s *Session, tail, raw string) string {
	rf := s.Regions()
	if rf == nil {
		logUnresolvedOnce(s.Path(), raw)
		return ""
	}
	// Direct match first (most common: region named "StartZone").
	for i := range rf.Regions {
		if rf.Regions[i].Name == tail {
			return rf.Regions[i].Name
		}
	}
	// Underscores-as-spaces fallback.
	spaced := strings.ReplaceAll(tail, "_", " ")
	for i := range rf.Regions {
		if rf.Regions[i].Name == spaced {
			return rf.Regions[i].Name
		}
	}
	logUnresolvedOnce(s.Path(), raw)
	return ""
}

// resolveCameraRef mirrors resolveRegionRef for war3map.w3c. Cameras have the
// same name-uniqueness guarantee + the same space → underscore codegen
// transform.
func resolveCameraRef(s *Session, tail, raw string) string {
	cf := s.Cameras()
	if cf == nil {
		logUnresolvedOnce(s.Path(), raw)
		return ""
	}
	for i := range cf.Cameras {
		if cf.Cameras[i].Name == tail {
			return cf.Cameras[i].Name
		}
	}
	spaced := strings.ReplaceAll(tail, "_", " ")
	for i := range cf.Cameras {
		if cf.Cameras[i].Name == spaced {
			return cf.Cameras[i].Name
		}
	}
	logUnresolvedOnce(s.Path(), raw)
	return ""
}

// resolveUnitOrDestRef does the actual creation_number → display-name lookup
// for unit + destructible references. isUnit branches on which kind's table
// to walk (units.Entities for "unit", doodads.Doodads for "dest").
//
// Returns "" (which the caller folds into "leave raw") when:
//   - the FourCC + number suffix can't be parsed
//   - no entity with that creation_number exists
//   - the matched entity's TypeID can't be resolved through the kind's
//     MergedObjects map (rare — missing SLK or test stubs)
//
// We do NOT match TypeID against the FourCC in the gg_*_<FourCC>_<NNNN> — the
// codegen name's FourCC is informational; if a unit was retyped between map
// authoring and trigger codegen, we still want the editor to show the actual
// type, not the stale one in the variable name.
func resolveUnitOrDestRef(s *Session, tail string, isUnit bool, raw string) string {
	m := unitDestRefRE.FindStringSubmatch(tail)
	if m == nil {
		return ""
	}
	// FourCC in m[1] is informational; the creation_number drives the lookup.
	nStr := m[2]
	n, err := strconv.ParseUint(nStr, 10, 32)
	if err != nil {
		return ""
	}
	cn := uint32(n)

	var typeID string
	var found bool
	if isUnit {
		units := s.Units()
		if units == nil {
			return ""
		}
		for i := range units.Entities {
			if units.Entities[i].CreationNumber == cn {
				typeID = units.Entities[i].TypeID
				found = true
				break
			}
		}
	} else {
		dd := s.Doodads()
		if dd == nil {
			return ""
		}
		for i := range dd.Doodads {
			if dd.Doodads[i].CreationNumber == cn {
				typeID = dd.Doodads[i].TypeID
				found = true
				break
			}
		}
	}
	if !found {
		// Entity was deleted (or this gg_*_ reference dangles). Let the caller
		// fall back to the raw identifier so the user can still see something.
		return ""
	}

	// Look up the type's display name through the same path Explorer uses
	// (MergedObjects + TRIGSTR/WESTRING resolution). cfg picks units vs
	// destructables — placed doodads from war3map.doo include both decorative
	// doodads (war3map.w3d shadow) and destructibles (war3map.w3b shadow),
	// but the codegen-generated gg_dest_ name applies to destructibles only
	// (doodads aren't trigger-addressable). Use destructables config.
	var cfg *KindConfig
	if isUnit {
		cfg = UnitsConfig()
	} else {
		cfg = DestructablesConfig()
	}
	merged, _, err := MergedObjects(cfg)
	if err != nil {
		// Loading the base SLK failed — the editor will still work for
		// renderable maps but trigger-display names degrade to raw. Don't
		// spam logs (MergedObjects already does once-per-process via the
		// kind cache).
		return ""
	}
	row, ok := merged[typeID]
	if !ok {
		// Type isn't in the merged table — maybe a custom whose w3u shadow
		// didn't load. Fall back to "<FourCC> (NNNN)" so the user still sees
		// the disambiguator instead of the codegen blob.
		return fmt.Sprintf("%s (%d)", typeID, cn)
	}
	name := strings.TrimSpace(resolveDisplay(row.Fields["name"], s.Strings()))
	if name == "" {
		// Stock rows occasionally have empty names (placeholder entries that
		// aren't displayed in HiveWE either). Fall back to TypeID.
		name = typeID
	}
	return fmt.Sprintf("%s (%d)", name, cn)
}

// EnrichTriggerWithResolvedNames is the exported wrapper used by package
// main's Wails-bound GetTrigger. Same semantics as the package-internal
// enrichTriggerWithResolvedNames — pulled out as an explicit pass-through
// because Wails bindings need to reach this from outside internal/forge.
func EnrichTriggerWithResolvedNames(s *Session, tr *wtg.Trigger) *wtg.Trigger {
	return enrichTriggerWithResolvedNames(s, tr)
}

// ResolveEntityName is the exported single-reference resolver. Useful for
// tests + ad-hoc tooling; the bulk MCP/Wails paths use
// EnrichTriggerWithResolvedNames which calls this internally per parameter.
func ResolveEntityName(s *Session, raw string) string {
	return resolveEntityName(s, raw)
}

// enrichTriggerWithResolvedNames returns a deep-COPY of tr with every
// Parameter.ResolvedDisplay populated where the Value is a gg_*_ reference
// the session can resolve. The copy is required because the cached
// *wtg.Trigger is shared across concurrent triggers.get calls — mutating it
// directly would race the next reader (and would also leak stale resolutions
// once Phase 2a's mutators land).
//
// Pass-through for non-GUI triggers (script + comment have no ECAs) and for
// triggers with empty ECA lists.
func enrichTriggerWithResolvedNames(s *Session, tr *wtg.Trigger) *wtg.Trigger {
	if tr == nil {
		return nil
	}
	if len(tr.ECAs) == 0 {
		// Cheap path — return the original pointer. No params to walk + no
		// risk of mutation because we didn't touch it.
		return tr
	}
	cp := *tr
	cp.ECAs = make([]wtg.ECA, len(tr.ECAs))
	for i := range tr.ECAs {
		cp.ECAs[i] = enrichECA(s, tr.ECAs[i])
	}
	return &cp
}

// enrichECA deep-copies one ECA + its children + parameters, filling
// ResolvedDisplay on every parameter whose Value resolves to a real entity.
// Recursive — handles nested sub-parameters and the magic-ECA child trees.
func enrichECA(s *Session, e wtg.ECA) wtg.ECA {
	out := e
	if len(e.Parameters) > 0 {
		out.Parameters = make([]wtg.Parameter, len(e.Parameters))
		for i := range e.Parameters {
			out.Parameters[i] = enrichParameter(s, e.Parameters[i])
		}
	}
	if len(e.Children) > 0 {
		out.Children = make([]wtg.ECA, len(e.Children))
		for i := range e.Children {
			out.Children[i] = enrichECA(s, e.Children[i])
		}
	}
	return out
}

// enrichParameter copies one Parameter and fills ResolvedDisplay when the
// Value is a recognized gg_*_ reference. Recurses into SubParameter (a
// CreateUnit call inside a parameter slot) + ArrayIndex (a [foo] subscript
// expression on a variable).
func enrichParameter(s *Session, p wtg.Parameter) wtg.Parameter {
	out := p
	if disp := resolveEntityName(s, p.Value); disp != "" {
		out.ResolvedDisplay = disp
	}
	if p.HasSubParameter && p.SubParameter != nil {
		sub := enrichECA(s, *p.SubParameter)
		out.SubParameter = &sub
	}
	if p.IsArray && p.ArrayIndex != nil {
		idx := enrichParameter(s, *p.ArrayIndex)
		out.ArrayIndex = &idx
	}
	return out
}
