---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/entities_mutate_test.go"
ref: HEAD
---

package forge

import (
	"bytes"
	"encoding/json"
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
)

// Fixtures copied from the format packages' testdata (real maps). Loading these
// into a Session lets the round-trip tests prove that create+delete+undo
// restore the original on-disk bytes exactly.
var (
	unitsFixture   = filepath.Join("testdata", "units_wc3_survival_v1_6.doo")
	doodadsFixture = filepath.Join("testdata", "doodads_enfo_ffb_v2_64f.doo")
)

// loadUnitsSession builds a Session backed by the parsed units fixture. Returns
// the session and the byte-exact pre-edit encoding for undo equality checks.
func loadUnitsSession(t *testing.T) (*Session, []byte) {
	t.Helper()
	data, err := os.ReadFile(unitsFixture)
	if err != nil {
		t.Fatalf("read units fixture: %v", err)
	}
	f, err := unitsdoo.Parse(data)
	if err != nil {
		t.Fatalf("parse units fixture: %v", err)
	}
	preEdit, err := unitsdoo.Encode(f)
	if err != nil {
		t.Fatalf("encode units fixture: %v", err)
	}
	// Sanity: the parsed fixture must re-encode to the original file (this is
	// the invariant our undo equality depends on).
	if !bytes.Equal(preEdit, data) {
		t.Fatalf("units fixture does not round-trip Parse→Encode; cannot trust undo equality")
	}
	s := &Session{loaded: true, units: f}
	return s, preEdit
}

func loadDoodadsSession(t *testing.T) (*Session, []byte) {
	t.Helper()
	data, err := os.ReadFile(doodadsFixture)
	if err != nil {
		t.Fatalf("read doodads fixture: %v", err)
	}
	f, err := doodadsdoo.Parse(data)
	if err != nil {
		t.Fatalf("parse doodads fixture: %v", err)
	}
	preEdit, err := doodadsdoo.Encode(f)
	if err != nil {
		t.Fatalf("encode doodads fixture: %v", err)
	}
	if !bytes.Equal(preEdit, data) {
		t.Fatalf("doodads fixture does not round-trip Parse→Encode; cannot trust undo equality")
	}
	s := &Session{loaded: true, doodads: f}
	return s, preEdit
}

// TestCreateUnit_ListCountAndFields verifies create → list count +1 and that
// get returns an entity carrying the requested fields.
func TestCreateUnit_ListCountAndFields(t *testing.T) {
	s, _ := loadUnitsSession(t)
	before := len(s.units.Entities)

	pos := [3]float32{512, -256, 64}
	cn, err := s.CreateUnit("hfoo", 1, pos, 1.25, 2.0)
	if err != nil {
		t.Fatalf("CreateUnit: %v", err)
	}
	if got := len(s.units.Entities); got != before+1 {
		t.Fatalf("entity count = %d, want %d", got, before+1)
	}

	// Find the new entity and assert requested fields landed.
	var found *unitsdoo.Entity
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == cn {
			found = &s.units.Entities[i]
			break
		}
	}
	if found == nil {
		t.Fatalf("created unit cn=%d not found in list", cn)
	}
	if found.TypeID != "hfoo" {
		t.Errorf("TypeID = %q, want %q", found.TypeID, "hfoo")
	}
	if found.Player != 1 {
		t.Errorf("Player = %d, want 1", found.Player)
	}
	if found.Position != pos {
		t.Errorf("Position = %v, want %v", found.Position, pos)
	}
	if found.Rotation != 1.25 {
		t.Errorf("Rotation = %v, want 1.25", found.Rotation)
	}
	if found.Scale != ([3]float32{2.0, 2.0, 2.0}) {
		t.Errorf("Scale = %v, want [2 2 2]", found.Scale)
	}
	if !s.dirtyUnits {
		t.Error("expected dirtyUnits after CreateUnit")
	}

	// The new creation_number must exceed all pre-existing ones.
	for _, e := range s.units.Entities {
		if e.CreationNumber == cn {
			continue
		}
		if e.CreationNumber >= cn {
			t.Errorf("allocated cn=%d not greater than existing cn=%d", cn, e.CreationNumber)
		}
	}

	// The created entity must itself encode without error (validates RandomData
	// + scale defaults are valid for the writer).
	if _, err := unitsdoo.Encode(s.units); err != nil {
		t.Fatalf("Encode after CreateUnit: %v", err)
	}
}

// TestCreateUnit_UndoRestoresBytes asserts undo of a create removes the unit and
// the file re-encodes to the exact pre-edit bytes.
func TestCreateUnit_UndoRestoresBytes(t *testing.T) {
	s, preEdit := loadUnitsSession(t)
	before := len(s.units.Entities)

	if _, err := s.CreateUnit("hfoo", 2, [3]float32{1, 2, 3}, 0, 0); err != nil {
		t.Fatalf("CreateUnit: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := len(s.units.Entities); got != before {
		t.Fatalf("after undo, count = %d, want %d", got, before)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of create did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
}

// TestDeleteUnit_CountAndUndoRestoresBytes asserts delete → count -1 and undo
// re-adds the unit with byte-identical re-encoding (preservation fields intact).
func TestDeleteUnit_CountAndUndoRestoresBytes(t *testing.T) {
	s, preEdit := loadUnitsSession(t)
	before := len(s.units.Entities)
	if before == 0 {
		t.Fatal("fixture has no units to delete")
	}
	// Delete a middle entity so re-insert-at-index is genuinely exercised.
	victim := s.units.Entities[before/2].CreationNumber

	if err := s.DeleteUnit(victim); err != nil {
		t.Fatalf("DeleteUnit: %v", err)
	}
	if got := len(s.units.Entities); got != before-1 {
		t.Fatalf("after delete, count = %d, want %d", got, before-1)
	}
	// Confirm it's actually gone.
	for _, e := range s.units.Entities {
		if e.CreationNumber == victim {
			t.Fatalf("deleted unit cn=%d still present", victim)
		}
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := len(s.units.Entities); got != before {
		t.Fatalf("after undo, count = %d, want %d", got, before)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of delete did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
}

// TestDeleteUnit_NotFound asserts deleting a non-existent creation_number errors.
func TestDeleteUnit_NotFound(t *testing.T) {
	s, _ := loadUnitsSession(t)
	if err := s.DeleteUnit(0xDEADBEEF); err == nil {
		t.Fatal("expected error deleting non-existent unit")
	}
}

// TestCreateUnit_BadTypeID asserts a non-4-byte type_id is rejected before
// mutating anything.
func TestCreateUnit_BadTypeID(t *testing.T) {
	s, _ := loadUnitsSession(t)
	before := len(s.units.Entities)
	if _, err := s.CreateUnit("hf", 0, [3]float32{}, 0, 0); err == nil {
		t.Fatal("expected error for short type_id")
	}
	if got := len(s.units.Entities); got != before {
		t.Fatalf("count changed on rejected create: %d", got)
	}
}

// TestCreateDoodad_ListCountAndFields mirrors the unit create test for doodads.
func TestCreateDoodad_ListCountAndFields(t *testing.T) {
	s, _ := loadDoodadsSession(t)
	before := len(s.doodads.Doodads)

	pos := [3]float32{128, 64, 0}
	cn, err := s.CreateDoodad("ATtr", pos, 0.5, 1.5, 3)
	if err != nil {
		t.Fatalf("CreateDoodad: %v", err)
	}
	if got := len(s.doodads.Doodads); got != before+1 {
		t.Fatalf("doodad count = %d, want %d", got, before+1)
	}

	var found *doodadsdoo.Doodad
	for i := range s.doodads.Doodads {
		if s.doodads.Doodads[i].CreationNumber == cn {
			found = &s.doodads.Doodads[i]
			break
		}
	}
	if found == nil {
		t.Fatalf("created doodad cn=%d not found", cn)
	}
	if found.TypeID != "ATtr" {
		t.Errorf("TypeID = %q, want %q", found.TypeID, "ATtr")
	}
	if found.Position != pos {
		t.Errorf("Position = %v, want %v", found.Position, pos)
	}
	if found.Rotation != 0.5 {
		t.Errorf("Rotation = %v, want 0.5", found.Rotation)
	}
	// Doodad scale is stored RAW (no /128); 1.5 stays 1.5.
	if found.Scale != ([3]float32{1.5, 1.5, 1.5}) {
		t.Errorf("Scale = %v, want [1.5 1.5 1.5]", found.Scale)
	}
	if found.Variation != 3 {
		t.Errorf("Variation = %d, want 3", found.Variation)
	}
	if !s.dirtyDoodads {
		t.Error("expected dirtyDoodads after CreateDoodad")
	}
	if _, err := doodadsdoo.Encode(s.doodads); err != nil {
		t.Fatalf("Encode after CreateDoodad: %v", err)
	}
}

// TestCreateDoodad_UndoRestoresBytes asserts undo of a doodad create restores
// the exact pre-edit bytes (including the trailing special-doodads section).
func TestCreateDoodad_UndoRestoresBytes(t *testing.T) {
	s, preEdit := loadDoodadsSession(t)
	before := len(s.doodads.Doodads)

	if _, err := s.CreateDoodad("ATtr", [3]float32{10, 20, 30}, 0, 0, 0); err != nil {
		t.Fatalf("CreateDoodad: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := len(s.doodads.Doodads); got != before {
		t.Fatalf("after undo, count = %d, want %d", got, before)
	}
	got, err := doodadsdoo.Encode(s.doodads)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of doodad create did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
}

// TestDeleteDoodad_CountAndUndoRestoresBytes asserts delete → count -1 and undo
// re-adds the doodad byte-identically (skinIDPresent / itemDropSetSizes intact).
func TestDeleteDoodad_CountAndUndoRestoresBytes(t *testing.T) {
	s, preEdit := loadDoodadsSession(t)
	before := len(s.doodads.Doodads)
	if before == 0 {
		t.Fatal("fixture has no doodads to delete")
	}
	victim := s.doodads.Doodads[before/2].CreationNumber

	if err := s.DeleteDoodad(victim); err != nil {
		t.Fatalf("DeleteDoodad: %v", err)
	}
	if got := len(s.doodads.Doodads); got != before-1 {
		t.Fatalf("after delete, count = %d, want %d", got, before-1)
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := len(s.doodads.Doodads); got != before {
		t.Fatalf("after undo, count = %d, want %d", got, before)
	}
	got, err := doodadsdoo.Encode(s.doodads)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of doodad delete did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
}

// TestDeleteDoodad_RedoAfterUndo exercises the Apply (redo) path: delete, undo,
// redo should leave the doodad gone again and count back at before-1.
func TestDeleteDoodad_RedoAfterUndo(t *testing.T) {
	s, _ := loadDoodadsSession(t)
	before := len(s.doodads.Doodads)
	victim := s.doodads.Doodads[0].CreationNumber

	if err := s.DeleteDoodad(victim); err != nil {
		t.Fatalf("DeleteDoodad: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	if got := len(s.doodads.Doodads); got != before-1 {
		t.Fatalf("after redo, count = %d, want %d", got, before-1)
	}
	for _, d := range s.doodads.Doodads {
		if d.CreationNumber == victim {
			t.Fatalf("redo did not re-delete doodad cn=%d", victim)
		}
	}
}

// TestHandleUnitsCreate_Wire exercises the handler param-decode + dispatch end
// to end against the shared Current session, including the {creation_number}
// response shape.
func TestHandleUnitsCreate_Wire(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	s, _ := loadUnitsSession(t)
	Current = s

	res, err := handleUnitsCreate(json.RawMessage(`{"type_id":"hfoo","player":3,"x":100,"y":200,"z":0,"rotation":0.7,"scale":1.5}`))
	if err != nil {
		t.Fatalf("handleUnitsCreate: %v", err)
	}
	r, ok := res.(entityCreateResponse)
	if !ok {
		t.Fatalf("response type = %T, want entityCreateResponse", res)
	}
	if r.CreationNumber == 0 {
		t.Fatal("creation_number is zero")
	}
	// units.delete via handler should remove it.
	if _, err := handleUnitsDelete(json.RawMessage(`{"creation_number":` + itoa(r.CreationNumber) + `}`)); err != nil {
		t.Fatalf("handleUnitsDelete: %v", err)
	}
	for _, e := range s.units.Entities {
		if e.CreationNumber == r.CreationNumber {
			t.Fatalf("unit cn=%d not deleted via handler", r.CreationNumber)
		}
	}
}

// TestHandleEntities_InvalidParams asserts malformed JSON surfaces a clean error
// rather than a panic, matching the existing handler contract.
func TestHandleEntities_InvalidParams(t *testing.T) {
	cases := []struct {
		name string
		fn   func(json.RawMessage) (any, error)
		body string
	}{
		{"units.create", handleUnitsCreate, `{"player":"bad"}`},
		{"units.delete", handleUnitsDelete, `{"creation_number":"bad"}`},
		{"doodads.create", handleDoodadsCreate, `{"variation":"bad"}`},
		{"doodads.delete", handleDoodadsDelete, `{"creation_number":"bad"}`},
	}
	for _, tc := range cases {
		if _, err := tc.fn(json.RawMessage(tc.body)); err == nil {
			t.Errorf("%s: expected error for malformed params", tc.name)
		}
	}
}

// TestHandleUnitsCreate_NoMap asserts the no-map-loaded precondition errors.
func TestHandleUnitsCreate_NoMap(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	Current = &Session{}
	if _, err := handleUnitsCreate(json.RawMessage(`{"type_id":"hfoo","x":0,"y":0}`)); err == nil {
		t.Fatal("expected error when no map loaded")
	}
}

// pickUnitCN returns the creation_number of an arbitrary placed unit from the
// fixture (the first entity) — enough for the per-instance field tests, which
// don't care which unit they edit.
func pickUnitCN(t *testing.T, s *Session) uint32 {
	t.Helper()
	if len(s.units.Entities) == 0 {
		t.Fatal("fixture has no units")
	}
	return s.units.Entities[0].CreationNumber
}

func findUnit(s *Session, cn uint32) *unitsdoo.Entity {
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == cn {
			return &s.units.Entities[i]
		}
	}
	return nil
}

// TestSetUnitInstanceField_ScalarFields covers each scalar field's write +
// dirty flip + value landing.
func TestSetUnitInstanceField_ScalarFields(t *testing.T) {
	s, _ := loadUnitsSession(t)
	cn := pickUnitCN(t, s)

	cases := []struct {
		field string
		value float64
		check func(e *unitsdoo.Entity) bool
	}{
		{UnitFieldGold, 12500, func(e *unitsdoo.Entity) bool { return e.GoldAmount == 12500 }},
		{UnitFieldHPPct, 50, func(e *unitsdoo.Entity) bool { return e.HitPointsPct == 50 }},
		{UnitFieldHPPct, -1, func(e *unitsdoo.Entity) bool { return e.HitPointsPct == -1 }},
		{UnitFieldManaPct, 75, func(e *unitsdoo.Entity) bool { return e.ManaPct == 75 }},
		{UnitFieldPlayer, 5, func(e *unitsdoo.Entity) bool { return e.Player == 5 }},
		{UnitFieldHeroLevel, 7, func(e *unitsdoo.Entity) bool { return e.HeroLevel == 7 }},
		{UnitFieldHeroLevel, 0, func(e *unitsdoo.Entity) bool { return e.HeroLevel == 1 }}, // floored to 1
		{UnitFieldHeroStr, 30, func(e *unitsdoo.Entity) bool { return e.HeroStr == 30 }},
		{UnitFieldTargetAcquisition, 600, func(e *unitsdoo.Entity) bool { return e.TargetAcquisition == 600 }},
		{UnitFieldCustomColor, 3, func(e *unitsdoo.Entity) bool { return e.CustomColor == 3 }},
		{UnitFieldCustomColor, -1, func(e *unitsdoo.Entity) bool { return e.CustomColor == -1 }},
	}
	for _, tc := range cases {
		s.dirtyUnits = false
		if err := s.SetUnitInstanceField(cn, tc.field, tc.value); err != nil {
			t.Fatalf("SetUnitInstanceField(%s=%v): %v", tc.field, tc.value, err)
		}
		e := findUnit(s, cn)
		if e == nil {
			t.Fatalf("unit cn=%d vanished after %s", cn, tc.field)
		}
		if !tc.check(e) {
			t.Errorf("field %s=%v did not land: %+v", tc.field, tc.value, e)
		}
		if !s.dirtyUnits {
			t.Errorf("field %s: expected dirtyUnits", tc.field)
		}
		// Edited fixture must still encode.
		if _, err := unitsdoo.Encode(s.units); err != nil {
			t.Fatalf("Encode after %s: %v", tc.field, err)
		}
	}
}

// TestSetUnitInstanceField_UnknownAndBad asserts validation errors for an
// unknown field, a negative uint32 value, and item_drops routed through the
// scalar setter.
func TestSetUnitInstanceField_UnknownAndBad(t *testing.T) {
	s, _ := loadUnitsSession(t)
	cn := pickUnitCN(t, s)
	if err := s.SetUnitInstanceField(cn, "bogus", 1); err == nil {
		t.Error("expected error for unknown field")
	}
	if err := s.SetUnitInstanceField(cn, UnitFieldGold, -5); err == nil {
		t.Error("expected error for negative gold")
	}
	if err := s.SetUnitInstanceField(cn, UnitFieldItemDrops, 0); err == nil {
		t.Error("expected error routing item_drops through scalar setter")
	}
	if err := s.SetUnitInstanceField(999999, UnitFieldGold, 1); err == nil {
		t.Error("expected error for missing creation_number")
	}
}

// TestSetUnitInstanceField_NoOp asserts setting a field to its current value
// does not flip the dirty flag (Save-pill stability for blur-commit panels).
func TestSetUnitInstanceField_NoOp(t *testing.T) {
	s, _ := loadUnitsSession(t)
	cn := pickUnitCN(t, s)
	cur := findUnit(s, cn).GoldAmount
	s.dirtyUnits = false
	if err := s.SetUnitInstanceField(cn, UnitFieldGold, float64(cur)); err != nil {
		t.Fatalf("SetUnitInstanceField no-op: %v", err)
	}
	if s.dirtyUnits {
		t.Error("no-op edit should not flip dirtyUnits")
	}
}

// TestSetUnitInstanceField_UndoRedoBytes is the core acceptance test: a gold
// edit, undone, must re-encode to the original bytes; redone, must re-apply.
func TestSetUnitInstanceField_UndoRedoBytes(t *testing.T) {
	s, preEdit := loadUnitsSession(t)
	cn := pickUnitCN(t, s)
	orig := findUnit(s, cn).GoldAmount

	if err := s.SetUnitInstanceField(cn, UnitFieldGold, 12500); err != nil {
		t.Fatalf("SetUnitInstanceField: %v", err)
	}
	if findUnit(s, cn).GoldAmount != 12500 {
		t.Fatal("gold did not apply")
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if got := findUnit(s, cn).GoldAmount; got != orig {
		t.Fatalf("after undo gold = %d, want original %d", got, orig)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of field edit did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	if findUnit(s, cn).GoldAmount != 12500 {
		t.Fatal("redo did not re-apply gold")
	}
}

// TestSetUnitInstanceItemDrops_SetGetUndo covers replacing the drop list,
// clearing it, and undo restoring exact bytes (item-drop set boundaries are the
// trickiest preservation field — see feedback_unitsdoo_drop_set_boundaries).
func TestSetUnitInstanceItemDrops_SetGetUndo(t *testing.T) {
	s, preEdit := loadUnitsSession(t)
	cn := pickUnitCN(t, s)

	drops := []UnitInstanceItemDrop{
		{ItemID: "rde1", Chance: 60},
		{ItemID: "rde2", Chance: 40},
	}
	if err := s.SetUnitInstanceItemDrops(cn, drops); err != nil {
		t.Fatalf("SetUnitInstanceItemDrops: %v", err)
	}
	e := findUnit(s, cn)
	if len(e.ItemDrops) != 2 || e.ItemDrops[0].ItemID != "rde1" || e.ItemDrops[1].Chance != 40 {
		t.Fatalf("item drops did not land: %+v", e.ItemDrops)
	}
	// Must still encode (proves itemDropSetSizes was reset consistently).
	if _, err := unitsdoo.Encode(s.units); err != nil {
		t.Fatalf("Encode after set drops: %v", err)
	}

	// Undo restores byte-faithful original.
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	got, err := unitsdoo.Encode(s.units)
	if err != nil {
		t.Fatalf("Encode after undo: %v", err)
	}
	if !bytes.Equal(got, preEdit) {
		t.Fatalf("undo of item-drop edit did not restore exact bytes (len got=%d want=%d)", len(got), len(preEdit))
	}

	// Bad FourCC rejected.
	if err := s.SetUnitInstanceItemDrops(cn, []UnitInstanceItemDrop{{ItemID: "toolong", Chance: 1}}); err == nil {
		t.Error("expected error for non-4-byte item_id")
	}
}

// TestHandleUnitsSetField_Wire exercises the MCP handler param-decode for both
// the scalar value path and the item_drops path against the shared Current.
func TestHandleUnitsSetField_Wire(t *testing.T) {
	prev := Current
	t.Cleanup(func() { Current = prev })
	s, _ := loadUnitsSession(t)
	Current = s
	cn := pickUnitCN(t, s)

	if _, err := handleUnitsSetField(json.RawMessage(`{"creation_number":` + itoa(cn) + `,"field":"gold","value":12500}`)); err != nil {
		t.Fatalf("handleUnitsSetField gold: %v", err)
	}
	if findUnit(s, cn).GoldAmount != 12500 {
		t.Fatal("wire gold edit did not land")
	}
	if _, err := handleUnitsSetField(json.RawMessage(`{"creation_number":` + itoa(cn) + `,"field":"item_drops","item_drops":[{"item_id":"rde1","chance":100}]}`)); err != nil {
		t.Fatalf("handleUnitsSetField item_drops: %v", err)
	}
	if e := findUnit(s, cn); len(e.ItemDrops) != 1 || e.ItemDrops[0].ItemID != "rde1" {
		t.Fatalf("wire item_drops edit did not land: %+v", findUnit(s, cn).ItemDrops)
	}
	// Missing value for a scalar field errors cleanly.
	if _, err := handleUnitsSetField(json.RawMessage(`{"creation_number":` + itoa(cn) + `,"field":"gold"}`)); err == nil {
		t.Error("expected error for scalar field with no value")
	}
}

// itoa is a tiny uint32→string helper to avoid pulling strconv into the test's
// hot loop assertions for a single conversion.
func itoa(v uint32) string {
	if v == 0 {
		return "0"
	}
	var buf [10]byte
	i := len(buf)
	for v > 0 {
		i--
		buf[i] = byte('0' + v%10)
		v /= 10
	}
	return string(buf[i:])
}
