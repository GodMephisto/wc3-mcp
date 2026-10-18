---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/regions_mutate_test.go"
ref: HEAD
---

package forge

import (
	"os"
	"path/filepath"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3r"
)

// regions_mutate_test.go exercises the Region (rect) Editor mutators: the
// in-memory create/move/resize/rename/delete + undo/redo behavior, the
// no-map/no-match error paths, and a full open→edit→save→reopen round-trip
// proving regions persist on disk through w3r.Encode (the Stage-2 acceptance
// gate). The round-trip builds a synthetic folder map from real fixtures (a
// committed war3map.w3i + the wc3-survival war3map.w3r) so it runs
// unconditionally rather than skipping when an external map dir is absent.

var (
	regionsW3IFixture = filepath.Join("testdata", "regions_wc3_survival_v1_6.w3i")
	regionsW3RFixture = filepath.Join("testdata", "regions_wc3_survival_v1_6.w3r")
)

// loadRegionsSession builds an in-memory Session holding the parsed w3r fixture.
// Used by the pure-mutator tests (no disk I/O); the round-trip test uses a real
// folder map instead.
func loadRegionsSession(t *testing.T) *Session {
	t.Helper()
	data, err := os.ReadFile(regionsW3RFixture)
	if err != nil {
		t.Fatalf("read w3r fixture: %v", err)
	}
	f, err := w3r.Parse(data)
	if err != nil {
		t.Fatalf("parse w3r fixture: %v", err)
	}
	return &Session{loaded: true, regions: f}
}

func TestCreateRegion_AppendsAndAllocatesCN(t *testing.T) {
	s := loadRegionsSession(t)
	before := len(s.regions.Regions)

	cn, err := s.CreateRegion("Spawn Zone", -512, -256, 512, 256, "", "", [3]uint8{255, 0, 0})
	if err != nil {
		t.Fatalf("CreateRegion: %v", err)
	}
	if got := len(s.regions.Regions); got != before+1 {
		t.Fatalf("region count = %d, want %d", got, before+1)
	}
	info, ok := findRegionInfoOn(s, cn)
	if !ok {
		t.Fatalf("created region %d not found", cn)
	}
	if info.Name != "Spawn Zone" {
		t.Errorf("name = %q, want %q", info.Name, "Spawn Zone")
	}
	if info.MinX != -512 || info.MinY != -256 || info.MaxX != 512 || info.MaxY != 256 {
		t.Errorf("bounds = %v,%v,%v,%v want -512,-256,512,256", info.MinX, info.MinY, info.MaxX, info.MaxY)
	}
	if info.Color != [3]int{255, 0, 0} {
		t.Errorf("color = %v, want [255 0 0]", info.Color)
	}
	if info.GGRef != "gg_rct_Spawn_Zone" {
		t.Errorf("gg_ref = %q, want gg_rct_Spawn_Zone", info.GGRef)
	}
	if !s.IsDirty() {
		t.Errorf("expected dirty after CreateRegion")
	}
}

// findRegionInfoOn is a test helper: FindRegionInfo reads the global Current,
// but the unit tests use isolated Sessions, so resolve directly off s.
func findRegionInfoOn(s *Session, cn int32) (RegionInfo, bool) {
	for i := range s.regions.Regions {
		if s.regions.Regions[i].CreationNumber == cn {
			return regionToInfo(&s.regions.Regions[i]), true
		}
	}
	return RegionInfo{}, false
}

func TestCreateRegion_NormalizesSwappedBounds(t *testing.T) {
	s := loadRegionsSession(t)
	// Pass corners reversed (a viewport rect-drag from top-right to bottom-left).
	cn, err := s.CreateRegion("Box", 512, 256, -512, -256, "", "", [3]uint8{0, 0, 0})
	if err != nil {
		t.Fatalf("CreateRegion: %v", err)
	}
	info, _ := findRegionInfoOn(s, cn)
	if info.MinX != -512 || info.MinY != -256 || info.MaxX != 512 || info.MaxY != 256 {
		t.Errorf("bounds not normalized: %v,%v,%v,%v", info.MinX, info.MinY, info.MaxX, info.MaxY)
	}
}

func TestCreateRegion_NoMapErrors(t *testing.T) {
	s := &Session{}
	if _, err := s.CreateRegion("X", 0, 0, 1, 1, "", "", [3]uint8{}); err == nil {
		t.Fatalf("expected error creating region with no map loaded")
	}
}

func TestCreateRegion_AllocatesFreshTableWhenAbsent(t *testing.T) {
	// A map that shipped no war3map.w3r: s.regions is nil. The first create
	// must allocate a version-5 table so the region (and file) persist.
	s := &Session{loaded: true}
	cn, err := s.CreateRegion("First", 0, 0, 128, 128, "", "", [3]uint8{1, 2, 3})
	if err != nil {
		t.Fatalf("CreateRegion (no table): %v", err)
	}
	if s.regions == nil || s.regions.Version != 5 {
		t.Fatalf("expected fresh version-5 table, got %+v", s.regions)
	}
	if cn != 1 {
		t.Errorf("first creation_number = %d, want 1", cn)
	}
}

func TestMoveRegion_TranslatesAndPreservesSize(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("Mover", -100, -100, 100, 100, "", "", [3]uint8{})
	if err := s.MoveRegion(cn, 50, -25); err != nil {
		t.Fatalf("MoveRegion: %v", err)
	}
	info, _ := findRegionInfoOn(s, cn)
	if info.MinX != -50 || info.MinY != -125 || info.MaxX != 150 || info.MaxY != 75 {
		t.Errorf("moved bounds = %v,%v,%v,%v want -50,-125,150,75", info.MinX, info.MinY, info.MaxX, info.MaxY)
	}
}

func TestMoveRegion_ZeroDeltaNoOp(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("Still", 0, 0, 10, 10, "", "", [3]uint8{})
	// Clear dirty by saving would need disk; instead assert no new history entry.
	undoBefore := s.HistoryUndoCount()
	if err := s.MoveRegion(cn, 0, 0); err != nil {
		t.Fatalf("MoveRegion (no-op): %v", err)
	}
	if s.HistoryUndoCount() != undoBefore {
		t.Errorf("zero-delta MoveRegion recorded an undo step")
	}
}

func TestResizeRegion_SetsAbsoluteBounds(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("R", 0, 0, 10, 10, "", "", [3]uint8{})
	if err := s.ResizeRegion(cn, -300, -200, 300, 200); err != nil {
		t.Fatalf("ResizeRegion: %v", err)
	}
	info, _ := findRegionInfoOn(s, cn)
	if info.MinX != -300 || info.MinY != -200 || info.MaxX != 300 || info.MaxY != 200 {
		t.Errorf("resized bounds = %v,%v,%v,%v", info.MinX, info.MinY, info.MaxX, info.MaxY)
	}
}

func TestRenameRegion_ChangesNameAndGGRef(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("Old Name", 0, 0, 1, 1, "", "", [3]uint8{})
	if err := s.RenameRegion(cn, "New Name"); err != nil {
		t.Fatalf("RenameRegion: %v", err)
	}
	info, _ := findRegionInfoOn(s, cn)
	if info.Name != "New Name" || info.GGRef != "gg_rct_New_Name" {
		t.Errorf("rename failed: name=%q gg=%q", info.Name, info.GGRef)
	}
}

func TestRenameRegion_RejectsEmpty(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("Has Name", 0, 0, 1, 1, "", "", [3]uint8{})
	if err := s.RenameRegion(cn, ""); err == nil {
		t.Errorf("expected error renaming to empty string")
	}
}

func TestDeleteRegion_RemovesAndErrorsOnUnknown(t *testing.T) {
	s := loadRegionsSession(t)
	cn, _ := s.CreateRegion("Doomed", 0, 0, 1, 1, "", "", [3]uint8{})
	before := len(s.regions.Regions)
	if err := s.DeleteRegion(cn); err != nil {
		t.Fatalf("DeleteRegion: %v", err)
	}
	if got := len(s.regions.Regions); got != before-1 {
		t.Errorf("count after delete = %d, want %d", got, before-1)
	}
	if _, ok := findRegionInfoOn(s, cn); ok {
		t.Errorf("region %d still present after delete", cn)
	}
	if err := s.DeleteRegion(999999); err == nil {
		t.Errorf("expected error deleting unknown creation_number")
	}
}

func TestRegion_UndoRedo(t *testing.T) {
	s := loadRegionsSession(t)
	base := len(s.regions.Regions)

	// Create → undo removes it → redo re-adds it.
	cn, _ := s.CreateRegion("UndoMe", -10, -10, 10, 10, "", "", [3]uint8{9, 9, 9})
	if len(s.regions.Regions) != base+1 {
		t.Fatalf("create count = %d, want %d", len(s.regions.Regions), base+1)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo create: %v", err)
	}
	if len(s.regions.Regions) != base {
		t.Fatalf("after undo count = %d, want %d", len(s.regions.Regions), base)
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo create: %v", err)
	}
	info, ok := findRegionInfoOn(s, cn)
	if !ok {
		t.Fatalf("region gone after redo")
	}
	if info.Color != [3]int{9, 9, 9} {
		t.Errorf("redo lost color: %v", info.Color)
	}

	// Resize → undo restores old bounds → redo re-applies.
	if err := s.ResizeRegion(cn, -100, -100, 100, 100); err != nil {
		t.Fatalf("ResizeRegion: %v", err)
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo resize: %v", err)
	}
	info, _ = findRegionInfoOn(s, cn)
	if info.MinX != -10 || info.MaxX != 10 {
		t.Errorf("undo resize did not restore bounds: %v", info)
	}
	if err := s.Redo(); err != nil {
		t.Fatalf("Redo resize: %v", err)
	}
	info, _ = findRegionInfoOn(s, cn)
	if info.MinX != -100 || info.MaxX != 100 {
		t.Errorf("redo resize did not re-apply: %v", info)
	}

	// Delete → undo restores it.
	if err := s.DeleteRegion(cn); err != nil {
		t.Fatalf("DeleteRegion: %v", err)
	}
	if _, ok := findRegionInfoOn(s, cn); ok {
		t.Fatalf("region present after delete")
	}
	if err := s.Undo(); err != nil {
		t.Fatalf("Undo delete: %v", err)
	}
	if _, ok := findRegionInfoOn(s, cn); !ok {
		t.Fatalf("undo delete did not restore region")
	}
}

// TestRegion_SaveRoundTrip is the Stage-2 acceptance gate: open a synthetic
// folder map, create + edit a region, Save, reopen from disk, and confirm the
// region survived with its mutated state. Also confirms war3map.w3r was written
// (it didn't exist in the source... actually it did — the fixture ships one, so
// this also proves an EXISTING table is extended losslessly).
func TestRegion_SaveRoundTrip(t *testing.T) {
	tmp := t.TempDir()
	w3i, err := os.ReadFile(regionsW3IFixture)
	if err != nil {
		t.Fatalf("read w3i fixture: %v", err)
	}
	w3rData, err := os.ReadFile(regionsW3RFixture)
	if err != nil {
		t.Fatalf("read w3r fixture: %v", err)
	}
	if err := os.WriteFile(filepath.Join(tmp, "war3map.w3i"), w3i, 0o644); err != nil {
		t.Fatalf("write w3i: %v", err)
	}
	if err := os.WriteFile(filepath.Join(tmp, "war3map.w3r"), w3rData, 0o644); err != nil {
		t.Fatalf("write w3r: %v", err)
	}

	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("freshly-opened session is dirty")
	}
	baseCount := 0
	if r := s.Regions(); r != nil {
		baseCount = len(r.Regions)
	}

	cn, err := s.CreateRegion("Persisted Zone", -640, -480, 640, 480, "RAhr", "Doodads\\Test", [3]uint8{12, 34, 56})
	if err != nil {
		t.Fatalf("CreateRegion: %v", err)
	}
	if err := s.RenameRegion(cn, "Renamed Zone"); err != nil {
		t.Fatalf("RenameRegion: %v", err)
	}
	if err := s.ResizeRegion(cn, -100, -50, 100, 50); err != nil {
		t.Fatalf("ResizeRegion: %v", err)
	}
	if !s.IsDirty() {
		t.Fatalf("expected dirty before save")
	}
	if err := s.Save(); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if s.IsDirty() {
		t.Errorf("expected clean after Save")
	}

	// Reopen from disk and confirm the mutated region survived.
	s2 := &Session{}
	if err := s2.Open(tmp); err != nil {
		t.Fatalf("re-Open: %v", err)
	}
	r2 := s2.Regions()
	if r2 == nil {
		t.Fatalf("war3map.w3r missing after reopen")
	}
	if got := len(r2.Regions); got != baseCount+1 {
		t.Fatalf("region count after reopen = %d, want %d", got, baseCount+1)
	}
	var found *w3r.Region
	for i := range r2.Regions {
		if r2.Regions[i].CreationNumber == cn {
			found = &r2.Regions[i]
			break
		}
	}
	if found == nil {
		t.Fatalf("created region %d disappeared on reopen", cn)
	}
	if found.Name != "Renamed Zone" {
		t.Errorf("name after reopen = %q, want Renamed Zone", found.Name)
	}
	if found.Left != -100 || found.Bottom != -50 || found.Right != 100 || found.Top != 50 {
		t.Errorf("bounds after reopen = %v,%v,%v,%v want -100,-50,100,50",
			found.Left, found.Bottom, found.Right, found.Top)
	}
	if found.WeatherID != "RAhr" {
		t.Errorf("weather_id after reopen = %q, want RAhr", found.WeatherID)
	}
	if found.AmbientID != "Doodads\\Test" {
		t.Errorf("ambient_id after reopen = %q, want Doodads\\Test", found.AmbientID)
	}
	if found.Color != [3]uint8{12, 34, 56} {
		t.Errorf("color after reopen = %v, want [12 34 56]", found.Color)
	}
}
