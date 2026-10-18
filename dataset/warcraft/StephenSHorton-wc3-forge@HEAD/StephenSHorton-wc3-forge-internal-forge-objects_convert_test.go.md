---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_convert_test.go"
ref: HEAD
---

package forge

import (
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
)

// Convert tests exercise ConvertObject (Phase 3 polish #3). Stub-driven so we
// don't need real CASC: we install a synthetic doodad row + synthetic
// destructable row that share one field FourCC ("bnam" → "name"), so an
// override on the doodad's "name" carries over to the destructable.

// stubKindBaseAlt installs a stub for `kind` without resetting any other
// kind. Used by the convert tests where both the source and destination
// kinds need independent stubs (stubKindBase also clobbers baseAssetReader
// which would deadlock the second installer's cleanup).
func stubKindBaseAlt(t *testing.T, kind, rowID, race, fieldFourCC, fieldColumn string, applies func(*ObjectFieldMeta)) {
	t.Helper()
	resetObjectBaseCacheForTest(kind)
	t.Cleanup(func() { resetObjectBaseCacheForTest(kind) })
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

// TestConvertObject_DoodadToDestructable: build a custom doodad, override its
// name, convert it → expect a new destructable custom with the name carried
// over AND the source custom gone. Then Undo restores everything.
func TestConvertObject_DoodadToDestructable(t *testing.T) {
	// Field FourCCs must match across kinds for the carry-over to fire. Real
	// WC3 uses different FourCCs per kind, but the convert function picks up
	// every override whose FourCC ALSO exists in the destination metadata —
	// so for the stub we install the same FourCC on both sides and prove the
	// translation pipeline works.
	stubKindBaseAlt(t, "doodads", "ATtr", "", "bnam", "name",
		func(f *ObjectFieldMeta) { f.UseDoodad = true })
	stubKindBaseAlt(t, "destructables", "LTlt", "", "bnam", "name",
		func(f *ObjectFieldMeta) { f.UseDestructable = true })

	tmp := minimalFolderMap(t)
	if err := Current.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	t.Cleanup(func() { Current.Close() })

	// Create a doodad custom + override its name.
	doodadID, err := Current.AddCustomObject(DoodadsConfig(), "", "ATtr")
	if err != nil {
		t.Fatalf("AddCustomObject (doodads): %v", err)
	}
	if err := Current.SetObjectField(DoodadsConfig(), doodadID, "name", "ConvertMe"); err != nil {
		t.Fatalf("SetObjectField on doodad custom: %v", err)
	}

	// Convert.
	destID, err := Current.ConvertObject("doodads", doodadID, "destructables")
	if err != nil {
		t.Fatalf("ConvertObject: %v", err)
	}
	if destID == "" {
		t.Fatal("ConvertObject returned empty id")
	}

	// Source custom should be gone.
	if mods := DoodadsConfig().GetMods(Current); mods != nil {
		for _, c := range mods.Customs {
			if c.ID == doodadID {
				t.Errorf("source doodad %q still present post-convert", doodadID)
			}
		}
	}

	// Destination custom must exist with the carried-over override.
	destMods := DestructablesConfig().GetMods(Current)
	if destMods == nil {
		t.Fatal("destructable mods nil post-convert")
	}
	var found bool
	for _, c := range destMods.Customs {
		if c.ID == destID {
			found = true
			if got := c.Overrides["bnam"]; got != "ConvertMe" {
				t.Errorf("override carried over as %q, want ConvertMe", got)
			}
		}
	}
	if !found {
		t.Errorf("destination custom %q not found", destID)
	}

	// Undo unwinds the entire convert: source custom returns, destination
	// custom disappears. Convert wraps everything in a BeginUndoGroup so a
	// single Undo reverses the whole operation.
	if err := Current.Undo(); err != nil {
		t.Fatalf("Undo: %v", err)
	}
	if dm := DestructablesConfig().GetMods(Current); dm != nil {
		for _, c := range dm.Customs {
			if c.ID == destID {
				t.Errorf("destination custom %q survived Undo", destID)
			}
		}
	}
	srcMods := DoodadsConfig().GetMods(Current)
	if srcMods == nil {
		t.Fatal("doodad mods nil post-undo")
	}
	var restored bool
	for _, c := range srcMods.Customs {
		if c.ID == doodadID {
			restored = true
			if c.Overrides["bnam"] != "ConvertMe" {
				t.Errorf("source overrides not restored: %+v", c.Overrides)
			}
		}
	}
	if !restored {
		t.Errorf("source custom %q not restored on Undo", doodadID)
	}
}

// TestConvertObject_RejectsBadPair: any pair other than doodads↔destructables
// must error out cleanly without mutating state.
func TestConvertObject_RejectsBadPair(t *testing.T) {
	stubKindBaseAlt(t, "doodads", "ATtr", "", "bnam", "name",
		func(f *ObjectFieldMeta) { f.UseDoodad = true })
	stubKindBaseAlt(t, "units", "hpea", "human", "unam", "name",
		func(f *ObjectFieldMeta) { f.UseUnit = true })

	tmp := minimalFolderMap(t)
	if err := Current.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	t.Cleanup(func() { Current.Close() })

	if _, err := Current.ConvertObject("doodads", "ATtr", "units"); err == nil {
		t.Error("expected error converting doodad → unit")
	}
	if _, err := Current.ConvertObject("units", "hpea", "doodads"); err == nil {
		t.Error("expected error converting unit → doodad")
	}
	if _, err := Current.ConvertObject("doodads", "ATtr", "doodads"); err == nil {
		t.Error("expected error converting same kind → same kind")
	}
}
