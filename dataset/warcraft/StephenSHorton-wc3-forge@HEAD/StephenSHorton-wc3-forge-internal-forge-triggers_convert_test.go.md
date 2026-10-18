---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_convert_test.go"
ref: HEAD
---

package forge

import (
	"errors"
	"os"
	"path/filepath"
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wct"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// Convert-to-Lua tests. The flow now distinguishes pure JASS (transpiled
// automatically) from vJASS (blocked). Tests cover both branches.

// writeConvertFixture creates a tiny extracted-map folder under t.TempDir().
// Returns the path. Includes:
//   - war3map.w3i with FileVersion=TFT (Lua=false initially)
//   - war3map.j with a small JASS body (used as the synthesized Map Header
//     for hand-rolled-script tests)
//   - optionally war3map.wtg + war3map.wct (built from the provided triggers
//     argument). When triggers is nil, no wtg/wct is written — the loader
//     synthesizes a hand-rolled-script "Map Header" trigger from war3map.j.
func writeConvertFixture(t *testing.T, triggers *wtg.Triggers) string {
	t.Helper()
	dir := t.TempDir()

	info := &w3i.Info{
		FileVersion: w3i.FileVersionTFT,
		MapVersion:  1,
		Name:        "ConvertFixture",
		Author:      "tests",
		Tileset:     'L',
		Lua:         false,
	}
	infoBytes, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("w3i.Encode: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), infoBytes, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}

	jSource := []byte("function main takes nothing returns nothing\n    call DoNothing()\nendfunction\n")
	if err := os.WriteFile(filepath.Join(dir, "war3map.j"), jSource, 0o644); err != nil {
		t.Fatalf("write j: %v", err)
	}

	if triggers != nil {
		td := TriggerDataSnapshot()
		if td == nil {
			t.Fatalf("TriggerDataSnapshot returned nil — embedded data broken")
		}
		wtgBytes, err := wtg.Encode(triggers, td.ArgumentCounts)
		if err != nil {
			t.Fatalf("wtg.Encode: %v", err)
		}
		if err := os.WriteFile(filepath.Join(dir, "war3map.wtg"), wtgBytes, 0o644); err != nil {
			t.Fatalf("write wtg: %v", err)
		}
		wctFile := &wct.File{Version: 0x80000004, SubVersion: 1}
		wctFile.CustomTexts = collectOrderedCustomTexts(triggers, wctFile.IsPre131)
		wctBytes, err := wct.Encode(wctFile, triggers)
		if err != nil {
			t.Fatalf("wct.Encode: %v", err)
		}
		if err := os.WriteFile(filepath.Join(dir, "war3map.wct"), wctBytes, 0o644); err != nil {
			t.Fatalf("write wct: %v", err)
		}
	}

	return dir
}

// guiOnlyTriggers builds a *wtg.Triggers with one MapInitializationEvent-
// driven GUI trigger that calls DoNothing. No custom_text, no is_script —
// this is the canonical "safe to convert" shape.
func guiOnlyTriggers() *wtg.Triggers {
	return &wtg.Triggers{
		Version:    0x80000004,
		SubVersion: 7,
		TrigDefVer: 2,
		Categories: []wtg.Category{
			{Classifier: wtg.ClassifierMap, ID: 0, ParentID: -1, Name: "Map Header"},
			{Classifier: wtg.ClassifierCategory, ID: 1, ParentID: 0, Name: "Main"},
		},
		Triggers: []wtg.Trigger{
			{
				Classifier: wtg.ClassifierGUI, ID: 10, ParentID: 1,
				Name: "OnInit", IsEnabled: true, InitiallyOn: true,
				ECAs: []wtg.ECA{
					{Type: wtg.ECAEvent, Name: "MapInitializationEvent", Enabled: true},
					{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true},
				},
			},
		},
		Elements: []wtg.ElementRef{
			{Kind: wtg.ElementKindCategory, Index: 0},
			{Kind: wtg.ElementKindCategory, Index: 1},
			{Kind: wtg.ElementKindTrigger, Index: 0},
		},
	}
}

// TestCheckConvertToLua_GuiOnlyClean asserts a GUI-only map (no script
// triggers, no custom_text) returns an empty Blockers list.
func TestCheckConvertToLua_GuiOnlyClean(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_PureJassScriptTriggerNotBlocked asserts an is_script
// trigger with only pure-JASS keywords is NOT a blocker — it gets transpiled.
func TestCheckConvertToLua_PureJassScriptTriggerNotBlocked(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "PureJassThing", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "function PureJassThing_Actions takes nothing returns nothing\n    call DoNothing()\nendfunction\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for pure-JASS, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_ModuleScriptTriggerNoLongerBlocks asserts an
// is_script trigger whose only vJASS surface is a `module` (Phase 4) is NOT
// a blocker — modules are now consumed by PreprocessModules and pasted into
// implementing structs.
//
// This test originally asserted the opposite (module blocks). Phase 4
// eliminated the last source-level vJASS blocker; the test now confirms the
// converse semantics and verifies the convert flow proceeds.
func TestCheckConvertToLua_ModuleScriptTriggerNoLongerBlocks(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "ModuleOnly", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "module Foo\n    integer x = 0\nendmodule\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for module-only trigger after Phase 4, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_LibraryScriptTriggerNoLongerBlocks asserts that
// after Phase 2 a script trigger whose body is a vJASS `library` (no
// struct/module/interface/define) is NOT a blocker — it's handled by
// PreprocessLibScope.
func TestCheckConvertToLua_LibraryScriptTriggerNoLongerBlocks(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "LibraryOnly", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "library Foo\n    private function bar takes nothing returns nothing\n        call DoNothing()\n    endfunction\nendlibrary\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for library-only script after Phase 2, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_MixedTriggers asserts a map mixing a pure-JASS
// trigger with a module-bearing trigger has NO blockers after Phase 4 —
// modules are no longer source-level blockers.
func TestCheckConvertToLua_MixedTriggers(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers,
		wtg.Trigger{
			Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
			Name: "Pure", IsEnabled: true, InitiallyOn: true, IsScript: true,
			CustomText: "function Pure_Actions takes nothing returns nothing\n    call DoNothing()\nendfunction\n",
		},
		wtg.Trigger{
			Classifier: wtg.ClassifierScript, ID: 12, ParentID: 1,
			Name: "WithModule", IsEnabled: true, InitiallyOn: true, IsScript: true,
			CustomText: "module S\n    integer x = 0\nendmodule\n",
		},
	)
	tr.Elements = append(tr.Elements,
		wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1},
		wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 2},
	)
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers after Phase 4 (module no longer blocks), got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_CustomTextPureJassNotBlocked asserts a GUI trigger with
// pure-JASS custom_text overlay is NOT a blocker.
func TestCheckConvertToLua_CustomTextPureJassNotBlocked(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierGUI, ID: 12, ParentID: 1,
		Name: "GuiPlusJass", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{
			{Type: wtg.ECAEvent, Name: "MapInitializationEvent", Enabled: true},
		},
		CustomText: "call BJDebugMsg(\"hi\")\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for pure-JASS custom_text, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_HandRolledPureJassNotBlocked asserts a map with no
// .wtg but a pure-JASS hand-rolled war3map.j is NOT a blocker (the transpiler
// handles the synthesized Map Header script).
func TestCheckConvertToLua_HandRolledPureJassNotBlocked(t *testing.T) {
	dir := writeConvertFixture(t, nil) // no wtg → loader synthesizes Map Header
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for hand-rolled pure-JASS map, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_HandRolledModuleNoLongerBlocks asserts a
// hand-rolled war3map.j whose only vJASS surface is `module` is NOT a
// blocker after Phase 4 (modules consumed by PreprocessModules).
//
// Pre-Phase-2 this test used `library`; pre-Phase-3 `struct`; pre-Phase-4
// `module`. All are now handled. The function is retained so a future
// safety-net keyword has a regression slot to graduate into.
func TestCheckConvertToLua_HandRolledModuleNoLongerBlocks(t *testing.T) {
	dir := t.TempDir()
	info := &w3i.Info{
		FileVersion: w3i.FileVersionTFT,
		Name:        "ModuleOnlyFixture",
		Tileset:     'L',
	}
	infoBytes, _ := w3i.Encode(info)
	_ = os.WriteFile(filepath.Join(dir, "war3map.w3i"), infoBytes, 0o644)
	_ = os.WriteFile(filepath.Join(dir, "war3map.j"), []byte("module Stuff\n    integer x = 0\nendmodule\n"), 0o644)

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for module-only hand-rolled map after Phase 4, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestCheckConvertToLua_HandRolledLibraryNoLongerBlocks asserts a hand-rolled
// war3map.j whose only vJASS surface is library/scope is NOT a blocker after
// Phase 2.
func TestCheckConvertToLua_HandRolledLibraryNoLongerBlocks(t *testing.T) {
	dir := t.TempDir()
	info := &w3i.Info{
		FileVersion: w3i.FileVersionTFT,
		Name:        "LibraryOnlyFixture",
		Tileset:     'L',
	}
	infoBytes, _ := w3i.Encode(info)
	_ = os.WriteFile(filepath.Join(dir, "war3map.w3i"), infoBytes, 0o644)
	_ = os.WriteFile(filepath.Join(dir, "war3map.j"), []byte(
		"library Stuff\n"+
			"    private function helper takes nothing returns nothing\n"+
			"    endfunction\n"+
			"endlibrary\n"+
			"function main takes nothing returns nothing\n"+
			"    call DoNothing()\n"+
			"endfunction\n",
	), 0o644)

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	res, err := s.CheckConvertToLua()
	if err != nil {
		t.Fatalf("CheckConvertToLua: %v", err)
	}
	if len(res.Blockers) != 0 {
		t.Fatalf("expected no blockers for library-only hand-rolled map, got %d: %+v", len(res.Blockers), res.Blockers)
	}
}

// TestConvertToLua_SuccessRoundTrip asserts the happy path (pure GUI map):
//   - GUI-only map opens
//   - ConvertToLua succeeds (no blockers)
//   - war3map.lua appears on disk with codegen output
//   - war3map.j is gone
//   - info.Lua is true
//   - Save persists the info change (re-open shows Lua=true)
//
// Backup defaults to true; the backup folder should exist on disk after
// conversion.
func TestConvertToLua_SuccessRoundTrip(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if s.Info().Lua {
		t.Fatalf("fixture says Lua=true at open — bug in writeConvertFixture")
	}

	res, err := s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: false})
	if err != nil {
		t.Fatalf("ConvertToLua: %v", err)
	}
	if res == nil || len(res.Blockers) != 0 {
		t.Fatalf("expected empty Blockers on success, got %+v", res)
	}

	luaPath := filepath.Join(dir, "war3map.lua")
	luaBytes, err := os.ReadFile(luaPath)
	if err != nil {
		t.Fatalf("read war3map.lua: %v", err)
	}
	if !strings.Contains(string(luaBytes), "-- generated by wc3-forge") {
		t.Errorf("war3map.lua missing codegen header — got first 200 chars: %q", string(luaBytes[:min(200, len(luaBytes))]))
	}

	if _, err := os.Stat(filepath.Join(dir, "war3map.j")); !errors.Is(err, os.ErrNotExist) {
		t.Errorf("war3map.j should be deleted, got err=%v", err)
	}

	if !s.Info().Lua {
		t.Errorf("info.Lua not flipped to true after ConvertToLua")
	}
	if !s.IsDirty() {
		t.Errorf("session should be dirty after ConvertToLua (info change pending)")
	}

	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}

	s2 := &Session{}
	if err := s2.Open(dir); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	if !s2.Info().Lua {
		t.Errorf("info.Lua did not survive Save+re-open")
	}
}

// TestConvertToLua_BackupCreatesFolderCopy exercises the backup path for a
// folder-source map.
func TestConvertToLua_BackupCreatesFolderCopy(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	_, err := s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: true})
	if err != nil {
		t.Fatalf("ConvertToLua: %v", err)
	}
	backup := dir + ".backup"
	if !pathExists(backup) {
		t.Fatalf("expected backup folder at %q", backup)
	}
	// Backup folder should still have war3map.j (pre-conversion).
	if _, err := os.Stat(filepath.Join(backup, "war3map.j")); err != nil {
		t.Errorf("backup folder missing war3map.j: %v", err)
	}
}

// TestConvertToLua_TranspilesPureJass asserts a map with a pure-JASS script
// trigger ends up with transpiled Lua in war3map.lua (not the original JASS).
func TestConvertToLua_TranspilesPureJass(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "Helper", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "function Helper_Init takes nothing returns nothing\n    call BJDebugMsg(\"hi from helper\")\nendfunction\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if _, err := s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: false}); err != nil {
		t.Fatalf("ConvertToLua: %v", err)
	}
	lua, err := os.ReadFile(filepath.Join(dir, "war3map.lua"))
	if err != nil {
		t.Fatalf("read war3map.lua: %v", err)
	}
	if !strings.Contains(string(lua), "function Helper_Init()") {
		t.Errorf("expected transpiled Lua function decl, got: ...%s...", string(lua[max(0, len(lua)-400):]))
	}
	if strings.Contains(string(lua), "function Helper_Init takes nothing returns nothing") {
		t.Errorf("expected JASS to be replaced; still found original signature")
	}
}

func TestConvertToLua_Undo(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	originalJASS, err := os.ReadFile(filepath.Join(dir, "war3map.j"))
	if err != nil {
		t.Fatalf("read original war3map.j: %v", err)
	}

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if _, err := s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: false}); err != nil {
		t.Fatalf("ConvertToLua: %v", err)
	}
	if !s.CanUndo() {
		t.Fatalf("CanUndo=false after ConvertToLua — Command not recorded")
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}

	restored, err := os.ReadFile(filepath.Join(dir, "war3map.j"))
	if err != nil {
		t.Fatalf("read restored war3map.j: %v", err)
	}
	if string(restored) != string(originalJASS) {
		t.Errorf("restored war3map.j differs from original\n  original: %q\n  restored: %q",
			string(originalJASS), string(restored))
	}

	if _, err := os.Stat(filepath.Join(dir, "war3map.lua")); !errors.Is(err, os.ErrNotExist) {
		t.Errorf("war3map.lua should be deleted on Undo, got err=%v", err)
	}

	if s.Info().Lua {
		t.Errorf("info.Lua did not flip back to false on Undo")
	}
}

func TestConvertToLua_AlreadyLuaRefuses(t *testing.T) {
	dir := writeConvertFixture(t, guiOnlyTriggers())
	infoBytes, err := os.ReadFile(filepath.Join(dir, "war3map.w3i"))
	if err != nil {
		t.Fatalf("read w3i: %v", err)
	}
	info, err := w3i.Parse(infoBytes)
	if err != nil {
		t.Fatalf("parse w3i: %v", err)
	}
	info.Lua = true
	info.FileVersion = w3i.FileVersionRefV28
	infoBytes2, err := w3i.Encode(info)
	if err != nil {
		t.Fatalf("re-encode w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(dir, "war3map.w3i"), infoBytes2, 0o644); err != nil {
		t.Fatalf("rewrite w3i: %v", err)
	}

	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if !s.Info().Lua {
		t.Fatalf("expected Lua=true after patching")
	}
	_, err = s.ConvertToLuaWithOptions(ConvertToLuaOptions{Backup: false})
	if !errors.Is(err, ErrAlreadyLua) {
		t.Errorf("expected ErrAlreadyLua, got %v", err)
	}
}

// TestTranspilePreview_Smoke checks that TranspilePreview returns a non-empty
// section for a script trigger with pure-JASS body.
func TestTranspilePreview_Smoke(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "PreviewMe", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "function PreviewMe_Actions takes nothing returns nothing\n    call DoNothing()\nendfunction\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	preview, err := s.TranspilePreview()
	if err != nil {
		t.Fatalf("TranspilePreview: %v", err)
	}
	if len(preview.Sections) != 1 {
		t.Fatalf("expected 1 section, got %d: %+v", len(preview.Sections), preview.Sections)
	}
	if preview.Sections[0].Kind != "script" {
		t.Errorf("expected kind=script, got %+v", preview.Sections[0])
	}
	if !strings.Contains(preview.Sections[0].Transpiled, "function PreviewMe_Actions()") {
		t.Errorf("expected transpiled Lua, got: %s", preview.Sections[0].Transpiled)
	}
}

// TestTranspilePreview_SurfacesInlineErrorCount is the P0#1 regression: a
// section whose body contains unparseable statements must surface a NON-ZERO
// error count on section.Errors, matching the number of inline error()/RawStmt
// markers the emitter wrote. Previously the dialog under-reported these to 0
// because TranspileScript discarded the recovered File.Errors.
func TestTranspilePreview_SurfacesInlineErrorCount(t *testing.T) {
	// A script trigger with three unparseable top-level statements. After the
	// full preprocessor pipeline these stay as garbage statements; the parser
	// recovers each into a RawStmt → error() marker.
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "Broken", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "garbage one two\nmore junk here\nstill broken tokens\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	preview, err := s.TranspilePreview()
	if err != nil {
		t.Fatalf("TranspilePreview: %v", err)
	}
	var broken *TranspileSection
	for i := range preview.Sections {
		if preview.Sections[i].ID == 11 {
			broken = &preview.Sections[i]
			break
		}
	}
	if broken == nil {
		t.Fatalf("missing section for trigger 11; got %+v", preview.Sections)
	}
	inlineCount := len(broken.Diagnostics)
	if inlineCount == 0 {
		t.Fatalf("expected diagnostics for untranslatable statements, got none:\n%s", broken.Transpiled)
	}
	if strings.Contains(broken.Transpiled, "jass2lua failed") {
		t.Errorf("transpiled output should be marker-free (diagnostics are structured now):\n%s", broken.Transpiled)
	}
	if len(broken.Errors) == 0 {
		t.Fatalf("P0#1 regression: section reported 0 errors but produced %d diagnostics", inlineCount)
	}
	if len(broken.Errors) != inlineCount {
		t.Errorf("section.Errors count (%d) should match diagnostics count (%d)\nerrors: %v\nlua:\n%s",
			len(broken.Errors), inlineCount, broken.Errors, broken.Transpiled)
	}
}

// TestTranspilePreview_SurfacesStructMethodErrorCount is the P0#1 COMPLETION
// regression: a script section that defines a vJASS struct whose method body
// contains an unparseable statement must surface a NON-ZERO section.Errors
// count. Before this fix the struct-method error() markers were emitted into
// the spliced Lua but swallowed (SpliceStructLua returned only a string), so
// the section under-reported these to 0 — the exact gap the verifier found on
// "Dialog System" (91 inline error() / 0 reported).
func TestTranspilePreview_SurfacesStructMethodErrorCount(t *testing.T) {
	tr := guiOnlyTriggers()
	// A struct with two methods: one clean, one whose body has an unparseable
	// statement. The struct preprocessor strips the block to a marker; the
	// splice transpiles each method body and now surfaces the recovered error.
	body := "struct Widget\n" +
		"    method ok takes nothing returns nothing\n" +
		"        call BJDebugMsg(\"hi\")\n" +
		"    endmethod\n" +
		"    method bork takes nothing returns nothing\n" +
		"        garbage tokens here\n" +
		"    endmethod\n" +
		"endstruct\n"
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "StructBork", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: body,
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	preview, err := s.TranspilePreview()
	if err != nil {
		t.Fatalf("TranspilePreview: %v", err)
	}
	var sec *TranspileSection
	for i := range preview.Sections {
		if preview.Sections[i].ID == 11 {
			sec = &preview.Sections[i]
			break
		}
	}
	if sec == nil {
		t.Fatalf("missing section for trigger 11; got %+v", preview.Sections)
	}
	inlineCount := len(sec.Diagnostics)
	if inlineCount == 0 {
		t.Fatalf("expected diagnostics from the bad struct method, got none:\n%s", sec.Transpiled)
	}
	if len(sec.Errors) == 0 {
		t.Fatalf("P0#1 completion regression: struct-method failure produced %d diagnostics but section reported 0 errors", inlineCount)
	}
	// At least one surfaced error must point at the offending struct method.
	found := false
	for _, e := range sec.Errors {
		if strings.Contains(e, "struct Widget method bork") {
			found = true
			break
		}
	}
	if !found {
		t.Errorf("expected a surfaced error naming `struct Widget method bork`, got: %v", sec.Errors)
	}
}

// TestTranspilePreview_BareStatementScriptRoutesClean is the P0#2 regression:
// a script-classified trigger whose body is a bare statement list (no
// top-level function/globals) must route through the body-fragment parser and
// transpile to CLEAN Lua with ZERO surfaced errors. Previously such a body
// hit the top-level parser and produced a wall of error() (one per line).
func TestTranspilePreview_BareStatementScriptRoutesClean(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "BareBody", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "if x then\n    call F()\n    set y = 1\nendif\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	preview, err := s.TranspilePreview()
	if err != nil {
		t.Fatalf("TranspilePreview: %v", err)
	}
	var sec *TranspileSection
	for i := range preview.Sections {
		if preview.Sections[i].ID == 11 {
			sec = &preview.Sections[i]
			break
		}
	}
	if sec == nil {
		t.Fatalf("missing section for trigger 11; got %+v", preview.Sections)
	}
	if len(sec.Errors) != 0 {
		t.Fatalf("P0#2 regression: bare-statement script reported %d errors (should be 0): %v\nlua:\n%s",
			len(sec.Errors), sec.Errors, sec.Transpiled)
	}
	if strings.Contains(sec.Transpiled, "error(") {
		t.Errorf("bare-statement script should transpile clean (no error() markers), got:\n%s", sec.Transpiled)
	}
	for _, want := range []string{"if x then", "F()", "y = 1"} {
		if !strings.Contains(sec.Transpiled, want) {
			t.Errorf("expected %q in transpiled Lua, got:\n%s", want, sec.Transpiled)
		}
	}
}

// TestTranspilePreview_FullScriptStillUsesScriptParser confirms P0#2 did not
// regress true full-script bodies: a body with a top-level `function Foo
// takes ...` still goes through TranspileScript (emits the function decl
// verbatim, NOT wrapped in a synthetic shim) and transpiles cleanly.
func TestTranspilePreview_FullScriptStillUsesScriptParser(t *testing.T) {
	tr := guiOnlyTriggers()
	tr.Triggers = append(tr.Triggers, wtg.Trigger{
		Classifier: wtg.ClassifierScript, ID: 11, ParentID: 1,
		Name: "FullScript", IsEnabled: true, InitiallyOn: true, IsScript: true,
		CustomText: "function Foo takes nothing returns nothing\n    call DoNothing()\nendfunction\n",
	})
	tr.Elements = append(tr.Elements, wtg.ElementRef{Kind: wtg.ElementKindTrigger, Index: 1})
	dir := writeConvertFixture(t, tr)
	s := &Session{}
	if err := s.Open(dir); err != nil {
		t.Fatalf("Open: %v", err)
	}
	preview, err := s.TranspilePreview()
	if err != nil {
		t.Fatalf("TranspilePreview: %v", err)
	}
	var sec *TranspileSection
	for i := range preview.Sections {
		if preview.Sections[i].ID == 11 {
			sec = &preview.Sections[i]
			break
		}
	}
	if sec == nil {
		t.Fatalf("missing section for trigger 11; got %+v", preview.Sections)
	}
	if len(sec.Errors) != 0 {
		t.Fatalf("full-script body reported %d errors (should be 0): %v\nlua:\n%s", len(sec.Errors), sec.Errors, sec.Transpiled)
	}
	if !strings.Contains(sec.Transpiled, "function Foo()") {
		t.Errorf("expected verbatim function decl `function Foo()`, got:\n%s", sec.Transpiled)
	}
	// The bare-statement shim would wrap in a section-named function; the
	// full-script path must NOT do that.
	if strings.Contains(sec.Transpiled, "function FullScript()") || strings.Contains(sec.Transpiled, "function Custom_Script__FullScript()") {
		t.Errorf("full-script body must not be wrapped in a synthetic shim, got:\n%s", sec.Transpiled)
	}
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}

func max(a, b int) int {
	if a > b {
		return a
	}
	return b
}

// TestAppendErrorMessage_FiltersEmpty is the Phase 5 regression test for the
// "empty error bullet" bug. Previously, sections would surface blank entries
// in the transpiler-diagnostics list when an unsurfaced parser branch pushed
// an empty `err.Error()` string. The defensive filter drops them.
func TestAppendErrorMessage_FiltersEmpty(t *testing.T) {
	cases := []struct {
		name string
		err  error
		want int
	}{
		{"nil", nil, 0},
		{"empty string", errors.New(""), 0},
		{"whitespace only", errors.New("   \t\n  "), 0},
		{"real message", errors.New("unexpected token"), 1},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			got := appendErrorMessage(nil, c.err)
			if len(got) != c.want {
				t.Errorf("got %d entries, want %d (entries=%v)", len(got), c.want, got)
			}
		})
	}
}
