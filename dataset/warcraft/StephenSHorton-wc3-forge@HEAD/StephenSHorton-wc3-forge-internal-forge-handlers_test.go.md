---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
)

// TestHandleUnitsMove_InvalidParams asserts the param-unmarshal path surfaces
// a clean error for malformed input. Same shape as the existing handlers.
func TestHandleUnitsMove_InvalidParams(t *testing.T) {
	if _, err := handleUnitsMove(json.RawMessage(`{"creation_number":"not-a-number"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleDoodadsMove_InvalidParams mirrors the units variant: malformed
// JSON should surface a wrapped unmarshal error, not a panic.
func TestHandleDoodadsMove_InvalidParams(t *testing.T) {
	if _, err := handleDoodadsMove(json.RawMessage(`{"creation_number":"not-a-number"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleDoodadsMove_NoMap asserts the "no map loaded" path returns an
// error rather than silently no-op-ing — keeps the bridge's contract loud
// about the precondition.
func TestHandleDoodadsMove_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	_, err := handleDoodadsMove(json.RawMessage(`{"creation_number":1,"x":0,"y":0,"z":0}`))
	if err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// TestHandleMapSave_MPQSentinelMessage exercises the error-mapping branch:
// when Session.Save returns a wrapped ErrMPQRepackFailed (which now
// happens only when an MPQ-backed source cannot repack — here, a source with
// no archive/path), the handler turns it into the user-visible message instead
// of letting the wrapped sentinel leak.
//
// With the pure-Go MPQ writer landed, the failure surfaces at flush() time
// (the per-file write now buffers); a real .w3x with a valid path saves
// successfully. This test uses a degenerate no-path source to hit the guarded
// fallback without a fixture.
func TestHandleMapSave_MPQSentinelMessage(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })

	s := &Session{}
	s.loaded = true
	s.path = "fake.w3x"
	// No path + no archive => write buffers, flush returns the wrapped sentinel.
	s.source = newMPQSource(nil, "")
	// Set dirtyUnits + a units file so Save reaches the write+flush path. Empty
	// Entities slice is fine — Encode handles it.
	s.units = &unitsdoo.File{}
	s.dirtyUnits = true
	Current = s

	_, err := handleMapSave(nil)
	if err == nil {
		t.Fatal("expected error from MPQ-backed save with no repack target")
	}
	if !strings.Contains(err.Error(), "extract the map to a folder") {
		t.Errorf("expected user-visible MPQ message, got %q", err.Error())
	}
}

// TestHandleUnitsRotate_InvalidParams asserts the param-unmarshal path surfaces
// a clean error for malformed input.
func TestHandleUnitsRotate_InvalidParams(t *testing.T) {
	if _, err := handleUnitsRotate(json.RawMessage(`{"creation_number":"bad"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleUnitsRotate_NoMap asserts the "no map loaded" path returns an error.
func TestHandleUnitsRotate_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	_, err := handleUnitsRotate(json.RawMessage(`{"creation_number":1,"rotation":1.57}`))
	if err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// TestHandleDoodadsRotate_InvalidParams mirrors the units variant.
func TestHandleDoodadsRotate_InvalidParams(t *testing.T) {
	if _, err := handleDoodadsRotate(json.RawMessage(`{"creation_number":"bad"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleDoodadsRotate_NoMap asserts "no map loaded" returns an error.
func TestHandleDoodadsRotate_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	_, err := handleDoodadsRotate(json.RawMessage(`{"creation_number":1,"rotation":1.57}`))
	if err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// TestHandleUnitsScale_InvalidParams asserts malformed input returns an error.
func TestHandleUnitsScale_InvalidParams(t *testing.T) {
	if _, err := handleUnitsScale(json.RawMessage(`{"creation_number":"bad"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleUnitsScale_NoMap asserts "no map loaded" returns an error.
func TestHandleUnitsScale_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	_, err := handleUnitsScale(json.RawMessage(`{"creation_number":1,"sx":1.0,"sy":1.0,"sz":1.0}`))
	if err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// TestHandleDoodadsScale_InvalidParams mirrors the units variant.
func TestHandleDoodadsScale_InvalidParams(t *testing.T) {
	if _, err := handleDoodadsScale(json.RawMessage(`{"creation_number":"bad"}`)); err == nil {
		t.Fatal("expected error for malformed creation_number")
	}
}

// TestHandleDoodadsScale_NoMap asserts "no map loaded" returns an error.
func TestHandleDoodadsScale_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	_, err := handleDoodadsScale(json.RawMessage(`{"creation_number":1,"sx":1.0,"sy":1.0,"sz":1.0}`))
	if err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// TestHandleMapSave_OK confirms the happy path: a folder-backed session with
// no dirty edits returns ok:true.
func TestHandleMapSave_OK(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })

	tmp := t.TempDir()
	s := &Session{}
	s.loaded = true
	s.path = tmp
	s.source = folderSource{root: tmp}
	// dirtyUnits=false so Save short-circuits without needing a real units file.
	Current = s

	res, err := handleMapSave(nil)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	m, ok := res.(map[string]any)
	if !ok {
		t.Fatalf("expected map response, got %T", res)
	}
	if m["ok"] != true {
		t.Errorf("expected ok:true, got %v", m["ok"])
	}
}

// TestHandleWindowSetTitle_RoundTrip asserts the agent label is stored on the
// session and surfaced in the response. Doesn't need a loaded map — the label
// is session-global state, not per-map.
func TestHandleWindowSetTitle_RoundTrip(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}

	res, err := handleWindowSetTitle(json.RawMessage(`{"label":"testing cliff fix"}`))
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	r, ok := res.(windowSetTitleResponse)
	if !ok {
		t.Fatalf("expected windowSetTitleResponse, got %T", res)
	}
	if !r.OK || r.Label != "testing cliff fix" {
		t.Errorf("unexpected response: %+v", r)
	}
	if got := Current.AgentLabel(); got != "testing cliff fix" {
		t.Errorf("session AgentLabel = %q, want %q", got, "testing cliff fix")
	}
}

// TestHandleWindowSetTitle_EmptyClears asserts the empty-string case is
// accepted (not rejected as "missing label") so agents can clear their tag.
func TestHandleWindowSetTitle_EmptyClears(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	Current.SetAgentLabel("stale label")

	if _, err := handleWindowSetTitle(json.RawMessage(`{"label":""}`)); err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if got := Current.AgentLabel(); got != "" {
		t.Errorf("AgentLabel after clear = %q, want empty", got)
	}
}

// TestHandleWindowSetTitle_InvalidParams asserts malformed JSON returns a
// clean error rather than panicking.
func TestHandleWindowSetTitle_InvalidParams(t *testing.T) {
	if _, err := handleWindowSetTitle(json.RawMessage(`{"label":123}`)); err == nil {
		t.Fatal("expected error for non-string label")
	}
}

// TestSessionAgentLabel_NotifiesOnce confirms the listener fires exactly once
// per distinct value — repeated SetAgentLabel("foo") calls are no-ops so the
// window-title refresh chain doesn't churn.
func TestSessionAgentLabel_NotifiesOnce(t *testing.T) {
	s := &Session{}
	var fires []string
	s.OnAgentLabelChanged(func(l string) { fires = append(fires, l) })

	s.SetAgentLabel("first")
	s.SetAgentLabel("first") // duplicate — no-op
	s.SetAgentLabel("second")
	s.SetAgentLabel("")

	want := []string{"first", "second", ""}
	if len(fires) != len(want) {
		t.Fatalf("got %d fires, want %d (%v)", len(fires), len(want), fires)
	}
	for i := range want {
		if fires[i] != want[i] {
			t.Errorf("fire %d = %q, want %q", i, fires[i], want[i])
		}
	}
}

// TestHandleUnitsList_Filter exercises the optional filter param. Sets up a
// synthetic session with a mix of TypeIDs + players, calls handleUnitsList
// four times (no filter, type_id only, player only, both), and asserts the
// returned entity slice matches the expected subset each time. Verifies that
// Player=0 is a real filter value (the pointer-based optional distinguishes
// absent from zero).
func TestHandleUnitsList_Filter(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })

	Current = &Session{loaded: true}
	Current.units = &unitsdoo.File{
		Entities: []unitsdoo.Entity{
			{CreationNumber: 1, TypeID: "ngol", Player: 27},
			{CreationNumber: 2, TypeID: "ngol", Player: 0},
			{CreationNumber: 3, TypeID: "hfoo", Player: 0},
			{CreationNumber: 4, TypeID: "hfoo", Player: 1},
			{CreationNumber: 5, TypeID: "ntav", Player: 27},
		},
	}

	cases := []struct {
		name   string
		params string
		want   []uint32 // expected creation_numbers
	}{
		{"no filter", `{}`, []uint32{1, 2, 3, 4, 5}},
		{"empty params", ``, []uint32{1, 2, 3, 4, 5}},
		{"type_id only", `{"filter":{"type_id":"ngol"}}`, []uint32{1, 2}},
		{"player only", `{"filter":{"player":27}}`, []uint32{1, 5}},
		{"player zero is real", `{"filter":{"player":0}}`, []uint32{2, 3}},
		{"both filters", `{"filter":{"type_id":"hfoo","player":0}}`, []uint32{3}},
		{"no match", `{"filter":{"type_id":"xxxx"}}`, []uint32{}},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			res, err := handleUnitsList(json.RawMessage(tc.params))
			if err != nil {
				t.Fatalf("unexpected error: %v", err)
			}
			m, ok := res.(map[string]any)
			if !ok {
				t.Fatalf("expected map response, got %T", res)
			}
			ents, ok := m["entities"].([]unitsListEntity)
			if !ok {
				t.Fatalf("entities key not a []unitsListEntity, got %T", m["entities"])
			}
			got := make([]uint32, 0, len(ents))
			for _, e := range ents {
				got = append(got, e.CreationNumber)
			}
			if len(got) != len(tc.want) {
				t.Fatalf("got %d entities (%v), want %d (%v)", len(got), got, len(tc.want), tc.want)
			}
			for i := range tc.want {
				if got[i] != tc.want[i] {
					t.Errorf("entity %d = %d, want %d", i, got[i], tc.want[i])
				}
			}
		})
	}
}

// TestHandleUnitsList_InvalidParams asserts malformed JSON surfaces a clean
// error rather than panicking.
func TestHandleUnitsList_InvalidParams(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{loaded: true, units: &unitsdoo.File{}}
	if _, err := handleUnitsList(json.RawMessage(`{"filter":{"player":"not-a-number"}}`)); err == nil {
		t.Fatal("expected error for malformed player filter")
	}
}
