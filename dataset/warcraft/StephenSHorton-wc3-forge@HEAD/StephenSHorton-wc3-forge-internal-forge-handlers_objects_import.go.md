---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_objects_import.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"errors"
	"fmt"
	"strings"
)

type objectsImportFromMapParams struct {
	SourceMapPath string          `json:"source_map_path"`
	Objects       []ImportRequest `json:"objects"`
	OnCollision   string          `json:"on_collision"` // "skip" (default) | "remap"
}

// handleObjectsImportFromMap copies objects (+ their dependencies) from a source
// map into the loaded map. See Session.ImportObjectsFromMap.
func handleObjectsImportFromMap(params json.RawMessage) (any, error) {
	var p objectsImportFromMapParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if strings.TrimSpace(p.SourceMapPath) == "" {
		return nil, errors.New("source_map_path is required")
	}
	if len(p.Objects) == 0 {
		return nil, errors.New("objects is required (a list of {kind, id})")
	}
	for i, o := range p.Objects {
		if len(strings.TrimSpace(o.ID)) == 0 || strings.TrimSpace(o.Kind) == "" {
			return nil, fmt.Errorf("objects[%d]: kind and id are required", i)
		}
	}
	if p.OnCollision != "" && p.OnCollision != "skip" && p.OnCollision != "remap" {
		return nil, errors.New("on_collision must be 'skip' or 'remap'")
	}
	return Current.ImportObjectsFromMap(p.SourceMapPath, p.Objects, p.OnCollision)
}
