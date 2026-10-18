---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_terrain.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"fmt"
)

// Terrain-brush wire handlers. These expose the per-corner terrain mutators
// (terrain_mutate.go) over the MCP bridge. Registered in RegisterAll
// (handlers.go) via reg(). Wire contract (col/row are 0-based corner grid
// indices; height is game-space Z in WC3 world units):
//
//   terrain.set_tile   {col:int, row:int, ground_tile_id:string} -> {ok:true}
//   terrain.set_height {col:int, row:int, height:number}         -> {ok:true}
//   terrain.get_tile   {col:int, row:int}
//        -> {col, row, ground_tile_id:string, height:number}
//
// All mutating handlers route through the undo-aware Session methods, which
// flip dirtyTerrain and fire the terrain entity-changed event. get_tile is a
// pure read-back so an agent can confirm an edit landed without a Save.

// terrainSetTileParams carries the corner coordinates + the target ground tile
// FourCC for terrain.set_tile. The FourCC must already be present in the map's
// ground palette (the brush resolves it to a palette index, it does not grow
// the palette).
type terrainSetTileParams struct {
	Col          int    `json:"col"`
	Row          int    `json:"row"`
	GroundTileID string `json:"ground_tile_id"`
}

func handleTerrainSetTile(params json.RawMessage) (any, error) {
	var p terrainSetTileParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.GroundTileID == "" {
		return nil, fmt.Errorf("ground_tile_id is required")
	}
	if err := Current.SetTerrainTile(p.Col, p.Row, p.GroundTileID); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainSetHeightParams carries the corner coordinates + the new ground height
// (game-space Z) for terrain.set_height.
type terrainSetHeightParams struct {
	Col    int     `json:"col"`
	Row    int     `json:"row"`
	Height float32 `json:"height"`
}

func handleTerrainSetHeight(params json.RawMessage) (any, error) {
	var p terrainSetHeightParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.SetTerrainHeight(p.Col, p.Row, p.Height); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainGetTileParams carries just the corner coordinates for the read-back.
type terrainGetTileParams struct {
	Col int `json:"col"`
	Row int `json:"row"`
}

func handleTerrainGetTile(params json.RawMessage) (any, error) {
	var p terrainGetTileParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	info, err := Current.GetTerrainTile(p.Col, p.Row)
	if err != nil {
		return nil, err
	}
	return info, nil
}

// ---------------------------------------------------------------------------
// Brush handlers — region edits over a footprint (terrain_brush.go). Shared
// shape fields: col/row are the footprint center (0-based corner indices),
// radius is in corner units (0 = single corner), shape is "circle" (default)
// or "square". Each returns {ok:true}.
// ---------------------------------------------------------------------------

// terrainPaintTileParams — terrain.paint_tile. ground_tile_id must already be
// in the map's ground palette.
type terrainPaintTileParams struct {
	Col          int     `json:"col"`
	Row          int     `json:"row"`
	Radius       float64 `json:"radius"`
	Shape        string  `json:"shape"`
	GroundTileID string  `json:"ground_tile_id"`
}

func handleTerrainPaintTile(params json.RawMessage) (any, error) {
	var p terrainPaintTileParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.GroundTileID == "" {
		return nil, fmt.Errorf("ground_tile_id is required")
	}
	if err := Current.PaintTileBrush(p.Col, p.Row, p.Radius, p.Shape, p.GroundTileID); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainBrushHeightParams — terrain.brush_height. mode ∈ {raise,lower,flatten,
// smooth}; strength is game-Z per dab (or a 0..1 fraction for smooth); target is
// the flatten level (game-Z).
type terrainBrushHeightParams struct {
	Col      int     `json:"col"`
	Row      int     `json:"row"`
	Radius   float64 `json:"radius"`
	Shape    string  `json:"shape"`
	Mode     string  `json:"mode"`
	Strength float32 `json:"strength"`
	Target   float32 `json:"target"`
}

func handleTerrainBrushHeight(params json.RawMessage) (any, error) {
	var p terrainBrushHeightParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.HeightBrush(p.Col, p.Row, p.Radius, p.Shape, p.Mode, p.Strength, p.Target); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainBrushCliffParams — terrain.brush_cliff. mode ∈ {raise,lower,set};
// level is the target layer for "set"; cliff_tile_id selects the cliff tileset
// (empty → slot 0).
type terrainBrushCliffParams struct {
	Col         int     `json:"col"`
	Row         int     `json:"row"`
	Radius      float64 `json:"radius"`
	Shape       string  `json:"shape"`
	Mode        string  `json:"mode"`
	Level       int     `json:"level"`
	CliffTileID string  `json:"cliff_tile_id"`
}

func handleTerrainBrushCliff(params json.RawMessage) (any, error) {
	var p terrainBrushCliffParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.CliffBrush(p.Col, p.Row, p.Radius, p.Shape, p.Mode, p.Level, p.CliffTileID); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainBrushRampParams — terrain.brush_ramp. on=true marks the footprint as a
// walkable ramp; false reverts to a sheer cliff wall.
type terrainBrushRampParams struct {
	Col    int     `json:"col"`
	Row    int     `json:"row"`
	Radius float64 `json:"radius"`
	Shape  string  `json:"shape"`
	On     bool    `json:"on"`
}

func handleTerrainBrushRamp(params json.RawMessage) (any, error) {
	var p terrainBrushRampParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.RampBrush(p.Col, p.Row, p.Radius, p.Shape, p.On); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

// terrainBrushWaterParams — terrain.brush_water. mode ∈ {add,remove}: "add"
// floods the footprint with water; "remove" clears it. height (optional) is the
// absolute water-surface Z in game units, applied flat across the footprint; when
// omitted, "add" auto-places the surface just above the ground under the center.
type terrainBrushWaterParams struct {
	Col    int      `json:"col"`
	Row    int      `json:"row"`
	Radius float64  `json:"radius"`
	Shape  string   `json:"shape"`
	Mode   string   `json:"mode"`
	Height *float32 `json:"height"`
}

func handleTerrainBrushWater(params json.RawMessage) (any, error) {
	var p terrainBrushWaterParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	var h float32
	hasHeight := p.Height != nil
	if hasHeight {
		h = *p.Height
	}
	// An explicit height means the caller wants that exact level, so overwrite
	// existing visible water; auto (no height) preserves it.
	if err := Current.WaterBrush(p.Col, p.Row, p.Radius, p.Shape, p.Mode, h, hasHeight, hasHeight); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}
