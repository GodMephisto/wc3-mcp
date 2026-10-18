---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_triggers.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"errors"
	"fmt"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wct"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// Trigger Editor MCP surface (Phase 1a — read-only).
//
// Three handlers ship in 1a:
//
//   triggers.tree            — full tree of categories+triggers+variables
//                              the frontend can render without further calls.
//   triggers.get             — single trigger, category, or variable detail
//                              (ECAs + custom_text included).
//   triggers.functions_meta  — TriggerData.txt vocabulary (events, conditions,
//                              actions, calls) for label templating client-side.
//
// All three are read-only; Phase 2a will add triggers.set_*, triggers.add_*,
// triggers.delete_*, triggers.rename, triggers.move.

// registerTriggerHandlers wires the trigger-editor MCP methods onto bridge
// via the `reg` closure RegisterAll passes in. Kept as a separate function
// (matching the pattern registerObjectKind set) so the wiring point stays
// scoped and a future search-by-handler-prefix yields exactly one register
// site per namespace.
func registerTriggerHandlers(reg func(method string, h bridge.Handler)) {
	reg("triggers.tree", handleTriggersTree)
	reg("triggers.get", handleTriggersGet)
	reg("triggers.functions_meta", handleTriggersFunctionsMeta)
	// Phase 2a — structural mutation surface. Each handler validates,
	// dispatches into the corresponding Session mutator, and returns the
	// updated tree + the affected node's get-payload so the JS layer
	// doesn't need a second round-trip after each mutation.
	reg("triggers.add_category", handleTriggersAddCategory)
	reg("triggers.add_gui", handleTriggersAddGUI)
	reg("triggers.add_script", handleTriggersAddScript)
	reg("triggers.add_comment", handleTriggersAddComment)
	reg("triggers.add_variable", handleTriggersAddVariable)
	reg("triggers.delete", handleTriggersDelete)
	reg("triggers.rename", handleTriggersRename)
	reg("triggers.set_enabled", handleTriggersSetEnabled)
	reg("triggers.set_initially_on", handleTriggersSetInitiallyOn)
	reg("triggers.set_run_on_init", handleTriggersSetRunOnInit)
	reg("triggers.move", handleTriggersMove)
	reg("triggers.set_custom_text", handleTriggersSetCustomText)
	reg("triggers.set_description", handleTriggersSetDescription)
	reg("triggers.set_variable", handleTriggersSetVariable)
	reg("triggers.set_map_header_script", handleTriggersSetMapHeaderScript)
	// Phase 2b1 — ECA-list mutation. add_eca/delete_eca/move_eca operate on the
	// per-trigger ECA tree; set_param_value writes one Parameter slot inside an
	// existing ECA. All return the standard triggerMutationResponse with the
	// updated trigger's detail so the frontend can repaint without a 2nd fetch.
	reg("triggers.add_eca", handleTriggersAddECA)
	reg("triggers.delete_eca", handleTriggersDeleteECA)
	reg("triggers.move_eca", handleTriggersMoveECA)
	reg("triggers.set_eca_enabled", handleTriggersSetECAEnabled)
	reg("triggers.set_param_value", handleTriggersSetParamValue)
	// Phase 2b2 — sub-function builder + array toggle. The MCP wire shape uses
	// param_path []int (which addresses sub-parameter chains via the recursive
	// .SubParameter.Parameters[] walk). The legacy param_index field on
	// set_param_value remains supported for 2b1 callers via resolveParamPath.
	reg("triggers.set_param_sub_function", handleTriggersSetParamSubFunction)
	reg("triggers.clear_param_sub_function", handleTriggersClearParamSubFunction)
	reg("triggers.set_param_array", handleTriggersSetParamArray)
	// Phase 2b2 — nested ECA add. Targets magic-ECA children (IfThenElse Then-
	// branch, ForLoop body, etc.). Independent from add_eca (top-level).
	reg("triggers.add_nested_eca", handleTriggersAddNestedECA)
	// Phase 2b2 — entity instance pickers. Live-map data for unit/destructible/
	// region/camera entity-typed parameter slots.
	reg("triggers.list_unit_instances", handleTriggersListUnitInstances)
	reg("triggers.list_destructable_instances", handleTriggersListDestructableInstances)
	reg("triggers.list_regions", handleTriggersListRegions)
	reg("triggers.list_cameras", handleTriggersListCameras)
	// Phase 3 — Lua codegen + Test Map + cross-trigger search.
	//
	//   triggers.generate_script — generates the war3map.lua text, does NOT write
	//   triggers.save_script     — generates + writes war3map.lua to the source
	//   triggers.test_map        — generate + save + launch WC3 with the map preloaded
	//   triggers.search          — fuzzy substring search across all triggers
	reg("triggers.generate_script", handleTriggersGenerateScript)
	reg("triggers.save_script", handleTriggersSaveScript)
	reg("triggers.test_map", handleTriggersTestMap)
	reg("triggers.search", handleTriggersSearch)
	// Convert Map to Lua. Two-step: check (read-only blocker scan) then
	// convert (apply if clean, refuse if blocked). The convert handler
	// returns the blocker list rather than erroring when blocked, so the
	// UI can render the dedicated blocker dialog instead of a toast.
	reg("triggers.check_convert_to_lua", handleTriggersCheckConvertToLua)
	reg("triggers.convert_to_lua", handleTriggersConvertToLua)
	reg("triggers.transpile_preview", handleTriggersTranspilePreview)
}

// TriggerTreeNode is the one-shot tree-shape DTO. The frontend stitches
// nodes together via ParentID into a hierarchical view; we send a flat array
// to avoid the overhead of repeated network round-trips.
//
// Kind is a wire-stable discriminator that maps directly to the per-node UI:
//   - "category" / "map" / "library" — folder, may have children
//   - "trigger_gui"   — a normal trigger with ECAs
//   - "trigger_script" — a custom-script (JASS/Lua) trigger
//   - "trigger_comment" — a comment node
//   - "variable" — a global trigger variable (udg_*)
//
// Display-only fields (Icon hint, IsComment, IsEnabled, IsScript, etc.) are
// kept verbatim so the UI can reflect the exact toggles set in the .wtg.
type TriggerTreeNode struct {
	ID                  int32  `json:"id"`
	ParentID            int32  `json:"parent_id"`
	Kind                string `json:"kind"`
	Name                string `json:"name"`
	Description         string `json:"description,omitempty"`
	IsComment           bool   `json:"is_comment,omitempty"`
	IsEnabled           bool   `json:"is_enabled,omitempty"`
	IsScript            bool   `json:"is_script,omitempty"`
	InitiallyOn         bool   `json:"initially_on,omitempty"`
	RunOnInitialization bool   `json:"run_on_initialization,omitempty"`
	OpenState           bool   `json:"open_state,omitempty"`
}

// TriggerTreeResponse is the triggers.tree payload. Nodes are returned in
// the same order they were parsed from disk — the frontend keys off
// ParentID for hierarchy and uses array position for sibling order.
type TriggerTreeResponse struct {
	Nodes []TriggerTreeNode `json:"nodes"`
	// IsPre131 + HasGlobalJASS are advisory hints the UI uses to label the
	// map's trigger-format origin in the dialog header. Optional.
	IsPre131      bool `json:"is_pre_131,omitempty"`
	HasGlobalJASS bool `json:"has_global_jass,omitempty"`
}

// handleTriggersTree returns the full tree of categories/triggers/variables
// for the loaded map. Empty (but non-error) when no map is loaded or the map
// has no triggers; the frontend renders an empty-state message in that case.
func handleTriggersTree(_ json.RawMessage) (any, error) {
	t := Current.Triggers()
	if t == nil {
		return TriggerTreeResponse{Nodes: []TriggerTreeNode{}}, nil
	}
	out := make([]TriggerTreeNode, 0, len(t.Categories)+len(t.Triggers)+len(t.Variables))
	for _, c := range t.Categories {
		out = append(out, TriggerTreeNode{
			ID: c.ID, ParentID: c.ParentID,
			Kind:      classifierToKind(c.Classifier, false, false),
			Name:      c.Name,
			IsComment: c.IsComment,
			OpenState: c.OpenState,
		})
	}
	for _, tr := range t.Triggers {
		out = append(out, TriggerTreeNode{
			ID: tr.ID, ParentID: tr.ParentID,
			Kind:                triggerKind(tr),
			Name:                tr.Name,
			Description:         tr.Description,
			IsComment:           tr.IsComment,
			IsEnabled:           tr.IsEnabled,
			IsScript:            tr.IsScript,
			InitiallyOn:         tr.InitiallyOn,
			RunOnInitialization: tr.RunOnInitialization,
		})
	}
	for _, v := range t.Variables {
		out = append(out, TriggerTreeNode{
			ID: v.ID, ParentID: v.ParentID,
			Kind: "variable",
			Name: v.Name,
		})
	}
	return TriggerTreeResponse{
		Nodes:         out,
		IsPre131:      t.IsPre131,
		HasGlobalJASS: Current.TriggersScripts() != nil && Current.TriggersScripts().GlobalJASS != "",
	}, nil
}

// classifierToKind maps a wtg.Classifier to the wire kind tag. For non-leaf
// nodes (map / library / category) this is the entire mapping; for triggers,
// triggerKind below handles the script/gui/comment branch.
func classifierToKind(c wtg.Classifier, isComment, isScript bool) string {
	switch c {
	case wtg.ClassifierMap:
		return "map"
	case wtg.ClassifierLibrary:
		return "library"
	case wtg.ClassifierCategory:
		return "category"
	case wtg.ClassifierGUI:
		if isComment {
			return "trigger_comment"
		}
		if isScript {
			return "trigger_script"
		}
		return "trigger_gui"
	case wtg.ClassifierComment:
		return "trigger_comment"
	case wtg.ClassifierScript:
		return "trigger_script"
	case wtg.ClassifierVariable:
		return "variable"
	}
	return "unknown"
}

// triggerKind picks the per-trigger node kind. Branch on Classifier first;
// for ClassifierGUI we further break out IsComment / IsScript so the
// frontend's icon picker can show the right glyph (the same Classifier::gui
// hosts every flavor in older sub_versions).
func triggerKind(tr wtg.Trigger) string {
	if tr.Classifier == wtg.ClassifierComment {
		return "trigger_comment"
	}
	if tr.Classifier == wtg.ClassifierScript {
		return "trigger_script"
	}
	if tr.IsComment {
		return "trigger_comment"
	}
	if tr.IsScript {
		return "trigger_script"
	}
	return "trigger_gui"
}

// triggersGetParams is the input DTO for triggers.get. We accept an int32 id
// and resolve it across categories/triggers/variables in one pass — the wtg
// guarantees ids are unique across all three kinds within a single map.
type triggersGetParams struct {
	ID int32 `json:"id"`
}

// TriggerDetail is the triggers.get payload. Exactly one of Category /
// Trigger / Variable is non-nil; Kind disambiguates.
type TriggerDetail struct {
	Kind     string         `json:"kind"`
	Category *wtg.Category  `json:"category,omitempty"`
	Trigger  *wtg.Trigger   `json:"trigger,omitempty"`
	Variable *wtg.Variable  `json:"variable,omitempty"`
}

// handleTriggersGet resolves an id to its full record. The Trigger payload
// includes ECAs (with nested children + sub-parameters) and custom_text in
// one shot — the dialog's right-pane render needs everything together.
func handleTriggersGet(params json.RawMessage) (any, error) {
	var p triggersGetParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	t := Current.Triggers()
	if t == nil {
		return nil, errors.New("no map loaded (or map has no triggers)")
	}
	for i := range t.Categories {
		if t.Categories[i].ID == p.ID {
			return TriggerDetail{Kind: "category", Category: &t.Categories[i]}, nil
		}
	}
	for i := range t.Triggers {
		if t.Triggers[i].ID == p.ID {
			// Enrich every Parameter.ResolvedDisplay with the human-readable
			// name for gg_unit_/gg_dest_ references before returning. Deep
			// copies so the cached *wtg.Triggers stays byte-faithful for
			// Phase 2a's encoder. enrichTriggerWithResolvedNames is a
			// no-op when the trigger has no ECAs (script + comment kinds).
			enriched := enrichTriggerWithResolvedNames(Current, &t.Triggers[i])
			return TriggerDetail{Kind: triggerKind(t.Triggers[i]), Trigger: enriched}, nil
		}
	}
	for i := range t.Variables {
		if t.Variables[i].ID == p.ID {
			return TriggerDetail{Kind: "variable", Variable: &t.Variables[i]}, nil
		}
	}
	return nil, fmt.Errorf("no trigger node with id %d", p.ID)
}

// TriggerFunctionMeta is one row from a TriggerData.txt function section
// ([TriggerEvents]/[TriggerConditions]/[TriggerActions]/[TriggerCalls]),
// flattened into a UI-renderable shape. Argc is the resolved argument count
// (1 of the 2 load-bearing numbers; the other is in ArgTypes). DisplayName,
// ParametersTemplate, and Category come from the _Foo_* companion keys.
//
// The wire stays close to the TriggerData layout so the frontend can do all
// its label-substitution client-side using get_parameters_names-style
// template walks — no Go-side label rendering, which keeps the per-trigger
// payload trim and lets the UI re-render labels on font / locale changes
// without re-fetching.
type TriggerFunctionMeta struct {
	Name               string   `json:"name"`
	Section            string   `json:"section"` // "TriggerEvents" | "TriggerConditions" | "TriggerActions" | "TriggerCalls"
	Argc               int      `json:"argc"`
	ArgTypes           []string `json:"arg_types"` // ordered, length == argc
	ReturnType         string   `json:"return_type,omitempty"` // TriggerCalls only
	DisplayName        string   `json:"display_name,omitempty"`
	ParametersTemplate []string `json:"parameters_template,omitempty"` // _Foo_Parameters tokens (~Arg slots interleaved with literal segments)
	Defaults           []string `json:"defaults,omitempty"`
	Limits             []string `json:"limits,omitempty"`
	Category           string   `json:"category,omitempty"`
	ScriptName         string   `json:"script_name,omitempty"`
	Hint               string   `json:"hint,omitempty"`
}

// TriggerPresetMeta is one row from [TriggerParams]. Mirrors wtg.Preset
// but lives in handlers_triggers.go so the wire payload stays insulated
// from upstream struct changes.
type TriggerPresetMeta struct {
	Name        string `json:"name"`         // the key (e.g. "Player00")
	Type        string `json:"type"`         // the type this preset is a value FOR
	Value       string `json:"value"`        // JASS literal
	DisplayName string `json:"display_name"` // editor label (often WESTRING_*)
}

// TriggerTypeMeta is one row from [TriggerTypes]. BaseType is the alias root
// (empty for atomic types); CanBeGlobal / CanCompare are the per-type
// constraints HiveWE honors when populating dropdowns.
type TriggerTypeMeta struct {
	Name        string `json:"name"`
	BaseType    string `json:"base_type,omitempty"`
	DisplayName string `json:"display_name,omitempty"`
	CanBeGlobal bool   `json:"can_be_global,omitempty"`
	CanCompare  bool   `json:"can_compare,omitempty"`
}

// TriggerFunctionsMetaResponse is the triggers.functions_meta payload —
// every function-shaped entry in TriggerData.txt, plus the type/category
// dictionaries the UI needs to render labels (resolve preset values, look
// up category icons, etc.).
//
// Static for the process lifetime (the snapshot doesn't change at runtime).
// Cached at the handler-level by triggerFunctionsMetaOnce.
type TriggerFunctionsMetaResponse struct {
	Functions  []TriggerFunctionMeta `json:"functions"`
	// Categories maps TC_* code → display label (after WESTRING resolution
	// would happen if we had that wired — for 1a we expose the raw label).
	Categories map[string]string `json:"categories,omitempty"`
	// Types maps trigger-variable-type name ("integer", "unit", "abilcode")
	// → the type's display label. Kept for backward compatibility with the
	// Phase 1a payload; TypeMeta carries the richer info Phase 2b1 needs.
	Types map[string]string `json:"types,omitempty"`
	// Phase 2b1 additions — picker/param-editor data so the frontend can
	// build dropdowns without a second round-trip per click.
	Presets  []TriggerPresetMeta `json:"presets,omitempty"`
	TypeMeta []TriggerTypeMeta   `json:"type_meta,omitempty"`
}

// handleTriggersFunctionsMeta returns the full TriggerData.txt vocabulary,
// cached after first call. Backed by the embedded snapshot — no live CASC
// involvement. Returned payload is large (~6k entries × ~200 bytes each) but
// only one call per session.
func handleTriggersFunctionsMeta(_ json.RawMessage) (any, error) {
	resp := triggerFunctionsMetaCached()
	if resp == nil {
		return nil, errors.New("trigger data snapshot unavailable")
	}
	return resp, nil
}

// triggerFunctionsMetaOnce + triggerFunctionsMetaResp memoize the response
// across calls; the underlying TriggerData snapshot is itself process-wide.
var (
	triggerFunctionsMetaResp *TriggerFunctionsMetaResponse
)

// GetTriggerFunctionsMetaCached is the exported accessor used by package
// main's Wails wrapper. Same semantics as triggerFunctionsMetaCached but
// callable from outside the package.
func GetTriggerFunctionsMetaCached() *TriggerFunctionsMetaResponse {
	return triggerFunctionsMetaCached()
}

func triggerFunctionsMetaCached() *TriggerFunctionsMetaResponse {
	if triggerFunctionsMetaResp != nil {
		return triggerFunctionsMetaResp
	}
	td := TriggerDataSnapshot()
	if td == nil {
		return nil
	}
	resp := &TriggerFunctionsMetaResponse{
		Functions:  make([]TriggerFunctionMeta, 0, len(td.ArgumentCounts)),
		Categories: map[string]string{},
		Types:      map[string]string{},
	}
	sections := []string{"TriggerEvents", "TriggerConditions", "TriggerActions", "TriggerCalls"}
	for _, sect := range sections {
		rows, ok := td.Sections[sect]
		if !ok {
			continue
		}
		for key, tokens := range rows {
			if len(key) > 0 && key[0] == '_' {
				continue // companion meta
			}
			m := TriggerFunctionMeta{Name: key, Section: sect}
			argc, _ := td.Argc(key)
			m.Argc = argc
			// Slice out argument types per the per-section layout.
			// TriggerActions/Events/Conditions: [version, types...]
			// TriggerCalls: [version, event_flag, return_type, types...]
			args := tokens
			switch sect {
			case "TriggerCalls":
				if len(args) >= 3 {
					m.ReturnType = args[2]
					m.ArgTypes = filterNonTypes(args[3:])
				}
			default:
				if len(args) >= 1 {
					m.ArgTypes = filterNonTypes(args[1:])
				}
			}
			// Companion meta — _Foo_DisplayName, _Foo_Parameters, etc.
			if dn, ok := rows["_"+key+"_DisplayName"]; ok && len(dn) > 0 {
				m.DisplayName = strings.Join(dn, ",")
			}
			if pt, ok := rows["_"+key+"_Parameters"]; ok {
				m.ParametersTemplate = pt
			}
			if d, ok := rows["_"+key+"_Defaults"]; ok {
				m.Defaults = d
			}
			if l, ok := rows["_"+key+"_Limits"]; ok {
				m.Limits = l
			}
			if c, ok := rows["_"+key+"_Category"]; ok && len(c) > 0 {
				m.Category = c[0]
			}
			if sn, ok := rows["_"+key+"_ScriptName"]; ok && len(sn) > 0 {
				m.ScriptName = sn[0]
			}
			if hint := td.HintFor(key); hint != "" {
				m.Hint = hint
			}
			resp.Functions = append(resp.Functions, m)
		}
	}
	// Categories ([TriggerCategories]) — TC_* → display label (col 0).
	if rows, ok := td.Sections["TriggerCategories"]; ok {
		for key, tokens := range rows {
			if len(key) > 0 && key[0] == '_' {
				continue
			}
			if len(tokens) > 0 {
				resp.Categories[key] = tokens[0]
			}
		}
	}
	// Types ([TriggerTypes]) — name → display string (col 3 per the txt
	// comment block). Populates both the legacy `Types` map and the richer
	// `TypeMeta` array.
	for _, tt := range td.TriggerTypes() {
		resp.TypeMeta = append(resp.TypeMeta, TriggerTypeMeta{
			Name:        tt.Name,
			BaseType:    tt.BaseType,
			DisplayName: tt.DisplayName,
			CanBeGlobal: tt.CanBeGlobal,
			CanCompare:  tt.CanCompare,
		})
	}
	if rows, ok := td.Sections["TriggerTypes"]; ok {
		for key, tokens := range rows {
			if len(key) > 0 && key[0] == '_' {
				continue
			}
			// Col 3 holds the display string; some rows are shorter (legacy).
			if len(tokens) >= 4 {
				resp.Types[key] = tokens[3]
			} else if len(tokens) > 0 {
				resp.Types[key] = tokens[0]
			}
		}
	}
	// Presets ([TriggerParams]) — every row, untyped at the wire level. The
	// frontend filters by Type at render time. Keeps the payload one large
	// transferable rather than N per-type requests.
	if rows, ok := td.Sections["TriggerParams"]; ok {
		for key, toks := range rows {
			if len(key) > 0 && key[0] == '_' {
				continue
			}
			if len(toks) < 2 {
				continue
			}
			pm := TriggerPresetMeta{Name: key, Type: toks[1]}
			if len(toks) >= 3 {
				pm.Value = toks[2]
			}
			if len(toks) >= 4 {
				pm.DisplayName = toks[3]
			}
			resp.Presets = append(resp.Presets, pm)
		}
	}
	triggerFunctionsMetaResp = resp
	return resp
}

// filterNonTypes drops the "nothing" sentinel HiveWE skips for argc but keeps
// the typed slots (in their original order). The shape of TriggerData.txt
// rows like `DoNothing=0,nothing` means we'd otherwise emit a phantom
// argument; the editor's argc count skips it (per ParseTriggerData's
// counter) and the per-function detail should match.
func filterNonTypes(toks []string) []string {
	out := make([]string, 0, len(toks))
	for _, t := range toks {
		if t == "" || t == "nothing" {
			continue
		}
		// Numeric tokens are version markers / flags carried in the wrong
		// position by some companion meta — drop them too. (Real WC3 type
		// names are non-numeric.)
		if isNumeric(t) {
			continue
		}
		out = append(out, t)
	}
	return out
}

func isNumeric(s string) bool {
	if s == "" {
		return false
	}
	for _, r := range s {
		if (r < '0' || r > '9') && r != '.' && r != '-' {
			return false
		}
	}
	return true
}

// Keep wct + wtg imports referenced for compile-time guard against future
// goimports re-grooming. The types here use them indirectly.
var _ = wct.File{}
var _ = wtg.Triggers{}

// ---------------------------------------------------------------------------
// Phase 2a mutation handlers. Pattern matches the Object Editor's
// makeSetFieldHandler/makeCreateCustomHandler: validate, dispatch, return
// the updated tree + the affected node's get-payload in one round-trip so
// the JS caller doesn't have to follow up with a tree-refresh fetch.
// ---------------------------------------------------------------------------

// triggerMutationResponse is the standard wire shape returned by every
// mutation handler. Tree is the post-mutation full tree; Detail is the
// triggers.get payload for the affected node (NewID for adds, ID for
// edits/deletes). Detail is nil when the affected node was deleted.
type triggerMutationResponse struct {
	Tree   TriggerTreeResponse `json:"tree"`
	NewID  int32               `json:"new_id,omitempty"`
	Detail *TriggerDetail      `json:"detail,omitempty"`
}

// buildMutationResponse re-runs handleTriggersTree + handleTriggersGet so
// the response carries the latest state. Detail is best-effort — a missing
// node returns nil rather than erroring (the deleted-node case).
func buildMutationResponse(affectedID int32) triggerMutationResponse {
	resp := triggerMutationResponse{NewID: affectedID}
	if t, err := handleTriggersTree(nil); err == nil {
		if tt, ok := t.(TriggerTreeResponse); ok {
			resp.Tree = tt
		}
	}
	if affectedID != 0 {
		params, _ := json.Marshal(triggersGetParams{ID: affectedID})
		if d, err := handleTriggersGet(params); err == nil {
			if dd, ok := d.(TriggerDetail); ok {
				resp.Detail = &dd
			}
		}
	}
	return resp
}

type triggersAddCategoryParams struct {
	Name     string `json:"name"`
	ParentID int32  `json:"parent_id"`
}

func handleTriggersAddCategory(params json.RawMessage) (any, error) {
	var p triggersAddCategoryParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	// Idempotency guard: a single logical add_category that arrives twice in
	// quick succession (retried/echoed request, or a race across control
	// surfaces) must not create two category nodes. dedupedAddTriggerCategory
	// collapses an identical (name,parent) repeat within a tight window onto
	// the node it just created. See triggers_addcategory_guard.go.
	id, _, err := dedupedAddTriggerCategory(p.Name, p.ParentID)
	if err != nil {
		return nil, err
	}
	return buildMutationResponse(id), nil
}

type triggersAddTriggerParams struct {
	Name     string `json:"name"`
	ParentID int32  `json:"parent_id"`
}

func handleTriggersAddGUI(params json.RawMessage) (any, error) {
	var p triggersAddTriggerParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	id, err := Current.AddGUITrigger(p.Name, p.ParentID)
	if err != nil {
		return nil, err
	}
	return buildMutationResponse(id), nil
}

func handleTriggersAddScript(params json.RawMessage) (any, error) {
	var p triggersAddTriggerParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	id, err := Current.AddScriptTrigger(p.Name, p.ParentID)
	if err != nil {
		return nil, err
	}
	return buildMutationResponse(id), nil
}

func handleTriggersAddComment(params json.RawMessage) (any, error) {
	var p triggersAddTriggerParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	id, err := Current.AddCommentTrigger(p.Name, p.ParentID)
	if err != nil {
		return nil, err
	}
	return buildMutationResponse(id), nil
}

type triggersAddVariableParams struct {
	Name         string `json:"name"`
	Type         string `json:"type"`
	IsArray      bool   `json:"is_array"`
	ArraySize    int32  `json:"array_size"`
	InitialValue string `json:"initial_value"`
}

func handleTriggersAddVariable(params json.RawMessage) (any, error) {
	var p triggersAddVariableParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	id, err := Current.AddTriggerVariable(p.Name, p.Type, p.IsArray, p.ArraySize, p.InitialValue)
	if err != nil {
		return nil, err
	}
	return buildMutationResponse(id), nil
}

type triggersIDOnlyParams struct {
	ID int32 `json:"id"`
}

func handleTriggersDelete(params json.RawMessage) (any, error) {
	var p triggersIDOnlyParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteTriggerNode(p.ID); err != nil {
		return nil, err
	}
	// Affected id is 0 — the node is gone; just return the refreshed tree.
	return buildMutationResponse(0), nil
}

type triggersRenameParams struct {
	ID   int32  `json:"id"`
	Name string `json:"name"`
}

func handleTriggersRename(params json.RawMessage) (any, error) {
	var p triggersRenameParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.RenameTriggerNode(p.ID, p.Name); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

type triggersIDBoolParams struct {
	ID    int32 `json:"id"`
	Value bool  `json:"value"`
}

func handleTriggersSetEnabled(params json.RawMessage) (any, error) {
	var p triggersIDBoolParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerEnabled(p.ID, p.Value); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

func handleTriggersSetInitiallyOn(params json.RawMessage) (any, error) {
	var p triggersIDBoolParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerInitiallyOn(p.ID, p.Value); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

func handleTriggersSetRunOnInit(params json.RawMessage) (any, error) {
	var p triggersIDBoolParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerRunOnInit(p.ID, p.Value); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

type triggersMoveParams struct {
	ID          int32 `json:"id"`
	NewParentID int32 `json:"new_parent_id"`
}

func handleTriggersMove(params json.RawMessage) (any, error) {
	var p triggersMoveParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.MoveTriggerNode(p.ID, p.NewParentID); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

type triggersSetTextParams struct {
	ID   int32  `json:"id"`
	Text string `json:"text"`
}

func handleTriggersSetCustomText(params json.RawMessage) (any, error) {
	var p triggersSetTextParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerCustomText(p.ID, p.Text); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

func handleTriggersSetDescription(params json.RawMessage) (any, error) {
	var p triggersSetTextParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerDescription(p.ID, p.Text); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

type triggersSetVariableParams struct {
	ID           int32  `json:"id"`
	Name         string `json:"name"`
	Type         string `json:"type"`
	IsArray      bool   `json:"is_array"`
	ArraySize    int32  `json:"array_size"`
	InitialValue string `json:"initial_value"`
}

func handleTriggersSetVariable(params json.RawMessage) (any, error) {
	var p triggersSetVariableParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTriggerVariable(p.ID, p.Name, p.Type, p.IsArray, p.ArraySize, p.InitialValue); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.ID), nil
}

type triggersSetMapHeaderParams struct {
	Content string `json:"content"`
}

func handleTriggersSetMapHeaderScript(params json.RawMessage) (any, error) {
	var p triggersSetMapHeaderParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetMapHeaderScript(p.Content); err != nil {
		return nil, err
	}
	return buildMutationResponse(0), nil
}

// ---------------------------------------------------------------------------
// Phase 2b1 — ECA / param mutation handlers. Each maps directly onto a
// Session mutator. eca_path is a JSON array of integer child indices so
// 2b2 can address nested ECAs without breaking the wire shape.
// ---------------------------------------------------------------------------

type triggersAddECAParams struct {
	TriggerID int32  `json:"trigger_id"`
	ECAType   int    `json:"eca_type"` // wtg.ECAType (0=event, 1=condition, 2=action, 3=call)
	Name      string `json:"name"`
	Position  *int   `json:"position,omitempty"` // nil → append
}

func handleTriggersAddECA(params json.RawMessage) (any, error) {
	var p triggersAddECAParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	pos := -1
	if p.Position != nil {
		pos = *p.Position
	}
	if _, err := Current.AddECA(p.TriggerID, wtg.ECAType(p.ECAType), p.Name, pos); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersECAPathParams struct {
	TriggerID int32 `json:"trigger_id"`
	ECAPath   []int `json:"eca_path"`
}

func handleTriggersDeleteECA(params json.RawMessage) (any, error) {
	var p triggersECAPathParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteECA(p.TriggerID, p.ECAPath); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersMoveECAParams struct {
	TriggerID   int32 `json:"trigger_id"`
	ECAPath     []int `json:"eca_path"`
	NewPosition int   `json:"new_position"`
}

func handleTriggersMoveECA(params json.RawMessage) (any, error) {
	var p triggersMoveECAParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.MoveECA(p.TriggerID, p.ECAPath, p.NewPosition); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersSetECAEnabledParams struct {
	TriggerID int32 `json:"trigger_id"`
	ECAPath   []int `json:"eca_path"`
	Enabled   bool  `json:"enabled"`
}

func handleTriggersSetECAEnabled(params json.RawMessage) (any, error) {
	var p triggersSetECAEnabledParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetECAEnabled(p.TriggerID, p.ECAPath, p.Enabled); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

// triggersSetParamValueParams accepts BOTH the legacy 2b1 shape (param_index)
// and the 2b2 shape (param_path). resolveParamPath collapses them: if
// param_path is non-empty it wins, otherwise we synthesize [param_index].
// This keeps every 2b1 MCP caller working without re-coding the wire shape.
type triggersSetParamValueParams struct {
	TriggerID  int32  `json:"trigger_id"`
	ECAPath    []int  `json:"eca_path"`
	ParamIndex int    `json:"param_index"` // legacy 2b1 shape, single int
	ParamPath  []int  `json:"param_path"`  // 2b2 shape, addresses sub-parameter chains
	Value      string `json:"value"`
	ParamType  int    `json:"param_type"` // wtg.ParamType (0=preset, 1=variable, 2=function, 3=string)
}

// resolveParamPath picks the effective paramPath the mutator should use. 2b2
// shape wins when present; otherwise wrap the legacy paramIndex into a single-
// element slice. Returns nil only when both fields are missing.
func resolveParamPath(path []int, idx int) []int {
	if len(path) > 0 {
		return path
	}
	return []int{idx}
}

func handleTriggersSetParamValue(params json.RawMessage) (any, error) {
	var p triggersSetParamValueParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	pp := resolveParamPath(p.ParamPath, p.ParamIndex)
	if err := Current.SetParamValue(p.TriggerID, p.ECAPath, pp, p.Value, wtg.ParamType(p.ParamType)); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

// ---------------------------------------------------------------------------
// Phase 2b2 — sub-function builder + array toggle MCP handlers. Each wraps
// the matching Session mutator.
// ---------------------------------------------------------------------------

type triggersSetParamSubFunctionParams struct {
	TriggerID int32  `json:"trigger_id"`
	ECAPath   []int  `json:"eca_path"`
	ParamPath []int  `json:"param_path"`
	SubName   string `json:"sub_name"`
}

func handleTriggersSetParamSubFunction(params json.RawMessage) (any, error) {
	var p triggersSetParamSubFunctionParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetParamSubFunction(p.TriggerID, p.ECAPath, p.ParamPath, p.SubName); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersClearParamSubFunctionParams struct {
	TriggerID int32 `json:"trigger_id"`
	ECAPath   []int `json:"eca_path"`
	ParamPath []int `json:"param_path"`
}

func handleTriggersClearParamSubFunction(params json.RawMessage) (any, error) {
	var p triggersClearParamSubFunctionParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.ClearParamSubFunction(p.TriggerID, p.ECAPath, p.ParamPath); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersSetParamArrayParams struct {
	TriggerID int32 `json:"trigger_id"`
	ECAPath   []int `json:"eca_path"`
	ParamPath []int `json:"param_path"`
	IsArray   bool  `json:"is_array"`
}

func handleTriggersSetParamArray(params json.RawMessage) (any, error) {
	var p triggersSetParamArrayParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetParamArray(p.TriggerID, p.ECAPath, p.ParamPath, p.IsArray); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

type triggersAddNestedECAParams struct {
	TriggerID  int32  `json:"trigger_id"`
	ParentPath []int  `json:"parent_path"`
	ECAType    int    `json:"eca_type"`
	Name       string `json:"name"`
	GroupID    uint32 `json:"group_id"`
}

func handleTriggersAddNestedECA(params json.RawMessage) (any, error) {
	var p triggersAddNestedECAParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if _, err := Current.AddNestedECA(p.TriggerID, p.ParentPath, wtg.ECAType(p.ECAType), p.Name, p.GroupID); err != nil {
		return nil, err
	}
	return buildMutationResponse(p.TriggerID), nil
}

// ---------------------------------------------------------------------------
// Phase 2b2 — entity instance pickers. Used by the Trigger Editor's
// unit/destructable/region/camera ParamEditor branches. Returns the rows the
// frontend renders in a modal picker; the user's selected creation_number
// (units/destructables) or name (regions/cameras) becomes the gg_*_* string
// passed to SetParamValue.
// ---------------------------------------------------------------------------

// TriggerUnitInstance is one placed unit row exposed to the unit-instance
// picker. Position is in game coords (passed through verbatim from the .doo).
// Player is 0-indexed (player 0 = red, etc.). Name is the per-instance custom
// name when set, otherwise the type's display name from the merged SLK.
type TriggerUnitInstance struct {
	CreationNumber uint32  `json:"creation_number"`
	TypeID         string  `json:"type_id"`
	Player         uint32  `json:"player"`
	X              float32 `json:"x"`
	Y              float32 `json:"y"`
	Name           string  `json:"name"`
	GGRef          string  `json:"gg_ref"` // gg_unit_<TypeID>_<NNNN>
}

func handleTriggersListUnitInstances(_ json.RawMessage) (any, error) {
	out := buildUnitInstances()
	return map[string]any{"instances": out}, nil
}

// BuildTriggerUnitInstances is the exported accessor for package main's
// Wails wrapper. Pure-read; safe to call any time.
func BuildTriggerUnitInstances() []TriggerUnitInstance { return buildUnitInstances() }

// buildUnitInstances enumerates the placed units, applying the same display-
// name resolution the gg_*_ resolver uses. Pure-read, no mutation.
func buildUnitInstances() []TriggerUnitInstance {
	u := Current.Units()
	if u == nil {
		return nil
	}
	merged, _, _ := MergedObjects(UnitsConfig())
	out := make([]TriggerUnitInstance, 0, len(u.Entities))
	for _, e := range u.Entities {
		row := TriggerUnitInstance{
			CreationNumber: e.CreationNumber,
			TypeID:         e.TypeID,
			Player:         e.Player,
			X:              e.Position[0],
			Y:              e.Position[1],
			GGRef:          fmt.Sprintf("gg_unit_%s_%04d", e.TypeID, e.CreationNumber),
		}
		if merged != nil {
			if rec, ok := merged[e.TypeID]; ok {
				row.Name = strings.TrimSpace(resolveDisplay(rec.Fields["name"], Current.Strings()))
			}
		}
		if row.Name == "" {
			row.Name = e.TypeID
		}
		out = append(out, row)
	}
	return out
}

// TriggerDestructableInstance is one placed destructible row. Same shape as
// TriggerUnitInstance minus Player (destructibles don't have an owner).
type TriggerDestructableInstance struct {
	CreationNumber uint32  `json:"creation_number"`
	TypeID         string  `json:"type_id"`
	X              float32 `json:"x"`
	Y              float32 `json:"y"`
	Name           string  `json:"name"`
	GGRef          string  `json:"gg_ref"` // gg_dest_<TypeID>_<NNNN>
}

func handleTriggersListDestructableInstances(_ json.RawMessage) (any, error) {
	out := buildDestructableInstances()
	return map[string]any{"instances": out}, nil
}

// BuildTriggerDestructableInstances is the exported accessor for Wails.
func BuildTriggerDestructableInstances() []TriggerDestructableInstance {
	return buildDestructableInstances()
}

func buildDestructableInstances() []TriggerDestructableInstance {
	d := Current.Doodads()
	if d == nil {
		return nil
	}
	merged, _, _ := MergedObjects(DestructablesConfig())
	out := make([]TriggerDestructableInstance, 0, len(d.Doodads))
	for _, dd := range d.Doodads {
		row := TriggerDestructableInstance{
			CreationNumber: dd.CreationNumber,
			TypeID:         dd.TypeID,
			X:              dd.Position[0],
			Y:              dd.Position[1],
			GGRef:          fmt.Sprintf("gg_dest_%s_%04d", dd.TypeID, dd.CreationNumber),
		}
		if merged != nil {
			if rec, ok := merged[dd.TypeID]; ok {
				row.Name = strings.TrimSpace(resolveDisplay(rec.Fields["name"], Current.Strings()))
			}
		}
		if row.Name == "" {
			row.Name = dd.TypeID
		}
		out = append(out, row)
	}
	return out
}

// TriggerRegionInfo is one row from war3map.w3r exposed to the region picker.
// GGRef is the codegen-generated global name (gg_rct_<name with spaces →
// underscores>) the trigger uses.
type TriggerRegionInfo struct {
	Name           string  `json:"name"`
	CreationNumber int32   `json:"creation_number"`
	Left           float32 `json:"left"`
	Right          float32 `json:"right"`
	Top            float32 `json:"top"`
	Bottom         float32 `json:"bottom"`
	GGRef          string  `json:"gg_ref"`
}

func handleTriggersListRegions(_ json.RawMessage) (any, error) {
	out := buildRegionsList()
	return map[string]any{"regions": out}, nil
}

// BuildTriggerRegions is the exported accessor for Wails.
func BuildTriggerRegions() []TriggerRegionInfo { return buildRegionsList() }

func buildRegionsList() []TriggerRegionInfo {
	r := Current.Regions()
	if r == nil {
		return nil
	}
	out := make([]TriggerRegionInfo, 0, len(r.Regions))
	for _, rg := range r.Regions {
		out = append(out, TriggerRegionInfo{
			Name:           rg.Name,
			CreationNumber: rg.CreationNumber,
			Left:           rg.Left,
			Right:          rg.Right,
			Top:            rg.Top,
			Bottom:         rg.Bottom,
			GGRef:          "gg_rct_" + strings.ReplaceAll(rg.Name, " ", "_"),
		})
	}
	return out
}

// TriggerCameraInfo is one row from war3map.w3c. Subset of the Camera struct
// the picker actually needs (name, target, distance — enough for the user to
// disambiguate between presets).
type TriggerCameraInfo struct {
	Name     string  `json:"name"`
	TargetX  float32 `json:"target_x"`
	TargetY  float32 `json:"target_y"`
	Distance float32 `json:"distance"`
	GGRef    string  `json:"gg_ref"`
}

func handleTriggersListCameras(_ json.RawMessage) (any, error) {
	out := buildCamerasList()
	return map[string]any{"cameras": out}, nil
}

// BuildTriggerCameras is the exported accessor for Wails.
func BuildTriggerCameras() []TriggerCameraInfo { return buildCamerasList() }

func buildCamerasList() []TriggerCameraInfo {
	c := Current.Cameras()
	if c == nil {
		return nil
	}
	out := make([]TriggerCameraInfo, 0, len(c.Cameras))
	for _, cm := range c.Cameras {
		out = append(out, TriggerCameraInfo{
			Name:     cm.Name,
			TargetX:  cm.TargetX,
			TargetY:  cm.TargetY,
			Distance: cm.Distance,
			GGRef:    "gg_cam_" + strings.ReplaceAll(cm.Name, " ", "_"),
		})
	}
	return out
}

// ---------------------------------------------------------------------------
// Phase 3 — Lua codegen + Test Map + search handlers.
// ---------------------------------------------------------------------------

// triggerScriptResult is the wire shape for generate_script / save_script.
// Text is the Lua source (always populated for generate_script; only when
// the caller asks for it on save_script). Bytes is the file-size in bytes
// (populated on save_script).
type triggerScriptResult struct {
	Text  string `json:"text,omitempty"`
	Bytes int    `json:"bytes,omitempty"`
}

// handleTriggersGenerateScript generates the Lua war3map.lua text and
// returns it WITHOUT writing. Honors ErrLuaOnly + ErrPreserveScript.
func handleTriggersGenerateScript(_ json.RawMessage) (any, error) {
	text, err := Current.GenerateTriggerScript()
	if err != nil {
		return nil, err
	}
	return triggerScriptResult{Text: text, Bytes: len(text)}, nil
}

// triggersSaveScriptParams is the wire shape for triggers.save_script. The
// optional overwrite flag lets a caller proceed past the non-codegen guard
// (which refuses to clobber a hand-authored war3map.lua by default). When
// overwrite is true the existing file is still backed up to war3map.lua.bak
// before regeneration, so the original bytes stay recoverable.
type triggersSaveScriptParams struct {
	Overwrite bool `json:"overwrite,omitempty"`
}

// handleTriggersSaveScript generates + writes war3map.lua via the session's
// source. Returns the byte count; the text is omitted from the response to
// keep the wire payload small (callers wanting both should call
// generate_script first).
//
// Non-destructive: if the existing war3map.lua was NOT produced by wc3-forge's
// codegen (it lacks the generator marker line), the handler refuses with
// ErrScriptNotCodegen unless {overwrite:true} is passed. Passing overwrite
// backs the original up to war3map.lua.bak before regenerating. This is the
// fix for the live-confirmed bug where save_script clobbered a 51908-byte
// hand-authored script with 4361 bytes of codegen.
func handleTriggersSaveScript(params json.RawMessage) (any, error) {
	var p triggersSaveScriptParams
	if len(params) > 0 && string(params) != "null" {
		if err := json.Unmarshal(params, &p); err != nil {
			return nil, fmt.Errorf("invalid params: %w", err)
		}
	}
	n, err := Current.SaveTriggerScriptGuarded(p.Overwrite)
	if err != nil {
		return nil, err
	}
	return triggerScriptResult{Bytes: n}, nil
}

// handleTriggersTestMap is the full pipeline: save pending edits, regen +
// write war3map.lua, launch WC3 with the map preloaded. Errors propagate
// to the caller (the JS layer toasts them).
func handleTriggersTestMap(_ json.RawMessage) (any, error) {
	if err := Current.TestMap(); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// triggersSearchParams is the wire shape for triggers.search.
type triggersSearchParams struct {
	Query string `json:"query"`
	Limit int    `json:"limit,omitempty"`
}

// triggersSearchResponse wraps the hits + a flag telling the UI whether
// the result was truncated at the cap.
type triggersSearchResponse struct {
	Hits      []TriggerSearchHit `json:"hits"`
	Truncated bool               `json:"truncated,omitempty"`
}

// handleTriggersCheckConvertToLua surfaces the blocker scan via MCP. Always
// returns the result payload (Blockers may be empty) — never an error other
// than "no map loaded". UIs poll this from a "Convert to Lua…" preview.
func handleTriggersCheckConvertToLua(_ json.RawMessage) (any, error) {
	res, err := Current.CheckConvertToLua()
	if err != nil {
		return nil, err
	}
	return res, nil
}

// handleTriggersConvertToLua runs the full conversion. When blockers are
// found, the underlying ConvertToLua returns ErrConvertBlocked with the
// blocker list embedded in the result; we surface that as a NON-error
// response carrying {blockers: [...]} so the UI's blocker dialog renders
// from a typed payload instead of an error-string parse. Other errors
// (ErrAlreadyLua, codegen failures, disk errors) propagate as errors.
//
// Accepts an optional {backup: bool} param. Defaults to true (safer default
// for the "modernize my map" workflow).
func handleTriggersConvertToLua(params json.RawMessage) (any, error) {
	opts := ConvertToLuaOptions{Backup: true}
	if len(params) > 0 && string(params) != "null" {
		if err := json.Unmarshal(params, &opts); err != nil {
			return nil, fmt.Errorf("invalid params: %w", err)
		}
	}
	res, err := Current.ConvertToLuaWithOptions(opts)
	if err != nil {
		if errors.Is(err, ErrConvertBlocked) {
			return res, nil
		}
		return nil, err
	}
	return res, nil
}

// handleTriggersTranspilePreview returns the read-only diff payload the
// Convert-to-Lua dialog renders before the user commits. No writes occur.
func handleTriggersTranspilePreview(_ json.RawMessage) (any, error) {
	preview, err := Current.TranspilePreview()
	if err != nil {
		return nil, err
	}
	return preview, nil
}

// handleTriggersSearch performs the cross-trigger fuzzy search and returns
// the top N hits. Empty query returns the first N triggers by name (so the
// search palette renders a non-empty initial view).
func handleTriggersSearch(params json.RawMessage) (any, error) {
	var p triggersSearchParams
	if len(params) > 0 {
		if err := json.Unmarshal(params, &p); err != nil {
			return nil, fmt.Errorf("invalid params: %w", err)
		}
	}
	limit := p.Limit
	if limit <= 0 {
		limit = 50
	}
	hits := Current.SearchTriggers(p.Query, limit+1) // ask for one extra to detect truncation
	truncated := false
	if len(hits) > limit {
		hits = hits[:limit]
		truncated = true
	}
	return triggersSearchResponse{Hits: hits, Truncated: truncated}, nil
}
