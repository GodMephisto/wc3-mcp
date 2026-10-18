---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_api.go"
ref: HEAD
---

package forge

import (
	"errors"
	"fmt"
)

// Public re-exports of the Object Editor read/write surface for the Wails
// App layer. The generic backed surface (listObjects/getObject/...) lives
// in handlers_objects.go; this file gives package main typed entry points
// that don't go through json.RawMessage.
//
// Phase 2b: generic ListObjects/GetObject/SetObjectField/CreateCustomObject/
// DeleteCustomObject take a `kind` string ("units"/"items"/"abilities"/...)
// and route through kindConfigFor — six MCP-shaped routes collapse to four
// Wails methods. The Phase-1b unit-typed wrappers (ListUnitObjects,
// GetUnitObject, ...) remain as thin aliases so existing callers keep working
// byte-for-byte.

// UnitObjectListEntity is the public alias of the bridge's list-row shape.
// Same JSON tags so a Wails-bound []UnitObjectListEntity and an MCP
// objects.units.list response decode identically on the frontend.
type UnitObjectListEntity = objectsUnitsListEntity

// UnitObjectField is the public alias of the bridge's per-field row shape.
type UnitObjectField = objectsUnitsField

// UnitObjectDetail is the public alias of the bridge's get-result shape.
type UnitObjectDetail = objectsUnitsGetResult

// ObjectListEntity is the kind-agnostic public alias of the list-row shape.
// Identical structure to UnitObjectListEntity (the JSON wire format doesn't
// change per kind), just renamed for the generic surface.
type ObjectListEntity = objectsUnitsListEntity

// ObjectField is the kind-agnostic public alias of the per-field row shape.
type ObjectField = objectsUnitsField

// ObjectDetail is the kind-agnostic public alias of the get-result shape.
type ObjectDetail = objectsUnitsGetResult

// resolveKind is the generic-surface helper that maps a wire-stable kind
// string to the registered KindConfig, returning a clean error rather than
// the kindConfigFor panic for invalid input. Used by every public generic
// method so a bad kind from the JS surface degrades to a toast instead of
// a Wails-bridge stack trace.
func resolveKind(kind string) (*KindConfig, error) {
	if kind == "" {
		return nil, errors.New("kind is required")
	}
	cfg, ok := kindConfigs[kind]
	if !ok {
		return nil, fmt.Errorf("unknown kind %q (expected units/items/abilities/buffs/destructables/doodads/upgrades)", kind)
	}
	return cfg, nil
}

// ListObjects returns the merged tree for any registered kind. Empty slice
// when no CASC is reachable for that kind's MetaData — callers should treat
// that as "empty state", not an error.
func ListObjects(kind string) ([]ObjectListEntity, error) {
	cfg, err := resolveKind(kind)
	if err != nil {
		return nil, err
	}
	return listObjects(cfg)
}

// GetObject returns one merged row's full field table for any registered kind.
// Returns (nil, error) when the id isn't known.
func GetObject(kind, id string) (*ObjectDetail, error) {
	if id == "" {
		return nil, errors.New("id is required")
	}
	cfg, err := resolveKind(kind)
	if err != nil {
		return nil, err
	}
	return getObject(cfg, id)
}

// SetObjectField writes `value` to the named field on the object with FourCC
// `id` for the given kind. column may be FourCC or column-name; the mutator
// normalizes. Returns the post-mutation full detail payload so the caller
// doesn't need a separate Get round-trip.
func SetObjectField(kind, id, column, value string) (*ObjectDetail, error) {
	if id == "" {
		return nil, errors.New("id is required")
	}
	if column == "" {
		return nil, errors.New("column is required")
	}
	cfg, err := resolveKind(kind)
	if err != nil {
		return nil, err
	}
	if err := Current.SetObjectField(cfg, id, column, value); err != nil {
		return nil, err
	}
	return getObject(cfg, id)
}

// CreateCustomObject appends a new custom row of the given kind derived from
// baseID and returns its chosen FourCC ID plus full detail payload. When id
// is empty the allocator picks the next free FourCC starting from baseID's
// first character.
func CreateCustomObject(kind, baseID, id string) (string, *ObjectDetail, error) {
	if baseID == "" {
		return "", nil, errors.New("base_id is required")
	}
	cfg, err := resolveKind(kind)
	if err != nil {
		return "", nil, err
	}
	chosenID, err := Current.AddCustomObject(cfg, id, baseID)
	if err != nil {
		return "", nil, err
	}
	d, err := getObject(cfg, chosenID)
	if err != nil {
		return chosenID, nil, fmt.Errorf("get newly-created %q: %w", chosenID, err)
	}
	return chosenID, d, nil
}

// ConvertObject converts an object from srcKind to dstKind. Returns the new
// destination custom's ID. Today only doodads↔destructables is supported —
// other pairs return an error from Session.ConvertObject. Stock-source rows
// stay in place (we can't delete them); custom-source rows get removed so the
// converted result is the user's intended single replacement.
func ConvertObject(srcKind, srcID, dstKind string) (string, error) {
	if srcID == "" {
		return "", errors.New("src_id is required")
	}
	if _, err := resolveKind(srcKind); err != nil {
		return "", err
	}
	if _, err := resolveKind(dstKind); err != nil {
		return "", err
	}
	return Current.ConvertObject(srcKind, srcID, dstKind)
}

// DeleteCustomObject removes a custom row of the given kind by id. Errors if
// id isn't a custom (stock rows aren't deletable).
func DeleteCustomObject(kind, id string) error {
	if id == "" {
		return errors.New("id is required")
	}
	cfg, err := resolveKind(kind)
	if err != nil {
		return err
	}
	return Current.DeleteCustomObject(cfg, id)
}

// ---------------------------------------------------------------------------
// Phase-1b units-typed wrappers — kept as thin aliases so the existing
// frontend bindings keep working byte-for-byte. New code should target the
// generic ListObjects/GetObject/SetObjectField/CreateCustomObject/
// DeleteCustomObject directly with kind="units".
// ---------------------------------------------------------------------------

// ListUnitObjects returns the merged-units tree. Empty slice when no CASC
// is reachable — callers should treat that as "empty state", not an error.
func ListUnitObjects() ([]UnitObjectListEntity, error) {
	return listObjects(UnitsConfig())
}

// GetUnitObject returns one merged unit's full field table by FourCC id.
func GetUnitObject(id string) (*UnitObjectDetail, error) {
	if id == "" {
		return nil, errors.New("id is required")
	}
	return getObject(UnitsConfig(), id)
}

// SetUnitObjectField writes `value` to the named field on the unit.
func SetUnitObjectField(id, column, value string) (*UnitObjectDetail, error) {
	return SetObjectField("units", id, column, value)
}

// CreateCustomUnitObject appends a new custom unit and returns the chosen ID
// + full detail payload.
func CreateCustomUnitObject(baseID, id string) (string, *UnitObjectDetail, error) {
	return CreateCustomObject("units", baseID, id)
}

// DeleteCustomUnitObject removes the custom unit with the given id.
func DeleteCustomUnitObject(id string) error {
	return DeleteCustomObject("units", id)
}
