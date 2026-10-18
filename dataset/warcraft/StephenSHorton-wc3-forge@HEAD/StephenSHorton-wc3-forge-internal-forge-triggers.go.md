---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers.go"
ref: HEAD
---

package forge

import (
	"errors"
	"fmt"
	"log"
	"path/filepath"
	"strings"
	"sync"

	"github.com/StephenSHorton/wc3-forge/internal/formats/wct"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
	wtgdata "github.com/StephenSHorton/wc3-forge/internal/formats/wtg/data"
	"github.com/StephenSHorton/wc3-forge/internal/wc3launch"
)

// Trigger Editor data layer — Phase 1a (read-only).
//
// The Session owns one *wtg.Triggers and one *wct.File per loaded map; this
// file wires the parse → resolve → expose flow. Phase 2a will add mutation +
// encode + dirty tracking; Phase 3 wires Lua codegen.
//
// Three integration patterns matter:
//
//  1. Embedded TriggerData snapshot. The wtg parser needs argc per function
//     name. Live CASC could provide it but we ship our own snapshot under
//     internal/formats/wtg/data/ so Phase-1a parses are independent of the
//     user's WC3 install. The CASC mount IS consulted on Open as a soft
//     check: if its TriggerData.txt size disagrees with our snapshot, we
//     log a warning so future agents know to refresh.
//
//  2. Hand-rolled-script maps. Many real maps (the Enfo family, almost every
//     LTS project) have no .wtg at all — every trigger lives in war3map.lua
//     or war3map.j. The Trigger Editor still needs SOMETHING to display, so
//     Open synthesizes a "Map Header" Classifier::script entry whose
//     CustomText is the raw script source. Editable CodeMirror lands in 2a;
//     in 1a we render it in a <pre> block.
//
//  3. wct binding. The .wct stores per-trigger script blobs in a flat list;
//     the .wtg's category-then-trigger walk determines the order. Per HiveWE
//     triggers.ixx L924-938, that order is "outer loop categories, inner
//     loop triggers whose parent_id matches, skip is_comment in 1.31+".
//     bindCustomTexts replicates that walk so each Trigger.CustomText points
//     at the right blob.

// triggerDataOnce + triggerDataValue cache the parsed embedded snapshot so
// every Session.Open doesn't re-parse the same 689k of TriggerData.txt.
// Process-lifetime singleton — refresh requires a binary rebuild (the embed
// reads at compile time).
//
// The error is captured and stuck in triggerDataErr so callers can branch on
// "the snapshot is broken" without re-running ParseTriggerData on every Open.
var (
	triggerDataOnce  sync.Once
	triggerDataValue *wtg.TriggerData
	triggerDataErr   error
)

// TriggerDataSnapshot returns the process-wide *wtg.TriggerData built from
// the embedded snapshot under internal/formats/wtg/data/. Idempotent + cheap
// after first call. Returns nil if the embedded snapshot failed to parse —
// which would be a build-time error (the snapshot ships with the binary), so
// real-world callers can assume non-nil.
func TriggerDataSnapshot() *wtg.TriggerData {
	triggerDataOnce.Do(func() {
		td, err := wtg.ParseTriggerData(wtgdata.TriggerDataTXT)
		if err != nil {
			triggerDataErr = err
			log.Printf("triggers: parse embedded TriggerData.txt: %v", err)
			return
		}
		if err := td.LoadTriggerStrings(wtgdata.TriggerStringsTXT); err != nil {
			// Soft-fail: missing tooltips don't break the editor.
			log.Printf("triggers: parse embedded TriggerStrings.txt: %v", err)
		}
		triggerDataValue = td
	})
	return triggerDataValue
}

// Triggers returns the loaded map's parsed war3map.wtg, or nil if no map is
// loaded / the map has no triggers. The returned pointer is shared — callers
// must not mutate (no mutation API in 1a, so this is more about future-proof
// discipline than current safety).
func (s *Session) Triggers() *wtg.Triggers {
	s.mu.RLock()
	defer s.mu.RUnlock()
	return s.triggers
}

// TriggersScripts returns the loaded map's parsed war3map.wct, or nil if no
// map is loaded / the map has no .wct file. The returned pointer is shared.
func (s *Session) TriggersScripts() *wct.File {
	s.mu.RLock()
	defer s.mu.RUnlock()
	return s.triggersWct
}

// loadTriggersForOpenV2 is the Phase 2a version of loadTriggersForOpen with
// the extra return fields the Session needs to discriminate hand-rolled-script
// maps at Save time (the boolean) and to know which script file to write
// (the string). The legacy 2-return shape is preserved as a wrapper for any
// caller still using it.
func loadTriggersForOpenV2(src fileSource, isLuaMap bool) (*wtg.Triggers, *wct.File, bool, string) {
	td := TriggerDataSnapshot()
	if td == nil {
		log.Printf("triggers: embedded snapshot unusable; trigger editor will be empty")
		return nil, nil, false, ""
	}
	wtgBytes, hasWTG, err := src.read("war3map.wtg")
	if err != nil {
		log.Printf("triggers: read war3map.wtg: %v", err)
	}
	wctBytes, hasWCT, err := src.read("war3map.wct")
	if err != nil {
		log.Printf("triggers: read war3map.wct: %v", err)
	}
	var triggers *wtg.Triggers
	var wctFile *wct.File
	if hasWTG {
		parsed, err := wtg.Parse(wtgBytes, td.ArgumentCounts)
		if err != nil {
			log.Printf("triggers: parse war3map.wtg: %v", err)
		} else {
			triggers = parsed
		}
	}
	if hasWCT {
		parsed, err := wct.Parse(wctBytes)
		if err != nil {
			log.Printf("triggers: parse war3map.wct: %v", err)
		} else {
			wctFile = parsed
		}
	}
	if triggers != nil && wctFile != nil {
		bindCustomTexts(triggers, wctFile)
		return triggers, wctFile, false, ""
	}
	if triggers != nil {
		return triggers, nil, false, ""
	}
	// No .wtg. If the map has a script source, synthesize a hand-rolled
	// "Map Header" entry so the user can at least read it (and now: edit it).
	if scriptPath, scriptBytes := readScriptSource(src, isLuaMap); scriptPath != "" {
		return synthesizeHandRolledScriptTriggers(scriptPath, string(scriptBytes)), nil, true, scriptPath
	}
	return nil, nil, false, ""
}

// loadTriggersForOpen is the original 2-return wrapper, retained for any
// test or external caller that wasn't migrated to V2. Discards the extra
// hand-rolled-script metadata.
func loadTriggersForOpen(src fileSource, isLuaMap bool) (*wtg.Triggers, *wct.File) {
	td := TriggerDataSnapshot()
	if td == nil {
		log.Printf("triggers: embedded snapshot unusable; trigger editor will be empty")
		return nil, nil
	}
	wtgBytes, hasWTG, err := src.read("war3map.wtg")
	if err != nil {
		log.Printf("triggers: read war3map.wtg: %v", err)
	}
	wctBytes, hasWCT, err := src.read("war3map.wct")
	if err != nil {
		log.Printf("triggers: read war3map.wct: %v", err)
	}

	var triggers *wtg.Triggers
	var wctFile *wct.File

	if hasWTG {
		parsed, err := wtg.Parse(wtgBytes, td.ArgumentCounts)
		if err != nil {
			log.Printf("triggers: parse war3map.wtg: %v", err)
		} else {
			triggers = parsed
		}
	}
	if hasWCT {
		parsed, err := wct.Parse(wctBytes)
		if err != nil {
			log.Printf("triggers: parse war3map.wct: %v", err)
		} else {
			wctFile = parsed
		}
	}

	if triggers != nil && wctFile != nil {
		bindCustomTexts(triggers, wctFile)
		return triggers, wctFile
	}

	if triggers != nil {
		// .wtg without .wct — the per-trigger custom_text columns stay empty.
		// This is unusual but legal; the UI just shows blank script bodies for
		// any custom-script triggers.
		return triggers, nil
	}

	// No .wtg. If the map has a script source, synthesize a hand-rolled
	// "Map Header" entry so the user can at least read it.
	if scriptPath, scriptBytes := readScriptSource(src, isLuaMap); scriptPath != "" {
		return synthesizeHandRolledScriptTriggers(scriptPath, string(scriptBytes)), nil
	}

	return nil, nil
}

// readScriptSource returns the map's primary script file (war3map.lua when
// isLuaMap, war3map.j otherwise). Returns ("", nil) when neither is present
// — that's a legal "no script" map (rare; usually melee templates).
//
// Prefers the declared script kind first but falls back to the other if the
// declared one is absent — Reforged occasionally mislabels a JASS map as Lua
// in w3i (and vice versa) when authors hand-edit.
func readScriptSource(src fileSource, isLuaMap bool) (path string, data []byte) {
	primary := "war3map.j"
	fallback := "war3map.lua"
	if isLuaMap {
		primary, fallback = fallback, primary
	}
	if b, ok, _ := src.read(primary); ok {
		return primary, b
	}
	if b, ok, _ := src.read(fallback); ok {
		return fallback, b
	}
	return "", nil
}

// synthesizeHandRolledScriptTriggers builds a *wtg.Triggers from raw script
// source bytes. The resulting structure has:
//   - A Map Header category (id=0)
//   - One script trigger inside it whose CustomText is the raw script
//
// Editable CodeMirror lands in Phase 2a; in 1a the UI renders this in a
// monospace <pre> block.
func synthesizeHandRolledScriptTriggers(scriptName, scriptText string) *wtg.Triggers {
	return &wtg.Triggers{
		Version:    0x80000004,
		SubVersion: 7,
		TrigDefVer: 2,
		Categories: []wtg.Category{
			{
				Classifier: wtg.ClassifierMap,
				ID:         0,
				ParentID:   -1,
				Name:       "Map Header",
				OpenState:  true,
			},
		},
		Triggers: []wtg.Trigger{
			{
				Classifier:  wtg.ClassifierScript,
				ID:          1,
				ParentID:    0,
				Name:        scriptName,
				Description: "Hand-rolled script (no war3map.wtg in map)",
				CustomText:  scriptText,
				IsEnabled:   true,
				IsScript:    true,
				InitiallyOn: true,
			},
		},
	}
}

// ---------------------------------------------------------------------------
// Phase 3 — Lua codegen + Test Map. See triggers_codegen.go for the emitter.
// ---------------------------------------------------------------------------

// GenerateTriggerScript produces the Lua war3map.lua source from the
// currently-loaded map's triggers + placed entities + map info. Returns
// (text, nil) on success.
//
// Refuses generation in three cases:
//   - no map loaded — returns a friendly error
//   - info.Lua == false — returns ErrLuaOnly (the user must convert the map
//     to Lua via WC3 World Editor → Map → Convert to Lua first)
//   - existing war3map.lua starts with PreserveScriptMarker — returns
//     ErrPreserveScript (the user has hand-edited the script and wants the
//     editor to leave it alone)
//
// The preserve-marker check is the only one that touches disk: we read the
// existing war3map.lua bytes (if any) and peek at the first line. Pure-text
// inputs (tests) can call GenerateLuaScript directly to bypass the marker
// check.
func (s *Session) GenerateTriggerScript() (string, error) {
	s.mu.RLock()
	src := s.source
	handRolled := s.triggerIsHandRolled
	in := CodegenInputs{
		Triggers: s.triggers,
		Units:    s.units,
		Doodads:  s.doodads,
		Terrain:  s.terrain,
		Info:     s.info,
		Regions:  s.regions,
		Cameras:  s.cameras,
		TD:       TriggerDataSnapshot(),
	}
	if s.triggersWct != nil {
		in.GlobalJASS = s.triggersWct.GlobalJASS
	}
	// Snapshot the hand-rolled Map Header text under the lock so we can return
	// it verbatim without holding the lock across codegen.
	var handRolledText string
	if handRolled && s.triggers != nil && len(s.triggers.Triggers) > 0 {
		handRolledText = s.triggers.Triggers[0].CustomText
	}
	loaded := s.loaded
	s.mu.RUnlock()
	if !loaded {
		return "", fmt.Errorf("no map loaded")
	}

	// Hand-rolled-script maps have no GUI tree to lower — the script file IS the
	// source of truth. Return its current (possibly-edited) text verbatim,
	// regardless of language. This is the native-JASS passthrough path.
	if handRolled {
		return handRolledText, nil
	}

	isLua := in.Info == nil || in.Info.Lua

	// Pick the language-appropriate script file + preserve marker.
	scriptFile, marker := "war3map.lua", PreserveScriptMarker
	if !isLua {
		scriptFile, marker = "war3map.j", PreserveScriptMarkerJASS
	}
	// Preserve-marker peek. Reading the existing script under the source's read
	// path covers both folder + MPQ backed sessions.
	if src != nil {
		if b, ok, _ := src.read(scriptFile); ok && len(b) > 0 {
			first := string(b)
			if i := strings.IndexByte(first, '\n'); i >= 0 {
				first = first[:i]
			}
			if strings.TrimSpace(first) == marker {
				return "", ErrPreserveScript
			}
		}
	}
	if isLua {
		return GenerateLuaScript(in)
	}
	return GenerateJASSScript(in)
}

// SaveTriggerScript generates the Lua script and writes it to war3map.lua
// in the session's source. Honors the preserve marker (same as
// GenerateTriggerScript). Returns the written byte count on success.
//
// MPQ-backed sessions write through the source.write path (which repacks the
// archive in place, returning ErrMPQRepackFailed only on an unpreservable
// archive); folder-backed sessions write straight to disk.
func (s *Session) SaveTriggerScript() (int, error) {
	text, err := s.GenerateTriggerScript()
	if err != nil {
		return 0, err
	}
	s.mu.RLock()
	src := s.source
	isLua := s.info == nil || s.info.Lua
	handRolled := s.triggerIsHandRolled
	scriptName := s.mapHeaderScriptName
	s.mu.RUnlock()
	if src == nil {
		return 0, fmt.Errorf("no source for writing")
	}
	// JASS maps write war3map.j; Lua maps write war3map.lua.
	scriptFile := "war3map.lua"
	if !isLua {
		scriptFile = "war3map.j"
	}
	// Hand-rolled maps remember the exact file they loaded from (Reforged
	// occasionally mislabels the w3i Lua flag); honor that over the flag.
	if handRolled && scriptName != "" {
		scriptFile = scriptName
	}
	if err := src.write(scriptFile, []byte(text)); err != nil {
		return 0, fmt.Errorf("write %s: %w", scriptFile, err)
	}
	// This is a DIRECT write bypassing Save's batch commit, and war3map.j/.lua is
	// in the change-detection baseline. Refresh its stamp so a subsequent Save
	// (e.g. a sky or trigger write) doesn't read this script write back as an
	// external change and spuriously refuse. Folder-only; a no-op for MPQ (which
	// buffers until flush). See atomic_save.go.
	s.refreshBaselineEntry(src, scriptFile)
	return len(text), nil
}

// TestMap is the full pipeline: Save the rest of the map's dirty files,
// regenerate + write war3map.lua, then launch WC3 with the map preloaded
// via internal/wc3launch.LaunchWithMap.
//
// Folder-backed sessions can't launch in WC3 directly (the engine wants a
// packaged .w3x); we surface the wc3launch.ErrMapNotFound error so the UI
// can render a friendly toast. Calling code is expected to be the editor
// toolbar "Test Map" button or the MCP triggers.test_map handler.
func (s *Session) TestMap() error {
	// Snapshot the regen-relevant state BEFORE Save() (which resets
	// dirtyTriggers). triggersEdited tells us whether the user actually changed
	// the GUI tree this session.
	s.mu.RLock()
	handRolled := s.triggerIsHandRolled
	triggersEdited := s.dirtyTriggers
	src := s.source
	isLua := s.info == nil || s.info.Lua
	s.mu.RUnlock()

	// 1. Save pending edits to other files (units, doodads, info, terrain,
	//    gameplay, object mods, triggers wtg/wct). For hand-rolled maps this is
	//    also where the Map Header script text reaches disk.
	if err := s.Save(); err != nil {
		return fmt.Errorf("save pending edits: %w", err)
	}

	// 2. Decide whether to regenerate the script file from the trigger tree.
	//    See scriptNeedsRegen for the policy. The key case this protects: an
	//    imported, untouched war3map.j (the original post-JassHelper script of
	//    an old JASS map) is PRESERVED — regenerating from a possibly partial or
	//    vJASS .wtg would replace a known-good script with a worse one. That's
	//    what makes "open an old JASS map → Test Map" launch the real script
	//    instead of a broken regeneration. Hand-rolled maps never regenerate
	//    (their script was already written by Save).
	var existing []byte
	var existingOK bool
	if !handRolled && src != nil && !triggersEdited {
		scriptFile := "war3map.lua"
		if !isLua {
			scriptFile = "war3map.j"
		}
		existing, existingOK, _ = src.read(scriptFile)
	}
	if scriptNeedsRegen(handRolled, triggersEdited, isLua, existing, existingOK) {
		// Use the non-destructive guarded path: it refuses to clobber a hand-
		// authored war3map.lua (one not carrying the codegen marker) with
		// ErrScriptNotCodegen. For Test Map that refusal is NOT fatal — the
		// on-disk script (written externally / by the World Editor) is exactly
		// what we want to package and launch, so we keep it as-is and continue.
		if _, err := s.SaveTriggerScriptGuarded(false); err != nil && !errors.Is(err, ErrScriptNotCodegen) {
			return fmt.Errorf("regen script: %w", err)
		}
	}

	// 3. Decide the launchable .w3x path.
	//    - Folder-backed sessions: WC3 can't open a loose folder, so package
	//      the folder into a sibling out/<map>.w3x and launch THAT.
	//    - MPQ-backed sessions: launch the .w3x path stored at Open time.
	launchPath := s.Path()
	if _, isFolder := src.(folderSource); isFolder {
		root := s.Path()
		mapName := root
		if info := s.Info(); info != nil && info.Name != "" {
			mapName = info.Name
		} else {
			mapName = filepath.Base(root)
		}
		outPath := filepath.Join(filepath.Dir(root), "out", sanitizeMapFileName(mapName)+".w3x")
		if err := s.PackageFolderToW3X(outPath); err != nil {
			return fmt.Errorf("package folder for test: %w", err)
		}
		launchPath = outPath
	}
	return wc3launch.LaunchWithMap(launchPath)
}

// scriptNeedsRegen is the Test Map / save policy for whether to regenerate the
// script file (war3map.j/.lua) from the trigger tree:
//
//   - hand-rolled maps: never (the script IS the source; Save persisted edits).
//   - the user edited triggers this session: yes (compile their changes).
//   - otherwise, regenerate only if the on-disk script is ABSENT/empty or was
//     itself produced by wc3-forge codegen (first line == the codegen marker).
//     A foreign, untouched script (imported / hand- / JassHelper-authored) is
//     PRESERVED.
//
// Pure over its inputs so the policy is unit-tested without launching WC3.
func scriptNeedsRegen(handRolled, triggersEdited, isLua bool, existing []byte, existingOK bool) bool {
	if handRolled {
		return false
	}
	if triggersEdited {
		return true
	}
	marker := CodegenMarkerLine
	if !isLua {
		marker = JASSCodegenMarkerLine
	}
	return !existingOK || len(existing) == 0 || firstLine(existing) == marker
}

// ---------------------------------------------------------------------------
// Phase 3 — global cross-trigger search. Returns a flat list of hits with
// enough context for the search palette to render + navigate.
// ---------------------------------------------------------------------------

// TriggerSearchHit is one row in a triggers.search response. TriggerID is
// the parent trigger's id (for click-to-navigate); Path is the ECA-path
// inside that trigger (matches the [ec_index, child_index, ...] convention
// used by the set_param_value mutator); Snippet is a short matched-line
// preview for the UI.
type TriggerSearchHit struct {
	TriggerID   int32  `json:"trigger_id"`
	TriggerName string `json:"trigger_name"`
	Kind        string `json:"kind"` // "trigger" | "category" | "variable" | "eca" | "param"
	Path        []int  `json:"path,omitempty"`
	ECAName     string `json:"eca_name,omitempty"`
	Snippet     string `json:"snippet"`
	Category    string `json:"category,omitempty"`
}

// SearchTriggers returns up to maxResults fuzzy matches for query across
// every loaded trigger, category, variable, ECA name, and parameter value.
// Empty query returns the first maxResults rows by trigger name (so the
// palette feels populated on first open).
//
// Scoring tiers (same shape as ObjectSearchPalette):
//
//	1000 = exact match
//	 500 = prefix match
//	 300 = name-substring
//	 100 = ECA-name substring
//	  50 = param-value substring
//	  25 = category-name substring
//
// Ties broken by trigger name, then ECA path.
func (s *Session) SearchTriggers(query string, maxResults int) []TriggerSearchHit {
	if maxResults <= 0 {
		maxResults = 50
	}
	s.mu.RLock()
	t := s.triggers
	s.mu.RUnlock()
	if t == nil {
		return nil
	}
	q := strings.TrimSpace(strings.ToLower(query))
	type scored struct {
		hit   TriggerSearchHit
		score int
	}
	var hits []scored
	add := func(h TriggerSearchHit, score int) {
		if score <= 0 && q != "" {
			return
		}
		hits = append(hits, scored{hit: h, score: score})
	}

	// Index of category id → name for the "category" enrichment column on
	// each hit.
	catName := map[int32]string{}
	for _, c := range t.Categories {
		catName[c.ID] = c.Name
	}

	scoreText := func(text string) int {
		if q == "" {
			return 1
		}
		l := strings.ToLower(text)
		if l == q {
			return 1000
		}
		if strings.HasPrefix(l, q) {
			return 500
		}
		if strings.Contains(l, q) {
			return 300
		}
		return 0
	}

	for _, c := range t.Categories {
		score := scoreText(c.Name)
		if score == 0 {
			continue
		}
		add(TriggerSearchHit{
			TriggerID:   c.ID,
			TriggerName: c.Name,
			Kind:        "category",
			Snippet:     c.Name,
		}, score-25)
	}
	for _, v := range t.Variables {
		score := scoreText(v.Name)
		if score > 0 {
			add(TriggerSearchHit{
				TriggerID:   v.ID,
				TriggerName: v.Name,
				Kind:        "variable",
				Snippet:     v.Name + ":" + v.Type,
			}, score)
		}
	}
	for ti := range t.Triggers {
		tr := &t.Triggers[ti]
		// Trigger name match.
		if score := scoreText(tr.Name); score > 0 {
			add(TriggerSearchHit{
				TriggerID:   tr.ID,
				TriggerName: tr.Name,
				Kind:        "trigger",
				Snippet:     tr.Name,
				Category:    catName[tr.ParentID],
			}, score)
		}
		// Custom-text match (script triggers).
		if tr.CustomText != "" && q != "" {
			if strings.Contains(strings.ToLower(tr.CustomText), q) {
				add(TriggerSearchHit{
					TriggerID:   tr.ID,
					TriggerName: tr.Name,
					Kind:        "eca",
					Snippet:     snippetFromText(tr.CustomText, q),
					Category:    catName[tr.ParentID],
				}, 200)
			}
		}
		// Walk ECAs.
		walkECAsForSearch(tr.ECAs, nil, func(path []int, e *wtg.ECA) {
			if score := scoreText(e.Name); score > 0 {
				add(TriggerSearchHit{
					TriggerID:   tr.ID,
					TriggerName: tr.Name,
					Kind:        "eca",
					Path:        append([]int(nil), path...),
					ECAName:     e.Name,
					Snippet:     e.Name,
					Category:    catName[tr.ParentID],
				}, score/2) // half-weight because ECA names appear in lots of triggers
			}
			for pi, p := range e.Parameters {
				if p.Value == "" || q == "" {
					continue
				}
				if !strings.Contains(strings.ToLower(p.Value), q) {
					continue
				}
				add(TriggerSearchHit{
					TriggerID:   tr.ID,
					TriggerName: tr.Name,
					Kind:        "param",
					Path:        append([]int(nil), path...),
					ECAName:     e.Name,
					Snippet:     fmt.Sprintf("%s arg[%d]=%s", e.Name, pi, p.Value),
					Category:    catName[tr.ParentID],
				}, 50)
			}
		})
	}

	// Stable sort: score desc, then trigger name asc, then path length asc.
	// We do an insertion-style sort to avoid pulling in `sort` for small N.
	// In practice maxResults caps the visible output anyway.
	for i := 1; i < len(hits); i++ {
		j := i
		for j > 0 {
			a, b := hits[j-1], hits[j]
			if a.score < b.score ||
				(a.score == b.score && a.hit.TriggerName > b.hit.TriggerName) {
				hits[j-1], hits[j] = b, a
				j--
			} else {
				break
			}
		}
	}
	if len(hits) > maxResults {
		hits = hits[:maxResults]
	}
	out := make([]TriggerSearchHit, 0, len(hits))
	for _, h := range hits {
		out = append(out, h.hit)
	}
	return out
}

// snippetFromText finds the first match of q in body and returns a
// trimmed ~80-char window centered on the match. Case-insensitive match
// but case-preserving snippet.
func snippetFromText(body, q string) string {
	idx := strings.Index(strings.ToLower(body), q)
	if idx < 0 {
		if len(body) > 80 {
			return strings.TrimSpace(body[:80]) + "…"
		}
		return body
	}
	start := idx - 30
	if start < 0 {
		start = 0
	}
	end := idx + len(q) + 50
	if end > len(body) {
		end = len(body)
	}
	out := body[start:end]
	if start > 0 {
		out = "…" + out
	}
	if end < len(body) {
		out = out + "…"
	}
	return strings.ReplaceAll(out, "\n", " ")
}

// walkECAsForSearch is a depth-first walk that invokes fn for each ECA
// (top-level + nested children of magic ECAs). path tracks the [eca_index,
// child_index, ...] address so callers can navigate back to the row.
func walkECAsForSearch(ecas []wtg.ECA, prefix []int, fn func(path []int, e *wtg.ECA)) {
	for i := range ecas {
		path := append(append([]int(nil), prefix...), i)
		fn(path, &ecas[i])
		if len(ecas[i].Children) > 0 {
			walkECAsForSearch(ecas[i].Children, path, fn)
		}
	}
}

// bindCustomTexts walks the categories-then-triggers order HiveWE writes to
// .wct (triggers.ixx L924-938) and stitches each .wct entry onto its
// corresponding Trigger.CustomText.
//
// 1.31+ format SKIPS is_comment triggers in the wct, so the bind walk does
// too. Pre-1.31 includes every trigger.
//
// If counts don't match (a corrupt or out-of-sync pair), we bind as many as
// we can and log the remainder — better to render partial scripts than to
// drop them all.
func bindCustomTexts(t *wtg.Triggers, w *wct.File) {
	skipComments := !w.IsPre131
	i := 0
	for _, cat := range t.Categories {
		for ti := range t.Triggers {
			tr := &t.Triggers[ti]
			if tr.ParentID != cat.ID {
				continue
			}
			if skipComments && tr.IsComment {
				continue
			}
			if i >= len(w.CustomTexts) {
				log.Printf("triggers: wct ran out at entry %d (have %d, need more) — leaving remaining triggers' custom_text empty",
					i, len(w.CustomTexts))
				return
			}
			tr.CustomText = w.CustomTexts[i]
			i++
		}
	}
	if i < len(w.CustomTexts) {
		log.Printf("triggers: wct had %d entries but only %d bound — %d unused", len(w.CustomTexts), i, len(w.CustomTexts)-i)
	}
}
