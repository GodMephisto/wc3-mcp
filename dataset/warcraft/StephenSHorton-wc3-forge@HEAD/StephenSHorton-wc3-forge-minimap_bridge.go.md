---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "minimap_bridge.go"
ref: HEAD
---

package main

import (
	"encoding/base64"
	"encoding/json"
	"fmt"
	"os"
	"path/filepath"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

// minimap_bridge.go registers the minimap.bake MCP handler at the App/main
// layer. Unlike the gameplay/swap-tileset handlers (which live in
// internal/forge), the minimap bake function depends on top-level CASC-color
// helpers (tileColor / CliffToGroundTexture / WaterColors / tilesetFallbackColor
// in package main), so the handler must live here and is wired onto the bridge
// from main() right after forge.RegisterAll.
//
// This closes the last "GUI-only edit" gap: App.GenerateMinimapBytes baked a
// terrain minimap only for the in-app panel. minimap.bake gives an agent the
// same render — returned as base64 PNG bytes AND dumped to a temp PNG file
// (path returned) so the agent can OPEN it and self-verify the result visually,
// which the project's verification methodology requires. Optionally persists
// the baked image as war3mapMap.blp into the loaded map (the editor's own
// on-disk minimap format), marking the session dirty so Save writes it.
//
//   minimap.bake {persist?:bool} ->
//     {ok, ext:"png", bytes_base64, width, height, png_path, persisted, blp_path?}

// registerMinimapBridge wires minimap.bake onto the bridge. Called from main()
// after forge.RegisterAll (which owns the rest of the catalog).
func registerMinimapBridge(b *bridge.Bridge) {
	b.Register("minimap.bake", handleMinimapBake)
}

type minimapBakeParams struct {
	// Persist, when true, also writes the baked image as war3mapMap.blp into the
	// loaded map (via the Import Manager mutator, so it lands in the archive +
	// marks the session dirty for the next Save). The on-disk WC3 minimap is BLP,
	// so persistence uses the BLP encoder; the returned preview bytes are PNG.
	Persist bool `json:"persist"`
}

func handleMinimapBake(params json.RawMessage) (any, error) {
	var p minimapBakeParams
	if len(params) > 0 {
		if err := json.Unmarshal(params, &p); err != nil {
			return nil, fmt.Errorf("invalid params: %w", err)
		}
	}
	t := forge.Current.Terrain()
	if t == nil {
		return nil, fmt.Errorf("no map loaded")
	}
	pngBytes, err := BakeMinimapPNG(t)
	if err != nil {
		return nil, fmt.Errorf("bake minimap: %w", err)
	}

	// Dump the PNG to a temp file so the agent can open it and verify visually
	// (the bytes are also returned base64 for programmatic use).
	pngPath := filepath.Join(os.TempDir(), fmt.Sprintf("wc3-forge-minimap-%d.png", os.Getpid()))
	if werr := os.WriteFile(pngPath, pngBytes, 0o644); werr != nil {
		// Non-fatal: the base64 bytes are still returned; just note the dump
		// failed by leaving png_path empty.
		pngPath = ""
	}

	resp := map[string]any{
		"ok":           true,
		"ext":          "png",
		"bytes_base64": base64.StdEncoding.EncodeToString(pngBytes),
		"width":        int(t.Width) - 1,
		"height":       int(t.Height) - 1,
		"png_path":     pngPath,
		"persisted":    false,
	}

	if p.Persist {
		blpBytes, berr := BakeMinimapBLP(t)
		if berr != nil {
			return nil, fmt.Errorf("bake minimap blp: %w", berr)
		}
		// AddImportFile writes war3mapMap.blp into the archive root and marks the
		// session dirty (the same mutator the Import Manager uses). The minimap is
		// resolved from the archive root, so this overrides any prior baked image
		// on the next Save.
		written, aerr := forge.Current.AddImportFile("war3mapMap.blp", blpBytes)
		if aerr != nil {
			return nil, fmt.Errorf("persist war3mapMap.blp: %w", aerr)
		}
		resp["persisted"] = true
		resp["blp_path"] = written
	}

	return resp, nil
}
