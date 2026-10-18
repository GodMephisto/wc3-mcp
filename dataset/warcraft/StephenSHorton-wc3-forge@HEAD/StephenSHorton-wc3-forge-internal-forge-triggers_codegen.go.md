---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_codegen.go"
ref: HEAD
---

// Package forge — Lua codegen for the Trigger Editor (Phase 3).
//
// Port of HiveWE's convert_gui_to_jass (src/base/triggers/gui.cpp) +
// MapScriptWriter (src/base/triggers/triggers.ixx L28-307) +
// generate_map_script (src/base/triggers/map_script.cpp L768-873), reduced to
// LUA ONLY. Per locked decisions: JASS is not supported. Maps whose w3i has
// Lua==false refuse generation with ErrLuaOnly so the user knows to convert
// via World Editor first.
//
// Pipeline (mirrors HiveWE map_script.cpp):
//
//	Phase A — concat each non-comment-enabled trigger's source into one big
//	          string. GUI triggers → convertGUIToLua; script triggers →
//	          custom_text verbatim.
//	Phase B — scrape gg_unit_XXXX_NNNN / gg_dest_XXXX_NNNN refs out of that
//	          string to know which named globals to emit (HiveWE does this
//	          via std::string::find loops; we mirror with a tiny regex).
//	Phase C — emit InitGlobals + CreateAllUnits + CreateAllDestructables +
//	          (the trigger script bag) + InitCustomTriggers +
//	          RunInitializationTriggers + main + config.
//
// The 15 magic ECA names that need per-name lowering (Risk #4 in the
// trigger-editor-analysis.md):
//
//	IfThenElseMultiple, ForLoopAMultiple, ForLoopA, ForLoopBMultiple,
//	ForLoopB, ForLoopVarMultiple, ForLoopVar, AndMultiple, OrMultiple,
//	SetVariable, CommentString, CustomScriptCode, WaitForCondition,
//	OperatorCompare* (Boolean/Integer/Real/String), OperatorInt,
//	OperatorReal, OperatorString, AddTriggerEvent,
//	EnumDestructablesInCircleBJMultiple. (Plus IfThenElse — same family
//	as IfThenElseMultiple but a different shape.)
//
// Anything else falls through to the generic dispatch: emit
// `script_name(arg1, arg2, …)` where script_name is the function's
// `_Foo_ScriptName` companion when present, else the function's own name.
//
// IMPORTANT: this file is pure over its inputs — it takes the parsed wtg +
// custom_text map + units/doodads + TriggerData + w3i and returns a string.
// All file I/O happens in session.go's GenerateAndSaveScript / TestMap path.
package forge

import (
	"errors"
	"fmt"
	"regexp"
	"sort"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/forge/jass2lua"
	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3c"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3r"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// ErrLuaOnly is returned by GenerateLuaScript when the loaded map's w3i says
// the map is JASS-mode. Lua-only is a locked Phase-3 decision; users must
// convert in WC3 World Editor first.
var ErrLuaOnly = errors.New("wc3-forge generates Lua only — convert the map to Lua via WC3 World Editor first (Map → Convert to Lua)")

// PreserveScriptMarker is the Lua-comment guard string that, when present at
// the top of war3map.lua, tells the codegen to refuse regeneration. The
// editor honors it to preserve hand-edited script files. Rename from the C++
// fork's "HIVEWE_PRESERVE_SCRIPT" convention.
const PreserveScriptMarker = "-- WC3FORGE_PRESERVE_SCRIPT"

// ErrPreserveScript is returned when the existing war3map.lua starts with
// PreserveScriptMarker.
var ErrPreserveScript = errors.New("war3map.lua starts with " + PreserveScriptMarker + " — remove the marker to regenerate")

// CodegenInputs bundles the pieces GenerateLuaScript needs. Callers
// typically populate from Session getters; tests build it ad-hoc.
//
// LibInits is the aggregated list of vJASS libraries detected across every
// section (per-trigger custom_text + GlobalJASS + Map Header), already in
// dependency-respecting order. The codegen emits a synthetic
// _vjass_init_libs() function that calls each LibraryInit.InitFunc in
// order, then calls _vjass_init_libs() from main() after globals init but
// before user trigger registration.
//
// StructInits is the aggregated list of vJASS structs that defined a
// `static method onInit` (Phase 3). The codegen appends each
// `Foo.onInit()` call AFTER the library inits inside _vjass_init_libs()
// so structs can use library-provided globals during their own init.
type CodegenInputs struct {
	Triggers    *wtg.Triggers          // war3map.wtg parsed tree
	CustomTexts map[int32]string       // trigger id → custom_text (overrides Trigger.CustomText if both set)
	Units       *unitsdoo.File         // war3mapUnits.doo (placed units / items)
	Doodads     *doodadsdoo.File       // war3map.doo (placed doodads/destructables)
	Terrain     *w3e.File              // war3map.w3e (for CenterOffset → world coords)
	Info        *w3i.Info              // war3map.w3i (player slots, forces, camera bounds, …)
	Regions     *w3r.File              // war3map.w3r (optional)
	Cameras     *w3c.File              // war3map.w3c (optional)
	TD          *wtg.TriggerData       // UI/TriggerData.txt
	GlobalJASS  string                 // war3map.wct GlobalJASS block (concatenated verbatim into the script body, after globals)
	LibInits    []jass2lua.LibraryInit // aggregated vJASS library init order (Phase 2)
	StructInits []jass2lua.StructInit  // aggregated vJASS struct onInit hooks (Phase 3)
}

// GenerateLuaScript is the Phase-3 entry point. Returns the war3map.lua
// source as a single string + a nil error on success. Refuses generation
// when Info.Lua is false (JASS maps must be converted in World Editor
// first).
//
// Output structure matches HiveWE's emit order (map_script.cpp L811-846)
// reduced to Lua syntax:
//
//	-- generated by wc3-forge ...
//	<globals> as direct assignments  (udg_*, gg_rct_*, gg_cam_*, gg_trg_*, gg_unit_*, gg_dest_*)
//	function InitGlobals() ... end
//	function ItemTable_<n>()           -- for each random_item_table  (omitted; no ItemTable mutation surface yet)
//	function UnitItemDrops_<n>() / DoodadItemDrops_<n>() — per-entity item drop tables
//	function InitSounds() ... end       (currently empty — sounds not parsed yet)
//	function CreateAllDestructables() ... end
//	function CreateAllItems() ... end
//	function CreateAllUnits() ... end
//	function CreateRegions() ... end
//	function CreateCameras() ... end
//	<global_jass>
//	<per-trigger script>                — InitTrig_<Trig> + pre-actions + body
//	function InitCustomTriggers() ... end
//	function RunInitializationTriggers() ... end
//	function InitCustomPlayerSlots() ... end
//	function InitCustomTeams() ... end  (only when info.custom_forces)
//	function InitAllyPriorities() ... end
//	function main() ... end
//	function config() ... end
//
// The return value is a single string; the caller writes it (under any name)
// via session's source.write path. Files are not touched here.
func GenerateLuaScript(in CodegenInputs) (string, error) {
	if in.Info != nil && !in.Info.Lua {
		return "", ErrLuaOnly
	}
	if in.Triggers == nil {
		in.Triggers = &wtg.Triggers{}
	}
	if in.TD == nil {
		return "", fmt.Errorf("GenerateLuaScript: TriggerData snapshot is required")
	}

	cg := &luaCodegen{in: in, fnCounter: 0}

	// Phase A — concat every non-comment, enabled trigger's source.
	// HiveWE prefers custom_text when set (so script triggers / hand-edited
	// GUI overrides flow through); we do the same.
	var triggerScript strings.Builder
	var initTriggers []string // var names (gg_trg_Foo) of MapInitializationEvent triggers
	for i := range in.Triggers.Triggers {
		tr := &in.Triggers.Triggers[i]
		if tr.IsComment || !tr.IsEnabled {
			continue
		}
		// custom_text wins (script trigger or user-edited GUI). Empty
		// string means "GUI trigger, lower from ECAs".
		ct := tr.CustomText
		if in.CustomTexts != nil {
			if v, ok := in.CustomTexts[tr.ID]; ok {
				ct = v
			}
		}
		if ct != "" {
			triggerScript.WriteString(ct)
			triggerScript.WriteByte('\n')
			continue
		}
		triggerScript.WriteString(cg.convertTriggerToLua(tr, &initTriggers))
	}

	// Phase B — scrape gg_unit_/gg_dest_ refs out of the concatenated script
	// to know which placed entities need named globals. HiveWE uses
	// std::string::find loops; we use a regex for clarity. Mirror its
	// substring positions exactly (offsets 8 / 13 in the C++ code map to
	// the "XXXX_NNNN" tail after the 8-char "gg_unit_" / "gg_dest_" prefix).
	unitVars, destVars := scrapeNamedRefs(triggerScript.String())

	// Phase C — emit. Bound the upfront capacity loosely; tests + benchmarks
	// show real maps land in the 100k-3M byte range.
	cg.script.Grow(1 << 16)

	// Header comment block — explicit so accidental hand-edits are
	// recognizable.
	cg.script.WriteString("-- generated by wc3-forge\n")
	cg.script.WriteString("-- DO NOT EDIT (use Trigger Editor)\n")
	cg.script.WriteString("-- To preserve hand-edits, prepend a line: " + PreserveScriptMarker + "\n\n")

	cg.emitGlobals(unitVars, destVars)
	cg.emitInitGlobals()
	// Item tables — random_item_tables / per-unit / per-doodad item drops.
	// HiveWE emits these; we mirror so DroppedItem APIs still resolve at
	// runtime. The per-entity drops use unit.item_sets / doodad.item_sets
	// which the parsers expose.
	cg.emitItemTables()
	cg.emitInitSounds() // empty fn — sounds not parsed yet
	cg.emitCreateAllDestructables(destVars)
	cg.emitCreateAllItems()
	cg.emitCreateAllUnits(unitVars)
	cg.emitCreateRegions()
	cg.emitCreateCameras()

	if strings.TrimSpace(in.GlobalJASS) != "" {
		cg.script.WriteString("\n-- global custom-script header (from war3map.wct GlobalJASS)\n")
		cg.script.WriteString(in.GlobalJASS)
		if !strings.HasSuffix(in.GlobalJASS, "\n") {
			cg.script.WriteByte('\n')
		}
		cg.script.WriteByte('\n')
	}

	cg.script.WriteString("\n-- triggers\n")
	cg.script.WriteString(triggerScript.String())

	cg.emitInitCustomTriggers()
	cg.emitRunInitializationTriggers(initTriggers)
	cg.emitInitCustomPlayerSlots()
	cg.emitInitCustomTeams()
	cg.emitInitAllyPriorities()
	cg.emitVJASSInitLibs()
	cg.emitMain()
	cg.emitConfig()

	return cg.script.String(), nil
}

// emitVJASSInitLibs writes a synthetic function that calls each vJASS
// library's initializer in dependency-respecting order, then each vJASS
// struct's `static method onInit` (Phase 3). Called from main()
// (after InitGlobals, before InitCustomTriggers) by emitMain.
//
// Order: libraries first, then structs. Libraries can register globals or
// helpers structs depend on, but structs typically don't define library-
// scope state, so library-before-struct is the safe order. Within each
// group, source order is preserved (libraries' dep-sort already runs in
// the preprocessor; structs aren't dep-sorted — they're appended in
// section-iteration order).
//
// When neither libraries nor structs have init hooks, the function still
// emits (empty body) so the main() call site doesn't need a conditional.
// The cost is one extra nop call per map load — well below noise floor.
func (c *luaCodegen) emitVJASSInitLibs() {
	c.script.WriteString("function _vjass_init_libs()\n")
	// Deduplicate libraries by name in case the same library appears across
	// multiple sections (rare, but textmacros can paste a library declaration
	// into every trigger that runtextmacro's it). The first occurrence in
	// dep order wins; subsequent duplicates are silently dropped.
	seenLib := map[string]bool{}
	for _, lib := range c.in.LibInits {
		if seenLib[lib.Name] {
			continue
		}
		seenLib[lib.Name] = true
		if lib.InitFunc == "" {
			continue
		}
		c.script.WriteString(fmt.Sprintf("\t%s()\n", lib.InitFunc))
	}
	// Struct onInit hooks. Same dedupe story — a struct could appear in
	// multiple sections via textmacro paste.
	seenStruct := map[string]bool{}
	for _, st := range c.in.StructInits {
		if seenStruct[st.StructName] {
			continue
		}
		seenStruct[st.StructName] = true
		if st.InitMethod == "" {
			continue
		}
		c.script.WriteString(fmt.Sprintf("\t%s.%s()\n", st.StructName, st.InitMethod))
	}
	c.script.WriteString("end\n\n")
}

// ---------------------------------------------------------------------------
// luaCodegen — emitter state.
// ---------------------------------------------------------------------------

type luaCodegen struct {
	in         CodegenInputs
	script     strings.Builder
	indent     int
	fnCounter  int
	preActions strings.Builder // active per-trigger pre-action sink (closure-emitted helpers)
}

func (c *luaCodegen) writeIndent(b *strings.Builder) {
	for i := 0; i < c.indent; i++ {
		b.WriteByte('\t')
	}
}

// line writes a single line at the current indent into the active buffer.
func (c *luaCodegen) line(b *strings.Builder, s string) {
	c.writeIndent(b)
	b.WriteString(s)
	b.WriteByte('\n')
}

// fnName mints a Lua-safe per-trigger helper-function name.
// HiveWE: "Trig_<trigger>_HiveWE<counter>".
func (c *luaCodegen) fnName(trigger string) string {
	c.fnCounter++
	return fmt.Sprintf("Trig_%s_Wc3Forge%d", sanitizeIdent(trigger), c.fnCounter-1)
}

// sanitizeIdent makes a trigger name safe for Lua identifier use (spaces →
// underscore, drop everything else non-[A-Za-z0-9_]).
func sanitizeIdent(s string) string {
	var b strings.Builder
	for _, r := range strings.TrimSpace(s) {
		switch {
		case r >= 'a' && r <= 'z', r >= 'A' && r <= 'Z', r >= '0' && r <= '9', r == '_':
			b.WriteRune(r)
		default:
			b.WriteByte('_')
		}
	}
	if b.Len() == 0 {
		return "Anon"
	}
	return b.String()
}

// ---------------------------------------------------------------------------
// scrapeNamedRefs — Phase B. Walks the concatenated trigger script for
// gg_unit_/gg_dest_ refs, returning maps creation_number → FourCC.
// Mirrors HiveWE's std::string::find at map_script.cpp L795-809.
//
// We use a regex rather than substring loops for clarity. The HiveWE
// substring path uses fixed offsets (8 = len("gg_unit_") and 13 = past the
// FourCC + underscore). For destructables creation_number is variable-
// length (1+ digits, no padding); for units it's 4-digit zero-padded.
// Both shapes are accepted here so older + newer maps both round-trip.
// ---------------------------------------------------------------------------

var (
	reUnitRef = regexp.MustCompile(`gg_unit_([A-Za-z0-9_]{4})_(\d+)`)
	reDestRef = regexp.MustCompile(`gg_dest_([A-Za-z0-9_]{4})_(\d+)`)
)

// scrapeNamedRefs returns creation_number → FourCC maps for the unit and
// destructable globals the trigger script references. Numbers are kept as
// strings so we can re-emit them in the same zero-padded form HiveWE used.
func scrapeNamedRefs(script string) (units, dests map[string]string) {
	units = map[string]string{}
	dests = map[string]string{}
	for _, m := range reUnitRef.FindAllStringSubmatch(script, -1) {
		units[m[2]] = m[1]
	}
	for _, m := range reDestRef.FindAllStringSubmatch(script, -1) {
		dests[m[2]] = m[1]
	}
	return units, dests
}

// sortedRefs returns the keys of a refs map in deterministic order so the
// emitted globals block is stable across runs (numeric-aware sort).
func sortedRefs(refs map[string]string) []string {
	keys := make([]string, 0, len(refs))
	for k := range refs {
		keys = append(keys, k)
	}
	sort.Slice(keys, func(i, j int) bool {
		// Numeric compare first (left-pad with zeros so widths align).
		if len(keys[i]) != len(keys[j]) {
			return len(keys[i]) < len(keys[j])
		}
		return keys[i] < keys[j]
	})
	return keys
}

// ---------------------------------------------------------------------------
// Phase C emitters.
// ---------------------------------------------------------------------------

// emitGlobals writes the top-level Lua assignments for udg_ vars, gg_rct_,
// gg_cam_, gg_trg_, gg_unit_, gg_dest_. Lua has no `globals` block —
// statements at file scope assign module-globals directly.
func (c *luaCodegen) emitGlobals(units, dests map[string]string) {
	t := c.in.Triggers
	td := c.in.TD

	c.script.WriteString("-- globals\n")

	// Variables (udg_*). Arrays use the __jarray helper Blizzard ships in
	// blizzard.lua to mimic JASS auto-init.
	for _, v := range t.Variables {
		base := td.BaseType(v.Type)
		_ = base
		if v.IsArray {
			c.script.WriteString(fmt.Sprintf("udg_%s = __jarray(%s)\n", v.Name, luaInitialDefaultForType(td, v.Type)))
		} else {
			c.script.WriteString(fmt.Sprintf("udg_%s = %s\n", v.Name, luaInitialDefaultForType(td, v.Type)))
		}
	}

	// Regions.
	if c.in.Regions != nil {
		for _, r := range c.in.Regions.Regions {
			c.script.WriteString(fmt.Sprintf("gg_rct_%s = nil\n", sanitizeIdent(r.Name)))
		}
	}
	// Cameras.
	if c.in.Cameras != nil {
		for _, cm := range c.in.Cameras.Cameras {
			c.script.WriteString(fmt.Sprintf("gg_cam_%s = nil\n", sanitizeIdent(cm.Name)))
		}
	}
	// Triggers — one gg_trg_<TriggerName> per non-comment, enabled trigger.
	for _, tr := range t.Triggers {
		if tr.IsComment || !tr.IsEnabled {
			continue
		}
		c.script.WriteString(fmt.Sprintf("gg_trg_%s = nil\n", sanitizeIdent(tr.Name)))
	}
	// gg_unit_/gg_dest_ — only the ones the trigger script actually references.
	for _, num := range sortedRefs(units) {
		c.script.WriteString(fmt.Sprintf("gg_unit_%s_%s = nil\n", units[num], num))
	}
	for _, num := range sortedRefs(dests) {
		c.script.WriteString(fmt.Sprintf("gg_dest_%s_%s = nil\n", dests[num], num))
	}
	c.script.WriteByte('\n')
}

// luaInitialDefaultForType returns the Lua expression for a variable's
// initial value. Mirrors HiveWE's generate_global_variables lua branch
// (map_script.cpp L43-51 + L104-146) — generally `nil` for object types,
// `__jarray("")` for arrays, or the TriggerTypeDefaults / TriggerParams
// resolved value when a non-array carries an initial_value.
//
// We don't have access to the variable's initial_value here (called from
// emitGlobals which only walks vars); InitGlobals does the per-var
// assignment with init values. Top-level globals just need to exist as
// non-nil-when-table / nil-otherwise.
func luaInitialDefaultForType(td *wtg.TriggerData, typeName string) string {
	base := td.BaseType(typeName)
	switch base {
	case "integer", "real":
		return "0"
	case "string":
		return `""`
	case "boolean":
		return "false"
	default:
		return "nil"
	}
}

// emitInitGlobals writes the InitGlobals function — per-variable
// assignments using TriggerParams presets for the JASS literal of any
// initial_value, falling back to TriggerTypeDefaults otherwise. Array
// variables loop over array_size and set each slot.
func (c *luaCodegen) emitInitGlobals() {
	td := c.in.TD
	t := c.in.Triggers
	c.script.WriteString("function InitGlobals()\n")
	c.script.WriteString("\tlocal i = 0\n")
	for _, v := range t.Variables {
		base := td.BaseType(v.Type)
		// Resolve the value expression. Three cases:
		//   1. variable is_initialized — use TriggerParams[v.InitialValue]
		//      column 2 if present, else interpret the raw text per base type
		//   2. has a default in [TriggerTypeDefaults] for the base type
		//   3. nothing initialized & no default — skip (matches HiveWE)
		var valueExpr string
		hasInit := false
		if v.IsInitialized && v.InitialValue != "" {
			hasInit = true
			if presetRows, ok := td.Sections["TriggerParams"]; ok {
				if toks, ok := presetRows[v.InitialValue]; ok && len(toks) >= 3 {
					valueExpr = toks[2]
				}
			}
			if valueExpr == "" {
				if base == "string" {
					valueExpr = `"` + escapeLuaString(v.InitialValue) + `"`
				} else {
					valueExpr = v.InitialValue
				}
			}
		}
		if !hasInit {
			if defs, ok := td.Sections["TriggerTypeDefaults"]; ok {
				if toks, ok := defs[base]; ok && len(toks) > 0 && toks[0] != "" {
					valueExpr = toks[0]
				}
			}
			if valueExpr == "" {
				continue
			}
		}
		if v.IsArray {
			size := v.ArraySize
			if size <= 0 {
				size = 1
			}
			c.script.WriteString(fmt.Sprintf("\tfor i = 0, %d do\n", size))
			c.script.WriteString(fmt.Sprintf("\t\tudg_%s[i] = %s\n", v.Name, valueExpr))
			c.script.WriteString("\tend\n")
		} else {
			c.script.WriteString(fmt.Sprintf("\tudg_%s = %s\n", v.Name, valueExpr))
		}
	}
	c.script.WriteString("end\n\n")
}

// escapeLuaString escapes a string literal for inclusion in Lua source
// (backslashes + double quotes).
func escapeLuaString(s string) string {
	r := strings.NewReplacer(`\`, `\\`, `"`, `\"`, "\n", `\n`, "\r", `\r`)
	return r.Replace(s)
}

// emitCreateAllUnits writes the placed-unit creation. unitVars is the
// scrape-derived (creation_number → FourCC) map so we know which unit IDs
// need a named global vs an anonymous local.
//
// Mirrors HiveWE's generate_units (map_script.cpp L149-255), reduced to
// Lua: BlzCreateUnitWithSkin, then SetUnitState life/mana, hero level/stats,
// acquire range, abilities (SelectHeroSkill + initial order), inventory
// (UnitAddItemToSlotById), and item-drop trigger wiring.
func (c *luaCodegen) emitCreateAllUnits(unitVars map[string]string) {
	c.script.WriteString("function CreateAllUnits()\n")
	c.script.WriteString("\tlocal u\n\tlocal unitID\n\tlocal t\n\tlocal life\n")
	if c.in.Units == nil || c.in.Terrain == nil {
		c.script.WriteString("end\n\n")
		return
	}
	offset := c.in.Terrain.CenterOffset
	for _, e := range c.in.Units.Entities {
		if e.TypeID == "sloc" {
			continue
		}
		ref := "u"
		num := fmt.Sprintf("%04d", e.CreationNumber)
		if _, ok := unitVars[num]; ok {
			ref = fmt.Sprintf("gg_unit_%s_%s", e.TypeID, num)
		}
		c.script.WriteString(fmt.Sprintf(
			"\t%s = BlzCreateUnitWithSkin(Player(%d), FourCC(%q), %.4f, %.4f, %.4f, FourCC(%q))\n",
			ref, e.Player,
			e.TypeID,
			e.Position[0]*128.0+offset[0],
			e.Position[1]*128.0+offset[1],
			radToDeg(e.Rotation),
			pickSkinID(e),
		))
		if e.HitPointsPct != -1 {
			c.script.WriteString(fmt.Sprintf("\tlife = GetUnitState(%s, UNIT_STATE_LIFE)\n", ref))
			c.script.WriteString(fmt.Sprintf("\tSetUnitState(%s, UNIT_STATE_LIFE, %f * life)\n", ref, float32(e.HitPointsPct)/100.0))
		}
		if e.ManaPct != -1 {
			c.script.WriteString(fmt.Sprintf("\tSetUnitState(%s, UNIT_STATE_MANA, %d)\n", ref, e.ManaPct))
		}
		if e.HeroLevel > 1 {
			c.script.WriteString(fmt.Sprintf("\tSetHeroLevel(%s, %d, false)\n", ref, e.HeroLevel))
		}
		if e.HeroStr != 0 {
			c.script.WriteString(fmt.Sprintf("\tSetHeroStr(%s, %d, true)\n", ref, e.HeroStr))
		}
		if e.HeroAgi != 0 {
			c.script.WriteString(fmt.Sprintf("\tSetHeroAgi(%s, %d, true)\n", ref, e.HeroAgi))
		}
		if e.HeroInt != 0 {
			c.script.WriteString(fmt.Sprintf("\tSetHeroInt(%s, %d, true)\n", ref, e.HeroInt))
		}
		if e.TargetAcquisition != -1 {
			r := e.TargetAcquisition
			if r == -2 {
				r = 200
			}
			c.script.WriteString(fmt.Sprintf("\tSetUnitAcquireRange(%s, %f)\n", ref, r))
		}
		for _, j := range e.AbilityModifications {
			// Each skill add — Level field on AbilityMod records "times learned"
			// for hero skills. We don't have abilities.slk piped in for v1
			// codegen, so we skip the orderon/orderoff toggle and just emit
			// SelectHeroSkill per level. Non-hero ability autocast is a TODO.
			for k := uint32(0); k < j.Level; k++ {
				c.script.WriteString(fmt.Sprintf("\tSelectHeroSkill(%s, FourCC(%q))\n", ref, j.AbilityID))
			}
		}
		for _, item := range e.Inventory {
			c.script.WriteString(fmt.Sprintf("\tUnitAddItemToSlotById(%s, FourCC(%q), %d)\n", ref, item.ItemID, item.Slot))
		}
		if len(e.ItemDrops) > 0 {
			c.script.WriteString("\tt = CreateTrigger()\n")
			c.script.WriteString(fmt.Sprintf("\tTriggerRegisterUnitEvent(t, %s, EVENT_UNIT_DEATH)\n", ref))
			c.script.WriteString(fmt.Sprintf("\tTriggerRegisterUnitEvent(t, %s, EVENT_UNIT_CHANGE_OWNER)\n", ref))
			c.script.WriteString(fmt.Sprintf("\tTriggerAddAction(t, UnitItemDrops_%d)\n", e.CreationNumber))
		}
	}
	c.script.WriteString("end\n\n")
}

// pickSkinID returns the FourCC HiveWE emits as the BlzCreateUnitWithSkin
// skin argument: SkinID when non-empty, else TypeID.
func pickSkinID(e unitsdoo.Entity) string {
	if e.SkinID != "" {
		return e.SkinID
	}
	return e.TypeID
}

// pickDoodadSkinID is the doodad analogue of pickSkinID.
func pickDoodadSkinID(d doodadsdoo.Doodad) string {
	if d.SkinID != "" {
		return d.SkinID
	}
	return d.TypeID
}

func radToDeg(r float32) float32 {
	return r * (180.0 / 3.14159265358979)
}

// emitCreateAllItems writes BlzCreateItemWithSkin calls for the placed
// items in war3mapUnits.doo. Items live in the same Entities slice but the
// HiveWE codegen path puts them in their own CreateAllItems function (so
// the load order can be controlled). We don't have a Units.items vs
// Units.entities split today — items are stored on the same Entities slice
// (classifier via TypeID lookup against items.slk). Without a SLK ref
// handy, we skip the items-only emit; HiveWE's main() calls CreateAllItems
// + CreateAllUnits separately but the runtime tolerates an empty
// CreateAllItems.
func (c *luaCodegen) emitCreateAllItems() {
	c.script.WriteString("function CreateAllItems()\nend\n\n")
}

// emitCreateAllDestructables writes the placed-doodad creation. destVars
// tells us which doodad creation_numbers the script references by name.
// Doodads without item drops AND not referenced get skipped entirely —
// matching HiveWE's L289-291 behavior (anonymous, no drops, no script
// reference = nothing to emit).
func (c *luaCodegen) emitCreateAllDestructables(destVars map[string]string) {
	c.script.WriteString("function CreateAllDestructables()\n")
	c.script.WriteString("\tlocal d\n\tlocal t\n\tlocal life\n")
	if c.in.Doodads == nil || c.in.Terrain == nil {
		c.script.WriteString("end\n\n")
		return
	}
	offset := c.in.Terrain.CenterOffset
	for _, d := range c.in.Doodads.Doodads {
		id := "d"
		num := fmt.Sprintf("%d", d.CreationNumber)
		if _, ok := destVars[num]; ok {
			id = fmt.Sprintf("gg_dest_%s_%s", d.TypeID, num)
		}
		if id == "d" && len(d.ItemDrops) == 0 && d.MapItemTable == -1 {
			continue
		}
		c.script.WriteString(fmt.Sprintf(
			"\t%s = BlzCreateDestructableZWithSkin(FourCC(%q), %.4f, %.4f, %.4f, %.4f, %f, %d, FourCC(%q))\n",
			id, d.TypeID,
			d.Position[0]*128.0+offset[0],
			d.Position[1]*128.0+offset[1],
			d.Position[2]*128.0,
			radToDeg(d.Rotation),
			d.Scale[0],
			d.Variation,
			pickDoodadSkinID(d),
		))
		if d.Life != 100 {
			c.script.WriteString(fmt.Sprintf("\tlife = GetDestructableLife(%s)\n", id))
			c.script.WriteString(fmt.Sprintf("\tSetDestructableLife(%s, %f * life)\n", id, float32(d.Life)/100.0))
		}
		if len(d.ItemDrops) > 0 {
			c.script.WriteString("\tt = CreateTrigger()\n")
			c.script.WriteString(fmt.Sprintf("\tTriggerRegisterDeathEvent(t, %s)\n", id))
			c.script.WriteString("\tTriggerAddAction(t, SaveDyingWidget)\n")
			c.script.WriteString(fmt.Sprintf("\tTriggerAddAction(t, DoodadItemDrops_%d)\n", d.CreationNumber))
		} else if d.MapItemTable != -1 {
			c.script.WriteString("\tt = CreateTrigger()\n")
			c.script.WriteString(fmt.Sprintf("\tTriggerRegisterDeathEvent(t, %s)\n", id))
			c.script.WriteString("\tTriggerAddAction(t, SaveDyingWidget)\n")
			c.script.WriteString(fmt.Sprintf("\tTriggerAddAction(t, ItemTable_%d)\n", d.MapItemTable))
		}
	}
	c.script.WriteString("end\n\n")
}

// emitCreateRegions writes a CreateRegions function that constructs each
// gg_rct_<Name> via Rect(...) and (optionally) AddWeatherEffect.
func (c *luaCodegen) emitCreateRegions() {
	c.script.WriteString("function CreateRegions()\n")
	c.script.WriteString("\tlocal we\n")
	if c.in.Regions == nil {
		c.script.WriteString("end\n\n")
		return
	}
	for _, r := range c.in.Regions.Regions {
		name := sanitizeIdent(r.Name)
		l, rt := r.Left, r.Right
		if l > rt {
			l, rt = rt, l
		}
		b, tp := r.Bottom, r.Top
		if b > tp {
			b, tp = tp, b
		}
		c.script.WriteString(fmt.Sprintf("\tgg_rct_%s = Rect(%f, %f, %f, %f)\n", name, l, b, rt, tp))
		if r.WeatherID != "" && r.WeatherID != "\x00\x00\x00\x00" {
			c.script.WriteString(fmt.Sprintf("\twe = AddWeatherEffect(gg_rct_%s, FourCC(%q))\n", name, r.WeatherID))
			c.script.WriteString("\tEnableWeatherEffect(we, true)\n")
		}
	}
	c.script.WriteString("end\n\n")
}

// emitCreateCameras writes a CreateCameras function for each gg_cam_<Name>
// — CreateCameraSetup + CameraSetupSetField series + CameraSetupSetDestPosition.
func (c *luaCodegen) emitCreateCameras() {
	c.script.WriteString("function CreateCameras()\n")
	if c.in.Cameras == nil {
		c.script.WriteString("end\n\n")
		return
	}
	for _, cm := range c.in.Cameras.Cameras {
		name := sanitizeIdent(cm.Name)
		c.script.WriteString(fmt.Sprintf("\tgg_cam_%s = CreateCameraSetup()\n", name))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_ZOFFSET, %f, 0.0)\n", name, cm.ZOffset))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_ROTATION, %f, 0.0)\n", name, cm.Rotation))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_ANGLE_OF_ATTACK, %f, 0.0)\n", name, cm.AngleOfAttack))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_TARGET_DISTANCE, %f, 0.0)\n", name, cm.Distance))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_ROLL, %f, 0.0)\n", name, cm.Roll))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_FIELD_OF_VIEW, %f, 0.0)\n", name, cm.FOV))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_FARZ, %f, 0.0)\n", name, cm.FarZ))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetField(gg_cam_%s, CAMERA_FIELD_NEARZ, %f, 0.0)\n", name, cm.NearZ))
		c.script.WriteString(fmt.Sprintf("\tCameraSetupSetDestPosition(gg_cam_%s, %f, %f, 0.0)\n", name, cm.TargetX, cm.TargetY))
	}
	c.script.WriteString("end\n\n")
}

// emitInitSounds — placeholder. wc3-forge doesn't parse war3map.w3s yet so
// the sound table is empty; we emit the function so InitSounds() doesn't
// blow up at runtime.
func (c *luaCodegen) emitInitSounds() {
	c.script.WriteString("function InitSounds()\nend\n\n")
}

// emitItemTables — per-entity item-drop tables (UnitItemDrops_<n>,
// DoodadItemDrops_<n>) + map-level ItemTable_<n>. Mirrors HiveWE's
// generate_item_tables (map_script.cpp L538-583), reduced to Lua.
//
// For Phase 3 we emit the per-entity drops only when the entity has
// item_sets parsed. The map-level random_item_tables come from
// info.RandomItemTables.
func (c *luaCodegen) emitItemTables() {
	// Map-level ItemTable_<n>.
	if c.in.Info != nil {
		for _, t := range c.in.Info.RandomItemTables {
			c.script.WriteString(fmt.Sprintf("function ItemTable_%d()\n", t.CreationNumber))
			c.script.WriteString("\tlocal trigWidget = nil\n\tlocal trigUnit = nil\n\tlocal itemID = 0\n\tlocal canDrop = true\n")
			c.script.WriteString("\ttrigWidget = bj_lastDyingWidget\n")
			c.script.WriteString("\tif (trigWidget == nil) then\n")
			c.script.WriteString("\t\ttrigUnit = GetTriggerUnit()\n")
			c.script.WriteString("\tend\n")
			c.script.WriteString("\tif not(trigUnit == nil) then\n")
			c.script.WriteString("\t\tcanDrop = not IsUnitHidden(trigUnit)\n")
			c.script.WriteString("\t\tif (canDrop and not(GetChangingUnit() == nil)) then\n")
			c.script.WriteString("\t\t\tcanDrop = (GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE))\n")
			c.script.WriteString("\t\tend\n")
			c.script.WriteString("\tend\n")
			c.script.WriteString("\tif (canDrop) then\n")
			for _, set := range t.ItemSets {
				c.script.WriteString("\t\tRandomDistReset()\n")
				for _, item := range set.Items {
					emitItemTableEntry(&c.script, "\t\t", int(item.Chance), item.ID)
				}
				c.script.WriteString("\t\titemID = RandomDistChoose()\n")
				c.script.WriteString("\t\tif not(trigUnit == nil) then\n\t\t\tUnitDropItem(trigUnit, itemID)\n\t\tend\n")
				c.script.WriteString("\t\tif (trigUnit == nil) then\n\t\t\tWidgetDropItem(trigWidget, itemID)\n\t\tend\n")
			}
			c.script.WriteString("\tend\n")
			c.script.WriteString("\tbj_lastDyingWidget = nil\n")
			c.script.WriteString("\tDestroyTrigger(GetTriggeringTrigger())\n")
			c.script.WriteString("end\n\n")
		}
	}
	// Per-unit UnitItemDrops_<n>. unitsdoo flattens item-drop sets into a
	// single []ItemDrop on Parse — we treat the whole list as one set (the
	// per-set boundaries live in unexported itemDropSetSizes; HiveWE's
	// emit shape uses sets, but for v1 codegen one-set-per-entity is a
	// defensible default that still produces runnable scripts).
	if c.in.Units != nil {
		for _, e := range c.in.Units.Entities {
			if len(e.ItemDrops) == 0 {
				continue
			}
			c.script.WriteString(fmt.Sprintf("function UnitItemDrops_%d()\n", e.CreationNumber))
			c.script.WriteString("\tlocal trigWidget = nil\n\tlocal trigUnit = nil\n\tlocal itemID = 0\n\tlocal canDrop = true\n")
			c.script.WriteString("\ttrigWidget = bj_lastDyingWidget\n")
			c.script.WriteString("\tif (trigWidget == nil) then\n\t\ttrigUnit = GetTriggerUnit()\n\tend\n")
			c.script.WriteString("\tif (canDrop) then\n")
			c.script.WriteString("\t\tRandomDistReset()\n")
			for _, item := range e.ItemDrops {
				emitItemTableEntry(&c.script, "\t\t", int(item.Chance), item.ItemID)
			}
			c.script.WriteString("\t\titemID = RandomDistChoose()\n")
			c.script.WriteString("\t\tif not(trigUnit == nil) then\n\t\t\tUnitDropItem(trigUnit, itemID)\n\t\tend\n")
			c.script.WriteString("\tend\n")
			c.script.WriteString("\tbj_lastDyingWidget = nil\n")
			c.script.WriteString("\tDestroyTrigger(GetTriggeringTrigger())\n")
			c.script.WriteString("end\n\n")
		}
	}
	// Per-doodad DoodadItemDrops_<n>.
	if c.in.Doodads != nil {
		for _, d := range c.in.Doodads.Doodads {
			if len(d.ItemDrops) == 0 {
				continue
			}
			c.script.WriteString(fmt.Sprintf("function DoodadItemDrops_%d()\n", d.CreationNumber))
			c.script.WriteString("\tlocal trigWidget = nil\n\tlocal itemID = 0\n")
			c.script.WriteString("\ttrigWidget = bj_lastDyingWidget\n")
			c.script.WriteString("\tRandomDistReset()\n")
			for _, item := range d.ItemDrops {
				emitItemTableEntry(&c.script, "\t", int(item.Chance), item.ItemID)
			}
			c.script.WriteString("\titemID = RandomDistChoose()\n")
			c.script.WriteString("\tWidgetDropItem(trigWidget, itemID)\n")
			c.script.WriteString("\tbj_lastDyingWidget = nil\n")
			c.script.WriteString("\tDestroyTrigger(GetTriggeringTrigger())\n")
			c.script.WriteString("end\n\n")
		}
	}
}

// emitItemTableEntry writes one RandomDistAddItem call. The id may be a
// random-item placeholder ("YYI/" etc.) — HiveWE has a special branch that
// translates Y[ijklmnoY]I/ into a ChooseRandomItemEx call.
func emitItemTableEntry(b *strings.Builder, indent string, chance int, id string) {
	if id == "" {
		b.WriteString(fmt.Sprintf("%sRandomDistAddItem(-1, %d)\n", indent, chance))
		return
	}
	if len(id) == 4 && id[0] == 'Y' && id[2] == 'I' &&
		((id[1] >= 'i' && id[1] <= 'o') || id[1] == 'Y') {
		var itemType string
		switch id[1] {
		case 'i':
			itemType = "PERMANENT"
		case 'j':
			itemType = "CHARGED"
		case 'k':
			itemType = "POWERUP"
		case 'l':
			itemType = "ARTIFACT"
		case 'm':
			itemType = "PURCHASABLE"
		case 'n':
			itemType = "CAMPAIGN"
		case 'o':
			itemType = "MISCELLANEOUS"
		case 'Y':
			itemType = "ANY"
		}
		level := "-1"
		if id[3] != '/' {
			level = string(id[3])
		}
		b.WriteString(fmt.Sprintf("%sRandomDistAddItem(ChooseRandomItemEx(ITEM_TYPE_%s, %s), %d)\n", indent, itemType, level, chance))
		return
	}
	b.WriteString(fmt.Sprintf("%sRandomDistAddItem(FourCC(%q), %d)\n", indent, id, chance))
}

// emitInitCustomTriggers walks every non-comment-enabled trigger and emits
// a call to its InitTrig_<name> stub. (The stubs themselves are emitted by
// convertTriggerToLua as part of Phase A.)
func (c *luaCodegen) emitInitCustomTriggers() {
	c.script.WriteString("function InitCustomTriggers()\n")
	for _, tr := range c.in.Triggers.Triggers {
		if tr.IsComment || !tr.IsEnabled {
			continue
		}
		c.script.WriteString(fmt.Sprintf("\tInitTrig_%s()\n", sanitizeIdent(tr.Name)))
	}
	c.script.WriteString("end\n\n")
}

// emitRunInitializationTriggers fires each MapInitializationEvent-bound
// trigger via ConditionalTriggerExecute, in the same order they declared
// themselves. initTriggers is the list of var-names accumulated by Phase A.
func (c *luaCodegen) emitRunInitializationTriggers(initTriggers []string) {
	c.script.WriteString("function RunInitializationTriggers()\n")
	for _, n := range initTriggers {
		c.script.WriteString(fmt.Sprintf("\tConditionalTriggerExecute(%s)\n", n))
	}
	c.script.WriteString("end\n\n")
}

// emitInitCustomPlayerSlots writes the per-player slot init from w3i.
func (c *luaCodegen) emitInitCustomPlayerSlots() {
	c.script.WriteString("function InitCustomPlayerSlots()\n")
	if c.in.Info == nil {
		c.script.WriteString("end\n\n")
		return
	}
	playerType := []string{"MAP_CONTROL_USER", "MAP_CONTROL_COMPUTER", "MAP_CONTROL_NEUTRAL", "MAP_CONTROL_RESCUABLE"}
	races := []string{"RACE_PREF_RANDOM", "RACE_PREF_HUMAN", "RACE_PREF_ORC", "RACE_PREF_UNDEAD", "RACE_PREF_NIGHTELF"}
	for idx, p := range c.in.Info.Players {
		pn := int(p.InternalNumber)
		typ := int(p.Type)
		race := int(p.Race)
		if typ < 0 || typ >= len(playerType) {
			typ = 0
		}
		if race < 0 || race >= len(races) {
			race = 0
		}
		c.script.WriteString(fmt.Sprintf("\tSetPlayerStartLocation(Player(%d), %d)\n", pn, idx))
		if p.FixedStartPosition != 0 || race == 0 {
			c.script.WriteString(fmt.Sprintf("\tForcePlayerStartLocation(Player(%d), %d)\n", pn, idx))
		}
		c.script.WriteString(fmt.Sprintf("\tSetPlayerColor(Player(%d), ConvertPlayerColor(%d))\n", pn, pn))
		c.script.WriteString(fmt.Sprintf("\tSetPlayerRacePreference(Player(%d), %s)\n", pn, races[race]))
		c.script.WriteString(fmt.Sprintf("\tSetPlayerRaceSelectable(Player(%d), true)\n", pn))
		c.script.WriteString(fmt.Sprintf("\tSetPlayerController(Player(%d), %s)\n", pn, playerType[typ]))
	}
	c.script.WriteString("end\n\n")
}

// emitInitCustomTeams walks Forces from w3i and applies player-team /
// alliance / vision / control sharing per force flags.
func (c *luaCodegen) emitInitCustomTeams() {
	c.script.WriteString("function InitCustomTeams()\n")
	if c.in.Info == nil {
		c.script.WriteString("end\n\n")
		return
	}
	for i, f := range c.in.Info.Forces {
		// Find players in the force via PlayerMasks bit
		for _, p := range c.in.Info.Players {
			if f.PlayerMasks&(1<<uint(p.InternalNumber)) != 0 {
				c.script.WriteString(fmt.Sprintf("\tSetPlayerTeam(Player(%d), %d)\n", p.InternalNumber, i))
				if f.Flags.AlliedVictory {
					c.script.WriteString(fmt.Sprintf("\tSetPlayerState(Player(%d), PLAYER_STATE_ALLIED_VICTORY, 1)\n", p.InternalNumber))
				}
			}
		}
		for _, j := range c.in.Info.Players {
			if f.PlayerMasks&(1<<uint(j.InternalNumber)) == 0 {
				continue
			}
			for _, k := range c.in.Info.Players {
				if f.PlayerMasks&(1<<uint(k.InternalNumber)) == 0 || j.InternalNumber == k.InternalNumber {
					continue
				}
				if f.Flags.Allied {
					c.script.WriteString(fmt.Sprintf("\tSetPlayerAllianceStateAllyBJ(Player(%d), Player(%d), true)\n", j.InternalNumber, k.InternalNumber))
				}
				if f.Flags.ShareVision {
					c.script.WriteString(fmt.Sprintf("\tSetPlayerAllianceStateVisionBJ(Player(%d), Player(%d), true)\n", j.InternalNumber, k.InternalNumber))
				}
				if f.Flags.ShareUnitControl {
					c.script.WriteString(fmt.Sprintf("\tSetPlayerAllianceStateControlBJ(Player(%d), Player(%d), true)\n", j.InternalNumber, k.InternalNumber))
				}
				if f.Flags.ShareAdvancedUnitControl {
					c.script.WriteString(fmt.Sprintf("\tSetPlayerAllianceStateFullControlBJ(Player(%d), Player(%d), true)\n", j.InternalNumber, k.InternalNumber))
				}
			}
		}
	}
	c.script.WriteString("end\n\n")
}

// emitInitAllyPriorities — simplified port of HiveWE's
// generate_ally_priorities. We emit the function shell even when there are
// no priorities configured so the runtime stays happy.
func (c *luaCodegen) emitInitAllyPriorities() {
	c.script.WriteString("function InitAllyPriorities()\n")
	if c.in.Info == nil {
		c.script.WriteString("end\n\n")
		return
	}
	playerToStartLoc := map[int32]int{}
	current := 0
	for _, p := range c.in.Info.Players {
		playerToStartLoc[p.InternalNumber] = current
		current++
	}
	current = 0
	for _, p := range c.in.Info.Players {
		count := 0
		for _, j := range c.in.Info.Players {
			if p.InternalNumber == j.InternalNumber {
				continue
			}
			if p.AllyLowPriorities&(1<<uint(j.InternalNumber)) != 0 || p.AllyHighPriorities&(1<<uint(j.InternalNumber)) != 0 {
				count++
			}
		}
		c.script.WriteString(fmt.Sprintf("\tSetStartLocPrioCount(%d, %d)\n", current, count))
		idx := 0
		for _, j := range c.in.Info.Players {
			if p.InternalNumber == j.InternalNumber {
				continue
			}
			if p.AllyLowPriorities&(1<<uint(j.InternalNumber)) != 0 {
				c.script.WriteString(fmt.Sprintf("\tSetStartLocPrio(%d, %d, %d, MAP_LOC_PRIO_LOW)\n", current, idx, playerToStartLoc[j.InternalNumber]))
				idx++
			} else if p.AllyHighPriorities&(1<<uint(j.InternalNumber)) != 0 {
				c.script.WriteString(fmt.Sprintf("\tSetStartLocPrio(%d, %d, %d, MAP_LOC_PRIO_HIGH)\n", current, idx, playerToStartLoc[j.InternalNumber]))
				idx++
			}
		}
		current++
	}
	c.script.WriteString("end\n\n")
}

// emitMain writes the main() function: camera bounds, day/night models,
// sound environments, then the init-sequence calls. Most of the per-tileset
// path data (DayAmbience etc.) lives in WorldEditData which we don't
// embed, so we use sane defaults matching HiveWE's empty-string behavior.
func (c *luaCodegen) emitMain() {
	c.script.WriteString("function main()\n")
	if c.in.Info != nil {
		i := c.in.Info
		c.script.WriteString(fmt.Sprintf(
			"\tSetCameraBounds(%f + GetCameraMargin(CAMERA_MARGIN_LEFT), %f + GetCameraMargin(CAMERA_MARGIN_BOTTOM), %f - GetCameraMargin(CAMERA_MARGIN_RIGHT), %f - GetCameraMargin(CAMERA_MARGIN_TOP), %f + GetCameraMargin(CAMERA_MARGIN_LEFT), %f - GetCameraMargin(CAMERA_MARGIN_TOP), %f - GetCameraMargin(CAMERA_MARGIN_RIGHT), %f + GetCameraMargin(CAMERA_MARGIN_BOTTOM))\n",
			i.CameraLeftBottom.X-512, i.CameraLeftBottom.Y-256,
			i.CameraRightTop.X+512, i.CameraRightTop.Y+256,
			i.CameraLeftTop.X-512, i.CameraLeftTop.Y+256,
			i.CameraRightBottom.X+512, i.CameraRightBottom.Y-256,
		))
	}
	c.script.WriteString("\tSetDayNightModels(\"\", \"\")\n")
	c.script.WriteString("\tNewSoundEnvironment(\"\")\n")
	c.script.WriteString("\tSetAmbientDaySound(\"\")\n")
	c.script.WriteString("\tSetAmbientNightSound(\"\")\n")
	c.script.WriteString("\tSetMapMusic(\"Music\", true, 0)\n")
	c.script.WriteString("\tInitSounds()\n")
	c.script.WriteString("\tCreateRegions()\n")
	c.script.WriteString("\tCreateCameras()\n")
	c.script.WriteString("\tCreateAllDestructables()\n")
	c.script.WriteString("\tCreateAllItems()\n")
	c.script.WriteString("\tCreateAllUnits()\n")
	c.script.WriteString("\tInitBlizzard()\n")
	c.script.WriteString("\tInitGlobals()\n")
	// vJASS library initializers run after InitGlobals (so they can see
	// udg_* / gg_* globals) but BEFORE InitCustomTriggers (so triggers can
	// reference library-provided functions).
	c.script.WriteString("\t_vjass_init_libs()\n")
	c.script.WriteString("\tInitCustomTriggers()\n")
	c.script.WriteString("\tRunInitializationTriggers()\n")
	c.script.WriteString("end\n\n")
}

// emitConfig writes the config() function — engine-side metadata + start
// locations + slot init.
func (c *luaCodegen) emitConfig() {
	c.script.WriteString("function config()\n")
	if c.in.Info != nil {
		c.script.WriteString(fmt.Sprintf("\tSetMapName(%q)\n", c.in.Info.Name))
		c.script.WriteString(fmt.Sprintf("\tSetMapDescription(%q)\n", c.in.Info.Description))
		c.script.WriteString(fmt.Sprintf("\tSetPlayers(%d)\n", len(c.in.Info.Players)))
		c.script.WriteString(fmt.Sprintf("\tSetTeams(%d)\n", len(c.in.Info.Forces)))
		c.script.WriteString("\tSetGamePlacement(MAP_PLACEMENT_USE_MAP_SETTINGS)\n\n")
	}
	if c.in.Units != nil && c.in.Terrain != nil {
		offset := c.in.Terrain.CenterOffset
		for _, e := range c.in.Units.Entities {
			if e.TypeID == "sloc" {
				c.script.WriteString(fmt.Sprintf(
					"\tDefineStartLocation(%d, %f, %f)\n",
					e.Player, e.Position[0]*128+offset[0], e.Position[1]*128+offset[1],
				))
			}
		}
	}
	c.script.WriteString("\n\tInitCustomPlayerSlots()\n")
	if c.in.Info != nil && c.in.Info.Flags.CustomForces {
		c.script.WriteString("\tInitCustomTeams()\n")
	} else if c.in.Info != nil {
		for _, p := range c.in.Info.Players {
			c.script.WriteString(fmt.Sprintf("\tSetPlayerSlotAvailable(Player(%d), MAP_CONTROL_USER)\n", p.InternalNumber))
		}
		c.script.WriteString("\tInitGenericPlayerSlots()\n")
	}
	c.script.WriteString("\tInitAllyPriorities()\n")
	c.script.WriteString("end\n")
}

// ---------------------------------------------------------------------------
// convertTriggerToLua + convertECAToLua + convertParameterToLua — the 15
// magic ECAs land in convertECAToLua.
// ---------------------------------------------------------------------------

// convertTriggerToLua emits one trigger's worth of script: pre-actions,
// optional condition fn, action fn, and the InitTrig_<Name> stub. Mirrors
// HiveWE's convert_gui_to_jass (gui.cpp L415-515).
//
// initTriggers accumulates trigger-variable names for triggers bound to
// MapInitializationEvent — those get a ConditionalTriggerExecute in
// RunInitializationTriggers().
func (c *luaCodegen) convertTriggerToLua(tr *wtg.Trigger, initTriggers *[]string) string {
	name := sanitizeIdent(tr.Name)
	triggerVar := "gg_trg_" + name
	actionName := "Trig_" + name + "_Actions"
	condName := "Trig_" + name + "_Conditions"

	// Reset the per-trigger pre-action sink.
	c.preActions.Reset()

	var events strings.Builder
	var conditions strings.Builder
	var actions strings.Builder

	hasConditions := false
	for _, eca := range tr.ECAs {
		if !eca.Enabled {
			continue
		}
		switch eca.Type {
		case wtg.ECAEvent:
			if eca.Name == "MapInitializationEvent" {
				*initTriggers = append(*initTriggers, triggerVar)
				continue
			}
			// args = resolved params (skip the first ~Arg "trigger" slot —
			// not present in the wtg; first ECA arg is the trigger handle).
			var args []string
			for k, p := range eca.Parameters {
				typ := c.typeFor(eca.Name, k)
				val, _ := c.resolveParameter(&p, tr.Name, typ, false)
				if typ == "VarAsString_Real" {
					val = `"` + val + `"`
				}
				args = append(args, val)
			}
			argList := strings.Join(args, ", ")
			if argList != "" {
				events.WriteString(fmt.Sprintf("\t%s(%s, %s)\n", eca.Name, triggerVar, argList))
			} else {
				events.WriteString(fmt.Sprintf("\t%s(%s)\n", eca.Name, triggerVar))
			}
		case wtg.ECACondition:
			hasConditions = true
			cond, _ := c.convertECAToLua(&eca, tr.Name, false)
			conditions.WriteString(fmt.Sprintf("\tif (not (%s)) then\n\t\treturn false\n\tend\n", cond))
		case wtg.ECAAction:
			a, _ := c.convertECAToLua(&eca, tr.Name, true)
			actions.WriteString("\t" + a + "\n")
		}
	}

	var out strings.Builder
	out.WriteString(c.preActions.String())
	if hasConditions {
		out.WriteString(fmt.Sprintf("function %s()\n", condName))
		out.WriteString(conditions.String())
		out.WriteString("\treturn true\nend\n\n")
	}
	out.WriteString(fmt.Sprintf("function %s()\n", actionName))
	out.WriteString(actions.String())
	out.WriteString("end\n\n")
	out.WriteString(fmt.Sprintf("function InitTrig_%s()\n", name))
	out.WriteString(fmt.Sprintf("\t%s = CreateTrigger()\n", triggerVar))
	out.WriteString(events.String())
	if hasConditions {
		out.WriteString(fmt.Sprintf("\tTriggerAddCondition(%s, Condition(%s))\n", triggerVar, condName))
	}
	if !tr.InitiallyOn {
		out.WriteString(fmt.Sprintf("\tDisableTrigger(%s)\n", triggerVar))
	}
	out.WriteString(fmt.Sprintf("\tTriggerAddAction(%s, %s)\n", triggerVar, actionName))
	out.WriteString("end\n\n")
	return out.String()
}

// typeFor returns the declared parameter type for the k-th slot of
// function `name`. Mirrors HiveWE's Triggers::get_type (gui.cpp L5-18).
func (c *luaCodegen) typeFor(name string, k int) string {
	td := c.in.TD
	if rows, ok := td.Sections["TriggerActions"]; ok {
		if toks, ok := rows[name]; ok && 1+k < len(toks) {
			return toks[1+k]
		}
	}
	if rows, ok := td.Sections["TriggerCalls"]; ok {
		if toks, ok := rows[name]; ok && 3+k < len(toks) {
			return toks[3+k]
		}
	}
	if rows, ok := td.Sections["TriggerEvents"]; ok {
		if toks, ok := rows[name]; ok && 1+k < len(toks) {
			return toks[1+k]
		}
	}
	if rows, ok := td.Sections["TriggerConditions"]; ok {
		if toks, ok := rows[name]; ok && 1+k < len(toks) {
			return toks[1+k]
		}
	}
	return ""
}

// scriptNameFor returns the function's _Foo_ScriptName companion if
// present, otherwise its own name. Used so functions whose script name
// differs from the GUI name emit the correct call.
func (c *luaCodegen) scriptNameFor(name string) string {
	for _, sect := range []string{"TriggerActions", "TriggerCalls", "TriggerEvents", "TriggerConditions"} {
		if rows, ok := c.in.TD.Sections[sect]; ok {
			if toks, ok := rows["_"+name+"_ScriptName"]; ok && len(toks) > 0 {
				return toks[0]
			}
		}
	}
	return name
}

// codeFourCC code paths require a string with a base type of "string" to
// be wrapped in quotes; everything else passes through. Mirrors HiveWE's
// get_base_type chain.
func (c *luaCodegen) baseTypeOf(typeName string) string {
	if c.in.TD == nil {
		return typeName
	}
	return c.in.TD.BaseType(typeName)
}

// isFourCCType reports whether the given type is one of the FourCC code
// types HiveWE wraps in FourCC("...") at emit time. Cross-references
// gui.cpp L43-52 + L77-82 exhaustive list.
func isFourCCType(t string) bool {
	switch t {
	case "abilcode", "buffcode", "destructablecode", "itemcode", "ordercode",
		"techcode", "unitcode", "heroskillcode", "weathereffectcode",
		"timedlifebuffcode", "doodadcode", "terraintype":
		return true
	}
	return false
}

// resolveParameter is the port of HiveWE's resolve_parameter (gui.cpp L25-89).
// Returns the Lua expression for one parameter slot.
func (c *luaCodegen) resolveParameter(p *wtg.Parameter, triggerName, expectedType string, addCall bool) (string, error) {
	switch p.Type {
	case wtg.ParamInvalid:
		return "", fmt.Errorf("invalid parameter type in trigger %q", triggerName)
	case wtg.ParamPreset:
		// Look up [TriggerParams][value] — col 1 is its declared type, col
		// 2 is the JASS literal to inline. Strings need backtick → " sub.
		presetRows := c.in.TD.Sections["TriggerParams"]
		if presetRows == nil {
			return "", nil
		}
		toks := presetRows[p.Value]
		if len(toks) < 3 {
			return "", nil
		}
		presetType := toks[1]
		literal := toks[2]
		if c.baseTypeOf(presetType) == "string" {
			return strings.ReplaceAll(literal, "`", `"`), nil
		}
		// FourCC types: strip the surrounding ' and wrap in FourCC("...").
		if presetType == "timedlifebuffcode" || isFourCCType(expectedType) {
			lit := strings.ReplaceAll(literal, "'", "")
			return fmt.Sprintf(`FourCC(%q)`, lit), nil
		}
		return literal, nil
	case wtg.ParamFunction:
		if p.HasSubParameter && p.SubParameter != nil {
			s, err := c.convertECAToLua(p.SubParameter, triggerName, addCall)
			if err != nil {
				return "", err
			}
			return s, nil
		}
		return p.Value + "()", nil
	case wtg.ParamVariable:
		out := p.Value
		if !strings.HasPrefix(out, "gg_") {
			out = "udg_" + out
		}
		if p.IsArray && p.ArrayIndex != nil {
			ix, err := c.resolveParameter(p.ArrayIndex, triggerName, "integer", false)
			if err != nil {
				return "", err
			}
			out += "[" + ix + "]"
		}
		return out, nil
	case wtg.ParamString:
		base := c.baseTypeOf(expectedType)
		if base == "string" {
			// HiveWE: only escape backslashes, not double-quotes. Quoted via
			// `"` directly.
			return `"` + strings.ReplaceAll(p.Value, `\`, `\\`) + `"`, nil
		}
		if isFourCCType(expectedType) {
			return fmt.Sprintf(`FourCC(%q)`, p.Value), nil
		}
		return p.Value, nil
	}
	return "", nil
}

// convertECAToLua lowers one ECA to a Lua expression-or-statement (the
// add_call flag distinguishes between contexts that want a trailing
// statement-form call vs an expression-form value).
//
// This function is the big switch over the 15 magic ECA names from Risk
// #4. The generic fall-through handles every other function.
//
// Each magic case directly mirrors HiveWE's gui.cpp branches L102-413.
// Names and corner cases (group=1 for then-branch, OperatorString uses ..
// in Lua, AndMultiple uses an extracted condition function, etc.) ported
// verbatim so future agents diffing against HiveWE for bug reports see a
// 1:1 correspondence.
func (c *luaCodegen) convertECAToLua(eca *wtg.ECA, triggerName string, addCall bool) (string, error) {
	// HiveWE's gui.cpp L98-100 gates ALL ECAs on enabled. wtg.Parse sets
	// sub-parameter ECAs to Enabled=true automatically; only user-disabled
	// top-level rows skip emission.
	if !eca.Enabled {
		return "", nil
	}

	// Magic ECA #1: IfThenElse — three-param if/then/else with sub-function
	// sources.
	if eca.Name == "IfThenElse" {
		fn := c.fnName(triggerName)
		condStr, _ := c.resolveParameter(&eca.Parameters[0], triggerName, c.typeFor(eca.Name, 0), false)
		thenStr, _ := c.resolveParameter(&eca.Parameters[1], triggerName, c.typeFor(eca.Name, 1), true)
		elseStr, _ := c.resolveParameter(&eca.Parameters[2], triggerName, c.typeFor(eca.Name, 2), true)
		fmt.Fprintf(&c.preActions, "function %s()\n\treturn %s\nend\n\n", fn, condStr)
		var b strings.Builder
		fmt.Fprintf(&b, "if (%s()) then\n\t%s\nelse\n\t%s\nend", fn, thenStr, elseStr)
		return b.String(), nil
	}

	// Magic ECA #2: IfThenElseMultiple — multi-child if/then/else (the
	// GUI's most common conditional). children carry type=condition for
	// the condition list, type=action for then/else (group=1 → then, else
	// → else).
	if eca.Name == "IfThenElseMultiple" {
		fn := c.fnName(triggerName)
		var conds []string
		for _, child := range eca.Children {
			if child.Type != wtg.ECACondition {
				continue
			}
			s, _ := c.convertECAToLua(&child, triggerName, false)
			conds = append(conds, s)
		}
		fmt.Fprintf(&c.preActions, "function %s()\n", fn)
		for _, cnd := range conds {
			fmt.Fprintf(&c.preActions, "\tif (not (%s)) then\n\t\treturn false\n\tend\n", cnd)
		}
		fmt.Fprintf(&c.preActions, "\treturn true\nend\n\n")
		var b strings.Builder
		fmt.Fprintf(&b, "if (%s()) then\n", fn)
		for _, child := range eca.Children {
			if child.Type != wtg.ECAAction {
				continue
			}
			if child.Group == 1 {
				s, _ := c.convertECAToLua(&child, triggerName, true)
				fmt.Fprintf(&b, "\t%s\n", s)
			}
		}
		fmt.Fprintf(&b, "else\n")
		for _, child := range eca.Children {
			if child.Type != wtg.ECAAction {
				continue
			}
			if child.Group != 1 {
				s, _ := c.convertECAToLua(&child, triggerName, true)
				fmt.Fprintf(&b, "\t%s\n", s)
			}
		}
		fmt.Fprintf(&b, "end")
		return b.String(), nil
	}

	// Magic ECAs #3-#6: ForLoopA / ForLoopAMultiple / ForLoopB /
	// ForLoopBMultiple. variable name is bj_forLoopAIndex / B.
	if strings.HasPrefix(eca.Name, "ForLoopA") || strings.HasPrefix(eca.Name, "ForLoopB") {
		loopVar := "bj_forLoopAIndex"
		loopEnd := "bj_forLoopAIndexEnd"
		if strings.HasPrefix(eca.Name, "ForLoopB") {
			loopVar = "bj_forLoopBIndex"
			loopEnd = "bj_forLoopBIndexEnd"
		}
		startExpr, _ := c.resolveParameter(&eca.Parameters[0], triggerName, c.typeFor(eca.Name, 0), false)
		endExpr, _ := c.resolveParameter(&eca.Parameters[1], triggerName, c.typeFor(eca.Name, 1), false)
		var b strings.Builder
		fmt.Fprintf(&b, "%s = %s\n%s = %s\nwhile (%s <= %s) do\n",
			loopVar, startExpr, loopEnd, endExpr, loopVar, loopEnd)
		if strings.HasSuffix(eca.Name, "Multiple") {
			for _, child := range eca.Children {
				s, _ := c.convertECAToLua(&child, triggerName, true)
				fmt.Fprintf(&b, "\t%s\n", s)
			}
		} else {
			body, _ := c.resolveParameter(&eca.Parameters[2], triggerName, c.typeFor(eca.Name, 2), true)
			fmt.Fprintf(&b, "\t%s\n", body)
		}
		fmt.Fprintf(&b, "\t%s = %s + 1\nend", loopVar, loopVar)
		return b.String(), nil
	}

	// Magic ECAs #7-#8: ForLoopVar / ForLoopVarMultiple.
	if eca.Name == "ForLoopVar" || eca.Name == "ForLoopVarMultiple" {
		varExpr, _ := c.resolveParameter(&eca.Parameters[0], triggerName, "integer", false)
		startExpr, _ := c.resolveParameter(&eca.Parameters[1], triggerName, c.typeFor(eca.Name, 1), false)
		endExpr, _ := c.resolveParameter(&eca.Parameters[2], triggerName, c.typeFor(eca.Name, 2), false)
		var b strings.Builder
		fmt.Fprintf(&b, "%s = %s\nwhile (%s <= %s) do\n", varExpr, startExpr, varExpr, endExpr)
		if eca.Name == "ForLoopVarMultiple" {
			for _, child := range eca.Children {
				s, _ := c.convertECAToLua(&child, triggerName, true)
				fmt.Fprintf(&b, "\t%s\n", s)
			}
		} else {
			body, _ := c.resolveParameter(&eca.Parameters[3], triggerName, c.typeFor(eca.Name, 3), true)
			fmt.Fprintf(&b, "\t%s\n", body)
		}
		fmt.Fprintf(&b, "\t%s = %s + 1\nend", varExpr, varExpr)
		return b.String(), nil
	}

	// Magic ECAs #9-#10: AndMultiple / OrMultiple — extract a closure that
	// computes the boolean composition, then emit a call to it.
	if eca.Name == "AndMultiple" || eca.Name == "OrMultiple" {
		fn := c.fnName(triggerName)
		var conds []string
		for _, child := range eca.Children {
			s, _ := c.convertECAToLua(&child, triggerName, false)
			conds = append(conds, s)
		}
		fmt.Fprintf(&c.preActions, "function %s()\n", fn)
		if eca.Name == "AndMultiple" {
			for _, cnd := range conds {
				fmt.Fprintf(&c.preActions, "\tif (not(%s)) then\n\t\treturn false\n\tend\n", cnd)
			}
			fmt.Fprintf(&c.preActions, "\treturn true\nend\n\n")
		} else {
			for _, cnd := range conds {
				fmt.Fprintf(&c.preActions, "\tif (%s) then\n\t\treturn true\n\tend\n", cnd)
			}
			fmt.Fprintf(&c.preActions, "\treturn false\nend\n\n")
		}
		return fn + "()", nil
	}

	// Magic ECA #11: SetVariable — assignment. The variable's type is
	// looked up against the variables table so the value side resolves
	// against the right base type.
	if eca.Name == "SetVariable" {
		var varType string
		for _, v := range c.in.Triggers.Variables {
			if v.Name == eca.Parameters[0].Value {
				varType = v.Type
				break
			}
		}
		lhs, _ := c.resolveParameter(&eca.Parameters[0], triggerName, "", false)
		rhs, _ := c.resolveParameter(&eca.Parameters[1], triggerName, varType, false)
		return fmt.Sprintf("%s = %s", lhs, rhs), nil
	}

	// Magic ECA #12: CommentString — emit nothing usable; HiveWE puts a
	// "-- <comment>" line in Lua but we elide it (matches the codegen
	// pipeline's "comments don't ship to runtime" intent). Returning empty
	// drops the row from output.
	if eca.Name == "CommentString" {
		body, _ := c.resolveParameter(&eca.Parameters[0], triggerName, "", false)
		return "-- " + strings.TrimSpace(body), nil
	}

	// Magic ECA #13: CustomScriptCode — inline the raw script value
	// verbatim, no escaping.
	if eca.Name == "CustomScriptCode" {
		v, _ := c.resolveParameter(&eca.Parameters[0], triggerName, "", false)
		// resolveParameter wraps strings in quotes; CustomScriptCode wants
		// the raw value. Strip the surrounding quote pair we just added.
		if strings.HasPrefix(v, `"`) && strings.HasSuffix(v, `"`) {
			v = strings.TrimSuffix(strings.TrimPrefix(v, `"`), `"`)
			// Unescape the simple backslash escapes resolveParameter inserted.
			v = strings.ReplaceAll(v, `\\`, `\`)
		}
		return v, nil
	}

	// Resolve all parameter slots up front for the remaining branches.
	resolved := make([]string, len(eca.Parameters))
	for i := range eca.Parameters {
		typ := c.typeFor(eca.Name, i)
		v, _ := c.resolveParameter(&eca.Parameters[i], triggerName, typ, false)
		resolved[i] = v
	}

	// Magic ECA #14: any other *Multiple — closure-wrapped body via the
	// generated function call.
	if strings.HasSuffix(eca.Name, "Multiple") {
		scriptName := c.scriptNameFor(eca.Name)
		// Strip the leading "_" if present from companion-key lookup.
		if strings.HasPrefix(scriptName, "_"+eca.Name+"_") {
			scriptName = scriptName[len("_"+eca.Name+"_"):]
		}
		fn := c.fnName(triggerName)
		fmt.Fprintf(&c.preActions, "function %s()\n", fn)
		for _, child := range eca.Children {
			s, _ := c.convertECAToLua(&child, triggerName, true)
			fmt.Fprintf(&c.preActions, "\t%s\n", s)
		}
		fmt.Fprintf(&c.preActions, "end\n\n")
		if eca.Name == "EnumDestructablesInCircleBJMultiple" {
			return fmt.Sprintf("%s(%s, %s, %s)", scriptName, resolved[0], resolved[1], fn), nil
		}
		return fmt.Sprintf("%s(%s, %s)", scriptName, resolved[0], fn), nil
	}

	// Magic ECA #15: WaitForCondition — while loop with a 0.1s tick.
	if eca.Name == "WaitForCondition" {
		return fmt.Sprintf("while (not (%s)) do\n\tTriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, %s))\nend", resolved[0], resolved[1]), nil
	}

	// Magic ECA #16-#18: OperatorCompare* / OperatorInt / OperatorReal —
	// emit "<lhs> <op> <rhs>" with `!=` → `~=` in Lua.
	if strings.HasPrefix(eca.Name, "OperatorCompare") || eca.Name == "OperatorInt" || eca.Name == "OperatorReal" {
		op := resolved[1]
		if op == "!=" {
			op = "~="
		}
		return fmt.Sprintf("%s %s %s", resolved[0], op, resolved[2]), nil
	}

	// Magic ECA #19: OperatorString — Lua uses .. for string concat.
	if eca.Name == "OperatorString" {
		return fmt.Sprintf("(%s .. %s)", resolved[0], resolved[1]), nil
	}

	// Magic ECA #20: AddTriggerEvent — injects param[0] (the trigger var)
	// inside the call-list of param[1] (an event-bind function call).
	if eca.Name == "AddTriggerEvent" {
		_ = addCall
		// resolved[1] should look like "FuncName(args, ...)". Insert
		// param[0] as the first argument.
		s := resolved[1]
		idx := strings.IndexByte(s, '(')
		if idx == -1 {
			return resolved[1], nil
		}
		return s[:idx+1] + resolved[0] + ", " + s[idx+1:], nil
	}

	// Generic fall-through. Replace boolexpr/boolcall/code slots with a
	// closure indirection — HiveWE generates per-slot helpers; we do the
	// same so callbacks land at the right call-site.
	var args strings.Builder
	for k := range eca.Parameters {
		t := c.typeFor(eca.Name, k)
		switch t {
		case "boolexpr":
			fn := c.fnName(triggerName)
			fmt.Fprintf(&c.preActions, "function %s()\n\treturn %s\nend\n\n", fn, resolved[k])
			args.WriteString(fn)
		case "boolcall":
			fn := c.fnName(triggerName)
			fmt.Fprintf(&c.preActions, "function %s()\n\treturn %s\nend\n\n", fn, resolved[k])
			args.WriteString(fn + "()")
		case "code":
			fn := c.fnName(triggerName)
			code, _ := c.resolveParameter(&eca.Parameters[k], triggerName, t, true)
			fmt.Fprintf(&c.preActions, "function %s()\n\t%s\nend\n\n", fn, code)
			args.WriteString(fn)
		default:
			args.WriteString(resolved[k])
		}
		if k < len(eca.Parameters)-1 {
			args.WriteString(", ")
		}
	}
	scriptName := c.scriptNameFor(eca.Name)
	return scriptName + "(" + args.String() + ")", nil
}
