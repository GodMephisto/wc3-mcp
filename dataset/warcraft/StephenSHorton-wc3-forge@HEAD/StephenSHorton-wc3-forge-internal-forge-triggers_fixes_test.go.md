---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_fixes_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"errors"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// TestAddTriggerCategory_Repro counts categories before/after a single add to
// confirm one Session call produces exactly one persisted category (round-trip
// through wtg.Encode/Parse). Baseline that the pure Go path is single-add.
func TestAddTriggerCategory_Repro(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	before := len(s.Triggers().Categories)

	id, err := s.AddTriggerCategory("___repro_cat___", -1)
	if err != nil {
		t.Fatalf("AddTriggerCategory: %v", err)
	}
	if got := len(s.Triggers().Categories); got != before+1 {
		t.Fatalf("in-memory category count: got %d, want %d", got, before+1)
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}

	wtgBytes, err := os.ReadFile(filepath.Join(dir, "war3map.wtg"))
	if err != nil {
		t.Fatalf("read wtg: %v", err)
	}
	td := TriggerDataSnapshot()
	parsed, err := wtg.Parse(wtgBytes, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("parse wtg: %v", err)
	}
	matches := 0
	for _, c := range parsed.Categories {
		if c.Name == "___repro_cat___" {
			matches++
		}
	}
	t.Logf("added id=%d, parsed categories=%d (was %d), name matches=%d", id, len(parsed.Categories), before, matches)
	if matches != 1 {
		t.Fatalf("expected exactly 1 persisted category named ___repro_cat___, got %d", matches)
	}
	if len(parsed.Categories) != before+1 {
		t.Fatalf("persisted category count: got %d, want %d", len(parsed.Categories), before+1)
	}
}

// TestAddCategoryHandler_NoDuplicateOnDoubleFire is the bug-2 regression: a
// single logical add_category that arrives twice in quick succession (the
// proven double-invocation signature) must yield exactly ONE persisted
// category, not two. Drives the MCP handler directly (handleTriggersAddCategory
// → dedupe guard → Session) so the guard is exercised end to end.
func TestAddCategoryHandler_NoDuplicateOnDoubleFire(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	useCurrent(t, s)
	resetAddCategoryGuard()
	before := len(s.Triggers().Categories)

	params, _ := json.Marshal(triggersAddCategoryParams{Name: "___dbl_cat___", ParentID: -1})

	// First call — real add.
	if _, err := handleTriggersAddCategory(params); err != nil {
		t.Fatalf("first add_category: %v", err)
	}
	// Second call, same name+parent, immediately after — the double-fire.
	if _, err := handleTriggersAddCategory(params); err != nil {
		t.Fatalf("second add_category: %v", err)
	}

	if got := len(s.Triggers().Categories); got != before+1 {
		t.Fatalf("in-memory categories after double-fire: got %d, want %d (duplicate not suppressed)", got, before+1)
	}

	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	wtgBytes, err := os.ReadFile(filepath.Join(dir, "war3map.wtg"))
	if err != nil {
		t.Fatalf("read wtg: %v", err)
	}
	td := TriggerDataSnapshot()
	parsed, err := wtg.Parse(wtgBytes, td.ArgumentCounts)
	if err != nil {
		t.Fatalf("parse wtg: %v", err)
	}
	matches := 0
	for _, c := range parsed.Categories {
		if c.Name == "___dbl_cat___" {
			matches++
		}
	}
	if matches != 1 {
		t.Fatalf("wtg encodes %d categories named ___dbl_cat___, want exactly 1", matches)
	}
}

// TestAddCategoryGuard_AllowsDistinctNames confirms the dedupe guard does NOT
// suppress legitimately distinct adds (different name under the same parent).
func TestAddCategoryGuard_AllowsDistinctNames(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	useCurrent(t, s)
	resetAddCategoryGuard()
	before := len(s.Triggers().Categories)

	for _, name := range []string{"___a___", "___b___"} {
		params, _ := json.Marshal(triggersAddCategoryParams{Name: name, ParentID: -1})
		if _, err := handleTriggersAddCategory(params); err != nil {
			t.Fatalf("add_category %q: %v", name, err)
		}
	}
	if got := len(s.Triggers().Categories); got != before+2 {
		t.Fatalf("distinct adds: got %d categories, want %d", got, before+2)
	}
}

// --- Bug 1: non-destructive save_script -------------------------------------

// writeLuaMapFixture builds a folder map with a Lua-mode w3i + the given
// war3map.lua contents. No wtg/wct, so codegen lowers an empty trigger tree.
func writeLuaMapFixture(t *testing.T, luaContents string) string {
	t.Helper()
	dir := t.TempDir()
	info := &w3i.Info{FileVersion: w3i.FileVersionRefV28, MapVersion: 1, Name: "LuaFixture", Tileset: 'L', Lua: true}
	b, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("w3i.Encode: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), b, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.lua"), []byte(luaContents), 0o644); err != nil {
		t.Fatalf("write lua: %v", err)
	}
	return dir
}

// TestSaveScript_RefusesToClobberHandAuthored is the bug-1 regression: a
// hand-authored war3map.lua (no codegen marker) must NOT be silently
// overwritten by triggers.save_script. The default path refuses with
// ErrScriptNotCodegen and leaves the file byte-for-byte intact.
func TestSaveScript_RefusesToClobberHandAuthored(t *testing.T) {
	const handAuthored = "-- my hand-written map script\nfunction main()\n\tprint(\"hello from a real script\")\nend\n"
	dir := writeLuaMapFixture(t, handAuthored)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	useCurrent(t, s)

	// Default save_script (no overwrite) must refuse.
	if _, err := handleTriggersSaveScript(nil); err == nil {
		t.Fatalf("save_script silently overwrote a hand-authored war3map.lua (expected ErrScriptNotCodegen)")
	} else if !errors.Is(err, ErrScriptNotCodegen) {
		t.Fatalf("save_script error = %v, want ErrScriptNotCodegen", err)
	}

	// The file on disk must be untouched.
	got, err := os.ReadFile(filepath.Join(dir, "war3map.lua"))
	if err != nil {
		t.Fatalf("read lua after refused save: %v", err)
	}
	if string(got) != handAuthored {
		t.Fatalf("hand-authored war3map.lua was modified despite refusal:\n got: %q\nwant: %q", string(got), handAuthored)
	}
	// No backup should exist yet (we never overwrote).
	if _, err := os.Stat(filepath.Join(dir, "war3map.lua"+ScriptBackupSuffix)); err == nil {
		t.Fatalf("unexpected backup created on a refused (no-overwrite) save")
	}
}

// TestSaveScript_OverwriteBacksUpHandAuthored confirms that an explicit
// overwrite still preserves the original bytes via the .bak sidecar.
//
// Hand-rolled-script maps now round-trip their authoritative Map Header text
// verbatim (no GUI tree to lower, and no fresh scaffold to wrap around a script
// that already has its own main/config). So we edit the Map Header first, then
// assert the overwrite save writes the EDITED script while .bak preserves the
// original on-disk bytes.
func TestSaveScript_OverwriteBacksUpHandAuthored(t *testing.T) {
	const handAuthored = "-- precious hand-written script\nlocal x = 42\n"
	const edited = "-- precious hand-written script (edited)\nlocal x = 43\nlocal y = 7\n"
	dir := writeLuaMapFixture(t, handAuthored)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	useCurrent(t, s)

	if !s.triggerIsHandRolled {
		t.Fatalf("fixture should classify as hand-rolled (no .wtg + a war3map.lua)")
	}
	if err := s.SetMapHeaderScript(edited); err != nil {
		t.Fatalf("SetMapHeaderScript: %v", err)
	}

	params, _ := json.Marshal(triggersSaveScriptParams{Overwrite: true})
	if _, err := handleTriggersSaveScript(params); err != nil {
		t.Fatalf("save_script overwrite=true: %v", err)
	}

	// war3map.lua should now hold the edited Map Header script verbatim.
	got, err := os.ReadFile(filepath.Join(dir, "war3map.lua"))
	if err != nil {
		t.Fatalf("read lua: %v", err)
	}
	if string(got) != edited {
		t.Fatalf("after overwrite, war3map.lua != edited script:\n got: %q\nwant: %q", string(got), edited)
	}
	// The original on-disk bytes must be recoverable from the backup.
	bak, err := os.ReadFile(filepath.Join(dir, "war3map.lua"+ScriptBackupSuffix))
	if err != nil {
		t.Fatalf("read backup: %v", err)
	}
	if string(bak) != handAuthored {
		t.Fatalf("backup does not match original:\n got: %q\nwant: %q", string(bak), handAuthored)
	}
}

// TestSaveScript_OverwritesCodegenFreely confirms a codegen-owned war3map.lua
// (with the marker) is regenerated without refusal and without a needless
// backup — the normal Trigger Editor regenerate path stays working.
func TestSaveScript_OverwritesCodegenFreely(t *testing.T) {
	dir := writeLuaMapFixture(t, CodegenMarkerLine+"\n-- stale codegen body\n")
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	useCurrent(t, s)

	n, err := handleTriggersSaveScript(nil)
	if err != nil {
		t.Fatalf("save_script over codegen file: %v", err)
	}
	res, ok := n.(triggerScriptResult)
	if !ok || res.Bytes <= 0 {
		t.Fatalf("unexpected save_script result: %+v", n)
	}
	got, err := os.ReadFile(filepath.Join(dir, "war3map.lua"))
	if err != nil {
		t.Fatalf("read lua: %v", err)
	}
	if !looksLikeCodegen(got) {
		t.Fatalf("regenerated war3map.lua lost its codegen marker (first line=%q)", firstLine(got))
	}
	// No backup for a codegen-owned overwrite.
	if _, err := os.Stat(filepath.Join(dir, "war3map.lua"+ScriptBackupSuffix)); err == nil {
		t.Fatalf("backup created when overwriting a codegen-owned file (should not happen)")
	}
}

// TestCodegenMarkerStaysInSync guards against the generator and the guard
// drifting apart: GenerateLuaScript's first line MUST equal CodegenMarkerLine,
// otherwise the non-destructive guard would treat fresh codegen as
// hand-authored.
func TestCodegenMarkerStaysInSync(t *testing.T) {
	td := TriggerDataSnapshot()
	if td == nil {
		t.Skip("no embedded TriggerData")
	}
	out, err := GenerateLuaScript(CodegenInputs{TD: td})
	if err != nil {
		t.Fatalf("GenerateLuaScript: %v", err)
	}
	if fl := firstLine([]byte(out)); fl != CodegenMarkerLine {
		t.Fatalf("codegen first line = %q, guard expects %q — they have drifted", fl, CodegenMarkerLine)
	}
}

// useCurrent installs s as the package-global Current session for the duration
// of the test and restores the previous value on cleanup (matching the pattern
// in handlers_test.go). Required because the MCP handlers read forge.Current.
func useCurrent(t *testing.T, s *Session) {
	t.Helper()
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = s
}
