---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_entities.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"fmt"
)

// handlers_entities.go holds the MCP bridge handlers for placed-entity
// create/delete (units + doodads). The matching reg() lines live in
// RegisterAll (handlers.go). Kept separate from handlers.go's body so the
// parallel terrain/MCP agents don't merge-conflict on it.
//
// WIRE-METHOD CONTRACT (exact names + params):
//   units.create    {type_id, player, x, y, z?, rotation?, scale?} -> {creation_number}
//   units.delete    {creation_number} -> {ok:true}
//   doodads.create  {type_id, x, y, z?, rotation?, scale?, variation?} -> {creation_number}
//   doodads.delete  {creation_number} -> {ok:true}
//
// x/y/z are WC3 world units (origin = map center), the same convention as
// units.move / doodads.move. rotation is radians (Z-axis). scale defaults to
// 1.0 when omitted or zero.

// unitsCreateParams matches the units.create wire shape. Z/Rotation/Scale are
// optional — JSON-absent decodes to the Go zero value, and CreateUnit treats
// scale==0 as "default 1.0".
type unitsCreateParams struct {
	TypeID   string  `json:"type_id"`
	Player   uint32  `json:"player"`
	X        float32 `json:"x"`
	Y        float32 `json:"y"`
	Z        float32 `json:"z"`
	Rotation float32 `json:"rotation"`
	Scale    float32 `json:"scale"`
}

type entityCreateResponse struct {
	CreationNumber uint32 `json:"creation_number"`
}

func handleUnitsCreate(params json.RawMessage) (any, error) {
	var p unitsCreateParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.TypeID == "" {
		return nil, fmt.Errorf("type_id is required")
	}
	cn, err := Current.CreateUnit(p.TypeID, p.Player, [3]float32{p.X, p.Y, p.Z}, p.Rotation, p.Scale)
	if err != nil {
		return nil, err
	}
	return entityCreateResponse{CreationNumber: cn}, nil
}

// entityDeleteParams matches units.delete / doodads.delete (both take just a
// creation_number).
type entityDeleteParams struct {
	CreationNumber uint32 `json:"creation_number"`
}

func handleUnitsDelete(params json.RawMessage) (any, error) {
	var p entityDeleteParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteUnit(p.CreationNumber); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// unitsSetFieldParams matches the units.set_field wire shape: a scalar
// per-instance field edit. `value` is a JSON number for the scalar fields and
// `item_drops` is supplied instead for the drop-list field (mutually
// exclusive). Field names are the UnitField* constants in entities_mutate.go.
//
//   units.set_field {creation_number, field, value}              -> {ok:true}
//   units.set_field {creation_number, field:"item_drops",
//                    item_drops:[{item_id, chance}]}             -> {ok:true}
type unitsSetFieldParams struct {
	CreationNumber uint32                 `json:"creation_number"`
	Field          string                 `json:"field"`
	Value          *float64               `json:"value"`
	ItemDrops      []UnitInstanceItemDrop `json:"item_drops"`
}

// handleUnitsSetField sets one per-instance field on a placed unit. Routes the
// item_drops field to the slice setter; everything else takes a numeric value.
func handleUnitsSetField(params json.RawMessage) (any, error) {
	var p unitsSetFieldParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.Field == "" {
		return nil, fmt.Errorf("field is required")
	}
	if p.Field == UnitFieldItemDrops {
		if err := Current.SetUnitInstanceItemDrops(p.CreationNumber, p.ItemDrops); err != nil {
			return nil, err
		}
		return map[string]any{"ok": true}, nil
	}
	if p.Value == nil {
		return nil, fmt.Errorf("value is required for field %q", p.Field)
	}
	if err := Current.SetUnitInstanceField(p.CreationNumber, p.Field, *p.Value); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// doodadsCreateParams matches the doodads.create wire shape. Unlike units it
// has no player and adds an optional variation; Z/Rotation/Scale are optional.
type doodadsCreateParams struct {
	TypeID    string  `json:"type_id"`
	X         float32 `json:"x"`
	Y         float32 `json:"y"`
	Z         float32 `json:"z"`
	Rotation  float32 `json:"rotation"`
	Scale     float32 `json:"scale"`
	Variation uint32  `json:"variation"`
}

func handleDoodadsCreate(params json.RawMessage) (any, error) {
	var p doodadsCreateParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.TypeID == "" {
		return nil, fmt.Errorf("type_id is required")
	}
	cn, err := Current.CreateDoodad(p.TypeID, [3]float32{p.X, p.Y, p.Z}, p.Rotation, p.Scale, p.Variation)
	if err != nil {
		return nil, err
	}
	return entityCreateResponse{CreationNumber: cn}, nil
}

func handleDoodadsDelete(params json.RawMessage) (any, error) {
	var p entityDeleteParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteDoodad(p.CreationNumber); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// ---------------------------------------------------------------------------
// Start locations (sloc entities). Addressed by start-location INDEX (the
// trigger-addressable identity → gg_start_location_<index>), NOT by
// creation_number. See startloc_mutate.go for the on-disk conventions.
//
// WIRE-METHOD CONTRACT:
//   start_locations.list   {} -> {start_locations: [{index, creation_number, position[3], rotation}]}
//   start_locations.create {index, x, y, z?, rotation?} -> {creation_number}
//   start_locations.move   {index, x, y, z?}            -> {ok:true}
//   start_locations.delete {index}                      -> {ok:true}
// ---------------------------------------------------------------------------

// startLocationCreateParams matches start_locations.create. Z/Rotation are
// optional — JSON-absent decodes to the Go zero value, and CreateStartLocation
// treats rotation==0 as "default 3π/2" (the Reforged sloc facing).
type startLocationCreateParams struct {
	Index    uint32  `json:"index"`
	X        float32 `json:"x"`
	Y        float32 `json:"y"`
	Z        float32 `json:"z"`
	Rotation float32 `json:"rotation"`
}

// startLocationMoveParams matches start_locations.move (Z optional).
type startLocationMoveParams struct {
	Index uint32  `json:"index"`
	X     float32 `json:"x"`
	Y     float32 `json:"y"`
	Z     float32 `json:"z"`
}

// startLocationIndexParams matches start_locations.delete (just an index).
type startLocationIndexParams struct {
	Index uint32 `json:"index"`
}

func handleStartLocationsList(params json.RawMessage) (any, error) {
	return map[string]any{"start_locations": Current.ListStartLocations()}, nil
}

func handleStartLocationsCreate(params json.RawMessage) (any, error) {
	var p startLocationCreateParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	cn, err := Current.CreateStartLocation(p.Index, [3]float32{p.X, p.Y, p.Z}, p.Rotation)
	if err != nil {
		return nil, err
	}
	return entityCreateResponse{CreationNumber: cn}, nil
}

func handleStartLocationsMove(params json.RawMessage) (any, error) {
	var p startLocationMoveParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.MoveStartLocation(p.Index, [3]float32{p.X, p.Y, p.Z}); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

func handleStartLocationsDelete(params json.RawMessage) (any, error) {
	var p startLocationIndexParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteStartLocation(p.Index); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}
