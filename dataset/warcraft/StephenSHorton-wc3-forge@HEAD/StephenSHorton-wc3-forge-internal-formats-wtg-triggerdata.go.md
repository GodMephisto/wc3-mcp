---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/triggerdata.go"
ref: HEAD
---

package wtg

import (
	"bufio"
	"bytes"
	"strconv"
	"strings"
)

// TriggerData is the parsed in-memory view of UI/TriggerData.txt and
// UI/TriggerStrings.txt. The wtg parser uses the ArgumentCounts table for
// ECA / Parameter parsing — argc per function name doesn't live in the .wtg
// binary, only in this txt file (the load-bearing fact behind the embedded
// snapshot strategy).
//
// The Sections map keeps the raw row-per-section view so callers (the MCP
// surface, future code generator) can introspect every key without us having
// to grow the type per kind. The companion `_<name>_<key>` keys live in the
// SAME section as their function — TriggerData.txt's convention is one section
// per function family (Events / Conditions / Actions / Calls / Params / …),
// and the per-function metadata sits inline at point-of-definition.
type TriggerData struct {
	// Sections is the raw "[Section]" → {key: comma-separated-values} map.
	// Both function definitions ("CreateUnit") AND their metadata companions
	// ("_CreateUnit_DisplayName") share the section they were defined in;
	// callers branch on the leading underscore.
	//
	// Values are the COMMA-separated tokens of the right-hand side, with
	// quoted strings unwrapped. Empty trailing values are preserved so the
	// _Foo_Parameters template's "~Arg," " literal" sequences survive
	// intact. (The split is comma-aware but ignores commas inside double-
	// quoted strings.)
	Sections map[string]map[string][]string

	// SectionOrder preserves the order sections appeared in the source file
	// so callers that iterate by section get a deterministic walk (used by
	// debug dumps + by registerObjectKind-style enumeration if it ever lands).
	SectionOrder []string

	// ArgumentCounts is name → argc derived per HiveWE's algorithm
	// (count non-empty / non-numeric / non-"nothing" tokens; subtract 1 from
	// [TriggerCalls] because col 1 there is the return type).
	//
	// MUST be populated for EVERY function the wtg parser might encounter —
	// missing key triggers a hard error in Parse rather than silent argc=0
	// (a silent zero would misalign every subsequent ECA in the file; see
	// the report's Risk #3).
	ArgumentCounts map[string]int

	// TriggerStrings is the parsed UI/TriggerStrings.txt — function-name →
	// hint text mapping for hover tooltips. Optional in 1a (the read-only
	// viewer doesn't surface tooltips yet); load_version_31 doesn't depend
	// on it. Kept here so the Phase 1b cross-ref work doesn't need a second
	// loader.
	TriggerStrings map[string]string
}

// ParseTriggerData decodes UI/TriggerData.txt into a *TriggerData with the
// ArgumentCounts table populated per HiveWE's algorithm
// (Triggers::load in triggers.ixx L546-573). Returns an error only on a true
// I/O / scan failure — unknown sections / malformed lines are silently
// tolerated, matching the World Editor's permissive behavior.
func ParseTriggerData(data []byte) (*TriggerData, error) {
	td := &TriggerData{
		Sections:       map[string]map[string][]string{},
		ArgumentCounts: map[string]int{},
		TriggerStrings: map[string]string{},
	}
	if err := td.loadInto(data); err != nil {
		return nil, err
	}
	td.computeArgumentCounts()
	return td, nil
}

// LoadTriggerStrings parses UI/TriggerStrings.txt and folds the function-name
// → hint mapping into td.TriggerStrings. Sections in that file are
// [TriggerEventStrings] / [TriggerConditionStrings] / [TriggerActionStrings] /
// [TriggerCallStrings]; the leaf key is either `FunctionName=Hint` or
// `FunctionNameHint="..."`. We collapse the trailing `Hint` suffix variant
// onto the function name so a single lookup serves both.
func (td *TriggerData) LoadTriggerStrings(data []byte) error {
	// Reuse the same INI scanner; the file shape is identical (sections + k=v).
	raw := map[string]map[string][]string{}
	tmp := &TriggerData{Sections: raw, ArgumentCounts: map[string]int{}, TriggerStrings: map[string]string{}}
	if err := tmp.loadInto(data); err != nil {
		return err
	}
	for _, section := range raw {
		for k, v := range section {
			if len(v) == 0 {
				continue
			}
			// The hint shape is a single string value (no comma-split needed
			// from the caller's perspective); rejoin if the parser split it.
			value := joinValues(v)
			// Collapse "FooHint" → "Foo" so a single lookup serves either
			// shape. Functions don't end in "Hint" so the suffix is safe to
			// drop unconditionally.
			name := strings.TrimSuffix(k, "Hint")
			td.TriggerStrings[name] = value
		}
	}
	return nil
}

// loadInto is the shared INI-style scanner. Section detection is `[name]`;
// k=v lines are split on the first `=`. Quoted values keep the quotes off.
// Comments (`//`) and blank lines are skipped.
func (td *TriggerData) loadInto(data []byte) error {
	// UTF-8 BOM strip — WC3 INIs ship one on the first line.
	data = bytes.TrimPrefix(data, []byte{0xEF, 0xBB, 0xBF})

	sc := bufio.NewScanner(bytes.NewReader(data))
	sc.Buffer(make([]byte, 0, 64*1024), 2*1024*1024)

	var current map[string][]string
	for sc.Scan() {
		line := strings.TrimSpace(sc.Text())
		if line == "" {
			continue
		}
		if strings.HasPrefix(line, "//") || strings.HasPrefix(line, ";") {
			continue
		}
		if strings.HasPrefix(line, "[") && strings.HasSuffix(line, "]") {
			name := strings.TrimSpace(line[1 : len(line)-1])
			if name == "" {
				current = nil
				continue
			}
			if existing, ok := td.Sections[name]; ok {
				current = existing
			} else {
				current = map[string][]string{}
				td.Sections[name] = current
				td.SectionOrder = append(td.SectionOrder, name)
			}
			continue
		}
		if current == nil {
			continue
		}
		eq := strings.IndexByte(line, '=')
		if eq < 0 {
			continue
		}
		key := strings.TrimSpace(line[:eq])
		raw := line[eq+1:]
		// Strip trailing inline comments — TriggerData.txt has a few
		// `Foo=1,2 // note` lines. The split is naive (no quoted-`//`
		// support), but the file in practice doesn't put `//` inside
		// values.
		if i := strings.Index(raw, "//"); i >= 0 {
			raw = raw[:i]
		}
		raw = strings.TrimSpace(raw)
		current[key] = splitCSV(raw)
	}
	return sc.Err()
}

// computeArgumentCounts mirrors HiveWE's Triggers::load L556-572: walk every
// [TriggerActions] / [TriggerEvents] / [TriggerConditions] / [TriggerCalls]
// entry whose key doesn't start with `_`, count non-empty / non-numeric /
// non-"nothing" tokens. Subtract 1 from [TriggerCalls] because col 1 there is
// the return type.
//
// CRITICAL: this must match HiveWE's count exactly or the wtg parser will
// silently misalign on real maps. The per-section short-circuit is the only
// asymmetry; everything else is one shared filter.
func (td *TriggerData) computeArgumentCounts() {
	for _, sect := range []string{"TriggerActions", "TriggerEvents", "TriggerConditions", "TriggerCalls"} {
		rows, ok := td.Sections[sect]
		if !ok {
			continue
		}
		for key, tokens := range rows {
			if len(key) > 0 && key[0] == '_' {
				continue // companion meta-key, not a function definition
			}
			count := 0
			for _, t := range tokens {
				if t == "" {
					continue
				}
				if t == "nothing" {
					continue
				}
				if isNumber(t) {
					continue
				}
				count++
			}
			if sect == "TriggerCalls" {
				count-- // the return type column was counted; HiveWE drops it
			}
			if count < 0 {
				count = 0
			}
			td.ArgumentCounts[key] = count
		}
	}
}

// isNumber reports whether s parses as an integer OR a float literal —
// matches HiveWE's is_number helper, which accepts both forms (TriggerData
// argc columns hold version numbers like "0" or "1" but also limits like
// "0.5"). The strconv path is fine for the ~10k call sites in production.
func isNumber(s string) bool {
	if s == "" {
		return false
	}
	if _, err := strconv.ParseFloat(s, 64); err == nil {
		return true
	}
	return false
}

// splitCSV breaks a TriggerData.txt right-hand side into its
// comma-separated tokens, respecting double-quoted strings (quotes
// stripped from the returned value). Empty trailing tokens are preserved
// — they're load-bearing for _Foo_Parameters templates whose final
// segment is a literal string after a placeholder.
//
// The format is permissive: `"a, b",c` → ["a, b", "c"]; `"`-escapes are
// not supported (TriggerData.txt doesn't use them).
func splitCSV(s string) []string {
	if s == "" {
		return nil
	}
	var out []string
	var buf strings.Builder
	inQuote := false
	for i := 0; i < len(s); i++ {
		c := s[i]
		switch {
		case c == '"':
			inQuote = !inQuote
			// Drop the quote character itself; HiveWE's parser does the same
			// (the std::string value never contains the surrounding quotes).
		case c == ',' && !inQuote:
			out = append(out, strings.TrimSpace(buf.String()))
			buf.Reset()
		default:
			buf.WriteByte(c)
		}
	}
	out = append(out, strings.TrimSpace(buf.String()))
	return out
}

// joinValues rejoins a previously-split token list into the original CSV
// representation for callers that only want the right-hand-side as one
// string (TriggerStrings hints, for example).
func joinValues(v []string) string {
	return strings.Join(v, ",")
}

// Argc returns the argument count for the given function name, or (0, false)
// if the name is not present in the loaded TriggerData. Callers that must
// distinguish "no such function" from "function with zero args" should branch
// on the boolean — the wtg parser does.
func (td *TriggerData) Argc(name string) (int, bool) {
	n, ok := td.ArgumentCounts[name]
	return n, ok
}

// HintFor returns the English hover-tooltip text for the given function name,
// or "" if no hint is loaded for that name. Hints come from
// UI/TriggerStrings.txt — see LoadTriggerStrings. The frontend's Trigger
// Editor surfaces this via native title="" tooltips on each ECA row.
//
// Callers can branch on the empty-string return to skip rendering tooltips
// for functions that don't have a hint (custom user-defined functions never
// do; many WC3 stock functions have one).
func (td *TriggerData) HintFor(name string) string {
	if td == nil {
		return ""
	}
	return td.TriggerStrings[name]
}

// ---------------------------------------------------------------------------
// Phase 2b1 — accessors used by the ECA-authoring path (function picker,
// per-param default seeding, preset dropdowns, type-alias resolution).
//
// Each lookup walks Sections; cost is a per-key map probe across the four
// function families. Callers that hit these in hot loops should memoize.
// ---------------------------------------------------------------------------

// FunctionSection returns the section name in which `name` is defined
// ("TriggerEvents" | "TriggerConditions" | "TriggerActions" | "TriggerCalls"),
// or "" if `name` isn't a known function. Used by the picker + the ECA
// mutator's ECAType validation (a TriggerActions function can't be added as
// an event, etc.).
func (td *TriggerData) FunctionSection(name string) string {
	if td == nil || name == "" {
		return ""
	}
	for _, sect := range []string{"TriggerEvents", "TriggerConditions", "TriggerActions", "TriggerCalls"} {
		rows, ok := td.Sections[sect]
		if !ok {
			continue
		}
		if _, ok := rows[name]; ok {
			return sect
		}
	}
	return ""
}

// FunctionRow returns the raw tokens for the function-definition row of
// `name` (the right-hand-side of `name=…`), or nil if the function isn't
// known. The row layout depends on the section:
//
//   - TriggerEvents/Conditions/Actions: [since_version, argType0, argType1, …]
//   - TriggerCalls: [since_version, event_flag, return_type, argType0, argType1, …]
//
// Companion rows (`_name_*`) are NOT returned here; use FunctionCompanion.
func (td *TriggerData) FunctionRow(name string) []string {
	if td == nil || name == "" {
		return nil
	}
	for _, sect := range []string{"TriggerEvents", "TriggerConditions", "TriggerActions", "TriggerCalls"} {
		rows, ok := td.Sections[sect]
		if !ok {
			continue
		}
		if v, ok := rows[name]; ok {
			return v
		}
	}
	return nil
}

// FunctionCompanion returns the value tokens for the `_name_<key>` companion
// row in the same section as `name`. Returns nil if either the function or
// the companion key isn't present. Common keys: "DisplayName", "Parameters",
// "Defaults", "Limits", "Category", "ScriptName".
func (td *TriggerData) FunctionCompanion(name, key string) []string {
	if td == nil || name == "" || key == "" {
		return nil
	}
	sect := td.FunctionSection(name)
	if sect == "" {
		return nil
	}
	rows, ok := td.Sections[sect]
	if !ok {
		return nil
	}
	return rows["_"+name+"_"+key]
}

// FunctionArgTypes returns the ordered argument-type list for `name`,
// filtering out the leading version + (for TriggerCalls) event-flag +
// return-type prefix columns AND any "nothing" / numeric tokens HiveWE's
// argc counter rejects. Result length equals argc[name] in steady state.
//
// Returns nil for unknown function names.
func (td *TriggerData) FunctionArgTypes(name string) []string {
	row := td.FunctionRow(name)
	if row == nil {
		return nil
	}
	sect := td.FunctionSection(name)
	// Skip the prefix columns per section.
	var rest []string
	switch sect {
	case "TriggerCalls":
		if len(row) < 3 {
			return nil
		}
		rest = row[3:]
	default:
		if len(row) < 1 {
			return nil
		}
		rest = row[1:]
	}
	out := make([]string, 0, len(rest))
	for _, t := range rest {
		if t == "" || t == "nothing" {
			continue
		}
		if isNumber(t) {
			continue
		}
		out = append(out, t)
	}
	return out
}

// FunctionReturnType returns the return-type cell for TriggerCalls entries,
// or "" for any other section / unknown name.
func (td *TriggerData) FunctionReturnType(name string) string {
	if td.FunctionSection(name) != "TriggerCalls" {
		return ""
	}
	row := td.FunctionRow(name)
	if len(row) < 3 {
		return ""
	}
	return row[2]
}

// FunctionDefaults returns the _name_Defaults companion as a per-slot string
// slice (length matches argc[name]). Empty string per slot when the row's
// entry is "_" (HiveWE's "leave the param unset" sentinel) or when the
// companion is missing entirely.
//
// Used to seed new ECAs' Parameter.Value at AddECA time so the user sees
// the same starting state HiveWE shows.
func (td *TriggerData) FunctionDefaults(name string) []string {
	n, _ := td.Argc(name)
	out := make([]string, n)
	row := td.FunctionCompanion(name, "Defaults")
	for i := 0; i < n && i < len(row); i++ {
		v := row[i]
		if v == "_" {
			v = ""
		}
		out[i] = v
	}
	return out
}

// FunctionLimits returns the _name_Limits companion split into two-token
// groups per arg (min, max). For unbounded slots the cell is "_". Returns
// nil when no Limits companion exists.
func (td *TriggerData) FunctionLimits(name string) []string {
	return td.FunctionCompanion(name, "Limits")
}

// FunctionCategory returns the _name_Category companion (single token like
// "TC_DESTRUCT"), or "" when missing.
func (td *TriggerData) FunctionCategory(name string) string {
	v := td.FunctionCompanion(name, "Category")
	if len(v) == 0 {
		return ""
	}
	return v[0]
}

// FunctionDisplayName returns the _name_DisplayName companion as a single
// joined string, or "" when missing. Display name doubles as the picker's
// human-readable label.
func (td *TriggerData) FunctionDisplayName(name string) string {
	v := td.FunctionCompanion(name, "DisplayName")
	if len(v) == 0 {
		return ""
	}
	return strings.Join(v, ",")
}

// FunctionParametersTemplate returns the _name_Parameters template tokens
// (literal segments interleaved with ~ArgN slot placeholders), or nil when
// missing. Same data the frontend's renderECALabel already consumes.
func (td *TriggerData) FunctionParametersTemplate(name string) []string {
	return td.FunctionCompanion(name, "Parameters")
}

// Preset is one row from [TriggerParams]. Value is the JASS literal (col 2);
// DisplayName is the editor-facing label (col 3; typically a WESTRING_*
// token that downstream WTS resolution will fold into a localized string).
type Preset struct {
	Name        string // the key (e.g. "Player00")
	Type        string // the type token (col 1) — what this preset is a value FOR
	Value       string // the JASS literal (col 2)
	DisplayName string // the editor label (col 3, often a WESTRING_*)
}

// PresetsForType returns every [TriggerParams] row whose value-type matches
// `typeName`. Used by the ParamEditor's dropdown for player / playercolor /
// gameSpeed / etc. — anything the user picks from a finite preset list.
//
// Type aliasing: a preset whose Type is `playercolor` matches a `playercolor`
// param exactly. The caller is responsible for walking the BaseType chain
// when the param type is a custom alias (e.g. `unitcode` → `integer`) AND
// they want both `unitcode` and `integer` presets to show — typically the
// caller filters by the most-specific type only.
func (td *TriggerData) PresetsForType(typeName string) []Preset {
	if td == nil || typeName == "" {
		return nil
	}
	rows, ok := td.Sections["TriggerParams"]
	if !ok {
		return nil
	}
	out := make([]Preset, 0, 16)
	for key, toks := range rows {
		if len(key) > 0 && key[0] == '_' {
			continue
		}
		if len(toks) < 2 {
			continue
		}
		if toks[1] != typeName {
			continue
		}
		p := Preset{Name: key, Type: toks[1]}
		if len(toks) >= 3 {
			p.Value = toks[2]
		}
		if len(toks) >= 4 {
			p.DisplayName = toks[3]
		}
		out = append(out, p)
	}
	return out
}

// TriggerType is one row from [TriggerTypes]. BaseType is the underlying type
// for aliases (e.g. `unitcode` → `integer`); empty for atomic types.
// DefaultValue is reserved for Phase 2b2 (TriggerTypeDefaults integration).
type TriggerType struct {
	Name         string
	BaseType     string
	DisplayName  string
	CanBeGlobal  bool
	CanCompare   bool
	DefaultValue string
}

// TriggerTypes returns every [TriggerTypes] row as a TriggerType slice. The
// order matches the raw map-iteration order (no guarantee); callers that
// need a sorted result should sort by Name themselves.
func (td *TriggerData) TriggerTypes() []TriggerType {
	if td == nil {
		return nil
	}
	rows, ok := td.Sections["TriggerTypes"]
	if !ok {
		return nil
	}
	out := make([]TriggerType, 0, len(rows))
	for key, toks := range rows {
		if len(key) > 0 && key[0] == '_' {
			continue
		}
		tt := TriggerType{Name: key}
		// Col 0: since_version (skip), Col 1: can_be_global, Col 2: can_compare,
		// Col 3: display, Col 4: base_type.
		if len(toks) >= 2 {
			tt.CanBeGlobal = toks[1] == "1"
		}
		if len(toks) >= 3 {
			tt.CanCompare = toks[2] == "1"
		}
		if len(toks) >= 4 {
			tt.DisplayName = toks[3]
		}
		if len(toks) >= 5 {
			tt.BaseType = toks[4]
		}
		out = append(out, tt)
	}
	return out
}

// BaseType walks the [TriggerTypes] alias chain rooted at `typeName` and
// returns the most-derived atomic type. Returns `typeName` itself when the
// type has no alias (or when the chain is malformed). The walk is bounded at
// 16 hops to defend against cyclic data.
//
// Useful when a param's declared type is `unitcode` but the picker wants to
// know whether to use an integer literal editor underneath; BaseType returns
// "integer" in that case.
func (td *TriggerData) BaseType(typeName string) string {
	if td == nil || typeName == "" {
		return typeName
	}
	rows, ok := td.Sections["TriggerTypes"]
	if !ok {
		return typeName
	}
	cur := typeName
	for hop := 0; hop < 16; hop++ {
		toks, ok := rows[cur]
		if !ok {
			return cur
		}
		if len(toks) < 5 {
			return cur
		}
		base := toks[4]
		if base == "" || base == cur {
			return cur
		}
		cur = base
	}
	return cur
}

// EventsByCategory groups every [TriggerEvents] function name by its
// _Foo_Category companion. Unknown / missing categories land in "" so the
// picker can put them under a default "Other" bucket. Lists are returned
// sorted by function name for deterministic UI output.
func (td *TriggerData) EventsByCategory() map[string][]string {
	return td.functionsByCategory("TriggerEvents")
}

// ConditionsByCategory — same shape as EventsByCategory, for conditions.
func (td *TriggerData) ConditionsByCategory() map[string][]string {
	return td.functionsByCategory("TriggerConditions")
}

// ActionsByCategory — same shape as EventsByCategory, for actions.
func (td *TriggerData) ActionsByCategory() map[string][]string {
	return td.functionsByCategory("TriggerActions")
}

// CallsByCategory — same shape as EventsByCategory, for calls.
func (td *TriggerData) CallsByCategory() map[string][]string {
	return td.functionsByCategory("TriggerCalls")
}

func (td *TriggerData) functionsByCategory(sect string) map[string][]string {
	if td == nil {
		return nil
	}
	rows, ok := td.Sections[sect]
	if !ok {
		return nil
	}
	out := map[string][]string{}
	for key := range rows {
		if len(key) > 0 && key[0] == '_' {
			continue
		}
		cat := ""
		if c, ok := rows["_"+key+"_Category"]; ok && len(c) > 0 {
			cat = c[0]
		}
		out[cat] = append(out[cat], key)
	}
	// Sort each bucket for deterministic UI ordering. Callers can group / sort
	// further client-side; the deterministic baseline here makes diff-based
	// snapshot tests viable.
	for _, names := range out {
		sortStrings(names)
	}
	return out
}

// sortStrings is a tiny in-place sort (insertion sort, fine for the <500-row
// bucket sizes here) so we don't pull "sort" into the import set just for
// this. Keeps the package's import surface minimal.
func sortStrings(s []string) {
	for i := 1; i < len(s); i++ {
		v := s[i]
		j := i - 1
		for j >= 0 && s[j] > v {
			s[j+1] = s[j]
			j--
		}
		s[j+1] = v
	}
}
