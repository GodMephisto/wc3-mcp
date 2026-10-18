---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_regions.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"fmt"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3r"
)

// handlers_regions.go holds the MCP bridge handlers for the Region (rect)
// Editor — create/move/resize/rename/delete plus the read-only list/get
// accessors. The matching reg() lines live in RegisterAll (handlers.go); the
// mutators live in regions_mutate.go. Kept in its own file so it mirrors the
// handlers_entities.go split and doesn't merge-conflict with the big bodies.
//
// WIRE-METHOD CONTRACT (exact names + params):
//   regions.list    {} -> {regions: [RegionInfo, ...]}
//   regions.get     {creation_number} -> RegionInfo
//   regions.create  {name, min_x, min_y, max_x, max_y, weather_id?, ambient_id?,
//                     color?[r,g,b]} -> {creation_number}
//   regions.move    {creation_number, dx, dy} -> {ok:true}
//   regions.resize  {creation_number, min_x, min_y, max_x, max_y} -> {ok:true}
//   regions.rename  {creation_number, name} -> {ok:true}
//   regions.delete  {creation_number} -> {ok:true}
//
// min_x/min_y/max_x/max_y are WC3 world units (origin = map center) — the SAME
// convention as units.move and the placed-entity create surface. They map to
// the w3r Left/Bottom/Right/Top fields verbatim.

// RegionInfo is the full Region row the regions.* surface (and the GUI panel +
// overlay) read. Richer than the trigger-picker's TriggerRegionInfo: it carries
// the weather/ambient/color the editor lets the user tune. The unexported w3r
// pad byte is intentionally omitted (round-trip-only preservation detail).
type RegionInfo struct {
	Name           string  `json:"name"`
	CreationNumber int32   `json:"creation_number"`
	MinX           float32 `json:"min_x"`
	MinY           float32 `json:"min_y"`
	MaxX           float32 `json:"max_x"`
	MaxY           float32 `json:"max_y"`
	WeatherID      string  `json:"weather_id"`  // 4-byte FourCC or "" (none)
	AmbientID      string  `json:"ambient_id"`  // C-string sound label or ""
	Color          [3]int  `json:"color"`       // R,G,B 0..255
	GGRef          string  `json:"gg_ref"`      // gg_rct_<Name> codegen handle
}

// BuildRegionInfos is the exported accessor for Wails. Returns every region as
// a RegionInfo (empty slice when no map / no regions).
func BuildRegionInfos() []RegionInfo {
	r := Current.Regions()
	if r == nil {
		return []RegionInfo{}
	}
	out := make([]RegionInfo, 0, len(r.Regions))
	for i := range r.Regions {
		out = append(out, regionToInfo(&r.Regions[i]))
	}
	return out
}

// FindRegionInfo returns the RegionInfo for the given creation_number (the
// exported single-region accessor for Wails). ok=false when no map is loaded
// or no region matches.
func FindRegionInfo(creationNumber int32) (RegionInfo, bool) {
	r := Current.Regions()
	if r == nil {
		return RegionInfo{}, false
	}
	for i := range r.Regions {
		if r.Regions[i].CreationNumber == creationNumber {
			return regionToInfo(&r.Regions[i]), true
		}
	}
	return RegionInfo{}, false
}

func regionToInfo(rg *w3r.Region) RegionInfo {
	return RegionInfo{
		Name:           rg.Name,
		CreationNumber: rg.CreationNumber,
		MinX:           rg.Left,
		MinY:           rg.Bottom,
		MaxX:           rg.Right,
		MaxY:           rg.Top,
		WeatherID:      rg.WeatherID,
		AmbientID:      rg.AmbientID,
		Color:          [3]int{int(rg.Color[0]), int(rg.Color[1]), int(rg.Color[2])},
		GGRef:          "gg_rct_" + strings.ReplaceAll(rg.Name, " ", "_"),
	}
}

// --- MCP handlers ----------------------------------------------------------

func handleRegionsList(_ json.RawMessage) (any, error) {
	return map[string]any{"regions": BuildRegionInfos()}, nil
}

type regionGetParams struct {
	CreationNumber int32 `json:"creation_number"`
}

func handleRegionsGet(params json.RawMessage) (any, error) {
	var p regionGetParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	info, ok := FindRegionInfo(p.CreationNumber)
	if !ok {
		return nil, fmt.Errorf("no region with creation_number %d", p.CreationNumber)
	}
	return info, nil
}

// regionsCreateParams matches the regions.create wire shape. weather_id /
// ambient_id / color are optional — JSON-absent decodes to the Go zero value
// (empty string / nil slice), which CreateRegion treats as "no weather / no
// ambient / default opaque white".
type regionsCreateParams struct {
	Name      string    `json:"name"`
	MinX      float32   `json:"min_x"`
	MinY      float32   `json:"min_y"`
	MaxX      float32   `json:"max_x"`
	MaxY      float32   `json:"max_y"`
	WeatherID string    `json:"weather_id"`
	AmbientID string    `json:"ambient_id"`
	Color     []int     `json:"color"` // optional [r,g,b] 0..255
}

type regionCreateResponse struct {
	CreationNumber int32 `json:"creation_number"`
}

func handleRegionsCreate(params json.RawMessage) (any, error) {
	var p regionsCreateParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.Name == "" {
		return nil, fmt.Errorf("name is required")
	}
	color, err := ParseRegionColor(p.Color)
	if err != nil {
		return nil, err
	}
	cn, err := Current.CreateRegion(p.Name, p.MinX, p.MinY, p.MaxX, p.MaxY, p.WeatherID, p.AmbientID, color)
	if err != nil {
		return nil, err
	}
	return regionCreateResponse{CreationNumber: cn}, nil
}

// ParseRegionColor maps the optional [r,g,b] array to a [3]uint8. A nil /
// empty slice means "use the default" — opaque white (255,255,255), which is
// what a fresh WorldEdit region carries and what renders visibly in the
// overlay. Values are clamped to 0..255. Exported so the Wails App.CreateRegion
// binding shares the exact same defaulting/clamping as the MCP handler.
func ParseRegionColor(c []int) ([3]uint8, error) {
	if len(c) == 0 {
		return [3]uint8{255, 255, 255}, nil
	}
	if len(c) != 3 {
		return [3]uint8{}, fmt.Errorf("color must be a [r,g,b] array of 3 values, got %d", len(c))
	}
	var out [3]uint8
	for i, v := range c {
		if v < 0 {
			v = 0
		}
		if v > 255 {
			v = 255
		}
		out[i] = uint8(v)
	}
	return out, nil
}

type regionsMoveParams struct {
	CreationNumber int32   `json:"creation_number"`
	DX             float32 `json:"dx"`
	DY             float32 `json:"dy"`
}

func handleRegionsMove(params json.RawMessage) (any, error) {
	var p regionsMoveParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.MoveRegion(p.CreationNumber, p.DX, p.DY); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

type regionsResizeParams struct {
	CreationNumber int32   `json:"creation_number"`
	MinX           float32 `json:"min_x"`
	MinY           float32 `json:"min_y"`
	MaxX           float32 `json:"max_x"`
	MaxY           float32 `json:"max_y"`
}

func handleRegionsResize(params json.RawMessage) (any, error) {
	var p regionsResizeParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.ResizeRegion(p.CreationNumber, p.MinX, p.MinY, p.MaxX, p.MaxY); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

type regionsRenameParams struct {
	CreationNumber int32  `json:"creation_number"`
	Name           string `json:"name"`
}

func handleRegionsRename(params json.RawMessage) (any, error) {
	var p regionsRenameParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.Name == "" {
		return nil, fmt.Errorf("name is required")
	}
	if err := Current.RenameRegion(p.CreationNumber, p.Name); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

type regionsDeleteParams struct {
	CreationNumber int32 `json:"creation_number"`
}

func handleRegionsDelete(params json.RawMessage) (any, error) {
	var p regionsDeleteParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.DeleteRegion(p.CreationNumber); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}
