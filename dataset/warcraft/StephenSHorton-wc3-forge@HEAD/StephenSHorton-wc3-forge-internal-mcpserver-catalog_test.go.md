---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mcpserver/catalog_test.go"
ref: HEAD
---

package mcpserver

import (
	"encoding/json"
	"testing"
)

// TestCatalogIntegrity guards the generated tools.json: it must parse, every
// tool must have a unique name and an object input schema, and exactly the two
// known local tools (sessions_list, session_select) may lack a bridge method.
func TestCatalogIntegrity(t *testing.T) {
	entries, err := loadCatalog()
	if err != nil {
		t.Fatalf("loadCatalog: %v", err)
	}
	if len(entries) < 100 {
		t.Fatalf("expected >=100 tools in catalog, got %d (regenerate with `npm run gen:tools`)", len(entries))
	}

	seen := map[string]bool{}
	local := map[string]bool{}
	for _, e := range entries {
		if e.Name == "" {
			t.Errorf("catalog has a tool with an empty name")
			continue
		}
		if seen[e.Name] {
			t.Errorf("duplicate tool name %q", e.Name)
		}
		seen[e.Name] = true

		var schema map[string]any
		if err := json.Unmarshal(e.InputSchema, &schema); err != nil {
			t.Errorf("tool %q inputSchema is not a JSON object: %v", e.Name, err)
		} else if schema["type"] != "object" {
			t.Errorf("tool %q inputSchema type=%v, want \"object\"", e.Name, schema["type"])
		}

		if e.Method == "" {
			local[e.Name] = true
		}
	}

	wantLocal := map[string]bool{"sessions_list": true, "session_select": true}
	if len(local) != len(wantLocal) {
		t.Errorf("expected exactly %d local tools, got %d: %v", len(wantLocal), len(local), keys(local))
	}
	for name := range wantLocal {
		if !local[name] {
			t.Errorf("expected local tool %q to be present with no bridge method", name)
		}
	}
	for name := range local {
		if !wantLocal[name] {
			t.Errorf("tool %q has no bridge method but is not a known local tool", name)
		}
	}
}

func keys(m map[string]bool) []string {
	out := make([]string, 0, len(m))
	for k := range m {
		out = append(out, k)
	}
	return out
}
