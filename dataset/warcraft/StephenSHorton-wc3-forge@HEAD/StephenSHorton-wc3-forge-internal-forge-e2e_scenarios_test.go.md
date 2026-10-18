---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/e2e_scenarios_test.go"
ref: HEAD
---

// Package forge — end-to-end bridge SCENARIO suite.
//
// Where bridge_integration_test.go pins the wire contract (framing, auth, a few
// happy paths), this file is the broad regression battery: it drives whole
// editor workflows over the real JSON-RPC/NDJSON bridge — terrain, units, start
// locations, regions, imports, history (undo/redo/groups), view, gameplay
// constants, diagnostics, and the v1.0.0 save-safety guarantees (atomic save,
// backup-on-save, external-change refusal + force) — and asserts both the wire
// results AND the on-disk effects.
//
// These are the "ci" tier scenarios from the e2e design pass: every one works
// on a bare machine with the committed folder fixture and NO Warcraft III / CASC
// install, so they run in the normal `go test ./...` CI step. The CASC-dependent
// surfaces (Object Editor base SLKs, model thumbnails) live in the PowerShell
// release runner under test/e2e/ instead, which drives the actual built .exe on
// a machine that has WC3 installed.
//
// All helpers (startBridge, rpc, expectOK, expectErr) are shared with
// bridge_integration_test.go in this package.
package forge

import (
	"encoding/json"
	"os"
	"path/filepath"
	"strings"
	"testing"
)

// -----------------------------------------------------------------------------
// Fixture + assertion helpers
// -----------------------------------------------------------------------------

// copyExtractedFixture copies the committed folder map at
// internal/forge/testdata/extracted-map into a fresh t.TempDir() and returns the
// new path. Unlike makeMapFixture (w3i + units.doo only), this fixture ALSO
// carries war3map.w3e (a 9×9 v12 terrain grid) and war3mapUnits.doo (10 units
// incl. Paladin cn=21 Hpal at [-400,300,0], plus start location index 0), so it
// can drive terrain + the full save-safety path. The copy is mutated freely by
// each test without touching the committed fixture.
func copyExtractedFixture(t *testing.T) string {
	t.Helper()
	src := filepath.Join("testdata", "extracted-map")
	entries, err := os.ReadDir(src)
	if err != nil {
		t.Fatalf("read fixture dir %s: %v", src, err)
	}
	dst := t.TempDir()
	for _, e := range entries {
		if e.IsDir() {
			continue // fixture is flat
		}
		b, err := os.ReadFile(filepath.Join(src, e.Name()))
		if err != nil {
			t.Fatalf("read fixture file %s: %v", e.Name(), err)
		}
		if err := os.WriteFile(filepath.Join(dst, e.Name()), b, 0o644); err != nil {
			t.Fatalf("write fixture file %s: %v", e.Name(), err)
		}
	}
	return dst
}

// openExtracted copies the extracted fixture, opens it over the bridge, asserts
// the open succeeded, and returns the folder path for on-disk assertions.
func openExtracted(t *testing.T, port int, tok string) string {
	t.Helper()
	dir := copyExtractedFixture(t)
	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open")
	return dir
}

// mustErrContains asserts the response is a handler error (-32000) whose message
// contains substr. The bridge wraps any handler-returned error as
// "Handler threw: <err>" with code -32000 (see internal/bridge/bridge.go), so we
// match on the message rather than re-asserting the generic code everywhere.
func mustErrContains(t *testing.T, resp rpcResponse, method, substr string) {
	t.Helper()
	if resp.Error == nil {
		t.Fatalf("%s: expected an error containing %q, got ok result=%s", method, substr, string(resp.Result))
	}
	if !strings.Contains(resp.Error.Message, substr) {
		t.Fatalf("%s: error message %q does not contain %q", method, resp.Error.Message, substr)
	}
}

// decodeResult unmarshals resp.Result into v, failing the test on error.
func decodeResult(t *testing.T, resp rpcResponse, method string, v any) {
	t.Helper()
	raw := expectOK(t, resp, method)
	if err := json.Unmarshal(raw, v); err != nil {
		t.Fatalf("%s: decode result %s: %v", method, string(raw), err)
	}
}

// -----------------------------------------------------------------------------
// Terrain
// -----------------------------------------------------------------------------

// TestE2E_Terrain_SetHeightTileRoundTrip edits a corner's height + tile, reads
// them back via terrain.get_tile, then saves + reopens and confirms the edits
// persisted to war3map.w3e on disk. Heights are quantized (raw=int32(z*4)),
// so we compare with a tolerance / on the recovered step.
func TestE2E_Terrain_SetHeightTileRoundTrip(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := openExtracted(t, port, tok)

	// Read the original tile id at (0,0) and pick a DIFFERENT palette id that is
	// already in use somewhere on the map (set_tile requires the FourCC to be in
	// the ground palette — it doesn't grow it).
	type tile struct {
		Col     int     `json:"col"`
		Row     int     `json:"row"`
		Ground  string  `json:"ground_tile_id"`
		Height  float64 `json:"height"`
	}
	var t00 tile
	decodeResult(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": 0, "row": 0}), "terrain.get_tile", &t00)
	if t00.Ground == "" {
		t.Fatalf("terrain.get_tile(0,0) returned empty ground_tile_id")
	}
	var other string
	for _, c := range [][2]int{{8, 8}, {4, 4}, {1, 1}, {8, 0}, {0, 8}} {
		var tx tile
		decodeResult(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": c[0], "row": c[1]}), "terrain.get_tile", &tx)
		if tx.Ground != "" && tx.Ground != t00.Ground {
			other = tx.Ground
			break
		}
	}

	// Set a height and read it back.
	expectOK(t, rpc(t, port, tok, "terrain.set_height", map[string]any{"col": 0, "row": 0, "height": 128.0}), "terrain.set_height")
	var after tile
	decodeResult(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": 0, "row": 0}), "terrain.get_tile", &after)
	if after.Height < 127.0 || after.Height > 129.0 {
		t.Errorf("set_height not reflected: got height=%v want ~128 (quantized)", after.Height)
	}

	// Set a tile if we found a distinct palette id.
	if other != "" {
		expectOK(t, rpc(t, port, tok, "terrain.set_tile", map[string]any{"col": 0, "row": 0, "ground_tile_id": other}), "terrain.set_tile")
		var painted tile
		decodeResult(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": 0, "row": 0}), "terrain.get_tile", &painted)
		if painted.Ground != other {
			t.Errorf("set_tile not reflected: got %q want %q", painted.Ground, other)
		}
	}

	// Save, close, reopen — the height edit must survive on disk.
	expectOK(t, rpc(t, port, tok, "map.save", nil), "map.save")
	if _, err := os.Stat(filepath.Join(dir, "war3map.w3e")); err != nil {
		t.Fatalf("war3map.w3e missing after save: %v", err)
	}
	expectOK(t, rpc(t, port, tok, "map.close", nil), "map.close")
	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open (reopen)")
	var reopened tile
	decodeResult(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": 0, "row": 0}), "terrain.get_tile", &reopened)
	if reopened.Height < 127.0 || reopened.Height > 129.0 {
		t.Errorf("height did not persist across save+reopen: got %v want ~128", reopened.Height)
	}
}

// TestE2E_Terrain_OutOfRange confirms an out-of-bounds corner errors gracefully
// (a JSON-RPC error, not a crash). The fixture grid is 9×9 (valid col/row 0..8).
func TestE2E_Terrain_OutOfRange(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)
	mustErrContains(t, rpc(t, port, tok, "terrain.get_tile", map[string]any{"col": 9999, "row": 9999}), "terrain.get_tile", "")
}

// -----------------------------------------------------------------------------
// Units + start locations
// -----------------------------------------------------------------------------

// TestE2E_Units_CreateSetFieldDelete drives the unit-authoring loop end to end:
// create -> list grows -> set_field (gold/player) persists -> delete -> list
// shrinks, plus the validation errors.
func TestE2E_Units_CreateSetFieldDelete(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type entity struct {
		CreationNumber uint32     `json:"creation_number"`
		TypeID         string     `json:"type_id"`
		Position       [3]float32 `json:"position"`
		Player         uint32     `json:"player"`
	}
	type listResult struct {
		Entities []entity `json:"entities"`
	}

	var before listResult
	decodeResult(t, rpc(t, port, tok, "units.list", nil), "units.list", &before)
	startCount := len(before.Entities)
	if startCount == 0 {
		t.Fatalf("fixture has no units")
	}

	// Create.
	var created struct {
		CreationNumber uint32 `json:"creation_number"`
	}
	decodeResult(t, rpc(t, port, tok, "units.create", map[string]any{"type_id": "hfoo", "player": 0, "x": 512, "y": 512}), "units.create", &created)
	if created.CreationNumber == 0 {
		t.Fatalf("units.create returned creation_number 0")
	}
	newCN := created.CreationNumber

	var grown listResult
	decodeResult(t, rpc(t, port, tok, "units.list", nil), "units.list", &grown)
	if len(grown.Entities) != startCount+1 {
		t.Errorf("after create: count=%d want %d", len(grown.Entities), startCount+1)
	}

	// set_field gold + player.
	expectOK(t, rpc(t, port, tok, "units.set_field", map[string]any{"creation_number": newCN, "field": "gold", "value": 1234}), "units.set_field gold")
	expectOK(t, rpc(t, port, tok, "units.set_field", map[string]any{"creation_number": newCN, "field": "player", "value": 3}), "units.set_field player")
	var afterField listResult
	decodeResult(t, rpc(t, port, tok, "units.list", map[string]any{"filter": map[string]any{"type_id": "hfoo"}}), "units.list filter", &afterField)
	found := false
	for _, e := range afterField.Entities {
		if e.CreationNumber == newCN {
			found = true
			if e.Player != 3 {
				t.Errorf("set_field player not reflected: got %d want 3", e.Player)
			}
		}
	}
	if !found {
		t.Errorf("created unit cn=%d not found in filtered list", newCN)
	}

	// Validation errors.
	mustErrContains(t, rpc(t, port, tok, "units.create", map[string]any{"type_id": "toolong", "player": 0, "x": 0, "y": 0}), "units.create bad len", "must be exactly 4 bytes")
	mustErrContains(t, rpc(t, port, tok, "units.create", map[string]any{"player": 0, "x": 0, "y": 0}), "units.create no type", "type_id is required")
	mustErrContains(t, rpc(t, port, tok, "units.set_field", map[string]any{"creation_number": newCN, "field": "nonsense", "value": 1}), "units.set_field bad field", "unknown unit instance field")
	mustErrContains(t, rpc(t, port, tok, "units.move", map[string]any{"creation_number": 888888, "x": 0, "y": 0, "z": 0}), "units.move bad cn", "no unit with creation_number")

	// Delete -> list shrinks back.
	expectOK(t, rpc(t, port, tok, "units.delete", map[string]any{"creation_number": newCN}), "units.delete")
	var afterDelete listResult
	decodeResult(t, rpc(t, port, tok, "units.list", nil), "units.list (post-delete)", &afterDelete)
	if len(afterDelete.Entities) != startCount {
		t.Errorf("after delete: count=%d want %d", len(afterDelete.Entities), startCount)
	}
	mustErrContains(t, rpc(t, port, tok, "units.delete", map[string]any{"creation_number": 777777}), "units.delete bad cn", "no unit with creation_number")
}

// TestE2E_StartLocations_Lifecycle exercises start_locations list/create/move/delete.
func TestE2E_StartLocations_Lifecycle(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type sloc struct {
		Index          int        `json:"index"`
		CreationNumber uint32     `json:"creation_number"`
		Position       [3]float32 `json:"position"`
	}
	type slocList struct {
		StartLocations []sloc `json:"start_locations"`
	}

	var before slocList
	decodeResult(t, rpc(t, port, tok, "start_locations.list", nil), "start_locations.list", &before)
	startN := len(before.StartLocations)

	// Create at a free index (the fixture occupies index 0). create takes an
	// explicit index; reusing an occupied one errors with "already exists".
	expectOK(t, rpc(t, port, tok, "start_locations.create", map[string]any{"index": 1, "x": 1000, "y": 2000}), "start_locations.create")
	var after slocList
	decodeResult(t, rpc(t, port, tok, "start_locations.list", nil), "start_locations.list (post-create)", &after)
	if len(after.StartLocations) != startN+1 {
		t.Errorf("after create: %d start locations, want %d", len(after.StartLocations), startN+1)
	}

	// Re-creating an occupied index errors gracefully.
	mustErrContains(t, rpc(t, port, tok, "start_locations.create", map[string]any{"index": 0, "x": 0, "y": 0}), "start_locations.create dup", "already exists")

	// Move then delete the one we created; list returns to the original count.
	expectOK(t, rpc(t, port, tok, "start_locations.move", map[string]any{"index": 1, "x": 1500, "y": 2500}), "start_locations.move")
	expectOK(t, rpc(t, port, tok, "start_locations.delete", map[string]any{"index": 1}), "start_locations.delete")
	var afterDelete slocList
	decodeResult(t, rpc(t, port, tok, "start_locations.list", nil), "start_locations.list (post-delete)", &afterDelete)
	if len(afterDelete.StartLocations) != startN {
		t.Errorf("after delete: %d start locations, want %d", len(afterDelete.StartLocations), startN)
	}
}

// -----------------------------------------------------------------------------
// Regions (war3map.w3r)
// -----------------------------------------------------------------------------

// TestE2E_Regions_Lifecycle creates a region, lists/gets/moves/resizes/renames/
// deletes it, exercises the negative paths, and round-trips through save+reopen.
func TestE2E_Regions_Lifecycle(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := openExtracted(t, port, tok)

	type region struct {
		CreationNumber uint32  `json:"creation_number"`
		Name           string  `json:"name"`
		MinX           float64 `json:"min_x"`
		MinY           float64 `json:"min_y"`
		MaxX           float64 `json:"max_x"`
		MaxY           float64 `json:"max_y"`
	}
	type regionList struct {
		Regions []region `json:"regions"`
	}

	// Empty to start.
	var empty regionList
	decodeResult(t, rpc(t, port, tok, "regions.list", nil), "regions.list", &empty)
	if len(empty.Regions) != 0 {
		t.Fatalf("fixture should have no regions, got %d", len(empty.Regions))
	}

	// Create.
	var created struct {
		CreationNumber uint32 `json:"creation_number"`
	}
	decodeResult(t, rpc(t, port, tok, "regions.create", map[string]any{
		"name": "SpawnZone", "min_x": -512, "min_y": -256, "max_x": 512, "max_y": 256,
	}), "regions.create", &created)
	if created.CreationNumber == 0 {
		t.Fatalf("regions.create returned creation_number 0")
	}
	cn := created.CreationNumber

	var afterCreate regionList
	decodeResult(t, rpc(t, port, tok, "regions.list", nil), "regions.list (post-create)", &afterCreate)
	if len(afterCreate.Regions) != 1 || afterCreate.Regions[0].Name != "SpawnZone" {
		t.Fatalf("expected 1 region SpawnZone, got %+v", afterCreate.Regions)
	}

	// Move preserves width/height (1024×512), translates by (dx,dy).
	expectOK(t, rpc(t, port, tok, "regions.move", map[string]any{"creation_number": cn, "dx": 100, "dy": 50}), "regions.move")
	var moved region
	decodeResult(t, rpc(t, port, tok, "regions.get", map[string]any{"creation_number": cn}), "regions.get", &moved)
	if (moved.MaxX-moved.MinX) != 1024 || (moved.MaxY-moved.MinY) != 512 {
		t.Errorf("move did not preserve size: w=%v h=%v want 1024×512", moved.MaxX-moved.MinX, moved.MaxY-moved.MinY)
	}

	// Resize with inverted corners must normalize so min<=max.
	expectOK(t, rpc(t, port, tok, "regions.resize", map[string]any{"creation_number": cn, "min_x": 200, "min_y": 300, "max_x": -200, "max_y": -300}), "regions.resize")
	var resized region
	decodeResult(t, rpc(t, port, tok, "regions.get", map[string]any{"creation_number": cn}), "regions.get (resized)", &resized)
	if resized.MinX > resized.MaxX || resized.MinY > resized.MaxY {
		t.Errorf("resize did not normalize inverted corners: %+v", resized)
	}

	expectOK(t, rpc(t, port, tok, "regions.rename", map[string]any{"creation_number": cn, "name": "Boss Arena"}), "regions.rename")

	// Negative paths.
	mustErrContains(t, rpc(t, port, tok, "regions.get", map[string]any{"creation_number": 99999}), "regions.get bad cn", "creation_number")
	mustErrContains(t, rpc(t, port, tok, "regions.create", map[string]any{"name": "", "min_x": 0, "min_y": 0, "max_x": 1, "max_y": 1}), "regions.create empty name", "name")

	// Save + reopen: the region persists in war3map.w3r.
	expectOK(t, rpc(t, port, tok, "map.save", nil), "map.save")
	if fi, err := os.Stat(filepath.Join(dir, "war3map.w3r")); err != nil || fi.Size() == 0 {
		t.Fatalf("war3map.w3r missing/empty after save: err=%v", err)
	}
	expectOK(t, rpc(t, port, tok, "map.close", nil), "map.close")
	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open (reopen)")
	var reopened regionList
	decodeResult(t, rpc(t, port, tok, "regions.list", nil), "regions.list (reopen)", &reopened)
	if len(reopened.Regions) != 1 {
		t.Errorf("region did not persist across save+reopen: got %d want 1", len(reopened.Regions))
	}

	// Delete.
	cn2 := reopened.Regions[0].CreationNumber
	expectOK(t, rpc(t, port, tok, "regions.delete", map[string]any{"creation_number": cn2}), "regions.delete")
	var afterDelete regionList
	decodeResult(t, rpc(t, port, tok, "regions.list", nil), "regions.list (post-delete)", &afterDelete)
	if len(afterDelete.Regions) != 0 {
		t.Errorf("region not deleted: got %d want 0", len(afterDelete.Regions))
	}
}

// -----------------------------------------------------------------------------
// Imports (war3map.imp) + the v1.0.0 baseline-refresh edge
// -----------------------------------------------------------------------------

// TestE2E_Imports_Lifecycle adds/lists/renames/removes an import and verifies the
// folder source writes the bytes to disk eagerly, plus the negative paths.
func TestE2E_Imports_Lifecycle(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := openExtracted(t, port, tok)

	type importEntry struct {
		Path string `json:"path"`
		Size int    `json:"size"`
	}
	type importList struct {
		Imports []importEntry `json:"imports"`
	}

	var empty importList
	decodeResult(t, rpc(t, port, tok, "imports.list", nil), "imports.list", &empty)
	if len(empty.Imports) != 0 {
		t.Fatalf("fixture should have no imports, got %d", len(empty.Imports))
	}

	// Add a small file (data_base64 of "hello e2e world" = 15 bytes).
	const b64 = "aGVsbG8gZTJlIHdvcmxk" // "hello e2e world"
	expectOK(t, rpc(t, port, tok, "imports.add", map[string]any{"path": `war3mapImported\test.txt`, "data_base64": b64}), "imports.add")
	if _, err := os.Stat(filepath.Join(dir, "war3mapImported", "test.txt")); err != nil {
		t.Errorf("imported file not written to disk: %v", err)
	}
	var afterAdd importList
	decodeResult(t, rpc(t, port, tok, "imports.list", nil), "imports.list (post-add)", &afterAdd)
	if len(afterAdd.Imports) != 1 {
		t.Fatalf("after add: %d imports, want 1", len(afterAdd.Imports))
	}

	// Rename moves the on-disk file too.
	expectOK(t, rpc(t, port, tok, "imports.rename", map[string]any{"old_path": `war3mapImported\test.txt`, "new_path": `war3mapImported\renamed.txt`}), "imports.rename")
	if _, err := os.Stat(filepath.Join(dir, "war3mapImported", "renamed.txt")); err != nil {
		t.Errorf("renamed import not on disk: %v", err)
	}

	// Negative paths.
	mustErrContains(t, rpc(t, port, tok, "imports.add", map[string]any{"path": `..\..\evil.txt`, "data_base64": b64}), "imports.add traversal", "unsafe")
	mustErrContains(t, rpc(t, port, tok, "imports.add", map[string]any{"path": "", "data_base64": b64}), "imports.add empty", "required")
	mustErrContains(t, rpc(t, port, tok, "imports.rename", map[string]any{"old_path": `war3mapImported\nope.dat`, "new_path": `war3mapImported\x.dat`}), "imports.rename missing", "no imported file")

	// Remove.
	expectOK(t, rpc(t, port, tok, "imports.remove", map[string]any{"path": `war3mapImported\renamed.txt`}), "imports.remove")
	var afterRemove importList
	decodeResult(t, rpc(t, port, tok, "imports.list", nil), "imports.list (post-remove)", &afterRemove)
	if len(afterRemove.Imports) != 0 {
		t.Errorf("after remove: %d imports, want 0", len(afterRemove.Imports))
	}
}

// TestE2E_Imports_BaselineRefresh is the v1.0.0 regression for the external-
// change guard's interaction with DIRECT writers: imports.add writes a file
// outside Save's batch commit, so it must refresh the change-detection baseline.
// Without that, the very next map.save would re-stat war3map.imp, see the
// editor's own write, and spuriously refuse with ErrSourceChangedOnDisk. Two
// consecutive saves must both succeed.
func TestE2E_Imports_BaselineRefresh(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	const b64 = "ZWRnZQ==" // "edge"
	expectOK(t, rpc(t, port, tok, "imports.add", map[string]any{"path": `war3mapImported\edge.txt`, "data_base64": b64}), "imports.add")

	r1 := rpc(t, port, tok, "map.save", nil)
	if r1.Error != nil {
		t.Fatalf("first save after imports.add was refused: %s", r1.Error.Message)
	}
	r2 := rpc(t, port, tok, "map.save", nil)
	if r2.Error != nil {
		t.Fatalf("second consecutive save was refused (baseline not consistent): %s", r2.Error.Message)
	}
}

// -----------------------------------------------------------------------------
// Save-safety (the 1.0 headline) over the bridge
// -----------------------------------------------------------------------------

// TestE2E_Save_StaleRefusalThenForce drives the v1.0.0 external-change story end
// to end over the wire: dirty war3map.w3e, tamper it on disk, and confirm a
// normal map.save is REFUSED (with the committed marker) while the tampered bytes
// stay untouched; then map.save{force:true} succeeds and backs the tampered bytes
// up to .bak.
//
// IMPORTANT: the staleness guard only checks files in THIS save's dirty write
// set, so we must dirty the SAME file we tamper (terrain.set_height -> w3e).
func TestE2E_Save_StaleRefusalThenForce(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := openExtracted(t, port, tok)
	w3e := filepath.Join(dir, "war3map.w3e")

	// Clean save #1 establishes a fresh baseline for w3e.
	expectOK(t, rpc(t, port, tok, "terrain.set_height", map[string]any{"col": 0, "row": 0, "height": 64.0}), "terrain.set_height #1")
	expectOK(t, rpc(t, port, tok, "map.save", nil), "map.save #1")

	// External tool changes war3map.w3e on disk.
	orig, err := os.ReadFile(w3e)
	if err != nil {
		t.Fatalf("read w3e: %v", err)
	}
	tampered := append(append([]byte(nil), orig...), 1, 2, 3, 4)
	if err := os.WriteFile(w3e, tampered, 0o644); err != nil {
		t.Fatalf("tamper w3e: %v", err)
	}

	// Dirty w3e again, then save -> must be refused, and the tampered bytes
	// must be left untouched.
	expectOK(t, rpc(t, port, tok, "terrain.set_height", map[string]any{"col": 1, "row": 1, "height": 96.0}), "terrain.set_height #2")
	refused := rpc(t, port, tok, "map.save", nil)
	mustErrContains(t, refused, "map.save (stale)", "changed on disk since it was opened")
	if cur, _ := os.ReadFile(w3e); string(cur) != string(tampered) {
		t.Errorf("refused save clobbered the externally-changed war3map.w3e")
	}

	// Force overwrite: succeeds, and the prior (tampered) bytes are backed up.
	expectOK(t, rpc(t, port, tok, "map.save", map[string]any{"force": true}), "map.save (force)")
	bak, err := os.ReadFile(w3e + ".bak")
	if err != nil {
		t.Fatalf("expected .bak after forced overwrite: %v", err)
	}
	if string(bak) != string(tampered) {
		t.Errorf("backup does not hold the clobbered (tampered) bytes: bak=%d tampered=%d", len(bak), len(tampered))
	}
}

// TestE2E_MapInfo_GetSetUndo exercises map.info_get / map.info_set and confirms
// the info edit is on the undo history.
func TestE2E_MapInfo_GetSetUndo(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	var info struct {
		Name string `json:"Name"` // info_get returns the raw w3i.Info (PascalCase keys)
	}
	decodeResult(t, rpc(t, port, tok, "map.info_get", nil), "map.info_get", &info)
	orig := info.Name

	expectOK(t, rpc(t, port, tok, "map.info_set", map[string]any{"updates": map[string]any{"name": "E2E Renamed"}}), "map.info_set")
	var afterSet struct {
		Name string `json:"Name"`
	}
	decodeResult(t, rpc(t, port, tok, "map.info_get", nil), "map.info_get (post-set)", &afterSet)
	if afterSet.Name != "E2E Renamed" {
		t.Errorf("info_set name not applied: got %q", afterSet.Name)
	}

	expectOK(t, rpc(t, port, tok, "history.undo", nil), "history.undo")
	var afterUndo struct {
		Name string `json:"Name"`
	}
	decodeResult(t, rpc(t, port, tok, "map.info_get", nil), "map.info_get (post-undo)", &afterUndo)
	if afterUndo.Name != orig {
		t.Errorf("undo did not revert info name: got %q want %q", afterUndo.Name, orig)
	}
}

// -----------------------------------------------------------------------------
// History (undo/redo + groups)
// -----------------------------------------------------------------------------

// TestE2E_History_UndoRedo is the core history regression: a units.move flows
// through recordCommand and is fully reversible (undo restores all 3 axes) and
// replayable (redo re-applies). Also covers no-op undo on an empty stack.
func TestE2E_History_UndoRedo(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type entity struct {
		CreationNumber uint32     `json:"creation_number"`
		Position       [3]float32 `json:"position"`
	}
	type listResult struct {
		Entities []entity `json:"entities"`
	}
	posOf := func(cn uint32) [3]float32 {
		var l listResult
		decodeResult(t, rpc(t, port, tok, "units.list", nil), "units.list", &l)
		for _, e := range l.Entities {
			if e.CreationNumber == cn {
				return e.Position
			}
		}
		t.Fatalf("cn=%d not in list", cn)
		return [3]float32{}
	}

	orig := posOf(21)
	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{"creation_number": 21, "x": 512, "y": -256, "z": 64}), "units.move")
	if got := posOf(21); got != [3]float32{512, -256, 64} {
		t.Fatalf("move not applied: got %v", got)
	}

	expectOK(t, rpc(t, port, tok, "history.undo", nil), "history.undo")
	if got := posOf(21); got != orig {
		t.Errorf("undo did not restore position: got %v want %v", got, orig)
	}

	expectOK(t, rpc(t, port, tok, "history.redo", nil), "history.redo")
	if got := posOf(21); got != [3]float32{512, -256, 64} {
		t.Errorf("redo did not re-apply position: got %v", got)
	}

	// Drain + an extra undo on the empty stack is a no-op (not an error).
	expectOK(t, rpc(t, port, tok, "history.undo", nil), "history.undo (drain)")
	expectOK(t, rpc(t, port, tok, "history.undo", nil), "history.undo (empty no-op)")
}

// TestE2E_History_Groups confirms transactional grouping: N mutations between
// begin_group/end_group collapse to ONE undo step, undo is blocked while a group
// is open, end_group without begin errors, and abort_group recovers a dangling group.
func TestE2E_History_Groups(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type histList struct {
		Undo           []any `json:"undo"`
		OpenGroupDepth int   `json:"open_group_depth"`
	}
	depth := func() (int, int) {
		var h histList
		decodeResult(t, rpc(t, port, tok, "history.list", nil), "history.list", &h)
		return len(h.Undo), h.OpenGroupDepth
	}

	// Make a mutation FIRST so the undo stack is non-empty: Session.Undo checks
	// "stack empty?" (no-op) BEFORE "group open?" (error), so the open-group block
	// only manifests as an error when there's actually something to undo.
	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{"creation_number": 21, "x": 50, "y": 50, "z": 0}), "units.move (pre-group)")
	undoBefore, _ := depth()

	expectOK(t, rpc(t, port, tok, "history.begin_group", map[string]any{"label": "Batch move"}), "history.begin_group")
	if _, og := depth(); og != 1 {
		t.Errorf("open_group_depth=%d while group open, want 1", og)
	}
	// Undo is blocked while a group is open (stack is non-empty from the move above).
	mustErrContains(t, rpc(t, port, tok, "history.undo", nil), "history.undo (blocked)", "group")

	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{"creation_number": 21, "x": 100, "y": 100, "z": 0}), "units.move #1")
	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{"creation_number": 21, "x": 200, "y": 200, "z": 0}), "units.move #2")
	expectOK(t, rpc(t, port, tok, "history.end_group", nil), "history.end_group")

	undoAfter, og := depth()
	if og != 0 {
		t.Errorf("open_group_depth=%d after end_group, want 0", og)
	}
	if undoAfter != undoBefore+1 {
		t.Errorf("group added %d undo entries, want exactly 1", undoAfter-undoBefore)
	}

	// end_group / abort_group without a begin error.
	mustErrContains(t, rpc(t, port, tok, "history.end_group", nil), "history.end_group (no begin)", "without")
	mustErrContains(t, rpc(t, port, tok, "history.abort_group", nil), "history.abort_group (no begin)", "without")

	// abort_group recovers a dangling group and unblocks undo.
	expectOK(t, rpc(t, port, tok, "history.begin_group", map[string]any{"label": "Dangling"}), "history.begin_group #2")
	expectOK(t, rpc(t, port, tok, "history.abort_group", nil), "history.abort_group")
	if _, og := depth(); og != 0 {
		t.Errorf("open_group_depth=%d after abort, want 0", og)
	}
	expectOK(t, rpc(t, port, tok, "history.undo", nil), "history.undo (after abort)")
}

// -----------------------------------------------------------------------------
// View, gameplay constants, diagnostics
// -----------------------------------------------------------------------------

// TestE2E_View_Modes covers view.set_mode idempotent SET + get_mode read-back +
// invalid/missing rejection, and view.set_doodad_category_visible.
func TestE2E_View_Modes(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type modeResult struct {
		Mode string `json:"mode"`
	}
	set := func(m string) modeResult {
		var r modeResult
		decodeResult(t, rpc(t, port, tok, "view.set_mode", map[string]any{"mode": m}), "view.set_mode", &r)
		return r
	}
	if r := set("terrain"); r.Mode != "terrain" {
		t.Errorf("set_mode terrain -> %q", r.Mode)
	}
	if r := set("terrain"); r.Mode != "terrain" {
		t.Errorf("idempotent set_mode terrain -> %q", r.Mode)
	}
	var gm modeResult
	decodeResult(t, rpc(t, port, tok, "view.get_mode", nil), "view.get_mode", &gm)
	if gm.Mode != "terrain" {
		t.Errorf("get_mode -> %q want terrain", gm.Mode)
	}
	mustErrContains(t, rpc(t, port, tok, "view.set_mode", map[string]any{"mode": "banana"}), "view.set_mode invalid", "terrain")
	mustErrContains(t, rpc(t, port, tok, "view.set_mode", nil), "view.set_mode missing", "required")

	expectOK(t, rpc(t, port, tok, "view.set_doodad_category_visible", map[string]any{"category": "Trees/Destructibles", "visible": false}), "view.set_doodad_category_visible")
	mustErrContains(t, rpc(t, port, tok, "view.set_doodad_category_visible", map[string]any{"category": "", "visible": true}), "view.set_doodad_category_visible empty", "required")
}

// TestE2E_GameplayConstants round-trips a [Misc] override through apply+get
// (war3mapMisc.txt is map-backed, so this is CASC-free).
func TestE2E_GameplayConstants(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	var applied struct {
		RowsApplied int `json:"rows_applied"`
	}
	decodeResult(t, rpc(t, port, tok, "gameplay_constants.apply", map[string]any{
		"rows": []map[string]any{{"section": "Misc", "key": "GoldPerLumber", "value": "7"}},
	}), "gameplay_constants.apply", &applied)
	if applied.RowsApplied != 1 {
		t.Errorf("rows_applied=%d want 1", applied.RowsApplied)
	}

	var got struct {
		Rows []struct {
			Section string `json:"section"`
			Key     string `json:"key"`
			Value   string `json:"value"`
		} `json:"rows"`
	}
	decodeResult(t, rpc(t, port, tok, "gameplay_constants.get", nil), "gameplay_constants.get", &got)
	found := false
	for _, r := range got.Rows {
		if r.Section == "Misc" && r.Key == "GoldPerLumber" && r.Value == "7" {
			found = true
		}
	}
	if !found {
		t.Errorf("applied gameplay constant not read back: %+v", got.Rows)
	}
}

// TestE2E_Diagnostics_Headless asserts diagnostics.get is well-formed in headless
// (ok:false, no frontend snapshot, Go-side namespaces present) and that
// bridge.panics stays 0 across the whole run — a passive health gate proving no
// handler panicked. Also covers diagnostics.arm round-trip.
func TestE2E_Diagnostics_Headless(t *testing.T) {
	_, port, tok := startBridge(t)
	openExtracted(t, port, tok)

	type diagShape struct {
		OK bool `json:"ok"`
		Go struct {
			Bridge struct {
				Calls  int `json:"calls"`
				Panics int `json:"panics"`
			} `json:"bridge"`
		} `json:"go"`
	}
	getDiag := func(label string) diagShape {
		var d diagShape
		decodeResult(t, rpc(t, port, tok, "diagnostics.get", nil), label, &d)
		return d
	}

	// Baseline. NB: the bridge call/panic counters are PROCESS-GLOBAL singletons
	// (they accumulate across every test in this `go test` process), so we assert
	// our OWN operations don't add panics (a delta), not an absolute count — other
	// tests in the package legitimately exercise recovered-panic paths.
	base := getDiag("diagnostics.get (baseline)")

	for i := 0; i < 3; i++ {
		expectOK(t, rpc(t, port, tok, "bridge.ping", nil), "bridge.ping")
	}

	diag := getDiag("diagnostics.get")
	if diag.OK {
		t.Errorf("diagnostics.get ok=true in headless, want false (no frontend snapshot)")
	}
	if diag.Go.Bridge.Calls <= base.Go.Bridge.Calls {
		t.Errorf("go.bridge.calls did not increase after dispatches: base=%d now=%d", base.Go.Bridge.Calls, diag.Go.Bridge.Calls)
	}
	if diag.Go.Bridge.Panics != base.Go.Bridge.Panics {
		t.Errorf("our operations added %d handler panic(s) (base=%d now=%d)", diag.Go.Bridge.Panics-base.Go.Bridge.Panics, base.Go.Bridge.Panics, diag.Go.Bridge.Panics)
	}

	var arm struct {
		Armed bool `json:"armed"`
	}
	decodeResult(t, rpc(t, port, tok, "diagnostics.arm", map[string]any{"on": true}), "diagnostics.arm on", &arm)
	if !arm.Armed {
		t.Errorf("diagnostics.arm on -> armed=false")
	}
	decodeResult(t, rpc(t, port, tok, "diagnostics.arm", map[string]any{"on": false}), "diagnostics.arm off", &arm)
	if arm.Armed {
		t.Errorf("diagnostics.arm off -> armed=true")
	}
}
