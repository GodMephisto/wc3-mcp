---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/startloc_mutate_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"encoding/json"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
)

// startLocCount counts sloc entities currently in the session's units file.
func startLocCount(s *Session) int {
	n := 0
	for i := range s.units.Entities {
		if s.units.Entities[i].TypeID == slocTypeID {
			n++
		}
	}
	return n
}

// findStartLoc returns the sloc entity with the given index, or nil.
func findStartLoc(s *Session, index uint32) *unitsdoo.Entity {
	for i := range s.units.Entities {
		if s.units.Entities[i].TypeID == slocTypeID && s.units.Entities[i].Player == index {
			return &s.units.Entities[i]
		}
	}
	return nil
}

// TestCreateStartLocation_FieldsAndEncode verifies create lands a sloc entity
// with the sloc conventions (TypeID "sloc", Player=index, default rotation,
// scale 1.0) and that the file still re-encodes (no skin_id/scale corruption).
func TestCreateStartLocation_FieldsAndEncode(t *testing.T) {
	s, _ := loadUnitsSession(t)
	before := startLocCount(s)

	// Pick an index not already present so the create succeeds.
	idx := uint32(11)
	for findStartLoc(s, idx) != nil {
		idx++
	}
	pos := [3]float32{640, -384, 0}
	cn, err := s.CreateStartLocation(idx, pos, 0)
	if err != nil {
		t.Fatalf("CreateStartLocation: %v", err)
	}
	if got := startLocCount(s); got != before+1 {
		t.Fatalf("sloc count = %d, want %d", got, before+1)
	}
	e := findStartLoc(s, idx)
	if e == nil {
		t.Fatalf("created sloc index=%d not found", idx)
	}
	if e.TypeID != slocTypeID {
		t.Errorf("TypeID = %q, want %q", e.TypeID, slocTypeID)
	}
	if e.CreationNumber != cn {
		t.Errorf("CreationNumber = %d, want %d", e.CreationNumber, cn)
	}
	if e.Player != idx {
		t.Errorf("Player = %d, want %d (index)", e.Player, idx)
	}
	if e.Position != pos {
		t.Errorf("Position = %v, want %v", e.Position, pos)
	}
	if e.Rotation != slocStartRotation {
		t.Errorf("Rotation = %v, want default %v", e.Rotation, slocStartRotation)
	}
	if e.Scale != ([3]float32{1, 1, 1}) {
		t.Errorf("Scale = %v, want [1 1 1]", e.Scale)
	}
	// scaleRaw must be pinned to 1.0 (sloc convention) so Encode reproduces the
	// byte-faithful sloc layout instead of Scale*128.
	if raw := unitsdoo.ScaleRaw(e); raw != ([3]float32{1, 1, 1}) {
		t.Errorf("scaleRaw = %v, want [1 1 1] (sloc convention)", raw)
	}
	if !s.dirtyUnits {
		t.Error("expected dirtyUnits after CreateStartLocation")
	}
	if _, err := unitsdoo.Encode(s.units); err != nil {
		t.Fatalf("Encode after CreateStartLocation: %v", err)
	}
}

// TestCreateStartLocation_DuplicateRejected asserts a second create at the same
// index errors without mutating state.
func TestCreateStartLocation_DuplicateRejected(t *testing.T) {
	s, _ := loadUnitsSession(t)
	idx := uint32(7)
	for findStartLoc(s, idx) != nil {
		idx++
	}
	if _, err := s.CreateStartLocation(idx, [3]float32{1, 2, 3}, 0); err != nil {
		t.Fatalf("CreateStartLocation: %v", err)
	}
	before := len(s.units.Entities)
	if _, err := s.CreateStartLocation(idx, [3]float32{4, 5, 6}, 0); err == nil {
		t.Fatal("expected error creating duplicate start location index")
	}
	if got := len(s.units.Entities); got != before {
		t.Fatalf("count changed on rejected duplicate create: %d", got)
	}
}

// TestCreateStartLocation_UndoRestoresBytes asserts undo of a sloc create
// restores the exact pre-edit bytes (the SetScaleRaw(1.0) layout must survive
// the create→undo→re-encode round trip).
func TestCreateStartLocation_UndoRestoresBytes(t *testing.T) {
	s, preEdit := loadUnitsSession(t)
	idx := uint32(15)
	for findStartLoc(s, idx) != nil {
		idx++
	}
	if _, err := s.CreateStartLocation(idx, [3]float32{100, 200, 0}, 0); err != nil {
		t.Fatalf("CreateStartLocation: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of sloc create did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
}

// TestMoveStartLocation_ByIndexAndUndo asserts move relocates the sloc looked
// up by index and that undo restores its original position.
func TestMoveStartLocation_ByIndexAndUndo(t *testing.T) {
	s, _ := loadUnitsSession(t)
	idx := uint32(3)
	for findStartLoc(s, idx) != nil {
		idx++
	}
	if _, err := s.CreateStartLocation(idx, [3]float32{10, 20, 0}, 0); err != nil {
		t.Fatalf("CreateStartLocation: %v", err)
	}
	want := [3]float32{555, -777, 32}
	if err := s.MoveStartLocation(idx, want); err != nil {
		t.Fatalf("MoveStartLocation: %v", err)
	}
	if e := findStartLoc(s, idx); e == nil || e.Position != want {
		t.Fatalf("after move, position = %v, want %v", e.Position, want)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if e := findStartLoc(s, idx); e == nil || e.Position != ([3]float32{10, 20, 0}) {
		t.Fatalf("after undo, position = %v, want [10 20 0]", e.Position)
	}
	// Moving a non-existent index errors.
	if err := s.MoveStartLocation(0xBEEF, [3]float32{}); err == nil {
		t.Fatal("expected error moving non-existent start location")
	}
}

// TestDeleteStartLocation_ByIndexAndUndo asserts delete removes the sloc by
// index and undo re-inserts it byte-identically.
func TestDeleteStartLocation_ByIndexAndUndo(t *testing.T) {
	s, _ := loadUnitsSession(t)
	idx := uint32(20)
	for findStartLoc(s, idx) != nil {
		idx++
	}
	if _, err := s.CreateStartLocation(idx, [3]float32{1, 2, 3}, 0); err != nil {
		t.Fatalf("CreateStartLocation: %v", err)
	}
	// Snapshot the post-create encoding so undo-of-delete can be byte-checked
	// back to it.
	postCreate, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after create: %v", err)
	}
	if err := s.DeleteStartLocation(idx); err != nil {
		t.Fatalf("DeleteStartLocation: %v", err)
	}
	if findStartLoc(s, idx) != nil {
		t.Fatalf("sloc index=%d still present after delete", idx)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if findStartLoc(s, idx) == nil {
		t.Fatalf("sloc index=%d not restored after undo", idx)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, postCreate) {
		t.Fatalf("undo of sloc delete did not restore post-create bytes (len got=%d want=%d)", len(got), len(postCreate))
	}
	// Deleting a non-existent index errors.
	if err := s.DeleteStartLocation(0xBEEF); err == nil {
		t.Fatal("expected error deleting non-existent start location")
	}
}

// TestListStartLocations_SortedByIndex asserts the list accessor returns slocs
// sorted by start-location index and carries the right index/cn/position.
func TestListStartLocations_SortedByIndex(t *testing.T) {
	s, _ := loadUnitsSession(t)
	// Add a couple slocs at out-of-order indices on top of any in the fixture.
	hi := uint32(40)
	for findStartLoc(s, hi) != nil {
		hi++
	}
	lo := hi + 5
	if _, err := s.CreateStartLocation(lo, [3]float32{9, 9, 0}, 0); err != nil {
		t.Fatalf("CreateStartLocation lo: %v", err)
	}
	if _, err := s.CreateStartLocation(hi, [3]float32{8, 8, 0}, 0); err != nil {
		t.Fatalf("CreateStartLocation hi: %v", err)
	}
	list := s.ListStartLocations()
	if len(list) < 2 {
		t.Fatalf("list len = %d, want >= 2", len(list))
	}
	for i := 1; i < len(list); i++ {
		if list[i-1].Index > list[i].Index {
			t.Fatalf("list not sorted by index: %d before %d", list[i-1].Index, list[i].Index)
		}
	}
}

// TestHandleStartLocations_Wire exercises the MCP handlers end-to-end against
// the shared Current session (create → list → move → delete).
func TestHandleStartLocations_Wire(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	s, _ := loadUnitsSession(t)
	Current = s

	res, err := handleStartLocationsCreate(json.RawMessage(`{"index":21,"x":300,"y":400,"z":0}`))
	if err != nil {
		t.Fatalf("handleStartLocationsCreate: %v", err)
	}
	r, ok := res.(entityCreateResponse)
	if !ok || r.CreationNumber == 0 {
		t.Fatalf("create response = %#v", res)
	}
	listRes, err := handleStartLocationsList(nil)
	if err != nil {
		t.Fatalf("handleStartLocationsList: %v", err)
	}
	m, ok := listRes.(map[string]any)
	if !ok {
		t.Fatalf("list response type = %T", listRes)
	}
	if _, ok := m["start_locations"].([]StartLocationInfo); !ok {
		t.Fatalf("list response missing start_locations slice: %#v", m)
	}
	if _, err := handleStartLocationsMove(json.RawMessage(`{"index":21,"x":1,"y":2,"z":3}`)); err != nil {
		t.Fatalf("handleStartLocationsMove: %v", err)
	}
	if e := findStartLoc(s, 21); e == nil || e.Position != ([3]float32{1, 2, 3}) {
		t.Fatalf("move via handler did not relocate sloc: %#v", e)
	}
	if _, err := handleStartLocationsDelete(json.RawMessage(`{"index":21}`)); err != nil {
		t.Fatalf("handleStartLocationsDelete: %v", err)
	}
	if findStartLoc(s, 21) != nil {
		t.Fatal("delete via handler did not remove sloc")
	}
}
