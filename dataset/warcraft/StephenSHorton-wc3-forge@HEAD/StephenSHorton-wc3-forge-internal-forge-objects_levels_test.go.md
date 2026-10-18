---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_levels_test.go"
ref: HEAD
---

package forge

import (
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
)

// sampleAbilityMeta mirrors the real Reforged AbilityMetaData.slk shape: it
// has usehero/useunit/useitem/usecreep/useSpecific columns but NO `useAbility`
// column. Before the fix, AppliesToAbilities() gated on useAbility (always
// absent) and silently filtered out EVERY ability field — even `name` (anam).
const sampleAbilityMeta = `ID;PWXL;N;E
B;X10;Y3;D0
C;Y1
C;X1;K"id"
C;X2;K"field"
C;X3;K"category"
C;X4;K"displayName"
C;X5;K"type"
C;X6;K"useHero"
C;X7;K"useUnit"
C;X8;K"useItem"
C;X9;K"useCreep"
C;X10;K"useSpecific"
C;Y2
C;X1;K"anam"
C;X2;K"Name"
C;X3;K"text"
C;X4;K"WESTRING_AE_NAME"
C;X5;K"string"
C;X6;K1
C;X7;K1
C;X8;K1
C;X9;K1
C;X10;K0
C;Y3
C;X1;K"Hbz1"
C;X2;K"Damage"
C;X3;K"data"
C;X4;K"WESTRING_AEVAL_HBZ1"
C;X5;K"int"
C;X6;K1
C;X7;K0
C;X8;K0
C;X9;K0
C;X10;K0
E
`

// TestParseObjectMetadata_AbilityRelaxation is the Bug-2 regression at the
// metadata layer: an ability metadata SLK with no `useAbility` column must
// still report AppliesToAbilities()==true for its fields (the whole SLK is the
// ability field set). Before the fix every ability field was filtered out.
func TestParseObjectMetadata_AbilityRelaxation(t *testing.T) {
	meta, err := ParseObjectMetadata([]byte(sampleAbilityMeta))
	if err != nil {
		t.Fatalf("ParseObjectMetadata: %v", err)
	}
	if len(meta.Fields) != 2 {
		t.Fatalf("Fields len = %d, want 2", len(meta.Fields))
	}
	applies := 0
	for i := range meta.Fields {
		if meta.Fields[i].AppliesToAbilities() {
			applies++
		}
	}
	if applies != 2 {
		t.Errorf("AppliesToAbilities true for %d/2 fields; want all (relaxation failed → fields invisible)", applies)
	}
	anam := meta.ByID["anam"]
	if anam == nil || !anam.AppliesToAbilities() {
		t.Errorf("the ability name field (anam) must be visible: %+v", anam)
	}
}

// sampleUpgradeMeta mirrors UpgradeMetaData.slk: NO use* columns at all.
// AppliesToUpgrades() gated on useUpgrade/useUpgr → always false → all fields
// filtered out before the fix.
const sampleUpgradeMeta = `ID;PWXL;N;E
B;X5;Y2;D0
C;Y1
C;X1;K"id"
C;X2;K"field"
C;X3;K"category"
C;X4;K"displayName"
C;X5;K"type"
C;Y2
C;X1;K"gnam"
C;X2;K"Name"
C;X3;K"text"
C;X4;K"WESTRING_GE_NAME"
C;X5;K"string"
E
`

// TestParseObjectMetadata_UpgradeRelaxation: an upgrade metadata SLK with no
// use* columns must report AppliesToUpgrades()==true (Bug 2 for w3q 'gbpx'
// family — entire SLK is the upgrade field set).
func TestParseObjectMetadata_UpgradeRelaxation(t *testing.T) {
	meta, err := ParseObjectMetadata([]byte(sampleUpgradeMeta))
	if err != nil {
		t.Fatalf("ParseObjectMetadata: %v", err)
	}
	if len(meta.Fields) != 1 {
		t.Fatalf("Fields len = %d, want 1", len(meta.Fields))
	}
	if !meta.Fields[0].AppliesToUpgrades() {
		t.Error("upgrade field must be visible when useUpgrade column is absent")
	}
}

// TestParseObjectMetadata_UnitGatingPreserved confirms the relaxation does NOT
// over-show: UnitMetaData HAS use* columns, so an item-only field (useUnit=0)
// must still report AppliesToUnits()==false. Guards against the relaxation
// accidentally turning every gate off.
func TestParseObjectMetadata_UnitGatingPreserved(t *testing.T) {
	meta, err := ParseObjectMetadata([]byte(sampleUnitMeta))
	if err != nil {
		t.Fatalf("ParseObjectMetadata: %v", err)
	}
	iabi := meta.ByID["iabi"]
	if iabi == nil {
		t.Fatal("missing iabi")
	}
	if iabi.AppliesToUnits() {
		t.Error("item-only field must stay filtered out of the units editor")
	}
	if !iabi.AppliesToItems() {
		t.Error("item field should apply to items")
	}
}

// stubAbilitiesBase installs a synthetic abilities base: one stock ability row
// ("AHbz") plus metadata for a leveled `damage`/Hbz1 field and a `name`/anam
// field, with applicability sourced the SAME way production does (via
// ParseObjectMetadata over the no-useAbility SLK). This proves the read path
// surfaces fields after the Bug-2 fix end-to-end, not just at the unit-test
// level.
func stubAbilitiesBase(t *testing.T) *KindConfig {
	t.Helper()
	resetObjectBaseCacheForTest("abilities")
	prevReader := baseAssetReader
	baseAssetReader = nil
	t.Cleanup(func() {
		baseAssetReader = prevReader
		resetObjectBaseCacheForTest("abilities")
	})
	stubBase := slk.New()
	stubBase.Rows = map[string]slk.MappedRow{
		"AHbz": {"name": "Blizzard", "race": "human"},
	}
	meta, err := ParseObjectMetadata([]byte(sampleAbilityMeta))
	if err != nil {
		t.Fatalf("parse stub ability meta: %v", err)
	}
	setObjectBaseForTest("abilities", stubBase, meta)
	return AbilitiesConfig()
}

// TestAbilitiesFieldsMeta_NonEmpty is the Bug-2 end-to-end assertion: with a
// realistic (no-useAbility) ability metadata loaded, fields_meta for the
// abilities kind must return a non-empty set. Before the fix it returned [].
func TestAbilitiesFieldsMeta_NonEmpty(t *testing.T) {
	cfg := stubAbilitiesBase(t)
	out, err := fieldsMetaForKind(cfg)
	if err != nil {
		t.Fatalf("fieldsMetaForKind: %v", err)
	}
	if len(out) == 0 {
		t.Fatal("abilities fields_meta is empty — ability fields invisible (Bug 2 regressed)")
	}
	foundName := false
	for _, f := range out {
		if f.ID == "anam" {
			foundName = true
		}
	}
	if !foundName {
		t.Error("ability name field (anam) not present in fields_meta")
	}
}

// TestAbilityWriteReadSymmetry is the Bug-3 regression: set_field on an ability
// then get must return the new value. The write path resolves the FourCC with
// no AppliesFn filter while the read path enforces it; after the Bug-2 fix the
// two are symmetric so a written field reads back.
//
// Also exercises Bug-1's consumer wiring: a base-slot edit (level 0) and a
// per-level edit (level 2) both round-trip through the get payload without
// collapsing.
func TestAbilityWriteReadSymmetry(t *testing.T) {
	cfg := stubAbilitiesBase(t)
	tmp := minimalFolderMap(t)
	prevCurrent := Current
	Current = &Session{}
	t.Cleanup(func() {
		Current.Close()
		Current = prevCurrent
	})
	if err := Current.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}

	// Base-slot write (level 0) on the stock ability.
	if err := Current.SetObjectField(cfg, "AHbz", "name", "Frost Storm"); err != nil {
		t.Fatalf("SetObjectField name: %v", err)
	}
	// Per-level writes on the leveled `damage`/Hbz1 field — two distinct
	// values at levels 1 and 2.
	if err := Current.SetObjectFieldLevel(cfg, "AHbz", "damage", 1, "100"); err != nil {
		t.Fatalf("SetObjectFieldLevel L1: %v", err)
	}
	if err := Current.SetObjectFieldLevel(cfg, "AHbz", "damage", 2, "250"); err != nil {
		t.Fatalf("SetObjectFieldLevel L2: %v", err)
	}

	// Read back through getObject — the same path that proved Bug-3 returned
	// [] before the fix.
	got, err := getObject(cfg, "AHbz")
	if err != nil {
		t.Fatalf("getObject: %v", err)
	}
	if len(got.Fields) == 0 {
		t.Fatal("getObject returned no fields for an ability (Bug 2/3 regressed)")
	}
	var nameRow, dmgRow *objectsUnitsField
	for i := range got.Fields {
		switch got.Fields[i].ID {
		case "anam":
			nameRow = &got.Fields[i]
		case "Hbz1":
			dmgRow = &got.Fields[i]
		}
	}
	if nameRow == nil {
		t.Fatal("ability name field not readable back after write (write/read asymmetry)")
	}
	if nameRow.Value != "Frost Storm" {
		t.Errorf("name value = %q, want Frost Storm", nameRow.Value)
	}
	if !nameRow.Overridden {
		t.Error("name field should be marked overridden")
	}
	if dmgRow == nil {
		t.Fatal("leveled damage field not readable back")
	}
	if dmgRow.Levels[1] != "100" {
		t.Errorf("damage level 1 = %q, want 100", dmgRow.Levels[1])
	}
	if dmgRow.Levels[2] != "250" {
		t.Errorf("damage level 2 = %q, want 250 (levels collapsed?)", dmgRow.Levels[2])
	}

	// And the on-disk shadow must carry the two per-level entries (not one
	// collapsed value) — this is the Bug-1 store-layer assertion.
	mods := cfg.GetMods(Current)
	if mods == nil || len(mods.OriginalEdits) != 1 {
		t.Fatalf("expected 1 OriginalEdit on the shadow, got %+v", mods)
	}
	edit := mods.OriginalEdits[0]
	if edit.Overrides["anam"] != "Frost Storm" {
		t.Errorf("base-slot override missing/wrong: %+v", edit.Overrides)
	}
	levelVals := map[uint32]string{}
	for _, lo := range edit.Levels {
		if lo.FourCC == "Hbz1" {
			levelVals[lo.Level] = lo.Value
		}
	}
	if levelVals[1] != "100" || levelVals[2] != "250" {
		t.Errorf("per-level shadow entries collapsed: %+v", edit.Levels)
	}
}

// TestSetObjectFieldLevel_RejectsNonOpt confirms a level>0 write to a non-opt
// kind (units, w3u — no per-level wire slot) is rejected rather than silently
// dropped on Save. Guards the "do no harm" invariant: a value the user can't
// persist must error loudly.
func TestSetObjectFieldLevel_RejectsNonOpt(t *testing.T) {
	stubUnitsBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	err := s.SetObjectFieldLevel(UnitsConfig(), "hpea", "name", 2, "X")
	if err == nil {
		t.Fatal("expected error for level>0 on a non-opt kind")
	}
}

// TestSetObjectFieldLevel_Undo confirms the leveled command's Apply/Revert
// round-trip: set level-2, undo, the per-level entry is gone.
func TestSetObjectFieldLevel_Undo(t *testing.T) {
	cfg := stubAbilitiesBase(t)
	tmp := minimalFolderMap(t)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if err := s.SetObjectFieldLevel(cfg, "AHbz", "damage", 2, "250"); err != nil {
		t.Fatalf("SetObjectFieldLevel: %v", err)
	}
	mods := cfg.GetMods(s)
	if mods == nil || len(mods.OriginalEdits) != 1 || len(mods.OriginalEdits[0].Levels) != 1 {
		t.Fatalf("expected 1 leveled entry, got %+v", mods)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	mods = cfg.GetMods(s)
	// Undo should clear the level entry (and drop the now-empty edit row).
	for _, e := range mods.OriginalEdits {
		for _, lo := range e.Levels {
			if lo.FourCC == "damage" || lo.FourCC == "Hbz1" {
				t.Errorf("leveled entry survived Undo: %+v", lo)
			}
		}
	}
}
