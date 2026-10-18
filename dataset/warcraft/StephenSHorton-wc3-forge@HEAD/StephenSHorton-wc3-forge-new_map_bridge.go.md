---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "new_map_bridge.go"
ref: HEAD
---

package main

import (
	"encoding/json"
	"fmt"
	"os"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

// new_map_bridge.go registers the map.new MCP handler at the package-main layer.
// Like minimap.bake (see minimap_bridge.go), map creation can't live in
// internal/forge because writeNewMap depends on top-level helpers — the CASC
// tileset palette (tilesetPalette → loadTerrainSLK/loadCliffTypesSLK) and the
// minimap bake (BakeMinimapBLP) — so the handler is wired onto the bridge from
// main() right after forge.RegisterAll.
//
// This closes the "an agent can't BEGIN a map" gap: CreateNewMap/WriteNewMap were
// GUI-only Wails bindings. map.new gives an agent the same synthesize-and-open
// flow with an explicit destination path (no save dialog).
//
//	map.new {name, width, height, tileset, path} -> {ok, path, name, width, height}

// registerMapNewBridge wires map.new onto the bridge. Called from main() after
// forge.RegisterAll, alongside registerMinimapBridge.
func registerMapNewBridge(b *bridge.Bridge) {
	b.Register("map.new", handleMapNew)
}

func handleMapNew(params json.RawMessage) (any, error) {
	var p NewMapParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	// Refuse to overwrite an existing file — the agent supplies the path
	// directly (no save-dialog confirmation), so a clobber would be silent.
	if p.Path != "" {
		if _, err := os.Stat(p.Path); err == nil {
			return nil, fmt.Errorf("map.new: path already exists: %s", p.Path)
		} else if !os.IsNotExist(err) {
			return nil, fmt.Errorf("map.new: %w", err)
		}
	}
	// writeNewMap validates path/dimensions and synthesizes the .w3x.
	if err := writeNewMap(p); err != nil {
		return nil, err
	}
	if err := forge.Current.Open(p.Path); err != nil {
		return nil, fmt.Errorf("created map but failed to open it: %w", err)
	}
	return map[string]any{
		"ok":     true,
		"path":   p.Path,
		"name":   p.Name,
		"width":  p.Width,
		"height": p.Height,
	}, nil
}
