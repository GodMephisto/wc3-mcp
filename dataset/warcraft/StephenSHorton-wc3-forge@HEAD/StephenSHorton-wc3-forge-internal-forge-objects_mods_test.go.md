---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_mods_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3objmod"
)

// stubUnitsBase replaces the lazy-loaded units base + metadata with a tiny
// synthetic set so the write tests don't depend on a real CASC mount. We
// install one stock unit ("hpea") with one field ("name", FourCC "unam"),
// plus a stand-in metadata row whose FieldMap binds them together.
//
// Restore the base + reset the per-kind cache on test exit so other tests
// don't see the stub.
func stubUnitsBase(t *testing.T) {
	t.Helper()
	resetObjectBaseCacheForTest("units")
	prevReader := baseAssetReader
	baseAssetReader = nil
	t.Cleanup(func() {
		baseAssetReader = prevReader
		resetObjectBaseCacheForTest("units")
	})
	stubBase := slk.New()
	stubBase.Rows = map[string]slk.MappedRow{
		"hpea": slk.MappedRow{"name": "Peasant", "race": "human"},
	}
	stubMeta := &UnitMetadata{
		Fields: []UnitFieldMeta{
			{
				ID: "unam", Field: "Name", Category: "text", Type: "string",
				UseUnit: true, UseHero: true, UseBuilding: true,
			},
			{
				ID: "uhpm", Field: "HP", Category: "stats", Type: "int",
				UseUnit: true, UseHero: true, UseBuilding: true,
			},
		},
		ByID: map[string]*UnitFieldMeta{},
	}
	for i := range stubMeta.Fields {
		stubMeta.ByID[stubMeta.Fields[i].ID] = &stubMeta.Fields[i]
	}
	setObjectBaseForTest("units", stubBase, stubMeta)
}

// stubKindBase is the generic counterpart of stubUnitsBase used by the
// Phase-2b per-kind table tests. Installs one stock row + one metadata
// field for the named kind, with the Applies* flag set via applies(). The
// row has the named race column populated when race != "".
//
// All existing per-kind cache state is cleared up-front + restored on
// cleanup so tests don't leak into each other.
func stubKindBase(t *testing.T, kind, rowID, race, fieldFourCC, fieldColumn string, applies func(*ObjectFieldMeta)) {
	t.Helper()
	resetObjectBaseCacheForTest(kind)
	prevReader := baseAssetReader
	baseAssetReader = nil
	t.Cleanup(func() {
		baseAssetReader = prevReader
		resetObjectBaseCacheForTest(kind)
	})
	stubBase := slk.New()
	row := slk.MappedRow{"name": "StockRow"}
	if race != "" {
		row["race"] = race
	}
	stubBase.Rows = map[string]slk.MappedRow{rowID: row}
	stubMeta := &ObjectMetadata{
		Fields: []ObjectFieldMeta{
			{ID: fieldFourCC, Field: fieldColumn, Category: "text", Type: "string"},
		},
		ByID: map[string]*ObjectFieldMeta{},
	}
	applies(&stubMeta.Fields[0])
	stubMeta.ByID[fieldFourCC] = &stubMeta.Fields[0]
	setObjectBaseForTest(kind, stubBase, stubMeta)
}

// minimalFolderMap creates a tempdir folder-map containing only a
// war3map.w3i (the required file Session.Open insists on). Returns the
// tempdir path. The map has no units, no doodads, no terrain — just enough
// to satisfy Open.
func minimalFolderMap(t *testing.T) string {
	t.Helper()
	tmp := t.TempDir()
	// Smallest valid w3i: version 28, magic "HM3W" wrapper isn't used by
	// the parser (it sees raw w3i bytes). Easier: copy from a fixture if
	// available; else use the survival-game extracted map.
	src := `C:\Users\4step\projects\wc3-survival-game\map\extracted\war3map.w3i`
	if _, err := os.Stat(src); err != nil {
		t.Skipf("no war3map.w3i fixture at %s", src)
	}
	data, err := os.ReadFile(src)
	if err != nil {
		t.Fatalf("read fixture w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(tmp, "war3map.w3i"), data, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	return tmp
}

// TestSetUnitField_StockUnit_Roundtrip verifies editing a stock unit's
// field creates an OriginalEdit row, persists through Save, and re-loads
// as the new value. End-to-end smoke for the write pipeline.
func TestSetUnitField_StockUnit_Roundtrip(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)

	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if s.IsDirty() {
		t.Fatal("freshly-opened session dirty")
	}

	if err := s.SetUnitField("hpea", "name", "TestName"); err != nil {
		t.Fatalf("SetUnitField: %v", err)
	}
	if !s.IsDirty() {
		t.Fatal("expected dirty after SetUnitField")
	}

	// The mutation should land in OriginalEdits keyed by FourCC.
	mods := s.UnitMods()
	if mods == nil {
		t.Fatal("expected unitMods to be allocated")
	}
	if len(mods.OriginalEdits) != 1 || mods.OriginalEdits[0].BaseID != "hpea" {
		t.Fatalf("expected 1 OriginalEdit on hpea, got %+v", mods.OriginalEdits)
	}
	if mods.OriginalEdits[0].Overrides["unam"] != "TestName" {
		t.Errorf("override = %q, want TestName", mods.OriginalEdits[0].Overrides["unam"])
	}

	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if s.IsDirty() {
		t.Fatal("expected clean after Save")
	}

	// Reopen the map and verify the override persisted.
	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	mods2 := s2.UnitMods()
	if mods2 == nil {
		t.Fatal("expected unitMods after re-open")
	}
	if len(mods2.OriginalEdits) != 1 {
		t.Fatalf("expected 1 OriginalEdit after re-open, got %d", len(mods2.OriginalEdits))
	}
	// Keys are FourCC (Parse was called with nil FieldMap at Open).
	if mods2.OriginalEdits[0].Overrides["unam"] != "TestName" {
		t.Errorf("post-reopen override = %q, want TestName",
			mods2.OriginalEdits[0].Overrides["unam"])
	}
}

// TestSetUnitField_AcceptsColumnNameAndFourCC verifies the SetUnitField
// caller can pass either "name" or "unam" — the mutator normalizes via
// UnitMetaData to the FourCC the on-disk shadow uses.
func TestSetUnitField_AcceptsColumnNameAndFourCC(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.SetUnitField("hpea", "name", "ByColumn"); err != nil {
		t.Fatalf("SetUnitField by column: %v", err)
	}
	if err := s.SetUnitField("hpea", "unam", "ByFourCC"); err != nil {
		t.Fatalf("SetUnitField by FourCC: %v", err)
	}
	// Both edits should land on the same FourCC slot; the second wins.
	mods := s.UnitMods()
	if mods == nil || len(mods.OriginalEdits) != 1 {
		t.Fatalf("expected 1 edit, got %+v", mods)
	}
	if mods.OriginalEdits[0].Overrides["unam"] != "ByFourCC" {
		t.Errorf("final = %q, want ByFourCC", mods.OriginalEdits[0].Overrides["unam"])
	}
}

// TestSetUnitField_Idempotent verifies same-value SetUnitField is a no-op:
// no dirty flip, no history entry, no entity-changed event.
func TestSetUnitField_Idempotent(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.SetUnitField("hpea", "name", "Same"); err != nil {
		t.Fatalf("SetUnitField initial: %v", err)
	}
	beforeUndoCount := s.HistoryUndoCount()
	var events int
	s.OnEntityChanged(func(EntityChange) { events++ })

	// Second call with same value — no-op expected.
	if err := s.SetUnitField("hpea", "name", "Same"); err != nil {
		t.Fatalf("SetUnitField idempotent: %v", err)
	}
	if s.HistoryUndoCount() != beforeUndoCount {
		t.Errorf("idempotent SetUnitField added history entry")
	}
	if events != 0 {
		t.Errorf("idempotent SetUnitField fired %d events", events)
	}
}

// TestSetUnitField_UndoRedo: edit, undo restores stock, redo re-applies.
// Also tests the hadOverride flag — undoing the FIRST edit on a stock
// unit should DELETE the override row entirely, not leave an empty
// edit row pointing at the base.
func TestSetUnitField_UndoRedo(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}

	if err := s.SetUnitField("hpea", "name", "Edited"); err != nil {
		t.Fatalf("SetUnitField: %v", err)
	}
	if !s.CanUndo() {
		t.Fatal("expected CanUndo after SetUnitField")
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	mods := s.UnitMods()
	// Critical: undoing the first edit on a stock unit should leave the
	// OriginalEdits table EMPTY (no spurious empty row).
	if mods == nil {
		t.Fatal("unitMods should still be allocated after Undo")
	}
	if len(mods.OriginalEdits) != 0 {
		t.Errorf("expected empty OriginalEdits after Undo, got %+v", mods.OriginalEdits)
	}

	if err := s.Redo(); err != nil {
		t.Fatalf("Redo: %v", err)
	}
	mods = s.UnitMods()
	if len(mods.OriginalEdits) != 1 || mods.OriginalEdits[0].Overrides["unam"] != "Edited" {
		t.Errorf("post-Redo OriginalEdits = %+v, want one Edited row", mods.OriginalEdits)
	}
}

// TestAddCustomUnit_RoundtripAndSave: add a custom, save, reopen, see it.
// Confirms the custom-table write path through Encode and the allocator's
// chosen ID survives the round-trip.
func TestAddCustomUnit_RoundtripAndSave(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	chosenID, err := s.AddCustomUnit("", "hpea")
	if err != nil {
		t.Fatalf("AddCustomUnit: %v", err)
	}
	if chosenID == "" {
		t.Fatal("allocator returned empty id")
	}
	if chosenID[0] != 'h' {
		t.Errorf("expected allocator to use base char prefix, got %q", chosenID)
	}
	if !s.IsDirty() {
		t.Fatal("expected dirty after AddCustomUnit")
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}

	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	mods := s2.UnitMods()
	if mods == nil || len(mods.Customs) != 1 {
		t.Fatalf("expected 1 custom after re-open, got %+v", mods)
	}
	if mods.Customs[0].ID != chosenID || mods.Customs[0].BaseID != "hpea" {
		t.Errorf("got %+v, want id=%s base=hpea", mods.Customs[0], chosenID)
	}
}

// TestAddCustomUnit_ExplicitIDCollisionRejected: passing an id that
// already exists in customs or that shadows a stock unit must fail.
func TestAddCustomUnit_ExplicitIDCollisionRejected(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if _, err := s.AddCustomUnit("h001", "hpea"); err != nil {
		t.Fatalf("first AddCustomUnit: %v", err)
	}
	// Same id again — collision.
	if _, err := s.AddCustomUnit("h001", "hpea"); err == nil {
		t.Error("expected error on duplicate custom id")
	}
	// Shadow a stock unit — collision.
	if _, err := s.AddCustomUnit("hpea", "hpea"); err == nil {
		t.Error("expected error on shadowing stock id")
	}
}

// TestDeleteCustomUnit_UndoRestoresOverrides: deletion must snapshot the
// custom's full state so Undo restores not just the row but the per-field
// overrides as well.
func TestDeleteCustomUnit_UndoRestoresOverrides(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	id, err := s.AddCustomUnit("", "hpea")
	if err != nil {
		t.Fatalf("AddCustomUnit: %v", err)
	}
	if err := s.SetUnitField(id, "name", "MyCustom"); err != nil {
		t.Fatalf("SetUnitField on custom: %v", err)
	}
	// Verify the override is on the custom (not on OriginalEdits).
	mods := s.UnitMods()
	if mods.Customs[0].Overrides["unam"] != "MyCustom" {
		t.Fatalf("expected override on custom, got %+v", mods)
	}

	if err := s.DeleteCustomUnit(id); err != nil {
		t.Fatalf("DeleteCustomUnit: %v", err)
	}
	if mods := s.UnitMods(); len(mods.Customs) != 0 {
		t.Errorf("expected 0 customs after delete, got %d", len(mods.Customs))
	}

	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	mods = s.UnitMods()
	if len(mods.Customs) != 1 {
		t.Fatalf("expected 1 custom after Undo, got %d", len(mods.Customs))
	}
	if mods.Customs[0].ID != id || mods.Customs[0].Overrides["unam"] != "MyCustom" {
		t.Errorf("Undo did not restore overrides: %+v", mods.Customs[0])
	}
}

// TestAllocateCustomID_SkipsExisting: the allocator must avoid both stock
// IDs and already-used custom IDs.
func TestAllocateCustomID_SkipsExisting(t *testing.T) {
	stubUnitsBase(t)
	s := &Session{}
	s.unitMods = &w3objmod.File{
		Version: 3,
		Customs: []w3objmod.CustomObject{
			{ID: "h000", BaseID: "hpea"},
			{ID: "h001", BaseID: "hpea"},
		},
	}
	got := allocateCustomID(s, UnitsConfig(), "hpea")
	if got != "h002" {
		t.Errorf("allocator picked %q, want h002 (next free)", got)
	}
}

// TestSetUnitField_UnknownIDErrors: passing an id that's neither a stock
// nor a custom must error and leave the session clean.
func TestSetUnitField_UnknownIDErrors(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.SetUnitField("XXXX", "name", "Whatever"); err == nil {
		t.Fatal("expected error for unknown unit id")
	}
	if s.IsDirty() {
		t.Errorf("session dirty after failed SetUnitField")
	}
}
