---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/session_polish_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"testing"
)

const polishFixture = `C:\Users\4step\projects\wc3-survival-game\map\extracted`

// TestSetMapInfo_Undoable is the item-1 regression test: map.info_set must be
// undoable. Previously the map-info mutation went through MutateInfo directly
// and never reached recordCommand, so the history stack stayed empty and Undo
// could not restore the prior values. SetMapInfo now records a setMapInfoCmd.
func TestSetMapInfo_Undoable(t *testing.T) {
	tmp := copyFixtureToTemp(t, polishFixture)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	info := s.Info()
	if info == nil {
		t.Fatalf("expected loaded info")
	}
	origName := info.Name
	origAuthor := info.Author
	origDesc := info.Description

	newName := origName + " (edited)"
	newAuthor := origAuthor + " (edited)"
	newDesc := origDesc + " (edited)"

	if s.CanUndo() {
		t.Fatalf("expected empty history before edit")
	}

	changed, err := s.SetMapInfo(map[string]any{
		"name":        newName,
		"author":      newAuthor,
		"description": newDesc,
	})
	if err != nil {
		t.Fatalf("SetMapInfo: %v", err)
	}
	if changed != 3 {
		t.Fatalf("changed_fields = %d, want 3", changed)
	}
	if got := s.Info(); got.Name != newName || got.Author != newAuthor || got.Description != newDesc {
		t.Fatalf("post-edit info = {%q,%q,%q}, want {%q,%q,%q}",
			got.Name, got.Author, got.Description, newName, newAuthor, newDesc)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after SetMapInfo")
	}
	// History stack must now show the edit (the bug: it stayed empty).
	hist := s.HistoryList()
	if len(hist.Undo) != 1 {
		t.Fatalf("history.undo len = %d, want 1 (edit must be recorded)", len(hist.Undo))
	}
	if !s.CanUndo() {
		t.Fatalf("expected CanUndo after recorded edit")
	}

	// Undo restores the prior name/author/description.
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := s.Info(); got.Name != origName || got.Author != origAuthor || got.Description != origDesc {
		t.Fatalf("post-undo info = {%q,%q,%q}, want original {%q,%q,%q}",
			got.Name, got.Author, got.Description, origName, origAuthor, origDesc)
	}

	// Redo re-applies the edit.
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	if got := s.Info(); got.Name != newName || got.Author != newAuthor || got.Description != newDesc {
		t.Fatalf("post-redo info = {%q,%q,%q}, want edited {%q,%q,%q}",
			got.Name, got.Author, got.Description, newName, newAuthor, newDesc)
	}
}

// TestSetMapInfo_NoOpDoesNotRecord asserts an empty / unrecognized-keys update
// records no history entry (matches the no-op short-circuit contract).
func TestSetMapInfo_NoOpDoesNotRecord(t *testing.T) {
	tmp := copyFixtureToTemp(t, polishFixture)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	changed, err := s.SetMapInfo(map[string]any{"totallyUnknownKey": 123})
	if err != nil {
		t.Fatalf("SetMapInfo: %v", err)
	}
	if changed != 0 {
		t.Fatalf("changed = %d, want 0 for unknown key", changed)
	}
	if s.CanUndo() {
		t.Errorf("no-op SetMapInfo must not record history")
	}
	if s.IsDirty() {
		t.Errorf("no-op SetMapInfo must not flip dirty")
	}
}

// TestSetMapInfo_NoMap asserts a clear error when no map is loaded.
func TestSetMapInfo_NoMap(t *testing.T) {
	s := &Session{}
	if _, err := s.SetMapInfo(map[string]any{"name": "x"}); err == nil {
		t.Fatal("expected error from SetMapInfo on unloaded session")
	}
}

// TestHandleMapInfoSetUndoable_WiresThroughHistory exercises the MCP handler
// end-to-end against the global Current session to prove the wire surface is
// the undoable one.
func TestHandleMapInfoSetUndoable_WiresThroughHistory(t *testing.T) {
	tmp := copyFixtureToTemp(t, polishFixture)
	if err := Current.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	t.Cleanup(func() { Current.Close() })

	orig := Current.Info().Name
	params, _ := json.Marshal(map[string]any{"updates": map[string]any{"name": orig + " (mcp)"}})
	res, err := handleMapInfoSetUndoable(params)
	if err != nil {
		t.Fatalf("handleMapInfoSetUndoable: %v", err)
	}
	resp, ok := res.(mapInfoSetResponse)
	if !ok || resp.ChangedFields != 1 {
		t.Fatalf("response = %#v, want changed_fields=1", res)
	}
	if !Current.CanUndo() {
		t.Fatalf("history must record the map.info_set edit")
	}
	if err := Current.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := Current.Info().Name; got != orig {
		t.Fatalf("post-undo name = %q, want original %q", got, orig)
	}
}

// TestSelectionSet_HonorsPrimary covers item 2: selection.set must honor an
// optional client `primary` (index OR kind:id) and fall back to the last item
// when omitted.
func TestSelectionSet_HonorsPrimary(t *testing.T) {
	items := []SelectionItem{
		{Kind: "unit", ID: 10},
		{Kind: "unit", ID: 20},
		{Kind: "doodad", ID: 30},
	}

	tests := []struct {
		name    string
		primary any // nil → omitted
		want    int
	}{
		{"omitted defaults to last", nil, 2},
		{"explicit index 0", 0, 0},
		{"explicit index 1", 1, 1},
		{"kind:id selector", "doodad:30", 2},
		{"kind:id selector middle", "unit:20", 1},
		{"bare id selector", "10", 0},
		{"out-of-range index falls back to last", 99, 2},
		{"unknown selector falls back to last", "unit:999", 2},
	}

	for _, tc := range tests {
		t.Run(tc.name, func(t *testing.T) {
			payload := map[string]any{"items": items}
			if tc.primary != nil {
				payload["primary"] = tc.primary
			}
			raw, _ := json.Marshal(payload)
			res, err := handleSelectionSetWithPrimary(raw)
			if err != nil {
				t.Fatalf("handleSelectionSetWithPrimary: %v", err)
			}
			st, ok := res.(SelectionState)
			if !ok {
				t.Fatalf("result type = %T, want SelectionState", res)
			}
			if st.Primary != tc.want {
				t.Errorf("primary = %d, want %d", st.Primary, tc.want)
			}
			if len(st.Items) != len(items) {
				t.Errorf("items len = %d, want %d", len(st.Items), len(items))
			}
		})
	}
	// Clean up global selection state.
	t.Cleanup(func() { Current.SetSelection(nil, -1) })
}

// TestSelectionSet_EmptyClears asserts an empty items list clears selection
// (Primary == -1) regardless of any supplied primary.
func TestSelectionSet_EmptyClears(t *testing.T) {
	raw, _ := json.Marshal(map[string]any{"items": []SelectionItem{}, "primary": 0})
	res, err := handleSelectionSetWithPrimary(raw)
	if err != nil {
		t.Fatalf("handleSelectionSetWithPrimary: %v", err)
	}
	st := res.(SelectionState)
	if st.Primary != -1 || len(st.Items) != 0 {
		t.Errorf("empty selection = %+v, want {Items:[] Primary:-1}", st)
	}
}

// TestResolveSelectionPrimary_Direct unit-tests the resolver in isolation.
func TestResolveSelectionPrimary_Direct(t *testing.T) {
	items := []SelectionItem{{Kind: "unit", ID: 1}, {Kind: "item", ID: 2}}
	if got := resolveSelectionPrimary(items, nil); got != 1 {
		t.Errorf("nil primary -> %d, want 1 (last)", got)
	}
	if got := resolveSelectionPrimary(items, json.RawMessage("null")); got != 1 {
		t.Errorf("null primary -> %d, want 1 (last)", got)
	}
	if got := resolveSelectionPrimary(items, json.RawMessage(`0`)); got != 0 {
		t.Errorf("index 0 -> %d, want 0", got)
	}
	if got := resolveSelectionPrimary(items, json.RawMessage(`"item:2"`)); got != 1 {
		t.Errorf("item:2 -> %d, want 1", got)
	}
	if got := resolveSelectionPrimary(nil, json.RawMessage(`0`)); got != -1 {
		t.Errorf("empty items -> %d, want -1", got)
	}
}

// TestViewGetMode covers item 3: view.get_mode reports the session's recorded
// pick mode, defaulting to "doodad", and view.set_mode updates it.
func TestViewGetMode(t *testing.T) {
	s := &Session{}
	if got := s.ViewMode(); got != "doodad" {
		t.Errorf("default ViewMode = %q, want \"doodad\"", got)
	}
	s.SetViewMode("terrain")
	if got := s.ViewMode(); got != "terrain" {
		t.Errorf("after SetViewMode(terrain) = %q, want \"terrain\"", got)
	}
	s.SetViewMode("doodad")
	if got := s.ViewMode(); got != "doodad" {
		t.Errorf("after SetViewMode(doodad) = %q, want \"doodad\"", got)
	}
	// Invalid mode is ignored (stays at last valid value).
	s.SetViewMode("bogus")
	if got := s.ViewMode(); got != "doodad" {
		t.Errorf("after SetViewMode(bogus) = %q, want unchanged \"doodad\"", got)
	}
}

// TestHandleViewGetMode exercises the MCP handler shape against Current.
func TestHandleViewGetMode(t *testing.T) {
	Current.SetViewMode("terrain")
	t.Cleanup(func() { Current.SetViewMode("doodad") })
	res, err := handleViewGetMode(nil)
	if err != nil {
		t.Fatalf("handleViewGetMode: %v", err)
	}
	m, ok := res.(map[string]any)
	if !ok {
		t.Fatalf("result type = %T, want map[string]any", res)
	}
	if m["mode"] != "terrain" {
		t.Errorf("mode = %v, want \"terrain\"", m["mode"])
	}
}

// TestHandleViewSetMode_RecordsRequest asserts view.set_mode records the
// requested mode on the session so view.get_mode reflects it.
func TestHandleViewSetMode_RecordsRequest(t *testing.T) {
	Current.SetViewMode("doodad")
	t.Cleanup(func() { Current.SetViewMode("doodad") })
	params, _ := json.Marshal(map[string]any{"mode": "terrain"})
	if _, err := handleViewSetMode(params); err != nil {
		t.Fatalf("handleViewSetMode: %v", err)
	}
	if got := Current.ViewMode(); got != "terrain" {
		t.Errorf("after view.set_mode terrain, ViewMode = %q, want \"terrain\"", got)
	}
}

// TestHandleViewSetMode_Idempotent verifies the SET-not-TOGGLE contract: two
// identical calls land on the requested mode (no oscillation) and the response
// echoes the resulting mode for read-back.
func TestHandleViewSetMode_Idempotent(t *testing.T) {
	Current.SetViewMode("doodad")
	t.Cleanup(func() { Current.SetViewMode("doodad") })
	params, _ := json.Marshal(map[string]any{"mode": "terrain"})
	for i := 0; i < 2; i++ {
		res, err := handleViewSetMode(params)
		if err != nil {
			t.Fatalf("call %d: %v", i, err)
		}
		m := res.(map[string]any)
		if m["mode"] != "terrain" {
			t.Errorf("call %d: response mode = %v, want \"terrain\"", i, m["mode"])
		}
		if got := Current.ViewMode(); got != "terrain" {
			t.Errorf("call %d: ViewMode = %q, want \"terrain\" (must not oscillate)", i, got)
		}
	}
}

// TestHandleViewSetMode_RequiresMode verifies an empty/missing mode is rejected
// (SET needs an explicit target — no more toggle fallback).
func TestHandleViewSetMode_RequiresMode(t *testing.T) {
	if _, err := handleViewSetMode(json.RawMessage(`{}`)); err == nil {
		t.Errorf("handleViewSetMode with no mode should error")
	}
	if _, err := handleViewSetMode(json.RawMessage(`{"mode":"bogus"}`)); err == nil {
		t.Errorf("handleViewSetMode with invalid mode should error")
	}
}

// TestHandleViewSetDoodadCategoryVisible_Idempotent verifies the SET-not-TOGGLE
// contract for doodad visibility: repeated identical calls keep the recorded
// state stable and the response echoes the resulting visibility.
func TestHandleViewSetDoodadCategoryVisible_Idempotent(t *testing.T) {
	const cat = "Trees/Destructibles"
	t.Cleanup(func() { Current.SetDoodadCategoryVisible(cat, true) })
	params, _ := json.Marshal(map[string]any{"category": cat, "visible": false})
	for i := 0; i < 2; i++ {
		res, err := handleViewSetDoodadCategoryVisible(params)
		if err != nil {
			t.Fatalf("call %d: %v", i, err)
		}
		m := res.(map[string]any)
		if m["visible"] != false {
			t.Errorf("call %d: response visible = %v, want false", i, m["visible"])
		}
		if m["category"] != cat {
			t.Errorf("call %d: response category = %v, want %q", i, m["category"], cat)
		}
		if got := Current.GetDoodadCategoryVisible(cat); got != false {
			t.Errorf("call %d: GetDoodadCategoryVisible = %v, want false (must not oscillate)", i, got)
		}
	}
	// Categories never set default to visible; "*" is never shadowed.
	if !Current.GetDoodadCategoryVisible("Structures") {
		t.Errorf("unset category should default visible")
	}
	Current.SetDoodadCategoryVisible("*", false)
	if !Current.GetDoodadCategoryVisible("*") {
		t.Errorf("\"*\" must not be shadowed (always reports default visible)")
	}
}
