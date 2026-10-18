---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/trigger-editor-bindings.ts"
ref: HEAD
---

// Local shim for the Trigger Editor's Wails bindings (Phase 1a, read-only).
// Pattern matches object-editor-bindings.ts: declare the App methods + JSON
// shapes here so TypeScript is happy before the first `wails build`
// regenerates wailsjs/go/main/App.js. After a fresh build these can collapse
// to re-exports from the generated module.

export type TriggerKind =
  | 'map'
  | 'library'
  | 'category'
  | 'trigger_gui'
  | 'trigger_script'
  | 'trigger_comment'
  | 'variable'
  | 'unknown'

export interface TriggerTreeNode {
  id: number
  parent_id: number
  kind: TriggerKind
  name: string
  description?: string
  is_comment?: boolean
  is_enabled?: boolean
  is_script?: boolean
  initially_on?: boolean
  run_on_initialization?: boolean
  open_state?: boolean
}

export interface TriggerTree {
  nodes: TriggerTreeNode[]
  is_pre_131?: boolean
  has_global_jass?: boolean
}

// One ECA parameter. SubParameter is recursive (a sub-function call's
// parameters are themselves Parameters). ArrayIndex carries the index
// expression for variable[expr] references.
//
// ResolvedDisplay is a UI-only enrichment populated server-side by the
// Trigger Editor's GET path — present when `value` is a recognized
// codegen-generated entity reference (gg_unit_*, gg_dest_*) that resolved
// to a placed entity's display name. Empty / absent otherwise. The frontend
// prefers it over `value` for display, but the literal `value` stays
// canonical for round-trip / copy-paste.
export interface TriggerParameter {
  type: number // ParamType: -1 invalid, 0 preset, 1 variable, 2 function, 3 string
  value: string
  has_sub_parameter?: boolean
  sub_parameter?: TriggerECA
  unknown?: number
  is_array?: boolean
  array_index?: TriggerParameter
  resolved_display?: string
}

// One Event / Condition / Action / Call row inside a trigger.
export interface TriggerECA {
  type: number // ECAType: 0 event, 1 condition, 2 action, 3 call
  group?: number
  name: string
  enabled: boolean
  parameters?: TriggerParameter[]
  children?: TriggerECA[]
}

export interface TriggerCategoryRecord {
  classifier: number
  id: number
  parent_id: number
  name: string
  open_state: boolean
  is_comment?: boolean
}

export interface TriggerRecord {
  classifier: number
  id: number
  parent_id: number
  name: string
  description?: string
  custom_text?: string
  is_comment?: boolean
  is_enabled: boolean
  is_script?: boolean
  initially_on: boolean
  run_on_initialization?: boolean
  ecas?: TriggerECA[]
}

export interface TriggerVariableRecord {
  name: string
  type: string
  unknown?: number
  is_array?: boolean
  array_size?: number
  is_initialized?: boolean
  initial_value?: string
  id: number
  parent_id: number
}

export interface TriggerDetail {
  kind: TriggerKind
  category?: TriggerCategoryRecord
  trigger?: TriggerRecord
  variable?: TriggerVariableRecord
}

// One row from a TriggerData.txt function family. ArgTypes is the ordered
// type list (e.g. ["unitcode", "player", "location"]); ParametersTemplate
// is the _Foo_Parameters template the UI walks to render labels.
export interface TriggerFunctionMeta {
  name: string
  section: 'TriggerEvents' | 'TriggerConditions' | 'TriggerActions' | 'TriggerCalls'
  argc: number
  arg_types: string[]
  return_type?: string
  display_name?: string
  parameters_template?: string[]
  defaults?: string[]
  limits?: string[]
  category?: string
  script_name?: string
  hint?: string
}

export interface TriggerPresetMeta {
  name: string
  type: string
  value: string
  display_name: string
}

export interface TriggerTypeMeta {
  name: string
  base_type?: string
  display_name?: string
  can_be_global?: boolean
  can_compare?: boolean
}

export interface TriggerFunctionsMeta {
  functions: TriggerFunctionMeta[]
  categories?: Record<string, string>
  types?: Record<string, string>
  // Phase 2b1 additions: enumerate every [TriggerParams] row and every
  // [TriggerTypes] row so the picker + param-editor can build dropdowns
  // without a per-click round-trip.
  presets?: TriggerPresetMeta[]
  type_meta?: TriggerTypeMeta[]
}

export interface TriggerMutationResult {
  tree: TriggerTree
  new_id?: number
  detail?: TriggerDetail | null
}

interface WailsApp {
  ListTriggerTree: () => Promise<TriggerTree>
  GetTrigger: (id: number) => Promise<TriggerDetail>
  GetTriggerFunctionsMeta: () => Promise<TriggerFunctionsMeta | null>
  // Phase 2a — mutation surface. Each method dispatches to the matching
  // Session mutator; returns the post-mutation tree + (when applicable) the
  // affected node's detail.
  AddTriggerCategory: (name: string, parentID: number) => Promise<TriggerMutationResult>
  AddGUITrigger: (name: string, parentID: number) => Promise<TriggerMutationResult>
  AddScriptTrigger: (name: string, parentID: number) => Promise<TriggerMutationResult>
  AddCommentTrigger: (name: string, parentID: number) => Promise<TriggerMutationResult>
  AddTriggerVariable: (name: string, varType: string, isArray: boolean, arraySize: number, initValue: string) => Promise<TriggerMutationResult>
  DeleteTriggerNode: (id: number) => Promise<TriggerMutationResult>
  RenameTriggerNode: (id: number, name: string) => Promise<TriggerMutationResult>
  SetTriggerEnabled: (id: number, enabled: boolean) => Promise<TriggerMutationResult>
  SetTriggerInitiallyOn: (id: number, initiallyOn: boolean) => Promise<TriggerMutationResult>
  SetTriggerRunOnInit: (id: number, runOnInit: boolean) => Promise<TriggerMutationResult>
  MoveTriggerNode: (id: number, newParentID: number) => Promise<TriggerMutationResult>
  SetTriggerCustomText: (id: number, text: string) => Promise<TriggerMutationResult>
  SetTriggerDescription: (id: number, text: string) => Promise<TriggerMutationResult>
  SetTriggerVariable: (id: number, name: string, varType: string, isArray: boolean, arraySize: number, initValue: string) => Promise<TriggerMutationResult>
  SetMapHeaderScript: (content: string) => Promise<TriggerMutationResult>
  // Phase 2b1 — ECA / param mutation. ecaType: 0=event, 1=condition, 2=action,
  // 3=call. ecaPath is a child-index path — length-1 for top-level ECAs.
  AddTriggerECA: (triggerID: number, ecaType: number, name: string, position: number) => Promise<TriggerMutationResult>
  DeleteTriggerECA: (triggerID: number, ecaPath: number[]) => Promise<TriggerMutationResult>
  MoveTriggerECA: (triggerID: number, ecaPath: number[], newPosition: number) => Promise<TriggerMutationResult>
  SetTriggerECAEnabled: (triggerID: number, ecaPath: number[], enabled: boolean) => Promise<TriggerMutationResult>
  // Phase 2b2 — paramPath replaces 2b1's paramIndex. paramPath[0] indexes the
  // leaf ECA's parameters; each subsequent element drills into a SubParameter
  // chain (recursive sub-function authoring). paramType: 0=preset, 1=variable,
  // 2=function, 3=string.
  SetTriggerParamValue: (triggerID: number, ecaPath: number[], paramPath: number[], value: string, paramType: number) => Promise<TriggerMutationResult>
  SetTriggerParamSubFunction: (triggerID: number, ecaPath: number[], paramPath: number[], subName: string) => Promise<TriggerMutationResult>
  ClearTriggerParamSubFunction: (triggerID: number, ecaPath: number[], paramPath: number[]) => Promise<TriggerMutationResult>
  SetTriggerParamArray: (triggerID: number, ecaPath: number[], paramPath: number[], isArray: boolean) => Promise<TriggerMutationResult>
  AddTriggerNestedECA: (triggerID: number, parentPath: number[], ecaType: number, name: string, groupID: number) => Promise<TriggerMutationResult>
  // Phase 2b2 — entity instance pickers. Live-map data for the unit/dest/
  // region/camera ParamEditor branches.
  ListTriggerUnitInstances: () => Promise<TriggerUnitInstance[]>
  ListTriggerDestructableInstances: () => Promise<TriggerDestructableInstance[]>
  ListTriggerRegions: () => Promise<TriggerRegionInfo[]>
  ListTriggerCameras: () => Promise<TriggerCameraInfo[]>
  // Phase 3 — codegen + search.
  GenerateTriggerScript: () => Promise<TriggerScriptResult>
  SaveTriggerScript: () => Promise<TriggerScriptResult>
  TestMap: () => Promise<void>
  SearchTriggers: (query: string, limit: number) => Promise<TriggerSearchHit[]>
}

// Phase 3 — codegen result + search hit.
export interface TriggerScriptResult {
  text?: string
  bytes?: number
}

export interface TriggerSearchHit {
  trigger_id: number
  trigger_name: string
  kind: 'trigger' | 'category' | 'variable' | 'eca' | 'param'
  path?: number[]
  eca_name?: string
  snippet: string
  category?: string
}

// Phase 2b2 — entity-picker rows. Each carries gg_ref the Trigger Editor
// commits as the Parameter.Value when the user picks the row.
export interface TriggerUnitInstance {
  creation_number: number
  type_id: string
  player: number
  x: number
  y: number
  name: string
  gg_ref: string
}

export interface TriggerDestructableInstance {
  creation_number: number
  type_id: string
  x: number
  y: number
  name: string
  gg_ref: string
}

export interface TriggerRegionInfo {
  name: string
  creation_number: number
  left: number
  right: number
  top: number
  bottom: number
  gg_ref: string
}

export interface TriggerCameraInfo {
  name: string
  target_x: number
  target_y: number
  distance: number
  gg_ref: string
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function app(): WailsApp {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return (window as any).go?.main?.App as WailsApp
}

export function ListTriggerTree(): Promise<TriggerTree> {
  return app().ListTriggerTree()
}

export function GetTrigger(id: number): Promise<TriggerDetail> {
  return app().GetTrigger(id)
}

export function GetTriggerFunctionsMeta(): Promise<TriggerFunctionsMeta | null> {
  return app().GetTriggerFunctionsMeta()
}

// Phase 2a mutation re-exports. Frontend imports these by name; runtime
// dispatches through the Wails-generated bindings object.
export function AddTriggerCategory(name: string, parentID: number): Promise<TriggerMutationResult> {
  return app().AddTriggerCategory(name, parentID)
}
export function AddGUITrigger(name: string, parentID: number): Promise<TriggerMutationResult> {
  return app().AddGUITrigger(name, parentID)
}
export function AddScriptTrigger(name: string, parentID: number): Promise<TriggerMutationResult> {
  return app().AddScriptTrigger(name, parentID)
}
export function AddCommentTrigger(name: string, parentID: number): Promise<TriggerMutationResult> {
  return app().AddCommentTrigger(name, parentID)
}
export function AddTriggerVariable(name: string, varType: string, isArray: boolean, arraySize: number, initValue: string): Promise<TriggerMutationResult> {
  return app().AddTriggerVariable(name, varType, isArray, arraySize, initValue)
}
export function DeleteTriggerNode(id: number): Promise<TriggerMutationResult> {
  return app().DeleteTriggerNode(id)
}
export function RenameTriggerNode(id: number, name: string): Promise<TriggerMutationResult> {
  return app().RenameTriggerNode(id, name)
}
export function SetTriggerEnabled(id: number, enabled: boolean): Promise<TriggerMutationResult> {
  return app().SetTriggerEnabled(id, enabled)
}
export function SetTriggerInitiallyOn(id: number, initiallyOn: boolean): Promise<TriggerMutationResult> {
  return app().SetTriggerInitiallyOn(id, initiallyOn)
}
export function SetTriggerRunOnInit(id: number, runOnInit: boolean): Promise<TriggerMutationResult> {
  return app().SetTriggerRunOnInit(id, runOnInit)
}
export function MoveTriggerNode(id: number, newParentID: number): Promise<TriggerMutationResult> {
  return app().MoveTriggerNode(id, newParentID)
}
export function SetTriggerCustomText(id: number, text: string): Promise<TriggerMutationResult> {
  return app().SetTriggerCustomText(id, text)
}
export function SetTriggerDescription(id: number, text: string): Promise<TriggerMutationResult> {
  return app().SetTriggerDescription(id, text)
}
export function SetTriggerVariable(id: number, name: string, varType: string, isArray: boolean, arraySize: number, initValue: string): Promise<TriggerMutationResult> {
  return app().SetTriggerVariable(id, name, varType, isArray, arraySize, initValue)
}
export function SetMapHeaderScript(content: string): Promise<TriggerMutationResult> {
  return app().SetMapHeaderScript(content)
}

// ECAType + ParamType constants — matches the Go-side wtg.ECAType /
// wtg.ParamType enums. Kept here so call sites don't sprinkle magic numbers.
export const ECAType = {
  Event: 0,
  Condition: 1,
  Action: 2,
  Call: 3,
} as const
export const ParamType = {
  Invalid: -1,
  Preset: 0,
  Variable: 1,
  Function: 2,
  String: 3,
} as const

export function AddTriggerECA(triggerID: number, ecaType: number, name: string, position: number): Promise<TriggerMutationResult> {
  return app().AddTriggerECA(triggerID, ecaType, name, position)
}
export function DeleteTriggerECA(triggerID: number, ecaPath: number[]): Promise<TriggerMutationResult> {
  return app().DeleteTriggerECA(triggerID, ecaPath)
}
export function MoveTriggerECA(triggerID: number, ecaPath: number[], newPosition: number): Promise<TriggerMutationResult> {
  return app().MoveTriggerECA(triggerID, ecaPath, newPosition)
}
export function SetTriggerECAEnabled(triggerID: number, ecaPath: number[], enabled: boolean): Promise<TriggerMutationResult> {
  return app().SetTriggerECAEnabled(triggerID, ecaPath, enabled)
}
export function SetTriggerParamValue(triggerID: number, ecaPath: number[], paramPath: number[], value: string, paramType: number): Promise<TriggerMutationResult> {
  return app().SetTriggerParamValue(triggerID, ecaPath, paramPath, value, paramType)
}
export function SetTriggerParamSubFunction(triggerID: number, ecaPath: number[], paramPath: number[], subName: string): Promise<TriggerMutationResult> {
  return app().SetTriggerParamSubFunction(triggerID, ecaPath, paramPath, subName)
}
export function ClearTriggerParamSubFunction(triggerID: number, ecaPath: number[], paramPath: number[]): Promise<TriggerMutationResult> {
  return app().ClearTriggerParamSubFunction(triggerID, ecaPath, paramPath)
}
export function SetTriggerParamArray(triggerID: number, ecaPath: number[], paramPath: number[], isArray: boolean): Promise<TriggerMutationResult> {
  return app().SetTriggerParamArray(triggerID, ecaPath, paramPath, isArray)
}
export function AddTriggerNestedECA(triggerID: number, parentPath: number[], ecaType: number, name: string, groupID: number): Promise<TriggerMutationResult> {
  return app().AddTriggerNestedECA(triggerID, parentPath, ecaType, name, groupID)
}
export function ListTriggerUnitInstances(): Promise<TriggerUnitInstance[]> {
  return app().ListTriggerUnitInstances()
}
export function ListTriggerDestructableInstances(): Promise<TriggerDestructableInstance[]> {
  return app().ListTriggerDestructableInstances()
}
export function ListTriggerRegions(): Promise<TriggerRegionInfo[]> {
  return app().ListTriggerRegions()
}
export function ListTriggerCameras(): Promise<TriggerCameraInfo[]> {
  return app().ListTriggerCameras()
}

// Phase 3 — codegen + search.
export function GenerateTriggerScript(): Promise<TriggerScriptResult> {
  return app().GenerateTriggerScript()
}
export function SaveTriggerScript(): Promise<TriggerScriptResult> {
  return app().SaveTriggerScript()
}
export function TestMap(): Promise<void> {
  return app().TestMap()
}
export function SearchTriggers(query: string, limit: number = 50): Promise<TriggerSearchHit[]> {
  return app().SearchTriggers(query, limit)
}

// Stringify a parameter into a human-readable inline label. Mirrors
// HiveWE's get_parameters_names recursive substitution (trigger_editor.cpp
// L389-476).
//
// For a parameter:
//   - If has_sub_parameter: render `subname(arg1, arg2, …)` recursively
//   - Otherwise: render the literal `value` (strings get quoted)
//
// The caller usually drives this via renderECALabel which walks the
// _Foo_Parameters template's ~ArgN slots.
export function paramLabel(p: TriggerParameter | undefined | null): string {
  if (!p) return ''
  if (p.has_sub_parameter && p.sub_parameter) {
    const sub = p.sub_parameter
    const args = (sub.parameters ?? []).map((sp) => paramLabel(sp))
    return `${sub.name}(${args.join(', ')})`
  }
  if (p.type === 3 /* ParamString */) {
    // String params show with surrounding quotes so the user can tell them
    // apart from preset / variable references. Color codes (|c…|r) inside
    // the value stay intact — the downstream renderer parses the whole
    // labelled string for color tokens, and double-quote chars never appear
    // in a color tag so the parse is unambiguous.
    return JSON.stringify(p.resolved_display || p.value)
  }
  // For non-string params, prefer the resolved display when present (it's
  // the entity's human-readable name) but keep the raw value as the source
  // of truth for any caller that needs canonical bytes.
  return p.resolved_display || p.value || '(unset)'
}

// Render an ECA's display label by walking its parameters_template (from
// TriggerData.txt's _Foo_Parameters key). Tokens starting with "~" are
// parameter slot references — pop the next Parameter and call paramLabel.
// All other tokens are literal text fragments.
//
// Returns the function name if no template is available (the picker shows
// "FunctionName(arg1, arg2)" as a fallback). Magic ECAs (IfThenElseMultiple,
// ForLoopAMultiple, AndMultiple) are usually displayed as "If, then, else"
// at the parent level with their child rows handling the body — the caller
// branches on the function name.
export function renderECALabel(
  eca: TriggerECA,
  template: string[] | undefined,
): string {
  if (!template || template.length === 0) {
    const args = (eca.parameters ?? []).map((p) => paramLabel(p))
    return `${eca.name}(${args.join(', ')})`
  }
  const params = eca.parameters ?? []
  let argIdx = 0
  const parts: string[] = []
  for (const tok of template) {
    if (tok.startsWith('~')) {
      parts.push(paramLabel(params[argIdx]))
      argIdx++
    } else {
      parts.push(tok)
    }
  }
  return parts.join('')
}

// Magic ECAs whose children render under synthetic "If/Then/Else/Loop"
// section headers per HiveWE trigger_editor.cpp L351-365. Used by the
// Trigger Editor's right pane to add the extra labels under each parent
// row's child list. Phase 2a: lift to a richer per-name child-bucket
// renderer when authoring lands.
export const MAGIC_ECAS = new Set<string>([
  'IfThenElse',
  'IfThenElseMultiple',
  'ForLoopA',
  'ForLoopAMultiple',
  'ForLoopB',
  'ForLoopBMultiple',
  'ForLoopVar',
  'ForLoopVarMultiple',
  'AndMultiple',
  'OrMultiple',
])
