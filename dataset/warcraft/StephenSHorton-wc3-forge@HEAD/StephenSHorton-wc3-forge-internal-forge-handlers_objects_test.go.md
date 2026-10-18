---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_objects_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"testing"
)

// TestHandleObjectsUnitsGet_InvalidParams: bad JSON in → wrapped error out.
func TestHandleObjectsUnitsGet_InvalidParams(t *testing.T) {
	if _, err := handleObjectsUnitsGet(json.RawMessage(`{"id":123}`)); err == nil {
		t.Fatal("expected error for non-string id")
	}
}

// TestHandleObjectsUnitsGet_MissingID: empty id is a precondition violation,
// not a 404 on the underlying merge — the handler should reject before
// touching the data layer.
func TestHandleObjectsUnitsGet_MissingID(t *testing.T) {
	if _, err := handleObjectsUnitsGet(json.RawMessage(`{}`)); err == nil {
		t.Fatal("expected error for empty id")
	}
}

// TestHandleObjectsUnitsList_NoCASC: with no base-asset reader registered
// (the harness's default), the handler should return an empty list rather
// than crash. Confirms graceful degradation when CASC is unreachable —
// matters because wc3-forge starts before CASC is mounted on some paths.
func TestHandleObjectsUnitsList_NoCASC(t *testing.T) {
	// Reset the per-kind base cache so a previous test that happened to
	// load CASC doesn't leak into this one. Tests run in the same process —
	// without this, ordering changes the outcome.
	prevReader := baseAssetReader
	t.Cleanup(func() { baseAssetReader = prevReader })
	baseAssetReader = nil
	// Force re-load by clearing the per-kind sync.Once-guarded slot. The
	// generic loadObjectBase short-circuits after the first call; we want
	// it to run again with the nil reader for the units kind.
	resetObjectBaseCacheForTest("units")

	out, err := handleObjectsUnitsList(json.RawMessage(`{}`))
	if err != nil {
		t.Fatalf("list with no CASC: %v", err)
	}
	rows, ok := out.([]objectsUnitsListEntity)
	if !ok {
		t.Fatalf("expected []objectsUnitsListEntity, got %T", out)
	}
	if len(rows) != 0 {
		t.Errorf("expected empty list with no CASC, got %d rows", len(rows))
	}
}
